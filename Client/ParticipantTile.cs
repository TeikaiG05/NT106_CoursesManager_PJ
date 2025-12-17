using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class ParticipantTile : UserControl
    {
        private string displayName;
        private string avatarUrl;

        public ParticipantTile()
        {
            InitializeComponent();
            this.BackColor = Color.Transparent;
            picAvatar.Visible = false;
            lblInitials.Visible = true;
            picAvatar.LoadCompleted += PicAvatar_LoadCompleted;
        }

        public string DisplayName
        {
            get => displayName;
            set
            {
                displayName = value;
                lblName.Text = displayName;
                lblInitials.Text = GetInitialsFromName(displayName);
            }
        }

        public string AvatarUrl
        {
            get => avatarUrl;
            set
            {
                avatarUrl = value;
                ApplyAvatarFromUrl(avatarUrl);
            }
        }

        public Image AvatarImage
        {
            get => picAvatar.Image;
            set
            {
                avatarUrl = null;
                if (value == null)
                {
                    ShowInitials();
                    return;
                }

                picAvatar.ImageLocation = null;
                picAvatar.Image = value;
                picAvatar.Visible = true;
                lblInitials.Visible = false;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            MakeAvatarRound();
        }

        private void MakeAvatarRound()
        {
            var gp = new GraphicsPath();
            gp.AddEllipse(0, 0, pnlAvatar.Width - 1, pnlAvatar.Height - 1);
            pnlAvatar.Region = new Region(gp);
        }

        private void ApplyAvatarFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                ShowInitials();
                return;
            }

            try
            {
                picAvatar.Image = null;
                picAvatar.ImageLocation = url;
                picAvatar.LoadAsync();
            }
            catch
            {
                ShowInitials();
            }
        }

        private void PicAvatar_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null && picAvatar.Image != null)
            {
                picAvatar.Visible = true;
                lblInitials.Visible = false;
            }
            else
            {
                ShowInitials();
            }
        }

        private void ShowInitials()
        {
            picAvatar.ImageLocation = null;
            picAvatar.Image = null;
            picAvatar.Visible = false;
            lblInitials.Visible = true;
            lblInitials.BringToFront();
        }

        private string GetInitialsFromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "??";

            var parts = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "??";

            if (parts.Length == 1)
                return parts[0].Substring(0, 1).ToUpper();

            string first = parts[0];
            string last = parts[parts.Length - 1];   // thay cho parts[^1]

            return (first[0].ToString() + last[0].ToString()).ToUpper();
        }
    }
}
