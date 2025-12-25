using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Server
{
    /// <summary>
    /// ============================================================================
    /// Db.cs (Server) - Giao tiếp với SQL Server từ phía Server
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Xác thực người dùng (login, register, token)
    /// Quản lý tin nhắn nhóm (lưu, tải lịch sử)
    /// Quản lý thành viên lớp
    /// Lưu/tải tin nhắn riêng (1-1)
    /// Cập nhật mật khẩu (reset)
    /// 
    /// CÔNG VIỆC CỤ THỤ:
    /// 
    /// 1. UsernameExists(string email) → bool
    ///    - Kiểm tra email đã tồn tại trong database không
    ///    - Dùng lúc register (prevent duplicate email)
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    if (Db.UsernameExists("user@email.com")) {
    ///        // Email đã tồn tại
    ///    }
    ///    ```
    /// 
    /// 2. InsertUser(firstname, surname, birthday, gender, email, passwordHex)
    ///    - INSERT user mới vào bảng Users
    ///    - Lưu password dạng hex (SHA256)
    ///    - birthday có thể null
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    Db.InsertUser("Nguyễn", "Văn A", new DateTime(2000,1,1), 
    ///                  "Male", "user@email.com", passwordHashHex);
    ///    ```
    /// 
    /// 3. FindByLogin(email, passwordHex) → UserInfo?
    ///    - SELECT user từ Users WHERE email AND passwordHex match
    ///    - Trả về tuple (Firstname, Surname, Birthday, Gender, Email, Role, Avatar)
    ///    - Trả về null nếu không tìm thấy
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    var user = Db.FindByLogin("user@email.com", passwordHash);
    ///    if (user.HasValue) {
    ///        // Login thành công
    ///        var (fn, sn, bd, gd, em, role, av) = user.Value;
    ///    }
    ///    ```
    /// 
    /// 4. GetByEmail(email) → UserInfo?
    ///    - SELECT user từ Users WHERE email
    ///    - Dùng để lấy thông tin user (không kiểm tra password)
    ///    
    /// 5. InsertGroupMessage(roomCode, fromEmail, fromName, message, sentAt = null)
    ///    - INSERT tin nhắn nhóm vào bảng GroupMessages
    ///    - SentAt = sentAt ?? DateTime.UtcNow (nếu null → thời hiện tại)
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    Db.InsertGroupMessage("class001", "user@email.com", "Nguyễn Văn A", 
    ///                          "Xin chào!", DateTime.UtcNow);
    ///    ```
    /// 
    /// 6. GetGroupMessages(roomCode, take = 50) → List<GroupChatMsgEx>
    ///    - SELECT TOP (take) tin từ GroupMessages WHERE roomCode
    ///    - ORDER BY SentAt ASC (tin cũ trước)
    ///    - Trả về list kèm timestamp (sentAt)
    ///    - Dùng lúc client request lịch sử
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    var messages = Db.GetGroupMessages("class001", 100);
    ///    foreach (var msg in messages) {
    ///        // msg.fromEmail, msg.message, msg.sentAt
    ///    }
    ///    ```
    /// 
    /// 7. InsertPrivateMessage(fromEmail, toEmail, message)  [MỚI]
    ///    - INSERT tin nhắn riêng vào bảng PrivateMessages
    ///    - Tự động ghi SentAt = DateTime.UtcNow
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    Db.InsertPrivateMessage("user1@email.com", "user2@email.com", 
    ///                            "Chào bạn!");
    ///    ```
    /// 
    /// 8. GetPrivateMessages(userEmail, friendEmail, take = 100)
    ///    → List<(FromEmail, ToEmail, Message, SentAt)>  [MỚI]
    ///    - SELECT tin nhắn 2 chiều (A→B hoặc B→A)
    ///    - ORDER BY SentAt ASC (tin cũ trước)
    ///    - Dùng lúc client load lịch sử chat 1-1
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    var history = Db.GetPrivateMessages("user1@email.com", "user2@email.com", 100);
    ///    foreach (var (from, to, msg, time) in history) {
    ///        // Hiển thị tin nhắn
    ///    }
    ///    ```
    /// 
    /// 9. GetGroupMembers(roomCode) → List<GroupMemberDto>
    ///    - SELECT tất cả member của lớp (từ ClassMembers + Users)
    ///    - Trả về: email, fullName, role
    ///    - Dùng lúc client request danh sách thành viên
    ///    
    /// 10. UpdateUserPassword(email, passwordHex)
    ///     - UPDATE password của user
    ///     - Dùng lúc reset mật khẩu
    /// 
    /// 11. GetAllUsers() → DataTable
    ///     - SELECT Email, Role từ Users
    ///     - Dùng để load danh sách user (cho admin panel hoặc stats)
    /// 
    /// DATABASE TABLES:
    /// - Users: email, passwordHex, firstname, surname, birthday, gender, role, avatar
    /// - GroupMessages: roomCode, fromEmail, fromName, message, sentAt
    /// - PrivateMessages: fromEmail, toEmail, message, sentAt (MỚI)
    /// - ClassMembers: classId, email, role
    /// - Classes: id, name, code, ownerEmail
    /// 
    /// SQL PATTERNS:
    /// - Parameterized queries (chống SQL injection)
    /// - Nullable datetime (birthday có thể null)
    /// - UNION để query tin 2 chiều (from→to hoặc to→from)
    /// 
    /// ============================================================================
    /// </summary>
    internal static class Db
    {
        #region Connection String
        private static string ConnStr
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
        }
        #endregion

        #region UsernameExists
        public static bool UsernameExists(string email)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT 1 FROM dbo.Users WHERE Email=@e", cn))
            {
                cmd.Parameters.AddWithValue("@e", email);
                cn.Open();
                object o = cmd.ExecuteScalar();
                return o != null;
            }
        }
        #endregion

        #region InsertUser
        public static void InsertUser(string firstname, string surname, DateTime? birthday, string gender, string email, string passwordHex)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"INSERT INTO dbo.Users(Firstname,Surname,Birthday,Gender,Email,PasswordEncrypted) VALUES (@fn,@sn,@bd,@gd,@em,@ph)", cn))
            {
                cmd.Parameters.AddWithValue("@fn", firstname);
                cmd.Parameters.AddWithValue("@sn", surname);
                var pBd = cmd.Parameters.Add("@bd", SqlDbType.Date);
                pBd.Value = birthday.HasValue ? (object)birthday.Value : DBNull.Value;
                cmd.Parameters.AddWithValue("@gd", gender);
                cmd.Parameters.AddWithValue("@em", email);
                var p = cmd.Parameters.Add("@ph", SqlDbType.Char, 64);
                p.Value = passwordHex;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region GetByEmail
        public static (string Firstname, string Surname, DateTime? Birthday, string Gender, string Email, string Role, string Avatar)? GetByEmail(string email)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT TOP 1 Firstname,Surname,Birthday,Gender,Email, Role, Avatar FROM dbo.Users WHERE Email=@e", cn))
            {
                cmd.Parameters.AddWithValue("@e", email);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return null;
                    return (
                        rd["Firstname"].ToString(),
                        rd["Surname"].ToString(),
                        rd["Birthday"] == DBNull.Value ? (DateTime?)null : (DateTime)rd["Birthday"],
                        rd["Gender"].ToString(),
                        rd["Email"].ToString(),
                        rd["Role"] == DBNull.Value ? "Student" : rd["Role"].ToString(),
                        rd["Avatar"] == DBNull.Value ? null : rd["Avatar"].ToString()
                    );
                }
            }
        }
        #endregion

        #region FindByLogin
        public static (string Firstname, string Surname, DateTime? Birthday, string Gender, string Email, string Role, string Avatar)? FindByLogin(string email, string passwordHex)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"SELECT Firstname, Surname, Birthday, Gender, Email, Role, Avatar FROM dbo.Users WHERE Email = @e AND PasswordEncrypted = @ph", cn))
            {
                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@ph", passwordHex);

                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return null;

                    string fn = rd.IsDBNull(0) ? "" : rd.GetString(0);
                    string sn = rd.IsDBNull(1) ? "" : rd.GetString(1);
                    DateTime? bd = rd.IsDBNull(2) ? (DateTime?)null : rd.GetDateTime(2);
                    string gd = rd.IsDBNull(3) ? "" : rd.GetString(3);
                    string em = rd.IsDBNull(4) ? "" : rd.GetString(4);
                    string role = rd.IsDBNull(5) ? "Student" : rd.GetString(5);
                    string avatar = rd.IsDBNull(6) ? null : rd.GetString(6);

                    return (fn, sn, bd, gd, em, role, avatar);
                }
            }
        }
        #endregion

        #region InsertGroupMessage
        public static void InsertGroupMessage(string roomCode, string fromEmail, string fromName, string message, DateTime? sentAt = null)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"INSERT INTO dbo.GroupMessages(RoomCode, FromEmail, FromName, Message, SentAt) VALUES (@rc, @fe, @fn, @msg, @sa)", cn))
            {
                cmd.Parameters.AddWithValue("@rc", roomCode);
                cmd.Parameters.AddWithValue("@fe", fromEmail);
                cmd.Parameters.AddWithValue("@fn", (object)(fromName ?? ""));
                cmd.Parameters.AddWithValue("@msg", message);
                cmd.Parameters.AddWithValue("@sa", (object)(sentAt ?? DateTime.UtcNow));

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region GetGroupMessages
        public static List<GroupChatMsgEx> GetGroupMessages(string roomCode, int take = 50)
        {
            var list = new List<GroupChatMsgEx>();

            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"SELECT TOP (@take) RoomCode, FromEmail, FromName, Message, SentAt FROM dbo.GroupMessages WHERE RoomCode = @rc ORDER BY SentAt DESC", cn))
            {
                cmd.Parameters.AddWithValue("@take", take);
                cmd.Parameters.AddWithValue("@rc", roomCode);

                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new GroupChatMsgEx
                        {
                            roomCode = rd.GetString(0),
                            fromEmail = rd.GetString(1),
                            fromName = rd.IsDBNull(2) ? null : rd.GetString(2),
                            message = rd.GetString(3),
                            sentAt = rd.GetDateTime(4),
                        });
                    }
                }
            }

            list.Reverse();
            return list;
        }
        #endregion

        public static DataTable GetAllUsers()
        {
            var dt = new DataTable();

            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"SELECT Firstname, Surname, Email, Role FROM dbo.Users ORDER BY Email", cn))
            {
                cn.Open();
                using (var ad = new SqlDataAdapter(cmd))
                {
                    ad.Fill(dt);
                }
            }

            return dt;
        }

        public static void UpdateUserPassword(string email, string passwordHex)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"UPDATE dbo.Users SET PasswordEncrypted = @ph WHERE Email = @em", cn))
            {
                cmd.Parameters.AddWithValue("@ph", passwordHex);
                cmd.Parameters.AddWithValue("@em", email);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        

        #region GetGroupMembers
        public static List<GroupMemberDto> GetGroupMembers(string roomCode)
        {
            var list = new List<GroupMemberDto>();

            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                @"SELECT DISTINCT cm.Email, 
                         ISNULL(u.Firstname + ' ' + u.Surname, cm.Email) AS FullName, 
                         cm.Role
                  FROM dbo.ClassMembers cm
                  INNER JOIN dbo.Classes c ON cm.ClassId = c.Id
                  LEFT JOIN dbo.Users u ON cm.Email = u.Email
                  WHERE c.Code = @roomCode
                  ORDER BY cm.Role DESC, cm.Email", cn))
            {
                cmd.Parameters.AddWithValue("@roomCode", roomCode);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new GroupMemberDto
                        {
                            email = rd.IsDBNull(0) ? null : rd.GetString(0),
                            fullName = rd.IsDBNull(1) ? null : rd.GetString(1),
                            role = rd.IsDBNull(2) ? null : rd.GetString(2)
                        });
                    }
                }
            }

            return list;
        }
        #endregion

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
                cmd.Parameters.AddWithValue("@sentAt", DateTime.UtcNow);

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
