using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Common;

namespace NT106_BT2
{
    public partial class ChatForm : Form
    {
        private readonly string currentUserEmail;
        private readonly string currentUserName;
        private string toEmail;
        private readonly Dictionary<string, Friend> friends = new Dictionary<string, Friend>();
        private readonly Dictionary<string, List<ChatMessage>> chatHistory = new Dictionary<string, List<ChatMessage>>();

        public class Friend
        {
            public string Email { get; set; }
            public string Name { get; set; }
            public string LastMessage { get; set; } = "";
            public DateTime LastTime { get; set; } = DateTime.MinValue;
        }

        public class ChatMessage
        {
            public string Content { get; set; }
            public bool IsFromCurrentUser { get; set; }
            public DateTime Time { get; set; }
        }


        public ChatForm(string email, string name)
        {
            currentUserEmail = email;
            currentUserName = name;
            InitializeComponent();
            TcpHelper.OnMessageReceived += TcpHelper_OnMessageReceived;
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            luongChatItems.Controls.Clear();
            luongChatItems.Controls.Add(labelChatTitle);

            tieuDeChat.Text = "";
            // ví dụ tạm trong ChatForm_Load
            TcpHelper.OnMessageReceived += (line) => { System.Diagnostics.Debug.WriteLine("[ALL MSG] " + line); };

            await TcpHelper.ConnectAsync();
            oTimKiem.Text = "Search by email";
            labelChatTitle.Text = "Chat";
            ReloadFriendBanners();
        }

        private void oTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            string email = oTimKiem.Text.Trim();
            if (string.IsNullOrEmpty(email) || email == "Search by email") return;
            if (email.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Không thể chat với chính mình.");
                return;
            }

            toEmail = email;
            string friendName = email.Split('@')[0];

            if (!friends.ContainsKey(email))
            {
                friends[email] = new Friend { Email = email, Name = friendName };
            }
            ReloadFriendBanners();
            SelectFriend(email);
        }

        private void SelectFriend(string email)
        {
            toEmail = email;
            Friend f;
            if (friends.TryGetValue(email, out f))
                labelChatTitle.Text = f.Name;
            else
                labelChatTitle.Text = email;
            LoadChatHistory(email);
        }

        private void LoadChatHistory(string email)
        {
            khungTinNhan.Controls.Clear();
            if (chatHistory.ContainsKey(email))
            {
                foreach (var msg in chatHistory[email])
                {
                    if (msg.Content.StartsWith("[FILE]|"))
                    {
                        DisplayFileFromPayload(msg.Content, msg.IsFromCurrentUser);
                    }
                    else if (msg.Content.StartsWith("[EMOJI]"))
                    {
                        string emoji = msg.Content.Substring(7);
                        DisplayEmoji(emoji, msg.IsFromCurrentUser);
                    }
                    else
                    {
                        AddMessageToUI(msg);
                    }
                }
            }
            khungTinNhan.VerticalScroll.Value = khungTinNhan.VerticalScroll.Maximum;
        }

        private async void nutGui_Click(object sender, EventArgs e)
        {
            string content = oNhapTin.Text.Trim();
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(toEmail)) return;

            var pkt = new PrivateChatMsg
            {
                fromEmail = currentUserEmail,
                toEmail = toEmail,
                message = content
            };

            string json = JsonConvert.SerializeObject(pkt);
            Console.WriteLine("SEND: " + json);
            await TcpHelper.SendLineAsync(json);

            var msg = new ChatMessage
            {
                Content = content,
                IsFromCurrentUser = true,
                Time = DateTime.Now
            };

            SaveToHistory(toEmail, msg);
            AddMessageToUI(msg);

