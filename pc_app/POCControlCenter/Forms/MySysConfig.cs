using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POCClientNetLibrary;

using System.IO;
using POCControlCenter.DataEntity;
using System.Collections;
using System.Reflection;
using AutoUpdaterDotNET;
using System.Threading;
using POCControlCenter.Service;
using POCControlCenter.Service.Model;
using POCControlCenter.Service.Entity;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using AForge.Video.DirectShow;
using System.Text.RegularExpressions;

namespace POCControlCenter
{
    public partial class MySysConfig : Form
    {
        private const string PARAFILE = "runpara.ini";
        
        private List<MyKeyValue> list_priv = new List<MyKeyValue>();
        private List<MyKeyValue> list_city = new List<MyKeyValue>();
        private List<MyKeyValue> list_region = new List<MyKeyValue>();

        private FilterInfoCollection videoDevices;//所有摄像设备
        private VideoCaptureDevice videoDevice;//摄像设备
        private VideoCapabilities[] videoCapabilities;//摄像头分辨率



        ControlMainForm mainform;
        public MySysConfig(ControlMainForm mainform)
        {
            InitializeComponent();            
            this.mainform = mainform;

        }

        private void button2_Click(object sender, EventArgs e)
        {

            


            //首先要检证
            //写入inifile文件            
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, PARAFILE);
            //指定ini文件的路径
            IniFile ini = new IniFile(path);

            if (cbTVMode.SelectedIndex == -1)
                ini.IniWriteValue("other", "navigate", "talkgroup");
            else if (cbTVMode.SelectedIndex == 0)
                ini.IniWriteValue("other", "navigate", "talkgroup");
            else
                ini.IniWriteValue("other", "navigate", "org_talkgroup");

            if (cbMoniResolv.SelectedIndex == -1)
                ini.IniWriteValue("video", "moni_resolv", "480p");
            else if (cbMoniResolv.SelectedIndex == 0)
                ini.IniWriteValue("video", "moni_resolv", "1080p");
            else if (cbMoniResolv.SelectedIndex == 1)
                ini.IniWriteValue("video", "moni_resolv", "720p");
            else if (cbMoniResolv.SelectedIndex == 2)
                ini.IniWriteValue("video", "moni_resolv", "480p");
            else if (cbMoniResolv.SelectedIndex == 3)
                ini.IniWriteValue("video", "moni_resolv", "360p");
            else if (cbMoniResolv.SelectedIndex == 4)
                ini.IniWriteValue("video", "moni_resolv", "240p");


            if (cbMapLocAutoUpdate.Checked)
                ini.IniWriteValue("maploc", "autoenable", "Y");
            else
                ini.IniWriteValue("maploc", "autoenable", "N");

            if (cbVideoLivePopup.Checked)
                ini.IniWriteValue("video", "livepopup", "Y");
            else
                ini.IniWriteValue("video", "livepopup", "N");

            //短消息接收提示 cbMsgPopup
            if (cbMsgPopup.Checked)
                ini.IniWriteValue("other", "msgpopup", "Y");
            else
                ini.IniWriteValue("other", "msgpopup", "N");

            //音视频设备异常提示开关 cbDeviceErrorPrompt
            if (cbDeviceErrorPrompt.Checked)
                ini.IniWriteValue("other", "av_device_error", "Y");
            else
                ini.IniWriteValue("other", "av_device_error", "N");

            //主窗口标题自定义 tbMainformCaption
            if (tbMainformCaption.Text.Trim().Equals(""))
                ini.IniWriteValue("other", "maincaption", "");
            else
                ini.IniWriteValue("other", "maincaption", tbMainformCaption.Text.Trim());

            //
            if (cbMapProvince.SelectedIndex > -1)
            {
                ini.IniWriteValue("other", "map_province_name", ((MyKeyValue)cbMapProvince.SelectedItem).pValue );
                ini.IniWriteValue("other", "map_province_code", ((MyKeyValue)cbMapProvince.SelectedItem).pKey);

            } else
            {
                ini.IniWriteValue("other", "map_province_name", "" );
                ini.IniWriteValue("other", "map_province_code", "" );
            }

            if (cbMapCity.SelectedIndex > -1)
            {
                ini.IniWriteValue("other", "map_city_name", ((MyKeyValue)cbMapCity.SelectedItem).pValue);
                ini.IniWriteValue("other", "map_city_code", ((MyKeyValue)cbMapCity.SelectedItem).pKey);

            } else
            {
                ini.IniWriteValue("other", "map_city_name", "");
                ini.IniWriteValue("other", "map_city_code", "");
            }

