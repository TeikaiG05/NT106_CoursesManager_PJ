using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using NAudio.Wave;

namespace NT106_BT2
{
    public partial class LobbyForm : Form
    {
        private readonly string roomCode;
        private readonly string roomName;
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private bool micOn = false;
        private bool shareOn = false;
        private bool leftCall = false;

        private Label lblShareInfo;
        private PictureBox picShare;
        private ScreenShareSenderUdp shareSender;
        private AudioSenderUdp audioSender;
        private WaveOutEvent waveOut;
        private BufferedWaveProvider audioBuffer;

        public LobbyForm(string roomCode, string roomName)
        {
            InitializeComponent();

            this.roomCode = roomCode;
            this.roomName = roomName;

            Text = $"Meeting in \"{roomName}\"";

            Load += LobbyForm_Load;
            btnMic.Click += btnMic_Click;
            btnShare.Click += btnShare_Click;
            btnLeave.Click += btnLeave_Click;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _ = SendLeaveIfNeededAsync(); // fire-and-forget để giải phóng khi user đóng cửa sổ
        }

        private void LobbyForm_Load(object sender, EventArgs e)
        {
            splitContainer2.Orientation = Orientation.Horizontal;
            splitContainer2.FixedPanel = FixedPanel.Panel2;
            splitContainer2.SplitterWidth = 4;
            splitContainer2.IsSplitterFixed = false;
            EnableDrag(pnlTop);

            pnlShare.BackColor = Color.Black;

            picShare = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };
            pnlShare.Controls.Add(picShare);
            picShare.BringToFront();

