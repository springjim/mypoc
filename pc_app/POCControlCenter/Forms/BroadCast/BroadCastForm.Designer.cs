namespace POCControlCenter.BroadCast
{
    partial class BroadCastForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BroadCastForm));
            this.labGroupName = new System.Windows.Forms.Label();
            this.flpUserPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.btnSelFile = new System.Windows.Forms.Button();
            this.buttonTALK = new System.Windows.Forms.Button();
            this.labelPlayerName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labGroupName
            // 
            this.labGroupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labGroupName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(233)))));
            this.labGroupName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labGroupName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labGroupName.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.labGroupName.Location = new System.Drawing.Point(4, 4);
            this.labGroupName.Name = "labGroupName";
            this.labGroupName.Size = new System.Drawing.Size(1088, 38);
            this.labGroupName.TabIndex = 0;
            this.labGroupName.Text = "语音广播: XXX";
            this.labGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labGroupName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnFormMouseDown);
            this.labGroupName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnFormMouseMove);
            this.labGroupName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnFormMouseUp);
            // 
            // flpUserPanel
            // 
            this.flpUserPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpUserPanel.AutoScroll = true;
            this.flpUserPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpUserPanel.BackColor = System.Drawing.Color.LightGray;
            this.flpUserPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpUserPanel.Location = new System.Drawing.Point(4, 45);
            this.flpUserPanel.Name = "flpUserPanel";
            this.flpUserPanel.Size = new System.Drawing.Size(1088, 410);
            this.flpUserPanel.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Red;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClose.Location = new System.Drawing.Point(971, 465);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(112, 49);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "退出广播";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAddUser
            // 
            this.btnAddUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.btnAddUser.FlatAppearance.BorderSize = 0;
            this.btnAddUser.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnAddUser.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnAddUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddUser.ForeColor = System.Drawing.Color.White;
            this.btnAddUser.Image = global::POCControlCenter.Properties.Resources.addgroupx;
            this.btnAddUser.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddUser.Location = new System.Drawing.Point(15, 469);
            this.btnAddUser.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(147, 41);
            this.btnAddUser.TabIndex = 18;
            this.btnAddUser.Text = "新增人员";
            this.btnAddUser.UseVisualStyleBackColor = false;
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
            // 
            // btnSelFile
            // 
            this.btnSelFile.BackColor = System.Drawing.Color.Gainsboro;
            this.btnSelFile.FlatAppearance.BorderSize = 0;
            this.btnSelFile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnSelFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnSelFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelFile.ForeColor = System.Drawing.Color.Black;
            this.btnSelFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSelFile.Location = new System.Drawing.Point(177, 469);
            this.btnSelFile.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.btnSelFile.Name = "btnSelFile";
            this.btnSelFile.Size = new System.Drawing.Size(137, 41);
            this.btnSelFile.TabIndex = 19;
            this.btnSelFile.Text = "选择录音广播...";
            this.btnSelFile.UseVisualStyleBackColor = false;
            this.btnSelFile.Click += new System.EventHandler(this.btnSelFile_Click);
            // 
            // buttonTALK
            // 
            this.buttonTALK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.buttonTALK.FlatAppearance.BorderSize = 0;
            this.buttonTALK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.buttonTALK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.buttonTALK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTALK.ForeColor = System.Drawing.Color.White;
            this.buttonTALK.Image = global::POCControlCenter.Properties.Resources.d_speak40;
            this.buttonTALK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonTALK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonTALK.Location = new System.Drawing.Point(793, 465);
            this.buttonTALK.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.buttonTALK.Name = "buttonTALK";
            this.buttonTALK.Size = new System.Drawing.Size(155, 49);
            this.buttonTALK.TabIndex = 20;
            this.buttonTALK.Text = "开始广播";
            this.buttonTALK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonTALK.UseVisualStyleBackColor = false;
            this.buttonTALK.Click += new System.EventHandler(this.buttonTALK_Click);
            // 
            // labelPlayerName
            // 
            this.labelPlayerName.AutoSize = true;
            this.labelPlayerName.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelPlayerName.ForeColor = System.Drawing.Color.Green;
            this.labelPlayerName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelPlayerName.Location = new System.Drawing.Point(566, 470);
            this.labelPlayerName.Name = "labelPlayerName";
            this.labelPlayerName.Size = new System.Drawing.Size(21, 31);
            this.labelPlayerName.TabIndex = 21;
            this.labelPlayerName.Tag = "999";
            this.labelPlayerName.Text = ".";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(481, 478);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 22;
            this.label1.Text = "讲话人:";
            // 
            // BroadCastForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(1095, 533);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelPlayerName);
            this.Controls.Add(this.buttonTALK);
            this.Controls.Add(this.btnSelFile);
            this.Controls.Add(this.btnAddUser);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.flpUserPanel);
            this.Controls.Add(this.labGroupName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BroadCastForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BroadCastForm";
            this.Load += new System.EventHandler(this.BroadCastForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        public System.Windows.Forms.FlowLayoutPanel flpUserPanel;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.Button btnSelFile;
        public System.Windows.Forms.Label labGroupName;
        private System.Windows.Forms.Button buttonTALK;
        public System.Windows.Forms.Label labelPlayerName;
        private System.Windows.Forms.Label label1;
    }
}