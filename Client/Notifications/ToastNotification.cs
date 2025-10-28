using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NT106_BT2.Notifications
{
    public partial class ToastNotification : UserControl
    {
        private Timer fadeTimer;
        private bool fadingIn = true;
        private double opacity = 0;
        private int displayTime = 2500; // ms
        private DateTime startTime;
        private Color baseColor;

        public enum ToastType
        {
            Success,
            Error,
            Warning,
            Info
        }

        public ToastNotification(string message, ToastType type)
        {
            InitializeComponent();
            ApplyStyle(type);
            lbMessage.Text = message;

            this.DoubleBuffered = true;
            this.Visible = false;

            fadeTimer = new Timer();
            fadeTimer.Interval = 30;
            fadeTimer.Tick += FadeTimer_Tick;
        }

        private void ApplyStyle(ToastType type)
        {
            string emoji;
            switch (type)
            {
                case ToastType.Success:
                    baseColor = Color.FromArgb(46, 204, 113); // xanh lá
                    emoji = "✔️";
                    break;
                case ToastType.Error:
                    baseColor = Color.FromArgb(231, 76, 60); // đỏ
                    emoji = "❌";
                    break;
                case ToastType.Warning:
                    baseColor = Color.FromArgb(241, 196, 15); // vàng
                    emoji = "⚠️";
                    break;
                default:
                    baseColor = Color.FromArgb(52, 152, 219); // xanh dương
                    emoji = "ℹ️";
                    break;
            }

            this.BackColor = baseColor;
            lbIcon.Text = emoji;
            lbIcon.BackColor = baseColor;
            lbMessage.BackColor = baseColor;
        }

        public void ShowNotification(Control parent)
        {
            if (parent == null) return;

            this.Visible = true;
            parent.Controls.Add(this);
            this.BringToFront();

            // góc phải dưới
            this.Left = parent.ClientSize.Width - this.Width - 20;
            this.Top = parent.ClientSize.Height - this.Height - 20;

            fadingIn = true;
            opacity = 0;
            startTime = DateTime.Now;

            fadeTimer.Start();
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (fadingIn)
            {
                opacity += 0.08;
                if (opacity >= 1)
                {
                    opacity = 1;
                    fadingIn = false;
                    startTime = DateTime.Now;
                }
            }
            else
            {
                if ((DateTime.Now - startTime).TotalMilliseconds > displayTime)
                {
                    opacity -= 0.05;
                    if (opacity <= 0)
                    {
                        fadeTimer.Stop();
                        this.Parent?.Controls.Remove(this);
                        this.Dispose();
                        return;
                    }
                }
            }
            this.Invalidate();
        }

        // Vẽ bo góc + màu mượt
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int radius = 10;
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            using (GraphicsPath path = GetRoundedRectPath(rect, radius))
            using (SolidBrush bg = new SolidBrush(Color.FromArgb((int)(opacity * 255), baseColor)))
            {
                e.Graphics.FillPath(bg, path);
                this.Region = new Region(path);
            }

            base.OnPaint(e);
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
