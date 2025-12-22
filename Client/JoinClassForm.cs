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
    /// <summary>
    /// ============================================================================
    /// JoinClassForm.cs - Form input để tham gia lớp bằng mã code
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Hiển thị dialog nhập mã lớp
    /// Validate input
    /// Trả về code đã nhập hoặc cancel
    /// 
    /// CÔNG VIỆC CỤ THể:
    /// 
    /// 1. Constructor: JoinClassForm()
    ///    - Gọi InitializeComponent()
    ///    - Set AcceptButton = btnJoin (enter → join)
    ///    - Set CancelButton = btnCancel (esc → cancel)
    /// 
    /// 2. Thuộc tính: ClassCode
    ///    - Property read-only
    ///    - Trả về tbClasscode.Text.Trim()
    /// 
    /// 3. btnJoin_Click(object sender, EventArgs e)
    ///    - Validate: ClassCode phải không trống
    ///    - Nếu trống: MessageBox.Show("Vui lòng nhập mã lớp")
    ///    - Nếu OK: DialogResult = DialogResult.OK → Close()
    /// 
    /// 4. btnCancel_Click(object sender, EventArgs e)
    ///    - DialogResult = DialogResult.Cancel → Close()
    /// 
    /// USAGE PATTERN:
    /// ```csharp
    /// using (var frm = new JoinClassForm())
    /// {
    ///     if (frm.ShowDialog() == DialogResult.OK)
    ///     {
    ///         string code = frm.ClassCode;
    ///         // Xử lý join
    ///     }
    /// }
    /// ```
    /// 
    /// ============================================================================
    /// </summary>
    public partial class JoinClassForm : Form
    {
        public string ClassCode => tbClasscode.Text.Trim();
        public JoinClassForm()
        {
            InitializeComponent();
            this.AcceptButton = btnJoin;
            this.CancelButton = btnCancel;
        }

        #region Join Button
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
