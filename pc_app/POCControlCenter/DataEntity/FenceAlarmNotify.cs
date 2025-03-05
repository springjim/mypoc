using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class FenceAlarmNotify
    {
        public String action_time { get; set; }
        public String fence_name { get; set; }
        public String user_name { get; set; }
        public int    group_id { get; set; }
        public int    fence_userid { get; set; }
        public int    user_id { get; set; }

        public int fence_id { get; set; }
        public int    id { get; set; }           //alarm表的id
        public String action_type { get; set; }  //alarm表的告警类型:是income还是outcome	

    }

    public class NetHttpFenceAlarmNotifyData
    {
        public String error { get; set; }

        public List<FenceAlarmNotify> data { get; set; }
    }
}
