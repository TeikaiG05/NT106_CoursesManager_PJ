using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class MembersForm : Form
    {
        // Constructor dùng khi gọi ShowChild(new MembersForm(roomCode))
        public MembersForm(string roomCode)
        {
            InitializeComponent();

            // form UI tweaks
            Text = string.IsNullOrEmpty(roomCode) ? "Group Members" : $"Members of {roomCode}";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None; // phù hợp để ShowChild (embedded)
            // Configure ListView (lvMember được tạo bởi Designer)
            ConfigureListView();

            
            btnClose.Text = "Close";
            btnClose.Click += (s, e) => Close();

            // bắt đầu tải dữ liệu bất đồng bộ
            LoadMembersAsync(roomCode);
        }

        
        public MembersForm(IEnumerable<MemberDto> members, string roomCode = null) : this(roomCode ?? "")
        {
            // nếu đã truyền members trực tiếp, bỏ qua request server
            if (members != null && members.Any())
            {
                PopulateList(members);
            }
        }

        private void ConfigureListView()
        {
            lvMember.View = View.Details;
            lvMember.FullRowSelect = true;
            lvMember.GridLines = true;
            lvMember.MultiSelect = false;
            if (lvMember.Columns.Count == 0)
            {
                lvMember.Columns.Add("Email", 300);
                lvMember.Columns.Add("Full name", 260);
                lvMember.Columns.Add("Role", 100);
            }
        }

        private async void LoadMembersAsync(string roomCode)
        {
            if (string.IsNullOrWhiteSpace(roomCode))
                return;

            try
            {
                // gọi helper để request server
                var members = await GroupMembersHelper.RequestMembersAsync(roomCode);
                PopulateList(members ?? Enumerable.Empty<MemberDto>());
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                    MessageBox.Show(this, "Không thể tải danh sách thành viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateList(IEnumerable<MemberDto> members)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IEnumerable<MemberDto>>(PopulateList), members);
                return;
            }

            lvMember.BeginUpdate();
            try
            {
                lvMember.Items.Clear();
                foreach (var m in members)
                {
                    var it = new ListViewItem(m.email ?? "");
                    it.SubItems.Add(m.fullName ?? "");
                    it.SubItems.Add(m.role ?? "");
                    lvMember.Items.Add(it);
                }
            }
            finally
            {
                lvMember.EndUpdate();
            }
        }

        // Optional helper: lấy thành viên được chọn
        public MemberDto GetSelectedMember()
        {
            if (lvMember.SelectedItems.Count == 0) return null;
            var it = lvMember.SelectedItems[0];
            return new MemberDto
            {
                email = it.SubItems[0].Text,
                fullName = it.SubItems[1].Text,
                role = it.SubItems[2].Text
            };
        }
    }
}