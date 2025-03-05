namespace POCControlCenter.Agora.Controls
{
    partial class LiveUserControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnCameraSwitch = new System.Windows.Forms.Button();
            this.time = new System.Windows.Forms.Label();
            this.username = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnVideo = new System.Windows.Forms.Button();
            this.btnAudio = new System.Windows.Forms.Button();
            this.remoteVideoView = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnSnap = new System.Windows.Forms.Button();
            this.panelMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.remoteVideoView)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.AllowDrop = true;
            this.panelMenu.BackColor = System.Drawing.Color.Silver;
            this.panelMenu.Controls.Add(this.btnSnap);
            this.panelMenu.Controls.Add(this.btnCameraSwitch);
            this.panelMenu.Controls.Add(this.time);
            this.panelMenu.Controls.Add(this.username);
            this.panelMenu.Controls.Add(this.btnStop);
            this.panelMenu.Controls.Add(this.btnVideo);
            this.panelMenu.Controls.Add(this.btnAudio);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(593, 48);
            this.panelMenu.TabIndex = 0;
            // 
            // btnCameraSwitch
            // 
            this.btnCameraSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCameraSwitch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCameraSwitch.FlatAppearance.BorderSize = 0;
            this.btnCameraSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCameraSwitch.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCameraSwitch.Image = global::POCControlCenter.Properties.Resources.videocall_over;
            this.btnCameraSwitch.Location = new System.Drawing.Point(405, 0);
            this.btnCameraSwitch.Margin = new System.Windows.Forms.Padding(0);
            this.btnCameraSwitch.Name = "btnCameraSwitch";
            this.btnCameraSwitch.Size = new System.Drawing.Size(40, 48);
            this.btnCameraSwitch.TabIndex = 14;
            this.toolTip1.SetToolTip(this.btnCameraSwitch, "切换摄像头");
            this.btnCameraSwitch.UseVisualStyleBackColor = true;
            this.btnCameraSwitch.Visible = false;
            // 
            // time
            // 
            this.time.Dock = System.Windows.Forms.DockStyle.Left;
            this.time.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.time.ForeColor = System.Drawing.Color.White;
            this.time.Location = new System.Drawing.Point(171, 0);
            this.time.Name = "time";
            this.time.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.time.Size = new System.Drawing.Size(161, 48);
            this.time.TabIndex = 13;
            this.time.Text = "dddd";
            this.time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.time.Visible = false;
            // 
            // username
            // 
            this.username.Dock = System.Windows.Forms.DockStyle.Left;
            this.username.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.username.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.username.ForeColor = System.Drawing.Color.White;
            this.username.Location = new System.Drawing.Point(0, 0);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(171, 48);
            this.username.TabIndex = 12;
            this.username.Text = "ddddddd";
            this.username.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.username.Visible = false;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Image = global::POCControlCenter.Properties.Resources.cancel;
            this.btnStop.Location = new System.Drawing.Point(547, 0);
            this.btnStop.Margin = new System.Windows.Forms.Padding(0);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(40, 48);
            this.btnStop.TabIndex = 10;
            this.toolTip1.SetToolTip(this.btnStop, "停止直播/监控");
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Visible = false;
            // 
            // btnVideo
            // 
            this.btnVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVideo.FlatAppearance.BorderSize = 0;
            this.btnVideo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVideo.Image = global::POCControlCenter.Properties.Resources.btn_camera_open1;
            this.btnVideo.Location = new System.Drawing.Point(497, 0);
            this.btnVideo.Margin = new System.Windows.Forms.Padding(0);
            this.btnVideo.Name = "btnVideo";
            this.btnVideo.Size = new System.Drawing.Size(40, 48);
            this.btnVideo.TabIndex = 9;
            this.toolTip1.SetToolTip(this.btnVideo, "显示/关闭对方视频");
            this.btnVideo.UseVisualStyleBackColor = true;
            this.btnVideo.Visible = false;
            // 
            // btnAudio
            // 
            this.btnAudio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAudio.FlatAppearance.BorderSize = 0;
            this.btnAudio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAudio.Image = global::POCControlCenter.Properties.Resources.btn_audio_open1;
            this.btnAudio.Location = new System.Drawing.Point(453, 0);
            this.btnAudio.Margin = new System.Windows.Forms.Padding(0);
            this.btnAudio.Name = "btnAudio";
            this.btnAudio.Size = new System.Drawing.Size(40, 48);
            this.btnAudio.TabIndex = 8;
            this.toolTip1.SetToolTip(this.btnAudio, "显示/关闭对方音频");
            this.btnAudio.UseVisualStyleBackColor = true;
            this.btnAudio.Visible = false;
            // 
            // remoteVideoView
            // 
            this.remoteVideoView.BackgroundImage = global::POCControlCenter.Properties.Resources.videocalling1;
            this.remoteVideoView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.remoteVideoView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remoteVideoView.Location = new System.Drawing.Point(0, 48);
            this.remoteVideoView.Name = "remoteVideoView";
            this.remoteVideoView.Size = new System.Drawing.Size(593, 440);
            this.remoteVideoView.TabIndex = 1;
            this.remoteVideoView.TabStop = false;
            // 
            // btnSnap
            // 
            this.btnSnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSnap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSnap.FlatAppearance.BorderSize = 0;
            this.btnSnap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSnap.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSnap.Image = global::POCControlCenter.Properties.Resources.POC图标设计_52;
            this.btnSnap.Location = new System.Drawing.Point(359, 0);
            this.btnSnap.Margin = new System.Windows.Forms.Padding(0);
            this.btnSnap.Name = "btnSnap";
            this.btnSnap.Size = new System.Drawing.Size(40, 48);
            this.btnSnap.TabIndex = 15;
            this.toolTip1.SetToolTip(this.btnSnap, "抓拍");
            this.btnSnap.UseVisualStyleBackColor = true;
            this.btnSnap.Visible = false;
            // 
            // LiveUserControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Controls.Add(this.remoteVideoView);
            this.Controls.Add(this.panelMenu);
            this.DoubleBuffered = true;
            this.Name = "LiveUserControl";
            this.Size = new System.Drawing.Size(593, 488);
            this.panelMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.remoteVideoView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Button btnStop;
        public System.Windows.Forms.Button btnVideo;
        public System.Windows.Forms.Panel panelMenu;
        public System.Windows.Forms.Label time;
        public System.Windows.Forms.Label username;
        public System.Windows.Forms.PictureBox remoteVideoView;
        public System.Windows.Forms.Button btnAudio;
        public System.Windows.Forms.Button btnCameraSwitch;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.Button btnSnap;
    }
}
