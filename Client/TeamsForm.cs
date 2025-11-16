using Guna.UI2.WinForms;
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
    public partial class TeamsForm : Form
    {
        private readonly Dashboard parent;

        public TeamsForm()
        {
            InitializeComponent();
            this.Load += TeamsForm_Load;
        }

        private void TeamsForm_Load(object sender, EventArgs e)
        {
            LoadTeamsFromDb();
        }
        private void LoadTeamsFromDb()
        {
            flowTeams.Controls.Clear();

            if (string.IsNullOrEmpty(Session.Email))
                return;

            var dt = DbClient.GetClassesByUser(Session.Email);

            foreach (System.Data.DataRow row in dt.Rows)
            {
                string mon = row["Name"].ToString();
                string lop = row["Code"].ToString();
                AddCourseCard(mon, lop);
            }
        }
        public TeamsForm(Dashboard Dparent) : this()
        {
            parent = Dparent;
            bool canCreate = Session.Email == "admin@localhost" || string.Equals(Session.Role, "Owner", StringComparison.OrdinalIgnoreCase);
            tsmiCreateTeam.Visible = canCreate;
        }   

        private void btnJoinOrCreate_Click(object sender, EventArgs e)
        {
            var p = new Point(0, btnJoinOrCreate.Height);
            ContextMenu.Show(btnJoinOrCreate, p);
        }

        private void tsmiCreateTeam_Click(object sender, EventArgs e)
        {
            bool canCreate = Session.Email == "admin@localhost" || string.Equals(Session.Role, "Owner", StringComparison.OrdinalIgnoreCase);
            if (!canCreate)
            {
                MessageBox.Show("Chỉ Admin và Teacher mới có quyền tạo nhóm.", "Không đủ quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var f = new CreateClassForm())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    string tenLop = f.ClassName;
                    string maLop = f.ClassCode;

                    try
                    {
                        DbClient.InsertClass(tenLop, maLop, Session.Email);
                        AddCourseCard(tenLop, maLop);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không tạo được lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void AddCourseCard(string mon, string lop)
        {
            var card = new TeamsCard();
            card.Mon = mon;
            card.Lop = lop;
            card.Margin = new Padding(5);
            card.Click += (s, e) =>
            {
                parent.OpenChildForm(new GroupChatForm(mon, lop, parent));
            };
            flowTeams.Controls.Add(card);
        }

        private void tsmiJoinTeam_Click(object sender, EventArgs e)
        {
            using (var f = new JoinClassForm())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    string code = f.ClassCode;

                    try
                    {
                        var result = DbClient.JoinClassByCode(code, Session.Email, "Student");

                        if (result == null)
                        {
                            MessageBox.Show("Không tìm thấy lớp với mã này.", "Sai mã lớp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var info = result.Value;
                        AddCourseCard(info.Name, info.Code);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể join lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
