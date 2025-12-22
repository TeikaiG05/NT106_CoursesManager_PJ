using System;

namespace Common
{
    /// <summary>
    /// ============================================================================
    /// MediaPacket.cs - Đóng/giải nén gói tin media (video/audio)
    /// ============================================================================
    /// 
    /// CHỨC NĂNG CHÍNH:
    /// Đóng gói (pack) dữ liệu video/audio + metadata
    /// Giải nén (unpack) gói tin nhận được
    /// Định dạng header chuẩn 16 bytes
    /// Hỗ trợ chunking (chia frame thành chunks)
    /// 
    /// TÍNH NĂNG:
    /// - Header cố định 16 bytes
    /// - Payload (dữ liệu) có kích thước linh hoạt
    /// - Hỗ trợ phân đoạn frame thành chunks
    /// - Sử dụng little-endian encoding
    /// 
    /// ============================================================================
    /// HEADER LAYOUT (16 bytes - little-endian)
    /// ============================================================================
    /// 
    /// [0..3]  roomId (int 32-bit)
    ///    → ID phòng cuộc gọi
    ///    → Server dùng để route gói tin đến đúng room
    /// 
    /// [4..7]  userId (int 32-bit)
    ///    → ID người gửi trong room
    ///    → Dùng để định danh sender
    /// 
    /// [8..11] frameId (int 32-bit)
    ///    → ID của frame (để ghép chunks)
    ///    → Client tự tăng để mỗi frame có ID unique
    /// 
    /// [12..13] chunkIndex (ushort 16-bit)
    ///    → Vị trí chunk trong frame (0, 1, 2, ...)
    ///    → Dùng để ghép chunks đúng thứ tự
    /// 
    /// [14..15] chunkCount (ushort 16-bit)
    ///    → Tổng số chunks của frame này
    ///    → Nếu = 0 → dữ liệu audio (không cần ghép)
    /// 
    /// PAYLOAD (dữ liệu)
    ///    → Phần dữ liệu thực (JPEG bytes, PCM bytes, etc)
    ///    → Kích thước linh hoạt
    /// 
    /// VÍ DỤ LAYOUT GÓI TIN:
    /// ```
    /// [16 bytes header] + [payload bytes]
    /// [roomId][userId][frameId][chunkIdx][chunkCnt] + [jpeg data...]
    /// ```
    /// 
    /// ============================================================================
    /// PUBLIC METHODS
    /// ============================================================================
    /// 
    /// 1. Pack(roomId, userId, frameId, chunkIndex, chunkCount, payload, payloadLen)
    ///    → byte[] (gói tin hoàn chỉnh)
    ///    
    ///    Công việc:
    ///    - Validate input: payload null → Empty array
    ///    - payloadLen phải ≤ payload.Length
    ///    - Tạo buffer = HeaderSize + payloadLen bytes
    ///    - WriteInt: roomId, userId, frameId vào buffer
    ///    - WriteUShort: chunkIndex, chunkCount vào buffer
    ///    - Copy payload data vào buffer (từ offset 16)
    ///    - Trả về buffer hoàn chỉnh
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    byte[] jpegData = ...;
    ///    byte[] packet = MediaPacket.Pack(1001, 42, 100, 0, 5, jpegData, jpegData.Length);
    ///    await udpClient.SendAsync(packet, packet.Length, serverEndpoint);
    ///    ```
    /// 
    /// 2. TryUnpack(datagram, len, out roomId, out userId, ...)
    ///    → bool (true nếu unpack thành công)
    ///    
    ///    Công việc:
    ///    - Validate: datagram null hoặc len < HeaderSize → return false
    ///    - ReadInt: lấy roomId, userId, frameId từ datagram
    ///    - ReadUShort: lấy chunkIndex, chunkCount từ datagram
    ///    - Tính payloadLen = len - HeaderSize
    ///    - Copy payload bytes vào mảng
    ///    - out các giá trị
    ///    - Trả về true nếu thành công
    ///    
    ///    USAGE:
    ///    ```csharp
    ///    if (MediaPacket.TryUnpack(buffer, bufferLen,
    ///        out int roomId, out int userId, out int frameId,
    ///        out ushort chunkIdx, out ushort chunkCnt,
    ///        out byte[] payload, out int payloadLen))
    ///    {
    ///        // Xử lý chunk
    ///    }
    ///    ```
    /// 
    /// ============================================================================
    /// PRIVATE HELPER METHODS
    /// ============================================================================
    /// 
    /// WriteInt(byte[] b, int offset, int value)
    ///    - Ghi 4 bytes (little-endian)
    ///    - b[off+0] = byte 0-7
    ///    - b[off+1] = byte 8-15
    ///    - b[off+2] = byte 16-23
    ///    - b[off+3] = byte 24-31
    /// 
    /// ReadInt(byte[] b, int offset) → int
    ///    - Đọc 4 bytes (little-endian)
    ///    - Kết hợp: b[0] | (b[1]<<8) | (b[2]<<16) | (b[3]<<24)
    /// 
    /// WriteUShort(byte[] b, int offset, ushort value)
    ///    - Ghi 2 bytes (little-endian)
    ///    - b[off+0] = byte 0-7
    ///    - b[off+1] = byte 8-15
    /// 
    /// ReadUShort(byte[] b, int offset) → ushort
    ///    - Đọc 2 bytes (little-endian)
    ///    - Kết hợp: b[0] | (b[1]<<8)
    /// 
    /// ============================================================================
    /// CHUNKING STRATEGY (chia frame thành chunks)
    /// ============================================================================
    /// 
    /// VIDEOWHY CHUNKING?
    /// - JPEG frame có thể lớn (100KB+)
    /// - UDP có giới hạn 65KB/packet
    /// - Chia thành chunks nhỏ hơn để gửi riêng rẽ
    /// 
    /// EXAMPLE:
    /// - Frame size: 120KB (JPEG)
    /// - Chunk size: 20KB
    /// - Tổng chunks: 6
    /// - Gửi 6 packets với chunkIndex = 0,1,2,3,4,5
    /// - FrameAssembler nhận và ghép lại
    /// 
    /// AUDIO (không chunking):
    /// - Audio data thường nhỏ hơn (PCM 16-bit)
    /// - Gửi 1 packet với chunkCount = 0
    /// - FrameAssembler nhận trực tiếp không ghép
    /// 
    /// ============================================================================
    /// CONSTANTS
    /// ============================================================================
    /// 
    /// HeaderSize = 16 (bytes)
    /// - Kích thước header cố định
    /// - roomId (4) + userId (4) + frameId (4) + chunkIndex (2) + chunkCount (2)
    /// 
    /// ============================================================================
    /// </summary>
    public static class MediaPacket
    {
        // Header layout (16 bytes):
        // [0..3]  roomId (int)
        // [4..7]  userId (int)
        // [8..11] frameId (int)
        // [12..13] chunkIndex (ushort)
        // [14..15] chunkCount (ushort)
        public const int HeaderSize = 16;

