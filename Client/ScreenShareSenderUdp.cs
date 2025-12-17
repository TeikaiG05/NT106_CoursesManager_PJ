using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NT106_BT2
{
    internal sealed class ScreenShareSenderUdp
    {
        private readonly string _roomCode;
        private CancellationTokenSource _cts;
        private int _frameId = 1;

        public int Fps { get; set; } = 2;
        public int MaxWidth { get; set; } = 720;
        public long JpegQuality { get; set; } = 35;

        private const int MaxChunkPayload = 1300;

        public ScreenShareSenderUdp(string roomCode) => _roomCode = roomCode;

        public void Start()
        {
            if (_cts != null) return;
            CallUdp.EnsureStarted();
            _cts = new CancellationTokenSource();
            _ = Loop(_cts.Token);
        }

        public void Stop()
        {
            try { _cts?.Cancel(); } catch { }
            _cts = null;
        }

        private async Task Loop(CancellationToken token)
        {
            int delay = Math.Max(1, 1000 / Math.Max(1, Fps));

            while (!token.IsCancellationRequested)
            {
                try
                {
                    byte[] jpg = CaptureJpeg(MaxWidth, JpegQuality);
                    await SendFrameUdpAsync(jpg, token);
                }
                catch { }

                try { await Task.Delay(delay, token); } catch { break; }
            }
        }

        private async Task SendFrameUdpAsync(byte[] jpg, CancellationToken token)
        {
            if (jpg == null || jpg.Length == 0) return;
            if (CallUdp.RoomId <= 0 || CallUdp.UserId <= 0) return;

            int chunkCount = (int)Math.Ceiling(jpg.Length / (double)MaxChunkPayload);
            if (chunkCount <= 0 || chunkCount > ushort.MaxValue) return;

            int frameId = _frameId;
            _frameId = _frameId == int.MaxValue ? 1 : _frameId + 1;

            for (ushort i = 0; i < chunkCount; i++)
            {
                int offset = i * MaxChunkPayload;
                int len = Math.Min(MaxChunkPayload, jpg.Length - offset);

                var chunk = new byte[len];
                Buffer.BlockCopy(jpg, offset, chunk, 0, len);

                try { await CallUdp.SendFrameChunkAsync(frameId, i, (ushort)chunkCount, chunk, len); }
                catch { }

                if (token.IsCancellationRequested) break;
            }
        }

        private static byte[] CaptureJpeg(int maxWidth, long quality)
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            var bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);
            }

            Image img = bmp;

            if (maxWidth > 0 && bmp.Width > maxWidth)
            {
                int h = (int)(bmp.Height * (maxWidth / (double)bmp.Width));
                var resized = new Bitmap(maxWidth, h, PixelFormat.Format24bppRgb);
                using (var gr = Graphics.FromImage(resized))
                {
                    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                    gr.DrawImage(bmp, 0, 0, maxWidth, h);
                }
                img = resized;
            }

            try
            {
                var ms = new MemoryStream();
                var enc = GetJpegEncoder();
                var ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                img.Save(ms, enc, ep);
                return ms.ToArray();
            }
            finally
            {
                if (!ReferenceEquals(img, bmp)) img.Dispose();
            }
        }

        private static ImageCodecInfo GetJpegEncoder()
        {
            foreach (var c in ImageCodecInfo.GetImageEncoders())
                if (c.MimeType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase))
                    return c;
            return null;
        }
    }
}
