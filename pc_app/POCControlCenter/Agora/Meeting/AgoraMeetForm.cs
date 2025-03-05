using agora.rtc;
using POCClientNetLibrary;
using POCControlCenter.BroadCast;
using POCControlCenter.DataEntity;
using POCControlCenter.Service;
using POCControlCenter.Service.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.Agora.Meeting
{
    public partial class AgoraMeetForm : Form
    {
        //模仿窗口标题栏拖动
        private bool mIsMouseDown = false;
        private Point mFormLocation;     // Form的location
        private Point mMouseOffset;      // 鼠标的按下位置
                                         //
        internal static IEngine engine_ = null;
        internal static dumpHandler dump_handler_ = null;
        //以下用于 消息的参数
        private ChatClient client;
        private HashSet<String> userList;
       
        private HashSet<User> members = new HashSet<User>();  //包含的成员列表 (包含调度台本身)
        private String currStreamType = "streamBig";  //当前主流(摄像头) 还是辅流(屏幕分享) streamSub 

        public string mChannelName { get; set; } = "";

        /// <summary>
        /// 1: 语音会议  2: 视频会议
        /// </summary>
        private short mVideoType = 0;
        public bool connectStatus = false;  //连接状态
        private string APPID_KEY = "";  //这个从接口中获取

        public IAgoraRtcEngine rtc_engine_ = null;
        private IAgoraRtcEngine screen_share_engine_ = null;  //专用于屏幕分享

        private AgoraRtcEngineEventHandler event_handler_ = null;
        private string AgoraMeetForm_TAG = "AgoraMeetForm";
        private List<AgoraRemoteUserInfo> mRemoteUsers;        // 当前房间里的远端用户（除了本地用户）

        public bool audioStatus;  //false 不采集声音     true 采集
        public bool videoStatus;  //false 不采集摄像头   true 采集
        private bool mIsSetScreenSuccess;   // 是否设置屏幕参数成功
        private string mRtcToken;

        private delegate void EndScreenShareDelegate();
        //调用窗口
        private AgoraToastForm mToastForm;

        public AgoraMeetForm(ChatClient client, HashSet<String> userList,string mChannelName,short mVideoType)
        {
            InitializeComponent();
            // just for debug
            dump_handler_ = new dumpHandler(DumpStatus);
            this.client = client;
            this.userList = userList;
            this.mChannelName = mChannelName;
            this.mVideoType = mVideoType;
            CheckForIllegalCrossThreadCalls = false; //这样用不安全

        }

        internal void onEndScreenShare()
        {
            if (!this.IsHandleCreated) return;
            if (screenShareCheckBox.InvokeRequired == true)
            {
                EndScreenShareDelegate uld = new EndScreenShareDelegate(onEndScreenShare);
                this.Invoke(uld, new object[] { });
                return;
            }
            //
            screenShareCheckBox.Checked = false;
            // 关闭屏幕分享功能
            if (!mIsSetScreenSuccess) return;

            if (null != screen_share_engine_)
            {
                screen_share_engine_.StopScreenCapture();
                screen_share_engine_.LeaveChannel();

                int ret = rtc_engine_.JoinChannel(mRtcToken,
                    mChannelName, "camera", (uint)LocalSharedData.CURRENTUser.user_id);
            }
             
            if (mToastForm != null)
            {
                mToastForm.Dispose();
                mToastForm=null;
            }
              

            this.currStreamType = "streamBig";
          
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

        private void OnFormMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mIsMouseDown = true;
                mFormLocation = this.Location;
                mMouseOffset = Control.MousePosition;
            }
        }

        private void OnFormMouseUp(object sender, MouseEventArgs e)
        {
            mIsMouseDown = false;
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

        private int JoinChannel(string channelName, string rtcToken, uint uid)
        {
            int ret = -1;
            if (null != rtc_engine_)
            {

                ret = rtc_engine_.JoinChannel(rtcToken,
                    channelName, "", uid);

                AgoraMeetForm.dump_handler_(AgoraMeetForm_TAG + "JoinChannel token ", ret);

            }
            return ret;
        }

        private void AgoraMeetForm_Load(object sender, EventArgs e)
        {
            //
            LocalSharedData.setVideoCallRunningState(true);
            localUserLabel.Text = LocalSharedData.CURRENTUser.user_name;
            connectStatus = false;
            //1. 先调用agora接口获取appid,频道token
            AgoraRtcTokenResponse resp = PocClient.getAgoraRtcToken(mChannelName);
            if (resp.code == 0)
            {
                if (resp.data == null)
                    return;

                this.APPID_KEY = resp.data.appId;
                int ret = -1;
                ret = Init(APPID_KEY, mChannelName);
                int initRet = ret;
                //2. 加入频道
                if (initRet == 0 && null != rtc_engine_)
                {
                    mRtcToken = resp.data.rtcToken;
                    ret = JoinChannel(mChannelName, resp.data.rtcToken, Convert.ToUInt32(LocalSharedData.CURRENTUser.user_id));
                    if (ret == 0)
                    {
                        DumpStatus("joinChannel  result:", ret);
                        //加入成功
                        audioStatus = true;
                        videoStatus = true; 
                    }

                }

                //
                if (initRet == 0 && null != screen_share_engine_)
                {
                    //
                }

            }
        }

        internal void OnSetScreenParamsCallback(ScreenCaptureSourceInfo sourceinfo,bool success)
        {
            mIsSetScreenSuccess = success;
            if (success)
            {                
                this.Invoke(new Action(() =>
                {
                    if (null != screen_share_engine_)
                    {
                        ScreenCaptureParameters captureParams;

                        int ret = -1;
                        if (sourceinfo.type == ScreenCaptureSourceType.ScreenCaptureSourceType_Screen)
                        {
                            captureParams = new ScreenCaptureParameters
                            {
                                captureMouseCursor = true,
                                enableHighLight = true,
                                highLightWidth = 5,
                                highLightColor = 0xffff80ff,
                                frameRate = 15,
                                windowFocus = true,
                                dimensions = new VideoDimensions(1080, 720)

                            };
                            ret = screen_share_engine_.StartScreenCaptureByDisplayId((uint)sourceinfo.sourceId,
                                new agora.rtc.Rectangle(), captureParams);

                        }
                        else if (sourceinfo.type == ScreenCaptureSourceType.ScreenCaptureSourceType_Window)
                        {
                            captureParams = new ScreenCaptureParameters
                            {
                                captureMouseCursor = true,
                                enableHighLight = true,
                                highLightWidth=5,
                                highLightColor=0xffff80ff,
                                frameRate = 15,
                                windowFocus = true,
                                dimensions = new VideoDimensions(1080, 720)

                            };
                            ret = screen_share_engine_.StartScreenCaptureByWindowId(sourceinfo.sourceId,
                                new agora.rtc.Rectangle(), captureParams);

                        }

                        screen_share_engine_.SetScreenCaptureScenario(SCREEN_SCENARIO_TYPE.SCREEN_SCENARIO_VIDEO);
                        ret = screen_share_engine_.EnableVideo();

                        captureParams = new ScreenCaptureParameters
                        {
                            captureMouseCursor = true,
                            enableHighLight = true,
                            highLightWidth = 5,
                            highLightColor = 0xffff80ff,
                            frameRate = 15,
                            windowFocus = true,
                            dimensions = new VideoDimensions(1080, 720)

                        };
                        screen_share_engine_.UpdateScreenCaptureParameters(captureParams);

                        ChannelMediaOptions option = new ChannelMediaOptions();

                        //先退出原来摄像头的视频流
                        if (null != rtc_engine_)
                        {
                            ret = rtc_engine_.LeaveChannel();
                            AgoraMeetForm.dump_handler_(AgoraMeetForm_TAG + "LeaveChannel", ret);
                        }

                        //pc屏幕分享加入同频道，
                        ret = screen_share_engine_.JoinChannel(mRtcToken, mChannelName, "screen",
                        (uint)LocalSharedData.CURRENTUser.user_id);

                        if (!this.audioStatus)
                        {
                            rtc_engine_.MuteLocalAudioStream(true);
                            screen_share_engine_.MuteLocalAudioStream(true);
                        }
                        else
                        {
                            rtc_engine_.MuteLocalAudioStream(false);
                            screen_share_engine_.MuteLocalAudioStream(false);
                        }                           


                        if (this.loopBackCheckBox.Checked)
                            screen_share_engine_.EnableLoopbackRecording(true, "");
                        else
                            screen_share_engine_.EnableLoopbackRecording(false, "");

                        if (ret == 0)
                        {
                            if (mToastForm == null || mToastForm.IsDisposed )
                                mToastForm = new AgoraToastForm(this);
                            mToastForm.SetText("[" + LocalSharedData.CURRENTUser.user_name + "]:    正在屏幕共享");
                            mToastForm.Show();
                        }
                        

                    }


                }));
                
            }
            else
            {
                this.screenShareCheckBox.Checked = false;
                screen_share_engine_.LeaveChannel();
                rtc_engine_.JoinChannel(mRtcToken, mChannelName, "camera", (uint)LocalSharedData.CURRENTUser.user_id);
                
            }
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

            /////////////////屏幕分享rtc_engine
            if (null == screen_share_engine_)
            {
                screen_share_engine_ = AgoraRtcEngine.CreateAgoraRtcEngine(AgoraEngineType.SubProcess);
            }
            ret = screen_share_engine_.Initialize(rtc_engine_ctx); 
            screen_share_engine_.InitEventHandler(event_handler_);


            //启用用户音量提示  https://docportal.shengwang.cn/cn/video-legacy/API%20Reference/csharp/API/class_irtcengine.html#api_enableaudiovolumeindication
            int rret = rtc_engine_.EnableAudioVolumeIndication(300, 3, true);  //报告本地用户的语音
            Console.WriteLine("----->EnableAudioVolumeIndication ret={0}", rret);
            this.localVoiceProgressBar.Visible = true;

            AgoraMeetForm.dump_handler_( AgoraMeetForm_TAG + "Initialize", ret);
            if (ret == 0)
            {
                rtc_engine_.EnableAudio();               
                rtc_engine_.EnableVideo();
                rtc_engine_.StartPreview();
                this.localInfoLabel.Text = "视频已关闭";
                this.localInfoLabel.Visible = false;
                VideoCanvas vs = new VideoCanvas((ulong)localVideoPanel.Handle, RENDER_MODE_TYPE.RENDER_MODE_FIT, channelId);
                vs.uid = 0;
                ret = rtc_engine_.SetupLocalVideo(vs);
                Console.WriteLine("----->SetupLocalVideo ret={0}", ret);                           


            }
            return ret;

        }

        private int UnInit()
        {
            int ret = -1;
            if (null != rtc_engine_)
            {
                ret = rtc_engine_.LeaveChannel();
                AgoraMeetForm.dump_handler_(AgoraMeetForm_TAG + "LeaveChannel", ret);

                rtc_engine_.Dispose();
                rtc_engine_ = null;
            }

            if (null != screen_share_engine_)
            {
                ret = screen_share_engine_.LeaveChannel();
                AgoraMeetForm.dump_handler_(AgoraMeetForm_TAG + "LeaveChannel", ret);

                screen_share_engine_.Dispose();
                screen_share_engine_ = null;
            }

            return ret;
        }

        class AgoraRemoteUserInfo
        {
            public string userId { get; set; }

            public int position { get; set; }
        }

        /// <summary>
        /// 获取空闲窗口的位置
        /// </summary>
        private int GetIdleRemoteVideoPosition(String userId)
        {
            if (string.IsNullOrEmpty(this.remoteUserLabel1.Text) || this.remoteUserLabel1.Text.StartsWith(userId + "["))
                return 1;
            else if (string.IsNullOrEmpty(this.remoteUserLabel2.Text) || this.remoteUserLabel2.Text.StartsWith(userId + "["))
                return 2;
            else if (string.IsNullOrEmpty(this.remoteUserLabel3.Text) || this.remoteUserLabel3.Text.StartsWith(userId + "["))
                return 3;
            else if (string.IsNullOrEmpty(this.remoteUserLabel4.Text) || this.remoteUserLabel4.Text.StartsWith(userId + "["))
                return 4;
            else if (string.IsNullOrEmpty(this.remoteUserLabel5.Text) || this.remoteUserLabel5.Text.StartsWith(userId + "["))
                return 5;
            return -1;
        }

        /// <summary>
        /// 获取远端用户的的窗口位置
        /// </summary>
        private int GetRemoteVideoPosition(String userId)
        {
            if (this.remoteUserLabel1.Text.StartsWith(userId + "["))
                return 1;
            else if (this.remoteUserLabel2.Text.StartsWith(userId + "["))
                return 2;
            else if (this.remoteUserLabel3.Text.StartsWith(userId + "["))
                return 3;
            else if (this.remoteUserLabel4.Text.StartsWith(userId + "["))
                return 4;
            else if (this.remoteUserLabel5.Text.StartsWith(userId + "["))
                return 5;
            return -1;
        }

        private Panel GetRemoteVideoPanel(int pos)
        {
            if (pos == 1)
                return remoteVideoPanel1;
            else if (pos == 2)
                return remoteVideoPanel2;
            else if (pos == 3)
                return remoteVideoPanel3;
            else if (pos == 4)
                return remoteVideoPanel4;
            else if (pos == 5)
                return remoteVideoPanel5;
            return null;
        }

        /// <summary>
        /// 是否显示提示远端用户是否打开视频的画面
        /// </summary>
        private void SetVisableInfoView(int pos, bool visable)
        {
            switch (pos)
            {
                case 1:
                    this.remoteInfoLabel1.Visible = visable;
                    break;
                case 2:
                    this.remoteInfoLabel2.Visible = visable;
                    break;
                case 3:
                    this.remoteInfoLabel3.Visible = visable;
                    break;
                case 4:
                    this.remoteInfoLabel4.Visible = visable;
                    break;
                case 5:
                    this.remoteInfoLabel5.Visible = visable;
                    break;
            }
        }

        /// <summary>
        /// 传入userId， 返回userId[姓名] 的组合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string userIdjoinName(string userId)
        {

            return userId + "[" + LocalSharedData.GetUserName(Convert.ToInt32(userId)) + "]";

        }

        /// <summary>
        /// 获取空闲的窗口句柄分发给进房用户或屏幕分享界面
        /// </summary>
        private IntPtr GetHandleAndSetUserId(int pos, string userId, bool isOpenSubStream)
        {
            switch (pos)
            {
                case 1:
                    this.remoteUserLabel1.Text = this.userIdjoinName(userId) + (isOpenSubStream ? "(屏幕分享)" : "");
                    return this.remoteVideoPanel1.Handle;
                case 2:
                    this.remoteUserLabel2.Text = this.userIdjoinName(userId) + (isOpenSubStream ? "(屏幕分享)" : "");
                    return this.remoteVideoPanel2.Handle;
                case 3:
                    this.remoteUserLabel3.Text = this.userIdjoinName(userId) + (isOpenSubStream ? "(屏幕分享)" : "");
                    return this.remoteVideoPanel3.Handle;
                case 4:
                    this.remoteUserLabel4.Text = this.userIdjoinName(userId) + (isOpenSubStream ? "(屏幕分享)" : "");
                    return this.remoteVideoPanel4.Handle;
                case 5:
                    this.remoteUserLabel5.Text = this.userIdjoinName(userId) + (isOpenSubStream ? "(屏幕分享)" : "");
                    return this.remoteVideoPanel5.Handle;
                default:
                    return IntPtr.Zero;
            }
        }

        /// <summary>
        /// 根据位置设置远端用户的音量提示功能显示
        /// </summary>
        private void SetRemoteVoiceVisable(int pos, bool visable)
        {
            if (pos == 1)
                this.remoteVoiceProgressBar1.Visible = visable;
            if (pos == 2)
                this.remoteVoiceProgressBar2.Visible = visable;
            if (pos == 3)
                this.remoteVoiceProgressBar3.Visible = visable;
            if (pos == 4)
                this.remoteVoiceProgressBar4.Visible = visable;
            if (pos == 5)
                this.remoteVoiceProgressBar5.Visible = visable;
        }

        /// <summary>
        /// 设置远端主流的麦克风音量
        /// </summary>
        private void SetRemoteVoiceVolume(string userId, int volume)
        {
            if (this.remoteUserLabel1.Text.StartsWith(userId + "[") && this.remoteVoiceProgressBar1.Visible)
                this.remoteVoiceProgressBar1.Value = volume;

            else if (this.remoteUserLabel2.Text.StartsWith(userId + "[") && this.remoteVoiceProgressBar2.Visible)
                this.remoteVoiceProgressBar2.Value = volume;

            else if (this.remoteUserLabel3.Text.StartsWith(userId + "[") && this.remoteVoiceProgressBar3.Visible)
                this.remoteVoiceProgressBar3.Value = volume;

            else if (this.remoteUserLabel4.Text.StartsWith(userId + "[") && this.remoteVoiceProgressBar4.Visible)
                this.remoteVoiceProgressBar4.Value = volume;

            else if (this.remoteUserLabel5.Text.StartsWith(userId + "[") && this.remoteVoiceProgressBar5.Visible)
                this.remoteVoiceProgressBar5.Value = volume;
        }
        
        /// <summary>
        /// 将远程用户音量全置为0
        /// </summary>
        private void SetAllRemoteVoiceVolumeZero()
        {
            this.remoteVoiceProgressBar1.Value = 0;
            this.remoteVoiceProgressBar2.Value = 0;
            this.remoteVoiceProgressBar3.Value = 0;
            this.remoteVoiceProgressBar4.Value = 0;
            this.remoteVoiceProgressBar5.Value = 0;
        }

        /// <summary>
        /// 根据用户是否退房找到用户画面当前窗口的位置
        /// </summary>
        private int FindOccupyRemoteVideoPosition(string userId, bool isExitRoom)
        {
            int pos = -1;
            if (this.remoteUserLabel1.Text.StartsWith(userId + "["))
            {
                pos = 1;
                if (isExitRoom)
                    this.remoteUserLabel1.Text = "";
            }
            if (this.remoteUserLabel2.Text.StartsWith(userId + "["))
            {
                pos = 2;
                if (isExitRoom)
                    this.remoteUserLabel2.Text = "";
            }
            if (this.remoteUserLabel3.Text.StartsWith(userId + "["))
            {
                pos = 3;
                if (isExitRoom)
                    this.remoteUserLabel3.Text = "";
            }
            if (this.remoteUserLabel4.Text.StartsWith(userId + "["))
            {
                pos = 4;
                if (isExitRoom)
                    this.remoteUserLabel4.Text = "";
            }
            if (this.remoteUserLabel5.Text.StartsWith(userId + "["))
            {
                pos = 5;
                if (isExitRoom)
                    this.remoteUserLabel5.Text = "";
            }
            if (isExitRoom)
                SetVisableInfoView(pos, true);
            return pos;
        }


        internal class AgoraRtcEngineEventHandler : IAgoraRtcEngineEventHandler
        {
            private AgoraMeetForm inst_ = null;  //当前窗口实例
            private IAgoraRtcEngine rtc_engine_ = null;
            private VideoCanvas vc_remote1;
            private VideoCanvas vc_remote2;
            private VideoCanvas vc_remote3;
            private VideoCanvas vc_remote4;
            private VideoCanvas vc_remote5;

            public AgoraRtcEngineEventHandler(AgoraMeetForm  _agoraMeetForm, IAgoraRtcEngine rtc_engine)
            {
                inst_ = _agoraMeetForm;
                rtc_engine_ = rtc_engine;
            }

            public override void OnActiveSpeaker(uint uid)
            {
                base.OnActiveSpeaker(uid);
            }



            //用于用户音量提示回调
            //这个会有2次回调，一次给本地用，一个是远程用
            public override void OnAudioVolumeIndication(AudioVolumeInfo[] speakers, uint speakerNumber, int totalVolume)
            {
                Console.WriteLine("=====>OnAudioVolumeIndication speakerNumber={0} totalVolume={1} speakers={2}", speakerNumber, totalVolume, speakers);

                if (inst_.IsHandleCreated)
                    inst_.BeginInvoke(new Action(() => 
                    {
                        //一次给本地用
                        if (speakerNumber == 0)
                        {
                            inst_.localVoiceProgressBar.Value = totalVolume; //volume：0~255
                        }

                        //远程的volume
                        if (speakers==null || speakers.Length == 0)
                        {
                            //全为空
                            inst_.SetAllRemoteVoiceVolumeZero();

                        } else
                        {
                            foreach (AudioVolumeInfo info in speakers)
                            {
                                if (info.uid.ToString().Equals("0") && inst_.localVoiceProgressBar.Visible)
                                    inst_.localVoiceProgressBar.Value = (int)info.volume; //volume：0~255
                                else
                                    inst_.SetRemoteVoiceVolume(info.uid.ToString(), (int)info.volume);
                            }
                        }                        

                        /*
                        if (speakerNumber <= 1) {
                            //本地的
                           if (speakerNumber == 1)
                            {
                                inst_.localVoiceProgressBar.Value = totalVolume; //volume：0~255
                            }
                            else if (speakerNumber == 0)
                            {
                                inst_.localVoiceProgressBar.Value = 0; //volume：0~255
                            }

                        } else
                        {
                            
                        }
                        */
                        
                    }));
                //base.OnAudioVolumeIndication(speakers, speakerNumber, totalVolume);
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
                    inst_.connectStatus = true;                    

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
                    Console.WriteLine("----->OnUserJoined uid={0}", uid);

                    if (inst_.mVideoType == 2)
                    {
                        // 显示远端用户主流画面
                        int pos = inst_.GetIdleRemoteVideoPosition(uid.ToString());
                        if (pos != -1)
                        {
                            inst_.SetVisableInfoView(pos, false);
                            IntPtr ptr = inst_.GetHandleAndSetUserId(pos, uid.ToString(), false);
                            if (ptr == IntPtr.Zero) return;
                            if (pos==1)
                                vc_remote1 = new VideoCanvas((ulong)ptr,RENDER_MODE_TYPE.RENDER_MODE_FIT, inst_.mChannelName, uid);
                            else if (pos == 2)
                                vc_remote2 = new VideoCanvas((ulong)ptr, RENDER_MODE_TYPE.RENDER_MODE_FIT, inst_.mChannelName, uid);
                            else if (pos == 3)
                                vc_remote3 = new VideoCanvas((ulong)ptr, RENDER_MODE_TYPE.RENDER_MODE_FIT, inst_.mChannelName, uid);
                            else if (pos == 4)
                                vc_remote4 = new VideoCanvas((ulong)ptr, RENDER_MODE_TYPE.RENDER_MODE_FIT, inst_.mChannelName, uid);
                            else if (pos == 5)
                                vc_remote5 = new VideoCanvas((ulong)ptr, RENDER_MODE_TYPE.RENDER_MODE_FIT, inst_.mChannelName, uid);

                            // 添加远端用户
                            User user = LocalSharedData.UserAllGroupCustomer.Find(delegate (User o) {
                                return o.userId == Convert.ToInt32(uid);
                            });
                            if (user != null)
                                inst_.members.Add(user);

                            int ret = -1;
                            if (pos==1)
                                ret= rtc_engine_.SetupRemoteVideo(vc_remote1);
                            else if (pos == 2)
                                ret = rtc_engine_.SetupRemoteVideo(vc_remote2);
                            else if (pos == 3)
                                ret = rtc_engine_.SetupRemoteVideo(vc_remote3);
                            else if (pos == 4)
                                ret = rtc_engine_.SetupRemoteVideo(vc_remote4);
                            else if (pos == 5)
                                ret = rtc_engine_.SetupRemoteVideo(vc_remote5);

                            Console.WriteLine("----->SetupRemoteVideo, ret={0}", ret);

                        }
                            
                       
                    }
                    else
                    {
                        //语音通话
                        //inst_.remoteVideoView.BackgroundImage = Properties.Resources.VoiceTalkBg;

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
                if (inst_.mVideoType==2)
                {

                    inst_.BeginInvoke(new Action(() =>
                    {
                        //vc_remote = new VideoCanvas(null,RENDER_MODE_TYPE.RENDER_MODE_FIT, inst_.mChannelName, uid);
                        //int ret = rtc_engine_.SetupRemoteVideo(vc_remote);
                        //Console.WriteLine("----->SetupRemoteVideo, ret={0}", ret);
                        //inst_.remoteVideoView.Visible = false;

                        // 移除远端用户主流画面
                        int pos = inst_.FindOccupyRemoteVideoPosition(uid.ToString(),true);
                        //int pos = inst_.GetRemoteVideoPosition(uid.ToString());

                        if (pos != -1)
                        {                            
                            inst_.SetVisableInfoView(pos, true);
                            inst_.SetRemoteVoiceVisable(pos, false);

                            // 去除远端
                            User user = LocalSharedData.UserAllGroupCustomer.Find(delegate (User o) {
                                return o.userId == Convert.ToInt32(uid);
                            });
                            if (user != null)
                            {
                                //inst_.userList.Remove(Convert.ToString(uid)); //保持一致
                                inst_.members.Remove(user); //保持一致
                            }                              


                            Panel remotePanel= inst_.GetRemoteVideoPanel(pos);
                            if (remotePanel!=null)
                                remotePanel.Refresh();  //可以调用refresh, 不用模拟 videoCanvas

                        }  

                    }));

                }
                else
                {
                    //音频通话的关闭
                    inst_.BeginInvoke(new Action(() =>
                    {
                        

                    }));

                }
            }

            public override void OnUserMuteAudio(uint uid, bool muted)
            {
                base.OnUserMuteAudio(uid, muted);
            }

            public override void OnUserMuteVideo(uint uid, bool muted)
            {
                //base.OnUserMuteVideo(uid, muted);
                //远端关闭和开启视频
                inst_.BeginInvoke(new Action(() =>
                {
                    int pos = inst_.GetRemoteVideoPosition(uid.ToString());
                    if (pos != -1)
                    {
                         
                        inst_.SetVisableInfoView(pos, muted);

                        Panel remotePanel = inst_.GetRemoteVideoPanel(pos);
                        if (remotePanel != null)
                            remotePanel.Refresh();  //可以调用refresh, 不用模拟 videoCanvas
                    }
                        
                }
                ));


             }

            public override void OnUserEnableVideo(uint uid, bool enabled)
            {
                base.OnUserEnableVideo(uid, enabled);
            }
        }

        private void AgoraMeetForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnInit(); 

            try
            {
                LocalSharedData.setVideoCallRunningState(false);
            }
            finally
            {
                LocalSharedData.setVideoCallRunningState(false);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //
            if (client != null)
            {
                //初始化时邀请的
                foreach (string str in userList)
                {
                    if (client != null)
                    {
                        client.SendMessage((new Data()).MeetChatCommandMessageEncode
                            ((short)MeetChat.MeetType.AV, (short)MeetChat.MeetCommand.End,
                            Convert.ToInt16(mChannelName),
                            LocalSharedData.CURRENTUser.user_id,
                            Convert.ToInt32(str), LocalSharedData.CURRENTUser.user_name,
                            LocalSharedData.GetUserName(Convert.ToInt32(str)), "desc"));

                    }
                    Task.Delay(20).Wait();
                }

                //显示后邀请的，与 初始化时邀请的有重叠
                foreach (User user in members)
                {
                    if (client != null)
                    {
                        client.SendMessage((new Data()).MeetChatCommandMessageEncode
                            ((short)MeetChat.MeetType.AV, (short)MeetChat.MeetCommand.End,
                            Convert.ToInt16(mChannelName),
                            LocalSharedData.CURRENTUser.user_id,
                            user.userId, LocalSharedData.CURRENTUser.user_name,
                            user.userName, "desc"));

                    }
                    Task.Delay(20).Wait();
                }

            }

            this.Close();
        }

        private void btnAudio_Click(object sender, EventArgs e)
        {
            if (this.rtc_engine_ != null)
            {
                if (audioStatus)
                {
                    //停推声音
                    rtc_engine_.MuteLocalAudioStream(true);
                    if (screen_share_engine_!=null)
                        screen_share_engine_.MuteLocalAudioStream(true);

                    this.btnAudio.Image = global::POCControlCenter.Properties.Resources.btn_audio_close1;

                    audioStatus = !audioStatus;
                }
                else
                {

                    //推流声音
                    rtc_engine_.MuteLocalAudioStream(false);
                    if (screen_share_engine_ != null)
                        screen_share_engine_.MuteLocalAudioStream(false);

                    this.btnAudio.Image = global::POCControlCenter.Properties.Resources.btn_audio_open1;
                    audioStatus = !audioStatus;
                }

            }
        }

        private void btnVideo_Click(object sender, EventArgs e)
        {
            if (this.rtc_engine_ != null)
            {
                if (videoStatus)
                {
                    //停推视频
                    rtc_engine_.MuteLocalVideoStream(true);
                    this.btnVideo.Image = global::POCControlCenter.Properties.Resources.btn_camera_close1;
                    videoStatus = !videoStatus;
                }
                else
                {

                    //推流视频
                    rtc_engine_.MuteLocalVideoStream(false);
                    this.btnVideo.Image = global::POCControlCenter.Properties.Resources.btn_camera_open1;
                    videoStatus = !videoStatus;
                }

            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            //弹出人员选择框
            SelUserForm crtform = new SelUserForm();
            crtform.clearChecklist();

            foreach (User user in LocalSharedData.UserAllGroupCustomer)
            {
                if (user.userId != LocalSharedData.CURRENTUser.user_id)
                {
                    
                    if (!this.members.Contains(user))
                    {
                        //不在才加入
                        if (user.logon == 1)
                            crtform.addChecklist(Convert.ToString(user.userId), user.userName + " <" + WinFormsStringResource.StatusOnline + ">");
                        else
                            crtform.addChecklist(Convert.ToString(user.userId), user.userName);

                    }

                }
            }
            //
            try
            {
                if (crtform.ShowDialog() == DialogResult.OK)
                {
                    string members = crtform.getMember();
                    if (members.Trim().Equals(""))
                        return;

                    string[] memArr = members.Split(',');
                    foreach (string str in memArr)
                    {
                        if (client != null)
                        {
                            client.SendMessage((new Data()).MeetChatCommandMessageEncode
                                ((short)MeetChat.MeetType.AV, (short)MeetChat.MeetCommand.Invite,
                                Convert.ToInt16(mChannelName),
                                LocalSharedData.CURRENTUser.user_id,
                                Convert.ToInt32(str), LocalSharedData.CURRENTUser.user_name,
                                LocalSharedData.GetUserName(Convert.ToInt32(str)), this.currStreamType));

                        }
                        Task.Delay(20).Wait();
                    }


                }
            }
            finally
            {
                crtform = null;
            }
        }

        private void screenShareCheckBox_Click(object sender, EventArgs e)
        {
            if (this.screenShareCheckBox.Checked)
            {
                // 开启屏幕分享功能
                AgoraScreenForm screenForm = new AgoraScreenForm(this,rtc_engine_);
                screenForm.ShowDialog();                
            }
            else
            {
                // 关闭屏幕分享功能
                if (!mIsSetScreenSuccess) return;

                screen_share_engine_.StopScreenCapture();
                screen_share_engine_.LeaveChannel();

                int ret = rtc_engine_.JoinChannel(mRtcToken,
                    mChannelName, "camera",(uint) LocalSharedData.CURRENTUser.user_id);


                if (mToastForm != null)
                {
                    mToastForm.Dispose();
                    mToastForm = null;
                }
                  
                

                this.currStreamType = "streamBig";
                
            }
        }

        private void loopBackCheckBox_Click(object sender, EventArgs e)
        {
            if (mIsSetScreenSuccess)
            {
                if (this.loopBackCheckBox.Checked)
                {
                    //screen_share_engine_.MuteLocalAudioStream(false);
                    screen_share_engine_.EnableLoopbackRecording(true, "");
                } else
                {
                    //screen_share_engine_.MuteLocalAudioStream(true);
                    screen_share_engine_.EnableLoopbackRecording(false, "");
                }
            }
        }
    }
}
