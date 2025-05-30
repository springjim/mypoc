﻿namespace POCControlCenter
{
    partial class DateTimeSelForm
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
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.SuspendLayout();
            // 
            // monthCalendar
            // 
            this.monthCalendar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monthCalendar.Location = new System.Drawing.Point(0, 0);
            this.monthCalendar.Margin = new System.Windows.Forms.Padding(10, 13, 10, 13);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 0;
            this.monthCalendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar_DateSelected);
            // 
            // DateTimeSelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 207);
            this.Controls.Add(this.monthCalendar);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DateTimeSelForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "日期选择";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DateTimeSelForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.MonthCalendar monthCalendar;
    }
}