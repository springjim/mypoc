using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
    public class AgoraRtcTokenResponse : ResponseBase
    {

        public Data data { get; set; }

        public class Data
        {
            public string rtcToken { get; set; }
            public int expire { get; set; }
            public string appId { get; set; }
        }        

    }
}
