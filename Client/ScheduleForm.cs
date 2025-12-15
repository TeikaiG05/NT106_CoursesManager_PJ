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


    public partial class ScheduleForm : Form
    {
        private DateTime currentMonth = DateTime.Today;
        private DateTime selectedDate = DateTime.Today;

        public class StudentTask
        {
            public string Title { get; set; }
            public bool Completed { get; set; }
            public DateTime Date { get; set; }
        }
        public ScheduleForm()
        {
            InitializeComponent();

            currentMonth = DateTime.Today;
            selectedDate = DateTime.Today;

            TaskStorage.LoadFromFile();   

            SetupListView();
            DrawCalendar();
            LoadTasks();
        }
        private void DrawCalendar()
        {
            panelCalendar.Controls.Clear();
            lblMonthYear.Text = currentMonth.ToString("MMMM yyyy");

            DateTime firstDay = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            int offset = (int)firstDay.DayOfWeek;
            DateTime startDate = firstDay.AddDays(-offset);

            int cols = 7;
            int rows = 6;
            int w = panelCalendar.Width / cols;
            int h = panelCalendar.Height / rows;

            for (int i = 0; i < 42; i++)
            {
                DateTime date = startDate.AddDays(i);

                Button btn = new Button
                {
                    Width = w - 4,
                    Height = h - 4,
                    Left = (i % cols) * w,
                    Top = (i / cols) * h,
                    Text = date.Day.ToString(),
                    Tag = date,
                    TextAlign = ContentAlignment.TopLeft
                };

                if (date.Date == DateTime.Today)
                    btn.BackColor = Color.LightGreen;
                else if (date.Date == selectedDate)
                    btn.BackColor = Color.LightBlue;
                else if (date.Month != currentMonth.Month)
                    btn.BackColor = Color.LightGray;
                else
                    btn.BackColor = Color.White;

                if (TaskStorage.GetTasks(date).Count > 0)
                    btn.Text += "\n●";

                btn.Click += Day_Click;

                panelCalendar.Controls.Add(btn);
            }
        }

        private void Day_Click(object sender, EventArgs e)
        {
            selectedDate = (DateTime)((Button)sender).Tag;
            DrawCalendar();
            LoadTasks();
        }

        private void Day_DoubleClick(object sender, EventArgs e)
        {
            selectedDate = (DateTime)((Button)sender).Tag;
            LoadTasks();
            txtTask.Focus();
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            currentMonth = DateTime.Today;
            selectedDate = DateTime.Today;
            DrawCalendar();
            LoadTasks();
        }
        private void SetupListView()
        {
            lvTasks.View = View.Details;
            lvTasks.CheckBoxes = true;
            lvTasks.Columns.Add("Task", 250);
        }

        private void LoadTasks()
        {
            lvTasks.Items.Clear();

            foreach (var task in TaskStorage.GetTasks(selectedDate))
            {
                lvTasks.Items.Add(new ListViewItem(task));
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTask.Text))
                return;

            TaskStorage.AddTask(selectedDate, txtTask.Text);

            txtTask.Clear();
            LoadTasks();
            DrawCalendar();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvTasks.CheckedItems.Count == 0)
                return;
            for (int i = lvTasks.CheckedItems.Count - 1; i >= 0; i--)
            {
                ListViewItem item = lvTasks.CheckedItems[i];
                string task = item.Text;

                TaskStorage.RemoveTask(selectedDate, task);
            }

            LoadTasks();
            DrawCalendar();
        }
        private void lvTasks_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            e.Item.Checked = e.IsSelected;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int month = currentMonth.Month;
            int year = currentMonth.Year;

            if (month == 12)
            {
                month = 1;
                year += 1;
            }
            else
            {
                month += 1;
            }

            currentMonth = new DateTime(year, month, 1);

            DrawCalendar();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            int month = currentMonth.Month;
            int year = currentMonth.Year;

            if (month == 1)
            {
                month = 12;
                year -= 1;
            }
            else
            {
                month -= 1;
            }

            currentMonth = new DateTime(year, month, 1);

            DrawCalendar();
        }
    }

}
