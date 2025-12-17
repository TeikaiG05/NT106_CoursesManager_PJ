using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class ChatForm : Form
    {
        private readonly Dictionary<string, Friend> friends =
            new Dictionary<string, Friend>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, List<Message>> chatHistory =
            new Dictionary<string, List<Message>>(StringComparer.OrdinalIgnoreCase);

        private string currentSelectedFriendEmail;

        private readonly string currentUserEmail;
        private readonly string currentUserName;

        public ChatForm() : this("unknown@example.com", "Unknown User") { }

        public ChatForm(string myEmail, string myName)
        {
            InitializeComponent();

            currentUserEmail = myEmail;
            currentUserName = myName;

            this.Load += ChatForm_Load;
            oTimKiem.KeyDown += oTimKiem_KeyDown;
            nutGui.Click += nutGui_Click;

            oTimKiem.Text = "Search by email";
            oTimKiem.ForeColor = Color.Silver;
            oTimKiem.GotFocus += (s, e) =>
            {
                if (oTimKiem.Text == "Search by email")
                {
                    oTimKiem.Text = "";
                    oTimKiem.ForeColor = Color.Black;
                }
            };
            oTimKiem.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(oTimKiem.Text))
                {
                    oTimKiem.Text = "Search by email";
                    oTimKiem.ForeColor = Color.Silver;
                }
            };

            TcpHelper.OnMessageReceived += TcpHelper_OnMessageReceived;
            TcpHelper.OnError += msg =>
            {
                MessageBox.Show(msg, "TCP Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
        }

        private async void ChatForm_Load(object sender, EventArgs e)
        {
            luongChatItems.Controls.Clear();
            luongChatItems.Controls.Add(labelChatTitle);

            tieuDeChat.Text = "";
            tieuDeChiTiet.Text = "People (0)";

            await TcpHelper.ConnectAsync();
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

            if (!friends.ContainsKey(email))
            {
                var friend = new Friend
                {
                    Email = email,
                    Name = email,
                    LastMessage = "No messages yet",
                    LastMessageTime = DateTime.Now,
                    IsOnline = true
                };

                friends[email] = friend;
                chatHistory[email] = new List<Message>();
            }

            SelectFriend(email);
        }

        private void SelectFriend(string email)
        {
            currentSelectedFriendEmail = email;
            var f = friends[email];

            tieuDeChat.Text = f.Name;
            tieuDeChiTiet.Text = "People (1)";

            LoadChatHistory(email);
            HighlightSelectedFriend(email);
        }

        private void LoadChatHistory(string email)
        {
            khungTinNhan.Controls.Clear();

            if (!chatHistory.ContainsKey(email)) return;

            foreach (var msg in chatHistory[email])
                DisplayMessage(msg);
        }

        private void HighlightSelectedFriend(string email)
        {
            foreach (Control c in luongChatItems.Controls)
            {
                if (c is Panel p && p.Tag is string tag)
                {
                    p.BackColor = tag.Equals(email, StringComparison.OrdinalIgnoreCase)
                        ? Color.FromArgb(210, 227, 252)
                        : Color.White;
                }
            }
        }

        private async void nutGui_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentSelectedFriendEmail))
            {
                MessageBox.Show("Vui lòng tìm email người cần chat ở ô Search trước.");
                return;
            }

            string content = oNhapTin.Text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            string toEmail = currentSelectedFriendEmail;

            try
            {
                var pkt = new PrivateChatMsg
                {
                    type = "PRIVATE_CHAT",
                    fromEmail = currentUserEmail,
                    toEmail = toEmail,
                    message = content
                };

                string json = JsonConvert.SerializeObject(pkt);
                Console.WriteLine("SEND: " + json); 
                await TcpHelper.SendLineAsync(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi tin: " + ex.Message);
                return;
            }

            var msg = new Message
            {
                FromEmail = currentUserEmail,
                FromName = currentUserName,
                ToEmail = toEmail,
                Content = content,
                SentAt = DateTime.Now,
                IsFromCurrentUser = true
            };

            if (!chatHistory.ContainsKey(toEmail))
                chatHistory[toEmail] = new List<Message>();

            chatHistory[toEmail].Add(msg);

            if (!friends.ContainsKey(toEmail))
            {
                friends[toEmail] = new Friend
                {
                    Email = toEmail,
                    Name = toEmail,
                    LastMessage = content,
                    LastMessageTime = msg.SentAt,
                    IsOnline = true
                };
            }

            friends[toEmail].LastMessage = content;
            friends[toEmail].LastMessageTime = msg.SentAt;

            DisplayMessage(msg);
            oNhapTin.Clear();

            EnsureBannerExists(toEmail);
            ReloadFriendBannersSorted();
        }

        private void TcpHelper_OnMessageReceived(string line)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(TcpHelper_OnMessageReceived), line);
                return;
            }

            Console.WriteLine("RECV: " + line);              
            Console.WriteLine("CurrentUserEmail: " + currentUserEmail); 

            try
            {
                dynamic env = JsonConvert.DeserializeObject(line);
                if (env == null || env.type == null)
                    return;

                string type = (string)env.type;

                if (type == "PRIVATE_CHAT")
                {
                    var chat = JsonConvert.DeserializeObject<PrivateChatMsg>(line);
                    if (chat == null) return;

                    if (!string.Equals(chat.toEmail, currentUserEmail, StringComparison.OrdinalIgnoreCase))
                        return;

                    HandleIncomingPrivateMessage(chat.fromEmail, chat.message);
                }

            }
            catch
            {
            }
        }

        private void HandleIncomingPrivateMessage(string fromEmail, string content)
        {
            if (!friends.ContainsKey(fromEmail))
            {
                var friend = new Friend
                {
                    Email = fromEmail,
                    Name = fromEmail,
                    LastMessage = content,
                    LastMessageTime = DateTime.Now,
                    IsOnline = true
                };
                friends[fromEmail] = friend;
                chatHistory[fromEmail] = new List<Message>();

                EnsureBannerExists(fromEmail);
            }

            var msg = new Message
            {
                FromEmail = fromEmail,
                FromName = fromEmail,
                ToEmail = currentUserEmail,
                Content = content,
                SentAt = DateTime.Now,
                IsFromCurrentUser = false
            };

            chatHistory[fromEmail].Add(msg);

            if (string.Equals(currentSelectedFriendEmail, fromEmail, StringComparison.OrdinalIgnoreCase))
                DisplayMessage(msg);

            friends[fromEmail].LastMessage = content;
            friends[fromEmail].LastMessageTime = msg.SentAt;

            ReloadFriendBannersSorted();
        }


        private void DisplayMessage(Message msg)
        {
            var container = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 4, 0, 4)
            };

            var bubble = new Label
            {
                Text = msg.Content,
                AutoSize = true,
                MaximumSize = new Size(260, 0),  
                Padding = new Padding(8),
                BorderStyle = BorderStyle.None
            };

            if (msg.IsFromCurrentUser)
            {
                bubble.BackColor = Color.FromArgb(0, 120, 215);
                bubble.ForeColor = Color.White;

                bubble.TextAlign = ContentAlignment.MiddleLeft;
                bubble.Margin = new Padding(80, 0, 10, 0);
            }
            else
            {
                bubble.BackColor = Color.FromArgb(240, 240, 240);
                bubble.ForeColor = Color.Black;

                bubble.TextAlign = ContentAlignment.MiddleLeft;
                bubble.Margin = new Padding(10, 0, 80, 0);
            }

            container.Controls.Add(bubble);
            khungTinNhan.Controls.Add(container);
            khungTinNhan.ScrollControlIntoView(container);
        }






        private void EnsureBannerExists(string email)
        {
            bool exists = luongChatItems.Controls
                .OfType<Panel>()
                .Any(p => p.Tag is string tag &&
                          tag.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (exists) return;

            var friend = friends[email];
            Panel banner = CreateFriendBanner(friend);
            luongChatItems.Controls.Add(banner);
        }

        private void ReloadFriendBannersSorted()
        {
            var headerLabel = labelChatTitle;

            var orderedFriends = friends.Values
                .OrderByDescending(f => f.LastMessageTime)
                .ToList();

            luongChatItems.Controls.Clear();
            luongChatItems.Controls.Add(headerLabel);

            foreach (var f in orderedFriends)
            {
                var b = CreateFriendBanner(f);
                luongChatItems.Controls.Add(b);
            }
        }

        private Panel CreateFriendBanner(Friend friend)
        {
            var banner = new Panel
            {
                Size = new Size(220, 70),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 4),
                Tag = friend.Email,
                Cursor = Cursors.Hand
            };

            banner.Click += (s, e) => SelectFriend(friend.Email);

            var avatar = new Panel
            {
                Size = new Size(44, 44),
                Location = new Point(8, 12),
                BackColor = GenerateColorFromName(friend.Name)
            };
            avatar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode =
                    System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(avatar.BackColor),
                    0, 0, avatar.Width, avatar.Height);

                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                string initial = string.IsNullOrEmpty(friend.Name)
                    ? "?"
                    : friend.Name[0].ToString().ToUpperInvariant();

                e.Graphics.DrawString(initial,
                    new Font("Segoe UI", 14, FontStyle.Bold),
                    Brushes.White,
                    new RectangleF(0, 0, avatar.Width, avatar.Height),
                    sf);
            };
            avatar.Click += (s, e) => SelectFriend(friend.Email);
            banner.Controls.Add(avatar);

            var lblName = new Label
            {
                Text = friend.Name,
                Location = new Point(60, 10),
                Size = new Size(140, 18),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            lblName.Click += (s, e) => SelectFriend(friend.Email);
            banner.Controls.Add(lblName);

            var lblLast = new Label
            {
                Text = friend.LastMessage,
                Location = new Point(60, 30),
                Size = new Size(140, 16),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                AutoEllipsis = true
            };
            lblLast.Click += (s, e) => SelectFriend(friend.Email);
            banner.Controls.Add(lblLast);

            var lblTime = new Label
            {
                Text = FormatTime(friend.LastMessageTime),
                Location = new Point(60, 48),
                Size = new Size(140, 16),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleRight
            };
            lblTime.Click += (s, e) => SelectFriend(friend.Email);
            banner.Controls.Add(lblTime);

            return banner;
        }

        private string FormatTime(DateTime time)
        {
            var span = DateTime.Now - time;
            if (span.TotalMinutes < 1) return "now";
            if (span.TotalHours < 1) return ((int)span.TotalMinutes) + "m";
            if (span.TotalDays < 1) return ((int)span.TotalHours) + "h";
            if (span.TotalDays < 7) return ((int)span.TotalDays) + "d";
            return time.ToString("dd/MM");
        }

        private Color GenerateColorFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return Color.Gray;
            int hash = name.GetHashCode();
            Color[] colors =
            {
                Color.FromArgb(255, 107, 107),
                Color.FromArgb(66, 133, 244),
                Color.FromArgb(52, 168, 83),
                Color.FromArgb(251, 188, 4),
                Color.FromArgb(156, 39, 176),
                Color.FromArgb(0, 150, 136)
            };
            return colors[Math.Abs(hash) % colors.Length];
        }

        
    }


    public class Friend
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public bool IsOnline { get; set; }
    }

    public class Message
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ToEmail { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsFromCurrentUser { get; set; }
    }

    public class PrivateChatMsg
    {
        public string type { get; set; } = "PRIVATE_CHAT";
        public string fromEmail { get; set; }
        public string toEmail { get; set; }
        public string message { get; set; }
    }
}
