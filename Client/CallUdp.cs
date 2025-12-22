using Common;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NT106_BT2
{
    /// <summary>
    /// ============================================================================
    /// CallUdp.cs - Quản lý truyền video/audio cho cuộc gọi qua UDP
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Khởi động UDP client để truyền media (video/audio)
    /// Gửi khung video (JPEG) dưới dạng các chunk
    /// Nhận khung video từ các user khác và lắp ráp lại
    /// Nhận dữ liệu audio (PCM raw)
    /// Xử lý reassembly (ghép các chunk thành frame hoàn chỉnh)
    /// 
    /// CÔNG VIỆC CỤ THể:
    /// 1. EnsureStarted()
    ///    - Khởi động UDP client nếu chưa có
    ///    - OS tự chọn port local (port = 0)
    ///    - Bắt đầu ReceiveLoop() để lắng nghe gói UDP
    /// 
    /// 2. SetIds(int roomId, int userId)
    ///    - Lưu roomId và userId của user hiện tại
    ///    - Dùng để định danh gói tin khi gửi
    /// 
    /// 3. SendFrameChunkAsync(frameId, chunkIndex, chunkCount, payload, payloadLen)
    ///    - Gửi một chunk của frame video
    ///    - Sử dụng MediaPacket.Pack() để đóng gói
    ///    - Gửi tới server (relay) qua UDP
    /// 
    /// 4. ReceiveLoop(CancellationToken token)
    ///    - Chạy async trong background
    ///    - Liên tục nhận gói UDP từ server
    ///    - Unpack bằng MediaPacket.TryUnpack()
    ///    - Nếu là audio (chunkCount == 0): gọi OnAudioReceived
    ///    - Nếu là video: sử dụng FrameAssembler để ghép chunks
    ///    - Khi đủ chunks: gọi OnFrameReceived với frame hoàn chỉnh
    /// 
    /// 5. Stop()
    ///    - Dừng ReceiveLoop (hủy CancellationToken)
    ///    - Đóng UDP client
    ///    - Reset RoomId, UserId, LocalPort
    /// 
    /// EVENTS:
    /// - OnFrameReceived(int fromUserId, byte[] jpegData)
    ///   → Kích hoạt khi nhận xong 1 frame video
    /// - OnAudioReceived(int fromUserId, byte[] pcmData)
    ///   → Kích hoạt khi nhận dữ liệu audio
    /// 
    /// BIẾN TOÀN CỤC:
    /// - udp: UdpClient instance
    /// - cts: CancellationTokenSource để dừng receive loop
    /// - assembler: FrameAssembler để ghép chunks thành frames
    /// - LocalPort, RoomId, UserId: Thông tin định danh
    /// 
    /// GHI CHÚ:
    /// - Sử dụng MediaPacket format chuẩn (16 bytes header)
    /// - FrameAssembler có timeout 3s để dọn các frame unfinished
    /// - Audio không cần ghép, gửi trực tiếp dưới dạng PCM
    /// 
    /// ============================================================================
    /// </summary>
    internal static class CallUdp
    {
        private static UdpClient udp;
        private static CancellationTokenSource cts;

        private static readonly FrameAssembler assembler = new FrameAssembler();

        public static int LocalPort { get; private set; }
        public static int RoomId { get; private set; }
        public static int UserId { get; private set; }

        private static readonly string Host = ConfigurationManager.AppSettings["ServerHost"] ?? "127.0.0.1";
        private static readonly int UdpPort = int.TryParse(ConfigurationManager.AppSettings["UdpPort"], out int p) ? p : 9001;
        private static IPEndPoint ServerEndPoint => new IPEndPoint(IPAddress.Parse(Host), UdpPort);

        // byte jpeg chia sẻ khung
        public static event Action<int /*fromUserId*/, byte[] /*jpeg*/> OnFrameReceived;
        public static event Action<int /*fromUserId*/, byte[] /*pcm*/> OnAudioReceived;

        public static void EnsureStarted()
        {
            if (udp != null) return;

            udp = new UdpClient(0); // OS tự chọn port
            LocalPort = ((IPEndPoint)udp.Client.LocalEndPoint).Port;

            cts = new CancellationTokenSource();
            _ = ReceiveLoop(cts.Token);
        }

        public static void SetIds(int roomId, int userId)
        {
            RoomId = roomId;
            UserId = userId;
        }

        public static async Task SendFrameChunkAsync(int frameId, ushort chunkIndex, ushort chunkCount, byte[] payload, int payloadLen)
        {
            if (udp == null) return;
            if (RoomId <= 0 || UserId <= 0) return;

            var pkt = MediaPacket.Pack(RoomId, UserId, frameId, chunkIndex, chunkCount, payload, payloadLen);
            try { await udp.SendAsync(pkt, pkt.Length, ServerEndPoint); } catch { }
        }

        public static void Stop()
        {
            try { cts?.Cancel(); } catch { }
            try { udp?.Close(); } catch { }

            udp = null;
            cts = null;
            LocalPort = 0;
            RoomId = 0;
            UserId = 0;
        }

        private static async Task ReceiveLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && udp != null)
            {
                UdpReceiveResult r;
                try { r = await udp.ReceiveAsync(); }
                catch { break; }

                var data = r.Buffer;
                int len = data?.Length ?? 0;
                if (len < MediaPacket.HeaderSize) continue;

                if (!MediaPacket.TryUnpack(data, len, out int roomId, out int fromUserId, out int frameId,
                    out ushort chunkIndex, out ushort chunkCount, out byte[] payload, out int payloadLen))
                {
                    continue;
                }

                if (RoomId > 0 && roomId != RoomId) continue;

                // Gói âm thanh: chunkCount == 0 -> byte PCM thô, không tái cấu trúc.
                if (chunkCount == 0)
                {
                    var pcm = payload;
                    if (payloadLen > 0 && (payload == null || payload.Length != payloadLen))
                    {
                        pcm = new byte[payloadLen];
                        if (payload != null)
                            Buffer.BlockCopy(payload, 0, pcm, 0, Math.Min(payloadLen, payload.Length));
                    }

                    try { OnAudioReceived?.Invoke(fromUserId, pcm ?? Array.Empty<byte>()); } catch { }
                    continue;
                }

                var full = assembler.Push(roomId, fromUserId, frameId, chunkIndex, chunkCount, payload);
                if (full != null && full.Length > 0)
                {
                    try { OnFrameReceived?.Invoke(fromUserId, full); } catch { }
                }
            }
        }
    }
}
