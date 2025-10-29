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

        private static CancellationTokenSource ctsListen;

        private static readonly string Host = ConfigurationManager.AppSettings["ServerHost"] ?? "127.0.0.1";
        private static readonly int Port = int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out int p) ? p : 8080;

        public static event Action<string> OnMessageReceived;  // 🔹 Sự kiện cho chat realtime (JSON đến từ server)
        public static event Action<string> OnError;            // 🔹 Báo lỗi kết nối

        public static bool IsConnected => cli?.Connected ?? false;

        // ========== KẾT NỐI ==========
        public static async Task ConnectAsync()
        {
            try
            {
                if (cli != null && cli.Connected) return;

                cli = new TcpClient();
                await cli.ConnectAsync(Host, Port);

                ns = cli.GetStream();
                rd = new StreamReader(ns, new UTF8Encoding(false));
                wr = new StreamWriter(ns, new UTF8Encoding(false)) { AutoFlush = true };

                Console.WriteLine($"✅ Connected to {Host}:{Port}");

                // 🔹 Bắt đầu đọc dữ liệu liên tục trong background
                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (cli != null && cli.Connected)
                        {
                            var line = await rd.ReadLineAsync();
                            if (line == null) break;
                            OnMessageReceived?.Invoke(line);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke("❌ Connection lost: " + ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                OnError?.Invoke("❌ Cannot connect: " + ex.Message);
                throw;
            }
        }
        public static async Task LogoutAsync(string username, string token)
        {
            if (cli == null || !cli.Connected) return;

            var req = new
            {
                type = Common.MsgType.LOGOUT,
                username = username,
                token = token
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(req);
            await SendLineAsync(json);

            // Sau khi gửi logout thì ngắt kết nối
            Disconnect();
        }


        // ========== GỬI DỮ LIỆU ==========
        public static async Task SendLineAsync(string line)
        {
            if (cli == null || !cli.Connected)
                await ConnectAsync();

            try
            {
                await wr.WriteLineAsync(line);
                await wr.FlushAsync();
            }
            catch (IOException)
            {
                Console.WriteLine("⚠️ Connection lost. Reconnecting...");
                await ConnectAsync();
            }
        }


        // ========== LẮNG NGHE PHẢN HỒI ==========
        private static async Task ListenLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && IsConnected)
                {
                    string line = await rd.ReadLineAsync();
                    if (line == null) break;
                    OnMessageReceived?.Invoke(line); // 🔹 Gửi event lên UI
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke("Listen error: " + ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        // ========== NGẮT KẾT NỐI ==========
        public static void Disconnect()
        {
            try
            {
                ctsListen?.Cancel();
                rd?.Close();
                wr?.Close();
                ns?.Close();
                cli?.Close();
                cli = null;
                ns = null;
                rd = null;
                wr = null;
                Console.WriteLine("🔌 Disconnected");
            }
            catch (Exception ex)
            {
                OnError?.Invoke("Disconnect error: " + ex.Message);
            }
        }
    }
}
