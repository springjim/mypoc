using agora.rtc;
using POCClientNetLibrary;
using POCControlCenter.Agora.Controls;
using POCControlCenter.DataEntity;
using POCControlCenter.Service;
using POCControlCenter.Service.Entity;
using POCControlCenter.Service.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.Agora.AVLive
{
    public partial class AVLiveCenterForm : Form
    {

        //常量
        public const int WM_SYSCOMMAND = 0x0112;

        //窗体移动
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        private ChatClient client;

        public string mAudienceChannelName { get; set; } = "";
        private string APPID_KEY = "";  //这个从接口中获取

        //agora live_boardcast        

        private IAgoraRtcChannelEventHandler channel_event_handler_ = null;

        private readonly string agora_sdk_log_file_path_ = "agorasdk.log";


        private IAgoraRtcEngine rtc_engine_ = null;
        private IAgoraRtcChannel channel_ = null;
        

        // 视频控件的最大多少路参数
        public static int VIDEO_USERCONTROL_NUM_MAX = 20;

        //
        private ConcurrentDictionary<Int32, LivePushItem> CURRENT_LIVELIST =
            new ConcurrentDictionary<Int32, LivePushItem>();

        private VideoLayoutMode CURRENT_VIDEOLayoutMode;
        private string picSnapPath = "";

        //锁对象可以是任意对象,但是被锁的代码需要保证是同一把锁,不能用匿名对象
        //Object obj = new Object();   //用于加入时用
        //Object objc = new Object();  //用于加入时用


        PictureBox selected;        

        private System.Drawing.Rectangle dragBox;
        Bitmap feedbackBmp;

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern bool SendMessage(
            IntPtr hWnd, // handle to destination window
            int Msg, // message
            int wParam, // first message parameter
            int lParam //  second message parameter
        );

        public AVLiveCenterForm(ChatClient client)
        {
            InitializeComponent();

            this.client = client;
            mAudienceChannelName = LocalSharedData.CURRENTUser.user_id + ""; //当前登录帐号
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_3_2;
            this.picSnapPath = ControlMainForm.getSingleControlMainForm().FullDownloadPicPath;

        }


        /// <summary>
        /// 设置鼠标样式
        /// </summary>
        /// <param name="fileName">自定义的鼠标样式文件</param>
        /// <returns>鼠标样式</returns>
        private  Cursor SetCursor()
        {         

            //画图
            Bitmap bit = Properties.Resources.videocalling1;
            Bitmap myNewCursor = new Bitmap(bit.Width, bit.Height);
            Graphics g = Graphics.FromImage(myNewCursor);
            g.Clear(Color.FromArgb(0, 0, 0, 0));
            
            g.DrawImage(bit, bit.Width / 2 - 15, bit.Height / 2, bit.Width / 2, bit.Height / 2);
            

            Cursor cursor;
            //获取图片的句柄
            try
            {
                cursor = new Cursor(myNewCursor.GetHicon());
            }
            catch
            {
                cursor = new Cursor(Icon.FromHandle(myNewCursor.GetHicon()).Handle);
            }

            //释放资源
            g.Dispose();

            return cursor;
        }



        private int Init(string appId, string channelId)
        {
            int ret = -1;
            if (null == rtc_engine_)
            {
                rtc_engine_ = AgoraRtcEngine.CreateAgoraRtcEngine();
            }
            LogConfig log_config = new LogConfig(agora_sdk_log_file_path_);
            log_config.level = LOG_LEVEL.LOG_LEVEL_WARN;   //从warn级别开始记录

            RtcEngineContext rtc_engine_ctx = new RtcEngineContext(APPID_KEY, AREA_CODE.AREA_CODE_CN, log_config);
            ret = rtc_engine_.Initialize(rtc_engine_ctx);
            rtc_engine_.SetChannelProfile(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING);


            return ret;

        }

        private int UnInit()
        {
            int ret = -1;

            if (null != channel_)
            {
                //first_channel_.Unpublish();
                ret = channel_.LeaveChannel();
                channel_.Dispose();
                //CSharpForm.dump_handler_(JoinMultipleChannel_TAG + "LeaveChannel", ret);
            }


            if (null != rtc_engine_)
            {
                rtc_engine_.Dispose(true);
                rtc_engine_ = null;
            }

            return ret;

        }

        public void  clearLivePushItem(int userId)
        {
            //lock (obj){
                int item_index = 0;
                LivePushItem pushItem = null;
                for (; item_index < CURRENT_LIVELIST.Count; item_index++)
                {
                    CURRENT_LIVELIST.TryGetValue(item_index, out pushItem);
                    if (pushItem != null)
                    {
                        if (pushItem.fromUserId == userId)
                        {
                            pushItem.fromUserId = 0;
                            pushItem.fromUserName = "";
                            pushItem.destChannelName = "";
                            pushItem.connectStatus = AVLiveStatusEnum.Init;
                            //

                            pushItem.remoteAudioStatus = true;
                            pushItem.remoteVideoStatus = true;
                            pushItem.isLiveBoardType = true;
                            break;
                        }

                    }

                }
            //}            
                
         }

        public void JoinChannel(LivePushDto dto)
        {
            //先看是不是已在了    
            int item_index = 0;
            LivePushItem pushItem = null;
            for (; item_index < CURRENT_LIVELIST.Count; item_index++)
            {

                CURRENT_LIVELIST.TryGetValue(item_index, out pushItem);
                if (pushItem.fromUserId == dto.userId)
                {
                    return;
                }
            }
            //先查找有无余下的LiveUserControl
            int ret = -1;
            item_index = 0;
            pushItem = null;
            LiveUserControl liveControl = null;
            for (; item_index < CURRENT_LIVELIST.Count; item_index++)
            {

                CURRENT_LIVELIST.TryGetValue(item_index, out pushItem);
                if (pushItem.fromUserId == 0)
                {
                    liveControl = pushItem.control;
                    //设置值
                    pushItem.cellNo = item_index;
                    pushItem.fromUserId = dto.userId;
                    pushItem.fromUserName = dto.userName;
                    pushItem.destChannelName = dto.channelName;

                    break;
                }
            }


            if (null != rtc_engine_)
            {
                ret = rtc_engine_.EnableAudio();
                ret = rtc_engine_.EnableVideo();
            }

            if (pushItem==null || liveControl == null)
            {
                //没有余下的
                return;
            }

            AgoraRtcTokenResponse resp = PocClient.getAgoraRtcToken(dto.channelName);
            if (resp.code == 0)
            {
                //初始化各个频道
                channel_ = rtc_engine_.CreateChannel(dto.channelName);
                ret = channel_.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
                channel_event_handler_ = new JoinMultipleChannelChannelEventHandler(this, rtc_engine_, dto, liveControl, pushItem);
                channel_.InitEventHandler(channel_event_handler_);

                ret = channel_.JoinChannel(resp.data.rtcToken, "",
                    Convert.ToUInt32(LocalSharedData.CURRENTUser.user_id)
                    , new ChannelMediaOptions(true, true, false, false));

                if (ret == 0)
                {
                    pushItem.channel = channel_;
                    pushItem.channel_event_handler = channel_event_handler_;
                }

            }


        }

        internal class JoinMultipleChannelChannelEventHandler : IAgoraRtcChannelEventHandler
        {
            private AVLiveCenterForm inst_ = null;  //当前窗口实例
            private IAgoraRtcEngine rtc_engine_ = null;
            private VideoCanvas vc_remote;
             
            private LiveUserControl liveControl_ = null;
            private LivePushDto dto_ = null;
            private LivePushItem pushItem_ = null;
            private uint remoteUid;

            public JoinMultipleChannelChannelEventHandler(AVLiveCenterForm _joinMultipleChannelChannel, IAgoraRtcEngine _rtc_engine,
                LivePushDto _dto, LiveUserControl _liveControl, LivePushItem _pushItem)
            {
                inst_ = _joinMultipleChannelChannel;
                rtc_engine_ = _rtc_engine;
                 
                liveControl_ = _liveControl;
                dto_ = _dto;
                pushItem_ = _pushItem;
            }

            public override void OnChannelError(string channelId, int err, string msg)
            {
                MessageBox.Show(String.Format("=====>OnChannelError {0} {1} {2}", channelId, err, msg));
                Console.WriteLine("=====>OnChannelError {0} {1} {2}", channelId, err, msg);
            }

            public override void OnChannelWarning(string channelId, int warn, string msg)
            {
                Console.WriteLine("=====>OnChannelError {0} {1} {2}", channelId, warn, msg);
            }

            public override void OnAudioPublishStateChanged(string channelId, STREAM_PUBLISH_STATE oldState, STREAM_PUBLISH_STATE newState, int elapseSinceLastState)
            {
                Console.WriteLine("----->OnAudioPublishStateChanged, channelId={0}", channelId);
            }

            public override void OnAudioSubscribeStateChanged(string channelId, uint uid, STREAM_SUBSCRIBE_STATE oldState, STREAM_SUBSCRIBE_STATE newState, int elapseSinceLastState)
            {
                Console.WriteLine("----->OnAudioSubscribeStateChanged, channelId={0}", channelId);
            }

            public override void OnClientRoleChanged(string channelId, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole)
            {
                Console.WriteLine("----->OnClientRoleChanged, channelId={0}", channelId);
            }

            public override void OnJoinChannelSuccess(string channelId, uint uid, int elapsed)
            {
                Console.WriteLine("----->OnJoinChannelSuccess, channelId={0} uid={1}", channelId, uid);
            }

            public override void OnLeaveChannel(string channelId, RtcStats stats)
            {
                Console.WriteLine("----->OnLeaveChannel, channelId={0}", channelId);
            }

            public override void OnRejoinChannelSuccess(string channelId, uint uid, int elapsed)
            {
                Console.WriteLine("----->OnRejoinChannelSuccess, channelId={0}", channelId);
            }

            public override void OnRemoteAudioStateChanged(string channelId, uint uid, REMOTE_AUDIO_STATE state, REMOTE_AUDIO_STATE_REASON reason, int elapsed)
            {
                Console.WriteLine("----->OnRemoteAudioStateChanged, channelId={0}", channelId);
            }

            public override void OnRemoteVideoStateChanged(string channelId, uint uid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
            {
                Console.WriteLine("----->OnRemoteVideoStateChanged, channelId={0} state={1} reason={2}", channelId, state, reason);
            }


            /// <summary>
            /// 对方加入频道
            /// </summary>
            /// <param name="channelId"></param>
            /// <param name="uid"></param>
            /// <param name="elapsed"></param>
            public override void OnUserJoined(string channelId, uint uid, int elapsed)
            {
                //lock (inst_.objc) {
                    inst_.BeginInvoke(new Action(() =>
                    {

                        Console.WriteLine("----->OnUserJoined uid={0}", uid);
                        if (liveControl_.remoteVideoView.Handle == IntPtr.Zero) return;
                        vc_remote = new VideoCanvas((ulong)liveControl_.remoteVideoView.Handle,
                            RENDER_MODE_TYPE.RENDER_MODE_FIT, dto_.channelName, uid);
                        int ret = rtc_engine_.SetupRemoteVideo(vc_remote);
                        Console.WriteLine("----->SetupRemoteVideo, ret={0}", ret);
                        //
                        if (dto_.channelName.EndsWith("-live"))
                            liveControl_.username.Text = dto_.userName;
                        else
                            liveControl_.username.Text = dto_.userName + "[监控]";
                        //计时
                        pushItem_.liveStartTime = DateTime.Now;
                        pushItem_.timer = new Timer();
                        pushItem_.timer.Interval = 500;
                        pushItem_.timer.Tick += Timer_Tick;
                        pushItem_.timer.Enabled = true;


                        liveControl_.username.Visible = true;
                        liveControl_.time.Visible = true;
                        liveControl_.btnAudio.Visible = true;
                        liveControl_.btnVideo.Visible = true;
                        liveControl_.btnStop.Visible = true;
                        liveControl_.btnCameraSwitch.Visible = true;
                        liveControl_.btnSnap.Visible = true;

                        if (dto_.channelName.EndsWith("-live"))
                            liveControl_.panelMenu.BackColor = Color.DarkGreen;
                        else
                            liveControl_.panelMenu.BackColor = Color.OrangeRed;

                        remoteUid = uid;

                        //按钮事件
                       
                        liveControl_.btnAudio.Click += BtnAudio_Click;                        
                        liveControl_.btnVideo.Click += BtnVideo_Click; 
                        liveControl_.btnStop.Click += BtnStop_Click;                        
                        liveControl_.btnCameraSwitch.Click += BtnCameraSwitch_Click;                        
                        liveControl_.remoteVideoView.MouseDoubleClick += RemoteVideoView_MouseDoubleClick;
                        liveControl_.btnSnap.Click += BtnSnap_Click;


                    }));

               // }              


            }



            private void BtnSnap_Click(object sender, EventArgs e)
            {
                if (inst_!= null)
                {
                    string subPath = Convert.ToString(remoteUid);
                    string directoryPath = Path.Combine(inst_.picSnapPath, subPath);
                    Directory.CreateDirectory(directoryPath);
                    string dateString = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
                    string filePath = Path.Combine(directoryPath, dateString+".jpg");                   
                    int ret=rtc_engine_.TakeSnapshot(dto_.channelName, remoteUid, filePath);

                }
            }

            private  string String2Unicode(string source)
            {
                var bytes = Encoding.Unicode.GetBytes(source);
                var stringBuilder = new StringBuilder();
                for (var i = 0; i < bytes.Length; i += 2)
                {
                    stringBuilder.AppendFormat("\\u{0:x2}{1:x2}", bytes[i + 1], bytes[i]);
                }
                return stringBuilder.ToString();
            }


            private void BtnCameraSwitch_Click(object sender, EventArgs e)
            {
                //
                //发送远程,要求停止直播的socket消息

                if (inst_.client != null)
                {
                    //video_type=3 表示直播
                    inst_.client.SendMessage(
                                          (new Data()).AVChatNewCommandMessageEncode
                                          ((short)3, (short)6, LocalSharedData.CURRENTUser.user_id,
                                          Convert.ToInt32(remoteUid), LocalSharedData.CURRENTUser.user_name, dto_.userName,
                                          "live_hangup"));
                }

            }

            private void RemoteVideoView_MouseDoubleClick(object sender, MouseEventArgs e)
            {
                //
                LivePushItem video = null;
                if (sender is PictureBox)
                    video = (LivePushItem)(((PictureBox)sender).Tag);
               
                if (video != null)
                {
                    if (video.controlMaxState == 1)
                        inst_.LoadVideoControl();
                    else
                    {
                        video.controlMaxState = 1;
                        inst_.LoadVideoControl_Max(video.control);
                    }
                      
                }

            }

            private void BtnStop_Click(object sender, EventArgs e)
            {
                //发送远程,要求停止直播的socket消息

                if (inst_.client != null)
                {
                    //video_type=3 表示直播
                    inst_.client.SendMessage(
                                          (new Data()).AVChatNewCommandMessageEncode
                                          ((short)3, (short)4, LocalSharedData.CURRENTUser.user_id,
                                          Convert.ToInt32(remoteUid), LocalSharedData.CURRENTUser.user_name, dto_.userName,
                                          "live_hangup"));
                }
            }

            private void BtnVideo_Click(object sender, EventArgs e)
            {
                //
                inst_.BeginInvoke(new Action(() =>
                {
                    if (pushItem_.remoteVideoStatus)
                    {
                        //关闭
                        pushItem_.channel.MuteRemoteVideoStream(remoteUid, true);
                        liveControl_.btnVideo.Image = Properties.Resources.btn_camera_close1;
                        liveControl_.remoteVideoView.Refresh();
                        Console.WriteLine("----->BtnVideo_Click, ret={0}", pushItem_.remoteVideoStatus);
                    }
                    else
                    {
                        //打开
                        pushItem_.channel.MuteRemoteVideoStream(remoteUid, false);
                        liveControl_.btnVideo.Image = Properties.Resources.btn_camera_open1;
                        Console.WriteLine("----->BtnVideo_Click, ret={0}", pushItem_.remoteVideoStatus);
                    }
                    pushItem_.remoteVideoStatus = !pushItem_.remoteVideoStatus;
                }));

            }

            private void BtnAudio_Click(object sender, EventArgs e)
            {
                //
                inst_.BeginInvoke(new Action(() =>
                {
                    if (pushItem_.remoteAudioStatus)
                    {
                        //关闭
                        pushItem_.channel.MuteRemoteAudioStream(remoteUid, true);
                        liveControl_.btnAudio.Image = Properties.Resources.btn_audio_close1;
                        Console.WriteLine("----->BtnAudio_Click, ret={0}", pushItem_.remoteAudioStatus);
                    }
                    else
                    {
                        //打开
                        pushItem_.channel.MuteRemoteAudioStream(remoteUid, false);
                        liveControl_.btnAudio.Image = Properties.Resources.btn_audio_open1;
                        Console.WriteLine("----->BtnAudio_Click, ret={0}", pushItem_.remoteAudioStatus);
                    }
                    pushItem_.remoteAudioStatus = !pushItem_.remoteAudioStatus;
                }));
               

            }

            private void Timer_Tick(object sender, EventArgs e)
            {
                //
                liveControl_.time.Text = DateTime.Now.Subtract(pushItem_.liveStartTime).ToString(@"hh\:mm\:ss");
            }

            /// <summary>
            /// 对方离开了频道
            /// </summary>
            /// <param name="channelId"></param>
            /// <param name="uid"></param>
            /// <param name="reason"></param>
            public override void OnUserOffline(string channelId, uint uid, USER_OFFLINE_REASON_TYPE reason)
            {
                inst_.BeginInvoke(new Action(() =>
                {

                    Console.WriteLine("----->OnUserOffline, channelId={0}", channelId);

                    //以下一定要减掉，因为每次onUserJoin时，事件会重复加上的，则会调用多次
                    liveControl_.btnAudio.Click -= BtnAudio_Click;
                    liveControl_.btnVideo.Click -= BtnVideo_Click;
                    liveControl_.btnStop.Click -= BtnStop_Click;
                    liveControl_.btnCameraSwitch.Click -= BtnCameraSwitch_Click;
                    liveControl_.remoteVideoView.MouseDoubleClick -= RemoteVideoView_MouseDoubleClick;
                    liveControl_.btnSnap.Click -= BtnSnap_Click;

                    liveControl_.remoteVideoView.Refresh(); //无流时,自动显示背景

                    //inst_.clearLivePushItem(dto_.userId);
                    inst_.clearLivePushItem(Convert.ToInt32(uid));

                    liveControl_.username.Visible = false;
                    liveControl_.time.Visible = false;

                    liveControl_.btnAudio.Visible = false;
                    liveControl_.btnVideo.Visible = false;
                    liveControl_.btnStop.Visible = false;
                    liveControl_.btnCameraSwitch.Visible = false;
                    liveControl_.btnSnap.Visible = false;

                    liveControl_.panelMenu.BackColor = Color.Silver;
                    //停止计时
                    if (pushItem_.timer != null)
                    {
                        pushItem_.timer.Stop();
                    }
                }));
               
            }

            public override void OnVideoPublishStateChanged(string channelId, STREAM_PUBLISH_STATE oldState, STREAM_PUBLISH_STATE newState, int elapseSinceLastState)
            {
                Console.WriteLine("----->OnVideoPublishStateChanged, channelId={0}", channelId);
            }

            public override void OnVideoSubscribeStateChanged(string channelId, uint uid, STREAM_SUBSCRIBE_STATE oldState, STREAM_SUBSCRIBE_STATE newState, int elapseSinceLastState)
            {
                Console.WriteLine("----->OnVideoSubscribeStateChanged, channelId={0}", channelId);
            }
        }


        private void btnMinX_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMaxNorm_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                btnMaxNorm.BackgroundImage = Properties.Resources.mainwnd_restore;

            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                btnMaxNorm.BackgroundImage = Properties.Resources.mainwnd_fullscreen;
            }
        }

        private void btnCloseX_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AVLiveCenterForm_Load(object sender, EventArgs e)
        {

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            //
            //1. 先调用agora接口获取appid,频道token
            AgoraRtcTokenResponse resp = PocClient.getAgoraRtcToken(mAudienceChannelName);
            if (resp.code == 0)
            {
                if (resp.data == null)
                    return;

                this.APPID_KEY = resp.data.appId;
                int ret = -1;
                ret = Init(APPID_KEY, mAudienceChannelName);
                //2. 加入频道
                if (ret == 0 && null != rtc_engine_)
                {

                }

            }
            //初始化
            for (int i = 0; i < VIDEO_USERCONTROL_NUM_MAX; i++)
            {
                LivePushItem pushItem = new LivePushItem();
                pushItem.cellNo = i; //记住序号
                LiveUserControl video_control = new LiveUserControl();                              

                 
                video_control.remoteVideoView.AllowDrop = true;
                //允许拖拽操作
                video_control.AllowDrop = true;

                //拖放发起方
                video_control.remoteVideoView.MouseDown += RemoteVideoView_MouseDown;
                video_control.remoteVideoView.MouseMove += RemoteVideoView_MouseMove1;
                video_control.remoteVideoView.MouseUp += RemoteVideoView_MouseUp;
                video_control.remoteVideoView.GiveFeedback += RemoteVideoView_GiveFeedback;


                video_control.remoteVideoView.DragDrop += RemoteVideoView_DragDrop;

                video_control.remoteVideoView.DragEnter += RemoteVideoView_DragEnter;
                video_control.remoteVideoView.DragOver += RemoteVideoView_DragOver;

                //video_control.remoteVideoView.MouseClick += RemoteVideoView_MouseClick;

                //video_control.remoteVideoView.Paint += RemoteVideoView_Paint;
                
                video_control.remoteVideoView.Tag = pushItem;  //反向绑定, tag关联
                pushItem.control = video_control;

                CURRENT_LIVELIST.TryAdd(i, pushItem);
            }

            tableLayoutPanelVideo.GetType().GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tableLayoutPanelVideo, true, null);

            LoadVideoControl();

        }

        private void RemoteVideoView_DragOver(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.None) == DragDropEffects.None
                && (e.KeyState & (8 + 32)) == (8 + 32))
            {
                //CTRL+ALT
                e.Effect = DragDropEffects.None;
               
            }
            else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link
                && (e.KeyState & (32)) == (32))
            {
                //ALT
                e.Effect = DragDropEffects.Link;
               
            }
            else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy
                && (e.KeyState & (8)) == (8))
            {
                //CTRL
                e.Effect = DragDropEffects.Copy;
                
            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move
                && (e.KeyState & (4)) == (4))
            {
                //SHIFT
                e.Effect = DragDropEffects.Move;
                
            }
            else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                //无
                e.Effect = DragDropEffects.Copy;
               
            }
            else
            {
                e.Effect = DragDropEffects.None;
               
            }

        }

        private void RemoteVideoView_MouseUp(object sender, MouseEventArgs e)
        {
            dragBox = System.Drawing.Rectangle.Empty;
        }

        private void RemoteVideoView_MouseMove1(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (dragBox != System.Drawing.Rectangle.Empty &&
                    !dragBox.Contains(e.X, e.Y))
                {
                    PictureBox self = (PictureBox)sender;
                    var effect = self.DoDragDrop(self, DragDropEffects.All | DragDropEffects.Link);
                    if (effect == DragDropEffects.Move)
                        self.Image = null;
                }
            }
        }

        private void RemoteVideoView_MouseDown(object sender, MouseEventArgs e)
        {
            dragBox = new System.Drawing.Rectangle(new Point(e.X - (SystemInformation.DragSize.Width / 2),
              e.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize);
        }

        Image ImgDeal(Image img, byte alpha, int width, int height)
        {
            var bmpSize = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmpSize))
            {
                g.DrawImage(img, new System.Drawing.Rectangle(0, 0, width, height));
            }
            var bmpAlpha = new Bitmap(width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var color = bmpSize.GetPixel(i, j);
                    bmpAlpha.SetPixel(i, j, Color.FromArgb(alpha, color));
                }
            }
            return (Image)bmpAlpha.Clone();
        }


        /// <summary> 
        /// 辅助类 定义Gdi32 API函数 
        /// </summary> 
        public class GDI32
        {
            public const int SRCCOPY = 0x00CC0020;
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
                            int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary> 
        /// 辅助类 定义User32 API函数 
        /// </summary> 
        public class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }


        public static Image ScreenshotControlIntPtr(IntPtr handle)
        {           

            IntPtr hdcSrc = User32.GetWindowDC(handle);
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            GDI32.SelectObject(hdcDest, hOld);
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            Image img = Image.FromHbitmap(hBitmap);
            GDI32.DeleteObject(hBitmap);
            return img;

        }




        private void RemoteVideoView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            PictureBox self = (PictureBox)sender;            
            feedbackBmp = new Bitmap(ScreenshotControlIntPtr(self.Handle));            

            int width = self.Width;
            int height = self.Height;

            if (CURRENT_VIDEOLayoutMode == VideoLayoutMode.LAYOUT_L_6)
            {
                width /= 2;
                height /= 2;
            }


            if (e.Effect == DragDropEffects.None)
            {
                var img = ImgDeal(feedbackBmp, 70, width, height);
                Cursor.Current = new Cursor(((Bitmap)img).GetHicon());
            }
            else
            {
                var img = ImgDeal(feedbackBmp, 255, width, height);
                Cursor.Current = new Cursor(((Bitmap)img).GetHicon());
            }
        }

        private void RemoteVideoView_Paint(object sender, PaintEventArgs e)
        {
            var pb = (PictureBox)sender;
            pb.BackColor = Color.Silver;
            if (selected == pb)
            {
                ControlPaint.DrawBorder(e.Graphics, pb.ClientRectangle,
                   Color.Blue, 5, ButtonBorderStyle.Solid,  // Left
                   Color.Blue, 5, ButtonBorderStyle.Solid,  // Top
                   Color.Blue, 5, ButtonBorderStyle.Solid,  // Right
                   Color.Blue, 5, ButtonBorderStyle.Solid); // Bottom
            }
        }

        private void RemoteVideoView_MouseClick(object sender, MouseEventArgs e)
        {
           // SelectBox((PictureBox)sender);
        }

        private void RemoteVideoView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(PictureBox)))
            {
                e.Effect = DragDropEffects.None;

            }else
            {
                
            }
            Console.WriteLine("RemoteVideoView_DragEnter:" + e.Effect.ToString());

        }

        /// <summary>
        /// Fires after dragging has completed on the target control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoteVideoView_DragDrop(object sender, DragEventArgs e)
        {           

            var target = (PictureBox)sender;
            
            if (e.Data.GetDataPresent(typeof(PictureBox)))
            {
                var source = (PictureBox)e.Data.GetData(typeof(PictureBox));
                if (source != target)
                {
                    //
                    LivePushItem sourceLiveItem = (LivePushItem)source.Tag;
                    LivePushItem targetLiveItem = (LivePushItem)target.Tag;
                    //在CURRENTLIVE_LIST中对调,然后
                    SwapLivePushItem(sourceLiveItem,targetLiveItem);
                    LoadVideoControl();

                    return;
                }
            }
            Console.WriteLine("Don't do DragDrop");
        }


        private void SwapLivePushItem(LivePushItem sourceLiveItem, LivePushItem targetLiveItem)
        {
            int item_index = 0;
            LivePushItem pushItem = null;
            int sourceIndex = -1;
            int targetIndex = -1;
            for (; item_index < CURRENT_LIVELIST.Count; item_index++)
            {
                CURRENT_LIVELIST.TryGetValue(item_index, out pushItem);
                if (pushItem != null && pushItem==sourceLiveItem)
                {
                    sourceIndex = item_index;

                } else if (pushItem != null && pushItem == targetLiveItem)
                {
                    targetIndex = item_index;

                }

            }
            //
            if (sourceIndex>=0 && targetIndex >= 0)
            {
                pushItem = sourceLiveItem;
                int maxState = sourceLiveItem.controlMaxState;
                int maxStateTarget = targetLiveItem.controlMaxState;
                pushItem.controlMaxState = maxState;
                sourceLiveItem.controlMaxState = maxStateTarget;

                CURRENT_LIVELIST.TryUpdate(sourceIndex, targetLiveItem, sourceLiveItem );
                CURRENT_LIVELIST.TryUpdate(targetIndex, pushItem, targetLiveItem);
                Console.WriteLine("SwapLivePushItem OK");
            }

        }


        /// <summary>
        /// Set the selected image, and trigger repaint on all boxes.
        /// </summary>
        /// <param name="pb"></param>
        private void SelectBox(PictureBox pb)
        {
            if (selected != pb)
            {
                selected = pb;
            }
            else
            {
                selected = null;
            }

            // Cause each box to repaint
            foreach (var box in CURRENT_LIVELIST) box.Value.control.remoteVideoView.Invalidate();

        }
                            

       

        private void LoadVideoControl_Max(LiveUserControl userVideo)
        {
            //
            int colNum = 1, rowNum = 1;

            int iWidth = (int)(tableLayoutPanelVideo.Width / colNum);
            int iHeight = (int)(tableLayoutPanelVideo.Height / rowNum);
            tableLayoutPanelVideo.Controls.Clear();
            tableLayoutPanelVideo.RowCount = rowNum;
            tableLayoutPanelVideo.ColumnCount = colNum;


            for (int i = 0; i < tableLayoutPanelVideo.ColumnStyles.Count; i++)
            {
                tableLayoutPanelVideo.ColumnStyles[i].SizeType = SizeType.Absolute;
                tableLayoutPanelVideo.ColumnStyles[i].Width = iWidth;
            }
            for (int i = 0; i < tableLayoutPanelVideo.RowStyles.Count; i++)
            {
                tableLayoutPanelVideo.RowStyles[i].SizeType = SizeType.Absolute;
                tableLayoutPanelVideo.RowStyles[i].Height = iHeight;
            }

            
            tableLayoutPanelVideo.Controls.Add(userVideo, 0, 0);
            tableLayoutPanelVideo.SetColumnSpan(userVideo, 1);
            tableLayoutPanelVideo.SetRowSpan(userVideo, 1);          
                                          

        }

        /// <summary>
        /// 显示: 正常模式下
        /// </summary>
        private void LoadVideoControl()
        {
            int colNum = 3, rowNum = 3;
            if (CURRENT_VIDEOLayoutMode == VideoLayoutMode.LAYOUT_2_2)
            {
                colNum = 2;
                rowNum = 2;
            }
            else if (CURRENT_VIDEOLayoutMode == VideoLayoutMode.LAYOUT_3_2)
            {
                colNum = 3;
                rowNum = 2;
            }
            else if (CURRENT_VIDEOLayoutMode == VideoLayoutMode.LAYOUT_3_3 || CURRENT_VIDEOLayoutMode == VideoLayoutMode.LAYOUT_L_6)
            {
                colNum = 3;
                rowNum = 3;
            }
            else if (CURRENT_VIDEOLayoutMode == VideoLayoutMode.LAYOUT_4_2)
            {
                colNum = 4;
                rowNum = 2;
            }
            else if (CURRENT_VIDEOLayoutMode == VideoLayoutMode.LAYOUT_4_3)
            {
                colNum = 4;
                rowNum = 3;
            }
            else if (CURRENT_VIDEOLayoutMode == VideoLayoutMode.LAYOUT_4_4)
            {
                colNum = 4;
                rowNum = 4;
            }
            else if (CURRENT_VIDEOLayoutMode == VideoLayoutMode.LAYOUT_5_4)
            {
                colNum = 5;
                rowNum = 4;
            }

            int iWidth = (int)(tableLayoutPanelVideo.Width / colNum);
            int iHeight = (int)(tableLayoutPanelVideo.Height / rowNum);
            tableLayoutPanelVideo.Controls.Clear();
            tableLayoutPanelVideo.RowCount = rowNum;
            tableLayoutPanelVideo.ColumnCount = colNum;

            for (int i = 0; i < tableLayoutPanelVideo.ColumnStyles.Count; i++)
            {
                tableLayoutPanelVideo.ColumnStyles[i].SizeType = SizeType.Absolute;
                tableLayoutPanelVideo.ColumnStyles[i].Width = iWidth;
            }
            for (int i = 0; i < tableLayoutPanelVideo.RowStyles.Count; i++)
            {
                tableLayoutPanelVideo.RowStyles[i].SizeType = SizeType.Absolute;
                tableLayoutPanelVideo.RowStyles[i].Height = iHeight;
            }

            int cache_index = 0;

            if (CURRENT_VIDEOLayoutMode != VideoLayoutMode.LAYOUT_L_6)
            {
                for (int i = 0; i < (rowNum); i++)
                {
                    for (int j = 0; j < colNum; j++)
                    {
                        cache_index = ((i * colNum) + j);
                        if (cache_index < CURRENT_LIVELIST.Count)
                        {
                            bool IsOk = false;
                            LivePushItem item = null;
                            LiveUserControl contrVideo = null;
                            IsOk = CURRENT_LIVELIST.TryGetValue(cache_index, out item);
                            if (IsOk)
                            {
                                item.controlMaxState = 0;

                                contrVideo = item.control;
                                contrVideo.Padding = contrVideo.Margin = new Padding(0); //让他具有捷思锐的黑色边框效果
                                contrVideo.Width = iWidth;
                                contrVideo.Height = iHeight;
                               
                                tableLayoutPanelVideo.SetColumnSpan(contrVideo, 1);
                                tableLayoutPanelVideo.SetRowSpan(contrVideo, 1);
                                contrVideo.Dock = DockStyle.Fill;
                                tableLayoutPanelVideo.Controls.Add(contrVideo, j % colNum, i % rowNum);

                                

                            }

                        }

                    }
                }
            }
            else
            {
                //L型排版
                for (int i = 0; i < (rowNum); i++)
                {
                    for (int j = 0; j < colNum; j++)
                    {
                        cache_index = ((i * colNum) + j);
                        if (cache_index < CURRENT_LIVELIST.Count)
                        {
                            bool IsOk = false;
                            LivePushItem item = null;
                            LiveUserControl contrVideo = null;
                            IsOk = CURRENT_LIVELIST.TryGetValue(cache_index, out item);
                            if (IsOk)
                            {                                
                                
                                contrVideo = item.control;
                                contrVideo.Padding = contrVideo.Margin = new Padding(0); //让他具有捷思锐的黑色边框效果
                                contrVideo.Width = iWidth;
                                contrVideo.Height = iHeight;

                                if (cache_index == 0)
                                {
                                    item.controlMaxState = 0;
                                    contrVideo.Width = iWidth * 2;
                                    contrVideo.Height = iHeight * 2;
                                    
                                    contrVideo.Dock = DockStyle.Fill;
                                    tableLayoutPanelVideo.Controls.Add(contrVideo, j % colNum, i % rowNum);
                                    tableLayoutPanelVideo.SetColumnSpan(contrVideo, 2);
                                    tableLayoutPanelVideo.SetRowSpan(contrVideo, 2);


                                }
                                else if (cache_index == 1)
                                {
                                    item.controlMaxState = 0;
                                    contrVideo.Dock = DockStyle.Fill;
                                    tableLayoutPanelVideo.Controls.Add(contrVideo, 2, 0);
                                    tableLayoutPanelVideo.SetColumnSpan(contrVideo, 1);
                                    tableLayoutPanelVideo.SetRowSpan(contrVideo, 1);

                                }
                                else if (cache_index == 2)
                                {
                                    item.controlMaxState = 0;
                                    contrVideo.Dock = DockStyle.Fill;
                                    tableLayoutPanelVideo.Controls.Add(contrVideo, 2, 1);
                                    tableLayoutPanelVideo.SetColumnSpan(contrVideo, 1);
                                    tableLayoutPanelVideo.SetRowSpan(contrVideo, 1);

                                }
                                else if (cache_index == 3)
                                {
                                    item.controlMaxState = 0;
                                    contrVideo.Dock = DockStyle.Fill;
                                    tableLayoutPanelVideo.Controls.Add(contrVideo, 0, 2);
                                    tableLayoutPanelVideo.SetColumnSpan(contrVideo, 1);
                                    tableLayoutPanelVideo.SetRowSpan(contrVideo, 1);

                                }
                                else if (cache_index == 4)
                                {
                                    item.controlMaxState = 0;
                                    contrVideo.Dock = DockStyle.Fill;
                                    tableLayoutPanelVideo.Controls.Add(contrVideo, 1, 2);
                                    tableLayoutPanelVideo.SetColumnSpan(contrVideo, 1);
                                    tableLayoutPanelVideo.SetRowSpan(contrVideo, 1);
                                }
                                else if (cache_index == 5)
                                {
                                    item.controlMaxState = 0;
                                    contrVideo.Dock = DockStyle.Fill;
                                    tableLayoutPanelVideo.Controls.Add(contrVideo, 2, 2);
                                    tableLayoutPanelVideo.SetColumnSpan(contrVideo, 1);
                                    tableLayoutPanelVideo.SetRowSpan(contrVideo, 1);

                                }
                                else
                                {
                                    continue;
                                }


                            }

                        }

                    }
                }
            }


        }



        private void AVLiveCenterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnInit();
        }

        private void btn_rec_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_2_2;
            LoadVideoControl();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_3_2;
            LoadVideoControl();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_L_6;
            LoadVideoControl();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_4_2;
            LoadVideoControl();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_3_3;
            LoadVideoControl();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_4_3;
            LoadVideoControl();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_4_4;
            LoadVideoControl();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_5_4;
            LoadVideoControl();
        }

        private void panelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            //
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
    }
}
