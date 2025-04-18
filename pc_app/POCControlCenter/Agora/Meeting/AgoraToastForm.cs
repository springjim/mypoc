﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.Agora.Meeting
{
    public partial class AgoraToastForm : Form
    {
        private System.Timers.Timer mTimer = null;
        private AgoraMeetForm mMainForm;
        public AgoraToastForm(AgoraMeetForm form)
        {
            InitializeComponent();
            mMainForm = form;
            this.Disposed += new EventHandler(OnDisposed);

            // 显示在顶部中间
            int x = (System.Windows.Forms.SystemInformation.WorkingArea.Width - this.ClientSize.Width) / 2;
            int y = 0;
            //this.Height = 42;
            this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            this.Location = (Point)new Size(x, y);         //窗体的起始位置为(x,y)

        }

        private void OnDisposed(object sender, EventArgs e)
        {
            //清理资源
            if (mTimer != null)
            {
                mTimer.Stop();
            }
        }
        public void SetText(string text, int time = 0)
        {
            this.textLabel.Text = text;
            if (time != 0)
            {
                mTimer = new System.Timers.Timer(time);
                mTimer.Interval = time;
                mTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerEvent);
                mTimer.Start();
            }
        }

        private void OnTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                this.DialogResult = DialogResult.Abort;
                mTimer.Stop();
                this.Hide();
            }));
        }

        public void SetPosition(int x, int y)
        {
            this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            this.Location = (Point)new Size(x, y);         //窗体的起始位置为(x,y)
        }

        public void SetBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        public void SetTextColor(Color color)
        {
            this.textLabel.ForeColor = color;
        }

        private void exitPicBox_Click(object sender, EventArgs e)
        {
            mMainForm.onEndScreenShare();
        }
    }
}