            lblShareInfo = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f),
                Text = "No one is sharing screen"
            };
            pnlShare.Controls.Add(lblShareInfo);
            lblShareInfo.BringToFront();

            CallUdp.OnFrameReceived += CallUdp_OnFrameReceived;
            shareSender = new ScreenShareSenderUdp(roomCode);
            audioSender = new AudioSenderUdp();
            InitAudioPlayback();
            CallUdp.OnAudioReceived += CallUdp_OnAudioReceived;
            if (micOn) audioSender.Start();

            flpParticipants.AutoScroll = true;
            flpParticipants.WrapContents = false; // keep a single row, horizontal scroll if overflow
            flpParticipants.FlowDirection = FlowDirection.LeftToRight;

            string me = Session.FullName ?? Session.Email;
            SetParticipants(new[] { me });
            TcpHelper.OnMessageReceived += Tcp_OnMessageReceived;

            UpdateMicButtonUI();
            UpdateShareButtonUI();
        }

        #region Move form
        private void EnableDrag(Control dragArea)
        {
            dragArea.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }
            };
        }
        #endregion

        #region Public API – ChatPage gọi sang

        public void SetParticipants(IEnumerable<string> displayNames)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IEnumerable<string>>(SetParticipants), displayNames);
                return;
            }

            flpParticipants.SuspendLayout();
            flpParticipants.Controls.Clear();

            if (displayNames != null)
            {
                foreach (var name in displayNames)
                {
                    if (string.IsNullOrWhiteSpace(name)) continue;

                    var tile = new ParticipantTile
                    {
                        Width = 120,
                        Height = 120,
                        Margin = new Padding(8),
                        DisplayName = name,
                        AvatarUrl = string.Equals(name?.Trim(), (Session.FullName ?? Session.Email)?.Trim(), StringComparison.OrdinalIgnoreCase)
                            ? (Session.Avatar ?? Defaults.DefaultAvatarUrl)
                            : Defaults.DefaultAvatarUrl
                    };

                    flpParticipants.Controls.Add(tile);
                }
            }

            flpParticipants.ResumeLayout();
        }

        public void SetSharingUser(string sharerName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetSharingUser), sharerName);
                return;
            }

            if (string.IsNullOrWhiteSpace(sharerName))
                lblShareInfo.Text = "No one is sharing screen";
            else
                lblShareInfo.Text = $"Screen shared by {sharerName}";
        }

        #endregion

        #region Button events

        private void btnMic_Click(object sender, EventArgs e)
        {
            micOn = !micOn;
            UpdateMicButtonUI();
            ApplyMicState();
        }

        private async void btnShare_Click(object sender, EventArgs e)
        {
            shareOn = !shareOn;
            UpdateShareButtonUI();

            string me = Session.FullName ?? Session.Email;

            try
            {
                await TcpHelper.SendCallShareAsync(roomCode, shareOn ? me : "");
                SetSharingUser(shareOn ? me : "");
                if (shareOn) shareSender.Start();
                else shareSender.Stop();
            }
            catch { }
        }

        private async void btnLeave_Click(object sender, EventArgs e)
        {
            await SendLeaveIfNeededAsync();
            Close();
        }

        private async Task SendLeaveIfNeededAsync()
        {
            if (leftCall) return;
            leftCall = true;

            try { await TcpHelper.SendCallLeaveAsync(roomCode); } catch { }
            try { shareSender?.Stop(); } catch { }
            try { audioSender?.Stop(); } catch { }
            try { CallUdp.OnFrameReceived -= CallUdp_OnFrameReceived; } catch { }
            try { CallUdp.OnAudioReceived -= CallUdp_OnAudioReceived; } catch { }
            try { waveOut?.Stop(); waveOut?.Dispose(); } catch { }
            audioBuffer = null;
            CallUdp.Stop();
            TcpHelper.OnMessageReceived -= Tcp_OnMessageReceived;
        }

        #endregion

        #region Update button UI

        private void UpdateMicButtonUI()
        {
            if (micOn)
            {
                btnMic.BackColor = Color.FromArgb(255, 197, 140, 255);
                btnMic.ForeColor = Color.Black;
                btnMic.Text = "  Mic";
            }
            else
            {
                btnMic.BackColor = Color.DarkGray;
                btnMic.ForeColor = Color.White;
                btnMic.Text = " Muted";
            }
        }

        private void ApplyMicState()
        {
            if (micOn) audioSender?.Start();
            else audioSender?.Stop();
        }

        private void UpdateShareButtonUI()
        {
            if (shareOn)
            {
                btnShare.BackColor = Color.FromArgb(255, 197, 140, 255);
                btnShare.ForeColor = Color.Black;
            }
            else
            {
                btnShare.BackColor = Color.WhiteSmoke;
                btnShare.ForeColor = Color.Black;
            }
        }

        #endregion

        #region Dispose
        private void InitAudioPlayback()
        {
            try
            {
                audioBuffer = new BufferedWaveProvider(new WaveFormat(16000, 1))
                {
                    DiscardOnBufferOverflow = true
                };
                waveOut = new WaveOutEvent();
                waveOut.Init(audioBuffer);
                waveOut.Play();
            }
            catch { }
        }

        private void CallUdp_OnAudioReceived(int fromUserId, byte[] pcm)
        {
            if (IsDisposed) return;
            if (fromUserId == CallUdp.UserId) return; // ignore echo
            if (pcm == null || pcm.Length == 0) return;
            if (InvokeRequired) { BeginInvoke(new Action(() => CallUdp_OnAudioReceived(fromUserId, pcm))); return; }

            try { audioBuffer?.AddSamples(pcm, 0, pcm.Length); } catch { }
        }

        private void CallUdp_OnFrameReceived(int fromUserId, byte[] jpeg)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { BeginInvoke(new Action(() => CallUdp_OnFrameReceived(fromUserId, jpeg))); return; }

            try
            {
                using (var ms = new MemoryStream(jpeg))
                using (var img = Image.FromStream(ms))
                {
                    // clone để tránh stream dispose issue
                    var clone = new Bitmap(img);
                    var old = picShare.Image;
                    picShare.Image = clone;
                    old?.Dispose();
                }

                lblShareInfo.Visible = false;
                picShare.BringToFront();
            }
            catch { }
        }

        private void Tcp_OnMessageReceived(string json)
        {
            if (IsDisposed) return;
            if (InvokeRequired) { BeginInvoke(new Action(() => Tcp_OnMessageReceived(json))); return; }

            Newtonsoft.Json.Linq.JObject obj;
            try { obj = Newtonsoft.Json.Linq.JObject.Parse(json); }
            catch { return; }

            var type = ((string)obj["type"] ?? "").Trim();
            if (!type.Equals(Common.MsgType.CALL_FRAME, StringComparison.OrdinalIgnoreCase))
                return;

            var room = ((string)obj["roomCode"] ?? "").Trim();
            if (!string.Equals(room, roomCode, StringComparison.OrdinalIgnoreCase))
                return;

            // sender uses jpgB64; keep backward compatibility with previous key name
            var b64 = (string)obj["jpgB64"] ?? (string)obj["jpegBase64"];
            if (string.IsNullOrWhiteSpace(b64)) return;

            try
            {
                byte[] bytes = Convert.FromBase64String(b64);
                using (var ms = new MemoryStream(bytes))
                using (var img = Image.FromStream(ms))
                {
                    var bmp = new Bitmap(img);
                    var old = picShare.Image;
                    picShare.Image = bmp;
                    old?.Dispose();
                }

                lblShareInfo.Visible = false;
                picShare.BringToFront();
            }
            catch
            {
            }
        }


        #endregion
    }
}
