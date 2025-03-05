using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace POCClientNetLibrary
{
    public partial class UserControlVideo : UserControl
    {
        /// <summary>
        /// vlc插件对象
        /// </summary>
        //public System.IntPtr vlc_instance { get; set; }

        /// <summary>
        /// 媒体播放器对象
        /// </summary>
        //public System.IntPtr vlc_media_player { get; set; }
        /// <summary>
        /// 关联的用户id
        /// </summary>
        public Int32 userid { get; set; }

        /// <summary>
        /// 重要,对应的CURRENT_VIDEOLIST中的序号
        /// </summary>
        public int index { get; set; }       

        /// <summary>
        /// streamurl, 如rtmp://ip:port/stream
        /// </summary>
        public string streamurl { get; set; }


        /// <summary>
        /// 用于停止直播或监控的调用,app
        /// </summary>
        public string app { get; set; }

        /// <summary>
        /// 用于停止直播或监控的调用,stream
        /// </summary>
        public string stream { get; set; }
        
        //开始时间
        public Int32 publishdate { get; set; }

        //视频类型: 直播或监控
        public string action_type { get; set; }

        /// <summary>
        /// 状态, 0 表示未用，1表示已有流播放中
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 是否最大化, 0表示未有, 1表示是最大化
        /// </summary>
        public int ismaxsize { get; set; }

        /// <summary>
        /// 音量, 0表示关闭音, 1表示最大音
        /// </summary>
        public int volume { get; set; }

        //定时器不容易同步
        /// <summary>
        /// 定时器用于显示直播或监控时间的显示
        /// </summary>
        //public System.Timers.Timer timer { get; set; }

        /// <summary>
        /// 播放尝试次数
        /// </summary>
        public Int32  trytime {get;set;}

        /// <summary>
        /// 获得视频的当前播放时间,单位: 秒
        /// </summary>
        public double curtime { get; set; }

        /// <summary>
        /// 用户姓名,区别于label控件
        /// </summary>
        public string username_attr { get; set; }

        //2017.11.09  新增了基于SIP VOIP 的视频通话
        /// <summary>
        /// 用于区分类型，初始下全部POC_VIDEO_TYPE.NORMAL
        /// </summary>
        public POC_VIDEO_TYPE poc_video_type { get; set; }

        /// <summary>
        /// SIP视频或语音通话时会用到,表示SDK返回的connectionID
        /// </summary>
        public int SIP_ConnectionId { get; set; }

        /// <summary>
        /// SIP视频或语音通话时会用到,表示SDK返回的lineID
        /// </summary>
        public int SIP_LineId { get; set; }

        /// <summary>
        /// SIP视频或语音通话时状态
        /// </summary>
        public bool SIP_bCalling { get; set; }
        /// <summary>
        /// SIP视频或语音通话时状态
        /// </summary>
        public bool SIP_bCallEstablished { get; set; }
        /// <summary>
        /// SIP视频或语音通话时状态
        /// </summary>
        public bool SIP_bCallHeld { get; set; }

        //以下用于视频通话或语音通话的计时
        public System.Windows.Forms.Timer SIP_callDurationTimer { get; set; }
        public TimeSpan SIP_callTime { get; set; }
        public string SIP_callTimeStr { get; set; }

        //2017.11.10 加一个视频通话的SCALE模式
        public PictureBoxSizeMode SIP_sizeMode { get; set; }

        public int VIDEO_HEIGHT = 144;
        public int VIDEO_WIDTH = 176;

        public UserControlVideo()
        {
            InitializeComponent();
           
        }

        int distance = 20;
        Point pointStart;
        bool bStart = false;

        private void UserControlVideo_Load(object sender, EventArgs e)
        {
            ToolTip p = new ToolTip();
            p.ShowAlways = true;
            p.SetToolTip(this.panelMenu, "Right click for popup menu,Double click for resize window");

            panelMenu.Tag = this;
            panelVideo.Tag = this;
            panelForDrag.Tag = this;

            btnFresh.Tag = this;
            btnSnap.Tag = this;
            //
            btnStop.Tag = this;
            btnSwitch.Tag = this;
            btnShare.Tag = this;

            username.Tag = this;
            time.Tag = this;
                    
            
        }

        private void btnVideoScale_Click(object sender, EventArgs e)
        {
            contextMenuStripVideoScale.Show(

        (sender as Button),

        (sender as Button).PointToClient(Cursor.Position),

        ToolStripDropDownDirection.Default);
        }
       

        private void toolStripMenuItemStretchImage_Click(object sender, EventArgs e)
        {
            //模向拉伸 SIP Video
            SIP_sizeMode = PictureBoxSizeMode.StretchImage;
            if (poc_video_type == POC_VIDEO_TYPE.DOUBLE_VIDEO ||
                poc_video_type == POC_VIDEO_TYPE.DOUBLE_VOICE)
            {
                doResize(this.SIP_sizeMode);
            }

        }

        private void toolStripMenuItemCenterImage_Click(object sender, EventArgs e)
        {
            //填满屏幕 SIP Video
            SIP_sizeMode = PictureBoxSizeMode.CenterImage;
            if (poc_video_type == POC_VIDEO_TYPE.DOUBLE_VIDEO ||
                poc_video_type == POC_VIDEO_TYPE.DOUBLE_VOICE)
            {
                doResize(this.SIP_sizeMode);
            }
        }

        private void toolStripMenuItemZoom_Click(object sender, EventArgs e)
        {
            //原始尺寸,保证原来的长宽比  320x240  SIP Video
            SIP_sizeMode = PictureBoxSizeMode.Zoom;
            if (poc_video_type == POC_VIDEO_TYPE.DOUBLE_VIDEO ||
                poc_video_type == POC_VIDEO_TYPE.DOUBLE_VOICE)
            {
                doResize(this.SIP_sizeMode);
            }
        }

        private void UserControlVideo_Resize(object sender, EventArgs e)
        {
            //最大化或缩小屏幕时,会影响SIP视频的remotewindow
            if (poc_video_type == POC_VIDEO_TYPE.DOUBLE_VIDEO ||
                poc_video_type==POC_VIDEO_TYPE.DOUBLE_VOICE )
            {
                doResize(this.SIP_sizeMode);                
            }             
            //MessageBox.Show("改变了大小");
        }

        public  void  doResize(PictureBoxSizeMode sizeMode)
        {
            //            

        }
        

        private void panelMenu_MouseDown(object sender, MouseEventArgs e)
        {
            //移到主界面中
            /*
            if (e.Button == MouseButtons.Left)
            {
                pointStart = new Point(e.X, e.Y);
                bStart = true;

            }
            */

        }

        private void panelMenu_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }             

       

        private void panelForDrag_MouseMove(object sender, MouseEventArgs e)
        {
            //移到主界面中
            /*
            if (!bStart)
            {
                return;
            }
            int dis = (int)Math.Sqrt(Math.Pow(e.X - pointStart.X, 2) + Math.Pow(e.Y - pointStart.Y, 2));
            if (dis < distance)
                return;
            
            DoDragDrop(sender, DragDropEffects.All);
            bStart = false;
            */

        }
        

        private void panelForDrag_MouseUp(object sender, MouseEventArgs e)
        {
            //移到主界面中
            //bStart = false;
        }

        
    }
}
