using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;


using System.Net;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Text.RegularExpressions;

using SimpleJson;

using POCControlCenter.DataEntity;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Forms;
using POCClientNetLibrary;
using System.Diagnostics;
using System.Globalization;
using System.Data;
using System.Web;

namespace POCControlCenter
{
    //文件类型
    public static class Filetype
    {
        // Application.StartupPath +"\\sendImage\\";
        public static String path = System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "pocfile";

        public static String TYPE_CHAT_AUDIO = "audio";
        public static String TYPE_CHAT_VIDEO = "video";
        public static String TYPE_CHAT_PICTRUE = "picture";//picture
        public static String TYPE_CHAT_FILE = "file";

        public static String TYPE_FILETYPE_UPDATE = "update";
        public static String TYPE_FILETYPE_DOWNLOAD = "download";

        public static String Pic_Path = path + Path.DirectorySeparatorChar + "pic" + Path.DirectorySeparatorChar;
        public static String File_Path = path + Path.DirectorySeparatorChar + "file" + Path.DirectorySeparatorChar;
        public static String Audio_Path = path + Path.DirectorySeparatorChar + "audio" + Path.DirectorySeparatorChar;
        public static String Video_Path = path + Path.DirectorySeparatorChar + "video" + Path.DirectorySeparatorChar;
        public static String Icon_Path = path + Path.DirectorySeparatorChar + "icon" + Path.DirectorySeparatorChar;
        public static String Other_Path = path + Path.DirectorySeparatorChar + "other" + Path.DirectorySeparatorChar;


        public static String GetFileRecvDir(String strFiletype)
        {
            String savedir = Icon_Path;

            if (String.Compare(strFiletype, TYPE_CHAT_FILE, true) == 0)
            {
                savedir = File_Path;
            }
            else if (String.Compare(strFiletype, Pic_Path, true) == 0)
            {
                savedir = Pic_Path;
            }
            else if (String.Compare(strFiletype, TYPE_CHAT_AUDIO, true) == 0)
            {
                savedir = Audio_Path;
            }
            else if (String.Compare(strFiletype, TYPE_CHAT_VIDEO, true) == 0)
            {
                savedir = Video_Path;
            }
            else
            {
                savedir = Other_Path;
            }

            System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(savedir);
            if (!dInfo.Exists)
                dInfo.Create();

            return savedir;
        }

    }

    //文件传输的状态
    public static class Filestate
    {
        public static String Idle = "idle";
        public static String Uploading = "uploading";
        public static String Upload_fail = "upload_fail";
        public static String Downloading = "downloading";
        public static String Download_fail = "download_fail";
        public static String Complete = "complete";
        public static String Error = "error";
    }

    ///<summary>
    ///上传事件委托
    ///</summary>
    ///<param name="sender"></param>
    ///<param name="e"></param>
    public delegate void WebClientUploadEvent(object sender, POCControlCenter.UploadEventArgs e);

    ///<summary>
    ///下载事件委托
    ///</summary>
    ///<param name="sender"></param>
    ///<param name="e"></param>
    public delegate void WebClientDownloadEvent(object sender, POCControlCenter.DownloadEventArgs e);

    ///<summary>
    ///上传事件参数
    ///</summary>
    public class UploadEventArgs
    {
        ///<summary>
        ///上传数据总大小
        ///</summary>
        public long totalBytes;
        ///<summary>
        ///已发数据大小
        ///</summary>
        public long bytesSent;

        ///<summary>
        ///发送进度(0-1)
        ///</summary>
        public double sendProgress;
        ///<summary>
        ///发送速度Bytes/s
        ///</summary>
        public double sendSpeed;

        public int groupID;
        public int userID;
        public int recvID;
        public string fileKey;

        public string state;

        public UploadEventArgs(long outtotalBytes, long outbytesSent, int outuserID, string outfileKey)
        {
            totalBytes = outtotalBytes;
            bytesSent = outbytesSent;
            userID = outuserID;
            fileKey = outfileKey;
        }
    }

    ///<summary>
    ///下载事件参数
    ///</summary>
    public class DownloadEventArgs
    {
        ///<summary>
        ///下载数据总大小
        ///</summary>
        public long totalBytes;
        ///<summary>
        ///已接收数据大小
        ///</summary>
        public long bytesReceived;
        ///<summary>
        ///接收数据进度(0-1)
        ///</summary>
        public double ReceiveProgress;
        ///<summary>
        ///接收速度Bytes/s
        ///</summary>
        public double receiveSpeed;

        public int groupID;
        public int userID;
        public int sendID;
        public string fileKey;

        public string state;

        public DownloadEventArgs(long outtotalBytes, long outbytesReceived, int outuserID, string outfileKey)
        {
            totalBytes = outtotalBytes;
            bytesReceived = outbytesReceived;
            userID = outuserID;
            fileKey = outfileKey;
        }
    }


    public class HttpAPI
    {
        //WEBIP,Web服务器：包含web管理接口        
        public static String WEBIP = "159.75.230.229";   //    

        public static String WEBPort = "8080";

        //MEDIAIP,媒体服务器,包含Socket通信功能
       
        public static String MEDIAIP = "159.75.230.229";     //    
        
        public static String MEDIAPort = "17001";

        //用于视频直播、监控服务器的接口调用(视频监控申请、停止等)
        
        public static String VIDEOIP = "159.75.230.229"; //     

        //视频服务器的端口
        public static String VIDEOPort = "8799";
        //视频录像所在XAMPP的端口
        public static String VIDEO_XAMPP_Port = "80";


        //2017.4.20 新增视频服务器: 如巡更打卡        
        public static String VEDIOIP_PREFIX = "159.75.230.229:80/weiding";    
        
        //http://211.159.177.23/mapviewloc_baidu_for_sos.php?lng=114.049271&lat=22.514032&place=553
        public static String SOS_MAP_LOACTION_URL = "http://"+ VIDEOIP+":"+ VIDEO_XAMPP_Port+ "/mapviewloc_baidu_for_sos.php";

