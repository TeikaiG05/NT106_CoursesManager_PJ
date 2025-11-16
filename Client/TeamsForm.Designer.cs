namespace NT106_BT2
{
    partial class TeamsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeamsForm));
            this.panelTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.flowTeams = new System.Windows.Forms.FlowLayoutPanel();
            this.ContextMenu = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
            this.btnJoinOrCreate = new Guna.UI2.WinForms.Guna2Button();
            this.tsmiCreateTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiJoinTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTop.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.ContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTop.Controls.Add(this.btnJoinOrCreate);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1148, 60);
            this.panelTop.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Tai Le", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "Teams";
            // 
            // panelContent
            // 
            this.panelContent.Controls.Add(this.flowTeams);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 60);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1148, 688);
            this.panelContent.TabIndex = 1;
            // 
            // flowTeams
            // 
            this.flowTeams.AutoScroll = true;
            this.flowTeams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowTeams.Location = new System.Drawing.Point(0, 0);
            this.flowTeams.Name = "flowTeams";
            this.flowTeams.Size = new System.Drawing.Size(1148, 688);
            this.flowTeams.TabIndex = 0;
            // 
            // ContextMenu
            // 
            this.ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCreateTeam,
            this.tsmiJoinTeam});
            this.ContextMenu.Name = "guna2ContextMenuStrip1";
            this.ContextMenu.RenderStyle.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(143)))), ((int)(((byte)(255)))));
            this.ContextMenu.RenderStyle.BorderColor = System.Drawing.Color.Gainsboro;
            this.ContextMenu.RenderStyle.ColorTable = null;
            this.ContextMenu.RenderStyle.RoundedEdges = true;
            this.ContextMenu.RenderStyle.SelectionArrowColor = System.Drawing.Color.White;
            this.ContextMenu.RenderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.ContextMenu.RenderStyle.SelectionForeColor = System.Drawing.Color.White;
            this.ContextMenu.RenderStyle.SeparatorColor = System.Drawing.Color.Gainsboro;
            this.ContextMenu.RenderStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.ContextMenu.Size = new System.Drawing.Size(181, 70);
            // 
            // btnJoinOrCreate
            // 
            this.btnJoinOrCreate.BackColor = System.Drawing.Color.Transparent;
            this.btnJoinOrCreate.ContextMenuStrip = this.ContextMenu;
            this.btnJoinOrCreate.DefaultAutoSize = true;
            this.btnJoinOrCreate.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnJoinOrCreate.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnJoinOrCreate.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnJoinOrCreate.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnJoinOrCreate.FillColor = System.Drawing.Color.WhiteSmoke;
            this.btnJoinOrCreate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnJoinOrCreate.ForeColor = System.Drawing.Color.Black;
            this.btnJoinOrCreate.Image = ((System.Drawing.Image)(resources.GetObject("btnJoinOrCreate.Image")));
            this.btnJoinOrCreate.Location = new System.Drawing.Point(964, 16);
            this.btnJoinOrCreate.Name = "btnJoinOrCreate";
            this.btnJoinOrCreate.Size = new System.Drawing.Size(161, 27);
            this.btnJoinOrCreate.TabIndex = 1;
            this.btnJoinOrCreate.Text = "Join or create team";
            this.btnJoinOrCreate.Click += new System.EventHandler(this.btnJoinOrCreate_Click);
            // 
            // tsmiCreateTeam
            // 
            this.tsmiCreateTeam.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tsmiCreateTeam.Image = ((System.Drawing.Image)(resources.GetObject("tsmiCreateTeam.Image")));
            this.tsmiCreateTeam.Name = "tsmiCreateTeam";
            this.tsmiCreateTeam.Size = new System.Drawing.Size(180, 22);
            this.tsmiCreateTeam.Text = "Create team";
            this.tsmiCreateTeam.Click += new System.EventHandler(this.tsmiCreateTeam_Click);
            // 
            // tsmiJoinTeam
            // 
            this.tsmiJoinTeam.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tsmiJoinTeam.Image = ((System.Drawing.Image)(resources.GetObject("tsmiJoinTeam.Image")));
            this.tsmiJoinTeam.Name = "tsmiJoinTeam";
            this.tsmiJoinTeam.Size = new System.Drawing.Size(180, 22);
            this.tsmiJoinTeam.Text = "Join team";
            this.tsmiJoinTeam.Click += new System.EventHandler(this.tsmiJoinTeam_Click);
            // 
            // TeamsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1148, 748);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TeamsForm";
            this.Text = "TeamsForm";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelContent.ResumeLayout(false);
            this.ContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private Guna.UI2.WinForms.Guna2Button btnJoinOrCreate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.FlowLayoutPanel flowTeams;
        private Guna.UI2.WinForms.Guna2ContextMenuStrip ContextMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateTeam;
        private System.Windows.Forms.ToolStripMenuItem tsmiJoinTeam;
    }
}