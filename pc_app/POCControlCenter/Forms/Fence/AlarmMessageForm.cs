using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace POCControlCenter.Fence
{
    public partial class AlarmMessageForm : Form
    {
        private bool mIsMouseDown = false;
        private Point mFormLocation;     // Form的location
        private Point mMouseOffset;      // 鼠标的按下位置
        private System.Timers.Timer mTimer = null;

        public int fenceAlarmNotifyId = 0;
        public string fencePoints;
        public string endLatitude;
        public string endLongitude;
        public AlarmMessageForm()
        {
            InitializeComponent();
        }

        public void setText(int delayCloseMs = 0)
        {
            
            if (delayCloseMs != 0)
            {
                mTimer = new System.Timers.Timer(delayCloseMs);
                mTimer.Interval = delayCloseMs;
                mTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerEvent);
                mTimer.Start();
            }
        }

        public void OnTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                this.DialogResult = DialogResult.Abort;
                mTimer.Stop();
                this.Close();
            }));
        }
        

        private void OnFormMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mIsMouseDown = true;
                mFormLocation = this.Location;
                mMouseOffset = Control.MousePosition;
            }
        }

        private void OnFormMouseMove(object sender, MouseEventArgs e)
        {
            if (mIsMouseDown)
            {
                Point pt = Control.MousePosition;
                int x = mMouseOffset.X - pt.X;
                int y = mMouseOffset.Y - pt.Y;

                this.Location = new Point(mFormLocation.X - x, mFormLocation.Y - y);
            }
        }

        private void OnFormMouseUp(object sender, MouseEventArgs e)
        {
            mIsMouseDown = false;
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (null != mTimer)
            {
                mTimer.Stop();
            }
        }

        private void okBtnClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }        

       

        private void exitPicBox_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            System.Diagnostics.Process.Start("iexplore.exe",
                      HttpAPI.FenceAlarmMap_URL + "?fence_name=" + HttpUtility.UrlEncode(this.labFenceName.Text)
                      + "&fence_points=" + fencePoints
                      + "&user_name=" + HttpUtility.UrlEncode(this.labUserName.Text)
                      + "&notify_time_str=" + this.labNotifyTimeStr.Text
                      + "&stay_time_min=" + this.labStayTimeMin.Text
                      + "&end_latitude=" + endLatitude
                      + "&end_longitude=" + endLongitude
                      + "&alarm_type_name=" + HttpUtility.UrlEncode(this.labAlarmTypeName.Text)

                      );

            this.DialogResult = DialogResult.OK;
            this.Close();

        }
    }
}