            if (friends.ContainsKey(toEmail))
            {
                friends[toEmail].LastMessage = content.Length > 20 ? content.Substring(0, 20) + "..." : content;
                friends[toEmail].LastTime = DateTime.Now;
            }
            ReloadFriendBanners();
            oNhapTin.Clear();
        }

        private void TcpHelper_OnMessageReceived(string line)
        {
            Console.WriteLine("RECV: " + line);
            try
            {
                var root = JObject.Parse(line);
                var type = (string)root["type"];

                if (type == "PRIVATE_CHAT")
                {
                    string to = (string)root["toEmail"];
                    string from = (string)root["fromEmail"];
                    string msgContent = (string)root["message"];

                    if (!string.Equals(to, currentUserEmail, StringComparison.OrdinalIgnoreCase)) return;

                    BeginInvoke(new Action(() =>
                    {
                        string fromEmail = from;

                        if (!friends.ContainsKey(fromEmail))
                        {
                            friends[fromEmail] = new Friend
                            {
                                Email = fromEmail,
                                Name = fromEmail.Split('@')[0],
                                LastTime = DateTime.Now
                            };
                        }

                        if (msgContent.StartsWith("[FILE]|"))
                        {
                            var fileMsg = new ChatMessage
                            {
                                Content = msgContent,
                                IsFromCurrentUser = false,
                                Time = DateTime.Now
                            };
                            SaveToHistory(fromEmail, fileMsg);
                            if (toEmail == fromEmail)
                                DisplayFileFromPayload(msgContent, false);
                        }
                        else if (msgContent.StartsWith("[EMOJI]"))
                        {
                            var emojiMsg = new ChatMessage
                            {
                                Content = msgContent,
                                IsFromCurrentUser = false,
                                Time = DateTime.Now
                            };
                            SaveToHistory(fromEmail, emojiMsg);
                            if (toEmail == fromEmail)
                            {
                                string emoji = msgContent.Substring(7);
                                DisplayEmoji(emoji, false);
                            }
                        }
                        else
                        {
                            var chatMsg = new ChatMessage
                            {
                                Content = msgContent,
                                IsFromCurrentUser = false,
                                Time = DateTime.Now
                            };
                            SaveToHistory(fromEmail, chatMsg);
                            if (toEmail == fromEmail)
                                AddMessageToUI(chatMsg);
                        }

                        string displayMsg = msgContent.StartsWith("[FILE]|") ? "[Hình ảnh]" :
                                            msgContent.StartsWith("[EMOJI]") ? "Emoji" :
                                            (msgContent.Length > 20 ? msgContent.Substring(0, 20) + "..." : msgContent);

                        friends[fromEmail].LastMessage = displayMsg;
                        friends[fromEmail].LastTime = DateTime.Now;

                        ReloadFriendBanners();
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parse error: " + ex.Message);
            }
        }


        private void SaveToHistory(string friendEmail, ChatMessage msg)
        {
            if (!chatHistory.ContainsKey(friendEmail))
                chatHistory[friendEmail] = new List<ChatMessage>();
            chatHistory[friendEmail].Add(msg);
        }

        private void AddMessageToUI(ChatMessage msg)
        {
            var container = new Panel { AutoSize = true, Margin = new Padding(0, 8, 0, 8) };
            var bubble = new Label
            {
                Text = msg.Content,
                AutoSize = true,
                MaximumSize = new Size(320, 0),
                Padding = new Padding(12, 8, 12, 8),
                Font = new Font("Segoe UI", 10f),
                BackColor = msg.IsFromCurrentUser ? Color.FromArgb(0, 120, 215) : Color.FromArgb(245, 248, 250),
                ForeColor = msg.IsFromCurrentUser ? Color.White : Color.Black,
                Margin = msg.IsFromCurrentUser ? new Padding(80, 0, 10, 0) : new Padding(10, 0, 80, 0)
            };
            container.Controls.Add(bubble);
            khungTinNhan.Controls.Add(container);
            khungTinNhan.ScrollControlIntoView(container);
        }

        private void ReloadFriendBanners()
        {
            luongChatItems.Controls.Clear();
            var lblChat = new Label
            {
                Text = "Chat",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 149, 237),
                AutoSize = true,
                Margin = new Padding(20, 20, 0, 10)
            };
            luongChatItems.Controls.Add(lblChat);

            var sep = new Panel
            {
                Height = 1,
                Width = 280,
                BackColor = Color.FromArgb(230, 230, 230),
                Margin = new Padding(20, 0, 0, 15)
            };
            luongChatItems.Controls.Add(sep);

            foreach (var kv in friends.OrderByDescending(x => x.Value.LastTime))
            {
                luongChatItems.Controls.Add(CreateFriendBanner(kv.Value));
            }
        }

        private Control CreateFriendBanner(Friend f)
        {
            var banner = new Panel
            {
                Height = 70,
                Width = luongChatItems.Width - 15,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 1),
                Cursor = Cursors.Hand,
                Tag = f.Email
            };

            var avatar = new PictureBox
            {
                Size = new Size(46, 46),
                Location = new Point(18, 12),
                BackColor = Color.FromArgb(220, 230, 255),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            var lblName = new Label
            {
                Text = f.Name,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = true,
                Location = new Point(75, 16)
            };

            var lblLast = new Label
            {
                Text = string.IsNullOrEmpty(f.LastMessage) ? "No messages yet" : f.LastMessage,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(75, 38),
                MaximumSize = new Size(200, 0)
            };

            banner.Controls.AddRange(new Control[] { avatar, lblName, lblLast });
            banner.Click += (s, e) =>
            {
                string mail = (string)((Panel)s).Tag;
                SelectFriend(mail);
            };
            return banner;
        }

        private async void nutEmoji_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toEmail)) { MessageBox.Show("Chưa chọn người để gửi."); return; }

            string[] emojis = { "😀", "😂", "🤣", "😊", "🙂", "😍", "🥰", "😘", "😜", "😎", "🤩", "🥺", "😢", "😭", "😡", "🤬", "😴", "🤔", "😳", "🙄", "😇", "😈", "👍", "👎", "❤️", "💕", "🔥", "✨", "💯", "🎉" };
            string selected = "";

            using (var frm = new Form { Text = "Chọn emoji", Width = 420, Height = 240, FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent, MaximizeBox = false })
            {
                var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
                foreach (string em in emojis)
                {
                    var btn = new Button { Text = em, Width = 55, Height = 55, Font = new Font("Segoe UI Emoji", 22f), FlatStyle = FlatStyle.Flat, Margin = new Padding(6) };
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Click += (s, ev) => { selected = em; frm.DialogResult = DialogResult.OK; frm.Close(); };
                    panel.Controls.Add(btn);
                }
                frm.Controls.Add(panel);
                if (frm.ShowDialog(this) != DialogResult.OK) return;
            }

            if (string.IsNullOrEmpty(selected)) return;

            var pkt = new PrivateChatMsg
            {
                fromEmail = currentUserEmail,
                toEmail = toEmail,
                message = "[EMOJI]" + selected
            };
            string json = JsonConvert.SerializeObject(pkt);
            await TcpHelper.SendLineAsync(json);

            var emojiMsg = new ChatMessage
            {
                Content = "[EMOJI]" + selected,
                IsFromCurrentUser = true,
                Time = DateTime.Now
            };
            SaveToHistory(toEmail, emojiMsg);
            DisplayEmoji(selected, true);
        }

        private async void nutFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toEmail)) { MessageBox.Show("Chưa chọn người để gửi."); return; }
            using (var dlg = new OpenFileDialog { Filter = "Ảnh|*.jpg;*.jpeg;*.png;*.gif|Tất cả|*.*" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                byte[] bytes = File.ReadAllBytes(dlg.FileName);
                if (bytes.Length > 5_000_000) { MessageBox.Show("File quá lớn (tối đa 5MB)."); return; }
                string base64 = Convert.ToBase64String(bytes);

                string payload = "[FILE]|" + Path.GetFileName(dlg.FileName) + "|" + base64;
                var pkt = new PrivateChatMsg
                {
                    fromEmail = currentUserEmail,
                    toEmail = toEmail,
                    message = payload
                };
                string json = JsonConvert.SerializeObject(pkt);
                await TcpHelper.SendLineAsync(json);

                var fileMsg = new ChatMessage
                {
                    Content = payload,
                    IsFromCurrentUser = true,
                    Time = DateTime.Now
                };
                SaveToHistory(toEmail, fileMsg);
                DisplayFileFromPayload(payload, true);
            }
        }

        private void DisplayFileFromPayload(string payload, bool isMe)
        {
            var parts = payload.Split('|');
            if (parts.Length < 3) return;

            string fileName = parts[1];
            string base64 = parts[2];

            var container = new Panel { AutoSize = true, Margin = new Padding(0, 8, 0, 8) };
            var pic = new PictureBox
            {
                Width = 220,
                Height = 150,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(240, 240, 240),
                Margin = isMe ? new Padding(80, 0, 10, 0) : new Padding(10, 0, 80, 0)
            };

            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                string tmp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + "_" + fileName);
                File.WriteAllBytes(tmp, bytes);
                pic.ImageLocation = tmp;
            }
            catch { }

            var lbl = new Label { Text = fileName, AutoSize = true, Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Margin = new Padding(pic.Margin.Left, 2, 0, 0) };
            container.Controls.AddRange(new Control[] { pic, lbl });
            khungTinNhan.Controls.Add(container);
            khungTinNhan.ScrollControlIntoView(container);
        }

        private void DisplayEmoji(string emoji, bool isMe)
        {
            var container = new Panel { AutoSize = true, Margin = new Padding(0, 6, 0, 6) };
            var lbl = new Label
            {
                Text = emoji,
                AutoSize = true,
                Font = new Font("Segoe UI Emoji", 28f),
                Padding = new Padding(10),
                BackColor = isMe ? Color.FromArgb(0, 120, 215) : Color.FromArgb(245, 248, 250),
                ForeColor = isMe ? Color.White : Color.Black,
                Margin = isMe ? new Padding(80, 0, 10, 0) : new Padding(10, 0, 80, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            container.Controls.Add(lbl);
            khungTinNhan.Controls.Add(container);
            khungTinNhan.ScrollControlIntoView(container);
        }
    }
}

