namespace NT106_BT2
{
    partial class FilePage
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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.btnFileTab = new Guna.UI2.WinForms.Guna2Button();
            this.btnAnh = new Guna.UI2.WinForms.Guna2Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.flpFiles = new System.Windows.Forms.FlowLayoutPanel();
            this.flpMedia = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelContent.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.panelHeader);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelContent);
            this.splitContainer1.Size = new System.Drawing.Size(948, 705);
            this.splitContainer1.SplitterDistance = 47;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // panelHeader
            // 
            this.panelHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelHeader.Controls.Add(this.btnFileTab);
            this.panelHeader.Controls.Add(this.btnAnh);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(948, 47);
            this.panelHeader.TabIndex = 0;
            // 
            // btnFileTab
            // 
            this.btnFileTab.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnFileTab.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnFileTab.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnFileTab.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnFileTab.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnFileTab.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnFileTab.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileTab.ForeColor = System.Drawing.Color.White;
            this.btnFileTab.Location = new System.Drawing.Point(474, 0);
            this.btnFileTab.Name = "btnFileTab";
            this.btnFileTab.Size = new System.Drawing.Size(474, 47);
            this.btnFileTab.TabIndex = 1;
            this.btnFileTab.Text = "File";
            // 
            // btnAnh
            // 
            this.btnAnh.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnAnh.Checked = true;
            this.btnAnh.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnAnh.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnAnh.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnAnh.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnAnh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnAnh.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnh.ForeColor = System.Drawing.Color.White;
            this.btnAnh.Location = new System.Drawing.Point(0, 0);
            this.btnAnh.Name = "btnAnh";
            this.btnAnh.Size = new System.Drawing.Size(474, 47);
            this.btnAnh.TabIndex = 0;
            this.btnAnh.Text = "Ảnh";
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.flpMedia);
            this.panelContent.Controls.Add(this.flpFiles);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(948, 657);
            this.panelContent.TabIndex = 0;
            // 
            // flpFiles
            // 
            this.flpFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpFiles.Location = new System.Drawing.Point(0, 0);
            this.flpFiles.Name = "flpFiles";
            this.flpFiles.Size = new System.Drawing.Size(948, 657);
            this.flpFiles.TabIndex = 0;
            this.flpFiles.Visible = false;
            // 
            // flpMedia
            // 
            this.flpMedia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpMedia.Location = new System.Drawing.Point(0, 0);
            this.flpMedia.Name = "flpMedia";
            this.flpMedia.Size = new System.Drawing.Size(948, 657);
            this.flpMedia.TabIndex = 1;
            // 
            // FilePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(948, 705);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FilePage";
            this.Text = "FilePage";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelHeader;
        private Guna.UI2.WinForms.Guna2Button btnFileTab;
        private Guna.UI2.WinForms.Guna2Button btnAnh;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.FlowLayoutPanel flpFiles;
        private System.Windows.Forms.FlowLayoutPanel flpMedia;
    }
}