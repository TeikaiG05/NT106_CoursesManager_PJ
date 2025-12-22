using System;
using System.Collections.Generic;
using System.Data;
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
    /// <summary>
    /// ============================================================================
    /// ChatForm.cs - Giao diện chat 1-1 giữa 2 người dùng
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Hiển thị danh sách bạn chat
    /// Tìm kiếm và thêm bạn mới
    /// Gửi tin nhắn text
    /// Gửi emoji
    /// Gửi file/ảnh dạng base64
    /// Lưu lịch sử chat vào database
    /// Load lịch sử chat khi mở form
    /// Hiển thị lịch sử chat theo người
    /// 
    /// CÔNG VIỆC CỤ THỂ:
    /// 
    /// 1. ChatForm_Load(object sender, EventArgs e)
    ///    - Khởi tạo: oTimKiem.Text = "Search by email"
    ///    - Load danh sách bạn từ database (LoadFriendsFromDatabase)
    ///    - Vẽ friend banners
    /// 
    /// 2. oTimKiem_KeyDown(object sender, KeyEventArgs e)
    ///    - Khi user nhấn ENTER trong ô tìm kiếm
    ///    - Validate: không được trống, không chat với chính mình
    ///    - Thêm bạn mới nếu chưa có
    ///    - Gọi SelectFriend() để mở chat
    /// 
    /// 3. SelectFriend(string email)
    ///    - Đặt toEmail = email
    ///    - Cập nhật labelChatTitle với tên bạn
    ///    - Load lịch sử chat từ database (LoadChatHistoryFromDatabase)
    /// 
    /// 4. LoadChatHistoryFromDatabase(string friendEmail)  [MỚI]
    ///    - Gọi DbClient.GetPrivateMessages(currentUserEmail, friendEmail)
    ///    - Xóa khung tin nhắn cũ
    ///    - Duyệt qua từng tin nhắn:
    ///      * Nếu [FILE]|... → DisplayFileFromPayload()
    ///      * Nếu [EMOJI]... → DisplayEmoji()
    ///      * Nếu text thường → AddMessageToUI()
    ///    - Scroll xuống dưới cùng
    /// 
    /// 5. nutGui_Click(object sender, EventArgs e)
    ///    - Lấy nội dung từ oNhapTin.Text
    ///    - Validate: không trống, có người nhận
    ///    - Tạo PrivateChatMsg và gửi qua TcpHelper
    ///    - **LƯU VÀO DATABASE** (DbClient.InsertPrivateMessage)  [MỚI]
    ///    - Hiển thị tin nhắn lên UI (AddMessageToUI)
    ///    - Cập nhật LastMessage, LastTime của bạn
    ///    - Vẽ lại friend banners
    ///    - Xóa text box
    /// 
    /// 6. TcpHelper_OnMessageReceived(string line)
    ///    - Lắng nghe event OnMessageReceived từ TCP
    ///    - Parse JSON để lấy type = "PRIVATE_CHAT"
    ///    - Kiểm tra tin là dành cho user hiện tại (to == currentUserEmail)
    ///    - **LƯU TỪNG TIN NHẮN VÀO DATABASE**  [MỚI]
    ///    - Nếu là person hiện đang chat: hiển thị (DisplayFileFromPayload, DisplayEmoji, AddMessageToUI)
    ///    - Cập nhật friend banner
    /// 
    /// 7. LoadFriendsFromDatabase()  [MỚI]
    ///    - Gọi DbClient.GetAllUsers()
    ///    - Với mỗi user: lấy 1 tin gần nhất (GetPrivateMessages(..., take=1))
    ///    - Thêm vào friends dict với LastTime, LastMessage
    /// 
    /// 8. SaveToHistory(string friendEmail, ChatMessage msg)
    ///    - Lưu tin nhắn vào chatHistory dictionary (in-memory)
    ///    - Dùng cho display
    /// 
    /// 9. AddMessageToUI(ChatMessage msg)
    ///    - Tạo Panel chứa Label bubble
    ///    - Nếu IsFromCurrentUser: màu xanh, căn phải
    ///    - Nếu từ bạn: màu xám, căn trái
    ///    - Thêm vào khungTinNhan
    /// 
    /// 10. ReloadFriendBanners()
    ///     - Clear luongChatItems
    ///     - Vẽ label "Chat"
    ///     - Duyệt friends sắp xếp theo LastTime DESC
    ///     - Vẽ CreateFriendBanner cho từng bạn
    /// 
    /// 11. CreateFriendBanner(Friend f) → Control
    ///     - Tạo Panel chứa avatar, name, last message
    ///     - Click → SelectFriend(email)
    /// 
    /// 12. nutEmoji_Click(object sender, EventArgs e)
    ///     - Mở form chọn emoji (30 emojis)
    ///     - Gửi "[EMOJI]" + emoji
    ///     - Lưu và hiển thị
    /// 
    /// 13. nutFile_Click(object sender, EventArgs e)
    ///     - OpenFileDialog để chọn file
    ///     - Validate file size ≤ 5MB
    ///     - Convert file thành base64
    ///     - Gửi "[FILE]|FileName|Base64Data"
    ///     - Lưu và hiển thị
    /// 
    /// 14. DisplayFileFromPayload(string payload, bool isMe)
    ///     - Parse "[FILE]|FileName|Base64" → lấy FileName, Base64
    ///     - Convert base64 → bytes → temp file
    ///     - Tạo PictureBox để hiển thị ảnh
    /// 
    /// 15. DisplayEmoji(string emoji, bool isMe)
    ///     - Hiển thị emoji dưới dạng Label lớn
    ///     - Font: "Segoe UI Emoji", size 28
    /// 
    /// DỊCH VỤ LIÊN KẾT:
    /// - TcpHelper: Gửi/nhận tin nhắn
    /// - DbClient: Lưu/tải tin nhắn từ database
    /// 
    /// CÓ 2 NƠI LƯU DỮ LIỆU:
    /// 1. In-memory: Dictionary<string, List<ChatMessage>> chatHistory
    /// 2. Database: PrivateMessages table
    /// 
    /// ============================================================================
    /// </summary>
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
            oTimKiem.Text = "Search by email";
            labelChatTitle.Text = "Chat";
            
            // Load lịch sử từ database khi form mở
            LoadFriendsFromDatabase();
            
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
            
            // Load lịch sử từ database nếu có
            LoadChatHistoryFromDatabase(email);
        }

        // Thêm phương thức mới để load từ database
        private void LoadChatHistoryFromDatabase(string friendEmail)
        {
            khungTinNhan.Controls.Clear();
            
            try
            {
                // Lấy tin nhắn từ database
                var dbMessages = DbClient.GetPrivateMessages(currentUserEmail, friendEmail);
                
                // Xóa lịch sử cũ trong memory và load từ database
                if (chatHistory.ContainsKey(friendEmail))
                    chatHistory[friendEmail].Clear();
                else
                    chatHistory[friendEmail] = new List<ChatMessage>();

                foreach (var msg in dbMessages)
                {
                    var chatMsg = new ChatMessage
                    {
                        Content = msg.Message,
                        IsFromCurrentUser = string.Equals(msg.FromEmail, currentUserEmail, StringComparison.OrdinalIgnoreCase),
                        Time = msg.SentAt
                    };
                    
                    chatHistory[friendEmail].Add(chatMsg);
                    
                    // Hiển thị tin nhắn
                    if (msg.Message.StartsWith("[FILE]|"))
                    {
                        DisplayFileFromPayload(msg.Message, chatMsg.IsFromCurrentUser);
                    }
                    else if (msg.Message.StartsWith("[EMOJI]"))
                    {
                        string emoji = msg.Message.Substring(7);
                        DisplayEmoji(emoji, chatMsg.IsFromCurrentUser);
                    }
                    else
                    {
                        AddMessageToUI(chatMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi load lịch sử từ database: " + ex.Message);
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
            
            // LƯU VÀO DATABASE
            try
            {
                DbClient.InsertPrivateMessage(currentUserEmail, toEmail, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu tin nhắn vào database: " + ex.Message);
            }

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
                            
                            // LƯU VÀO DATABASE
                            try
                            {
                                DbClient.InsertPrivateMessage(fromEmail, to, msgContent);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Lỗi lưu file chat vào DB: " + ex.Message);
                            }
                            
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
                            
                            // LƯU VÀO DATABASE
                            try
                            {
                                DbClient.InsertPrivateMessage(fromEmail, to, msgContent);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Lỗi lưu emoji chat vào DB: " + ex.Message);
                            }
                            
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
                            
                            // LƯU VÀO DATABASE
                            try
                            {
                                DbClient.InsertPrivateMessage(fromEmail, to, msgContent);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Lỗi lưu text chat vào DB: " + ex.Message);
                            }
                            
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

        private void LoadFriendsFromDatabase()
        {
            try
            {
                // Lấy tất cả người dùng từ database
                var dt = DbClient.GetAllUsers();
                
                foreach (DataRow row in dt.Rows)
                {
                    string userEmail = row["Email"].ToString();
                    if (userEmail.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase))
                        continue;
                    
                    // Lấy lịch sử với mỗi người
                    var messages = DbClient.GetPrivateMessages(currentUserEmail, userEmail, 1);
                    
                    if (messages.Count > 0)
                    {
                        var lastMsg = messages[messages.Count - 1];
                        
                        if (!friends.ContainsKey(userEmail))
                        {
                            friends[userEmail] = new Friend
                            {
                                Email = userEmail,
                                Name = userEmail.Split('@')[0]
                            };
                        }
                        
                        friends[userEmail].LastTime = lastMsg.SentAt;
                        friends[userEmail].LastMessage = lastMsg.Message.Length > 20 
                            ? lastMsg.Message.Substring(0, 20) + "..." 
                            : lastMsg.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load bạn bè từ database: " + ex.Message);
            }
        }
    }
}

