using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace NT106_BT2
{
    public partial class Dashboard : Form
    {
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
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

            Application.Restart(); // khởi động lại app từ đầu (quay về Login)
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
            }
            finally
            {
                TcpHelper.Disconnect();
                Application.Exit();
            }
        }
        public void OpenChildForm(Form childForm)
        {
            if (currentFormChild != null)
                currentFormChild.Close();

            currentFormChild = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            guna2ContainerControl1.Controls.Clear();
            guna2ContainerControl1.Controls.Add(childForm);
            guna2ContainerControl1.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            OpenChildForm(new Profile(fullName, birthday, gender, email));
            lb_profile.Checked = true;
            EnableDrag(pnTitleBar);
        }
        #region Move form
        private void EnableDrag(Control dragArea)
        {
            dragArea.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }
            };
        }
        #endregion

        private void btnTeams_Click(object sender, EventArgs e)
        {
            OpenChildForm(new TeamsForm(this));
        }
    }
}
