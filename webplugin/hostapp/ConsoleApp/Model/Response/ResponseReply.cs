using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Model.Response
{
    /// <summary>
    /// 对请求指令的回复，一般是是判断下基本环境，如抢麦是要麦克风的，如果没有则立即返回失败，一般是返回成功
    /// </summary>
    public class ResponseReply: ResponseBase
    {
        public string messageId { get; set; }

        /// <summary>
        /// 0 表示成功收到指令，如果为其它值，表示不允许下达这个请求，并在error字段中给以说明
        /// </summary>
        public int code { get; set; } = 0;

        public string error { get; set; } = "";

        // 静态方法：创建成功回复对象
        public static ResponseReply CreateSuccessReply(string messageId)
        {
            return new ResponseReply
            {
                messageType= "RESPONSE",
                messageId = messageId,
                code = 0,
                error = ""
            };
        }

        //
        // 静态方法：创建失败回复对象
        public static ResponseReply CreateFailureReply(string messageId, int errorCode, string errorMessage)
        {
            return new ResponseReply
            {
                messageType = "RESPONSE",
                messageId = messageId,
                code = errorCode,
                error = errorMessage
            };
        }


    }
}
