using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NT106_BT2
{
    public partial class GroupChatForm : Form
    {
        private readonly Dashboard parent;
        private Form currentChildForm;

        // Cache mỗi chat page theo room
        private readonly Dictionary<string, ChatPage> chatPages = new Dictionary<string, ChatPage>();

        public GroupChatForm()
        {
            InitializeComponent();
        }

        public GroupChatForm(string className, string classCode, Dashboard dashboardParent) : this()
        {
            parent = dashboardParent;
            lbClassname.Text = className;
            lbClasscode.Text = classCode;

            OpenChatPage(classCode);
        }

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
            ShowChild(new FilePage());
        }

        // -----------------------------------------
        // CHỈ SỬA 2 HÀM DƯỚI LÀ FIX TOÀN BỘ LỖI
        // -----------------------------------------

        private void OpenChatPage(string roomCode)
        {
            ChatPage page;

            // Tạo 1 lần duy nhất
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

        private void ShowChild(Form child)
        {
            if (currentChildForm != null)
                currentChildForm.Hide();  // Không Close() nữa!!

            currentChildForm = child;

            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;

            pnlGroupContainer.Controls.Clear();
            pnlGroupContainer.Controls.Add(child);

            child.Show();
        }
    }
}
