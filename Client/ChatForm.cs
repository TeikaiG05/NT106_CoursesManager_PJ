using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class ChatForm : Form
    {
        public ChatForm()
        {
            InitializeComponent();
            this.Text = "Chat App";
            this.Load += Form_Load;
        }

        private void Form_Load(object sender, EventArgs e)
        {

        }

        private void nutDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nutNav_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button nut = sender as Guna.UI2.WinForms.Guna2Button;
        }

        private void nutGui_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(oNhapTin.Text))
            {
                string tin = oNhapTin.Text;
                Label tinMoi = new Label();
                tinMoi.Text = tin;
                tinMoi.AutoSize = true;
                tinMoi.MaximumSize = new Size(khungTinNhan.Width - 20, 0);
                tinMoi.Padding = new Padding(5);
                tinMoi.Margin = new Padding(5);
                tinMoi.BackColor = Color.LightBlue;
                tinMoi.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                khungTinNhan.Controls.Add(tinMoi);
                khungTinNhan.Controls.SetChildIndex(tinMoi, 0);

                khungTinNhan.ScrollControlIntoView(tinMoi);

                oNhapTin.Clear();
            }
        }
    }
}
