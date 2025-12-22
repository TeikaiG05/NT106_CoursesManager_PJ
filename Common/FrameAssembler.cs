using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    /// <summary>
    /// ============================================================================
    /// FrameAssembler.cs - Lắp ráp các chunk video thành frame hoàn chỉnh
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Nhận các chunk của 1 frame
    /// Lưu trữ chunks theo frameId
    /// Khi đủ chunks → ghép thành frame hoàn chỉnh
    /// Tự động dọn các frame bị timeout
    /// Thread-safe (sử dụng lock)
    /// 
    /// LÝ DO CẦN FRAME ASSEMBLER:
    /// - Video frames được chia thành nhiều chunks
    /// - Các chunks đến từ UDP (có thể đến không đúng thứ tự)
    /// - Cần ghép lại thành frame JPEG hoàn chỉnh
    /// - Sau đó mới có thể hiển thị
    /// 
    /// ============================================================================
    /// NESTED CLASS: FrameBuf (Frame Buffer)
    /// ============================================================================
    /// 
    /// Lưu trữ thông tin 1 frame đang được lắp ráp:
    /// 
    /// - RoomId: ID phòng (để verify)
    /// - UserId: ID người gửi (để verify)
    /// - FrameId: ID frame (để verify consistency)
    /// - ChunkCount: Tổng số chunks cần nhận
    /// - CreatedAtUtc: Thời gian tạo (để timeout)
    /// - Chunks: Mảng byte[][] chứa từng chunk
    /// - Received: Số chunks đã nhận (để biết khi nào đủ)
    /// 
    /// ============================================================================
    /// PUBLIC METHODS
    /// ============================================================================
    /// 
    /// 1. Push(roomId, userId, frameId, chunkIndex, chunkCount, payload)
    ///    → byte[] (frame hoàn chỉnh hoặc null)
    ///    
    ///    Công việc:
    ///    - Dọn các frame bị timeout (Cleanup_NoLock)
    ///    - Tạo key = (userId, frameId)
    ///    - Nếu key chưa tồn tại:
    ///      * Tạo FrameBuf mới
    ///      * Tạo mảng chunks size = chunkCount
    ///      * Thêm vào dictionary
    ///    - Validate: chunkIndex phải < ChunkCount
    ///    - Nếu chunk tại vị trí chưa có:
    ///      * Lưu payload vào Chunks[chunkIndex]
    ///      * Tăng Received counter
    ///    - Nếu Received < ChunkCount:
    ///      * Chưa đủ → return null (chờ chunks khác)
    ///    - Nếu Received == ChunkCount:
    ///      * Frame hoàn chỉnh!
    ///      * Ghép tất cả chunks lại
    ///      * Xóa khỏi dictionary
    ///      * Return frame hoàn chỉnh
    ///    
    ///    RETURN:
    ///    - null: Chưa đủ chunks
    ///    - byte[]: Frame hoàn chỉnh (JPEG data)
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    var assembler = new FrameAssembler();
    ///    byte[] frame = assembler.Push(roomId, userId, frameId, 
    ///                                 chunkIdx, chunkCnt, payload);
    ///    if (frame != null) {
    ///        // Frame hoàn chỉnh → hiển thị JPEG
    ///    }
    ///    ```
    /// 
    /// 2. FrameTimeout (property)
    ///    - Default = 3 giây
    ///    - Frame bị timeout sẽ bị xóa
    ///    - Để tránh lãng phí memory nếu client ngắt kết nối giữa chừng
    ///    
    /// ============================================================================
    /// PRIVATE METHODS
    /// ============================================================================
    /// 
    /// Cleanup_NoLock()
    ///    - Gọi mỗi khi Push() được gọi
    ///    - Duyệt tất cả frame trong dictionary
    ///    - Nếu (now - CreatedAtUtc) > FrameTimeout:
    ///      * Xóa frame khỏi dictionary
    ///    - Tránh memory leak
    /// 
    /// ============================================================================
    /// THREAD SAFETY
    /// ============================================================================
    /// 
    /// Sử dụng lock để đảm bảo thread-safe:
    /// ```csharp
    /// lock (_lock)
    /// {
    ///     // Mỗi thao tác dictionary đều trong lock
    /// }
    /// ```
    /// 
    /// Các client có thể call Push() đồng thời từ UDP receive loop
    /// Lock đảm bảo không có race condition
    /// !!!Lock có thể là bottleneck nếu có quá nhiều frames
    /// 
    /// ============================================================================
    /// MEMORY MANAGEMENT
    /// ============================================================================
    /// 
    /// STORAGE:
    /// - Dictionary key: (userId, frameId) tuple
    /// - Mỗi frame có Chunks array (size = chunkCount)
    /// - Mỗi chunk có thể lớn (20KB ~ 1MB tùy video quality)
    /// 
    /// CLEANUP:
    /// - Khi frame hoàn chỉnh → xóa ngay
    /// - Khi timeout (3s) → xóa
    /// - Tránh memory leak nếu chunks chậm đến
    /// 
    /// WORST CASE:
    /// - 10 concurrent streams
    /// - 5 chunks/frame
    /// - ~500KB/frame
    /// - Max ~2.5MB at once (usually much less)
    /// 
    /// ============================================================================
    /// USAGE EXAMPLE
    /// ============================================================================
    /// 
    /// ```csharp
    /// private FrameAssembler assembler = new FrameAssembler();
    /// 
    /// // Khi nhận gói UDP:
    /// if (MediaPacket.TryUnpack(data, len, out int roomId, out int userId,
    ///     out int frameId, out ushort chunkIdx, out ushort chunkCnt,
    ///     out byte[] payload, out int payloadLen))
    /// {
    ///     // Thêm chunk vào assembler
    ///     byte[] completeFrame = assembler.Push(roomId, userId, frameId,
    ///         chunkIdx, chunkCnt, payload);
    ///     
    ///     if (completeFrame != null) {
    ///         // Frame đã hoàn chỉnh
    ///         OnFrameReceived?.Invoke(userId, completeFrame);
    ///         // Hiển thị JPEG frame
    ///     }
    /// }
    /// ```
    /// 
    /// ============================================================================
    /// INTERNALS: GHÉP CHUNKS
    /// ============================================================================
    /// 
    /// Khi frame hoàn chỉnh (Received == ChunkCount):
    /// 
    /// 1. Tính tổng kích thước: total = sum(chunk lengths)
    /// 2. Tạo buffer cuối cùng: all = new byte[total]
    /// 3. Ghép từng chunk:
    ///    ```csharp
    ///    int offset = 0;
    ///    for (int i = 0; i < chunks.Length; i++)
    ///    {
    ///        byte[] chunk = chunks[i];
    ///        Buffer.BlockCopy(chunk, 0, all, offset, chunk.Length);
    ///        offset += chunk.Length;
    ///    }
    ///    ```
    /// 4. Return all (frame hoàn chỉnh)
    /// 
    /// ============================================================================
    /// </summary>
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
