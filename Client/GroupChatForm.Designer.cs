namespace NT106_BT2
{
    partial class GroupChatForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupChatForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lbClasscode = new System.Windows.Forms.Label();
            this.lbClassname = new System.Windows.Forms.Label();
            this.picAvatar = new Guna.UI2.WinForms.Guna2PictureBox();
            this.btnFile = new Guna.UI2.WinForms.Guna2Button();
            this.btnChat = new Guna.UI2.WinForms.Guna2Button();
            this.btnBack = new Guna.UI2.WinForms.Guna2Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlGroupContainer = new Guna.UI2.WinForms.Guna2Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lbClasscode);
            this.panel1.Controls.Add(this.lbClassname);
            this.panel1.Controls.Add(this.picAvatar);
            this.panel1.Controls.Add(this.btnFile);
            this.panel1.Controls.Add(this.btnChat);
            this.panel1.Controls.Add(this.btnBack);
            this.panel1.Cursor = System.Windows.Forms.Cursors.No;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(221, 755);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 233);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "________________________";
            // 
            // lbClasscode
            // 
            this.lbClasscode.AutoSize = true;
            this.lbClasscode.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbClasscode.Location = new System.Drawing.Point(12, 201);
            this.lbClasscode.Name = "lbClasscode";
            this.lbClasscode.Size = new System.Drawing.Size(67, 16);
            this.lbClasscode.TabIndex = 5;
            this.lbClasscode.Text = "Classcode";
            // 
            // lbClassname
            // 
            this.lbClassname.AutoSize = true;
            this.lbClassname.Font = new System.Drawing.Font("Microsoft Tai Le", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbClassname.Location = new System.Drawing.Point(12, 175);
            this.lbClassname.Name = "lbClassname";
            this.lbClassname.Size = new System.Drawing.Size(72, 16);
            this.lbClassname.TabIndex = 4;
            this.lbClassname.Text = "Classname";
            // 
            // picAvatar
            // 
            this.picAvatar.ImageRotate = 0F;
            this.picAvatar.Location = new System.Drawing.Point(12, 89);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.Size = new System.Drawing.Size(85, 72);
            this.picAvatar.TabIndex = 3;
            this.picAvatar.TabStop = false;
            // 
            // btnFile
            // 
            this.btnFile.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnFile.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnFile.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnFile.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnFile.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnFile.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnFile.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnFile.ForeColor = System.Drawing.Color.Black;
            this.btnFile.Image = ((System.Drawing.Image)(resources.GetObject("btnFile.Image")));
            this.btnFile.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnFile.Location = new System.Drawing.Point(3, 345);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(221, 45);
            this.btnFile.TabIndex = 2;
            this.btnFile.Text = "File";
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // btnChat
            // 
            this.btnChat.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnChat.Checked = true;
            this.btnChat.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnChat.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnChat.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnChat.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnChat.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnChat.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnChat.ForeColor = System.Drawing.Color.Black;
            this.btnChat.Image = ((System.Drawing.Image)(resources.GetObject("btnChat.Image")));
            this.btnChat.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnChat.Location = new System.Drawing.Point(3, 298);
            this.btnChat.Name = "btnChat";
            this.btnChat.Size = new System.Drawing.Size(221, 45);
            this.btnChat.TabIndex = 1;
            this.btnChat.Text = "Chat";
            this.btnChat.Click += new System.EventHandler(this.btnChat_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.Transparent;
            this.btnBack.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnBack.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnBack.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnBack.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnBack.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.Gray;
            this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
            this.btnBack.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnBack.Location = new System.Drawing.Point(7, 12);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(212, 33);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "All Teams";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlGroupContainer);
            this.splitContainer1.Size = new System.Drawing.Size(1148, 755);
            this.splitContainer1.SplitterDistance = 221;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 1;
            // 
            // pnlGroupContainer
            // 
            this.pnlGroupContainer.BackColor = System.Drawing.SystemColors.Control;
            this.pnlGroupContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGroupContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlGroupContainer.Name = "pnlGroupContainer";
            this.pnlGroupContainer.Size = new System.Drawing.Size(926, 755);
            this.pnlGroupContainer.TabIndex = 0;
            // 
            // GroupChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 755);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "GroupChatForm";
            this.Text = "GroupChatForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private Guna.UI2.WinForms.Guna2Button btnBack;
        private System.Windows.Forms.Label lbClasscode;
        private System.Windows.Forms.Label lbClassname;
        private Guna.UI2.WinForms.Guna2PictureBox picAvatar;
        private Guna.UI2.WinForms.Guna2Button btnFile;
        private Guna.UI2.WinForms.Guna2Button btnChat;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2Panel pnlGroupContainer;
    }
}