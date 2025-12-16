using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Server
{
    internal static class PasswordResetManager
    {
        private class Entry
        {
            public string Otp { get; set; }
            public DateTime ExpiresAt { get; set; }
        }

        private static readonly ConcurrentDictionary<string, Entry> store = new ConcurrentDictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        private static readonly Random rnd = new Random();

        public static string GenerateOtpFor(string email, int minutesValid = 5)
        {
            var otp = rnd.Next(100000, 999999).ToString();
            var entry = new Entry { Otp = otp, ExpiresAt = DateTime.UtcNow.AddMinutes(minutesValid) };
            store[email] = entry;
            return otp;
        }

        public static bool ValidateOtp(string email, string otp)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp)) return false;
            if (!store.TryGetValue(email, out var entry)) return false;
            if (DateTime.UtcNow > entry.ExpiresAt) { store.TryRemove(email, out _); return false; }
            bool ok = string.Equals(entry.Otp, otp, StringComparison.Ordinal);
            if (ok) store.TryRemove(email, out _);
            return ok;
        }

        public static void RemoveOtp(string email)
        {
            store.TryRemove(email, out _);
        }

        public static void SendOtpEmail(string toEmail, string otp)
        {
            var host = ConfigurationManager.AppSettings["SmtpHost"];
            var portStr = ConfigurationManager.AppSettings["SmtpPort"];
            var user = ConfigurationManager.AppSettings["SmtpUser"];
            var pass = ConfigurationManager.AppSettings["SmtpPass"];
            var from = ConfigurationManager.AppSettings["SmtpFrom"] ?? user;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                throw new InvalidOperationException("SMTP settings are not configured on server.");

            int port = 587;
            int.TryParse(portStr, out port);

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(from);
                mail.To.Add(toEmail);
                mail.Subject = "Password reset - Mã xác minh";
                mail.Body = $"Mã xác minh của bạn là: {otp}\nMã có hiệu lực trong 5 phút.";

                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.Credentials = new NetworkCredential(user, pass);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
    }
}