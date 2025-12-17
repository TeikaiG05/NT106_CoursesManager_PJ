namespace NT106_BT2
{
    partial class ChatForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelCenter;

        private System.Windows.Forms.FlowLayoutPanel luongChatItems;
        private System.Windows.Forms.FlowLayoutPanel khungTinNhan;

        private System.Windows.Forms.TextBox oTimKiem;
        private System.Windows.Forms.TextBox oNhapTin;
        private System.Windows.Forms.Button nutGui;
        private System.Windows.Forms.Button nutFile;
        private System.Windows.Forms.Button nutEmoji;

        private System.Windows.Forms.Label labelChatTitle;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelLeft = new System.Windows.Forms.Panel();
            this.luongChatItems = new System.Windows.Forms.FlowLayoutPanel();
            this.oTimKiem = new System.Windows.Forms.TextBox();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.khungTinNhan = new System.Windows.Forms.FlowLayoutPanel();
            this.panelInput = new System.Windows.Forms.Panel();
            this.oNhapTin = new System.Windows.Forms.TextBox();
            this.nutGui = new System.Windows.Forms.Button();
            this.nutFile = new System.Windows.Forms.Button();
            this.nutEmoji = new System.Windows.Forms.Button();
            this.labelChatTitle = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();

            this.panelLeft.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.panelInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.White;
            this.panelLeft.Controls.Add(this.luongChatItems);
            this.panelLeft.Controls.Add(this.oTimKiem);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(257, 607);
            this.panelLeft.TabIndex = 2;
            // 
            // luongChatItems
            // 
            this.luongChatItems.AutoScroll = true;
            this.luongChatItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.luongChatItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.luongChatItems.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.luongChatItems.Location = new System.Drawing.Point(0, 20);
            this.luongChatItems.Name = "luongChatItems";
            this.luongChatItems.Size = new System.Drawing.Size(257, 587);
            this.luongChatItems.TabIndex = 0;
            this.luongChatItems.WrapContents = false;
            // 
            // oTimKiem
            // 
            this.oTimKiem.Dock = System.Windows.Forms.DockStyle.Top;
            this.oTimKiem.Location = new System.Drawing.Point(0, 0);
            this.oTimKiem.Margin = new System.Windows.Forms.Padding(9);
            this.oTimKiem.Name = "oTimKiem";
            this.oTimKiem.Size = new System.Drawing.Size(257, 20);
            this.oTimKiem.TabIndex = 1;
            this.oTimKiem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.oTimKiem_KeyDown);
            // 
            // panelCenter
            // 
            this.panelCenter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelCenter.Controls.Add(this.khungTinNhan);
            this.panelCenter.Controls.Add(this.panelInput);
            this.panelCenter.Controls.Add(this.labelChatTitle);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(257, 0);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(549, 607);
            this.panelCenter.TabIndex = 0;
            // 
            // khungTinNhan
            // 
            this.khungTinNhan.AutoScroll = true;
            this.khungTinNhan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.khungTinNhan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.khungTinNhan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.khungTinNhan.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.khungTinNhan.Location = new System.Drawing.Point(0, 43);
            this.khungTinNhan.Name = "khungTinNhan";
            this.khungTinNhan.Size = new System.Drawing.Size(549, 503);
            this.khungTinNhan.TabIndex = 0;
            this.khungTinNhan.WrapContents = false;
            // 
            // panelInput
            // 
            this.panelInput.BackColor = System.Drawing.Color.White;
            this.panelInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelInput.Controls.Add(this.oNhapTin);
            this.panelInput.Controls.Add(this.nutGui);
            this.panelInput.Controls.Add(this.nutFile);
            this.panelInput.Controls.Add(this.nutEmoji);
            this.panelInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInput.Location = new System.Drawing.Point(0, 546);
            this.panelInput.Name = "panelInput";
            this.panelInput.Size = new System.Drawing.Size(549, 61);
            this.panelInput.TabIndex = 1;
            // 
            // oNhapTin
            // 
            this.oNhapTin.Location = new System.Drawing.Point(13, 13);
            this.oNhapTin.Multiline = true;
            this.oNhapTin.Name = "oNhapTin";
            this.oNhapTin.Size = new System.Drawing.Size(343, 35);
            this.oNhapTin.TabIndex = 0;
            // 
            // nutGui
            // 
            this.nutGui.Location = new System.Drawing.Point(364, 13);
            this.nutGui.Name = "nutGui";
            this.nutGui.Size = new System.Drawing.Size(51, 35);
            this.nutGui.TabIndex = 1;
            this.nutGui.Text = "Gửi";
            this.nutGui.Click += new System.EventHandler(this.nutGui_Click);
            // 
            // nutFile
            // 
            this.nutFile.Location = new System.Drawing.Point(424, 13);
            this.nutFile.Name = "nutFile";
            this.nutFile.Size = new System.Drawing.Size(34, 35);
            this.nutFile.TabIndex = 2;
            this.nutFile.Text = "📎";
            this.nutFile.Click += new System.EventHandler(this.nutFile_Click);
            // 
            // nutEmoji
            // 
            this.nutEmoji.Location = new System.Drawing.Point(463, 13);
            this.nutEmoji.Name = "nutEmoji";
            this.nutEmoji.Size = new System.Drawing.Size(34, 35);
            this.nutEmoji.TabIndex = 3;
            this.nutEmoji.Text = "😀";
            this.nutEmoji.Click += new System.EventHandler(this.nutEmoji_Click);
            // 
            // labelChatTitle
            // 
            this.labelChatTitle.BackColor = System.Drawing.Color.White;
            this.labelChatTitle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelChatTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelChatTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelChatTitle.Location = new System.Drawing.Point(0, 0);
            this.labelChatTitle.Name = "labelChatTitle";
            this.labelChatTitle.Padding = new System.Windows.Forms.Padding(13, 0, 0, 0);
            this.labelChatTitle.Size = new System.Drawing.Size(549, 43);
            this.labelChatTitle.TabIndex = 2;
            this.labelChatTitle.Text = "Chat";
            this.labelChatTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(806, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(223, 607);
            this.panelRight.TabIndex = 1;
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 607);
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.panelLeft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ChatForm";
            this.Text = "ChatForm";
            this.Load += new System.EventHandler(this.ChatForm_Load);
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelCenter.ResumeLayout(false);
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.Panel panelRight;
    }
}
