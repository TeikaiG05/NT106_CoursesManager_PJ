using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class ChatPage : Form
    {
        private readonly string roomCode;

        #region CONSTRUCTOR
        public ChatPage(string roomCode)
        {
            InitializeComponent();
            this.roomCode = roomCode?.Trim();

            InitTableLayout();
            btnBrowse.Click += btnBrowse_Click;
            btnSend.Enabled = false;
            System.Diagnostics.Debug.WriteLine($"[ChatPage] ctor room={this.roomCode}, email={Session.Email}");

            TcpHelper.OnMessageReceived += TcpHelper_OnMessageReceived;
            System.Diagnostics.Debug.WriteLine("[ChatPage] Subscribed to TcpHelper.OnMessageReceived");

            _ = TcpHelper.ConnectAsync();
            _ = LoadHistoryAsync();
        }
        #endregion

        #region FORM CLOSED
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            TcpHelper.OnMessageReceived -= TcpHelper_OnMessageReceived;
            System.Diagnostics.Debug.WriteLine("[ChatPage] Unsubscribed from TcpHelper.OnMessageReceived");
            base.OnFormClosed(e);
        }
        #endregion

        #region INIT TABLE LAYOUT
        private void InitTableLayout()
        {
            tblMessages.SuspendLayout();

            tblMessages.Dock = DockStyle.Fill;
            tblMessages.AutoScroll = true;

            tblMessages.AutoSize = false;
            tblMessages.GrowStyle = TableLayoutPanelGrowStyle.AddRows;

            tblMessages.ColumnCount = 1;
            tblMessages.ColumnStyles.Clear();
            tblMessages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            tblMessages.RowStyles.Clear();
            tblMessages.RowCount = 0;

            tblMessages.ResumeLayout();
        }
        #endregion

        #region LOAD HISTORY
        private async Task LoadHistoryAsync()
        {
            try
            {
                var req = new GroupChatHistoryReq
                {
                    type = MsgType.GROUP_CHAT_HISTORY_REQ,
                    roomCode = roomCode,
                    take = 50
                };

                await TcpHelper.SendLineAsync(JsonConvert.SerializeObject(req));
                System.Diagnostics.Debug.WriteLine($"[ChatPage] Sent HISTORY_REQ room={roomCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[ChatPage] LoadHistoryAsync error: " + ex.Message);
            }
        }
        #endregion

        #region SEND
        private async void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            AddBubble($"Me: {msg}", true);
            txtMessage.Clear();

            try
            {
                await TcpHelper.SendGroupChatAsync(roomCode, msg, Session.Email, Session.FullName ?? Session.Email
                    );
                System.Diagnostics.Debug.WriteLine($"[ChatPage] Sent GROUP_CHAT room={roomCode}, msg={msg}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region RECEIVE
        private void TcpHelper_OnMessageReceived(string json)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(TcpHelper_OnMessageReceived), json);
                return;
            }

            JObject obj;
            try { obj = JObject.Parse(json); }
            catch { return; }

            string type = ((string)obj["type"] ?? "").Trim();

            if (type.Equals(MsgType.GROUP_CHAT_HISTORY_RES, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var res = JsonConvert.DeserializeObject<GroupChatHistoryRes>(json);
                    if (res?.messages == null) return;
                    if (!string.Equals(res.roomCode?.Trim(), roomCode, StringComparison.OrdinalIgnoreCase))
                        return;

                    System.Diagnostics.Debug.WriteLine($"[ChatPage] HISTORY_RES count={res.messages.Count}");

                    foreach (var m in res.messages)
                    {
                        bool isMe = string.Equals(
                            m.fromEmail?.Trim(),
                            Session.Email?.Trim(),
                            StringComparison.OrdinalIgnoreCase);

                        string display = isMe ? $"Me: {m.message}" : $"{m.fromName ?? m.fromEmail}: {m.message}";

                        AddBubble(display, isMe);
                    }
                    LoadFileHistory();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[ChatPage] HISTORY_RES parse error: " + ex.Message);
                }

                return;
            }

            if (!type.Equals(MsgType.GROUP_CHAT, StringComparison.OrdinalIgnoreCase))
                return;

            GroupChatMsg chat;
            try { chat = JsonConvert.DeserializeObject<GroupChatMsg>(json); }
            catch { return; }

            if (chat == null) return;

            string rc = chat.roomCode?.Trim();
            if (!string.Equals(rc, roomCode, StringComparison.OrdinalIgnoreCase))
                return;

            bool me = string.Equals(
                chat.fromEmail?.Trim(),
                Session.Email?.Trim(),
                StringComparison.OrdinalIgnoreCase);

            if (me) return;

            string displayText = $"{chat.fromName ?? chat.fromEmail}: {chat.message}";
            AddBubble(displayText, false);
        }
        #endregion

        private void LoadFileHistory()
        {
            try
            {
                DataTable dt = DbClient.GetFilesByRoom(roomCode);
                System.Diagnostics.Debug.WriteLine($"[ChatPage] LoadFileHistory count={dt.Rows.Count}");

                foreach (DataRow row in dt.Rows)
                {
                    string filePath = row["FilePath"].ToString();
                    string fileName = row["FileName"].ToString();

                    long sizeBytes = 0;
                    long.TryParse(row["FileSizeBytes"]?.ToString(), out sizeBytes);

                    string uploadedBy = dt.Columns.Contains("UploadedBy")
                        ? row["UploadedBy"]?.ToString()
                        : null;

                    bool isMe = string.Equals(
                        uploadedBy?.Trim(),
                        Session.Email?.Trim(),
                        StringComparison.OrdinalIgnoreCase);

                    if (IsImage(filePath))
                        AddImageBubble(filePath, isMe);
                    else
                        AddFileBubble(filePath, fileName, sizeBytes, isMe);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[ChatPage] LoadFileHistory error: " + ex.Message);
            }
        }

        #region UI BUBBLE
        private void AddBubble(string text, bool isMe)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, bool>(AddBubble), text, isMe);
                return;
            }

            int maxWidth = tblMessages.ClientSize.Width - 60;
            if (maxWidth < 150) maxWidth = 150;

            var bubble = new Panel
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth, 0),
                BackColor = isMe ? Color.SteelBlue : Color.Gainsboro,
                Padding = new Padding(10),
                Margin = isMe ? new Padding(150, 5, 10, 5) : new Padding(10, 5, 150, 5),
                Anchor = isMe ? AnchorStyles.Right : AnchorStyles.Left
            };

            var lbl = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth - 20, 0),
                Text = text,
                ForeColor = isMe ? Color.White : Color.Black,
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            bubble.Controls.Add(lbl);
            lbl.AutoSize = true;

            tblMessages.RowCount++;
            tblMessages.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblMessages.Controls.Add(bubble, 0, tblMessages.RowCount - 1);

            tblMessages.ScrollControlIntoView(bubble);

            System.Diagnostics.Debug.WriteLine($"[ChatPage] Bubble added: {text}");
        }
        #endregion

        #region BTN ENABLE
        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                btnSend.Enabled = false;
            }

            else
            {
                btnSend.Enabled = true;
            }
        }
        #endregion

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file đính kèm";
                ofd.Filter = "Tất cả|*.*|Hình ảnh|*.png;*.jpg;*.jpeg;*.gif;*.bmp|Tài liệu|*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.xls;*.xlsx";
                ofd.Multiselect = false;

                if (ofd.ShowDialog(this) != DialogResult.OK)
                    return;

                string sourcePath = ofd.FileName;
                string fileName = Path.GetFileName(sourcePath);

                string attachDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments");
                Directory.CreateDirectory(attachDir);

                string destPath = Path.Combine(attachDir,
                    Guid.NewGuid().ToString("N") + Path.GetExtension(sourcePath));

                File.Copy(sourcePath, destPath, true);

                var fi = new FileInfo(destPath);
                DbClient.InsertRoomFile(roomCode, fileName, destPath, fi.Length, Session.Email);

                if (IsImage(destPath))
                    AddImageBubble_Me(destPath);
                else
                    AddFileBubble_Me(destPath, fileName, fi.Length);
            }
        }
        private bool IsImage(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp";
        }

        private void AddImageBubble_Me(string filePath)
        {
            AddImageBubble(filePath, true);
        }

        private void AddFileBubble_Me(string filePath, string fileName, long sizeBytes)
        {
            AddFileBubble(filePath, fileName, sizeBytes, true);
        }

        private void AddImageBubble(string filePath, bool isMe)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, bool>(AddImageBubble), filePath, isMe);
                return;
            }

            if (!File.Exists(filePath)) return;

            int maxWidth = tblMessages.ClientSize.Width - 60;
            if (maxWidth < 150) maxWidth = 150;

            var panel = new Panel
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth, 0),
                Padding = new Padding(5),
                Margin = isMe ? new Padding(150, 5, 10, 5) : new Padding(10, 5, 150, 5),
                BackColor = Color.Transparent,
                Anchor = isMe ? AnchorStyles.Right | AnchorStyles.Top
                              : AnchorStyles.Left | AnchorStyles.Top
            };

            var pic = new PictureBox
            {
                Width = 200,
                Height = 150,
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                pic.Image = Image.FromStream(fs);
            }

            pic.Click += (s, e) =>
            {
                using (var f = new ImagePreviewForm(filePath))
                {
                    f.ShowDialog(this);
                }
            };

            panel.Controls.Add(pic);

            tblMessages.RowCount++;
            tblMessages.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblMessages.Controls.Add(panel, 0, tblMessages.RowCount - 1);

            tblMessages.ScrollControlIntoView(panel);
        }

        private void AddFileBubble(string filePath, string fileName, long sizeBytes, bool isMe)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, string, long, bool>(AddFileBubble),
                            filePath, fileName, sizeBytes, isMe);
                return;
            }

            int maxWidth = tblMessages.ClientSize.Width - 60;
            if (maxWidth < 150) maxWidth = 150;

            var row = new Panel
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth, 0),
                Margin = isMe ? new Padding(150, 5, 10, 5) : new Padding(10, 5, 150, 5),
                BackColor = Color.WhiteSmoke,
                Anchor = isMe ? AnchorStyles.Right | AnchorStyles.Top
                              : AnchorStyles.Left | AnchorStyles.Top,
                Padding = new Padding(10, 8, 10, 8)
            };

            var icon = new PictureBox
            {
                Width = 32,
                Height = 32,
                Left = 0,
                Top = 4,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = SystemIcons.Application.ToBitmap()
            };

            var lblName = new Label
            {
                Left = 40,
                Top = 0,
                AutoSize = true,
                MaximumSize = new Size(maxWidth - 60, 0),
                Text = fileName,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var lblSize = new Label
            {
                Left = 40,
                Top = lblName.Bottom + 2,
                AutoSize = true,
                Text = FormatSize(sizeBytes),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };

            row.Cursor = Cursors.Hand;
            row.Click += (s, e) => System.Diagnostics.Process.Start(filePath);

            row.Controls.Add(icon);
            row.Controls.Add(lblName);
            row.Controls.Add(lblSize);

            tblMessages.RowCount++;
            tblMessages.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblMessages.Controls.Add(row, 0, tblMessages.RowCount - 1);

            tblMessages.ScrollControlIntoView(row);
        }

        private string FormatSize(long bytes)
        {
            double kb = bytes / 1024.0;
            if (kb < 1024) return $"{kb:0.#} KB";
            double mb = kb / 1024.0;
            return $"{mb:0.#} MB";
        }
    }
}
