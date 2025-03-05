using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace POCControlCenter.Agora
{
    public  class ConfigHelper
    {
        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key,
   string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32.dll")]
        private static extern int GetUserDefaultLCID();

        [DllImport("kernel32.dll")]

        private static extern int GetGeoInfo(int geoid, int geoType, StringBuilder lpGeoData, int cchData, int langid);
        string config_file_path_
        {
            get;
            set;
        }

        string config_ui_path_
        {
            get;
            set;
        }
        public ConfigHelper()
        {
            // path to res/config/API_Example.ini
            config_file_path_ = System.IO.Directory.GetCurrentDirectory() + "\\API_Example.ini";
            if (!File.Exists(config_file_path_))
            {
                //CSharpForm.dump_handler_(config_file_path_ + " not Exists!!!!", -1);
            }
            int lcid = GetUserDefaultLCID();
            if (lcid == 2052)//chinese
            {
                config_ui_path_ = System.IO.Directory.GetCurrentDirectory() + "\\zh-cn.ini";
            }
            else//english
            {
                config_ui_path_ = System.IO.Directory.GetCurrentDirectory() + "\\en.ini";
            }

            if (!File.Exists(config_ui_path_))
            {
                //CSharpForm.dump_handler_(config_ui_path_ + " not Exists!!!!", -1);
            }
        }

        public void SetValue(string Section, string Key, string Value)
        {
            long OpStation = WritePrivateProfileString(Section, Key, Value, config_file_path_);
            if (OpStation == 0)
            {
                // fail
                string msg = "save " + Key + "/" + Value;
                //CSharpForm.dump_handler_(msg, -1);
            }
        }

        public string GetValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, Key, "", temp, 1024, config_file_path_);
            return temp.ToString();
        }

        public string GetUIValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, Key, "", temp, 1024, config_ui_path_);
            return temp.ToString();
        }

        public void OpenConfigFile()
        {
            try
            {
                System.Diagnostics.Process.Start("notepad.exe", config_file_path_);
            }
            catch (System.ComponentModel.Win32Exception we)
            {
                System.Console.WriteLine(we.Message);
            }
        }
    }
}
