using System;

namespace Common
{
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
