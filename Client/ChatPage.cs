using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Common;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class ChatPage : Form
    {
        private readonly string roomCode;

        public ChatPage(string roomCode)
        {
            InitializeComponent();
            this.roomCode = roomCode;

            // Cấu hình FlowLayoutPanel nếu chưa set trong Designer
            flpMessages.AutoScroll = true;
            flpMessages.WrapContents = false;
            flpMessages.FlowDirection = FlowDirection.TopDown;

            // Đăng ký sự kiện nhận tin
            TcpHelper.OnMessageReceived += TcpHelper_OnMessageReceived;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Hủy đăng ký khi form đóng để tránh leak event
            TcpHelper.OnMessageReceived -= TcpHelper_OnMessageReceived;
            base.OnFormClosed(e);
        }

        private void TcpHelper_OnMessageReceived(string json)
        {
            if (this.IsDisposed) return;

            // Nếu đang ở thread khác UI → invoke lại
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(TcpHelper_OnMessageReceived), json);
                return;
            }

            // DEBUG nếu cần:
            // MessageBox.Show("ChatPage recv: " + json);

            JObject jo;
            try
            {
                jo = JObject.Parse(json);
            }
            catch
            {
                return;
            }

            var type = (string)jo["type"];
            if (!string.Equals(type, MsgType.GROUP_CHAT, StringComparison.OrdinalIgnoreCase))
                return;

            var rc = (string)jo["roomCode"];
            if (!string.Equals(rc, roomCode, StringComparison.OrdinalIgnoreCase))
                return;

            var fromEmail = (string)jo["fromEmail"];
            var fromName = (string)jo["fromName"];
            var message = (string)jo["message"];

            if (string.IsNullOrWhiteSpace(message))
                return;

            bool isMe = string.Equals(fromEmail, Session.Email, StringComparison.OrdinalIgnoreCase);
            string prefix = isMe ? "Me" : (fromName ?? fromEmail);

            AddMessageBubble($"{prefix}: {message}", isMe);
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            // Hiện luôn trên flpMessages cho chính mình
            string selfLine = $"Me: {msg}";
            AddMessageBubble(selfLine, true);

            txtMessage.Clear();

            // Gửi lên server cho các client khác
            await TcpHelper.SendGroupChatAsync(
                roomCode,
                msg,
                Session.Email,
                Session.FullName ?? Session.Email
            );
        }

        private void AddMessageBubble(string text, bool isMe)
        {
            var bubble = new Panel();
            bubble.AutoSize = true;
            bubble.MaximumSize = new Size(flpMessages.Width - 40, 0);
            bubble.Padding = new Padding(10);
            bubble.Margin = new Padding(5);
            bubble.BackColor = isMe ? Color.LightSkyBlue : Color.Gainsboro;

            var lbl = new Label();
            lbl.AutoSize = true;
            lbl.MaximumSize = new Size(bubble.MaximumSize.Width - 20, 0);
            lbl.Text = text;
            lbl.ForeColor = isMe ? Color.White : Color.Black;

            bubble.Controls.Add(lbl);

            // Với FlowLayoutPanel, muốn canh trái/phải thì xài RightToLeft
            bubble.RightToLeft = isMe ? RightToLeft.Yes : RightToLeft.No;

            flpMessages.Controls.Add(bubble);
            flpMessages.ScrollControlIntoView(bubble);
        }
    }
}
