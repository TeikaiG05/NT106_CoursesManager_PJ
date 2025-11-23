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
                // Nếu đã kết nối + đã listen => khỏi tạo thêm
                if (cli != null && cli.Connected && listening)
                {
                    System.Diagnostics.Debug.WriteLine("[TCP] Already connected & listening");
                    return;
                }

                // Nếu TcpClient đã tồn tại nhưng stream chết => tạo lại stream
                if (cli != null && cli.Connected && !listening)
                {
                    System.Diagnostics.Debug.WriteLine("[TCP] Connected nhưng loop chưa chạy → start loop");
                    InitStreams();
                    StartListen();
                    return;
                }

                // Tạo kết nối mới
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

        public static Task SendGroupChatAsync(string roomCode, string message,
                                              string fromEmail, string fromName)
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
