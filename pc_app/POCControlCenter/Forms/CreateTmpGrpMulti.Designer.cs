namespace POCControlCenter
{
    partial class CreateTmpGrpMulti
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateTmpGrpMulti));
            this.label1 = new System.Windows.Forms.Label();
            this.checkedListBoxMember = new System.Windows.Forms.CheckedListBox();
            this.textBoxGrpName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.cbBroadCast = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // checkedListBoxMember
            // 
            this.checkedListBoxMember.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBoxMember.CheckOnClick = true;
            this.checkedListBoxMember.FormattingEnabled = true;
            resources.ApplyResources(this.checkedListBoxMember, "checkedListBoxMember");
            this.checkedListBoxMember.Items.AddRange(new object[] {
            resources.GetString("checkedListBoxMember.Items"),
            resources.GetString("checkedListBoxMember.Items1")});
            this.checkedListBoxMember.Name = "checkedListBoxMember";
            this.checkedListBoxMember.Sorted = true;
            // 
            // textBoxGrpName
            // 
            resources.ApplyResources(this.textBoxGrpName, "textBoxGrpName");
            this.textBoxGrpName.Name = "textBoxGrpName";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
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
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.buttonOK.FlatAppearance.BorderSize = 0;
            this.buttonOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.buttonOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.ForeColor = System.Drawing.Color.White;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // cbBroadCast
            // 
            resources.ApplyResources(this.cbBroadCast, "cbBroadCast");
            this.cbBroadCast.ForeColor = System.Drawing.Color.Blue;
            this.cbBroadCast.Name = "cbBroadCast";
            this.cbBroadCast.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CreateTmpGrpMulti
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbBroadCast);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxGrpName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkedListBoxMember);
            this.Controls.Add(this.label1);
            this.Name = "CreateTmpGrpMulti";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox checkedListBoxMember;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonOK;
        public System.Windows.Forms.TextBox textBoxGrpName;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.CheckBox cbBroadCast;
    }
}