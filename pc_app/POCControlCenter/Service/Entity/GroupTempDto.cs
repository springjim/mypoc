using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Entity
{
    /// <summary>
    /// 
    /// 当类要在 POST中传递时，要求对于int或long可为null， 一定要加 int? 等类型，否则在REST服务端无法过滤掉该字段
    /// 以前旧的group_temp在 POCControlCenter.DataEntity
    /// </summary>
    public class GroupTempDto
    {     

        public int groupId { get; set; }
        /**
         * 
         */
        public String groupName { get; set; }
        /**
         * 
         */
        public int? cmpid { get; set; }
        /**
         * 
         */
        public int? ownerId { get; set; }
        /**
         * 0表示普通临时组, 1表示广播组
         */
        public int? aclass { get; set; }
        /**
         * 
         */
        public long?  createDate { get; set; }
        /**
         * 
         */
        public String  userIds { get; set; }
        /**
         * 调度员发起的为1, 终端发起的为0
         */
        public int? priv { get; set; }
    }
}
