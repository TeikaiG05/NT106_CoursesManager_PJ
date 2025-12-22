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
    /// ScheduleForm.cs - Quản lý lịch học và công việc theo ngày
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Hiển thị lịch tháng
    /// Chọn ngày xem công việc
    /// Thêm công việc cho ngày
    /// Xóa công việc
    /// Điều hướng tháng (prev/next)
    /// Quay lại ngày hôm nay
    /// Lưu/tải công việc từ file
    /// 
    /// CÔNG VIỆC CỤ THỂ:
    /// 
    /// 1. ScheduleForm() - Constructor
    ///    - Khởi tạo: currentMonth = DateTime.Today, selectedDate = DateTime.Today
    ///    - Load công việc từ file: TaskStorage.LoadFromFile()
    ///    - SetupListView(): cấu hình lvTasks
    ///    - DrawCalendar(): vẽ lịch tháng
    ///    - LoadTasks(): tải công việc cho ngày hôm nay
    /// 
    /// 2. DrawCalendar()
    ///    - Clear panelCalendar
    ///    - Tính ngày đầu tiên của tháng
    ///    - Tính offset (thứ bắt đầu)
    ///    - Vẽ 42 button (6 hàng × 7 cột):
    ///      * Nếu là hôm nay: màu xanh lá
    ///      * Nếu là ngày được chọn: màu xanh dương
    ///      * Nếu là tháng khác: màu xám
    ///      * Nếu có công việc: thêm ● dấu chấm
    ///    - Gán click handler
    /// 
    /// 3. Day_Click(object sender, EventArgs e)
    ///    - Cập nhật selectedDate từ button.Tag
    ///    - Vẽ lại lịch (để highlight ngày mới)
    ///    - Load lại danh sách công việc
    /// 
    /// 4. Day_DoubleClick(object sender, EventArgs e)
    ///    - Focus vào txtTask để nhập nhanh
    /// 
    /// 5. SetupListView()
    ///    - lvTasks.View = View.Details (hiển thị chi tiết)
    ///    - lvTasks.CheckBoxes = true (có checkbox)
    ///    - Thêm cột "Task" (width = 250)
    /// 
    /// 6. LoadTasks()
    ///    - Clear lvTasks
    ///    - Lấy tasks từ TaskStorage.GetTasks(selectedDate)
    ///    - Thêm từng task vào ListView
    /// 
    /// 7. btnAdd_Click(object sender, EventArgs e)
    ///    - Validate: txtTask.Text không trống
    ///    - Gọi TaskStorage.AddTask(selectedDate, task)
    ///    - Clear txtTask
    ///    - LoadTasks() để làm mới danh sách
    ///    - DrawCalendar() để thêm dấu ● nếu cần
    /// 
    /// 8. btnDelete_Click(object sender, EventArgs e)
    ///    - Lấy các item đã check (CheckedItems)
    ///    - Với mỗi item (duyệt từ cuối để tránh index shift):
    ///      * Gọi TaskStorage.RemoveTask(selectedDate, task)
    ///    - LoadTasks() để refresh
    ///    - DrawCalendar() để cập nhật dấu ●
    /// 
    /// 9. lvTasks_ItemSelectionChanged(...)
    ///    - Khi chọn item: item.Checked = selected
    /// 
    /// 10. btnToday_Click(object sender, EventArgs e)
    ///     - Reset: currentMonth = DateTime.Today, selectedDate = DateTime.Today
    ///     - DrawCalendar() + LoadTasks()
    /// 
    /// 11. btnNext_Click(object sender, EventArgs e)
    ///     - Tăng tháng (wrap around năm)
    ///     - DrawCalendar()
    /// 
    /// 12. btnPrev_Click(object sender, EventArgs e)
    ///     - Giảm tháng (wrap around năm)
    ///     - DrawCalendar()
    /// 
    /// NESTED CLASS:
    /// StudentTask
    ///    - Title: string (nội dung công việc)
    ///    - Completed: bool (đã hoàn thành?)
    ///    - Date: DateTime (ngày của công việc)
    /// 
    /// BIẾN TOÀN CỤC:
    /// - currentMonth: Tháng đang xem
    /// - selectedDate: Ngày được chọn (xem công việc)
    /// 
    /// DỊCH VỤ LIÊN KẾT:
    /// - TaskStorage: Quản lý lưu/tải công việc từ file
    /// 
    /// GHI CHÚ:
    /// - Lịch hiển thị 6 hàng × 7 cột = 42 ngày (gồm cả ngày tháng trước/sau)
    /// - Công việc lưu trữ dạng file (không database)
    /// 
    /// ============================================================================
    /// </summary>


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
