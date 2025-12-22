using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    /// <summary>
    /// ============================================================================
    /// RoomState.cs - Quản lý phòng chat nhóm và cuộc gọi video
    /// ============================================================================
    /// 
    /// CÓ 3 CLASS CHÍNH:
    /// 1. ClientSession - Thông tin 1 client trong phòng
    /// 2. RoomState - Trạng thái 1 phòng
    /// 3. RoomManager - Quản lý tất cả phòng
    /// 
    /// ============================================================================
    /// CLASS 1: ClientSession (Thông tin 1 client)
    /// ============================================================================
    /// 
    /// PROPERTIES:
    /// - UserId (int): ID unique của client trong phòng (gán bởi RoomManager)
    /// - DisplayName (string): Tên hiển thị (Họ Tên hoặc email)
    /// - RoomId (int): ID phòng mà client đang ở
    /// - Tcp (TcpClient): Connection TCP từ client (để gửi tin TCP)
    /// - UdpEndPoint (IPEndPoint): UDP endpoint của client (để relay video/audio)
    /// 
    /// USAGE:
    /// ```csharp
    /// var sess = new ClientSession {
    ///     UserId = 1,
    ///     DisplayName = "Nguyễn Văn A",
    ///     RoomId = 1000,
    ///     Tcp = tcpClient,
    ///     UdpEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.5"), 5000)
    /// };
    /// ```
    /// 
    /// ============================================================================
    /// CLASS 2: RoomState (Trạng thái 1 phòng)
    /// ============================================================================
    /// 
    /// PROPERTIES:
    /// - RoomId (int): ID phòng (gán bởi RoomManager)
    /// - RoomCode (string): Code phòng (từ client: "class123")
    /// - SharingUser (string): Tên người đang chia sẻ màn hình (empty nếu không có)
    /// - Clients (List<ClientSession>): Danh sách client trong phòng
    /// 
    /// METHODS:
    /// - ParticipantsCsv() → string
    ///    → Trả về tên tất cả client, ngăn cách bằng ";"
    ///    → Ví dụ: "Nguyễn Văn A;Trần Thị B;Lê Văn C"
    ///    → Dùng để log hoặc hiển thị
    /// 
    /// EXAMPLE:
    /// ```csharp
    /// var room = new RoomState {
    ///     RoomId = 1000,
    ///     RoomCode = "class001",
    ///     SharingUser = "",
    ///     Clients = new List<ClientSession>()
    /// };
    /// room.Clients.Add(sess1);
    /// room.Clients.Add(sess2);
    /// var csv = room.ParticipantsCsv(); // "Alice;Bob"
    /// ```
    /// 
    /// ============================================================================
    /// CLASS 3: RoomManager (Quản lý tất cả phòng)
    /// ============================================================================
    /// 
    /// CHỨC NĂNG:
    /// Tạo phòng mới hoặc tìm phòng có sẵn
    /// Thêm client vào phòng
    /// Xóa client khỏi phòng
    /// Tìm phòng bằng roomId hoặc roomCode
    /// Tìm client bằng UDP endpoint
    /// Quản lý chia sẻ màn hình
    /// 
    /// THREAD SAFETY:
    /// - Sử dụng lock để đảm bảo thread-safe
    /// - Tất cả public methods đều thread-safe
    /// 
    /// BIẾN TOÀN CỤC:
    /// - _byCode: Dictionary<roomCode, RoomState>
    /// - _byId: Dictionary<roomId, RoomState>
    /// - _nextRoomId: Counter từ 1000 (1000, 1001, 1002, ...)
    /// - _nextUserId: Counter từ 1 (1, 2, 3, ...)
    /// 
    /// 1. JoinOrCreate(roomCode, displayName, tcp, udpEp)
    ///    → (RoomState room, int userId)
    ///    
    ///    Công việc:
    ///    - Tìm phòng có code = roomCode
    ///    - Nếu chưa có:
    ///      * Tạo RoomState mới
    ///      * Gán RoomId = _nextRoomId++ (1000, 1001, ...)
    ///      * Thêm vào _byCode và _byId
    ///    - Tạo ClientSession mới
    ///      * Gán UserId = _nextUserId++ (1, 2, 3, ...)
    ///      * Set DisplayName, RoomId, Tcp, UdpEndPoint
    ///    - Thêm session vào room.Clients
    ///    - Trả về (room, userId)
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    var roomMgr = new RoomManager();
    ///    var (room, userId) = roomMgr.JoinOrCreate(
    ///        "class001", 
    ///        "Nguyễn Văn A",
    ///        tcpClient,
    ///        new IPEndPoint(IPAddress.Parse("192.168.1.5"), 5000)
    ///    );
    ///    // room.RoomId = 1000, userId = 1
    ///    // Lần sau join cùng roomCode → sẽ lấy phòng cũ, userId mới
    ///    ```
    /// 
    /// 2. Leave(TcpClient tcp)
    ///    - Tìm session có tcp = this tcp
    ///    - Xóa session khỏi room.Clients
    ///    - Nếu SharingUser là người vừa rời: reset SharingUser = ""
    ///    - Nếu room không còn client nào:
    ///      * Xóa room khỏi _byId
    ///      * Xóa room khỏi _byCode
    ///    - Dùng lúc client disconnect
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    roomMgr.Leave(tcpClient);
    ///    // Đã xóa client khỏi phòng
    ///    // Nếu phòng rỗng → xóa phòng
    ///    ```
    /// 
    /// 3. GetRoomById(int roomId) → RoomState
    ///    - Tìm room có RoomId = roomId
    ///    - Trả về room hoặc null nếu không tìm thấy
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    var room = roomMgr.GetRoomById(1000);
    ///    if (room != null) {
    ///        // Xử lý phòng
    ///    }
    ///    ```
    /// 
    /// 4. FindByUdpEndpoint(IPEndPoint ep) → ClientSession
    ///    - Tìm session có UdpEndPoint match
    ///    - Dùng để relay UDP: nhận gói từ ep → gửi đến các session khác
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    var sess = roomMgr.FindByUdpEndpoint(sourceIpEndPoint);
    ///    if (sess != null) {
    ///        // Biết ai gửi gói UDP → relay đến session khác
    ///    }
    ///    ```
    /// 
    /// 5. UpdateSharing(int roomId, string displayNameOrEmpty)
    ///    - Set SharingUser = displayName (ai đang chia sẻ)
    ///    - Nếu displayName rỗng: reset (không ai chia sẻ)
    ///    - Dùng lúc user bật/tắt chia sẻ màn hình
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    roomMgr.UpdateSharing(1000, "Nguyễn Văn A");
    ///    // Nguyễn Văn A bắt đầu chia sẻ
    ///    
    ///    roomMgr.UpdateSharing(1000, "");
    ///    // Không ai chia sẻ nữa
    ///    ```
    /// 
    /// 6. GetRoomClients(int roomId) → List<ClientSession>
    ///    - Lấy copy của danh sách client trong phòng
    ///    - Thread-safe (snapshot của list)
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    var clients = roomMgr.GetRoomClients(1000);
    ///    foreach (var c in clients) {
    ///        // Gửi tin tới từng client
    ///    }
    ///    ```
    /// 
    /// ============================================================================
    /// LIFECYCLE: Join → In Room → Leave
    /// ============================================================================
    /// 
    /// JOIN:
    /// 1. Client gửi CALL_JOIN (roomCode, udpPort)
    /// 2. Server gọi roomMgr.JoinOrCreate()
    /// 3. Phòng được tạo (lần đầu) hoặc reuse (lần sau)
    /// 4. Server gửi CALL_JOIN response (roomId, userId)
    /// 5. Client lưu roomId, userId để dùng cho UDP
    /// 
    /// IN ROOM:
    /// - Client nhận CallStateRes (danh sách members)
    /// - Client gửi CALL_FRAME qua UDP
    /// - Server relay frame từ UDP source → các client khác
    /// - User bật chia sẻ → gửi CALL_SHARE
    /// 
    /// LEAVE:
    /// 1. Client gửi CALL_LEAVE hoặc disconnect
    /// 2. Server gọi roomMgr.Leave(tcp)
    /// 3. Client bị xóa khỏi phòng
    /// 4. Nếu phòng rỗng → xóa phòng
    /// 5. Server broadcast CALL_STATE update (member list thay đổi)
    /// 
    /// ============================================================================
    /// MEMORY MANAGEMENT
    /// ============================================================================
    /// 
    /// PHÒNG BỊ XÓA KHI:
    /// - Tất cả client disconnect
    /// - Server tắt
    /// 
    /// GIÁM SÁT MEMORY:
    /// - Mỗi phòng: ~1KB (RoomState object)
    /// - Mỗi client: ~0.5KB (ClientSession object)
    /// - Worst case: 100 phòng × 10 client = ~550KB (negligible)
    /// 
    /// ============================================================================
    /// </summary>
    public sealed class ClientSession
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public int RoomId { get; set; }

        public TcpClient Tcp { get; set; }
        public IPEndPoint UdpEndPoint { get; set; }
    }

    public sealed class RoomState
    {
        public int RoomId { get; set; }
        public string RoomCode { get; set; }

        public string SharingUser { get; set; }
        public List<ClientSession> Clients { get; } = new List<ClientSession>();

        public string ParticipantsCsv()
        {
            return string.Join(";", Clients.Select(c => c.DisplayName));
        }
    }

    public sealed class RoomManager
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, RoomState> _byCode = new Dictionary<string, RoomState>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, RoomState> _byId = new Dictionary<int, RoomState>();
        private int _nextRoomId = 1000;
        private int _nextUserId = 1;

        public (RoomState room, int userId) JoinOrCreate(string roomCode, string displayName, TcpClient tcp, IPEndPoint udpEp)
        {
            lock (_lock)
            {
                if (!_byCode.TryGetValue(roomCode, out var room))
                {
                    room = new RoomState
                    {
                        RoomId = _nextRoomId++,
                        RoomCode = roomCode,
                        SharingUser = ""
                    };
                    _byCode[roomCode] = room;
                    _byId[room.RoomId] = room;
                }

                int userId = _nextUserId++;

                var sess = new ClientSession
                {
                    UserId = userId,
                    DisplayName = displayName,
                    RoomId = room.RoomId,
                    Tcp = tcp,
                    UdpEndPoint = udpEp
                };

                room.Clients.Add(sess);
                return (room, userId);
            }
        }

        public void Leave(TcpClient tcp)
        {
            lock (_lock)
            {
                foreach (var room in _byId.Values.ToList())
                {
                    var found = room.Clients.FirstOrDefault(c => c.Tcp == tcp);
                    if (found != null)
                    {
                        room.Clients.Remove(found);
                        if (!string.IsNullOrWhiteSpace(room.SharingUser) && room.SharingUser == found.DisplayName)
                            room.SharingUser = "";

                        if (room.Clients.Count == 0)
                        {
                            _byId.Remove(room.RoomId);
                            _byCode.Remove(room.RoomCode);
                        }
                        return;
                    }
                }
            }
        }

        public RoomState GetRoomById(int roomId)
        {
            lock (_lock)
            {
                _byId.TryGetValue(roomId, out var room);
                return room;
            }
        }

        public ClientSession FindByUdpEndpoint(IPEndPoint ep)
        {
            lock (_lock)
            {
                foreach (var room in _byId.Values)
                {
                    var found = room.Clients.FirstOrDefault(c =>
                        c.UdpEndPoint != null &&
                        c.UdpEndPoint.Address.Equals(ep.Address) &&
                        c.UdpEndPoint.Port == ep.Port);

                    if (found != null) return found;
                }
                return null;
            }
        }

        public void UpdateSharing(int roomId, string displayNameOrEmpty)
        {
            lock (_lock)
            {
                if (_byId.TryGetValue(roomId, out var room))
                {
                    room.SharingUser = displayNameOrEmpty ?? "";
                }
            }
        }

        public List<ClientSession> GetRoomClients(int roomId)
        {
            lock (_lock)
            {
                if (_byId.TryGetValue(roomId, out var room))
                    return room.Clients.ToList();

                return new List<ClientSession>();
            }
        }
    }
}
