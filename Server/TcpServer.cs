using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal sealed class TcpServer
    {
        #region Nested types

        private sealed class ClientInfo
        {
            public TcpClient Client;
            public StreamWriter Writer;
            public string Email;
            public string FullName;
        }

        private sealed class Envelope
        {
            public string type { get; set; }
        }

        #endregion

        #region Fields

        private readonly Action<string, string> log;
        private TcpListener lis;

        private static readonly List<ClientInfo> clients = new List<ClientInfo>();
        private static readonly object clientsLock = new object();

        #endregion

        #region Ctor & logging

        public TcpServer() : this(null) { }

        public TcpServer(Action<string, string> logger)
        {
            log = logger;
        }

        private void Log(string src, string msg)
        {
            log?.Invoke(src, msg);
        }

        #endregion

        #region Start / Stop

        public void Start(int port)
        {
            lis = new TcpListener(IPAddress.Any, port);
            lis.Start();
            Log("Server", $"Listening on 0.0.0.0:{port}");
            _ = AcceptLoop();
        }

        public void Stop()
        {
            try
            {
                lis?.Stop();
                Log("Server", "Stopped");
            }
            catch { }
        }

        #endregion

        #region Accept & Handle

        private async Task AcceptLoop()
        {
            while (true)
            {
                TcpClient cli;
                try
                {
                    cli = await lis.AcceptTcpClientAsync();
                }
                catch
                {
                    break;
                }

                var ep = cli.Client.RemoteEndPoint != null
                    ? cli.Client.RemoteEndPoint.ToString()
                    : "client";

                Log("Accept", ep);
                _ = Task.Run(() => Handle(cli, ep));
            }
        }

        private async Task Handle(TcpClient cli, string ep)
        {
            ClientInfo myInfo = null;

            try
            {
                var ns = cli.GetStream();
                var rd = new StreamReader(ns, new UTF8Encoding(false));
                var wr = new StreamWriter(ns, new UTF8Encoding(false)) { AutoFlush = true };

                while (cli.Connected)
                {
                    string line = await rd.ReadLineAsync();
                    if (line == null) break;

                    Log(ep, "recv: " + line);

                    string type = null;
                    try
                    {
                        var env = JsonConvert.DeserializeObject<Envelope>(line);
                        type = env?.type;
                    }
                    catch
                    {
                        await SendErr(wr, "JSON không hợp lệ");
                        Log(ep, "send: ERROR json");
                        continue;
                    }

                    #region LOGIN

                    if (type == MsgType.LOGIN)
                    {
                        LoginReq login = null;
                        try
                        {
                            login = JsonConvert.DeserializeObject<LoginReq>(line);
                        }
                        catch
                        {
                            await SendErr(wr, "LOGIN: dữ liệu không hợp lệ");
                            Log(ep, "send: ERROR login");
                            continue;
                        }

                        var r = Db.FindByLogin(login.username, login.passwordHash);
                        if (r.HasValue)
                        {
                            var v = r.Value;
                            var user = new UserDto
                            {
                                username = login.username,
                                fullName = (v.Firstname + " " + v.Surname).Trim(),
                                email = v.Email,
                                birthday = v.Birthday.HasValue ? v.Birthday.Value.ToString("yyyy-MM-dd") : null,
                                gender = v.Gender,
                                role = v.Role
                            };

                            var issued = TokenManager.Issue(login.username);
                            await SendOk(wr, MsgType.LOGIN, "OK", user, issued.token, issued.exp);
                            Log(ep, "send: OK login (token)");

                            if (myInfo == null)
                            {
                                myInfo = new ClientInfo
                                {
                                    Client = cli,
                                    Writer = wr,
                                    Email = login.username,
                                    FullName = user.fullName
                                };
                                lock (clientsLock)
                                {
                                    clients.Add(myInfo);
                                }
                            }
                            else
                            {
                                myInfo.Email = login.username;
                                myInfo.FullName = user.fullName;
                            }
                        }
                        else
                        {
                            await SendErr(wr, "Sai email/mật khẩu");
                            Log(ep, "send: ERROR wrong creds");
                        }

                        continue;
                    }

                    #endregion

                    #region REGISTER

                    if (type == MsgType.REGISTER)
                    {
                        RegisterReq reg = null;
                        try
                        {
                            reg = JsonConvert.DeserializeObject<RegisterReq>(line);
                        }
                        catch
                        {
                            await SendErr(wr, "REGISTER: dữ liệu không hợp lệ");
                            Log(ep, "send: ERROR register json");
                            continue;
                        }

                        if (Db.UsernameExists(reg.email))
                        {
                            await SendErr(wr, "Email đã tồn tại");
                            Log(ep, "send: ERROR email exists");
                        }
                        else
                        {
                            DateTime? bd = null;
                            if (!string.IsNullOrWhiteSpace(reg.birthday) &&
                                DateTime.TryParse(reg.birthday, out var d))
                            {
                                bd = d;
                            }

                            string fn = "", sn = "";
                            if (!string.IsNullOrWhiteSpace(reg.fullName))
                            {
                                var parts = reg.fullName.Trim()
                                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                fn = parts.FirstOrDefault() ?? "";
                                sn = parts.Length > 1 ? parts[parts.Length - 1] : fn;
                            }

                            Db.InsertUser(fn, sn, bd, reg.gender, reg.email, reg.passwordHash);
                            await SendOk(wr, MsgType.REGISTER, "Registered", null);
                            Log(ep, "send: OK register");
                        }

                        continue;
                    }

                    #endregion

                    #region LOGOUT

                    if (type == MsgType.LOGOUT)
                    {
                        LogoutReq lo = null;
                        try
                        {
                            lo = JsonConvert.DeserializeObject<LogoutReq>(line);
                        }
                        catch
                        {
                            await SendErr(wr, "LOGOUT: dữ liệu không hợp lệ");
                            Log(ep, "send: ERROR logout json");
                            continue;
                        }

                        TokenManager.Invalidate(lo.username, lo.token);
                        await SendOk(wr, MsgType.LOGOUT, "Logged out", null);
                        Log(ep, "send: OK logout");
                        continue;
                    }

                    #endregion

                    #region LOGIN_WITH_TOKEN

                    if (type == MsgType.LOGIN_WITH_TOKEN)
                    {
                        TokenLoginReq treq = null;
                        try
                        {
                            treq = JsonConvert.DeserializeObject<TokenLoginReq>(line);
                        }
                        catch
                        {
                            await SendErr(wr, "TOKEN LOGIN: dữ liệu không hợp lệ");
                            Log(ep, "send: ERROR token login json");
                            continue;
                        }

                        if (TokenManager.Validate(treq.username, treq.token))
                        {
                            var r = Db.GetByEmail(treq.username);
                            if (r.HasValue)
                            {
                                var v = r.Value;
                                var user = new UserDto
                                {
                                    username = treq.username,
                                    fullName = (v.Firstname + " " + v.Surname).Trim(),
                                    email = v.Email,
                                    birthday = v.Birthday.HasValue ? v.Birthday.Value.ToString("yyyy-MM-dd") : null,
                                    gender = v.Gender
                                };

                                var issued = TokenManager.Issue(treq.username);
                                await SendOk(wr, MsgType.LOGIN, "OK", user, issued.token, issued.exp);
                                Log(ep, "send: OK token login");

                                if (myInfo == null)
                                {
                                    myInfo = new ClientInfo
                                    {
                                        Client = cli,
                                        Writer = wr,
                                        Email = treq.username,
                                        FullName = user.fullName
                                    };
                                    lock (clientsLock)
                                    {
                                        clients.Add(myInfo);
                                    }
                                }
                                else
                                {
                                    myInfo.Email = treq.username;
                                    myInfo.FullName = user.fullName;
                                }
                            }
                            else
                            {
                                await SendErr(wr, "Không tìm thấy người dùng");
                            }
                        }
                        else
                        {
                            await SendErr(wr, "Token không hợp lệ hoặc đã hết hạn");
                        }

                        continue;
                    }

                    #endregion

                    #region GROUP_CHAT_HISTORY_REQ  (NEW)

                    if (type == MsgType.GROUP_CHAT_HISTORY_REQ)
                    {
                        GroupChatHistoryReq hreq = null;
                        try
                        {
                            hreq = JsonConvert.DeserializeObject<GroupChatHistoryReq>(line);
                        }
                        catch
                        {
                            await SendErr(wr, "GROUP_CHAT_HISTORY_REQ: dữ liệu không hợp lệ");
                            continue;
                        }

                        if (myInfo == null)
                        {
                            await SendErr(wr, "Bạn cần đăng nhập trước");
                            continue;
                        }

                        var msgs = Db.GetGroupMessages(hreq.roomCode, hreq.take);

                        var hres = new GroupChatHistoryRes
                        {
                            roomCode = hreq.roomCode,
                            messages = msgs
                        };

                        string outJson = JsonConvert.SerializeObject(hres);
                        await wr.WriteLineAsync(outJson);
                        await wr.FlushAsync();

                        Log("HISTORY", $"Sent {msgs.Count} messages for room {hreq.roomCode} to {myInfo.Email}");
                        continue;
                    }

                    #endregion

                    #region GROUP_CHAT

                    if (type == MsgType.GROUP_CHAT)
                    {
                        GroupChatMsg chat = null;
                        try
                        {
                            chat = JsonConvert.DeserializeObject<GroupChatMsg>(line);
                        }
                        catch
                        {
                            await SendErr(wr, "GROUP_CHAT: dữ liệu không hợp lệ");
                            Log(ep, "send: ERROR group_chat json");
                            continue;
                        }

                        if (myInfo == null)
                        {
                            await SendErr(wr, "Bạn cần đăng nhập trước khi chat");
                            Log(ep, "send: ERROR not logged in");
                            continue;
                        }

                        // ===== NEW: LƯU DB TRƯỚC KHI BROADCAST =====
                        try
                        {
                            Db.InsertGroupMessage(chat.roomCode, chat.fromEmail, chat.fromName, chat.message);
                            Log("GROUP_CHAT", $"Saved to DB room={chat.roomCode}");
                        }
                        catch (Exception dbEx)
                        {
                            Log("GROUP_CHAT", $"DB save FAILED: {dbEx.Message}");
                        }

                        string json = JsonConvert.SerializeObject(chat);

                        List<ClientInfo> snapshot;
                        lock (clientsLock)
                        {
                            snapshot = clients.ToList();
                        }

                        Log("GROUP_CHAT", $"Broadcasting to {snapshot.Count} clients in room {chat.roomCode}");

                        int sentCount = 0;
                        foreach (var c in snapshot.ToList())
                        {
                            try
                            {
                                Log("GROUP_CHAT",
                                    $"Sending to client: {c.Email ?? "null"}, Connected: {c.Client?.Connected ?? false}");

                                await c.Writer.WriteLineAsync(json);
                                await c.Writer.FlushAsync();

                                sentCount++;
                                Log("GROUP_CHAT", $"✓ Successfully sent to {c.Email ?? "null"}");
                            }
                            catch (Exception ex)
                            {
                                Log("GROUP_CHAT",
                                    $"FAILED to send to {c.Email ?? "null"}: {ex.Message}\n{ex.StackTrace}");

                                try { c.Client?.Close(); } catch { }

                                lock (clientsLock)
                                {
                                    clients.Remove(c);
                                }
                            }
                        }

                        Log(ep,
                            $"broadcast: {chat.roomCode} - {chat.fromName}: {chat.message} (sent to {sentCount} clients)");
                        continue;
                    }

                    #endregion

                    await SendErr(wr, "Yêu cầu không hợp lệ");
                    Log(ep, "send: ERROR unknown type");
                }

                Log(ep, "disconnected");

                if (myInfo != null)
                {
                    lock (clientsLock)
                    {
                        clients.Remove(myInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ep, "error: " + ex.Message);
            }
        }

        #endregion

        #region Send OK / ERROR helpers

        private static async Task SendOk(StreamWriter wr, string type, string message, UserDto user, string token, DateTime exp)
        {
            var res = new OkRes
            {
                ok = true,
                type = type,
                message = message,
                user = user,
                token = token,
                expires = exp.ToString("o")
            };

            string json = JsonConvert.SerializeObject(res);
            await wr.WriteLineAsync(json);
            await wr.FlushAsync();
        }

        private static async Task SendOk(StreamWriter wr, string type, string message, UserDto user)
        {
            var res = new OkRes
            {
                ok = true,
                type = type,
                message = message,
                user = user
            };

            string json = JsonConvert.SerializeObject(res);
            await wr.WriteLineAsync(json);
            await wr.FlushAsync();
        }

        private static async Task SendErr(StreamWriter wr, string error)
        {
            var res = new ErrRes
            {
                ok = false,
                type = "ERROR",
                error = error
            };

            string json = JsonConvert.SerializeObject(res);
            await wr.WriteLineAsync(json);
            await wr.FlushAsync();
        }

        #endregion
    }
}
