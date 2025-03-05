namespace POCControlCenter.Agora
{
    partial class AgoraAVInviteForm
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
            this.labelFromUserName = new System.Windows.Forms.Label();
            this.labVideoType = new System.Windows.Forms.Label();
            this.btn_receive = new System.Windows.Forms.PictureBox();
            this.btn_hangup = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btn_receive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_hangup)).BeginInit();
            this.SuspendLayout();
            // 
            // labelFromUserName
            // 
            this.labelFromUserName.AutoSize = true;
            this.labelFromUserName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelFromUserName.ForeColor = System.Drawing.Color.Blue;
            this.labelFromUserName.Location = new System.Drawing.Point(44, 35);
            this.labelFromUserName.Name = "labelFromUserName";
            this.labelFromUserName.Size = new System.Drawing.Size(39, 20);
            this.labelFromUserName.TabIndex = 0;
            this.labelFromUserName.Text = "xxx";
            // 
            // labVideoType
            // 
            this.labVideoType.AutoSize = true;
            this.labVideoType.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labVideoType.Location = new System.Drawing.Point(44, 89);
            this.labVideoType.Name = "labVideoType";
            this.labVideoType.Size = new System.Drawing.Size(179, 20);
            this.labVideoType.TabIndex = 1;
            this.labVideoType.Text = "音视频通话邀请...";
            // 
            // btn_receive
            // 
            this.btn_receive.Image = global::POCControlCenter.Properties.Resources.btn_startcall_normal;
            this.btn_receive.Location = new System.Drawing.Point(421, 75);
            this.btn_receive.Name = "btn_receive";
            this.btn_receive.Size = new System.Drawing.Size(60, 60);
            this.btn_receive.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btn_receive.TabIndex = 3;
            this.btn_receive.TabStop = false;
            this.btn_receive.Click += new System.EventHandler(this.btn_receive_Click);
            // 
            // btn_hangup
            // 
            this.btn_hangup.Image = global::POCControlCenter.Properties.Resources.btn_endcall_normal;
            this.btn_hangup.Location = new System.Drawing.Point(558, 75);
            this.btn_hangup.Name = "btn_hangup";
            this.btn_hangup.Size = new System.Drawing.Size(60, 60);
            this.btn_hangup.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btn_hangup.TabIndex = 2;
            this.btn_hangup.TabStop = false;
            this.btn_hangup.Click += new System.EventHandler(this.hangup_Click);
            // 
            // AgoraAVInviteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 151);
            this.Controls.Add(this.btn_receive);
            this.Controls.Add(this.btn_hangup);
            this.Controls.Add(this.labVideoType);
            this.Controls.Add(this.labelFromUserName);
            this.Name = "AgoraAVInviteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "音视频通话邀请";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AgoraAVInviteForm_FormClosed);
            this.Load += new System.EventHandler(this.AgoraAVInviteForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btn_receive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_hangup)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labVideoType;
        private System.Windows.Forms.PictureBox btn_hangup;
        private System.Windows.Forms.PictureBox btn_receive;
        public System.Windows.Forms.Label labelFromUserName;
    }
}