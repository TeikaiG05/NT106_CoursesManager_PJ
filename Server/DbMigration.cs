using System;
using System.Data;
using System.Data.SqlClient;

namespace Server
{
    /// <summary>
    /// ============================================================================
    /// DbMigration.cs - Migration Database (tạo bảng tự động)
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Tạo bảng PrivateMessages nếu chưa tồn tại
    /// Tạo index để tối ưu hiệu suất
    /// Chạy tự động lúc server khởi động
    /// Không làm hỏng dữ liệu nếu chạy lại nhiều lần (idempotent)
    /// 
    /// CÔNG VIỆC CỤ THỤ:
    /// 
    /// 1. RunMigrations(Action<string, string> log)
    ///    - Entry point duy nhất
    ///    - Gọi từ Server.cs lúc khởi tạo
    ///    - Chạy tất cả migration methods
    ///    - In log từng bước
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    DbMigration.RunMigrations((source, message) => {
    ///        Console.WriteLine($"[{source}] {message}");
    ///    });
    ///    ```
    /// 
    /// 2. CreatePrivateMessagesTable(Action<string, string> log)
    ///    - Tạo bảng PrivateMessages nếu chưa tồn tại
    ///    - Sử dụng IF NOT EXISTS (idempotent)
    ///    - Schema bảng:
    ///      * Id: INT PRIMARY KEY IDENTITY(1,1)
    ///      * FromEmail: NVARCHAR(255) - người gửi
    ///      * ToEmail: NVARCHAR(255) - người nhận
    ///      * Message: NVARCHAR(MAX) - nội dung tin
    ///      * SentAt: DATETIME DEFAULT GETDATE() - thời gian
    ///    - Foreign key:
    ///      * FromEmail → Users.Email
    ///      * ToEmail → Users.Email
    ///    - In log: "Bảng PrivateMessages đã sẵn sàng"
    /// 
    /// 3. CreateIndexes(Action<string, string> log)
    ///    - Tạo index cho PrivateMessages nếu chưa tồn tại
    ///    - Index name: IX_PrivateMessages_FromTo
    ///    - Index columns: (FromEmail, ToEmail, SentAt)
    ///    - Lợi ích:
    ///      * Tìm kiếm tin nhắn 2 chiều nhanh hơn
    ///      * ORDER BY SentAt cũng được index
    ///    - In log: "Index PrivateMessages đã sẵn sàng"
    /// 
    /// IDEMPOTENT PATTERN:
    /// - Tất cả CREATE statement dùng IF NOT EXISTS
    /// - Có thể chạy lại nhiều lần mà không error
    /// - Perfect cho automated deployment
    /// 
    /// USAGE PATTERN:
    /// ```csharp
    /// public class Server : Form {
    ///     public Server() {
    ///         // ...
    ///         DbMigration.RunMigrations(Log);
    ///     }
    /// }
    /// ```
    /// 
    /// DATABASE CHANGES:
    /// - Lần 1 chạy: Tạo bảng + index
    /// - Lần 2 chạy: Kiểm tra → đã tồn tại → skip
    /// - Lần 3+ chạy: Tương tự lần 2
    /// 
    /// BENEFITS:
    /// Không cần chạy SQL script thủ công
    /// Tự động setup database khi deploy
    /// Tự động tối ưu hiệu suất (index)
    /// Không làm mất dữ liệu cũ (IF NOT EXISTS)
    /// Logging để monitor process
    /// 
    /// ============================================================================
    /// </summary>
    /// <summary>
    /// Quản lý migration database - tự động tạo các bảng nếu chưa tồn tại
    /// </summary>
    internal static class DbMigration
    {
        private static string ConnStr
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
        }

        /// <summary>
        /// Chạy tất cả migration - gọi lúc server khởi động
        /// </summary>
        public static void RunMigrations(Action<string, string> log)
        {
            try
            {
                log?.Invoke("Migration", "Bắt đầu kiểm tra và tạo bảng...");

                CreatePrivateMessagesTable(log);
                CreateIndexes(log);

                log?.Invoke("Migration", "Hoàn tất migration");
            }
            catch (Exception ex)
            {
                log?.Invoke("Migration", $"Lỗi migration: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo bảng PrivateMessages nếu chưa tồn tại
        /// </summary>
        private static void CreatePrivateMessagesTable(Action<string, string> log)
        {
            using (var cn = new SqlConnection(ConnStr))
            {
                cn.Open();

                // Kiểm tra xem bảng có tồn tại không
                using (var cmd = new SqlCommand(
                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                      WHERE TABLE_NAME = 'PrivateMessages' AND TABLE_SCHEMA = 'dbo')
                      BEGIN
                        CREATE TABLE dbo.PrivateMessages (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            FromEmail NVARCHAR(255) NOT NULL,
                            ToEmail NVARCHAR(255) NOT NULL,
                            Message NVARCHAR(MAX) NOT NULL,
                            SentAt DATETIME DEFAULT GETDATE(),
                            FOREIGN KEY (FromEmail) REFERENCES dbo.Users(Email),
                            FOREIGN KEY (ToEmail) REFERENCES dbo.Users(Email)
                        )
                      END", cn))
                {
                    cmd.ExecuteNonQuery();
                    log?.Invoke("Migration", "Bảng PrivateMessages đã sẵn sàng");
                }
            }
        }

        /// <summary>
        /// Tạo các index để tối ưu hiệu suất
        /// </summary>
        private static void CreateIndexes(Action<string, string> log)
        {
            using (var cn = new SqlConnection(ConnStr))
            {
                cn.Open();

                // Index cho tìm kiếm nhanh
                using (var cmd = new SqlCommand(
                    @"IF NOT EXISTS (SELECT * FROM sys.indexes 
                      WHERE name='IX_PrivateMessages_FromTo' AND object_id=OBJECT_ID('dbo.PrivateMessages'))
                      BEGIN
                        CREATE INDEX IX_PrivateMessages_FromTo 
                        ON dbo.PrivateMessages(FromEmail, ToEmail, SentAt)
                      END", cn))
                {
                    cmd.ExecuteNonQuery();
                    log?.Invoke("Migration", "Index PrivateMessages đã sẵn sàng");
                }
            }
        }
    }
}