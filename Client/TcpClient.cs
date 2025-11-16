using Common;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT106_BT2
{
    internal static class TcpHelper
    {
        private static TcpClient cli;
        private static NetworkStream ns;
        private static StreamReader rd;
        private static StreamWriter wr;

        private static CancellationTokenSource ctsListen;

        private static readonly string Host =
            ConfigurationManager.AppSettings["ServerHost"] ?? "127.0.0.1";
        private static readonly int Port =
            int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out int p) ? p : 8080;

        public static event Action<string> OnMessageReceived;
        public static event Action<string> OnError;

        public static bool IsConnected => cli?.Connected ?? false;

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

                ctsListen = new CancellationTokenSource();
                var token = ctsListen.Token;

                // VÒNG LẶP LẮNG NGHE
                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (cli != null && cli.Connected)
                        {
                            var line = await rd.ReadLineAsync();
                            if (line == null) break;

                            MessageBox.Show("TcpHelper recv: " + line);   // <= tạm bật

                            OnMessageReceived?.Invoke(line);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke("Connection lost: " + ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                OnError?.Invoke("Cannot connect: " + ex.Message);
                throw;
            }
        }

        public static async Task SendLineAsync(string line)
        {
            if (cli == null || !cli.Connected)
                await ConnectAsync();

            try
            {
                await wr.WriteLineAsync(line);
                await wr.FlushAsync();
            }
            catch (IOException ex)
            {
                OnError?.Invoke("Connection lost while sending: " + ex.Message);
            }
        }

        public static async Task SendGroupChatAsync(string roomCode, string message,
                                                    string fromEmail, string fromName)
        {
            var chat = new Common.GroupChatMsg
            {
                type = Common.MsgType.GROUP_CHAT,
                roomCode = roomCode,
                fromEmail = fromEmail,
                fromName = fromName,
                message = message
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(chat);
            await SendLineAsync(json);
        }

        public static async Task LogoutAsync(string username, string token)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
                return;

            var req = new Common.LogoutReq
            {
                type = Common.MsgType.LOGOUT,
                username = username,
                token = token
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(req);
            await SendLineAsync(json);
        }

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
            }
            catch (Exception ex)
            {
                OnError?.Invoke("Disconnect error: " + ex.Message);
            }
        }
    }

}
