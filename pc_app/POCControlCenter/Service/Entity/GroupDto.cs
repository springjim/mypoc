using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service.Entity
{
    /// <summary>
    /// 新的固定组类
    /// </summary>
    public class GroupDto
    {
        public int gid { get; set; }
        public int groupId { get; set; }
        public string groupName { get; set; }
        public int? cmpid { get; set; }
        /**
         * 群主，一般没用这个字段
         */
        public int? ownerId { get; set; }
        public int? aclass { get; set; }
        public long? createDate { get; set; }
        public string  groupNumber { get; set; }
        public int? busiessId { get; set; }

        /// <summary>
        /// 定制的岗位类型,1表示机动队, 2绿化队, 3安防队,4其它
        /// </summary>
        public int? worktype { get; set; }
        /// <summary>
        /// 关联部门ID,如果不是boss_cmporg中的talkgroupid,则是部分对讲群
        /// </summary>
        public int? orgid { get; set; }


    }
}
