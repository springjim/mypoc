using agora.rtc;
using POCClientNetLibrary;
using POCControlCenter.Service;
using POCControlCenter.Service.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.Agora
{

    delegate void dumpHandler(string tag, int ret);
    public partial class JoinChannelVideoView : Form
    {    

      
        internal static dumpHandler dump_handler_ = null;

        //以下用于AVChatNew消息的参数
        private ChatClient mSocketClient;
        public string mChannelName { get; set; } = "";
        private string mFromUserId = "";
        private string mFromUserName = "";
        private string mToUserId = "";
        private string mToUserName = "";
        private string mDesc = "";
        private short mVideoType = 0; 

        private System.Timers.Timer mTimer = null;
        public SoundPlayer player;
        private int delayTimeMs = 30000;  //超时时间设置，单位ms
        public bool connectStatus = false;  //连接状态
        private string APPID_KEY = "";  //这个从接口中获取
        
        public IAgoraRtcEngine rtc_engine_ = null;

        public bool audioStatus;  //false 不采集声音     true 采集
        public bool videoStatus;  //false 不采集摄像头   true 采集

        private bool mIncomeCall=false;  //true 表示被邀请  false 主叫方  
        private Action action; 

        public JoinChannelVideoView()
        {
            InitializeComponent();
            // just for debug
            dump_handler_ = new dumpHandler(DumpStatus);           

            mTimer = new System.Timers.Timer(delayTimeMs);
            mTimer.Interval = delayTimeMs;
            mTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerEvent);
            mTimer.Start();

        }

        public void stopTimer(bool flag)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() =>
                {
                    if (mTimer != null)
                    {
                        if (flag)
                        {
                            //停止定时器
                            mTimer.Stop();

                        }
                        else
                        {
                            mTimer.Interval = 5000; //退出时,要用定时器关闭时,可以设短一点时间
                            mTimer.Start();
                        }

                    }
                }));
            }
          
            
        }


        public void statusUpdate(bool connectStatus)
        {
            this.BeginInvoke(new Action(() =>
            {

                if (connectStatus)
                {
                    btnAudio.Visible = true;
                    if (mVideoType==2)
                        btnCamera.Visible = true;
                    else
                        btnCamera.Visible = false;
                    //已连接对方
                    labelStatus.BackColor = Color.Green;
                    labelStatus.Text = "已接通";
                    //其它按钮状态....
                    audioStatus = true;
                    videoStatus = true;
                    this.btnCamera.Image = global::POCControlCenter.Properties.Resources.btn_camera_open1;
                    this.btnAudio.Image = global::POCControlCenter.Properties.Resources.btn_audio_open1;

                } else
                {
                    btnAudio.Visible = false;
                    btnCamera.Visible = false;
                    //labelStatus.BackColor = Color.Transparent;
                    labelStatus.Text = "等待对方接听中...";
                    audioStatus = true;
                    videoStatus = true;
                }

            }));
         }


        public JoinChannelVideoView(ChatClient socketClient,string channelName, string fromUserId, string fromUserName, string toUserId, 
        string toUserName, string desc, Action prevFormClose, bool incomeCall, short videoType) :this()
        {

            //加入
            this.mSocketClient = socketClient;
            this.mChannelName = channelName;            

            this.mDesc = desc;

            this.mIncomeCall = incomeCall;
            this.mVideoType = videoType;

            //考虑是主叫还是被叫,这里处理不一样的
            if (!incomeCall)
            {
                //主叫
                this.mFromUserId = fromUserId ;
                this.mFromUserName = fromUserName;
                this.mToUserId = toUserId;
                this.mToUserName = toUserName;

            } else
            {
                //被叫
                this.mFromUserId = toUserId;
                this.mFromUserName = toUserName;
                this.mToUserId = fromUserId;
                this.mToUserName = fromUserName;
            }
            

            prevFormClose();

        }

        

        /// <summary>
        /// 由socket消息发过来的
        /// video_command; //视频命令，
        /// 1: 请求  2: 应答 3: 拒绝 4: 挂断 (含已连接时挂断, 未连接时挂断, 不管是主叫方还是被叫方都是这
        /// </summary>
        /// <param name="videoCommand"></param>
        public void MessageNotify(int videoCommand)
        {

            if (this.IsHandleCreated)
                this.BeginInvoke(new Action(() =>
                {
                    if (videoCommand == 3)
                    {
                        this.labelStatus.Text = "对方拒绝接听";
                        this.labelStatus.BackColor = Color.Yellow;
                        stopTimer(false); //启动关闭

                    } else if (videoCommand == 5)
                    {
                        this.labelStatus.Text = "对方正通话中...";
                        this.labelStatus.BackColor = Color.Red;
                        stopTimer(false); //启动关闭

                    }
                }));
               
        }

        public void OnTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Abort;
                    mTimer.Stop();
                    this.Close();
                }));
            }
           
        }

        public void DumpStatus(string tag, int ret)
        {
            string tips = tag;
            if (ret != 0)
            {
                tips += " failed, ret =" + ret.ToString();
            }
            else
            {
                tips += " ok";
            }
            Console.WriteLine(tips);
             
        }

       

         

        private void JoinChannelVideoView_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnInit();

            if (player != null )
            {
                player.Stop();
                player.Dispose();
            }

            try
            {
                LocalSharedData.setVideoCallRunningState(false);
            }
            finally
            {
                LocalSharedData.setVideoCallRunningState(false);
            }

        }

        

        private void JoinChannelVideoView_Load(object sender, EventArgs e)
        {
            LocalSharedData.setVideoCallRunningState(true);
            //
            labelToUserName.Text = mToUserName;
            connectStatus = false;
            statusUpdate(connectStatus);  //处理状态
            //1. 先调用agora接口获取appid,频道token
            AgoraRtcTokenResponse resp = PocClient.getAgoraRtcToken(mChannelName);
            if (resp.code == 0)
            {
                if (resp.data == null)
                    return;

                this.APPID_KEY = resp.data.appId;
                int ret = -1;
                ret= Init(APPID_KEY, mChannelName);
                //2. 加入频道
                if (ret==0 && null != rtc_engine_)
                {  
                   
                    ret = JoinChannel(mChannelName,resp.data.rtcToken, Convert.ToUInt32(mFromUserId));
                    if (ret == 0)
                    {
                        DumpStatus("joinChannel  result:", ret);
                        //加入成功
                        audioStatus = true;
                        videoStatus = true;
                       
                        //3. 是主叫方,  发送socket消息，初始化按钮和状态
                        if (mSocketClient != null && !mIncomeCall )
                        {
                            if (player == null)
                            {
                                player = new SoundPlayer();
                                player.SoundLocation = Path.Combine(System.Environment.CurrentDirectory, "telephone-ring.wav");
                                player.Load();
                                player.Play();
                                player.PlayLooping();
                            }

                            if (mVideoType == 2)
                            {
                                mSocketClient.SendMessage(  (new Data()).AVChatNewCommandMessageEncode
                                                                         ((short)2, (short)1, LocalSharedData.CURRENTUser.user_id,
                                                                          Convert.ToInt32(mToUserId), LocalSharedData.CURRENTUser.user_name, mToUserName,
                                                                          "desc"));
                            } else
                            {
                                mSocketClient.SendMessage((new Data()).AVChatNewCommandMessageEncode
                                                                        ((short)1, (short)1, LocalSharedData.CURRENTUser.user_id,
                                                                         Convert.ToInt32(mToUserId), LocalSharedData.CURRENTUser.user_name, mToUserName,
                                                                         "desc"));
                            }

                            

                            labelStatus.Text = "等待对方接听中...";
                        }                       

                       
                    }

                }
                

            }         
           
           

        }

        private void JoinChannelVideoView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (null != mTimer)
            {
                mTimer.Stop();
            }
        }



        ///////////////////////////////////
        private readonly string JoinChannelVideo_TAG = "[JoinChannelVideo] ";
        private AgoraRtcEngineEventHandler event_handler_ = null;
        

        private  string GetSDKVersion()
        {
            if (null == rtc_engine_)
                return "-" + (ERROR_CODE_TYPE.ERR_NOT_INITIALIZED).ToString();

            return rtc_engine_.GetVersion();
        }

        private  int JoinChannel(string channelName, string rtcToken, uint uid)
        {
            int ret = -1;
            if (null != rtc_engine_)
            {

                ret = rtc_engine_.JoinChannel(rtcToken,
                    channelName, "", uid);

                JoinChannelVideoView.dump_handler_(JoinChannelVideo_TAG + "JoinChannel token ", ret);

            }
            return ret;
        }

        private int LeaveChannel()
        {
            int ret = -1;
            if (null != rtc_engine_)
            {
                rtc_engine_.StopPreview();
                ret = rtc_engine_.LeaveChannel();
                JoinChannelVideoView.dump_handler_(JoinChannelVideo_TAG + "LeaveChannel", ret);
            }
            rtc_engine_.Dispose();
            rtc_engine_ = null;
            JoinChannelVideoView.dump_handler_(JoinChannelVideo_TAG + "Dispose", ret);
            return ret;

        }

        private int UnInit()
        {
            int ret = -1;
            if (null != rtc_engine_)
            {
                ret = rtc_engine_.LeaveChannel();
                JoinChannelVideoView.dump_handler_(JoinChannelVideo_TAG + "LeaveChannel", ret);

                rtc_engine_.Dispose();
                rtc_engine_ = null;
            }
            return ret;
        }

        private int Init(string appId, string channelId)
        {

            int ret = -1;
            

            if (null == rtc_engine_)
            {
                rtc_engine_ = AgoraRtcEngine.CreateAgoraRtcEngine();
            }
            event_handler_ = new AgoraRtcEngineEventHandler(this, rtc_engine_);
            rtc_engine_.InitEventHandler(event_handler_);

            RtcEngineContext rtc_engine_ctx = new RtcEngineContext(appId);
            ret = rtc_engine_.Initialize(rtc_engine_ctx);
            JoinChannelVideoView.dump_handler_(JoinChannelVideo_TAG + "Initialize", ret);
            if (ret == 0)
            {
                rtc_engine_.EnableAudio();
                if (mVideoType == 2)
                {
                    rtc_engine_.EnableVideo();
                    rtc_engine_.StartPreview();

                    VideoCanvas vs = new VideoCanvas((ulong)localVideoView.Handle, RENDER_MODE_TYPE.RENDER_MODE_FIT, channelId);
                    vs.uid = 0;
                    ret = rtc_engine_.SetupLocalVideo(vs);
                    Console.WriteLine("----->SetupLocalVideo ret={0}", ret);

                } else
                {
                    //语音通话
                    rtc_engine_.DisableVideo();
                    localVideoView.Visible = false;
                    btnCamera.Visible = false;
                }          
            

            }
            return ret;

        }


        /// <summary>
        /// ////////////////////////////////////////////////////////////////
        /// </summary>
        //内部类 IAgoraRtcChannelEventHandler 的实现
        internal class AgoraRtcEngineEventHandler : IAgoraRtcEngineEventHandler
        {
            private JoinChannelVideoView  inst_ = null;  //当前窗口实例
            private IAgoraRtcEngine rtc_engine_ = null;
            private VideoCanvas vc_remote;
            private VideoCanvas vc_local;

            public AgoraRtcEngineEventHandler(JoinChannelVideoView _joinChannelVideo,IAgoraRtcEngine rtc_engine)
            {
                inst_ = _joinChannelVideo;
                rtc_engine_ = rtc_engine;
            }

            public override void OnConnectionStateChanged(CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
            {
                base.OnConnectionStateChanged(state, reason);
            }

            public override void OnError(int err, string msg)
            {
                Console.WriteLine("=====>OnError {0} {1}", err, msg);
            }
            public override void OnWarning(int warn, string msg)
            {
                Console.WriteLine("=====>OnWarning {0} {1}", warn, msg);
            }
            /// <summary>
            /// 加入频道成功
            /// </summary>
            /// <param name="channel"></param>
            /// <param name="uid"></param>
            /// <param name="elapsed"></param>
            public override void OnJoinChannelSuccess(string channel, uint uid, int elapsed)
            {
                Console.WriteLine("----->OnJoinChannelSuccess channel={0} uid={1}", channel, uid);

                inst_.BeginInvoke(new Action(() =>
                {

                    if (inst_.mIncomeCall)
                    {
                        inst_.connectStatus = true;
                        inst_.stopTimer(true);
                        inst_.statusUpdate(inst_.connectStatus);
                    }

                }));

                  

            }

            /// <summary>
            /// 离开频道
            /// </summary>
            /// <param name="stats"></param>
            public override void OnLeaveChannel(RtcStats stats)
            {
                base.OnLeaveChannel(stats);
            }

            public override void OnLocalAudioStats(LocalAudioStats stats)
            {
                base.OnLocalAudioStats(stats);
            }

            public override void OnLocalVideoStats(LocalVideoStats stats)
            {
                base.OnLocalVideoStats(stats);
            }

            public override void OnRejoinChannelSuccess(string channel, uint uid, int elapsed)
            {
                base.OnRejoinChannelSuccess(channel, uid, elapsed);
            }

            public override void OnRemoteAudioStats(RemoteAudioStats stats)
            {
                base.OnRemoteAudioStats(stats);
            }

            public override void OnRemoteVideoStats(RemoteVideoStats stats)
            {
                base.OnRemoteVideoStats(stats);
            }

            /// <summary>
            /// 对方加入频道
            /// </summary>
            /// <param name="uid"></param>
            /// <param name="elapsed"></param>
            public override void OnUserJoined(uint uid, int elapsed)
            {
                inst_.BeginInvoke(new Action(() =>
                {
                    inst_.connectStatus = true;
                    inst_.stopTimer(true);
                    inst_.statusUpdate(inst_.connectStatus);
                    Console.WriteLine("----->OnUserJoined uid={0}", uid);

                    if (inst_.mVideoType == 2)
                    {
                        if (inst_.remoteVideoView.Handle == IntPtr.Zero) return;
                        vc_remote = new VideoCanvas((ulong)inst_.remoteVideoView.Handle,
                            RENDER_MODE_TYPE.RENDER_MODE_FIT, inst_.mChannelName, uid);
                        int ret = rtc_engine_.SetupRemoteVideo(vc_remote);
                        Console.WriteLine("----->SetupRemoteVideo, ret={0}", ret);
                    } else
                    {
                        //语音通话
                        inst_.remoteVideoView.BackgroundImage = Properties.Resources.VoiceTalkBg;

                    }

                  

                    if (!inst_.mIncomeCall)
                    {
                        if (inst_.player != null)
                        {
                            inst_.player.Stop();
                            inst_.player.Dispose();
                            inst_.player = null;
                        }
                    }                    

                }));                   

            }

            /// <summary>
            /// 对方离开了频道
            /// </summary>
            /// <param name="uid"></param>
            /// <param name="reason"></param>
            public override void OnUserOffline(uint uid, USER_OFFLINE_REASON_TYPE reason)
            {
                Console.WriteLine("----->OnUserOffline reason={0}", reason);
                onRemoteUserLeft(uid);
            }


            private void onRemoteUserLeft(uint uid)
            {
                if (vc_remote != null && vc_remote.uid == uid)
                {
                    
                    inst_.BeginInvoke(new Action(() =>
                    {
                        //vc_remote = new VideoCanvas(null,RENDER_MODE_TYPE.RENDER_MODE_FIT, inst_.mChannelName, uid);
                        //int ret = rtc_engine_.SetupRemoteVideo(vc_remote);
                        //Console.WriteLine("----->SetupRemoteVideo, ret={0}", ret);
                        //inst_.remoteVideoView.Visible = false;

                        inst_.remoteVideoView.Refresh();  //可以调用refresh, 不用模拟 videoCanvas

                        inst_.labelStatus.BackColor = Color.Yellow;

                        inst_.labelStatus.Text = "对方挂断";
                        inst_.stopTimer(false); //启动定时器进行自行关闭   
                        inst_.btnAudio.Visible = false;
                        inst_.btnCamera.Visible = false;

                    }));

                } else
                {
                    //音频通话的关闭
                    inst_.BeginInvoke(new Action(() =>
                    { 
                        inst_.labelStatus.BackColor = Color.Yellow;

                        inst_.labelStatus.Text = "对方挂断";
                        inst_.stopTimer(false); //启动定时器进行自行关闭   
                        inst_.btnAudio.Visible = false;
                        inst_.btnCamera.Visible = false;

                    }));

                }
            }

          


        }

        private void btnHangup_Click(object sender, EventArgs e)
        {
            //发送挂断的socket消息
            
            if (mSocketClient != null)
            {
                mSocketClient.SendMessage(
                                      (new Data()).AVChatNewCommandMessageEncode
                                      ((short)2, (short)4, LocalSharedData.CURRENTUser.user_id,
                                      Convert.ToInt32(mToUserId), LocalSharedData.CURRENTUser.user_name, mToUserName,
                                      "hangup"));
            }
            this.Close();
        }

        private void btnAudio_Click(object sender, EventArgs e)
        {
            //
            if (this.rtc_engine_ != null)
            {
                if (audioStatus)
                {
                    //停推声音
                    rtc_engine_.MuteLocalAudioStream(true);
                    this.btnAudio.Image = global::POCControlCenter.Properties.Resources.btn_audio_close1;
                    audioStatus = !audioStatus;
                } else
                {

                    //推流声音
                    rtc_engine_.MuteLocalAudioStream(false);
                    this.btnAudio.Image = global::POCControlCenter.Properties.Resources.btn_audio_open1;
                    audioStatus = !audioStatus;
                }

            }           

        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            if (this.rtc_engine_ != null)
            {
                if (videoStatus)
                {
                    //停推视频
                    rtc_engine_.MuteLocalVideoStream(true);
                    this.btnCamera.Image = global::POCControlCenter.Properties.Resources.btn_camera_close1;
                    videoStatus = !videoStatus;
                }
                else
                {

                    //推流视频
                    rtc_engine_.MuteLocalVideoStream(false);
                    this.btnCamera.Image = global::POCControlCenter.Properties.Resources.btn_camera_open1;
                    videoStatus = !videoStatus;
                }

            }
        }
    }
}