            if (cbMapRegion.SelectedIndex > -1)
            {
                ini.IniWriteValue("other", "map_region_name", ((MyKeyValue)cbMapRegion.SelectedItem).pValue);
                ini.IniWriteValue("other", "map_region_code", ((MyKeyValue)cbMapRegion.SelectedItem).pKey);

            } else
            {
                ini.IniWriteValue("other", "map_region_name", "" );
                ini.IniWriteValue("other", "map_region_code", "" );
            }           
            
            //再存储经纬度坐标
            List<RegionChinaDto> list = null;
            RegionChinaResponse resp = null;
            String str = "";

            if (cbMapRegion.SelectedIndex > -1)
            {
                
                str = ((MyKeyValue)cbMapRegion.SelectedItem).pKey;

                resp = PocClient.getRegionChinaCommon(Convert.ToInt32(str));
                if (resp != null && resp.data != null && resp.data.Count > 0)
                    list = resp.data;

                
                if (list != null && list.Count>0)
                {
                    ini.IniWriteValue("other", "map_lat", list[0].latitude.ToString("f6"));
                    ini.IniWriteValue("other", "map_lng", list[0].longitude.ToString("f6"));
                }

            } else if (cbMapCity.SelectedIndex > -1)
            {
                
                str = ((MyKeyValue)cbMapCity.SelectedItem).pKey;

                resp = PocClient.getRegionChinaCommon(Convert.ToInt32(str));
                if (resp != null && resp.data != null && resp.data.Count > 0)
                    list = resp.data;
                
                if (list != null && list.Count > 0)
                {
                    ini.IniWriteValue("other", "map_lat", list[0].latitude.ToString("f6"));
                    ini.IniWriteValue("other", "map_lng", list[0].longitude.ToString("f6"));
                }

            } else if (cbMapProvince.SelectedIndex > -1)
            {                 

                str = ((MyKeyValue)cbMapProvince.SelectedItem).pKey;

                resp = PocClient.getRegionChinaCommon(Convert.ToInt32(str));
                if (resp != null && resp.data != null && resp.data.Count > 0)
                    list = resp.data;

               
                if (list != null && list.Count > 0)
                {
                    ini.IniWriteValue("other", "map_lat", list[0].latitude.ToString("f6"));
                    ini.IniWriteValue("other", "map_lng", list[0].longitude.ToString("f6"));
                }
            }

                //地图
            if (cbMapType.SelectedIndex==-1)
                ini.IniWriteValue("other", "maptype", "");
            else if (cbMapType.SelectedIndex == 0)
                ini.IniWriteValue("other", "maptype", "baidu");
            else if (cbMapType.SelectedIndex == 1)
                ini.IniWriteValue("other", "maptype", "google");
            else if (cbMapType.SelectedIndex == 2)
                ini.IniWriteValue("other", "maptype", "baidu_offline");
            else if (cbMapType.SelectedIndex == 3)
                ini.IniWriteValue("other", "maptype", "mapinfo_offline");
            //语言
            if (cbLangType.SelectedIndex == -1)
                ini.IniWriteValue("other", "langtype", "");
            else if (cbLangType.SelectedIndex == 0)
                ini.IniWriteValue("other", "langtype", "zh-CN");
            else if (cbLangType.SelectedIndex == 1)
                ini.IniWriteValue("other", "langtype", "zh-TW");
            else if (cbLangType.SelectedIndex == 2)
                ini.IniWriteValue("other", "langtype", "en-US");
            //围栏报警
            if (cbFenceAlarmType.SelectedIndex == -1)
                ini.IniWriteValue("other", "fencealarmtype", "");
            else if (cbFenceAlarmType.SelectedIndex == 0)
                ini.IniWriteValue("other", "fencealarmtype", "voice");
            else if (cbFenceAlarmType.SelectedIndex == 1)
                ini.IniWriteValue("other", "fencealarmtype", "silent");

            //各种记录路径保存
            ini.IniWriteValue("download", "pathvideo", tbDownPathVideo.Text.Trim());
            ini.IniWriteValue("download", "pathaudio", tbDownPathAudio.Text.Trim());

            

