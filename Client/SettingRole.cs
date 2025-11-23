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
    public partial class SettingRole : Form
    {
        public SettingRole()
        {
            InitializeComponent();
            // Load bảng user khi mở form
            LoadUserList();

            // Add items vào combobox
            comborole.Items.Add("Student");
            comborole.Items.Add("Teacher");

            // Gán sự kiện
            maildata.CellClick += maildata_CellClick;
            butsave.Click += butsave_Click;
        }
        private void LoadUserList()
        {
            maildata.DataSource = DbClient.GetAllUsers();
        }
        private void maildata_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = maildata.Rows[e.RowIndex];

                mailrole.Text = row.Cells["Email"].Value.ToString();
                comborole.SelectedItem = row.Cells["Role"].Value.ToString();
            }
        }

        private void butsave_Click(object sender, EventArgs e)
        {
            string email = mailrole.Text.Trim();
            string role = comborole.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng chọn user trong bảng!");
                return;
            }

            if (string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Vui lòng chọn Role!");
                return;
            }

            DbClient.UpdateUserRole(email, role);

            MessageBox.Show("Cập nhật quyền thành công!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadUserList(); // reload bảng sau khi cập nhật
        }

        private void searchbox_TextChanged(object sender, EventArgs e)
        {
            string keyword = searchbox.Text.Trim().ToLower();

            DataTable dt = DbClient.GetAllUsers();

            var filtered = dt.AsEnumerable()
                             .Where(row => row.Field<string>("Email")
                             .ToLower().Contains(keyword));

            if (filtered.Any())
                maildata.DataSource = filtered.CopyToDataTable();
            else
                maildata.DataSource = null;
        }

        private void SettingRole_Load(object sender, EventArgs e)
        {

        }
    }
}