        public static byte[] Pack(int roomId, int userId, int frameId, ushort chunkIndex, ushort chunkCount, byte[] payload, int payloadLen)
        {
            if (payload == null) payload = Array.Empty<byte>();
            if (payloadLen < 0) payloadLen = 0;
            if (payloadLen > payload.Length) payloadLen = payload.Length;

            var buf = new byte[HeaderSize + payloadLen];

            WriteInt(buf, 0, roomId);
            WriteInt(buf, 4, userId);
            WriteInt(buf, 8, frameId);
            WriteUShort(buf, 12, chunkIndex);
            WriteUShort(buf, 14, chunkCount);

            if (payloadLen > 0)
                Buffer.BlockCopy(payload, 0, buf, HeaderSize, payloadLen);

            return buf;
        }

        public static bool TryUnpack(byte[] datagram, int len,
            out int roomId, out int userId, out int frameId,
            out ushort chunkIndex, out ushort chunkCount,
            out byte[] payload, out int payloadLen)
        {
            roomId = userId = frameId = 0;
            chunkIndex = chunkCount = 0;
            payload = null;
            payloadLen = 0;

            if (datagram == null || len < HeaderSize) return false;

            roomId = ReadInt(datagram, 0);
            userId = ReadInt(datagram, 4);
            frameId = ReadInt(datagram, 8);
            chunkIndex = ReadUShort(datagram, 12);
            chunkCount = ReadUShort(datagram, 14);

            payloadLen = len - HeaderSize;
            payload = new byte[payloadLen];
            if (payloadLen > 0)
                Buffer.BlockCopy(datagram, HeaderSize, payload, 0, payloadLen);

            return true;
        }

        private static void WriteInt(byte[] b, int off, int v)
        {
            b[off + 0] = (byte)(v);
            b[off + 1] = (byte)(v >> 8);
            b[off + 2] = (byte)(v >> 16);
            b[off + 3] = (byte)(v >> 24);
        }

        private static int ReadInt(byte[] b, int off)
        {
            return (b[off + 0])
                 | (b[off + 1] << 8)
                 | (b[off + 2] << 16)
                 | (b[off + 3] << 24);
        }

        private static void WriteUShort(byte[] b, int off, ushort v)
        {
            b[off + 0] = (byte)(v);
            b[off + 1] = (byte)(v >> 8);
        }

        private static ushort ReadUShort(byte[] b, int off)
        {
            return (ushort)((b[off + 0]) | (b[off + 1] << 8));
        }
    }
}
