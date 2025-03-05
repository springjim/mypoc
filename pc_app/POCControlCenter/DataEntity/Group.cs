using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    /// <summary>
    /// 旧的组，兼容方式
    /// </summary>
    public class Group
    {
        /// <summary>
        /// 群类型，用于指定各种模式下的组类型，从而右键弹出相应上下文菜单
        /// </summary>
        public GroupTypeEnum group_type { get; set; }

        /// <summary>
        /// 临时组中 0表示普通临时组, 1表示广播组
        /// </summary>
        public int? aclass { get; set; }
        public int group_id { get; set; }
        public String user_ids { get; set; }
        public String group_name { get; set; }
        /// <summary>
        /// 用于群主ID
        /// </summary>
        public int owner_id { get; set; }
        public String create_date { get; set; }
        
    }

    public class NetHttpGroupData
    {
        public String error { get; set; }

        public List<Group> data { get; set; }
    }


}
