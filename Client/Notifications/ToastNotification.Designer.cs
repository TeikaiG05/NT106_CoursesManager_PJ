using System.Windows.Forms;
using System.Drawing;

namespace NT106_BT2.Notifications
{
    partial class ToastNotification
    {
        private Label lbIcon;
        private Label lbMessage;

        private void InitializeComponent()
        {
            this.lbIcon = new Label();
            this.lbMessage = new Label();

            // 
            // lbIcon
            // 
            this.lbIcon.Font = new Font("Segoe UI Emoji", 14F, FontStyle.Regular, GraphicsUnit.Point);
            this.lbIcon.Location = new Point(10, 8);
            this.lbIcon.Size = new Size(34, 34);
            this.lbIcon.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbMessage
            // 
            this.lbMessage.Font = new Font("Segoe UI", 10F);
            this.lbMessage.ForeColor = Color.White;
            this.lbMessage.Location = new Point(50, 12);
            this.lbMessage.Size = new Size(240, 30);
            this.lbMessage.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ToastNotification
            // 
            this.BackColor = Color.Gray;
            this.Controls.Add(this.lbIcon);
            this.Controls.Add(this.lbMessage);
            this.Name = "ToastNotification";
            this.Size = new Size(310, 50);
            this.BorderStyle = BorderStyle.None;
        }
    }
}
