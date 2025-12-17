using Common;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NT106_BT2
{
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

        // jpeg bytes của frame share
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

                // Audio packet: chunkCount == 0 -> raw PCM bytes, no reassembly
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
