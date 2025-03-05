using POCControlCenter.Admin;
using POCControlCenter.DataEntity;
using POCControlCenter.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter
{
    public class LocalSharedData
    {
        private static readonly object mylock= new object();

        public static String      CURRENTGroupName = "";

        /// <summary>
        /// 旧的这个兼容,因为要调用的REST接口服务端有访问socket 
        /// </summary>
        public static User_Old  CURRENTUser = new User_Old();

        /// <summary>
        /// 组类型: 1 表示固定的, 2表示临时的
        /// </summary>
        public static String      CURRENTGroupType = "1"; 

        //以下是纯对讲模式的共享变量
        //************************************
        /// <summary>
        /// 纯对讲模式下的所有固定组
        /// </summary>
        public static List<Group> UserAllGROUP = new List<Group>();
        /// <summary>
        /// 纯对讲模式下的所有临时组
        /// </summary>
        public static List<Group> UserAllTempGROUP = new List<Group>();  
        /// <summary>
        /// 纯对讲模式下的所有人员名单
        /// </summary>
        public static List<User>  UserAllGroupCustomer = new List<User>();

        public static Dictionary<int, List<User>> GROUPAllUser = new Dictionary<int, List<User>>();

        public static Dictionary<int, User> UserAll = new Dictionary<int, User>();
        
        public static List<Group> OrgAsscGROUP_NoSelf = new List<Group>();
        
              

   

        //表示状态的
        public static Boolean isVideoCallRunning = false;


        public static Boolean getVideoCallRunningState()
        {
            lock (mylock)
            {
                return isVideoCallRunning;
            }
        }

        public static void setVideoCallRunningState(Boolean state)
        {
            lock (mylock)
            {
                isVideoCallRunning = state;
            }
        }

        public static String GetUserName( int userID )
        {
            if ( UserAll.ContainsKey(userID) )
                return UserAll[userID].userName;
            else
                return "NULL";
        }

        public static string GetGroupName(int groupId)
        {
            Group group = UserAllGROUP.Find(delegate (Group o) {

                return o.group_id == groupId;

            });

            if (group == null)
            {
                group = UserAllTempGROUP.Find(delegate (Group o) {

                    return o.group_id == groupId;

                });
            }

            if (group == null)
                return "NULL";
            else
                return group.group_name;

        }

        
    }
}
