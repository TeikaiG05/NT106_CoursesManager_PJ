using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NT106_BT2
{
    /// <summary>
    /// ============================================================================
    /// GroupChatForm.cs - Giao diện chat nhóm trong 1 lớp học
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Quản lý giao diện nhóm chat (container form)
    /// Hiển thị thông tin lớp (tên, code, avatar)
    /// Chuyển đổi giữa các trang: Chat, File, Members
    /// Cache ChatPage để không phải tạo lại
    /// 
    /// CÔNG VIỆC CỤ THể:
    /// 
    /// 1. GroupChatForm() - Constructor mặc định
    ///    - Gọi InitializeComponent()
    /// 
    /// 2. GroupChatForm(string className, string classCode, Image avatar, Dashboard dashboardParent)
    ///    - Constructor có tham số
    ///    - Lưu reference tới parent Dashboard
    ///    - Set lbClassname.Text = className
    ///    - Set lbClasscode.Text = classCode
    ///    - Set picAvatar.Image = avatar (nếu có)
    ///    - Mở ChatPage mặc định
    /// 
    /// 3. btnBack_Click(object sender, EventArgs e)
    ///    - Quay lại trang Teams
    ///    - Gọi parent.OpenChildForm(new TeamsForm(parent))
    /// 
    /// 4. btnChat_Click(object sender, EventArgs e)
    ///    - Mở ChatPage của lớp này
    ///    - Gọi OpenChatPage(lbClasscode.Text)
    /// 
    /// 5. btnFile_Click(object sender, EventArgs e)
    ///    - Mở FilePage (trang xem/tải file)
    ///    - Gọi ShowChild(new FilePage(roomCode))
    /// 
    /// 6. butMember_Click(object sender, EventArgs e)
    ///    - Mở MembersForm (danh sách thành viên)
    ///    - Gọi ShowChild(new MembersForm(roomCode))
    /// 
    /// 7. OpenChatPage(string roomCode)
    ///    - Kiểm tra trong dictionary chatPages
    ///    - Nếu chưa có: tạo ChatPage mới, thêm vào dict
    ///    - Nếu có rồi: lấy từ dict (reuse)
    ///    - Gọi ShowChild() để hiển thị
    /// 
    /// 8. ShowChild(Form child)
    ///    - Ẩn form hiện tại (currentChildForm)
    ///    - Cập nhật currentChildForm = child
    ///    - Set child properties:
    ///      * TopLevel = false (không phải window riêng)
    ///      * FormBorderStyle = FormBorderStyle.None (không border)
    ///      * Dock = DockStyle.Fill (fill toàn panel)
    ///    - Clear pnlGroupContainer.Controls
    ///    - Thêm child vào container
    ///    - Gọi child.Show()
    /// 
    /// BIẾN TOÀN CỤC:
    /// - parent: Dashboard reference để quay lại
    /// - currentChildForm: Form con đang hiển thị
    /// - chatPages: Dictionary<string, ChatPage> để cache pages
    /// 
    /// KIẾN TRÚC:
    /// GroupChatForm là MDI (Multiple Document Interface)
    /// - pnlGroupContainer chứa các form con (ChatPage, FilePage, MembersForm)
    /// - Từng form con là child form không có border
    /// 
    /// ============================================================================
    /// </summary>
    public partial class GroupChatForm : Form
    {
        private readonly Dashboard parent;
        private Form currentChildForm;
        private readonly Dictionary<string, ChatPage> chatPages = new Dictionary<string, ChatPage>();

        public GroupChatForm()
        {
            InitializeComponent();
        }

        public GroupChatForm(string className, string classCode, Image avatar, Dashboard dashboardParent) : this()
        {
            parent = dashboardParent;
            lbClassname.Text = className;
            lbClasscode.Text = classCode;

            if (avatar != null)
                picAvatar.Image = avatar;

            OpenChatPage(classCode);
        }

        #region Button Clicks
        private void btnBack_Click(object sender, EventArgs e)
        {
            parent.OpenChildForm(new TeamsForm(parent));
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            OpenChatPage(lbClasscode.Text);
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            string roomCode = lbClasscode.Text;
            ShowChild(new FilePage(roomCode));
        }
        #endregion

        #region Form Events
        private void OpenChatPage(string roomCode)
        {
            ChatPage page;

            if (!chatPages.ContainsKey(roomCode))
            {
                page = new ChatPage(roomCode);
                chatPages.Add(roomCode, page);
            }
            else
            {
                page = chatPages[roomCode];
            }
            
            ShowChild(page);
        }
        #endregion

        #region Helper Methods
        private void ShowChild(Form child)
        {
            if (currentChildForm != null)
                currentChildForm.Hide();

            currentChildForm = child;

            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;

            pnlGroupContainer.Controls.Clear();
            pnlGroupContainer.Controls.Add(child);

            child.Show();
        }
        #endregion

        private void butMember_Click(object sender, EventArgs e)
        {
            string roomCode = lbClasscode.Text;
            ShowChild(new MembersForm(roomCode) );
        }
    }
}
