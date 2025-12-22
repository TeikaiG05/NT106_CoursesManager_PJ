using System;
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// ============================================================================
    /// Protocol.cs - Định nghĩa tất cả các loại message/request/response
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Định nghĩa tất cả loại tin nhắn (message types)
    /// Định nghĩa các request/response classes
    /// Dùng để serialize/deserialize JSON giữa Client và Server
    /// 
    /// CÓ 3 PHẦN CHÍNH:
    /// 1. MsgType (static class) - Các hằng số loại tin nhắn
    /// 2. Request Classes - Dữ liệu gửi từ Client tới Server
    /// 3. Response Classes - Dữ liệu trả về từ Server tới Client
    /// 
    /// ============================================================================
    /// PHẦN I: MsgType - LOẠI TIN NHẮN
    /// ============================================================================
    /// 
    /// XÁC THỰC & PHIÊN (Authentication):
    /// - "REGISTER": Đăng ký tài khoản mới
    /// - "LOGIN": Đăng nhập với email + password
    /// - "LOGIN_WITH_TOKEN": Đăng nhập bằng token (auto-login)
    /// - "LOGOUT": Đăng xuất
    /// 
    /// CHAT NHÓM (Group Chat):
    /// - "GROUP_CHAT": Tin nhắn chat nhóm
    /// - "GROUP_CHAT_HISTORY_REQ": Yêu cầu lịch sử chat nhóm
    /// - "GROUP_CHAT_HISTORY_RES": Response lịch sử chat nhóm
    /// 
    /// THÀNH VIÊN NHÓM (Group Members):
    /// - "GET_GROUP_MEMBERS": Yêu cầu lấy danh sách thành viên
    /// - "GROUP_MEMBERS": Response danh sách thành viên
    /// 
    /// CUỘC GỌI VIDEO/AUDIO (Video Call):
    /// - "CALL_JOIN": Yêu cầu tham gia cuộc gọi
    /// - "CALL_LEAVE": Yêu cầu rời khỏi cuộc gọi
    /// - "CALL_STATE": Trạng thái các thành viên trong cuộc gọi
    /// - "CALL_SHARE": Chia sẻ màn hình
    /// - "CALL_FRAME": Gửi frame video
    /// 
    /// RESET MẬT KHẨU (Password Reset):
    /// - "RESET_REQUEST": Yêu cầu reset mật khẩu (OTP)
    /// - "RESET_CONFIRM": Xác nhận reset với OTP
    /// 
    /// ============================================================================
    /// PHẦN II: REQUEST CLASSES - DỮ LIỆU GỬI ĐI
    /// ============================================================================
    /// 
    /// 1. RegisterReq (Đăng ký tài khoản)
    ///    - type = "REGISTER"
    ///    - username, email, passwordHash
    ///    - fullName, gender, birthday
    /// 
    /// 2. LoginReq (Đăng nhập)
    ///    - type = "LOGIN"
    ///    - username (email), passwordHash
    /// 
    /// 3. TokenLoginReq (Đăng nhập bằng token)
    ///    - type = "LOGIN_WITH_TOKEN"
    ///    - username (email), token
    /// 
    /// 4. LogoutReq (Đăng xuất)
    ///    - type = "LOGOUT"
    ///    - username, token
    /// 
    /// 5. GroupChatMsg (Chat nhóm)
    ///    - type = "GROUP_CHAT"
    ///    - roomCode, fromEmail, fromName, message
    /// 
    /// 6. GroupChatHistoryReq (Yêu cầu lịch sử)
    ///    - type = "GROUP_CHAT_HISTORY_REQ"
    ///    - roomCode, take (số tin muốn lấy)
    /// 
    /// 7. GetGroupMembersReq (Yêu cầu danh sách thành viên)
    ///    - type = "GET_GROUP_MEMBERS"
    ///    - roomCode, requestId (để match response)
    /// 
    /// 8. CallJoinReq (Tham gia cuộc gọi)
    ///    - type = "CALL_JOIN"
    ///    - roomCode, email, name, udpPort
    /// 
    /// 9. CallLeaveReq (Rời khỏi cuộc gọi)
    ///    - type = "CALL_LEAVE"
    ///    - roomCode, email
    /// 
    /// 10. CallShareReq (Chia sẻ màn hình)
    ///     - type = "CALL_SHARE"
    ///     - roomCode, sharerName
    /// 
    /// 11. CallFrameMsg (Gửi frame video)
    ///     - type = "CALL_FRAME"
    ///     - roomCode, fromEmail, fromName
    ///     - jpgB64 (frame dạng JPEG base64)
    /// 
    /// 12. ResetRequest (Yêu cầu OTP)
    ///     - type = "RESET_REQUEST"
    ///     - email
    /// 
    /// 13. ResetConfirmReq (Xác nhận reset)
    ///     - type = "RESET_CONFIRM"
    ///     - email, otp, passwordHash (mật khẩu mới)
    /// 
    /// 14. PrivateChatMsg (Chat riêng 1-1)
    ///     - type = "PRIVATE_CHAT"
    ///     - fromEmail, toEmail, message
    /// 
    /// ============================================================================
    /// PHẦN III: RESPONSE CLASSES - DỮ LIỆU TRẢ VỀ
    /// ============================================================================
    /// 
    /// 1. OkRes (Response thành công)
    ///    - ok = true
    ///    - type (loại response)
    ///    - message (thông báo)
    ///    - user (UserDto - thông tin người dùng)
    ///    - token (token đăng nhập)
    ///    - expires (thời gian hết hạn token - ISO8601)
    /// 
    /// 2. ErrRes (Response lỗi)
    ///    - ok = false
    ///    - type = "ERROR"
    ///    - error (thông báo lỗi)
    /// 
    /// 3. GroupChatHistoryRes (Lịch sử chat nhóm)
    ///    - type = "GROUP_CHAT_HISTORY_RES"
    ///    - roomCode
    ///    - messages (List<GroupChatMsgEx>)
    /// 
    /// 4. GroupChatMsgEx (Tin nhắn nhóm với timestamp)
    ///    - Kế thừa từ GroupChatMsg
    ///    - Thêm: sentAt (DateTime)
    /// 
    /// 5. CallJoinRes (Phản hồi CALL_JOIN)
    ///    - ok = true
    ///    - type = "CALL_JOIN"
    ///    - roomCode, roomId, userId
    /// 
    /// 6. CallStateRes (Trạng thái cuộc gọi)
    ///    - type = "CALL_STATE"
    ///    - roomCode
    ///    - members (List<CallMemberDto> - danh sách thành viên online)
    /// 
    /// 7. CallShareRes (Trạng thái chia sẻ)
    ///    - type = "CALL_SHARE"
    ///    - roomCode, sharerName (ai đang chia sẻ)
    /// 
    /// 8. GroupMembersRes (Danh sách thành viên)
    ///    - type = "GROUP_MEMBERS"
    ///    - roomCode, requestId (match request)
    ///    - members (List<GroupMemberDto>)
    /// 
    /// ============================================================================
    /// DTO (DATA TRANSFER OBJECT) CLASSES
    /// ============================================================================
    /// 
    /// UserDto - Thông tin người dùng
    ///    - username, email, fullName, role, avatar
    ///    - gender, birthday
    /// 
    /// CallMemberDto - Thành viên trong cuộc gọi
    ///    - email, name, cameraOn, micOn
    /// 
    /// GroupMemberDto - Thành viên lớp
    ///    - email, fullName, role
    /// 
    /// ============================================================================
    /// USAGE PATTERN
    /// ============================================================================
    /// 
    /// GỬI REQUEST:
    /// ```csharp
    /// var req = new LoginReq { username = "user@email.com", passwordHash = "hash" };
    /// string json = JsonConvert.SerializeObject(req);
    /// await TcpHelper.SendLineAsync(json);
    /// ```
    /// 
    /// NHẬN RESPONSE:
    /// ```csharp
    /// var res = JsonConvert.DeserializeObject<OkRes>(jsonString);
    /// if (res.ok) { /* success */ } else { /* error */ }
    /// ```
    /// 
    /// ============================================================================
    /// </summary>
    public static class MsgType
    {
        public const string REGISTER = "REGISTER";
        public const string LOGIN = "LOGIN";
        public const string LOGOUT = "LOGOUT";
        public const string LOGIN_WITH_TOKEN = "LOGIN_WITH_TOKEN";
        public const string GROUP_CHAT = "GROUP_CHAT";
        public const string GROUP_CHAT_HISTORY_REQ = "GROUP_CHAT_HISTORY_REQ";
        public const string GROUP_CHAT_HISTORY_RES = "GROUP_CHAT_HISTORY_RES";
        public const string GET_GROUP_MEMBERS = "GET_GROUP_MEMBERS";        
        public const string GROUP_MEMBERS = "GROUP_MEMBERS";                

        public const string CALL_JOIN = "CALL_JOIN";
        public const string CALL_LEAVE = "CALL_LEAVE";
        public const string CALL_STATE = "CALL_STATE";
        public const string CALL_SHARE = "CALL_SHARE";
        public const string CALL_FRAME = "CALL_FRAME";


        public const string RESET_REQUEST = "RESET_REQUEST";
        public const string RESET_CONFIRM = "RESET_CONFIRM";
    }

    public class TokenLoginReq
    {
        public string type { get; set; } = MsgType.LOGIN_WITH_TOKEN;
        public string username { get; set; }         // email
        public string token { get; set; }
    }

    public class RegisterReq
    {
        public string type { get; set; } = MsgType.REGISTER;
        public string username { get; set; }
        public string gender { get; set; }
        public string passwordHash { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public string birthday { get; set; }
    }

    public class LoginReq
    {
        public string type { get; set; } = MsgType.LOGIN;
        public string username { get; set; }
        public string passwordHash { get; set; }
    }

    public class UserDto
    {
        public string username { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string fullName { get; set; }
        public string birthday { get; set; }
        public string role { get; set; }
        public string avatar { get; set; }
    }

    public class GroupChatMsg
    {
        public string type { get; set; } = MsgType.GROUP_CHAT;
        public string roomCode { get; set; }
        public string fromEmail { get; set; }
        public string fromName { get; set; }
        public string message { get; set; }
    }

    public class OkRes
    {
        public bool ok { get; set; } = true;
        public string type { get; set; }
        public string message { get; set; }
        public UserDto user { get; set; }
        public string token { get; set; }
        public string expires { get; set; } //ISO8601
    }

    public class ErrRes
    {
        public bool ok { get; set; } = false;
        public string type { get; set; } = "ERROR";
        public string error { get; set; }
    }
  

    public class LogoutReq
    {
        public string type { get; set; } = MsgType.LOGOUT;
        public string username { get; set; }
        public string token { get; set; }
    }

    public class GroupChatHistoryReq
    {
        public string type { get; set; } = MsgType.GROUP_CHAT_HISTORY_REQ;
        public string roomCode { get; set; }
        public int take { get; set; } = 50;   // lấy 50 tin gần nhất
    }

    public class GroupChatHistoryRes
    {
        public string type { get; set; } = MsgType.GROUP_CHAT_HISTORY_RES;
        public string roomCode { get; set; }
        public List<GroupChatMsgEx> messages { get; set; }
    }

    public class GroupChatMsgEx : GroupChatMsg
    {
        public DateTime sentAt { get; set; }
    }

    public class CallJoinReq
    {
        public string type { get; set; } = MsgType.CALL_JOIN;
        public string roomCode { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public int udpPort { get; set; }
    }

    public class CallJoinRes
    {
        public bool ok { get; set; } = true;
        public string type { get; set; } = MsgType.CALL_JOIN;
        public string roomCode { get; set; }
        public int roomId { get; set; }
        public int userId { get; set; }
    }

    public class CallShareReq
    {
        public string type { get; set; } = MsgType.CALL_SHARE;
        public string roomCode { get; set; }
        public string sharerName { get; set; }
    }

    public class CallShareRes
    {
        public string type { get; set; } = MsgType.CALL_SHARE;
        public string roomCode { get; set; }
        public string sharerName { get; set; }
    }

    public class CallLeaveReq
    {
        public string type { get; set; } = MsgType.CALL_LEAVE;
        public string roomCode { get; set; }
        public string email { get; set; }
    }

    public class CallMemberDto
    {
        public string email { get; set; }
        public string name { get; set; }

        public bool cameraOn { get; set; }
        public bool micOn { get; set; }
    }

    public class CallStateRes
    {
        public string type { get; set; } = MsgType.CALL_STATE;
        public string roomCode { get; set; }
        public System.Collections.Generic.List<CallMemberDto> members { get; set; }
    }


    public static partial class MsgTypeExtensions { } // no-op
    
    public class ResetRequest
    {
        public string type { get; set; } = MsgType.RESET_REQUEST;
        public string email { get; set; }
    }

    public class ResetConfirmReq
    {
        public string type { get; set; } = MsgType.RESET_CONFIRM;
        public string email { get; set; }
        public string otp { get; set; }
        public string passwordHash { get; set; }
    }

    public class PrivateChatMsg
    {
        public string type = "PRIVATE_CHAT";
        public string fromEmail { get; set; }
        public string toEmail { get; set; }
        public string message { get; set; }
    }

    public class GroupMemberDto
    {
        public string email { get; set; }
        public string fullName { get; set; }
        public string role { get; set; }
    }

    public class GroupMembersRes
    {
        public string type { get; set; } = "GROUP_MEMBERS";
        public string roomCode { get; set; }
        public List<GroupMemberDto> members { get; set; }
        public string requestId { get; set; }
    }

    public class GetGroupMembersReq
    {
        public string type { get; set; } = "GET_GROUP_MEMBERS";
        public string roomCode { get; set; }
        public string requestId { get; set; }
    }

    public class CallFrameMsg
    {
        public string type { get; set; } = MsgType.CALL_FRAME;
        public string roomCode { get; set; }
        public string fromEmail { get; set; }
        public string fromName { get; set; }
        public string jpgB64 { get; set; }
    }
}
