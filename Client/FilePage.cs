using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class FilePage : Form
    {
        private readonly string roomCode;
        public FilePage(string RoomCode)
        {
            InitializeComponent();
            roomCode = RoomCode;

            this.Load += FilePage_Load;
            btnAnh.Click += BtnAnh_Click;
            btnFileTab.Click += BtnFileTab_Click;
        }

        private void FilePage_Load(object sender, EventArgs e)
        {
            flpMedia.Visible = true;
            flpFiles.Visible = false;

            LoadFilesForRoom();
        }

        private void BtnAnh_Click(object sender, EventArgs e)
        {
            flpMedia.BringToFront();
            flpMedia.Visible = true;
            flpFiles.Visible = false;
        }

        private void BtnFileTab_Click(object sender, EventArgs e)
        {
            flpFiles.BringToFront();
            flpFiles.Visible = true;
            flpMedia.Visible = false;
        }

        private void LoadFilesForRoom()
        {
            flpMedia.Controls.Clear();
            flpFiles.Controls.Clear();

            DataTable dt = DbClient.GetFilesByRoom(roomCode);

            foreach (DataRow row in dt.Rows)
            {
                string filePath = row["FilePath"].ToString();
                string fileName = row["FileName"].ToString();

                if (IsImage(filePath))
                {
                    AddMediaItem(filePath);
                }
                else
                {
                    AddFileItem(filePath, fileName);
                }
            }
        }

        private bool IsImage(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp";
        }

        private void AddMediaItem(string filePath)
        {
            if (!File.Exists(filePath)) return;

            var pic = new PictureBox();
            pic.Width = 120;
            pic.Height = 120;
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            pic.Margin = new Padding(5);

            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    pic.Image = Image.FromStream(fs);
                }
            }
            catch { }

            pic.Cursor = Cursors.Hand;
            pic.Click += (s, e) =>
            {
                using (var f = new ImagePreviewForm(filePath))
                {
                    f.ShowDialog(this);
                }
            };

            flpMedia.Controls.Add(pic);
        }

        private void AddFileItem(string filePath, string fileName)
        {
            if (!File.Exists(filePath)) return;

            FileInfo fi = new FileInfo(filePath);

            Panel row = new Panel();
            row.Height = 50;
            row.Width = flpFiles.ClientSize.Width - 25;
            row.Margin = new Padding(3);
            row.BackColor = Color.White;
            row.BorderStyle = BorderStyle.None;

            PictureBox icon = new PictureBox();
            icon.Width = 32;
            icon.Height = 32;
            icon.Left = 10;
            icon.Top = 9;
            icon.SizeMode = PictureBoxSizeMode.Zoom;
            icon.Image = SystemIcons.Application.ToBitmap();

            Label lblName = new Label();
            lblName.AutoSize = false;
            lblName.Left = 50;
            lblName.Top = 5;
            lblName.Width = row.Width - 60;
            lblName.Text = fileName;
            lblName.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            Label lblSize = new Label();
            lblSize.AutoSize = true;
            lblSize.Left = 50;
            lblSize.Top = 28;
            lblSize.Font = new Font("Segoe UI", 9);
            lblSize.ForeColor = Color.Gray;
            lblSize.Text = FormatSize(fi.Length);

            row.Controls.Add(icon);
            row.Controls.Add(lblName);
            row.Controls.Add(lblSize);

            row.Cursor = Cursors.Hand;
            row.Click += (s, e) => System.Diagnostics.Process.Start(filePath);

            flpFiles.Controls.Add(row);
        }

        private string FormatSize(long bytes)
        {
            double kb = bytes / 1024.0;
            if (kb < 1024)
                return $"{kb:0.##} KB";

            double mb = kb / 1024.0;
            return $"{mb:0.##} MB";
        }
    }
}
