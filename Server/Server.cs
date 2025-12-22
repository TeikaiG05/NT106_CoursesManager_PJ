using System;
using System.Windows.Forms;

namespace Server
{
    /// <summary>
    /// ============================================================================
    /// Server.cs - Form chính của ứng dụng Server (giao diện điều khiển)
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Khởi tạo TCP Server (cổng 8080)
    /// Khởi tạo UDP Relay Server (cổng 9001)
    /// Chạy Database Migration tự động
    /// Hiển thị logs từ server lên ListView
    /// Nút Start/Stop để khởi động/dừng server
    /// 
    /// CÔNG VIỆC CỤ THỤ:
    /// 
    /// 1. Constructor: Server()
    ///    - Gọi InitializeComponent()
    ///    - Khởi tạo ListView để hiển thị logs
    ///      * Cột: Time (120px), Source (120px), Message (600px)
    ///      * View = View.Details (chi tiết)
    ///      * FullRowSelect = true (chọn toàn hàng)
    ///    - Tạo TcpServer instance (kèm Log callback)
    ///    - Tạo UdpRelayServer instance (kèm RoomManager từ TcpServer)
    ///    - Hook event FormClosing để dừng server khi đóng form
    ///    - Gọi DbMigration.RunMigrations(Log) để tạo bảng database
    /// 
    /// 2. btnStart_Click(object sender, EventArgs e)
    ///    - Gọi server.Start(TCP_PORT = 8080)
    ///    - Gọi udp.Start()
    ///    - In log: "Started TCP :8080, UDP :9001"
    /// 
    /// 3. btnStop_Click(object sender, EventArgs e)
    ///    - Gọi udp.Stop()
    ///    - Gọi server.Stop()
    ///    - In log: "Stopped"
    /// 
    /// 4. Log(string source, string message)
    ///    - Callback được TcpServer/UdpServer gọi
    ///    - Nếu IsDisposed: return (tránh error)
    ///    - Nếu InvokeRequired: BeginInvoke để thread-safe
    ///    - Gọi AddRow() để thêm log vào ListView
    /// 
    /// 5. AddRow(string source, string message)
    ///    - Tạo ListViewItem mới
    ///    - Thêm cột: Time, Source, Message
    ///    - Thêm vào lvLog
    ///    - EnsureVisible() để scroll tới dòng mới
    ///    - Giữ max 1000 dòng (xóa dòng đầu nếu vượt quá)
    /// 
    /// 6. Server_FormClosing(object sender, FormClosingEventArgs e)
    ///    - Khi đóng form: dừng UDP
    ///    - Khi đóng form: dừng TCP
    ///    - Try-catch để tránh error nếu chưa start
    /// 
    /// HẰNG SỐ:
    /// - TCP_PORT = 8080 (cổng TCP)
    /// - UDP_PORT = 9001 (cổng UDP relay)
    /// 
    /// BIẾN TOÀN CỤC:
    /// - server: TcpServer instance (xử lý TCP)
    /// - udp: UdpRelayServer instance (relay video/audio)
    /// - lvLog: ListView hiển thị logs
    /// 
    /// DỊCH VỤ LIÊN KẾT:
    /// - TcpServer: Máy chủ TCP chính
    /// - UdpRelayServer: Relay UDP cho cuộc gọi
    /// - DbMigration: Tạo bảng database tự động
    /// 
    /// LOG FLOW:
    /// TcpServer → Log("TCP", "message")
    /// UdpServer → Log("UDP", "message")
    /// DbMigration → Log("Migration", "message")
    ///      ↓
    /// Server.Log() → Invoke AddRow()
    ///      ↓
    /// lvLog.Items.Add() → Hiển thị trên UI
    /// 
    /// ============================================================================
    /// </summary>
    public partial class Server : Form
    {
        private TcpServer server;
        private UdpRelayServer udp;

        private const int TCP_PORT = 8080;
        private const int UDP_PORT = 9001;

        public Server()
        {
            InitializeComponent();

            lvLog.View = View.Details;
            lvLog.FullRowSelect = true;
            lvLog.GridLines = true;
            lvLog.Columns.Add("Time", 120);
            lvLog.Columns.Add("Source", 120);
            lvLog.Columns.Add("Message", 600);

            server = new TcpServer(Log);

            udp = new UdpRelayServer(UDP_PORT, server.Rooms);

            this.FormClosing += Server_FormClosing;
            DbMigration.RunMigrations(Log);
        }

        private void Log(string source, string message)
        {
            if (lvLog.IsDisposed) return;
            if (lvLog.InvokeRequired)
            {
                lvLog.BeginInvoke(new Action(() => AddRow(source, message)));
            }
            else
            {
                AddRow(source, message);
            }
        }

        private void AddRow(string source, string message)
        {
            var it = new ListViewItem(DateTime.Now.ToString("HH:mm:ss"));
            it.SubItems.Add(source ?? "");
            it.SubItems.Add(message ?? "");
            lvLog.Items.Add(it);
            it.EnsureVisible();
            if (lvLog.Items.Count > 1000) lvLog.Items.RemoveAt(0);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start(TCP_PORT);
            udp.Start();
            Log("Server", $"Started TCP :{TCP_PORT}, UDP :{UDP_PORT}");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            udp.Stop();
            server.Stop();
            Log("Server", "Stopped");
        }

        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { udp?.Stop(); } catch { }
            try { server?.Stop(); } catch { }
        }
    }
}
