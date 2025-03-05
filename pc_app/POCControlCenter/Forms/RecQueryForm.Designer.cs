namespace POCControlCenter
{
    partial class RecQueryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecQueryForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageVideoRec = new System.Windows.Forms.TabPage();
            this.dgv_video = new System.Windows.Forms.DataGridView();
            this.checkvideo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.seqno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.playonline = new System.Windows.Forms.DataGridViewImageColumn();
            this.download = new System.Windows.Forms.DataGridViewImageColumn();
            this.video_type_str = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.record_date_str = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.user_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.custaccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stream = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.duration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filesize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.video_path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.cb_video_autoplay = new System.Windows.Forms.CheckBox();
            this.tb_videopageindex = new System.Windows.Forms.TextBox();
            this.video_desc = new System.Windows.Forms.Label();
            this.video_pagetag = new System.Windows.Forms.Label();
            this.tbLast_Video = new System.Windows.Forms.Button();
            this.tbNext_Video = new System.Windows.Forms.Button();
            this.tbPrev_Video = new System.Windows.Forms.Button();
            this.tbFirst_Video = new System.Windows.Forms.Button();
            this.btnVideoQuery = new System.Windows.Forms.Button();
            this.tabPageAudioRec = new System.Windows.Forms.TabPage();
            this.dgv_audio = new System.Windows.Forms.DataGridView();
            this.checkaudio = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.seqno_a = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.playonline_a = new System.Windows.Forms.DataGridViewImageColumn();
            this.download_a = new System.Windows.Forms.DataGridViewImageColumn();
            this.record_date_str_a = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.user_name_a = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.custaccount_a = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.group_name_a = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.audio_name_a = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.duration_a = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filesize_a = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.audio_path_a = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel5 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.cb_audio_autoplay = new System.Windows.Forms.CheckBox();
            this.tb_audiopageindex = new System.Windows.Forms.TextBox();
            this.audio_desc = new System.Windows.Forms.Label();
            this.audio_pagetag = new System.Windows.Forms.Label();
            this.tbLast_Audio = new System.Windows.Forms.Button();
            this.tbNext_Audio = new System.Windows.Forms.Button();
            this.tbPrev_Audio = new System.Windows.Forms.Button();
            this.tbFirst_Audio = new System.Windows.Forms.Button();
            this.btnAudioQuery = new System.Windows.Forms.Button();
            this.tabPageGrpFileRec = new System.Windows.Forms.TabPage();
            this.dgv_session = new System.Windows.Forms.DataGridView();
            this.checksession = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.seqno_s = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.playonline_s = new System.Windows.Forms.DataGridViewImageColumn();
            this.download_s = new System.Windows.Forms.DataGridViewImageColumn();
            this.file_type_str = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.file_uptime_str = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.user_name_s = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.custaccount_b = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.group_name_s = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.file_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.file_key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.user_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel6 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.cb_session_autoplay = new System.Windows.Forms.CheckBox();
            this.tb_sessionpageindex = new System.Windows.Forms.TextBox();
            this.session_desc = new System.Windows.Forms.Label();
            this.session_pagetag = new System.Windows.Forms.Label();
            this.tbLast_Session = new System.Windows.Forms.Button();
            this.tbNext_Session = new System.Windows.Forms.Button();
            this.tbPrev_Session = new System.Windows.Forms.Button();
            this.tbFirst_Session = new System.Windows.Forms.Button();
            this.btnSessionQuery = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.media_state = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.recorddate = new System.Windows.Forms.Label();
            this.username = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.createdate_video = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.duration_video = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.filename_video = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewImageColumn2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewImageColumn3 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewImageColumn4 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewImageColumn5 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewImageColumn6 = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabControl1.SuspendLayout();
            this.tabPageVideoRec.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_video)).BeginInit();
            this.panel4.SuspendLayout();
            this.tabPageAudioRec.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_audio)).BeginInit();
            this.panel5.SuspendLayout();
            this.tabPageGrpFileRec.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_session)).BeginInit();
            this.panel6.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.panelContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPageVideoRec);
            this.tabControl1.Controls.Add(this.tabPageAudioRec);
            this.tabControl1.Controls.Add(this.tabPageGrpFileRec);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageVideoRec
            // 
            this.tabPageVideoRec.Controls.Add(this.dgv_video);
            this.tabPageVideoRec.Controls.Add(this.panel4);
            resources.ApplyResources(this.tabPageVideoRec, "tabPageVideoRec");
            this.tabPageVideoRec.Name = "tabPageVideoRec";
            this.tabPageVideoRec.UseVisualStyleBackColor = true;
            // 
            // dgv_video
            // 
            this.dgv_video.AllowUserToAddRows = false;
            this.dgv_video.AllowUserToDeleteRows = false;
            this.dgv_video.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgv_video.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_video.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.checkvideo,
            this.seqno,
            this.playonline,
            this.download,
            this.video_type_str,
            this.record_date_str,
            this.user_name,
            this.custaccount,
            this.stream,
            this.duration,
            this.filesize,
            this.video_path});
            resources.ApplyResources(this.dgv_video, "dgv_video");
            this.dgv_video.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgv_video.Name = "dgv_video";
            this.dgv_video.RowHeadersVisible = false;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_video.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_video.RowTemplate.Height = 23;
            this.dgv_video.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_video_CellContentClick);
            this.dgv_video.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_video_CellLeave);
            // 
            // checkvideo
            // 
            this.checkvideo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.checkvideo.FalseValue = "0";
            this.checkvideo.Frozen = true;
            resources.ApplyResources(this.checkvideo, "checkvideo");
            this.checkvideo.Name = "checkvideo";
            this.checkvideo.TrueValue = "1";
            // 
            // seqno
            // 
            this.seqno.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.seqno.DataPropertyName = "seqno";
            resources.ApplyResources(this.seqno, "seqno");
            this.seqno.Name = "seqno";
            this.seqno.ReadOnly = true;
            // 
            // playonline
            // 
            resources.ApplyResources(this.playonline, "playonline");
            this.playonline.Image = global::POCControlCenter.Properties.Resources.play_18;
            this.playonline.Name = "playonline";
            this.playonline.ReadOnly = true;
            // 
            // download
            // 
            resources.ApplyResources(this.download, "download");
            this.download.Image = global::POCControlCenter.Properties.Resources.download_file_18;
            this.download.Name = "download";
            this.download.ReadOnly = true;
            // 
            // video_type_str
            // 
            this.video_type_str.DataPropertyName = "video_type_str";
            resources.ApplyResources(this.video_type_str, "video_type_str");
            this.video_type_str.Name = "video_type_str";
            this.video_type_str.ReadOnly = true;
            // 
            // record_date_str
            // 
            this.record_date_str.DataPropertyName = "record_date_str";
            resources.ApplyResources(this.record_date_str, "record_date_str");
            this.record_date_str.Name = "record_date_str";
            // 
            // user_name
            // 
            this.user_name.DataPropertyName = "user_name";
            resources.ApplyResources(this.user_name, "user_name");
            this.user_name.Name = "user_name";
            // 
            // custaccount
            // 
            this.custaccount.DataPropertyName = "cust_account";
            resources.ApplyResources(this.custaccount, "custaccount");
            this.custaccount.Name = "custaccount";
            // 
            // stream
            // 
            this.stream.DataPropertyName = "stream";
            resources.ApplyResources(this.stream, "stream");
            this.stream.Name = "stream";
            this.stream.ReadOnly = true;
            // 
            // duration
            // 
            this.duration.DataPropertyName = "duration";
            resources.ApplyResources(this.duration, "duration");
            this.duration.Name = "duration";
            this.duration.ReadOnly = true;
            // 
            // filesize
            // 
            this.filesize.DataPropertyName = "filesize";
            resources.ApplyResources(this.filesize, "filesize");
            this.filesize.Name = "filesize";
            this.filesize.ReadOnly = true;
            this.filesize.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // video_path
            // 
            this.video_path.DataPropertyName = "vedio_path";
            resources.ApplyResources(this.video_path, "video_path");
            this.video_path.Name = "video_path";
            this.video_path.ReadOnly = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button1);
            this.panel4.Controls.Add(this.cb_video_autoplay);
            this.panel4.Controls.Add(this.tb_videopageindex);
            this.panel4.Controls.Add(this.video_desc);
            this.panel4.Controls.Add(this.video_pagetag);
            this.panel4.Controls.Add(this.tbLast_Video);
            this.panel4.Controls.Add(this.tbNext_Video);
            this.panel4.Controls.Add(this.tbPrev_Video);
            this.panel4.Controls.Add(this.tbFirst_Video);
            this.panel4.Controls.Add(this.btnVideoQuery);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cb_video_autoplay
            // 
            resources.ApplyResources(this.cb_video_autoplay, "cb_video_autoplay");
            this.cb_video_autoplay.Name = "cb_video_autoplay";
            this.cb_video_autoplay.UseVisualStyleBackColor = true;
            // 
            // tb_videopageindex
            // 
            resources.ApplyResources(this.tb_videopageindex, "tb_videopageindex");
            this.tb_videopageindex.Name = "tb_videopageindex";
            this.tb_videopageindex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_videopageindex_KeyDown);
            this.tb_videopageindex.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_videopageindex_KeyPress);
            // 
            // video_desc
            // 
            resources.ApplyResources(this.video_desc, "video_desc");
            this.video_desc.ForeColor = System.Drawing.Color.Teal;
            this.video_desc.Name = "video_desc";
            // 
            // video_pagetag
            // 
            resources.ApplyResources(this.video_pagetag, "video_pagetag");
            this.video_pagetag.ForeColor = System.Drawing.Color.Teal;
            this.video_pagetag.Name = "video_pagetag";
            // 
            // tbLast_Video
            // 
            this.tbLast_Video.BackColor = System.Drawing.Color.Transparent;
            this.tbLast_Video.FlatAppearance.BorderSize = 0;
            this.tbLast_Video.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbLast_Video.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbLast_Video, "tbLast_Video");
            this.tbLast_Video.ForeColor = System.Drawing.Color.White;
            this.tbLast_Video.Image = global::POCControlCenter.Properties.Resources.page_endx;
            this.tbLast_Video.Name = "tbLast_Video";
            this.tbLast_Video.UseVisualStyleBackColor = false;
            this.tbLast_Video.Click += new System.EventHandler(this.tbLast_Video_Click);
            // 
            // tbNext_Video
            // 
            this.tbNext_Video.BackColor = System.Drawing.Color.Transparent;
            this.tbNext_Video.FlatAppearance.BorderSize = 0;
            this.tbNext_Video.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbNext_Video.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbNext_Video, "tbNext_Video");
            this.tbNext_Video.ForeColor = System.Drawing.Color.White;
            this.tbNext_Video.Image = global::POCControlCenter.Properties.Resources.page_nextx;
            this.tbNext_Video.Name = "tbNext_Video";
            this.tbNext_Video.UseVisualStyleBackColor = false;
            this.tbNext_Video.Click += new System.EventHandler(this.tbNext_Video_Click);
            // 
            // tbPrev_Video
            // 
            this.tbPrev_Video.BackColor = System.Drawing.Color.Transparent;
            this.tbPrev_Video.FlatAppearance.BorderSize = 0;
            this.tbPrev_Video.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbPrev_Video.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbPrev_Video, "tbPrev_Video");
            this.tbPrev_Video.ForeColor = System.Drawing.Color.White;
            this.tbPrev_Video.Image = global::POCControlCenter.Properties.Resources.page_prevx;
            this.tbPrev_Video.Name = "tbPrev_Video";
            this.tbPrev_Video.UseVisualStyleBackColor = false;
            this.tbPrev_Video.Click += new System.EventHandler(this.tbPrev_Video_Click);
            // 
            // tbFirst_Video
            // 
            this.tbFirst_Video.BackColor = System.Drawing.Color.Transparent;
            this.tbFirst_Video.FlatAppearance.BorderSize = 0;
            this.tbFirst_Video.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbFirst_Video.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbFirst_Video, "tbFirst_Video");
            this.tbFirst_Video.ForeColor = System.Drawing.Color.White;
            this.tbFirst_Video.Image = global::POCControlCenter.Properties.Resources.page_firstx;
            this.tbFirst_Video.Name = "tbFirst_Video";
            this.tbFirst_Video.UseVisualStyleBackColor = false;
            this.tbFirst_Video.Click += new System.EventHandler(this.tbFirst_Video_Click);
            // 
            // btnVideoQuery
            // 
            this.btnVideoQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.btnVideoQuery.FlatAppearance.BorderSize = 0;
            this.btnVideoQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnVideoQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.btnVideoQuery, "btnVideoQuery");
            this.btnVideoQuery.ForeColor = System.Drawing.Color.White;
            this.btnVideoQuery.Name = "btnVideoQuery";
            this.btnVideoQuery.UseVisualStyleBackColor = false;
            this.btnVideoQuery.Click += new System.EventHandler(this.btnVideoQuery_Click);
            // 
            // tabPageAudioRec
            // 
            this.tabPageAudioRec.Controls.Add(this.dgv_audio);
            this.tabPageAudioRec.Controls.Add(this.panel5);
            resources.ApplyResources(this.tabPageAudioRec, "tabPageAudioRec");
            this.tabPageAudioRec.Name = "tabPageAudioRec";
            this.tabPageAudioRec.UseVisualStyleBackColor = true;
            // 
            // dgv_audio
            // 
            this.dgv_audio.AllowUserToAddRows = false;
            this.dgv_audio.AllowUserToDeleteRows = false;
            this.dgv_audio.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.dgv_audio.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_audio.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.checkaudio,
            this.seqno_a,
            this.playonline_a,
            this.download_a,
            this.record_date_str_a,
            this.user_name_a,
            this.custaccount_a,
            this.group_name_a,
            this.audio_name_a,
            this.duration_a,
            this.filesize_a,
            this.audio_path_a});
            resources.ApplyResources(this.dgv_audio, "dgv_audio");
            this.dgv_audio.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgv_audio.Name = "dgv_audio";
            this.dgv_audio.RowHeadersVisible = false;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_audio.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_audio.RowTemplate.Height = 23;
            this.dgv_audio.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_audio_CellContentClick);
            this.dgv_audio.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_audio_CellLeave);
            // 
            // checkaudio
            // 
            this.checkaudio.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.checkaudio.FalseValue = "0";
            this.checkaudio.Frozen = true;
            resources.ApplyResources(this.checkaudio, "checkaudio");
            this.checkaudio.Name = "checkaudio";
            this.checkaudio.TrueValue = "1";
            // 
            // seqno_a
            // 
            this.seqno_a.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.seqno_a.DataPropertyName = "seqno";
            resources.ApplyResources(this.seqno_a, "seqno_a");
            this.seqno_a.Name = "seqno_a";
            this.seqno_a.ReadOnly = true;
            // 
            // playonline_a
            // 
            resources.ApplyResources(this.playonline_a, "playonline_a");
            this.playonline_a.Image = global::POCControlCenter.Properties.Resources.play_18;
            this.playonline_a.Name = "playonline_a";
            this.playonline_a.ReadOnly = true;
            // 
            // download_a
            // 
            resources.ApplyResources(this.download_a, "download_a");
            this.download_a.Image = global::POCControlCenter.Properties.Resources.download_file_18;
            this.download_a.Name = "download_a";
            this.download_a.ReadOnly = true;
            // 
            // record_date_str_a
            // 
            this.record_date_str_a.DataPropertyName = "record_date_str";
            resources.ApplyResources(this.record_date_str_a, "record_date_str_a");
            this.record_date_str_a.Name = "record_date_str_a";
            // 
            // user_name_a
            // 
            this.user_name_a.DataPropertyName = "user_name";
            resources.ApplyResources(this.user_name_a, "user_name_a");
            this.user_name_a.Name = "user_name_a";
            // 
            // custaccount_a
            // 
            this.custaccount_a.DataPropertyName = "cust_account";
            resources.ApplyResources(this.custaccount_a, "custaccount_a");
            this.custaccount_a.Name = "custaccount_a";
            // 
            // group_name_a
            // 
            this.group_name_a.DataPropertyName = "group_name";
            resources.ApplyResources(this.group_name_a, "group_name_a");
            this.group_name_a.Name = "group_name_a";
            // 
            // audio_name_a
            // 
            this.audio_name_a.DataPropertyName = "audio_name";
            resources.ApplyResources(this.audio_name_a, "audio_name_a");
            this.audio_name_a.Name = "audio_name_a";
            this.audio_name_a.ReadOnly = true;
            // 
            // duration_a
            // 
            this.duration_a.DataPropertyName = "duration";
            resources.ApplyResources(this.duration_a, "duration_a");
            this.duration_a.Name = "duration_a";
            this.duration_a.ReadOnly = true;
            // 
            // filesize_a
            // 
            this.filesize_a.DataPropertyName = "filesize";
            resources.ApplyResources(this.filesize_a, "filesize_a");
            this.filesize_a.Name = "filesize_a";
            this.filesize_a.ReadOnly = true;
            this.filesize_a.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // audio_path_a
            // 
            this.audio_path_a.DataPropertyName = "audio_path";
            resources.ApplyResources(this.audio_path_a, "audio_path_a");
            this.audio_path_a.Name = "audio_path_a";
            this.audio_path_a.ReadOnly = true;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.button2);
            this.panel5.Controls.Add(this.cb_audio_autoplay);
            this.panel5.Controls.Add(this.tb_audiopageindex);
            this.panel5.Controls.Add(this.audio_desc);
            this.panel5.Controls.Add(this.audio_pagetag);
            this.panel5.Controls.Add(this.tbLast_Audio);
            this.panel5.Controls.Add(this.tbNext_Audio);
            this.panel5.Controls.Add(this.tbPrev_Audio);
            this.panel5.Controls.Add(this.tbFirst_Audio);
            this.panel5.Controls.Add(this.btnAudioQuery);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cb_audio_autoplay
            // 
            resources.ApplyResources(this.cb_audio_autoplay, "cb_audio_autoplay");
            this.cb_audio_autoplay.Name = "cb_audio_autoplay";
            this.cb_audio_autoplay.UseVisualStyleBackColor = true;
            // 
            // tb_audiopageindex
            // 
            resources.ApplyResources(this.tb_audiopageindex, "tb_audiopageindex");
            this.tb_audiopageindex.Name = "tb_audiopageindex";
            this.tb_audiopageindex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_audiopageindex_KeyDown);
            this.tb_audiopageindex.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_audiopageindex_KeyPress);
            // 
            // audio_desc
            // 
            resources.ApplyResources(this.audio_desc, "audio_desc");
            this.audio_desc.ForeColor = System.Drawing.Color.Teal;
            this.audio_desc.Name = "audio_desc";
            // 
            // audio_pagetag
            // 
            resources.ApplyResources(this.audio_pagetag, "audio_pagetag");
            this.audio_pagetag.ForeColor = System.Drawing.Color.Teal;
            this.audio_pagetag.Name = "audio_pagetag";
            // 
            // tbLast_Audio
            // 
            this.tbLast_Audio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tbLast_Audio.FlatAppearance.BorderSize = 0;
            this.tbLast_Audio.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbLast_Audio.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbLast_Audio, "tbLast_Audio");
            this.tbLast_Audio.ForeColor = System.Drawing.Color.White;
            this.tbLast_Audio.Image = global::POCControlCenter.Properties.Resources.result_last;
            this.tbLast_Audio.Name = "tbLast_Audio";
            this.tbLast_Audio.UseVisualStyleBackColor = false;
            this.tbLast_Audio.Click += new System.EventHandler(this.tbLast_Audio_Click);
            // 
            // tbNext_Audio
            // 
            this.tbNext_Audio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tbNext_Audio.FlatAppearance.BorderSize = 0;
            this.tbNext_Audio.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbNext_Audio.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbNext_Audio, "tbNext_Audio");
            this.tbNext_Audio.ForeColor = System.Drawing.Color.White;
            this.tbNext_Audio.Image = global::POCControlCenter.Properties.Resources.result_next;
            this.tbNext_Audio.Name = "tbNext_Audio";
            this.tbNext_Audio.UseVisualStyleBackColor = false;
            this.tbNext_Audio.Click += new System.EventHandler(this.tbNext_Audio_Click);
            // 
            // tbPrev_Audio
            // 
            this.tbPrev_Audio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tbPrev_Audio.FlatAppearance.BorderSize = 0;
            this.tbPrev_Audio.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbPrev_Audio.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbPrev_Audio, "tbPrev_Audio");
            this.tbPrev_Audio.ForeColor = System.Drawing.Color.White;
            this.tbPrev_Audio.Image = global::POCControlCenter.Properties.Resources.result_prev;
            this.tbPrev_Audio.Name = "tbPrev_Audio";
            this.tbPrev_Audio.UseVisualStyleBackColor = false;
            this.tbPrev_Audio.Click += new System.EventHandler(this.tbPrev_Audio_Click);
            // 
            // tbFirst_Audio
            // 
            this.tbFirst_Audio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tbFirst_Audio.FlatAppearance.BorderSize = 0;
            this.tbFirst_Audio.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbFirst_Audio.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbFirst_Audio, "tbFirst_Audio");
            this.tbFirst_Audio.ForeColor = System.Drawing.Color.White;
            this.tbFirst_Audio.Image = global::POCControlCenter.Properties.Resources.result_first;
            this.tbFirst_Audio.Name = "tbFirst_Audio";
            this.tbFirst_Audio.UseVisualStyleBackColor = false;
            this.tbFirst_Audio.Click += new System.EventHandler(this.tbFirst_Audio_Click);
            // 
            // btnAudioQuery
            // 
            this.btnAudioQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.btnAudioQuery.FlatAppearance.BorderSize = 0;
            this.btnAudioQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnAudioQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.btnAudioQuery, "btnAudioQuery");
            this.btnAudioQuery.ForeColor = System.Drawing.Color.White;
            this.btnAudioQuery.Name = "btnAudioQuery";
            this.btnAudioQuery.UseVisualStyleBackColor = false;
            this.btnAudioQuery.Click += new System.EventHandler(this.btnAudioQuery_Click);
            // 
            // tabPageGrpFileRec
            // 
            this.tabPageGrpFileRec.Controls.Add(this.dgv_session);
            this.tabPageGrpFileRec.Controls.Add(this.panel6);
            resources.ApplyResources(this.tabPageGrpFileRec, "tabPageGrpFileRec");
            this.tabPageGrpFileRec.Name = "tabPageGrpFileRec";
            this.tabPageGrpFileRec.UseVisualStyleBackColor = true;
            // 
            // dgv_session
            // 
            this.dgv_session.AllowUserToAddRows = false;
            this.dgv_session.AllowUserToDeleteRows = false;
            this.dgv_session.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.dgv_session.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_session.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.checksession,
            this.seqno_s,
            this.playonline_s,
            this.download_s,
            this.file_type_str,
            this.file_uptime_str,
            this.user_name_s,
            this.custaccount_b,
            this.group_name_s,
            this.file_name,
            this.file_key,
            this.user_id});
            resources.ApplyResources(this.dgv_session, "dgv_session");
            this.dgv_session.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgv_session.Name = "dgv_session";
            this.dgv_session.RowHeadersVisible = false;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_session.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_session.RowTemplate.Height = 23;
            this.dgv_session.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_session_CellContentClick);
            // 
            // checksession
            // 
            this.checksession.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.checksession.FalseValue = "0";
            this.checksession.Frozen = true;
            resources.ApplyResources(this.checksession, "checksession");
            this.checksession.Name = "checksession";
            this.checksession.TrueValue = "1";
            // 
            // seqno_s
            // 
            this.seqno_s.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.seqno_s.DataPropertyName = "seqno";
            resources.ApplyResources(this.seqno_s, "seqno_s");
            this.seqno_s.Name = "seqno_s";
            this.seqno_s.ReadOnly = true;
            // 
            // playonline_s
            // 
            resources.ApplyResources(this.playonline_s, "playonline_s");
            this.playonline_s.Image = global::POCControlCenter.Properties.Resources.play;
            this.playonline_s.Name = "playonline_s";
            this.playonline_s.ReadOnly = true;
            // 
            // download_s
            // 
            resources.ApplyResources(this.download_s, "download_s");
            this.download_s.Image = global::POCControlCenter.Properties.Resources.download_file_18;
            this.download_s.Name = "download_s";
            this.download_s.ReadOnly = true;
            // 
            // file_type_str
            // 
            this.file_type_str.DataPropertyName = "file_type_str";
            resources.ApplyResources(this.file_type_str, "file_type_str");
            this.file_type_str.Name = "file_type_str";
            this.file_type_str.ReadOnly = true;
            // 
            // file_uptime_str
            // 
            this.file_uptime_str.DataPropertyName = "file_uptime_str";
            resources.ApplyResources(this.file_uptime_str, "file_uptime_str");
            this.file_uptime_str.Name = "file_uptime_str";
            // 
            // user_name_s
            // 
            this.user_name_s.DataPropertyName = "user_name";
            resources.ApplyResources(this.user_name_s, "user_name_s");
            this.user_name_s.Name = "user_name_s";
            // 
            // custaccount_b
            // 
            this.custaccount_b.DataPropertyName = "cust_account";
            resources.ApplyResources(this.custaccount_b, "custaccount_b");
            this.custaccount_b.Name = "custaccount_b";
            // 
            // group_name_s
            // 
            this.group_name_s.DataPropertyName = "group_name";
            resources.ApplyResources(this.group_name_s, "group_name_s");
            this.group_name_s.Name = "group_name_s";
            // 
            // file_name
            // 
            this.file_name.DataPropertyName = "file_name";
            resources.ApplyResources(this.file_name, "file_name");
            this.file_name.Name = "file_name";
            this.file_name.ReadOnly = true;
            // 
            // file_key
            // 
            this.file_key.DataPropertyName = "file_key";
            resources.ApplyResources(this.file_key, "file_key");
            this.file_key.Name = "file_key";
            this.file_key.ReadOnly = true;
            // 
            // user_id
            // 
            this.user_id.DataPropertyName = "user_id";
            resources.ApplyResources(this.user_id, "user_id");
            this.user_id.Name = "user_id";
            this.user_id.ReadOnly = true;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.button3);
            this.panel6.Controls.Add(this.cb_session_autoplay);
            this.panel6.Controls.Add(this.tb_sessionpageindex);
            this.panel6.Controls.Add(this.session_desc);
            this.panel6.Controls.Add(this.session_pagetag);
            this.panel6.Controls.Add(this.tbLast_Session);
            this.panel6.Controls.Add(this.tbNext_Session);
            this.panel6.Controls.Add(this.tbPrev_Session);
            this.panel6.Controls.Add(this.tbFirst_Session);
            this.panel6.Controls.Add(this.btnSessionQuery);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.button3, "button3");
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // cb_session_autoplay
            // 
            resources.ApplyResources(this.cb_session_autoplay, "cb_session_autoplay");
            this.cb_session_autoplay.Name = "cb_session_autoplay";
            this.cb_session_autoplay.UseVisualStyleBackColor = true;
            // 
            // tb_sessionpageindex
            // 
            resources.ApplyResources(this.tb_sessionpageindex, "tb_sessionpageindex");
            this.tb_sessionpageindex.Name = "tb_sessionpageindex";
            this.tb_sessionpageindex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_sessionpageindex_KeyDown);
            this.tb_sessionpageindex.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_sessionpageindex_KeyPress);
            // 
            // session_desc
            // 
            resources.ApplyResources(this.session_desc, "session_desc");
            this.session_desc.ForeColor = System.Drawing.Color.Teal;
            this.session_desc.Name = "session_desc";
            // 
            // session_pagetag
            // 
            resources.ApplyResources(this.session_pagetag, "session_pagetag");
            this.session_pagetag.ForeColor = System.Drawing.Color.Teal;
            this.session_pagetag.Name = "session_pagetag";
            // 
            // tbLast_Session
            // 
            this.tbLast_Session.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tbLast_Session.FlatAppearance.BorderSize = 0;
            this.tbLast_Session.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbLast_Session.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbLast_Session, "tbLast_Session");
            this.tbLast_Session.ForeColor = System.Drawing.Color.White;
            this.tbLast_Session.Image = global::POCControlCenter.Properties.Resources.result_last;
            this.tbLast_Session.Name = "tbLast_Session";
            this.tbLast_Session.UseVisualStyleBackColor = false;
            this.tbLast_Session.Click += new System.EventHandler(this.tbLast_Session_Click);
            // 
            // tbNext_Session
            // 
            this.tbNext_Session.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tbNext_Session.FlatAppearance.BorderSize = 0;
            this.tbNext_Session.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbNext_Session.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbNext_Session, "tbNext_Session");
            this.tbNext_Session.ForeColor = System.Drawing.Color.White;
            this.tbNext_Session.Image = global::POCControlCenter.Properties.Resources.result_next;
            this.tbNext_Session.Name = "tbNext_Session";
            this.tbNext_Session.UseVisualStyleBackColor = false;
            this.tbNext_Session.Click += new System.EventHandler(this.tbNext_Session_Click);
            // 
            // tbPrev_Session
            // 
            this.tbPrev_Session.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tbPrev_Session.FlatAppearance.BorderSize = 0;
            this.tbPrev_Session.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbPrev_Session.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbPrev_Session, "tbPrev_Session");
            this.tbPrev_Session.ForeColor = System.Drawing.Color.White;
            this.tbPrev_Session.Image = global::POCControlCenter.Properties.Resources.result_prev;
            this.tbPrev_Session.Name = "tbPrev_Session";
            this.tbPrev_Session.UseVisualStyleBackColor = false;
            this.tbPrev_Session.Click += new System.EventHandler(this.tbPrev_Session_Click);
            // 
            // tbFirst_Session
            // 
            this.tbFirst_Session.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.tbFirst_Session.FlatAppearance.BorderSize = 0;
            this.tbFirst_Session.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.tbFirst_Session.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.tbFirst_Session, "tbFirst_Session");
            this.tbFirst_Session.ForeColor = System.Drawing.Color.White;
            this.tbFirst_Session.Image = global::POCControlCenter.Properties.Resources.result_first;
            this.tbFirst_Session.Name = "tbFirst_Session";
            this.tbFirst_Session.UseVisualStyleBackColor = false;
            this.tbFirst_Session.Click += new System.EventHandler(this.tbFirst_Session_Click);
            // 
            // btnSessionQuery
            // 
            this.btnSessionQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.btnSessionQuery.FlatAppearance.BorderSize = 0;
            this.btnSessionQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.btnSessionQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            resources.ApplyResources(this.btnSessionQuery, "btnSessionQuery");
            this.btnSessionQuery.ForeColor = System.Drawing.Color.White;
            this.btnSessionQuery.Name = "btnSessionQuery";
            this.btnSessionQuery.UseVisualStyleBackColor = false;
            this.btnSessionQuery.Click += new System.EventHandler(this.btnSessionQuery_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitter1);
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Controls.Add(this.panel3);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelContainer, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.media_state);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.recorddate);
            this.panel1.Controls.Add(this.username);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.createdate_video);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.duration_video);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.filename_video);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // media_state
            // 
            resources.ApplyResources(this.media_state, "media_state");
            this.media_state.ForeColor = System.Drawing.Color.Teal;
            this.media_state.Name = "media_state";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // trackBar1
            // 
            this.trackBar1.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.trackBar1, "trackBar1");
            this.trackBar1.LargeChange = 3;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.trackBar1.Enter += new System.EventHandler(this.trackBar1_Enter);
            this.trackBar1.Leave += new System.EventHandler(this.trackBar1_Leave);
            this.trackBar1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar1_MouseUp);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // recorddate
            // 
            resources.ApplyResources(this.recorddate, "recorddate");
            this.recorddate.ForeColor = System.Drawing.Color.Teal;
            this.recorddate.Name = "recorddate";
            // 
            // username
            // 
            resources.ApplyResources(this.username, "username");
            this.username.ForeColor = System.Drawing.Color.Teal;
            this.username.Name = "username";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // createdate_video
            // 
            resources.ApplyResources(this.createdate_video, "createdate_video");
            this.createdate_video.ForeColor = System.Drawing.Color.Teal;
            this.createdate_video.Name = "createdate_video";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // duration_video
            // 
            resources.ApplyResources(this.duration_video, "duration_video");
            this.duration_video.ForeColor = System.Drawing.Color.Teal;
            this.duration_video.Name = "duration_video";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // filename_video
            // 
            resources.ApplyResources(this.filename_video, "filename_video");
            this.filename_video.ForeColor = System.Drawing.Color.Teal;
            this.filename_video.Name = "filename_video";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panelContainer
            // 
            this.panelContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelContainer.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.panelContainer, "panelContainer");
            this.panelContainer.Name = "panelContainer";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::POCControlCenter.Properties.Resources.shengbo_bar;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dataGridViewImageColumn1
            // 
            resources.ApplyResources(this.dataGridViewImageColumn1, "dataGridViewImageColumn1");
            this.dataGridViewImageColumn1.Image = global::POCControlCenter.Properties.Resources.play;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.ReadOnly = true;
            // 
            // dataGridViewImageColumn2
            // 
            resources.ApplyResources(this.dataGridViewImageColumn2, "dataGridViewImageColumn2");
            this.dataGridViewImageColumn2.Image = global::POCControlCenter.Properties.Resources.download_file;
            this.dataGridViewImageColumn2.Name = "dataGridViewImageColumn2";
            this.dataGridViewImageColumn2.ReadOnly = true;
            // 
            // dataGridViewImageColumn3
            // 
            resources.ApplyResources(this.dataGridViewImageColumn3, "dataGridViewImageColumn3");
            this.dataGridViewImageColumn3.Image = global::POCControlCenter.Properties.Resources.play;
            this.dataGridViewImageColumn3.Name = "dataGridViewImageColumn3";
            this.dataGridViewImageColumn3.ReadOnly = true;
            // 
            // dataGridViewImageColumn4
            // 
            resources.ApplyResources(this.dataGridViewImageColumn4, "dataGridViewImageColumn4");
            this.dataGridViewImageColumn4.Image = global::POCControlCenter.Properties.Resources.download_file;
            this.dataGridViewImageColumn4.Name = "dataGridViewImageColumn4";
            this.dataGridViewImageColumn4.ReadOnly = true;
            // 
            // dataGridViewImageColumn5
            // 
            resources.ApplyResources(this.dataGridViewImageColumn5, "dataGridViewImageColumn5");
            this.dataGridViewImageColumn5.Image = global::POCControlCenter.Properties.Resources.play;
            this.dataGridViewImageColumn5.Name = "dataGridViewImageColumn5";
            this.dataGridViewImageColumn5.ReadOnly = true;
            // 
            // dataGridViewImageColumn6
            // 
            resources.ApplyResources(this.dataGridViewImageColumn6, "dataGridViewImageColumn6");
            this.dataGridViewImageColumn6.Image = global::POCControlCenter.Properties.Resources.download_file;
            this.dataGridViewImageColumn6.Name = "dataGridViewImageColumn6";
            this.dataGridViewImageColumn6.ReadOnly = true;
            // 
            // RecQueryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.Name = "RecQueryForm";
            this.Tag = "99";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RecQueryForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RecQueryForm_FormClosed);
            this.Load += new System.EventHandler(this.RecQueryForm_Load);
            this.Shown += new System.EventHandler(this.RecQueryForm_Shown);
            this.tabControl1.ResumeLayout(false);
            this.tabPageVideoRec.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_video)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.tabPageAudioRec.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_audio)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.tabPageGrpFileRec.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_session)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.panelContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageVideoRec;
        private System.Windows.Forms.TabPage tabPageAudioRec;
        private System.Windows.Forms.TabPage tabPageGrpFileRec;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgv_video;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label createdate_video;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label duration_video;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label filename_video;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.Button btnVideoQuery;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn2;
        private System.Windows.Forms.Button tbFirst_Video;
        private System.Windows.Forms.Button tbLast_Video;
        private System.Windows.Forms.Button tbNext_Video;
        private System.Windows.Forms.Button tbPrev_Video;
        private System.Windows.Forms.TextBox tb_videopageindex;
        private System.Windows.Forms.Label video_desc;
        private System.Windows.Forms.Label video_pagetag;
        private System.Windows.Forms.CheckBox cb_video_autoplay;
        private System.Windows.Forms.DataGridView dgv_audio;
        private System.Windows.Forms.CheckBox cb_audio_autoplay;
        private System.Windows.Forms.TextBox tb_audiopageindex;
        private System.Windows.Forms.Label audio_desc;
        private System.Windows.Forms.Label audio_pagetag;
        private System.Windows.Forms.Button tbLast_Audio;
        private System.Windows.Forms.Button tbNext_Audio;
        private System.Windows.Forms.Button tbPrev_Audio;
        private System.Windows.Forms.Button tbFirst_Audio;
        private System.Windows.Forms.Button btnAudioQuery;
        private System.Windows.Forms.DataGridView dgv_session;
        private System.Windows.Forms.CheckBox cb_session_autoplay;
        private System.Windows.Forms.TextBox tb_sessionpageindex;
        private System.Windows.Forms.Label session_desc;
        private System.Windows.Forms.Label session_pagetag;
        private System.Windows.Forms.Button tbLast_Session;
        private System.Windows.Forms.Button tbNext_Session;
        private System.Windows.Forms.Button tbPrev_Session;
        private System.Windows.Forms.Button tbFirst_Session;
        private System.Windows.Forms.Button btnSessionQuery;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label username;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label recorddate;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label media_state;
        public System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn3;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn4;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn5;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn6;
        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checksession;
        private System.Windows.Forms.DataGridViewTextBoxColumn seqno_s;
        private System.Windows.Forms.DataGridViewImageColumn playonline_s;
        private System.Windows.Forms.DataGridViewImageColumn download_s;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_type_str;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_uptime_str;
        private System.Windows.Forms.DataGridViewTextBoxColumn user_name_s;
        private System.Windows.Forms.DataGridViewTextBoxColumn custaccount_b;
        private System.Windows.Forms.DataGridViewTextBoxColumn group_name_s;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn file_key;
        private System.Windows.Forms.DataGridViewTextBoxColumn user_id;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkvideo;
        private System.Windows.Forms.DataGridViewTextBoxColumn seqno;
        private System.Windows.Forms.DataGridViewImageColumn playonline;
        private System.Windows.Forms.DataGridViewImageColumn download;
        private System.Windows.Forms.DataGridViewTextBoxColumn video_type_str;
        private System.Windows.Forms.DataGridViewTextBoxColumn record_date_str;
        private System.Windows.Forms.DataGridViewTextBoxColumn user_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn custaccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn stream;
        private System.Windows.Forms.DataGridViewTextBoxColumn duration;
        private System.Windows.Forms.DataGridViewTextBoxColumn filesize;
        private System.Windows.Forms.DataGridViewTextBoxColumn video_path;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkaudio;
        private System.Windows.Forms.DataGridViewTextBoxColumn seqno_a;
        private System.Windows.Forms.DataGridViewImageColumn playonline_a;
        private System.Windows.Forms.DataGridViewImageColumn download_a;
        private System.Windows.Forms.DataGridViewTextBoxColumn record_date_str_a;
        private System.Windows.Forms.DataGridViewTextBoxColumn user_name_a;
        private System.Windows.Forms.DataGridViewTextBoxColumn custaccount_a;
        private System.Windows.Forms.DataGridViewTextBoxColumn group_name_a;
        private System.Windows.Forms.DataGridViewTextBoxColumn audio_name_a;
        private System.Windows.Forms.DataGridViewTextBoxColumn duration_a;
        private System.Windows.Forms.DataGridViewTextBoxColumn filesize_a;
        private System.Windows.Forms.DataGridViewTextBoxColumn audio_path_a;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}