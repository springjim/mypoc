using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Entity
{
    public class LivePushDto
    {
        public int userId { get; set; }
        public String userName { get; set; }
        public String channelName { get; set; }
    }
}