            ini.IniWriteValue("download", "pathsession", tbDownPathSession.Text.Trim());
            if (!String.IsNullOrEmpty(tbDownPathSession.Text.Trim()))
            {
                //2023.11.04 由于agora的截图本地路径中不允许有中文
                //https://pythonjishu.com/bthyzezdnopicqp/ 检测中文方法
                if (Regex.IsMatch(tbDownPathSession.Text.Trim(), @"[\u4e00-\u9fa5]"))
                {
                    MessageBox.Show("抓拍存放路径设置中，不允许有中文");
                    return;
                }

            }

            ControlMainForm.getSingleControlMainForm().FullDownloadPicPath = tbDownPathSession.Text.Trim();
            //视频播放长宽比率
            //16:9  4:3  1:1  16:10  2.21:1   2.35:1  2.39:1  5:4
            if (cbAspectRatio.SelectedIndex ==- 1)
                ini.IniWriteValue("video", "aspect_ratio", "default");
            else if (cbAspectRatio.SelectedIndex == 0)
                ini.IniWriteValue("video", "aspect_ratio", "default");
            else if (cbAspectRatio.SelectedIndex == 1)
                ini.IniWriteValue("video", "aspect_ratio", "16:9");
            else if (cbAspectRatio.SelectedIndex == 2)
                ini.IniWriteValue("video", "aspect_ratio", "4:3");
            else if (cbAspectRatio.SelectedIndex == 3)
                ini.IniWriteValue("video", "aspect_ratio", "1:1");
            else if (cbAspectRatio.SelectedIndex == 4)
                ini.IniWriteValue("video", "aspect_ratio", "16:10");
            else if (cbAspectRatio.SelectedIndex == 5)
                ini.IniWriteValue("video", "aspect_ratio", "2.21:1");
            else if (cbAspectRatio.SelectedIndex == 6)
                ini.IniWriteValue("video", "aspect_ratio", "2.35:1");
            else if (cbAspectRatio.SelectedIndex == 7)
                ini.IniWriteValue("video", "aspect_ratio", "2.39:1");
            else if (cbAspectRatio.SelectedIndex == 8)
                ini.IniWriteValue("video", "aspect_ratio", "5:4");
            
            //
            if (cbNonCurGrpVoice.SelectedIndex == -1)
                ini.IniWriteValue("voice", "noncurgroup", "onlycurrent");
            else if (cbNonCurGrpVoice.SelectedIndex == 0)
                ini.IniWriteValue("voice", "noncurgroup", "onlycurrent");
            else if (cbNonCurGrpVoice.SelectedIndex == 1)
                ini.IniWriteValue("voice", "noncurgroup", "all");

            //*******************************************************************            

            ini.IniWriteValue("maploc", "frequency", tbInterval.Text.Trim());
            ini.IniWriteValue("maploc", "gps_valid", tbGpsOnInterval.Text.Trim());
            ini.IniWriteValue("zone", "user_server_zoneinterval", tbZoneInterval.Text.Trim());
            //保存sip设置
            string stun = "stun.sipgate.net";
            if (comboBoxSTUN.SelectedIndex > -1)
            {
                ini.IniWriteValue("sip", "stun", comboBoxSTUN.SelectedItem.ToString());
                stun = comboBoxSTUN.SelectedItem.ToString();
            } else if (!comboBoxSTUN.Text.Trim().Equals(""))
            {
                ini.IniWriteValue("sip", "stun", comboBoxSTUN.Text.Trim());
                stun = comboBoxSTUN.Text.Trim();
            }

            //listen_port
            string listen_port = "5060";

            if (!sipPortBox.Text.Trim().Equals(""))
            {
                ini.IniWriteValue("sip", "listen_port", sipPortBox.Text.Trim());
                listen_port = sipPortBox.Text.Trim();
            }

            //active_playbackdevice
            string active_playbackdevice = "";
            if (comboBoxPlayback.SelectedIndex > -1)
            {
                ini.IniWriteValue("sip", "active_playbackdevice", comboBoxPlayback.SelectedItem.ToString());
                active_playbackdevice = comboBoxPlayback.SelectedItem.ToString();

            }

            //active_recorddevice
            string active_recorddevice = "";
            if (comboBoxRecord.SelectedIndex > -1)
            {
                ini.IniWriteValue("sip", "active_recorddevice", comboBoxRecord.SelectedItem.ToString());
                active_recorddevice = comboBoxRecord.SelectedItem.ToString();
            }

