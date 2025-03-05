using POCControlCenter.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter
{
    public partial class SOSPopUpForm : Form
    {
        private ControlMainForm mainForm;
        private bool isTalk = false;
        public int groupid;
        public  int userid;
        public SOSPopUpForm(ControlMainForm mainForm, int groupid, int userid)
        {
            this.mainForm = mainForm;
            this.groupid = groupid;
            this.userid = userid;

            InitializeComponent();
        }

        private void SOSPopUpForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.SOS_Session_Finish = true;
            mainForm.SOS_Session_UserID = 0;

            mainForm.sosPopupForm = null;
            mainForm.SOS_INCOME_CALL_ENABLE = false;
            webMap.Dispose();

        }

        private delegate void UpdatePlayUIStatusDelegate(Control ctrl, string s);
        public void SetPlayUIStatus(String Name)
        {
            if (labelPlayerName.InvokeRequired == true)
            {
                UpdatePlayUIStatusDelegate uld = new UpdatePlayUIStatusDelegate(UpdatePlayUIStatus);
                labelPlayerName.Invoke(uld, new object[] { labelPlayerName, Name });
            }
            else
            {
                UpdatePlayUIStatus(labelPlayerName, Name);
            }
        }

        private void UpdatePlayUIStatus(Control ctrl, string Name)
        {
            //2017.8.11 加入对讲单呼的识别            
            labelPlayerName.Text = Name;

        }

        private delegate void MapLocAndGetAdressDelegate(object loc);

        private void doMapLocAndGetAdress(object obj)
        {
            if (webMap.InvokeRequired == true)
            {
                MapLocAndGetAdressDelegate uld = new MapLocAndGetAdressDelegate(doMapLocAndGetAdress);
                webMap.Invoke(uld, new object[] { obj });
            }
            //           
            ////查找位置描述
            webMap.Visible = true;
            maploading.Visible = false;

            UpdateSOSLoc updatesosloc = (UpdateSOSLoc)obj;
            string url = HttpAPI.SOS_MAP_LOACTION_URL + "?lng=" + updatesosloc.lng
                + "&lat=" + updatesosloc.lat + "&place=" + HttpAPI.ToUrlEncode(updatesosloc.username);

            webMap.Navigate(HttpAPI.SOS_MAP_LOACTION_URL + "?lng=" + updatesosloc.lng
                + "&lat=" + updatesosloc.lat + "&place=" + HttpAPI.ToUrlEncode(updatesosloc.username));
           
            url = HttpAPI.queryBaiduLocdesc(updatesosloc.lat, updatesosloc.lng);

            labAddr.Text = url;

        }

        public void MapLocAndGetAdress(string username, double lng, double lat)
        {


            //UpdateSOSLoc updatesosloc = new UpdateSOSLoc();
            //updatesosloc.username = username;
            //updatesosloc.lat = lat;
            //updatesosloc.lng = lng;
            //Thread MapLocAndGetAdressThread = new Thread(new ParameterizedThreadStart(doMapLocAndGetAdress)) { Priority = ThreadPriority.Normal };

            //MapLocAndGetAdressThread.Start(updatesosloc);

            //查找位置描述
            webMap.Visible = true;
            maploading.Visible = false;

            //UpdateSOSLoc updatesosloc = (UpdateSOSLoc)obj;
            string url = HttpAPI.SOS_MAP_LOACTION_URL + "?lng=" + lng
                + "&lat=" + lat + "&place=" + HttpAPI.ToUrlEncode(username);

            webMap.Navigate(HttpAPI.SOS_MAP_LOACTION_URL + "?lng=" + lng
                + "&lat=" + lat + "&place=" + HttpAPI.ToUrlEncode(username));

            url = HttpAPI.queryBaiduLocdesc(lat, lng);

            labAddr.Text = url;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="display_map"></param>
        public void MapLocInit(Boolean display_map)
        {
            //地图已经显示了,就不要屏弊了   
            if (display_map)
            {
                webMap.Visible = false;
                maploading.Visible = true;
            }

        }

        private void buttonTALK_Click(object sender, EventArgs e)
        {
            //
            if (!isTalk)
            {
                //未说话
                isTalk = true;
                //由于服务器要识别发给谁，并且还要显示发自方的userid,所以以下将groupid替代为调度台的userid
                //mainForm.StartChat(2, groupid, userid);
                mainForm.StartChat(2, LocalSharedData.CURRENTUser.user_id, userid);
                labelPlayerName.Text = "我方说话中...";
                buttonTALK.Text = WinFormsStringResource.Main_buttonTALK_Text2;
                //发出SOS对讲开始信号
                mainForm.SendSOSTalkStart(userid);

            }
            else
            {
                isTalk = false;
                mainForm.EndChat_forSOS();
                labelPlayerName.Text = "";
                buttonTALK.Text = WinFormsStringResource.Main_buttonTALK_Text1;
                //发出SOS对讲结束信号
                mainForm.SendSOSTalkStop(userid);

            }


        }

        private void webMap_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //WebBrowser web = (WebBrowser)sender;
            
                //ControlMainForm_ShowGroupLocation(LocalSharedData.CURRENTUser.group_id, webBrowserBAIDUMAP);
                this.webMap.Document.Window.Error += Window_Error;

            
        }

        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            MessageBox.Show("脚本错误，第" + e.LineNumber + "行：" + e.Description);
            e.Handled = true;
        }
    }
}
