using Microsoft.Win32;
using POCClientNetLibrary;
using POCControlCenter.DataEntity;
using POCControlCenter.Service;
using POCControlCenter.Trtc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace POCControlCenter
{

    static class Program
    {
        public static ChatClient client;
        public static ControlMainForm mainForm;
        private const  string PARAFILE = "platform.ini";        
                

        public static ChatClient Init_ConnMediaServer()
        {
            var chatClient = new ChatClient(Int32.Parse(HttpAPI.MEDIAPort), HttpAPI.MEDIAIP);
            return chatClient;
        }

        // 外部函数声明
        [DllImport("User32.dll")]
        private static extern Int32 SetProcessDPIAware();

        public static EventWaitHandle ProgramStarted;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            // 尝试创建一个命名事件
            bool createNew;
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "POCPlatformStartEvent", out createNew);

            // 如果该命名事件已经存在(存在有前一个运行实例)，则发事件通知并退出
            if (!createNew)
            {
                ProgramStarted.Set();
                return;
            }

            SetProcessDPIAware();   // 默认关闭高DPI，避免SDK录制出错
            Log.Open();           

            /////////////////

            Process processes = Process.GetCurrentProcess();
            Log.I(String.Format("Progress <{0}, {1}>", processes.ProcessName, processes.Id));

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;          

            //再读取platform.ini
            string loginwelcome = "欢迎登入多媒体指挥调度系统";
            bool parafile_exists = InitPlatFormPara(out loginwelcome);
            if (!parafile_exists)
            {
                MessageBox.Show("未发现" + PARAFILE + "文件！系统退出!");
                Application.Exit();
            }
            //静态变量还要更新一次
            UpdateALLPlatFormPara();

            Application.EnableVisualStyles();
            //获得当前登录的Windows用户标示 
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();

            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            if (!principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("请用具有管理员权限的Windows帐号打开本应用");
                return;
            }

            Application.SetCompatibleTextRenderingDefault(false);

            //由于webbrowser需要注册表注册才能使用IE11及相关
            RegistryHelper.InitRegisterComm();

            client = Init_ConnMediaServer();
            RegistryHelper.Read(client);

            ControlLoginNew login = new ControlLoginNew(client, loginwelcome);
            login.mstrAccount = client.UserName;
            login.mstrPassword = client.Password;

            //根据操作系统的默认语言项来显示登入框的界面语言
            if (System.Globalization.CultureInfo.InstalledUICulture.Name == "zh-CN")
            {                                               
                login.labelWelcome.Text = loginwelcome;
                login.comboBoxLanguage.SelectedIndex = 0;

            }
            else if (System.Globalization.CultureInfo.InstalledUICulture.Name == "zh-TW")
            {                
                login.Text = "系統登入";
                login.labelAccount.Text = "帳號:";
                login.labelPassword.Text = "密碼:";
                login.labelLanguage.Text = "語言:";
                login.buttonCancel.Text = "取消";
                login.buttonOK.Text = "確定";
                login.labelWelcome.Text = loginwelcome;
                login.comboBoxLanguage.SelectedIndex = 1;

            }
            else
            {                
                login.Text = "User login";
                login.labelAccount.Text = "Account:";
                login.labelPassword.Text = "Password:";
                login.labelLanguage.Text = "Language:";
                login.buttonCancel.Text = "Cancel";
                login.buttonOK.Text = "OK";
                login.labelWelcome.Text = loginwelcome;
                login.comboBoxLanguage.SelectedIndex = 2;
            }

            DialogResult res = login.ShowDialog();
            if (res == DialogResult.OK)
            {               
                try
                {                
                  
                    //登入后，还要更新一次,因为里面有和语言资源相关的赋值
                    UpdateALLPlatFormPara();
                    client.UserName = login.mstrAccount;
                    client.Password = login.mstrPassword;

                    Dictionary<int, User>.KeyCollection keyCol = LocalSharedData.UserAll.Keys;
                    List<int> myuserids = new List<int>();
                    foreach (int key in keyCol)
                    {
                        myuserids.Add(key);
                    }
                    client.userids = myuserids;

                    RegistryHelper.Write(client);
                    mainForm = new ControlMainForm(client);

                    Application.Run(mainForm);
                } catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            

            Log.Close();

        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           
            if (e.IsTerminating)
            {
                //MessageBox.Show("发现异常，即将终止程序");
            }

        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            
            //List<T> 转换为 BindingCollection<Location>(ILocation)会报错
            //记住，在正式生产发布时，下面要注释
            MessageBox.Show("Application_ThreadException: "+e.Exception.Message);
            Debug.WriteLine(e.Exception.Message);

        }

        private static void UpdateALLPlatFormPara()
        {
            HttpAPI.UriUserStr = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/api/user/?";
            HttpAPI.UriGroupStr = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/api/group/?";
            HttpAPI.UriLocationStr = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/api/location/?";
            HttpAPI.UriFenceStr = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/api/fence/?";
            HttpAPI.UriLocationMAP = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/back/";
            HttpAPI.UriLocationBaiduMAP = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/back/baidumap.jsp";
            HttpAPI.UriLocationTrackMAP = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/back/baidu_trackback.jsp";
            HttpAPI.UriLocationFenceMAP = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/back/baidu_fence.jsp";

            
            HttpAPI.UriVideoStop_MoniOrLive = "http://" + HttpAPI.VIDEOIP + ":8081/control/drop/publisher";
            HttpAPI.UriVideoMonitor_Apply = "http://" + HttpAPI.VIDEOIP + ":"+ HttpAPI.VIDEOPort + "/vmonitor/v1/monitor_apply.json";
           
            HttpAPI.UriVideoMonitor_Apply_RTSP = "http://" + HttpAPI.VIDEOIP + ":"+ HttpAPI.VIDEOPort + "/vmonitor/v1/monitor_apply_rtsp.json";                       


            /**上传*/
            HttpAPI.UpdateURL = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/servlet/UploadHandleServlet";
            /**下载*/
            HttpAPI.DownloadURL = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/servlet/DownloadHandleServlet";
           
            HttpAPI.FenceAlarmMap_URL = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/back/" + WinFormsStringResource.LocationFenceAlarmMap;

            HttpAPI.SOS_MAP_LOACTION_URL = "http://" + HttpAPI.VIDEOIP + ":" + HttpAPI.VIDEO_XAMPP_Port + "/mapviewloc_baidu_for_sos.php";
                        
            //离线地图的本地目录及文件
            HttpAPI.UriLocationMAPOffline_Dir = "map/baidumap";
            HttpAPI.UriLocationMAPOffline_MapFile = "baidumap.html";
            HttpAPI.UriLocationMAPOffline_FenceFile = "baidu_fence.html";
            HttpAPI.UriLocationMAPOffline_TrackFile = "baidu_trackback.html";
            //离线地图的服务访问地图(基站版本或公网版本的用户选择了离线百度地图)
            //UriLocationMAPOffline_MAPACTION_URL_PREFIX指定baidumap_action.jsp前缀
            HttpAPI.UriLocationMAPOffline_MAPACTION_URL_PREFIX = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/back/";
            
            HttpAPI.UriLocationMAPOffline_FenceACTION_API_PREFIX = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/api/fence/?action=query_grp_gps";

            //UriLocationMAPOffline_FENCEACTION_URL_PREFIX指定../api/fence/的前缀(轨迹和围栏都引用)
            HttpAPI.UriLocationMAPOffline_FENCEACTION_URL_PREFIX = "http://" + HttpAPI.WEBIP + ":" + HttpAPI.WEBPort + "/ptt/";


        }

        /// <summary>
        /// 初始化总的配置
        /// </summary>
        private static bool InitPlatFormPara(out string loginwelcome)
        {
            //读取配置文件
            loginwelcome = "欢迎登入多媒体指挥调度系统";
            string path = System.IO.Path.Combine(Application.StartupPath, PARAFILE);
            if (!File.Exists(path))
                return false;
            
            IniFile ini = new IniFile(path);

            if (ini.IniReadValue("basestation", "flag").Equals(""))
                ControlMainForm.JIZHAN_ENABLE = false;
            else
            {
                if (ini.IniReadValue("basestation", "flag").ToUpper().Equals("TRUE"))
                    ControlMainForm.JIZHAN_ENABLE = true;
                else
                    ControlMainForm.JIZHAN_ENABLE = false;
            }

            //
            string servname = ini.IniReadValue("server", "servname").Trim();
           
            if (servname.Equals(""))
            {
                servname = "RB_OLD";
            }
            else
            {
                servname = servname.ToUpper();
            }
            string server_ipaddr = ini.IniReadValue("server", "ipaddr").Trim();
            string server_ipaddr_port= ini.IniReadValue("server", "ipaddr_port").Trim();

            string server_voice_port = ini.IniReadValue("server", "ipaddr_voice_port").Trim();
            string server_ipaddr_voice= ini.IniReadValue("server", "ipaddr_voice").Trim();
            

            string server_prefix = ini.IniReadValue("server", "servprefix").Trim();
            string web_port = ini.IniReadValue("web", "port").Trim();
            string web_appname = ini.IniReadValue("web", "appname").Trim();            
            string dbaddr = ini.IniReadValue("db", "dbaddr").Trim();
            string loginwelcomecaption = ini.IniReadValue("login", "loginwelcome").Trim();

            //进行服务器处理
            if (!loginwelcomecaption.Trim().Equals(""))
            {
                loginwelcome = loginwelcomecaption.Trim();
            }         

           
            //2. HttpAPI 中WEBIP(web服务),MEDIAIP(媒体服务),VIDEOIP(视频服务),VEDIOIP_PREFIX(boss网站) 处理            
           
            HttpAPI.WEBIP = "119.28.7.70";
            if (!server_ipaddr.Equals(""))
                HttpAPI.WEBIP = server_ipaddr_voice;

            HttpAPI.VIDEOPort = "8699";        
            if (!server_ipaddr_port.Equals(""))
                HttpAPI.VIDEOPort = server_ipaddr_port;

            //语音服务器, 与视频服务器分离
            HttpAPI.MEDIAIP = "119.28.7.70";
            if (!server_ipaddr_voice.Equals(""))
                HttpAPI.MEDIAIP = server_ipaddr_voice;

            HttpAPI.WEBPort = "8080";           
            if (!server_voice_port.Equals(""))
                HttpAPI.WEBPort = server_voice_port;


            HttpAPI.VIDEOIP = "119.28.7.70";
            if (!server_ipaddr.Equals(""))
                HttpAPI.VIDEOIP = server_ipaddr;

            HttpAPI.VIDEO_XAMPP_Port = "80";
            if (!web_port.Equals(""))
                HttpAPI.VIDEO_XAMPP_Port = web_port;
          
          
            HttpAPI.VEDIOIP_PREFIX = "119.28.7.70:80/bossv";
            if (!server_ipaddr.Equals("") &&
                !web_port.Equals("") &&
                !web_appname.Equals(""))
            {
                HttpAPI.VEDIOIP_PREFIX = server_ipaddr + ":" + web_port + "/" + web_appname;
            }
        
            return true;

        }

        
        
    }

    public class LanguageHelper
    {       
       
     
        #region SetLang
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lang">language:zh-CN, en-US</param>
        /// <param name="form">the form you need to set</param>
        /// <param name="formType">the type of the form </param>
        public static void SetLang(string lang, Form form, Type formType)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);

            if (form != null)
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(formType);
                resources.ApplyResources(form, "$this");
                AppLang(form, resources);
            }
        }
        #endregion
        #region AppLang for control
        /// <summary>
        ///  loop set the propery of the control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resources"></param>
        private static void AppLang(Control control, System.ComponentModel.ComponentResourceManager resources)
        {
            if (control is MenuStrip)
            {
                resources.ApplyResources(control, control.Name);
                MenuStrip ms = (MenuStrip)control;
                if (ms.Items.Count > 0)
                {
                    foreach (ToolStripMenuItem c in ms.Items)
                    {
                        AppLang(c, resources);
                    }
                }
            }
            foreach (Control c in control.Controls)
            {
                resources.ApplyResources(c, c.Name);
                AppLang(c, resources);
            }
        }
        #endregion
        #region AppLang for menuitem
        /// <summary>
        /// set the toolscript 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resources"></param>
        private static void AppLang(ToolStripMenuItem item, System.ComponentModel.ComponentResourceManager resources)
        {
            if (item is ToolStripMenuItem)
            {
                resources.ApplyResources(item, item.Name);
                ToolStripMenuItem tsmi = (ToolStripMenuItem)item;
                if (tsmi.DropDownItems.Count > 0)
                {
                    foreach (ToolStripMenuItem c in tsmi.DropDownItems)
                    {
                        AppLang(c, resources);
                    }
                }
            }
        }
        #endregion
    }
}
