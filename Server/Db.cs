using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Server
{
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
        public static (string Firstname, string Surname, DateTime? Birthday, string Gender, string Email)? GetByEmail(string email)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT TOP 1 Firstname,Surname,Birthday,Gender,Email FROM dbo.Users WHERE Email=@e", cn))
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
                        rd["Email"].ToString()
                    );
                }
            }
        }
        #endregion

        #region FindByLogin
        public static (string Firstname, string Surname, DateTime? Birthday, string Gender, string Email, string Role)? FindByLogin(string email, string passwordHex)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"SELECT Firstname, Surname, Birthday, Gender, Email, Role FROM dbo.Users WHERE Email = @e AND PasswordEncrypted = @ph", cn))
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

                    return (fn, sn, bd, gd, em, role);
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

    }
}

