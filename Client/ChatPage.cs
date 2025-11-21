using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

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
    }
}
