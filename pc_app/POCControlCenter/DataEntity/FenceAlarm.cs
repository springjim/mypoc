using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class FenceAlarm
    {
        public int id { get; set; }

        public String action_time { get; set; }
        public String fence_name { get; set; }
        public String user_name { get; set; }
        public int group_id { get; set; }
        public int fence_userid { get; set; }
        public int user_id { get; set; }
        //2017.7.6 加入

        //alarm表的告警类型:是income还是outcome
        private String action_type { get; set; }  	

    }
}
