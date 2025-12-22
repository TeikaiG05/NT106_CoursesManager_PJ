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
    /// CreateClassForm.cs - Form input để tạo lớp mới
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Hiển thị dialog nhập tên lớp và mã code
    /// Validate input
    /// Trả về tên + code hoặc cancel
    /// 
    /// CÔNG VIỆC CỤ THể:
    /// 
    /// 1. Constructor: CreateClassForm()
    ///    - Gọi InitializeComponent()
    ///    - Set AcceptButton = btnCreate (enter → create)
    ///    - Set CancelButton = btnCancel (esc → cancel)
    /// 
    /// 2. Thuộc tính: ClassName
    ///    - Property read-only
    ///    - Trả về tbClassname.Text.Trim()
    /// 
    /// 3. Thuộc tính: ClassCode
    ///    - Property read-only
    ///    - Trả về tbClasscode.Text.Trim()
    /// 
    /// 4. btnCreate_Click(object sender, EventArgs e)
    ///    - Validate: ClassName và ClassCode đều không trống
    ///    - Nếu sai: MessageBox.Show("Vui lòng nhập đầy đủ...")
    ///    - Nếu OK: DialogResult = DialogResult.OK → Close()
    /// 
    /// 5. btnCancel_Click(object sender, EventArgs e)
    ///    - DialogResult = DialogResult.Cancel → Close()
    /// 
    /// USAGE PATTERN:
    /// ```csharp
    /// using (var frm = new CreateClassForm())
    /// {
    ///     if (frm.ShowDialog() == DialogResult.OK)
    ///     {
    ///         string name = frm.ClassName;
    ///         string code = frm.ClassCode;
    ///         // Xử lý tạo lớp
    ///     }
    /// }
    /// ```
    /// 
    /// ============================================================================
    /// </summary>
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
