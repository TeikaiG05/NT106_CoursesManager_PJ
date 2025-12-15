using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class ImagePreviewForm : Form
    {
        private readonly string path;
        public ImagePreviewForm(string imagePath)
        {
            InitializeComponent();
            path = imagePath;
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;

            this.Load += ImagePreviewForm_Load;
            this.KeyDown += ImagePreviewForm_KeyDown;
            this.picPreview.Click += (s, e) => this.Close();
        }

        private void ImagePreviewForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists(path)) return;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                picPreview.Image = Image.FromStream(fs);
            }
        }

        private void ImagePreviewForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
