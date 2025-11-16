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
    public partial class GroupChatForm : Form
    {
        private readonly Dashboard parent;
        private Form currentChildForm;

        public GroupChatForm()
        {
            InitializeComponent();
        }
        public GroupChatForm(string Classname, string Classcode, Dashboard Dparent) : this()
        {
            parent = Dparent;
            lbClasscode.Text = Classcode;
            lbClassname.Text = Classname;

            OpenChildInGroup(new ChatPage(Classcode));
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            parent.OpenChildForm(new TeamsForm(parent));
        }

        private void OpenChildInGroup(Form childForm)
        {
            if (currentChildForm != null)
                currentChildForm.Close();

            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            pnlGroupContainer.Controls.Clear();
            pnlGroupContainer.Controls.Add(childForm);
            childForm.Show();
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            OpenChildInGroup(new ChatPage(lbClasscode.Text));
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            OpenChildInGroup(new FilePage());
        }
    }
}