            //active_networkinterface
            string active_networkinterface = "Auto";
            if (comboBoxNetwork.SelectedIndex > -1)
            {
                ini.IniWriteValue("sip", "active_networkinterface", comboBoxNetwork.SelectedItem.ToString());
                active_networkinterface = comboBoxNetwork.SelectedItem.ToString();

            }
            else if (!comboBoxNetwork.Text.Trim().Equals(""))
            {
                ini.IniWriteValue("sip", "active_networkinterface", comboBoxNetwork.Text.Trim());
                active_networkinterface = comboBoxNetwork.Text.Trim();

            }

            //active_videodevice
            string active_videodevice = "";
            if (comboBoxVideo.SelectedIndex > -1)
            {
                ini.IniWriteValue("sip", "active_videodevice", comboBoxVideo.SelectedItem.ToString());
                active_videodevice = comboBoxVideo.SelectedItem.ToString();

            }

            //encrypted_call
            int encrypted_call = 0;
            if (1==2) //不加密
            {
                ini.IniWriteValue("sip", "encrypted_call", "1");
                encrypted_call = 1;
            }
            else
            {
                ini.IniWriteValue("sip", "encrypted_call", "0");
                encrypted_call = 0;
            }

            //video_call
            int video_call = 1;
            if (1==1)  //允许视频通话
                ini.IniWriteValue("sip", "video_call", "1");
            else
            {
                ini.IniWriteValue("sip", "video_call", "0");
                video_call = 0;
            }


            //      
            
            this.DialogResult = DialogResult.OK;

        }

        private void MySysConfig_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            labelPlatformVer.Text = Application.ProductVersion;            

