using System;
using System.Threading.Tasks;
using NAudio.Wave;

namespace NT106_BT2
{
    internal sealed class AudioSenderUdp : IDisposable
    {
        private WaveInEvent _waveIn;
        private bool _running;

        private const int SampleRate = 16000;
        private const int Channels = 1;
        private const int BufferMs = 40; // ~25 packets/sec

        public void Start()
        {
            if (_running) return;

            CallUdp.EnsureStarted();

            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(SampleRate, 16, Channels),
                BufferMilliseconds = BufferMs,
                NumberOfBuffers = 3
            };
            _waveIn.DataAvailable += WaveIn_DataAvailable;
            _waveIn.StartRecording();

            _running = true;
        }

        public void Stop()
        {
            if (!_running) return;
            try { _waveIn.DataAvailable -= WaveIn_DataAvailable; } catch { }
            try { _waveIn.StopRecording(); } catch { }
            try { _waveIn.Dispose(); } catch { }
            _waveIn = null;
            _running = false;
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (e.BytesRecorded <= 0) return;
            if (CallUdp.RoomId <= 0 || CallUdp.UserId <= 0) return;

            var payload = new byte[e.BytesRecorded];
            Buffer.BlockCopy(e.Buffer, 0, payload, 0, e.BytesRecorded);

            // chunkCount = 0 => audio packet (handled separately)
            _ = Task.Run(() => CallUdp.SendFrameChunkAsync(0, 0, 0, payload, payload.Length));
        }

        public void Dispose() => Stop();
    }
}
