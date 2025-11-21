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
    public partial class CreateClassForm : Form
    {
        public string ClassName => tbClassname.Text.Trim();
        public string ClassCode => tbClasscode.Text.Trim();

        #region Constructor
        public CreateClassForm()
        {
            InitializeComponent();
            this.AcceptButton = btnCreate;
            this.CancelButton = btnCancel;
        }
        #endregion

        #region Create Button
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ClassName) || string.IsNullOrWhiteSpace(ClassCode))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên lớp và Mã lớp.", "Thiếu thông tin",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region Cancel Button
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion
    }
}
