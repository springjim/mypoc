using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;  //记得u这俩.
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)] //com+可见
    public partial class TrackPlayBackForm : Form
    {

        ChromiumWebBrowser webBrower = null;

        public int userid=0;
        public int groupid=0;
        public int ZoneInterval_UserServer=0;
        public int query_userid=0;
        public string lng;
        public string lat;
        public string url; 
        public TrackPlayBackForm(string url)
        {
            InitializeComponent();
            

        }

        private void TrackPlayBackForm_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(url);
            webBrower = new ChromiumWebBrowser(url);
            webBrower.Dock = DockStyle.Fill;// 填充方式

            //过几秒开始js交互
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;//新cefsharp绑定需要优先申明            
            webBrower.IsBrowserInitializedChanged += WebBrower_IsBrowserInitializedChanged;
            webBrower.LoadError += WebBrower_LoadError;
            this.Controls.Add(webBrower);

        }

        private void WebBrower_LoadError(object sender, LoadErrorEventArgs e)
        {
            MessageBox.Show("脚本错误，" + e.ErrorText);
        }

        private void WebBrower_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
        {
            if (webBrower != null)
            {
                try
                {
                    //webBrower.Refresh();

                    webBrower.ExecuteScriptAsync("invokebyparam",
                        LocalSharedData.CURRENTUser.user_id, LocalSharedData.CURRENTUser.group_id, userid, 2, ZoneInterval_UserServer, lng, lat );

                    Task.Delay(3000).ContinueWith((a)=>
                    {
                        webBrower.ExecuteScriptAsync("invokebyparam",
                                                LocalSharedData.CURRENTUser.user_id, groupid, query_userid, 2, this.ZoneInterval_UserServer, lng, lat );

                    });


                }
                catch (Exception e3)
                {
                    MessageBox.Show(e3.Message);
                }
            }
        }

        
    }
}
