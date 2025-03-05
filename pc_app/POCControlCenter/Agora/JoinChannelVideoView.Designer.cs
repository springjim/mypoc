namespace POCControlCenter.Agora
{
    partial class JoinChannelVideoView
    {

        /// <summary> 
        /// 
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 
        /// </summary>
        /// <param name="disposing">If release delegate resource, true; or fals</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Componet Designer Generated Code

        /// <summary> 
        /// The method designer supported - don't modify
        /// Modify the code by designer
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelToUserName = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.localVideoView = new System.Windows.Forms.PictureBox();
            this.remoteVideoView = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAudio = new System.Windows.Forms.PictureBox();
            this.btnCamera = new System.Windows.Forms.PictureBox();
            this.btnHangup = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.localVideoView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.remoteVideoView)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnAudio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCamera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnHangup)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelToUserName);
            this.groupBox1.Controls.Add(this.labelStatus);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(932, 64);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            // 
            // labelToUserName
            // 
            this.labelToUserName.AutoSize = true;
            this.labelToUserName.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelToUserName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelToUserName.Location = new System.Drawing.Point(3, 21);
            this.labelToUserName.Name = "labelToUserName";
            this.labelToUserName.Size = new System.Drawing.Size(89, 20);
            this.labelToUserName.TabIndex = 15;
            this.labelToUserName.Text = "对方名称";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.labelStatus.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelStatus.Location = new System.Drawing.Point(126, 21);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(99, 20);
            this.labelStatus.TabIndex = 14;
            this.labelStatus.Text = "1v1 Video";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.localVideoView);
            this.groupBox2.Controls.Add(this.remoteVideoView);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(932, 569);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            // 
            // localVideoView
            // 
            this.localVideoView.Location = new System.Drawing.Point(0, 24);
            this.localVideoView.Name = "localVideoView";
            this.localVideoView.Size = new System.Drawing.Size(222, 176);
            this.localVideoView.TabIndex = 5;
            this.localVideoView.TabStop = false;
            // 
            // remoteVideoView
            // 
            this.remoteVideoView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.remoteVideoView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.remoteVideoView.Cursor = System.Windows.Forms.Cursors.Default;
            this.remoteVideoView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remoteVideoView.Location = new System.Drawing.Point(3, 21);
            this.remoteVideoView.Name = "remoteVideoView";
            this.remoteVideoView.Size = new System.Drawing.Size(926, 545);
            this.remoteVideoView.TabIndex = 6;
            this.remoteVideoView.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 567);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(932, 66);
            this.panel1.TabIndex = 16;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.btnAudio, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCamera, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnHangup, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(932, 66);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // btnAudio
            // 
            this.btnAudio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAudio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAudio.Image = global::POCControlCenter.Properties.Resources.btn_audio_open1;
            this.btnAudio.Location = new System.Drawing.Point(3, 3);
            this.btnAudio.Name = "btnAudio";
            this.btnAudio.Size = new System.Drawing.Size(304, 60);
            this.btnAudio.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnAudio.TabIndex = 14;
            this.btnAudio.TabStop = false;
            this.btnAudio.Click += new System.EventHandler(this.btnAudio_Click);
            // 
            // btnCamera
            // 
            this.btnCamera.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCamera.Image = global::POCControlCenter.Properties.Resources.btn_camera_open1;
            this.btnCamera.Location = new System.Drawing.Point(623, 3);
            this.btnCamera.Name = "btnCamera";
            this.btnCamera.Size = new System.Drawing.Size(306, 60);
            this.btnCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnCamera.TabIndex = 15;
            this.btnCamera.TabStop = false;
            this.btnCamera.Click += new System.EventHandler(this.btnCamera_Click);
            // 
            // btnHangup
            // 
            this.btnHangup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHangup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnHangup.Image = global::POCControlCenter.Properties.Resources.btn_endcall_normal;
            this.btnHangup.Location = new System.Drawing.Point(313, 3);
            this.btnHangup.Name = "btnHangup";
            this.btnHangup.Size = new System.Drawing.Size(304, 60);
            this.btnHangup.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnHangup.TabIndex = 16;
            this.btnHangup.TabStop = false;
            this.btnHangup.Click += new System.EventHandler(this.btnHangup_Click);
            // 
            // JoinChannelVideoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 633);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "JoinChannelVideoView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JoinChannelVideoView_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.JoinChannelVideoView_FormClosed);
            this.Load += new System.EventHandler(this.JoinChannelVideoView_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.localVideoView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.remoteVideoView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnAudio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCamera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnHangup)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelToUserName;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.PictureBox localVideoView;
        public System.Windows.Forms.PictureBox remoteVideoView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox btnHangup;
        private System.Windows.Forms.PictureBox btnCamera;
        private System.Windows.Forms.PictureBox btnAudio;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}