            //读取配置文件
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, PARAFILE);
            //指定ini文件的路径
            IniFile ini = new IniFile(path);
            string str = "";

            //2018.1.25 地图默认显示位置，第一级            
            List<RegionChinaDto> list=null;
            RegionChinaResponse resp= PocClient.getRegionChina(0);
            if (resp != null && resp.data != null && resp.data.Count > 0)
                list = resp.data;

            cbMapProvince.SelectedIndexChanged -= cbMapProvince_SelectedIndexChanged;
            cbMapProvince.Items.Clear();
           
            foreach (RegionChinaDto reg in list)
            {
                cbMapProvince.Items.Add(new MyKeyValue(reg.code.ToString(),reg.myname.ToString()));
            }           

            this.cbMapProvince.DisplayMember = "pValue";
            this.cbMapProvince.ValueMember = "pKey";            

            this.cbMapCity.DisplayMember = "pValue";
            this.cbMapCity.ValueMember = "pKey";            

            this.cbMapRegion.DisplayMember = "pValue";
            this.cbMapRegion.ValueMember = "pKey";          

            //           


            //查本地设置:省份
            if (ini.IniReadValue("other", "map_province_code").Trim().Equals(""))                           
            {
                cbMapProvince.SelectedIndex = -1;                

            } else
            {
                str = ini.IniReadValue("other", "map_province_code").Trim();
                
                for (int i= 0; i < cbMapProvince.Items.Count;i++ )
                {
                    if (((MyKeyValue)cbMapProvince.Items[i]).pKey== str)
                    {
                        this.cbMapProvince.SelectedIndex = i;
                        break;
                    }
                }

                
                resp = PocClient.getRegionChina(Convert.ToInt32(str));
                if (resp != null && resp.data != null && resp.data.Count > 0)
                    list = resp.data;

                //
                cbMapCity.SelectedIndexChanged -= cbMapCity_SelectedIndexChanged;
                cbMapCity.Items.Clear();
                foreach (RegionChinaDto reg in list)
                {
                    cbMapCity.Items.Add(new MyKeyValue(reg.code.ToString(), reg.myname.ToString()));
                }                

                //再查有无市级
                str = ini.IniReadValue("other", "map_city_code").Trim();
                if (!str.Equals(""))
                {
                    for (int i = 0; i < cbMapCity.Items.Count; i++)
                    {
                        if (((MyKeyValue)cbMapCity.Items[i]).pKey == str)
                        {
                            this.cbMapCity.SelectedIndex = i;
                            break;
                        }
                    }
                    //再查有无区/县级                   
                    resp = PocClient.getRegionChina(Convert.ToInt32(str));
                    if (resp != null && resp.data != null && resp.data.Count > 0)
                        list = resp.data;

                    cbMapRegion.Items.Clear();
                    foreach (RegionChinaDto reg in list)
                    {
                        cbMapRegion.Items.Add(new MyKeyValue(reg.code.ToString(), reg.myname.ToString()));
                    }
                    str = ini.IniReadValue("other", "map_region_code").Trim();
                    if (!str.Equals(""))
                    {
                        for (int i = 0; i < cbMapRegion.Items.Count; i++)
                        {
                            if (((MyKeyValue)cbMapRegion.Items[i]).pKey == str)
                            {
                                this.cbMapRegion.SelectedIndex = i;
                                break;
                            }
                        }

                    } else
                    {
                        this.cbMapRegion.SelectedIndex = -1;
                    }

                } else
                {
                    this.cbMapCity.SelectedIndex = -1;                    
                }

            }

            cbMapProvince.SelectedIndexChanged += cbMapProvince_SelectedIndexChanged;
            cbMapCity.SelectedIndexChanged += cbMapCity_SelectedIndexChanged;
            //*****************************
            if (ini.IniReadValue("maploc", "autoenable").Equals(""))
                cbMapLocAutoUpdate.Checked = false;
            else
            {
                if (ini.IniReadValue("maploc", "autoenable").Equals("Y"))
                    cbMapLocAutoUpdate.Checked = true;
                else
                    cbMapLocAutoUpdate.Checked = false;

            }
            //导航模式
            if (ini.IniReadValue("other", "navigate").Equals(""))
                cbTVMode.SelectedIndex =0;
            else
            {
                if (ini.IniReadValue("other", "navigate").Equals("talkgroup"))
                    cbTVMode.SelectedIndex = 0;
                else
                    cbTVMode.SelectedIndex = 1;

            }

            //视频监控分辨率
            if (ini.IniReadValue("video", "moni_resolv").Equals(""))
                cbMoniResolv.SelectedIndex = 2;
            else
            {
                if (ini.IniReadValue("video", "moni_resolv").Equals("1080p"))
                    cbMoniResolv.SelectedIndex = 0;
                else if (ini.IniReadValue("video", "moni_resolv").Equals("720p"))
                    cbMoniResolv.SelectedIndex =1;
                else if (ini.IniReadValue("video", "moni_resolv").Equals("480p"))
                    cbMoniResolv.SelectedIndex = 2;
                else if (ini.IniReadValue("video", "moni_resolv").Equals("360p"))
                    cbMoniResolv.SelectedIndex = 3;
                else if (ini.IniReadValue("video", "moni_resolv").Equals("240p"))
                    cbMoniResolv.SelectedIndex = 4;

            }

            //**********************
            if (ini.IniReadValue("video", "livepopup").Equals(""))
                cbVideoLivePopup.Checked = false;
            else
            {
                if (ini.IniReadValue("video", "livepopup").Equals("Y"))
                    cbVideoLivePopup.Checked = true;
                else
                    cbVideoLivePopup.Checked = false;
            }

            //短消息开关
            if (ini.IniReadValue("other", "msgpopup").Equals(""))
                cbMsgPopup.Checked = false;
            else
            {
                if (ini.IniReadValue("other", "msgpopup").Equals("Y"))
                    cbMsgPopup.Checked = true;
                else
                    cbMsgPopup.Checked = false;
            }

            //音视频设备开关
            if (ini.IniReadValue("other", "av_device_error").Equals(""))
                cbDeviceErrorPrompt.Checked = true;
            else
            {
                if (ini.IniReadValue("other", "av_device_error").Equals("Y"))
                    cbDeviceErrorPrompt.Checked = true;
                else
                    cbDeviceErrorPrompt.Checked = false;
            }

            //
            if (ini.IniReadValue("maploc", "frequency").Equals(""))
                tbInterval.Text = "20000";
            else
                tbInterval.Text = ini.IniReadValue("maploc", "frequency");

            //tbGpsOnInterval
            if (ini.IniReadValue("maploc", "gps_valid").Equals(""))
                tbGpsOnInterval.Text = "10"; //默认10分钟
            else
                tbGpsOnInterval.Text = ini.IniReadValue("maploc", "gps_valid");

            //自定义主窗口标题
            tbMainformCaption.Text = ini.IniReadValue("other", "maincaption").Trim();

            //读取用户与服务器时间差
            if (ini.IniReadValue("zone", "user_server_zoneinterval").Equals(""))
                tbZoneInterval.Text = "0";
            else
                tbZoneInterval.Text = ini.IniReadValue("zone", "user_server_zoneinterval");

            //2017.11.21 非当前组对讲
            if (ini.IniReadValue("voice", "noncurgroup").Equals(""))
                cbNonCurGrpVoice.SelectedIndex = 0;
            else
            {
                str = ini.IniReadValue("voice", "noncurgroup").Trim();
                if (str.Equals("all"))
                    cbNonCurGrpVoice.SelectedIndex =1;
                else if (str.Equals("onlycurrent"))
                    cbNonCurGrpVoice.SelectedIndex = 0;                 

            }

            //2017.10.8 
            if (ini.IniReadValue("other", "maptype").Equals(""))
                cbMapType.SelectedIndex =-1;
            else
            {
                str = ini.IniReadValue("other", "maptype").Trim();
                if (str.Equals("baidu"))
                    cbMapType.SelectedIndex = 0;
                else if (str.Equals("google"))
                    cbMapType.SelectedIndex = 1;
                else if (str.Equals("baidu_offline"))
                    cbMapType.SelectedIndex = 2;
                else if (str.Equals("mapinfo_offline"))
                    cbMapType.SelectedIndex = 3;

            }
            //语言设置
            if (ini.IniReadValue("other", "langtype").Equals(""))
                cbLangType.SelectedIndex = -1;
            else
            {
                str = ini.IniReadValue("other", "langtype").Trim();
                if (str.Equals("zh-CN"))
                    cbLangType.SelectedIndex = 0;
                else if (str.Equals("zh-TW"))
                    cbLangType.SelectedIndex = 1;
                else if (str.Equals("en-US"))
                    cbLangType.SelectedIndex = 2;
            }
            //围栏警告
            if (ini.IniReadValue("other", "fencealarmtype").Equals(""))
                cbFenceAlarmType.SelectedIndex = -1;
            else
            {
                str = ini.IniReadValue("other", "fencealarmtype").Trim();
                if (str.Equals("voice"))
                    cbFenceAlarmType.SelectedIndex = 0;
                else if (str.Equals("silent"))
                    cbFenceAlarmType.SelectedIndex = 1;               
            }

            //视频录像下载路径
            if (ini.IniReadValue("download", "pathvideo").Equals(""))
            {
                tbDownPathVideo.Text = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "poc_video");
            } else
            {
                tbDownPathVideo.Text = ini.IniReadValue("download", "pathvideo").Trim();
            }

            //语音记录下载路径
            if (ini.IniReadValue("download", "pathaudio").Equals(""))
            {
                tbDownPathAudio.Text = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "poc_audio");
            }
            else
            {
                tbDownPathAudio.Text = ini.IniReadValue("download", "pathaudio").Trim();
            }
            //抓拍路径
            if (ini.IniReadValue("download", "pathsession").Equals(""))
            {
                tbDownPathSession.Text = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "poc_session");
            }
            else
            {
                tbDownPathSession.Text = ini.IniReadValue("download", "pathsession").Trim();
            }

            //视频播放长宽比率
            if (ini.IniReadValue("video", "aspect_ratio").Equals(""))
                cbAspectRatio.SelectedIndex =0;
            else
            {
               // 16:9  4:3  1:1  16:10  2.21:1   2.35:1  2.39:1  5:4
                str = ini.IniReadValue("video", "aspect_ratio").Trim();
                if (str.Equals("16:9"))
                    cbAspectRatio.SelectedIndex = 1;
                else if (str.Equals("4:3"))
                    cbAspectRatio.SelectedIndex = 2;
                else if (str.Equals("1:1"))
                    cbAspectRatio.SelectedIndex = 3;
                else if (str.Equals("16:10"))
                    cbAspectRatio.SelectedIndex = 4;
                else if (str.Equals("2.21:1"))
                    cbAspectRatio.SelectedIndex = 5;
                else if (str.Equals("2.35:1"))
                    cbAspectRatio.SelectedIndex = 6;
                else if (str.Equals("2.39:1"))
                    cbAspectRatio.SelectedIndex = 7;
                else if (str.Equals("5:4"))
                    cbAspectRatio.SelectedIndex = 8;

            }
           
            //
            if (ini.IniReadValue("sip", "stun").Trim().Equals(""))
                comboBoxSTUN.Text = "stun.sipgate.net";
            else
            {
                comboBoxSTUN.SelectedIndex = comboBoxSTUN.Items.IndexOf("stun.sipgate.net");
            }
            //
            if (ini.IniReadValue("sip", "listen_port").Trim().Equals(""))
                sipPortBox.Text = "5060";
            else
            {
                sipPortBox.Text = ini.IniReadValue("sip", "listen_port").Trim();
            }

            //音视频设备读取
            List<string> devsAudioIn = new List<string>();

            MMDeviceEnumerator enumberator = new MMDeviceEnumerator();
            MMDeviceCollection deviceCollection = enumberator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All);
            for (int waveInDevice = 0; waveInDevice < WaveIn.DeviceCount; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                foreach (MMDevice device in deviceCollection)
                {
                    if (device.FriendlyName.StartsWith(deviceInfo.ProductName))
                    {
                        devsAudioIn.Add(device.FriendlyName);
                        break;
                    }
                }
            }
            comboBoxRecord.DataSource = devsAudioIn;            

            ///////////////////
            List<string> devsAudioOut = new List<string>();
            MMDeviceCollection mmcollect = enumberator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            for (int waveOutDevice = 0; waveOutDevice < WaveOut.DeviceCount; waveOutDevice++)
            {
                WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveOutDevice);
                foreach (MMDevice device in mmcollect)
                {
                    if (device.FriendlyName.StartsWith(deviceInfo.ProductName))
                    {
                        devsAudioOut.Add(device.FriendlyName);
                        break;
                    }
                }
            }

            comboBoxPlayback.DataSource = devsAudioOut;
           

            //摄像头设备
            List<string> devsVideoIn = new List<string>();

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);//得到机器所有接入的摄像设备
            if (videoDevices.Count != 0)//读取到摄像设备
            {
                foreach (FilterInfo device in videoDevices)
                {
                    devsVideoIn.Add(device.Name);//把摄像设备添加到摄像列表中
                }
            }

            comboBoxVideo.DataSource = devsVideoIn;
           

            //
            if (ini.IniReadValue("sip", "active_playbackdevice").Trim().Equals(""))
            {
                comboBoxPlayback.SelectedIndex = -1;
            }            
            else
            {
                comboBoxPlayback.SelectedIndex= comboBoxPlayback.Items.IndexOf(ini.IniReadValue("sip", "active_playbackdevice").Trim());
            }
            //
            if (ini.IniReadValue("sip", "active_recorddevice").Trim().Equals(""))
            {
                comboBoxRecord.SelectedIndex = -1;
            }
            else
            {
                comboBoxRecord.SelectedIndex = comboBoxRecord.Items.IndexOf(ini.IniReadValue("sip", "active_recorddevice").Trim());
            }
            //
            if (ini.IniReadValue("sip", "active_networkinterface").Trim().Equals(""))
            {
                comboBoxNetwork.SelectedIndex = -1;
            }
            else
            {
                if (comboBoxNetwork.Items.IndexOf(ini.IniReadValue("sip", "active_networkinterface").Trim())>-1)
                    comboBoxNetwork.SelectedIndex = comboBoxNetwork.Items.IndexOf(ini.IniReadValue("sip", "active_networkinterface").Trim());
                else
                {
                    comboBoxNetwork.Items.Add(ini.IniReadValue("sip", "active_networkinterface").Trim());
                    comboBoxNetwork.SelectedIndex = comboBoxNetwork.Items.IndexOf(ini.IniReadValue("sip", "active_networkinterface").Trim());

                }
            }
            //
            if (ini.IniReadValue("sip", "active_videodevice").Trim().Equals(""))
            {
                comboBoxVideo.SelectedIndex = -1;
            }
            else
            {
                comboBoxVideo.SelectedIndex = comboBoxVideo.Items.IndexOf(ini.IniReadValue("sip", "active_videodevice").Trim());
            }
            //
            if (ini.IniReadValue("sip", "encrypted_call").Trim().Equals(""))
            {
                //encryptedCallCheckBox.Checked = false;
            }
            else if (ini.IniReadValue("sip", "encrypted_call").Trim().Equals("1"))
            {
                //encryptedCallCheckBox.Checked = true;
            }
            else if (ini.IniReadValue("sip", "encrypted_call").Trim().Equals("0"))
            {
                //encryptedCallCheckBox.Checked = false;
            }
            //
            if (ini.IniReadValue("sip", "video_call").Trim().Equals(""))
            {
                //videoCallCheckBox.Checked = false;
            }
            else if (ini.IniReadValue("sip", "video_call").Trim().Equals("1"))
            {
                //videoCallCheckBox.Checked = true;
            }
            else if (ini.IniReadValue("sip", "video_call").Trim().Equals("0"))
            {
                //videoCallCheckBox.Checked = false;
            }            
            
            //SetDataToControls();

        }             

        
        

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (tbDownPathVideo.Text.Trim() != "")
                folderBrowserDialog1.SelectedPath = tbDownPathVideo.Text.Trim();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tbDownPathVideo.Text = folderBrowserDialog1.SelectedPath;

            }
            else
            {
                return;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            if (tbDownPathAudio.Text.Trim() != "")
                folderBrowserDialog1.SelectedPath = tbDownPathAudio.Text.Trim();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tbDownPathAudio.Text = folderBrowserDialog1.SelectedPath;

            }
            else
            {
                return;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            if (tbDownPathSession.Text.Trim() != "")
                folderBrowserDialog1.SelectedPath = tbDownPathSession.Text.Trim();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tbDownPathSession.Text = folderBrowserDialog1.SelectedPath;

            }
            else
            {
                return;
            }
        }      

               

        private void cbMapProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
            
            if (cbMapProvince.SelectedIndex==-1 || cbMapProvince.SelectedItem == null)
                return;

            string str = ((MyKeyValue)cbMapProvince.SelectedItem).pKey;
           // string sql_map = "select * from region_china where parent_code='"+ str 
           //     + "'  order by convert(myname using gbk) asc   ";
            //List<DapperRegionChina> list = MyDapperDAO<DapperRegionChina>.Query(sql_map, null);

            List<RegionChinaDto> list = null;
            RegionChinaResponse resp = PocClient.getRegionChina(Convert.ToInt32(str));
            if (resp != null && resp.data != null && resp.data.Count > 0)
                list = resp.data;


            cbMapCity.Items.Clear();
            cbMapRegion.Items.Clear();

            cbMapCity.SelectedIndexChanged -= cbMapCity_SelectedIndexChanged;
            foreach (RegionChinaDto reg in list)
            {
                cbMapCity.Items.Add(new MyKeyValue(reg.code.ToString(), reg.myname.ToString()));
            }           
            cbMapCity.DisplayMember = "pValue";
            cbMapCity.ValueMember = "pKey";           
            cbMapCity.SelectedIndexChanged += cbMapCity_SelectedIndexChanged;

        }

        private void cbMapCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMapCity.SelectedIndex==-1 || cbMapCity.SelectedItem == null)
                return;

            string str = ((MyKeyValue)cbMapCity.SelectedItem).pKey;

            //string sql_map = "select * from region_china where parent_code='" + str
            //    + "'  order by convert(myname using gbk) asc   ";
            //List<DapperRegionChina> list = MyDapperDAO<DapperRegionChina>.Query(sql_map, null);

            List<RegionChinaDto> list = null;
            RegionChinaResponse resp = PocClient.getRegionChina(Convert.ToInt32(str));
            if (resp != null && resp.data != null && resp.data.Count > 0)
                list = resp.data;

            cbMapRegion.Items.Clear();
            foreach (RegionChinaDto reg in list)
            {
                cbMapRegion.Items.Add(new MyKeyValue(reg.code.ToString(), reg.myname.ToString()));
            }
            cbMapRegion.DisplayMember = "pValue";
            cbMapRegion.ValueMember = "pKey";            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            //
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600);
            AutoUpdater.ReportErrors = true;
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ShowRemindLaterButton = false;

            AutoUpdater.RunUpdateAsAdmin = true; //要需要管理员身份更新

            AutoUpdater.LetUserSelectRemindLater = false;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
            AutoUpdater.RemindLaterAt = 2;

            //AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
            //设置zip解压路径
            
            AutoUpdater.DownloadPath = Application.StartupPath;
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.Forced;

            //AutoUpdater.Start("http://"+ PocClient.WEBIP + "/pc-upgrade/PlatformAutoUpdater-Zonhiyi.xml", 
            //    System.Reflection.Assembly.GetExecutingAssembly());

            AutoUpdater.Start("http://" + PocClient.WEBIP + "/pc-upgrade/PlatformAutoUpdater-Voffon.xml",
                System.Reflection.Assembly.GetExecutingAssembly());

        }

        private void AutoUpdater_ApplicationExitEvent()
        {
            Text = @"Closing application...";
            Thread.Sleep(10000);
            Application.Exit();
        }

    }
}
