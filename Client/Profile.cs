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
    public partial class Profile : Form
    {
        public Profile(string name, string birthday, string gender, string email)
        {
            InitializeComponent();

            // Giả sử bạn có các control như Label hoặc TextBox
            lbName.Text = name;
            tbBirthday.Text = birthday;
            tbGender.Text = gender;
            tbEmail.Text = email;
        }
    }
}
