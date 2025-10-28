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
    public partial class LogoutForm : Form
    {
        public bool IsConfirmed { get; private set; } = false;

        public LogoutForm()
        {
            InitializeComponent();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            IsConfirmed = true;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }

        private void LogoutForm_Load(object sender, EventArgs e)
        {

        }
    }
}
