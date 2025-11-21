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
}
