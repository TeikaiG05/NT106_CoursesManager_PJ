namespace NT106_BT2
{
    partial class TeamsCard
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
            this.lbClassname = new System.Windows.Forms.Label();
            this.lbClasscode = new System.Windows.Forms.Label();
            this.picAvatar = new Guna.UI2.WinForms.Guna2PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // lbClassname
            // 
            this.lbClassname.AutoSize = true;
            this.lbClassname.Font = new System.Drawing.Font("Microsoft Tai Le", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbClassname.Location = new System.Drawing.Point(129, 37);
            this.lbClassname.Name = "lbClassname";
            this.lbClassname.Size = new System.Drawing.Size(113, 19);
            this.lbClassname.TabIndex = 0;
            this.lbClassname.Text = "Phung Gia Kiet";
            // 
            // lbClasscode
            // 
            this.lbClasscode.AutoSize = true;
            this.lbClasscode.Font = new System.Drawing.Font("Microsoft Tai Le", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbClasscode.Location = new System.Drawing.Point(129, 60);
            this.lbClasscode.Name = "lbClasscode";
            this.lbClasscode.Size = new System.Drawing.Size(81, 19);
            this.lbClasscode.TabIndex = 2;
            this.lbClasscode.Text = "23520818";
            // 
            // picAvatar
            // 
            this.picAvatar.Image = global::NT106_BT2.Properties.Resources.avatar_anh_meo_cute_3;
            this.picAvatar.ImageRotate = 0F;
            this.picAvatar.Location = new System.Drawing.Point(26, 37);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.Size = new System.Drawing.Size(90, 75);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 1;
            this.picAvatar.TabStop = false;
            // 
            // TeamsCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lbClasscode);
            this.Controls.Add(this.picAvatar);
            this.Controls.Add(this.lbClassname);
            this.Name = "TeamsCard";
            this.Size = new System.Drawing.Size(274, 148);
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbClassname;
        private Guna.UI2.WinForms.Guna2PictureBox picAvatar;
        private System.Windows.Forms.Label lbClasscode;
    }
}
