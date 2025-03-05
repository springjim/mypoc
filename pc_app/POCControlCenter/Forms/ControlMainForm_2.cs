
using POCControlCenter.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter
{
    public partial class ControlMainForm: Form
    {
      

        private UserGPSConfigForm userGpsConfigForm;
        
        //
        
        //以下是关于SOS弹窗的状态变量
        public  bool SOS_INCOME_CALL_ENABLE = false;    //表示是否是SOS进入对讲模式
        public  SOSPopUpForm  sosPopupForm=null;
        /// <summary>
        /// 当前SOS会话的人员ID，当如果没有时为0，如果有则是具体人员的ID
        /// </summary>
        public  int SOS_Session_UserID = 0;
        /// <summary>
        /// 当前的SOS会话有无退出
        /// </summary>
        public  bool SOS_Session_Finish = true;
        public List<long> UnReceiveSOSUserIDS = new List<long>();

        /// <summary>
        /// 1表示是从对讲群组进入的，2表示通过搜索进来, 3 表示通过通讯录进来的
        /// </summary>
        private int SELECT_TREEVIEW_MODE = 1;          

               
       

        //每种查询的实始结果记录数
        public int count_video = 0;
        public int count_audio = 0;
        public int count_session = 0;

        //每页多少记录
        public int page_size_video = 15;
        public int page_size_audio = 15;
        public int page_size_session = 15;

        //共多少页
        public int page_num_video = 0;
        public int page_num_audio = 0;
        public int page_num_session = 0;

        //当前多少页号 (从1开始)
        public int page_index_video = 1;
        public int page_index_audio = 1;
        public int page_index_session = 1;

        //当前视频记录的绑定对象
        private IList<VideoRec> IVideoRec = new List<VideoRec>();
        private BindingList<VideoRec> BVideoRec;

        private IList<AudioRec> IAudioRec = new List<AudioRec>();
        private BindingList<AudioRec> BAudioRec;

        private IList<POCSessionRec> IPOCSessionRec = new List<POCSessionRec>();
        private BindingList<POCSessionRec> BPOCSessionRec;

        //
        //查询条件字段: 视频记录部分
        string video_type = "";
        string poc_account;
        string poc_account_like = "N";
        string cust_account;
        string cust_account_like = "N";

        string user_name2;
        string user_name_like = "N";
        string record_date_from;
        string record_date_to;

        string file_size_from;
        string file_size_to;
        string video_duration_from;
        string video_duration_to;
        string page_size;
        //查询条件字段: 语音记录部分       
        string poc_account_audio;
        string poc_account_like_audio = "N";

        string cust_account_audio;
        string cust_account_like_audio = "N";

        string user_name2_audio;
        string user_name_like_audio = "N";
        string record_date_from_audio;
        string record_date_to_audio;

        string audio_duration_from;
        string audio_duration_to;
        string page_size_audio_query;
        string group_id_audio;
        //查询条件字段: 会话记录部分
        string file_type_session;
        string poc_account_session;
        string poc_account_like_session = "N";

        string cust_account_session;
        string cust_account_like_session = "N";

        string user_name2_session;
        string user_name_like_session = "N";
        string record_date_from_session;
        string record_date_to_session;

        string page_size_session_query;
        string group_id_session;

        //*************************************
        //构造视频录像的网址
        ///hdd/htdocs/video/record//moni_600012159_6b6_8b8-1504087251-2017-08-30-180051.flv
        string HTTP_URL_PREFIX_VIDEO = "http://" + HttpAPI.VIDEOIP+":"+ HttpAPI.VIDEO_XAMPP_Port + "/video/record/";

        ///hdd/htdocs/voice//2017-08-11/80001196/600010169/80001196_600010169_1502435647899.amr
        string HTTP_URL_PREFIX_AUDIO = "http://" + HttpAPI.VIDEOIP+":"+ HttpAPI.VIDEO_XAMPP_Port+ "/voice/";

        //群组会话中的文件网址构造
        //这里有个bug, 北京服务器也开启了语音服务
        string HTTP_URL_PREFIX_SESSION = "http://" + HttpAPI.VIDEOIP + "/voice/";

        //private MediaPlayForm meidaPlayForm;
        //*******************************************************



        private void queryByPageIndex_Video(int pageIndex, string video_type, string poc_account, string poc_account_like, string cust_account, string cust_account_like, string user_name, string user_name_like, string record_date_from, string record_date_to, string file_size_from, string file_size_to, string video_duration_from, string video_duration_to, string page_size)
        {

            List<VideoRec> listVideo =
                HttpAPI.queryVideoRecByPageIndex(pageIndex.ToString(), video_type, poc_account, poc_account_like, cust_account, cust_account_like,
                user_name, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
                video_duration_from, video_duration_to, page_size);
            BVideoRec.Clear();
            //BindingManager_videobase.PositionChanged += null;
            //重新绑定
            foreach (VideoRec rec in listVideo)
            {
                //时间/状态处理
                if (rec.recorddate != null && rec.recorddate > 0)
                {

                    DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(rec.recorddate);
                    //转成本地时间                                                                     

                    DateTime localdt = utcdt.ToLocalTime();
                    //2017.8.16 加入时区差异的处理                                    
                    localdt = localdt.AddHours(ZoneInterval_UserServer);

                    rec.record_date_str = localdt.ToString("yyyy-MM-dd HH:mm:ss");
                    //时长/文件大小格式化
                    rec.duration = Math.Round(rec.duration / 60.0, 3);
                    rec.filesize = Math.Round(rec.filesize / (1024.0 * 1024), 3);

                }
                if (rec.video_type.Equals("LIVE"))
                    rec.video_type_str = WinFormsStringResource.Video_Live;
                else
                    rec.video_type_str = WinFormsStringResource.Video_Moni;

                BVideoRec.Add(rec);
            }

            //BindingManager_videobase.PositionChanged += BindingManager_videobase_PositionChanged;

        }

        private void queryByPageIndex_Audio(int pageIndex, string poc_account, string poc_account_like,
            string cust_account, string cust_account_like, string user_name, string user_name_like,
            string record_date_from, string record_date_to,
            string audio_duration_from, string audio_duration_to, string page_size, string group_id)
        {

            List<AudioRec> listVideo =
                HttpAPI.queryAudioRecByPageIndex(pageIndex.ToString(), poc_account, poc_account_like, cust_account, cust_account_like,
                user_name, user_name_like, record_date_from, record_date_to,
                audio_duration_from, audio_duration_to, page_size, Convert.ToInt32(group_id));

            BAudioRec.Clear();
            //BindingManager_audiobase.PositionChanged += null;
            //重新绑定
            foreach (AudioRec rec in listVideo)
            {
                //时间/状态处理
                if (rec.recorddate != null && rec.recorddate > 0)
                {
                    DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(rec.recorddate);
                    //转成本地时间                                                                     

                    DateTime localdt = utcdt.ToLocalTime();
                    //2017.8.16 加入时区差异的处理                                    
                    localdt = localdt.AddHours(ZoneInterval_UserServer);
                    rec.record_date_str = localdt.ToString("yyyy-MM-dd HH:mm:ss");



                }

                BAudioRec.Add(rec);
            }

            //BindingManager_audiobase.PositionChanged += BindingManager_audiobase_PositionChanged;

        }

        private void queryByPageIndex_Session(int pageIndex, string file_type, string poc_account, string poc_account_like,
       string cust_account, string cust_account_like, string user_name, string user_name_like,
       string record_date_from, string record_date_to,
       string page_size, string group_id)
        {
            List<POCSessionRec> listVideo =
                HttpAPI.queryPOCSessionRecByPageIndex(pageIndex.ToString(), file_type, poc_account, poc_account_like, cust_account, cust_account_like,
                user_name, user_name_like, record_date_from, record_date_to,
                this.page_size_session_query, Convert.ToInt32(group_id_session));

            BPOCSessionRec.Clear();
            //BindingManager_sessionbase.PositionChanged += null;
            //重新绑定
            foreach (POCSessionRec rec in listVideo)
            {
                //时间/状态处理
                if (rec.file_uptime != null && rec.file_uptime > 0)
                {
                    DateTime utcdt = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(rec.file_uptime / 1000);
                    //转成本地时间                                                                     

                    DateTime localdt = utcdt.ToLocalTime();
                    //2017.8.16 加入时区差异的处理                                    
                    localdt = localdt.AddHours(ZoneInterval_UserServer);
                    rec.file_uptime_str = localdt.ToString("yyyy-MM-dd HH:mm:ss");

                }

                BPOCSessionRec.Add(rec);
            }

            //BindingManager_sessionbase.PositionChanged += BindingManager_sessionbase_PositionChanged;

        }
      
            

       


        

       

        



    }
}
