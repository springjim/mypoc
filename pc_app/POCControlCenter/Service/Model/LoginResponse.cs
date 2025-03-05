using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
    public class LoginResponse: ResponseBase
    {

        public Data data { get; set; }

        public class Data
        {
            public String cmpId { get; set; }  //企业的ID，在视频会议中可当作房间号
            public String accessToken { get; set; }
            public String tokenType { get; set; }
            public String expiresIn { get; set; }
            public String refreshToken { get; set; }
            public String scope { get; set; }

            //以下是用于 TRTC SDK的信息
            public String userSig { get; set; }
            public String sdkappId { get; set; }
        }       


    }
}
