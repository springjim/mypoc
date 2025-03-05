using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.DataEntity
{
    public enum GroupTypeEnum
    {
        /// <summary>
        /// 纯对讲模式下的固定组
        /// </summary>
        TALK_FIX,
        /// <summary>
        ///纯对讲模式下的临时组 
        /// </summary>
        TALK_TMP,

        /// <summary>
        /// 广播组是临时组的一种,在临时表的class字段为1表示广播组，0表示普通的临时组
        /// </summary>
        TALK_TMP_BROADCAST,

        /// <summary>
        /// 组织模式下的关联部门组，且含自己
        /// </summary>
        ORG_ASSC_INCLUDE_SELF,
        /// <summary>
        /// 组织模式下的关联部门组，且不含自己
        /// </summary>
        ORG_ASSC_NO_SELF,
        /// <summary>
        /// 组织模式下的无关联部门的固定组
        /// </summary>
        ORG_UNASSC_FIX,
        /// <summary>
        /// 组织模式下的无关联部门的临时组
        /// </summary>
        ORG_UNASSC_TMP,
        /// <summary>
        /// 通讯录下的对讲终端类
        /// </summary>
        CONTACT_TALK_DEVICE,
        /// <summary>
        /// 通讯录下的调度台类
        /// </summary>
        CONTACT_PLATFORM_DEVICE,
        /// <summary>
        /// 通讯录下的摄像头类
        /// </summary>
        CONTACT_CAMERA_DEVICE,

        /// <summary>
        /// 电子围栏
        /// </summary>
        CONTACT_FENCE

    }
}
