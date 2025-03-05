namespace POCControlCenter
{
    partial class UserGPSConfigForm
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
            this.locationInterval_str = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.locationMode_str = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.priv_hideLocSwitch_str = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.flag_autoLocation_str = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labUserName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // locationInterval_str
            // 
            this.locationInterval_str.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.locationInterval_str.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.locationInterval_str.FormattingEnabled = true;
            this.locationInterval_str.Location = new System.Drawing.Point(176, 225);
            this.locationInterval_str.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.locationInterval_str.Name = "locationInterval_str";
            this.locationInterval_str.Size = new System.Drawing.Size(187, 31);
            this.locationInterval_str.TabIndex = 43;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(53, 232);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(107, 23);
            this.label15.TabIndex = 42;
            this.label15.Text = "定位间隔(秒)";
            // 
            // locationMode_str
            // 
            this.locationMode_str.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.locationMode_str.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.locationMode_str.FormattingEnabled = true;
            this.locationMode_str.Items.AddRange(new object[] {
            "一般",
            "高精",
            "用户设置"});
            this.locationMode_str.Location = new System.Drawing.Point(176, 172);
            this.locationMode_str.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.locationMode_str.Name = "locationMode_str";
            this.locationMode_str.Size = new System.Drawing.Size(187, 31);
            this.locationMode_str.TabIndex = 41;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(80, 178);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(78, 23);
            this.label16.TabIndex = 40;
            this.label16.Text = "定位模式";
            // 
            // priv_hideLocSwitch_str
            // 
            this.priv_hideLocSwitch_str.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.priv_hideLocSwitch_str.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.priv_hideLocSwitch_str.FormattingEnabled = true;
            this.priv_hideLocSwitch_str.Items.AddRange(new object[] {
            "开通",
            "未开通"});
            this.priv_hideLocSwitch_str.Location = new System.Drawing.Point(176, 119);
            this.priv_hideLocSwitch_str.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.priv_hideLocSwitch_str.Name = "priv_hideLocSwitch_str";
            this.priv_hideLocSwitch_str.Size = new System.Drawing.Size(187, 31);
            this.priv_hideLocSwitch_str.TabIndex = 39;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(21, 125);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(145, 23);
            this.label17.TabIndex = 38;
            this.label17.Text = "终端隐藏GPS开关";
            // 
            // flag_autoLocation_str
            // 
            this.flag_autoLocation_str.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.flag_autoLocation_str.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.flag_autoLocation_str.FormattingEnabled = true;
            this.flag_autoLocation_str.Items.AddRange(new object[] {
            "开通",
            "未开通"});
            this.flag_autoLocation_str.Location = new System.Drawing.Point(176, 67);
            this.flag_autoLocation_str.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.flag_autoLocation_str.Name = "flag_autoLocation_str";
            this.flag_autoLocation_str.Size = new System.Drawing.Size(187, 31);
            this.flag_autoLocation_str.TabIndex = 37;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(48, 74);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(112, 23);
            this.label18.TabIndex = 36;
            this.label18.Text = "开启定位上报";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button2.Location = new System.Drawing.Point(176, 349);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 41);
            this.button2.TabIndex = 55;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.buttonOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.buttonOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOK.ForeColor = System.Drawing.Color.White;
            this.buttonOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonOK.Location = new System.Drawing.Point(375, 349);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(102, 41);
            this.buttonOK.TabIndex = 54;
            this.buttonOK.Text = "確定";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(44, 291);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(317, 23);
            this.label1.TabIndex = 56;
            this.label1.Text = "这里设置修改后，终端POC重启才能生效";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(380, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(281, 23);
            this.label2.TabIndex = 57;
            this.label2.Text = "如果选是，则终端没有修改GPS权限";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(80, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 23);
            this.label3.TabIndex = 58;
            this.label3.Text = "用户名称";
            // 
            // labUserName
            // 
            this.labUserName.AutoSize = true;
            this.labUserName.ForeColor = System.Drawing.Color.Blue;
            this.labUserName.Location = new System.Drawing.Point(172, 25);
            this.labUserName.Name = "labUserName";
            this.labUserName.Size = new System.Drawing.Size(14, 23);
            this.labUserName.TabIndex = 59;
            this.labUserName.Text = ".";
            // 
            // UserGPSConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 402);
            this.Controls.Add(this.labUserName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.locationInterval_str);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.locationMode_str);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.priv_hideLocSwitch_str);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.flag_autoLocation_str);
            this.Controls.Add(this.label18);
            this.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UserGPSConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GPS定位设置";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.UserGPSConfigForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox locationInterval_str;
        private System.Windows.Forms.Label label15;
        public System.Windows.Forms.ComboBox locationMode_str;
        private System.Windows.Forms.Label label16;
        public System.Windows.Forms.ComboBox priv_hideLocSwitch_str;
        private System.Windows.Forms.Label label17;
        public System.Windows.Forms.ComboBox flag_autoLocation_str;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label labUserName;
    }
}