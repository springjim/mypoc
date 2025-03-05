namespace POCControlCenter
{
    partial class SOSPopUpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SOSPopUpForm));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labAddr = new System.Windows.Forms.Label();
            this.labUsername = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labTime = new System.Windows.Forms.Label();
            this.labUserid = new System.Windows.Forms.Label();
            this.labGroupName = new System.Windows.Forms.Label();
            this.labelPlayerName = new System.Windows.Forms.Label();
            this.maploading = new System.Windows.Forms.PictureBox();
            this.buttonTALK = new System.Windows.Forms.Button();
            this.webMap = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.maploading)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.buttonCancel.FlatAppearance.BorderSize = 0;
            this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.buttonCancel.ForeColor = System.Drawing.Color.White;
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(16, 282);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(102, 33);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "上一个";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Visible = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(208, 282);
            this.button1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 35);
            this.button1.TabIndex = 8;
            this.button1.Text = "下一个";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(149, 288);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 23);
            this.label1.TabIndex = 9;
            this.label1.Text = "1/1";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 248);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 23);
            this.label2.TabIndex = 10;
            this.label2.Text = "报警位址:";
            // 
            // labAddr
            // 
            this.labAddr.AutoSize = true;
            this.labAddr.ForeColor = System.Drawing.Color.DarkBlue;
            this.labAddr.Location = new System.Drawing.Point(82, 248);
            this.labAddr.Name = "labAddr";
            this.labAddr.Size = new System.Drawing.Size(141, 23);
            this.labAddr.TabIndex = 11;
            this.labAddr.Text = "正在获取位置中...";
            // 
            // labUsername
            // 
            this.labUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labUsername.BackColor = System.Drawing.Color.DarkSlateGray;
            this.labUsername.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labUsername.ForeColor = System.Drawing.Color.Snow;
            this.labUsername.Location = new System.Drawing.Point(422, 13);
            this.labUsername.Name = "labUsername";
            this.labUsername.Size = new System.Drawing.Size(255, 42);
            this.labUsername.TabIndex = 12;
            this.labUsername.Text = "...";
            this.labUsername.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(423, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 23);
            this.label4.TabIndex = 13;
            this.label4.Text = "时间";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(435, 267);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 23);
            this.label5.TabIndex = 14;
            this.label5.Text = "帐号";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(435, 296);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 23);
            this.label6.TabIndex = 15;
            this.label6.Text = "所在群组";
            // 
            // labTime
            // 
            this.labTime.AutoSize = true;
            this.labTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.labTime.Location = new System.Drawing.Point(514, 99);
            this.labTime.Name = "labTime";
            this.labTime.Size = new System.Drawing.Size(22, 23);
            this.labTime.TabIndex = 16;
            this.labTime.Text = "...";
            // 
            // labUserid
            // 
            this.labUserid.AutoSize = true;
            this.labUserid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.labUserid.Location = new System.Drawing.Point(526, 267);
            this.labUserid.Name = "labUserid";
            this.labUserid.Size = new System.Drawing.Size(22, 23);
            this.labUserid.TabIndex = 17;
            this.labUserid.Text = "...";
            // 
            // labGroupName
            // 
            this.labGroupName.AutoSize = true;
            this.labGroupName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.labGroupName.Location = new System.Drawing.Point(526, 296);
            this.labGroupName.Name = "labGroupName";
            this.labGroupName.Size = new System.Drawing.Size(22, 23);
            this.labGroupName.TabIndex = 18;
            this.labGroupName.Text = "...";
            // 
            // labelPlayerName
            // 
            this.labelPlayerName.AutoSize = true;
            this.labelPlayerName.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold);
            this.labelPlayerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.labelPlayerName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelPlayerName.Location = new System.Drawing.Point(422, 55);
            this.labelPlayerName.Name = "labelPlayerName";
            this.labelPlayerName.Size = new System.Drawing.Size(63, 36);
            this.labelPlayerName.TabIndex = 19;
            this.labelPlayerName.Tag = "999";
            this.labelPlayerName.Text = "......";
            // 
            // maploading
            // 
            this.maploading.Image = ((System.Drawing.Image)(resources.GetObject("maploading.Image")));
            this.maploading.Location = new System.Drawing.Point(12, 13);
            this.maploading.Name = "maploading";
            this.maploading.Size = new System.Drawing.Size(403, 232);
            this.maploading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.maploading.TabIndex = 20;
            this.maploading.TabStop = false;
            // 
            // buttonTALK
            // 
            this.buttonTALK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTALK.BackColor = System.Drawing.Color.Green;
            this.buttonTALK.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonTALK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTALK.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold);
            this.buttonTALK.ForeColor = System.Drawing.Color.White;
            this.buttonTALK.Image = global::POCControlCenter.Properties.Resources.talk_allow;
            this.buttonTALK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonTALK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonTALK.Location = new System.Drawing.Point(427, 163);
            this.buttonTALK.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.buttonTALK.Name = "buttonTALK";
            this.buttonTALK.Size = new System.Drawing.Size(250, 53);
            this.buttonTALK.TabIndex = 5;
            this.buttonTALK.Text = "点击对讲";
            this.buttonTALK.UseVisualStyleBackColor = false;
            this.buttonTALK.Click += new System.EventHandler(this.buttonTALK_Click);
            // 
            // webMap
            // 
            this.webMap.Location = new System.Drawing.Point(12, 13);
            this.webMap.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.webMap.MinimumSize = new System.Drawing.Size(23, 29);
            this.webMap.Name = "webMap";
            this.webMap.Size = new System.Drawing.Size(403, 231);
            this.webMap.TabIndex = 0;
            this.webMap.Visible = false;
            this.webMap.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webMap_DocumentCompleted);
            // 
            // SOSPopUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gold;
            this.ClientSize = new System.Drawing.Size(689, 324);
            this.Controls.Add(this.maploading);
            this.Controls.Add(this.labelPlayerName);
            this.Controls.Add(this.labGroupName);
            this.Controls.Add(this.labUserid);
            this.Controls.Add(this.labTime);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labUsername);
            this.Controls.Add(this.labAddr);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonTALK);
            this.Controls.Add(this.webMap);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SOSPopUpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SOS报警";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SOSPopUpForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.maploading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonTALK;
        public System.Windows.Forms.Button buttonCancel;
        public System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.WebBrowser webMap;
        public System.Windows.Forms.Label labAddr;
        public System.Windows.Forms.Label labUsername;
        public System.Windows.Forms.Label labTime;
        public System.Windows.Forms.Label labUserid;
        public System.Windows.Forms.Label labGroupName;
        public System.Windows.Forms.Label labelPlayerName;
        public System.Windows.Forms.PictureBox maploading;
    }
}