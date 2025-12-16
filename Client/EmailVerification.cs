using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class EmailVerification : Form
    {
        private string otp_xacminh;
        private DateTime otp_hethan;

        private const string Myemail = "dat23520258@gmail.com";
        private const string Apppassword = "qbwp mrof bhub zeyh";
        public EmailVerification()
        {
            InitializeComponent();
        }

        private void gunabutguima_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(gunatextmailxacminh.Text))
            {
                MessageBox.Show("Vui lòng nhập email!");
                return;
            }


            try
            {
                // 1. Sinh OTP
                otp_xacminh = GenerateOtp();
                otp_hethan = DateTime.Now.AddMinutes(5);


                // 2. Gửi email
                SendOtpEmail(gunatextmailxacminh.Text.Trim(), otp_xacminh);


                MessageBox.Show("Đã gửi mã xác minh tới email!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi email: " + ex.Message);
            }
        }

        private void gunabutxacminh_Click(object sender, EventArgs e)
        {
            if (DateTime.Now > otp_hethan)
            {
                MessageBox.Show("Mã đã hết hạn!");
                return;
            }


            if (gunatextotp.Text.Trim() == otp_xacminh)
            {
                MessageBox.Show("Xác minh email thành công ");
            }
            else
            {
                MessageBox.Show("Mã xác minh không đúng!");
            }
        }

        // ===== HÀM SINH OTP =====
        private string GenerateOtp()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString(); // 6 số
        }


        // ===== HÀM GỬI EMAIL =====
        private void SendOtpEmail(string toEmail, string otp)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(Myemail);
            mail.To.Add(toEmail);
            mail.Subject = "Xác minh email";
            mail.Body = $"Mã xác minh của bạn là: {otp}\nMã có hiệu lực trong 5 phút.";


            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(Myemail, Apppassword);
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}
