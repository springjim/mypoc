using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    //public class GroupTemp
    //{
    //    //调度员发起的为1, 终端发起的为0
    //    public int priv { get; set; }
    //    public int group_id { get; set;  }
    //    public String user_ids { get; set; }
    //    public String group_name { get; set; }
    //    public int owner_id { get; set; }
    //    public String create_date { get; set; }
    //}

    public class NetHttpGroupTempData
    {
        public String error { get; set; }

        public String msg { get; set; }

        public Group data { get; set; }
    }

}
