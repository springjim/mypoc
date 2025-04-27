using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCClientNetLibrary
{
    /// <summary>
    /// 组同步消息
    /// </summary>
    public class GroupSyncMessage
    {
        public int groupId { get; set; }
        private byte groupNameLength { get; set; }
        private String groupName { get; set; }
        /// <summary>
        /// 被邀请的人ID
        /// </summary>
        private int userId { get; set; }
        /// <summary>
        /// 邀请人的人ID
        /// </summary>
        private int inviteId { get; set; }

        public override string ToString()
        {
            return "InviteMessage [groupId=" + groupId + ", groupNameLength="
                    + groupNameLength + ", groupName=" + groupName + ", userId="
                    + userId + ", inviteId=" + inviteId + "]";
        }

    }
}
