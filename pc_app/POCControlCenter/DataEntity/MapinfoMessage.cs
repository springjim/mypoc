using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public class MapinfoMessage
    {
        public String command { get; set; }
        public List<ArgumentItem>  arglist { get; set; }
    }

    public class ArgumentItem
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
        //定位时所在组
        public String group_name { get; set; }
        //纬度
        public string lat { get; set; }
        //经度
        public string lng { get; set; }

        //定位时间
        public string gpstime { get; set; }
        public int? batterylevel { get; set; }
        public string myclass_str { get; set; }
        public string flag_record_str { get; set; }
        public string life_state_str { get; set; }
        public int? logon { get; set; }

    }
}
