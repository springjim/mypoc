using POCControlCenter.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using POCClientNetLibrary;
using NAudio.Wave;
using System.Diagnostics;
using System.Collections.Specialized;
using System.IO;
using System.Drawing.Imaging;
using System.Net; 

using System.Security.Cryptography;

using System.Runtime.InteropServices; 
using System.Security.Permissions; 
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;
using Microsoft.VisualBasic;

using System.Collections;

using System.Windows.Forms.Integration;

using POCControlCenter.Admin;
using POCControlCenter.Comm;
using System.Windows.Interop;

using POCControlCenter.Record;
using CefSharp.WinForms;
using CefSharp;
using POCControlCenter.Service;
using POCControlCenter.Service.Model;
using Newtonsoft.Json;
using POCControlCenter.Trtc;
using WebSocketSharp;
using Log = POCControlCenter.Service.Log;
using POCControlCenter.Service.Entity;
using POCControlCenter.Fence;
using POCControlCenter.Agora;
using POCControlCenter.Agora.AVLive;
using Newtonsoft.Json.Linq;
using POCControlCenter.Service.Local;
using POCControlCenter.BroadCast;
using POCControlCenter.Agora.Meeting;

namespace POCControlCenter
{

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)] //com+可见
    public partial class ControlMainForm : Form
    {
        [System.Runtime.InteropServices.ComVisibleAttribute(true)]// 将该类设置为com可访问 

        

        public MyRecordForm fullRecordForm;
        //public Cov19ScanQueryForm fullCov19ScanQueryForm;
        //video vlc控件
        private Thread VideoQueryThread;
        private Thread VideoTickThread;

        private bool gpsValidQueryActive;   // 

        private bool videoQueryActive;   //用于流的在线查询标志
        private bool videoTickActive;     //每个vlc插件的轮询,只要用于获取流信息和状态控制
        private object lockThis_video = new object();  //用于VideoQueryThread, VideoTickThread 线程间的同步
        private bool queryMoniActive = false;
        //2017.5.22 加入视频控件的参数
        public static int VIDEO_USERCONTROL_NUM_MAX = 20;
        //2018.2.8  加入IPC控件的参数
        public static int VIDEO_USERCONTROL_IPC_NUM_MAX = 6;

        public static VideoLayoutMode CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_3_2;
        private List<UserControlVideo> CURRENT_VIDEOLIST = new List<UserControlVideo>(VIDEO_USERCONTROL_NUM_MAX);  //按照最多20个来设计 

        public static VideoLayoutMode CURRENT_VIDEOLayoutMode_IPC = VideoLayoutMode.LAYOUT_2_3;
        private List<UserControlVideo> CURRENT_VIDEOLIST_IPC = new List<UserControlVideo>(VIDEO_USERCONTROL_IPC_NUM_MAX);  //IPC按照最多6个来设计


        public static string Capture_Path = "";
        private static int MONITOR_TRY_TIME = 5;
        private static int MONITOR_TRY_Count = 0;
        //默认是正常模式显示
        public static VideoLayoutScaleMode CURRENT_VideoLayoutScaleMode = VideoLayoutScaleMode.VideoLayoutScaleMode_Normal;

        private UserMoniForm moniform;

        private IList<Location> ILocation = new List<Location>();
        private IList<DayTrack> IDayTrack = new List<DayTrack>();
        //private IList<DapperUser> IDapperUser = new List<DapperUser>();

        private IList<FenceAlarmNotify> IFenceAlarm = new List<FenceAlarmNotify>();

        //当前位置的绑定对象
        private BindingList<Location> BLocation;

        //当前日期的轨迹分析对象
        private BindingList<DayTrack> BDayTrack;

        //当前电子围栏的告警对象
        private BindingList<FenceAlarmNotify> BFenceAlarm;

        //视频直播与监控的背景图统一 #37be9c 
        private Color Video_BgColor_Moni = Color.FromArgb(56, 190, 156);
        private Color Video_BgColor_Live = Color.FromArgb(56, 190, 156);
        private Color Video_SIP_BgColor = Color.FromArgb(56, 190, 156);

        private const string PARAFILE = "runpara.ini";

        //记录实时定位相关的间隔(以毫秒计)
        private int TimerUpdateLoc_Interval = 5000;

        private int GPSValidLoc_IntervalMin = 10; //GPS有效时间

        //2017.9.28 视频的直播请求是否要接收弹窗,为false表示自动确认终端发起的请求
        private bool VideoLivePopup = false;       


        //2017.8.16 用户所在时区与服务器所在时区间的差,单位小时
        public int ZoneInterval_UserServer = 0;
        //2017.10.8 
        private string FullMapType = "";
        //2017.11.7 
        private string FullMapInfo_Offline_Dir = "";  //相对目录表示
        private string FullMapInfo_Main_Process = "";  //相对目录表示
        private string FullMapInfo_Tabs_Dir = "";  //相对目录表示

        private string FullFenceAlarmType = "";
        //2017.10.12 
        private string FullVideoAspectRatio = "default";
        private bool FullMsgPopup = false;    //是否弹出消息提示框
        private string FullMainCaption = "";    //主窗口的标题自定义
        //
        private string FullNonCurrentVoice = "onlycurrent";  //非当前组的对讲设置

        //2017.10.7 用于播放轨迹查询
        public TrackPlayBackForm trackPlayBackForm;

        //2017.10.13 
        private bool SystemExit = false;
        //2017.10.14 
        private System.Threading.Timer Timer_GC;  //用于GC定时回收机制

        //2017.10.16
        private string FullReplaceWebPrex = "";        //用于下载mp4文件的前缀取消,内网就不一样
        public string FullDownloadVideoPath = "";      //录像下载本地路径
        public string FullDownloadAudioPath = "";      //语音下载本地路径
        public  string FullDownloadPicPath = "";   //直播与监控的截图存放的本地路径

        public string MAP_DEFAULT_LAT = "0";
        public string MAP_DEFAULT_LNG = "0";

        /// <summary>
        /// 纯对讲
        /// </summary>
        public static string FullTVMode_TalkGroup = "talkgroup";
        /// <summary>
        /// 组织架构
        /// </summary>
        public static string FullTVMode_ORG_TalkGroup = "org_talkgroup";

        /// <summary>
        /// 左边导航模式: 纯对讲模式(2层架构), 组织架构模式
        /// </summary>
        private string FullTVMode = "talkgroup";

        //Sunisoft.IrisSkin.SkinEngine skin = new Sunisoft.IrisSkin.SkinEngine();
        //2017.6.8 由于要用在基站上，暂时不支持用离线百度地图
        public static bool JIZHAN_ENABLE = false;         //表示是用于基站的      
        public static bool DBALONE = false;               //表示数据库是否与服务布署在一起       

        //定时轮询发送GPS定位命令
        public TreeNode Full_LoopGPSCommand_Node;
        public int Full_LoopGPSCommand_Accuracy = 0;

        //2017.7.7 电子围栏的规则
        private FenceRuleDef fenceRuleForm;

        //2017.8.11, 用于对讲单呼
        private OnePOCForm onePocForm = null;
        public static bool SINGLE_POC_ENABLE = false;  //表示是否是对讲单呼模式  

        //2023.9.23, 用于广播对讲
        private BroadCastForm broadCastForm = null; 
        public static bool BROADCAST_POC_ENABLE = false;  //表示是否是广播对讲模式

        //2023.10.3  用于临时对讲
        private TempTalkForm tempTalkForm = null;
        public static bool TEMPTALK_POC_ENABLE = false;   //表示是否是临时对讲模式,即兴创建的临时组，关闭时就解散

        /// <summary>
        /// 1、false值，表示用麦克风对讲，则maveIn要开始录音、结束录音
        /// 2、true值， 表示用文件获取语音帧并播放流，则不要求maveIn的开启录音，即全程不要录音
        /// </summary>
        public volatile static bool BROADCAST_POC_FILE_MODE_ENABLE = false; //指广播模式用文件、还是用麦克风对讲
       
       
        private Point pi;
        private Point piContact;

        //2017.8.31 
        private RecQueryForm full_RecQueryForm = null;               
             
        //
        private LocalVideoViewForm localvideoForm = null;
        private RequestGPSForm requestGPSForm = null;

        //2017.11.22 检测设备, 是否提示
        private bool AudioInputDevice_State = false;  //麦克风
        private bool AudioOutputDevice_State = false; //音箱或扬声器
        private bool VideoInputDevice_State = true;       //摄像头, 故意改为true, 因为声网SDK能检查出有无摄像头
        private bool FullAVDeviceErrorPrompt = true;      //发现上面的设备异常是否提醒

        private ArrayList allGroupComboboxItems = new ArrayList();
        private Boolean enableGroupSwitch = false;


        //记录第一次登入的群组ID, 用于系统退出时向服务器发送报文之用
        public int FirstGROUP_ID = 0;
        public string Full_RecordDeviceName = "";    //音频输入设备名称
        public int    Full_RecordDeviceIndex = 0;    //音频输入设备序号

        public string Full_PlaybackDeviceName = "";  //音箱
        public int    Full_PlaybackDeviceIndex = 0;  //音箱设备序号

        public Boolean FullKickOff = false; //是否被踢出

        //最近一次定位组或个人的ID
        /// <summary>
        /// 0 表示组级定位, 1表示个人定位
        /// </summary>
        public int Full_GPSCommand_Mode = 0;     //0 表示组级定位, 1表示个人定位

        public int Full_GPSCommand_GROUP_ID = 0;
        public int Full_GPSCommand_USER_ID = 0;
        /// <summary>
        /// 用户选择的定位精度项,用于决定组级的回调是等多少秒执行
        /// 0:自动(8秒),1:一般(5秒),2:高精(10秒)
        /// </summary>
        public int Full_GPSCommand_GROUP_LocationMode = 0;
        public VideoShareForm videoShareform;

        /// <summary>
        /// 默认显示对讲面板
        /// </summary>
        public bool Full_ShowTalkPanel = true;

        /// <summary>
        /// 默认不显示IPC面板
        /// </summary>
        public bool Full_ShowIPCPanel = false;

        /// <summary>
        /// 是否显示顶级控制面板
        /// </summary>
        public bool Full_ShowTOPPanel = true;              
                


        //agora 新的直播与监控中心的对话框, by 2022.12.05
        AVLiveCenterForm fullAVLiveCenWin = null;
        

        //Agora 视频通话
        public JoinChannelVideoView agoraVideoForm;

        //Agora 视频邀请
        public AgoraAVInviteForm avInvite;
        

        private bool _notifyNetworkDisconnect = true;
        public static ControlMainForm newInstance;

        private ChromiumWebBrowser webBrower = null;
        public RegisterJSObject registerJsObj = null;
        ///***************************************

        private WebSocket webSocket;
        private Boolean webSocket_Manual_shutdown = false; //标示是否是手动关闭的

        //定义一个WebSocket timer
        private System.Threading.Timer ws_time;

        private System.Threading.Timer ws_check_time; //检测ws_time

        //WebSocket重连次数
        static int ws_retry_count = 5;



        public class JSCALLOBJECT
        {
            public static string userid;
            public static string calltype;

            public string CallType
            {
                get { return userid; }
                set { userid = value; }
            }

            public string UserID
            {
                get { return userid; }
                set { userid = value; }
            }

            public void ClickEvent(string call, string userid)
            {
                this.CallType = call;
                this.UserID = userid;
            }
        }

        //public static string getMsg(string MsgId)
        //{
        //    ResourceManager rm = new ResourceManager("POCControlCenter.WinFormsStringResource", Assembly.GetExecutingAssembly());
        //    CultureInfo     ci = Thread.CurrentThread.CurrentCulture;
        //    return rm.GetString(MsgId, ci);
        //}       




        Dictionary<string, int> mFaceMapID = new Dictionary<string, int>();
        Dictionary<string, string> mFaceMap = new Dictionary<string, string>();

        List<string> fullEmojiList = new List<string>();

        /// <summary>
        /// 以下是解决窗口显示的闪烁问题
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void InitEmojiList()
        {

            fullEmojiList.Add("NO");
            fullEmojiList.Add("OK");
            fullEmojiList.Add("爱你");
            fullEmojiList.Add("爱情");
            fullEmojiList.Add("爱心");
            fullEmojiList.Add("傲慢");
            fullEmojiList.Add("白眼");

            fullEmojiList.Add("抱拳");
            fullEmojiList.Add("鄙视");
            fullEmojiList.Add("闭嘴");
            fullEmojiList.Add("便便");
            fullEmojiList.Add("擦汗");

            fullEmojiList.Add("菜刀");
            fullEmojiList.Add("差劲");
            fullEmojiList.Add("呲牙");
            fullEmojiList.Add("大兵");
            fullEmojiList.Add("大哭");

            fullEmojiList.Add("蛋糕");
            fullEmojiList.Add("刀");
            fullEmojiList.Add("得意");
            fullEmojiList.Add("凋谢");
            fullEmojiList.Add("发呆");

            fullEmojiList.Add("发抖");
            fullEmojiList.Add("发怒");
            fullEmojiList.Add("饭");
            fullEmojiList.Add("飞吻");
            fullEmojiList.Add("奋斗");

            fullEmojiList.Add("尴尬");
            fullEmojiList.Add("勾引");
            fullEmojiList.Add("鼓掌");
            fullEmojiList.Add("哈欠");
            fullEmojiList.Add("害羞");

            fullEmojiList.Add("憨笑");
            fullEmojiList.Add("坏笑");
            fullEmojiList.Add("挥手");
            fullEmojiList.Add("回头");
            fullEmojiList.Add("饥饿");

            fullEmojiList.Add("激动");
            fullEmojiList.Add("街舞");
            fullEmojiList.Add("惊恐");
            fullEmojiList.Add("惊讶");
            fullEmojiList.Add("咖啡");

            fullEmojiList.Add("磕头");
            fullEmojiList.Add("可爱");
            fullEmojiList.Add("可怜");
            fullEmojiList.Add("抠鼻");
            fullEmojiList.Add("骷髅");

            fullEmojiList.Add("酷");
            fullEmojiList.Add("快哭了");
            fullEmojiList.Add("困");
            fullEmojiList.Add("篮球");
            fullEmojiList.Add("冷汗");

            fullEmojiList.Add("礼物");
            fullEmojiList.Add("流汗");
            fullEmojiList.Add("流泪");
            fullEmojiList.Add("玫瑰");
            fullEmojiList.Add("美女");

            fullEmojiList.Add("难过");
            fullEmojiList.Add("怄火");
            fullEmojiList.Add("啤酒");
            fullEmojiList.Add("飘虫");
            fullEmojiList.Add("撇嘴");

            fullEmojiList.Add("乒乓球");
            fullEmojiList.Add("钱");
            fullEmojiList.Add("强");
            fullEmojiList.Add("敲打");
            fullEmojiList.Add("亲亲");

            fullEmojiList.Add("糗大了");
            fullEmojiList.Add("拳头");
            fullEmojiList.Add("弱");
            fullEmojiList.Add("色");
            fullEmojiList.Add("闪电");

            fullEmojiList.Add("胜利");
            fullEmojiList.Add("示爱");
            fullEmojiList.Add("衰");
            fullEmojiList.Add("睡");
            fullEmojiList.Add("太阳");

            fullEmojiList.Add("调皮");
            fullEmojiList.Add("跳绳");
            fullEmojiList.Add("跳跳");
            fullEmojiList.Add("偷笑");
            fullEmojiList.Add("吐");

            fullEmojiList.Add("微笑");
            fullEmojiList.Add("委屈");
            fullEmojiList.Add("握手");
            fullEmojiList.Add("西瓜");
            fullEmojiList.Add("吓");

            fullEmojiList.Add("献吻");
            fullEmojiList.Add("心碎");
            fullEmojiList.Add("嘘");
            fullEmojiList.Add("疑问");
            fullEmojiList.Add("阴险");

            fullEmojiList.Add("拥抱");
            fullEmojiList.Add("右哼哼");
            fullEmojiList.Add("右太极");
            fullEmojiList.Add("月亮");
            fullEmojiList.Add("晕");

            fullEmojiList.Add("再见");
            fullEmojiList.Add("炸弹");
            fullEmojiList.Add("折磨");
            fullEmojiList.Add("咒骂");
            fullEmojiList.Add("猪头");

            fullEmojiList.Add("抓狂");
            fullEmojiList.Add("转圈");
            fullEmojiList.Add("足球");
            fullEmojiList.Add("左哼哼");
            fullEmojiList.Add("左太极");

        }

        private void InitFaceMap()
        {
            //////////////////////////////////////////////////
            //////////////////////////////////////////////////
            //////////////////////////////////////////////////
            // TODO Auto-generated method stub
            mFaceMap.Add("[呲牙]", "f000.gif");
            mFaceMap.Add("[调皮]", "f001.gif");
            mFaceMap.Add("[流汗]", "f002.gif");
            mFaceMap.Add("[偷笑]", "f003.gif");
            mFaceMap.Add("[再见]", "f004.gif");
            mFaceMap.Add("[敲打]", "f005.gif");
            mFaceMap.Add("[擦汗]", "f006.gif");
            mFaceMap.Add("[猪头]", "f007.gif");
            mFaceMap.Add("[玫瑰]", "f008.gif");
            mFaceMap.Add("[流泪]", "f009.gif");
            mFaceMap.Add("[大哭]", "f010.gif");
            mFaceMap.Add("[嘘]", "f011.gif");
            mFaceMap.Add("[酷]", "f012.gif");
            mFaceMap.Add("[抓狂]", "f013.gif");
            mFaceMap.Add("[委屈]", "f014.gif");
            mFaceMap.Add("[便便]", "f015.gif");
            mFaceMap.Add("[炸弹]", "f016.gif");
            mFaceMap.Add("[菜刀]", "f017.gif");
            mFaceMap.Add("[可爱]", "f018.gif");
            mFaceMap.Add("[色]", "f019.gif");

            mFaceMap.Add("[害羞]", "f020.gif");
            mFaceMap.Add("[得意]", "f021.gif");
            mFaceMap.Add("[吐]", "f022.gif");
            mFaceMap.Add("[微笑]", "f023.gif");
            mFaceMap.Add("[发怒]", "f024.gif");
            mFaceMap.Add("[尴尬]", "f025.gif");
            mFaceMap.Add("[惊恐]", "f026.gif");
            mFaceMap.Add("[冷汗]", "f027.gif");
            mFaceMap.Add("[爱心]", "f028.gif");
            mFaceMap.Add("[示爱]", "f029.gif");
            mFaceMap.Add("[白眼]", "f030.gif");
            mFaceMap.Add("[傲慢]", "f031.gif");
            mFaceMap.Add("[难过]", "f032.gif");
            mFaceMap.Add("[惊讶]", "f033.gif");
            mFaceMap.Add("[疑问]", "f034.gif");
            mFaceMap.Add("[睡]", "f035.gif");
            mFaceMap.Add("[亲亲]", "f036.gif");
            mFaceMap.Add("[憨笑]", "f037.gif");
            mFaceMap.Add("[爱情]", "f038.gif");
            mFaceMap.Add("[衰]", "f039.gif");

            mFaceMap.Add("[撇嘴]", "f040.gif");
            mFaceMap.Add("[阴险]", "f041.gif");
            mFaceMap.Add("[奋斗]", "f042.gif");
            mFaceMap.Add("[发呆]", "f043.gif");
            mFaceMap.Add("[右哼哼]", "f044.gif");
            mFaceMap.Add("[拥抱]", "f045.gif");
            mFaceMap.Add("[坏笑]", "f046.gif");
            mFaceMap.Add("[飞吻]", "f047.gif");
            mFaceMap.Add("[鄙视]", "f048.gif");
            mFaceMap.Add("[晕]", "f049.gif");
            mFaceMap.Add("[大兵]", "f050.gif");
            mFaceMap.Add("[可怜]", "f051.gif");
            mFaceMap.Add("[强]", "f052.gif");
            mFaceMap.Add("[弱]", "f053.gif");
            mFaceMap.Add("[握手]", "f054.gif");
            mFaceMap.Add("[胜利]", "f055.gif");
            mFaceMap.Add("[抱拳]", "f056.gif");
            mFaceMap.Add("[凋谢]", "f057.gif");
            mFaceMap.Add("[饭]", "f058.gif");
            mFaceMap.Add("[蛋糕]", "f059.gif");

            mFaceMap.Add("[西瓜]", "f060.gif");
            mFaceMap.Add("[啤酒]", "f061.gif");
            mFaceMap.Add("[飘虫]", "f062.gif");
            mFaceMap.Add("[勾引]", "f063.gif");
            mFaceMap.Add("[OK]", "f064.gif");
            mFaceMap.Add("[爱你]", "f065.gif");
            mFaceMap.Add("[咖啡]", "f066.gif");
            mFaceMap.Add("[钱]", "f067.gif");
            mFaceMap.Add("[月亮]", "f068.gif");
            mFaceMap.Add("[美女]", "f069.gif");
            mFaceMap.Add("[刀]", "f070.gif");
            mFaceMap.Add("[发抖]", "f071.gif");
            mFaceMap.Add("[差劲]", "f072.gif");
            mFaceMap.Add("[拳头]", "f073.gif");
            mFaceMap.Add("[心碎]", "f074.gif");
            mFaceMap.Add("[太阳]", "f075.gif");
            mFaceMap.Add("[礼物]", "f076.gif");
            mFaceMap.Add("[足球]", "f077.gif");
            mFaceMap.Add("[骷髅]", "f078.gif");
            mFaceMap.Add("[挥手]", "f079.gif");

            mFaceMap.Add("[闪电]", "f080.gif");
            mFaceMap.Add("[饥饿]", "f081.gif");
            mFaceMap.Add("[困]", "f082.gif");
            mFaceMap.Add("[咒骂]", "f083.gif");
            mFaceMap.Add("[折磨]", "f084.gif");
            mFaceMap.Add("[抠鼻]", "f085.gif");
            mFaceMap.Add("[鼓掌]", "f086.gif");
            mFaceMap.Add("[糗大了]", "f087.gif");
            mFaceMap.Add("[左哼哼]", "f088.gif");
            mFaceMap.Add("[哈欠]", "f089.gif");
            mFaceMap.Add("[快哭了]", "f090.gif");
            mFaceMap.Add("[吓]", "f091.gif");
            mFaceMap.Add("[篮球]", "f092.gif");
            mFaceMap.Add("[乒乓球]", "f093.gif");
            mFaceMap.Add("[NO]", "f094.gif");
            mFaceMap.Add("[跳跳]", "f095.gif");
            mFaceMap.Add("[怄火]", "f096.gif");
            mFaceMap.Add("[转圈]", "f097.gif");
            mFaceMap.Add("[磕头]", "f098.gif");
            mFaceMap.Add("[回头]", "f099.gif");

            mFaceMap.Add("[跳绳]", "f100.gif");
            mFaceMap.Add("[激动]", "f101.gif");
            mFaceMap.Add("[街舞]", "f102.gif");
            mFaceMap.Add("[献吻]", "f103.gif");
            mFaceMap.Add("[左太极]", "f104.gif");
            mFaceMap.Add("[右太极]", "f105.gif");
            mFaceMap.Add("[闭嘴]", "f106.gif");

            //////////////////////////////////////////////////
            //////////////////////////////////////////////////
            //////////////////////////////////////////////////

            mFaceMapID.Add("f000.gif", 0);
            mFaceMapID.Add("f001.gif", 1);
            mFaceMapID.Add("f002.gif", 2);
            mFaceMapID.Add("f003.gif", 3);
            mFaceMapID.Add("f004.gif", 4);
            mFaceMapID.Add("f005.gif", 5);
            mFaceMapID.Add("f006.gif", 6);
            mFaceMapID.Add("f007.gif", 7);
            mFaceMapID.Add("f008.gif", 8);
            mFaceMapID.Add("f009.gif", 9);
            mFaceMapID.Add("f010.gif", 10);
            mFaceMapID.Add("f011.gif", 11);
            mFaceMapID.Add("f012.gif", 12);
            mFaceMapID.Add("f013.gif", 13);
            mFaceMapID.Add("f014.gif", 14);
            mFaceMapID.Add("f015.gif", 15);
            mFaceMapID.Add("f016.gif", 16);
            mFaceMapID.Add("f017.gif", 17);
            mFaceMapID.Add("f018.gif", 18);
            mFaceMapID.Add("f019.gif", 19);

            mFaceMapID.Add("f020.gif", 20);
            mFaceMapID.Add("f021.gif", 21);
            mFaceMapID.Add("f022.gif", 22);
            mFaceMapID.Add("f023.gif", 23);
            mFaceMapID.Add("f024.gif", 24);
            mFaceMapID.Add("f025.gif", 25);
            mFaceMapID.Add("f026.gif", 26);
            mFaceMapID.Add("f027.gif", 27);
            mFaceMapID.Add("f028.gif", 28);
            mFaceMapID.Add("f029.gif", 29);
            mFaceMapID.Add("f030.gif", 30);
            mFaceMapID.Add("f031.gif", 31);
            mFaceMapID.Add("f032.gif", 32);
            mFaceMapID.Add("f033.gif", 33);
            mFaceMapID.Add("f034.gif", 34);
            mFaceMapID.Add("f035.gif", 35);
            mFaceMapID.Add("f036.gif", 36);
            mFaceMapID.Add("f037.gif", 37);
            mFaceMapID.Add("f038.gif", 38);
            mFaceMapID.Add("f039.gif", 39);

            mFaceMapID.Add("f040.gif", 40);
            mFaceMapID.Add("f041.gif", 41);
            mFaceMapID.Add("f042.gif", 42);
            mFaceMapID.Add("f043.gif", 43);
            mFaceMapID.Add("f044.gif", 44);
            mFaceMapID.Add("f045.gif", 45);
            mFaceMapID.Add("f046.gif", 46);
            mFaceMapID.Add("f047.gif", 47);
            mFaceMapID.Add("f048.gif", 48);
            mFaceMapID.Add("f049.gif", 49);
            mFaceMapID.Add("f050.gif", 50);
            mFaceMapID.Add("f051.gif", 51);
            mFaceMapID.Add("f052.gif", 52);
            mFaceMapID.Add("f053.gif", 53);
            mFaceMapID.Add("f054.gif", 54);
            mFaceMapID.Add("f055.gif", 55);
            mFaceMapID.Add("f056.gif", 56);
            mFaceMapID.Add("f057.gif", 57);
            mFaceMapID.Add("f058.gif", 58);
            mFaceMapID.Add("f059.gif", 59);

            mFaceMapID.Add("f060.gif", 60);
            mFaceMapID.Add("f061.gif", 61);
            mFaceMapID.Add("f062.gif", 62);
            mFaceMapID.Add("f063.gif", 63);
            mFaceMapID.Add("f064.gif", 64);
            mFaceMapID.Add("f065.gif", 65);
            mFaceMapID.Add("f066.gif", 66);
            mFaceMapID.Add("f067.gif", 67);
            mFaceMapID.Add("f068.gif", 68);
            mFaceMapID.Add("f069.gif", 69);
            mFaceMapID.Add("f070.gif", 70);
            mFaceMapID.Add("f071.gif", 71);
            mFaceMapID.Add("f072.gif", 72);
            mFaceMapID.Add("f073.gif", 73);
            mFaceMapID.Add("f074.gif", 74);
            mFaceMapID.Add("f075.gif", 75);
            mFaceMapID.Add("f076.gif", 76);
            mFaceMapID.Add("f077.gif", 77);
            mFaceMapID.Add("f078.gif", 78);
            mFaceMapID.Add("f079.gif", 79);

            mFaceMapID.Add("f080.gif", 80);
            mFaceMapID.Add("f081.gif", 81);
            mFaceMapID.Add("f082.gif", 82);
            mFaceMapID.Add("f083.gif", 83);
            mFaceMapID.Add("f084.gif", 84);
            mFaceMapID.Add("f085.gif", 85);
            mFaceMapID.Add("f086.gif", 86);
            mFaceMapID.Add("f087.gif", 87);
            mFaceMapID.Add("f088.gif", 88);
            mFaceMapID.Add("f089.gif", 89);
            mFaceMapID.Add("f090.gif", 90);
            mFaceMapID.Add("f091.gif", 91);
            mFaceMapID.Add("f092.gif", 92);
            mFaceMapID.Add("f093.gif", 93);
            mFaceMapID.Add("f094.gif", 94);
            mFaceMapID.Add("f095.gif", 95);
            mFaceMapID.Add("f096.gif", 96);
            mFaceMapID.Add("f097.gif", 97);
            mFaceMapID.Add("f098.gif", 98);
            mFaceMapID.Add("f099.gif", 99);

            mFaceMapID.Add("f100.gif", 100);
            mFaceMapID.Add("f101.gif", 101);
            mFaceMapID.Add("f102.gif", 102);
            mFaceMapID.Add("f103.gif", 103);
            mFaceMapID.Add("f104.gif", 104);
            mFaceMapID.Add("f105.gif", 105);
            mFaceMapID.Add("f106.gif", 106);

            //////////////////////////////////////////////////
            //////////////////////////////////////////////////
            //////////////////////////////////////////////////
        }

       



        public IEnumerable<TKey> FindKeysFromValue<TKey, TValue>(IDictionary<TKey, TValue> dic, TValue value)
        {
            return dic.TakeWhile(pair => pair.Value.Equals(value)).Select(pair => pair.Key);
        }
        public String GetFaceMapKey(String value)
        {
            if (mFaceMap.ContainsValue(value))
                return mFaceMap.Single(pair => pair.Value.Equals(value)).Key;
            else
                return string.Empty;
        }
        public String GetFaceMapVal(String key)
        {
            return mFaceMap[key];
        }

        public String GetFaceMapIDKey(int faceval)
        {
            if (mFaceMapID.ContainsValue(faceval))
                return mFaceMapID.Single(pair => pair.Value.Equals(faceval)).Key;
            else
                return string.Empty;
        }
        public int GetFaceMapIDVal(String facekey)
        {
            if (mFaceMapID.ContainsKey(facekey))
                return mFaceMapID[facekey];
            else
                return (int)-1;
        }


        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern long AnimateWindow(long hwnd, long dwTime, long dwFlags);

        [System.Runtime.InteropServices.DllImport("winmm.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int PlaySound(string lpszSoundName, int hModule, int dwFlags);

        const int SND_FILENAME = 131072;
        const int SND_ALIAS = 65536;
        const int SND_SYNC = 0;
        const int SND_ASYNC = 0x0001;
        const int SND_LOOP = 0x0008;
        //
        //SND_FILENAME|SND_ASYNC |SND_LOOP
        //
        private static void PlaySound(string fileStr)
        {
            PlaySound(fileStr, 0, SND_FILENAME | SND_ASYNC | SND_LOOP);
        }
        public static void runSound(String FileName)
        {
            PlaySound(FileName, 0, SND_ASYNC | SND_FILENAME | SND_LOOP);
        }
        public static void stopSound()
        {
            PlaySound(null, 0, SND_ASYNC);
        }

        private static void PlayOne(string fileStr)
        {
            PlaySound(fileStr, 0, SND_FILENAME | SND_SYNC);
        }


        public string GetRingSoundPath()
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\RingSound\\";
            return path;
        }
        public string GetCallRingFile()
        {
            return GetRingSoundPath() + "call_incoming.wav";
        }
        public string GetCallBackRingFile()
        {
            return GetRingSoundPath() + "call_incoming.wav";
        }
        public string GetMessageRingFile()
        {
            return GetRingSoundPath() + "call_incoming.wav";
        }


        // private static readonly LogSource Log = new LogSource();
        private readonly ChatClient client;   //底层Socket操作封装的类
        private WaveIn sourceRecordStream = null;
        private void InitWaveRecord()
        {
            try
            {
                Debug.WriteLine("Full_RecordDeviceName=" + this.Full_RecordDeviceName);
                int deviceNumber = 0;
                if (this.Full_RecordDeviceName!=null && !this.Full_RecordDeviceName.Trim().Equals(""))
                {
                    String[] devArr= MMDeviceService.GetInputDevices();
                    if (devArr!=null && devArr.Length > 0)
                    {
                        for(int index=0;index<devArr.Length; index++)
                        {
                            if (devArr[index].Equals(this.Full_RecordDeviceName)){
                                deviceNumber = index;
                                break;
                            }
                        }
                    }
                }

                this.Full_RecordDeviceIndex = deviceNumber; //记住序号

                if (sourceRecordStream == null)
                {
                    sourceRecordStream = new WaveIn();
                    sourceRecordStream.DeviceNumber = deviceNumber;

                    sourceRecordStream.BufferMilliseconds = 20;

                    sourceRecordStream.WaveFormat = new WaveFormat(8000, 16, 1);
                    sourceRecordStream.DataAvailable += sourceStream_DataAvailable;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void InitWavePlay()
        {
            try
            {
                Debug.WriteLine("Full_PlaybackDeviceName=" + this.Full_PlaybackDeviceName);
                int deviceNumber = 0;
                if (this.Full_PlaybackDeviceName != null && !this.Full_PlaybackDeviceName.Trim().Equals(""))
                {
                    String[] devArr = MMDeviceService.GetOutputDevices();
                    if (devArr != null && devArr.Length > 0)
                    {
                        for (int index = 0; index < devArr.Length; index++)
                        {
                            if (devArr[index].Equals(this.Full_PlaybackDeviceName))
                            {
                                deviceNumber = index;
                                break;
                            }
                        }
                    }
                }

                this.Full_PlaybackDeviceIndex = deviceNumber;

                client.InitWavePlayDevices(deviceNumber);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ReleaseWaveRecord()
        {
            if (sourceRecordStream != null)
            {
                sourceRecordStream.StopRecording();
                sourceRecordStream.DataAvailable -= sourceStream_DataAvailable;
                sourceRecordStream.Dispose();

                sourceRecordStream = null;
            }
             
        }
        private void sourceStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<WaveInEventArgs>(sourceStream_DataAvailable), sender, e);
            }
            else
            {
                if (sourceRecordStream == null) return;
                client.RecordMediaData(e.Buffer, e.BytesRecorded);
            }
        }
        public void EndChat()
        {
            client.recordingbuffer.Reset();
            if (sourceRecordStream != null)
                sourceRecordStream.StopRecording();

            EndPOCrobMICTimer(true);
        }

        public void EndChat_forSOS()
        {
            client.recordingbuffer.Reset();
            if (sourceRecordStream != null)
                sourceRecordStream.StopRecording();

            //EndPOCrobMICTimer(true);
        }
        

        /// <summary>
        /// 1 表示普通对讲 (需要抢麦) 2 表示SOS对讲(不需抢麦，直发服务器，且语音报文又不一样)
        /// </summary>
        /// <param name="EncodeMedia_Mode"> 1 表示普通对讲 (需要抢麦) 2 表示SOS对讲(不需抢麦，直发服务器，且语音报文又不一样)</param>
        public void StartChat(int EncodeMedia_Mode, int SOS_GROUPID, int SOS_USERID)
        {
            client.EncodeMedia_Mode = EncodeMedia_Mode;
            client.SOS_GroupID = SOS_GROUPID;
            client.SOS_UserID = SOS_USERID;

            client.recordingbuffer.Reset();
            if (sourceRecordStream != null)
                sourceRecordStream.StartRecording();
            else
            {
                InitWaveRecord();
                sourceRecordStream.StartRecording();
            }

            //广播对讲模式下不用计时
            if (EncodeMedia_Mode == 1  && !BROADCAST_POC_ENABLE)
                EndPOCrobMICTimer(false);
            
        }

        public void CallPersonStart()
        {
            InitWaveRecord();

            if (client.recordingbuffer != null)
                client.recordingbuffer.Reset();
            if (sourceRecordStream != null)
                sourceRecordStream.StartRecording();
        }
        public void CallPersonEnd()
        {
            if (client.recordingbuffer != null)
                client.recordingbuffer.Reset();
            if (sourceRecordStream != null)
                sourceRecordStream.StopRecording();
        }
        

        public static ControlMainForm getSingleControlMainForm()
        {
            return newInstance;
        }

        public ControlMainForm(ChatClient cc)
        {

            newInstance = this;
            client = cc;
            
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            InitializeComponent();           
                       
        }

        //public void SetWebBrowserBAIDUMAP()
        //{
        //    WebBrowser web = this.webBrowserBAIDUMAP;
        //    web.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowserBAIDUMAP_DocumentCompleted);
        //}

        //2017.11.21 终端用户进入某组，但未说话
        private void NetUser_EnterGroup(int groupID, int userID)
        {
            if (InvokeRequired)
            {
                UpdateNetUserUIStatusDelegate uld = new UpdateNetUserUIStatusDelegate(NetUser_EnterGroup);
                this.Invoke(uld, new object[] { groupID, userID });
                return;
            }

            TreeNodeCollection gtds = this.treeViewGROUP.Nodes;
            foreach (TreeNode it in gtds)
            {
                TreeNodeCollection utds = it.Nodes;
                //判断是否当前组
                if (it.Tag is Group && ((Group)it.Tag).group_id == groupID)
                {

                    //以下是当前组各人的状态图标
                    foreach (TreeNode u in utds)
                    {
                        if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID))
                        {
                            u.ImageIndex = 3;
                            u.SelectedImageIndex = 3;

                            User myUser = LocalSharedData.UserAll[((User)u.Tag).userId];
                            if (myUser != null && myUser.lifeState != 1)
                            {
                                u.ImageIndex = 14;
                                u.SelectedImageIndex = 14;
                            }

                            break;
                        }

                    }

                }
                else
                {
                    //非当前组的                    
                    foreach (TreeNode u in utds)
                    {
                        if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID))
                        {
                            u.ImageIndex = 6;
                            u.SelectedImageIndex = 6;
                            User myUser = LocalSharedData.UserAll[((User)u.Tag).userId];
                            if (myUser != null && myUser.lifeState != 1)
                            {
                                u.ImageIndex = 14;
                                u.SelectedImageIndex = 14;
                            }
                            break;
                        }

                    }

                }

            }
            //还要通知到 BroadCastForm 窗口
            if (broadCastForm != null && !broadCastForm.IsDisposed)
            {
                //回调
                broadCastForm.UpdateUserStatus(groupID, userID, 1);
            }

            //还要通知到 TempTalkForm 窗口
            if (tempTalkForm != null && !tempTalkForm.IsDisposed)
            {
                //回调
                tempTalkForm.UpdateUserStatus(groupID, userID, 1);
            }

        }

        //2017.11.21 终端用户离开某组，但未说话
        private void NetUser_ExitGroup(int groupID, int userID)
        {
            if (InvokeRequired)
            {
                UpdateNetUserUIStatusDelegate uld = new UpdateNetUserUIStatusDelegate(NetUser_ExitGroup);
                this.Invoke(uld, new object[] { groupID, userID });
                return;
            }

            TreeNodeCollection gtds = this.treeViewGROUP.Nodes;
            foreach (TreeNode it in gtds)
            {
                TreeNodeCollection utds = it.Nodes;
                //判断是否当前组
                if (it.Tag is Group && ((Group)it.Tag).group_id == groupID)
                {
                    //以下是当前组各人的状态图标
                    foreach (TreeNode u in utds)
                    {
                        if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID))
                        {
                            //当前组讲话人
                            u.ImageIndex = 6;
                            u.SelectedImageIndex = 6;
                            break;
                        }

                    }
                    break;
                }

            }

            //还要通知到 BroadCastForm 窗口
            if (broadCastForm != null && !broadCastForm.IsDisposed)
            {
                //回调
                broadCastForm.UpdateUserStatus(groupID, userID, 0);
            }

            if (tempTalkForm != null && !tempTalkForm.IsDisposed)
            {
                //回调
                tempTalkForm.UpdateUserStatus(groupID, userID, 0);
            }

        }

        //2017.11.17 加入对讲开始，对讲结束的图标处理
        private void NetUser_TalkStart(int groupID, int userID)
        {
            if (InvokeRequired)
            {
                UpdateNetUserUIStatusDelegate uld = new UpdateNetUserUIStatusDelegate(NetUser_TalkStart);
                this.Invoke(uld, new object[] { groupID, userID });
                return;
            }

            TreeNodeCollection gtds = this.treeViewGROUP.Nodes;
            foreach (TreeNode it in gtds)
            {
                TreeNodeCollection utds = it.Nodes;
                //判断是否当前组
                if (it.Tag is Group && ((Group)it.Tag).group_id == groupID)
                {
                    //当然这里可以考虑免打扰模式，后面要加入...
                    it.ImageIndex = 1;
                    it.SelectedImageIndex = 1;
                    //以下是当前组各人的状态图标
                    foreach (TreeNode u in utds)
                    {
                        if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID))
                        {
                            //当前组讲话人
                            u.ImageIndex = 5;
                            u.SelectedImageIndex = 5;
                        }
                        else if (u.ImageIndex == 3 || u.ImageIndex == 5)
                        {
                            u.ImageIndex = 4;
                            u.SelectedImageIndex = 4;
                        }
                    }

                }
                else
                {
                    //非当前组的
                    //以下是各人的状态图标
                    foreach (TreeNode u in utds)
                    {
                        if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID))
                        {
                            //非当前组显示该人在讲话中
                            u.ImageIndex = 8;
                            u.SelectedImageIndex = 8;
                            break;
                        }

                    }

                }

            }
        }

        //2017.11.17 加入对讲开始，对讲结束的图标处理
        private void NetUser_TalkStop(int groupID, int userID)
        {
            if (InvokeRequired)
            {
                UpdateNetUserUIStatusDelegate uld = new UpdateNetUserUIStatusDelegate(NetUser_TalkStop);
                this.Invoke(uld, new object[] { groupID, userID });
                return;
            }

            TreeNodeCollection gtds = this.treeViewGROUP.Nodes;
            foreach (TreeNode it in gtds)
            {
                TreeNodeCollection utds = it.Nodes;
                //判断是否当前组
                if (it.Tag is Group && ((Group)it.Tag).group_id == groupID)
                {
                    //当然这里可以考虑免打扰模式，后面要加入...
                    it.ImageIndex = 0;
                    it.SelectedImageIndex = 0;
                    //以下是当前组各人的状态图标
                    foreach (TreeNode u in utds)
                    {
                        if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID))
                        {
                            //当前组讲话人
                            u.ImageIndex = 3;
                            u.SelectedImageIndex = 3;
                        }
                        else if (u.ImageIndex == 4)
                        {
                            u.ImageIndex = 3;
                            u.SelectedImageIndex = 3;
                        }
                    }

                }
                else
                {
                    //非当前组的
                    //以下是各人的状态图标
                    foreach (TreeNode u in utds)
                    {
                        if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID))
                        {
                            //非当前组显示该人在讲话中
                            u.ImageIndex = 6;
                            u.SelectedImageIndex = 6;
                            break;
                        }

                    }

                }

            }
        }


        private delegate void UpdateNetUserUIStatusDelegate(int groupID, int userID);
        private void NetUser_Login(int groupID, int userID)
        {
            if (InvokeRequired)
            {
                UpdateNetUserUIStatusDelegate uld = new UpdateNetUserUIStatusDelegate(NetUser_Login);
                this.Invoke(uld, new object[] { groupID, userID });
                return;
            }

            //更新状态: 一些共享的量要同步
            if (LocalSharedData.UserAll.ContainsKey(userID) && LocalSharedData.UserAll[userID] != null)
                LocalSharedData.UserAll[userID].logon = 1;

            User findUser= LocalSharedData.UserAllGroupCustomer.Find(delegate (User o) {

                return o.userId == userID;

            });
            if (findUser != null)
                findUser.logon = 1;

            //

            TreeNodeCollection gtds = this.treeViewGROUP.Nodes;
            foreach (TreeNode it in gtds)
            {
                TreeNodeCollection utds = it.Nodes;
                foreach (TreeNode u in utds)
                {
                    if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID)
                      && it.Tag is Group && Int32.Equals(groupID, ((Group)it.Tag).group_id)
                        )
                    {

                        User user;
                        LocalSharedData.UserAll.TryGetValue(((User)u.Tag).userId, out user);
                        if (user == null) continue;
                        if (u.ImageIndex != 5 && u.ImageIndex != 8 && u.ImageIndex != 4)
                        {
                            u.ImageIndex = 3;
                            u.SelectedImageIndex = 3;
                        }
                        if (user.lifeState != 1)
                        {
                            u.ImageIndex = 14;
                            u.SelectedImageIndex = 14;
                        }
                        else
                        {
                            //GPSValidLoc_IntervalMin
                            if (user.lastGpsTimeMs > 0 &&
                                (Math.Abs(Utils.getCurrentTimeMillis() - user.lastGpsTimeMs)) / (1000 * 60) < GPSValidLoc_IntervalMin)
                            {
                                u.ImageIndex = 13;
                                u.SelectedImageIndex = 13;
                            }
                        }
                    }
                    else if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID)
                      && it.Tag is Group && !Int32.Equals(groupID, ((Group)it.Tag).group_id)
                      )
                    {
                        User user;
                        LocalSharedData.UserAll.TryGetValue(((User)u.Tag).userId, out user);
                        if (user == null) continue;

                        if (u.ImageIndex != 5 && u.ImageIndex != 8)
                        {
                            u.ImageIndex = 6;
                            u.SelectedImageIndex = 6;
                        }
                        //life_state 优先级高
                        if (user.lifeState != 1)
                        {
                            u.ImageIndex = 14;
                            u.SelectedImageIndex = 14;
                        }
                        else
                        if (user.lastGpsTimeMs > 0 &&
                             (Math.Abs(Utils.getCurrentTimeMillis() - user.lastGpsTimeMs)) / (1000 * 60) < GPSValidLoc_IntervalMin)
                        {
                            u.ImageIndex = 13;
                            u.SelectedImageIndex = 13;
                        }
                    }
                }
            }

            //contact树
            gtds = this.treeViewContact.Nodes;
            foreach (TreeNode it in gtds)
            {
                TreeNodeCollection utds = it.Nodes;
                foreach (TreeNode u in utds)
                {
                    if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID))
                    {
                        u.ImageIndex = 3;
                        u.SelectedImageIndex = 3;
                    }
                }
            }

            //还要通知到 BroadCastForm 窗口
            if (broadCastForm != null && !broadCastForm.IsDisposed)
            {
                //回调
                broadCastForm.UpdateUserStatus(groupID, userID, 1);
            }

            if (tempTalkForm != null && !tempTalkForm.IsDisposed)
            {
                //回调
                tempTalkForm.UpdateUserStatus(groupID, userID, 1);
            }
        }

        private void NetUser_UploadGPS(int groupID, int userID)
        {
            if (InvokeRequired)
            {
                UpdateNetUserUIStatusDelegate uld = new UpdateNetUserUIStatusDelegate(NetUser_UploadGPS);
                this.Invoke(uld, new object[] { groupID, userID });
                return;
            }

            //更新状态            

            TreeNodeCollection gtds = this.treeViewGROUP.Nodes;
            foreach (TreeNode it in gtds)
            {
                TreeNodeCollection utds = it.Nodes;
                foreach (TreeNode u in utds)
                {
                    if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID)
                      && it.Tag is Group
                        )
                    {

                        u.ImageIndex = 13;
                        u.SelectedImageIndex = 13;
                    }

                }
            }
        }

        /// <summary>
        /// 套用下 groupID, userID ，其实可以传入任何值
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        private void NetUser_UpdateAllUserGPSValid(int groupID, int userID)
        {
            if (InvokeRequired)
            {
                UpdateNetUserUIStatusDelegate uld = new UpdateNetUserUIStatusDelegate(NetUser_UpdateAllUserGPSValid);
                this.Invoke(uld, new object[] { groupID, userID });
                return;
            }

            //更新状态            
            TreeNodeCollection gtds = this.treeViewGROUP.Nodes;
            foreach (TreeNode it in gtds)
            {
                TreeNodeCollection utds = it.Nodes;
                foreach (TreeNode u in utds)
                {
                    if (u.Tag is User && it.Tag is Group)
                    {
                        User user;
                        LocalSharedData.UserAll.TryGetValue(((User)u.Tag).userId, out user);
                        if (user == null) continue;

                        //正在讲的过程中,不要更新图标
                        if (u.ImageIndex != 5 && u.ImageIndex != 8 && u.ImageIndex != 4)
                        {
                            u.ImageIndex = 3;
                            u.SelectedImageIndex = 3;
                        }
                        if (user.lifeState != 1 && user.logon == 1)
                        {
                            u.ImageIndex = 14;
                            u.SelectedImageIndex = 14;
                        }
                        else
                        {
                            //GPSValidLoc_IntervalMin

                            if (user.logon == 1 && user.lastGpsTimeMs > 0 &&
                                (Math.Abs(Utils.getCurrentTimeMillis() - user.lastGpsTimeMs)) / (1000 * 60) < GPSValidLoc_IntervalMin)
                            {
                                u.ImageIndex = 13;
                                u.SelectedImageIndex = 13;
                            }
                            else
                            {
                                //之前的
                                if (user.logon == 1)
                                {
                                    u.ImageIndex = 6;
                                    u.SelectedImageIndex = 6;

                                }
                                else
                                {
                                    u.ImageIndex = 9;
                                    u.SelectedImageIndex = 9;
                                }
                            }
                        }
                    }

                }
            }

        }

        /// <summary>
        /// 被踢出的提示窗口
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        private void KickOff(int groupID, int userID)
        {
            //先关闭和预清理
            OnExitMainForm();
            FullKickOff = true;
            KickOffForm kickForm = new KickOffForm();
            if (kickForm.ShowDialog() == DialogResult.OK)
            {
                //
                Application.Exit();
            }
        }
        private void NetUser_Logout(int groupID, int userID)
        {
            if (InvokeRequired)
            {
                UpdateNetUserUIStatusDelegate uld = new UpdateNetUserUIStatusDelegate(NetUser_Logout);
                this.Invoke(uld, new object[] { groupID, userID });
                return;
            }


            if (LocalSharedData.UserAll.ContainsKey(userID) && LocalSharedData.UserAll[userID] != null)
                LocalSharedData.UserAll[userID].logon = 0;

            User findUser = LocalSharedData.UserAllGroupCustomer.Find(delegate (User o) {

                return o.userId == userID;

            });
            if (findUser != null)
                findUser.logon = 0;

            //group树
            TreeNodeCollection gtds = this.treeViewGROUP.Nodes;
            foreach (TreeNode it in gtds)
            {
                TreeNodeCollection utds = it.Nodes;
                foreach (TreeNode u in utds)
                {
                    if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID))
                    {
                        u.ImageIndex = 9;
                        u.SelectedImageIndex = 9;
                    }
                }
            }

            //contact树
            gtds = this.treeViewContact.Nodes;
            foreach (TreeNode it in gtds)
            {
                TreeNodeCollection utds = it.Nodes;
                foreach (TreeNode u in utds)
                {
                    if (u.Tag is User && Int32.Equals(((User)u.Tag).userId, userID))
                    {
                        u.ImageIndex = 9;
                        u.SelectedImageIndex = 9;
                    }
                }
            }

            //还要通知到 BroadCastForm 窗口
            if (broadCastForm != null && !broadCastForm.IsDisposed)
            {
                //回调
                broadCastForm.UpdateUserStatus(groupID, userID, 0);
            }

            if (tempTalkForm != null && !tempTalkForm.IsDisposed)
            {
                //回调
                tempTalkForm.UpdateUserStatus(groupID, userID, 0);
            }

        }

        private int mfirstlogonuserid = 0;
        private int mfirstlogongroupid = 0;


        //地图上框选(矩形或圆形)
        public void JSCallFunction_Map(string useridlist)
        {

            List<string> useridarr = new List<string>(useridlist.Split(','));
            CreateTmpGrpMulti crtform = new CreateTmpGrpMulti();
            crtform.clearChecklist();

            foreach (User user in LocalSharedData.UserAllGroupCustomer)
            {
                if (user.userId != LocalSharedData.CURRENTUser.user_id)
                {
                    if (useridarr.Contains(user.userId.ToString()))
                    {
                        if (user.logon == 1)
                            crtform.addChecklist_checked(Convert.ToString(user.userId), user.userName + " <" + WinFormsStringResource.StatusOnline + ">");
                        else
                            crtform.addChecklist_checked(Convert.ToString(user.userId), user.userName);
                    }
                }

            }

            if (crtform.ShowDialog() == DialogResult.OK)
            {
                
                GroupTempDto dto = new GroupTempDto();
                dto.groupName = crtform.getGrpname();
                dto.userIds = Convert.ToString(LocalSharedData.CURRENTUser.user_id) + "," + crtform.getMember();
                dto.ownerId = LocalSharedData.CURRENTUser.user_id;
                dto.priv = 1;    //调度员发起的为1, 终端发起的为0

                Group grptmp = null;
                //调用以下接口, 会发送tcp消息给socket服务, 然后socket服务会群发消息，被PC socket客户端拦截处理了

                TempGroupResponse resp_t = PocClient.createTmpGroup(dto);
                if (resp_t != null)
                {
                    grptmp = new Group();
                    grptmp.group_id = resp_t.data.groupId;
                    grptmp.group_name = resp_t.data.groupName;
                    grptmp.owner_id = resp_t.data.ownerId.Value;
                    grptmp.user_ids = resp_t.data.userIds;

                }

                if (grptmp == null)
                {
                    MessageBox.Show("你没有创建群组的权限，请联系你的企业管理员申请");
                }

                if (grptmp != null)
                {
                    //这个组id有可能已经存在了
                    var alreadyGrps = LocalSharedData.UserAllTempGROUP.Select(grp => grp.group_id == grptmp.group_id);

                    if (alreadyGrps.ToArray().Count() > 0)
                    {
                        //调用以下接口, 会发送tcp消息给socket服务, 然后socket服务会群发消息，被PC socket客户端拦截处理了
                        //MessageBox.Show("不允许重复创建,该组:" + grptmp.group_name + ",已经包括所选人员了");
                        return;
                    }


                    TreeNode it = this.treeViewGROUP.Nodes.Add("[临时]" + grptmp.group_name);

                    //it.Tag = grptmp.group_id;
                    grptmp.group_type = GroupTypeEnum.TALK_TMP;
                    it.Tag = grptmp;
                    it.ImageIndex = 1;
                    it.ToolTipText = "GID:" + grptmp.group_id;

                    List<User> us = new List<User>();
                    GroupUserMemberResponse resp = PocClient.queryTmpGroupMemberByGroupId(grptmp.group_id);
                    if (resp != null && resp.data != null && resp.data.Count > 0)
                    {
                        us = resp.data;
                    }

                    LocalSharedData.GROUPAllUser.Add(grptmp.group_id, us);
                    LocalSharedData.UserAllTempGROUP.Add(grptmp);

                    for (int j = 0; j < us.Count; ++j)
                    {
                        if (us[j].userId == LocalSharedData.CURRENTUser.user_id)
                        {
                            TreeNode user = it.Nodes.Add(us[j].userName);
                            user.Tag = us[j];
                            user.ImageIndex = 6;
                            user.SelectedImageIndex = 6;

                            user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                             
                        }
                        else
                        {
                            if (us[j].logon == 1)
                            {
                                TreeNode user = it.Nodes.Add(us[j].userName);
                                user.Tag = us[j];
                                user.ImageIndex = 6;
                                user.SelectedImageIndex = 6;

                                user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";

                                if (mfirstlogonuserid == 0 && mfirstlogongroupid == 0)
                                {
                                    mfirstlogonuserid = us[j].userId;
                                    mfirstlogongroupid = grptmp.group_id;
                                }
                            }
                            else
                            {
                                TreeNode user = it.Nodes.Add(us[j].userName);
                                user.Tag = us[j];
                                user.ImageIndex = 9;
                                user.SelectedImageIndex = 9;

                                user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                            }
                        }
                    }

                    this.treeViewGROUP.SelectedNode = it;
                    LocalSharedData.CURRENTUser.group_id = ((Group)it.Tag).group_id;
                    LocalSharedData.CURRENTGroupName = it.Text;
                    client.SendMessage(
                    (new Data()).ReportMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
                );

                    //触发图标切换
                    NetUser_Login(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id);

                    //触发 cbCurrGroup控件内容更新
                    try
                    {
                        this.enableGroupSwitch = false;
                        UpdateCbCurrGroupItems();

                    }
                    finally
                    {
                        this.enableGroupSwitch = true;
                    }                   

                }


            }

        }


        //电子围栏中，调用windows
        //由于这个方法没有被使用，所以其中DBMYSQL 暂时不改
        public void JSCallFunction_Fence(string fenceid)
        {
            /*
            int fence_id = Int32.Parse(fenceid);
           
            if (fenceRuleForm == null)
                fenceRuleForm = new FenceRuleDef();

            fenceRuleForm.full_fence_id = fence_id;
            fenceRuleForm.full_ZoneInterval_UserServer = ZoneInterval_UserServer;

            string sql = "";            
           
            sql = "select a.id,a.user_id, a.fence_id,b.fence_name,c.user_name,(case  ifnull(a.rule_type,'outcome') when 'outcome' then '出去警告' else '进入警告' end) as rule_type,(case ifnull(a.rule_assign,'include') when 'include' then '黑名单' else '白名单' end) as rule_assign ,a.start_time,a.end_time  from ptt_fence_user a inner join ptt_fence b on a.fence_id=b.fence_id "
                + "  inner join ptt_user c on a.user_id=c.user_id   where a.fence_id=" + fence_id;           
           

            daFenceRule = new MySqlDataAdapter(sql, DbHelperMySQL.connectionString);
            MySqlCommandBuilder cb_Fencerule = new MySqlCommandBuilder(daFenceRule);
            dsFenceRule = new DataSet();
            daFenceRule.Fill(dsFenceRule);
            fenceRuleForm.dataGridViewRule.AutoGenerateColumns = false;
            fenceRuleForm.dataGridViewRule.DataSource = dsFenceRule.Tables[0];

            if (fenceRuleForm.ShowDialog() == DialogResult.OK)
            {
                fenceRuleForm.Dispose();
                fenceRuleForm = null;
            }

            */

        }

        public void JSCallFunction(String call, String Msg)
        {
            int typevalue = Int32.Parse(call);
            int invitedid = Int32.Parse(Msg);
            if (LocalSharedData.CURRENTUser.user_id == invitedid) return;

            if (typevalue == 1)
            {
                PersonCallForm.CreateCaller(
                    client,
                    LocalSharedData.CURRENTUser.group_id,
                    LocalSharedData.CURRENTUser.user_id,
                    invitedid,
                    OnCallChatEventNotfiy);
            }
            else if (typevalue == 2)
            {
                PersonCallForm.CreateCaller(
                    client,
                    LocalSharedData.CURRENTUser.group_id,
                    LocalSharedData.CURRENTUser.user_id,
                    invitedid,
                    OnCallChatEventNotfiy);
            }
        }

        private static bool[] m_webBrowser = new bool[4] { false, false, false, false };


        public static string GetMD5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.GetEncoding("GB2312").GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }

            return byte2String;
        }

        private void LoadVideoControl_Max(UserControlVideo userVideo)
        {


        }

        private void LoadVideoControl_IPC_ResizeMax(UserControlVideo userVideo)
        {


        }

        private void LoadVideoControl_IPC()
        {


        }

        private void LoadVideoControl_IPC_ResizeNormal()
        {


        }

        /// <summary>
        /// 显示: 正常模式下
        /// </summary>
        private void LoadVideoControl()
        {


        }

        private delegate void SetControlVideoStateAndDisplayDelegate(int ControlVideoIndex, ControlVideoStateEnum control_video_state,
            int bind_userid);

        private delegate void SetControlVideoStateAndDisplayDelegate_IPC(int ControlVideoIndex, ControlVideoStateEnum control_video_state,
            int bind_userid, string bind_ipcname);


        private void SetControlVideoStateAndDisplay_IPC(int ControlVideoIndex, ControlVideoStateEnum control_video_state,
            int bind_userid, string bind_ipcname)
        {

        }

       
        //注册全局系统热键，要求在失去焦点下也能触发热键
        private enum MyKeys
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            Win = 8
        }
        const int WM_HOTKEY = 0x312;
        [DllImportAttribute("user32.dll", EntryPoint = "RegisterHotKey")]
        public static extern bool RegisterHotKey
         (
             IntPtr hWnd,        //要注册热键的窗口句柄
             int id,             //热键编号
             int fsModifiers,    //特殊键如：Ctrl，Alt，Shift，Window
             int vk              //一般键如：A B C F1，F2 等
         );

        [DllImportAttribute("user32.dll", EntryPoint = "UnregisterHotKey")]
        public static extern bool UnregisterHotKey
            (
                IntPtr hWnd,        //注册热键的窗口句柄
                int id              //热键编号上面注册热键的编号
            );

        /// <summary>
        /// 2023.07.28 不实现IM功能了, 这个地方与无边框的FormResize有冲突
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                switch (m.WParam.ToInt32())
                {
                    case 200:
                        //MessageBox.Show("Ctrl+ALT+B触发啦");
                        if (fullIMWin != null)
                        {
                            fullIMWin.InvokeClipWindow();
                        }
                        break;
                }
            }

            base.WndProc(ref m);
        }
        */

        //注册全局系统热键，要求在失去焦点下也能触发热键 end

        private void ControlMainForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
                this.WindowState = System.Windows.Forms.FormWindowState.Maximized;               

                RegisterHotKey(this.Handle, 200, (int)MyKeys.Ctrl | (int)MyKeys.Alt, (int)Keys.B); //注册热键Alt+B  
               

               
                Point point = new Point();
                point.X = btn_notify_sos.Location.X + (btn_notify_sos.Size.Width - btn_sos_num.Size.Width) / 2;
                point.Y = btn_notify_sos.Location.Y + btn_notify_sos.Size.Height / 2 - btn_sos_num.Size.Height - 6;
                btn_sos_num.Location = point;
                btn_sos_num.Visible = false;

                //对menustrip进行个性化绘图定制
                GRPcontextMenuStrip.Renderer = new ToolStripProfessionalRenderer(new MyMenuStripColorTable());
                CALLcontextMenuStrip.Renderer = new ToolStripProfessionalRenderer(new MyMenuStripColorTable());

                //读取所有系统设置, 应放在最前面
                InitRunPara();

                if (!FullMainCaption.Equals(""))
                    this.Text = FullMainCaption;

                //是否监听非当前组的语音
                if (FullNonCurrentVoice.Equals("onlycurrent"))
                {
                    if (client != null)
                        client.SetPlayNonCurrGroup(false);
                }
                else if (FullNonCurrentVoice.Equals("all"))
                {
                    if (client != null)
                        client.SetPlayNonCurrGroup(true);
                }
                

                //this.webBrowserBAIDUMAP.ObjectForScripting = this;//具体公开的对象,这里可以公开自定义对象            

                CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_3_2;
                CURRENT_VIDEOLayoutMode_IPC = VideoLayoutMode.LAYOUT_2_3;

                //对视频控件列表进行初始化
                if (FullVideoAspectRatio.Trim().Equals(""))
                    FullVideoAspectRatio = "default";             
                 

                this.BLocation = new BindingCollection<Location>(ILocation);                
                this.BFenceAlarm = new BindingCollection<FenceAlarmNotify>(IFenceAlarm);               
                
                BLocation.Clear();               
                BFenceAlarm.Clear();

                this.BVideoRec = new BindingCollection<VideoRec>(IVideoRec);
                BVideoRec.Clear();
                //
                this.BAudioRec = new BindingCollection<AudioRec>(IAudioRec);
                BAudioRec.Clear();
                //
                this.BPOCSessionRec = new BindingCollection<POCSessionRec>(IPOCSessionRec);
                BPOCSessionRec.Clear();

                //InitEmojiList();  //这个用于 poc <=> im

                this.labelPlayerName.Text = "";

                // Assign the ImageList to the TreeView.
                this.treeViewGROUP.ImageList = imageList1;
                this.treeViewSearch.ImageList = imageList1;
                this.treeViewContact.ImageList = imageList1;

                //2019.10.02 由于地图上的在线状态与 右树中的节点在线(较准确)一直不一致，因为来源不一样
                //现在每次改为登入时自动清空为离线状态(logon=0),然后由im 心跳自动置为1,这样较准确

                PocClient.allUserLogoutByCmpid();  //先将本cmpid下的所有用户设为下线, 然后由app端的im 心跳自动置为1,这样较准确
                PocClient.logonOrLogout(1);  //自身设为上线

                InitTreeNodes();
                //再初始化 通讯录树
                InitTreeNodes_Contact();

                //初始化后，用下面的触发选择
                UpdateCbCurrGroupItems();              


                this.toolStripStatusLab_username.Text = LocalSharedData.CURRENTUser.user_name + "(" + LocalSharedData.CURRENTUser.user_id + ")--正在登录";
                this.toolStripStatusLab_userid.Text = LocalSharedData.CURRENTUser.user_id.ToString();
                this.toolStripStatusLab_grpname.Text = LocalSharedData.CURRENTGroupName;
                this.toolStripStatusLabUsernameCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub1 + "Name：";
                this.toolStripStatusLabUserIDCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub1 + "ID：";
                this.toolStripStatusLabCurgrpCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub2 + "";


                //初始化查找多媒体设备: 麦克风、音箱、摄像头
                MainForm_Loaded_InitWave(sender, e);

                InitWaveRecord(); //初始化录音设备

                InitChatClientEvent();  //初始化通信事件回调

                client.NetInit();  //初始化网络连接

                InitWavePlay(); //初始化音箱设备

                //发上线包
                client.SendMessage(
                    (new Data()).ReportMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
                );
                //开启心跳
                client.StartHeartBeat();
                

                this.EndFenceAlarmTimer(false);


                //每2分钟垃圾回收一次
                Timer_GC = new System.Threading.Timer(TimerCallback, null, 120000, 120000);


                //线程异常的处理
                exceptionHappened = new ThreadExceptionEventHandler(ShowThreadException);                
                
                
                StartGPSValidQueryTask();  //更新定位图标的，表示最近有定位


                //*************************************************

                //最后再检查多媒体设备的异常是否要提示
                if (!this.VideoInputDevice_State || !this.AudioOutputDevice_State || !this.AudioInputDevice_State)
                {
                    if (FullAVDeviceErrorPrompt)
                    {
                        AVDeviceErrorForm avDeviceForm = new AVDeviceErrorForm(VideoInputDevice_State, AudioOutputDevice_State, AudioInputDevice_State);
                        avDeviceForm.ShowDialog();
                    }
                }

                loadCefSharpWebBrowser();

                //初始化websocket客户端连接
                initWebsocket();
                

            }
            finally
            {
                enableGroupSwitch = true;
            }


        }

        /// <summary>
        /// 返回类似于  u123?token=55085a2879d537e635741565279be7f5
        /// </summary>
        /// <returns></returns>
        private string getWsFragment()
        {
            DateTime epoch = new DateTime(1970, 1, 1); // unix 时间戳
            Int64 currTime = (Int64)(DateTime.UtcNow - epoch).TotalMilliseconds / 1000;

            String ws_userId = LocalSharedData.CURRENTUser.cmpid + "-" + Convert.ToString(currTime);

            return ws_userId + "?token=" + Utils.MD5(ws_userId + ":" + "pttpoAADJim@abb");

        }

        /// <summary>
        /// 实现了心跳，断线重连机制
        /// </summary>
        private void initWebsocket()
        {

            ws_time = new System.Threading.Timer(new System.Threading.TimerCallback(ws_reconn));

            //
            webSocket = new WebSocket("ws://" + PocClient.VIDEOIP + ":" + PocClient.NEWPort + "/ptt" + "/myws/" + getWsFragment());
            webSocket.EmitOnPing = true;
            webSocket.OnMessage += WebSocket_OnMessage;
            webSocket.OnOpen += WebSocket_OnOpen;
            webSocket.OnClose += WebSocket_OnClose;
            webSocket.OnError += WebSocket_OnError;
            webSocket.Connect();
            //
            ws_check_time = new System.Threading.Timer(new System.Threading.TimerCallback(ws_check), null, 10000, 10000);

        }

        private void ws_check(object state)
        {
            Debug.WriteLine("开始检查ws_time");

            if (webSocket!=null &&  webSocket.ReadyState== WebSocketState.Open)
            {
                webSocket.Send("heart");
            }
            else
            {
                if (webSocket != null)
                    webSocket.Close();
                webSocket = null;

                //先释放以下的定时器
                ws_time.Dispose();
                ws_check_time.Dispose();

                Thread.Sleep(2000);
                initWebsocket();
            }
        }

        private void ws_reconn(object state)
        {
            if (ws_retry_count > 0)
            {
                Console.WriteLine("");
                ws_retry_count--;
                webSocket.Connect();
            }
            else
            {
                //重连五次后，如果还没有连上，则重新创建一个
                webSocket = new WebSocket("ws://" + PocClient.VIDEOIP + ":" + PocClient.NEWPort + "/ptt" + "/myws/" + getWsFragment());
                webSocket.EmitOnPing = true;
                webSocket.OnMessage += WebSocket_OnMessage;
                webSocket.OnOpen += WebSocket_OnOpen;
                webSocket.OnClose += WebSocket_OnClose;
                webSocket.OnError += WebSocket_OnError;

                ws_retry_count = 5;
                webSocket.Connect();
            }
        }

        private void WebSocket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Console.WriteLine("WebSocket_OnError");
            Log.I("WebSocket_OnError:" + e.Message);

        }

        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("WebSocket_OnClose");
            Log.I("WebSocket_OnClose");
            if (!webSocket_Manual_shutdown)
            {
                //不是手动关闭的
                Console.WriteLine("服务器断开,开始重连");
                Log.I("服务器断开,开始重连");
                //触发一次Timer
                try
                {
                    if (ws_time != null)
                    {
                        //这里有可能 ws_time不为null，但已经释放了，所以要捕捉异常 System.ObjectDisposedException
                        ws_time.Change(1000, Timeout.Infinite);
                    }
                } catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("WebSocket_OnOpen");
            Log.I("WebSocket_OnOpen");
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsPing)
            {
                // Do something to notify that a ping has been received. 

                return;
            }
            //接收到数据
            // 这里需要回到 UI 线程进行操作，防止窗口句柄还未创建     

            SocketMessage<Object> message = JsonConvert.DeserializeObject<SocketMessage<Object>>(e.Data);
            if (message != null)
            {
                if (message.messageType.Equals("FENCE_ALARM_NOTIFY"))
                {
                    //if (message.data is Newtonsoft.Json.Linq.JObject)
                    //{


                    FenceAlarmNotifyDto dto = ((Newtonsoft.Json.Linq.JObject)(message.data)).ToObject<FenceAlarmNotifyDto>();
                    if (dto != null)
                    {
                        FenceAlarmNotifyInfoResponse resp = PocClient.queryAlarmNotifyInfo(dto.id);
                        if (resp != null)
                        {

                            showAlarmNotify(resp);
                        }

                    }
                    //}


                }
                else if (message.messageType.Equals("AV_LIVE_PUSH"))
                {
                    //直播推流
                    //MessageBox.Show(JsonConvert.SerializeObject(message));

                    LivePushDto dto = ((Newtonsoft.Json.Linq.JObject)(message.data)).ToObject<LivePushDto>();
                    showAVLiveNotify(dto);

                }


            }


        }

        //在线程中调用UI主线程方法

        //定义一个委托  MessageEntity是消息实体类
        delegate void ShowMessageCallback(FenceAlarmNotifyInfoResponse message);

        delegate void ShowMessageCallback_AVLive(LivePushDto dto);

        //委托方法
        private void ShowMessageAVLive(LivePushDto dto)
        {
            //加入提示
            if (this.VideoLivePopup && dto!=null )
            {
                DialogResult result = MessageBox.Show("["+dto.userName+"]"+ " 发来直播,要切换到图文直播窗口吗", WinFormsStringResource.PromptStr, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (fullAVLiveCenWin == null || fullAVLiveCenWin.IsDisposed)
                    {
                        fullAVLiveCenWin =
                        new AVLiveCenterForm(client);
                    }
                    fullAVLiveCenWin.Show();


                    //1.  要检查有无在线的，要考虑去重
                    //加入所有的在线频道（直播或监控）
                    AgoraChannelActiveResponse resp = PocClient.queryAgoraChannelActive();
                    if (resp.code == 0 && resp.data.Count > 0)
                    {
                        List<AgoraChannelActiveResponse.Data> datas = resp.data;
                        foreach (AgoraChannelActiveResponse.Data item in datas)
                        {
                            LivePushDto dto_2 = new LivePushDto();
                            dto_2.channelName = item.channelName;
                            int pos = dto_2.channelName.IndexOf("-");
                            int userId = Convert.ToInt32(dto_2.channelName.Substring(0, pos));
                            dto_2.userId = userId;
                            dto_2.userName = LocalSharedData.GetUserName(userId);

                            fullAVLiveCenWin.JoinChannel(dto_2);   //JoinChannel 方法有考虑去重的

                        }
                    }

                    //2. 加入新的
                    fullAVLiveCenWin.JoinChannel(dto);   //JoinChannel 方法有考虑去重的

                } else
                {
                    //不看
                    return;
                }
            } else
            {
                if (dto != null)
                {
                    if (fullAVLiveCenWin == null || fullAVLiveCenWin.IsDisposed)
                    {
                        fullAVLiveCenWin =
                        new AVLiveCenterForm(client);
                    }

                    fullAVLiveCenWin.Show();


                    //1.  要检查有无在线的，要考虑去重
                    //加入所有的在线频道（直播或监控）
                    AgoraChannelActiveResponse resp = PocClient.queryAgoraChannelActive();
                    if (resp.code == 0 && resp.data.Count > 0)
                    {
                        List<AgoraChannelActiveResponse.Data> datas = resp.data;
                        foreach (AgoraChannelActiveResponse.Data item in datas)
                        {
                            LivePushDto dto_2 = new LivePushDto();
                            dto_2.channelName = item.channelName;
                            int pos = dto_2.channelName.IndexOf("-");
                            int userId = Convert.ToInt32(dto_2.channelName.Substring(0, pos));
                            dto_2.userId = userId;
                            dto_2.userName = LocalSharedData.GetUserName(userId);

                            fullAVLiveCenWin.JoinChannel(dto_2);   //JoinChannel 方法有考虑去重的

                        }
                    }

                    //2. 加入新的
                    fullAVLiveCenWin.JoinChannel(dto);   //JoinChannel 方法有考虑去重的

                }
            }
           
        }

        //委托方法
        private void ShowMessageSmall(FenceAlarmNotifyInfoResponse message)
        {
            //右下角消息框Form
            AlarmMessageForm msgBox = new AlarmMessageForm();

            msgBox.labAlarmTypeName.Text = message.data.alarmTypeName;
            msgBox.labFenceName.Text = message.data.fenceName;
            msgBox.labNotifyTimeStr.Text = message.data.notifyTimeStr;
            msgBox.labStayTimeMin.Text = message.data.stayTimeMin + "(分钟)";
            msgBox.labUserName.Text = message.data.userName;

            msgBox.fenceAlarmNotifyId = message.data.id;
            msgBox.fencePoints = message.data.fencePoints;
            msgBox.endLongitude = Convert.ToString(message.data.endLongitude);
            msgBox.endLatitude = Convert.ToString(message.data.endLatitude);

            msgBox.setText(60000);

            msgBox.Show(ControlMainForm.getSingleControlMainForm());

        }

        //调用委托
        private void showAlarmNotify(FenceAlarmNotifyInfoResponse message)
        {

            //创建委托
            ShowMessageCallback wt = new ShowMessageCallback(ShowMessageSmall);
            //这段代码在主窗体类里面写着，this指主窗体
            this.BeginInvoke(wt, new Object[] { message });
            //BeginInvoke(wt);
        }


        //调用委托
        private void showAVLiveNotify(LivePushDto message)
        {

            //创建委托
            ShowMessageCallback_AVLive wt = new ShowMessageCallback_AVLive(ShowMessageAVLive);
            //这段代码在主窗体类里面写着，this指主窗体
            this.BeginInvoke(wt, new Object[] { message });
            //BeginInvoke(wt);
        }


        public void loadCefSharpWebBrowser()
        {

            CefSharpSettings.LegacyJavascriptBindingEnabled = true;//新cefsharp绑定需要优先申明

            if (!JIZHAN_ENABLE && !FullMapType.Equals("baidu_offline") && !FullMapType.Equals("google_offline") && !FullMapType.Equals("mapinfo_offline"))
            {
                //加载定位地图
                try
                {
                    //this.tabPageMapInfo.Parent = null;
                    webBrower = new ChromiumWebBrowser(HttpAPI.UriLocationMAP + WinFormsStringResource.LocationBaiduMAP);

                    registerJsObj = new RegisterJSObject(this);
                    webBrower.RegisterJsObject("MainForm", registerJsObj, new CefSharp.BindingOptions() { CamelCaseJavascriptNames = false });
                    webBrower.IsBrowserInitializedChanged += WebBrower_IsBrowserInitializedChanged;
                    webBrower.LoadError += WebBrower_LoadError;
                    this.panel17.Controls.Add(webBrower);
                    webBrower.Dock = DockStyle.Fill;// 填充方式

                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }


            }
            else if (JIZHAN_ENABLE || FullMapType.Equals("baidu_offline") || FullMapType.Equals("google_offline"))
            {
                //2017.10.8 启用离线               
                //加载定位地图
                try
                {
                    //this.tabPageMapInfo.Parent = null;
                    webBrower = new ChromiumWebBrowser(System.Environment.CurrentDirectory
                        + "/" + HttpAPI.UriLocationMAPOffline_Dir + "/" + HttpAPI.UriLocationMAPOffline_MapFile);
                    webBrower.Dock = DockStyle.Fill;// 填充方式

                    CefSharpSettings.LegacyJavascriptBindingEnabled = true;//新cefsharp绑定需要优先申明
                    registerJsObj = new RegisterJSObject(this);
                    webBrower.RegisterJsObject("MainForm", registerJsObj, new CefSharp.BindingOptions() { CamelCaseJavascriptNames = false });
                    webBrower.IsBrowserInitializedChanged += WebBrower_IsBrowserInitializedChanged;
                    webBrower.LoadError += WebBrower_LoadError;
                    this.panel17.Controls.Add(webBrower);

                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }

            }
            else if (FullMapType.Equals("mapinfo_offline"))
            {

                //加载mapinfo离线地图,所以也适用于基站环境
                string mapinfo_exe_file = Path.Combine(System.Environment.CurrentDirectory, FullMapInfo_Offline_Dir, FullMapInfo_Main_Process);
                if (!File.Exists(mapinfo_exe_file))
                {
                    MessageBox.Show("你选择的mapinfo离线地图没有发现资源");
                }
                else
                {

                }

            }


        }

        private void WebBrower_LoadError(object sender, LoadErrorEventArgs e)
        {
            MessageBox.Show("脚本错误，" + e.ErrorText);
            //e.Handled = true;
        }

        private void WebBrower_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs args)
        {
            //加载调用 js方法          


            //Thread.Sleep(10000); //同步延迟下执行
            if (webBrower != null)
            {
                //webBrower.Refresh();
                //ControlMainForm_ShowGroupLocation(LocalSharedData.CURRENTUser.group_id, webBrower);
            }


            Task.Delay(20000).ContinueWith((a) =>
            {

                if (webBrower != null)
                    ControlMainForm_ShowGroupLocation(LocalSharedData.CURRENTUser.group_id, webBrower);
            });


            if (mfirstlogonuserid != 0 && mfirstlogongroupid != 0)
            {

            }

        }

        public void JSCallFunction_PersonTrackPlayBack(string userid, string username)
        {
            //MessageBox.Show("JSCallFunction_PersonTrackPlayBack(string userid)");
            string url = HttpAPI.UriLocationMAP + WinFormsStringResource.LocationTrackMAP +
                        "?user_id=" + userid +
                        "&group_id=" + LocalSharedData.CURRENTUser.group_id.ToString() +
                        "&query_uid=" + userid + "&time=2&mapdefault_lng=" + MAP_DEFAULT_LNG +
                        "&mapdefault_lat=" + MAP_DEFAULT_LAT;

            if (trackPlayBackForm == null)
            {
                trackPlayBackForm = new TrackPlayBackForm(url);
            }

            trackPlayBackForm.userid = LocalSharedData.CURRENTUser.user_id;
            trackPlayBackForm.groupid = LocalSharedData.CURRENTUser.group_id;
            trackPlayBackForm.ZoneInterval_UserServer = ZoneInterval_UserServer;
            trackPlayBackForm.query_userid = Convert.ToInt32(userid);
            //
            trackPlayBackForm.lng = MAP_DEFAULT_LNG;
            trackPlayBackForm.lat = MAP_DEFAULT_LAT;
            trackPlayBackForm.url = url;




            trackPlayBackForm.Text = username + ":轨迹查询";


            trackPlayBackForm.ShowDialog();
            trackPlayBackForm = null;

        }

        public void JSCallFunction_PersonAudioCall(string userid, string username)
        {

            //2017.11.20 更新为视频通话、语音通话           

            //MessageBox.Show("JSCallFunction_PersonAudioCall(string userid)");
            //判断是否是自己
            if (LocalSharedData.CURRENTUser.user_id.ToString().Equals(userid))
            {
                System.Windows.Forms.MessageBox.Show("自己不能与自己对讲");
                return;
            }


            //string grpcaption = username;  //对方的名称
            //string userids = userid + "," + LocalSharedData.CURRENTUser.user_id; //双方的userid串接
            //executeSinglePOC(grpcaption, userids);

            User user = LocalSharedData.UserAll[Convert.ToInt32(userid)];
            if (user == null)
            {
                System.Windows.Forms.MessageBox.Show("系统内部错误,没有发现用户的缓存: " + userid);
                return;
            }

            System.Threading.ThreadPool.QueueUserWorkItem((obj) =>
            {
                //String userid = userid;
                if (userid == "")
                {
                    System.Windows.Forms.MessageBox.Show("用户节点分析报错");
                    return;
                }
                //string username = LocalSharedData.GetUserName(Convert.ToInt32(userid));

                string username2 = LocalSharedData.GetUserName(Convert.ToInt32(userid));


                if (agoraVideoForm == null || agoraVideoForm.IsDisposed)
                {
                    agoraVideoForm =
                                        new JoinChannelVideoView(client, LocalSharedData.CURRENTUser.user_id + "",
                                        LocalSharedData.CURRENTUser.user_id + "", LocalSharedData.CURRENTUser.user_name,
                                        userid, username, "desc",

                                        new Action(delegate { }), false, (short)1);
                }

                agoraVideoForm.ShowDialog();
                agoraVideoForm = null;



            });




        }

        public void JSCallFunction_PersonVideoCall(string userid, string username)
        {
            //MessageBox.Show("JSCallFunction_PersonVideoCall(string userid)");
            //判断是否是自己
            if (LocalSharedData.CURRENTUser.user_id.ToString().Equals(userid))
            {
                System.Windows.Forms.MessageBox.Show("自己不能与自己视频监控");
                return;
            }


            User user = LocalSharedData.UserAll[Convert.ToInt32(userid)];
            if (user == null)
            {
                System.Windows.Forms.MessageBox.Show("系统内部错误,没有发现用户的缓存: " + userid);
                return;
            }

            System.Threading.ThreadPool.QueueUserWorkItem((obj) =>
            {
                //String userid = userid;
                if (userid == "")
                {
                    System.Windows.Forms.MessageBox.Show("用户节点分析报错");
                    return;
                }

                string username2 = LocalSharedData.GetUserName(Convert.ToInt32(userid));


                if (agoraVideoForm == null || agoraVideoForm.IsDisposed)
                {
                    agoraVideoForm =
                                        new JoinChannelVideoView(client, LocalSharedData.CURRENTUser.user_id + "",
                                        LocalSharedData.CURRENTUser.user_id + "", LocalSharedData.CURRENTUser.user_name,
                                        userid, username, "desc",

                                        new Action(delegate { }), false, (short)2);
                }

                agoraVideoForm.ShowDialog();
                agoraVideoForm = null;



            });


        }

        public void StartGPSValidQueryTask()
        {
            Task delay = Task.Delay(2000);
            delay.ContinueWith(t =>
            {
                gpsValidQueryActive = true;

                try
                {

                    while (gpsValidQueryActive)
                    {
                        Debug.WriteLine("NetUser_UpdateAllUserGPSValid(0, 0)");
                        NetUser_UpdateAllUserGPSValid(0, 0);
                        //查询流的间隔
                        //这里子线程中的sleep不会影响主线程的卡顿, 要注意这个在服务端会有很多的log导出,所以要去注释掉
                        Thread.Sleep(10 * 1000);


                    }
                }
                catch (Exception e)
                {
                    //MessageBox.Show("StartGPSValidQueryTask:" + e.Message);
                }

            });

        }



        private string _appKey = null;     
                 

       

                   

        private static void TimerCallback(object o)
        {
            Console.WriteLine("垃圾开始回收: in TimerCallback method");
            GC.Collect();
        }       

       

        //查询在线流的线程 : 停止 
        public void StopVideoQuery()
        {
            videoQueryActive = false;
            if (VideoQueryThread != null)
                VideoQueryThread.Interrupt();
        }

        //查询在线流的线程  
        public void StartVideoQuery()
        {
            VideoQueryThread = new Thread(doVideoQuery) { Priority = ThreadPriority.Normal };
            videoQueryActive = true;
            VideoQueryThread.Start();

        }

        public void StopVideoTick()
        {
            videoTickActive = false;
            if (VideoTickThread != null)
                VideoTickThread.Interrupt();
        }

        //用于每个video的tick分析线程
        public void StartVideoTick()
        {
            VideoTickThread = new Thread(doVideoTick) { Priority = ThreadPriority.Normal };
            videoTickActive = true;
            VideoTickThread.Start();
        }

        //VideoTickThread有异常的情况下退出的问题
        private delegate void ThreadExceptionEventHandler(Exception ex);
        private ThreadExceptionEventHandler exceptionHappened;
        private Exception exceptions;
        private void ShowThreadException(Exception ex)
        {
            this.exceptions = ex;
            //MessageBox.Show("StartVideoTick 线程ex=" + ex.Message + ",重启新的StartVideoTick");
            if (!SystemExit)
                StartVideoTick();

        }
        private void OnThreadExceptionHappened(Exception ex)
        {
            if (this.exceptionHappened != null)
            {
                exceptionHappened(ex);
            }
        }

        //VideoTickThread有异常的情况下退出的问题 end


        //每个vlc定时检查
        private void doVideoTick()
        {

        }


        private void doVideoQuery()
        {

        }

        //定义一个代理
        private delegate void DispVideoQueryDelegate();
        private delegate void DispVideoTickDelegate();
        //


        private delegate void doViewVideoDelegate(DataTable dt);
        //private delegate void doViewVideoIPCDelegate(List<DapperIPC> dt);

        private void doViewVideo(DataTable dt)
        {


        }


        //
        private void execVideoQuery()
        {

            Task.Run(() =>
            {

                Debug.WriteLine("execVideoQuery run:" + DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                DataTable dt = null;
                List<LiveMedia> medias = null;
                try
                {
                    string grp_clause = "";
                    //只检查了固定组
                    for (int i = 0; i < LocalSharedData.UserAllGROUP.Count; i++)
                    {
                        if (grp_clause.Equals(""))
                            grp_clause = " c.group_id=" + LocalSharedData.UserAllGROUP[i].group_id;
                        else
                            grp_clause = grp_clause + " or  c.group_id=" + LocalSharedData.UserAllGROUP[i].group_id;
                    }

                    if (grp_clause.Equals(""))
                        return;

                    //2017.09.14 用HTTPAPI 代替
                    //2017.11.27 改用httpapi_async
                    //medias = HttpAPI.findLiveVideo(LocalSharedData.CURRENTUser.user_id.ToString());
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("action", "execVideoQuery");
                    param.Add("userid", LocalSharedData.CURRENTUser.user_id.ToString());

                    var simpleUri = new Uri(HttpAPI.UriUserStr);
                    var newUri = simpleUri.ExtendQuery(param);
                    String strURL = newUri.AbsoluteUri;
                    RequestInfo info = new RequestInfo(strURL);
                    DataEntity.NetHttpLiveMediaData nud = null;
                    Action<ResponseInfo> act = new Action<ResponseInfo>(x =>
                    {
                        //回调对结果的处理
                        nud = SimpleJson.SimpleJson.DeserializeObject<DataEntity.NetHttpLiveMediaData>(
                                x.GetString(Encoding.UTF8));
                        if (nud.error == "0")
                        {
                            Debug.WriteLine("Action<ResponseInfo> findLiveVideo 得到结果!" + nud.data.Count);
                            medias = nud.data;
                            dt = Utils.ToDataTable(new ArrayList(medias));
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                Task.Run(() =>
                                {
                                    doViewVideo(dt);

                                }).ConfigureAwait(false);


                            }

                        }

                    });
                    HttpRequestFactory.AddRequestTask(info, act);

                    //2018.2.8 加入IPC
                    //List<DapperIPC> IPCS = MyDapperDAO<DapperIPC>.Query("select * from boss_ipc where cmpid="+LocalSharedData.CURRENTUser.cmpid,
                    //    null);
                    //if (IPCS.Count > 0)
                    //{

                    //    doViewVideoIPC(IPCS);
                    //}

                }
                catch (Exception ee)
                {
                    Debug.WriteLine("DB query Timer exception : " + ee.Message);

                }
                finally
                {
                    if (dt != null)
                    {
                        dt.Dispose();
                        Debug.WriteLine("轮询中的dt释放  ");
                    }

                    if (medias != null)
                    {
                        medias.Clear();
                        medias = null;
                        Debug.WriteLine("轮询中的medias释放  ");
                    }

                }
            }).ConfigureAwait(false);

        }





        public void JSCallFunction_PersonMsg(string userid, string username)
        {
            //MessageBox.Show("JSCallFunction_PersonMsg(string userid)");
            //必须先创建临时组,然后切换到与该人员的对讲组
            //先搜索是否已经有了该人员的单呼群组
            if (LocalSharedData.CURRENTUser.user_id.ToString().Equals(userid))
            {
                MessageBox.Show("自己不能与自己通信");
                return;
            }

            Boolean isfind = false;
            int find_groupid = 0;
            foreach (KeyValuePair<int, List<User>> pair in LocalSharedData.GROUPAllUser)
            {
                //查询组内成员只有2人包含调度员和目标人
                if (pair.Value.Count == 2 &&
                    pair.Value.Find(user => user.userId == LocalSharedData.CURRENTUser.user_id) != null
                    && pair.Value.Find(user => user.userId == Convert.ToInt32(userid)) != null)
                {
                    find_groupid = pair.Key;
                    isfind = true;
                    break;
                }
            }
            //
            if (isfind && find_groupid > 0)
            {
                DialogResult result_q = MessageBox.Show("已经存在与'" + username + "'的对讲组了,是否切换与之通信(短信或图文)",
                    WinFormsStringResource.PromptStr, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result_q == DialogResult.No)
                {
                    return;
                }
                else
                {
                    setTabControlPageDisplay(1);
                    //tabControl1.SelectedIndex = 1;
                    //this.tabPageCHANNEL.Parent = this.tabControl1;                       

                    LocalSharedData.CURRENTUser.group_id = find_groupid;
                    LocalSharedData.CURRENTGroupName = LocalSharedData.GetGroupName(find_groupid);

                    client.SendMessage(
                        (new Data()).ReportMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
                    );

                    this.toolStripStatusLab_username.Text = LocalSharedData.CURRENTUser.user_name + "(" + LocalSharedData.CURRENTUser.user_id + ")--正在登录";
                    this.toolStripStatusLab_userid.Text = LocalSharedData.CURRENTUser.user_id.ToString();
                    this.toolStripStatusLab_grpname.Text = LocalSharedData.CURRENTGroupName;
                    this.toolStripStatusLabUsernameCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub1 + "Name：";
                    this.toolStripStatusLabUserIDCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub1 + "ID：";
                    this.toolStripStatusLabCurgrpCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub2 + "";

                    //再同步 cbCurrGroup
                    for (int i = 0; i < cbCurrGroup.Items.Count; i++)
                    {
                        if (((MyKeyValue)cbCurrGroup.Items[i]).pKey.Equals(LocalSharedData.CURRENTUser.group_id.ToString()))
                        {
                            cbCurrGroup.SelectedIndex = i;
                            //显示正在与谁实时信息                           

                            break;
                        }
                    }


                }

            }
            else
            {
                //没有找到，弹出创建的对话框
                CreateTmpGrpMulti crtform = new CreateTmpGrpMulti();
                crtform.clearChecklist();

                foreach (User user in LocalSharedData.UserAllGroupCustomer)
                {
                    if (user.userId == Convert.ToInt32(userid))
                    {
                        if (user.logon == 1)
                            crtform.addChecklist_checked(Convert.ToString(user.userId), user.userName + " <" + WinFormsStringResource.StatusOnline + ">");
                        else
                            crtform.addChecklist_checked(Convert.ToString(user.userId), user.userName);

                        break;
                    }

                }
                crtform.textBoxGrpName.Text = username;

                if (crtform.ShowDialog() == DialogResult.OK)
                {          

                    GroupTempDto dto = new GroupTempDto();
                    dto.groupName = crtform.getGrpname();
                    dto.userIds = Convert.ToString(LocalSharedData.CURRENTUser.user_id) + "," + crtform.getMember();
                    dto.ownerId = LocalSharedData.CURRENTUser.user_id;
                    dto.priv = 1;    //调度员发起的为1, 终端发起的为0

                    Group grptmp = null;
                    TempGroupResponse resp_t = PocClient.createTmpGroup(dto);
                    if (resp_t != null)
                    {
                        grptmp = new Group();
                        grptmp.group_id = resp_t.data.groupId;
                        grptmp.group_name = resp_t.data.groupName;
                        grptmp.owner_id = resp_t.data.ownerId.Value;
                        grptmp.user_ids = resp_t.data.userIds;

                    }

                    if (grptmp != null)
                    {
                        //
                        TreeNode it = this.treeViewGROUP.Nodes.Add(grptmp.group_name);

                        //it.Tag = grptmp.group_id;
                        grptmp.group_type = GroupTypeEnum.TALK_TMP;
                        it.Tag = grptmp;
                        it.ImageIndex = 1;
                        it.ToolTipText = "GID:" + grptmp.group_id;

                        List<User> us = new List<User>();

                        GroupUserMemberResponse resp = PocClient.queryTmpGroupMemberByGroupId(grptmp.group_id);
                        if (resp != null && resp.data != null && resp.data.Count > 0)
                        {
                            us = resp.data;
                        }

                        LocalSharedData.GROUPAllUser.Add(grptmp.group_id, us);
                        LocalSharedData.UserAllTempGROUP.Add(grptmp);

                        for (int j = 0; j < us.Count; ++j)
                        {
                            if (us[j].userId == LocalSharedData.CURRENTUser.user_id)
                            {
                                TreeNode user = it.Nodes.Add(us[j].userName);
                                user.Tag = us[j];
                                user.ImageIndex = 2;

                                user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                                 
                            }
                            else
                            {
                                if (us[j].logon == 1)
                                {
                                    TreeNode user = it.Nodes.Add(us[j].userName);
                                    user.Tag = us[j];
                                    user.ImageIndex = 3;

                                    user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";

                                    if (mfirstlogonuserid == 0 && mfirstlogongroupid == 0)
                                    {
                                        mfirstlogonuserid = us[j].userId;
                                        mfirstlogongroupid = grptmp.group_id;
                                    }
                                }
                                else
                                {
                                    TreeNode user = it.Nodes.Add(us[j].userName);
                                    user.Tag = us[j];
                                    user.ImageIndex = 4;

                                    user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                                }
                            }
                        }

                        this.treeViewGROUP.SelectedNode = it;
                        //并自动切换到该临时组上来
                        setTabControlPageDisplay(1);
                        //tabControl1.SelectedIndex = 1;
                        //this.tabPageCHANNEL.Parent = this.tabControl1;                           

                        LocalSharedData.CURRENTUser.group_id = grptmp.group_id;
                        LocalSharedData.CURRENTGroupName = grptmp.group_name;

                        client.SendMessage(
                            (new Data()).ReportMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
                        );

                        this.toolStripStatusLab_username.Text = LocalSharedData.CURRENTUser.user_name + "(" + LocalSharedData.CURRENTUser.user_id + ")--正在登录";
                        this.toolStripStatusLab_userid.Text = LocalSharedData.CURRENTUser.user_id.ToString();
                        this.toolStripStatusLab_grpname.Text = LocalSharedData.CURRENTGroupName;
                        this.toolStripStatusLabUsernameCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub1 + "Name：";
                        this.toolStripStatusLabUserIDCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub1 + "ID：";
                        this.toolStripStatusLabCurgrpCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub2 + "";

                        //再同步 cbCurrGroup

                        for (int i = 0; i < cbCurrGroup.Items.Count; i++)
                        {
                            if (((MyKeyValue)cbCurrGroup.Items[i]).pKey.Equals(LocalSharedData.CURRENTUser.group_id.ToString()))
                            {
                                cbCurrGroup.SelectedIndex = i;
                                
                                break;
                            }
                        }
                        //
                        //触发 cbCurrGroup控件内容更新
                        try
                        {
                            this.enableGroupSwitch = false;
                            UpdateCbCurrGroupItems();

                        }
                        finally
                        {
                            this.enableGroupSwitch = true;
                        }


                    }

                }

            }

        }




        private void ControlMainForm_ShowGroupLocation(int group_id, ChromiumWebBrowser sender)
        {
            if (group_id == GroupService.EMPTY_GROUP_ID) return;

            System.Collections.Hashtable hsh = new System.Collections.Hashtable();

            if (LocalSharedData.GROUPAllUser.Count == 0) return;

            List<User> us = LocalSharedData.GROUPAllUser[group_id];

            //if (us == null) us = HttpAPI.groupUserMember(group_id);
            if (us == null)
            {
                GroupUserMemberResponse resp = PocClient.queryGroupMemberByGroupId(group_id);
                if (resp != null && resp.data != null && resp.data.Count > 0)
                    us = resp.data;
            }


            for (int j = 0; j < us.Count; ++j)
            {
                hsh.Add(us[j].userId, us[j].userName);
            }

            string strUS = SimpleJson.SimpleJson.SerializeObject(hsh, SimpleJson.SimpleJson.PocoJsonSerializerStrategy);

            //2017.7.27 改为只调一次的方法 (js,setInterval)
            //2017.10.8 增加对离线地图的支持
            try
            {
                if (webBrower != null)
                    webBrower.ExecuteScriptAsync("invokebyparam_fence",
                         LocalSharedData.CURRENTUser.user_id, group_id, strUS);

                //myinterval,userid,groupid,username
                if (JIZHAN_ENABLE || this.FullMapType.Equals("baidu_offline") || this.FullMapType.Equals("google_offline"))
                {
                    if (webBrower != null)
                        webBrower.ExecuteScriptAsync("StartLocInterval_GroupMember",
                             this.TimerUpdateLoc_Interval, LocalSharedData.CURRENTUser.user_id.ToString(), group_id.ToString(), strUS,
                        HttpAPI.UriLocationMAPOffline_FenceACTION_API_PREFIX);
                }
                else
                {
                    if (webBrower != null)
                        webBrower.ExecuteScriptAsync("StartLocInterval_GroupMember",
                        this.TimerUpdateLoc_Interval, LocalSharedData.CURRENTUser.user_id.ToString(), group_id.ToString(), strUS
                        , MAP_DEFAULT_LNG, MAP_DEFAULT_LAT);
                }

                //显示当前的定位群组
                Boolean isfind = false;
                foreach (Group grp in LocalSharedData.UserAllGROUP)
                {
                    if (grp.group_id == group_id)
                    {
                        toolStripStatusLabelCurrGrpMap.Text = grp.group_name;
                        isfind = true;
                        break;
                    }
                }
                if (!isfind)
                {
                    foreach (Group grp in LocalSharedData.UserAllTempGROUP)
                    {
                        if (grp.group_id == group_id)
                        {
                            toolStripStatusLabelCurrGrpMap.Text = grp.group_name;
                            break;
                        }
                    }
                }


            }
            catch (Exception e2)
            {

            }



        }

        private void ControlMainForm_ShowUserLocation(int user_id, int group_id)
        {

            System.Collections.Hashtable hsh = new System.Collections.Hashtable();

            if (LocalSharedData.UserAllGroupCustomer.Count == 0) return;

            String userName = "";
            List<User> us = LocalSharedData.UserAllGroupCustomer;
            User u = us.Find(delegate (User user) { return user.userId == user_id; });
            userName = u.userName;

            //2017.7.27 改为只调一次的方法 (js,setInterval)
            //2017.10.8 加入离线地图或基站的支持
            try
            {
                //myinterval,userid,groupid,username
                if (JIZHAN_ENABLE || this.FullMapType.Equals("baidu_offline") || this.FullMapType.Equals("google_offline"))
                    if (webBrower != null)
                        webBrower.ExecuteScriptAsync("StartLocInterval_GroupOne",
                    new object[] { this.TimerUpdateLoc_Interval, user_id.ToString(), group_id.ToString(), userName, HttpAPI.UriLocationMAPOffline_FenceACTION_API_PREFIX });

                    else
                        webBrower.ExecuteScriptAsync("StartLocInterval_GroupOne",
                                       new object[] { this.TimerUpdateLoc_Interval, user_id.ToString(), group_id.ToString(), userName,
                                       MAP_DEFAULT_LNG, MAP_DEFAULT_LAT });

            }
            catch (Exception e2)
            {

            }

            /*
            try
            {
                //2017.8.16 加入时区处理
                //2017.10.8 加入离线地图或基站的支持
                if (JIZHAN_ENABLE || this.FullMapType.Equals("baidu_offline") || this.FullMapType.Equals("google_offline"))

                    webBrowserMAPTRACK.Document.InvokeScript("invokebyparam",
                    new object[] { LocalSharedData.CURRENTUser.user_id, group_id, user_id, 2, this.ZoneInterval_UserServer, HttpAPI.UriLocationMAPOffline_FENCEACTION_URL_PREFIX });
                else
                    webBrowserMAPTRACK.Document.InvokeScript("invokebyparam",
                    new object[] { LocalSharedData.CURRENTUser.user_id, group_id, user_id, 2, this.ZoneInterval_UserServer });


            }
            catch (Exception e3)
            {

            }
            */

        }

        private String getGrpType(String groupid)
        {
            string result = "1";  //组类型: 1 表示固定的, 2表示临时的
            foreach (Group grp in LocalSharedData.UserAllGROUP)
            {
                if (grp.group_id == Convert.ToInt32(groupid))
                {
                    return "1";
                }
            }

            foreach (Group grp in LocalSharedData.UserAllTempGROUP)
            {
                if (grp.group_id == Convert.ToInt32(groupid))
                {
                    return "2";
                }
            }

            return result;


        }

        private void changeDatagridviewRowColor()
        {
            //2017.12.07 去除在线颜色的处理
            /*
            for (int i = 0; i < this.dataGridViewMaploc.Rows.Count - 1; i++)
            {
                if (dataGridViewMaploc.Rows[i].Cells["stateDataGridViewTextBoxColumn"].Value.ToString() == "在线")
                {
                    dataGridViewMaploc.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                }
            }
            */
        }



        /// <summary>
        /// 当前选择的模式
        /// </summary>
        /// <param name="selTreeViewMode"></param>
        private void UpdateCbCurrGroupItems()
        {
            //http://bbs.csdn.net/topics/390418578 
            BindingSource bs = new BindingSource();
            bs.DataSource = allGroupComboboxItems;
            cbCurrGroup.DataSource = bs;
            allGroupComboboxItems.Clear();

            allGroupComboboxItems.Add(new MyKeyValue(GroupService.EMPTY_GROUP_ID+"",""));

            foreach (Group grp in LocalSharedData.UserAllGROUP)
            {
                allGroupComboboxItems.Add(new MyKeyValue(grp.group_id.ToString(), grp.group_name));
            }
            foreach (Group grp in LocalSharedData.UserAllTempGROUP)
            {
                //广播组是特殊的临时组，不放入
                if (grp.group_type!= GroupTypeEnum.TALK_TMP_BROADCAST)
                    allGroupComboboxItems.Add(new MyKeyValue(grp.group_id.ToString(), grp.group_name));
            }


            this.cbCurrGroup.DisplayMember = "pValue";
            this.cbCurrGroup.ValueMember = "pKey";
            bs.ResetBindings(false);
            //再重新定义当前已选择的组            
            Boolean isfind = false;
            if (LocalSharedData.CURRENTUser.group_id > 0)
            {
                for (int i = 0; i < cbCurrGroup.Items.Count; i++)
                {
                    if (((MyKeyValue)cbCurrGroup.Items[i]).pKey.Equals(LocalSharedData.CURRENTUser.group_id.ToString()))
                    {
                        cbCurrGroup.SelectedIndex = i;

                        isfind = true;
                        break;
                    }
                }
                //
                if (!isfind && cbCurrGroup.Items.Count > 0)
                {
                    //可能当前选择的组正在被删除,那么默认选择第一个
                    cbCurrGroup.SelectedIndex = 0;

                    LocalSharedData.CURRENTUser.group_id = Convert.ToInt32(((MyKeyValue)cbCurrGroup.Items[0]).pKey);
                    LocalSharedData.CURRENTGroupName = ((MyKeyValue)cbCurrGroup.Items[0]).pValue;
                    client.SendMessage(
                    (new Data()).ReportMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
                );

                    this.toolStripStatusLab_username.Text = LocalSharedData.CURRENTUser.user_name + "(" + LocalSharedData.CURRENTUser.user_id + ")--正在登录";
                    this.toolStripStatusLab_userid.Text = LocalSharedData.CURRENTUser.user_id.ToString();
                    this.toolStripStatusLab_grpname.Text = LocalSharedData.CURRENTGroupName;
                    this.toolStripStatusLabUsernameCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub1 + "Name：";
                    this.toolStripStatusLabUserIDCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub1 + "ID：";
                    this.toolStripStatusLabCurgrpCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub2 + "";

                }

            }

        }

        private void MainForm_Loaded_InitWave(object sender, EventArgs e)
        {
            try
            {
                var outDevices = MMDeviceService.GetOutputDevices();

                var inDevices = MMDeviceService.GetInputDevices();

                if (inDevices.GetLength(0) >= 1)
                {

                    //为了与SIP的设置合并,有bug,两个deviceName不一致
                    //if (!Full_RecordDeviceName.Equals(""))
                    //    Recording.SelectedIndex = Recording.Items.IndexOf(Full_RecordDeviceName);

                    this.AudioInputDevice_State = true;  //有声音输入设备
                }
                else
                {
                    buttonTALK.Enabled = false;
                    buttonTALK.BackColor = Color.FromArgb(210, 210, 210);
                    buttonTALK.Image = global::POCControlCenter.Properties.Resources.talk_disable;
                    buttonTALK.ForeColor = Color.Gray;
                    this.AudioInputDevice_State = false;
                }

                if (outDevices != null && outDevices.Length > 0 && outDevices.GetLength(0) >= 1)
                {

                    //为了与SIP的设置合并,有bug,两个deviceName不一致
                    //if (!Full_PlaybackDeviceName.Equals(""))
                    //    Playback.SelectedIndex = Playback.Items.IndexOf(Full_PlaybackDeviceName);

                    this.AudioOutputDevice_State = true;  //有声音输出设备
                }
                else
                {
                    this.AudioOutputDevice_State = false;  //没有声音输出设备
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        /// <summary>
        /// Gets all system sound devices
        /// </summary>
        /// <returns></returns>
        public static string[] GetOutputDevices()
        {
            var enumerator = new MMDeviceEnumerator();
            //2017.11.27 修改为
            MMDeviceCollection mmcollect = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            if (mmcollect != null && mmcollect.Count > 0)
                return mmcollect.Select(endpoint => endpoint.FriendlyName).ToArray();
            else
                return new string[] { };
            //return enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).
            // Select(endpoint => endpoint.FriendlyName).ToArray();
        }
        public static string[] GetInputDevices()
        {
            var enumerator = new MMDeviceEnumerator();
            MMDeviceCollection mmcollect = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            if (mmcollect != null && mmcollect.Count > 0)
                return mmcollect.Select(endpoint => endpoint.FriendlyName).ToArray();
            else
                return new string[] { };
            //return enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).
            //     Select(endpoint => endpoint.FriendlyName).ToArray();
        }


        private bool m_blUIStalk = false;
        private bool m_blUIPlay = false;

        private void UpdatePlayUIStatus(Control ctrl, string Name)
        {

            labelPlayerName.Text = Name;

        }

        private delegate void UpdatePlayUIStatusDelegate(Control ctrl, string s);
        private void UpdateButtonTalkUIStatus(Control ctrl)
        {

            if (m_blUIStalk == false)
            {
                buttonTALK.Image = global::POCControlCenter.Properties.Resources.talk_allow;
                buttonTALK.Text = WinFormsStringResource.Main_buttonTALK_Text1;
                //#286090
                // buttonTALK.BackColor = Color.FromArgb(40, 96, 144);
                buttonTALK.BackColor = Color.FromArgb(3, 117, 232);
                buttonTALK.ForeColor = Color.White;

            }
            else
            {
                buttonTALK.Text = WinFormsStringResource.Main_buttonTALK_Text2;
                buttonTALK.Image = global::POCControlCenter.Properties.Resources.talk_allow;
                //37be9c               
                //buttonTALK.BackColor = Color.FromArgb(56, 190, 156);
                buttonTALK.BackColor = Color.FromArgb(255, 0, 0);
                buttonTALK.ForeColor = Color.White;

            }
        }
        private delegate void UpdateButtonTalkUIStatusDelegate(Control ctrl);
        private void SetPlayUIStatus(String Name)
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
        private void SetButtonTalkUIStatus()
        {
            if (buttonTALK.InvokeRequired == true)
            {
                UpdateButtonTalkUIStatusDelegate uld = new UpdateButtonTalkUIStatusDelegate(UpdateButtonTalkUIStatus);
                buttonTALK.Invoke(uld, new object[] { buttonTALK });
            }
            else
            {
                UpdateButtonTalkUIStatus(buttonTALK);
            }
        }

        public void SendSOSTalkStart(int toUserId)
        {
            client.SendMessage(new Data().SOSLocationMessageEncode(LocalSharedData.CURRENTUser.user_id, toUserId, (int)(Utils.getCurrentTimeMillis() / 1000)));
        }

        public void SendSOSTalkStop(int toUserId)
        {
            client.SendMessage(new Data().SOSReleaseMessageEncode(LocalSharedData.CURRENTUser.user_id, toUserId));
        }

        private void SendTalkCommand()
        {
            if (m_blUIStalk == false)
            {
                //发出抢麦信号
                client.SendMessage((new Data(MyType.TYPE_ROB_MIC)).ToByte());
            }
            else
            {
                EndChat();
                client.AEC_State = 0;
                client.SendMessage((new Data(MyType.TYPE_REALASE_MIC)).ToByte());

                m_blUIStalk = false;
                SetButtonTalkUIStatus();
                //释放麦,还有超时

                NetUser_TalkStop(LocalSharedData.CURRENTUser.group_id,
                    LocalSharedData.CURRENTUser.user_id);
            }
        }

        private void buttonTALK_Click(object sender, EventArgs e)
        {
            //checkBoxGroupSync
            //2023.09.09 抢麦成功后，再同步，会容易造成第一次PC讲话时,APP只听到声音，但无讲话人和状态显示
            //所以决定，不用这个反人类的强制同步功能
            SendTalkCommand();
        }

        private void InitChatClientEvent()
        {
            client.MessageReceived += ServerOnMessageEventNotfiy;
            client.ChatMessageReceived += ServerOnChatEventNotfiy;
            //client.VideoMessageReceived += ServerOnVideoMessageEventNotfiy;
            client.AVChatNewMessageReceived += ServerOnAVChatNewMessageReceived;

            client.ServerDisconnected += ServerOnClientDisconnected;
            client.ServerConnected += ServerOnClientConnected;
        }



        //2017.9.27
        //所有Video消息事件的处理
        private delegate void ServerOnVideoMessageEventDelegate(VideoMessage packet);

        private delegate void ClickVideoButtonEventDelegate();

        private void CreatefullMoniCenWinAndShow()
        {
            try
            {
                if (btn_video.InvokeRequired)
                {
                    ClickVideoButtonEventDelegate onmsgevent = new ClickVideoButtonEventDelegate(CreatefullMoniCenWinAndShow);
                    btn_video.Invoke(onmsgevent, new object[] { });
                    return;
                }
                else
                {
                    //DoServerOnVideoMessageEventDelegate(cht);
                   

                }
            }
            catch (Exception e1)
            {
                Debug.WriteLine(e1.Message);
            }

        }

        /// <summary>
        /// 处理AVChatNewMessage消息，用了声网的音视频通话
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerOnAVChatNewMessageReceived(object sender, ServerEventArgs e)
        {

            AVChatNewMessage message = e.avchat_new;

            //创建委托
            ShowAVChatMessageCallback wt = new ShowAVChatMessageCallback(ShowAVChatMessageSmall);
            //这段代码在主窗体类里面写着，this指主窗体
            this.BeginInvoke(wt, new Object[] { message });




        }

        //在线程中调用UI主线程方法
        //定义一个委托  MessageEntity是消息实体类
        delegate void ShowAVChatMessageCallback(AVChatNewMessage message);
        //委托方法
        private void ShowAVChatMessageSmall(AVChatNewMessage message)
        {

            //private short video_type;    //视频类型:  1、语音通话  2、视频通话
            //private short video_command; //视频命令， 1: 请求  2: 应答  3:拒绝  4： 主动或被动挂断 5: 对方占线中 
            if (message.video_command == 3)
            {
                if (agoraVideoForm != null && !agoraVideoForm.IsDisposed)
                {
                    agoraVideoForm.MessageNotify(message.video_command);

                }
                else
                {
                    MessageForm msgBox = new MessageForm();
                    msgBox.setText("对方 【" + message.fromUserName + "】拒绝接听", 3000);
                    msgBox.setCancelBtn(false);
                    msgBox.Show(ControlMainForm.getSingleControlMainForm());
                }


            }
            else if (message.video_command == 1)
            {
                //接收到请求
                //查询当前有在用,则可以判断当前正在音视频会话中,,,后续可以扩展成向对方发占线中...
                if (avInvite != null && !avInvite.IsDisposed)
                {
                    MessageForm msgBox = new MessageForm();
                    if (message.video_type == 1)
                        msgBox.setText("有语音通话请求： 【" + message.fromUserName + "】", 3000);
                    else
                        msgBox.setText("有视频通话请求： 【" + message.fromUserName + "】", 3000);

                    msgBox.setCancelBtn(false);
                    msgBox.Show(ControlMainForm.getSingleControlMainForm());
                    return;
                }

                if (LocalSharedData.getVideoCallRunningState())
                {
                    //向对方发送占线中
                    if (client != null)
                    {
                        client.SendMessage(
                                              (new Data()).AVChatNewCommandMessageEncode
                                              ((short)2, (short)5, LocalSharedData.CURRENTUser.user_id,
                                              Convert.ToInt32(message.fromUserId), LocalSharedData.CURRENTUser.user_name,
                                              message.fromUserName,
                                              "busy"));
                    }

                    MessageForm msgBox = new MessageForm();
                    if (message.video_type == 1)
                        msgBox.setText("有语音通话请求： 【" + message.fromUserName + "】", 3000);
                    else
                        msgBox.setText("有视频通话请求： 【" + message.fromUserName + "】", 3000);

                    msgBox.setCancelBtn(false);
                    msgBox.Show(ControlMainForm.getSingleControlMainForm());
                    return;
                }

                avInvite = new AgoraAVInviteForm(ControlMainForm.getSingleControlMainForm(), client, message.fromUserId + "",
                    message.toUserId + "", message.toUserName,
                    message.fromUserId + "", message.fromUserName, message.desc, message.video_type);

                avInvite.labelFromUserName.Text = message.fromUserName;
                avInvite.ShowDialog();

            }
            else if (message.video_command == 4)
            {
                //APP主叫,但PC未接听时, 对方挂断了
                //
                if (avInvite != null && !avInvite.IsDisposed)
                {
                    //发送消息
                    avInvite.notifyRefuseMessage();

                }
                //但是接通后，APP挂掉，完全依赖于 agora的channel的onRemoteUserLeft来处理了，这段进不了


            }
            else if (message.video_command == 5)
            {
                //对方占线
                //要发送给 JoinChannelVideoView 
                if (agoraVideoForm != null && !agoraVideoForm.IsDisposed)
                {
                    agoraVideoForm.MessageNotify(message.video_command);

                }
                else
                {
                    MessageForm msgBox = new MessageForm();
                    msgBox.setText("对方 【" + message.fromUserName + "】正在通话中...", 3000);
                    msgBox.setCancelBtn(false);
                    msgBox.Show(ControlMainForm.getSingleControlMainForm());
                }

            }


        }                    

      


        private void setTabControlPageDisplay(int index)
        {
            if (index == 0)
            {
                //改背景
                //btn_map.BackColor = Color.FromArgb(47, 64, 80);
                //btn_map.ForeColor = Color.White;
                //btn_map.Image = Properties.Resources.m_map;

                //btn_msg.BackColor = Color.Transparent;
                //btn_msg.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_msg.Image = Properties.Resources.m_message_u;

                //btn_video.BackColor = Color.Transparent;
                //btn_video.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_video.Image = Properties.Resources.m_video_u;



                //btn_rec.BackColor = Color.Transparent;
                //btn_rec.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_rec.Image = Properties.Resources.recquery_u;

                //btn_sys.BackColor = Color.Transparent;
                //btn_sys.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_sys.Image = Properties.Resources.syscfg_u;





            }
            else if (index == 1)
            {

                //this.tabPageMapInfo.Parent = null;

                //
                //btn_map.BackColor = Color.Transparent;
                //btn_map.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_map.Image = Properties.Resources.m_map_u;

                //btn_msg.BackColor = Color.FromArgb(47, 64, 80);
                //btn_msg.ForeColor = Color.White;
                //btn_msg.Image = Properties.Resources.m_message;

                //btn_video.BackColor = Color.Transparent;
                //btn_video.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_video.Image = Properties.Resources.m_video_u;


                //btn_rec.BackColor = Color.Transparent;
                //btn_rec.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_rec.Image = Properties.Resources.recquery_u;

                //btn_sys.BackColor = Color.Transparent;
                //btn_sys.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_sys.Image = Properties.Resources.syscfg_u;



            }
            else if (index == 2)
            {

                //this.tabPageMapInfo.Parent = null;


                //btn_map.BackColor = Color.Transparent;
                //btn_map.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_map.Image = Properties.Resources.m_map_u;

                //btn_msg.BackColor = Color.Transparent;
                //btn_msg.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_msg.Image = Properties.Resources.m_message_u;

                //btn_video.BackColor = Color.FromArgb(47, 64, 80);
                //btn_video.ForeColor = Color.White;
                //btn_video.Image = Properties.Resources.m_video;



                //btn_rec.BackColor = Color.Transparent;
                //btn_rec.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_rec.Image = Properties.Resources.recquery_u;

                //btn_sys.BackColor = Color.Transparent;
                //btn_sys.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_sys.Image = Properties.Resources.syscfg_u;



            }
            else if (index == 3)
            {




                //btn_map.BackColor = Color.Transparent;
                //btn_map.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_map.Image = Properties.Resources.m_map_u;

                //btn_msg.BackColor = Color.Transparent;
                //btn_msg.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_msg.Image = Properties.Resources.m_message_u;

                //btn_video.BackColor = Color.Transparent;
                //btn_video.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_video.Image = Properties.Resources.m_video_u;



                //btn_rec.BackColor = Color.Transparent;
                //btn_rec.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_rec.Image = Properties.Resources.recquery_u;

                //btn_sys.BackColor = Color.Transparent;
                //btn_sys.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_sys.Image = Properties.Resources.syscfg_u;



            }
            else if (index == 4)
            {



                //btn_map.BackColor = Color.Transparent;
                //btn_map.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_map.Image = Properties.Resources.m_map_u;

                //btn_msg.BackColor = Color.Transparent;
                //btn_msg.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_msg.Image = Properties.Resources.m_message_u;

                //btn_video.BackColor = Color.Transparent;
                //btn_video.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_video.Image = Properties.Resources.m_video_u;



                //btn_rec.BackColor = Color.Transparent;
                //btn_rec.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_rec.Image = Properties.Resources.recquery_u;

                //btn_sys.BackColor = Color.Transparent;
                //btn_sys.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_sys.Image = Properties.Resources.syscfg_u;



            }
            else if (index == 5)
            {



                //btn_map.BackColor = Color.Transparent;
                //btn_map.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_map.Image = Properties.Resources.m_map_u;

                //btn_msg.BackColor = Color.Transparent;
                //btn_msg.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_msg.Image = Properties.Resources.m_message_u;

                //btn_video.BackColor = Color.Transparent;
                //btn_video.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_video.Image = Properties.Resources.m_video_u;



                //btn_rec.BackColor = Color.FromArgb(47, 64, 80);
                //btn_rec.ForeColor = Color.White;
                //btn_rec.Image = Properties.Resources.recquery;

                //btn_sys.BackColor = Color.Transparent;
                //btn_sys.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_sys.Image = Properties.Resources.syscfg_u;



            }
            else if (index == 6)
            {


                //btn_map.BackColor = Color.Transparent;
                //btn_map.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_map.Image = Properties.Resources.m_map_u;

                //btn_msg.BackColor = Color.Transparent;
                //btn_msg.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_msg.Image = Properties.Resources.m_message_u;

                //btn_video.BackColor = Color.Transparent;
                //btn_video.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_video.Image = Properties.Resources.m_video_u;


                //btn_rec.BackColor = Color.Transparent;
                //btn_rec.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_rec.Image = Properties.Resources.recquery_u;

                //btn_sys.BackColor = Color.FromArgb(47, 64, 80);
                //btn_sys.ForeColor = Color.White;
                //btn_sys.Image = Properties.Resources.syscfg;




            }
            else if (index == 7)
            {


                //btn_map.BackColor = Color.Transparent;
                //btn_map.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_map.Image = Properties.Resources.m_map_u;

                //btn_msg.BackColor = Color.Transparent;
                //btn_msg.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_msg.Image = Properties.Resources.m_message_u;

                //btn_video.BackColor = Color.Transparent;
                //btn_video.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_video.Image = Properties.Resources.m_video_u;


                //btn_rec.BackColor = Color.Transparent;
                //btn_rec.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_rec.Image = Properties.Resources.recquery_u;

                //btn_sys.BackColor = Color.Transparent;
                //btn_sys.ForeColor = Color.FromArgb(153, 153, 153);
                //btn_sys.Image = Properties.Resources.syscfg_u;


                //                

            }


        }

        /*
        private void AddChatP2GText(ChatMessage chtMsg)
        {

            //将消息发向WPF IM窗口
            SDKClient.Model.MessagePackage imMessage = new SDKClient.Model.MessagePackage();
            imMessage.id = Guid.NewGuid().ToString();
            imMessage.code = 0;
            imMessage.error = "";
            imMessage.time = DateTime.Now;
            imMessage.from = chtMsg.getUserid();
            imMessage.to = chtMsg.getGroupid();
            SDKClient.Model.message data = new SDKClient.Model.message();
            data.type = "groupChat";
            data.subType = "txt";
            data.chatType = 0;

            SDKClient.Model.TxtBody txtBody = new SDKClient.Model.TxtBody();

            // 左边是 poc, 右边是im格式
            txtBody.text = chtMsg.getMsgcontent();
            txtBody.text = txtBody.text.Replace("[NO]", "/NO");
            txtBody.text = txtBody.text.Replace("[OK]", "/OK");
            txtBody.text = txtBody.text.Replace("[爱你]", "/爱你");
            txtBody.text = txtBody.text.Replace("[爱情]", "/爱情");
            txtBody.text = txtBody.text.Replace("[爱心]", "/爱心");
            txtBody.text = txtBody.text.Replace("[傲慢]", "/傲慢");
            txtBody.text = txtBody.text.Replace("[白眼]", "/白眼");

            txtBody.text = txtBody.text.Replace("[抱拳]", "/抱拳");
            txtBody.text = txtBody.text.Replace("[鄙视]", "/鄙视");
            txtBody.text = txtBody.text.Replace("[闭嘴]", "/闭嘴");
            txtBody.text = txtBody.text.Replace("[便便]", "/便便");
            txtBody.text = txtBody.text.Replace("[擦汗]", "/擦汗");
            txtBody.text = txtBody.text.Replace("[菜刀]", "/菜刀");
            txtBody.text = txtBody.text.Replace("[差劲]", "/差劲");

            txtBody.text = txtBody.text.Replace("[呲牙]", "/呲牙");
            txtBody.text = txtBody.text.Replace("[大兵]", "/大兵");
            txtBody.text = txtBody.text.Replace("[大哭]", "/大哭");
            txtBody.text = txtBody.text.Replace("[蛋糕]", "/蛋糕");
            txtBody.text = txtBody.text.Replace("[刀]", "/刀");
            txtBody.text = txtBody.text.Replace("[得意]", "/得意");
            txtBody.text = txtBody.text.Replace("[凋谢", "/凋谢");

            txtBody.text = txtBody.text.Replace("[发呆]", "/发呆");
            txtBody.text = txtBody.text.Replace("[发抖]", "/发抖");
            txtBody.text = txtBody.text.Replace("[发怒]", "/发怒");
            txtBody.text = txtBody.text.Replace("[饭]", "/饭");
            txtBody.text = txtBody.text.Replace("[飞吻]", "/飞吻");
            txtBody.text = txtBody.text.Replace("[奋斗]", "/奋斗");
            txtBody.text = txtBody.text.Replace("[尴尬]", "/尴尬");
            txtBody.text = txtBody.text.Replace("[勾引]", "/勾引");
            txtBody.text = txtBody.text.Replace("[鼓掌]", "/鼓掌");
            txtBody.text = txtBody.text.Replace("[哈欠]", "/哈欠");

            txtBody.text = txtBody.text.Replace("[害羞]", "/害羞");
            txtBody.text = txtBody.text.Replace("[憨笑]", "/憨笑");
            txtBody.text = txtBody.text.Replace("[坏笑]", "/坏笑");
            txtBody.text = txtBody.text.Replace("[挥手]", "/挥手");
            txtBody.text = txtBody.text.Replace("[回头]", "/回头");
            txtBody.text = txtBody.text.Replace("[饥饿]", "/饥饿");
            txtBody.text = txtBody.text.Replace("[激动]", "/激动");
            txtBody.text = txtBody.text.Replace("[街舞]", "/街舞");
            txtBody.text = txtBody.text.Replace("[惊恐]", "/惊恐");
            txtBody.text = txtBody.text.Replace("[惊讶]", "/惊讶");


            txtBody.text = txtBody.text.Replace("[咖啡]", "/咖啡");
            txtBody.text = txtBody.text.Replace("[磕头]", "/磕头");
            txtBody.text = txtBody.text.Replace("[可爱]", "/可爱");
            txtBody.text = txtBody.text.Replace("[可怜]", "/可怜");
            txtBody.text = txtBody.text.Replace("[抠鼻]", "/抠鼻");

            txtBody.text = txtBody.text.Replace("[骷髅]", "/骷髅");
            txtBody.text = txtBody.text.Replace("[酷]", "/酷");
            txtBody.text = txtBody.text.Replace("[快哭了]", "/快哭了");
            txtBody.text = txtBody.text.Replace("[困]", "/困");
            txtBody.text = txtBody.text.Replace("[篮球]", "/篮球");

            txtBody.text = txtBody.text.Replace("[冷汗]", "/冷汗");
            txtBody.text = txtBody.text.Replace("[礼物]", "/礼物");
            txtBody.text = txtBody.text.Replace("[流汗]", "/流汗");
            txtBody.text = txtBody.text.Replace("[流泪]", "/流泪");
            txtBody.text = txtBody.text.Replace("[玫瑰]", "/玫瑰");

            txtBody.text = txtBody.text.Replace("[美女]", "/美女");
            txtBody.text = txtBody.text.Replace("[难过]", "/难过");
            txtBody.text = txtBody.text.Replace("[怄火]", "/怄火");
            txtBody.text = txtBody.text.Replace("[啤酒]", "/啤酒");
            txtBody.text = txtBody.text.Replace("[飘虫]", "/飘虫");

            txtBody.text = txtBody.text.Replace("[撇嘴]", "/撇嘴");
            txtBody.text = txtBody.text.Replace("[乒乓球]", "/乒乓球");
            txtBody.text = txtBody.text.Replace("[钱]", "/钱");
            txtBody.text = txtBody.text.Replace("[强]", "/强");
            txtBody.text = txtBody.text.Replace("[敲打]", "/敲打");

            txtBody.text = txtBody.text.Replace("[亲亲]", "/亲亲");
            txtBody.text = txtBody.text.Replace("[糗大了]", "/糗大了");
            txtBody.text = txtBody.text.Replace("[拳头]", "/拳头");
            txtBody.text = txtBody.text.Replace("[弱]", "/弱");
            txtBody.text = txtBody.text.Replace("[色]", "/色");

            txtBody.text = txtBody.text.Replace("[闪电]", "/闪电");
            txtBody.text = txtBody.text.Replace("[胜利]", "/胜利");
            txtBody.text = txtBody.text.Replace("[示爱]", "/示爱");
            txtBody.text = txtBody.text.Replace("[衰]", "/衰");
            txtBody.text = txtBody.text.Replace("[睡]", "/睡");

            txtBody.text = txtBody.text.Replace("[太阳]", "/太阳");
            txtBody.text = txtBody.text.Replace("[调皮]", "/调皮");
            txtBody.text = txtBody.text.Replace("[跳绳]", "/跳绳");
            txtBody.text = txtBody.text.Replace("[跳跳]", "/跳跳");
            txtBody.text = txtBody.text.Replace("[偷笑]", "/偷笑");

            txtBody.text = txtBody.text.Replace("[吐]", "/吐");
            txtBody.text = txtBody.text.Replace("[微笑]", "/微笑");
            txtBody.text = txtBody.text.Replace("[委屈]", "/委屈");
            txtBody.text = txtBody.text.Replace("[握手]", "/握手");
            txtBody.text = txtBody.text.Replace("[西瓜]", "/西瓜");

            txtBody.text = txtBody.text.Replace("[吓]", "/吓");
            txtBody.text = txtBody.text.Replace("[献吻]", "/献吻");
            txtBody.text = txtBody.text.Replace("[心碎]", "/心碎");
            txtBody.text = txtBody.text.Replace("[嘘]", "/嘘");
            txtBody.text = txtBody.text.Replace("[疑问]", "/疑问");

            txtBody.text = txtBody.text.Replace("[阴险]", "/阴险");
            txtBody.text = txtBody.text.Replace("[拥抱]", "/拥抱");
            txtBody.text = txtBody.text.Replace("[右哼哼]", "/右哼哼");
            txtBody.text = txtBody.text.Replace("[右太极]", "/右太极");
            txtBody.text = txtBody.text.Replace("[月亮]", "/月亮");
            txtBody.text = txtBody.text.Replace("[晕]", "/晕");
            txtBody.text = txtBody.text.Replace("[再见]", "/再见");
            txtBody.text = txtBody.text.Replace("[炸弹]", "/炸弹");
            txtBody.text = txtBody.text.Replace("[折磨]", "/折磨");
            txtBody.text = txtBody.text.Replace("[咒骂]", "/咒骂");

            txtBody.text = txtBody.text.Replace("[猪头]", "/猪头");
            txtBody.text = txtBody.text.Replace("[抓狂]", "/抓狂");
            txtBody.text = txtBody.text.Replace("[转圈]", "/转圈");
            txtBody.text = txtBody.text.Replace("[足球]", "/足球");
            txtBody.text = txtBody.text.Replace("[左哼哼]", "/左哼哼");
            txtBody.text = txtBody.text.Replace("[左太极]", "/左太极");


            data.body = txtBody;
            data.senderInfo = new SDKClient.Model.message.SenderInfo
            {
                userName = LocalSharedData.GetUserName(Convert.ToInt32(chtMsg.getUserid()))
            };
            data.groupInfo = new SDKClient.Model.group
            {
                groupId = Convert.ToInt32(chtMsg.getGroupid())
            };

            imMessage.data = data;
            AppData.MainMV.ChatListVM.ReceiveMsg(imMessage);

        }

        */
        private void AddChatP2PText(ChatMessage chtMsg)
        {
            //disply(chtMsg, Color.Red, true);
            //MessageBox.Show("收到信息: ChatP2PText");

            string strInput = "[" + LocalSharedData.GetUserName(Int32.Parse(chtMsg.getUserid())) + "]";
            // this.SendTextMsg.MsgContent = strInput;            

            if (this.FullMsgPopup)
            {
                this.CallMsgPopup("MESSAGE_TEXT", strInput, chtMsg.getMsgcontent());
            }

        }

        //private delegate void OnChatTextEventDelegate(ChatMessage chtMsg, Color fontColor, Boolean sendORrecv);
        private delegate void OnChatTextEventDelegate(ChatMessage chtMsg);
        private void OnChatP2GText(ChatMessage chtMsg)
        {
            /*
            if (InvokeRequired)
            {
                OnChatTextEventDelegate uld = new OnChatTextEventDelegate(AddChatP2GText);
                this.Invoke(uld, new object[] { chtMsg });
                return;
            }
            AddChatP2GText(chtMsg);
            */
        }
        private void OnChatP2PText(ChatMessage chtMsg)
        {
            if (InvokeRequired)
            {
                OnChatTextEventDelegate uld = new OnChatTextEventDelegate(AddChatP2PText);
                this.Invoke(uld, new object[] { chtMsg, Color.Blue, false });
                return;
            }
            //disply(chtMsg, Color.Blue, false);
            AddChatP2PText(chtMsg);
        }

        private void OnChatP2GFile(ChatMessage chtMsg)
        {
            OnReceiveFileRequest(chtMsg);
        }
        private void OnChatP2PFile(ChatMessage chtMsg)
        {
            OnReceiveFileRequest(chtMsg);
        }

        private delegate void ServerOnChatEventDelegate(ChatMessage cht);
        private void ServerOnChatEventNotfiy(object sender, ServerEventArgs e)
        {
            ChatMessage cht = e.chat;
            Debug.WriteLine(cht.toString());
            string strmsgid = cht.getMsgtype();

            if (string.IsNullOrEmpty(strmsgid))
                return;

            if (InvokeRequired)
            {
                ServerOnChatEventDelegate onmsgevent = new ServerOnChatEventDelegate(OnServerOnChatEventDelegate);
                this.Invoke(onmsgevent, new object[] { cht });
                return;
            }
            else
            {
                OnServerOnChatEventDelegate(cht);
            }
        }

        private void OnServerOnChatEventDelegate(ChatMessage cht)
        {
            //todo 2022.12.29  关闭IM消息功能
            /*
            if (this.fullIMWin == null || client.IsCreateIMForm == false)
            {
                DialogResult result = MessageBox.Show("有收到即时消息,要打开消息窗口吗", WinFormsStringResource.PromptStr, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //
                    if (fullIMWin == null || client.IsCreateIMForm == false)
                    {
                        fullIMWin =

                        new WpfApplication1.MainWindow(client, LocalSharedData.CURRENTUser.user_id,
                        LocalSharedData.CURRENTUser.group_id, HttpAPI.WEBIP, Convert.ToInt32(HttpAPI.WEBPort));
                        ElementHost.EnableModelessKeyboardInterop(fullIMWin);
                        client.IsCreateIMForm = true;
                    }
                    //使用WindowInteropHelper类为WPF设置owner
                    WindowInteropHelper helper = new WindowInteropHelper(fullIMWin);
                    helper.Owner = this.Handle;
                    fullIMWin.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                    fullIMWin.Show();
                    Task.Delay(3000).ContinueWith((t) =>
                    {
                        switch (int.Parse(cht.getMsgtype()))
                        {
                            case MyType.TYPE_P2G_CHAT_TEXT:
                                OnChatP2GText(cht);
                                break;
                            case MyType.TYPE_P2P_CHAT_TEXT:
                                OnChatP2PText(cht);
                                break;

                            case MyType.TYPE_P2G_CHAT_FILE:
                                OnChatP2GFile(cht);
                                break;
                            case MyType.TYPE_P2P_CHAT_FILE:
                                OnChatP2PFile(cht);
                                break;

                            default:
                                Debug.WriteLine("chat message error!");
                                break;
                        }

                    });

                }
                else
                {
                    return;
                }

            }
            else
            {
                string strmsgid = cht.getMsgtype();
                int msgid = int.Parse(strmsgid);

                switch (msgid)
                {
                    case MyType.TYPE_P2G_CHAT_TEXT:
                        OnChatP2GText(cht);
                        break;
                    case MyType.TYPE_P2P_CHAT_TEXT:
                        OnChatP2PText(cht);
                        break;

                    case MyType.TYPE_P2G_CHAT_FILE:
                        OnChatP2GFile(cht);
                        break;
                    case MyType.TYPE_P2P_CHAT_FILE:
                        OnChatP2PFile(cht);
                        break;

                    default:
                        Debug.WriteLine("chat message error!");
                        break;
                }
            }
            */


        }

        private void CallMsgPopup(string msgType, string msgTitle, string msgContent)
        {
            MsgPopupForm msgPopup = new MsgPopupForm(this);
            msgPopup.labelMsg.Text = msgTitle;
            msgPopup.MESSAGE_TYPE = msgType;
            if (msgType.Equals("MESSAGE_TEXT"))
            {
                msgPopup.tbContent.Text = msgContent;
                msgPopup.tbContent.Visible = true;
                msgPopup.pictureType.Image = Properties.Resources.mail_yellow;
                msgPopup.Text = "发现1个短文消息";
            }
            else if (msgType.Equals("MESSAGE_FILE"))
            {
                //msgPopup.tbContent.Text = msgContent;
                msgPopup.tbContent.Visible = false;
                msgPopup.pictureType.Image = Properties.Resources.page;
                msgPopup.Text = "发现1个文件消息";
            }

            msgPopup.Show();
        }

        private void OnCallChatEventNotfiy(object sender, ServerEventArgs e)
        {
            //  e.calltype:  1:主叫        0：被叫
            //  e.callstate  0:挂机hangup  1: 接收 Accept;
            switch (e.calltype)
            {
                case 1:
                    if (e.callstate == 0)
                        CallPersonEnd();
                    break;

                case 0:
                    if (e.callstate == 0)
                        CallPersonEnd();
                    else
                        CallPersonStart();
                    break;
            }
        }

        private delegate void ServerOnMessageEventDelegate(ManageDataPacket mng);
        private void ServerOnMessageEventNotfiy(object sender, ServerEventArgs e)
        {
            try
            {
                ManageDataPacket mng = e.manage;
                Debug.WriteLine(mng.toString());
                if (InvokeRequired)
                {
                    ServerOnMessageEventDelegate onmsgevent = new ServerOnMessageEventDelegate(OnServerOnMessageEventDelegate);
                    this.Invoke(onmsgevent, new object[] { mng });
                    return;
                }
                else
                {
                    OnServerOnMessageEventDelegate(mng);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void OnServerOnMessageEventDelegate(ManageDataPacket mng)
        {
            String name;
            switch (mng.getMessageId())
            {
                case MyType.TYPE_SYS_MESSAGE:

                    if (mng.getType() == MySysMsgType.SYS_MSSAGE_IN_GROUP)
                    {
                        //某用户进入某组
                        Debug.WriteLine("NetUser_EnterGroup: groupid=" + mng.getGroupId() + ",userid=" + mng.getUserId());
                        NetUser_EnterGroup(mng.getGroupId(), mng.getUserId());

                    }
                    else if (mng.getType() == MySysMsgType.SYS_MSSAGE_OUT_GROUP)
                    {
                        //某用户离开某组
                        Debug.WriteLine("NetUser_ExitGroup: groupid=" + mng.getGroupId() + ",userid=" + mng.getUserId());
                        NetUser_ExitGroup(mng.getGroupId(), mng.getUserId());

                    }
                    else if (mng.getType() == MySysMsgType.SYS_MSSAGE_TALK_START)
                    {
                        //2017.8.11, 切换到某个临时单到单的对讲模式下 
                        if (SINGLE_POC_ENABLE && onePocForm != null && !onePocForm.IsDisposed)
                        {
                            onePocForm.m_blUIPlay = true;
                            //WinFormsStringResource.Main_labelPlayerName_Text
                            name = LocalSharedData.UserAll[mng.getUserId()].userName;
                            onePocForm.SetPlayUIStatus(name);

                        } else if (BROADCAST_POC_ENABLE && broadCastForm!=null && !broadCastForm.IsDisposed)
                        {
                            broadCastForm.m_blUIPlay = true;
                            name = LocalSharedData.UserAll[mng.getUserId()].userName;
                            broadCastForm.SetPlayUIStatus(name);

                        }
                        else if (TEMPTALK_POC_ENABLE  && tempTalkForm != null && !tempTalkForm.IsDisposed)
                        {
                            tempTalkForm.m_blUIPlay = true;
                            name = LocalSharedData.UserAll[mng.getUserId()].userName;
                            tempTalkForm.SetPlayUIStatus(name);

                        }
                        else if (LocalSharedData.CURRENTUser.group_id == mng.getGroupId())
                        {
                            //这里要考虑是否在同一个组
                            this.m_blUIPlay = true;
                            name = LocalSharedData.UserAll[mng.getUserId()].userName;
                            SetPlayUIStatus(name);
                        }
                        else if (LocalSharedData.CURRENTUser.group_id != mng.getGroupId())
                        {
                            //2017.11.16 做了重大改进,不是本组的处理
                            this.m_blUIPlay = true;
                            name = LocalSharedData.UserAll[mng.getUserId()].userName;
                            //2023.9 修改了不显示，没有多大意义
                            //SetPlayUIStatus("非当前组:" + name);

                        }

                        NetUser_TalkStart(mng.getGroupId(), mng.getUserId());

                    }
                    else if (mng.getType() == MySysMsgType.SYS_MSSAGE_TALK_STOP)
                    {
                        //2017.8.11
                        if (SINGLE_POC_ENABLE && onePocForm != null && !onePocForm.IsDisposed)
                        {
                            onePocForm.m_blUIPlay = false;
                            onePocForm.SetPlayUIStatus("");

                        } else if (BROADCAST_POC_ENABLE && broadCastForm!=null && !broadCastForm.IsDisposed)
                        {
                            broadCastForm.m_blUIPlay = false;
                            broadCastForm.SetPlayUIStatus("");

                        }
                        else if (TEMPTALK_POC_ENABLE  && tempTalkForm != null && !tempTalkForm.IsDisposed)
                        {
                            tempTalkForm.m_blUIPlay = false;
                            tempTalkForm.SetPlayUIStatus("");

                        }
                        else
                        {
                            m_blUIPlay = false;
                            SetPlayUIStatus("");
                        }
                        NetUser_TalkStop(mng.getGroupId(), mng.getUserId());

                    }
                    else if (mng.getType() == MySysMsgType.SYS_MSSAGE_TALK_START_TOPOC)
                    {
                        //2017.8.11
                        if (SINGLE_POC_ENABLE && onePocForm != null && !onePocForm.IsDisposed)
                        {
                            //onePocForm.pictureBox1.Image = global::POCControlCenter.Properties.Resources.shengbo_bar1;
                            onePocForm.m_blUIPlay = true;
                            name = LocalSharedData.GetUserName(mng.getUserId());
                            onePocForm.SetPlayUIStatus(name);

                        } else if (BROADCAST_POC_ENABLE && broadCastForm != null && !broadCastForm.IsDisposed)
                        {
                            broadCastForm.m_blUIPlay = true;
                            name = LocalSharedData.GetUserName(mng.getUserId());
                            broadCastForm.SetPlayUIStatus(name);

                        }
                        else if (TEMPTALK_POC_ENABLE && tempTalkForm != null && !tempTalkForm.IsDisposed)
                        {
                            tempTalkForm.m_blUIPlay = true;
                            name = LocalSharedData.GetUserName(mng.getUserId());
                            tempTalkForm.SetPlayUIStatus(name);

                        }
                        else if (LocalSharedData.CURRENTUser.group_id == mng.getGroupId())
                        {
                            //这里要考虑是否在同一个组
                            m_blUIPlay = true;
                            name = LocalSharedData.GetUserName(mng.getUserId());
                            SetPlayUIStatus(name);
                        }
                        else if (LocalSharedData.CURRENTUser.group_id != mng.getGroupId())
                        {
                            //2017.11.16 做了重大改进,不是本组的处理
                            m_blUIPlay = true;
                            name = LocalSharedData.UserAll[mng.getUserId()].userName;
                            //2023.9 修改了不显示，没有多大意义
                            //SetPlayUIStatus("非当前组:" + name);

                        }

                        NetUser_TalkStart(mng.getGroupId(), mng.getUserId());

                    }
                    else if (mng.getType() == MySysMsgType.SYS_MSSAGE_TALK_STOP_TOPOC)
                    {
                        //2017.8.11
                        if (SINGLE_POC_ENABLE && onePocForm != null && !onePocForm.IsDisposed)
                        {

                            onePocForm.m_blUIPlay = false;
                            onePocForm.SetPlayUIStatus("");

                        } else if (BROADCAST_POC_ENABLE && broadCastForm!=null && !broadCastForm.IsDisposed)
                        {
                            broadCastForm.m_blUIPlay = false;
                            broadCastForm.SetPlayUIStatus("");
                        }
                        else if (TEMPTALK_POC_ENABLE && tempTalkForm != null && !tempTalkForm.IsDisposed)
                        {
                            tempTalkForm.m_blUIPlay = false;
                            tempTalkForm.SetPlayUIStatus("");
                        }
                        else
                        {
                            m_blUIPlay = false;
                            SetPlayUIStatus("");
                        }
                        NetUser_TalkStop(mng.getGroupId(), mng.getUserId());

                    }
                    else if (mng.getType() == MySysMsgType.SYS_MSSAGE_ONLINE_PRESON)
                    {
                        NetUser_Login(mng.getGroupId(), mng.getUserId());
                    }
                    else if (mng.getType() == MySysMsgType.SYS_MSSAGE_OFFLINE_PRESON)
                    {
                        NetUser_Logout(mng.getGroupId(), mng.getUserId());
                    }

                    /** 某用户同意单聊邀请 */
                    else if (mng.getType() == MySysMsgType.SYS_MSSAGE_ENTER_PRESON)
                    {
                        CallPersonStart();
                        PersonCallForm.onRecvCalledAccept();
                    }
                    /** 某用户退出单聊 */
                    else if (mng.getType() == MySysMsgType.SYS_MSSAGE_EXIT_PRESON)
                    {
                        //todo 这地方要完善
                        DeleteGroupByFromNet(mng.getGroupId());
                        CallPersonEnd();
                        PersonCallForm.onRecvCallingCancel();
                    }
                    break;

                /** 某用户邀请单聊 */
                case MyType.TYPE_PERSON_INVITE:
                    {
                        PersonCallForm.CreateCalled(
                            client,
                            LocalSharedData.CURRENTUser.group_id,
                            LocalSharedData.CURRENTUser.user_id,
                            mng.getGroupId(),
                            mng.getInviteId(),
                            OnCallChatEventNotfiy
                            );
                    }
                    break;

                /** 某用户释放邀请单聊 */
                case MyType.TYPE_PERSON_INVITE_RELEASE:
                    {
                        PersonCallForm.onRecvCalledRefuse();
                    }
                    break;

                case MyType.TYPE_LOGIN:
                    NetUser_Login(mng.getGroupId(), mng.getUserId());
                    break;

                case MyType.TYPE_LOGOUT:
                    NetUser_Logout(mng.getGroupId(), mng.getUserId());
                    break;

                case MyType.TYPE_MIC_SUCCESS:

                    //2017.8.11
                    if (SINGLE_POC_ENABLE && onePocForm != null && !onePocForm.IsDisposed)
                    {
                        //单呼模式下的
                        //抢麦成功，如果强制
                        if (onePocForm.checkBoxGroupSync.Checked)
                        {
                            PocClient.syncGroup(Convert.ToString(onePocForm.groupid)
                                 , onePocForm.groupname, Convert.ToString(LocalSharedData.CURRENTUser.user_id),
                                 "2"
                                 );

                        }
                        onePocForm.m_blUIStalk = true;
                        onePocForm.SetButtonTalkUIStatus();
                        StartChat(1, 0, 0);

                    }
                    else if (BROADCAST_POC_ENABLE && broadCastForm!=null && !broadCastForm.IsDisposed)
                    {
                        //广播对讲开始
                        broadCastForm.m_blUIStalk = true;

                        //这里一定先开启录音判断，因为文件广播有点特殊，放在麦成功的回调中播放，播放完又立即变更了
                        //BROADCAST_POC_FILE_MODE_ENABLE
                        if (!BROADCAST_POC_FILE_MODE_ENABLE)
                        {
                            StartChat(1, 0, 0);
                        }
                        //这里一定要放到后面
                        broadCastForm.SetButtonTalkUIStatus();
                    }
                    else if (TEMPTALK_POC_ENABLE && tempTalkForm != null && !tempTalkForm.IsDisposed)
                    {
                        //广播对讲开始
                        //同步放到临时对讲的form_load中

                        tempTalkForm.m_blUIStalk = true;
                        
                        tempTalkForm.SetButtonTalkUIStatus();
                        StartChat(1, 0, 0);
                    }
                    else
                    {
                        //2017.2.28 新增以下功能
                        //抢麦成功，如果强制
                        //2023.09.09 抢麦成功后，再同步，会容易造成第一次PC讲话时,APP只听到声音，但无讲话人和状态显示
                        //所以决定，不用这个反人类的强制同步功能
                        if (this.checkBoxGroupSync.Checked)
                        {
                            PocClient.syncGroup(Convert.ToString(LocalSharedData.CURRENTUser.group_id)
                                 , LocalSharedData.CURRENTGroupName, Convert.ToString(LocalSharedData.CURRENTUser.user_id),
                                 getGrpType(Convert.ToString(LocalSharedData.CURRENTUser.group_id))
                                 );

                        }

                        m_blUIStalk = true;
                        SetButtonTalkUIStatus();
                        //调度台抢麦成功时,改变图标
                        NetUser_TalkStart(LocalSharedData.CURRENTUser.group_id,
                    LocalSharedData.CURRENTUser.user_id);

                        StartChat(1, 0, 0);

                    }                   
                                       
                    

                    break;

                case MyType.TYPE_MIC_FAILED:
                    //2017.8.11
                    if (SINGLE_POC_ENABLE && onePocForm != null && !onePocForm.IsDisposed)
                    {
                        onePocForm.m_blUIStalk = false;
                        onePocForm.SetButtonTalkUIStatus();
                    } else if (BROADCAST_POC_ENABLE && broadCastForm!=null && !broadCastForm.IsDisposed)
                    {
                        broadCastForm.m_blUIStalk = false;
                        broadCastForm.SetButtonTalkUIStatus();

                    }
                    else if (TEMPTALK_POC_ENABLE  && tempTalkForm != null && !tempTalkForm.IsDisposed)
                    {
                        tempTalkForm.m_blUIStalk = false;
                        tempTalkForm.SetButtonTalkUIStatus();
                    }
                    else
                    {
                        m_blUIStalk = false;
                        SetButtonTalkUIStatus();
                    }

                    break;

                case MyType.TYPE_DELETE_GROUP:
                    //MessageBox.Show("该组"+mng.getGroupId().ToString()+"将清除");
                    DeleteGroupByFromNet(mng.getGroupId());
                    break;
                case MyType.TYPE_RECIVED_INVITE:
                    //收到创建临时群组
                    CreateGroupByFromNet(mng.getGroupId());
                    break;
                case MyType.TYPE_SOS_LOCATION:
                    //接收到SOS按下上报   
                    SOS_INCOME_CALL_ENABLE = true;
                    //if (mng.getLatitude()<0.1 && SOS_Session_UserID==0 && SOS_Session_Finish )
                    if (mng.getLatitude() < 0.1)
                    {
                        //由于服务端是群发,所以这里要过滤
                        if (!LocalSharedData.UserAll.ContainsKey(mng.getUserId()))
                            break;
                        //立即进入SOS对讲模式
                        client.EncodeMedia_Mode = 2;
                        //第一次只是警告信息，其经纬度值为0，是先弹窗
                        CallSOSPopUp(mng);
                        //显示说话人
                        if (SOS_INCOME_CALL_ENABLE && sosPopupForm != null)
                        {
                            if (mng.getUserId() == SOS_Session_UserID)
                                //sosPopupForm.SetPlayUIStatus(LocalSharedData.GetUserName(mng.getUserId()));
                                sosPopupForm.SetPlayUIStatus("对方说话中...");

                        }


                    }
                    else if (mng.getLatitude() > 0 && SOS_Session_UserID > 0)
                    {
                        //看是否当前位置的      
                        if (!LocalSharedData.UserAll.ContainsKey(mng.getUserId()))
                            break;
                        UpdateSOSPopUp(mng);

                    }

                    break;
                case MyType.TYPE_SOS_KEY_RELEASE:

                    if (!LocalSharedData.UserAll.ContainsKey(mng.getUserId()))
                        break;

                    SOSKeyRelease(mng);

                    //接收到SOS放开的上报
                    //MessageBox.Show("收到TYPE_SOS_KEY_RELEASE:" + mng.getUserId() + "," +
                    //   mng.getGroupId());
                    break;
                case MyType.TYPE_GPS_COMMAND:
                    //接收到poc终端上报的GPS坐标的报文
                    Debug.WriteLine("收到GPS坐标:" + mng.getUserId());

                    NetUser_UploadGPS(mng.getGroupId(), mng.getUserId());

                    UpdateGPSLocation(mng.getErrorCode(), mng.getRequestType(),
                        mng.getUserId());

                    break;
                case MyType.KICK_OFF:
                    KickOff(mng.getGroupId(), mng.getUserId());
                    break;
                default:
                    break;

            }
        }

        private delegate void CallSOSPopUpDelegate(ManageDataPacket mng);
        private delegate void UpdateSOSPopUpDelegate(ManageDataPacket mng);
        private delegate void SOSKeyReleaseDelegate(ManageDataPacket mng);

        private void SOSKeyRelease(ManageDataPacket mng)
        {
            if (sosPopupForm != null)
            {
                if (sosPopupForm.InvokeRequired == true)
                {
                    SOSKeyReleaseDelegate uld = new SOSKeyReleaseDelegate(SOSKeyRelease);
                    sosPopupForm.Invoke(uld, new object[] { mng });
                }

                //显示是否当前人，因为一个窗口可能切换到其它人的了
                if (SOS_INCOME_CALL_ENABLE && sosPopupForm != null)
                {
                    if (mng.getUserId() == SOS_Session_UserID)
                    {
                        sosPopupForm.SetPlayUIStatus("");
                        SOS_Session_Finish = true;
                    }

                }

            }
            //立即进行普通对讲模式
            client.EncodeMedia_Mode = 1;

        }

        private void UpdateSOSPopUp(ManageDataPacket mng)
        {
            if (sosPopupForm != null)
            {
                if (sosPopupForm.InvokeRequired == true)
                {
                    UpdateSOSPopUpDelegate uld = new UpdateSOSPopUpDelegate(UpdateSOSPopUp);
                    sosPopupForm.Invoke(uld, new object[] { mng });
                }
                //显示是否当前人，因为一个窗口可能切换到其它人的了
                if (SOS_Session_UserID == mng.getUserId())
                {
                    sosPopupForm.MapLocAndGetAdress(LocalSharedData.GetUserName(mng.getUserId())
                      , mng.getLongitude(), mng.getLatitude());

                }
            }

        }
        private void CallSOSPopUp(ManageDataPacket mng)
        {
            if (buttonTALK.InvokeRequired == true)
            {
                CallSOSPopUpDelegate uld = new CallSOSPopUpDelegate(CallSOSPopUp);
                buttonTALK.Invoke(uld, new object[] { mng });
            }
            //
            if (sosPopupForm == null)
            {
                sosPopupForm = new SOSPopUpForm(this, mng.getGroupId(), mng.getUserId());
                sosPopupForm.MapLocInit(true);
                sosPopupForm.Owner = this;
                sosPopupForm.labUsername.Text = LocalSharedData.GetUserName(mng.getUserId());
                sosPopupForm.labUserid.Text = mng.getUserId().ToString();
                sosPopupForm.labGroupName.Text = LocalSharedData.GetGroupName(mng.getGroupId());
                long timesec = (long)mng.getSos_datetime();
                DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(timesec);
                //转成本地时间  
                DateTime localdt = utcdt.ToLocalTime();
                //2017.8.16 加入时区差异的处理                                    
                localdt = localdt.AddHours(ZoneInterval_UserServer);
                sosPopupForm.labTime.Text = localdt.ToString("yyyy-MM-dd HH:mm:ss");

                SOS_Session_UserID = mng.getUserId();
                client.SOS_POC_USERID_SPEAKING = SOS_Session_UserID;

                SOS_Session_Finish = false;
                sosPopupForm.StartPosition = FormStartPosition.CenterParent;
                //不能用模态showModal,因为会在线程中造成阻塞
                sosPopupForm.Show();
            }
            else
            {
                //未关闭的，有可能是同一个人的SOS，也有可能是另外一个人的SOS
                if (mng.getUserId() == SOS_Session_UserID)
                {
                    //还是同一人的
                    SOS_Session_Finish = false;
                    SOS_Session_UserID = mng.getUserId();
                    client.SOS_POC_USERID_SPEAKING = SOS_Session_UserID;
                    //但要更新时间
                    long timesec = (long)mng.getSos_datetime();
                    DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(timesec);
                    //转成本地时间  
                    DateTime localdt = utcdt.ToLocalTime();
                    //2017.8.16 加入时区差异的处理                                    
                    localdt = localdt.AddHours(ZoneInterval_UserServer);
                    sosPopupForm.labTime.Text = localdt.ToString("yyyy-MM-dd HH:mm:ss");
                    //地图要在另一个接受中更新
                    sosPopupForm.MapLocInit(false);

                }
                else
                {
                    //另外一个人, 则更新为当前人的 mng.getUserId() != SOS_Session_UserID

                    if (SOS_Session_Finish)
                    {
                        //上一个人已完成了
                        sosPopupForm.labUsername.Text = LocalSharedData.GetUserName(mng.getUserId());
                        sosPopupForm.labUserid.Text = mng.getUserId().ToString();
                        sosPopupForm.labGroupName.Text = LocalSharedData.GetGroupName(mng.getGroupId());
                        long timesec = (long)mng.getSos_datetime();
                        DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(timesec);
                        //转成本地时间  
                        DateTime localdt = utcdt.ToLocalTime();
                        //2017.8.16 加入时区差异的处理                                    
                        localdt = localdt.AddHours(ZoneInterval_UserServer);
                        sosPopupForm.labTime.Text = localdt.ToString("yyyy-MM-dd HH:mm:ss");

                        SOS_Session_UserID = mng.getUserId();
                        client.SOS_POC_USERID_SPEAKING = SOS_Session_UserID;
                        SOS_Session_Finish = false;
                        sosPopupForm.StartPosition = FormStartPosition.CenterParent;
                        sosPopupForm.userid = mng.getUserId();

                        //不能用模态showModal,因为会在线程中造成阻塞
                        sosPopupForm.MapLocInit(true);
                        sosPopupForm.Show();
                    }
                    else
                    {
                        //上一个未完成，这时它只有未读累积了,把未读的SOS帐号加入列表
                        //if (UnReceiveSOSUserIDS.IndexOf(mng.getUserId())==-1 )
                        //{
                        if (mng.getSos_id() > 0)
                        {
                            UnReceiveSOSUserIDS.Add(mng.getSos_id());
                            //右上角累积未读并更新
                            Debug.WriteLine("右上角累积未读并更新:" + UnReceiveSOSUserIDS.Count);
                            if (UnReceiveSOSUserIDS.Count > 0)
                            {
                                btn_sos_num.Visible = true;
                                btn_sos_num.Text = UnReceiveSOSUserIDS.Count.ToString();
                            }
                        }


                        //}

                    }

                }

            }

        }
        private void UpdateGPSLocation(byte errorCode, byte requestType, int userId)
        {
            //MessageBox.Show("接收到坐标:"+userId );

            if (LocalSharedData.UserAll.ContainsKey(userId))
            {
                LocalSharedData.UserAll[userId].lastGpsTimeMs = Convert.ToInt64(Utils.getCurrentTimeMillis());
                //改图标
            }

            if (this.Full_GPSCommand_Mode == 1 && requestType == (byte)1)
            {

                //个人定位
                if (errorCode == (byte)1)
                {
                    //MessageBox.Show("终端GPS权限未打开,请先通知对方手动打开，再用GPS定位");
                    return;
                }
                else if (errorCode == (byte)2)
                {
                    //MessageBox.Show("终端定时异常，可以再试一次");
                    return;
                }
                else if (errorCode == (byte)3)
                {
                    //MessageBox.Show("GPS定位超时，你可以选择自动定位方式");
                    return;
                }
                else if (errorCode == (byte)0)
                {
                    //MessageBox.Show("接收到坐标");
                    //return;
                }
                //点中个人节点
                //先显示地图中坐标       
                if (Full_GPSCommand_GROUP_ID == 0 || Full_GPSCommand_USER_ID == 0)
                    return;

                ControlMainForm_ShowUserLocation(Full_GPSCommand_USER_ID, Full_GPSCommand_GROUP_ID);
                
            }
            else if (Full_GPSCommand_Mode == 0 && Full_GPSCommand_GROUP_ID > 0 && requestType == (byte)1)
            {
                return;  
            }
          
            setTabControlPageDisplay(0);

        }

        /// <summary>
        /// 有可能是调度台自己创建的临时群,这里要处理去重
        /// </summary>
        /// <param name="groupid"></param>
        private void CreateGroupByFromNet(int groupid)
        {
            //
            if (!LocalSharedData.GetGroupName(groupid).Equals("NULL"))
                return;

            TempGroupResponse resp= PocClient.queryTmpGroup(groupid);
            if (resp == null || resp.data == null) return;
            GroupTempDto dto= resp.data;

            Group grp = new Group();
            grp.group_id = dto.groupId;
            grp.group_name = dto.groupName;
            if (dto.aclass==1)
                grp.group_type= GroupTypeEnum.TALK_TMP_BROADCAST;
            else
                grp.group_type = GroupTypeEnum.TALK_TMP;

            grp.owner_id = dto.ownerId.Value;           

            LocalSharedData.UserAllTempGROUP.Add(grp);
            TreeNode it = null;
            if (dto.aclass == 1)
                it = this.treeViewGROUP.Nodes.Add("[广播]" + grp.group_name);      
            else
                it = this.treeViewGROUP.Nodes.Add("[临时]" + grp.group_name);

            it.Tag = grp;
            it.ToolTipText = grp.group_name;

            List<User> us = new List<User>();

            GroupUserMemberResponse resp2 = PocClient.queryTmpGroupMemberByGroupId(grp.group_id);
            if (resp2 != null && resp2.data != null && resp2.data.Count > 0)
            {
                us = resp2.data;
            }

            LocalSharedData.GROUPAllUser.Add(grp.group_id, us);

            for (int j = 0; j < us.Count; ++j)
            {
                if (us[j].userId == LocalSharedData.CURRENTUser.user_id)
                {
                    TreeNode user = it.Nodes.Add(us[j].userName);

                    user.Tag = us[j];
                    if (((Group)it.Tag).group_id == LocalSharedData.CURRENTUser.group_id)
                    {
                        user.ImageIndex = 3;
                        user.SelectedImageIndex = 3;
                    }
                    else
                    {
                        user.ImageIndex = 6;
                        user.SelectedImageIndex = 6;
                    }

                    user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                    if (us[j].lifeState != 1)
                    {
                        user.ImageIndex = 14;
                        user.SelectedImageIndex = 14;
                    }

                }
                else
                {
                    if (us[j].logon == 1)
                    {
                        TreeNode user = it.Nodes.Add(us[j].userName);                        
                        user.Tag = us[j];
                        //初始化都当作不在该组,等着组员的报文过来修正
                        user.ImageIndex = 6;
                        user.SelectedImageIndex = 6;
                        user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                        if (us[j].lifeState != 1)
                        {
                            user.ImageIndex = 14;
                            user.SelectedImageIndex = 14;
                        }

                    }
                    else
                    {
                        TreeNode user = it.Nodes.Add(us[j].userName);                        
                        user.Tag = us[j];
                        user.ImageIndex = 9;
                        user.SelectedImageIndex = 9;
                        user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                    }
                }
            }

            //触发 cbCurrGroup控件内容更新
            try
            {
                this.enableGroupSwitch = false;
                UpdateCbCurrGroupItems();

            }
            finally
            {
                this.enableGroupSwitch = true;
            }

        }

        private void DeleteGroupByFromNet(int groupid)
        {
            TreeNode grpNode = null;

            foreach (TreeNode node in treeViewGROUP.Nodes)
            {
                if (node.Tag is Group && ((Group)node.Tag).group_id == groupid)
                    grpNode = node;
            }

            if (grpNode == null)
                return;

            //删除组节点 
            if (grpNode.Nodes.Count > 0)
            {
                TreeNodeCollection coll = grpNode.Nodes;
                for (int i = coll.Count - 1; i >= 0; i--)
                {
                    coll[i].Remove();
                }

            }
            grpNode.Remove();
            //同时还要删除UserAllTempGROUP
            foreach (Group grp in LocalSharedData.UserAllTempGROUP)
            {
                if (grp.group_id == groupid)
                {
                    LocalSharedData.UserAllTempGROUP.Remove(grp);
                    break;
                }
            }

            //触发 cbCurrGroup控件内容更新
            try
            {
                this.enableGroupSwitch = false;
                UpdateCbCurrGroupItems();

            }
            finally
            {
                this.enableGroupSwitch = true;
            }

        }

        //定义一个代理
        private delegate void DispMSGDelegate(object sender, ServerEventArgs e);


        private void doReconnectServer(object sender, ServerEventArgs e)
        {
            try
            {
                Debug.WriteLine("============================================================");
                Debug.WriteLine("ServerOnClientDisconnected  Start");

                //如果是调用线程与btnGetPic所在线程是同一个
                ReleaseWaveRecord();
                client.Exit();
                //client.StopHeartBeat();
                client.CloseConnection();
                client.ReleaseWavePlayDevices();
                client.Release();

                System.Threading.Thread.Sleep(100);

                Debug.WriteLine("重连接Socket Server  Restart");

                InitWaveRecord();
                client.Enter();

                //client.InitWavePlayDevices();
                InitWavePlay();

                if (client.NetInit())
                {
                    //toolStripBtnServerStatus.Image = Properties.Resources.green;
                    //toolStripBtnDBStatus.Image = Properties.Resources.green;
                    client.SendMessage((new Data()).LogoutMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id));

                    client.SendMessage(
                        (new Data()).ReportMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
                    );

                }
                else
                {
                    //toolStripBtnServerStatus.Image = Properties.Resources.red;
                    //toolStripBtnDBStatus.Image = Properties.Resources.red;
                }

                //断线重连，不要启动心跳, 让心跳线程在程序一开始就一直运行
                //client.StartHeartBeat();
                //Debug.WriteLine("重启动 StartHeartBeat");

                Debug.WriteLine("ServerOnClientDisconnected  end");
                Debug.WriteLine("============================================================");

            }
            catch (Exception)
            {
                Debug.WriteLine("ServerOnClientDisconnected  Exception");
                Debug.WriteLine("============================================================");
            }

        }

        private void ServerOnClientDisconnected(object sender, ServerEventArgs e)
        {

            if (this.buttonNewTmpGrp.InvokeRequired == false)
            {
                doReconnectServer(sender, e);
            }
            else
            {
                DispMSGDelegate DMSGD = new DispMSGDelegate(doReconnectServer);
                this.buttonNewTmpGrp.Invoke(DMSGD, sender, e);
            }

            //try
            //{
            //    Debug.WriteLine("============================================================");
            //    Debug.WriteLine("ServerOnClientDisconnected  Start");

            //    ReleaseWaveRecord();
            //    client.Exit();
            //    client.StopHeartBeat();
            //    client.CloseConnection();
            //    client.ReleaseWavePlayDevices();
            //    client.Release();

            //    System.Threading.Thread.Sleep( 100 );

            //    Debug.WriteLine("ServerOnClientDisconnected  Restart");

            //    InitWaveRecord();
            //    client.NetInit();
            //    client.InitWavePlayDevices();
            //    client.SendMessage(
            //        (new Data()).ReportMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
            //    );

            //    client.StartHeartBeat();

            //    Debug.WriteLine("ServerOnClientDisconnected  end");
            //    Debug.WriteLine("============================================================");
            //}
            //catch (Exception)
            //{
            //    Debug.WriteLine("ServerOnClientDisconnected  Exception");
            //    Debug.WriteLine("============================================================");
            //}
        }

        private void ServerOnClientConnected(object sender, ServerEventArgs e)
        {
            Debug.WriteLine("============================================================");
            Debug.WriteLine("ServerOnClientConnected");
            Debug.WriteLine("============================================================");
        }

        private void OnExitMainForm()
        {
            try
            {
                PocClient.logonOrLogout(0);

                UnregisterHotKey(this.Handle, 200); //注销热键                

                /*
                if (fullIMWin != null && client.IsCreateIMForm == true)
                    fullIMWin.Close();       
                */
                

                SystemExit = true;

                if (Timer_GC != null)
                    Timer_GC.Dispose();

                this.EndFenceAlarmTimer(true);

                ReleaseWaveRecord();

                client.Exit();

                client.StopHeartBeat();
                gpsValidQueryActive = false;

                StopVideoQuery();
                StopVideoTick();
                client.SendMessage((new Data(MyType.TYPE_REALASE_MIC)).ToByte());
                //2017.11.27 修改以下的登出报文, 因为考虑监听多个群组，要求报刚开始的第一个组
                //因为真的发最后登入的组，那么服务端会将它从内存结构中移除了，下次重登后就不会在多群组监听
                //模式下，听不到该组的对讲了
                if (FirstGROUP_ID > 0)
                    client.SendMessage(
                    (new Data()).LogoutMessageEncode(FirstGROUP_ID, LocalSharedData.CURRENTUser.user_id)
                );
                else
                    client.SendMessage(
                   (new Data()).LogoutMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
               );

                client.CloseConnection();
                client.ReleaseWavePlayDevices();
                client.Release();

                for (int i = 0; i < CURRENT_VIDEOLIST.Count; i++)
                {
                    //                   

                }

                if (ws_check_time != null)
                    ws_check_time.Dispose();

                webSocket_Manual_shutdown = true; //手动关闭

                if (webSocket != null)
                    webSocket.Close();
                webSocket = null;
            }
            catch (MissingMethodException me)
            {

            }

        }

        private void ControlMainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

            OnExitMainForm();

        }



        //#region // 收到对方  发送过来的 文本消息
        //public void newTextMsg( byte[] content, string title, Font titleFont, Color titleColor ) //收到对方发送过来文本消息
        //{
        //    LanMsg.MyExtRichTextBox rich = new MyExtRichTextBox();
        //    rich.AppendText(title);
        //    rich.Font      = titleFont;
        //    rich.ForeColor = titleColor;
        //    this.RTBRecord.AppendRtf(rich.Rtf);
        //    this.RTBRecord.AppendText("  ");
        //    LanMsg.ClassTextMsg textMsg = ( new ClassSerializers().DeSerializeBinary(new System.IO.MemoryStream(content)) as ClassTextMsg );
        //    int iniPos = this.RTBRecord.TextLength;//获得当前记录richBox中最后的位置
        //    rich.Clear();
        //    rich.Dispose();
        //    if (textMsg.ImageInfo != "")//如果消息中有图片，则添加图片
        //    {
        //        string[] imagePos = textMsg.ImageInfo.Split('|');
        //        int addPos  = 0;//
        //        int currPos = 0;//当前正要添加的文本位置
        //        int textPos = 0;//
        //        for (int i = 0; i < imagePos.Length - 1; i++)
        //        {
        //            string[] imageContent = imagePos[i].Split(','); // 获得图片所在的位置、图片名称、图片宽、高
        //            currPos = Convert.ToInt32(imageContent[0]);
        //            this.RTBRecord.AppendText(textMsg.MsgContent.Substring(textPos, currPos - addPos));
        //            this.RTBRecord.SelectionStart = this.RTBRecord.TextLength;
        //            textPos += currPos - addPos;
        //            addPos  += currPos - addPos;
        //            LanMsg.MyPicture pic = new MyPicture();
        //            pic.BackColor = this.RTBRecord.BackColor;
        //            if ( Convert.ToUInt32(imageContent[1]) < 96 )//如果发送的图片是自带的，则已知尺寸
        //            {
        //                pic.Image    = System.Drawing.Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("LanMsg.Resources." + imageContent[1] + ".gif"));
        //                pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
        //            }
        //            else// 如果发送的图片是自定义的，则需要知道尺寸
        //            {
        //                pic.Image    = System.Drawing.Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("LanMsg.Resources.ErrorImage.GIF"));
        //                pic.Tag      = imageContent[1];
        //                pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
        //                pic.Size     = new Size(Convert.ToInt32(imageContent[2]), Convert.ToInt32(imageContent[3]));
        //                this.ArrivalGifs.add(pic);
        //            }
        //            this.RTBRecord.InsertMyControl(pic);
        //            System.Drawing.ImageAnimator.Animate(pic.Image, new System.EventHandler(this.RTBRecordOnFrameChanged));
        //            addPos++;
        //        }
        //        this.RTBRecord.AppendText( textMsg.MsgContent.Substring(textPos, textMsg.MsgContent.Length - textPos) + "\n" );
        //    }
        //    else//如果消息中没有图片，则直接添加消息文本
        //    {
        //        this.RTBRecord.AppendText(textMsg.MsgContent + "\n");
        //    }
        //    this.RTBRecord.Focus();
        //    this.RTBRecord.Select(iniPos, this.RTBRecord.TextLength - iniPos);
        //    this.RTBRecord.SelectionFont  = textMsg.font;
        //    this.RTBRecord.Select(iniPos, this.RTBRecord.TextLength - iniPos);
        //    this.RTBRecord.SelectionColor = textMsg.color;
        //    this.RTBSend.Focus();
        //    //this.currUserInfo = this.FormMain.formMain.findUser(this.Tag.ToString());
        //    // if (this.currUserInfo != null)
        //    //    this.FormMain.formMain.sendMsgToOneUser(new LanMsg.Controls.ClassMsg(6, this.FormMain.formMain.selfInfo.ID, null), currUserInfo.IP, currUserInfo.Port);//告诉发消息的联系人已经收到发送的消息
        //    //this.FormMain.formMain.MsgAddToDB(textMsg.MsgContent, this.Tag.ToString(), this.FormMain.formMain.selfInfo.ID, this.FormMain.formMain.selfInfo.AssemblyVersion, System.DateTime.Now.ToString(), textMsg.ImageInfo, true);//将消息添加进数据库
        //}
        //#endregion
        //#region// 收到对方 发送来的自定义的GIF图片
        //public void newMsg( byte[] content, string title, Font titleFont, Color titleColor ) //收到对方发送过来的GIF图片
        //{
        //    System.IO.MemoryStream Ms    = new System.IO.MemoryStream(content);
        //    LanMsg.ClassSendImage sImage = (new ClassSerializers().DeSerializeBinary(Ms)) as ClassSendImage;
        //    Ms.Close();
        //    LanMsg.MyPicture pic = this.findPic( Convert.ToString(sImage.ID), this.ArrivalGifs );
        //    if (pic != null)
        //    {
        //        System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(Application.StartupPath + "\\ArrivalImage");
        //        if (!dInfo.Exists)
        //            dInfo.Create();
        //        pic.Image = sImage.Image;
        //        pic.Image.Save(Application.StartupPath + "\\ArrivalImage\\" + pic.Tag.ToString() + ".gif");
        //        pic.Size = pic.Image.Size;
        //        System.Drawing.ImageAnimator.Animate(pic.Image, new System.EventHandler(this.RTBRecordOnFrameChanged));
        //        pic.Refresh();
        //        pic.Invalidate();
        //        this.RTBRecord.Invalidate();
        //        this.RTBRecord.Refresh();
        //    }
        //}
        //#endregion
        //#region // 收到对方  发送过来的Rtf消息
        //public void newMsg( string rtfMsg, string title, Font titleFont, Color titleColor) //收到对方发送过来的Rtf消息
        //{
        //    System.Windows.Forms.RichTextBox rich = new RichTextBox();
        //    rich.AppendText(title);
        //    rich.Font      = titleFont;
        //    rich.ForeColor = titleColor;
        //    this.RTBRecord.AppendRtf(rich.Rtf);
        //    rich.Clear();
        //    rich.Dispose();
        //    this.RTBRecord.AppendTextAsRtf("");
        //    this.RTBRecord.AppendRtf(rtfMsg);
        //}
        //#endregion



        /// <summary>
        /// 来自IM的文本消息的发送请求
        /// </summary>
        /// <param name="im_message_content"></param>



        /*
         * 封装文件数据成ChatMessage
         */
        private ChatMessage prepareFileMessageData(string file, int msgType, String filetype, String content)
        {
            System.IO.FileInfo fInfo = new System.IO.FileInfo(file);

            ChatMessage chatmsg = new ChatMessage();
            chatmsg.setCharsetname("GB2312");

            String date = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
            String fileMD5 = Utils.GetMD5HashFromFile(file);// 文件关联时间，生成唯一MD5码

            chatmsg.setDatetime(date);
            chatmsg.setGroupid(LocalSharedData.CURRENTUser.group_id + "");
            chatmsg.setUserid(LocalSharedData.CURRENTUser.user_id + "");
            chatmsg.setMsgtype("" + msgType);
            chatmsg.setMsgcontent(content);
            chatmsg.setFiletype(filetype);

            chatmsg.setFilename(fInfo.Name);
            chatmsg.setFilepath(fInfo.FullName);
            chatmsg.setFilesize(fInfo.Length);
            chatmsg.setFileID(LocalSharedData.CURRENTUser.group_id + "");// 文件ID暂时设为GUID
            chatmsg.setFileMD5(fileMD5);
            chatmsg.setFilekey(fileMD5);

            chatmsg.setCurrentSize(0);
            chatmsg.setPercent(0);
            chatmsg.setState(POCControlCenter.Filestate.Downloading);

            return chatmsg;
        }

        ////////////////////////////////////////////////////////////////////////////

        private void RTBSendOnFrameChanged(object sender, EventArgs e)
        {

        }




        //
        // WinFormsStringResource
        //













        private delegate void OnChatFileEventDelegate(ChatMessage chtMsg);
        public void OnReceiveFileRequest(ChatMessage chatmsg)
        {
            /*
            if (InvokeRequired)
            {
                OnChatFileEventDelegate onmsgevent = new OnChatFileEventDelegate(ReceiveFileRequest);
                this.Invoke(onmsgevent, new object[] { chatmsg });
                return;
            }
            else
            {
                ReceiveFileRequest(chatmsg);
            }
            */
        }

        /*
        public void ReceiveFileRequest(ChatMessage chatmsg) //接收文件请求
        {
            //filetype=file 表示是非图片文件，即普通文件
            //MessageBox.Show("收到信息: ReceiveFileRequest");
            Debug.WriteLine(chatmsg.toString());

            //if (this.FullMsgPopup)
            //{
            //    this.CallMsgPopup("MESSAGE_FILE", "[" + LocalSharedData.GetUserName(Int32.Parse(chatmsg.getUserid()))
            //    + "/" + WinFormsStringResource.Main_labelACCOUNT_sub2
            //    + ":" + LocalSharedData.GetGroupName(Int32.Parse(chatmsg.getGroupid()))
            //    + "]", "");
            //}

            SDKClient.Model.MessagePackage imMessage;

            if (chatmsg.getFiletype().Equals("picture"))
            {
                //将消息发向WPF IM窗口
                imMessage = new SDKClient.Model.MessagePackage();
                imMessage.id = Guid.NewGuid().ToString();
                imMessage.code = 0;
                imMessage.error = "";
                imMessage.time = DateTime.Now;
                imMessage.from = chatmsg.getUserid();
                imMessage.to = chatmsg.getGroupid();
                SDKClient.Model.message data = new SDKClient.Model.message();
                data.type = "groupChat";
                data.subType = "img";
                data.chatType = 0;

                SDKClient.Model.ImgBody imgBody = new SDKClient.Model.ImgBody();
                imgBody.fileName = chatmsg.getFilekey() + "/" + chatmsg.getFilename();
                imgBody.id = chatmsg.getFilekey() + "/" + chatmsg.getFilename();
                imgBody.smallId = chatmsg.getFilekey() + "/" + chatmsg.getFilename();
                data.body = imgBody;

                data.senderInfo = new SDKClient.Model.message.SenderInfo
                {
                    userName = LocalSharedData.GetUserName(Convert.ToInt32(chatmsg.getUserid()))
                };
                data.groupInfo = new SDKClient.Model.group
                {
                    groupId = Convert.ToInt32(chatmsg.getGroupid())
                };

                imMessage.data = data;


            }
            else
            {
                //非图片文件

                //将消息发向WPF IM窗口
                imMessage = new SDKClient.Model.MessagePackage();
                imMessage.id = Guid.NewGuid().ToString();
                imMessage.code = 0;
                imMessage.error = "";
                imMessage.time = DateTime.Now;
                imMessage.from = chatmsg.getUserid();
                imMessage.to = chatmsg.getGroupid();
                SDKClient.Model.message data = new SDKClient.Model.message();
                data.type = "groupChat";
                data.subType = "file";
                data.chatType = 0;

                SDKClient.Model.fileBody fileBody = new SDKClient.Model.fileBody();
                fileBody.fileName = chatmsg.getFilekey() + "/" + chatmsg.getFilename();
                fileBody.id = chatmsg.getFilekey() + "/" + chatmsg.getFilename();
                fileBody.fileSize = Convert.ToInt32(chatmsg.getFilesize());

                data.body = fileBody;

                data.senderInfo = new SDKClient.Model.message.SenderInfo
                {
                    userName = LocalSharedData.GetUserName(Convert.ToInt32(chatmsg.getUserid()))
                };
                data.groupInfo = new SDKClient.Model.group
                {
                    groupId = Convert.ToInt32(chatmsg.getGroupid())
                };

                imMessage.data = data;

            }

            AppData.MainMV.ChatListVM.ReceiveMsg(imMessage);


        }
        
        */

        /// <summary>
        /// 一般对讲时的说话最大时间(秒)
        /// </summary>
        private const int ROBMICTIME_LENGTH = 60;  //普通对讲改为30秒
        private int OutTime = 0;

        #region // 检查 消息是否发送成功 timer事件
        private delegate void POCrobMICTimerDelegate(bool t);
        private void POCrobMICTimer(bool t) // 启用 或 禁用 发送功能
        {
            OutTime = 0;
            if (!t)
            {
                this.timerPOCrobMIC.Interval = 1000;
                this.timerPOCrobMIC.Start();
            }

            if (t)
            {
                this.timerPOCrobMIC.Stop();
                String text = "";

                //广播模式对讲不用计时器
                if (SINGLE_POC_ENABLE && onePocForm != null && !onePocForm.IsDisposed)
                    onePocForm.SetPlayUIStatus(text);
                else if (BROADCAST_POC_ENABLE && broadCastForm != null && !broadCastForm.IsDisposed)
                    broadCastForm.SetPlayUIStatus(text);
                else if (TEMPTALK_POC_ENABLE && tempTalkForm != null && !tempTalkForm.IsDisposed)
                    tempTalkForm.SetPlayUIStatus(text);
                else
                    SetPlayUIStatus(text);

            }
        }

        /// <summary>
        /// t为false表示开始计时, t为true不用本地计时
        /// </summary>
        /// <param name="t"></param>
        private void EndPOCrobMICTimer(bool t) // 启用 或 禁用 发送功能
        {
            if (InvokeRequired)
            {
                POCrobMICTimerDelegate uld = new POCrobMICTimerDelegate(POCrobMICTimer);
                this.Invoke(uld, new object[] { t });
                return;
            }
            POCrobMICTimer(t);
        }
        private void timerPOCrobMIC_Tick(object sender, EventArgs e)
        {
            OutTime++;

            string s1 = WinFormsStringResource.Main_timerPOCrobMIC_s1;
            string s2 = WinFormsStringResource.Main_timerPOCrobMIC_s2;

            String text = s1 + (ROBMICTIME_LENGTH - 1 - OutTime).ToString() + s2;
            //2017.8.11
            if (SINGLE_POC_ENABLE && onePocForm != null && !onePocForm.IsDisposed)
                onePocForm.SetPlayUIStatus(text);
            else if (BROADCAST_POC_ENABLE && broadCastForm != null && !broadCastForm.IsDisposed)
                broadCastForm.SetPlayUIStatus(text);

            else if (TEMPTALK_POC_ENABLE && tempTalkForm != null && !tempTalkForm.IsDisposed)
                tempTalkForm.SetPlayUIStatus(text);

            else
                SetPlayUIStatus(text);

            if (OutTime >= ROBMICTIME_LENGTH - 1)
            {
                m_blUIStalk = true;
                SendTalkCommand();
                EndPOCrobMICTimer(true);
            }
        }
        #endregion

        private void WebBrowserBAIDUMAP_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {


        }

        private void webBrowserMAPTRACK_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //WebBrowser web = (WebBrowser)sender;
            //if (web == this.webBrowserMAPTRACK)
            //    ControlMainForm_ShowGroupLocation( LocalSharedData.CURRENTUser.group_id, webBrowserMAPTRACK);
        }
        private void webBrowserFenceMAP_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            /*
            WebBrowser web = (WebBrowser)sender;
            if (web == this.webBrowserFenceMAP)
            {
                ControlMainForm_ShowGroupLocation(LocalSharedData.CURRENTUser.group_id, webBrowserFenceMAP);
                this.webBrowserFenceMAP.Document.Window.Error += Window_Error;
            }
            */
        }
        
        
        private void CALLcontextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //2018.1.23 是通过对讲群组,还是通过搜索进入的

            TreeNode node = null;
            if (SELECT_TREEVIEW_MODE == 1)
                node = this.treeViewGROUP.SelectedNode;
            else if (SELECT_TREEVIEW_MODE == 2)
                node = this.treeViewSearch.SelectedNode;
            else if (SELECT_TREEVIEW_MODE == 3)
                node = this.treeViewContact.SelectedNode;

            if (node == null) return;

            if (((ContextMenuStrip)sender).Items["toolStripMenuItemTalk"] == e.ClickedItem)
            {
                //音频通话   
                String userid = node.Tag is User ? ((User)node.Tag).userId.ToString() : "";
                if (userid == "")
                {
                    MessageBox.Show("用户节点分析报错");
                    return;
                }


                string username = LocalSharedData.GetUserName(Convert.ToInt32(userid));

                if (agoraVideoForm == null || agoraVideoForm.IsDisposed)
                {
                    agoraVideoForm =
                                        new JoinChannelVideoView(client, LocalSharedData.CURRENTUser.user_id + "",
                                        LocalSharedData.CURRENTUser.user_id + "", LocalSharedData.CURRENTUser.user_name,
                                        userid, username, "desc",

                                        new Action(delegate { }), false, (short)1);
                }

                agoraVideoForm.ShowDialog();
                agoraVideoForm = null;

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemView"] == e.ClickedItem)
            {
                //视频通话

                String userid = node.Tag is User ? ((User)node.Tag).userId.ToString() : "";
                if (userid == "")
                {
                    MessageBox.Show("用户节点分析报错");
                    return;
                }


                string username = LocalSharedData.GetUserName(Convert.ToInt32(userid));


                if (agoraVideoForm == null || agoraVideoForm.IsDisposed)
                {
                    agoraVideoForm =
                                        new JoinChannelVideoView(client, LocalSharedData.CURRENTUser.user_id + "",
                                        LocalSharedData.CURRENTUser.user_id + "", LocalSharedData.CURRENTUser.user_name,
                                        userid, username, "desc",

                                        new Action(delegate { }), false, (short)2);
                }

                agoraVideoForm.ShowDialog();
                agoraVideoForm = null;


            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemCreateTmpGroup"] == e.ClickedItem)
            {

                //
                string grpcaption = "[单呼]"+node.Text+"--"+LocalSharedData.CURRENTUser.user_name;  //对方的名称
                string userids = "" + ((User)node.Tag).userId + "," + LocalSharedData.CURRENTUser.user_id; //双方的userid串接


                executeSinglePOC(grpcaption, userids);

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemMoni"] == e.ClickedItem)
            {
                //
                String userid = node.Tag is User ? ((User)node.Tag).userId.ToString() : "";
                if (userid == "")
                {
                    MessageBox.Show("用户节点分析报错");
                    return;
                }

                String moni_resolv=readRunPara("video", "moni_resolv");
                JObject staff = new JObject();
                staff.Add(new JProperty("toUserIds", userid));
                staff.Add(new JProperty("moniResolv", moni_resolv));
                //MessageBox.Show(JsonConvert.SerializeObject(staff));
                client.SendMessage(
                               (new Data()).AVRemoteMoniMesssageEncode((byte)1,  LocalSharedData.CURRENTUser.user_id,
                                JsonConvert.SerializeObject(staff) ));       
                

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemRecordQuery"] == e.ClickedItem)
            {
                //toolStripMenuItemRecordQuery
                //打开查询对话框，并且预设一些查询条件
                if (full_RecQueryForm == null)
                {
                    full_RecQueryForm = new RecQueryForm(this.ZoneInterval_UserServer);
                }

                full_RecQueryForm.setQueryValueByPerson(((User)node.Tag).userId.ToString());
                full_RecQueryForm.ShowDialog();


            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemTrack"] == e.ClickedItem)
            {
                //toolStripMenuItemTrack
                //toolStripMenuItemTrack
                JSCallFunction_PersonTrackPlayBack(((User)node.Tag).userId.ToString(), node.Text);


            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemGPSCommand"] == e.ClickedItem)
            {
                //即时定位动作                
                TreeNode p = null;
                if (SELECT_TREEVIEW_MODE == 1)
                    p = node.Parent;
                else if (SELECT_TREEVIEW_MODE == 2)
                {
                    //从localshare, 以下有问题，在组织架构模式下可能出问题
                    foreach (TreeNode nodeb in treeViewGROUP.Nodes)
                    {
                        foreach (TreeNode nodec in nodeb.Nodes)
                        {
                            if ((int)nodec.Tag == (int)node.Tag)
                            {
                                p = nodeb;
                                break;
                            }
                        }
                    }

                }

                if (p != null)
                {
                    if (requestGPSForm == null)
                        requestGPSForm = new RequestGPSForm();
                    if (requestGPSForm != null)
                    {
                        if (requestGPSForm.ShowDialog() == DialogResult.OK)
                        {
                            if (requestGPSForm.action_cmd == 2)
                            {
                                //public int Full_GPSCommand_Mode = 0;     //0 表示组级定位, 1表示个人定位
                                Full_GPSCommand_Mode = 1;
                                Full_GPSCommand_GROUP_ID = ((Group)p.Tag).group_id;
                                Full_GPSCommand_USER_ID = ((User)node.Tag).userId;
                                int accuracy = requestGPSForm.cbAccuracy.SelectedIndex;
                                client.SendMessage(
                        (new Data()).GPSCommandMessageEncode(LocalSharedData.CURRENTUser.group_id,
                        ((User)node.Tag).userId, Convert.ToByte(accuracy), LocalSharedData.CURRENTUser.user_id));
                            }
                            else if (requestGPSForm.action_cmd == 1)
                            {
                                //点中个人节点
                                //先显示地图中坐标                    
                                ControlMainForm_ShowUserLocation(((User)node.Tag).userId, ((Group)p.Tag).group_id);
                                //再显示表格
                                List<Location> us = HttpAPI.queryLocationByUserid(((User)node.Tag).userId, ((Group)p.Tag).group_id);
                                if (us != null)
                                {
                                    BLocation.Clear();
                                    foreach (Location loc in us)
                                    {
                                        //时间/状态处理
                                        if (loc.curtime != null && loc.curtime > 0)
                                        {
                                            long timesec = (long)loc.curtime / 1000;
                                            DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(timesec);
                                            //转成本地时间                                                                      


                                            DateTime localdt = utcdt.ToLocalTime();
                                            //2017.8.16 加入时区差异的处理                                    
                                            localdt = localdt.AddHours(ZoneInterval_UserServer);

                                            loc.gpstime = localdt.ToString("yyyy-MM-dd HH:mm:ss");

                                        }
                                        if (loc.logon == 1)
                                            loc.state = WinFormsStringResource.StatusOnline;
                                        else
                                            loc.state = WinFormsStringResource.StatusOffline;

                                        if (loc.flag_record.Equals("Y"))
                                            loc.flag_record_str = WinFormsStringResource.VoiceRecord_OPEN;
                                        else
                                            loc.flag_record_str = WinFormsStringResource.VoiceRecord_CLOSE;

                                        if (loc.myclass == 0)
                                            loc.myclass_str = WinFormsStringResource.VoiceRight_0;
                                        else if (loc.myclass == 1)
                                            loc.myclass_str = WinFormsStringResource.VoiceRight_1;
                                        else
                                            loc.myclass_str = WinFormsStringResource.VoiceRight_2;

                                        if (loc.life_state == 1)
                                            loc.life_state_str = WinFormsStringResource.LifeState_1;
                                        else if (loc.life_state == 0)
                                            loc.life_state_str = WinFormsStringResource.LifeState_0;
                                        else
                                            loc.life_state_str = WinFormsStringResource.LifeState_NE1;

                                        //                                

                                        BLocation.Add(loc);
                                    }

                                    changeDatagridviewRowColor();
                                }

                                setTabControlPageDisplay(0);
                                //tabControl1.SelectedIndex = 0;

                            }

                        }
                    }
                }

            }
            
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemUserPriv"] == e.ClickedItem)
            {
                //2019.06.01 将三个权限集成在一起设置
                UserPrivSetInfoForm cfgForm = new UserPrivSetInfoForm();
                cfgForm.oper_field = 1;
                TreeNode p = node.Parent;
                //User user = HttpAPI.getUserInfoByUserid(((User)node.Tag).userId);
                User user = null;
                PttUserOneResponse userResp = PocClient.queryPttUserByUserId(((User)node.Tag).userId);
                if (userResp != null)
                    user = userResp.data;

                if (user == null) return;
                cfgForm.labelUserName.Text = user.userName;
                //显示录音
                ArrayList lists_cbRecordlist = new ArrayList();

                lists_cbRecordlist.Add(new MyKeyValue("Y", WinFormsStringResource.VoiceRecord_OPEN));
                lists_cbRecordlist.Add(new MyKeyValue("N", WinFormsStringResource.VoiceRecord_CLOSE));

                cfgForm.cbSwitchRecord.DisplayMember = "pValue";
                cfgForm.cbSwitchRecord.ValueMember = "pKey";
                cfgForm.cbSwitchRecord.DataSource = lists_cbRecordlist;
                cfgForm.cbSwitchRecord.SelectedValue = user.flagRecord;

                //显示对讲权限: 禁言、对讲等
                ArrayList lists_cbLifelist = new ArrayList();

                lists_cbLifelist.Add(new MyKeyValue("0", WinFormsStringResource.LifeState_0));
                lists_cbLifelist.Add(new MyKeyValue("1", WinFormsStringResource.LifeState_1));
                lists_cbLifelist.Add(new MyKeyValue("-1", WinFormsStringResource.LifeState_NE1));

                cfgForm.cbSwitchLife.DisplayMember = "pValue";
                cfgForm.cbSwitchLife.ValueMember = "pKey";
                cfgForm.cbSwitchLife.DataSource = lists_cbLifelist;
                cfgForm.cbSwitchLife.SelectedValue = user.lifeState.ToString();

                //显示对讲话权:普通、最高等
                ArrayList lists_cbVoiceRightlist = new ArrayList();

                lists_cbVoiceRightlist.Add(new MyKeyValue("0", WinFormsStringResource.VoiceRight_0));
                lists_cbVoiceRightlist.Add(new MyKeyValue("1", WinFormsStringResource.VoiceRight_1));
                lists_cbVoiceRightlist.Add(new MyKeyValue("2", WinFormsStringResource.VoiceRight_2));

                cfgForm.cbSwitchRight.DisplayMember = "pValue";
                cfgForm.cbSwitchRight.ValueMember = "pKey";
                cfgForm.cbSwitchRight.DataSource = lists_cbVoiceRightlist;

                cfgForm.cbSwitchRight.SelectedValue = user.myclass.ToString();

                //保存

                if (cfgForm.ShowDialog() == DialogResult.OK)
                {
                    DataEntity.ExecuteResult res = null;
                    //录音
                    if (cfgForm.cbSwitchRecord.SelectedValue.ToString().Equals("Y"))
                        res = HttpAPI.updateRTState(((User)node.Tag).userId, 1, 1);
                    else
                        res = HttpAPI.updateRTState(((User)node.Tag).userId, 1, 0);

                    //禁言权限
                    res = HttpAPI.updateRTState(((User)node.Tag).userId, 3, Convert.ToInt32(cfgForm.cbSwitchLife.SelectedValue.ToString()));
                    //对讲话权
                    res = HttpAPI.updateRTState(((User)node.Tag).userId, 2, Convert.ToInt32(cfgForm.cbSwitchRight.SelectedValue.ToString()));

                    if (res != null && res.error == 0)
                    {
                        if (user.logon == 1 && Convert.ToInt32(cfgForm.cbSwitchLife.SelectedValue.ToString()) != 1)
                        {
                            node.SelectedImageIndex = 14;
                            node.ImageIndex = 14;
                        }
                        else if (user.logon == 1)
                        {
                            node.SelectedImageIndex = 3;
                            node.ImageIndex = 3;
                        }

                        //重新刷新
                        ((User)node.Tag).lifeState = Convert.ToInt32(cfgForm.cbSwitchLife.SelectedValue.ToString());
                        ((User)node.Tag).flagRecord = cfgForm.cbSwitchRecord.SelectedValue.ToString();
                        ((User)node.Tag).myclass = Convert.ToInt32(cfgForm.cbSwitchRight.SelectedValue.ToString());
                        User myUser = LocalSharedData.UserAll[((User)node.Tag).userId];
                        if (myUser != null)
                        {
                            myUser.lifeState = Convert.ToInt32(cfgForm.cbSwitchLife.SelectedValue.ToString());
                            myUser.flagRecord = cfgForm.cbSwitchRecord.SelectedValue.ToString();
                            myUser.myclass = Convert.ToInt32(cfgForm.cbSwitchRight.SelectedValue.ToString());
                        }

                        //传播到
                        NetUser_EnterGroup(LocalSharedData.CURRENTUser.group_id, ((User)node.Tag).userId);
                        //以下从重考虑
                        //doUpdateMapLoc();

                    }
                    else
                    {
                        MessageBox.Show("operation fail!");
                    }
                }

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemLifeStateChange"] == e.ClickedItem)
            {
                //去除了

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemGpsSetInfo"] == e.ClickedItem)
            {
                //GPS设置
                int userId = ((User)node.Tag).userId;
                User user;
                LocalSharedData.UserAll.TryGetValue(userId, out user);
                if (user == null)
                    return;

                //
                if (userGpsConfigForm == null)
                    userGpsConfigForm = new UserGPSConfigForm();

                userGpsConfigForm.labUserName.Text = user.userName;

                setSelectIndexByValue(userGpsConfigForm.flag_autoLocation_str, user.flagAutoLocation);
                setSelectIndexByValue(userGpsConfigForm.priv_hideLocSwitch_str, user.privHideLocSwitch);
                setSelectIndexByValue(userGpsConfigForm.locationMode_str, user.locationMode + "");
                setSelectIndexByValue(userGpsConfigForm.locationInterval_str, user.locationInterval + "");

                if (userGpsConfigForm.ShowDialog() == DialogResult.OK)
                {

                    User userUpdate = new User();
                    userUpdate.userId = userId;
                    userUpdate.flagAutoLocation = userGpsConfigForm.flag_autoLocation_str.SelectedIndex == -1 ? "N" : userGpsConfigForm.flag_autoLocation_str.SelectedValue.ToString();
                    userUpdate.privHideLocSwitch = userGpsConfigForm.priv_hideLocSwitch_str.SelectedIndex == -1 ? "N" : userGpsConfigForm.priv_hideLocSwitch_str.SelectedValue.ToString();
                    userUpdate.locationMode = userGpsConfigForm.locationMode_str.SelectedIndex == -1 ? 2 : Convert.ToInt32( userGpsConfigForm.locationMode_str.SelectedValue.ToString());
                    userUpdate.locationInterval = userGpsConfigForm.locationInterval_str.SelectedIndex == -1 ? 0 : Convert.ToInt32(userGpsConfigForm.locationInterval_str.SelectedValue.ToString());

                   ResponseBase resp=  PocClient.updateUserInfo(userUpdate);
                  
                    //
                    if (resp.code== 0)
                    {
                        //更新成功,再查询一次       
                        user.flagAutoLocation = userGpsConfigForm.flag_autoLocation_str.SelectedIndex == -1 ? "N" : userGpsConfigForm.flag_autoLocation_str.SelectedValue.ToString();
                        user.privHideLocSwitch = userGpsConfigForm.priv_hideLocSwitch_str.SelectedIndex == -1 ? "N" : userGpsConfigForm.priv_hideLocSwitch_str.SelectedValue.ToString();
                        user.locationMode = Convert.ToInt32(userGpsConfigForm.locationMode_str.SelectedIndex == -1 ? "2" : userGpsConfigForm.locationMode_str.SelectedValue.ToString());
                        user.locationInterval = Convert.ToInt32(userGpsConfigForm.locationInterval_str.SelectedIndex == -1 ? "0" : userGpsConfigForm.locationInterval_str.SelectedValue.ToString());

                    }
                    else
                    {
                        //更新失败
                        MessageBox.Show("更新失败");
                    }

                }

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemForceRemove"] == e.ClickedItem)
            {
                //强拆
                //如果是固定组是不允许调度台强拆的
                TreeNode p = node.Parent;
                foreach (Group grp in LocalSharedData.UserAllGROUP)
                {
                    if (grp.group_id == ((Group)p.Tag).group_id)
                    {
                        MessageBox.Show(WinFormsStringResource.FocreRemoveFrmFixGroupError);
                        return;
                    }
                }
                //调用强拆操作
                ResponseBase res = null;
                res = PocClient.groupUserChange(((Group)p.Tag).group_id, 1, 0,
                   Convert.ToString(((User)node.Tag).userId));

                if (res != null && res.error == 0)
                {
                    //强拆成功
                    p.Nodes.Remove(node);
                }
                else
                {
                    MessageBox.Show(res.errorMsg);
                }

            }
        }






        /// <summary>
        /// 对讲单呼本质也是一个临时组实现的
        /// </summary>
        /// <param name="grpcaption"></param>
        /// <param name="userids"></param>
        public void executeSinglePOC(string grpcaption, string userids)
        {          

            GroupTempDto dto = new GroupTempDto();
            dto.groupName = grpcaption;
            dto.userIds = userids;
            dto.ownerId = LocalSharedData.CURRENTUser.user_id;
            dto.priv = 1;    //调度员发起的为1, 终端发起的为0

            Group grptmp = null;
            TempGroupResponse resp_t = PocClient.createTmpGroup(dto);
            if (resp_t != null)
            {
                grptmp = new Group();
                grptmp.group_id = resp_t.data.groupId;
                grptmp.group_name = resp_t.data.groupName;
                grptmp.owner_id = resp_t.data.ownerId.Value;
                grptmp.user_ids = resp_t.data.userIds;

            }

            if (grptmp == null)
            {
                MessageBox.Show("你的帐号没有开通单呼权限，请联系管理员");
                return;
            }

            if (onePocForm == null || onePocForm.IsDisposed)
                onePocForm = new OnePOCForm(client);

            onePocForm.Owner = this;

            onePocForm.Text = grpcaption;

            onePocForm.groupid = grptmp.group_id;
            onePocForm.groupname = userids;

            onePocForm.labelPlayerName.Text = "";
            SINGLE_POC_ENABLE = true;  //进入对讲单呼模式


            if (onePocForm.ShowDialog() == DialogResult.Cancel)
            {
                SINGLE_POC_ENABLE = false;  //退出对讲单呼模式
            }

            //恢复到以前的组
            client.SendMessage(
             (new Data()).ReportMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
         );

            client.AEC_State = 0;  //设为初始态

            onePocForm = null;

        }       



        private delegate void DoGroupGPSQuery_2Delegate();
        private void doGroupGPSQuery_2()
        {
            if (this.InvokeRequired)
            {
                DoGroupGPSQuery_2Delegate uld = new DoGroupGPSQuery_2Delegate(doGroupGPSQuery_2);
                this.Invoke(uld, new object[] { });
                return;
            }

            MessageBox.Show("显示组级点位结果");

            List<Location> us = null;
            us = HttpAPI.queryLocationByGroup(LocalSharedData.CURRENTUser.user_id,
                Full_GPSCommand_GROUP_ID, "");
            if (us != null)
            {
                BLocation.Clear();
                foreach (Location loc in us)
                {
                    //时间/状态处理
                    if (loc.curtime != null && loc.curtime > 0)
                    {
                        long timesec = (long)loc.curtime / 1000;
                        DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(timesec);
                        //转成本地时间  
                        DateTime localdt = utcdt.ToLocalTime();

                        //2017.8.16 加入时区差异的处理                                    
                        localdt = localdt.AddHours(ZoneInterval_UserServer);

                        loc.gpstime = localdt.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (loc.logon == 1)
                        loc.state = WinFormsStringResource.StatusOnline;
                    else
                        loc.state = WinFormsStringResource.StatusOffline;

                    if (loc.flag_record.Equals("Y"))
                        loc.flag_record_str = WinFormsStringResource.VoiceRecord_OPEN;
                    else
                        loc.flag_record_str = WinFormsStringResource.VoiceRecord_CLOSE;

                    if (loc.myclass == 0)
                        loc.myclass_str = WinFormsStringResource.VoiceRight_0;
                    else if (loc.myclass == 1)
                        loc.myclass_str = WinFormsStringResource.VoiceRight_1;
                    else
                        loc.myclass_str = WinFormsStringResource.VoiceRight_2;

                    if (loc.life_state == 1)
                        loc.life_state_str = WinFormsStringResource.LifeState_1;
                    else if (loc.life_state == 0)
                        loc.life_state_str = WinFormsStringResource.LifeState_0;
                    else
                        loc.life_state_str = WinFormsStringResource.LifeState_NE1;

                    BLocation.Add(loc);
                }
                changeDatagridviewRowColor();
            }

            //tabControl1.SelectedIndex = 0;
            setTabControlPageDisplay(0);
            ControlMainForm_ShowGroupLocation(Full_GPSCommand_GROUP_ID, this.webBrower);

        }


        private void doGroupGPSQuery(object obj)
        {

            Int32 locationMode = (Int32)obj;
            if (locationMode == 0)
            {
                Thread.Sleep(8000);
            }
            else if (locationMode == 1)
            {
                Thread.Sleep(5000);
            }
            else if (locationMode == 2)
            {
                Thread.Sleep(10000);
            }
            //dataGridViewMaploc
            doGroupGPSQuery_2();

        }

      
        private void treeViewGROUP_MouseDown(object sender, MouseEventArgs e)
        {
            pi = new Point(e.X, e.Y);
            Control c = sender as Control;
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = this.treeViewGROUP.GetNodeAt(e.X, e.Y);
                if (node == null) return;

                SELECT_TREEVIEW_MODE = 1;

                //选中鼠标右键按下所在的节点
                this.treeViewGROUP.SelectedNode = node;
                // Control c = node as Control;
                //判断组或个人，如果是组且是临时组则出现上下文菜单
                if (node.Tag != null && node.Tag is Group)    //node.Level == 0
                {
                     
                    //2018.08.19 修改
                    Group curGrp = (Group)node.Tag;

                    GroupService.GroupContextMenuItemDisplay(curGrp.group_type, this.GRPcontextMenuStrip);
                    this.GRPcontextMenuStrip.Show(c, e.Location);                    


                }
                else if (node.Tag != null && node.Tag is User)    //node.Level == 1
                {
                    if (((User)(node.Tag)).userId != LocalSharedData.CURRENTUser.user_id)
                    {
                        //不是自身帐号的节点
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemTalk"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemView"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemCreateTmpGroup"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemMoni"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemUserPriv"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemGpsSetInfo"].Enabled = true;


                        this.CALLcontextMenuStrip.Items["toolStripMenuItemForceRemove"].Enabled = true;


                    }
                    else
                    {

                        this.CALLcontextMenuStrip.Items["toolStripMenuItemTalk"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemView"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemCreateTmpGroup"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemMoni"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemUserPriv"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemGpsSetInfo"].Enabled = true;

                        this.CALLcontextMenuStrip.Items["toolStripMenuItemForceRemove"].Enabled = false;

                    }

                    this.CALLcontextMenuStrip.Show(c, e.Location);
                }
               

            }
        }

        private void treeViewGROUP_Click(object sender, EventArgs e)
        {
        }

        private void treeViewGROUP_MouseClick(object sender, MouseEventArgs e)
        {
            /*
            if (this.treeViewGROUP.Nodes.Count == 1)
            {
                TreeNode nd = this.treeViewGROUP.Nodes[0];
                TreeNode pd = this.treeViewGROUP.GetNodeAt(e.Location);
                if (nd.Equals(pd))
                {
                    ControlMainForm_ShowGroupLocation(LocalSharedData.CURRENTUser.group_id, webBrowserBAIDUMAP);
                    ControlMainForm_ShowGroupLocation(LocalSharedData.CURRENTUser.group_id, webBrowserMAPTRACK);
                    ControlMainForm_ShowGroupLocation(LocalSharedData.CURRENTUser.group_id, webBrowserFenceMAP);
                }
            }
            */
        }

        #region // 检查 timerFenceAlarm timer事件
        private static List<FenceAlarmNotify> mFenceAlarmData;
        private static int FenceAlarmCount = 0;
        private delegate void FenceAlarmTimerDelegate(bool t);

        private void FenceAlarmTimer(bool t) // 启用 或 禁用 发送功能
        {
            FenceAlarmCount = 0;

            Debug.WriteLine("FenceAlarmTimer Count:" + FenceAlarmCount);

            if (!t)
            {
                this.timerFenceAlarm.Interval = 60 * 1000;
                this.timerFenceAlarm.Start();
            }

            if (t)
            {
                this.timerFenceAlarm.Stop();
            }
        }

        private void EndFenceAlarmTimer(bool t) // 启用 或 禁用 发送功能
        {
            if (InvokeRequired)
            {
                FenceAlarmTimerDelegate uld = new FenceAlarmTimerDelegate(FenceAlarmTimer);
                this.Invoke(uld, new object[] { t });
                return;
            }

            FenceAlarmTimer(t);
        }

        private void timerFenceAlarm_Tick(object sender, EventArgs e)
        {

            //List<FenceAlarmNotify> temp = HttpAPI.QueryFenceAlarmInfo(LocalSharedData.CURRENTUser.user_id);
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("action", "query_fencealarm_num");
            param.Add("user_id", LocalSharedData.CURRENTUser.user_id.ToString());

            var simpleUri = new Uri(HttpAPI.UriFenceStr);
            var newUri = simpleUri.ExtendQuery(param);
            String strURL = newUri.AbsoluteUri;

            RequestInfo info = new RequestInfo(strURL);
            NetHttpFenceAlarmNotifyData nud = null;
            Action<ResponseInfo> act = new Action<ResponseInfo>(x =>
            {
                //回调对结果的处理
                nud = SimpleJson.SimpleJson.DeserializeObject<DataEntity.NetHttpFenceAlarmNotifyData>(
                    x.GetString(Encoding.UTF8));
                if (nud.error == "0")
                {
                    Debug.WriteLine("Action<ResponseInfo>: QueryFenceAlarmInfo 得到结果!" + nud.data.Count);
                    BFenceAlarm.Clear();
                    foreach (FenceAlarmNotify loc in nud.data)
                    {

                        if (loc.action_type.Equals("income"))
                            loc.action_type = WinFormsStringResource.FenceAlarmTypeINCOME;
                        else
                            loc.action_type = WinFormsStringResource.FenceAlarmTypeOUTCOME;

                        //2017.8.16 加入时区差异的处理
                        DateTime datetime = Utils.ConvertToDateTime_Stand(loc.action_time);
                        Debug.WriteLine(datetime.ToString());
                        datetime = datetime.AddHours(ZoneInterval_UserServer);
                        loc.action_time = datetime.ToString("yyyy-MM-dd hh:mm:ss");

                        BFenceAlarm.Add(loc);
                    }

                }


            });
            HttpRequestFactory.AddRequestTask(info, act);

            //mFenceAlarmData = temp;

        }
        #endregion

        private void contextMenuStripFenceAlarm_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            

            if (((ContextMenuStrip)sender).Items[0] == e.ClickedItem)
            {
                

            }
            else if (((ContextMenuStrip)sender).Items[1] == e.ClickedItem)
            {
                

            }
            else if (((ContextMenuStrip)sender).Items[2] == e.ClickedItem)
            {

               
            }
            else if (((ContextMenuStrip)sender).Items[3] == e.ClickedItem)
            {

                
            }
            else if (((ContextMenuStrip)sender).Items[4] == e.ClickedItem)
            {

                //treeViewFenceAlarm.Nodes.Clear();
                //消除所有通知
                this.BFenceAlarm.Clear();

            }
            else if (((ContextMenuStrip)sender).Items[5] == e.ClickedItem)
            {

                //treeViewFenceAlarm.Nodes.Clear();
                //申请不用重复通知
                /*
                 
                DialogResult dr = MessageBox.Show(WinFormsStringResource.FenceAlarmNoNotifyConfirm,
                    WinFormsStringResource.PromptStr, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {

                    string sql = "update ptt_fence_alarm set isnotify='Y' where fence_id in  ( select fence_id from ptt_fence where user_id="
                        + LocalSharedData.CURRENTUser.user_id + " )";
                    if (DbHelperMySQL.ExecuteSql(sql) > 0)
                    {
                        //
                        this.BFenceAlarm.Clear();
                    }

                }
                else
                {
                }

                */

            }
            else if (((ContextMenuStrip)sender).Items[6] == e.ClickedItem)
            {            


            }

        }

        private void 简体中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            //InitializeComponent();
            LanguageHelper.SetLang("zh-CN", this, typeof(ControlMainForm));

            ControlMainForm_Load(this, EventArgs.Empty);

            this.MaximizeBox = true;
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            //InitializeComponent();
            LanguageHelper.SetLang("en-US", this, typeof(ControlMainForm));

            ControlMainForm_Load(this, EventArgs.Empty);

            this.MaximizeBox = true;
        }

        private void webBrowserVideo_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void CALLcontextMenuStrip_Opening(object sender, CancelEventArgs e)
        {

        }

        private void GRPcontextMenuStrip_Click(object sender, EventArgs e)
        {
            //

        }

        private void GRPcontextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            TreeNode node = this.treeViewGROUP.SelectedNode;

            
            if (((ContextMenuStrip)sender).Items["toolStripMenuItemAudioRec"] == e.ClickedItem)
            {
                //录音文件用于广播
                RecorderForm recorderForm = new RecorderForm(this.Full_RecordDeviceIndex);
                if (recorderForm.ShowDialog() == DialogResult.Cancel)
                {
                    recorderForm.Dispose();
                    recorderForm = null;
                }
                
            }
            else  if (((ContextMenuStrip)sender).Items["toolStripMenuItemEnterBroadCast"] == e.ClickedItem)
            {
                //进入广播专用窗口
                int groupId = ((Group)node.Tag).group_id;
                List<User> users = new List<User>();
                //加调度台自身
                User selfUser= LocalSharedData.UserAllGroupCustomer.Find(delegate (User o) {

                    return o.userId == LocalSharedData.CURRENTUser.user_id;

                });
                if (selfUser != null)
                    users.Add(selfUser);

                foreach (TreeNode subNode in node.Nodes)
                {
                    if (!(subNode.Tag is User)) continue;
                    User user = (User)subNode.Tag;
                    if (user.userId!= LocalSharedData.CURRENTUser.user_id)
                    {
                        User myuser = LocalSharedData.UserAllGroupCustomer.Find(delegate (User o)                        {

                            return o.userId == user.userId;
                        });
                        if (myuser!=null)
                            users.Add(myuser);
                    }
                        
                }

                if (broadCastForm==null || broadCastForm.IsDisposed)
                    broadCastForm = new BroadCastForm(this.client, groupId, node.Text, users, 1);

                broadCastForm.labGroupName.Text = node.Text;
                broadCastForm.Owner = this;  //回调用

                BROADCAST_POC_ENABLE = true;  //进入广播对讲模式
                BROADCAST_POC_FILE_MODE_ENABLE = false; //默认是用mic对讲广播



                if (broadCastForm.ShowDialog() == DialogResult.Cancel)
                {
                    BROADCAST_POC_ENABLE = false;  //退出广播对讲模式

                    //退出会话组，并且底部curGroupCombobox要同步              

                    if (this.cbCurrGroup.Items.Count > 0)
                    {
                        enableGroupSwitch = true;
                        this.cbCurrGroup.SelectedValue = GroupService.EMPTY_GROUP_ID + "";
                    }

                    client.AEC_State = 0;  //设为初始态
                }

                //这里要区分是即时创建的广播组，还是用的临时形式广播组
                //1. 如果是即时创建的，则广播窗口退出时自动删除它 (因为删除时,Server端发每个终端，而终端自动处理清除和退出)
                //2. 如果是用的临时形式广播组,则广播窗口退出时,这里还要专门发一个 "强制退出广播"的报文


                broadCastForm = null;

            }
             else  if (((ContextMenuStrip)sender).Items["toolStripMenuItemExit"] == e.ClickedItem)
            {
                //退出会话组，并且底部curGroupCombobox要同步              
                
                if (this.cbCurrGroup.Items.Count > 0)
                {
                    enableGroupSwitch = true;
                    this.cbCurrGroup.SelectedValue = GroupService.EMPTY_GROUP_ID+"";
                }

            }
            else  if (((ContextMenuStrip)sender).Items["toolStripMenuItemEnter"] == e.ClickedItem)
            {
                //进入会话组，并且底部curGroupCombobox要同步
                int grpId = ((Group)node.Tag).group_id;
                if (this.cbCurrGroup.Items.Count > 0)
                {
                    enableGroupSwitch = true;
                    this.cbCurrGroup.SelectedValue = grpId+""; 
                }                

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemDelGrp"] == e.ClickedItem)
            {

                DialogResult result_q = MessageBox.Show("确定要删除吗",
                    "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result_q == DialogResult.No)
                {

                    return;
                }

                //删除组
                String grpid = ((Group)node.Tag).group_id.ToString();                          

                ResponseBase result = PocClient.deleteTmpGroup(Convert.ToInt32(grpid), 7, 1);

                if (result.error == 0)
                {
                    //删除组节点 
                    if (node.Nodes.Count > 0)
                    {
                        TreeNodeCollection coll = node.Nodes;
                        for (int i = coll.Count - 1; i >= 0; i--)
                        {
                            coll[i].Remove();
                        }

                    }
                    node.Remove();
                    //同时还要删除UserAllTempGROUP
                    foreach (Group grp in LocalSharedData.UserAllTempGROUP)
                    {
                        if (grp.group_id == Convert.ToInt32(grpid))
                        {
                            LocalSharedData.UserAllTempGROUP.Remove(grp);
                            //同时还要删除UserAll
                            LocalSharedData.GROUPAllUser.Remove(Convert.ToInt32(grpid));

                            //触发 cbCurrGroup控件内容更新
                            try
                            {
                                this.enableGroupSwitch = false;
                                UpdateCbCurrGroupItems();

                            }
                            finally
                            {
                                this.enableGroupSwitch = true;
                            }

                            break;
                        }
                    }

                    LocalSharedData.GROUPAllUser.Remove(Convert.ToInt32(grpid));

                }
                else
                {
                    MessageBox.Show(WinFormsStringResource.DelTmpGroupFail);
                }

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemModify"] == e.ClickedItem)
            {
                //修改组名称
                //有些组有临时名称
                string grpName = node.Text;
                Boolean tmpGrpFlag = false; Boolean tmpBroadCastFlag = false;

                if (!string.IsNullOrEmpty(grpName))
                {
                    if (grpName.StartsWith(GroupService.TMP_GROUP_PREFIX))
                    {
                        grpName = grpName.Substring(GroupService.TMP_GROUP_PREFIX.Length);
                        tmpGrpFlag = true;

                    } else if (grpName.StartsWith(GroupService.TMP_BROADCAST_GROUP_PREFIX))
                    {
                        grpName = grpName.Substring(GroupService.TMP_BROADCAST_GROUP_PREFIX.Length);
                        tmpBroadCastFlag = true;
                    }
                }

                string s = Interaction.InputBox(WinFormsStringResource.PromptStr,
                     tmpGrpFlag? WinFormsStringResource.TmpGrpNameModifyPrompt: "修改广播组名称",
                    grpName, -1, -1).Trim();  //-1表示在屏幕的中间

                //临时组名称的修改
                if (s != null && !s.Equals(""))
                {
                    if (s.Equals(grpName)) return;  //一样的由返回

                    GroupTempDto dto = new GroupTempDto();
                    dto.groupId = ((Group)node.Tag).group_id;
                    dto.groupName = s;
                    ResponseBase resp= PocClient.updateTmpGroup(dto);
                    if (resp.code != 0)
                    {
                        MessageBox.Show(resp.errorMsg);
                        return;
                    }

                    if (tmpGrpFlag)
                        node.Text = GroupService.TMP_GROUP_PREFIX + s;
                    else if (tmpBroadCastFlag)
                        node.Text = GroupService.TMP_BROADCAST_GROUP_PREFIX + s;
                    else
                        node.Text = s;

                    //更新
                    foreach (Group grp in LocalSharedData.UserAllTempGROUP)
                    {
                         if (grp.group_id== ((Group)node.Tag).group_id)
                        {
                            grp.group_name = s;
                            break;
                        }
                    }

                    //触发 cbCurrGroup控件内容更新
                    try
                    {
                        this.enableGroupSwitch = false;
                        UpdateCbCurrGroupItems();

                    }
                    finally
                    {
                        this.enableGroupSwitch = true;                        
                    }


                }

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemFenceManager"] == e.ClickedItem)
            {
                //干他妈的，这个废掉了
                /*
                //查询当前组有无围栏列表
                List<FenceInfo> myfence = null;
                AllFenceInfoResponse resp = PocClient.queryAllFenceInfo(LocalSharedData.CURRENTUser.cmpid);
                if (resp != null && resp.data != null)
                    myfence = resp.data;

                if (myfence == null || myfence.Count == 0)
                {
                    MessageBox.Show(WinFormsStringResource.NoFoundFenceDefine);
                    return;
                }

                if (fenceRuleForm == null)
                    fenceRuleForm = new FenceRuleDef();

                fenceRuleForm.cbFencelist.Items.Clear();
                ArrayList lists_cbFencelist = new ArrayList();

                foreach (FenceInfo fence in myfence)
                    lists_cbFencelist.Add(new MyKeyValue(fence.fenceId.ToString(), fence.fenceName));

                fenceRuleForm.cbFencelist.DisplayMember = "pValue";
                fenceRuleForm.cbFencelist.ValueMember = "pKey";
                fenceRuleForm.cbFencelist.DataSource = lists_cbFencelist;

                fenceRuleForm.full_fence_id = myfence[0].fenceId;

                fenceRuleForm.cbFencelist.SelectedIndexChanged += CbFencelist_SelectedIndexChanged;

                string sql = "select a.id,a.user_id, a.fence_id,b.fence_name,c.user_name,(case  ifnull(a.rule_type,'outcome') when 'outcome' then '出去警告' else '进入警告' end) as rule_type,(case ifnull(a.rule_assign,'include') when 'include' then '黑名单' else '白名单' end) as rule_assign ,a.start_time,a.end_time  from ptt_fence_user a inner join ptt_fence b on a.fence_id=b.fence_id "
                    + "  inner join ptt_user c on a.user_id=c.user_id   where a.fence_id=" + myfence[0].fenceId;


                daFenceRule = new MySqlDataAdapter(sql, DbHelperMySQL.connectionString);
                MySqlCommandBuilder cb_Fencerule = new MySqlCommandBuilder(daFenceRule);
                dsFenceRule = new DataSet();
                daFenceRule.Fill(dsFenceRule);
                fenceRuleForm.dataGridViewRule.AutoGenerateColumns = false;
                fenceRuleForm.dataGridViewRule.DataSource = dsFenceRule.Tables[0];

                if (fenceRuleForm.ShowDialog() == DialogResult.OK)
                {
                    fenceRuleForm.Dispose();
                    fenceRuleForm = null;
                }
                */

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemForceAdd"] == e.ClickedItem)
            {

                //当前组内不允许重复强插
                List<int> curGrpUser = new List<int>();
                foreach (TreeNode snode in node.Nodes)
                    curGrpUser.Add(((User)snode.Tag).userId);

                CreateTmpGrpMulti crtform = new CreateTmpGrpMulti();
                crtform.clearChecklist();
                crtform.textBoxGrpName.Text = node.Text;   //当前组名称
                crtform.textBoxGrpName.Enabled = false;

                if (node.Tag!=null && node.Tag is Group)
                {
                    if (((Group)node.Tag).group_type == GroupTypeEnum.TALK_TMP)
                        crtform.cbBroadCast.Checked = false;
                    else if (((Group)node.Tag).group_type == GroupTypeEnum.TALK_TMP_BROADCAST)
                        crtform.cbBroadCast.Checked = true;
                }

                crtform.cbBroadCast.Enabled = false;

                foreach (User user in LocalSharedData.UserAllGroupCustomer)
                {
                    if (user.userId != LocalSharedData.CURRENTUser.user_id
                        && curGrpUser.IndexOf(user.userId) == -1)
                    {
                        if (user.logon == 1)
                            crtform.addChecklist(Convert.ToString(user.userId), user.userName + " <" + WinFormsStringResource.StatusOnline + ">");
                        else
                            crtform.addChecklist(Convert.ToString(user.userId), user.userName);
                    }

                }

                if (crtform.ShowDialog() == DialogResult.OK)
                {
                    //MessageBox.Show(crtform.getMember());
                    //
                    ResponseBase res = PocClient.groupUserChange(((Group)node.Tag).group_id, 1, 1, crtform.getMember());

                    if (res != null && res.error == 0)
                    {
                        //                      
                        string[] added_userid = crtform.getMember().Split(new char[] { ',' });

                        for (int j = 0; j < added_userid.Length; ++j)
                        {
                            User us = LocalSharedData.UserAll[Convert.ToInt32(added_userid[j])];
                            if (us == null) continue;

                            if (Convert.ToInt32(added_userid[j]) == LocalSharedData.CURRENTUser.user_id)
                            {
                                TreeNode user = node.Nodes.Add(us.userName);
                                user.Tag = us;
                                if (us.logon == 1)
                                {
                                    user.ImageIndex = 6;
                                    user.SelectedImageIndex = 6;
                                }
                                else
                                {
                                    user.ImageIndex = 9;
                                    user.SelectedImageIndex = 9;
                                }

                                user.ToolTipText = us.userName + "【" + us.userId + "】";
                                
                            }
                            else
                            {
                                if (us.logon == 1)
                                {
                                    TreeNode user = node.Nodes.Add(us.userName);
                                    user.Tag = us;
                                    user.ImageIndex = 6;
                                    user.SelectedImageIndex = 6;

                                    user.ToolTipText = us.userName + "【" + us.userId + "】";

                                    if (mfirstlogonuserid == 0 && mfirstlogongroupid == 0)
                                    {
                                        mfirstlogonuserid = us.userId;
                                        mfirstlogongroupid = ((Group)node.Tag).group_id;
                                    }
                                }
                                else
                                {
                                    TreeNode user = node.Nodes.Add(us.userName);
                                    user.Tag = us;
                                    user.ImageIndex = 9;
                                    user.SelectedImageIndex = 9;

                                    user.ToolTipText = us.userName + "【" + us.userId + "】";
                                }
                            }
                        }


                    }
                    else
                    {
                        MessageBox.Show(res.errorMsg);
                    }

                }

                //强插end

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemAllGPSCommand"] == e.ClickedItem)
            {
                //实时对组内在线用户定位
                TreeNode p = node;
                if (p != null)
                {
                    if (requestGPSForm == null)
                        requestGPSForm = new RequestGPSForm();
                    if (requestGPSForm != null)
                    {
                        if (requestGPSForm.ShowDialog() == DialogResult.OK)
                        {
                            if (requestGPSForm.action_cmd == 3)
                            {
                                timerGPSReq.Enabled = false;
                                Full_LoopGPSCommand_Node = node;
                                Full_LoopGPSCommand_Accuracy = requestGPSForm.cbAccuracy.SelectedIndex;
                                timerGPSReq.Enabled = true;
                                ControlMainForm_ShowGroupLocation(((Group)node.Tag).group_id, this.webBrower);

                            }
                            else if (requestGPSForm.action_cmd == 2)
                            {
                                Full_GPSCommand_Mode = 0;
                                Full_GPSCommand_GROUP_LocationMode = requestGPSForm.cbAccuracy.SelectedIndex;
                                //toolStripMenuItemAllGPS                            
                                foreach (TreeNode nodea in node.Nodes)
                                {
                                    //
                                    Full_GPSCommand_GROUP_ID = ((Group)p.Tag).group_id;
                                    Full_GPSCommand_USER_ID = ((User)nodea.Tag).userId;
                                    int accuracy = requestGPSForm.cbAccuracy.SelectedIndex;
                                    client.SendMessage(
                            (new Data()).GPSCommandMessageEncode(Full_GPSCommand_GROUP_ID,
                            Full_GPSCommand_USER_ID, Convert.ToByte(accuracy), LocalSharedData.CURRENTUser.user_id));

                                }
                                toolStripStatusLabelCurrGrpMap.Text = node.Text;
                                Thread GroupGPSQueryThread = new Thread(new ParameterizedThreadStart(doGroupGPSQuery))
                                { Priority = ThreadPriority.Normal };

                                GroupGPSQueryThread.Start(Full_GPSCommand_GROUP_LocationMode);
                            }
                            else if (requestGPSForm.action_cmd == 1)
                            {
                                //仅查看群组定位
                                List<Location> us = null;
                                if (FullMapType == "mapinfo_offline")
                                {
                                    us = HttpAPI.queryLocationByGroup(LocalSharedData.CURRENTUser.user_id, ((Group)node.Tag).group_id, "");
                                    if (us != null)
                                    {
                                        if (us.Count == 0)
                                        {
                                            MessageBox.Show("未有发现定位数据");
                                        }
                                        MapinfoMessage mapmsg = new MapinfoMessage();
                                        mapmsg.command = "querygps_group";
                                        mapmsg.arglist = new List<ArgumentItem>();
                                        foreach (Location loc in us)
                                        {
                                            ArgumentItem item = new ArgumentItem();
                                            //时间/状态处理
                                            if (loc.curtime != null && loc.curtime > 0)
                                            {
                                                long timesec = (long)loc.curtime / 1000;
                                                DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(timesec);
                                                //转成本地时间  
                                                DateTime localdt = utcdt.ToLocalTime();

                                                //2017.8.16 加入时区差异的处理                                    
                                                localdt = localdt.AddHours(ZoneInterval_UserServer);

                                                loc.gpstime = localdt.ToString("yyyy-MM-dd HH:mm:ss");
                                            }
                                            //if (loc.logon == 1)
                                            //    loc.state = WinFormsStringResource.StatusOnline;
                                            //else
                                            //    loc.state = WinFormsStringResource.StatusOffline;

                                            if (loc.flag_record.Equals("Y"))
                                                loc.flag_record_str = WinFormsStringResource.VoiceRecord_OPEN;
                                            else
                                                loc.flag_record_str = WinFormsStringResource.VoiceRecord_CLOSE;

                                            if (loc.myclass == 0)
                                                loc.myclass_str = WinFormsStringResource.VoiceRight_0;
                                            else if (loc.myclass == 1)
                                                loc.myclass_str = WinFormsStringResource.VoiceRight_1;
                                            else
                                                loc.myclass_str = WinFormsStringResource.VoiceRight_2;

                                            if (loc.life_state == 1)
                                                loc.life_state_str = WinFormsStringResource.LifeState_1;
                                            else if (loc.life_state == 0)
                                                loc.life_state_str = WinFormsStringResource.LifeState_0;
                                            else
                                                loc.life_state_str = WinFormsStringResource.LifeState_NE1;

                                            item.gpstime = loc.gpstime;
                                            item.logon = loc.logon;
                                            item.user_id = loc.user_id.ToString();
                                            item.user_name = loc.user_name;
                                            item.group_name = loc.group_name;
                                            item.batterylevel = loc.batterylevel;
                                            item.lat = loc.latitude.ToString();
                                            item.lng = loc.longitude.ToString();
                                            item.myclass_str = loc.myclass_str;
                                            item.flag_record_str = loc.flag_record_str;
                                            item.life_state_str = loc.life_state_str;

                                            mapmsg.arglist.Add(item);

                                        }

                                        SendMessageToMapinfo(mapmsg);

                                    }
                                    setTabControlPageDisplay(0);
                                    //tabControl1.SelectedIndex = 0;
                                    toolStripStatusLabelCurrGrpMap.Text = node.Text;
                                    return;
                                }
                                //
                                //toolStripMenuItemAllGPS
                                //为了用户体验,在组切换时不处理组的地图显示的即时更新，由此处的菜单来完成
                                //2018.3.15 下面可以不要
                                string userid_clause = "";

                                /*
                                foreach (TreeNode nodea in node.Nodes)
                                {
                                    if (userid_clause.Equals(""))
                                        userid_clause = " a.user_id=" + (int)nodea.Tag;
                                    else
                                        userid_clause = userid_clause + " or a.user_id=" + (int)nodea.Tag;
                                }
                                */

                                //if (userid_clause.Equals(""))
                                //    return;

                                us = HttpAPI.queryLocationByGroup(LocalSharedData.CURRENTUser.user_id, ((Group)node.Tag).group_id, userid_clause);
                                if (us != null)
                                {
                                    BLocation.Clear();
                                    foreach (Location loc in us)
                                    {
                                        //时间/状态处理
                                        if (loc.curtime != null && loc.curtime > 0)
                                        {
                                            long timesec = (long)loc.curtime / 1000;
                                            DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(timesec);
                                            //转成本地时间  
                                            DateTime localdt = utcdt.ToLocalTime();

                                            //2017.8.16 加入时区差异的处理                                    
                                            localdt = localdt.AddHours(ZoneInterval_UserServer);

                                            loc.gpstime = localdt.ToString("yyyy-MM-dd HH:mm:ss");
                                        }
                                        if (loc.logon == 1)
                                            loc.state = WinFormsStringResource.StatusOnline;
                                        else
                                            loc.state = WinFormsStringResource.StatusOffline;

                                        if (loc.flag_record.Equals("Y"))
                                            loc.flag_record_str = WinFormsStringResource.VoiceRecord_OPEN;
                                        else
                                            loc.flag_record_str = WinFormsStringResource.VoiceRecord_CLOSE;

                                        if (loc.myclass == 0)
                                            loc.myclass_str = WinFormsStringResource.VoiceRight_0;
                                        else if (loc.myclass == 1)
                                            loc.myclass_str = WinFormsStringResource.VoiceRight_1;
                                        else
                                            loc.myclass_str = WinFormsStringResource.VoiceRight_2;

                                        if (loc.life_state == 1)
                                            loc.life_state_str = WinFormsStringResource.LifeState_1;
                                        else if (loc.life_state == 0)
                                            loc.life_state_str = WinFormsStringResource.LifeState_0;
                                        else
                                            loc.life_state_str = WinFormsStringResource.LifeState_NE1;

                                        BLocation.Add(loc);
                                    }
                                    changeDatagridviewRowColor();
                                }

                                //tabControl1.SelectedIndex = 0;
                                setTabControlPageDisplay(0);

                                ControlMainForm_ShowGroupLocation(((Group)node.Tag).group_id, this.webBrower);

                                toolStripStatusLabelCurrGrpMap.Text = node.Text;

                            }

                        }
                    }
                }

            }            
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemFileQuery"] == e.ClickedItem)
            {
                //
                //toolStripMenuItemFileQuery
                //打开查询对话框，并且预设一些查询条件
                if (full_RecQueryForm == null)
                {
                    full_RecQueryForm = new RecQueryForm(this.ZoneInterval_UserServer);
                }

                full_RecQueryForm.setQueryValueByGroup(((Group)node.Tag).group_id.ToString());
                full_RecQueryForm.ShowDialog();

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemDayReport"] == e.ClickedItem)
            {             


                

            }
        }       

        

        private void CbFencelist_SelectedIndexChanged(object sender, EventArgs e)
        {

            fenceRuleForm.full_fence_id = Convert.ToInt32(fenceRuleForm.cbFencelist.SelectedValue.ToString());


            List<FenceUserDto> data = new List<FenceUserDto>();
            AllFenceUserInfoResponse resp2 = PocClient.queryAllFenceUserInfo(fenceRuleForm.full_fence_id);
            if (resp2 != null && resp2.data != null)
                data = resp2.data;

            fenceRuleForm.dataGridViewRule.AutoGenerateColumns = false;
            fenceRuleForm.dataGridViewRule.DataSource = data;

        }

        private void tableLayoutPanelGROUP_Paint(object sender, PaintEventArgs e)
        {

        }


        private void buttonNewTmpGrp_Click(object sender, EventArgs e)
        {

            CreateTmpGrpMulti crtform = new CreateTmpGrpMulti();
            crtform.clearChecklist();

            foreach (User user in LocalSharedData.UserAllGroupCustomer)
            {
                if (user.userId != LocalSharedData.CURRENTUser.user_id)
                {
                    if (user.logon == 1)
                        crtform.addChecklist(Convert.ToString(user.userId), user.userName + " <" + WinFormsStringResource.StatusOnline + ">");
                    else
                        crtform.addChecklist(Convert.ToString(user.userId), user.userName);
                }
            }

            if (crtform.ShowDialog() == DialogResult.OK)
            {
                bool broadCast = crtform.cbBroadCast.Checked;

                GroupTempDto dto = new GroupTempDto();
                dto.groupName = crtform.getGrpname();
                dto.userIds = Convert.ToString(LocalSharedData.CURRENTUser.user_id) + "," + crtform.getMember();
                dto.ownerId = LocalSharedData.CURRENTUser.user_id;
                dto.priv = 1;    //调度员发起的为1, 终端发起的为0
                if (broadCast)
                    dto.aclass = 1;
                else
                    dto.aclass = 0;
                Group grptmp=null;
                TempGroupResponse resp_t= PocClient.createTmpGroup(dto);
                if (resp_t != null)
                {
                    grptmp = new Group();
                    grptmp.group_id = resp_t.data.groupId;
                    grptmp.group_name = resp_t.data.groupName;
                    grptmp.owner_id = resp_t.data.ownerId.Value;
                    grptmp.user_ids = resp_t.data.userIds;                    

                }              
                

                if (grptmp != null)
                {
                    //这个组id有可能已经存在了
                    var alreadyGrps = LocalSharedData.UserAllTempGROUP.Select(grp => grp.group_id == grptmp.group_id);

                    if (alreadyGrps.ToArray().Count() > 0)
                    {
                        //调用以下接口, 会发送tcp消息给socket服务, 然后socket服务会群发消息，被PC socket客户端拦截处理了
                        //MessageBox.Show("不允许重复创建,该组:" + grptmp.group_name + ",已经包括所选人员了");
                        return;
                    }

                    //
                    TreeNode it = null;
                    if (broadCast)
                    {
                        it = this.treeViewGROUP.Nodes.Add(GroupService.TMP_BROADCAST_GROUP_PREFIX + grptmp.group_name);
                        grptmp.group_type = GroupTypeEnum.TALK_TMP_BROADCAST;
                    } else
                    {
                        it = this.treeViewGROUP.Nodes.Add(GroupService.TMP_GROUP_PREFIX + grptmp.group_name);
                        grptmp.group_type = GroupTypeEnum.TALK_TMP;
                    }
                  
                    it.Tag = grptmp;
                    it.ImageIndex = 0;
                    it.SelectedImageIndex = 0;
                    it.ToolTipText = grptmp.group_name;

                    List<User> us = new List<User>();

                    GroupUserMemberResponse resp = PocClient.queryTmpGroupMemberByGroupId(grptmp.group_id);
                    if (resp != null && resp.data != null && resp.data.Count > 0)
                    {
                        us = resp.data;
                    }

                    LocalSharedData.GROUPAllUser.Add(grptmp.group_id, us);
                    LocalSharedData.UserAllTempGROUP.Add(grptmp);

                    for (int j = 0; j < us.Count; ++j)
                    {
                        if (us[j].userId == LocalSharedData.CURRENTUser.user_id)
                        {
                            TreeNode user = it.Nodes.Add(us[j].userName);
                            user.Tag = us[j];
                            user.ImageIndex = 6;
                            user.SelectedImageIndex = 6;
                            user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                             
                            
                        }
                        else
                        {
                            if (us[j].logon == 1)
                            {
                                TreeNode user = it.Nodes.Add(us[j].userName);
                                user.Tag = us[j];
                                user.ImageIndex = 6;
                                user.SelectedImageIndex = 6;
                                user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";

                                if (mfirstlogonuserid == 0 && mfirstlogongroupid == 0)
                                {
                                    mfirstlogonuserid = us[j].userId;
                                    mfirstlogongroupid = grptmp.group_id;
                                }
                            }
                            else
                            {
                                TreeNode user = it.Nodes.Add(us[j].userName);
                                user.Tag = us[j];
                                user.ImageIndex = 9;
                                user.SelectedImageIndex = 9;
                                user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                            }
                        }
                    }

                    this.treeViewGROUP.SelectedNode = it;
                    LocalSharedData.CURRENTUser.group_id = ((Group)it.Tag).group_id;
                    LocalSharedData.CURRENTGroupName = it.Text;
                    client.SendMessage(
                    (new Data()).ReportMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
                );

                    //触发图标切换
                    NetUser_Login(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id);

                    //触发 cbCurrGroup控件内容更新
                    try
                    {
                        this.enableGroupSwitch = false;
                        UpdateCbCurrGroupItems();

                    }
                    finally
                    {
                        this.enableGroupSwitch = true;
                    }

                }
                else
                {
                    MessageBox.Show("Sorry! 你可能没有创建组的权限,请联系管理员开通单呼权限 ");
                }

            }
        }

        private void buttonDelTmpGrp_Click(object sender, EventArgs e)
        {
            //
            DelTmpGrpMulti crtform = new DelTmpGrpMulti();
            crtform.clearChecklist();

            foreach (Group grp in LocalSharedData.UserAllTempGROUP)
            {
                crtform.addChecklist(Convert.ToString(grp.group_id), grp.group_name);
            }

            if (crtform.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show(crtform.getMember());                
                string delgrplist = crtform.getMember();
                string[] delgrparr = delgrplist.Split(',');
                foreach (string grpid in delgrparr)
                {
                    //删除                
                    ResponseBase result= PocClient.deleteTmpGroup(Convert.ToInt32(grpid), 7, 1);

                    if (result.error == 0)
                    {
                        //删除组节点 
                        foreach (TreeNode node in this.treeViewGROUP.Nodes)
                        {
                            if (((Group)node.Tag).group_id == Convert.ToInt32(grpid))
                            {
                                if (node.Nodes.Count > 0)
                                {
                                    TreeNodeCollection coll = node.Nodes;
                                    for (int i = coll.Count - 1; i >= 0; i--)
                                    {
                                        coll[i].Remove();
                                    }

                                }
                                node.Remove();
                                //同时还要删除UserAllTempGROUP
                                foreach (Group grp in LocalSharedData.UserAllTempGROUP)
                                {
                                    if (grp.group_id == Convert.ToInt32(grpid))
                                    {
                                        LocalSharedData.UserAllTempGROUP.Remove(grp);
                                        break;
                                    }
                                }
                                //同时还要删除UserAll
                                LocalSharedData.GROUPAllUser.Remove(Convert.ToInt32(grpid));




                                break;
                            }
                        }


                    } else
                    {
                        MessageBox.Show(result.errorMsg);
                    }

                }
                //触发 cbCurrGroup控件内容更新
                try
                {
                    this.enableGroupSwitch = false;
                    UpdateCbCurrGroupItems();

                }
                finally
                {
                    this.enableGroupSwitch = true;
                }


            }


        }





        private void DayTrackcontextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (((ContextMenuStrip)sender).Items["toolStripMenuItemExportExcel"] == e.ClickedItem)
            //{
            //    ExportToExcel d = new ExportToExcel();
            //    d.OutputAsExcelFile(this.dataGridViewDaytrack);
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_2_2;

            //OpenFileDialog ofd = new OpenFileDialog();
            //if (ofd.ShowDialog() == DialogResult.OK)
            //{
            //    vlc_player_.PlayFile(ofd.FileName);
            //}
            LoadVideoControl();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_3_2;
            LoadVideoControl();

        }

        private void contextMenuStripVideo_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //
            /*
            UserControlVideo video = (UserControlVideo)((ContextMenuStrip)sender).SourceControl;
            if (((ContextMenuStrip)sender).Items["toolStripMenuItemCapture"] == e.ClickedItem)
            {
                //源目标,当前文件名称: userid+时间组成
                string filename = video.userid.ToString() + "_" + DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss");
                FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
                if (Capture_Path.Equals(""))
                {
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string path = folderBrowserDialog1.SelectedPath;
                        Capture_Path = path;

                    }
                    else
                    {
                        return;
                    }
                }
                MediaPlayer.TakeSnapShot(video.vlc_media_player, Capture_Path, filename + ".jpg");


            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemVideoRefresh"] == e.ClickedItem)
            {
                //toolStripMenuItemRefresh
                MediaPlayer.NetWork_Media_Play(video,video.vlc_instance, video.vlc_media_player,
                   video.streamurl);

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemResize"] == e.ClickedItem)
            {
                //LoadVideoControl_Max                
                if (video.ismaxsize == 1)
                    LoadVideoControl();
                else
                    LoadVideoControl_Max(video);

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemVolume"] == e.ClickedItem)
            {
                //toolStripMenuItemVolume 音量控制

                if (video.volume == 1)
                {
                    MediaPlayer.SetVolume(video.vlc_media_player, 0);
                    video.volume = 0;
                }
                else
                {
                    MediaPlayer.SetVolume(video.vlc_media_player, 100);
                    video.volume = 1;

                }

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemStopMoni"] == e.ClickedItem)
            {
                //停止监控
                //确认
                DialogResult result = MessageBox.Show(WinFormsStringResource.QuestionStopMoni, WinFormsStringResource.PromptStr, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //调用http接口
                    HttpAPI.stopMoniOrLive(video.app, video.streamurl, video.userid);
                }

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemStopLive"] == e.ClickedItem)
            {
                //停止直播
                DialogResult result = MessageBox.Show(WinFormsStringResource.QuestionStopLive, WinFormsStringResource.PromptStr, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //调用http接口
                    HttpAPI.stopMoniOrLive(video.app, video.streamurl, video.userid);
                }

            }

            */

        }

        //定义一个代理
        private delegate void DispTimerTickDelegate(UserControlVideo video);

        private void doTimerTick(UserControlVideo video)
        {
            /*
            if (MediaPlayer.MediaPlayer_IsPlaying(video.vlc_media_player))
            {
                long timesec = Convert.ToInt32(MediaPlayer.MediaPlayer_GetCurrentTime(video.vlc_media_player))
                    + video.publishdate;


                if (Math.Abs(video.curtime - MediaPlayer.MediaPlayer_GetCurrentTime(video.vlc_media_player)) > 0.001)
                {

                    video.curtime = MediaPlayer.MediaPlayer_GetCurrentTime(video.vlc_media_player);
                    DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(timesec);
                    //转成本地时间
                    DateTime localdt = utcdt.ToLocalTime();
                    //string timeformat = localdt.ToString("yyyy-MM-dd HH:mm:ss");
                    video.time.Text = localdt.ToString("yyyy-MM-dd HH:mm:ss");
                    //
                    int width = 0; int height = 0;
                    MediaPlayer.libvlc_media_stats_t status = MediaPlayer.GetMediaStats(video.vlc_media_player, ref width, ref height);
                    if (status.i_read_bytes > 0)
                        video.username.Text = video.username_attr + "  【" + width.ToString() + "x" + height.ToString() + "/" + Math.Round(status.f_input_bitrate * 1000.0, 1).ToString() + "kbps】";

                }
                else
                {
                    video.trytime = video.trytime + 1;
                    if (video.trytime > 10)
                    {
                        Debug.WriteLine("准备关闭player,video.trytime=" + video.trytime);
                        video.status = 0;
                        video.trytime = 0;
                        //下面经常造成当机
                        try
                        {
                            //video.timer.Stop();
                            //video.timer.Enabled = false;
                            video.username.Text = video.username_attr + "(结束,时长:" + video.curtime.ToString() + "秒)";
                            MediaPlayer.MediaPlayer_Stop(video.vlc_media_player);

                        }
                        catch (Exception e2)
                        {
                            Debug.WriteLine("video  Timer_Tick found exception 2:" + e2.Message);
                        }
                    }
                }


            }
            else
            {
                //第一次trytime=0, 调用一次play,因为它有个延时执行
                if (video.trytime == 0)
                {
                    MediaPlayer.NetWork_Media_Play(video,video.vlc_instance,
                                                           video.vlc_media_player, video.streamurl);
                    video.trytime = video.trytime + 1;

                }
                else
                    video.trytime = video.trytime + 1;

                if (video.trytime > 20)
                {
                    Debug.WriteLine("video.trytime=" + video.trytime);
                    video.status = 0;
                    video.trytime = 0;
                    try
                    {

                        //video.timer.Enabled = false;
                        video.username.Text = video.username_attr + "(结束,时长:" + video.curtime.ToString() + "秒)";
                        MediaPlayer.MediaPlayer_Stop(video.vlc_media_player);

                    }
                    catch (Exception e2)
                    {
                        Debug.WriteLine("video  Timer_Tick found exception 2:" + e2.Message);
                    }
                }


            }
            */
        }

        /// <summary>
        /// 每个视频控件的定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {


        }



        //以下代码，废除，未用
        private void timerVideo_Tick(object sender, EventArgs e)
        {
            /*
            //检查数据库
            //发现该定时器有卡死bug
            System.Windows.Forms.Timer selftimer = (System.Windows.Forms.Timer)sender;
            Debug.WriteLine("timerVideo_Tick run...");
            try
            {
                if (selftimer.Enabled)
                    selftimer.Stop();
                string grp_clause = "";
                for (int i = 0; i < LocalSharedData.UserAllGROUP.Count; i++)
                {
                    if (grp_clause.Equals(""))
                        grp_clause = " c.group_id=" + LocalSharedData.UserAllGROUP[i].group_id;
                    else
                        grp_clause = grp_clause + " or  c.group_id=" + LocalSharedData.UserAllGROUP[i].group_id;
                }

                if (grp_clause.Equals(""))
                    return;

                StringBuilder sb = new StringBuilder(" SELECT  distinct  a.user_id,a.streamurl,a.publishdate  as pubdate, b.user_name as username,a.action_type  as video_type");
                sb.Append(" ,a.fixloc_lng as lng, a.fixloc_lat as lat, a.fixloc_place as place, a.app, a.stream  ");
                sb.Append("  FROM ptt_live_stream  a    INNER JOIN   ptt_user b on a.user_id=b.user_id ");
                sb.Append(" INNER JOIN   ptt_group_user c on b.user_id=c.user_id  where ("
                    + grp_clause + ")  ORDER  by a.publishdate asc ");

                DataSet ds = DbHelperMySQL.Query(sb.ToString());
                DataTable dt = ds.Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    int userid = Convert.ToInt32(dr["user_id"]);
                    //先判断是否已在显示中了
                    bool find = false;
                    for (int j = 0; j < CURRENT_VIDEOLIST.Count; j++)
                    {
                        if (CURRENT_VIDEOLIST[j].status == 1 && CURRENT_VIDEOLIST[j].userid == userid)
                        {
                            if (!MediaPlayer.MediaPlayer_IsPlaying(
                                   CURRENT_VIDEOLIST[j].vlc_media_player))
                            {
                                //要更新下,因为streamurl可能不一样
                                CURRENT_VIDEOLIST[j].username.Text = dr.Field<string>("username");
                                CURRENT_VIDEOLIST[j].app = dr.Field<string>("app");
                                CURRENT_VIDEOLIST[j].stream = dr.Field<string>("stream");
                                CURRENT_VIDEOLIST[j].streamurl = dr.Field<string>("streamurl");
                                CURRENT_VIDEOLIST[j].publishdate = Convert.ToInt32(dr["pubdate"]);
                                CURRENT_VIDEOLIST[j].action_type = dr.Field<string>("video_type");

                                if (MediaPlayer.NetWork_Media_Play(CURRENT_VIDEOLIST[j].vlc_instance,

                                                               CURRENT_VIDEOLIST[j].vlc_media_player, CURRENT_VIDEOLIST[j].streamurl))
                                {

                                    CURRENT_VIDEOLIST[j].status = 1;
                                    //CURRENT_VIDEOLIST[j].timer.Enabled = true;
                                    find = true;
                                    break;
                                }
                                else
                                {
                                    CURRENT_VIDEOLIST[j].status = 0;
                                    //CURRENT_VIDEOLIST[j].timer.Enabled = false;
                                    find = false;
                                    break;
                                }
                            }
                            else
                            {
                                find = true;
                                break;
                            }

                        }
                    }
                    //再到
                    if (!find)
                    {
                        for (int j = 0; j < CURRENT_VIDEOLIST.Count; j++)
                        {
                            if (CURRENT_VIDEOLIST[j].status == 0)
                            {
                                CURRENT_VIDEOLIST[j].userid = userid;
                                CURRENT_VIDEOLIST[j].username.Text = dr.Field<string>("username");
                                CURRENT_VIDEOLIST[j].app = dr.Field<string>("app");
                                CURRENT_VIDEOLIST[j].stream = dr.Field<string>("stream");
                                CURRENT_VIDEOLIST[j].streamurl = dr.Field<string>("streamurl");
                                CURRENT_VIDEOLIST[j].publishdate = Convert.ToInt32(dr["pubdate"]);
                                CURRENT_VIDEOLIST[j].action_type = dr.Field<string>("video_type");
                                //打开视频，并enable timer
                                if (MediaPlayer.NetWork_Media_Play(CURRENT_VIDEOLIST[j].vlc_instance,

                                    CURRENT_VIDEOLIST[j].vlc_media_player, CURRENT_VIDEOLIST[j].streamurl))
                                {

                                    CURRENT_VIDEOLIST[j].status = 1;
                                    //CURRENT_VIDEOLIST[j].timer.Enabled = true;
                                }

                                break;
                            }
                        }
                    }


                }
                selftimer.Start();
            }
            catch (Exception ee)
            {
                Debug.WriteLine("DB query Timer exception : " + ee.Message);
                selftimer.Enabled = true;
                selftimer.Start();
            }
            */

        }

        private void button4_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_4_2;
            LoadVideoControl();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_3_3;
            LoadVideoControl();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_4_3;
            LoadVideoControl();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_4_4;
            LoadVideoControl();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_5_4;
            LoadVideoControl();
        }

        private void contextMenuStripVideo_Opening(object sender, CancelEventArgs e)
        {
            //根据关联的usercontrol的状态
            //MessageBox.Show(((UserControlVideo)(((ContextMenuStrip)sender).SourceControl)).userid.ToString());
            UserControlVideo bindVideo = (UserControlVideo)((ContextMenuStrip)sender).SourceControl;
            if (bindVideo.userid == 0)
            {
                //没有视频流
                toolStripMenuItemCapture.Visible = false;
                toolStripMenuItemResize.Visible = true;

                if (bindVideo.ismaxsize == 1)
                    toolStripMenuItemResize.Text = WinFormsStringResource.VideoRestoreWindow;
                else
                    toolStripMenuItemResize.Text = WinFormsStringResource.VideoMaxWindow;

                toolStripMenuItemVolume.Visible = false;
                toolStripMenuItemStopMoni.Visible = false;
                toolStripMenuItemStopLive.Visible = false;
                toolStripMenuItemVideoRefresh.Visible = false;

            }
            else if (bindVideo.userid > 0 && bindVideo.status == 1)
            {
                //有流
                if (bindVideo.action_type != null && bindVideo.action_type.Equals("LIVE"))
                {
                    toolStripMenuItemStopMoni.Visible = false;
                    toolStripMenuItemStopLive.Visible = true;
                }
                else if (bindVideo.action_type != null && bindVideo.action_type.Equals("MONI"))
                {
                    toolStripMenuItemStopMoni.Visible = true;
                    toolStripMenuItemStopLive.Visible = false;
                }
                else if (bindVideo.poc_video_type == POC_VIDEO_TYPE.DOUBLE_VIDEO
                    || bindVideo.poc_video_type == POC_VIDEO_TYPE.DOUBLE_VOICE)
                {
                    //SIP的右键菜单
                    if (bindVideo.ismaxsize == 1)
                        toolStripMenuItemResize.Text = WinFormsStringResource.VideoRestoreWindow;
                    else
                        toolStripMenuItemResize.Text = WinFormsStringResource.VideoMaxWindow;

                    toolStripMenuItemVolume.Visible = false;
                    toolStripMenuItemStopMoni.Visible = false;
                    toolStripMenuItemStopLive.Visible = false;
                    toolStripMenuItemVideoRefresh.Visible = false;
                    toolStripMenuItemCapture.Visible = false;

                    toolStripMenuItemResize.Visible = true;
                    return;

                }

                toolStripMenuItemCapture.Visible = true;
                //最大化，还原
                if (bindVideo.ismaxsize == 1)
                    toolStripMenuItemResize.Text = WinFormsStringResource.VideoRestoreWindow;
                else
                    toolStripMenuItemResize.Text = WinFormsStringResource.VideoMaxWindow;

                toolStripMenuItemResize.Visible = true;
                //静音,放音
                if (bindVideo.volume == 0)
                    toolStripMenuItemVolume.Text = WinFormsStringResource.VideoVoiceOpen;
                else
                    toolStripMenuItemVolume.Text = WinFormsStringResource.VideoVoiceClose;


                toolStripMenuItemVolume.Visible = true;
                toolStripMenuItemVideoRefresh.Visible = true;

            }
            else if (bindVideo.userid > 0 && bindVideo.status == 0)
            {
                toolStripMenuItemCapture.Visible = false;
                toolStripMenuItemResize.Visible = true;
                if (bindVideo.ismaxsize == 1)
                    toolStripMenuItemResize.Text = WinFormsStringResource.VideoRestoreWindow;
                else
                    toolStripMenuItemResize.Text = WinFormsStringResource.VideoMaxWindow;

                toolStripMenuItemVolume.Visible = false;
                toolStripMenuItemStopMoni.Visible = false;
                toolStripMenuItemStopLive.Visible = false;

                toolStripMenuItemVideoRefresh.Visible = true;

            }

        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private delegate void UpdateMapLocDelegate();

        private void doUpdateMapLoc()
        {
            //2017.06.30 自动更新地图定位
            TreeNode it = treeViewGROUP.SelectedNode;
            if (it == null) return;

            //MessageBox.Show("开始查询");

            if (it.Nodes.Count == 0 || it.Level == 1)
            {
                //个人节点
                TreeNode p = it.Parent;
                if (p != null)
                {
                    //点中个人节点
                    //先显示地图中坐标
                    //if (!JIZHAN_ENABLE)
                    //{
                    //再显示表格
                    List<Location> us = HttpAPI.queryLocationByUserid(((User)it.Tag).userId, ((Group)p.Tag).group_id);
                    if (us != null)
                    {
                        BLocation.Clear();
                        foreach (Location loc in us)
                        {
                            //时间/状态处理
                            if (loc.curtime != null && loc.curtime > 0)
                            {
                                long timesec = (long)loc.curtime / 1000;
                                DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(timesec);
                                //转成本地时间  
                                DateTime localdt = utcdt.ToLocalTime();

                                //2017.8.16 加入时区差异的处理                                    
                                localdt = localdt.AddHours(ZoneInterval_UserServer);

                                loc.gpstime = localdt.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            if (loc.logon == 1)
                                loc.state = WinFormsStringResource.StatusOnline;
                            else
                                loc.state = WinFormsStringResource.StatusOffline;

                            if (loc.flag_record.Equals("Y"))
                                loc.flag_record_str = WinFormsStringResource.VoiceRecord_OPEN;
                            else
                                loc.flag_record_str = WinFormsStringResource.VoiceRecord_CLOSE;

                            if (loc.myclass == 0)
                                loc.myclass_str = WinFormsStringResource.VoiceRight_0;
                            else if (loc.myclass == 1)
                                loc.myclass_str = WinFormsStringResource.VoiceRight_1;
                            else
                                loc.myclass_str = WinFormsStringResource.VoiceRight_2;

                            if (loc.life_state == 1)
                                loc.life_state_str = WinFormsStringResource.LifeState_1;
                            else if (loc.life_state == 0)
                                loc.life_state_str = WinFormsStringResource.LifeState_0;
                            else
                                loc.life_state_str = WinFormsStringResource.LifeState_NE1;



                            BLocation.Add(loc);
                        }

                        changeDatagridviewRowColor();
                    }
                    //}

                }
            }
            else
            {
                //选中组
                string userid_clause = "";
                foreach (TreeNode nodea in it.Nodes)
                {
                    if (userid_clause.Equals(""))
                        userid_clause = " a.user_id=" + ((User)nodea.Tag).userId;
                    else
                        userid_clause = userid_clause + " or a.user_id=" + ((User)nodea.Tag).userId;
                }

                if (userid_clause.Equals(""))
                    return;

                //if (!JIZHAN_ENABLE)  //非基站版
                //{
                List<Location> us = HttpAPI.queryLocationByGroup(LocalSharedData.CURRENTUser.user_id, ((Group)it.Tag).group_id, userid_clause);
                if (us != null)
                {
                    BLocation.Clear();
                    foreach (Location loc in us)
                    {
                        //时间/状态处理
                        if (loc.curtime != null && loc.curtime > 0)
                        {
                            long timesec = (long)loc.curtime / 1000;
                            DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(timesec);
                            //转成本地时间  
                            DateTime localdt = utcdt.ToLocalTime();

                            //2017.8.16 加入时区差异的处理                                    
                            localdt = localdt.AddHours(ZoneInterval_UserServer);

                            loc.gpstime = localdt.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (loc.logon == 1)
                            loc.state = WinFormsStringResource.StatusOnline;
                        else
                            loc.state = WinFormsStringResource.StatusOffline;

                        if (loc.flag_record.Equals("Y"))
                            loc.flag_record_str = WinFormsStringResource.VoiceRecord_OPEN;
                        else
                            loc.flag_record_str = WinFormsStringResource.VoiceRecord_CLOSE;

                        if (loc.myclass == 0)
                            loc.myclass_str = WinFormsStringResource.VoiceRight_0;
                        else if (loc.myclass == 1)
                            loc.myclass_str = WinFormsStringResource.VoiceRight_1;
                        else
                            loc.myclass_str = WinFormsStringResource.VoiceRight_2;

                        if (loc.life_state == 1)
                            loc.life_state_str = WinFormsStringResource.LifeState_1;
                        else if (loc.life_state == 0)
                            loc.life_state_str = WinFormsStringResource.LifeState_0;
                        else
                            loc.life_state_str = WinFormsStringResource.LifeState_NE1;


                        BLocation.Add(loc);
                    }
                    changeDatagridviewRowColor();
                }
                //}

            }
        }

        private void timerUpdateLoc_Tick(object sender, EventArgs e)
        {

            //2017.7.27 IE老是出现 "是否停止运行脚本"的错误，以下写法部分注释(即地图网页不是从pc端触发)
            //2017.8.9  定时更新数据内容只包括用户状态及定位信息
            //2017.10.17 由于内存在24小时以上会有泄漏严重          

        }

        private void button8_Click(object sender, EventArgs e)
        {
            MySysConfig paraForm = new MySysConfig(this);
            paraForm.ShowDialog();
        }

        

        private void InitRunPara()
        {
            //读取配置文件
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, PARAFILE);
            //指定ini文件的路径
            IniFile ini = new IniFile(path);

            //mp4webprex
            if (!ini.IniReadValue("other", "mp4webprex").Trim().Equals(""))
                FullReplaceWebPrex = ini.IniReadValue("other", "mp4webprex").Trim();


            //读取默认经纬度
            //MAP_DEFAULT_LAT=
            if (ini.IniReadValue("other", "map_lat").Trim().Equals(""))
                MAP_DEFAULT_LAT = "0";
            else
            {
                MAP_DEFAULT_LAT = ini.IniReadValue("other", "map_lat").Trim();
            }


            if (ini.IniReadValue("other", "map_lng").Trim().Equals(""))
                MAP_DEFAULT_LNG = "0";
            else
            {
                MAP_DEFAULT_LNG = ini.IniReadValue("other", "map_lng").Trim();

            }

            //FullTVMode 即 treeview 
            if (ini.IniReadValue("other", "navigate").Trim().Equals(""))
                FullTVMode = "talkgroup";
            else
            {
                FullTVMode = ini.IniReadValue("other", "navigate").Trim();
            }


            if (ini.IniReadValue("maploc", "autoenable").Equals(""))
                timerUpdateLoc.Enabled = false;
            else
            {
                if (ini.IniReadValue("maploc", "autoenable").Equals("Y"))
                    timerUpdateLoc.Enabled = true;
                else
                    timerUpdateLoc.Enabled = false;
            }

            //VideoLivePopup
            if (ini.IniReadValue("video", "livepopup").Equals(""))
                VideoLivePopup = false;
            else
            {
                if (ini.IniReadValue("video", "livepopup").Equals("Y"))
                    VideoLivePopup = true;
                else
                    VideoLivePopup = false;
            }
            //FullDownloadVideoPath
            if (ini.IniReadValue("download", "pathvideo").Equals(""))
                FullDownloadVideoPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "poc_video");
            else
            {
                FullDownloadVideoPath = ini.IniReadValue("download", "pathvideo").Trim();
            }

            //FullDownloadAudioPath
            if (ini.IniReadValue("download", "pathaudio").Equals(""))
                FullDownloadAudioPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "poc_audio");
            else
            {
                FullDownloadAudioPath = ini.IniReadValue("download", "pathaudio").Trim();
            }

            //FullDownloadSessionPath
            if (ini.IniReadValue("download", "pathsession").Equals(""))
                FullDownloadPicPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "poc_session");
            else
            {
                FullDownloadPicPath = ini.IniReadValue("download", "pathsession").Trim();
            }


            //FullMsgPopup
            if (ini.IniReadValue("other", "msgpopup").Equals(""))
                FullMsgPopup = false;
            else
            {
                if (ini.IniReadValue("other", "msgpopup").Equals("Y"))
                    FullMsgPopup = true;
                else
                    FullMsgPopup = false;
            }

            //
            if (ini.IniReadValue("other", "av_device_error").Equals(""))
                FullAVDeviceErrorPrompt = true;
            else
            {
                if (ini.IniReadValue("other", "av_device_error").Equals("Y"))
                    FullAVDeviceErrorPrompt = true;
                else
                    FullAVDeviceErrorPrompt = false;
            }

            //FullMainCaption
            if (ini.IniReadValue("other", "maincaption").Trim().Equals(""))
                FullMainCaption = "";
            else
            {
                FullMainCaption = ini.IniReadValue("other", "maincaption").Trim();
            }
            //2017.11.21  非当前组的对讲设置
            //FullNonCurrentVoice
            if (ini.IniReadValue("voice", "noncurgroup").Trim().Equals(""))
                FullNonCurrentVoice = "onlycurrent";
            else
            {
                FullNonCurrentVoice = ini.IniReadValue("voice", "noncurgroup").Trim();
            }

            //GPSValidLoc_IntervalMin
            if (ini.IniReadValue("maploc", "gps_valid").Equals(""))
            {
                GPSValidLoc_IntervalMin = 10;
            }
            else
            {
                GPSValidLoc_IntervalMin = Convert.ToInt32(ini.IniReadValue("maploc", "gps_valid").Trim());
            }


            if (ini.IniReadValue("maploc", "frequency").Equals(""))
            {
                timerUpdateLoc.Interval = 5000;
                TimerUpdateLoc_Interval = 5000;
            }
            else
            {
                timerUpdateLoc.Interval = Convert.ToInt32(ini.IniReadValue("maploc", "frequency").Trim());  //

                TimerUpdateLoc_Interval = Convert.ToInt32(ini.IniReadValue("maploc", "frequency").Trim());
            }

            //2017.8.16 加入时差分析
            if (ini.IniReadValue("zone", "user_server_zoneinterval").Equals(""))
            {
                ZoneInterval_UserServer = 0;
            }
            else
            {
                ZoneInterval_UserServer = Convert.ToInt32(ini.IniReadValue("zone", "user_server_zoneinterval").Trim());

            }
            //2017.10.8 加入基站或公网下选择百度离线地图支持
            //FullMapType FullFenceAlarmType            
            FullMapType = ini.IniReadValue("other", "maptype").Trim();

            FullMapInfo_Offline_Dir = ini.IniReadValue("mapinfo_offline", "mapinfo_offline_dir").Trim();  //相对目录表示
            FullMapInfo_Main_Process = ini.IniReadValue("mapinfo_offline", "mapinfo_main_process").Trim();  //相对目录表示
            FullMapInfo_Tabs_Dir = ini.IniReadValue("mapinfo_offline", "mapinfo_tabs_dir").Trim();  //相对目录表示

            FullFenceAlarmType = ini.IniReadValue("other", "fencealarmtype").Trim();
            FullVideoAspectRatio = ini.IniReadValue("video", "aspect_ratio").Trim();

            Full_RecordDeviceName = ini.IniReadValue("sip", "active_recorddevice").Trim(); //录音设备
            Full_PlaybackDeviceName = ini.IniReadValue("sip", "active_playbackdevice").Trim(); //播音设备

        }

        private void webBrowserFenceMAP_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //
            //this.webBrowserFenceMAP.Document.Window.Error += Window_Error;
        }

        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {

            MessageBox.Show("脚本错误，第" + e.LineNumber + "行：" + e.Description);
            e.Handled = true;
            //throw new NotImplementedException();
        }

        private void treeViewGROUP_DoubleClick(object sender, EventArgs e)
        {
            //
            TreeNode node = this.treeViewGROUP.GetNodeAt(pi);
            if (pi.X < node.Bounds.Left || pi.X > node.Bounds.Right)
            {
                return;
            }
            else
            {
                //2017.11.20 由于会误操作双击,故暂时disable

                //if ((int)(node.Tag) == LocalSharedData.CURRENTUser.user_id) return;

                ////选中鼠标右键按下所在的节点
                //this.treeViewGROUP.SelectedNode = node;
                ////判断组或个人，如果是组且是临时组则出现上下文菜单
                //if (node.Level == 1)
                //{
                //    //
                //    string grpcaption = node.Text;  //对方的名称
                //    string userids = "" + (int)node.Tag + "," + LocalSharedData.CURRENTUser.user_id; //双方的userid串接


                //    executeSinglePOC(grpcaption, userids);

                //}

            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            //记录查询
            if (full_RecQueryForm == null)
            {
                full_RecQueryForm = new RecQueryForm(this.ZoneInterval_UserServer);
            }
            //full_RecQueryForm.vlcControl1 = new Vlc.DotNet.Forms.VlcControl();
            //if (full_RecQueryForm.tableLayoutPanel1.GetControlFromPosition(0, 0) != null)
            //    full_RecQueryForm.tableLayoutPanel1.Controls.Remove(full_RecQueryForm.tableLayoutPanel1.GetControlFromPosition(0, 0));
            //full_RecQueryForm.tableLayoutPanel1.Controls.Add(full_RecQueryForm.vlcControl1,0,0);
            //full_RecQueryForm.vlcControl1.Dock = DockStyle.Fill;
            //full_RecQueryForm.vlcControl1.BackColor = Color.Gray;
            //full_RecQueryForm.vlcControl1.VlcLibDirectoryNeeded += VlcControl1_VlcLibDirectoryNeeded;

            //full_RecQueryForm.vlcControl1.EndInit();
            //full_RecQueryForm.vlcControl1.Opening += VlcControl1_Opening;
            //full_RecQueryForm.vlcControl1.EndReached += VlcControl1_EndReached;
            //full_RecQueryForm.vlcControl1.EncounteredError += VlcControl1_EncounteredError;

            full_RecQueryForm.ShowDialog();

        }

        private void VlcControl1_Opening(object sender, Vlc.DotNet.Core.VlcMediaPlayerOpeningEventArgs e)
        {
            full_RecQueryForm.media_state.Text =
                WinFormsStringResource.MediaState_opened;
        }

        private void VlcControl1_EncounteredError(object sender, Vlc.DotNet.Core.VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            full_RecQueryForm.media_state.Text =
                WinFormsStringResource.MediaState_errored;
        }

        private void VlcControl1_EndReached(object sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            full_RecQueryForm.media_state.Text =
                WinFormsStringResource.MediaState_ended;
        }

        private void VlcControl1_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            e.VlcLibDirectory = new DirectoryInfo(System.Environment.CurrentDirectory);
        }

        private void webBrowserPatroll_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }
        ExeToWinform fr = null;
        public int MAPINFO_HANDLE = 0;
        private void button11_Click(object sender, EventArgs e)
        {
            //
            OpenFileDialog Oppf = new OpenFileDialog();
            Oppf.ShowDialog();
            if (Oppf.FileName != "")
            {
                
            }
        }

        //以下是进程间的通信
        private const int WM_USER = 0x0400;
        const int WM_COPYDATA = 0x004A; //+ 100;
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        public static int formResizeMode = 0;       // 此处指定窗体尺寸修改的模式

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern bool SendMessage(
            IntPtr hWnd, // handle to destination window
            int Msg, // message
            int wParam, // first message parameter
            int lParam //  second message parameter
        );
        //常量
        public const int WM_SYSCOMMAND = 0x0112;

        //窗体移动
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        //改变窗体大小
        public const int WMSZ_LEFT = 0xF001;
        public const int WMSZ_RIGHT = 0xF002;
        public const int WMSZ_TOP = 0xF003;
        public const int WMSZ_TOPLEFT = 0xF004;
        public const int WMSZ_TOPRIGHT = 0xF005;
        public const int WMSZ_BOTTOM = 0xF006;
        public const int WMSZ_BOTTOMLEFT = 0xF007;
        public const int WMSZ_BOTTOMRIGHT = 0xF008;


        //共用的调度台发向mapinfo程序的命令
        private void SendMessageToMapinfo(MapinfoMessage sendmsg)
        {
            if (MAPINFO_HANDLE > 0)
            {
                string msg_json = SimpleJson.SimpleJson.SerializeObject(sendmsg);
                byte[] sarr = System.Text.Encoding.Default.GetBytes(SimpleJson.SimpleJson.SerializeObject(msg_json));
                int len = sarr.Length;
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)100;
                cds.lpData = msg_json;
                cds.cbData = len + 1;
                //panelMapinfoContainer
                // SendMessage(panelMapinfoContainer.Controls[0].Handle, WM_COPYDATA, 0, ref cds);
                //SendMessage(MAPINFO_HANDLE, WM_COPYDATA, 0, ref cds);
            }
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            if (localvideoForm != null)
                localvideoForm.Show();
        }

        private void cbCurrGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (enableGroupSwitch)
            {
                //
                LocalSharedData.CURRENTUser.group_id = Convert.ToInt32(cbCurrGroup.SelectedValue.ToString());
                LocalSharedData.CURRENTGroupName = cbCurrGroup.Text;

                client.SendMessage(
                    (new Data()).ReportMessageEncode(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id)
                );

                this.toolStripStatusLab_username.Text = LocalSharedData.CURRENTUser.user_name + "(" + LocalSharedData.CURRENTUser.user_id + ")--正在登录";
                this.toolStripStatusLab_userid.Text = LocalSharedData.CURRENTUser.user_id.ToString();
                this.toolStripStatusLab_grpname.Text = LocalSharedData.CURRENTGroupName;
                //显示正在与谁实时信息

                this.toolStripStatusLabUsernameCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub1 + "Name：";
                this.toolStripStatusLabUserIDCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub1 + "ID：";
                this.toolStripStatusLabCurgrpCaption.Text = WinFormsStringResource.Main_labelACCOUNT_sub2 + "";

                //2017.11.17 更新图标
                NetUser_Login(LocalSharedData.CURRENTUser.group_id, LocalSharedData.CURRENTUser.user_id);

            }
        }

        //供外部调用SIP申请
        public void requestSipReg()
        {
            //
            /*
            if (AbtoPhone != null)
            {
                //sip更新注册请求
                try
                {
                    this.AbtoPhone.Config.RegExpire = 0;
                    this.AbtoPhone.ApplyConfig();
                    Thread.Sleep(15000);
                }
                finally
                {

                    SetAbtoPhoneCfg_IncludeRunParaIni(AbtoPhone.Config);
                    AbtoPhone.ApplyConfig();

                }

            }
            */

        }

        private void timerSipReg_Tick(object sender, EventArgs e)
        {
            /*
            if (AbtoPhone != null)
            {
                //sip更新注册请求
                try
                {
                    this.AbtoPhone.Config.RegExpire = 0;
                    this.AbtoPhone.ApplyConfig();
                    Thread.Sleep(15000);
                }
                finally
                {
                    

                    SetAbtoPhoneCfg_IncludeRunParaIni(AbtoPhone.Config);

                    AbtoPhone.ApplyConfig();


                }

            }
            */

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_2_2;

            //OpenFileDialog ofd = new OpenFileDialog();
            //if (ofd.ShowDialog() == DialogResult.OK)
            //{
            //    vlc_player_.PlayFile(ofd.FileName);
            //}
            LoadVideoControl();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_3_2;
            LoadVideoControl();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_4_2;
            LoadVideoControl();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_3_3;
            LoadVideoControl();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_4_3;
            LoadVideoControl();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_4_4;
            LoadVideoControl();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            CURRENT_VIDEOLayoutMode = VideoLayoutMode.LAYOUT_5_4;
            LoadVideoControl();
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            setTabControlPageDisplay(0);

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            /*

            if (fullIMWin == null || client.IsCreateIMForm == false)
            {
                fullIMWin =

                new WpfApplication1.MainWindow(client, LocalSharedData.CURRENTUser.user_id,
                LocalSharedData.CURRENTUser.group_id, HttpAPI.WEBIP, Convert.ToInt32(HttpAPI.WEBPort));
                ElementHost.EnableModelessKeyboardInterop(fullIMWin);
            }


            //使用WindowInteropHelper类为WPF设置owner
            WindowInteropHelper helper = new WindowInteropHelper(fullIMWin);
            helper.Owner = this.Handle;
            fullIMWin.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            fullIMWin.Show();
          */

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            
        }       

        
        private void treeViewGROUP_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            //todo将来这里做一些技巧，将快捷功能加到这里 

        }    


       

        



     
















        private void tb_videopageindex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20)
                e.KeyChar = (char)0;  //禁止空格键
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //处理负数
            if (e.KeyChar > 0x20)
            {
                try
                {
                    Int32.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }
        }





        private void tb_audiopageindex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20)
                e.KeyChar = (char)0;  //禁止空格键
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //处理负数
            if (e.KeyChar > 0x20)
            {
                try
                {
                    Int32.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }
        }





        private void tb_sessionpageindex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20)
                e.KeyChar = (char)0;  //禁止空格键
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //处理负数
            if (e.KeyChar > 0x20)
            {
                try
                {
                    Int32.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }
        }        



        private void setSelectIndex(ComboBox cb, string cb_value)
        {
            cb.SelectedIndex = -1;
            for (int i = 0; i < cb.Items.Count; i++)
            {
                //GetItemText返回的是 displaymember指定的值
                if (cb.GetItemText(cb.Items[i]).Equals(cb_value))
                {
                    cb.SelectedIndex = i;
                    break;
                }
            }
        }

        private void setSelectIndexByValue(ComboBox cb, string cb_value)
        {
            cb.SelectedIndex = -1;
            for (int i = 0; i < cb.Items.Count; i++)
            {
                //GetItemText返回的是 displaymember指定的值
                if (((MyKeyValue)cb.Items[i]).pKey.Equals(cb_value))
                {
                    cb.SelectedIndex = i;
                    break;
                }
            }
        }



        private void button5_Click_2(object sender, EventArgs e)
        {

            if (fullRecordForm == null || fullRecordForm.IsDisposed)
                fullRecordForm = new MyRecordForm(FullDownloadVideoPath, FullReplaceWebPrex);

            fullRecordForm.ShowDialog();

        }

        private void button13_Click(object sender, EventArgs e)
        {
            setTabControlPageDisplay(6);
        }                  
        
        


        private void ControlMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FullKickOff)
            {
                e.Cancel = false;
                return;
            }

            //如果检查到有监控时,则还要提示是否退出?
            AgoraChannelActiveResponse resp = PocClient.queryAgoraChannelActive();
            if (resp!=null && resp.code == 0 && resp.data.Count > 0)
            {
                DialogResult result = MessageBox.Show("发现监控仍在打开, 要求先关闭，请确认？ ", WinFormsStringResource.PromptStr, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;

                } else
                {
                    List<AgoraChannelActiveResponse.Data> datas = resp.data;
                    foreach (AgoraChannelActiveResponse.Data item in datas)
                    {
                        //发送远程,要求停止直播的socket消息

                        if (client != null)
                        {
                            String channelName = item.channelName;
                            int pos = channelName.IndexOf("-");
                            int userId = Convert.ToInt32(channelName.Substring(0, pos));
                            
                            client.SendMessage(
                                                  (new Data()).AVChatNewCommandMessageEncode
                                                  ((short)3, (short)4, LocalSharedData.CURRENTUser.user_id,
                                                  Convert.ToInt32(userId), LocalSharedData.CURRENTUser.user_name,
                                                   LocalSharedData.GetUserName(userId),
                                                  "live_hangup"));
                        }
                       
                    }
                }
               
            } else
            {
                //确定是否关闭
                DialogResult result = MessageBox.Show("确定要关闭？ ", WinFormsStringResource.PromptStr, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    _notifyNetworkDisconnect = false;

                }
            }
            
        }


        private void btn_notify_sos_MouseEnter(object sender, EventArgs e)
        {
            btn_sos_num.BackColor = Color.FromArgb(55, 190, 156);
        }

        private void btn_notify_sos_MouseLeave(object sender, EventArgs e)
        {
            btn_sos_num.BackColor = Color.Transparent;
        }

        private void btn_sos_num_MouseEnter(object sender, EventArgs e)
        {
            btn_notify_sos.BackColor = Color.FromArgb(55, 190, 156);
            btn_sos_num.BackColor = Color.FromArgb(55, 190, 156);
        }

        private void btn_sos_num_MouseLeave(object sender, EventArgs e)
        {
            btn_notify_sos.BackColor = Color.Transparent;
        }

        private void btn_notify_sos_Click(object sender, EventArgs e)
        {

            if (UnReceiveSOSUserIDS.Count > 0)
            {

                //
                string recid = "";
                foreach (long id in UnReceiveSOSUserIDS)
                {
                    if (recid == "")
                        recid = "'" + id + "'";
                    else
                        recid = recid + ",'" + id + "'";
                }

                btn_sos_num.Visible = false;
                UnReceiveSOSUserIDS.Clear();
                //显示sos报警窗口，只查最近的几条=UnReceiveSOSUserIDS.Count
                if (fullRecordForm == null || fullRecordForm.IsDisposed)
                    fullRecordForm = new MyRecordForm(FullDownloadVideoPath, FullReplaceWebPrex);
                
                fullRecordForm.ShowDialog();


            }
        }

        private void dataGridViewSoslog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            if (tbSearch.Text.Trim().Length > 0)
            {
                //出现了
                pbClear.Visible = true;
                tabPageContractSelf.Parent = null;
                tabPageSearch.Parent = tabControl1;
                tbSearch.Focus();
                treeViewSearch.Nodes.Clear();
                //
                TreeNode nodeTalk = this.treeViewSearch.Nodes.Add("对讲终端");
                Group talkGroup = new Group();
                talkGroup.group_type = GroupTypeEnum.CONTACT_TALK_DEVICE;
                talkGroup.group_name = "对讲终端";
                talkGroup.group_id = 1000;
                nodeTalk.Tag = talkGroup;
                nodeTalk.ImageIndex = 0;
                nodeTalk.SelectedImageIndex = 0;
                nodeTalk.ToolTipText = talkGroup.group_name;
                //加入用户
                int ctFind = 0;

                foreach (KeyValuePair<int, User> kv in LocalSharedData.UserAll)
                {
                    if (kv.Value.userId.ToString().Contains(tbSearch.Text.Trim())
                        || kv.Value.userName.Contains(tbSearch.Text.Trim()))
                    {
                        ctFind = ctFind + 1;
                        if (kv.Value.userId == LocalSharedData.CURRENTUser.user_id)
                        {
                            TreeNode user = nodeTalk.Nodes.Add(kv.Value.userName);
                            user.Tag = kv.Value;
                            user.ImageIndex = 6;
                            user.SelectedImageIndex = 6;

                            user.ToolTipText = kv.Value.userName + "【" + kv.Value.userId + "】";
                            //禁言状态
                            if (kv.Value.lifeState != 1)
                            {
                                user.ImageIndex = 14;
                                user.SelectedImageIndex = 14;
                            }
                        }
                        else
                        {
                            if (kv.Value.logon == 1)
                            {
                                TreeNode user = nodeTalk.Nodes.Add(kv.Value.userName);
                                user.Tag = kv.Value;
                                //初始化都当作不在该组,等着组员的报文过来修正
                                user.ImageIndex = 6;
                                user.SelectedImageIndex = 6;
                                user.ToolTipText = kv.Value.userName + "【" + kv.Value.userId + "】";

                                //禁言在已登录上改的
                                if (kv.Value.lifeState != 1)
                                {
                                    user.ImageIndex = 14;
                                    user.SelectedImageIndex = 14;
                                }
                            }
                            else
                            {
                                TreeNode user = nodeTalk.Nodes.Add(kv.Value.userName);
                                user.Tag = kv.Value;
                                user.ImageIndex = 9;
                                user.SelectedImageIndex = 9;
                                user.ToolTipText = kv.Value.userName + "【" + kv.Value.userId + "】";

                            }
                        }                        

                    }
                }
                //
                nodeTalk.Text = "对讲终端【" + ctFind + "】";

                //调度台用户
                TreeNode nodePlatform = this.treeViewSearch.Nodes.Add("调度台用户【0】");
                Group platformGroup = new Group();
                platformGroup.group_type = GroupTypeEnum.CONTACT_PLATFORM_DEVICE;
                platformGroup.group_name = "调度台用户";
                platformGroup.group_id = 2000;
                nodePlatform.Tag = platformGroup;
                nodePlatform.ImageIndex = 12;
                nodePlatform.SelectedImageIndex = 12;
                nodePlatform.ToolTipText = platformGroup.group_name;
                //摄像头终端
                TreeNode nodeCamera = this.treeViewSearch.Nodes.Add("摄像头【0】");
                Group cameraGroup = new Group();
                cameraGroup.group_type = GroupTypeEnum.CONTACT_CAMERA_DEVICE;
                cameraGroup.group_name = "摄像头";
                cameraGroup.group_id = 3000;
                nodeCamera.Tag = cameraGroup;
                nodeCamera.ImageIndex = 15;
                nodeCamera.SelectedImageIndex = 15;
                nodeCamera.ToolTipText = cameraGroup.group_name;


                treeViewSearch.ShowNodeToolTips = true;
                SELECT_TREEVIEW_MODE = 2;
            }
            else
            {
                SELECT_TREEVIEW_MODE = 3;
                pbClear.Visible = false;
                tabPageContractSelf.Parent = tabControl1;
                tabPageSearch.Parent = null;
                tbSearch.Focus();
            }
        }

        private void pbClear_Click(object sender, EventArgs e)
        {
            tbSearch.Text = "";
        }

        private void treeViewSearch_MouseDown(object sender, MouseEventArgs e)
        {
            pi = new Point(e.X, e.Y);
            Control c = sender as Control;
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = this.treeViewSearch.GetNodeAt(e.X, e.Y);

                if (node == null) return;

                SELECT_TREEVIEW_MODE = 2;
                
                //选中鼠标右键按下所在的节点
                this.treeViewSearch.SelectedNode = node;

                //判断组或个人，如果是组且是临时组则出现上下文菜单
                if (node.Tag != null && node.Tag is Group)    //node.Level == 0
                {

                    Group curGrp = (Group)node.Tag;
                    if (curGrp.group_type == GroupTypeEnum.CONTACT_TALK_DEVICE)
                    {
                        //对讲模式下的固定组(含自己)

                        this.GRPcontextMenuStrip.Items["toolStripMenuItemFenceManager"].Visible = false;
                        this.GRPcontextMenuStrip.Items["toolStripMenuItemDelGrp"].Visible = false;
                        this.GRPcontextMenuStrip.Items["toolStripMenuItemModify"].Visible = false;
                        this.GRPcontextMenuStrip.Items["toolStripMenuItemForceAdd"].Visible = false;
                        this.GRPcontextMenuStrip.Show(c, e.Location);

                    }
                    else if (curGrp.group_type == GroupTypeEnum.CONTACT_PLATFORM_DEVICE)
                    {
                        //调度台组 todo
                    }
                    else if (curGrp.group_type == GroupTypeEnum.CONTACT_CAMERA_DEVICE)
                    {
                        //摄像头组  todo                      

                    }

                }
                else if (node.Tag != null && node.Tag is User)    //node.Level == 1
                {
                    if (((User)(node.Tag)).userId != LocalSharedData.CURRENTUser.user_id)
                    {
                        //不是自身帐号的节点
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemTalk"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemView"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemCreateTmpGroup"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemMoni"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemUserPriv"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemGpsSetInfo"].Enabled = true;

                        this.CALLcontextMenuStrip.Items["toolStripMenuItemForceRemove"].Enabled = true;

                    }
                    else
                    {

                        this.CALLcontextMenuStrip.Items["toolStripMenuItemTalk"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemView"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemCreateTmpGroup"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemMoni"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemUserPriv"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemGpsSetInfo"].Enabled = true;

                        this.CALLcontextMenuStrip.Items["toolStripMenuItemForceRemove"].Enabled = false;

                    }

                    this.CALLcontextMenuStrip.Show(c, e.Location);
                }

                //if (node.Nodes.Count != 0) return;

            }
        }

        private void treeViewNotice_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                //选择了组
                if (e.Node.Checked)
                {
                    foreach (TreeNode node in e.Node.Nodes)
                    {
                        node.Checked = true;
                    }
                }
                else
                {
                    foreach (TreeNode node in e.Node.Nodes)
                    {
                        node.Checked = false;
                    }
                }

            }

        }




        private void btn_layout_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //设置显示的位置为鼠标所在的位置, 以下屏蔽不用
                //contextMenuStripLayout.Show(MousePosition.X, MousePosition.Y);

            }
        }

        private void toolStripMenuItemLayout_Talk_Click(object sender, EventArgs e)
        {
            //对讲面板开关切换,涉及到重新排版
            if (this.Full_ShowTalkPanel && this.Full_ShowIPCPanel)
            {
                //以前都显示，这时关闭对讲面板
                tableLayoutPanelBOTTOM.Controls.Remove(this.panelTALK);
                //tableLayoutPanelBOTTOM.Controls.Remove(this.tabControl1);
                //tableLayoutPanelBOTTOM.Controls.Add(this.tabControl1, 0, 1);
                //tableLayoutPanelBOTTOM.SetColumnSpan(this.tabControl1, 2);
                //this.tabControl1.Dock = DockStyle.Fill;
                //
                this.Full_ShowTalkPanel = false;

            }
            else if (!this.Full_ShowTalkPanel && this.Full_ShowIPCPanel)
            {
                //以前对讲面板关闭，IPC打开,这时打开对讲面板
                //tableLayoutPanelBOTTOM.Controls.Remove(this.panelTALK);
                //tableLayoutPanelBOTTOM.Controls.Remove(this.tabControl1);

                tableLayoutPanelBOTTOM.Controls.Add(this.panelTALK, 0, 1);

                tableLayoutPanelBOTTOM.SetColumnSpan(this.panelTALK, 1);
                this.panelTALK.Dock = DockStyle.Fill;

                //tableLayoutPanelBOTTOM.Controls.Add(this.tabControl1, 1, 1);

                //tableLayoutPanelBOTTOM.SetColumnSpan(this.tabControl1, 1);
                //this.tabControl1.Dock = DockStyle.Fill;
                //
                this.Full_ShowTalkPanel = true;

            }
            else if (this.Full_ShowTalkPanel && !this.Full_ShowIPCPanel)
            {
                //以前是对讲打开，IPC关闭，这时关闭对讲面板
                tableLayoutPanelBOTTOM.Controls.Remove(this.panelTALK);
                //tableLayoutPanelBOTTOM.Controls.Remove(this.tabControl1);

                // tableLayoutPanelBOTTOM.Controls.Add(this.tabControl1, 0, 1);
                // tableLayoutPanelBOTTOM.SetColumnSpan(this.tabControl1, 3);

                //this.tabControl1.Dock = DockStyle.Fill;
                //
                this.Full_ShowTalkPanel = false;

            }
            else if (!this.Full_ShowTalkPanel && !this.Full_ShowIPCPanel)
            {

                //以前是对讲关闭，IPC关闭，这时打开对讲面板
                //tableLayoutPanelBOTTOM.Controls.Remove(this.panelTALK);
                // tableLayoutPanelBOTTOM.Controls.Remove(this.tabControl1);

                tableLayoutPanelBOTTOM.Controls.Add(this.panelTALK, 0, 1);
                this.panelTALK.Dock = DockStyle.Fill;

                //tableLayoutPanelBOTTOM.Controls.Add(this.tabControl1, 1, 1);
                //tableLayoutPanelBOTTOM.SetColumnSpan(this.tabControl1, 2);
                //this.tabControl1.Dock = DockStyle.Fill;

                //
                this.Full_ShowTalkPanel = true;

            }

            LoadVideoControl();


        }

        private void toolStripMenuItemLayout_IPC_Click(object sender, EventArgs e)
        {
            //对讲面板开关切换,涉及到重新排版
            if (this.Full_ShowTalkPanel && this.Full_ShowIPCPanel)
            {
                //以前都显示，这时关闭IPC面板
                //tableLayoutPanelBOTTOM.Controls.Remove(this.panelIPC);
                //tableLayoutPanelBOTTOM.Controls.Remove(this.tabControl1);
                // tableLayoutPanelBOTTOM.Controls.Add(this.tabControl1, 1, 1);
                //tableLayoutPanelBOTTOM.SetColumnSpan(this.tabControl1, 2);
                //this.tabControl1.Dock = DockStyle.Fill;
                //
                this.Full_ShowIPCPanel = false;

            }
            else if (!this.Full_ShowTalkPanel && this.Full_ShowIPCPanel)
            {
                //以前对讲面板关闭，IPC打开,这时关闭IPC面板
                //tableLayoutPanelBOTTOM.Controls.Remove(this.panelIPC);

                // tableLayoutPanelBOTTOM.Controls.Remove(this.tabControl1);

                //tableLayoutPanelBOTTOM.Controls.Add(this.tabControl1, 0, 1);

                //tableLayoutPanelBOTTOM.SetColumnSpan(this.tabControl1, 3);
                //this.tabControl1.Dock = DockStyle.Fill;
                //
                this.Full_ShowIPCPanel = false;

            }
            else if (this.Full_ShowTalkPanel && !this.Full_ShowIPCPanel)
            {
                //以前是对讲打开，IPC关闭，这时打开IPC面板

                //tableLayoutPanelBOTTOM.Controls.Remove(this.tabControl1);
                // tableLayoutPanelBOTTOM.Controls.Add(this.tabControl1, 1, 1);
                //tableLayoutPanelBOTTOM.SetColumnSpan(this.tabControl1, 1);
                // this.tabControl1.Dock = DockStyle.Fill;
                //
                //tableLayoutPanelBOTTOM.Controls.Add(this.panelIPC, 2, 1);
                //tableLayoutPanelBOTTOM.SetColumnSpan(this.panelIPC, 1);
                //this.panelIPC.Dock = DockStyle.Fill;
                //
                this.Full_ShowIPCPanel = true;

            }
            else if (!this.Full_ShowTalkPanel && !this.Full_ShowIPCPanel)
            {

                //以前是对讲关闭，IPC关闭，这时打开IPC面板
                //tableLayoutPanelBOTTOM.Controls.Remove(this.panelTALK);
                //tableLayoutPanelBOTTOM.Controls.Remove(this.tabControl1);

                //tableLayoutPanelBOTTOM.Controls.Add(this.tabControl1, 0, 1);
                // tableLayoutPanelBOTTOM.SetColumnSpan(this.tabControl1, 2);
                //this.tabControl1.Dock = DockStyle.Fill;

                //tableLayoutPanelBOTTOM.Controls.Add(this.panelIPC, 2, 1);
                //this.panelIPC.Dock = DockStyle.Fill;

                //
                this.Full_ShowIPCPanel = true;

            }

            LoadVideoControl();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //

        }

        private void contextMenuStripVideoIPC_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            /*
            UserControlVideo video = (UserControlVideo)((ContextMenuStrip)sender).SourceControl;
            if (((ContextMenuStrip)sender).Items["toolStripMenuItemCaptureIPC"] == e.ClickedItem)
            {
                //源目标,当前文件名称: userid+时间组成
                string filename = video.userid.ToString() + "_" + DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss");
                FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
                if (Capture_Path.Equals(""))
                {
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string path = folderBrowserDialog1.SelectedPath;
                        Capture_Path = path;

                    }
                    else
                    {
                        return;
                    }
                }
                MediaPlayer.TakeSnapShot(video.vlc_media_player, Capture_Path, filename + ".jpg");

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemVideoRefreshIPC"] == e.ClickedItem)
            {
                //toolStripMenuItemRefresh
                MediaPlayer.NetWork_Media_Play(video,video.vlc_instance, video.vlc_media_player,
                   video.streamurl);

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemResizeIPC"] == e.ClickedItem)
            {
                //LoadVideoControl_Max                
                if (video.ismaxsize == 1)
                    LoadVideoControl_IPC_ResizeNormal();
                else
                    LoadVideoControl_IPC_ResizeMax(video);

            }
            else if (((ContextMenuStrip)sender).Items["toolStripMenuItemVolumeIPC"] == e.ClickedItem)
            {
                //toolStripMenuItemVolume 音量控制

                if (video.volume == 1)
                {
                    MediaPlayer.SetVolume(video.vlc_media_player, 0);
                    video.volume = 0;
                }
                else
                {
                    MediaPlayer.SetVolume(video.vlc_media_player, 100);
                    video.volume = 1;

                }
            }

            */

        }




        private void tb_qrcodenfcpageindex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20)
                e.KeyChar = (char)0;  //禁止空格键
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //处理负数
            if (e.KeyChar > 0x20)
            {
                try
                {
                    Int32.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }
        }

        private void dataGridViewQRCodeNFC_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {


        }

        private void timerGPSReq_Tick(object sender, EventArgs e)
        {
            //系统定时请求所有人的定位GPS, XBC的要求，因为有时终端没有即时上报GPS
            Debug.WriteLine("发送定时请求GPS命令......");
            if (Full_LoopGPSCommand_Node != null)
            {
                foreach (TreeNode nodea in Full_LoopGPSCommand_Node.Nodes)
                {
                    //
                    int MY_GPSCommand_GROUP_ID = ((Group)Full_LoopGPSCommand_Node.Tag).group_id;
                    int MY_GPSCommand_USER_ID = ((User)nodea.Tag).userId;
                    int accuracy = Full_LoopGPSCommand_Accuracy;
                    client.SendMessage(
            (new Data()).GPSCommandMessageEncode(MY_GPSCommand_GROUP_ID,
            MY_GPSCommand_USER_ID, Convert.ToByte(accuracy), LocalSharedData.CURRENTUser.user_id));

                }
            }


        }

        private void panelLogo_Click(object sender, EventArgs e)
        {

        }

        private void panelLogo_MouseClick(object sender, MouseEventArgs e)
        {
            if (webBrower != null)
            {
                webBrower.GetBrowser().Reload(true);
            }
        }

        private void btn_rec_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    //设置显示的位置为鼠标所在的位置  
            //    contextMenuStripExtend.Show(MousePosition.X, MousePosition.Y);

            //}
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {

        }     
              

        


        private void addNoAssocDeptGroup(TreeNode noAssoc_DeptRoot, int userId)
        {
            List<Group> noAssocFixGrpList = HttpAPI.userAllGroup(userId);

            if (noAssocFixGrpList != null && noAssocFixGrpList.Count > 0)
            {
                foreach (Group grp in noAssocFixGrpList)
                {
                    if (LocalSharedData.UserAllGROUP.Find(a => a.group_id == grp.group_id) == null
                        && LocalSharedData.OrgAsscGROUP_NoSelf.Find(a => a.group_id == grp.group_id) == null)
                    {
                        grp.group_type = GroupTypeEnum.ORG_UNASSC_FIX;
                        LocalSharedData.UserAllGROUP.Add(grp);
                        TreeNode node = noAssoc_DeptRoot.Nodes.Add(grp.group_name);
                        node.Tag = grp;
                        node.ImageIndex = 11;
                        node.SelectedImageIndex = 11;

                        //再增加人员
                        List<User> us = new List<User>();
                        GroupUserMemberResponse resp = PocClient.queryGroupMemberByGroupId(grp.group_id);
                        if (resp != null && resp.data != null && resp.data.Count > 0)
                            us = resp.data;

                        foreach (User user in us)
                        {
                            TreeNode nodess = node.Nodes.Add(user.userName);
                            nodess.Tag = user;
                            nodess.ImageIndex = 3;
                            nodess.SelectedImageIndex = 3;
                            if (!LocalSharedData.UserAll.ContainsKey(user.userId))
                                LocalSharedData.UserAll.Add(user.userId, user);

                        }
                        if (!LocalSharedData.GROUPAllUser.ContainsKey(grp.group_id))
                        {
                            LocalSharedData.GROUPAllUser.Add(grp.group_id, us);
                            allGroupComboboxItems.Add(new MyKeyValue(grp.group_id.ToString(), grp.group_name));
                        }


                    }

                }

            }

            List<Group> noAssocTmpGrpList = HttpAPI.userAllTempGroup(userId);
            if (noAssocTmpGrpList != null && noAssocTmpGrpList.Count > 0)
            {
                foreach (Group grp in noAssocTmpGrpList)
                {
                    if (LocalSharedData.UserAllGROUP.Find(a => a.group_id == grp.group_id) == null
                        && LocalSharedData.OrgAsscGROUP_NoSelf.Find(a => a.group_id == grp.group_id) == null)
                    {
                        grp.group_type = GroupTypeEnum.ORG_UNASSC_TMP;
                        LocalSharedData.UserAllTempGROUP.Add(grp);
                        TreeNode node = noAssoc_DeptRoot.Nodes.Add(grp.group_name);
                        node.Tag = grp;
                        node.ImageIndex = 11;
                        node.SelectedImageIndex = 11;

                        //再增加人员
                        List<User> us = new List<User>();

                        GroupUserMemberResponse resp = PocClient.queryTmpGroupMemberByGroupId(grp.group_id);
                        if (resp != null && resp.data != null && resp.data.Count > 0)
                        {
                            us = resp.data;
                        }

                        foreach (User user in us)
                        {
                            TreeNode nodess = node.Nodes.Add(user.userName);
                            nodess.Tag = user;
                            nodess.ImageIndex = 3;
                            nodess.SelectedImageIndex = 3;
                            if (!LocalSharedData.UserAll.ContainsKey(user.userId))
                                LocalSharedData.UserAll.Add(user.userId, user);
                        }

                        if (!LocalSharedData.GROUPAllUser.ContainsKey(grp.group_id))
                        {
                            LocalSharedData.GROUPAllUser.Add(grp.group_id, us);
                            allGroupComboboxItems.Add(new MyKeyValue(grp.group_id.ToString(), grp.group_name));
                        }

                    }

                }

            }

        }

        
        /*
        private TreeNode findFartherNode(TreeNode parent, int orgId)
        {
            if (parent == null) return null;
            if (parent.Tag != null)
            {
                if (parent.Tag is DapperOrg && ((DapperOrg)parent.Tag).orgid == orgId)

                    return parent;
            }

            TreeNode tnRet = null;

            foreach (TreeNode tn in parent.Nodes)
            {

                tnRet = findFartherNode(tn, orgId);

                if (tnRet != null) break;

            }

            return tnRet;


        }
        */


        private void InitTreeNodes_Contact()
        {
            TreeNode firstNode = null;
            TreeNodeCollection gtds = this.treeViewContact.Nodes;
            //清空
            foreach (TreeNode it in gtds)
                it.Nodes.Clear();
            this.treeViewContact.Nodes.Clear();
            //
            TreeNode nodeTalk = this.treeViewContact.Nodes.Add("对讲终端");
            Group talkGroup = new Group();
            talkGroup.group_type= GroupTypeEnum.CONTACT_TALK_DEVICE;
            talkGroup.group_name = "对讲终端";
            talkGroup.group_id = 1000;
            nodeTalk.Tag = talkGroup;
            nodeTalk.ImageIndex = 0;
            nodeTalk.SelectedImageIndex = 0;
            nodeTalk.ToolTipText = talkGroup.group_name;
            //加入用户
            int ct = LocalSharedData.UserAllGroupCustomer.Count;
            List<User> us = LocalSharedData.UserAllGroupCustomer;
            for (int j = 0; j < ct; j++)
            {
                if (us[j].userId == LocalSharedData.CURRENTUser.user_id)
                {
                    TreeNode user = nodeTalk.Nodes.Add(us[j].userName);
                    user.Tag = us[j];
                    user.ImageIndex = 6;
                    user.SelectedImageIndex = 6;
                    
                    user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                    //禁言状态
                    if (us[j].lifeState != 1)
                    {
                        user.ImageIndex = 14;
                        user.SelectedImageIndex = 14;
                    }
                }
                else
                {
                    if (us[j].logon == 1)
                    {
                        TreeNode user = nodeTalk.Nodes.Add(us[j].userName);                         
                        user.Tag = us[j];
                        //初始化都当作不在该组,等着组员的报文过来修正
                        user.ImageIndex = 6;
                        user.SelectedImageIndex = 6;
                        user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                        
                        //禁言在已登录上改的
                        if (us[j].lifeState != 1)
                        {
                            user.ImageIndex = 14;
                            user.SelectedImageIndex = 14;
                        }
                    }
                    else
                    {
                        TreeNode user = nodeTalk.Nodes.Add(us[j].userName);                        
                        user.Tag = us[j];
                        user.ImageIndex = 9;
                        user.SelectedImageIndex = 9;
                        user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";

                    }
                }
            }
            //调度台用户
            TreeNode nodePlatform = this.treeViewContact.Nodes.Add("调度台用户");
            Group platformGroup = new Group();
            platformGroup.group_type = GroupTypeEnum.CONTACT_PLATFORM_DEVICE;
            platformGroup.group_name = "调度台用户";
            platformGroup.group_id = 2000;
            nodePlatform.Tag = platformGroup;
            nodePlatform.ImageIndex = 12;
            nodePlatform.SelectedImageIndex = 12;
            nodePlatform.ToolTipText = platformGroup.group_name;
            //摄像头终端
            TreeNode nodeCamera = this.treeViewContact.Nodes.Add("摄像头");
            Group cameraGroup = new Group();
            cameraGroup.group_type = GroupTypeEnum.CONTACT_CAMERA_DEVICE;
            cameraGroup.group_name = "摄像头";
            cameraGroup.group_id = 3000;
            nodeCamera.Tag = cameraGroup;
            nodeCamera.ImageIndex = 15;
            nodeCamera.SelectedImageIndex = 15;
            nodeCamera.ToolTipText = cameraGroup.group_name;
            //后续从表中读取所有该cmpid下的摄像头


            //电子围栏
            TreeNode nodeFence = this.treeViewContact.Nodes.Add("电子围栏");
            Group fenceGroup = new Group();
            fenceGroup.group_type = GroupTypeEnum.CONTACT_FENCE;
            fenceGroup.group_name = "电子围栏";
            fenceGroup.group_id = 4000;
            nodeFence.Tag = fenceGroup;
            nodeFence.ImageIndex = 16;
            nodeFence.SelectedImageIndex = 16;
            nodeFence.ToolTipText = fenceGroup.group_name;

            //拉取该 cmpid所有的电子围栏


            this.treeViewContact.ShowNodeToolTips = true;

        }

        private void InitTreeNodes()
        {

            //当前用户所在的组            
            LocalSharedData.UserAllGROUP = Utils.ConvertNewGroupList(PocClient.userAllGroup());
            foreach (Group grp in LocalSharedData.UserAllGROUP)
                grp.group_type = GroupTypeEnum.TALK_FIX;

            //当前用户所在的临时组, 临时组要把 广播放在顶层，通过 ptt_group_temp表中class字段为1，来倒序实现           
            LocalSharedData.UserAllTempGROUP = Utils.ConvertNewGroupTempList(PocClient.userAllTempGroup());
            foreach (Group grp in LocalSharedData.UserAllTempGROUP)
            {
                if (grp.aclass.HasValue && grp.aclass.Value==1)
                    grp.group_type = GroupTypeEnum.TALK_TMP_BROADCAST;
                else
                    grp.group_type = GroupTypeEnum.TALK_TMP;

            }                


            GroupCustomerVoResponse resp = PocClient.getGroupCustomerVo();
            if (resp != null && resp.data != null && resp.data.Count > 0)
            {
                foreach (User it in resp.data)
                {
                    if (!LocalSharedData.UserAll.ContainsKey(it.userId))
                        LocalSharedData.UserAll.Add(it.userId, it);
                }
            }


            TreeNode firstNode = null;

            TreeNodeCollection gtds = this.treeViewGROUP.Nodes;
            //清空
            foreach (TreeNode it in gtds)
                it.Nodes.Clear();
            this.treeViewGROUP.Nodes.Clear();

            LocalSharedData.GROUPAllUser.Clear();
            allGroupComboboxItems.Clear();
            enableGroupSwitch = false;

            if (LocalSharedData.UserAllGROUP.Count > 0)
            {
                LocalSharedData.CURRENTUser.group_id = LocalSharedData.UserAllGROUP[0].group_id;
                LocalSharedData.CURRENTGroupName = LocalSharedData.UserAllGROUP[0].group_name;
                //
                FirstGROUP_ID = LocalSharedData.CURRENTUser.group_id;

            }

            for (int i = 0; i < LocalSharedData.UserAllGROUP.Count; ++i)
            {
                TreeNode it = this.treeViewGROUP.Nodes.Add(LocalSharedData.UserAllGROUP[i].group_name);

                if (i == 0)
                    firstNode = it;

                LocalSharedData.UserAllGROUP[i].group_type = GroupTypeEnum.TALK_FIX;
                it.Tag = LocalSharedData.UserAllGROUP[i];
                it.ImageIndex = 0;
                it.SelectedImageIndex = 0;
                it.ToolTipText = LocalSharedData.UserAllGROUP[i].group_name;

                List<User> us = new List<User>();

                GroupUserMemberResponse resp2 = PocClient.queryGroupMemberByGroupId(LocalSharedData.UserAllGROUP[i].group_id);
                if (resp2 != null && resp2.data != null && resp2.data.Count > 0)
                    us = resp2.data;

                LocalSharedData.GROUPAllUser.Add(LocalSharedData.UserAllGROUP[i].group_id, us);

                for (int j = 0; j < us.Count; ++j)
                {
                    if (us[j].userId == LocalSharedData.CURRENTUser.user_id)
                    {
                        TreeNode user = it.Nodes.Add(us[j].userName);

                        user.Tag = us[j];

                        if (((Group)it.Tag).group_id == LocalSharedData.CURRENTUser.group_id)
                        {
                            user.ImageIndex = 3;
                            user.SelectedImageIndex = 3;
                        }
                        else
                        {
                            user.ImageIndex = 6;
                            user.SelectedImageIndex = 6;
                        }
                        user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                        //禁言状态
                        if (us[j].lifeState != 1)
                        {
                            user.ImageIndex = 14;
                            user.SelectedImageIndex = 14;
                        }

                    }
                    else
                    {
                        if (us[j].logon == 1)
                        {
                            TreeNode user = it.Nodes.Add(us[j].userName);
                            //user.Tag = us[j].user_id;
                            user.Tag = us[j];
                            //初始化都当作不在该组,等着组员的报文过来修正
                            user.ImageIndex = 6;
                            user.SelectedImageIndex = 6;
                            user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";

                            if (mfirstlogonuserid == 0 && mfirstlogongroupid == 0)
                            {
                                mfirstlogonuserid = us[j].userId;
                                mfirstlogongroupid = LocalSharedData.UserAllGROUP[i].group_id;
                            }
                            //禁言在已登录上改的
                            if (us[j].lifeState != 1)
                            {
                                user.ImageIndex = 14;
                                user.SelectedImageIndex = 14;
                            }

                        }
                        else
                        {
                            TreeNode user = it.Nodes.Add(us[j].userName);
                            //user.Tag = us[j].user_id;
                            user.Tag = us[j];
                            user.ImageIndex = 9;
                            user.SelectedImageIndex = 9;
                            user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";

                        }
                    }
                }

            }

            for (int i = 0; i < LocalSharedData.UserAllTempGROUP.Count; ++i)
            {
                TreeNode it =null;
                if (LocalSharedData.UserAllTempGROUP[i].aclass.Value==1)
                    it = this.treeViewGROUP.Nodes.Add("[广播]" + LocalSharedData.UserAllTempGROUP[i].group_name);
                else
                    it = this.treeViewGROUP.Nodes.Add("[临时]" + LocalSharedData.UserAllTempGROUP[i].group_name);

                it.Tag = LocalSharedData.UserAllTempGROUP[i];
                it.ToolTipText = LocalSharedData.UserAllTempGROUP[i].group_name;

                List<User> us = new List<User>();

                GroupUserMemberResponse resp2 = PocClient.queryTmpGroupMemberByGroupId(LocalSharedData.UserAllTempGROUP[i].group_id);
                if (resp2 != null && resp2.data != null && resp2.data.Count > 0)
                {
                    us = resp2.data;
                }

                LocalSharedData.GROUPAllUser.Add(LocalSharedData.UserAllTempGROUP[i].group_id, us);

                for (int j = 0; j < us.Count; ++j)
                {
                    if (us[j].userId == LocalSharedData.CURRENTUser.user_id)
                    {
                        TreeNode user = it.Nodes.Add(us[j].userName);

                        user.Tag = us[j];
                        if (((Group)it.Tag).group_id == LocalSharedData.CURRENTUser.group_id)
                        {
                            user.ImageIndex = 3;
                            user.SelectedImageIndex = 3;
                        }
                        else
                        {
                            user.ImageIndex = 6;
                            user.SelectedImageIndex = 6;
                        }

                        user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                        if (us[j].lifeState != 1)
                        {
                            user.ImageIndex = 14;
                            user.SelectedImageIndex = 14;
                        }

                    }
                    else
                    {
                        if (us[j].logon == 1)
                        {
                            TreeNode user = it.Nodes.Add(us[j].userName);
                            //user.Tag = us[j].user_id;
                            user.Tag = us[j];
                            //初始化都当作不在该组,等着组员的报文过来修正
                            user.ImageIndex = 6;
                            user.SelectedImageIndex = 6;
                            user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                            if (us[j].lifeState != 1)
                            {
                                user.ImageIndex = 14;
                                user.SelectedImageIndex = 14;
                            }

                        }
                        else
                        {
                            TreeNode user = it.Nodes.Add(us[j].userName);
                            //user.Tag = us[j].user_id;
                            user.Tag = us[j];
                            user.ImageIndex = 9;
                            user.SelectedImageIndex = 9;
                            user.ToolTipText = us[j].userName + "【" + us[j].userId + "】";
                        }
                    }
                }

            }
            if (firstNode != null)
            {
                this.treeViewGROUP.SelectedNode = firstNode;
                this.treeViewGROUP.SelectedNode.ExpandAll();
            }

            this.treeViewGROUP.ShowNodeToolTips = true;

        }

        private void btn_layout_Click(object sender, EventArgs e)
        {
            //对讲面板开关切换,涉及到重新排版
            if (this.Full_ShowTalkPanel)
            {
                //以前都显示，这时关闭对讲面板
                tableLayoutPanelBOTTOM.Controls.Remove(this.panelTALK);

                tableLayoutPanelBOTTOM.Controls.Remove(this.panel17);
                tableLayoutPanelBOTTOM.Controls.Add(this.panel17, 0, 1);
                //tableLayoutPanelBOTTOM.SetColumn(this.panel17, 0);
                tableLayoutPanelBOTTOM.SetColumnSpan(this.panel17, 3);
                this.panel17.Dock = DockStyle.Fill;
                //
                this.Full_ShowTalkPanel = false;

            }
            else if (!this.Full_ShowTalkPanel)
            {
                //以前对讲面板关闭，IPC打开,这时打开对讲面板
                tableLayoutPanelBOTTOM.Controls.Remove(this.panelTALK);
                tableLayoutPanelBOTTOM.Controls.Remove(this.panel17);

                tableLayoutPanelBOTTOM.Controls.Add(this.panel17, 1, 1);
                tableLayoutPanelBOTTOM.SetColumnSpan(this.panel17, 2);
                this.panel17.Dock = DockStyle.Fill;

                tableLayoutPanelBOTTOM.Controls.Add(this.panelTALK, 0, 1);

                tableLayoutPanelBOTTOM.SetColumnSpan(this.panelTALK, 1);
                this.panelTALK.Dock = DockStyle.Fill;


                //
                this.Full_ShowTalkPanel = true;

            }

            //LoadVideoControl();

        }

        private void timerDate_Tick(object sender, EventArgs e)
        {
            labDate.Text = DateTime.Now.ToString(" HH:mm:ss dddd yyyy/MM/dd");
        }

        private void button5_Click_4(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button3_Click_3(object sender, EventArgs e)
        {
            this.Close();
        }

        private void minMaxSwitchWindow()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                btnMaxNorm.BackgroundImage = Properties.Resources.mainwnd_restore;

            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                btnMaxNorm.BackgroundImage = Properties.Resources.mainwnd_fullscreen;
            }
        }

        private void button4_Click_3(object sender, EventArgs e)
        {
            minMaxSwitchWindow();
        }

        private void btnCloseX_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click_4(object sender, EventArgs e)
        {

            //查询当前组有无围栏列表
            List<FenceInfo> myfence = null;
            AllFenceInfoResponse resp = PocClient.queryAllFenceInfo(LocalSharedData.CURRENTUser.cmpid);
            if (resp != null && resp.data != null)
                myfence = resp.data;

            if (myfence == null || myfence.Count == 0)
            {
                MessageBox.Show("未发现电子围栏, 可在地图上先创建围栏");
                return;
            }

            if (fenceRuleForm == null || fenceRuleForm.IsDisposed)
                fenceRuleForm = new FenceRuleDef();

            fenceRuleForm.cbFencelist.Items.Clear();
            ArrayList lists_cbFencelist = new ArrayList();

            foreach (FenceInfo fence in myfence)
                lists_cbFencelist.Add(new MyKeyValue(fence.fenceId.ToString(), fence.fenceName));

            fenceRuleForm.cbFencelist.DisplayMember = "pValue";
            fenceRuleForm.cbFencelist.ValueMember = "pKey";
            fenceRuleForm.cbFencelist.DataSource = lists_cbFencelist;

            fenceRuleForm.full_fence_id = myfence[0].fenceId;

            fenceRuleForm.cbFencelist.SelectedIndexChanged += CbFencelist_SelectedIndexChanged;

            List<FenceUserDto> data = new List<FenceUserDto>();
            AllFenceUserInfoResponse resp2 = PocClient.queryAllFenceUserInfo(myfence[0].fenceId);
            if (resp2 != null && resp2.data != null)
                data = resp2.data;


            fenceRuleForm.dataGridViewRule.AutoGenerateColumns = false;

            fenceRuleForm.dataGridViewRule.DataSource = data;

            if (fenceRuleForm.ShowDialog() == DialogResult.OK)
            {
                fenceRuleForm.Dispose();
                fenceRuleForm = null;
            }

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            /*
            if (fullCov19ScanQueryForm == null || fullCov19ScanQueryForm.IsDisposed)
                fullCov19ScanQueryForm = new Cov19ScanQueryForm();

            fullCov19ScanQueryForm.ShowDialog();
            */
        }

        private void btn_live_Click(object sender, EventArgs e)
        {
            if (fullAVLiveCenWin == null || fullAVLiveCenWin.IsDisposed)
            {
                fullAVLiveCenWin = new AVLiveCenterForm(client);
            }

            fullAVLiveCenWin.Show();
            //加入所有的在线频道（直播与监控）
            AgoraChannelActiveResponse resp= PocClient.queryAgoraChannelActive();
            if (resp.code==0 && resp.data.Count > 0)
            {
                List<AgoraChannelActiveResponse.Data>  datas = resp.data;
                foreach(AgoraChannelActiveResponse.Data item in datas)
                {
                    LivePushDto dto = new LivePushDto();
                    dto.channelName = item.channelName;
                    int pos = dto.channelName.IndexOf("-");
                    int userId = Convert.ToInt32(dto.channelName.Substring(0, pos));
                    dto.userId = userId;
                    dto.userName = LocalSharedData.GetUserName(userId);
                    fullAVLiveCenWin.JoinChannel(dto);
                }
            }

        }

        private void panel39_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void panel15_MouseDown(object sender, MouseEventArgs e)
        {
            if (formResizeMode == 0)
            {
                return;    // 若无缩放模式，则返回
            }
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, formResizeMode, 0);
        }

        private void ControlMainForm_MouseDown(object sender, MouseEventArgs e)
        {

        }

        public void MouseMove_Resize(object sender, MouseEventArgs e)
        {
            Control ctrl = (Control)sender;             // 获取鼠标处的控件
            Rectangle client = ctrl.ClientRectangle;    // 获取控件尺寸

            setSizeMode(client, new Point(e.X, e.Y), 10);   // 根据鼠标在区域client中的位置设置缩放模式

            if (formResizeMode == 0)
            {
                this.Cursor = Cursors.Default;
                return;    // 若无缩放模式，则返回
            }
            switch (formResizeMode)
            {
                case WMSZ_LEFT:
                case WMSZ_RIGHT:
                    this.Cursor = Cursors.SizeWE;
                    break;
                case WMSZ_TOP:
                case WMSZ_BOTTOM:
                    this.Cursor = Cursors.SizeNS;
                    break;
                case WMSZ_TOPLEFT:
                case WMSZ_BOTTOMRIGHT:
                    this.Cursor = Cursors.SizeNWSE;
                    break;
                case WMSZ_TOPRIGHT:
                case WMSZ_BOTTOMLEFT:
                    this.Cursor = Cursors.SizeNESW;
                    break;

            }

        }

        // 根据坐标 x,y 在rect上的位置控制尺寸调节模式
        private void setSizeMode(Rectangle R, Point P, int W)
        {
            formResizeMode = getSizeMode(R, P, W);
        }

        // 根据坐标 x,y 在rect上的位置控制尺寸调节模式
        private int getSizeMode(Rectangle R, Point P, int W)
        {

            // 中间区域
            Rectangle main = new Rectangle(R.Left + W, R.Top + W, R.Width - 2 * W, R.Height - 2 * W);
            if (main.Contains(P)) return 0;

            // 左侧区域
            Rectangle LeftRect = new Rectangle(R.Left + W, R.Top + W, W, R.Height - 2 * W);
            if (LeftRect.Contains(P)) return WMSZ_LEFT;

            // 右侧区域
            Rectangle RightRect = new Rectangle(R.Right - W, R.Top + W, W, R.Height - 2 * W);
            if (RightRect.Contains(P)) return WMSZ_RIGHT;

            // 顶部区域
            Rectangle TopRect = new Rectangle(R.Left + W, R.Top, R.Width - 2 * W, W);
            if (TopRect.Contains(P)) return WMSZ_TOP;

            // 底部区域
            Rectangle BottomRect = new Rectangle(R.Left + W, R.Bottom - W, R.Width - 2 * W, W);
            if (BottomRect.Contains(P)) return WMSZ_BOTTOM;

            // 左上区域
            Rectangle TOPLEFT = new Rectangle(R.Left, R.Top, W, W);
            if (TOPLEFT.Contains(P)) return WMSZ_TOPLEFT;

            // 右上区域
            Rectangle TOPRIGHT = new Rectangle(R.Right - W, R.Top, W, W);
            if (TOPRIGHT.Contains(P)) return WMSZ_TOPRIGHT;

            // 左下区域
            Rectangle BOTTOMLEFT = new Rectangle(R.Left, R.Bottom - W, W, W);
            if (BOTTOMLEFT.Contains(P)) return WMSZ_BOTTOMLEFT;

            // 右下区域
            Rectangle BOTTOMRIGHT = new Rectangle(R.Right - W, R.Bottom - W, W, W);
            if (BOTTOMRIGHT.Contains(P)) return WMSZ_BOTTOMRIGHT;

            return 0;
        }

        private void panel15_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove_Resize(sender, e);
        }

        private void tableLayoutPanelBOTTOM_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void tableLayoutPanelBOTTOM_MouseDown(object sender, MouseEventArgs e)
        {
            if (formResizeMode == 0)
            {
                return;    // 若无缩放模式，则返回
            }
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, formResizeMode, 0);
        }

        private void panel39_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void panelTopControl_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void tableLayoutPanel6_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void panelLogo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void panel39_DoubleClick(object sender, EventArgs e)
        {
            minMaxSwitchWindow();
        }

        private void label2_DoubleClick(object sender, EventArgs e)
        {
            minMaxSwitchWindow();
        }

        private void panelLogo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            minMaxSwitchWindow();
        }

        private void panel15_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private String readRunPara(String section, String key)
        {
            //读取配置文件
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, PARAFILE);
            //指定ini文件的路径
            IniFile ini = new IniFile(path);
            if (ini.IniReadValue(section, key).Equals(""))
                return "480p";
            else
                return ini.IniReadValue(section, key);

        }

        private void treeViewContact_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //
            if (e.Action == TreeViewAction.ByMouse)
            {
                
                if (e.Node.Checked)
                {
                    // 取消节点选中状态之后，取消所有父节点的选中状态
                    setChildNodeCheckedState(e.Node, true);

                }
                else
                {
                    // 取消节点选中状态之后，取消所有父节点的选中状态
                    setChildNodeCheckedState(e.Node, false);
                    // 如果节点存在父节点，取消父节点的选中状态
                    if (e.Node.Parent != null)
                    {
                        setParentNodeCheckedState(e.Node, false);
                    }
                }
            }
        }

        // 取消节点选中状态之后，取消所有父节点的选中状态
        private void setParentNodeCheckedState(TreeNode currNode, bool state)
        {
            TreeNode parentNode = currNode.Parent;

            parentNode.Checked = state;
            if (currNode.Parent.Parent != null)
            {
                setParentNodeCheckedState(currNode.Parent, state);
            }
        }


        // 选中节点之后，选中节点的所有子节点
        private void setChildNodeCheckedState(TreeNode currNode, bool state)
        {
            TreeNodeCollection nodes = currNode.Nodes;
            if (nodes.Count > 0)
                foreach (TreeNode tn in nodes)
                {

                    tn.Checked = state;
                    setChildNodeCheckedState(tn, state);
                }
        }

        private void CALLcontextMenuStrip_MouseHover(object sender, EventArgs e)
        {

        }

        private void treeViewContact_MouseDown(object sender, MouseEventArgs e)
        {
            piContact = new Point(e.X, e.Y);
            Control c = sender as Control;
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = this.treeViewContact.GetNodeAt(e.X, e.Y);
                if (node == null) return;

                SELECT_TREEVIEW_MODE = 3;

                //选中鼠标右键按下所在的节点
                this.treeViewContact.SelectedNode = node;
                
                //判断组或个人，如果是组且是临时组则出现上下文菜单
                if (node.Tag != null && node.Tag is Group)    //node.Level == 0
                {                    
                     
                    Group curGrp = (Group)node.Tag;

                    GroupService.GroupContextMenuItemDisplay(curGrp.group_type, this.GRPcontextMenuStrip);

                    this.GRPcontextMenuStrip.Show(c, e.Location);

                }
                else if (node.Tag != null && node.Tag is User)    //node.Level == 1
                {
                    if (((User)(node.Tag)).userId != LocalSharedData.CURRENTUser.user_id)
                    {
                        //不是自身帐号的节点
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemTalk"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemView"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemCreateTmpGroup"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemMoni"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemUserPriv"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemGpsSetInfo"].Enabled = true;

                        this.CALLcontextMenuStrip.Items["toolStripMenuItemForceRemove"].Enabled = true;

                    }
                    else
                    {

                        this.CALLcontextMenuStrip.Items["toolStripMenuItemTalk"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemView"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemCreateTmpGroup"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemMoni"].Enabled = false;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemUserPriv"].Enabled = true;
                        this.CALLcontextMenuStrip.Items["toolStripMenuItemGpsSetInfo"].Enabled = true;

                        this.CALLcontextMenuStrip.Items["toolStripMenuItemForceRemove"].Enabled = false;

                    }

                    this.CALLcontextMenuStrip.Show(c, e.Location);
                }

                //if (node.Nodes.Count != 0) return;

            }
        }

        private void btnBatchMoni_Click(object sender, EventArgs e)
        {
            HashSet<String> userList = new HashSet<string>(); //要去重

            //批量监控
            userList = GroupService.SelectedUserNode(this.SELECT_TREEVIEW_MODE, this.treeViewSearch, this.treeViewContact);

            userList.Remove(LocalSharedData.CURRENTUser.user_id + "");
           

            if (userList.Count==0)
            {
                MessageBox.Show("请选中用户");
                return;
            }

            DialogResult dr= MessageBox.Show("确认监控用户数:" + userList.Count, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {
                String moni_resolv = readRunPara("video", "moni_resolv");

                foreach (String userId in userList)
                {

                    JObject staff = new JObject();
                    staff.Add(new JProperty("toUserIds", userId));
                    staff.Add(new JProperty("moniResolv", moni_resolv));

                    client.SendMessage(
                                   (new Data()).AVRemoteMoniMesssageEncode((byte)1, LocalSharedData.CURRENTUser.user_id,
                                    JsonConvert.SerializeObject(staff)));

                    Task.Delay(20).Wait();  //用这个休眠，不堵塞主线程
                }
            }            
           

        }

        private void tabControl6_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SELECT_TREEVIEW_MODE
            if (tabControl6.SelectedIndex == 0)
            {
                SELECT_TREEVIEW_MODE = 1;
            } else if (tabControl6.SelectedIndex == 1)
            {
                SELECT_TREEVIEW_MODE = 3;
            }
        }

        private void GRPcontextMenuStrip_Opening(object sender, CancelEventArgs e)
        {

        }

        private void btnCreateGroup_Click(object sender, EventArgs e)
        {
            //要把本身UserId加进去
            HashSet<String> userList = new HashSet<string>(); //要去重            
            userList = GroupService.SelectedUserNode(this.SELECT_TREEVIEW_MODE, this.treeViewSearch, this.treeViewContact);

            userList.Remove(LocalSharedData.CURRENTUser.user_id + ""); //先删除是为了判断有无选别人

            if (userList.Count == 0)
            {
                MessageBox.Show("请选中用户");
                return;
            }           
            userList.Add(LocalSharedData.CURRENTUser.user_id + "");

            string grpName_ = "临时对讲";

            //创建临时组
            String  memberstr = "";
            foreach (string userId in userList )
            {                
                if (memberstr.Equals(""))
                    memberstr = userId;
                 else
                    memberstr = memberstr + "," + userId;                    
            }

            string utcTimeStamp = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmssfff");


            GroupTempDto dto = new GroupTempDto();
            dto.groupName = grpName_+ utcTimeStamp.Substring(10,7); //取 mmssfff 部分，共7位
            dto.userIds = memberstr;
            dto.ownerId = LocalSharedData.CURRENTUser.user_id;
            dto.priv = 1;    //调度员发起的为1, 终端发起的为0             
            dto.aclass = 0;
            Group grptmp = null;
            TempGroupResponse resp_t = PocClient.createTmpGroup(dto);
            if (resp_t != null)
            {
                grptmp = new Group();
                grptmp.group_id = resp_t.data.groupId;
                grptmp.group_name = resp_t.data.groupName;
                grptmp.owner_id = resp_t.data.ownerId.Value;
                grptmp.user_ids = resp_t.data.userIds;
            }

            //这里的临时组不用在树中保存（即要求server不要回发创建组消息给PC端），因为一旦关闭就会删除的
            if (grptmp == null)
            {
                MessageBox.Show("你的帐号没有开通权限，请联系管理员");
                return;
            }

            string grpTitle = grptmp.group_name;  //拼凑组名称,小括号里为当前人数            

            List<User> users = new List<User>();
            //加调度台自身
            User selfUser = LocalSharedData.UserAllGroupCustomer.Find(delegate (User o) {

                return o.userId == LocalSharedData.CURRENTUser.user_id;

            });
            if (selfUser != null)
                users.Add(selfUser);

            //
            foreach (string idstr in userList)
            {
                
                if ( Convert.ToInt32(idstr) != LocalSharedData.CURRENTUser.user_id)
                {
                    User myuser = LocalSharedData.UserAllGroupCustomer.Find(delegate (User o) {

                        return o.userId == Convert.ToInt32(idstr);
                    });

                    if (myuser != null)
                        users.Add(myuser);
                }
            }

            if (tempTalkForm == null || tempTalkForm.IsDisposed)
                tempTalkForm = new TempTalkForm(client, grptmp.group_id, grpTitle, users);

            //tempTalkForm.labGroupName.Text = grpTitle;
            tempTalkForm.Owner = this;  //回调用

            TEMPTALK_POC_ENABLE = true;  //进入临时对讲模式           

            
            if (tempTalkForm.ShowDialog() == DialogResult.Cancel)
            {
                TEMPTALK_POC_ENABLE = false;  //退出临时对讲模式

                //退出会话组，并且底部curGroupCombobox要同步              

                if (this.cbCurrGroup.Items.Count > 0)
                {
                    enableGroupSwitch = true;
                    this.cbCurrGroup.SelectedValue = GroupService.EMPTY_GROUP_ID + "";
                }

                client.AEC_State = 0;  //设为初始态
            }           


            tempTalkForm = null;

        }

        private void btnMeet_Click(object sender, EventArgs e)
        {                      

            //暂时不能超过6个人(包括自己)
            //要把本身UserId加进去
            HashSet<String> userList = new HashSet<string>(); //要去重            
            userList = GroupService.SelectedUserNode(this.SELECT_TREEVIEW_MODE, this.treeViewSearch, this.treeViewContact);

            userList.Remove(LocalSharedData.CURRENTUser.user_id + ""); //先删除是为了判断有无选别人

            if (userList.Count == 0)
            {
                MessageBox.Show("请选中用户");
                return;
            }
            userList.Add(LocalSharedData.CURRENTUser.user_id + "");

            if (userList.Count > 6)
            {
                MessageBox.Show("不能超过6个人(包括自己)");
                return;
            }

            //发出视频会商邀请报文，除了自己
            userList.Remove(LocalSharedData.CURRENTUser.user_id + "");

            foreach(string str in userList)
            {
                if (client != null)
                {
                    client.SendMessage((new Data()).MeetChatCommandMessageEncode
                        ((short)MeetChat.MeetType.AV, (short)MeetChat.MeetCommand.Invite,
                        Convert.ToInt16(PocConstant.GetInstance().cmpId),
                        LocalSharedData.CURRENTUser.user_id,
                        Convert.ToInt32(str), LocalSharedData.CURRENTUser.user_name,
                        LocalSharedData.GetUserName(Convert.ToInt32(str)), "streamBig"));

                }
                Task.Delay(20).Wait();
            }

            AgoraMeetForm meetForm = new AgoraMeetForm(client, userList, PocConstant.GetInstance().cmpId, 2);
            meetForm.ShowDialog();

            //2023.10.13 由于trtc只一次性免费10000分钟, 而声网agora能每月免费10000分钟，改成调用agora
            /*
            MeetForm meetForm = new MeetForm(client, userList);

            meetForm.ShowDialog();   
            */

        }

        private void btnBroadCast_Click(object sender, EventArgs e)
        {
            //动态广播方式
            //要把本身UserId加进去
            HashSet<String> userList = new HashSet<string>(); //要去重            
            userList = GroupService.SelectedUserNode(this.SELECT_TREEVIEW_MODE, this.treeViewSearch, this.treeViewContact);

            userList.Remove(LocalSharedData.CURRENTUser.user_id + ""); //先删除是为了判断有无选别人

            if (userList.Count == 0)
            {
                MessageBox.Show("请选中用户");
                return;
            }
            userList.Add(LocalSharedData.CURRENTUser.user_id + "");

            string grpName_ = "语音广播";

            //创建临时组
            String memberstr = "";
            foreach (string userId in userList)
            {
                if (memberstr.Equals(""))
                    memberstr = userId;
                else
                    memberstr = memberstr + "," + userId;
            }

            string utcTimeStamp = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmssfff");


            GroupTempDto dto = new GroupTempDto();
            dto.groupName = grpName_ + utcTimeStamp.Substring(10, 7); //取 mmssfff 部分，共7位
            dto.userIds = memberstr;
            dto.ownerId = LocalSharedData.CURRENTUser.user_id;
            dto.priv = 1;    //调度员发起的为1, 终端发起的为0             
            dto.aclass = 1;  //表示广播
            Group grptmp = null;
            TempGroupResponse resp_t = PocClient.createTmpGroup(dto);
            if (resp_t != null)
            {
                grptmp = new Group();
                grptmp.group_id = resp_t.data.groupId;
                grptmp.group_name = resp_t.data.groupName;
                grptmp.owner_id = resp_t.data.ownerId.Value;
                grptmp.user_ids = resp_t.data.userIds;
            }

            //这里的临时组不用在树中保存（即要求server不要回发创建组消息给PC端），因为一旦关闭就会删除的
            if (grptmp == null)
            {
                MessageBox.Show("你的帐号没有开通权限，请联系管理员");
                return;
            }

            string grpTitle = grptmp.group_name;               

            List<User> users = new List<User>();
            //加调度台自身
            User selfUser = LocalSharedData.UserAllGroupCustomer.Find(delegate (User o) {

                return o.userId == LocalSharedData.CURRENTUser.user_id;

            });
            if (selfUser != null)
                users.Add(selfUser);

            //
            foreach (string idstr in userList)
            {

                if (Convert.ToInt32(idstr) != LocalSharedData.CURRENTUser.user_id)
                {
                    User myuser = LocalSharedData.UserAllGroupCustomer.Find(delegate (User o) {

                        return o.userId == Convert.ToInt32(idstr);
                    });

                    if (myuser != null)
                        users.Add(myuser);
                }
            }

            if (broadCastForm == null || broadCastForm.IsDisposed)
                broadCastForm = new BroadCastForm(this.client, grptmp.group_id, grpTitle, users, 2);

            //broadCastForm.labGroupName.Text = node.Text;

            broadCastForm.Owner = this;  //回调用

            BROADCAST_POC_ENABLE = true;  //进入广播对讲模式
            BROADCAST_POC_FILE_MODE_ENABLE = false; //默认是用mic对讲广播



            if (broadCastForm.ShowDialog() == DialogResult.Cancel)
            {
                BROADCAST_POC_ENABLE = false;  //退出广播对讲模式
                BROADCAST_POC_FILE_MODE_ENABLE = false; //默认是用mic对讲广播

                //退出会话组，并且底部curGroupCombobox要同步              

                if (this.cbCurrGroup.Items.Count > 0)
                {
                    enableGroupSwitch = true;
                    this.cbCurrGroup.SelectedValue = GroupService.EMPTY_GROUP_ID + "";
                }

                client.AEC_State = 0;  //设为初始态
            }

            //这里要区分是即时创建的广播组，还是用的临时形式广播组
            //1. 如果是即时创建的，则广播窗口退出时自动删除它 (因为删除时,Server端发每个终端，而终端自动处理清除和退出)
            //2. 如果是用的临时形式广播组,则广播窗口退出时,这里还要专门发一个 "强制退出广播"的报文


            broadCastForm = null;


        }
    }

}

