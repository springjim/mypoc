using POCClientNetLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Comm
{
    class DirUtils
    {

        private const string PARAFILE = "runpara.ini";

        /// <summary>
        /// 查找抓拍目录，如果没有设置，则返回空的
        /// </summary>
        /// <returns></returns>
        public static string GetSnapPath()
        {
            //写入inifile文件            
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, PARAFILE);
            //指定ini文件的路径
            IniFile ini = new IniFile(path);

            //群组会话下载路径
            if (ini.IniReadValue("download", "pathsession").Trim().Equals(""))
            {
                return "";
            }
            else
            {
                return ini.IniReadValue("download", "pathsession").Trim();
            }
        }


    }
}
