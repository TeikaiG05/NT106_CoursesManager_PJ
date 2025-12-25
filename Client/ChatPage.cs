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
        private LobbyForm lobby;
        private bool joinBubbleShown = false;
        private Control joinCallBubble;
        private readonly string roomCode;

        #region CONSTRUCTOR
        public ChatPage(string roomCode)
        {
            InitializeComponent();
            this.roomCode = roomCode?.Trim();
            btnCall.Click += btnCall_Click;
            InitTableLayout();
            //btnBrowse.Click += btnBrowse_Click;
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

        #region TIMELINE ITEM
        private enum TimelineKind
        {
            Text,
            File
        }

        private sealed class TimelineItem
        {
            public TimelineKind Kind { get; set; }
            public DateTime Time { get; set; }

            public GroupChatMsgEx Message { get; set; }

            public DataRow FileRow { get; set; }
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
                await TcpHelper.SendGroupChatAsync(roomCode, msg, Session.Email, Session.FullName ?? Session.Email);
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

                    DataTable fileTable = DbClient.GetFilesByRoom(roomCode);
                    System.Diagnostics.Debug.WriteLine($"[ChatPage] FileHistory count={fileTable.Rows.Count}");

                    var timeline = new System.Collections.Generic.List<TimelineItem>();

                    foreach (var m in res.messages)
                    {
                        timeline.Add(new TimelineItem
                        {
                            Kind = TimelineKind.Text,
                            Time = m.sentAt,
                            Message = m
                        });
                    }

                    foreach (DataRow row in fileTable.Rows)
                    {
                        DateTime uploadedAt;

                        var uploadedObj = row["UploadedAt"];
                        if (uploadedObj is DateTime dt)
                            uploadedAt = dt;
                        else
                            uploadedAt = DateTime.MinValue;

                        timeline.Add(new TimelineItem
                        {
                            Kind = TimelineKind.File,
                            Time = uploadedAt,
                            FileRow = row
                        });
                    }
                    timeline.Sort((a, b) => a.Time.CompareTo(b.Time));

                    tblMessages.SuspendLayout();
                    tblMessages.Controls.Clear();
                    tblMessages.RowStyles.Clear();
                    tblMessages.RowCount = 0;

                    foreach (var item in timeline)
                    {
                        if (item.Kind == TimelineKind.Text)
                        {
                            var m = item.Message;

                            bool isMe = string.Equals(
                                m.fromEmail?.Trim(),
                                Session.Email?.Trim(),
                                StringComparison.OrdinalIgnoreCase);

                            string display = isMe ? $"Me: {m.message}" : $"{m.fromName ?? m.fromEmail}: {m.message}";

                            AddBubble(display, isMe);
                        }
                        else
                        {
                            string filePath = item.FileRow["FilePath"].ToString();
                            string fileName = item.FileRow["FileName"].ToString();
                            long sizeBytes = Convert.ToInt64(item.FileRow["FileSizeBytes"]);

                            bool isMeFile = false;
                            string uploader = null;
                            if (item.FileRow.Table.Columns.Contains("UploadedBy"))
                            {
                                uploader = item.FileRow["UploadedBy"]?.ToString();
                                isMeFile = string.Equals(uploader?.Trim(), Session.Email?.Trim(), StringComparison.OrdinalIgnoreCase);
                            }

                            string displayName = isMeFile ? "Me" : (string.IsNullOrWhiteSpace(uploader) ? "Unknown" : uploader);

                            if (IsImage(filePath))
                            {
                                AddImageBubble(filePath, isMeFile, displayName);
                            }
                            else
                            {
                                AddFileBubble(filePath, fileName, sizeBytes, isMeFile, displayName);
                            }
                        }
                    }

                    tblMessages.ResumeLayout();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[ChatPage] HISTORY_RES parse error: " + ex.Message);
                }

                return;
            }


            if (type.Equals(MsgType.CALL_STATE, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var state = JsonConvert.DeserializeObject<CallStateRes>(json);
                    if (state == null) return;

                    if (!string.Equals(state.roomCode?.Trim(), roomCode, StringComparison.OrdinalIgnoreCase))
                        return;

                    if (lobby != null && !lobby.IsDisposed && state.members != null)
                    {
                        var names = new System.Collections.Generic.List<string>();
                        foreach (var m in state.members)
                        {
                            string display = !string.IsNullOrWhiteSpace(m.name)
                                ? m.name
                                : m.email;
                            names.Add(display);
                        }

                        lobby.SetParticipants(names);
                    }

                    bool isMember = false;
                    if (state.members != null)
                    {
                        foreach (var m in state.members)
                        {
                            if (string.Equals(m.email?.Trim(), Session.Email?.Trim(),
                                StringComparison.OrdinalIgnoreCase))
                            {
                                isMember = true;
                                break;
                            }
                        }
                    }

                    bool hasCall = state.members != null && state.members.Count > 0;

                    if (hasCall && !isMember)
                    {
                        // luôn refresh bubble theo trạng thái mới
                        RemoveJoinCallBubble();
                        AddJoinCallBubble();
                    }
                    else
                    {
                        RemoveJoinCallBubble();
                    }

                    joinBubbleShown = joinCallBubble != null;

                    SetCallButtonEnabled(!hasCall || isMember);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[ChatPage] CALL_STATE parse error: " + ex.Message);
                }

                return;
            }

            if (type.Equals(MsgType.CALL_JOIN, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var res = JsonConvert.DeserializeObject<CallJoinRes>(json);
                    if (res == null) return;
                    if (!string.Equals(res.roomCode?.Trim(), roomCode, StringComparison.OrdinalIgnoreCase))
                        return;

                    CallUdp.SetIds(res.roomId, res.userId);
                    _ = CallUdp.SendWarmupAsync();

                    System.Diagnostics.Debug.WriteLine($"[ChatPage] CALL_JOIN RES roomId={res.roomId}, userId={res.userId}");
                }
                catch { }
                return;
            }

            if (type.Equals(MsgType.CALL_SHARE, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var res = JsonConvert.DeserializeObject<CallShareRes>(json);
                    if (res == null) return;
                    if (!string.Equals(res.roomCode?.Trim(), roomCode, StringComparison.OrdinalIgnoreCase))
                        return;

                    if (lobby != null && !lobby.IsDisposed)
                        lobby.SetSharingUser(res.sharerName);
                }
                catch { }
                return;
            }

            if (type.Equals(MsgType.ROOM_FILE_ADDED, StringComparison.OrdinalIgnoreCase))
            {
                RoomFileAddedMsg f;
                try { f = JsonConvert.DeserializeObject<RoomFileAddedMsg>(json); }
                catch { return; }

                if (f == null) return;
                if (!string.Equals(f.roomCode?.Trim(), roomCode, StringComparison.OrdinalIgnoreCase))
                    return;

                bool isMyFile = string.Equals(f.uploadedBy?.Trim(), Session.Email?.Trim(), StringComparison.OrdinalIgnoreCase);

                if (!isMyFile)
                {
                    if (IsImage(f.filePath))
                        AddImageBubble(f.filePath, false, f.uploadedBy ?? "Unknown");
                    else
                        AddFileBubble(f.filePath, f.fileName, f.fileSizeBytes, false, f.uploadedBy ?? "Unknown");
                }

                GroupChatForm.NotifyFileAdded(f.roomCode);
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

            bool isMyChat = string.Equals(
                chat.fromEmail?.Trim(),
                Session.Email?.Trim(),
                StringComparison.OrdinalIgnoreCase);

            if (isMyChat) return;

            string displayText = $"{chat.fromName ?? chat.fromEmail}: {chat.message}";
            AddBubble(displayText, false);

            return;
        }
        #endregion

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

        private void AddJoinCallBubble()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(AddJoinCallBubble));
                return;
            }

            RemoveJoinCallBubble(); // tránh nhân đôi bubble cũ

            int maxWidth = tblMessages.ClientSize.Width - 60;
            if (maxWidth < 200) maxWidth = 200;

            var bubble = new Panel
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth, 0),
                BackColor = Color.LightYellow,
                Padding = new Padding(10),
                Margin = new Padding(10, 5, 150, 5),
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            var lbl = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth - 20, 0),
                Text = "Đang có cuộc gọi nhóm trong lớp này. Nhấn 'Tham gia' để vào.",
                Font = new Font("Segoe UI", 9F)
            };

            var btnJoin = new Button
            {
                AutoSize = true,
                Text = "Tham gia cuộc gọi",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 6, 0, 0)
            };
            btnJoin.FlatAppearance.BorderSize = 0;

            lbl.Location = new Point(0, 0);
            btnJoin.Location = new Point(0, lbl.Bottom + 5);

            bubble.Controls.Add(lbl);
            bubble.Controls.Add(btnJoin);

            btnJoin.Click += async (s, e) =>
            {
                OpenLobbyForm();
                await TcpHelper.SendCallJoinAsync(roomCode);
            };

            tblMessages.RowCount++;
            tblMessages.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblMessages.Controls.Add(bubble, 0, tblMessages.RowCount - 1);

            tblMessages.ScrollControlIntoView(bubble);

            joinCallBubble = bubble;
        }

        private void RemoveJoinCallBubble()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(RemoveJoinCallBubble));
                return;
            }

            if (joinCallBubble != null)
            {
                try { tblMessages.Controls.Remove(joinCallBubble); } catch { }
                joinCallBubble.Dispose();
                joinCallBubble = null;
            }
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

        #region BTN BROWSE CLICK
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

                _ = TcpHelper.SendRoomFileAddedAsync(roomCode, fileName, destPath, fi.Length);
            }
        }
        #endregion

        #region HELPER METHODS
        private bool IsImage(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp";
        }

        private void AddImageBubble_Me(string filePath)
        {
            AddImageBubble(filePath, true, "Me");
        }

        private void AddFileBubble_Me(string filePath, string fileName, long sizeBytes)
        {
            AddFileBubble(filePath, fileName, sizeBytes, true, "Me");
        }

        private void AddImageBubble(string filePath, bool isMe, string senderName)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, bool, string>(AddImageBubble), filePath, isMe, senderName);
                return;
            }

            if (!File.Exists(filePath)) return;

            int maxWidth = tblMessages.ClientSize.Width - 60;
            if (maxWidth < 180) maxWidth = 180;

            var bubble = new Panel
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth, 0),
                BackColor = isMe ? Color.SteelBlue : Color.Gainsboro,
                Padding = new Padding(10),
                Margin = isMe ? new Padding(150, 5, 10, 5) : new Padding(10, 5, 150, 5),
                Anchor = isMe ? AnchorStyles.Right | AnchorStyles.Top : AnchorStyles.Left | AnchorStyles.Top,
                Cursor = Cursors.Hand
            };

            string senderLine = string.IsNullOrWhiteSpace(senderName) ? "" : senderName + ":";

            var lblSender = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth - 20, 0),
                Text = senderLine,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = isMe ? Color.WhiteSmoke : Color.DimGray
            };

            var pic = new PictureBox
            {
                Width = Math.Min(260, maxWidth - 20),
                Height = 160,
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var img = Image.FromStream(fs))
            {
                pic.Image = new Bitmap(img);
            }

            bubble.Controls.Add(lblSender);
            bubble.Controls.Add(pic);

            pic.Top = lblSender.Bottom + 6;

            EventHandler openPreview = (s, e) =>
            {
                using (var f = new ImagePreviewForm(filePath))
                    f.ShowDialog(this);
            };

            bubble.Click += openPreview;
            lblSender.Click += openPreview;
            pic.Click += openPreview;

            tblMessages.RowCount++;
            tblMessages.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblMessages.Controls.Add(bubble, 0, tblMessages.RowCount - 1);

            tblMessages.ScrollControlIntoView(bubble);
        }


        private void AddFileBubble(string filePath, string fileName, long sizeBytes, bool isMe, string senderName)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, string, long, bool, string>(AddFileBubble),
                            filePath, fileName, sizeBytes, isMe, senderName);
                return;
            }

            int maxWidth = tblMessages.ClientSize.Width - 60;
            if (maxWidth < 180) maxWidth = 180;

            var bubble = new Panel
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth, 0),
                BackColor = isMe ? Color.SteelBlue : Color.Gainsboro,
                Padding = new Padding(10),
                Margin = isMe ? new Padding(150, 5, 10, 5) : new Padding(10, 5, 150, 5),
                Anchor = isMe ? AnchorStyles.Right | AnchorStyles.Top : AnchorStyles.Left | AnchorStyles.Top,
                Cursor = Cursors.Hand
            };

            string senderLine = string.IsNullOrWhiteSpace(senderName) ? "" : senderName + ":";

            var lblSender = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth - 20, 0),
                Text = senderLine,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = isMe ? Color.WhiteSmoke : Color.DimGray
            };

            var fileCard = new Panel
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth - 20, 0),
                BackColor = isMe ? Color.FromArgb(40, 255, 255, 255) : Color.WhiteSmoke,
                Padding = new Padding(8),
                Margin = new Padding(0, 6, 0, 0)
            };

            var icon = new PictureBox
            {
                Width = 32,
                Height = 32,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = SystemIcons.Application.ToBitmap()
            };

            var lblName = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(maxWidth - 80, 0),
                Text = fileName,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = isMe ? Color.White : Color.Black
            };

            var lblSize = new Label
            {
                AutoSize = true,
                Text = FormatSize(sizeBytes),
                Font = new Font("Segoe UI", 8),
                ForeColor = isMe ? Color.WhiteSmoke : Color.Gray
            };

            icon.Location = new Point(0, 0);
            lblName.Location = new Point(icon.Right + 8, 0);
            lblSize.Location = new Point(icon.Right + 8, lblName.Bottom + 2);

            fileCard.Controls.Add(icon);
            fileCard.Controls.Add(lblName);
            fileCard.Controls.Add(lblSize);

            bubble.Controls.Add(lblSender);
            bubble.Controls.Add(fileCard);
            fileCard.Top = lblSender.Bottom + 6;

            EventHandler open = (s, e) => TryOpenFile(filePath);
            bubble.Click += open;
            lblSender.Click += open;
            fileCard.Click += open;
            icon.Click += open;
            lblName.Click += open;
            lblSize.Click += open;

            tblMessages.RowCount++;
            tblMessages.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblMessages.Controls.Add(bubble, 0, tblMessages.RowCount - 1);

            tblMessages.ScrollControlIntoView(bubble);
        }
      

        private void TryOpenFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File không tồn tại: " + filePath, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không mở được file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatSize(long bytes)
        {
            double kb = bytes / 1024.0;
            if (kb < 1024) return $"{kb:0.#} KB";
            double mb = kb / 1024.0;
            return $"{mb:0.#} MB";
        }

        private void SetCallButtonEnabled(bool enabled)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(SetCallButtonEnabled), enabled);
                return;
            }

            btnCall.Enabled = enabled;
        }

        private void OpenLobbyForm()
        {
            if (lobby == null || lobby.IsDisposed)
            {
                string displayName = roomCode;
                lobby = new LobbyForm(roomCode, displayName);
                lobby.FormClosed += (s, e) => SetCallButtonEnabled(true);
            }

            lobby.Show();
            lobby.Activate();
        }

        private async void btnCall_Click(object sender, EventArgs e)
        {
            SetCallButtonEnabled(false);

            string inviterName = Session.FullName ?? Session.Email ?? "Someone";
            string inviteMsg = $"{inviterName} da mo phong call. Bam 'Tham gia cuoc goi' de vao.";

            AddBubble($"Me: {inviteMsg}", true);
            OpenLobbyForm();

            try
            {
                await TcpHelper.SendGroupChatAsync(roomCode, inviteMsg, Session.Email, inviterName);
                await TcpHelper.SendCallJoinAsync(roomCode);
            }
            catch (Exception ex)
            {
                SetCallButtonEnabled(true);
                MessageBox.Show("Khong the bat dau cuoc goi: " + ex.Message,
                    "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
