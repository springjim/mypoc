using agora.rtc;
using POCControlCenter.Agora.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.Service.Entity
{

    /// <summary>
    /// 记录每个Item状态
    /// </summary>
   public class LivePushItem
    {
        /// <summary>
        /// 编号,从 1开始
        /// </summary>
        public int cellNo { get; set; }
        public IAgoraRtcChannel channel { get; set; } = null;
        public IAgoraRtcChannelEventHandler channel_event_handler { get; set; } = null;
        /// <summary>
        /// 目标channel名称
        /// </summary>
        public string destChannelName { get; set; } = "";
        /// <summary>
        /// 状态
        /// </summary>
        public AVLiveStatusEnum connectStatus { get; set; } = AVLiveStatusEnum.Init;
        /// <summary>
        /// 对方姓名
        /// </summary>
        public string fromUserName { get; set; } = "";
        /// <summary>
        /// 对方userId
        /// </summary>
        public int fromUserId { get; set; } = 0;
        /// <summary>
        /// true 表示直播, false 表示监控
        /// </summary>
        public bool isLiveBoardType { get; set; } = true;
        /// <summary>
        /// true 打开对方audio  false 关闭对方audio
        /// </summary>
        public bool remoteAudioStatus { get; set; } = true;
        /// <summary>
        /// true 打开对方video  false 关闭对方video
        /// </summary>
        public bool remoteVideoStatus { get; set; } = true;

        //UserControl 引用
        public LiveUserControl control { get; set; } = null;

        /// <summary>
        /// UserControl 是否最大化, 0表示未有, 1表示是最大化
        /// </summary>
        public int controlMaxState { get; set; } = 0;

        //计时器,用于显示直播的time字段内容
        public Timer timer { get; set; } = null;

        /// <summary>
        /// 开始直播时间
        /// </summary>
        public DateTime  liveStartTime { get; set; }
        
    }
}
