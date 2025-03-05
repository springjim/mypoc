using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    /// <summary>
    /// 对视频宫格控件的状态进行定义
    /// </summary>
    public enum  ControlVideoStateEnum
    {
        /// <summary>
        /// 空闲状态
        /// </summary>
        ControlVideo_IDLE,
        /// <summary>
        /// 语音通话拨打中
        /// </summary>
        ControlVideo_DoubleVoice_CALLING,
        /// <summary>
        /// 视频通话拨打中
        /// </summary>
        ControlVideo_DoubleVideo_CALLING,

        /// <summary>
        /// 语音通话进行中
        /// </summary>
        ControlVideo_DoubleVoice_PROCESSING,

        /// <summary>
        /// 视频通话进行中
        /// </summary>
        ControlVideo_DoubleVideo_PROCESSING,

        /// <summary>
        /// 视频直播进行中
        /// </summary>
        ControlVideo_VideoLive_PROCESSING,

        /// <summary>
        /// 视频监控进行中
        /// </summary>
        ControlVideo_VideoMoni_PROCESSING

    }
}
