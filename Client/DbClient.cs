using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace NT106_BT2
{
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
                    cmd.Parameters.AddWithValue("@role", "Owner");
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
                @"INSERT INTO dbo.RoomFiles(RoomCode, FileName, FilePath, FileSizeBytes, UploadedAt, UploadedBy)
          VALUES (@room, @name, @path, @size, SYSDATETIME(), @user)", cn))
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
            using (var cmd = new SqlCommand(
                @"SELECT FileName, FilePath, FileSizeBytes, UploadedBy FROM dbo.RoomFiles WHERE RoomCode = @room ORDER BY UploadedAt", cn))
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
        #endregion
    }
}
