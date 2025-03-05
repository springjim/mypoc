namespace POCControlCenter
{
    partial class PersonCallForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonCallForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelcall = new System.Windows.Forms.Label();
            this.labelcallname = new System.Windows.Forms.Label();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.buttonhangup = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timerCallTime = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelcall, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelcallname, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonAccept, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonhangup, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // labelcall
            // 
            resources.ApplyResources(this.labelcall, "labelcall");
            this.labelcall.Name = "labelcall";
            // 
            // labelcallname
            // 
            resources.ApplyResources(this.labelcallname, "labelcallname");
            this.tableLayoutPanel1.SetColumnSpan(this.labelcallname, 2);
            this.labelcallname.ForeColor = System.Drawing.Color.Red;
            this.labelcallname.Name = "labelcallname";
            // 
            // buttonAccept
            // 
            resources.ApplyResources(this.buttonAccept, "buttonAccept");
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // buttonhangup
            // 
            resources.ApplyResources(this.buttonhangup, "buttonhangup");
            this.buttonhangup.Name = "buttonhangup";
            this.buttonhangup.UseVisualStyleBackColor = true;
            this.buttonhangup.Click += new System.EventHandler(this.buttonhangup_Click);
            // 
            // pictureBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.pictureBox1, 3);
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::POCControlCenter.Properties.Resources.bk2012011410361007;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // timerCallTime
            // 
            this.timerCallTime.Tick += new System.EventHandler(this.timerCallTime_Tick);
            // 
            // PersonCallForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PersonCallForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PersonCallForm_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelcall;
        private System.Windows.Forms.Label labelcallname;
        private System.Windows.Forms.Button buttonAccept;
        private System.Windows.Forms.Button buttonhangup;
        private System.Windows.Forms.Timer timerCallTime;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}