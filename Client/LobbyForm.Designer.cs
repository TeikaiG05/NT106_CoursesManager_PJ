namespace NT106_BT2
{
    partial class LobbyForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLeave = new Guna.UI2.WinForms.Guna2Button();
            this.btnShare = new Guna.UI2.WinForms.Guna2Button();
            this.btnMic = new Guna.UI2.WinForms.Guna2Button();
            this.btnVideo = new Guna.UI2.WinForms.Guna2Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pnlShare = new System.Windows.Forms.Panel();
            this.flpParticipants = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1245, 788);
            this.splitContainer1.SplitterDistance = 51;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnLeave);
            this.panel1.Controls.Add(this.btnShare);
            this.panel1.Controls.Add(this.btnMic);
            this.panel1.Controls.Add(this.btnVideo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1245, 51);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(1104, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "|";
            // 
            // btnLeave
            // 
            this.btnLeave.BorderRadius = 10;
            this.btnLeave.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLeave.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnLeave.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnLeave.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnLeave.FillColor = System.Drawing.Color.Red;
            this.btnLeave.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnLeave.ForeColor = System.Drawing.Color.Black;
            this.btnLeave.Image = global::NT106_BT2.Properties.Resources.icons8_end_call_30;
            this.btnLeave.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnLeave.Location = new System.Drawing.Point(1135, 10);
            this.btnLeave.Margin = new System.Windows.Forms.Padding(0);
            this.btnLeave.Name = "btnLeave";
            this.btnLeave.Size = new System.Drawing.Size(98, 33);
            this.btnLeave.TabIndex = 3;
            this.btnLeave.Text = "Leave";
            this.btnLeave.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnShare
            // 
            this.btnShare.BorderRadius = 10;
            this.btnShare.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnShare.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnShare.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnShare.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnShare.FillColor = System.Drawing.Color.Violet;
            this.btnShare.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnShare.ForeColor = System.Drawing.Color.Black;
            this.btnShare.Image = global::NT106_BT2.Properties.Resources.icons8_present_to_all_48;
            this.btnShare.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnShare.Location = new System.Drawing.Point(989, 10);
            this.btnShare.Margin = new System.Windows.Forms.Padding(0);
            this.btnShare.Name = "btnShare";
            this.btnShare.Size = new System.Drawing.Size(98, 33);
            this.btnShare.TabIndex = 2;
            this.btnShare.Text = "Share";
            this.btnShare.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnMic
            // 
            this.btnMic.BorderRadius = 10;
            this.btnMic.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnMic.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnMic.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnMic.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnMic.FillColor = System.Drawing.Color.Violet;
            this.btnMic.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnMic.ForeColor = System.Drawing.Color.Black;
            this.btnMic.Image = global::NT106_BT2.Properties.Resources.icons8_mic_48;
            this.btnMic.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnMic.Location = new System.Drawing.Point(874, 10);
            this.btnMic.Name = "btnMic";
            this.btnMic.Size = new System.Drawing.Size(98, 33);
            this.btnMic.TabIndex = 1;
            this.btnMic.Text = "Mic";
            this.btnMic.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnVideo
            // 
            this.btnVideo.BorderRadius = 10;
            this.btnVideo.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnVideo.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnVideo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnVideo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnVideo.FillColor = System.Drawing.Color.Violet;
            this.btnVideo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnVideo.ForeColor = System.Drawing.Color.Black;
            this.btnVideo.Image = global::NT106_BT2.Properties.Resources.icons8_video_call_30;
            this.btnVideo.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnVideo.Location = new System.Drawing.Point(759, 10);
            this.btnVideo.Name = "btnVideo";
            this.btnVideo.Size = new System.Drawing.Size(98, 33);
            this.btnVideo.TabIndex = 0;
            this.btnVideo.Text = "Video";
            this.btnVideo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.pnlShare);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.flpParticipants);
            this.splitContainer2.Size = new System.Drawing.Size(1245, 736);
            this.splitContainer2.SplitterDistance = 636;
            this.splitContainer2.SplitterWidth = 1;
            this.splitContainer2.TabIndex = 0;
            // 
            // pnlShare
            // 
            this.pnlShare.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShare.Location = new System.Drawing.Point(0, 0);
            this.pnlShare.Name = "pnlShare";
            this.pnlShare.Size = new System.Drawing.Size(1245, 636);
            this.pnlShare.TabIndex = 0;
            // 
            // flpParticipants
            // 
            this.flpParticipants.AutoScroll = true;
            this.flpParticipants.AutoSize = true;
            this.flpParticipants.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.flpParticipants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpParticipants.Location = new System.Drawing.Point(0, 0);
            this.flpParticipants.Margin = new System.Windows.Forms.Padding(0);
            this.flpParticipants.Name = "flpParticipants";
            this.flpParticipants.Padding = new System.Windows.Forms.Padding(10);
            this.flpParticipants.Size = new System.Drawing.Size(1245, 99);
            this.flpParticipants.TabIndex = 1;
            // 
            // LobbyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1245, 788);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LobbyForm";
            this.Text = "LobbyForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private Guna.UI2.WinForms.Guna2Button btnLeave;
        private Guna.UI2.WinForms.Guna2Button btnShare;
        private Guna.UI2.WinForms.Guna2Button btnMic;
        private Guna.UI2.WinForms.Guna2Button btnVideo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.FlowLayoutPanel flpParticipants;
        private System.Windows.Forms.Panel pnlShare;
    }
}