using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Tool
{
    public class CommUtils
    {
        public static string readServerIp()
        {
            string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            if (string.IsNullOrEmpty(serverIP))
            {
                Log.E("app.config文件中未找到 ServerIP 配置！");            
                return "";
            }
            return serverIP;
        }

        public static string readServerPort()
        {
            string serverPort = ConfigurationManager.AppSettings["ServerPort"];
            if (string.IsNullOrEmpty(serverPort))
            {
                Log.E("app.config文件中未找到ServerPort配置！");
                return "";
            }
            return serverPort;
        }

    }
}
