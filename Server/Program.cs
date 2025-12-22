using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    /// <summary>
    /// ============================================================================
    /// Program.cs - Điểm khởi động của ứng dụng Server
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Entry point duy nhất của Server
    /// Khởi tạo Windows Forms Application
    /// Hiển thị form Server chính
    /// 
    /// CÔNG VIỆC CỤ THỤ:
    /// 
    /// 1. Main() - Static method [STAThread]
    ///    - [STAThread]: Single-threaded apartment (yêu cầu cho Windows Forms)
    ///    - Application.EnableVisualStyles(): Dùng Visual Styles hiện đại
    ///    - Application.SetCompatibleTextRenderingDefault(false): GDI+ rendering
    ///    - Application.Run(new Server()): Chạy form Server
    ///    
    ///    Luồng thực thi:
    ///    1. Application.Run() khởi tạo Server form
    ///    2. Server.Load() được gọi
    ///    3. DbMigration.RunMigrations() chạy (tạo bảng DB)
    ///    4. Form hiển thị, chờ user click Start
    ///    5. Khi đóng form → Application.Run() kết thúc
    ///    6. Program.Main() kết thúc → Server tắt
    /// 
    /// ============================================================================
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Điểm truy cập chính cho ứng dụng.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Server());
        }
    }
}
