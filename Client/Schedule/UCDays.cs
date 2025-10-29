using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT106_BT2.Schedule
{
    public partial class UCDays : UserControl
    {
        public UCDays()
        {
            InitializeComponent();
        }

        private void IBdays_Click(object sender, EventArgs e)
        {

        }
        public void days(int numday)
        {
            IBdays.Text = numday + "";
        }
    }
}
