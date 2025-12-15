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

            lbName.Text = name;
            tbBirthday.Text = birthday;
            tbGender.Text = gender;
            tbEmail.Text = email;
            string role = Session.Role;

            #region Set Role Text
            if (Session.Email == "admin@localhost")
                tbRole.Text = "Admin";
            else if (string.Equals(Session.Role, "Owner", StringComparison.OrdinalIgnoreCase))
                tbRole.Text = "Teacher";
            else
                tbRole.Text = "Student";
            #endregion
        }
    }
}
