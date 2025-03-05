using POCControlCenter.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.Service.Local
{
    /// <summary>
    /// 专用于组方法操作
    /// </summary>
    public class GroupService
    {
        /// <summary>
        /// 表示空的组ID，用于退出会话状态, 值为-1， Server端配合识别
        /// </summary>
        public static int EMPTY_GROUP_ID = -1;

        /// <summary>
        /// 临时组的前缀
        /// </summary>
        public static string TMP_GROUP_PREFIX = "[临时]";


        /// <summary>
        /// 临时广播组的前缀
        /// </summary>
        public static string TMP_BROADCAST_GROUP_PREFIX = "[广播]";

        /// <summary>
        /// 根据不同的Group类型，显示右键菜单
        /// </summary>
        /// <param name="typeEnum"></param>
        /// <param name="GRPcontextMenuStrip"></param>
        public static void GroupContextMenuItemDisplay(GroupTypeEnum typeEnum, ContextMenuStrip grpContextMenuStrip)
        {
            if (typeEnum == GroupTypeEnum.TALK_FIX)
            {
                //对讲模式下的固定组(含自己)   

                grpContextMenuStrip.Items["toolStripMenuItemDelGrp"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemModify"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemForceAdd"].Visible = false;

                grpContextMenuStrip.Items["toolStripMenuItemFenceManager"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemAllGPSCommand"].Visible = true;
                grpContextMenuStrip.Items["toolStripMenuItemFileQuery"].Visible = true;

                grpContextMenuStrip.Items["toolStripMenuItemEnter"].Visible = true;
                grpContextMenuStrip.Items["toolStripMenuItemEnterBroadCast"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemExit"].Visible = true;

                grpContextMenuStrip.Items["toolStripMenuItemAudioRec"].Visible = false;


            }
            else if (typeEnum == GroupTypeEnum.TALK_TMP)
            {
                //对讲模式下的临时组(含自己)                        
                grpContextMenuStrip.Items["toolStripMenuItemDelGrp"].Text = "解散对讲组";
                grpContextMenuStrip.Items["toolStripMenuItemDelGrp"].Visible = true;

                grpContextMenuStrip.Items["toolStripMenuItemModify"].Text = "修改组名称";
                grpContextMenuStrip.Items["toolStripMenuItemModify"].Visible = true;

                grpContextMenuStrip.Items["toolStripMenuItemForceAdd"].Text = "强插用户入组";
                grpContextMenuStrip.Items["toolStripMenuItemForceAdd"].Visible = true;

                grpContextMenuStrip.Items["toolStripMenuItemFenceManager"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemAllGPSCommand"].Visible = true;
                grpContextMenuStrip.Items["toolStripMenuItemFileQuery"].Visible = true;

                grpContextMenuStrip.Items["toolStripMenuItemEnter"].Visible = true;
                grpContextMenuStrip.Items["toolStripMenuItemEnterBroadCast"].Visible = false;  //广播会话
                grpContextMenuStrip.Items["toolStripMenuItemExit"].Visible = true;

                grpContextMenuStrip.Items["toolStripMenuItemAudioRec"].Visible = false;

            }
            else if (typeEnum == GroupTypeEnum.TALK_TMP_BROADCAST)
            {
                grpContextMenuStrip.Items["toolStripMenuItemDelGrp"].Text = "解散广播组";
                grpContextMenuStrip.Items["toolStripMenuItemDelGrp"].Visible = true;

                grpContextMenuStrip.Items["toolStripMenuItemModify"].Text = "修改广播组名称";
                grpContextMenuStrip.Items["toolStripMenuItemModify"].Visible = true;

                grpContextMenuStrip.Items["toolStripMenuItemForceAdd"].Text = "强插用户入广播组";
                grpContextMenuStrip.Items["toolStripMenuItemForceAdd"].Visible = true;

                grpContextMenuStrip.Items["toolStripMenuItemFenceManager"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemAllGPSCommand"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemFileQuery"].Visible = false;

                grpContextMenuStrip.Items["toolStripMenuItemEnter"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemEnterBroadCast"].Visible = true;  //广播会话
                grpContextMenuStrip.Items["toolStripMenuItemExit"].Visible = false;

                grpContextMenuStrip.Items["toolStripMenuItemAudioRec"].Visible = true;
            }
            else if (typeEnum == GroupTypeEnum.ORG_ASSC_INCLUDE_SELF)
            {
                //组织模式下的固定组(含自己)                       
                //先未启用               

            }
            else if (typeEnum == GroupTypeEnum.ORG_ASSC_NO_SELF)
            {
                //组织模式下的固定组(不含自己)                        
                //先未启用  

            }
            else if (typeEnum == GroupTypeEnum.ORG_UNASSC_FIX)
            {
                //组织模式下的未关联的固定组(含自己)                        
                //先未启用  


            }
            else if (typeEnum == GroupTypeEnum.ORG_UNASSC_TMP)
            {
                //组织模式下的未关联的临时组(含自己)                       
                //先未启用 
            }
            else if (typeEnum == GroupTypeEnum.CONTACT_TALK_DEVICE)
            {
                //联系人模式下的对讲终端     
             
                grpContextMenuStrip.Items["toolStripMenuItemDelGrp"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemModify"].Visible = false;          
                grpContextMenuStrip.Items["toolStripMenuItemForceAdd"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemFenceManager"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemAllGPSCommand"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemFileQuery"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemEnter"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemEnterBroadCast"].Visible = false;  //广播会话
                grpContextMenuStrip.Items["toolStripMenuItemExit"].Visible = false;

                grpContextMenuStrip.Items["toolStripMenuItemAudioRec"].Visible = false;

            }
            else if (typeEnum == GroupTypeEnum.CONTACT_PLATFORM_DEVICE)
            {
                //联系人模式下的调度台端  
                grpContextMenuStrip.Items["toolStripMenuItemDelGrp"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemModify"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemForceAdd"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemFenceManager"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemAllGPSCommand"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemFileQuery"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemEnter"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemEnterBroadCast"].Visible = false;  //广播会话
                grpContextMenuStrip.Items["toolStripMenuItemExit"].Visible = false;

                grpContextMenuStrip.Items["toolStripMenuItemAudioRec"].Visible = false;

            }
            else if (typeEnum == GroupTypeEnum.CONTACT_CAMERA_DEVICE)
            {
                //联系人模式下的摄像头
                grpContextMenuStrip.Items["toolStripMenuItemDelGrp"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemModify"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemForceAdd"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemFenceManager"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemAllGPSCommand"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemFileQuery"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemEnter"].Visible = false;
                grpContextMenuStrip.Items["toolStripMenuItemEnterBroadCast"].Visible = false;  //广播会话
                grpContextMenuStrip.Items["toolStripMenuItemExit"].Visible = false;

                grpContextMenuStrip.Items["toolStripMenuItemAudioRec"].Visible = false;
            }
        }

        /// <summary>
        /// 根据选择模式分析选中的节点
        /// </summary>
        /// <param name="SELECT_TREEVIEW_MODE"></param>
        /// <param name="treeViewSearch"></param>
        /// <param name="treeViewContact"></param>
        /// <returns></returns>
        public static HashSet<String> SelectedUserNode(int SELECT_TREEVIEW_MODE, TreeView treeViewSearch, TreeView treeViewContact)
        {

            HashSet<String> userList = new HashSet<string>();
            //批量监控
            if (SELECT_TREEVIEW_MODE == 2)
            {
                //从搜索中查
                foreach (TreeNode node in treeViewSearch.Nodes)
                {
                    if (node.Tag != null && node.Tag is Group)
                    {
                        foreach (TreeNode subNode in node.Nodes)
                        {
                            if (subNode.Checked && subNode.Tag != null && subNode.Tag is User)
                            {
                                userList.Add(((User)subNode.Tag).userId + "");
                            }
                        }

                    }
                    else if (node.Tag != null && node.Tag is User && node.Checked)
                    {
                        //子节点
                        userList.Add(((User)node.Tag).userId + "");
                    }

                }

            }
            else if (SELECT_TREEVIEW_MODE == 3)
            {
                //从通讯录中查
                foreach (TreeNode node in treeViewContact.Nodes)
                {
                    if (node.Tag != null && node.Tag is Group)
                    {
                        foreach (TreeNode subNode in node.Nodes)
                        {
                            if (subNode.Checked && subNode.Tag != null && subNode.Tag is User)
                            {
                                if (((User)subNode.Tag).userId != LocalSharedData.CURRENTUser.user_id)
                                    userList.Add(((User)subNode.Tag).userId + "");
                            }
                        }

                    }
                    else if (node.Tag != null && node.Tag is User && node.Checked)
                    {
                        //子节点
                        if (((User)node.Tag).userId != LocalSharedData.CURRENTUser.user_id)
                            userList.Add(((User)node.Tag).userId + "");
                    }

                }

            }

            return userList;

        }

    }
}
