using Common;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NT106_BT2
{
    /// <summary>
    /// ============================================================================
    /// TcpHelper.cs - Quản lý kết nối TCP giữa Client và Server
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Kết nối/ngắt kết nối TCP với server
    /// Gửi tin nhắn đến server (text, group chat, cuộc gọi, v.v)
    /// Nhận tin nhắn từ server (real-time listening)
    /// Xử lý reconnect tự động
    /// Quản lý stream (read/write) với Unicode encoding
    /// 
    /// CÔNG VIỆC CỤ THỂ:
    /// 1. ConnectAsync()
    ///    - Kết nối tới server (lấy từ config: ServerHost:ServerPort)
    ///    - Khởi tạo NetworkStream và StreamReader/Writer
    ///    - Bắt đầu lắng nghe tin nhắn từ server (ListenLoop)
    /// 
    /// 2. ListenLoop()
    ///    - Chạy trong background thread (async)
    ///    - Liên tục đọc tin nhắn từ server (ReadLineAsync)
    ///    - Gọi event OnMessageReceived để các handler xử lý
    ///    - Xử lý lỗi kết nối bị mất
    /// 
    /// 3. SendLineAsync(string line)
    ///    - Gửi tin nhắn dạng JSON tới server
    ///    - Kiểm tra kết nối, tự động reconnect nếu cần
    ///    - FlushAsync() để đảm bảo dữ liệu được gửi ngay
    /// 
    /// 4. SendGroupChatAsync(roomCode, message, fromEmail, fromName)
    ///    - Gửi tin nhắn nhóm (GROUP_CHAT)
    ///    - Serialize thành JSON và gửi
    /// 
    /// 5. SendCallJoinAsync(roomCode)
    ///    - Gửi yêu cầu tham gia cuộc gọi (CALL_JOIN)
    ///    - Kèm theo UDP port của client để server biết
    /// 
    /// 6. SendCallLeaveAsync(roomCode) / SendCallShareAsync(roomCode, sharerName)
    ///    - Xử lý các sự kiện liên quan cuộc gọi
    /// 
    /// 7. LogoutAsync(username, token)
    ///    - Gửi yêu cầu đăng xuất tới server
    /// 
    /// 8. Disconnect()
    ///    - Đóng tất cả stream và socket
    ///    - Hủy cancel token
    /// 
    /// EVENTS:
    /// - OnMessageReceived: Kích hoạt khi nhận tin từ server
    /// - OnError: Kích hoạt khi có lỗi (mất kết nối, gửi lỗi, etc)
    /// 
    /// BIẾN TOÀN CỤC:
    /// - cli: TcpClient instance
    /// - ns: NetworkStream để gửi/nhận dữ liệu
    /// - rd/wr: StreamReader/Writer để đọc/ghi dữ liệu
    /// - listening: Flag để biết loop có đang chạy không
    /// - cts: CancellationTokenSource để dừng background thread
    /// 
    /// ============================================================================
    /// </summary>
    internal static class TcpHelper
    {
        private static TcpClient cli;
        private static NetworkStream ns;
        private static StreamReader rd;
        private static StreamWriter wr;

        private static CancellationTokenSource cts;
        private static bool listening = false;

        private static readonly string Host = ConfigurationManager.AppSettings["ServerHost"] ?? "127.0.0.1";

        private static readonly int Port = int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out int p) ? p : 8080;

        public static event Action<string> OnMessageReceived;
        public static event Action<string> OnError;

        public static bool IsConnected => cli?.Connected ?? false;

        #region CONNECT
        public static async Task ConnectAsync()
        {
            try
            {
                if (cli != null && cli.Connected && listening)
                {
                    System.Diagnostics.Debug.WriteLine("[TCP] Already connected & listening");
                    return;
                }

                if (cli != null && cli.Connected && !listening)
                {
                    System.Diagnostics.Debug.WriteLine("[TCP] Connected nhưng loop chưa chạy → start loop");
                    InitStreams();
                    StartListen();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[TCP] Connecting to {Host}:{Port}");
                cli = new TcpClient();
                await cli.ConnectAsync(Host, Port);

                InitStreams();
                StartListen();
            }
            catch (Exception ex)
            {
                OnError?.Invoke("Cannot connect: " + ex.Message);
            }
        }

        private static void InitStreams()
        {
            ns = cli.GetStream();
            rd = new StreamReader(ns, new UTF8Encoding(false));
            wr = new StreamWriter(ns, new UTF8Encoding(false)) { AutoFlush = true };
        }
        #endregion

        #region LISTEN LOOP
        private static void StartListen()
        {
            if (listening) return;
            listening = true;

            cts = new CancellationTokenSource();
            _ = ListenLoop(cts.Token);
        }

        private static async Task ListenLoop(CancellationToken token)
        {
            System.Diagnostics.Debug.WriteLine("[TCP] ListenLoop started");

            try
            {
                while (!token.IsCancellationRequested && cli != null && cli.Connected)
                {
                    string line;
                    try
                    {
                        line = await rd.ReadLineAsync();
                    }
                    catch
                    {
                        OnError?.Invoke("Connection lost.");
                        break;
                    }

                    if (line == null)
                    {
                        OnError?.Invoke("Server closed the connection.");
                        break;
                    }

                    System.Diagnostics.Debug.WriteLine("[TCP] Received: " + line);

                    var handlers = OnMessageReceived?.GetInvocationList();
                    System.Diagnostics.Debug.WriteLine("[TCP] Handlers = " + (handlers?.Length ?? 0));

                    if (handlers != null)
                    {
                        foreach (var h in handlers)
                        {
                            try
                            {
                                ((Action<string>)h)(line);
                            }
                            catch (Exception e)
                            {
                                OnError?.Invoke("Handler error: " + e.Message);
                            }
                        }
                    }
                }
            }
            finally
            {
                listening = false;
                System.Diagnostics.Debug.WriteLine("[TCP] ListenLoop ended");
            }
        }
        #endregion

        #region SEND
        public static async Task SendLineAsync(string line)
        {
            try
            {
                if (!IsConnected)
                    await ConnectAsync();

                await wr.WriteLineAsync(line);
                await wr.FlushAsync();

                System.Diagnostics.Debug.WriteLine("[TCP] Sent: " + line);
            }
            catch (Exception ex)
            {
                OnError?.Invoke("Send failed: " + ex.Message);
            }
        }

        public static Task SendGroupChatAsync(string roomCode, string message, string fromEmail, string fromName)
        {
            var chat = new GroupChatMsg
            {
                type = MsgType.GROUP_CHAT,
                roomCode = roomCode,
                fromEmail = fromEmail,
                fromName = fromName,
                message = message
            };

            return SendLineAsync(JsonConvert.SerializeObject(chat));
        }

        public static Task LogoutAsync(string username, string token)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token))
                return Task.CompletedTask;

            var req = new LogoutReq
            {
                type = MsgType.LOGOUT,
                username = username,
                token = token
            };

            string json = JsonConvert.SerializeObject(req);
            return SendLineAsync(json);
        }
        #endregion

        #region CALL METHODS
        public static Task SendCallJoinAsync(string roomCode)
        {
            if (string.IsNullOrWhiteSpace(roomCode))
                throw new ArgumentException("roomCode is required", nameof(roomCode));

            CallUdp.EnsureStarted();

            var req = new CallJoinReq
            {
                roomCode = roomCode,
                email = Session.Email,
                name = Session.FullName ?? Session.Email,
                udpPort = CallUdp.LocalPort
            };

            string json = JsonConvert.SerializeObject(req);
            return SendLineAsync(json);
        }

        public static Task SendCallShareAsync(string roomCode, string sharerNameOrEmpty)
        {
            var req = new CallShareReq
            {
                roomCode = roomCode,
                sharerName = sharerNameOrEmpty ?? ""
            };
            return SendLineAsync(JsonConvert.SerializeObject(req));
        }

        public static Task SendCallLeaveAsync(string roomCode)
        {
            var req = new CallLeaveReq
            {
                roomCode = roomCode,
                email = Session.Email
            };
            return SendLineAsync(JsonConvert.SerializeObject(req));
        }

        public static Task SendRoomFileAddedAsync(string roomCode, string fileName, string filePath, long fileSizeBytes)
        {
            var msg = new RoomFileAddedMsg
            {
                roomCode = roomCode,
                fileName = fileName,
                filePath = filePath,
                fileSizeBytes = fileSizeBytes,
                uploadedBy = Session.Email
            };

            return SendLineAsync(JsonConvert.SerializeObject(msg));
        }
        #endregion

        #region DISCONNECT
        public static void Disconnect()
        {
            try
            {
                listening = false;
                cts?.Cancel();

                try { rd?.Close(); } catch { }
                try { wr?.Close(); } catch { }
                try { ns?.Close(); } catch { }
                try { cli?.Close(); } catch { }

                cli = null;
                ns = null;
                rd = null;
                wr = null;
            }
            catch (Exception ex)
            {
                OnError?.Invoke("Disconnect error: " + ex.Message);
            }
        }
        #endregion
    }
}
