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
    public partial class TeamsCard : UserControl
    {
        public TeamsCard()
        {
            InitializeComponent();
            RegisterClickEvent(this);
        }

        private void RegisterClickEvent(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                c.Click += Child_Click;
                if (c.HasChildren)
                    RegisterClickEvent(c);
            }
        }

        private void Child_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }
        
        public string Mon
        {
            get => lbClassname.Text;
            set => lbClassname.Text = value;
        }
        public string Lop
        {
            get => lbClasscode.Text;
            set => lbClasscode.Text = value;
        }
    }
}
