namespace NT106_BT2.Schedule
{
    partial class UCDays
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
            this.IBdays = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // IBdays
            // 
            this.IBdays.AutoSize = true;
            this.IBdays.BackColor = System.Drawing.SystemColors.Control;
            this.IBdays.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.IBdays.Location = new System.Drawing.Point(125, 0);
            this.IBdays.Name = "IBdays";
            this.IBdays.Size = new System.Drawing.Size(34, 25);
            this.IBdays.TabIndex = 1;
            this.IBdays.Text = "00";
            this.IBdays.Click += new System.EventHandler(this.IBdays_Click);
            // 
            // UCDays
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.IBdays);
            this.Name = "UCDays";
            this.Size = new System.Drawing.Size(162, 100);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label IBdays;
    }
}