        //*****************************************************
        //**********请注意以下的配置有变更时，一定要在同步更新下 Program.cs 中的UpdateALLPlatFormPara 方法

        public static String UriUserStr = "http://" + WEBIP + ":" + WEBPort + "/ptt/api/user/?";
        public static String UriGroupStr = "http://" + WEBIP + ":" + WEBPort + "/ptt/api/group/?";
        public static String UriLocationStr = "http://" + WEBIP + ":" + WEBPort + "/ptt/api/location/?";
        public static String UriFenceStr = "http://" + WEBIP + ":" + WEBPort + "/ptt/api/fence/?";
        //以下UriLocationMAP是共用设置, 地图、轨迹、围栏的地址都引用这个变量
        public static String UriLocationMAP = "http://" + WEBIP + ":" + WEBPort + "/ptt/back/";

        public static String UriLocationBaiduMAP = "http://" + WEBIP + ":" + WEBPort + "/ptt/back/baidumap.jsp";
        public static String UriLocationTrackMAP = "http://" + WEBIP + ":" + WEBPort + "/ptt/back/baidu_trackback.jsp";
        public static String UriLocationFenceMAP = "http://" + WEBIP + ":" + WEBPort + "/ptt/back/baidu_fence.jsp";

        //2017.5.23 加入 对视频服务器(rtmp)
        public static string UriVideoStop_MoniOrLive = "http://" + VIDEOIP + ":8081/control/drop/publisher";
        public static String UriVideoMonitor_Apply = "http://" + VIDEOIP + ":"+ VIDEOPort + "/vmonitor/v1/monitor_apply.json";
        //2017.9.20 加入 对视频服务器(rtsp)的启动请求
        public static String UriVideoMonitor_Apply_RTSP = "http://" + VIDEOIP + ":"+ VIDEOPort + "/vmonitor/v1/monitor_apply_rtsp.json";
        //2017.9.22 加入 对视频服务器(rtsp)的停止请求
        //public static String UriVideoMonitor_Stop_RTSP = "http://" + VIDEOIP + ":"+ VIDEOPort + "/vmonitor/v1/monitor_stop.json";
             

        /**上传*/
        public static String UpdateURL = "http://" + WEBIP + ":" + WEBPort + "/ptt/servlet/UploadHandleServlet";
        /**下载*/
        public static String DownloadURL = "http://" + WEBIP + ":" + WEBPort + "/ptt/servlet/DownloadHandleServlet";

        //2017.08.21 加入围栏的警告人员在地图上的显示
        public static String FenceAlarmMap_URL = "http://" + WEBIP + ":" + WEBPort + "/ptt/back/" + WinFormsStringResource.LocationFenceAlarmMap;

        //2017.10.8  加入离线地图的相对目录(map/baidumap)
        //离线地图的本地目录及文件
        public static String UriLocationMAPOffline_Dir = "map/baidumap";
        public static String UriLocationMAPOffline_MapFile = "baidumap.html";
        public static String UriLocationMAPOffline_FenceFile = "baidu_fence.html";
        public static String UriLocationMAPOffline_TrackFile = "baidu_trackback.html";
        //离线地图的服务访问地图(基站版本或公网版本的用户选择了离线百度地图)
        //UriLocationMAPOffline_MAPACTION_URL_PREFIX指定baidumap_action.jsp前缀
        public static String UriLocationMAPOffline_MAPACTION_URL_PREFIX = "http://" + WEBIP + ":" + WEBPort + "/ptt/back/";
        //2018.1.19 这个用来本地的地图人员定位查询
        public static String UriLocationMAPOffline_FenceACTION_API_PREFIX = "http://" + WEBIP + ":" + WEBPort + "/ptt/api/fence/?action=query_grp_gps";

        //UriLocationMAPOffline_FENCEACTION_URL_PREFIX指定../api/fence/的前缀(轨迹和围栏都引用)
        public static String UriLocationMAPOffline_FENCEACTION_URL_PREFIX = "http://" + WEBIP + ":" + WEBPort + "/ptt/";
        //*****************************************************
        //**********请注意以上的配置有变更时，一定要在同步更新下 Program.cs 中的UpdateALLPlatFormPara 方法
        //**********End****************************************
        //*****************************************************

        

        //2018.1.31 加入POST , contentType=application/x-www-form-urlencoded
        private static string GetHtmlContentFromWeb_POST(string sUrl, IDictionary<string, string> parameters)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(sUrl);

            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 20000;

            //如果需要POST数据   
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }

                byte[] btBodys = Encoding.UTF8.GetBytes(buffer.ToString());

                httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);
                
            }

            //byte[] btBodys = Encoding.UTF8.GetBytes(postform);
            //httpWebRequest.ContentLength = btBodys.Length;
            //httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();

            return responseContent;
        }
        
        

        public static string GetHtmlContentFromWeb(string sUrl)
        {
            string strBuff = "";
            string result = string.Empty;
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(sUrl);
                webRequest.Timeout = 20000;
                //webRequest.Method = "GET";
                //webRequest.Accept = "application/json";
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                Stream stream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("UTF-8"));
                strBuff = reader.ReadToEnd();

                reader.Close();
                stream.Close();
                //webRequest.Abort();
                webResponse.Close();

                return strBuff;
            }
            catch (Exception ex)
            {
                if (ex!=null)
                    Debug.WriteLine("GetHtmlContentFromWeb error:" + ex.Message + ex.StackTrace);

            }
            return result;

        }     
         

        public static DataEntity.ExecuteResult sendNotice(String senderid, String caption, String msgtext,string userlist )
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "sendNotice");

                //param.Add("senderid", senderid);
                //param.Add("caption", caption);
                //param.Add("msgtext", msgtext);
                //param.Add("userlist", userlist);

                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("senderid", senderid);
                parameters.Add("caption", caption);
                parameters.Add("msgtext", msgtext);
                parameters.Add("userlist", userlist);
                ExecuteResult nud = SimpleJson.SimpleJson.DeserializeObject<ExecuteResult>(GetHtmlContentFromWeb_POST(strURL, parameters ));
                if (nud.error == 0)
                    return nud;
                else
                    return null;
            }
            catch (Exception ex)
            {

            }

            return null;

        }      
        


        public static string ToUrlEncode(string strCode)
        {
            StringBuilder sb = new StringBuilder();

            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(strCode);
            System.Text.RegularExpressions.Regex regKey = new System.Text.RegularExpressions.Regex("^[A-Za-z0-9]+$");

            for (int i = 0; i < byStr.Length; i++)

            {

                string strBy = Convert.ToChar(byStr[i]).ToString();

                if (regKey.IsMatch(strBy))

                {

                    //是字母或者数字则不进行转换

                    sb.Append(strBy);

                }

                else

                {

                    sb.Append(@"%" + Convert.ToString(byStr[i], 16));

                }

            }

            return (sb.ToString());
        }                   
        
                
                  
               

       


        //2017.3.16 查询个人的实时坐标
        public static List<DataEntity.Location> queryLocationByUserid(int userID, int groupID)
        {
            DataTable dt = null;
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "queryLocationByUserid");
                param.Add("userid", userID.ToString());
                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpGroupData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupData>( httpResponseAsync(strURL).Result );
                NetHttpLocationExData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpLocationExData>(GetHtmlContentFromWeb(strURL));

                
                if (!nud.error.Equals("0"))
                    return null;

                List<Location> data = new List<Location>();
                dt = Utils.ToDataTable(new System.Collections.ArrayList(nud.data));
               
                foreach (DataRow dr in dt.Rows)
                {
                    Location loc = new Location();
                    if (!dr.IsNull("batterylevel"))
                        loc.batterylevel = Convert.ToInt32(dr["batterylevel"].ToString());

                    loc.curtime = Convert.ToInt64(dr["curtime"]);

                    if (!dr.IsNull("group_id"))
                        loc.group_id = Convert.ToInt32(dr["group_id"]);

                    if (!dr.IsNull("phone"))
                        loc.phone = dr.Field<string>("phone");

                    if (!dr.IsNull("user_id"))
                        loc.user_id = Convert.ToInt32(dr["user_id"]);

                    if (!dr.IsNull("user_name"))
                        loc.user_name = dr.Field<string>("user_name");

                    if (!dr.IsNull("group_name"))
                        loc.group_name = dr.Field<string>("group_name");

                    if (!dr.IsNull("loactionname"))
                        loc.loactionname = dr.Field<string>("loactionname");

                    if (!dr.IsNull("latitude"))
                        loc.latitude = Convert.ToDouble(dr["latitude"]);

                    if (!dr.IsNull("longitude"))
                        loc.longitude = Convert.ToDouble(dr["longitude"]);

                    if (!dr.IsNull("logon"))
                        loc.logon = Convert.ToInt32(dr["logon"]);

                    if (!dr.IsNull("flag_record"))
                        loc.flag_record = dr.Field<string>("flag_record");

                    if (!dr.IsNull("myclass"))
                        loc.myclass = Convert.ToInt32(dr["myclass"]);

                    if (!dr.IsNull("life_state"))
                        loc.life_state = Convert.ToInt32(dr["life_state"]);

                    data.Add(loc);

                }
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (dt != null)
                    dt.Dispose();
            }

            return null;
        }

        //2017.8.31 查询视频记录的指定分页的结果
        public static List<VideoRec> queryVideoRecByPageIndex(string page_index, string video_type, string poc_account, string poc_account_like, string cust_account, string cust_account_like, string user_name, string user_name_like, string record_date_from, string record_date_to, string file_size_from, string file_size_to, string video_duration_from, string video_duration_to, string page_size)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "getVideoResultByPageIndex");

                param.Add("page_index", page_index);
                param.Add("cmpid", LocalSharedData.CURRENTUser.cmpid.ToString());
                //string video_type,string poc_account,string poc_account_like,
                //string cust_account,string cust_account_like,string user_name,
                //string user_name_like,string record_date_from,string record_date_to,

                //string file_size_from,string file_size_to,string video_duration_from,
                //string video_duration_to,string page_size

                param.Add("video_type", video_type);
                param.Add("poc_account", poc_account);
                param.Add("poc_account_like", poc_account_like);

                //汉字要进行编码
                //param.Add("cust_account", cust_account);
                param.Add("cust_account", HttpUtility.UrlEncode(cust_account, Encoding.UTF8));

                param.Add("cust_account_like", cust_account_like);
                //汉字要进行编码
                //param.Add("user_name", user_name);
                param.Add("user_name", HttpUtility.UrlEncode(user_name, Encoding.UTF8));

                param.Add("user_name_like", user_name_like);




                param.Add("record_date_from", record_date_from);
                param.Add("record_date_to", record_date_to);

                if (file_size_from.Trim().Equals(""))
                    param.Add("file_size_from", "-1");
                else
                    param.Add("file_size_from", file_size_from);

                if (file_size_to.Trim().Equals(""))
                    param.Add("file_size_to", "-1");
                else
                    param.Add("file_size_to", file_size_to);

                if (video_duration_from.Trim().Equals(""))
                    param.Add("video_duration_from", "-1");
                else
                    param.Add("video_duration_from", video_duration_from);

                if (video_duration_to.Trim().Equals(""))
                    param.Add("video_duration_to", "-1");
                else
                    param.Add("video_duration_to", video_duration_to);

                param.Add("page_size", page_size);

                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpGroupData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupData>( httpResponseAsync(strURL).Result );
                NetHttpVideoRecData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpVideoRecData>(GetHtmlContentFromWeb(strURL));

                if (nud.error == "0")
                    return nud.data;

            }
            catch (Exception ex)
            {

            }

            return null;
        }

        //2017.8.31 查询视频记录的结果数量
        public static PageQueryTotal queryVideoRecTotalInfo(string video_type, string poc_account, string poc_account_like, string cust_account, string cust_account_like, string user_name, string user_name_like, string record_date_from, string record_date_to, string file_size_from, string file_size_to, string video_duration_from, string video_duration_to, string page_size)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "getVideoPageNum");
                param.Add("cmpid", LocalSharedData.CURRENTUser.cmpid.ToString());
                //string video_type,string poc_account,string poc_account_like,
                //string cust_account,string cust_account_like,string user_name,
                //string user_name_like,string record_date_from,string record_date_to,

                //string file_size_from,string file_size_to,string video_duration_from,
                //string video_duration_to,string page_size

                param.Add("video_type", video_type);
                param.Add("poc_account", poc_account);
                param.Add("poc_account_like", poc_account_like);
                //汉字要进行编码
                //param.Add("cust_account", cust_account);
                param.Add("cust_account", HttpUtility.UrlEncode(cust_account, Encoding.UTF8));

                param.Add("cust_account_like", cust_account_like);
                //汉字要进行编码
                //param.Add("user_name", user_name);
                param.Add("user_name", HttpUtility.UrlEncode(user_name, Encoding.UTF8));
                param.Add("user_name_like", user_name_like);
                param.Add("record_date_from", record_date_from);
                param.Add("record_date_to", record_date_to);

                if (file_size_from.Trim().Equals(""))
                    param.Add("file_size_from", "-1");
                else
                    param.Add("file_size_from", file_size_from);

                if (file_size_to.Trim().Equals(""))
                    param.Add("file_size_to", "-1");
                else
                    param.Add("file_size_to", file_size_to);

                if (video_duration_from.Trim().Equals(""))
                    param.Add("video_duration_from", "-1");
                else
                    param.Add("video_duration_from", video_duration_from);

                if (video_duration_to.Trim().Equals(""))
                    param.Add("video_duration_to", "-1");
                else
                    param.Add("video_duration_to", video_duration_to);

                param.Add("page_size", page_size);

                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpGroupData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupData>( httpResponseAsync(strURL).Result );
                NetHttpPageQueryTotalData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpPageQueryTotalData>(GetHtmlContentFromWeb(strURL));

                if (nud.error == "0")
                    return nud.data;

            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public static List<AudioRec> queryAudioRecByPageIndex(string page_index,
            string poc_account, string poc_account_like,
            string cust_account, string cust_account_like, string user_name, string user_name_like,
            string record_date_from, string record_date_to, string audio_duration_from,
            string audio_duration_to, string page_size, int group_id)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "getAudioResultByPageIndex");

                param.Add("page_index", page_index);
                param.Add("cmpid", LocalSharedData.CURRENTUser.cmpid.ToString());

                param.Add("poc_account", poc_account);
                param.Add("poc_account_like", poc_account_like);

                //汉字要进行编码
                //param.Add("cust_account", cust_account);
                param.Add("cust_account", HttpUtility.UrlEncode(cust_account, Encoding.UTF8));

                param.Add("cust_account_like", cust_account_like);
                //汉字要进行编码
                //param.Add("user_name", user_name);
                param.Add("user_name", HttpUtility.UrlEncode(user_name, Encoding.UTF8));


                param.Add("user_name_like", user_name_like);
                param.Add("record_date_from", record_date_from);
                param.Add("record_date_to", record_date_to);

                if (audio_duration_from.Trim().Equals(""))
                    param.Add("audio_duration_from", "-1");
                else
                    param.Add("audio_duration_from", audio_duration_from);

                if (audio_duration_to.Trim().Equals(""))
                    param.Add("audio_duration_to", "-1");
                else
                    param.Add("audio_duration_to", audio_duration_to);

                param.Add("page_size", page_size);
                param.Add("group_id", group_id.ToString());

                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpGroupData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupData>( httpResponseAsync(strURL).Result );
                NetHttpAudioRecData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpAudioRecData>(GetHtmlContentFromWeb(strURL));

                if (nud.error == "0")
                    return nud.data;

            }
            catch (Exception ex)
            {

            }

            return null;

        }

        //2017.9.3 查询语音记录
        public static PageQueryTotal queryAudioRecTotalInfo(string poc_account, string poc_account_like,
            string cust_account, string cust_account_like, string user_name, string user_name_like,
            string record_date_from, string record_date_to, string audio_duration_from,
            string audio_duration_to, string page_size, int group_id)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "getAudioPageNum");
                param.Add("cmpid", LocalSharedData.CURRENTUser.cmpid.ToString());

                param.Add("poc_account", poc_account);
                param.Add("poc_account_like", poc_account_like);


                param.Add("user_name_like", user_name_like);

                //汉字要进行编码
                //param.Add("cust_account", cust_account);
                param.Add("cust_account", HttpUtility.UrlEncode(cust_account, Encoding.UTF8));

                param.Add("cust_account_like", cust_account_like);
                //汉字要进行编码
                //param.Add("user_name", user_name);
                param.Add("user_name", HttpUtility.UrlEncode(user_name, Encoding.UTF8));

                param.Add("record_date_from", record_date_from);
                param.Add("record_date_to", record_date_to);

                if (audio_duration_from.Trim().Equals(""))
                    param.Add("audio_duration_from", "-1");
                else
                    param.Add("audio_duration_from", audio_duration_from);

                if (audio_duration_to.Trim().Equals(""))
                    param.Add("audio_duration_to", "-1");
                else
                    param.Add("audio_duration_to", audio_duration_to);

                param.Add("page_size", page_size);
                param.Add("group_id", group_id.ToString());

                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpGroupData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupData>( httpResponseAsync(strURL).Result );
                NetHttpPageQueryTotalData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpPageQueryTotalData>(GetHtmlContentFromWeb(strURL));

                if (nud.error == "0")
                    return nud.data;

            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public static List<POCSessionRec> queryPOCSessionRecByPageIndex(string page_index, string file_type,
            string poc_account, string poc_account_like,
            string cust_account, string cust_account_like, string user_name, string user_name_like,
            string record_date_from, string record_date_to,
            string page_size, int group_id)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "getPOCSessionResultByPageIndex");

                param.Add("page_index", page_index);
                param.Add("cmpid", LocalSharedData.CURRENTUser.cmpid.ToString());

                param.Add("file_type", file_type);
                param.Add("poc_account", poc_account);
                param.Add("poc_account_like", poc_account_like);
                //汉字要进行编码
                //param.Add("cust_account", cust_account);
                param.Add("cust_account", HttpUtility.UrlEncode(cust_account, Encoding.UTF8));

                param.Add("cust_account_like", cust_account_like);
                //汉字要进行编码
                //param.Add("user_name", user_name);
                param.Add("user_name", HttpUtility.UrlEncode(user_name, Encoding.UTF8));

                param.Add("user_name_like", user_name_like);
                param.Add("record_date_from", record_date_from);
                param.Add("record_date_to", record_date_to);

                param.Add("page_size", page_size);
                param.Add("group_id", group_id.ToString());

                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpGroupData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupData>( httpResponseAsync(strURL).Result );
                NetHttpPOCSessionRecData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpPOCSessionRecData>(GetHtmlContentFromWeb(strURL));

                if (nud.error == "0")
                    return nud.data;

            }
            catch (Exception ex)
            {

            }

            return null;

        }

        //2017.9.3 查询群组内会话记录
        public static PageQueryTotal queryPOCSessionRecTotalInfo(string file_type, string poc_account, string poc_account_like,
            string cust_account, string cust_account_like, string user_name, string user_name_like,
            string record_date_from, string record_date_to,
            string page_size, int group_id)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "getPOCSessionPageNum");
                param.Add("cmpid", LocalSharedData.CURRENTUser.cmpid.ToString());

                param.Add("file_type", file_type);
                param.Add("poc_account", poc_account);
                param.Add("poc_account_like", poc_account_like);
                //汉字要进行编码
                //param.Add("cust_account", cust_account);
                param.Add("cust_account", HttpUtility.UrlEncode(cust_account, Encoding.UTF8));

                param.Add("cust_account_like", cust_account_like);
                //汉字要进行编码
                //param.Add("user_name", user_name);
                param.Add("user_name", HttpUtility.UrlEncode(user_name, Encoding.UTF8));

                param.Add("user_name_like", user_name_like);
                param.Add("record_date_from", record_date_from);
                param.Add("record_date_to", record_date_to);

                param.Add("page_size", page_size);
                param.Add("group_id", group_id.ToString());

                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpGroupData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupData>( httpResponseAsync(strURL).Result );
                NetHttpPageQueryTotalData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpPageQueryTotalData>(GetHtmlContentFromWeb(strURL));

                if (nud.error == "0")
                    return nud.data;

            }
            catch (Exception ex)
            {

            }

            return null;

        }

        //2017.3.16 查询组内的实时坐标
        public static List<DataEntity.Location> queryLocationByGroup(int userID, int groupID, string userid_clause)
        {
            //2017.5.4 用直接查询db方式
            DataTable dt = null;
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "queryLocationByGroup");
                param.Add("groupid", groupID.ToString());

                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpGroupData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupData>( httpResponseAsync(strURL).Result );
                NetHttpLocationExData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpLocationExData>(GetHtmlContentFromWeb(strURL));

                if (!nud.error.Equals("0"))
                    return null;

                //DataSet ds = DbHelperMySQL.Query(sb.ToString());
                //DataTable dt = ds.Tables[0];
                dt = Utils.ToDataTable(new System.Collections.ArrayList(nud.data));
                List<Location> data = new List<Location>();

                foreach (DataRow dr in dt.Rows)
                {
                    Location loc = new Location();
                    if (!dr.IsNull("batterylevel"))
                        loc.batterylevel = Convert.ToInt32(dr["batterylevel"].ToString());

                    loc.curtime = Convert.ToInt64(dr["curtime"]);

                    if (!dr.IsNull("group_id"))
                        loc.group_id = Convert.ToInt32(dr["group_id"]);

                    if (!dr.IsNull("phone"))
                        loc.phone = dr.Field<string>("phone");

                    loc.user_id = Convert.ToInt32(dr["user_id"]);

                    if (!dr.IsNull("user_name"))
                        loc.user_name = dr.Field<string>("user_name");

                    if (!dr.IsNull("group_name"))
                        loc.group_name = dr.Field<string>("group_name");

                    if (!dr.IsNull("loactionname"))
                        loc.loactionname = dr.Field<string>("loactionname");

                    if (!dr.IsNull("latitude"))
                        loc.latitude = Convert.ToDouble(dr["latitude"]);

                    if (!dr.IsNull("longitude"))
                        loc.longitude = Convert.ToDouble(dr["longitude"]);

                    if (!dr.IsNull("logon"))
                        loc.logon = Convert.ToInt32(dr["logon"]);

                    if (!dr.IsNull("flag_record"))
                        loc.flag_record = dr.Field<string>("flag_record");

                    if (!dr.IsNull("myclass"))
                        loc.myclass = Convert.ToInt32(dr["myclass"]);

                    if (!dr.IsNull("life_state"))
                        loc.life_state = Convert.ToInt32(dr["life_state"]);

                    data.Add(loc);

                }
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (dt != null)
                    dt.Dispose();
            }

            return null;
        }

        public static List<DataEntity.Group> userAllGroup(int userID)
        {

            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "queryBelongGroupByUserId");
                param.Add("user_id", userID.ToString());

                var simpleUri = new Uri(UriGroupStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;
                
                NetHttpGroupData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupData>(GetHtmlContentFromWeb(strURL));

                if (nud.error == "0")
                    return nud.data;
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public static List<DataEntity.Group> userAllTempGroup(int userID)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "queryBelongTempGroupByUserId");
                param.Add("user_id", userID.ToString());

                var simpleUri = new Uri(UriGroupStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;
                
                NetHttpGroupData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupData>(GetHtmlContentFromWeb(strURL));

                if (nud.error == "0")
                    return nud.data;
            }
            catch (Exception ex)
            {
            }

            return null;
        }       
        

        public static DataEntity.ExecuteResult updateRTState(int userid, int rtstate_field, int rtstate_after_value)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "updateUserRTState");

                param.Add("userid", userid.ToString());
                param.Add("rtstate_field", rtstate_field.ToString());
                param.Add("rtstate_after_value", rtstate_after_value.ToString());

                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                ExecuteResult nud = SimpleJson.SimpleJson.DeserializeObject<ExecuteResult>(GetHtmlContentFromWeb(strURL));

                return nud;

            }
            catch (Exception ex)
            {
            }
            return null;
        }

        //通过
        public static DataEntity.SipSubscriber querySipSubscriber(int userid)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "querySipById");
                //getTmpGroupCustomer
                //param.Add("action", "getTmpGroupCustomer");
                param.Add("user_id", userid.ToString());
                var simpleUri = new Uri(UriUserStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;
                // NetHttpGroupUserData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupUserData>( httpResponseAsync(strURL).Result );
                NetHttpSipSubscriberData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpSipSubscriberData>(GetHtmlContentFromWeb(strURL));
                if (nud.error == "0")
                    return nud.data;
            }
            catch (Exception ex)
            {
            }
            return null;
        }


        public static string queryBaiduLocdesc(double lati, double lng)
        {
            try
            {
                StringBuilder sb = new StringBuilder(@"http://api.map.baidu.com/geocoder/v2/?location=");
                sb.Append(lati.ToString());
                sb.Append("," + lng.ToString());
                sb.Append(@"&output=json&pois=1&ak=gopum3dSZ1k0WdsxUGEdWF5VGDhgX0eP");
                //api.map.baidu.com/geocoder/v2/?location=22.571474,113.889946&output=json&pois=1&ak=TytiMilaKbValnCKfSTQR5jTt9PM7WlE

                var simpleUri = new Uri(sb.ToString());
                String strURL = simpleUri.AbsoluteUri;
                // NetHttpGroupUserData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpGroupUserData>( httpResponseAsync(strURL).Result );
                BaiduGeoLat nud = SimpleJson.SimpleJson.DeserializeObject<BaiduGeoLat>(GetHtmlContentFromWeb(strURL));
                if (nud.status == 0)
                {
                    if (!nud.result.sematic_description.Trim().Equals(""))
                        return nud.result.sematic_description.Trim();
                    else
                        nud.result.formatted_address.Trim();
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }               

       

        /**
         * 创建个人会话组
         * @param uid  自己
         * @param iniviteid  要邀请的人的id
         */
        public static int createPersonGroup(int uid, int iniviteid, int worktype)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "createPersonGroup");
                param.Add("user_id", uid.ToString());
                param.Add("iniviteid", iniviteid.ToString());
                param.Add("worktype", worktype.ToString());

                var simpleUri = new Uri(UriGroupStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpCallData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpCallData>( httpResponseAsync(strURL).Result );
                NetHttpCallData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpCallData>(GetHtmlContentFromWeb(strURL));
                if (nud.error == "0")
                    return nud.data;
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
        }

        /**
         * 加入个人会话
         * @param uid  自己id
         * @param iniviteid  邀请人id（谁邀请我）
         * @param group_id  被邀请的会话
         * @param callBack
         */
        public static int JoinPersonGroup(int uid, int inviteid, int group_id)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "createJoinPersonGroup");//createJoinPersonGroup
                param.Add("user_id", uid.ToString());
                param.Add("invite_id", inviteid.ToString());
                param.Add("group_id", group_id.ToString());
                param.Add("type", POCClientNetLibrary.MySysMsgType.SYS_MSSAGE_ENTER_PRESON.ToString());

                var simpleUri = new Uri(UriGroupStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpCallData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpCallData>( httpResponseAsync(strURL).Result );
                NetHttpCallData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpCallData>(GetHtmlContentFromWeb(strURL));
                if (nud.error == "0")
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

            return -1;
        }

        /**
         * 释放或退出单聊会话
         * @param uid  自己id
         * @param iniviteid  邀请人id（谁邀请我）
         * @param group_id  被邀请的会话
         * @param callBack
         */
        public static int ReleaseJoinPersonGroup(int uid, int inviteid, int group_id, int type)
        {
            // type === POCClientNetLibrary.MySysMsgType.SYS_MSSAGE_EXIT_PRESON
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "releaseJoinPersonGroup");
                param.Add("user_id", uid.ToString());
                param.Add("invite_id", inviteid.ToString());
                param.Add("group_id", group_id.ToString());
                param.Add("worktype", type.ToString());

                var simpleUri = new Uri(UriGroupStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                // NetHttpCallData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpCallData>( httpResponseAsync(strURL).Result );
                NetHttpCallData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpCallData>(GetHtmlContentFromWeb(strURL));
                if (nud.error == "0")
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

            return -1;
        }
        


        /**
         * QueryFenceAlarmInfo
         * @param uid  自己id
         */
        public static List<DataEntity.FenceAlarmNotify> QueryFenceAlarmInfo(int uid)
        {
            /*
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "query_fencealarm_num");
                param.Add("user_id", uid.ToString());

                var simpleUri = new Uri(UriFenceStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;                
               
                NetHttpFenceAlarmNotifyData nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpFenceAlarmNotifyData>(GetHtmlContentFromWeb(strURL));
                if (nud.error == "0")
                    return nud.data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("QueryFenceAlarmInfo Exception:" + ex.Message);
                return null;
            }
            */

            //2017.11.27 采用http异步方法
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("action", "query_fencealarm_num");
                param.Add("user_id", uid.ToString());

                var simpleUri = new Uri(UriFenceStr);
                var newUri = simpleUri.ExtendQuery(param);
                String strURL = newUri.AbsoluteUri;

                RequestInfo info = new RequestInfo(strURL);
                NetHttpFenceAlarmNotifyData nud = null;
                Action<ResponseInfo> act = new Action<ResponseInfo>(x => {
                    //回调对结果的处理
                    nud = SimpleJson.SimpleJson.DeserializeObject<DataEntity.NetHttpFenceAlarmNotifyData>(
                        x.GetString(Encoding.UTF8));
                    if (nud.error == "0")
                    {
                        Debug.WriteLine("Action<ResponseInfo>: QueryFenceAlarmInfo 得到结果!" + nud.data.Count);
                    }


                });
                HttpRequestFactory.AddRequestTask(info, act);

            } catch (Exception ex)
            {
                Debug.WriteLine("QueryFenceAlarmInfo Exception:" + ex.Message);
                return null;
            }

            return null;

        }

        /* ******* 
        /// 将本地文件上传到指定的服务器(HttpWebRequest方法)    
        *******/
        private ChatMessage chatmsg;
        private event WebClientUploadEvent delegateWebClientUploadEvent;
        private event WebClientDownloadEvent delegateWebClientDownloadEvent;

        public HttpAPI()
        {
            chatmsg = new ChatMessage();
            delegateWebClientUploadEvent = null;
            delegateWebClientDownloadEvent = null;
        }
        public HttpAPI(ChatMessage msg, WebClientUploadEvent upload, WebClientDownloadEvent download)
        {
            chatmsg = msg;
            delegateWebClientUploadEvent = upload;
            delegateWebClientDownloadEvent = download;
        }

        /// <summary>
        /// Http 上传文件
        /// </summary>
        public int UploadFileEx(string uploadfile, string url, string fileKey, NameValueCollection querystring, CookieContainer cookies)
        {
            UploadEventArgs args = null;

            try
            {
                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(new Uri(url));

                webrequest.CookieContainer = cookies;
                webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
                webrequest.Method = "POST";
                webrequest.KeepAlive = true;
                webrequest.Accept = "text/plain; charset=UTF-8";
                //对发送的数据不使用缓存   
                webrequest.AllowWriteStreamBuffering = false;
                //设置获得响应的超时时间（20秒）   
                webrequest.Timeout = 30 * 1000;

                // Build up the post param  
                StringBuilder sbh = new StringBuilder();
                foreach (string key in querystring.Keys)
                {
                    sbh.Append("--");
                    sbh.Append(boundary);
                    sbh.Append("\r\n");

                    sbh.Append("Content-Disposition: form-data; name=\"");
                    sbh.Append(key);
                    sbh.Append("\"\r\n");
                    sbh.Append("\r\n");
                    sbh.Append(querystring.Get(key));
                    sbh.Append("\r\n");
                }

                string postParam = sbh.ToString();
                byte[] postParamBytes = Encoding.UTF8.GetBytes(postParam);

                // Build up the post message header  
                StringBuilder sb = new StringBuilder();

                sb.Append("--");
                sb.Append(boundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition: form-data; name=\"");
                sb.Append(fileKey);
                sb.Append("\"; filename=\"");
                sb.Append(Path.GetFileName(uploadfile));
                sb.Append("\"");
                sb.Append("\r\n");
                sb.Append("Content-Type: ");
                sb.Append("application/octet-stream");// "application/vnd.ms-excel"
                sb.Append("\r\n");
                sb.Append("\r\n");

                string postHeader = sb.ToString();
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

                FileStream fileStream = new FileStream(uploadfile, FileMode.Open, FileAccess.Read);
                long length = postParamBytes.Length + postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;
                webrequest.ContentLength = length;

                Stream requestStream = webrequest.GetRequestStream();

                //
                // 1. step
                //
                // Write out our post paramdata  
                requestStream.Write(postParamBytes, 0, postParamBytes.Length);
                // Write out our post header  
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                //
                // 2. step
                //
                int buffersize = 4096;
                // Write out the file contents  
                byte[] buffer = new Byte[checked((uint)Math.Min(buffersize, (int)fileStream.Length))];
                int bytesRead = 0;

                args = new UploadEventArgs(chatmsg.getFilesize(), 0, Int32.Parse(chatmsg.getUserid()), chatmsg.getFileMD5());

                args.state = Filestate.Idle;
                if (delegateWebClientUploadEvent != null)
                    delegateWebClientUploadEvent(this, args);
                args.state = Filestate.Uploading;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);

                    args.bytesSent += bytesRead;
                    if (delegateWebClientUploadEvent != null)
                        delegateWebClientUploadEvent(this, args);
                }

                //
                // 3. step end
                //
                // Write out the trailing boundary  
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

                WebResponse responce = webrequest.GetResponse();
                Stream s = responce.GetResponseStream();
                StreamReader sr = new StreamReader(s);

                //读取服务器端返回的消息   
                String sReturnString = sr.ReadLine();
                s.Close();
                sr.Close();

                NetHttpFileResult nud = SimpleJson.SimpleJson.DeserializeObject<NetHttpFileResult>(sReturnString);
                if (nud.error == "0")
                {
                    args.state = Filestate.Complete;
                    args.bytesSent = fileStream.Length;
                    if (delegateWebClientUploadEvent != null)
                        delegateWebClientUploadEvent(this, args);
                    return 0;
                }

                args.state = Filestate.Upload_fail;
                if (delegateWebClientUploadEvent != null)
                    delegateWebClientUploadEvent(this, args);

                return -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件上传失败错误为" + ex.Message.ToString(), "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                args.state = Filestate.Error;
                if (delegateWebClientUploadEvent != null)
                    delegateWebClientUploadEvent(this, args);
                return -1;
            }
        }

        /// <summary>
        /// 不用考虑断点和回调的版本
        /// </summary>
        /// <param name="savefile"></param>
        /// <param name="url"></param>
        /// <param name="fileKey"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static int DownLoadFileEx_Jimmy(String savefile, String url, string fileKey, CookieContainer cookies)
        {
            DownloadEventArgs args = null;
            FileStream outputStream = null;
            HttpWebResponse response = null;
            Stream httpStream = null;
            try
            {
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
                webrequest.CookieContainer = cookies;
                webrequest.Method = "GET";
                webrequest.KeepAlive = true;
                webrequest.Accept = "text/plain; charset=UTF-8";

                response = (HttpWebResponse)webrequest.GetResponse();
                httpStream = response.GetResponseStream();

                long cl = response.ContentLength;

                int bufferSize = 4 * 1024;
                int readCount = 0;

                outputStream = new FileStream(savefile, FileMode.Create);
                //byte[] buffer = new Byte[checked((uint)Math.Min(bufferSize, (int)response.ContentLength))];
                byte[] buffer = new Byte[bufferSize];

                while ((readCount = httpStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    outputStream.Write(buffer, 0, readCount);

                }

                outputStream.Close();
                outputStream = null;

                httpStream.Close();
                httpStream = null;

                response.Close();
                response = null;

                return 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show("文件下载失败错误为" + ex.Message.ToString(), "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


                return -1;
            }
            finally
            {
                if (outputStream != null) outputStream.Close();
                if (httpStream != null) httpStream.Close();
                if (response != null) response.Close();
            }
        }

        /// <summary>
        /// Http 下载文件
        /// </summary>
        /// <param name="url">下载的路径</param>
        /// <param name="savefile">保存的文件名</param>
        /// <param name="uploadProgressChanged"></param>
        /// <param name="uploadFileCompleted"></param>
        /// 
        public int DownLoadFileEx(String savefile, String url, string fileKey, CookieContainer cookies)
        {
            DownloadEventArgs args = null;
            FileStream outputStream = null;
            HttpWebResponse response = null;
            Stream httpStream = null;
            try
            {
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
                webrequest.CookieContainer = cookies;
                webrequest.Method = "GET";
                webrequest.KeepAlive = true;
                webrequest.Accept = "text/plain; charset=UTF-8";

                response = (HttpWebResponse)webrequest.GetResponse();
                httpStream = response.GetResponseStream();

                long cl = response.ContentLength;

                int bufferSize = 4 * 1024;
                int readCount = 0;

                args = new DownloadEventArgs(chatmsg.getFilesize(), 0, Int32.Parse(chatmsg.getUserid()), chatmsg.getFileMD5());
                args.state = Filestate.Idle;
                if (delegateWebClientDownloadEvent != null)
                    delegateWebClientDownloadEvent(this, args);
                args.state = Filestate.Downloading;

                outputStream = new FileStream(savefile, FileMode.Create);
                //byte[] buffer = new Byte[checked((uint)Math.Min(bufferSize, (int)response.ContentLength))];
                byte[] buffer = new Byte[bufferSize];

                while ((readCount = httpStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    outputStream.Write(buffer, 0, readCount);

                    args.bytesReceived += readCount;
                    if (delegateWebClientDownloadEvent != null)
                        delegateWebClientDownloadEvent(this, args);
                }

                outputStream.Close();
                outputStream = null;

                httpStream.Close();
                httpStream = null;

                response.Close();
                response = null;

                args.state = Filestate.Complete;
                if (delegateWebClientDownloadEvent != null)
                    delegateWebClientDownloadEvent(this, args);

                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件下载失败错误为" + ex.Message.ToString(), "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                args.state = Filestate.Error;
                if (delegateWebClientDownloadEvent != null)
                    delegateWebClientDownloadEvent(this, args);
                return -1;
            }
            finally
            {
                if (outputStream != null) outputStream.Close();
                if (httpStream != null) httpStream.Close();
                if (response != null) response.Close();
            }
        }

        /**
         * 图片上传失败时，再次上传
         * @param chatmsg
         */
        public void UploadFile(ChatMessage chatmsg)
        {
            Debug.WriteLine(chatmsg.toString());

            String filepath = chatmsg.getFilepath();
            String filename = chatmsg.getFilename();

            NameValueCollection param = new NameValueCollection();
            param.Add("userid", chatmsg.getUserid());
            param.Add("groupid", chatmsg.getGroupid());
            param.Add("filekey", chatmsg.getFileMD5());
            param.Add("fileMD5", chatmsg.getFileMD5());
            param.Add("filename", chatmsg.getFilename());
            param.Add("filesize", chatmsg.getFilesize() + "");
            param.Add("filetype", chatmsg.getFiletype());
            param.Add("filemsgcontent", chatmsg.getMsgcontent());
            //Logger.i(TAG, "chatmsg :" + chatmsg);

            CookieContainer cookies = new CookieContainer();
            // //everything except upload file and url can be left blank if needed  
            int outdata = UploadFileEx(filepath, HttpAPI.UpdateURL, chatmsg.getFileMD5(), param, cookies);
        }

        /**
         * 图片下载失败时，再次下载
         * @param chatmsg
         */
        public void DownloadFile(ChatMessage chatmsg)
        {
            Debug.WriteLine("DownloadFile: " + chatmsg.toString());

            String savedir = Filetype.GetFileRecvDir(chatmsg.getFiletype());
            Debug.WriteLine("DownloadFile SYSTEM InitSavePath: " + savedir);

            String filepath;
            if (Directory.Exists(chatmsg.getFilepath()))
                filepath = Path.Combine(chatmsg.getFilepath(), chatmsg.getFilename());
            else
                filepath = Path.Combine(savedir, chatmsg.getFilename());

            Debug.WriteLine("DownloadFile savepath: " + filepath);

            String filekey = chatmsg.getFilekey();
            String downurl = HttpAPI.DownloadURL + "?userid=" + LocalSharedData.CURRENTUser.user_id.ToString() + "&filename=" + filekey;

            //File savefile = new File(savedir);
            chatmsg.setFilepath(filepath);

            CookieContainer cookies = new CookieContainer();
            int outdata = DownLoadFileEx(filepath, downurl, filekey, cookies);
        }
        /* ******* 
        /// 将本地文件上传到指定的服务器(HttpWebRequest方法)    
        *******/
    }
}
