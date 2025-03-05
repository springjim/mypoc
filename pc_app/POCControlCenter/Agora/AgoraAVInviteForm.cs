using POCClientNetLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.Agora
{
    public partial class AgoraAVInviteForm : Form
    {
        public SoundPlayer player;
        private System.Timers.Timer mTimer = null;
        private int delayTimeMs = 30000;  //超时时间设置，单位ms

        public  string mChannelName { get; set; } = "";
        private string mFromUserId = "";
        private string mFromUserName = "";
        private string mToUserId = "";
        private string mToUserName = "";
        private string mDesc = "";
        private short  mVideoType = 0;

        private ControlMainForm mainForm;
        private ChatClient client;   //底层Socket操作封装的类

        public AgoraAVInviteForm()
        {
            InitializeComponent();
            mTimer = new System.Timers.Timer(delayTimeMs);
            mTimer.Interval = delayTimeMs;
            mTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerEvent);
            mTimer.Start();
        }

        public AgoraAVInviteForm(ControlMainForm mainForm, ChatClient socketClient, string channelName, string fromUserId, string fromUserName, string toUserId,
        string toUserName, string desc, short videoType ) : this()
        {
            this.mainForm = mainForm;
            this.client = socketClient;
            this.mChannelName = channelName;
            this.mFromUserId = fromUserId;
            this.mFromUserName = fromUserName;
            this.mToUserId = toUserId;
            this.mToUserName = toUserName;
            this.mDesc = desc;
            this.mVideoType = videoType;
        }

        public void OnTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Abort;
                    mTimer.Stop();
                    if (mainForm != null)
                    {
                        mainForm.avInvite = null;
                    }
                    this.Close();
                }));
            }
            
        }

        private void AgoraAVInviteForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
            }
        }

        private void AgoraAVInviteForm_Load(object sender, EventArgs e)
        {
            if (player == null)
            {
                player = new SoundPlayer();
                player.SoundLocation = Path.Combine(System.Environment.CurrentDirectory, "telephone-ring.wav");
                player.Load();
                player.Play();
                player.PlayLooping();
            }

            if (mVideoType == 1)
                labVideoType.Text = "语音通话邀请...";
            else if (mVideoType == 2)
                labVideoType.Text = "视频通话邀请...";


        }

        private void hangup_Click(object sender, EventArgs e)
        {
            //发送拒绝socket消息
            if (client != null)
            {
                client.SendMessage(
                                      (new Data()).AVChatNewCommandMessageEncode
                                      ((short)2, (short)3, LocalSharedData.CURRENTUser.user_id,
                                      Convert.ToInt32(mToUserId), LocalSharedData.CURRENTUser.user_name, mToUserName,
                                      "hangup"));
            }

            if (mainForm != null)
            {
                mainForm.avInvite = null;
            }
            Close();
        }

        public void notifyRefuseMessage()
        {
            //
            labVideoType.Text = labVideoType.Text + " 【对方挂断了】";

            if (mTimer != null)
            {
                mTimer.Stop();
                mTimer.Interval = 3000;
                mTimer.Start();

            }            

        }

        private void btn_receive_Click(object sender, EventArgs e)
        {
            //
            Thread th = new Thread(new ThreadStart(delegate
            {


                Application.Run(new JoinChannelVideoView(
                    client, mChannelName, mToUserId, mToUserName,
                                       mFromUserId, mFromUserName,  "desc",
                    FormClose, true, mVideoType) );                

            }));

            th.Start();            

        }

        private void FormClose()
        {
            this.BeginInvoke(new MethodInvoker(delegate {
                if (mainForm != null)
                {
                    mainForm.avInvite = null;
                }
                this.Close();
            }));
        }

    }
}
