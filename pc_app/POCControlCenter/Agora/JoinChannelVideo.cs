using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using agora.rtc;


namespace POCControlCenter.Agora
{
    public class JoinChannelVideo: IEngine
    {
        private string app_id_ = "";
        private string channel_id_ = "";
        private readonly string JoinChannelVideo_TAG = "[JoinChannelVideo] ";
        private readonly string agora_sdk_log_file_path_ = "agorasdk.log";
        private IAgoraRtcEngine rtc_engine_ = null;
        private IAgoraRtcEngineEventHandler event_handler_ = null;
        private IntPtr local_win_id_ = IntPtr.Zero;
        private IntPtr remote_win_id_ = IntPtr.Zero;

        public JoinChannelVideo(IntPtr localWindowId, IntPtr remoteWindowId)
        {
            local_win_id_ = localWindowId;
            remote_win_id_ = remoteWindowId;
        }

        internal override int Init(string appId, string channelId)
        {

            int ret = -1;
            app_id_ = appId;            

            if (null == rtc_engine_)
            {
                rtc_engine_ = AgoraRtcEngine.CreateAgoraRtcEngine();
            }
            event_handler_ = new JoinChannelVideoEventHandler(this);
            rtc_engine_.InitEventHandler(event_handler_);

            RtcEngineContext rtc_engine_ctx = new RtcEngineContext(app_id_);
            ret = rtc_engine_.Initialize(rtc_engine_ctx);
            JoinChannelVideoView.dump_handler_(JoinChannelVideo_TAG + "Initialize", ret);
            if (ret == 0)
            {
                rtc_engine_.EnableAudio();
                rtc_engine_.EnableVideo();
                rtc_engine_.StartPreview();

                VideoCanvas vs = new VideoCanvas((ulong)local_win_id_, RENDER_MODE_TYPE.RENDER_MODE_FIT, channelId);
                vs.uid = 0;
                ret = rtc_engine_.SetupLocalVideo(vs);
                Console.WriteLine("----->SetupLocalVideo ret={0}", ret);

            }
            return ret;

        }

        internal override int UnInit()
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

        internal override int JoinChannel(string channelName,string rtcToken, uint uid)
        {
            int ret = -1;
            if (null != rtc_engine_)
            {
                
                ret=rtc_engine_.JoinChannel(rtcToken,
                    channelName, "", uid);

                JoinChannelVideoView.dump_handler_(JoinChannelVideo_TAG + "JoinChannel token ", ret);

            }
            return ret;
        }

        internal override int LeaveChannel()
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

        internal override string GetSDKVersion()
        {
            if (null == rtc_engine_)
                return "-" + (ERROR_CODE_TYPE.ERR_NOT_INITIALIZED).ToString();

            return rtc_engine_.GetVersion();
        }

        internal override IAgoraRtcEngine GetEngine()
        {
            return rtc_engine_;
        }

        internal string GetChannelId()
        {
            return channel_id_;
        }

        internal IntPtr GetLocalWinId()
        {
            return local_win_id_;
        }

        internal IntPtr GetRemoteWinId()
        {
            return remote_win_id_;
        }
    }

    // override if need
    internal class JoinChannelVideoEventHandler : IAgoraRtcEngineEventHandler
    {
        private JoinChannelVideo joinChannelVideo_inst_ = null;

        public JoinChannelVideoEventHandler(JoinChannelVideo _joinChannelVideo)
        {
            joinChannelVideo_inst_ = _joinChannelVideo;
        }

        public override void OnWarning(int warn, string msg)
        {
            Console.WriteLine("=====>OnWarning {0} {1}", warn, msg);
        }

        public override void OnError(int error, string msg)
        {
            Console.WriteLine("=====>OnError {0} {1}", error, msg);
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
          
        }

        public override void OnRejoinChannelSuccess(string channel, uint uid, int elapsed)
        {
            Console.WriteLine("----->OnRejoinChannelSuccess");
        }

        public override void OnLeaveChannel(RtcStats stats)
        {
            Console.WriteLine("----->OnLeaveChannel duration={0}", stats.duration);
        }

        public override void OnUserJoined(uint uid, int elapsed)
        {
            Console.WriteLine("----->OnUserJoined uid={0}", uid);
            if (joinChannelVideo_inst_.GetRemoteWinId() == IntPtr.Zero) return;
            var vc = new VideoCanvas((ulong)joinChannelVideo_inst_.GetRemoteWinId(), RENDER_MODE_TYPE.RENDER_MODE_FIT, joinChannelVideo_inst_.GetChannelId(), uid);
            int ret = joinChannelVideo_inst_.GetEngine().SetupRemoteVideo(vc);
            Console.WriteLine("----->SetupRemoteVideo, ret={0}", ret);
        }

        public override void OnUserOffline(uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Console.WriteLine("----->OnUserOffline reason={0}", reason);
        }

    }
}
