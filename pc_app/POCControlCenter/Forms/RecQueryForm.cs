using POCClientNetLibrary;
using POCControlCenter.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vlc.DotNet.Core.Interops.Signatures;
using Vlc.DotNet.Forms;

namespace POCControlCenter
{
    public partial class RecQueryForm : Form
    {
        private VideoQueryForm myVideoQueryForm = null;
        private AudioQueryForm myAudioQueryForm = null;
        private SessionQueryForm mySessionQueryForm = null;

        public VlcControl vlcControl1;
        public Image shengbo_file;

        private int ZoneInterval_UserServer;

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
        private const string PARAFILE = "runpara.ini";

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

        //查询条件字段: 视频记录部分 end
        BindingManagerBase BindingManager_videobase;
        BindingManagerBase BindingManager_audiobase;
        BindingManagerBase BindingManager_sessionbase;
        //*****************************************************************
        //构造视频录像的网址
        ///hdd/htdocs/video/record//moni_600012159_6b6_8b8-1504087251-2017-08-30-180051.flv
        string HTTP_URL_PREFIX_VIDEO = "http://" + HttpAPI.VIDEOIP + "/video/record/";

        ///hdd/htdocs/voice//2017-08-11/80001196/600010169/80001196_600010169_1502435647899.amr
        string HTTP_URL_PREFIX_AUDIO = "http://" + HttpAPI.VIDEOIP + "/voice/";

        //群组会话中的文件网址构造
        //这里有个bug, 北京服务器也开启了语音服务
        string HTTP_URL_PREFIX_SESSION = "http://" + HttpAPI.VIDEOIP + "/voice/";

        HttpAPI http_api = new HttpAPI();

        /// <summary>
        /// 表示是否手动拖动播放进度条
        /// </summary>
        Boolean enterSeek = false;

        /// <summary>
        /// 当前媒体类型: video, audio, other等
        /// 表示当前TabControl进入的类型
        /// </summary>
        string  currMediaType = "video";

        /// <summary>
        /// 描述当前正在播放的类型
        /// </summary>
        string currMediaPlayingType = "";

        /// <summary>
        /// 当前播放的位置, 视频
        /// </summary>
        int currMediaIndex_Video = -1;

        /// <summary>
        /// 当前播放的位置, 语音
        /// </summary>
        int currMediaIndex_Audio = -1;

        /// <summary>
        /// 记录trackbar的位置
        /// </summary>
        int currTrackBarIndex_Video = 0;
        int currTrackBarIndex_Audio = 0;
        //
        string FullDownloadVideoPath = "";
        string FullDownloadAudioPath = "";
        string FullDownloadSessionPath = "";

