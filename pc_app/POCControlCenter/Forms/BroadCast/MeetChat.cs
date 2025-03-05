using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.BroadCast
{
    public class MeetChat
    {
        public enum MeetType
        {
            PureAudio = 1,  //纯音频
            AV = 2  //音视频都有
        }
        public enum MeetCommand
        {
            Invite = 1,  //邀请请求
            Agree = 2,  //同意应答
            Refuse=3,   //拒绝应答
            Timeout=4,  //超时未应答
            Exit= 5,      //主动退出会议
            End=6       //PC发出结束会议
        }

        public enum ShareCommand
        {
            Start=1,  //屏幕分享开始
            Stop= 2     //屏幕分享结束
        }
        
    }
}
