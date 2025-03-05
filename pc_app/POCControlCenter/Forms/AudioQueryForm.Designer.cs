namespace POCControlCenter
{
    partial class AudioQueryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioQueryForm));
            this.cbPageSize = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.audio_duration_to = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.audio_duration_from = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.record_date_to = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.record_date_from = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.user_name_like = new System.Windows.Forms.CheckBox();
            this.user_name = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cust_account_like = new System.Windows.Forms.CheckBox();
            this.cust_account = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.poc_account_like = new System.Windows.Forms.CheckBox();
            this.poc_account = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbGroup = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbPageSize
            // 
            this.cbPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbPageSize, "cbPageSize");
            this.cbPageSize.FormattingEnabled = true;
            this.cbPageSize.Items.AddRange(new object[] {
            resources.GetString("cbPageSize.Items"),
            resources.GetString("cbPageSize.Items1"),
            resources.GetString("cbPageSize.Items2"),
            resources.GetString("cbPageSize.Items3"),
            resources.GetString("cbPageSize.Items4"),
            resources.GetString("cbPageSize.Items5"),
            resources.GetString("cbPageSize.Items6")});
            this.cbPageSize.Name = "cbPageSize";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.ForeColor = System.Drawing.Color.Teal;
            this.label11.Name = "label11";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.button2, "button2");
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.button1, "button1");
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // audio_duration_to
            // 
            resources.ApplyResources(this.audio_duration_to, "audio_duration_to");
            this.audio_duration_to.Name = "audio_duration_to";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // audio_duration_from
            // 
            resources.ApplyResources(this.audio_duration_from, "audio_duration_from");
            this.audio_duration_from.Name = "audio_duration_from";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // record_date_to
            // 
            resources.ApplyResources(this.record_date_to, "record_date_to");
            this.record_date_to.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.record_date_to.Name = "record_date_to";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // record_date_from
            // 
            resources.ApplyResources(this.record_date_from, "record_date_from");
            this.record_date_from.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.record_date_from.Name = "record_date_from";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // user_name_like
            // 
            resources.ApplyResources(this.user_name_like, "user_name_like");
            this.user_name_like.Name = "user_name_like";
            this.user_name_like.UseVisualStyleBackColor = true;
            // 
            // user_name
            // 
            resources.ApplyResources(this.user_name, "user_name");
            this.user_name.Name = "user_name";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // cust_account_like
            // 
            resources.ApplyResources(this.cust_account_like, "cust_account_like");
            this.cust_account_like.Name = "cust_account_like";
            this.cust_account_like.UseVisualStyleBackColor = true;
            // 
            // cust_account
            // 
            resources.ApplyResources(this.cust_account, "cust_account");
            this.cust_account.Name = "cust_account";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // poc_account_like
            // 
            resources.ApplyResources(this.poc_account_like, "poc_account_like");
            this.poc_account_like.Name = "poc_account_like";
            this.poc_account_like.UseVisualStyleBackColor = true;
            // 
            // poc_account
            // 
            resources.ApplyResources(this.poc_account, "poc_account");
            this.poc_account.Name = "poc_account";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cbGroup
            // 
            this.cbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbGroup, "cbGroup");
            this.cbGroup.FormattingEnabled = true;
            this.cbGroup.Items.AddRange(new object[] {
            resources.GetString("cbGroup.Items"),
            resources.GetString("cbGroup.Items1"),
            resources.GetString("cbGroup.Items2")});
            this.cbGroup.Name = "cbGroup";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // AudioQueryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbPageSize);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.audio_duration_to);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.audio_duration_from);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.record_date_to);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.record_date_from);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.user_name_like);
            this.Controls.Add(this.user_name);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cust_account_like);
            this.Controls.Add(this.cust_account);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.poc_account_like);
            this.Controls.Add(this.poc_account);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbGroup);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Name = "AudioQueryForm";
            this.Tag = "99";
            this.Load += new System.EventHandler(this.AudioQueryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox cbPageSize;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox audio_duration_to;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox audio_duration_from;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.DateTimePicker record_date_to;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.DateTimePicker record_date_from;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.CheckBox user_name_like;
        public System.Windows.Forms.TextBox user_name;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.CheckBox cust_account_like;
        public System.Windows.Forms.TextBox cust_account;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.CheckBox poc_account_like;
        public System.Windows.Forms.TextBox poc_account;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox cbGroup;
        private System.Windows.Forms.Label label1;
    }
}