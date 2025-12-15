using System;
using System.Drawing;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class TeamsCard : UserControl
    {
        public Image AvatarImage => picAvatar.Image;
        public TeamsCard()
        {
            InitializeComponent();
            RegisterClickEvent(this);
        }

        #region Event Registration
        private void RegisterClickEvent(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                c.Click += Child_Click;
                if (c.HasChildren)
                    RegisterClickEvent(c);
            }
        }
        #endregion

        #region Child Click
        private void Child_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }
        #endregion

        #region Override OnClick
        public string Mon
        {
            get => lbClassname.Text;
            set => lbClassname.Text = value;
        }

        public string Lop
        {
            get => lbClasscode.Text;
            set {
                lbClasscode.Text = value;
                GenerateAvatarFromCode(value); 
            }

        }

        private static readonly Color[] AvatarColors =
        {
            Color.FromArgb(0xC3, 0x00, 0xC3),
            Color.FromArgb(0x8A, 0x2B, 0xE2),
            Color.FromArgb(0xFF, 0x66, 0x99),
            Color.FromArgb(0xFF, 0xAA, 0x00),
            Color.FromArgb(0x00, 0xB8, 0xD4),
            Color.FromArgb(0x2D, 0x6A, 0x4F),
        };

        private void GenerateAvatarFromCode(string classCode)
        {
            if (string.IsNullOrWhiteSpace(classCode)) return;

            int w = picAvatar.Width > 0 ? picAvatar.Width : 60;
            int h = picAvatar.Height > 0 ? picAvatar.Height : 60;

            int hash = Math.Abs(classCode.GetHashCode());
            int colorIndex = hash % AvatarColors.Length;
            Color bgColor = AvatarColors[colorIndex];

            var bmp = new Bitmap(w, h);
            using (var g = Graphics.FromImage(bmp))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, 0, 0, w, h);
                }

                string letter = classCode.Trim()[0].ToString().ToUpper();

                using (var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                using (var font = new Font("Segoe UI", w / 2.5f, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    g.DrawString(letter, font, textBrush, new RectangleF(0, 0, w, h), sf);
                }
            }

            picAvatar.Image = bmp;
        }
        #endregion

    }
}
