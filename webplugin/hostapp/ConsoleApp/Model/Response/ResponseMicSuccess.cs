using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Model.Response
{
    public class ResponseMicSuccess: ResponseBase
    {
        public string error { get; set; } = "";

        public  ResponseMicSuccess ()
        {
            messageType = "TYPE_MIC_SUCCESS";
            error = "";
        }


    }
}
