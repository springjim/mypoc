using POCClientNetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Tool
{
    /// <summary>
    /// 专用于对runpara.ini 的参数进行操作
    /// </summary>
    public class RunparaUtils
    {
        private const string PARAFILE = "runpara.ini";
        private const string LISTEN_GROUP_SECTION = "voice";
        private const string LISTEN_GROUP_KEY = "listen_group";

        /// <summary>
        /// 当前正在监听的对讲组, 以逗号隔开
        /// </summary>
        public static string CurrentListenGroups;

        private static IniFile ini;

        static RunparaUtils()
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, PARAFILE);
            //指定ini文件的路径
            ini = new IniFile(path);
        }

        /// <summary>
        /// 读取所有监听组
        /// </summary>
        public static string[] readListenGroup()
        {
            string val = ini.IniReadValue(LISTEN_GROUP_SECTION, LISTEN_GROUP_KEY);
            if (String.IsNullOrEmpty(val))
            {
                CurrentListenGroups = null;
                return null;

            }
            else
            {
                CurrentListenGroups = val;
                return val.Split(',');
            }


        }


        /// <summary>
        /// 增加监听组
        /// </summary>
        /// <param name="groupId"></param>
        public static void addListenGroup(int groupId)
        {
            string val = ini.IniReadValue(LISTEN_GROUP_SECTION, LISTEN_GROUP_KEY);
            if (String.IsNullOrEmpty(val))
            {
                ini.IniWriteValue(LISTEN_GROUP_SECTION, LISTEN_GROUP_KEY, groupId + "");
                CurrentListenGroups = groupId + "";
                return;
            }

            // 将数组转换成 HashSet，自动去除重复项
            HashSet<string> hashSet = new HashSet<string>(val.Split(','));
            hashSet.Add(groupId + "");

            ini.IniWriteValue(LISTEN_GROUP_SECTION, LISTEN_GROUP_KEY, String.Join(",", hashSet.ToList()));
            CurrentListenGroups = String.Join(",", hashSet.ToList());
        }

        /// <summary>
        /// 移除监听组
        /// </summary>
        /// <param name="groupId"></param>
        public static void removeListenGroup(int groupId)
        {
            string val = ini.IniReadValue(LISTEN_GROUP_SECTION, LISTEN_GROUP_KEY);
            if (String.IsNullOrEmpty(val))
            {
                CurrentListenGroups = null;
                return;
            }

            // 将数组转换成 HashSet，自动去除重复项
            HashSet<string> hashSet = new HashSet<string>(val.Split(','));
            hashSet.Remove(groupId + "");

            ini.IniWriteValue(LISTEN_GROUP_SECTION, LISTEN_GROUP_KEY, String.Join(",", hashSet.ToList()));
            CurrentListenGroups = String.Join(",", hashSet.ToList());

        }
        
    }
}
