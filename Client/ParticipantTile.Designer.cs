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
            this.pnlAvatar = new System.Windows.Forms.Panel();
            this.lblInitials = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.pnlAvatar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlAvatar
            // 
            this.pnlAvatar.Controls.Add(this.lblInitials);
            this.pnlAvatar.Location = new System.Drawing.Point(0, 0);
            this.pnlAvatar.Margin = new System.Windows.Forms.Padding(0);
            this.pnlAvatar.Name = "pnlAvatar";
            this.pnlAvatar.Size = new System.Drawing.Size(140, 76);
            this.pnlAvatar.TabIndex = 0;
            // 
            // lblInitials
            // 
            this.lblInitials.AutoSize = true;
            this.lblInitials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInitials.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblInitials.Location = new System.Drawing.Point(0, 0);
            this.lblInitials.Name = "lblInitials";
            this.lblInitials.Size = new System.Drawing.Size(35, 13);
            this.lblInitials.TabIndex = 0;
            this.lblInitials.Text = "label1";
            // 
            // lblName
            // 
            this.lblName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblName.Location = new System.Drawing.Point(0, 86);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(140, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "label1";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ParticipantTile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.pnlAvatar);
            this.Name = "ParticipantTile";
            this.Size = new System.Drawing.Size(140, 99);
            this.pnlAvatar.ResumeLayout(false);
            this.pnlAvatar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlAvatar;
        private System.Windows.Forms.Label lblInitials;
        private System.Windows.Forms.Label lblName;
    }
}
