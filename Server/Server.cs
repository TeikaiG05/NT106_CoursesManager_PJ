using System;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
        private TcpServer server;
        private UdpRelayServer udp;

        private const int TCP_PORT = 8080;
        private const int UDP_PORT = 9001;

        public Server()
        {
            InitializeComponent();

            lvLog.View = View.Details;
            lvLog.FullRowSelect = true;
            lvLog.GridLines = true;
            lvLog.Columns.Add("Time", 120);
            lvLog.Columns.Add("Source", 120);
            lvLog.Columns.Add("Message", 600);

            server = new TcpServer(Log);

            udp = new UdpRelayServer(UDP_PORT, server.Rooms);

            this.FormClosing += Server_FormClosing;
        }

        private void Log(string source, string message)
        {
            if (lvLog.IsDisposed) return;
            if (lvLog.InvokeRequired)
            {
                lvLog.BeginInvoke(new Action(() => AddRow(source, message)));
            }
            else
            {
                AddRow(source, message);
            }
        }

        private void AddRow(string source, string message)
        {
            var it = new ListViewItem(DateTime.Now.ToString("HH:mm:ss"));
            it.SubItems.Add(source ?? "");
            it.SubItems.Add(message ?? "");
            lvLog.Items.Add(it);
            it.EnsureVisible();
            if (lvLog.Items.Count > 1000) lvLog.Items.RemoveAt(0);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start(TCP_PORT);
            udp.Start();
            Log("Server", $"Started TCP :{TCP_PORT}, UDP :{UDP_PORT}");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            udp.Stop();
            server.Stop();
            Log("Server", "Stopped");
        }

        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { udp?.Stop(); } catch { }
            try { server?.Stop(); } catch { }
        }
    }
}
