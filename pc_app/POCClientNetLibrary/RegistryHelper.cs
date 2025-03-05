using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;

namespace POCClientNetLibrary
{
    public class RegistryHelper
    {
        public static string AppPath
        {
            get
            {
                return Path.Combine(new[] { ChatHelper.SOFTWARE, ChatHelper.APP_NAME, ChatHelper.VERSION });
            }
        }

        public static void InitRegisterComm()
        {
            //IE for webbrowser设置，第1处
            //MessageBox.Show(AppPath);
            var key = Registry.CurrentUser;
            string appName = System.IO.Path.GetFileName(Application.ExecutablePath);
            //HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION
            string FEATURE_BROWSER_EMULATION_Path = @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
            if (key.OpenSubKey(FEATURE_BROWSER_EMULATION_Path) == null)
                key.CreateSubKey(FEATURE_BROWSER_EMULATION_Path);

            key = key.OpenSubKey(FEATURE_BROWSER_EMULATION_Path, true);
            if (key != null)
            {
                key.SetValue(appName, 0x2edf,RegistryValueKind.DWord);
            }
            //IE for webbrowser设置，第2处
            key = Registry.LocalMachine;
            FEATURE_BROWSER_EMULATION_Path = @"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION";
            if (key.OpenSubKey(FEATURE_BROWSER_EMULATION_Path) == null)
                key.CreateSubKey(FEATURE_BROWSER_EMULATION_Path);

            key = key.OpenSubKey(FEATURE_BROWSER_EMULATION_Path, true);
            if (key != null)
            {
                key.SetValue(appName, 0x2edf, RegistryValueKind.DWord);
            }
            //IE for webbrowser设置，第3处            
            key = Registry.LocalMachine;
            FEATURE_BROWSER_EMULATION_Path = @"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION";
            if (key.OpenSubKey(FEATURE_BROWSER_EMULATION_Path) == null)
                key.CreateSubKey(FEATURE_BROWSER_EMULATION_Path);

            key = key.OpenSubKey(FEATURE_BROWSER_EMULATION_Path, true);
            if (key != null)
            {
                key.SetValue(appName, 0x2edf, RegistryValueKind.DWord);
            }

            //解决IE跳出了stop running this script的提示, 提示如下
            //http://blog.csdn.net/u012081284/article/details/50215513

            key = Registry.CurrentUser;
            FEATURE_BROWSER_EMULATION_Path = @"Software\Microsoft\Internet Explorer\Styles";
            if (key.OpenSubKey(FEATURE_BROWSER_EMULATION_Path) == null)
                key.CreateSubKey(FEATURE_BROWSER_EMULATION_Path);

            key = key.OpenSubKey(FEATURE_BROWSER_EMULATION_Path, true);
            if (key != null)
            {
                key.SetValue("MaxScriptStatements", 0xffffffff,RegistryValueKind.QWord);
            }

            //解决FIPS 报错的问题,参考如下 2017.8.23
            //http://blog.csdn.net/KingOf007/article/details/53958686
            key = Registry.LocalMachine;
            FEATURE_BROWSER_EMULATION_Path = @"SYSTEM\CurrentControlSet\Control\Lsa\FipsAlgorithmPolicy";
            if (key.OpenSubKey(FEATURE_BROWSER_EMULATION_Path) == null)
                key.CreateSubKey(FEATURE_BROWSER_EMULATION_Path);

            key = key.OpenSubKey(FEATURE_BROWSER_EMULATION_Path, true);
            if (key != null)
            {
                //设为0，表示禁用FIPS加密算法
                key.SetValue("Enabled", 0x0, RegistryValueKind.DWord);
            }


        }

        public static void Write(ChatClient client)
        {
            var key = Registry.CurrentUser;
            if (key.OpenSubKey(AppPath) == null)
                key.CreateSubKey(AppPath);
            key = key.OpenSubKey(AppPath, true);
            if (key == null)
                return;

            #region Write to registry

            key.SetValue(ChatHelper.SERVER_IP, client.confServerAddress);

            key.SetValue(ChatHelper.SERVER_PORT, client.confServerPort);
            key.SetValue(ChatHelper.USER_NAME, client.UserName);
            key.SetValue(ChatHelper.PASSWORD, client.Password);

            #endregion
        }

        public static void Read(ChatClient client)
        {
            var key = Registry.CurrentUser;
            key = key.OpenSubKey(AppPath);

            if (key == null)
                return;

            #region Read registry keys

            string serverip = (string)key.GetValue(ChatHelper.SERVER_IP);
            string serverport = (string)key.GetValue(ChatHelper.SERVER_PORT);
            string username = (string)key.GetValue(ChatHelper.USER_NAME);
            string password = (string)key.GetValue(ChatHelper.PASSWORD);

            #endregion

            #region Initializing client
            if (serverip == null)
                client.confServerAddress = "";
            else
                client.confServerAddress = (string)(serverip);

            if (serverport == null)
                client.confServerPort = "";
            else
                client.confServerPort = (string)(serverport);

            if (username == null)
                client.UserName = "";
            else
                client.UserName = (string)(username);

            if (username == null)
                client.Password = "";
            else
                client.Password = (string)(password);

            #endregion
        }
    }
}
