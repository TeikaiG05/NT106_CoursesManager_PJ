using System;
using System.Collections.Generic;

namespace Common
{
    public static class MsgType
    {
        public const string REGISTER = "REGISTER";
        public const string LOGIN = "LOGIN";
        public const string LOGOUT = "LOGOUT";
        public const string LOGIN_WITH_TOKEN = "LOGIN_WITH_TOKEN";
        public const string GROUP_CHAT = "GROUP_CHAT";
        public const string GROUP_CHAT_HISTORY_REQ = "GROUP_CHAT_HISTORY_REQ";
        public const string GROUP_CHAT_HISTORY_RES = "GROUP_CHAT_HISTORY_RES";

        public const string CALL_JOIN = "CALL_JOIN";
        public const string CALL_LEAVE = "CALL_LEAVE";
        public const string CALL_STATE = "CALL_STATE";

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


    public static partial class MsgTypeExtensions { } // no-op (keeps file layout)
    
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
}
