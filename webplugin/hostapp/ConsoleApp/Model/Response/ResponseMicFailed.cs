using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Model.Response
{
    public class ResponseMicFailed : ResponseBase
    {
        public string error { get; set; } = "";
        public ResponseMicFailed()
        {
            messageType = "TYPE_MIC_FAILED";
            error = "抢麦失败";
        }
    }
}
