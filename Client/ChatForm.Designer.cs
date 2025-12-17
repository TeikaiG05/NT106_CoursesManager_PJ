namespace NT106_BT2
{
    partial class ChatForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelCenter;
        private System.Windows.Forms.Panel panelRight;

        private System.Windows.Forms.FlowLayoutPanel luongChatItems;
        private System.Windows.Forms.Label labelChatTitle;
        private System.Windows.Forms.Panel panelSearchContainer;
        private System.Windows.Forms.TextBox oTimKiem;

        private System.Windows.Forms.Label tieuDeChat;
        private System.Windows.Forms.FlowLayoutPanel khungTinNhan;
        private System.Windows.Forms.Panel panelInputBar;
        private System.Windows.Forms.TextBox oNhapTin;
        private System.Windows.Forms.Button nutGui;

        private System.Windows.Forms.Label tieuDeChiTiet;
        private System.Windows.Forms.Label labelOptions;
        private System.Windows.Forms.Label labelMute;
        private System.Windows.Forms.Label labelCopyLink;
        private System.Windows.Forms.Label labelManageApps;
        private System.Windows.Forms.Label labelLeave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelLeft = new System.Windows.Forms.Panel();
            this.luongChatItems = new System.Windows.Forms.FlowLayoutPanel();
            this.panelSearchContainer = new System.Windows.Forms.Panel();
            this.oTimKiem = new System.Windows.Forms.TextBox();
            this.labelChatTitle = new System.Windows.Forms.Label();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.khungTinNhan = new System.Windows.Forms.FlowLayoutPanel();
            this.panelInputBar = new System.Windows.Forms.Panel();
            this.inputBackground = new System.Windows.Forms.Panel();
            this.oNhapTin = new System.Windows.Forms.TextBox();
            this.nutGui = new System.Windows.Forms.Button();
            this.tieuDeChat = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.labelLeave = new System.Windows.Forms.Label();
            this.labelManageApps = new System.Windows.Forms.Label();
            this.labelCopyLink = new System.Windows.Forms.Label();
            this.labelMute = new System.Windows.Forms.Label();
            this.labelOptions = new System.Windows.Forms.Label();
            this.tieuDeChiTiet = new System.Windows.Forms.Label();
            this.panelLeft.SuspendLayout();
            this.panelSearchContainer.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.panelInputBar.SuspendLayout();
            this.inputBackground.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.White;
            this.panelLeft.Controls.Add(this.luongChatItems);
            this.panelLeft.Controls.Add(this.panelSearchContainer);
            this.panelLeft.Controls.Add(this.labelChatTitle);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(260, 622);
            this.panelLeft.TabIndex = 2;
            // 
            // luongChatItems
            // 
            this.luongChatItems.AutoScroll = true;
            this.luongChatItems.BackColor = System.Drawing.Color.White;
            this.luongChatItems.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.luongChatItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.luongChatItems.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.luongChatItems.Location = new System.Drawing.Point(0, 84);
            this.luongChatItems.Name = "luongChatItems";
            this.luongChatItems.Padding = new System.Windows.Forms.Padding(12, 4, 12, 8);
            this.luongChatItems.Size = new System.Drawing.Size(260, 538);
            this.luongChatItems.TabIndex = 0;
            this.luongChatItems.WrapContents = false;
            // 
            // panelSearchContainer
            // 
            this.panelSearchContainer.Controls.Add(this.oTimKiem);
            this.panelSearchContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearchContainer.Location = new System.Drawing.Point(0, 40);
            this.panelSearchContainer.Name = "panelSearchContainer";
            this.panelSearchContainer.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.panelSearchContainer.Size = new System.Drawing.Size(260, 44);
            this.panelSearchContainer.TabIndex = 1;
            // 
            // oTimKiem
            // 
            this.oTimKiem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.oTimKiem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.oTimKiem.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.oTimKiem.ForeColor = System.Drawing.Color.Silver;
            this.oTimKiem.Location = new System.Drawing.Point(12, 8);
            this.oTimKiem.Name = "oTimKiem";
            this.oTimKiem.Size = new System.Drawing.Size(236, 24);
            this.oTimKiem.TabIndex = 0;
            this.oTimKiem.Text = "Search by email";
            // 
            // labelChatTitle
            // 
            this.labelChatTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelChatTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelChatTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelChatTitle.ForeColor = System.Drawing.Color.Black;
            this.labelChatTitle.Location = new System.Drawing.Point(0, 0);
            this.labelChatTitle.Name = "labelChatTitle";
            this.labelChatTitle.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.labelChatTitle.Size = new System.Drawing.Size(260, 40);
            this.labelChatTitle.TabIndex = 2;
            this.labelChatTitle.Text = "Chat";
            this.labelChatTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelCenter
            // 
            this.panelCenter.BackColor = System.Drawing.Color.White;
            this.panelCenter.Controls.Add(this.khungTinNhan);
            this.panelCenter.Controls.Add(this.panelInputBar);
            this.panelCenter.Controls.Add(this.tieuDeChat);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(260, 0);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(885, 622);
            this.panelCenter.TabIndex = 0;
            // 
            // khungTinNhan
            // 
            this.khungTinNhan.AutoScroll = true;
            this.khungTinNhan.BackColor = System.Drawing.Color.White;
            this.khungTinNhan.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.khungTinNhan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.khungTinNhan.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.khungTinNhan.Location = new System.Drawing.Point(0, 40);
            this.khungTinNhan.Name = "khungTinNhan";
            this.khungTinNhan.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.khungTinNhan.Size = new System.Drawing.Size(885, 512);
            this.khungTinNhan.TabIndex = 0;
            this.khungTinNhan.WrapContents = false;
            // 
            // panelInputBar
            // 
            this.panelInputBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.panelInputBar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelInputBar.Controls.Add(this.inputBackground);
            this.panelInputBar.Controls.Add(this.nutGui);
            this.panelInputBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInputBar.Location = new System.Drawing.Point(0, 552);
            this.panelInputBar.Name = "panelInputBar";
            this.panelInputBar.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.panelInputBar.Size = new System.Drawing.Size(885, 70);
            this.panelInputBar.TabIndex = 1;
            // 
            // inputBackground
            // 
            this.inputBackground.BackColor = System.Drawing.Color.White;
            this.inputBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputBackground.Controls.Add(this.oNhapTin);
            this.inputBackground.Location = new System.Drawing.Point(0, 10);
            this.inputBackground.Name = "inputBackground";
            this.inputBackground.Padding = new System.Windows.Forms.Padding(10);
            this.inputBackground.Size = new System.Drawing.Size(734, 40);
            this.inputBackground.TabIndex = 0;
            // 
            // oNhapTin
            // 
            this.oNhapTin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.oNhapTin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.oNhapTin.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.oNhapTin.ForeColor = System.Drawing.Color.Black;
            this.oNhapTin.Location = new System.Drawing.Point(10, 10);
            this.oNhapTin.Multiline = true;
            this.oNhapTin.Name = "oNhapTin";
            this.oNhapTin.Size = new System.Drawing.Size(712, 18);
            this.oNhapTin.TabIndex = 0;
            // 
            // nutGui
            // 
            this.nutGui.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(133)))), ((int)(((byte)(244)))));
            this.nutGui.Cursor = System.Windows.Forms.Cursors.Hand;
            this.nutGui.FlatAppearance.BorderSize = 0;
            this.nutGui.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nutGui.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.nutGui.ForeColor = System.Drawing.Color.White;
            this.nutGui.Location = new System.Drawing.Point(755, 13);
            this.nutGui.Name = "nutGui";
            this.nutGui.Size = new System.Drawing.Size(44, 40);
            this.nutGui.TabIndex = 1;
            this.nutGui.Text = "➤";
            this.nutGui.UseVisualStyleBackColor = false;
            // 
            // tieuDeChat
            // 
            this.tieuDeChat.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tieuDeChat.Dock = System.Windows.Forms.DockStyle.Top;
            this.tieuDeChat.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.tieuDeChat.ForeColor = System.Drawing.Color.Black;
            this.tieuDeChat.Location = new System.Drawing.Point(0, 0);
            this.tieuDeChat.Name = "tieuDeChat";
            this.tieuDeChat.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.tieuDeChat.Size = new System.Drawing.Size(885, 40);
            this.tieuDeChat.TabIndex = 2;
            this.tieuDeChat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.White;
            this.panelRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelRight.Controls.Add(this.labelLeave);
            this.panelRight.Controls.Add(this.labelManageApps);
            this.panelRight.Controls.Add(this.labelCopyLink);
            this.panelRight.Controls.Add(this.labelMute);
            this.panelRight.Controls.Add(this.labelOptions);
            this.panelRight.Controls.Add(this.tieuDeChiTiet);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(1145, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(256, 622);
            this.panelRight.TabIndex = 1;
            // 
            // labelLeave
            // 
            this.labelLeave.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelLeave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(68)))), ((int)(((byte)(55)))));
            this.labelLeave.Location = new System.Drawing.Point(24, 185);
            this.labelLeave.Name = "labelLeave";
            this.labelLeave.Size = new System.Drawing.Size(180, 20);
            this.labelLeave.TabIndex = 0;
            this.labelLeave.Text = "Leave";
            // 
            // labelManageApps
            // 
            this.labelManageApps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelManageApps.ForeColor = System.Drawing.Color.Black;
            this.labelManageApps.Location = new System.Drawing.Point(24, 160);
            this.labelManageApps.Name = "labelManageApps";
            this.labelManageApps.Size = new System.Drawing.Size(180, 20);
            this.labelManageApps.TabIndex = 1;
            this.labelManageApps.Text = "Manage apps";
            // 
            // labelCopyLink
            // 
            this.labelCopyLink.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelCopyLink.ForeColor = System.Drawing.Color.Black;
            this.labelCopyLink.Location = new System.Drawing.Point(24, 135);
            this.labelCopyLink.Name = "labelCopyLink";
            this.labelCopyLink.Size = new System.Drawing.Size(180, 20);
            this.labelCopyLink.TabIndex = 2;
            this.labelCopyLink.Text = "Copy link to chat";
            // 
            // labelMute
            // 
            this.labelMute.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelMute.ForeColor = System.Drawing.Color.Black;
            this.labelMute.Location = new System.Drawing.Point(24, 110);
            this.labelMute.Name = "labelMute";
            this.labelMute.Size = new System.Drawing.Size(180, 20);
            this.labelMute.TabIndex = 3;
            this.labelMute.Text = "Mute";
            // 
            // labelOptions
            // 
            this.labelOptions.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelOptions.ForeColor = System.Drawing.Color.Black;
            this.labelOptions.Location = new System.Drawing.Point(14, 80);
            this.labelOptions.Name = "labelOptions";
            this.labelOptions.Size = new System.Drawing.Size(200, 24);
            this.labelOptions.TabIndex = 4;
            this.labelOptions.Text = "Options";
            // 
            // tieuDeChiTiet
            // 
            this.tieuDeChiTiet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tieuDeChiTiet.Dock = System.Windows.Forms.DockStyle.Top;
            this.tieuDeChiTiet.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.tieuDeChiTiet.ForeColor = System.Drawing.Color.Black;
            this.tieuDeChiTiet.Location = new System.Drawing.Point(0, 0);
            this.tieuDeChiTiet.Name = "tieuDeChiTiet";
            this.tieuDeChiTiet.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.tieuDeChiTiet.Size = new System.Drawing.Size(254, 40);
            this.tieuDeChiTiet.TabIndex = 5;
            this.tieuDeChiTiet.Text = "People ";
            this.tieuDeChiTiet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1401, 622);
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "ChatForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Messenger";
            this.panelLeft.ResumeLayout(false);
            this.panelSearchContainer.ResumeLayout(false);
            this.panelSearchContainer.PerformLayout();
            this.panelCenter.ResumeLayout(false);
            this.panelInputBar.ResumeLayout(false);
            this.inputBackground.ResumeLayout(false);
            this.inputBackground.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel inputBackground;
    }
}
