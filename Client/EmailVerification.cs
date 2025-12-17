using Common;
using Newtonsoft.Json;
using System;
using System.Net.Mail;
using System.Net;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class EmailVerification : Form
    {
        public EmailVerification()
        {
            InitializeComponent();
        }

        // Gửi yêu cầu server tạo & gửi OTP
        private async void gunabutguima_Click(object sender, EventArgs e)
        {
            try
            {
                string email = gunatextmailxacminh.Text.Trim();
                if (string.IsNullOrWhiteSpace(email)) { MessageBox.Show("Nhập email"); return; }

                var req = new { type = MsgType.RESET_REQUEST, email };
                await TcpHelper.SendLineAsync(Newtonsoft.Json.JsonConvert.SerializeObject(req));
                MessageBox.Show("Yêu cầu đã gửi. Nếu email tồn tại, bạn sẽ nhận được OTP.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi yêu cầu: " + ex.Message);
            }
        }

        // Gửi OTP + mật khẩu mới tới server (server xác thực OTP rồi cập nhật DB)
        private async void gunabutxacminh_Click(object sender, EventArgs e)
        {
            try
            {
                string email = gunatextmailxacminh.Text.Trim();
                string otp = gunatextotp.Text.Trim();
                string pass = gunatextmatkhaumoi.Text;
                string pass2 = gunatextxacnhanmatkhau.Text;

                if (pass != pass2) { MessageBox.Show("Mật khẩu nhập lại không khớp."); return; }
                if (string.IsNullOrWhiteSpace(otp)) { MessageBox.Show("Nhập OTP."); return; }
                if (!Login_Signup.IsStrongPassword(pass)) { MessageBox.Show("Mật khẩu yếu."); return; }

                var req = new
                {
                    type = MsgType.RESET_CONFIRM,
                    email,
                    otp,
                    passwordHash = PasswordHasher.Sha256Hex(pass)
                };

                await TcpHelper.SendLineAsync(Newtonsoft.Json.JsonConvert.SerializeObject(req));
                MessageBox.Show("Yêu cầu đổi mật khẩu đã gửi. Kiểm tra thông báo từ server.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi yêu cầu: " + ex.Message);
            }
        }

       
    }
}
