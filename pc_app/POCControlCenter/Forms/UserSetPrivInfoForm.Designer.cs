namespace POCControlCenter
{
    partial class UserPrivSetInfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserPrivSetInfoForm));
            this.label1 = new System.Windows.Forms.Label();
            this.labelUserName = new System.Windows.Forms.Label();
            this.labelItemName = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cbSwitchRight = new System.Windows.Forms.ComboBox();
            this.cbSwitchRecord = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbSwitchLife = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // labelUserName
            // 
            resources.ApplyResources(this.labelUserName, "labelUserName");
            this.labelUserName.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labelUserName.Name = "labelUserName";
            // 
            // labelItemName
            // 
            resources.ApplyResources(this.labelItemName, "labelItemName");
            this.labelItemName.Name = "labelItemName";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.button1, "button1");
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.button2, "button2");
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // cbSwitchRight
            // 
            this.cbSwitchRight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbSwitchRight, "cbSwitchRight");
            this.cbSwitchRight.FormattingEnabled = true;
            this.cbSwitchRight.Name = "cbSwitchRight";
            // 
            // cbSwitchRecord
            // 
            this.cbSwitchRecord.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbSwitchRecord, "cbSwitchRecord");
            this.cbSwitchRecord.FormattingEnabled = true;
            this.cbSwitchRecord.Name = "cbSwitchRecord";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cbSwitchLife
            // 
            this.cbSwitchLife.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbSwitchLife, "cbSwitchLife");
            this.cbSwitchLife.FormattingEnabled = true;
            this.cbSwitchLife.Name = "cbSwitchLife";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // UserPrivSetInfoForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbSwitchLife);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbSwitchRecord);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbSwitchRight);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelItemName);
            this.Controls.Add(this.labelUserName);
            this.Controls.Add(this.label1);
            this.Name = "UserPrivSetInfoForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.Label labelUserName;
        public System.Windows.Forms.ComboBox cbSwitchRight;
        public System.Windows.Forms.Label labelItemName;
        public System.Windows.Forms.ComboBox cbSwitchRecord;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox cbSwitchLife;
        public System.Windows.Forms.Label label3;
    }
}