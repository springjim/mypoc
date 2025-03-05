namespace POCControlCenter.BroadCast
{
    partial class RecorderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecorderForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.btnTxtSpeech = new System.Windows.Forms.Button();
            this.tbText = new System.Windows.Forms.TextBox();
            this.picGif = new System.Windows.Forms.PictureBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGif)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.radioButton2);
            this.panel1.Controls.Add(this.radioButton1);
            this.panel1.Controls.Add(this.btnTxtSpeech);
            this.panel1.Controls.Add(this.tbText);
            this.panel1.Controls.Add(this.picGif);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Controls.Add(this.btnRecord);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(672, 588);
            this.panel1.TabIndex = 7;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(324, 534);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(88, 19);
            this.radioButton2.TabIndex = 13;
            this.radioButton2.Text = "成年女声";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(202, 534);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(88, 19);
            this.radioButton1.TabIndex = 12;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "成年男声";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // btnTxtSpeech
            // 
            this.btnTxtSpeech.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.btnTxtSpeech.ForeColor = System.Drawing.Color.White;
            this.btnTxtSpeech.Location = new System.Drawing.Point(27, 522);
            this.btnTxtSpeech.Name = "btnTxtSpeech";
            this.btnTxtSpeech.Size = new System.Drawing.Size(155, 48);
            this.btnTxtSpeech.TabIndex = 11;
            this.btnTxtSpeech.Text = "文字合成录音";
            this.btnTxtSpeech.UseVisualStyleBackColor = false;
            this.btnTxtSpeech.Click += new System.EventHandler(this.btnTxtSpeech_Click);
            // 
            // tbText
            // 
            this.tbText.Location = new System.Drawing.Point(27, 306);
            this.tbText.Multiline = true;
            this.tbText.Name = "tbText";
            this.tbText.Size = new System.Drawing.Size(617, 200);
            this.tbText.TabIndex = 10;
            // 
            // picGif
            // 
            this.picGif.Location = new System.Drawing.Point(27, 34);
            this.picGif.Name = "picGif";
            this.picGif.Size = new System.Drawing.Size(617, 212);
            this.picGif.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picGif.TabIndex = 9;
            this.picGif.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(516, 522);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(128, 48);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "关闭窗口";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnRecord
            // 
            this.btnRecord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.btnRecord.ForeColor = System.Drawing.Color.White;
            this.btnRecord.Location = new System.Drawing.Point(27, 252);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(155, 48);
            this.btnRecord.TabIndex = 7;
            this.btnRecord.Text = "开始麦克风录音";
            this.btnRecord.UseVisualStyleBackColor = false;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(233)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(670, 31);
            this.label1.TabIndex = 14;
            this.label1.Text = "录音文件制作";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnFormMouseDown);
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnFormMouseMove);
            this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnFormMouseUp);
            // 
            // RecorderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(672, 588);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RecorderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RecorderForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGif)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button btnTxtSpeech;
        private System.Windows.Forms.TextBox tbText;
        private System.Windows.Forms.PictureBox picGif;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Label label1;
    }
}