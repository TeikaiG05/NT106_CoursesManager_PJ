using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NT106_BT2
{
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
    }
}
