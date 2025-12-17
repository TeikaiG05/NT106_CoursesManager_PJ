using Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal sealed class UdpRelayServer
    {
        private readonly int _port;
        private readonly RoomManager _rooms;

        private UdpClient _udp;
        private CancellationTokenSource _cts;

        public UdpRelayServer(int port, RoomManager rooms)
        {
            _port = port;
            _rooms = rooms;
        }

        public void Start()
        {
            if (_udp != null) return;

            _udp = new UdpClient(_port);
            _cts = new CancellationTokenSource();
            _ = Loop(_cts.Token);
        }

        public void Stop()
        {
            try { _cts?.Cancel(); } catch { }
            try { _udp?.Close(); } catch { }
            _udp = null;
            _cts = null;
        }

        private async Task Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _udp != null)
            {
                UdpReceiveResult r;
                try { r = await _udp.ReceiveAsync(); }
                catch { break; }

                var data = r.Buffer;
                int len = data?.Length ?? 0;
                if (len < MediaPacket.HeaderSize) continue;

                if (!MediaPacket.TryUnpack(data, len, out int roomId, out int userId, out int frameId,
                    out ushort chunkIndex, out ushort chunkCount, out _, out _))
                {
                    continue;
                }

                var clients = _rooms.GetRoomClients(roomId);
                if (clients == null || clients.Count == 0) continue;

                foreach (var c in clients)
                {
                    // Không gửi lại cho chính người gửi (so theo userId)
                    if (c.UserId == userId) continue;
                    if (c.UdpEndPoint == null) continue;

                    try { await _udp.SendAsync(data, len, c.UdpEndPoint); } catch { }
                }
            }
        }
    }
}
