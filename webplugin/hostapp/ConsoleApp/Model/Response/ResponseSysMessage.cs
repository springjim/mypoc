using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Model.Response
{
    public class ResponseSysMessage : ResponseBase
    {

        /**某用户开始说话*/
        public const int SYS_MSSAGE_TALK_START = 1;
        /**某用户停止说话*/
        public const int SYS_MSSAGE_TALK_STOP = 2;
        /**某用户进入某群组*/
        public const int SYS_MSSAGE_IN_GROUP = 3;
        /**某用户离开某群组*/
        public const int SYS_MSSAGE_OUT_GROUP = 4;
        /**某用户拒绝邀请*/
        public const int SYS_MSSAGE_REJECT_INVITE = 5;
        
        /**某用户同意单聊邀请*/
        public const int SYS_MSSAGE_ENTER_PRESON = 6;
        /**某用户拒绝单聊邀请*/
        public const int SYS_MSSAGE_EXIT_PRESON = 7;

        /**某用户上线*/
        public const int SYS_MSSAGE_ONLINE_PRESON = 8;
        /**某用户掉线*/
        public const int SYS_MSSAGE_OFFLINE_PRESON = 9;

        /**转POC某用户开始说话*/
        public const int SYS_MSSAGE_TALK_START_TOPOC = 10;
        /**转POC某用户停止说话*/
        public const int SYS_MSSAGE_TALK_STOP_TOPOC = 11;

        /**呼叫中对方正在通话*/
        public const int SYS_MSSAGE_TALK_INCALL = 12;

        public const int TYPE_TOPOC_START_MIC = 16;//申请中继台成功
        public const int TYPE_TOPOC_FAIL_MIC = 17; //申请中继台失败或释放中继台

        public const int TYPE_TOPOC_RELEASE_SUCCESS_MIC = 18;//申请中继台成功
        public const int TYPE_TOPOC_RELEASE_FAIL_MIC = 19;//申请中继台失败或释放中继台


        public string groupId { get; set; }
        public string userId { get; set;  }

        /// <summary>
        ///具体值有： 1： 表示某人在某组讲话  2：表示某人在某组停止讲话  3：某人进入某组  4：某人离开某组   8：某用户上线，但不确定组    9：某用户终端下线 
        /// </summary>       
        public int state { get; set; }

        public ResponseSysMessage(string groupId, string userId, int state)
        {
            messageType = "TYPE_SYS_MESSAGE";
            this.groupId = groupId;
            this.userId = userId;
            this.state = state;
            
        }


    }



}
