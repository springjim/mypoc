namespace POCControlCenter
{
    partial class LocalVideoViewForm
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
            Hide();

           // if (disposing && (components != null))
           // {
            //    components.Dispose();
            //}
           // base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureLocalVideo = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureLocalVideo)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureLocalVideo
            // 
            this.pictureLocalVideo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureLocalVideo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureLocalVideo.Location = new System.Drawing.Point(14, 17);
            this.pictureLocalVideo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureLocalVideo.Name = "pictureLocalVideo";
            this.pictureLocalVideo.Size = new System.Drawing.Size(416, 273);
            this.pictureLocalVideo.TabIndex = 26;
            this.pictureLocalVideo.TabStop = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(141, 298);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 40);
            this.button1.TabIndex = 27;
            this.button1.Text = "关闭";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LocalVideoViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 353);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureLocalVideo);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LocalVideoViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "本地摄像头预览";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LocalVideoViewForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureLocalVideo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.PictureBox pictureLocalVideo;
    }
}