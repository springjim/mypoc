using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Comm
{
    public class GroupUtils
    {
        /// <summary>
        /// 清除所有与组织架构视图相关的组
        /// </summary>
        public static void clearOrgAllGroup()
        {
            LocalSharedData.UserAllTempGROUP.Clear();
            LocalSharedData.OrgAsscGROUP_NoSelf.Clear();
            LocalSharedData.UserAllGROUP.Clear();
            LocalSharedData.UserAllGroupCustomer.Clear();
            LocalSharedData.GROUPAllUser.Clear();
            LocalSharedData.UserAll.Clear();

        }
    }
}
