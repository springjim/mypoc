namespace POCControlCenter
{
    partial class DownFilesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownFilesForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.progressBarTotal = new System.Windows.Forms.ProgressBar();
            this.progressBarFile = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.btnView = new System.Windows.Forms.Button();
            this.btnState = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // progressBarTotal
            // 
            resources.ApplyResources(this.progressBarTotal, "progressBarTotal");
            this.progressBarTotal.Name = "progressBarTotal";
            // 
            // progressBarFile
            // 
            resources.ApplyResources(this.progressBarFile, "progressBarFile");
            this.progressBarFile.Name = "progressBarFile";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btnView
            // 
            this.btnView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.btnView.FlatAppearance.BorderSize = 0;
            this.btnView.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnView.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.btnView, "btnView");
            this.btnView.ForeColor = System.Drawing.Color.White;
            this.btnView.Name = "btnView";
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnState
            // 
            this.btnState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.btnState.FlatAppearance.BorderSize = 0;
            this.btnState.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnState.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.btnState, "btnState");
            this.btnState.ForeColor = System.Drawing.Color.White;
            this.btnState.Name = "btnState";
            this.btnState.UseVisualStyleBackColor = false;
            this.btnState.Click += new System.EventHandler(this.btnState_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // DownFilesForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnState);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.progressBarFile);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.progressBarTotal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "DownFilesForm";
            this.Shown += new System.EventHandler(this.DownFilesForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBarTotal;
        private System.Windows.Forms.ProgressBar progressBarFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnState;
        private System.Windows.Forms.Label label5;
    }
}