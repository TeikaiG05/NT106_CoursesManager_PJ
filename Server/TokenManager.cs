using System;
using System.Collections.Concurrent;

namespace Server
{
    /// <summary>
    /// ============================================================================
    /// TokenManager.cs - Quản lý JWT Token cho auto-login
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Cấp token mới khi login
    /// Xác thực token (validate)
    /// Hủy token khi logout
    /// Kiểm tra hết hạn token
    /// 
    /// CÔNG VIỆC CỤ THỤ:
    /// 
    /// 1. Issue(string username) → (string token, DateTime exp)
    ///    - Tạo token random dùng Guid.NewGuid().ToString("N")
    ///    - Tính thời hạn: exp = DateTime.UtcNow + 7 days
    ///    - Lưu vào dictionary: store[token] = (username, exp)
    ///    - Trả về tuple (token, exp)
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    var (token, exp) = TokenManager.Issue("user@email.com");
    ///    // Gửi token cho client để lưu
    ///    ```
    /// 
    /// 2. Validate(string username, string token) → bool
    ///    - Kiểm tra token null → false
    ///    - Kiểm tra token có tồn tại trong store → false nếu không
    ///    - Kiểm tra username khớp với token → false nếu không
    ///    - Kiểm tra hết hạn: DateTime.UtcNow > exp?
    ///      * Nếu hết hạn: xóa từ store, return false
    ///      * Nếu còn hạn: return true
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    if (TokenManager.Validate(username, token)) {
    ///        // Token valid → auto-login thành công
    ///    } else {
    ///        // Token invalid hoặc hết hạn → login lại
    ///    }
    ///    ```
    /// 
    /// 3. Invalidate(string username, string token)
    ///    - Kiểm tra token null → return
    ///    - Kiểm tra token có tồn tại + username khớp?
    ///    - Nếu OK: xóa từ store bằng TryRemove()
    ///    - Gọi lúc logout để hủy token
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    TokenManager.Invalidate(username, token);
    ///    // Token đã bị hủy → không thể auto-login nữa
    ///    ```
    /// 
    /// STORAGE:
    /// - store: ConcurrentDictionary<string, (string user, DateTime exp)>
    /// - Thread-safe (không cần lock)
    /// - In-memory (không lưu vào DB)
    /// 
    /// TOKEN TTL (Time To Live):
    /// - ttl = TimeSpan.FromDays(7)
    /// - Token có hiệu lực 7 ngày
    /// - Sau 7 ngày tự động expire
    /// 
    /// AUTO-LOGIN FLOW:
    /// 1. User login lần đầu → Server issue token
    /// 2. Client lưu token vào settings
    /// 3. Lần sau user mở app → gửi token
    /// 4. Server validate token → OK nếu còn hạn
    /// 5. Tự động đăng nhập mà không cần nhập password
    /// 
    /// SECURITY:
    /// Token là GUID random (khó đoán)
    /// Token có expiration (expire sau 7 ngày)
    /// Hủy token khi logout (không thể reuse)
    /// In-memory storage (không lưu vào DB)
    /// 
    /// ============================================================================
    /// </summary>
    internal static class TokenManager
    {
        private static readonly ConcurrentDictionary<string, (string user, DateTime exp)> store = new ConcurrentDictionary<string, (string, DateTime)>();

        private static readonly TimeSpan ttl = TimeSpan.FromDays(7);

        public static (string token, DateTime exp) Issue(string username)
        {
            var token = Guid.NewGuid().ToString("N");
            var exp = DateTime.UtcNow.Add(ttl);
            store[token] = (username, exp);
            return (token, exp);
        }

        public static bool Validate(string username, string token)
        {
            if (token == null) return false;
            if (store.TryGetValue(token, out var v) == false) return false;
            if (!string.Equals(v.user, username, StringComparison.OrdinalIgnoreCase)) return false;
            if (DateTime.UtcNow > v.exp) { store.TryRemove(token, out _); return false; }
            return true;
        }

        public static void Invalidate(string username, string token)
        {
            if (token == null) return;
            if (store.TryGetValue(token, out var v) &&
                string.Equals(v.user, username, StringComparison.OrdinalIgnoreCase))
            {
                store.TryRemove(token, out _);
            }
        }
    }
}
