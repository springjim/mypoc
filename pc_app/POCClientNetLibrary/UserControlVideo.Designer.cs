namespace POCClientNetLibrary
{
    partial class UserControlVideo
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
            this.username = new System.Windows.Forms.Label();
            this.time = new System.Windows.Forms.Label();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnShare = new System.Windows.Forms.Button();
            this.btnVideoScale = new System.Windows.Forms.Button();
            this.btnSwitch = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnFresh = new System.Windows.Forms.Button();
            this.btnSnap = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStripVideoScale = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemStretchImage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCenterImage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.panelVideo = new System.Windows.Forms.Panel();
            this.panelForDrag = new System.Windows.Forms.Panel();
            this.panelMenu.SuspendLayout();
            this.contextMenuStripVideoScale.SuspendLayout();
            this.panelVideo.SuspendLayout();
            this.SuspendLayout();
            // 
            // username
            // 
            this.username.AutoSize = true;
            this.username.Dock = System.Windows.Forms.DockStyle.Left;
            this.username.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.username.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.username.ForeColor = System.Drawing.Color.White;
            this.username.Location = new System.Drawing.Point(0, 0);
            this.username.Name = "username";
            this.username.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.username.Size = new System.Drawing.Size(87, 29);
            this.username.TabIndex = 0;
            this.username.Text = "ddddddd";
            this.username.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // time
            // 
            this.time.AutoSize = true;
            this.time.Dock = System.Windows.Forms.DockStyle.Left;
            this.time.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.time.ForeColor = System.Drawing.Color.White;
            this.time.Location = new System.Drawing.Point(87, 0);
            this.time.Name = "time";
            this.time.Padding = new System.Windows.Forms.Padding(25, 6, 0, 0);
            this.time.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.time.Size = new System.Drawing.Size(79, 29);
            this.time.TabIndex = 1;
            this.time.Text = "dddd";
            this.time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelMenu
            // 
            this.panelMenu.AllowDrop = true;
            this.panelMenu.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelMenu.Controls.Add(this.btnShare);
            this.panelMenu.Controls.Add(this.btnVideoScale);
            this.panelMenu.Controls.Add(this.btnSwitch);
            this.panelMenu.Controls.Add(this.btnStop);
            this.panelMenu.Controls.Add(this.time);
            this.panelMenu.Controls.Add(this.username);
            this.panelMenu.Controls.Add(this.btnFresh);
            this.panelMenu.Controls.Add(this.btnSnap);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMenu.Location = new System.Drawing.Point(1, 1);
            this.panelMenu.Margin = new System.Windows.Forms.Padding(0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(735, 34);
            this.panelMenu.TabIndex = 2;
            this.panelMenu.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelMenu_DragEnter);
            this.panelMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMenu_MouseDown);
            this.panelMenu.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelForDrag_MouseMove);
            this.panelMenu.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelForDrag_MouseUp);
            // 
            // btnShare
            // 
            this.btnShare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShare.FlatAppearance.BorderSize = 0;
            this.btnShare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShare.Image = global::POCClientNetLibrary.Properties.Resources.vc_switch;
            this.btnShare.Location = new System.Drawing.Point(594, 0);
            this.btnShare.Name = "btnShare";
            this.btnShare.Size = new System.Drawing.Size(32, 32);
            this.btnShare.TabIndex = 7;
            this.toolTip1.SetToolTip(this.btnShare, "前后摄像头切换");
            this.btnShare.UseVisualStyleBackColor = true;
            this.btnShare.Visible = false;
            // 
            // btnVideoScale
            // 
            this.btnVideoScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVideoScale.FlatAppearance.BorderSize = 0;
            this.btnVideoScale.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVideoScale.Image = global::POCClientNetLibrary.Properties.Resources.vcontrol_scale;
            this.btnVideoScale.Location = new System.Drawing.Point(594, 0);
            this.btnVideoScale.Name = "btnVideoScale";
            this.btnVideoScale.Size = new System.Drawing.Size(32, 27);
            this.btnVideoScale.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btnVideoScale, "视频比例调整");
            this.btnVideoScale.UseVisualStyleBackColor = true;
            this.btnVideoScale.Visible = false;
            this.btnVideoScale.Click += new System.EventHandler(this.btnVideoScale_Click);
            // 
            // btnSwitch
            // 
            this.btnSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSwitch.FlatAppearance.BorderSize = 0;
            this.btnSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSwitch.Image = global::POCClientNetLibrary.Properties.Resources.vcontrol_switch;
            this.btnSwitch.Location = new System.Drawing.Point(632, 0);
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Size = new System.Drawing.Size(32, 27);
            this.btnSwitch.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btnSwitch, "切换为当前通话");
            this.btnSwitch.UseVisualStyleBackColor = true;
            this.btnSwitch.Visible = false;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Image = global::POCClientNetLibrary.Properties.Resources.vc_stop;
            this.btnStop.Location = new System.Drawing.Point(670, 0);
            this.btnStop.Margin = new System.Windows.Forms.Padding(0);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(32, 32);
            this.btnStop.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnStop, "挂断");
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Visible = false;
            // 
            // btnFresh
            // 
            this.btnFresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFresh.FlatAppearance.BorderSize = 0;
            this.btnFresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFresh.Image = global::POCClientNetLibrary.Properties.Resources.vc_refresh;
            this.btnFresh.Location = new System.Drawing.Point(632, 1);
            this.btnFresh.Name = "btnFresh";
            this.btnFresh.Size = new System.Drawing.Size(32, 32);
            this.btnFresh.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnFresh, "刷新画面");
            this.btnFresh.UseVisualStyleBackColor = true;
            this.btnFresh.Visible = false;
            // 
            // btnSnap
            // 
            this.btnSnap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSnap.FlatAppearance.BorderSize = 0;
            this.btnSnap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSnap.Image = global::POCClientNetLibrary.Properties.Resources.vc_snap;
            this.btnSnap.Location = new System.Drawing.Point(703, 0);
            this.btnSnap.Name = "btnSnap";
            this.btnSnap.Size = new System.Drawing.Size(32, 32);
            this.btnSnap.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnSnap, "抓拍");
            this.btnSnap.UseVisualStyleBackColor = true;
            this.btnSnap.Visible = false;
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "温馨提示";
            // 
            // contextMenuStripVideoScale
            // 
            this.contextMenuStripVideoScale.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripVideoScale.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemStretchImage,
            this.toolStripMenuItemCenterImage,
            this.toolStripMenuItemZoom});
            this.contextMenuStripVideoScale.Name = "contextMenuStripVideoScale";
            this.contextMenuStripVideoScale.Size = new System.Drawing.Size(139, 76);
            // 
            // toolStripMenuItemStretchImage
            // 
            this.toolStripMenuItemStretchImage.Name = "toolStripMenuItemStretchImage";
            this.toolStripMenuItemStretchImage.Size = new System.Drawing.Size(138, 24);
            this.toolStripMenuItemStretchImage.Text = "横向拉伸";
            this.toolStripMenuItemStretchImage.Click += new System.EventHandler(this.toolStripMenuItemStretchImage_Click);
            // 
            // toolStripMenuItemCenterImage
            // 
            this.toolStripMenuItemCenterImage.Name = "toolStripMenuItemCenterImage";
            this.toolStripMenuItemCenterImage.Size = new System.Drawing.Size(138, 24);
            this.toolStripMenuItemCenterImage.Text = "填满";
            this.toolStripMenuItemCenterImage.Click += new System.EventHandler(this.toolStripMenuItemCenterImage_Click);
            // 
            // toolStripMenuItemZoom
            // 
            this.toolStripMenuItemZoom.Name = "toolStripMenuItemZoom";
            this.toolStripMenuItemZoom.Size = new System.Drawing.Size(138, 24);
            this.toolStripMenuItemZoom.Text = "原始大小";
            this.toolStripMenuItemZoom.Click += new System.EventHandler(this.toolStripMenuItemZoom_Click);
            // 
            // panelVideo
            // 
            this.panelVideo.AllowDrop = true;
            this.panelVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.panelVideo.BackgroundImage = global::POCClientNetLibrary.Properties.Resources.videocalling1;
            this.panelVideo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelVideo.Controls.Add(this.panelForDrag);
            this.panelVideo.ForeColor = System.Drawing.Color.Transparent;
            this.panelVideo.Location = new System.Drawing.Point(1, 35);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(736, 478);
            this.panelVideo.TabIndex = 3;
            this.panelVideo.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelMenu_DragEnter);
            this.panelVideo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMenu_MouseDown);
            this.panelVideo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelForDrag_MouseMove);
            this.panelVideo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelForDrag_MouseUp);
            // 
            // panelForDrag
            // 
            this.panelForDrag.AllowDrop = true;
            this.panelForDrag.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelForDrag.BackColor = System.Drawing.Color.Transparent;
            this.panelForDrag.ForeColor = System.Drawing.Color.Transparent;
            this.panelForDrag.Location = new System.Drawing.Point(0, 0);
            this.panelForDrag.Name = "panelForDrag";
            this.panelForDrag.Size = new System.Drawing.Size(736, 478);
            this.panelForDrag.TabIndex = 0;
            this.panelForDrag.Visible = false;
            this.panelForDrag.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelMenu_DragEnter);
            this.panelForDrag.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMenu_MouseDown);
            this.panelForDrag.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelForDrag_MouseMove);
            this.panelForDrag.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelForDrag_MouseUp);
            // 
            // UserControlVideo
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
            this.Controls.Add(this.panelVideo);
            this.Controls.Add(this.panelMenu);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "UserControlVideo";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(737, 508);
            this.Tag = "999";
            this.Load += new System.EventHandler(this.UserControlVideo_Load);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelMenu_DragEnter);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMenu_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelForDrag_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelForDrag_MouseUp);
            this.Resize += new System.EventHandler(this.UserControlVideo_Resize);
            this.panelMenu.ResumeLayout(false);
            this.panelMenu.PerformLayout();
            this.contextMenuStripVideoScale.ResumeLayout(false);
            this.panelVideo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Label time;
        public System.Windows.Forms.Label username;
        public System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.Button btnFresh;
        public System.Windows.Forms.Button btnSnap;
        public System.Windows.Forms.Button btnStop;
        public System.Windows.Forms.Button btnSwitch;
        public System.Windows.Forms.Button btnVideoScale;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripVideoScale;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStretchImage;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCenterImage;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemZoom;
        public System.Windows.Forms.Button btnShare;
        public System.Windows.Forms.Panel panelVideo;
        public System.Windows.Forms.Panel panelForDrag;
    }
}
