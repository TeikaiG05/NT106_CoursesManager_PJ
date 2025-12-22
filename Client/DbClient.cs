using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace NT106_BT2
{
    /// <summary>
    /// ============================================================================
    /// DbClient.cs - Giao tiếp với SQL Server từ phía Client
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Quản lý lớp học (tạo, tham gia, lấy danh sách)
    /// Quản lý file trong phòng nhóm
    /// Lấy danh sách người dùng
    /// Cập nhật vai trò người dùng
    /// Lưu/tải lịch sử chat riêng
    /// 
    /// CÔNG VIỆC CỤ THỂ:
    /// 
    /// 1. InsertClass(name, code, ownerEmail) → int classId
    ///    - Tạo lớp học mới
    ///    - Tự động thêm owner vào ClassMembers với vai trò "Teacher"
    ///    - Trả về classId vừa tạo
    /// 
    /// 2. GetClassesByUser(email) → DataTable
    ///    - Lấy tất cả lớp mà user là thành viên
    ///    - Trả về bảng với cột: Name, Code
    /// 
    /// 3. JoinClassByCode(code, email, role = "Student") → (Name, Code)?
    ///    - Tìm lớp bằng code
    ///    - Thêm user vào lớp nếu chưa là thành viên
    ///    - Trả về (ClassName, ClassCode) hoặc null nếu code không tồn tại
    /// 
    /// 4. InsertRoomFile(roomCode, fileName, filePath, fileSizeBytes, uploadedBy)
    ///    - Lưu thông tin file vào database
    ///    - Ghi thời gian upload (SYSDATETIME)
    ///    - Lưu ai upload file đó
    /// 
    /// 5. GetFilesByRoom(roomCode) → DataTable
    ///    - Lấy tất cả file của 1 phòng
    ///    - Trả về bảng: FileName, FilePath, FileSizeBytes, UploadedAt, UploadedBy
    ///    - Sắp xếp theo UploadedAt ASC
    /// 
    /// 6. GetAllUsers() → DataTable
    ///    - Lấy danh sách tất cả user
    ///    - Trả về: Email, Role (mặc định = "Student")
    ///    - Sắp xếp theo Email
    /// 
    /// 7. UpdateUserRole(email, role)
    ///    - Cập nhật vai trò user
    ///    - Dùng transaction để cập nhật cả Users và ClassMembers
    ///    - Đảm bảo đồng bộ dữ liệu
    /// 
    /// 8. InsertPrivateMessage(fromEmail, toEmail, message)  [MỚI]
    ///    - Lưu tin nhắn riêng vào database
    ///    - Tự động ghi SentAt = DateTime.Now
    /// 
    /// 9. GetPrivateMessages(userEmail, friendEmail, take=100) → List<Tuple>  [MỚI]
    ///    - Lấy lịch sử chat 1-1
    ///    - Tìm tin nhắn 2 chiều (A→B hoặc B→A)
    ///    - Sắp xếp theo SentAt ASC (tin cũ trước)
    ///    - Trả về List<(FromEmail, ToEmail, Message, SentAt)>
    /// 
    /// BIẾN TOÀN CỤC:
    /// - ConnStr: Connection string từ config file
    /// 
    /// SQL USAGE:
    /// - Sử dụng parameterized queries để chống SQL injection
    /// - Transaction cho các thao tác liên quan (UpdateUserRole)
    /// - OUTPUT INSERTED.Id để lấy ID vừa insert
    /// 
    /// ============================================================================
    /// </summary>
    internal static class DbClient
    {
        private static string ConnStr => ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #region InsertClass
        public static int InsertClass(string name, string code, string ownerEmail)
        {
            using (var cn = new SqlConnection(ConnStr))
            {
                cn.Open();
                int classId;
                using (var cmd = new SqlCommand(@"INSERT INTO dbo.Classes(Name, Code, OwnerEmail) OUTPUT INSERTED.Id VALUES (@n,@c,@o)", cn))
                {
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@c", code);
                    cmd.Parameters.AddWithValue("@o", ownerEmail);
                    classId = (int)cmd.ExecuteScalar();
                }

                using (var cmd = new SqlCommand(@"INSERT INTO dbo.ClassMembers(ClassId, Email, Role) VALUES (@cid, @em, @role)", cn))
                {
                    cmd.Parameters.AddWithValue("@cid", classId);
                    cmd.Parameters.AddWithValue("@em", ownerEmail);
                    cmd.Parameters.AddWithValue("@role", "Teacher");
                    cmd.ExecuteNonQuery();
                }

                return classId;
            }
        }
        #endregion

        #region GetClassByCode
        public static DataTable GetClassesByUser(string email)
        {
            var table = new DataTable();

            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"SELECT c.Name, c.Code FROM dbo.Classes c INNER JOIN dbo.ClassMembers m ON c.Id = m.ClassId WHERE m.Email = @em", cn))
            {
                cmd.CommandTimeout = 5;
                cmd.Parameters.AddWithValue("@em", email);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    table.Load(rd);
                }
            }

            return table;
        }
        #endregion

        #region JoinClassByCode
        public static (string Name, string Code)? JoinClassByCode(string code, string email, string role = "Student")
        {
            using (var cn = new SqlConnection(ConnStr))
            {
                cn.Open();
                int classId;
                string name;
                string realCode;

                using (var cmdFind = new SqlCommand(@"SELECT TOP 1 Id, Name, Code FROM dbo.Classes WHERE Code = @c", cn))
                {
                    cmdFind.Parameters.AddWithValue("@c", code);

                    using (var rd = cmdFind.ExecuteReader())
                    {
                        if (!rd.Read()) return null;

                        classId = rd.GetInt32(0);
                        name = rd.GetString(1);
                        realCode = rd.GetString(2);
                    }
                }

                using (var cmdMem = new SqlCommand(@"IF NOT EXISTS(SELECT 1 FROM dbo.ClassMembers WHERE ClassId=@cid AND Email=@em) INSERT INTO dbo.ClassMembers(ClassId, Email, Role) VALUES (@cid, @em, @role)", cn))
                {
                    cmdMem.Parameters.AddWithValue("@cid", classId);
                    cmdMem.Parameters.AddWithValue("@em", email);
                    cmdMem.Parameters.AddWithValue("@role", role);
                    cmdMem.ExecuteNonQuery();
                }

                return (name, realCode);
            }
        }
        #endregion

        #region InsertRoomFile
        public static void InsertRoomFile(string roomCode, string fileName, string filePath, long fileSizeBytes, string uploadedBy)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                @"INSERT INTO dbo.RoomFiles(RoomCode, FileName, FilePath, FileSizeBytes, UploadedAt, UploadedBy) VALUES (@room, @name, @path, @size, SYSDATETIME(), @user)", cn))
            {
                cmd.Parameters.AddWithValue("@room", roomCode);
                cmd.Parameters.AddWithValue("@name", fileName);
                cmd.Parameters.AddWithValue("@path", filePath);
                cmd.Parameters.AddWithValue("@size", fileSizeBytes);
                cmd.Parameters.AddWithValue("@user", uploadedBy ?? (object)DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region GetFilesByRoom
        public static DataTable GetFilesByRoom(string roomCode)
        {
            var table = new DataTable();

            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"SELECT FileName, FilePath, FileSizeBytes, UploadedAt, UploadedBy FROM dbo.RoomFiles WHERE RoomCode = @room ORDER BY UploadedAt ASC;", cn))
            {
                cmd.Parameters.AddWithValue("@room", roomCode);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    table.Load(rd);
                }
            }

            return table;
        }

        public static DataTable GetAllUsers()
        {
            var table = new DataTable();
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"SELECT Email, ISNULL(Role, 'Student') AS Role FROM dbo.Users ORDER BY Email", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    table.Load(rd);
                }
            }
            return table;
        }

        public static void UpdateUserRole(string email, string role)
        {
            using (var cn = new SqlConnection(ConnStr))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    // Cập nhật vai trò người dùng chính
                    using (var cmd = new SqlCommand(@"UPDATE dbo.Users SET Role = @role WHERE Email = @email", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.ExecuteNonQuery();
                    }

                    // Đảm bảo vai trò thành viên lớp học được đồng bộ.
                    using (var cmd = new SqlCommand(
                        @"UPDATE dbo.ClassMembers SET Role = @role WHERE Email = @email", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
            }
        }
        #endregion
        // Thêm vào lớp DbClient

        #region PrivateMessages

        public static void InsertPrivateMessage(string fromEmail, string toEmail, string message)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                @"INSERT INTO dbo.PrivateMessages(FromEmail, ToEmail, Message, SentAt) 
          VALUES (@from, @to, @msg, @sentAt)", cn))
            {
                cmd.Parameters.AddWithValue("@from", fromEmail);
                cmd.Parameters.AddWithValue("@to", toEmail);
                cmd.Parameters.AddWithValue("@msg", message);
                cmd.Parameters.AddWithValue("@sentAt", DateTime.Now);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static List<(string FromEmail, string ToEmail, string Message, DateTime SentAt)>
            GetPrivateMessages(string userEmail, string friendEmail, int take = 100)
        {
            var list = new List<(string, string, string, DateTime)>();

            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                @"SELECT TOP (@take) FromEmail, ToEmail, Message, SentAt 
          FROM dbo.PrivateMessages 
          WHERE (FromEmail = @user AND ToEmail = @friend) 
             OR (FromEmail = @friend AND ToEmail = @user)
          ORDER BY SentAt ASC", cn))
            {
                cmd.Parameters.AddWithValue("@take", take);
                cmd.Parameters.AddWithValue("@user", userEmail);
                cmd.Parameters.AddWithValue("@friend", friendEmail);

                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add((
                            rd.GetString(0),
                            rd.GetString(1),
                            rd.GetString(2),
                            rd.GetDateTime(3)
                        ));
                    }
                }
            }

            return list;
        }

        #endregion
    }
}
