namespace NT106_BT2
{
    partial class ParticipantTile
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picAvatar = new System.Windows.Forms.PictureBox();
            this.pnlAvatar = new System.Windows.Forms.Panel();
            this.lblInitials = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.pnlAvatar.SuspendLayout();
            this.SuspendLayout();
            // 
            // picAvatar
            // 
            this.picAvatar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picAvatar.Location = new System.Drawing.Point(0, 0);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.Size = new System.Drawing.Size(50, 50);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 2;
            this.picAvatar.TabStop = false;
            this.picAvatar.Visible = false;
            // 
            // pnlAvatar
            // 
            this.pnlAvatar.Controls.Add(this.picAvatar);
            this.pnlAvatar.Controls.Add(this.lblInitials);
            this.pnlAvatar.Location = new System.Drawing.Point(5, 4);
            this.pnlAvatar.Margin = new System.Windows.Forms.Padding(0);
            this.pnlAvatar.Name = "pnlAvatar";
            this.pnlAvatar.Size = new System.Drawing.Size(50, 50);
            this.pnlAvatar.TabIndex = 0;
            // 
            // lblInitials
            // 
            this.lblInitials.BackColor = System.Drawing.Color.Transparent;
            this.lblInitials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInitials.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.lblInitials.Location = new System.Drawing.Point(0, 0);
            this.lblInitials.Name = "lblInitials";
            this.lblInitials.Size = new System.Drawing.Size(50, 50);
            this.lblInitials.TabIndex = 0;
            this.lblInitials.Text = "NA";
            this.lblInitials.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblName
            // 
            this.lblName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblName.Location = new System.Drawing.Point(0, 53);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(56, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ParticipantTile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.pnlAvatar);
            this.Name = "ParticipantTile";
            this.Size = new System.Drawing.Size(56, 66);
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.pnlAvatar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlAvatar;
        private System.Windows.Forms.Label lblInitials;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.PictureBox picAvatar;
    }
}
