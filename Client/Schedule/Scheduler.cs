using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT106_BT2.Schedule
{
    public partial class Scheduler : Form
    {
        int month, year;
        public Scheduler()
        {
            InitializeComponent();
        }

        private void Scheduler_Load(object sender, EventArgs e)
        {
            displayaDay();
        }
        private void LoadCalendar(int month, int year)
        {
            string monthname = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            LBDate.Text = $"{monthname} {year}";
        }
        private void displayaDay()
        {
            DateTime now = DateTime.Now;
            month = now.Month;
            year = now.Year;

            string monthname = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            LBDate.Text = monthname + "    " + year;



            DateTime startofthemonth = new DateTime(year, month, 1);
            int days = DateTime.DaysInMonth(year, month);
            int daysoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d"));

            for (int i = 1; i < daysoftheweek; i++)
            {
                ScheduleUserControl sucBlank = new ScheduleUserControl();
                dayscontainer.Controls.Add(sucBlank);

            }
            for (int i = 1; i <= days; i++)
            {
                UCDays ucdays = new UCDays();
                ucdays.days(i);
                dayscontainer.Controls.Add(ucdays);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dayscontainer.Controls.Clear();

            month--;
            if (month < 1)
            {
                month = 12;
                year--;
            }
            LoadCalendar(month, year);
            DateTime startofthemonth = new DateTime(year, month, 1);
            int days = DateTime.DaysInMonth(year, month);
            int daysoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d"));

            for (int i = 1; i < daysoftheweek; i++)
            {
                ScheduleUserControl sucBlank = new ScheduleUserControl();
                dayscontainer.Controls.Add(sucBlank);

            }
            for (int i = 1; i <= days; i++)
            {
                UCDays ucdays = new UCDays();
                ucdays.days(i);
                dayscontainer.Controls.Add(ucdays);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dayscontainer.Controls.Clear();

            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }
            LoadCalendar(month, year);
            DateTime startofthemonth = new DateTime(year, month, 1);
            int days = DateTime.DaysInMonth(year, month);
            int daysoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d"));

            for (int i = 1; i < daysoftheweek; i++)
            {
                ScheduleUserControl sucBlank = new ScheduleUserControl();
                dayscontainer.Controls.Add(sucBlank);

            }
            for (int i = 1; i <= days; i++)
            {
                UCDays ucdays = new UCDays();
                ucdays.days(i);
                dayscontainer.Controls.Add(ucdays);
            }
        }
    }
}
