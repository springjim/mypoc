namespace POCControlCenter.BroadCast
{
    partial class TempTalkForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TempTalkForm));
            this.label1 = new System.Windows.Forms.Label();
            this.labelPlayerName = new System.Windows.Forms.Label();
            this.buttonTALK = new System.Windows.Forms.Button();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.flpUserPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.labGroupName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(481, 483);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 30;
            this.label1.Text = "讲话人:";
            // 
            // labelPlayerName
            // 
            this.labelPlayerName.AutoSize = true;
            this.labelPlayerName.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelPlayerName.ForeColor = System.Drawing.Color.Green;
            this.labelPlayerName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelPlayerName.Location = new System.Drawing.Point(566, 475);
            this.labelPlayerName.Name = "labelPlayerName";
            this.labelPlayerName.Size = new System.Drawing.Size(21, 31);
            this.labelPlayerName.TabIndex = 29;
            this.labelPlayerName.Tag = "999";
            this.labelPlayerName.Text = ".";
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
            this.buttonTALK.Location = new System.Drawing.Point(793, 470);
            this.buttonTALK.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.buttonTALK.Name = "buttonTALK";
            this.buttonTALK.Size = new System.Drawing.Size(155, 49);
            this.buttonTALK.TabIndex = 28;
            this.buttonTALK.Text = "开始对讲";
            this.buttonTALK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonTALK.UseVisualStyleBackColor = false;
            this.buttonTALK.Click += new System.EventHandler(this.buttonTALK_Click);
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
            this.btnAddUser.Location = new System.Drawing.Point(15, 474);
            this.btnAddUser.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(147, 41);
            this.btnAddUser.TabIndex = 26;
            this.btnAddUser.Text = "新增人员";
            this.btnAddUser.UseVisualStyleBackColor = false;
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
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
            this.btnClose.Location = new System.Drawing.Point(971, 470);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(112, 49);
            this.btnClose.TabIndex = 25;
            this.btnClose.Text = "结束对讲";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
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
            this.flpUserPanel.Size = new System.Drawing.Size(1088, 415);
            this.flpUserPanel.TabIndex = 24;
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
            this.labGroupName.TabIndex = 23;
            this.labGroupName.Text = "临时对讲: XXX";
            this.labGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labGroupName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnFormMouseDown);
            this.labGroupName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnFormMouseMove);
            this.labGroupName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnFormMouseUp);
            // 
            // TempTalkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(1095, 533);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelPlayerName);
            this.Controls.Add(this.buttonTALK);
            this.Controls.Add(this.btnAddUser);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.flpUserPanel);
            this.Controls.Add(this.labGroupName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TempTalkForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TempTalkForm";
            this.Load += new System.EventHandler(this.TempTalkForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label labelPlayerName;
        private System.Windows.Forms.Button buttonTALK;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.Button btnClose;
        public System.Windows.Forms.FlowLayoutPanel flpUserPanel;
        public System.Windows.Forms.Label labGroupName;
    }
}