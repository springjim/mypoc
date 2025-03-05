using POCControlCenter.DataEntity;
using POCControlCenter.Service;
using POCControlCenter.Service.Entity;
using POCControlCenter.Service.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.Record
{
    public partial class MyRecordForm : Form
    {
        //2019.07.20
        private string FullReplacePrex = "";  //用于web寻址的替代
        //2017.10.16
        private string FullDownloadVideoPath = "";
        private string FullDownloadAudioPath = "";
        private string FullDownloadSessionPath = "";
        //2017.8.16 用户所在时区与服务器所在时区间的差,单位小时
        private int ZoneInterval_UserServer = 0;
        private DateTimeSelForm dateForm;        
      


        //警告记录查询 DapperSosLog      
        private int PAGE_NUM_SosLog = 0;
        private int PAGE_PAGESIZE_SosLog = 10;        
        private int PAGE_INDEX_SosLog = 0;  //当前页码
        private int COUNT_SosLog = 0;  //记录数量

        //视频录像/语音记录/会话记录查询 (沿用web api来实现) 
        private VideoQueryForm myVideoQueryForm = null;
        private AudioQueryForm myAudioQueryForm = null;       


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
        //2022.12.30 改成了 腾讯云 COS方式
        ///liveboard/296/aeb45a63e5411599ec20b3b316bff630_100024-live_0.mp4
        string HTTP_URL_PREFIX_VIDEO = "https://jimcos-1253868323.cos.ap-guangzhou.myqcloud.com/";

        ///hdd/htdocs/voice//2017-08-11/80001196/600010169/80001196_600010169_1502435647899.amr
        string HTTP_URL_PREFIX_AUDIO = "http://" + HttpAPI.VIDEOIP + ":" + HttpAPI.VIDEO_XAMPP_Port + "/voice/";

        //群组会话中的文件网址构造
        //这里有个bug, 北京服务器也开启了语音服务
        string HTTP_URL_PREFIX_SESSION = "http://" + HttpAPI.VIDEOIP + "/voice/";

        //private MediaPlayForm meidaPlayForm;
        //*******************************************************
        

        public MyRecordForm(string FullDownloadVideoPath,string ReplacePrex)
        {
            InitializeComponent();
            this.FullDownloadVideoPath = FullDownloadVideoPath;
            this.FullReplacePrex = ReplacePrex;
            //dgv_video,dgv_audio,session
            this.BVideoRec = new BindingCollection<VideoRec>(IVideoRec);
            this.dgv_video.AutoGenerateColumns = false;
            this.dgv_video.DataSource = BVideoRec;
            BVideoRec.Clear();
            //
            this.BAudioRec = new BindingCollection<AudioRec>(IAudioRec);
            this.dgv_audio.AutoGenerateColumns = false;
            this.dgv_audio.DataSource = BAudioRec;
            BAudioRec.Clear();
        }
        

        private void button26_Click(object sender, EventArgs e)
        {
            if (dateForm == null)
                dateForm = new DateTimeSelForm();

            if (textBox5.Text.Trim() != "")
            {
                dateForm.monthCalendar.SetDate(DateTime.ParseExact(textBox5.Text.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture));
            }

            if (dateForm.ShowDialog() == DialogResult.OK)
            {
                textBox5.Text = dateForm.monthCalendar.SelectionStart.ToString("yyyy-MM-dd");
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (dateForm == null)
                dateForm = new DateTimeSelForm();

            if (textBox4.Text.Trim() != "")
            {
                dateForm.monthCalendar.SetDate(DateTime.ParseExact(textBox4.Text.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture));
            }

            if (dateForm.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = dateForm.monthCalendar.SelectionStart.ToString("yyyy-MM-dd");
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            int limit = 10;
            string startDate = textBox5.Text.Trim();
            string endDate = textBox4.Text.Trim();

            if (cb_soslog.SelectedIndex == -1)
            {
                cb_soslog.SelectedIndex = cb_soslog.Items.IndexOf("10");
                PAGE_PAGESIZE_SosLog = 10;
            }
            else
                PAGE_PAGESIZE_SosLog = Convert.ToInt32(cb_soslog.Text);
              

            PageResponseBase<SoslogDto>  resp = PocClient.querySosLog(LocalSharedData.CURRENTUser.cmpid,1, startDate, endDate, PAGE_PAGESIZE_SosLog);

            if (resp.code == 0)
            {
                this.dataGridViewSoslog.AutoGenerateColumns = false;
                if (resp.page!=null && resp.page.list!=null && resp.page.list.Count > 0)
                {
                    dataGridViewSoslog.DataSource = resp.page.list;
                    this.soslog_pagetag.Visible = true;
                    PAGE_PAGESIZE_SosLog = limit;
                    int totalcount = 0;
                    totalcount = resp.page.totalCount;
                    if (totalcount == 0)
                    {
                        PAGE_NUM_SosLog = 0;
                        soslog_desc.Text = "";
                        soslog_pagetag.Text = "";
                    }
                    else
                    {
                        //设置 page_num_hr
                        COUNT_SosLog = totalcount;
                        PAGE_NUM_SosLog = totalcount / PAGE_PAGESIZE_SosLog + (totalcount % PAGE_PAGESIZE_SosLog > 0 ? 1 : 0);
                        this.soslog_pagetag.Text = "1/" + this.PAGE_NUM_SosLog.ToString();
                        this.soslog_desc.Text = WinFormsStringResource.QueryTotal + totalcount.ToString() + WinFormsStringResource.QueryRecDESC
                            + "," + WinFormsStringResource.QueryPerPage + this.PAGE_PAGESIZE_SosLog.ToString() +
                            WinFormsStringResource.QueryRecDESC + "," + WinFormsStringResource.QueryTotal
                            + this.PAGE_NUM_SosLog.ToString() + WinFormsStringResource.QueryPage;

                        tb_soslogpageindex.Text = "1";
                        PAGE_INDEX_SosLog = 1;

                    }

                } else
                {
                    //
                    MessageBox.Show(WinFormsStringResource.NoData);
                }
                
            }
            

            /*
            this.soslog_pagetag.Visible = true;

            this.PageModel_SosLog = new PageModel();
            this.PageModel_SosLog.IsCount = true;
            
            this.PageModel_SosLog.Tables = " ptt_soslog  a  inner  join  ptt_user  b on a.user_id=b.user_id  ";
            this.PageModel_SosLog.PageIndex = 1;


            if (cb_soslog.SelectedIndex == -1)
            {
                cb_soslog.SelectedIndex = cb_soslog.Items.IndexOf("10");
                this.PageModel_SosLog.PageSize = 10;
            }
            else
                this.PageModel_SosLog.PageSize = Convert.ToInt32(cb_soslog.Text);
            //
            PAGE_PAGESIZE_SosLog = this.PageModel_SosLog.PageSize;
            this.PageModel_SosLog.PKey = "a.id";
            this.PageModel_SosLog.Sort = "a.recdate desc ";
            StringBuilder sb = new StringBuilder("a.user_id,b.user_name,FROM_UNIXTIME(a.recdate, '%Y-%m-%d %H:%i:%S') as recdate_str,");
            sb.Append(" a.latitude,a.longitude,a.gps_addr, a.voice_filename,  (case a.alarm_level when 1 then  'SOS' when  2 then '其它'  else '其它' end ) as alarm_level_str  ");

            this.PageModel_SosLog.Fields = sb.ToString();
            this.PageModel_SosLog.Filter = " b.cmpid=" + LocalSharedData.CURRENTUser.cmpid;


            if (cbSOSType.SelectedIndex == 0)
            {
                this.PageModel_SosLog.Filter = this.PageModel_SosLog.Filter
                    + " and  a.alarm_level=1 ";
            }
            else if (cbSOSType.SelectedIndex == 1)
            {
                this.PageModel_SosLog.Filter = this.PageModel_SosLog.Filter
                    + " and  a.alarm_level=2  ";
            }
            //
            if (!textBox5.Text.Trim().Equals(""))
            {
                this.PageModel_SosLog.Filter = this.PageModel_SosLog.Filter
                                    + " and  FROM_UNIXTIME(a.recdate, '%Y-%m-%d')>='" + textBox5.Text.Trim() + "'";

            }
            if (!textBox4.Text.Trim().Equals(""))
            {
                this.PageModel_SosLog.Filter = this.PageModel_SosLog.Filter
                                    + " and  FROM_UNIXTIME(a.recdate, '%Y-%m-%d')<='" + textBox4.Text.Trim() + "'";

            }


            int totalcount = 0;
            this.dataGridViewSoslog.AutoGenerateColumns = false;

            FullList_DapperSosLog = MyDapperDAO<DapperSOSLog>.GetPageList(this.PageModel_SosLog, out totalcount);

            dataGridViewSoslog.DataSource = new BindingCollection<DapperSOSLog>(FullList_DapperSosLog);


            if (totalcount == 0)
            {
                PAGE_NUM_SosLog = 0;
                soslog_desc.Text = "";
                soslog_pagetag.Text = "";
            }
            else
            {
                //设置 page_num_hr
                COUNT_SosLog = totalcount;
                PAGE_NUM_SosLog = totalcount / PAGE_PAGESIZE_SosLog + (totalcount % PAGE_PAGESIZE_SosLog > 0 ? 1 : 0);
                this.soslog_pagetag.Text = "1/" + this.PAGE_NUM_SosLog.ToString();
                this.soslog_desc.Text = WinFormsStringResource.QueryTotal + totalcount.ToString() + WinFormsStringResource.QueryRecDESC
                    + "," + WinFormsStringResource.QueryPerPage + this.PAGE_PAGESIZE_SosLog.ToString() +
                    WinFormsStringResource.QueryRecDESC + "," + WinFormsStringResource.QueryTotal
                    + this.PAGE_NUM_SosLog.ToString() + WinFormsStringResource.QueryPage;

                tb_soslogpageindex.Text = "1";
                PAGE_INDEX_SosLog = 1;

            }
            */



        }

        private void tbFirst_Soslog_Click2(object sender, EventArgs e)
        {
            //toPageFirst<DapperSOSLog>(PAGE_NUM_SosLog, 1, PageModel_SosLog, dataGridViewSoslog, out FullList_DapperSosLog,
            //    soslog_pagetag, tb_soslogpageindex, ref PAGE_INDEX_SosLog);
           
            int pageno = 1;
            if (PAGE_NUM_SosLog > 0 && pageno <= PAGE_NUM_SosLog
                    && pageno >= 1)
            {
               
                string startDate = textBox5.Text.Trim();
                string endDate = textBox4.Text.Trim();
                soslog_pagetag.Text = pageno.ToString() + "/" + PAGE_NUM_SosLog.ToString();
                tb_soslogpageindex.Text = "1";
                PAGE_INDEX_SosLog = 1;

                PageResponseBase<SoslogDto> resp = PocClient.querySosLog(LocalSharedData.CURRENTUser.cmpid, 1, startDate, endDate, PAGE_PAGESIZE_SosLog);
               

                if (resp.page != null && resp.page.list != null && resp.page.list.Count > 0)
                {
                    dataGridViewSoslog.DataSource = resp.page.list;
                }                    

                /*
                if (page_model != null)
                {
                    page_model.PageIndex = pageno;
                    page_model.IsCount = false;
                    int totalcount = 0;
                    list_rest = MyDapperDAO<T>.GetPageList(page_model, out totalcount);

                    datagridview.DataSource = new BindingCollection<T>(list_rest);

                    lab_pagetagdesc.Text = pageno.ToString() + "/" + page_num_total.ToString();
                    page_index = 1;
                    tb_page_index.Text = "1";

                }
                */
            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
                return;
            }

        }   

         

        

        private void tbPrev_Soslog_Click(object sender, EventArgs e)
        {           

             
            if (PAGE_NUM_SosLog == 0) return;

            if (PAGE_INDEX_SosLog == 1)
            {
                MessageBox.Show(WinFormsStringResource.PageNoFirstAlert);
                return;
            }
            int pageno = PAGE_INDEX_SosLog - 1;
            if (pageno <= PAGE_NUM_SosLog && pageno >= 1)
            {

                string startDate = textBox5.Text.Trim();
                string endDate = textBox4.Text.Trim();

                tb_soslogpageindex.Text = pageno.ToString();
                soslog_pagetag.Text = pageno.ToString() + "/" + PAGE_NUM_SosLog.ToString();
                PAGE_INDEX_SosLog = pageno;
                tb_soslogpageindex.Text = pageno.ToString();

                PageResponseBase<SoslogDto> resp = PocClient.querySosLog(LocalSharedData.CURRENTUser.cmpid, pageno, startDate, endDate, PAGE_PAGESIZE_SosLog);


                if (resp.page != null && resp.page.list != null && resp.page.list.Count > 0)
                {
                    dataGridViewSoslog.DataSource = resp.page.list;
                }
                

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }

        }

        private void tbNext_Soslog_Click(object sender, EventArgs e)
        {          
         
            if (PAGE_NUM_SosLog == 0) return;

            if (PAGE_INDEX_SosLog == PAGE_NUM_SosLog)
            {
                MessageBox.Show(WinFormsStringResource.PageNoLastAlert);
                return;
            }
            int pageno = PAGE_INDEX_SosLog + 1;
            if (pageno <= PAGE_NUM_SosLog && pageno >= 1)
            {

                string startDate = textBox5.Text.Trim();
                string endDate = textBox4.Text.Trim();

                soslog_pagetag.Text = pageno.ToString() + "/" + PAGE_NUM_SosLog.ToString();
                PAGE_INDEX_SosLog = pageno;
                tb_soslogpageindex.Text = pageno.ToString();


                PageResponseBase<SoslogDto> resp = PocClient.querySosLog(LocalSharedData.CURRENTUser.cmpid, pageno, startDate, endDate, PAGE_PAGESIZE_SosLog);
                
                if (resp.page != null && resp.page.list != null && resp.page.list.Count > 0)
                {
                    dataGridViewSoslog.DataSource = resp.page.list;
                }
              
            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
            }

        }

        private void tbLast_Soslog_Click(object sender, EventArgs e)
        {
          
            
            if (PAGE_NUM_SosLog == 0) return;
            int pageno = PAGE_NUM_SosLog;
            if (pageno <= PAGE_NUM_SosLog && pageno >= 1)
            {

                string startDate = textBox5.Text.Trim();
                string endDate = textBox4.Text.Trim();

                soslog_pagetag.Text = pageno.ToString() + "/" + PAGE_NUM_SosLog.ToString();
                PAGE_INDEX_SosLog = pageno;
                tb_soslogpageindex.Text = pageno.ToString();
                //

                PageResponseBase<SoslogDto> resp = PocClient.querySosLog(LocalSharedData.CURRENTUser.cmpid, pageno, startDate, endDate, PAGE_PAGESIZE_SosLog);


                if (resp.page != null && resp.page.list != null && resp.page.list.Count > 0)
                {
                    dataGridViewSoslog.DataSource = resp.page.list;
                }
                

            }
            else
            {
                MessageBox.Show(WinFormsStringResource.QueryInValid_PageNo);
                return;
            }


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

        private void button33_Click(object sender, EventArgs e)
        {
            if (myAudioQueryForm == null)
                myAudioQueryForm = new AudioQueryForm();

            if (myAudioQueryForm.ShowDialog() == DialogResult.OK)
            {
                audio_pagetag.Visible = true;

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

        private void button30_Click(object sender, EventArgs e)
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

        private void tbFirst_Voice_Click(object sender, EventArgs e)
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

        private void tbPrev_Voice_Click(object sender, EventArgs e)
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

        private void tbNext_Voice_Click(object sender, EventArgs e)
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

        private void tbLast_Voice_Click(object sender, EventArgs e)
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

        private void button31_Click(object sender, EventArgs e)
        {
            //
            if (myVideoQueryForm == null)
                myVideoQueryForm = new VideoQueryForm();

            if (myVideoQueryForm.ShowDialog() == DialogResult.OK)
            {
                video_pagetag.Visible = true;

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

        private void button14_Click(object sender, EventArgs e)
        {
            //video多个下载
            List<string> filepaths = new List<string>();
            int oper_num = 0;
            string video_path_str = "";
            for (int i = 0; i < dgv_video.Rows.Count; i++)
            {
                string oper = "0";
                if (dgv_video.Rows[i].Cells[0].Value != null)
                    oper = dgv_video.Rows[i].Cells[0].Value.ToString().Trim();

                if (oper.Equals("1"))
                {
                    oper_num += 1;
                    video_path_str = dgv_video.Rows[i].Cells["video_path"].Value.ToString().Trim();
                    ///hdd/htdocs/video/record//moni_600012159_6b6_8b8-1504087251-2017-08-30-180051.flv

                    //FullReplacePrex 代表 /hdd/htdocs/video/record/ 等
                    video_path_str = video_path_str.Replace(FullReplacePrex + "/hdd/htdocs/video/record/", "");
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
            }
            else
            {
                DownFilesForm downFrm = new DownFilesForm("video", this.FullDownloadVideoPath);
                downFrm.setDownFiles(filepaths);
                downFrm.ShowDialog();

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

         
        

        private void MyRecordForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
           // this.Visible = false;
        }

        private void MyRecordForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // this.Visible = false;
            this.Close();
        }

        private void MyRecordForm_Load(object sender, EventArgs e)
        {
            
        }

        private void dgv_audio_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int rowindex = e.RowIndex;
                if (rowindex == -1) return;  //点击了header
                int CIndex = e.ColumnIndex;
                if (CIndex == dgv_audio.Columns["playonline_a"].Index)
                {
                    
                
                    ///hdd/htdocs/voice//2017-08-11/80001196/600010169/80001196_600010169_1502435647899.amr                   

                    //MessageBox.Show(dgv_audio.Rows[rowindex].Cells["audio_path_a"].Value.ToString());
                    string audio_path_str = dgv_audio.Rows[rowindex].Cells["audio_path_a"].Value.ToString().Trim();
                    ///hdd/htdocs/video/record//moni_600012159_6b6_8b8-1504087251-2017-08-30-180051.flv
                    audio_path_str = audio_path_str.Replace(FullReplacePrex+ "/hdd/htdocs/voice/", "");
                    if (audio_path_str.StartsWith("/"))
                        audio_path_str = audio_path_str.Substring(1);
                    //string HTTP_URL_PREFIX_AUDIO = "http://" + HttpAPI.VIDEOIP + "/voice/";
                    audio_path_str = HTTP_URL_PREFIX_AUDIO + audio_path_str;

                    //清理上次播放中的播放器占的内存等
                    MediaPlayForm FullMeidaPlayForm = new MediaPlayForm(audio_path_str);

                    FullMeidaPlayForm.currMediaType = "audio";
                    FullMeidaPlayForm.button1.Enabled = true;
                    FullMeidaPlayForm.button3.Enabled = true;

                    FullMeidaPlayForm.duration_video.Text = dgv_audio.Rows[rowindex].Cells["duration_a"].Value.ToString() + "(秒)";
                    FullMeidaPlayForm.createdate_video.Text = dgv_audio.Rows[rowindex].Cells["record_date_str_a"].Value.ToString();
                    FullMeidaPlayForm.username.Text = dgv_audio.Rows[rowindex].Cells["user_name_a"].Value.ToString();


                    FullMeidaPlayForm.trackBar1.Minimum = 0; //duration
                    double totalsec = Convert.ToDouble(dgv_audio.Rows[rowindex].Cells["duration_a"].Value.ToString().Trim());
                    FullMeidaPlayForm.trackBar1.Maximum = (int)totalsec;
                    FullMeidaPlayForm.trackBar1.Value = 0;
                    if (FullMeidaPlayForm.trackBar1.Maximum / 10 == 0)
                        FullMeidaPlayForm.trackBar1.TickFrequency = 1;
                    else
                        FullMeidaPlayForm.trackBar1.TickFrequency = FullMeidaPlayForm.trackBar1.Maximum / 10;
                                        

                    FullMeidaPlayForm.currMediaType = "audio";
                    FullMeidaPlayForm.currMediaPlayingType = "audio";
                    FullMeidaPlayForm.ShowDialog();

                    FullMeidaPlayForm.Dispose();                    
                    
                }

            }

            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
            }
        }

        private void dgv_video_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int rowindex = e.RowIndex;
                if (rowindex == -1) return;  //点击了header
                int CIndex = e.ColumnIndex;
                if (CIndex == dgv_video.Columns["playonline"].Index)
                {                  
                    
                    string video_path_str = dgv_video.Rows[rowindex].Cells["video_path"].Value.ToString().Trim();
                    //liveboard/296/aeb45a63e5411599ec20b3b316bff630_100024-live_0.mp4
                    //video_path_str = video_path_str.Replace(FullReplacePrex + "/hdd/htdocs/video/record/", "");
                    if (video_path_str.StartsWith("/"))
                        video_path_str = video_path_str.Substring(1);
                   
                    video_path_str = HTTP_URL_PREFIX_VIDEO + video_path_str;
                    

                    MediaPlayForm FullMeidaPlayForm = new MediaPlayForm(video_path_str);

                    FullMeidaPlayForm.currMediaType = "video";
                    FullMeidaPlayForm.button1.Enabled = true;
                    FullMeidaPlayForm.button3.Enabled = true;

                    FullMeidaPlayForm.duration_video.Text = dgv_video.Rows[rowindex].Cells["duration"].Value.ToString() + "(分钟)";
                    FullMeidaPlayForm.createdate_video.Text = dgv_video.Rows[rowindex].Cells["record_date_str"].Value.ToString();
                    FullMeidaPlayForm.username.Text = dgv_video.Rows[rowindex].Cells["user_name_video"].Value.ToString();


                    FullMeidaPlayForm.trackBar1.Minimum = 0; //duration
                    double totalsec = Convert.ToDouble(dgv_video.Rows[rowindex].Cells["duration"].Value.ToString().Trim()) * 60;
                    FullMeidaPlayForm.trackBar1.Maximum = (int)totalsec;
                    FullMeidaPlayForm.trackBar1.Value = 0;  //复位
                    if (FullMeidaPlayForm.trackBar1.Maximum / 10 == 0)
                        FullMeidaPlayForm.trackBar1.TickFrequency = 1;
                    else
                        FullMeidaPlayForm.trackBar1.TickFrequency = FullMeidaPlayForm.trackBar1.Maximum / 10;                    

                    //mediType
                    FullMeidaPlayForm.currMediaType = "video";
                    //当前正在播放或暂停的类型
                    FullMeidaPlayForm.currMediaPlayingType = "video";

                    FullMeidaPlayForm.ShowDialog();

                    FullMeidaPlayForm.Dispose();                    

                }

            }

            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
            }
        }

        private void dataGridViewSoslog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int rowindex = e.RowIndex;
                if (rowindex == -1) return;  //点击了header
                int CIndex = e.ColumnIndex;
                if (CIndex == dataGridViewSoslog.Columns["playonline_b"].Index)
                {
                    //MessageBox.Show("123");
                    //必须每次都创建
                    MediaPlayForm FullMeidaPlayForm;
                    
                    //dataGridViewTextBoxColumn5
                    string username = dataGridViewSoslog.Rows[rowindex].Cells["dataGridViewTextBoxColumn5"].Value.ToString();
                    //dataGridViewTextBoxColumn3
                    string createdatestr = dataGridViewSoslog.Rows[rowindex].Cells["dataGridViewTextBoxColumn3"].Value.ToString();


                    string voice_filename = "";
                    if (dataGridViewSoslog.Rows[rowindex].Cells["voice_filename"].Value != null)
                        voice_filename = dataGridViewSoslog.Rows[rowindex].Cells["voice_filename"].Value.ToString();
                    string lat = dataGridViewSoslog.Rows[rowindex].Cells["dataGridViewTextBoxColumn8"].Value.ToString();
                    string lng = dataGridViewSoslog.Rows[rowindex].Cells["dataGridViewTextBoxColumn7"].Value.ToString();

                    string gps_addr = "";
                    if (dataGridViewSoslog.Rows[rowindex].Cells["gps_addr"].Value != null)
                        gps_addr = dataGridViewSoslog.Rows[rowindex].Cells["gps_addr"].Value.ToString();
                    //根据voice_filename 查找录音 
                    string audioDuration = "";
                    string avurl = "";
                    if (voice_filename != "")
                    {                    

                        //string sql_audiorec = "select * from ptt_user_audiorec where audio_name='" + voice_filename + "'";
                        //DapperAudioRec audioRec = MyDapperDAO<DapperAudioRec>.QuerySingle(sql_audiorec, null);

                        UserAudioRecResponse resp = PocClient.getUserAudioRec(voice_filename);
                        if (resp!=null && resp.data != null)
                        {
                            //FullMeidaPlayForm.duration_video.Text
                            audioDuration = resp.data.duration.ToString() + "(秒)";
                            //hdd/htdocs/voice//2017-08-11/80001196/600010169/80001196_600010169_1502435647899.amr                   

                            //MessageBox.Show(dgv_audio.Rows[rowindex].Cells["audio_path_a"].Value.ToString());
                            string audio_path_str = resp.data.audioPath.Trim();
                            ///hdd/htdocs/video/record//moni_600012159_6b6_8b8-1504087251-2017-08-30-180051.flv
                            audio_path_str = audio_path_str.Replace(FullReplacePrex + "/hdd/htdocs/voice/", "");
                            if (audio_path_str.StartsWith("/"))
                                audio_path_str = audio_path_str.Substring(1);
                            //string HTTP_URL_PREFIX_AUDIO = "http://" + HttpAPI.VIDEOIP + "/voice/";
                            audio_path_str = HTTP_URL_PREFIX_AUDIO + audio_path_str;

                            avurl = audio_path_str;

                            //清理上次播放中的播放器占的内存等
                            //FullMeidaPlayForm.trackBar1.Minimum = 0; //duration
                            double totalsec = Convert.ToDouble(resp.data.duration);
                            /*
                            FullMeidaPlayForm.trackBar1.Maximum = (int)totalsec;
                            FullMeidaPlayForm.trackBar1.Value = 0;
                            if (FullMeidaPlayForm.trackBar1.Maximum / 10 == 0)
                                FullMeidaPlayForm.trackBar1.TickFrequency = 1;
                            else
                                FullMeidaPlayForm.trackBar1.TickFrequency = FullMeidaPlayForm.trackBar1.Maximum / 10;
                            */

                            //meidaPlayForm.vlcControl1.SetMedia(new Uri(audio_path_str), MediaPlayer.arguments);

                            //FullMeidaPlayForm.vlcControl1.Audio.IsMute = false;
                            //FullMeidaPlayForm.vlcControl1.Audio.Volume = 100;
                            //FullMeidaPlayForm.vlcControl1.Play(new Uri(audio_path_str));

                        }
                        else
                        {
                            audioDuration = "";
                            avurl = "";
                        }
                        

                    }


                    if (lat != "0")
                    {
                        FullMeidaPlayForm = new MediaPlayForm(avurl,
                            HttpAPI.SOS_MAP_LOACTION_URL + "?lng=" + lng
                            + "&lat=" + lat + "&place=" + HttpAPI.ToUrlEncode(username) );
                        //MediaPlayForm
                        //    meidaPlayForm = new MediaPlayForm();
                        FullMeidaPlayForm.currMediaType = "sos";
                        FullMeidaPlayForm.button1.Enabled = true;
                        FullMeidaPlayForm.button3.Enabled = true;

                        //FullMeidaPlayForm.webMap.Navigate(HttpAPI.SOS_MAP_LOACTION_URL + "?lng=" + lng
                         //   + "&lat=" + lat + "&place=" + HttpAPI.ToUrlEncode(username));
                        FullMeidaPlayForm.lat = lat;
                        FullMeidaPlayForm.lng = lng;
                        FullMeidaPlayForm.username_str = username;
                        //FullMeidaPlayForm.webMap.Visible = true;
                        FullMeidaPlayForm.pictureBoxGpsFail.Visible = false;

                    }
                    else
                    {
                        //pictureBoxGpsFail
                        //FullMeidaPlayForm.webMap.Visible = false;
                        FullMeidaPlayForm = new MediaPlayForm(avurl,
                           "");

                        FullMeidaPlayForm.pictureBoxGpsFail.Visible = true;

                    }

                    FullMeidaPlayForm.duration_video.Text = audioDuration;

                    FullMeidaPlayForm.createdate_video.Text = createdatestr;
                    FullMeidaPlayForm.username.Text = username;
                    FullMeidaPlayForm.mapaddr.Text = gps_addr;

                    FullMeidaPlayForm.currMediaType = "sos";
                    FullMeidaPlayForm.currMediaPlayingType = "sos";
                    FullMeidaPlayForm.ShowDialog();
                    FullMeidaPlayForm.Dispose();
                    
                    ////回收,这个内存一直在增加
                    //GC.Collect();

                }

            }

            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
            }
        }

       
       
    }
}
