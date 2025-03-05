namespace POCControlCenter.BroadCast
{
    partial class UserControlBroad
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
            this.labUserName = new System.Windows.Forms.Label();
            this.picDel = new System.Windows.Forms.PictureBox();
            this.picMic = new System.Windows.Forms.PictureBox();
            this.picSwitch = new System.Windows.Forms.PictureBox();
            this.picUserImg = new System.Windows.Forms.PictureBox();
            this.picInvite = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picDel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSwitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInvite)).BeginInit();
            this.SuspendLayout();
            // 
            // labUserName
            // 
            this.labUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labUserName.ForeColor = System.Drawing.SystemColors.Control;
            this.labUserName.Location = new System.Drawing.Point(3, 107);
            this.labUserName.Name = "labUserName";
            this.labUserName.Size = new System.Drawing.Size(139, 16);
            this.labUserName.TabIndex = 4;
            this.labUserName.Text = ".";
            this.labUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picDel
            // 
            this.picDel.Image = global::POCControlCenter.Properties.Resources.kick_user;
            this.picDel.Location = new System.Drawing.Point(3, 7);
            this.picDel.Name = "picDel";
            this.picDel.Size = new System.Drawing.Size(32, 29);
            this.picDel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDel.TabIndex = 3;
            this.picDel.TabStop = false;
            // 
            // picMic
            // 
            this.picMic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picMic.Image = global::POCControlCenter.Properties.Resources.talk_mic_pressed;
            this.picMic.Location = new System.Drawing.Point(110, 4);
            this.picMic.Name = "picMic";
            this.picMic.Size = new System.Drawing.Size(32, 39);
            this.picMic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMic.TabIndex = 2;
            this.picMic.TabStop = false;
            // 
            // picSwitch
            // 
            this.picSwitch.Image = global::POCControlCenter.Properties.Resources.ic_action_switch_camera;
            this.picSwitch.Location = new System.Drawing.Point(3, 49);
            this.picSwitch.Name = "picSwitch";
            this.picSwitch.Size = new System.Drawing.Size(32, 39);
            this.picSwitch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picSwitch.TabIndex = 1;
            this.picSwitch.TabStop = false;
            // 
            // picUserImg
            // 
            this.picUserImg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picUserImg.Image = global::POCControlCenter.Properties.Resources.user_icon_online;
            this.picUserImg.Location = new System.Drawing.Point(41, 39);
            this.picUserImg.Name = "picUserImg";
            this.picUserImg.Size = new System.Drawing.Size(60, 60);
            this.picUserImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picUserImg.TabIndex = 0;
            this.picUserImg.TabStop = false;
            // 
            // picInvite
            // 
            this.picInvite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picInvite.BackColor = System.Drawing.Color.Transparent;
            this.picInvite.Image = global::POCControlCenter.Properties.Resources.addgrp;
            this.picInvite.Location = new System.Drawing.Point(108, 49);
            this.picInvite.Name = "picInvite";
            this.picInvite.Size = new System.Drawing.Size(32, 39);
            this.picInvite.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picInvite.TabIndex = 5;
            this.picInvite.TabStop = false;
            // 
            // UserControlBroad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.Controls.Add(this.picInvite);
            this.Controls.Add(this.labUserName);
            this.Controls.Add(this.picDel);
            this.Controls.Add(this.picMic);
            this.Controls.Add(this.picSwitch);
            this.Controls.Add(this.picUserImg);
            this.DoubleBuffered = true;
            this.Name = "UserControlBroad";
            this.Size = new System.Drawing.Size(145, 126);
            ((System.ComponentModel.ISupportInitialize)(this.picDel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSwitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInvite)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.PictureBox picSwitch;
        public System.Windows.Forms.PictureBox picMic;
        public System.Windows.Forms.PictureBox picDel;
        public System.Windows.Forms.PictureBox picUserImg;
        public System.Windows.Forms.Label labUserName;
        public System.Windows.Forms.PictureBox picInvite;
    }
}
