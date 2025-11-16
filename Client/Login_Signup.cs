using Common;
using Guna.UI2.AnimatorNS;
using Guna.UI2.WinForms;
using NT106_BT2.Notifications;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static NT106_BT2.Notifications.ToastNotification;


namespace NT106_BT2
{
    public partial class Login_Signup : Form
    {
        private Guna2Transition Guna2Transistion1;
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private static readonly string SessPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"NT106_Exercise3", "session.json");

        private static readonly string[] MONTHS = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private bool initDone = false;
        public Login_Signup()
        {
            InitializeComponent();
            EnableDrag(pnTitleBar);
            Guna2Transistion1 = new Guna2Transition();
            pn_login.Visible = true;
            pn_regis.Visible = false;
            this.Shown += async (_, __) =>
            {
                await TcpHelper.ConnectAsync(); // 🔹 Kết nối tới server khi app mở
                TcpHelper.OnError += msg => Invoke(new Action(() =>
                    NotificationManager.Show(this, msg, ToastNotification.ToastType.Error)));

                TcpHelper.OnMessageReceived += msg => HandleServerMessage(msg); // 🔹 Đăng ký sự kiện nhận phản hồi
                await TryAutoLoginAsync();
            };
        }
        private void HandleServerMessage(string msg)
        {
            try
            {
                var ok = Newtonsoft.Json.JsonConvert.DeserializeObject<OkRes>(msg);
                var err = ok == null ? Newtonsoft.Json.JsonConvert.DeserializeObject<ErrRes>(msg) : null;

                if (ok != null && ok.ok)
                {
                    Invoke(new Action(() =>
                    {
                        switch (ok.type)
                        {
                            case Common.MsgType.LOGIN:
                                var u = ok.user ?? new Common.UserDto();

                                // Lưu session
                                Session.Email = u.email;
                                Session.Token = ok.token;
                                Session.Expire = ok.expires;
                                Session.Role = u.role;
                                Session.FullName = u.fullName;

                                if (tsRemember.Checked)
                                {
                                    SaveRememberToSettings(u.email, ok.token);
                                    SaveSessionToDisk(u.email, ok.token);
                                }
                                else
                                {
                                    ClearSavedSession();
                                }

                                SplitName(u.fullName, out var first, out var sur);
                                NotificationManager.Show(this, "Đăng nhập thành công!", ToastNotification.ToastType.Success);
                                ShowDashboardModal(first, sur, u.birthday ?? "", u.gender ?? "Other", u.email ?? "");
                                break;

                            case Common.MsgType.REGISTER:
                                NotificationManager.Show(this, "Đăng ký thành công!", ToastNotification.ToastType.Success);
                                cToLogin_Click(null, EventArgs.Empty);
                                ClearSignupFields();
                                break;

                            default:
                                Console.WriteLine($"📩 Server OK: {ok.type}");
                                break;
                        }
                    }));
                }
                else if (err != null)
                {
                    Invoke(new Action(() =>
                    {
                        NotificationManager.Show(this, err.error, ToastNotification.ToastType.Error);
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Parse error: " + ex.Message);
            }
        }

        #region UI: switch 2 panel
        private void cToLogin_Click(object sender, EventArgs e)
        {
            pn_regis.Visible = false;
            Guna2Transistion1.ShowSync(pn_login);
        }

        private void cSignup_Click(object sender, EventArgs e)
        {
            pn_regis.Visible = true;
            Guna2Transistion1.ShowSync(pn_regis);
        }
        #endregion

        #region Form life-cyclex
        private void Login_Signup_Load(object sender, EventArgs e)
        {
            InitCombos();

            initDone = false; // ĐANG KHỞI TẠO
            tsRemember.Checked = Properties.Settings.Default.RememberMe;
            cUsername.Text = Properties.Settings.Default.SavedEmail ?? "";
            initDone = true;  // KHỞI TẠO XONG
        }
        #endregion

        #region Helpers (validate + ui + session)
        private void InitCombos()
        {
            // Year
            cYear.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int y = currentYear; y >= 1950; y--) cYear.Items.Add(y.ToString());
            cYear.MaxDropDownItems = 5; cYear.DropDownHeight = 120; cYear.SelectedIndex = 0;

            // Month
            cMonth.Items.Clear();
            foreach (var m in MONTHS) cMonth.Items.Add(m);
            cMonth.MaxDropDownItems = 5; cMonth.DropDownHeight = 120; cMonth.SelectedIndex = 0;

            // Day
            cDay.Items.Clear();
            for (int d = 1; d <= 31; d++) cDay.Items.Add(d.ToString());
            cDay.MaxDropDownItems = 5; cDay.DropDownHeight = 120; cDay.SelectedIndex = 0;
        }

        private static bool HasDigit(string s) => !string.IsNullOrEmpty(s) && s.Any(char.IsDigit);

        private static bool IsValidEmail(string email)
        {
            try { var addr = new System.Net.Mail.MailAddress(email); return addr.Address == email; }
            catch { return false; }
        }

        private static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8) return false;
            bool up = password.Any(char.IsUpper);
            bool lo = password.Any(char.IsLower);
            bool di = password.Any(char.IsDigit);
            bool sp = password.Any(ch => !char.IsLetterOrDigit(ch));
            return up && lo && di && sp;
        }

        private string GetSelectedGender()
        {
            if (cMale.Checked) return "Male";
            if (cFemale.Checked) return "Female";
            if (cOther.Checked) return "Other";

            var toast = new ToastNotification("Vui lòng chọn giới tính.", ToastNotification.ToastType.Warning);
            toast.ShowNotification(this);
            return null;
        }


        private void ClearSignupFields()
        {
            cFirstname.Text = cSurname.Text = cEmail.Text = "";
            nw_password.Text = nw_cfpassword.Text = "";
            cMale.Checked = cFemale.Checked = cOther.Checked = false;
            cYear.SelectedIndex = cMonth.SelectedIndex = cDay.SelectedIndex = 0;
        }

        private static void SplitName(string full, out string first, out string sur)
        {
            first = sur = "";
            if (string.IsNullOrWhiteSpace(full)) return;
            var parts = full.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            first = parts.FirstOrDefault() ?? "";
            sur = parts.Length > 1 ? parts[parts.Length - 1] : first;
        }

        private void SaveSessionToDisk(string email, string token)
        {
            try
            {
                var dir = Path.GetDirectoryName(SessPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(SessPath, Newtonsoft.Json.JsonConvert.SerializeObject(new { email, token }));
            }
            catch { /* ignore */ }
        }

        private static void SaveRememberToSettings(string email, string token)
        {
            Properties.Settings.Default.SavedEmail = email ?? "";
            Properties.Settings.Default.SavedToken = token ?? "";
            Properties.Settings.Default.RememberMe = true;
            Properties.Settings.Default.Save();
        }

        private static void ClearSavedSession()
        {
            Properties.Settings.Default.SavedEmail = string.Empty;
            Properties.Settings.Default.SavedToken = string.Empty;
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.Save();
        }

        private DialogResult ShowDashboardModal(string first, string sur, string bday, string gender, string email)
        {
            this.Hide();
            DialogResult dr;
            using (var main = new Dashboard(first, sur, bday, gender, email))
            {
                dr = main.ShowDialog(this);
            }
            if (dr == DialogResult.OK) this.Show(); else this.Close();
            return dr;
        }
        #endregion

        #region Sign up
        private async void bt_signup_Click(object sender, EventArgs e)
        {
            string firstname = cFirstname.Text.Trim();
            string surname = cSurname.Text.Trim();
            string year = cYear.SelectedItem?.ToString();
            string month = cMonth.SelectedItem?.ToString();
            string day = cDay.SelectedItem?.ToString();
            string gender = GetSelectedGender();
            string email = cEmail.Text.Trim();
            string pass = nw_password.Text;
            string conf = nw_cfpassword.Text;

            if (string.IsNullOrWhiteSpace(firstname) || HasDigit(firstname)) { NotificationManager.Show(this, "Họ trống hoặc chứa số.", ToastNotification.ToastType.Warning); cFirstname.Focus(); return; }
            if (string.IsNullOrWhiteSpace(surname) || HasDigit(surname)) { NotificationManager.Show(this, "Tên trống hoặc chứa số.", ToastNotification.ToastType.Warning); cSurname.Focus(); return; }

            if (!int.TryParse(year, out int y) || !MONTHS.Contains(month) || !int.TryParse(day, out int d))
            { NotificationManager.Show(this, "Vui lòng chọn ngày sinh hợp lệ.", ToastNotification.ToastType.Warning); return; }

            int m = Array.IndexOf(MONTHS, month) + 1;
            DateTime birthdayDt;
            try { birthdayDt = new DateTime(y, m, d); }
            catch { NotificationManager.Show(this, "Ngày sinh không hợp lệ.", ToastNotification.ToastType.Warning); return; }

            if (string.IsNullOrEmpty(gender)) { NotificationManager.Show(this, "Vui lòng chọn giới tính.", ToastNotification.ToastType.Warning); return; }
            if (!IsValidEmail(email)) { NotificationManager.Show(this, "Email không hợp lệ.", ToastNotification.ToastType.Warning); cEmail.Focus(); return; }
            if (!IsStrongPassword(pass)) { NotificationManager.Show(this, "Mật khẩu phải ≥8 ký tự, có hoa, thường, số, ký tự đặc biệt.", ToastNotification.ToastType.Warning); nw_password.Focus(); return; }
            if (pass != conf) { NotificationManager.Show(this, "Mật khẩu xác nhận không khớp.", ToastNotification.ToastType.Warning); nw_cfpassword.Focus(); return; }

            var hashedPass = PasswordHasher.Sha256Hex(pass);

            var req = new RegisterReq
            {
                type = MsgType.REGISTER,
                username = email,
                gender = gender,
                email = email,
                passwordHash = hashedPass,
                fullName = (firstname + " " + surname).Trim(),
                birthday = birthdayDt.ToString("yyyy-MM-dd")
            };

            try
            {
                string jsonReq = Newtonsoft.Json.JsonConvert.SerializeObject(req);
                await TcpHelper.SendLineAsync(jsonReq);
                NotificationManager.Show(this, "Đang gửi yêu cầu đăng ký...", ToastNotification.ToastType.Info);
            }
            catch (Exception ex)
            {
                NotificationManager.Show(this, "Không kết nối được server: " + ex.Message, ToastNotification.ToastType.Error);
            }
        }
        #endregion

        #region Login + Auto-login
        private async void bt_login_Click(object sender, EventArgs e)
        {
            string email = cUsername.Text.Trim();
            string password = cPassword.Text.Trim();

            // Admin (local)
            if (email == "admin" && password == "admin")
            {
                NotificationManager.Show(this, "Xin chào Admin!", ToastNotification.ToastType.Info);
                await Task.Delay(1000);
                Session.Email = "admin@localhost";
                Session.Role = "Admin";
                ShowDashboardModal("Admin", "User", DateTime.Now.ToString("yyyy-MM-dd"), "Other", "admin@localhost");
                return;
            }

            if (!IsValidEmail(email)) { NotificationManager.Show(this, "Email không hợp lệ.", ToastNotification.ToastType.Warning); cUsername.Focus(); return; }
            if (string.IsNullOrEmpty(password)) { NotificationManager.Show(this, "Vui lòng nhập mật khẩu.", ToastNotification.ToastType.Warning); cPassword.Focus(); return; }

            var req = new LoginReq
            {
                type = MsgType.LOGIN,
                username = email,
                passwordHash = PasswordHasher.Sha256Hex(password)
            };

            try
            {
                string jsonReq = Newtonsoft.Json.JsonConvert.SerializeObject(req);
                await TcpHelper.SendLineAsync(jsonReq);

                // phản hồi từ server sẽ được xử lý tự động trong HandleServerMessage()
                NotificationManager.Show(this, "Đang đăng nhập...", ToastNotification.ToastType.Info);
            }
            catch (Exception ex)
            {
                NotificationManager.Show(this, "Không kết nối được server: " + ex.Message, ToastNotification.ToastType.Error);
            }
        }

        private async Task TryAutoLoginAsync()
        {
            if (Session.IsLoggingOut) return;
            var remember = Properties.Settings.Default.RememberMe;
            var savedEmail = Properties.Settings.Default.SavedEmail;
            var savedToken = Properties.Settings.Default.SavedToken;

            if (!remember || string.IsNullOrWhiteSpace(savedEmail) || string.IsNullOrWhiteSpace(savedToken))
                return;

            try
            {
                var req = new TokenLoginReq { username = savedEmail, token = savedToken };
                string jsonReq = Newtonsoft.Json.JsonConvert.SerializeObject(req);
                await TcpHelper.SendLineAsync(jsonReq);

                // ✅ Không cần chờ phản hồi ở đây — phản hồi được xử lý trong HandleServerMessage()
                NotificationManager.Show(this, "Đang đăng nhập lại bằng token...", ToastNotification.ToastType.Info);
            }
            catch (Exception ex)
            {
                NotificationManager.Show(this, "Không thể tự động đăng nhập: " + ex.Message, ToastNotification.ToastType.Error);
            }
        }

        #endregion

        #region Remember toggle
        private void tsRemember_CheckedChanged(object sender, EventArgs e)
        {
            if (!initDone) return;

            Properties.Settings.Default.RememberMe = tsRemember.Checked;

            if (!tsRemember.Checked)
            {
                Session.Clear();
                ClearSavedSession();
            }
            else
            {
                Properties.Settings.Default.Save();
            }
        }
        #endregion

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
        private void nw_password_IRC(object sender, EventArgs e)
        {
            nw_password.UseSystemPasswordChar = !nw_password.UseSystemPasswordChar;
            nw_password.IconRight = nw_password.UseSystemPasswordChar ? Properties.Resources.icons8_eye_close_50 : Properties.Resources.icons8_eye_open_50;
        }

        private void nw_cfpassword_IRC(object sender, EventArgs e)
        {
            nw_cfpassword.UseSystemPasswordChar = !nw_cfpassword.UseSystemPasswordChar;
            nw_cfpassword.IconRight = nw_cfpassword.UseSystemPasswordChar ? Properties.Resources.icons8_eye_close_50 : Properties.Resources.icons8_eye_open_50;
        }

        private void cPassword_IRC(object sender, EventArgs e)
        {
            cPassword.UseSystemPasswordChar = !cPassword.UseSystemPasswordChar;
            cPassword.IconRight = cPassword.UseSystemPasswordChar ? Properties.Resources.icons8_eye_close_50 : Properties.Resources.icons8_eye_open_50;
        }
    }
}
