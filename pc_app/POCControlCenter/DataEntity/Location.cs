using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class Location
    {
        public int group_id { get; set; }
        public int user_id { get; set; }
        public String phone { get; set; }
        public String user_name { get; set; }
        public String group_name { get; set; }
        public String loactionname { get; set; }
        public double  latitude { get; set; }
        public double longitude { get; set; }
        public int?   logon { get; set; }
        public long?  curtime { get; set; }
        public String  gpstime { get; set; }
        public String  state { get; set; }

        // 加入电量
        public int? batterylevel { get; set; }

        //2017.8.9  加入录音开关，lifestate,话权值
        public int? myclass { get; set; }
        public string myclass_str { get; set; }

        public String flag_record { get; set; }
        public string flag_record_str { get; set; }

        public int? life_state { get; set; }
        public string life_state_str { get; set; }



    }
    public class NetHttpLocationData
    {
        public String error { get; set; }
        public List<Location> data { get; set; }
    }
}
