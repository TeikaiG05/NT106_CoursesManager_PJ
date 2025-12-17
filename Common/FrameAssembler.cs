using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public sealed class FrameAssembler
    {
        private sealed class FrameBuf
        {
            public int RoomId;
            public int UserId;
            public int FrameId;
            public int ChunkCount;
            public DateTime CreatedAtUtc = DateTime.UtcNow;
            public byte[][] Chunks;
            public int Received;
        }

        private readonly object _lock = new object();
        private readonly Dictionary<(int userId, int frameId), FrameBuf> _frames = new Dictionary<(int, int), FrameBuf>();

        // timeout để dọn frame dở dang
        public TimeSpan FrameTimeout { get; set; } = TimeSpan.FromSeconds(3);

        public byte[] Push(int roomId, int userId, int frameId, ushort chunkIndex, ushort chunkCount, byte[] payload)
        {
            lock (_lock)
            {
                Cleanup_NoLock();

                var key = (userId, frameId);

                if (!_frames.TryGetValue(key, out var fb))
                {
                    fb = new FrameBuf
                    {
                        RoomId = roomId,
                        UserId = userId,
                        FrameId = frameId,
                        ChunkCount = chunkCount,
                        Chunks = new byte[chunkCount][]
                    };
                    _frames[key] = fb;
                }

                if (chunkIndex >= fb.Chunks.Length) return null;

                if (fb.Chunks[chunkIndex] == null)
                {
                    fb.Chunks[chunkIndex] = payload ?? Array.Empty<byte>();
                    fb.Received++;
                }

                if (fb.Received < fb.ChunkCount) return null;

                // complete
                int total = fb.Chunks.Sum(c => c?.Length ?? 0);
                var all = new byte[total];
                int off = 0;
                for (int i = 0; i < fb.Chunks.Length; i++)
                {
                    var c = fb.Chunks[i] ?? Array.Empty<byte>();
                    Buffer.BlockCopy(c, 0, all, off, c.Length);
                    off += c.Length;
                }

                _frames.Remove(key);
                return all;
            }
        }

        private void Cleanup_NoLock()
        {
            var now = DateTime.UtcNow;
            var dead = _frames
                .Where(kv => now - kv.Value.CreatedAtUtc > FrameTimeout)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var k in dead) _frames.Remove(k);
        }
    }
}
