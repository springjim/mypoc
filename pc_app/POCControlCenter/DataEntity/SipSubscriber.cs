using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public  class SipSubscriber
    {
        public int id { get; set; }
        public String username { get; set; }
        public String domain { get; set; }
        public String ha1 { get; set; }
    }

    public class NetHttpSipSubscriberData
    {
        public String error { get; set; }
        public SipSubscriber data { get; set; }
    }
}
