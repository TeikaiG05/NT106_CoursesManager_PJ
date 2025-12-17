using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server
{
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
