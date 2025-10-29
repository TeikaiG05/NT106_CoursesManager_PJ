using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace NT106_BT2
{
    public partial class Dashboard : Form
    {
        private readonly string email;
        private Form currentFormChild;
        private string fullName;
        private string birthday;
        private string gender;
        public Dashboard(string firstname, string surname, string birthday, string gender, string email)
        {
            InitializeComponent();
            this.fullName = firstname + " " + surname;
            this.birthday = birthday;
            this.gender = gender;
            this.email = email;
            this.Load += Dashboard_Load;
        }

        private void lb_profile_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Profile(fullName, birthday, gender, email));
        }
        #region Logout
        private async void btnLogout_Click(object sender, EventArgs e)
        {
            Session.IsLoggingOut = true;
            await TcpHelper.LogoutAsync(Session.Email, Session.Token);
            Session.Clear();
            this.Hide();
            new Login_Signup().Show();
        }
        #endregion

        private void btnChat_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ChatForm());
            //ChatForm chatWindow = new ChatForm();
            //chatWindow.StartPosition = FormStartPosition.Manual;
            //int x = this.Location.X + (this.Width - chatWindow.Width) / 2;
            //int y = this.Location.Y + (this.Height - chatWindow.Height) / 2;
            //chatWindow.Location = new Point(x, y);
            //chatWindow.Show();
        }
        private async void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // 🔒 Chỉ logout nếu đang đăng nhập và chưa gọi logout trước đó
                if (!Session.IsLoggingOut && Session.Email != null && Session.Token != null)
                {
                    Session.IsLoggingOut = true;
                    await TcpHelper.LogoutAsync(Session.Email, Session.Token);
                }
            }
            catch
            {
                // bỏ qua lỗi khi tắt app
            }
            finally
            {
                TcpHelper.Disconnect(); // 🔌 ngắt kết nối gọn gàng
                Application.Exit();
            }
        }
        private void OpenChildForm(Form childForm)
        {
            // Nếu đã có form con đang mở -> đóng lại
            if (currentFormChild != null)
                currentFormChild.Close();

            currentFormChild = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            guna2ContainerControl1.Controls.Clear(); // Xóa control cũ trong container
            guna2ContainerControl1.Controls.Add(childForm); // Thêm form mới vào container
            guna2ContainerControl1.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }
        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            OpenChildForm(new Profile(fullName, birthday, gender, email));
            lb_profile.Checked = true;
        }
    }
}
