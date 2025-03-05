namespace POCControlCenter.Agora.Meeting
{
    partial class AgoraToastForm
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
            this.exitPicBox = new System.Windows.Forms.PictureBox();
            this.textLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.exitPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // exitPicBox
            // 
            this.exitPicBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.exitPicBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.exitPicBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.exitPicBox.Image = global::POCControlCenter.Properties.Resources.cancel;
            this.exitPicBox.Location = new System.Drawing.Point(459, 0);
            this.exitPicBox.Margin = new System.Windows.Forms.Padding(4, 4, 8, 4);
            this.exitPicBox.Name = "exitPicBox";
            this.exitPicBox.Size = new System.Drawing.Size(31, 34);
            this.exitPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.exitPicBox.TabIndex = 36;
            this.exitPicBox.TabStop = false;
            this.exitPicBox.Click += new System.EventHandler(this.exitPicBox_Click);
            // 
            // textLabel
            // 
            this.textLabel.AutoSize = true;
            this.textLabel.BackColor = System.Drawing.Color.Yellow;
            this.textLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.textLabel.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.textLabel.ForeColor = System.Drawing.Color.Red;
            this.textLabel.Location = new System.Drawing.Point(0, 0);
            this.textLabel.Name = "textLabel";
            this.textLabel.Padding = new System.Windows.Forms.Padding(20, 4, 20, 4);
            this.textLabel.Size = new System.Drawing.Size(166, 33);
            this.textLabel.TabIndex = 35;
            this.textLabel.Text = "正在屏幕共享";
            this.textLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AgoraToastForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.Yellow;
            this.ClientSize = new System.Drawing.Size(490, 34);
            this.ControlBox = false;
            this.Controls.Add(this.exitPicBox);
            this.Controls.Add(this.textLabel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AgoraToastForm";
            this.Opacity = 0.7D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AgoraToastForm";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.exitPicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox exitPicBox;
        private System.Windows.Forms.Label textLabel;
    }
}