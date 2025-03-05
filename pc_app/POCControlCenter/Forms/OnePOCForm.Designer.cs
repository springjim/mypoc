namespace POCControlCenter
{
    partial class OnePOCForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnePOCForm));
            this.labelPlayerName = new System.Windows.Forms.Label();
            this.checkBoxGroupSync = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonTALK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelPlayerName
            // 
            resources.ApplyResources(this.labelPlayerName, "labelPlayerName");
            this.labelPlayerName.ForeColor = System.Drawing.Color.Green;
            this.labelPlayerName.Name = "labelPlayerName";
            this.labelPlayerName.Tag = "999";
            // 
            // checkBoxGroupSync
            // 
            resources.ApplyResources(this.checkBoxGroupSync, "checkBoxGroupSync");
            this.checkBoxGroupSync.Checked = true;
            this.checkBoxGroupSync.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGroupSync.Name = "checkBoxGroupSync";
            this.checkBoxGroupSync.UseVisualStyleBackColor = true;
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
            // pictureBox1
            // 
            this.pictureBox1.Image = global::POCControlCenter.Properties.Resources.shengbo_bar1;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // buttonTALK
            // 
            this.buttonTALK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.buttonTALK.FlatAppearance.BorderSize = 0;
            this.buttonTALK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.buttonTALK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.buttonTALK, "buttonTALK");
            this.buttonTALK.ForeColor = System.Drawing.Color.White;
            this.buttonTALK.Image = global::POCControlCenter.Properties.Resources.d_speak40;
            this.buttonTALK.Name = "buttonTALK";
            this.buttonTALK.UseVisualStyleBackColor = false;
            this.buttonTALK.Click += new System.EventHandler(this.buttonTALK_Click);
            // 
            // OnePOCForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonTALK);
            this.Controls.Add(this.checkBoxGroupSync);
            this.Controls.Add(this.labelPlayerName);
            this.Name = "OnePOCForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label labelPlayerName;
        private System.Windows.Forms.Button buttonTALK;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.CheckBox checkBoxGroupSync;
        public System.Windows.Forms.PictureBox pictureBox1;
    }
}