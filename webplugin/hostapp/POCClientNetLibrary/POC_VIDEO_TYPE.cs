using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCClientNetLibrary
{
    public enum POC_VIDEO_TYPE
    {
        /// <summary>
        /// 单通模式的视频监控，又叫视频调取
        /// </summary>
        SINGLE_VIDEO_MONI,
        /// <summary>
        /// 单通模式的视频直播
        /// </summary>
        SINGLE_VIDEO_LIVE,
        /// <summary>
        /// 双通模式的视频通话
        /// </summary>
        DOUBLE_VIDEO,
        /// <summary>
        /// 双通模式的语音通话
        /// </summary>
        DOUBLE_VOICE,
        /// <summary>
        /// 缺省值，不清楚什么模式
        /// </summary>
        NORMAL        
    }
}
