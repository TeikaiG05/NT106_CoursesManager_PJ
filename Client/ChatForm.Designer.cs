namespace NT106_BT2
{
    partial class ChatForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bangChinh = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nutDong = new Guna.UI2.WinForms.Guna2ControlBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chiaChinh = new System.Windows.Forms.SplitContainer();
            this.khungDanhSachChat = new System.Windows.Forms.Panel();
            this.luongChatItems = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.oTimKiem = new Guna.UI2.WinForms.Guna2TextBox();
            this.khungChatHienTai = new System.Windows.Forms.Panel();
            this.khungNhapTin = new System.Windows.Forms.Panel();
            this.nutEmoji = new Guna.UI2.WinForms.Guna2Button();
            this.nutDinhKem = new Guna.UI2.WinForms.Guna2Button();
            this.nutGui = new Guna.UI2.WinForms.Guna2Button();
            this.oNhapTin = new Guna.UI2.WinForms.Guna2TextBox();
            this.khungTinNhan = new System.Windows.Forms.FlowLayoutPanel();
            this.duongKe1 = new Guna.UI2.WinForms.Guna2Separator();
            this.tieuDeChat = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.nutRoiChat = new Guna.UI2.WinForms.Guna2Button();
            this.nutQuanLyApp = new Guna.UI2.WinForms.Guna2Button();
            this.nutCopyLink = new Guna.UI2.WinForms.Guna2Button();
            this.nutTatTieng = new Guna.UI2.WinForms.Guna2Button();
            this.tieuDeTuyChon = new System.Windows.Forms.Label();
            this.luongThanhVien = new System.Windows.Forms.FlowLayoutPanel();
            this.tieuDeChiTiet = new System.Windows.Forms.Label();
            this.bangChinh.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chiaChinh)).BeginInit();
            this.chiaChinh.Panel1.SuspendLayout();
            this.chiaChinh.Panel2.SuspendLayout();
            this.chiaChinh.SuspendLayout();
            this.khungDanhSachChat.SuspendLayout();
            this.luongChatItems.SuspendLayout();
            this.khungChatHienTai.SuspendLayout();
            this.khungNhapTin.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // bangChinh
            // 
            this.bangChinh.ColumnCount = 3;
            this.bangChinh.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.bangChinh.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bangChinh.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 188F));
            this.bangChinh.Controls.Add(this.panel1, 0, 0);
            this.bangChinh.Controls.Add(this.panel2, 1, 0);
            this.bangChinh.Controls.Add(this.panel3, 2, 0);
            this.bangChinh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bangChinh.Location = new System.Drawing.Point(0, 0);
            this.bangChinh.Margin = new System.Windows.Forms.Padding(2);
            this.bangChinh.Name = "bangChinh";
            this.bangChinh.RowCount = 1;
            this.bangChinh.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bangChinh.Size = new System.Drawing.Size(900, 569);
            this.bangChinh.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.panel1.Controls.Add(this.nutDong);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(38, 569);
            this.panel1.TabIndex = 0;
            // 
            // nutDong
            // 
            this.nutDong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nutDong.FillColor = System.Drawing.Color.Transparent;
            this.nutDong.IconColor = System.Drawing.Color.Black;
            this.nutDong.Location = new System.Drawing.Point(4, 4);
            this.nutDong.Margin = new System.Windows.Forms.Padding(2);
            this.nutDong.Name = "nutDong";
            this.nutDong.Size = new System.Drawing.Size(30, 24);
            this.nutDong.TabIndex = 0;
            this.nutDong.Click += new System.EventHandler(this.nutDong_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.chiaChinh);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(38, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(674, 569);
            this.panel2.TabIndex = 1;
            // 
            // chiaChinh
            // 
            this.chiaChinh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chiaChinh.Location = new System.Drawing.Point(0, 0);
            this.chiaChinh.Margin = new System.Windows.Forms.Padding(2);
            this.chiaChinh.Name = "chiaChinh";
            // 
            // chiaChinh.Panel1
            // 
            this.chiaChinh.Panel1.Controls.Add(this.khungDanhSachChat);
            // 
            // chiaChinh.Panel2
            // 
            this.chiaChinh.Panel2.Controls.Add(this.khungChatHienTai);
            this.chiaChinh.Size = new System.Drawing.Size(674, 569);
            this.chiaChinh.SplitterDistance = 224;
            this.chiaChinh.SplitterWidth = 3;
            this.chiaChinh.TabIndex = 0;
            // 
            // khungDanhSachChat
            // 
            this.khungDanhSachChat.Controls.Add(this.luongChatItems);
            this.khungDanhSachChat.Controls.Add(this.oTimKiem);
            this.khungDanhSachChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.khungDanhSachChat.Location = new System.Drawing.Point(0, 0);
            this.khungDanhSachChat.Margin = new System.Windows.Forms.Padding(2);
            this.khungDanhSachChat.Name = "khungDanhSachChat";
            this.khungDanhSachChat.Size = new System.Drawing.Size(224, 569);
            this.khungDanhSachChat.TabIndex = 0;
            // 
            // luongChatItems
            // 
            this.luongChatItems.AutoScroll = true;
            this.luongChatItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.luongChatItems.Controls.Add(this.label1);
            this.luongChatItems.Controls.Add(this.guna2Separator1);
            this.luongChatItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.luongChatItems.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.luongChatItems.Location = new System.Drawing.Point(0, 0);
            this.luongChatItems.Margin = new System.Windows.Forms.Padding(2);
            this.luongChatItems.Name = "luongChatItems";
            this.luongChatItems.Padding = new System.Windows.Forms.Padding(8);
            this.luongChatItems.Size = new System.Drawing.Size(224, 569);
            this.luongChatItems.TabIndex = 1;
            this.luongChatItems.WrapContents = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Chat";
            // 
            // guna2Separator1
            // 
            this.guna2Separator1.Location = new System.Drawing.Point(10, 34);
            this.guna2Separator1.Margin = new System.Windows.Forms.Padding(2);
            this.guna2Separator1.Name = "guna2Separator1";
            this.guna2Separator1.Size = new System.Drawing.Size(210, 10);
            this.guna2Separator1.TabIndex = 2;
            // 
            // oTimKiem
            // 
            this.oTimKiem.BorderRadius = 15;
            this.oTimKiem.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.oTimKiem.DefaultText = "";
            this.oTimKiem.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.oTimKiem.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.oTimKiem.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.oTimKiem.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.oTimKiem.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.oTimKiem.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.oTimKiem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.oTimKiem.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.oTimKiem.Location = new System.Drawing.Point(8, 8);
            this.oTimKiem.Margin = new System.Windows.Forms.Padding(2);
            this.oTimKiem.Name = "oTimKiem";
            this.oTimKiem.PlaceholderText = "Search";
            this.oTimKiem.SelectedText = "";
            this.oTimKiem.Size = new System.Drawing.Size(210, 29);
            this.oTimKiem.TabIndex = 0;
            // 
            // khungChatHienTai
            // 
            this.khungChatHienTai.Controls.Add(this.khungNhapTin);
            this.khungChatHienTai.Controls.Add(this.khungTinNhan);
            this.khungChatHienTai.Controls.Add(this.duongKe1);
            this.khungChatHienTai.Controls.Add(this.tieuDeChat);
            this.khungChatHienTai.Dock = System.Windows.Forms.DockStyle.Fill;
            this.khungChatHienTai.Location = new System.Drawing.Point(0, 0);
            this.khungChatHienTai.Margin = new System.Windows.Forms.Padding(2);
            this.khungChatHienTai.Name = "khungChatHienTai";
            this.khungChatHienTai.Size = new System.Drawing.Size(447, 569);
            this.khungChatHienTai.TabIndex = 0;
            // 
            // khungNhapTin
            // 
            this.khungNhapTin.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.khungNhapTin.Controls.Add(this.nutEmoji);
            this.khungNhapTin.Controls.Add(this.nutDinhKem);
            this.khungNhapTin.Controls.Add(this.nutGui);
            this.khungNhapTin.Controls.Add(this.oNhapTin);
            this.khungNhapTin.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.khungNhapTin.Location = new System.Drawing.Point(0, 488);
            this.khungNhapTin.Margin = new System.Windows.Forms.Padding(2);
            this.khungNhapTin.Name = "khungNhapTin";
            this.khungNhapTin.Size = new System.Drawing.Size(447, 81);
            this.khungNhapTin.TabIndex = 3;
            // 
            // nutEmoji
            // 
            this.nutEmoji.BorderRadius = 10;
            this.nutEmoji.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.nutEmoji.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.nutEmoji.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.nutEmoji.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.nutEmoji.FillColor = System.Drawing.Color.Transparent;
            this.nutEmoji.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nutEmoji.ForeColor = System.Drawing.Color.Black;
            this.nutEmoji.Location = new System.Drawing.Point(38, 53);
            this.nutEmoji.Margin = new System.Windows.Forms.Padding(2);
            this.nutEmoji.Name = "nutEmoji";
            this.nutEmoji.Size = new System.Drawing.Size(22, 24);
            this.nutEmoji.TabIndex = 3;
            // 
            // nutDinhKem
            // 
            this.nutDinhKem.BorderRadius = 10;
            this.nutDinhKem.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.nutDinhKem.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.nutDinhKem.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.nutDinhKem.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.nutDinhKem.FillColor = System.Drawing.Color.Transparent;
            this.nutDinhKem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nutDinhKem.ForeColor = System.Drawing.Color.Black;
            this.nutDinhKem.Location = new System.Drawing.Point(8, 53);
            this.nutDinhKem.Margin = new System.Windows.Forms.Padding(2);
            this.nutDinhKem.Name = "nutDinhKem";
            this.nutDinhKem.Size = new System.Drawing.Size(22, 24);
            this.nutDinhKem.TabIndex = 2;
            // 
            // nutGui
            // 
            this.nutGui.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nutGui.BorderRadius = 15;
            this.nutGui.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.nutGui.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.nutGui.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.nutGui.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.nutGui.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nutGui.ForeColor = System.Drawing.Color.White;
            this.nutGui.Location = new System.Drawing.Point(386, 8);
            this.nutGui.Margin = new System.Windows.Forms.Padding(2);
            this.nutGui.Name = "nutGui";
            this.nutGui.Size = new System.Drawing.Size(45, 41);
            this.nutGui.TabIndex = 1;
            this.nutGui.Click += new System.EventHandler(this.nutGui_Click);
            // 
            // oNhapTin
            // 
            this.oNhapTin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.oNhapTin.BorderRadius = 15;
            this.oNhapTin.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.oNhapTin.DefaultText = "";
            this.oNhapTin.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.oNhapTin.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.oNhapTin.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.oNhapTin.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.oNhapTin.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.oNhapTin.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.oNhapTin.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.oNhapTin.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.oNhapTin.Location = new System.Drawing.Point(8, 8);
            this.oNhapTin.Margin = new System.Windows.Forms.Padding(2);
            this.oNhapTin.Multiline = true;
            this.oNhapTin.Name = "oNhapTin";
            this.oNhapTin.PlaceholderText = "Type a new message";
            this.oNhapTin.SelectedText = "";
            this.oNhapTin.Size = new System.Drawing.Size(371, 41);
            this.oNhapTin.TabIndex = 0;
            // 
            // khungTinNhan
            // 
            this.khungTinNhan.AutoScroll = true;
            this.khungTinNhan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.khungTinNhan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.khungTinNhan.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.khungTinNhan.Location = new System.Drawing.Point(0, 40);
            this.khungTinNhan.Margin = new System.Windows.Forms.Padding(2);
            this.khungTinNhan.Name = "khungTinNhan";
            this.khungTinNhan.Padding = new System.Windows.Forms.Padding(8);
            this.khungTinNhan.Size = new System.Drawing.Size(447, 529);
            this.khungTinNhan.TabIndex = 2;
            this.khungTinNhan.WrapContents = false;
            // 
            // duongKe1
            // 
            this.duongKe1.Dock = System.Windows.Forms.DockStyle.Top;
            this.duongKe1.Location = new System.Drawing.Point(0, 32);
            this.duongKe1.Margin = new System.Windows.Forms.Padding(2);
            this.duongKe1.Name = "duongKe1";
            this.duongKe1.Size = new System.Drawing.Size(447, 8);
            this.duongKe1.TabIndex = 1;
            // 
            // tieuDeChat
            // 
            this.tieuDeChat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tieuDeChat.Dock = System.Windows.Forms.DockStyle.Top;
            this.tieuDeChat.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tieuDeChat.Location = new System.Drawing.Point(0, 0);
            this.tieuDeChat.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.tieuDeChat.Name = "tieuDeChat";
            this.tieuDeChat.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.tieuDeChat.Size = new System.Drawing.Size(447, 32);
            this.tieuDeChat.TabIndex = 0;
            this.tieuDeChat.Text = "Weekend escape";
            this.tieuDeChat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.nutRoiChat);
            this.panel3.Controls.Add(this.nutQuanLyApp);
            this.panel3.Controls.Add(this.nutCopyLink);
            this.panel3.Controls.Add(this.nutTatTieng);
            this.panel3.Controls.Add(this.tieuDeTuyChon);
            this.panel3.Controls.Add(this.luongThanhVien);
            this.panel3.Controls.Add(this.tieuDeChiTiet);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(712, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(188, 569);
            this.panel3.TabIndex = 2;
            // 
            // nutRoiChat
            // 
            this.nutRoiChat.Dock = System.Windows.Forms.DockStyle.Top;
            this.nutRoiChat.FillColor = System.Drawing.Color.Transparent;
            this.nutRoiChat.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nutRoiChat.ForeColor = System.Drawing.Color.Black;
            this.nutRoiChat.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.nutRoiChat.ImageOffset = new System.Drawing.Point(10, 0);
            this.nutRoiChat.Location = new System.Drawing.Point(0, 270);
            this.nutRoiChat.Margin = new System.Windows.Forms.Padding(2);
            this.nutRoiChat.Name = "nutRoiChat";
            this.nutRoiChat.Size = new System.Drawing.Size(186, 28);
            this.nutRoiChat.TabIndex = 6;
            this.nutRoiChat.Text = "Leave";
            this.nutRoiChat.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.nutRoiChat.TextOffset = new System.Drawing.Point(15, 0);
            // 
            // nutQuanLyApp
            // 
            this.nutQuanLyApp.Dock = System.Windows.Forms.DockStyle.Top;
            this.nutQuanLyApp.FillColor = System.Drawing.Color.Transparent;
            this.nutQuanLyApp.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nutQuanLyApp.ForeColor = System.Drawing.Color.Black;
            this.nutQuanLyApp.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.nutQuanLyApp.ImageOffset = new System.Drawing.Point(10, 0);
            this.nutQuanLyApp.Location = new System.Drawing.Point(0, 242);
            this.nutQuanLyApp.Margin = new System.Windows.Forms.Padding(2);
            this.nutQuanLyApp.Name = "nutQuanLyApp";
            this.nutQuanLyApp.Size = new System.Drawing.Size(186, 28);
            this.nutQuanLyApp.TabIndex = 5;
            this.nutQuanLyApp.Text = "Manage apps";
            this.nutQuanLyApp.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.nutQuanLyApp.TextOffset = new System.Drawing.Point(15, 0);
            // 
            // nutCopyLink
            // 
            this.nutCopyLink.Dock = System.Windows.Forms.DockStyle.Top;
            this.nutCopyLink.FillColor = System.Drawing.Color.Transparent;
            this.nutCopyLink.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nutCopyLink.ForeColor = System.Drawing.Color.Black;
            this.nutCopyLink.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.nutCopyLink.ImageOffset = new System.Drawing.Point(10, 0);
            this.nutCopyLink.Location = new System.Drawing.Point(0, 214);
            this.nutCopyLink.Margin = new System.Windows.Forms.Padding(2);
            this.nutCopyLink.Name = "nutCopyLink";
            this.nutCopyLink.Size = new System.Drawing.Size(186, 28);
            this.nutCopyLink.TabIndex = 4;
            this.nutCopyLink.Text = "Copy link to chat";
            this.nutCopyLink.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.nutCopyLink.TextOffset = new System.Drawing.Point(15, 0);
            // 
            // nutTatTieng
            // 
            this.nutTatTieng.Dock = System.Windows.Forms.DockStyle.Top;
            this.nutTatTieng.FillColor = System.Drawing.Color.Transparent;
            this.nutTatTieng.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nutTatTieng.ForeColor = System.Drawing.Color.Black;
            this.nutTatTieng.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.nutTatTieng.ImageOffset = new System.Drawing.Point(10, 0);
            this.nutTatTieng.Location = new System.Drawing.Point(0, 186);
            this.nutTatTieng.Margin = new System.Windows.Forms.Padding(2);
            this.nutTatTieng.Name = "nutTatTieng";
            this.nutTatTieng.Size = new System.Drawing.Size(186, 28);
            this.nutTatTieng.TabIndex = 3;
            this.nutTatTieng.Text = "Mute";
            this.nutTatTieng.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.nutTatTieng.TextOffset = new System.Drawing.Point(15, 0);
            // 
            // tieuDeTuyChon
            // 
            this.tieuDeTuyChon.Dock = System.Windows.Forms.DockStyle.Top;
            this.tieuDeTuyChon.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tieuDeTuyChon.Location = new System.Drawing.Point(0, 154);
            this.tieuDeTuyChon.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.tieuDeTuyChon.Name = "tieuDeTuyChon";
            this.tieuDeTuyChon.Padding = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.tieuDeTuyChon.Size = new System.Drawing.Size(186, 32);
            this.tieuDeTuyChon.TabIndex = 2;
            this.tieuDeTuyChon.Text = "Options";
            // 
            // luongThanhVien
            // 
            this.luongThanhVien.AutoScroll = true;
            this.luongThanhVien.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.luongThanhVien.Dock = System.Windows.Forms.DockStyle.Top;
            this.luongThanhVien.Location = new System.Drawing.Point(0, 32);
            this.luongThanhVien.Margin = new System.Windows.Forms.Padding(2);
            this.luongThanhVien.Name = "luongThanhVien";
            this.luongThanhVien.Padding = new System.Windows.Forms.Padding(8);
            this.luongThanhVien.Size = new System.Drawing.Size(186, 122);
            this.luongThanhVien.TabIndex = 1;
            // 
            // tieuDeChiTiet
            // 
            this.tieuDeChiTiet.Dock = System.Windows.Forms.DockStyle.Top;
            this.tieuDeChiTiet.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tieuDeChiTiet.Location = new System.Drawing.Point(0, 0);
            this.tieuDeChiTiet.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.tieuDeChiTiet.Name = "tieuDeChiTiet";
            this.tieuDeChiTiet.Padding = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.tieuDeChiTiet.Size = new System.Drawing.Size(186, 32);
            this.tieuDeChiTiet.TabIndex = 0;
            this.tieuDeChiTiet.Text = "People (0)";
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 569);
            this.Controls.Add(this.bangChinh);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ChatForm";
            this.Text = "ChatForm";
            this.bangChinh.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.chiaChinh.Panel1.ResumeLayout(false);
            this.chiaChinh.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chiaChinh)).EndInit();
            this.chiaChinh.ResumeLayout(false);
            this.khungDanhSachChat.ResumeLayout(false);
            this.luongChatItems.ResumeLayout(false);
            this.luongChatItems.PerformLayout();
            this.khungChatHienTai.ResumeLayout(false);
            this.khungNhapTin.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel bangChinh;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private Guna.UI2.WinForms.Guna2ControlBox nutDong;
        // Cột giữa
        private System.Windows.Forms.SplitContainer chiaChinh;
        private System.Windows.Forms.Panel khungDanhSachChat;
        private Guna.UI2.WinForms.Guna2TextBox oTimKiem;
        private System.Windows.Forms.FlowLayoutPanel luongChatItems;
        private System.Windows.Forms.Panel khungChatHienTai;
        private System.Windows.Forms.Panel khungNhapTin;
        private Guna.UI2.WinForms.Guna2TextBox oNhapTin;
        private Guna.UI2.WinForms.Guna2Button nutGui;
        private Guna.UI2.WinForms.Guna2Button nutDinhKem;
        private Guna.UI2.WinForms.Guna2Button nutEmoji;
        private System.Windows.Forms.FlowLayoutPanel khungTinNhan;
        private System.Windows.Forms.Label tieuDeChat;
        private Guna.UI2.WinForms.Guna2Separator duongKe1;
        // Cột phải
        private System.Windows.Forms.Label tieuDeChiTiet;
        private System.Windows.Forms.FlowLayoutPanel luongThanhVien;
        private System.Windows.Forms.Label tieuDeTuyChon;
        private Guna.UI2.WinForms.Guna2Button nutTatTieng;
        private Guna.UI2.WinForms.Guna2Button nutCopyLink;
        private Guna.UI2.WinForms.Guna2Button nutQuanLyApp;
        private Guna.UI2.WinForms.Guna2Button nutRoiChat;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
    }
}