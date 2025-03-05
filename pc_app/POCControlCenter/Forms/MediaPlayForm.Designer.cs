using System.IO;

namespace POCControlCenter
{
    partial class MediaPlayForm
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
            this.components = new System.ComponentModel.Container();
            this.media_state = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.username = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.createdate_video = new System.Windows.Forms.Label();
            this.duration_video = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.tableLayoutPanelMap = new System.Windows.Forms.TableLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.mapaddr = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pictureBoxGpsFail = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.picVideo = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.timerVlc = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelContainer.SuspendLayout();
            this.tableLayoutPanelMap.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGpsFail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVideo)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // media_state
            // 
            this.media_state.AutoSize = true;
            this.media_state.ForeColor = System.Drawing.Color.Teal;
            this.media_state.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.media_state.Location = new System.Drawing.Point(98, 16);
            this.media_state.Name = "media_state";
            this.media_state.Size = new System.Drawing.Size(50, 20);
            this.media_state.TabIndex = 25;
            this.media_state.Text = "label2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(15, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 20);
            this.label7.TabIndex = 24;
            this.label7.Text = "播放状态";
            // 
            // trackBar1
            // 
            this.trackBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(198)))), ((int)(((byte)(242)))));
            this.trackBar1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.trackBar1.LargeChange = 3;
            this.trackBar1.Location = new System.Drawing.Point(0, 0);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(925, 33);
            this.trackBar1.TabIndex = 23;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.trackBar1.Enter += new System.EventHandler(this.trackBar1_Enter);
            this.trackBar1.Leave += new System.EventHandler(this.trackBar1_Leave);
            this.trackBar1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(225, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 20);
            this.label4.TabIndex = 22;
            this.label4.Text = "创建时间";
            // 
            // username
            // 
            this.username.AutoSize = true;
            this.username.ForeColor = System.Drawing.Color.Teal;
            this.username.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.username.Location = new System.Drawing.Point(79, 14);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(50, 20);
            this.username.TabIndex = 20;
            this.username.Text = "label2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(9, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 19;
            this.label2.Text = "用户名称";
            // 
            // createdate_video
            // 
            this.createdate_video.AutoSize = true;
            this.createdate_video.ForeColor = System.Drawing.Color.Teal;
            this.createdate_video.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.createdate_video.Location = new System.Drawing.Point(294, 14);
            this.createdate_video.Name = "createdate_video";
            this.createdate_video.Size = new System.Drawing.Size(50, 20);
            this.createdate_video.TabIndex = 18;
            this.createdate_video.Text = "label6";
            // 
            // duration_video
            // 
            this.duration_video.AutoSize = true;
            this.duration_video.ForeColor = System.Drawing.Color.Teal;
            this.duration_video.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.duration_video.Location = new System.Drawing.Point(594, 14);
            this.duration_video.Name = "duration_video";
            this.duration_video.Size = new System.Drawing.Size(50, 20);
            this.duration_video.TabIndex = 15;
            this.duration_video.Text = "label4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(503, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "媒体时长";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelContainer, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(931, 534);
            this.tableLayoutPanel1.TabIndex = 26;
            // 
            // panelContainer
            // 
            this.panelContainer.Controls.Add(this.tableLayoutPanelMap);
            this.panelContainer.Controls.Add(this.pictureBox1);
            this.panelContainer.Controls.Add(this.picVideo);
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Location = new System.Drawing.Point(3, 64);
            this.panelContainer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(925, 367);
            this.panelContainer.TabIndex = 27;
            // 
            // tableLayoutPanelMap
            // 
            this.tableLayoutPanelMap.ColumnCount = 1;
            this.tableLayoutPanelMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMap.Controls.Add(this.panel4, 0, 0);
            this.tableLayoutPanelMap.Controls.Add(this.panel5, 0, 1);
            this.tableLayoutPanelMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMap.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMap.Name = "tableLayoutPanelMap";
            this.tableLayoutPanelMap.RowCount = 2;
            this.tableLayoutPanelMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMap.Size = new System.Drawing.Size(925, 367);
            this.tableLayoutPanelMap.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(198)))), ((int)(((byte)(242)))));
            this.panel4.Controls.Add(this.mapaddr);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(925, 30);
            this.panel4.TabIndex = 0;
            // 
            // mapaddr
            // 
            this.mapaddr.AutoSize = true;
            this.mapaddr.ForeColor = System.Drawing.Color.Teal;
            this.mapaddr.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mapaddr.Location = new System.Drawing.Point(83, 7);
            this.mapaddr.Name = "mapaddr";
            this.mapaddr.Size = new System.Drawing.Size(0, 20);
            this.mapaddr.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 23;
            this.label1.Text = "位置描述";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.pictureBoxGpsFail);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 30);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(925, 337);
            this.panel5.TabIndex = 1;
            // 
            // pictureBoxGpsFail
            // 
            this.pictureBoxGpsFail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(198)))), ((int)(((byte)(242)))));
            this.pictureBoxGpsFail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxGpsFail.Image = global::POCControlCenter.Properties.Resources.sos_gps_fail;
            this.pictureBoxGpsFail.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxGpsFail.Name = "pictureBoxGpsFail";
            this.pictureBoxGpsFail.Size = new System.Drawing.Size(925, 337);
            this.pictureBoxGpsFail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxGpsFail.TabIndex = 3;
            this.pictureBoxGpsFail.TabStop = false;
            this.pictureBoxGpsFail.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(925, 367);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // picVideo
            // 
            this.picVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picVideo.Location = new System.Drawing.Point(0, 0);
            this.picVideo.Name = "picVideo";
            this.picVideo.Size = new System.Drawing.Size(925, 367);
            this.picVideo.TabIndex = 28;
            this.picVideo.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(198)))), ((int)(((byte)(242)))));
            this.panel1.Controls.Add(this.username);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.duration_video);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.createdate_video);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(925, 52);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.trackBar1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 439);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(925, 33);
            this.panel2.TabIndex = 17;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(198)))), ((int)(((byte)(242)))));
            this.panel3.Controls.Add(this.comboBox1);
            this.panel3.Controls.Add(this.button3);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.media_state);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 480);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(925, 50);
            this.panel3.TabIndex = 18;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "4:3",
            "16:9",
            "原来比例"});
            this.comboBox1.Location = new System.Drawing.Point(239, 11);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(184, 27);
            this.comboBox1.TabIndex = 30;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button3.Location = new System.Drawing.Point(465, 4);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(86, 37);
            this.button3.TabIndex = 28;
            this.button3.Text = "暂停";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(753, 5);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 37);
            this.button1.TabIndex = 26;
            this.button1.Text = "重放";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timerVlc
            // 
            this.timerVlc.Interval = 500;
            this.timerVlc.Tick += new System.EventHandler(this.timerVlc_Tick);
            // 
            // MediaPlayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(198)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(931, 534);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MediaPlayForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "媒体在线播放";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MediaPlayForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MediaPlayForm_FormClosed);
            this.Load += new System.EventHandler(this.MediaPlayForm_Load);
            this.Resize += new System.EventHandler(this.MediaPlayForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelContainer.ResumeLayout(false);
            this.tableLayoutPanelMap.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGpsFail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVideo)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Label media_state;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panelContainer;
        public System.Windows.Forms.Label username;
        public System.Windows.Forms.Label createdate_video;
        public System.Windows.Forms.Label duration_video;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMap;
        private System.Windows.Forms.Panel panel4;
        public System.Windows.Forms.Label mapaddr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel5;
        public System.Windows.Forms.PictureBox pictureBoxGpsFail;
        private System.Windows.Forms.PictureBox picVideo;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Timer timerVlc;
    }
}