using Newtonsoft.Json;
using POCControlCenter.DataEntity;
using POCControlCenter.Service.Entity;
using POCControlCenter.Service.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service
{
    public class PocClient : IDisposable
    {
        private RestClient client;
        private RestRequest request;
        public int timeout = 60 * 1000; //默认设置超时60s


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

        //新的REST API的端口
        public static String NEWPort = "17003";

        //视频录像所在XAMPP的端口
        public static String VIDEO_XAMPP_Port = "80";


        //2017.4.20 新增视频服务器: 如巡更打卡        
        public static String VEDIOIP_PREFIX = "159.75.230.229:80/weiding";

        //http://211.159.177.23/mapviewloc_baidu_for_sos.php?lng=114.049271&lat=22.514032&place=553
        public static String SOS_MAP_LOACTION_URL = "http://" + VIDEOIP + ":" + VIDEO_XAMPP_Port + "/mapviewloc_baidu_for_sos.php";

        //*****************************************************
        //**********请注意以下的配置有变更时，一定要在同步更新下 Program.cs 中的UpdateALLPlatFormPara 方法
        public static String BaseUrl = "http://" + WEBIP + ":" + WEBPort;
        public static String BaseUrl_Video = "http://" + VIDEOIP + ":" + VIDEOPort;
        public static String BaseUrl_New = "http://" + VIDEOIP + ":" + NEWPort+ "/ptt";

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
        public static String UriVideoMonitor_Apply = "http://" + VIDEOIP + ":" + VIDEOPort + "/vmonitor/v1/monitor_apply.json";
        //2017.9.20 加入 对视频服务器(rtsp)的启动请求
        public static String UriVideoMonitor_Apply_RTSP = "http://" + VIDEOIP + ":" + VIDEOPort + "/vmonitor/v1/monitor_apply_rtsp.json";
        //2017.9.22 加入 对视频服务器(rtsp)的停止请求
        //public static String UriVideoMonitor_Stop_RTSP = "http://" + VIDEOIP + ":" + VIDEOPort + "/vmonitor/v1/monitor_stop.json";


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


        public static PttUserInfoResponse userInfo()
        {
            List<Parameter> pars = new List<Parameter>();
            PttUserInfoResponse data = RestHelper.GetInstance().Get<PttUserInfoResponse>("app/user-info", pars,RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        //查询指定用户
        public static PttUserOneResponse queryPttUserByUserId(int userID)
        {
            List<Parameter> pars = new List<Parameter>();

            PttUserOneResponse data = RestHelper.GetInstance().Get<PttUserOneResponse>("app/query-pttuser/"+userID, pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        //获取agora的appid,token等
        public static AgoraRtcTokenResponse getAgoraRtcToken(string channelName)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("channelName", channelName, ParameterType.QueryString));
            AgoraRtcTokenResponse data = RestHelper.GetInstance().Get<AgoraRtcTokenResponse>("app/agora-rtc-token", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        /// <summary>
        /// 获取region china
        /// </summary>
        /// <returns></returns>
        public static RegionChinaResponse getRegionChina(int parentCode)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("parentCode", parentCode, ParameterType.QueryString));
            RegionChinaResponse data = RestHelper.GetInstance().Get<RegionChinaResponse>("app/find-region-china", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        public static RegionChinaResponse getRegionChinaCommon(int code)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("code", code, ParameterType.QueryString));
            RegionChinaResponse data = RestHelper.GetInstance().Get<RegionChinaResponse>("app/find-regionchina-common", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }


        /// <summary>
        /// 获取用户所有组信息
        /// </summary>
        /// <returns></returns>
        public static GroupCustomerVoResponse getGroupCustomerVo()
        {
            List<Parameter> pars = new List<Parameter>();
            GroupCustomerVoResponse data = RestHelper.GetInstance().Get<GroupCustomerVoResponse>("app/get-group-customerVo", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        /// <summary>
        /// 获取固定组的成员信息
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static GroupUserMemberResponse queryGroupMemberByGroupId(int groupId)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("groupId", groupId, ParameterType.QueryString));
            GroupUserMemberResponse data = RestHelper.GetInstance().Get<GroupUserMemberResponse>("app/query-group-member-by-groupId", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }


        /// <summary>
        /// 查询临时组中的成员
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static GroupUserMemberResponse queryTmpGroupMemberByGroupId(int groupId)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("groupId", groupId, ParameterType.QueryString));
            GroupUserMemberResponse data = RestHelper.GetInstance().Get<GroupUserMemberResponse>("app/get-tmp-group-customer", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        
        /// <summary>
        /// 获取围栏告警信息
        /// </summary>
        /// <param name="notifyId"></param>
        /// <returns></returns>
        public static FenceAlarmNotifyInfoResponse queryAlarmNotifyInfo(int notifyId)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("notifyId", notifyId, ParameterType.QueryString));
            FenceAlarmNotifyInfoResponse data = RestHelper.GetInstance().Get<FenceAlarmNotifyInfoResponse>("app/fence-alarm-notify-info", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        //根据条件查询多个告警信息
        public static MultiFenceAlarmNotifyInfoResponse queryMultiAlarmNotifyInfo(int cmpid, string alarmType,
            string startNotifyTime,string endNotifyTime)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("cmpid", cmpid, ParameterType.QueryString));
            if (alarmType!=null)
                pars.Add(new Parameter("alarmType", alarmType, ParameterType.QueryString));

            if (startNotifyTime != null)
                pars.Add(new Parameter("startNotifyTime", startNotifyTime, ParameterType.QueryString));

            if (endNotifyTime != null)
                pars.Add(new Parameter("endNotifyTime", endNotifyTime, ParameterType.QueryString));

            MultiFenceAlarmNotifyInfoResponse data = RestHelper.GetInstance().Get<MultiFenceAlarmNotifyInfoResponse>("app/query-fence-notify", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        //查询指定cmpid的所有地理围栏
        public static AllFenceInfoResponse queryAllFenceInfo(int cmpid)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("cmpid", cmpid, ParameterType.QueryString));
            AllFenceInfoResponse data = RestHelper.GetInstance().Get<AllFenceInfoResponse>("app/all-fence", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        //查询soslog
        public static PageResponseBase<SoslogDto> querySosLog(int cmpid,int page,string startDate,string endDate,int limit)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("cmpid", cmpid, ParameterType.QueryString));
            pars.Add(new Parameter("page", page, ParameterType.QueryString));
            pars.Add(new Parameter("limit", limit, ParameterType.QueryString));
            pars.Add(new Parameter("startDate", startDate, ParameterType.QueryString));
            pars.Add(new Parameter("endDate", endDate, ParameterType.QueryString));

            PageResponseBase<SoslogDto> data = RestHelper.GetInstance().Get<PageResponseBase<SoslogDto>>("app/soslog/list", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        //查询指定fenceI的所含的人员警告定义
        public static AllFenceUserInfoResponse queryAllFenceUserInfo(int fenceId)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("fenceId", fenceId, ParameterType.QueryString));
            AllFenceUserInfoResponse data = RestHelper.GetInstance().Get<AllFenceUserInfoResponse>("app/all-fence-user", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }

        
        /// <summary>
        /// 查询用户的语音录音文件
        /// </summary>
        /// <param name="audioName"></param>
        /// <returns></returns>
        public static UserAudioRecResponse getUserAudioRec(string audioName)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("audioName", audioName, ParameterType.QueryString));
            UserAudioRecResponse  data = RestHelper.GetInstance().Get<UserAudioRecResponse>("app/user/get-user-audiorec", pars, RestHelper.REST_NEW);
            
            return data;
            
        }


        public static User_Old login(string mstrAccount, string strPass)
        {
            
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter( "action", "login", ParameterType.QueryString));
            pars.Add(new Parameter("phone", mstrAccount, ParameterType.QueryString));
            pars.Add(new Parameter("password", strPass, ParameterType.QueryString));
            pars.Add(new Parameter("platform", "Y", ParameterType.QueryString));         
           
            NetHttpUserData data=  RestHelper.GetInstance().Get<NetHttpUserData>("ptt/api/user", pars);
            if (data != null)
                return data.data;
            else
                return null;

        }     

       

        public static CommExists findUserByPhoneAndCmpCode(String cmpcode, String pocno)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("action", "findUserByPhoneAndCmpCode", ParameterType.QueryString));
            pars.Add(new Parameter("cmpcode", cmpcode, ParameterType.QueryString));
            pars.Add(new Parameter("pocno", pocno, ParameterType.QueryString));
            CommExists data = RestHelper.GetInstance().Get<CommExists>("ptt/api/user", pars);             
            return data;           

        }

        public static ExecuteResult syncGroup(String group_id, String group_name, String user_id, String grp_type)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("action", "syncGroupForVoice", ParameterType.QueryString));
            pars.Add(new Parameter("user_id", user_id, ParameterType.QueryString));
            pars.Add(new Parameter("group_id", group_id, ParameterType.QueryString));

            pars.Add(new Parameter("group_name", group_name, ParameterType.QueryString));
            pars.Add(new Parameter("grp_type", grp_type, ParameterType.QueryString));
           
            ExecuteResult data = RestHelper.GetInstance().Get<ExecuteResult>("ptt/api/group", pars);
            return data;

        }

        /// <summary>
        /// 按点到点的分享视频或录像
        /// </summary>
        /// <param name="shareTime">分享时间，取时间秒值</param>
        /// <param name="liveUrl">直播或监控的url地址或录像的点播url地址</param>
        /// <param name="videoType">视频类型 （0：直播，1：录像）</param>
        /// <param name="shareUsers">对方的userid,是单个的</param>
        /// <param name="user_id">自方的userid</param>
        /// <param name="username">自方的username</param>
        public static CommExists shareMoniOrLive(int shareTime, string liveUrl, string videoType, string shareUsers, int user_id, string username)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("action", "shareVideoLive", ParameterType.QueryString));
            pars.Add(new Parameter("user_id", user_id.ToString(), ParameterType.QueryString));
            pars.Add(new Parameter("username", username, ParameterType.QueryString));

            pars.Add(new Parameter("shareUsers", shareUsers, ParameterType.QueryString));
            pars.Add(new Parameter("videoType", videoType, ParameterType.QueryString));

            pars.Add(new Parameter("liveUrl", liveUrl, ParameterType.QueryString));
            pars.Add(new Parameter("shareTime", shareTime.ToString(), ParameterType.QueryString));
            CommExists data = RestHelper.GetInstance().Get<CommExists>("ptt/api/user", pars);
            return data; 

        }

        /// <summary>
        /// 查询临时群信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static TempGroupResponse queryTmpGroup(int groupId )
        {

            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("groupId", groupId, ParameterType.QueryString));
            TempGroupResponse data = RestHelper.GetInstance().Get<TempGroupResponse>("app/group/tmp-query", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;

        }

        /// <summary>
        /// 查询agora的活动的直播频道，已做了cmpid区分
        /// 目前版本只是临时性，一次性查agora 300条，然后一次性用cmpid含有200个userId去过滤
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static AgoraChannelActiveResponse queryAgoraChannelActive()
        {

            List<Parameter> pars = new List<Parameter>();
            AgoraChannelActiveResponse data = RestHelper.GetInstance().Get<AgoraChannelActiveResponse>("app/agora/agora-channel-query", pars, RestHelper.REST_NEW);
            if (data != null)
                return data;
            else
                return null;
        }



        /// <summary>
        /// 创建临时群信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static TempGroupResponse createTmpGroup(GroupTempDto dto)
        {

            return RestHelper.GetInstance().Post_hasData<TempGroupResponse>("app/group/tmp-create", dto, RestHelper.REST_NEW);
        }

        public static ResponseBase deleteTmpGroup(int groupId, int type, int priv)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("groupId", groupId, ParameterType.QueryString));
            pars.Add(new Parameter("type", type, ParameterType.QueryString));
            pars.Add(new Parameter("priv", priv, ParameterType.QueryString));
           return
                RestHelper.GetInstance().Get<ResponseBase>("app/group/tmp-delete", pars, RestHelper.REST_NEW);

        }

        /// <summary>
        /// 用户上线或下线更新user表
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static ResponseBase logonOrLogout(int state)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("state", state, ParameterType.QueryString));
            return
                 RestHelper.GetInstance().Get<ResponseBase>("app/user/logonOrLogout", pars, RestHelper.REST_NEW);

        }

        /// <summary>
        /// 当前的cpmid所含有用户全下线
        /// </summary>
        /// <returns></returns>
        public static ResponseBase allUserLogoutByCmpid()
        {
            List<Parameter> pars = new List<Parameter>();
             
            return
                 RestHelper.GetInstance().Get<ResponseBase>("app/user/allLogout", pars, RestHelper.REST_NEW);

        }


        public static GroupListResponse userAllGroup()
        {
            List<Parameter> pars = new List<Parameter>();

            return
                 RestHelper.GetInstance().Get<GroupListResponse>("app/group/query-belonggroup-by-userid", pars, RestHelper.REST_NEW);
        }


        public static GroupTempListResponse userAllTempGroup()
        {
            List<Parameter> pars = new List<Parameter>();

            return
                 RestHelper.GetInstance().Get<GroupTempListResponse>("app/group/query-belongtempgroup-by-userid", pars, RestHelper.REST_NEW);
        }


        /// <summary>
        /// 更新临时群信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static ResponseBase updateTmpGroup(GroupTempDto dto)
        {
           return
            RestHelper.GetInstance().Post_hasData<ResponseBase>("app/group/tmp-update", dto, RestHelper.REST_NEW);
             
        }

        public static ResponseBase saveFenceUser(FenceUserDto dto)
        {
            return
            RestHelper.GetInstance().Post_hasData<ResponseBase>("app/fence/save-user", dto, RestHelper.REST_NEW);
        }

        public static ResponseBase updateFenceUser(FenceUserDto dto)
        {
            return
            RestHelper.GetInstance().Post_hasData<ResponseBase>("app/fence/update-user", dto, RestHelper.REST_NEW);
        }

        //删除fence中的用户规则
        public static ResponseBase deleteFenceUser(int ruleId)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("ruleId", ruleId, ParameterType.QueryString));
            return
               RestHelper.GetInstance().Get<ResponseBase>("app/fence/delete-user", pars, RestHelper.REST_NEW);

        }

        public static ResponseBase updateUserInfo(User user)
        {
            return
           RestHelper.GetInstance().Post_hasData<ResponseBase>("app/user/update-user", user, RestHelper.REST_NEW);
        }

        /// <summary>
        /// 组的强插和强拆
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="groupTypeId"></param>
        /// <param name="changeType"></param>
        /// <param name="useridlist"></param>
        /// <returns></returns>
        public static ResponseBase groupUserChange(int groupid, int groupTypeId, int changeType, String useridlist)
        {
            List<Parameter> pars = new List<Parameter>();
            pars.Add(new Parameter("groupId", groupid, ParameterType.QueryString));
            pars.Add(new Parameter("groupType", groupTypeId, ParameterType.QueryString));
            pars.Add(new Parameter("changeType", changeType, ParameterType.QueryString));
            pars.Add(new Parameter("userIdList", useridlist, ParameterType.QueryString));
            return
                 RestHelper.GetInstance().Get<ResponseBase>("app/group/group-user-change", pars, RestHelper.REST_NEW);
        }


        /// <summary>
        /// 单个用户的视频监控请求
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="m_resolve"></param>
        ///  <param name="m_protocol">协议，支持rtmp,rtsp</param>
        public static CommResult applyMoni(int userid, int m_resolve, String m_protocol)
        {
            string formpara = "{\"users\":[" + userid + "],\"resolve\":" + m_resolve + "}";

            string resource = "vmonitor/v1/monitor_apply.json";
            if (m_protocol.Equals("rtsp"))
                resource = "vmonitor/v1/monitor_apply_rtsp.json";
            
            CommResult nud = new CommResult();
            nud.state = true;
            RestHelper.GetInstance().Post(resource, formpara, RestHelper.REST_VIDEO);
            nud.reason = "OK";
            return nud;
        }

     public void Dispose()
        {
            //_client=null;
            GC.SuppressFinalize(this);
        }
    }
}
