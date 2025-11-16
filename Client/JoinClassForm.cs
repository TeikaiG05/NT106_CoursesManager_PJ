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
    public partial class JoinClassForm : Form
    {
        public JoinClassForm()
        {
            InitializeComponent();
            this.AcceptButton = btnJoin;
            this.CancelButton = btnCancel;
        }

        public string ClassCode => tbClasscode.Text.Trim();

        private void btnJoin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ClassCode))
            {
                MessageBox.Show("Vui lòng nhập mã lớp.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