        private void InitRunPara()
        {
            //读取配置文件
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, PARAFILE);
            //指定ini文件的路径
            IniFile ini = new IniFile(path);
            
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
                FullDownloadSessionPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "poc_session");
            else
            {
                FullDownloadSessionPath = ini.IniReadValue("download", "pathsession").Trim();
            }
            

        }


        public RecQueryForm(int ZoneInterval_UserServer)
        {            

            InitializeComponent();            

            this.dgv_video.AutoGenerateColumns = false;
            this.dgv_audio.AutoGenerateColumns = false;
            this.dgv_session.AutoGenerateColumns = false;

            this.ZoneInterval_UserServer = ZoneInterval_UserServer;
            //前视频记录的绑定对象与dgv绑定
            this.BVideoRec = new BindingCollection<VideoRec>(IVideoRec);
            this.dgv_video.DataSource = BVideoRec;
            BVideoRec.Clear();
            //audio
            this.BAudioRec = new BindingCollection<AudioRec>(IAudioRec);
            this.dgv_audio.DataSource = BAudioRec;
            BAudioRec.Clear();
            //session
            this.BPOCSessionRec = new BindingCollection<POCSessionRec>(IPOCSessionRec);
            this.dgv_session.DataSource = BPOCSessionRec;
            BPOCSessionRec.Clear();
            //
            initVars();
            //默认显示vedio tab
            tabControl1.SelectedIndex = 0;
           

        }

        //外部调用:查询组内会话
        public void setQueryValueByGroup(string mygroup_id)
        {
            //
            file_type_session = "";

            group_id_session = mygroup_id;

            poc_account_session = "";
            poc_account_like_session = "N";

            cust_account_session = "";
            cust_account_like_session = "N";

            user_name2_session = "";
            user_name_like_session = "N";

            record_date_from_session = DateTime.Now.ToString("yyyy/MM/dd");
            record_date_to_session = DateTime.Now.ToString("yyyy/MM/dd");
            this.page_size_session_query = "10";

            PageQueryTotal pagetotal =
            HttpAPI.queryPOCSessionRecTotalInfo(file_type_session, poc_account_session,
                poc_account_like_session, cust_account_session,
                cust_account_like_session,
                user_name2_session, user_name_like_session, record_date_from_session, record_date_to_session,
                page_size_session_query, Convert.ToInt32(group_id_session));

            //MessageBox.Show(pagetotal.count.ToString());
            //如果pagetotal.count>0 , 还要进行下一步分页的第一页的获得
            //还要考虑时区差的加入，所有recorddate要处理
            if (pagetotal.count > 0)
            {
                this.count_session = pagetotal.count;
                this.page_size_session = Convert.ToInt32(page_size_session_query);
                this.page_num_session = pagetotal.page_num;
                this.page_index_session = 1;
                this.tb_sessionpageindex.Text = "1";
                this.session_pagetag.Text = "1/" + this.page_num_session.ToString();
                this.session_desc.Text = WinFormsStringResource.QueryTotal + count_session.ToString() + WinFormsStringResource.QueryRecDESC
                    + "," + WinFormsStringResource.QueryPerPage + this.page_size_session.ToString() +
                    WinFormsStringResource.QueryRecDESC + "," + WinFormsStringResource.QueryTotal
                    + this.page_num_session.ToString() + WinFormsStringResource.QueryPage;
                //查询第一页                    
                queryByPageIndex_Session(1, file_type_session, poc_account_session,
                    poc_account_like_session, cust_account_session,
                    cust_account_like_session, user_name2_session, user_name_like_session,
                    record_date_from_session, record_date_to_session,
                    page_size_session_query, group_id_session);

            }
            else
            {
                //清除并显示没有查到数据
                this.BPOCSessionRec.Clear();
                this.count_session = 0;
                this.page_num_session = 0;
                this.session_pagetag.Text = "";
                this.session_desc.Text = "";
                //MessageBox.Show(WinFormsStringResource.NoData);
            }
            tabControl1.SelectedIndex = 2;

        }


        //外部调用:查询个人内的语音记录、视频记录
        public void setQueryValueByPerson(string mypoc_account)
        {
            //1.自动查当天的、传入人的视频记录
            video_type = "";

            poc_account = mypoc_account;
            poc_account_like = "N";

            cust_account = "";
            cust_account_like = "N";

            user_name2 = "";
            user_name_like = "N";

            record_date_from = DateTime.Now.ToString("yyyy/MM/dd");
            record_date_to = DateTime.Now.ToString("yyyy/MM/dd");

            file_size_from = "";
            file_size_to = "";
            video_duration_from = "";
            video_duration_to = "";

            page_size = "10";

            PageQueryTotal pagetotal =
            HttpAPI.queryVideoRecTotalInfo(video_type, poc_account, poc_account_like, cust_account, cust_account_like,
                user_name2, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
                video_duration_from, video_duration_to, page_size);

            //MessageBox.Show(pagetotal.count.ToString());
            //如果pagetotal.count>0 , 还要进行下一步分页的第一页的获得
            //还要考虑时区差的加入，所有recorddate要处理
            if (pagetotal.count > 0)
            {
                this.count_video = pagetotal.count;
                this.page_size_video = Convert.ToInt32(page_size);
                this.page_num_video = pagetotal.page_num;
                this.page_index_video = 1;
                this.tb_videopageindex.Text = "1";
                this.video_pagetag.Text = "1/" + this.page_num_video.ToString();
                this.video_desc.Text = WinFormsStringResource.QueryTotal + count_video.ToString() + WinFormsStringResource.QueryRecDESC
                    + "," + WinFormsStringResource.QueryPerPage + this.page_size_video.ToString() +
                    WinFormsStringResource.QueryRecDESC + "," + WinFormsStringResource.QueryTotal
                    + this.page_num_video.ToString() + WinFormsStringResource.QueryPage;
                //查询第一页                    
                queryByPageIndex_Video(1, video_type, poc_account, poc_account_like, cust_account, cust_account_like,
            user_name2, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
            video_duration_from, video_duration_to, page_size);

            }
            else
            {
                //清除并显示没有查到数据
                this.BVideoRec.Clear();
                this.count_video = 0;
                this.page_num_video = 0;
                this.video_pagetag.Text = "";
                this.video_desc.Text = "";
                //MessageBox.Show(WinFormsStringResource.NoData);
            }

            //2.自动查当天的、传入人的语音记录
            group_id_audio = "-1";
            poc_account_audio = mypoc_account;
            poc_account_like_audio = "N";

            cust_account_audio = "";
            cust_account_like_audio = "N";

            user_name2_audio = "";
            user_name_like_audio = "N";

            record_date_from_audio = DateTime.Now.ToString("yyyy/MM/dd");
            record_date_to_audio = DateTime.Now.ToString("yyyy/MM/dd");

            audio_duration_from = "";
            audio_duration_to = "";

            this.page_size_audio_query = "10";

            pagetotal =
            HttpAPI.queryAudioRecTotalInfo(poc_account_audio, poc_account_like_audio, cust_account_audio,
                cust_account_like_audio,
                user_name2_audio, user_name_like_audio, record_date_from_audio, record_date_to_audio,
                audio_duration_from, audio_duration_to, page_size_audio_query, Convert.ToInt32(group_id_audio));

            //MessageBox.Show(pagetotal.count.ToString());
            //如果pagetotal.count>0 , 还要进行下一步分页的第一页的获得
            //还要考虑时区差的加入，所有recorddate要处理
            if (pagetotal.count > 0)
            {
                this.count_audio = pagetotal.count;
                this.page_size_audio = Convert.ToInt32(page_size_audio_query);
                this.page_num_audio = pagetotal.page_num;
                this.page_index_audio = 1;
                this.tb_audiopageindex.Text = "1";
                this.audio_pagetag.Text = "1/" + this.page_num_audio.ToString();
                this.audio_desc.Text = WinFormsStringResource.QueryTotal + count_audio.ToString() + WinFormsStringResource.QueryRecDESC
                    + "," + WinFormsStringResource.QueryPerPage + this.page_size_audio.ToString() +
                    WinFormsStringResource.QueryRecDESC + "," + WinFormsStringResource.QueryTotal
                    + this.page_num_audio.ToString() + WinFormsStringResource.QueryPage;
                //查询第一页                    
                queryByPageIndex_Audio(1, poc_account_audio, poc_account_like_audio, cust_account_audio,
                    cust_account_like_audio, user_name2_audio, user_name_like_audio,
                    record_date_from_audio, record_date_to_audio, audio_duration_from, audio_duration_to,
                    page_size_audio_query, group_id_audio);

            }
            else
            {
                //清除并显示没有查到数据
                this.BAudioRec.Clear();
                this.count_audio = 0;
                this.page_num_audio = 0;
                this.audio_pagetag.Text = "";
                this.audio_desc.Text = "";
                //MessageBox.Show(WinFormsStringResource.NoData);
            }

            tabControl1.SelectedIndex = 0;

        }


        /// <summary>
        /// 实始化一些textbox
        /// </summary>
        private void initVars()
        {

            //各种初始化
            video_pagetag.Text = "";
            video_desc.Text = "";
            audio_pagetag.Text = "";
            audio_desc.Text = "";
            session_pagetag.Text = "";
            session_desc.Text = "";

            //***********************************
            filename_video.Text = "";
            duration_video.Text = "";
            createdate_video.Text = "";
            username.Text = "";
            recorddate.Text = "";
            media_state.Text = "";
            trackBar1.Enabled = false;

            //初始化配置，指定引用库            

            //usercontrol_video.username.Text = "";
            //usercontrol_video.time.Text = "";

            ////创建vlc_instance
            //usercontrol_video.vlc_instance = MediaPlayer.Create_Media_Instance();
            //usercontrol_video.vlc_media_player =
            //    MediaPlayer.Create_MediaPlayer(usercontrol_video.vlc_instance,
            //    usercontrol_video.panelVideo.Handle);
            //
            BindingManager_videobase = this.BindingContext[dgv_video.DataSource];
            BindingManager_videobase.PositionChanged += null;

            BindingManager_audiobase = this.BindingContext[dgv_audio.DataSource];
            BindingManager_audiobase.PositionChanged += null;
            //
            BindingManager_sessionbase = this.BindingContext[dgv_session.DataSource];
            BindingManager_sessionbase.PositionChanged += null;

        }

        private void VlcControl1_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            e.VlcLibDirectory = new DirectoryInfo(System.Environment.CurrentDirectory);
            //var currentAssembly = Assembly.GetEntryAssembly();
            //var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            //if (currentDirectory == null)
            //    return;
            //if (AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture == ProcessorArchitecture.X86)
            //    e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"..\..\..\lib\x86\"));
            //else
            //    e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"..\..\..\lib\x64\"));
        }

        private void BindingManager_videobase_PositionChanged(object sender, EventArgs e)
        {
            int rown = ((BindingManagerBase)sender).Position;
            if (rown < 0) return;
            if (dgv_video.RowCount < (rown + 1)) return;
            //this.usercontrol_video.username.Text =
            //    dgv_video.Rows[rown].Cells["user_name"].Value.ToString();

            //this.filename_video.Text = dgv_video.Rows[rown].Cells["stream"].Value.ToString();
            //this.duration_video.Text = dgv_video.Rows[rown].Cells["duration"].Value.ToString()+"(分钟)";
            //this.createdate_video.Text = dgv_video.Rows[rown].Cells["filesize"].Value.ToString()+"(MB)";
            //this.username.Text= dgv_video.Rows[rown].Cells["user_name"].Value.ToString();
            //this.recorddate.Text= dgv_video.Rows[rown].Cells["record_date_str"].Value.ToString();

        }

        private void queryByPageIndex_Video(int pageIndex, string video_type, string poc_account, string poc_account_like, string cust_account, string cust_account_like, string user_name, string user_name_like, string record_date_from, string record_date_to, string file_size_from, string file_size_to, string video_duration_from, string video_duration_to, string page_size)
        {

            List<VideoRec> listVideo =
                HttpAPI.queryVideoRecByPageIndex(pageIndex.ToString(), video_type, poc_account, poc_account_like, cust_account, cust_account_like,
                user_name, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
                video_duration_from, video_duration_to, page_size);
            BVideoRec.Clear();
            BindingManager_videobase.PositionChanged += null;
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

            BindingManager_videobase.PositionChanged += BindingManager_videobase_PositionChanged;

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
            BindingManager_audiobase.PositionChanged += null;
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

            BindingManager_audiobase.PositionChanged += BindingManager_audiobase_PositionChanged;

        }

        private void BindingManager_audiobase_PositionChanged(object sender, EventArgs e)
        {
            int rown = ((BindingManagerBase)sender).Position;
            if (rown < 0) return;
            if (dgv_audio.RowCount < (rown + 1)) return;
            //this.usercontrol_video.username.Text =
            //    dgv_audio.Rows[rown].Cells["user_name_a"].Value.ToString();

            //this.filename_video.Text = dgv_audio.Rows[rown].Cells["audio_name_a"].Value.ToString();
            //this.duration_video.Text = dgv_audio.Rows[rown].Cells["duration_a"].Value.ToString()+"(秒)";
            //this.createdate_video.Text = dgv_audio.Rows[rown].Cells["filesize_a"].Value.ToString()+"(KB)";
            //this.username.Text = dgv_video.Rows[rown].Cells["user_name"].Value.ToString();
            //this.recorddate.Text = dgv_video.Rows[rown].Cells["record_date_str"].Value.ToString();

        }

        private void btnVideoQuery_Click(object sender, EventArgs e)
        {
            //
            if (myVideoQueryForm == null)
                myVideoQueryForm = new VideoQueryForm();

            if (myVideoQueryForm.ShowDialog() == DialogResult.OK)
            {
                video_type = "";
                if (myVideoQueryForm.cbVideo_type.SelectedIndex == 1)
                    video_type = "LIVE";
                else if (myVideoQueryForm.cbVideo_type.SelectedIndex == 2)
                    video_type = "MONI";

                poc_account = myVideoQueryForm.poc_account.Text.Trim();
                poc_account_like = "N";
                if (myVideoQueryForm.poc_account_like.Checked)
                    poc_account_like = "Y";

                cust_account = myVideoQueryForm.cust_account.Text.Trim();
                cust_account_like = "N";
                if (myVideoQueryForm.cust_account_like.Checked)
                    cust_account_like = "Y";

                user_name2 = myVideoQueryForm.user_name.Text.Trim();

                user_name_like = "N";
                if (myVideoQueryForm.user_name_like.Checked)
                    user_name_like = "Y";

                record_date_from = myVideoQueryForm.record_date_from.Text;
                record_date_to = myVideoQueryForm.record_date_to.Text;

                file_size_from = myVideoQueryForm.file_size_from.Text;
                file_size_to = myVideoQueryForm.file_size_to.Text;
                video_duration_from = myVideoQueryForm.video_duration_from.Text;
                video_duration_to = myVideoQueryForm.video_duration_to.Text;

                page_size = myVideoQueryForm.cbPageSize.Text;


                PageQueryTotal pagetotal =
                HttpAPI.queryVideoRecTotalInfo(video_type, poc_account, poc_account_like, cust_account, cust_account_like,
                    user_name2, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
                    video_duration_from, video_duration_to, page_size);

                //MessageBox.Show(pagetotal.count.ToString());
                //如果pagetotal.count>0 , 还要进行下一步分页的第一页的获得
                //还要考虑时区差的加入，所有recorddate要处理
                if (pagetotal.count > 0)
                {
                    this.count_video = pagetotal.count;
                    this.page_size_video = Convert.ToInt32(page_size);
                    this.page_num_video = pagetotal.page_num;
                    this.page_index_video = 1;
                    this.tb_videopageindex.Text = "1";
                    this.video_pagetag.Text = "1/" + this.page_num_video.ToString();
                    this.video_desc.Text = WinFormsStringResource.QueryTotal + count_video.ToString() + WinFormsStringResource.QueryRecDESC
                        + "," + WinFormsStringResource.QueryPerPage + this.page_size_video.ToString() +
                        WinFormsStringResource.QueryRecDESC + "," + WinFormsStringResource.QueryTotal
                        + this.page_num_video.ToString() + WinFormsStringResource.QueryPage;
                    //查询第一页                    
                    queryByPageIndex_Video(1, video_type, poc_account, poc_account_like, cust_account, cust_account_like,
                user_name2, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
                video_duration_from, video_duration_to, page_size);

                }
                else
                {
                    //清除并显示没有查到数据
                    this.BVideoRec.Clear();
                    this.count_video = 0;
                    this.page_num_video = 0;
                    this.video_pagetag.Text = "";
                    this.video_desc.Text = "";
                    MessageBox.Show(WinFormsStringResource.NoData);
                }

            }

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

        private void tb_videopageindex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //判断内容
                if (tb_videopageindex.Text.Trim() == "") return;
                int pageno = Convert.ToInt32(tb_videopageindex.Text.Trim());
                if (count_video > 0 && pageno <= this.page_num_video
                    && pageno >= 1)
                {
                    queryByPageIndex_Video(pageno, video_type, poc_account, poc_account_like, cust_account, cust_account_like,
                user_name2, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
                video_duration_from, video_duration_to, page_size);

                    this.video_pagetag.Text = pageno.ToString() + "/" + this.page_num_video.ToString();

                }
                else
                {
                    MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
                }

            }
        }

        private void tbFirst_Video_Click(object sender, EventArgs e)
        {
            int pageno = 1;
            if (this.count_video > 0 && pageno <= this.page_num_video
                    && pageno >= 1)
            {

                queryByPageIndex_Video(pageno, video_type, poc_account, poc_account_like, cust_account, cust_account_like,
                user_name2, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
                video_duration_from, video_duration_to, page_size);

                this.video_pagetag.Text = pageno.ToString() + "/" + this.page_num_video.ToString();
                this.page_index_video = 1;
                this.tb_videopageindex.Text = "1";

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }

        }

        private void tbLast_Video_Click(object sender, EventArgs e)
        {
            if (this.count_video == 0) return;
            if (this.page_num_video == 0) return;
            int pageno = page_num_video;
            if (this.count_video > 0 && pageno <= this.page_num_video
                    && pageno >= 1)
            {

                queryByPageIndex_Video(pageno, video_type, poc_account, poc_account_like, cust_account, cust_account_like,
                user_name2, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
                video_duration_from, video_duration_to, page_size);

                this.video_pagetag.Text = pageno.ToString() + "/" + this.page_num_video.ToString();
                this.page_index_video = this.page_num_video;
                this.tb_videopageindex.Text = page_num_video.ToString();

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }

        }

        private void tbPrev_Video_Click(object sender, EventArgs e)
        {
            if (this.count_video == 0) return;
            if (this.page_num_video == 0) return;
            if (page_index_video == 1)
            {
                MessageBox.Show(WinFormsStringResource.PageNoFirstAlert);
                return;
            }
            int pageno = this.page_index_video - 1;
            if (this.count_video > 0 && pageno <= this.page_num_video
                   && pageno >= 1)
            {

                queryByPageIndex_Video(pageno, video_type, poc_account, poc_account_like, cust_account, cust_account_like,
                user_name2, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
                video_duration_from, video_duration_to, page_size);

                this.video_pagetag.Text = pageno.ToString() + "/" + this.page_num_video.ToString();
                this.page_index_video = pageno;
                this.tb_videopageindex.Text = pageno.ToString();

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }

        }

        private void tbNext_Video_Click(object sender, EventArgs e)
        {
            if (this.count_video == 0) return;
            if (this.page_num_video == 0) return;

            if (page_index_video == this.page_num_video)
            {
                MessageBox.Show(WinFormsStringResource.PageNoLastAlert);
                return;
            }
            int pageno = this.page_index_video + 1;
            if (this.count_video > 0 && pageno <= this.page_num_video
                  && pageno >= 1)
            {

                queryByPageIndex_Video(pageno, video_type, poc_account, poc_account_like, cust_account, cust_account_like,
                user_name2, user_name_like, record_date_from, record_date_to, file_size_from, file_size_to,
                video_duration_from, video_duration_to, page_size);

                this.video_pagetag.Text = pageno.ToString() + "/" + this.page_num_video.ToString();
                this.page_index_video = pageno;
                this.tb_videopageindex.Text = pageno.ToString();

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }

        }

        private void dodgv_video_CellContentClick_2(int RowIndex_, int ColumnIndex_)
        {
            try
            {
                int rowindex = RowIndex_;
                if (rowindex == -1) return;  //点击了header
                int CIndex = ColumnIndex_;
                if (CIndex == dgv_video.Columns["playonline"].Index)
                {
                    currMediaIndex_Video = rowindex;   //记住 
                    //右边系列属性
                    this.filename_video.Text = dgv_video.Rows[rowindex].Cells["stream"].Value.ToString();
                    this.duration_video.Text = dgv_video.Rows[rowindex].Cells["duration"].Value.ToString() + "(分钟)";
                    this.createdate_video.Text = dgv_video.Rows[rowindex].Cells["filesize"].Value.ToString() + "(MB)";
                    this.username.Text = dgv_video.Rows[rowindex].Cells["user_name"].Value.ToString();
                    this.recorddate.Text = dgv_video.Rows[rowindex].Cells["record_date_str"].Value.ToString();

                    //MessageBox.Show(dgv_video.Rows[rowindex].Cells["video_path"].Value.ToString());
                    string video_path_str = dgv_video.Rows[rowindex].Cells["video_path"].Value.ToString().Trim();
                    ///hdd/htdocs/video/record//moni_600012159_6b6_8b8-1504087251-2017-08-30-180051.flv
                    video_path_str = video_path_str.Replace("/hdd/htdocs/video/record/", "");
                    if (video_path_str.StartsWith("/"))
                        video_path_str = video_path_str.Substring(1);
                    //string HTTP_URL_PREFIX_VIDEO = "http://" + HttpAPI.VIDEOIP + "/video/record/";
                    video_path_str = HTTP_URL_PREFIX_VIDEO + video_path_str;
                    //MessageBox.Show(video_path_str);
                    //清理上次播放中的播放器占的内存等
                    //vlcControl1.Play(video_path_str,MediaPlayer.arguments);
                    trackBar1.Minimum = 0; //duration
                    double totalsec = Convert.ToDouble(dgv_video.Rows[rowindex].Cells["duration"].Value.ToString().Trim()) * 60;
                    trackBar1.Maximum = (int)totalsec;
                    trackBar1.Value = 0;  //复位
                    if (trackBar1.Maximum / 10 == 0)
                        trackBar1.TickFrequency = 1;
                    else
                        trackBar1.TickFrequency = trackBar1.Maximum / 10;
                    //播放和暂停的切换
                    //vlcControl1.EndInit();

                    if (
                        dgv_video.Rows[rowindex].Cells["playonline"].Tag == null
                        ||
                        (int)dgv_video.Rows[rowindex].Cells["playonline"].Tag
                        == 0)
                    {

                        if (vlcControl1.State == MediaStates.Paused)
                            vlcControl1.Play();
                        else
                        {
                            //vlcControl1.Stop();
                            //vlcControl1.SetMedia()
                            vlcControl1.SetMedia(new Uri(video_path_str), MediaPlayer.arguments);
                            //vlcControl1.Play(new Uri(video_path_str), MediaPlayer.arguments);
                            vlcControl1.Play();
                        }

                        dgv_video.Rows[rowindex].Cells["playonline"].Value =
                            POCControlCenter.Properties.Resources.pause;
                        dgv_video.Rows[rowindex].Cells["playonline"].Tag = 1;
                        //其它的cell要停止
                        for (int i = 0; i < dgv_video.Rows.Count; i++)
                        {
                            if (i != rowindex)
                            {
                                dgv_video.Rows[i].Cells["playonline"].Value =
                                   POCControlCenter.Properties.Resources.play;
                                dgv_video.Rows[i].Cells["playonline"].Tag = 0;

                            }
                        }


                    }
                    else
                    {
                        if (vlcControl1.IsPlaying)
                            vlcControl1.Pause();
                        dgv_video.Rows[rowindex].Cells["playonline"].Value =
                            POCControlCenter.Properties.Resources.play;
                        dgv_video.Rows[rowindex].Cells["playonline"].Tag = 0;

                    }

                    //mediType
                    currMediaType = "video";
                    //改变状态

                    //if (this.usercontrol_video.vlc_media_player != IntPtr.Zero
                    //  && this.usercontrol_video.vlc_media_player != null)
                    //    MediaPlayer.Release_MediaPlayer(this.usercontrol_video.vlc_media_player);

                    //MediaPlayer.NetWork_Media_Play(this.usercontrol_video.vlc_instance,

                    //                    this.usercontrol_video.vlc_media_player, video_path_str);

                }

            }

            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
            }
        }

        private void dodgv_video_CellContentClick(DataGridViewCellEventArgs e)
        {
            try
            {
                int rowindex = e.RowIndex;
                if (rowindex == -1) return;  //点击了header
                int CIndex = e.ColumnIndex;
                if (CIndex == dgv_video.Columns["playonline"].Index)
                {
                    currMediaIndex_Video = rowindex;   //记住 
                    //右边系列属性
                    this.filename_video.Text = dgv_video.Rows[rowindex].Cells["stream"].Value.ToString();
                    this.duration_video.Text = dgv_video.Rows[rowindex].Cells["duration"].Value.ToString() + "(分钟)";
                    this.createdate_video.Text = dgv_video.Rows[rowindex].Cells["filesize"].Value.ToString() + "(MB)";
                    this.username.Text = dgv_video.Rows[rowindex].Cells["user_name"].Value.ToString();
                    this.recorddate.Text = dgv_video.Rows[rowindex].Cells["record_date_str"].Value.ToString();

                    //MessageBox.Show(dgv_video.Rows[rowindex].Cells["video_path"].Value.ToString());
                    string video_path_str = dgv_video.Rows[rowindex].Cells["video_path"].Value.ToString().Trim();
                    ///hdd/htdocs/video/record//moni_600012159_6b6_8b8-1504087251-2017-08-30-180051.flv
                    video_path_str = video_path_str.Replace("/hdd/htdocs/video/record/", "");
                    if (video_path_str.StartsWith("/"))
                        video_path_str = video_path_str.Substring(1);
                    //string HTTP_URL_PREFIX_VIDEO = "http://" + HttpAPI.VIDEOIP + "/video/record/";
                    video_path_str = HTTP_URL_PREFIX_VIDEO + video_path_str;
                    //MessageBox.Show(video_path_str);
                    //清理上次播放中的播放器占的内存等
                    //vlcControl1.Play(video_path_str,MediaPlayer.arguments);
                    trackBar1.Minimum = 0; //duration
                    double totalsec = Convert.ToDouble(dgv_video.Rows[rowindex].Cells["duration"].Value.ToString().Trim()) * 60;
                    trackBar1.Maximum = (int)totalsec;
                    trackBar1.Value = 0;  //复位
                    if (trackBar1.Maximum / 10 == 0)
                        trackBar1.TickFrequency = 1;
                    else
                        trackBar1.TickFrequency = trackBar1.Maximum / 10;
                    //播放和暂停的切换
                    //vlcControl1.EndInit();
                    

                    if (
                        dgv_video.Rows[rowindex].Cells["playonline"].Tag == null
                        ||
                        (int)dgv_video.Rows[rowindex].Cells["playonline"].Tag
                        == 0)
                    {

                        if (vlcControl1.State == MediaStates.Paused  && currMediaPlayingType.Equals("video"))
                        {
                            vlcControl1.Audio.IsMute = false;
                            vlcControl1.Audio.Volume = 100;
                            vlcControl1.Play();
                            
                        }
                        else
                        {

                            //if (vlcControl1.IsPlaying)
                            //    vlcControl1.Pause();                            

                            vlcControl1.SetMedia(new Uri(video_path_str), MediaPlayer.arguments);
                            //vlcControl1.Play(new Uri(video_path_str), MediaPlayer.arguments);
                            vlcControl1.Audio.IsMute = false;
                            vlcControl1.Audio.Volume = 100;
                            vlcControl1.Play();
                            
                        }

                        dgv_video.Rows[rowindex].Cells["playonline"].Value =
                            POCControlCenter.Properties.Resources.pause;
                        dgv_video.Rows[rowindex].Cells["playonline"].Tag = 1;
                        //其它的cell要停止
                        for (int i = 0; i < dgv_video.Rows.Count; i++)
                        {
                            if (i != rowindex)
                            {
                                dgv_video.Rows[i].Cells["playonline"].Value =
                                   POCControlCenter.Properties.Resources.play;
                                dgv_video.Rows[i].Cells["playonline"].Tag = 0;

                            }
                        }


                    }
                    else
                    {
                        
                        if (vlcControl1.IsPlaying)
                            vlcControl1.Pause();
                        dgv_video.Rows[rowindex].Cells["playonline"].Value =
                            POCControlCenter.Properties.Resources.play;
                        dgv_video.Rows[rowindex].Cells["playonline"].Tag = 0;

                    }

                    //mediType
                    currMediaType = "video";
                    //当前正在播放或暂停的类型
                    currMediaPlayingType = "video";

                    //改变状态

                    //if (this.usercontrol_video.vlc_media_player != IntPtr.Zero
                    //  && this.usercontrol_video.vlc_media_player != null)
                    //    MediaPlayer.Release_MediaPlayer(this.usercontrol_video.vlc_media_player);

                    //MediaPlayer.NetWork_Media_Play(this.usercontrol_video.vlc_instance,

                    //                    this.usercontrol_video.vlc_media_player, video_path_str);

                }

            }

            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
            }
        }

        private void dodgv_audio_CellContentClick2(int RowIndex_, int ColumnIndex_)
        {
            try
            {
                int rowindex = RowIndex_;
                if (rowindex == -1) return;  //点击了header
                int CIndex = ColumnIndex_;
                if (CIndex == dgv_audio.Columns["playonline_a"].Index)
                {
                    currMediaIndex_Audio = rowindex;   //记住 
                    //duration_a
                    this.filename_video.Text = dgv_audio.Rows[rowindex].Cells["audio_name_a"].Value.ToString();
                    this.duration_video.Text = dgv_audio.Rows[rowindex].Cells["duration_a"].Value.ToString() + "(秒)";
                    this.createdate_video.Text = dgv_audio.Rows[rowindex].Cells["filesize_a"].Value.ToString() + "(KB)";
                    this.username.Text = dgv_audio.Rows[rowindex].Cells["user_name_a"].Value.ToString();
                    this.recorddate.Text = dgv_audio.Rows[rowindex].Cells["record_date_str_a"].Value.ToString();

                    ///hdd/htdocs/voice//2017-08-11/80001196/600010169/80001196_600010169_1502435647899.amr                   

                    //MessageBox.Show(dgv_audio.Rows[rowindex].Cells["audio_path_a"].Value.ToString());
                    string audio_path_str = dgv_audio.Rows[rowindex].Cells["audio_path_a"].Value.ToString().Trim();
                    ///hdd/htdocs/video/record//moni_600012159_6b6_8b8-1504087251-2017-08-30-180051.flv
                    audio_path_str = audio_path_str.Replace("/hdd/htdocs/voice/", "");
                    if (audio_path_str.StartsWith("/"))
                        audio_path_str = audio_path_str.Substring(1);
                    //string HTTP_URL_PREFIX_AUDIO = "http://" + HttpAPI.VIDEOIP + "/voice/";
                    audio_path_str = HTTP_URL_PREFIX_AUDIO + audio_path_str;

                    //清理上次播放中的播放器占的内存等
                    trackBar1.Minimum = 0; //duration
                    double totalsec = Convert.ToDouble(dgv_audio.Rows[rowindex].Cells["duration_a"].Value.ToString().Trim());
                    trackBar1.Maximum = (int)totalsec;
                    trackBar1.Value = 0;
                    if (trackBar1.Maximum / 10 == 0)
                        trackBar1.TickFrequency = 1;
                    else
                        trackBar1.TickFrequency = trackBar1.Maximum / 10;


                    //播放和暂停的切换
                    if (
                        dgv_audio.Rows[rowindex].Cells["playonline_a"].Tag == null
                        ||
                        (int)dgv_audio.Rows[rowindex].Cells["playonline_a"].Tag
                        == 0)
                    {
                        if (vlcControl1.State == MediaStates.Paused)
                        {
                            //暂停后，再重新开始
                            vlcControl1.Play();
                            pictureBox1.Image = shengbo_file;
                            //pictureBox1.Image =
                           //     Image.FromFile(Path.Combine(System.Environment.CurrentDirectory, "shengbo_bar.gif"));
                        }
                        else
                        {
                            vlcControl1.SetMedia(new Uri(audio_path_str), MediaPlayer.arguments);
                            vlcControl1.Play();
                        }

                        dgv_audio.Rows[rowindex].Cells["playonline_a"].Value =
                            POCControlCenter.Properties.Resources.pause;
                        dgv_audio.Rows[rowindex].Cells["playonline_a"].Tag = 1;

                        //其它的cell要停止
                        for (int i = 0; i < dgv_audio.Rows.Count; i++)
                        {
                            if (i != rowindex)
                            {
                                dgv_audio.Rows[i].Cells["playonline_a"].Value =
                                   POCControlCenter.Properties.Resources.play;
                                dgv_audio.Rows[i].Cells["playonline_a"].Tag = 0;

                            }
                        }

                    }
                    else
                    {
                        if (vlcControl1.IsPlaying)
                            vlcControl1.Pause();
                        dgv_audio.Rows[rowindex].Cells["playonline_a"].Value =
                            POCControlCenter.Properties.Resources.play;
                        dgv_audio.Rows[rowindex].Cells["playonline_a"].Tag = 0;

                    }

                    currMediaType = "audio";

                }

            }

            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
            }
        }

        private void dodgv_audio_CellContentClick(DataGridViewCellEventArgs e)
        {
            //
            try
            {
                int rowindex = e.RowIndex;
                if (rowindex == -1) return;  //点击了header
                int CIndex = e.ColumnIndex;
                if (CIndex == dgv_audio.Columns["playonline_a"].Index)
                {
                    currMediaIndex_Audio = rowindex;   //记住 
                    //duration_a
                    this.filename_video.Text = dgv_audio.Rows[rowindex].Cells["audio_name_a"].Value.ToString();
                    this.duration_video.Text = dgv_audio.Rows[rowindex].Cells["duration_a"].Value.ToString() + "(秒)";
                    this.createdate_video.Text = dgv_audio.Rows[rowindex].Cells["filesize_a"].Value.ToString() + "(KB)";
                    this.username.Text = dgv_audio.Rows[rowindex].Cells["user_name_a"].Value.ToString();
                    this.recorddate.Text = dgv_audio.Rows[rowindex].Cells["record_date_str_a"].Value.ToString();

                    ///hdd/htdocs/voice//2017-08-11/80001196/600010169/80001196_600010169_1502435647899.amr                   

                    //MessageBox.Show(dgv_audio.Rows[rowindex].Cells["audio_path_a"].Value.ToString());
                    string audio_path_str = dgv_audio.Rows[rowindex].Cells["audio_path_a"].Value.ToString().Trim();
                    ///hdd/htdocs/video/record//moni_600012159_6b6_8b8-1504087251-2017-08-30-180051.flv
                    audio_path_str = audio_path_str.Replace("/hdd/htdocs/voice/", "");
                    if (audio_path_str.StartsWith("/"))
                        audio_path_str = audio_path_str.Substring(1);
                    //string HTTP_URL_PREFIX_AUDIO = "http://" + HttpAPI.VIDEOIP + "/voice/";
                    audio_path_str = HTTP_URL_PREFIX_AUDIO + audio_path_str;

                    //清理上次播放中的播放器占的内存等
                    trackBar1.Minimum = 0; //duration
                    double totalsec = Convert.ToDouble(dgv_audio.Rows[rowindex].Cells["duration_a"].Value.ToString().Trim());
                    trackBar1.Maximum = (int)totalsec;
                    trackBar1.Value = 0;
                    if (trackBar1.Maximum / 10 == 0)
                        trackBar1.TickFrequency = 1;
                    else
                        trackBar1.TickFrequency = trackBar1.Maximum / 10;

                   

                    //播放和暂停的切换
                    if (
                        dgv_audio.Rows[rowindex].Cells["playonline_a"].Tag == null
                        ||
                        (int)dgv_audio.Rows[rowindex].Cells["playonline_a"].Tag
                        == 0)
                    {
                        if (vlcControl1.State == MediaStates.Paused && currMediaPlayingType.Equals("audio"))
                        {
                            //暂停后，再重新开始                            
                            vlcControl1.Audio.IsMute = false;
                            vlcControl1.Audio.Volume = 100;
                            vlcControl1.Play();
                            pictureBox1.Image = shengbo_file;
                           // pictureBox1.Image =
                            //    Image.FromFile(Path.Combine(System.Environment.CurrentDirectory, "shengbo_bar.gif"));

                        }
                        else
                        {
                            vlcControl1.Audio.IsMute = false;
                            vlcControl1.Audio.Volume = 100;
                            vlcControl1.SetMedia(new Uri(audio_path_str), MediaPlayer.arguments);
                            vlcControl1.Play();
                        }

                        dgv_audio.Rows[rowindex].Cells["playonline_a"].Value =
                            POCControlCenter.Properties.Resources.pause;
                        dgv_audio.Rows[rowindex].Cells["playonline_a"].Tag = 1;

                        //其它的cell要停止
                        for (int i = 0; i < dgv_audio.Rows.Count; i++)
                        {
                            if (i != rowindex)
                            {
                                dgv_audio.Rows[i].Cells["playonline_a"].Value =
                                   POCControlCenter.Properties.Resources.play;
                                dgv_audio.Rows[i].Cells["playonline_a"].Tag = 0;

                            }
                        }

                    }
                    else
                    {
                        if (vlcControl1.IsPlaying)
                            vlcControl1.Pause();
                        dgv_audio.Rows[rowindex].Cells["playonline_a"].Value =
                            POCControlCenter.Properties.Resources.play;
                        dgv_audio.Rows[rowindex].Cells["playonline_a"].Tag = 0;

                    }

                    currMediaType = "audio";
                    currMediaPlayingType = "audio";


                }

            }

            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
            }

        }

        private void dgv_video_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //
            dodgv_video_CellContentClick(e);
            timer1.Enabled = true;

        }

        private void btnAudioQuery_Click(object sender, EventArgs e)
        {
            if (myAudioQueryForm == null)
                myAudioQueryForm = new AudioQueryForm();

            if (myAudioQueryForm.ShowDialog() == DialogResult.OK)
            {
                group_id_audio = "-1";
                if (myAudioQueryForm.cbGroup.SelectedIndex >= 0)
                    group_id_audio = myAudioQueryForm.cbGroup.SelectedValue.ToString();

                poc_account_audio = myAudioQueryForm.poc_account.Text.Trim();
                poc_account_like_audio = "N";
                if (myAudioQueryForm.poc_account_like.Checked)
                    poc_account_like_audio = "Y";

                cust_account_audio = myAudioQueryForm.cust_account.Text.Trim();
                cust_account_like_audio = "N";
                if (myAudioQueryForm.cust_account_like.Checked)
                    cust_account_like_audio = "Y";

                user_name2_audio = myAudioQueryForm.user_name.Text.Trim();

                user_name_like_audio = "N";
                if (myAudioQueryForm.user_name_like.Checked)
                    user_name_like_audio = "Y";

                record_date_from_audio = myAudioQueryForm.record_date_from.Text;
                record_date_to_audio = myAudioQueryForm.record_date_to.Text;

                audio_duration_from = myAudioQueryForm.audio_duration_from.Text.Trim();
                audio_duration_to = myAudioQueryForm.audio_duration_to.Text.Trim();

                this.page_size_audio_query = myAudioQueryForm.cbPageSize.Text;

                PageQueryTotal pagetotal =
                HttpAPI.queryAudioRecTotalInfo(poc_account_audio, poc_account_like_audio, cust_account_audio,
                    cust_account_like_audio,
                    user_name2_audio, user_name_like_audio, record_date_from_audio, record_date_to_audio,
                    audio_duration_from, audio_duration_to, page_size_audio_query, Convert.ToInt32(group_id_audio));

                //MessageBox.Show(pagetotal.count.ToString());
                //如果pagetotal.count>0 , 还要进行下一步分页的第一页的获得
                //还要考虑时区差的加入，所有recorddate要处理
                if (pagetotal.count > 0)
                {
                    this.count_audio = pagetotal.count;
                    this.page_size_audio = Convert.ToInt32(page_size_audio_query);
                    this.page_num_audio = pagetotal.page_num;
                    this.page_index_audio = 1;
                    this.tb_audiopageindex.Text = "1";
                    this.audio_pagetag.Text = "1/" + this.page_num_audio.ToString();
                    this.audio_desc.Text = WinFormsStringResource.QueryTotal + count_audio.ToString() + WinFormsStringResource.QueryRecDESC
                        + "," + WinFormsStringResource.QueryPerPage + this.page_size_audio.ToString() +
                        WinFormsStringResource.QueryRecDESC + "," + WinFormsStringResource.QueryTotal
                        + this.page_num_audio.ToString() + WinFormsStringResource.QueryPage;
                    //查询第一页                    
                    queryByPageIndex_Audio(1, poc_account_audio, poc_account_like_audio, cust_account_audio,
                        cust_account_like_audio, user_name2_audio, user_name_like_audio,
                        record_date_from_audio, record_date_to_audio, audio_duration_from, audio_duration_to,
                        page_size_audio_query, group_id_audio);

                }
                else
                {
                    //清除并显示没有查到数据
                    this.BAudioRec.Clear();
                    this.count_audio = 0;
                    this.page_num_audio = 0;
                    this.audio_pagetag.Text = "";
                    this.audio_desc.Text = "";
                    MessageBox.Show(WinFormsStringResource.NoData);
                }

            }
        }

        private void tb_audiopageindex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //判断内容
                if (tb_audiopageindex.Text.Trim() == "") return;
                int pageno = Convert.ToInt32(tb_audiopageindex.Text.Trim());
                if (count_audio > 0 && pageno <= this.page_num_audio
                    && pageno >= 1)
                {
                    queryByPageIndex_Audio(pageno, poc_account_audio, poc_account_like_audio,
                        cust_account_audio, cust_account_like_audio,
                user_name2_audio, user_name_like_audio, record_date_from_audio, record_date_to_audio,
                audio_duration_from, audio_duration_to, page_size_audio_query, group_id_audio);

                    this.audio_pagetag.Text = pageno.ToString() + "/" + this.page_num_audio.ToString();

                }
                else
                {
                    MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
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

        private void tbFirst_Audio_Click(object sender, EventArgs e)
        {
            int pageno = 1;
            if (this.count_audio > 0 && pageno <= this.page_num_audio
                    && pageno >= 1)
            {

                queryByPageIndex_Audio(pageno, poc_account_audio, poc_account_like_audio,
                    cust_account_audio, cust_account_like_audio,
                user_name2_audio, user_name_like_audio, record_date_from_audio, record_date_to_audio,
                audio_duration_from, audio_duration_to, page_size_audio_query, group_id_audio);

                this.audio_pagetag.Text = pageno.ToString() + "/" + this.page_num_audio.ToString();
                this.page_index_audio = 1;
                this.tb_audiopageindex.Text = "1";

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }

        }

        private void tbPrev_Audio_Click(object sender, EventArgs e)
        {
            if (this.count_audio == 0) return;
            if (this.page_num_audio == 0) return;
            if (page_index_audio == 1)
            {
                MessageBox.Show(WinFormsStringResource.PageNoFirstAlert);
                return;
            }
            int pageno = this.page_index_audio - 1;
            if (this.count_audio > 0 && pageno <= this.page_num_audio
                   && pageno >= 1)
            {

                queryByPageIndex_Audio(pageno, poc_account_audio, poc_account_like_audio, cust_account_audio,
                    cust_account_like_audio,
                user_name2_audio, user_name_like_audio, record_date_from_audio, record_date_to_audio,

                audio_duration_from, audio_duration_to, this.page_size_audio_query, group_id_audio);

                this.audio_pagetag.Text = pageno.ToString() + "/" + this.page_num_audio.ToString();
                this.page_index_audio = pageno;
                this.tb_audiopageindex.Text = pageno.ToString();

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }

        }

        private void tbNext_Audio_Click(object sender, EventArgs e)
        {
            if (this.count_audio == 0) return;
            if (this.page_num_audio == 0) return;

            if (page_index_audio == this.page_num_audio)
            {
                MessageBox.Show(WinFormsStringResource.PageNoLastAlert);
                return;
            }
            int pageno = this.page_index_audio + 1;
            if (this.count_audio > 0 && pageno <= this.page_num_audio
                  && pageno >= 1)
            {

                queryByPageIndex_Audio(pageno, poc_account_audio, poc_account_like_audio, cust_account_audio,
                    cust_account_like_audio,
                user_name2_audio, user_name_like_audio, record_date_from_audio, record_date_to_audio,

                audio_duration_from, audio_duration_to, this.page_size_audio_query, group_id_audio);

                this.audio_pagetag.Text = pageno.ToString() + "/" + this.page_num_audio.ToString();
                this.page_index_audio = pageno;
                this.tb_audiopageindex.Text = pageno.ToString();

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }
        }

        private void tbLast_Audio_Click(object sender, EventArgs e)
        {
            if (this.count_audio == 0) return;
            if (this.page_num_audio == 0) return;
            int pageno = page_num_audio;
            if (this.count_audio > 0 && pageno <= this.page_num_audio
                    && pageno >= 1)
            {

                queryByPageIndex_Audio(pageno, poc_account_audio, poc_account_like_audio, cust_account_audio,
                    cust_account_like_audio,
                user_name2_audio, user_name_like_audio, record_date_from_audio, record_date_to_audio,

                audio_duration_from, audio_duration_to, this.page_size_audio_query, group_id_audio);

                this.audio_pagetag.Text = pageno.ToString() + "/" + this.page_num_audio.ToString();
                this.page_index_audio = this.page_num_audio;
                this.tb_audiopageindex.Text = page_num_audio.ToString();

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }
        }

        private void btnSessionQuery_Click(object sender, EventArgs e)
        {
            if (mySessionQueryForm == null)
                mySessionQueryForm = new SessionQueryForm();

            if (mySessionQueryForm.ShowDialog() == DialogResult.OK)
            {
                file_type_session = "";
                if (mySessionQueryForm.cbFileType.SelectedIndex >= 0)
                    file_type_session = mySessionQueryForm.cbFileType.SelectedValue.ToString();

                group_id_session = "-1";
                if (mySessionQueryForm.cbGroup.SelectedIndex >= 0)
                    group_id_session = mySessionQueryForm.cbGroup.SelectedValue.ToString();

                poc_account_session = mySessionQueryForm.poc_account.Text.Trim();
                poc_account_like_session = "N";
                if (mySessionQueryForm.poc_account_like.Checked)
                    poc_account_like_session = "Y";

                cust_account_session = mySessionQueryForm.cust_account.Text.Trim();
                cust_account_like_session = "N";
                if (mySessionQueryForm.cust_account_like.Checked)
                    cust_account_like_session = "Y";

                user_name2_session = mySessionQueryForm.user_name.Text.Trim();

                user_name_like_session = "N";
                if (mySessionQueryForm.user_name_like.Checked)
                    user_name_like_session = "Y";

                record_date_from_session = mySessionQueryForm.record_date_from.Text;
                record_date_to_session = mySessionQueryForm.record_date_to.Text;

                this.page_size_session_query = mySessionQueryForm.cbPageSize.Text;

                PageQueryTotal pagetotal =
                HttpAPI.queryPOCSessionRecTotalInfo(file_type_session, poc_account_session,
                    poc_account_like_session, cust_account_session,
                    cust_account_like_session,
                    user_name2_session, user_name_like_session, record_date_from_session, record_date_to_session,
                    page_size_session_query, Convert.ToInt32(group_id_session));

                //MessageBox.Show(pagetotal.count.ToString());
                //如果pagetotal.count>0 , 还要进行下一步分页的第一页的获得
                //还要考虑时区差的加入，所有recorddate要处理
                if (pagetotal.count > 0)
                {
                    this.count_session = pagetotal.count;
                    this.page_size_session = Convert.ToInt32(page_size_session_query);
                    this.page_num_session = pagetotal.page_num;
                    this.page_index_session = 1;
                    this.tb_sessionpageindex.Text = "1";
                    this.session_pagetag.Text = "1/" + this.page_num_session.ToString();
                    this.session_desc.Text = WinFormsStringResource.QueryTotal + count_session.ToString() + WinFormsStringResource.QueryRecDESC
                        + "," + WinFormsStringResource.QueryPerPage + this.page_size_session.ToString() +
                        WinFormsStringResource.QueryRecDESC + "," + WinFormsStringResource.QueryTotal
                        + this.page_num_session.ToString() + WinFormsStringResource.QueryPage;
                    //查询第一页                    
                    queryByPageIndex_Session(1, file_type_session, poc_account_session,
                        poc_account_like_session, cust_account_session,
                        cust_account_like_session, user_name2_session, user_name_like_session,
                        record_date_from_session, record_date_to_session, 
                        page_size_session_query, group_id_session);

                }
                else
                {
                    //清除并显示没有查到数据
                    this.BPOCSessionRec.Clear();
                    this.count_session = 0;
                    this.page_num_session = 0;
                    this.session_pagetag.Text = "";
                    this.session_desc.Text = "";
                    MessageBox.Show(WinFormsStringResource.NoData);
                }

            }
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
            BindingManager_sessionbase.PositionChanged += null;
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

            BindingManager_sessionbase.PositionChanged += BindingManager_sessionbase_PositionChanged;

        }

        private void BindingManager_sessionbase_PositionChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
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

        private void tb_sessionpageindex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //判断内容
                if (tb_sessionpageindex.Text.Trim() == "") return;
                int pageno = Convert.ToInt32(tb_sessionpageindex.Text.Trim());
                if (count_session > 0 && pageno <= this.page_num_session
                    && pageno >= 1)
                {
                    queryByPageIndex_Session(pageno,file_type_session,poc_account_session,
                        poc_account_like_session,
                        cust_account_session, cust_account_like_session,
                user_name2_session, user_name_like_session, record_date_from_session,
                record_date_to_session,
                page_size_session_query, group_id_session);

                    this.session_pagetag.Text = pageno.ToString() + "/" + this.page_num_session.ToString();

                }
                else
                {
                    MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
                }

            }
        }

        private void tbFirst_Session_Click(object sender, EventArgs e)
        {
            int pageno = 1;
            if (this.count_session > 0 && pageno <= this.page_num_session
                    && pageno >= 1)
            {

                queryByPageIndex_Session(pageno,file_type_session, poc_account_session, poc_account_like_session,
                    cust_account_session, cust_account_like_session,
                user_name2_session, user_name_like_session, record_date_from_session, record_date_to_session,
                page_size_session_query, group_id_session);

                this.session_pagetag.Text = pageno.ToString() + "/" + this.page_num_session.ToString();
                this.page_index_session = 1;
                this.tb_sessionpageindex.Text = "1";

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }
        }

        private void tbPrev_Session_Click(object sender, EventArgs e)
        {
            if (this.count_session == 0) return;
            if (this.page_num_session == 0) return;
            if (page_index_session == 1)
            {
                MessageBox.Show(WinFormsStringResource.PageNoFirstAlert);
                return;
            }
            int pageno = this.page_index_session - 1;
            if (this.count_session > 0 && pageno <= this.page_num_session
                   && pageno >= 1)
            {

                queryByPageIndex_Session(pageno, file_type_session, poc_account_session, poc_account_like_session,
                    cust_account_session, cust_account_like_session,
                user_name2_session, user_name_like_session, record_date_from_session, record_date_to_session,
                page_size_session_query, group_id_session);

                this.session_pagetag.Text = pageno.ToString() + "/" + this.page_num_session.ToString();
                this.page_index_session = pageno;
                this.tb_sessionpageindex.Text = pageno.ToString();

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }
        }

        private void tbNext_Session_Click(object sender, EventArgs e)
        {
            if (this.count_session == 0) return;
            if (this.page_num_session == 0) return;

            if (page_index_session == this.page_num_session)
            {
                MessageBox.Show(WinFormsStringResource.PageNoLastAlert);
                return;
            }
            int pageno = this.page_index_session + 1;
            if (this.count_session > 0 && pageno <= this.page_num_session
                  && pageno >= 1)
            {

                queryByPageIndex_Session(pageno, file_type_session, poc_account_session, poc_account_like_session,
                    cust_account_session, cust_account_like_session,
                user_name2_session, user_name_like_session, record_date_from_session, record_date_to_session,
                page_size_session_query, group_id_session);

                this.session_pagetag.Text = pageno.ToString() + "/" + this.page_num_session.ToString();
                this.page_index_session = pageno;
                this.tb_sessionpageindex.Text = pageno.ToString();

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }
        }

        private void tbLast_Session_Click(object sender, EventArgs e)
        {
            if (this.count_session == 0) return;
            if (this.page_num_session == 0) return;
            int pageno = page_num_session;
            if (this.count_session > 0 && pageno <= this.page_num_session
                    && pageno >= 1)
            {

                queryByPageIndex_Session(pageno, file_type_session, poc_account_session, poc_account_like_session,
                    cust_account_session, cust_account_like_session,
                user_name2_session, user_name_like_session, record_date_from_session, record_date_to_session,
                page_size_session_query, group_id_session);

                this.session_pagetag.Text = pageno.ToString() + "/" + this.page_num_session.ToString();
                this.page_index_session = this.page_num_session;
                this.tb_sessionpageindex.Text = page_num_session.ToString();

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }

        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //会话中应该隐藏vlc
            if (tabControl1.SelectedIndex == 2)
                panel3.Visible = false;
            else
            {
                panel3.Visible = true;
                if (tabControl1.SelectedIndex == 0)
                {
                    vlcControl1.BringToFront();
                    currMediaType = "video";

                }
                else if  (tabControl1.SelectedIndex == 1)
                {
                    pictureBox1.BringToFront();
                    if (!vlcControl1.IsPlaying)
                    {
                        pictureBox1.Image = null;
                    } else
                    {
                        //如果有视频则不管


                    }
                    currMediaType = "audio";
                }

            }
        }

        private void RecQueryForm_FormClosed(object sender, FormClosedEventArgs e)
        {             
            try
            {      
                if (vlcControl1!=null)
                    vlcControl1.Dispose();

            } catch (MissingMethodException me)
            {
                MessageBox.Show(me.Message);
            }
        }

        private void dgv_audio_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dodgv_audio_CellContentClick(e);
            timer1.Enabled = true;
        }

        private void dgv_session_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //
            try
            {
                int rowindex = e.RowIndex;
                if (rowindex == -1) return;  //点击了header
                int CIndex = e.ColumnIndex;
                if (CIndex == dgv_session.Columns["download_s"].Index)
                {
                    //
                    ///hdd/htdocs/voice//2017-08-11/80001196/600010169/80001196_600010169_1502435647899.amr                   

                    //MessageBox.Show(dgv_session.Rows[rowindex].Cells["file_key"].Value.ToString());
                    string file_key_str = dgv_session.Rows[rowindex].Cells["file_key"].Value.ToString().Trim();
                    string file_name_str = dgv_session.Rows[rowindex].Cells["file_name"].Value.ToString().Trim();
                    string user_id = dgv_session.Rows[rowindex].Cells["user_id"].Value.ToString().Trim();

                    String savedir = Filetype.GetFileRecvDir(Filetype.TYPE_CHAT_FILE);                    

                    String filepath;
                    
                    filepath = Path.Combine(savedir, file_name_str);                   

                    String filekey = file_key_str;
                    String downurl = HttpAPI.DownloadURL + "?userid=" + user_id + "&filename=" + filekey;
                                                          

                    CookieContainer cookies = new CookieContainer();                    
                    int outdata = HttpAPI.DownLoadFileEx_Jimmy(filepath, downurl, filekey, cookies);
                    if (outdata == 0)
                    {
                        MessageBox.Show("成功保存于"+ filepath);
                    }

                }

            }

            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
            }

        }

        private void vlcControl1_EndReached(object sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            //
            MessageBox.Show("end");
        }

        private void vlcControl1_Stopped(object sender, Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs e)
        {
            //
            MessageBox.Show("stopped");
        }

        private void RecQueryForm_Load(object sender, EventArgs e)
        {
            //load 是每次打开对话框都触发
            //MessageBox.Show("RecQueryForm_Load");
            InitRunPara();
            vlcControl1 = new Vlc.DotNet.Forms.VlcControl();

            shengbo_file=  Image.FromFile(Path.Combine(System.Environment.CurrentDirectory, "shengbo_bar.gif"));

            foreach (Control control in panelContainer.Controls)
            {
                if (control is VlcControl)
                    panelContainer.Controls.Remove(control);
            }
            
            panelContainer.Controls.Add(vlcControl1);
            vlcControl1.Dock = DockStyle.Fill;
            vlcControl1.BackColor = Color.Gray;
            vlcControl1.VlcLibDirectoryNeeded += VlcControl1_VlcLibDirectoryNeeded;

            
            vlcControl1.Opening += VlcControl1_Opening;
            vlcControl1.EndReached += VlcControl1_EndReached;
            vlcControl1.EncounteredError += VlcControl1_EncounteredError;
            vlcControl1.Playing += VlcControl1_Playing;
            vlcControl1.TimeChanged += VlcControl1_TimeChanged;
            vlcControl1.Paused += VlcControl1_Paused;

            vlcControl1.EndInit();
            //
            vlcControl1.BringToFront();

        }

        private void MediaState_paused()
        {
            if (this.currMediaType.Equals("video") && this.currTrackBarIndex_Video > 0)
            {
                this.trackBar1.Value = currTrackBarIndex_Video;

            }
            else if (this.currMediaType.Equals("audio") && this.currTrackBarIndex_Audio > 0)
            {
                this.trackBar1.Value = currTrackBarIndex_Audio;
                this.pictureBox1.Image = null;
            }


        }

        private void VlcControl1_Paused(object sender, Vlc.DotNet.Core.VlcMediaPlayerPausedEventArgs e)
        {
            if (this.media_state.InvokeRequired == false)
            {
                MediaState_paused();
            }
            else
            {
                OperByVLCEventDelegate DMSGD = new OperByVLCEventDelegate(MediaState_paused);
                this.media_state.Invoke(DMSGD);
            }            

        }

        private void VlcControl1_TimeChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerTimeChangedEventArgs e)
        {
            if (this.media_state.InvokeRequired == false)
            {
                execMediaState_timechange(e.NewTime);
            }
            else
            {
                UpdateCurrTimeByVLCEventDelegate DMSGD = new UpdateCurrTimeByVLCEventDelegate(execMediaState_timechange);
                this.media_state.Invoke(DMSGD,new object[] { e.NewTime });
            }
        }

        private void execMediaState_timechange(long newtime)
        {
            //media_state.Text = newtime.ToString();
            if (!enterSeek)
            {
                if ((Int32)newtime / 1000> trackBar1.Maximum )
                    trackBar1.Value = trackBar1.Maximum;
                else
                    trackBar1.Value = (Int32)newtime / 1000;
            }

            if (this.currMediaType.Equals("video"))
                this.currTrackBarIndex_Video = trackBar1.Value;
            else
                this.currTrackBarIndex_Audio = trackBar1.Value;

        }

        private void VlcControl1_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            if (this.media_state.InvokeRequired == false)
            {
                MediaState_playing();
            }
            else
            {
                OperByVLCEventDelegate DMSGD = new OperByVLCEventDelegate(MediaState_playing);
                this.media_state.Invoke(DMSGD);
            }
        }

        private void RecQueryForm_Shown(object sender, EventArgs e)
        {
            //
            //MessageBox.Show("RecQueryForm_Shown");
        }

        private delegate void OperByVLCEventDelegate();
        private delegate void UpdateCurrTimeByVLCEventDelegate(long newtime);

        private void VlcControl1_Opening(object sender, Vlc.DotNet.Core.VlcMediaPlayerOpeningEventArgs e)
        {
            if (this.media_state.InvokeRequired == false)
            {
                execMediaState_opened();
            }
            else
            {
                OperByVLCEventDelegate DMSGD = new OperByVLCEventDelegate(execMediaState_opened);
                this.media_state.Invoke(DMSGD);
            }

            
        }

        private void execMediaState_opened()
        {
            trackBar1.Enabled = false;
            media_state.Text =
                            WinFormsStringResource.MediaState_opened;
        }
        private void MediaState_errored()
        {
            trackBar1.Enabled = false;
            media_state.Text =
               WinFormsStringResource.MediaState_errored;
            //
            vlcControl1.BackgroundImage = null;


        }

        private void MediaState_ended_autoinvoke()
        {
            
        }
        private void MediaState_ended()
        {
            media_state.Text =
                WinFormsStringResource.MediaState_ended;
            if (currMediaType.Equals("audio"))
            {
                pictureBox1.Image = null;
                if (this.currMediaIndex_Audio >= 0)
                {
                    dgv_audio.Rows[currMediaIndex_Audio].Cells["playonline_a"].Value =
                    POCControlCenter.Properties.Resources.play;
                    dgv_audio.Rows[currMediaIndex_Audio].Cells["playonline_a"].Tag = 0;
                }                

            }
            else if (currMediaType.Equals("video"))
            {
                //当前的index要设为停止状态的图标
                if (this.currMediaIndex_Video >= 0)
                {
                    dgv_video.Rows[currMediaIndex_Video].Cells["playonline"].Value =
                    POCControlCenter.Properties.Resources.play;
                    dgv_video.Rows[currMediaIndex_Video].Cells["playonline"].Tag = 0;
                }             
                               

            }
        }

        private void MediaState_playing()
        {
            trackBar1.Enabled = true;
            media_state.Text =
                WinFormsStringResource.MediaState_playing;
            if (currMediaType.Equals("audio"))
            {
                pictureBox1.Image = shengbo_file;
                //pictureBox1.Image =
                 //   Image.FromFile(Path.Combine(System.Environment.CurrentDirectory, "shengbo_bar.gif"));
                
            }
        }

        private void VlcControl1_EncounteredError(object sender, Vlc.DotNet.Core.VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            if (this.media_state.InvokeRequired == false)
            {
                MediaState_errored();
            }
            else
            {
                OperByVLCEventDelegate DMSGD = new OperByVLCEventDelegate(MediaState_errored);
                this.media_state.Invoke(DMSGD);
            }
        }

        private void VlcControl1_EndReached(object sender, Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs e)
        {
            if (this.media_state.InvokeRequired == false)
            {
                MediaState_ended();
            }
            else
            {
                OperByVLCEventDelegate DMSGD = new OperByVLCEventDelegate(MediaState_ended);
                this.media_state.Invoke(DMSGD);
            }                   
                       

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //
            if (vlcControl1.IsPlaying)
            {
                vlcControl1.Time = trackBar1.Value * 1000;
            }
            
        }

        private void trackBar1_Enter(object sender, EventArgs e)
        {
            //
            enterSeek = true;
        }

        private void trackBar1_Leave(object sender, EventArgs e)
        {
            enterSeek = false;
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            enterSeek = false;
        }

        private void dgv_video_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //动态根据值显示相应的图片

        }

        private void dgv_video_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            //离开当前cell时,并且选择的是下个播放列的cell时, 自动设为停止播放
            //if (e.ColumnIndex == dgv_video.Columns["playonline"].Index
            //    && e.RowIndex!=this.currMediaIndex_Video
            //    )
            //{
            //    if (vlcControl1.IsPlaying)
            //        vlcControl1.Pause();
            //    dgv_video.Rows[e.RowIndex].Cells["playonline"].Value =
            //        POCControlCenter.Properties.Resources.play;
            //    dgv_video.Rows[e.RowIndex].Cells["playonline"].Tag = 0;
            //}

        }

        private void dgv_audio_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            //离开当前cell时,并且选择的是下个播放列的cell时, 自动设为停止播放
            //if (e.ColumnIndex == dgv_audio.Columns["playonline_a"].Index)
            //{
            //    if (vlcControl1.IsPlaying)
            //        vlcControl1.Pause();
            //    dgv_audio.Rows[e.RowIndex].Cells["playonline_a"].Value =
            //        POCControlCenter.Properties.Resources.play;
            //    dgv_audio.Rows[e.RowIndex].Cells["playonline_a"].Tag = 0;
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if (currMediaType.Equals("video"))
            {
                if (cb_video_autoplay.Checked && vlcControl1.State==MediaStates.Ended)
                {
                    //顺序播放
                    if (this.currMediaIndex_Video < this.dgv_video.Rows.Count - 1)
                    {
                        dgv_video.Rows[this.currMediaIndex_Video].Cells["playonline"].Selected = false;

                        this.currMediaIndex_Video = this.currMediaIndex_Video + 1;
                        dgv_video.Rows[this.currMediaIndex_Video].Cells["playonline"].Selected = true;
                        
                        dodgv_video_CellContentClick_2(this.currMediaIndex_Video, dgv_video.Columns["playonline"].Index);                        
                        
                    }

                }
            } else if (currMediaType.Equals("audio"))
            {
                if (cb_audio_autoplay.Checked && vlcControl1.State == MediaStates.Ended)
                {
                    //顺序播放
                    if (this.currMediaIndex_Audio < this.dgv_audio.Rows.Count - 1)
                    {
                        //先取消上一行的
                        dgv_audio.Rows[this.currMediaIndex_Audio].Cells["playonline_a"].Selected = false;

                        this.currMediaIndex_Audio = this.currMediaIndex_Audio + 1;
                        dgv_audio.Rows[this.currMediaIndex_Audio].Cells["playonline_a"].Selected = true;

                        dodgv_audio_CellContentClick2(this.currMediaIndex_Audio,dgv_audio.Columns["playonline_a"].Index);
                        

                    }

                }
            }
        }

        private void RecQueryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            if (vlcControl1!=null &&  (vlcControl1.State==MediaStates.Opening
                || vlcControl1.State == MediaStates.Playing
                ))
            {
                //vlcControl1.Stop();
                MessageBox.Show("有媒体在播放，请先关闭它");
                e.Cancel = true;
            }
            else e.Cancel = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //video多个下载
            List<string> filepaths = new List<string>();
            int oper_num = 0;
            string video_path_str = "";
            for (int i = 0; i < dgv_video.Rows.Count; i++)
            {
                string oper = "0";
                if (dgv_video.Rows[i].Cells[0].Value!=null)
                    oper = dgv_video.Rows[i].Cells[0].Value.ToString().Trim();

                if (oper.Equals("1"))
                {
                    oper_num += 1;
                    video_path_str = dgv_video.Rows[i].Cells["video_path"].Value.ToString().Trim();
                    ///hdd/htdocs/video/record//moni_600012159_6b6_8b8-1504087251-2017-08-30-180051.flv
                    video_path_str = video_path_str.Replace("/hdd/htdocs/video/record/", "");
                    if (video_path_str.StartsWith("/"))
                        video_path_str = video_path_str.Substring(1);
                    //string HTTP_URL_PREFIX_VIDEO = "http://" + HttpAPI.VIDEOIP + "/video/record/";
                    video_path_str = HTTP_URL_PREFIX_VIDEO + video_path_str;
                    filepaths.Add(video_path_str);
                }

            }
            if (oper_num == 0)
            {
                MessageBox.Show("请至少选择一行记录");
                return;
            } else
            {
                DownFilesForm downFrm = new DownFilesForm("video", this.FullDownloadVideoPath);
                downFrm.setDownFiles(filepaths);
                downFrm.ShowDialog();

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //audio多个下载
            List<string> filepaths = new List<string>();
            int oper_num = 0;
            string audio_path_str = "";
            for (int i = 0; i < dgv_audio.Rows.Count; i++)
            {
                string oper = "0";
                if (dgv_audio.Rows[i].Cells[0].Value != null)
                    oper = dgv_audio.Rows[i].Cells[0].Value.ToString().Trim();

                if (oper.Equals("1"))
                {
                    oper_num += 1;
                    audio_path_str = dgv_audio.Rows[i].Cells["audio_path_a"].Value.ToString().Trim();
                    
                    audio_path_str = audio_path_str.Replace("/hdd/htdocs/voice/", "");
                    if (audio_path_str.StartsWith("/"))
                        audio_path_str = audio_path_str.Substring(1);
                    
                    audio_path_str = HTTP_URL_PREFIX_AUDIO + audio_path_str;                    
                    
                    filepaths.Add(audio_path_str);
                }

            }
            if (oper_num == 0)
            {
                MessageBox.Show("请至少选择一行记录");
                return;
            }
            else
            {
                DownFilesForm downFrm = new DownFilesForm("audio", this.FullDownloadAudioPath);
                downFrm.setDownFiles(filepaths);
                downFrm.ShowDialog();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }

}

