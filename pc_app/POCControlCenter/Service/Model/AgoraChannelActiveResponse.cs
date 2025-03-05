using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Model
{
    public class AgoraChannelActiveResponse : ResponseBase
    {

        public List<Data>  data { get; set; }

        public class Data
        {
            public String channelName { get; set; }

            public int userCount { get; set; }
        }
    }
}
