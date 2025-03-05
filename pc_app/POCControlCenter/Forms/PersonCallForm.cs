
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

namespace POCControlCenter
{
    public partial class PersonCallForm : Form
    {
        public static POCControlCenter.DataEntity.UserCallSession session = new DataEntity.UserCallSession();
        public static PersonCallForm  CallForm = null;
        public static ChatClient      chat;

        public delegate void  MessageReceivedEventHandler(object sender, ServerEventArgs e);
        public static event   MessageReceivedEventHandler MessageReceived;

        // 创建 被叫方
        public static int CreateCalled( ChatClient cc, int groupid, int userid, int callgroupid, int invitedid, MessageReceivedEventHandler Received)
        {
            if( session != null && session.state > 0 )
            {
                HttpAPI.ReleaseJoinPersonGroup(userid, invitedid, callgroupid, POCClientNetLibrary.MySysMsgType.SYS_MSSAGE_EXIT_PRESON);
                return 0;
            }

            session            = new DataEntity.UserCallSession();
            MessageReceived    = Received;
            chat = cc;

            session.calltype    = 0;
            session.callid      = userid;
            session.curgroudid  = groupid;
            session.calledid    = invitedid;
            session.callgroudid = callgroupid;

            session.state = 0;

            CallForm = new PersonCallForm();
            if (LocalSharedData.UserAll.ContainsKey(session.calledid))
            {
                //CallForm.labelcall.Text = "来电...";
                //CallForm.labelcallname.Text = LocalSharedData.UserAll[session.calledid].user_name;
                CallForm.SetUIStatus(LocalSharedData.UserAll[session.calledid].userName);
            }
            else
            {
                //CallForm.labelcall.Text = "来电...";
                //CallForm.labelcallname.Text = "没有找到名字..";

                CallForm.SetUIStatus(WinFormsStringResource.PersonCall_Text1);
            }

            CallForm.Show();
            CallForm.CallTimeOUTTimer( false );
            return 0;
        }

        // 创建 主叫方 
        public static int CreateCaller( ChatClient cc, int groupid, int userid, int invitedid, MessageReceivedEventHandler Received)
        {
            session         = new DataEntity.UserCallSession();
            MessageReceived = Received;
            chat = cc;

            session.calltype   = 1;
            session.callid     = userid;
            session.curgroudid = groupid;
            session.calledid   = invitedid;

            session.state      = 0;

            session.callgroudid = HttpAPI.createPersonGroup( session.callid, session.calledid, 0 );

            CallForm = new PersonCallForm();
            CallForm.buttonAccept.Visible = false;
            if (LocalSharedData.UserAll.ContainsKey(session.calledid))
            {
                // CallForm.labelcallname.Text = LocalSharedData.UserAll[session.calledid].user_name;
                CallForm.SetUIStatus(LocalSharedData.UserAll[session.calledid].userName);
            }

            CallForm.Show();
            CallForm.CallTimeOUTTimer(false);
            return 0;
        }

        // 主叫方 收到对方接收 Accept;
        public static void onRecvCalledAccept()
        {
            if( CallForm != null && CallForm.Visible )
                CallForm.EndCallTimeOUTTimer(true);
            session.state = 1;

            if (LocalSharedData.UserAll.ContainsKey(session.calledid))
            {
                // CallForm.labelcallname.Text = LocalSharedData.UserAll[session.calledid].user_name;
                CallForm.SetUIStatus(LocalSharedData.UserAll[session.calledid].userName);
            }

            if (CallForm != null && CallForm.Visible)
                CallForm.pictureBox1.Visible = false;

            chat.SendMessage( (new Data()).ReportMessageEncode(session.callgroudid, session.callid) );
        }
        // 主叫方 收到对方拒绝 refuse;
        public static void onRecvCalledRefuse( )
        {
            if (CallForm != null && CallForm.Visible)
                CallForm.EndCallTimeOUTTimer(true);

            // HttpAPI.ReleaseJoinPersonGroup( session.callid, session.calledid, session.callgroudid, POCClientNetLibrary.MySysMsgType.SYS_MSSAGE_EXIT_PRESON );
            if ( CallForm!=null && CallForm.Visible )
                CallForm.Close();
        }

        // 主叫方/被叫方 收到挂机 hangup
        public static void onRecvCallingCancel()
        {
            if( CallForm != null && CallForm.Visible )
            {
                CallForm.Close();
                chat.SendMessage((new Data()).ReportMessageEncode(session.curgroudid, session.callid));
            }
        }

        public PersonCallForm()
        {
            InitializeComponent();
        }

        protected static void OnMessageReceived( int calltype, int callstate )
        {
            var handler = MessageReceived;
            if (handler != null)
                handler( CallForm, new ServerEventArgs(calltype, callstate) );
        }

        // 被叫方接收 Accept; 被叫方
        private void buttonAccept_Click(object sender, EventArgs e)
        {
            EndCallTimeOUTTimer(true);

            HttpAPI.JoinPersonGroup( session.callid, session.calledid, session.callgroudid );
            chat.SendMessage( (new Data()).ReportMessageEncode( session.callgroudid, session.callid ) );
            OnMessageReceived( session.calltype, 1 );

            buttonAccept.Visible = false;
            session.state = 1;

            pictureBox1.Visible = false;
        }

        // 挂机 hangup：主叫方/被叫方 
        private void buttonhangup_Click(object sender, EventArgs e)
        {
            if (session.state == -1)
                return;

            if( session.state==0 )
                EndCallTimeOUTTimer(true);

            if( session.state==1 )
                OnMessageReceived( session.calltype, 0 );

            session.state = -1;

            HttpAPI.ReleaseJoinPersonGroup(session.callid,  session.calledid, session.callgroudid, POCClientNetLibrary.MySysMsgType.SYS_MSSAGE_EXIT_PRESON);
            CallForm.Close();
            chat.SendMessage((new Data()).ReportMessageEncode(session.curgroudid, session.callid));
        }

        private void PersonCallForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if ( session.state != 0 )
            {
                this.InvokeOnClick( buttonhangup, null );
            }

            session.state = -1;
            CallForm      = null;
        }


        [System.Runtime.InteropServices.DllImport("winmm.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int PlaySound(string lpszSoundName, int hModule, int dwFlags);

        const int SND_FILENAME = 131072;
        const int SND_ALIAS    = 65536;
        const int SND_SYNC     = 0;
        const int SND_ASYNC    = 0x0001;
        const int SND_LOOP     = 0x0008;
        //
        //SND_FILENAME|SND_ASYNC |SND_LOOP
        //
        private static void PlaySound(string fileStr)
        {
            PlaySound(fileStr, 0, SND_FILENAME | SND_ASYNC | SND_LOOP);
        }
        public void runSound(String FileName)
        {
            PlaySound(FileName, 0, SND_ASYNC | SND_FILENAME | SND_LOOP);
        }
        public void stopSound()
        {
            PlaySound(null, 0, SND_ASYNC);
        }

        #region // 铃声文件 加载路径 
        public string GetRingSoundPath()
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\RingSound\\";
            return path;
        }
        public string GetCallRingFile()
        {
            //return GetRingSoundPath() +"sound_call_incoming.wav";
            return GetRingSoundPath() + "call_incoming.wav";             
        }
        public string GetCallBackRingFile()
        {
            //return GetRingSoundPath() + "sound_call_back.wav";
            return GetRingSoundPath() + "call_incoming.wav";
        }
        #endregion

        #region // 检查 消息是否发送成功 timer事件
        private const int CALLTIME_LENGTH = 60;
        private int       OutTime = 0;

        private void SetUIStatus( String txt )
        {
            if (session.calltype == 0)
            {
                labelcall.Text     = WinFormsStringResource.PersonCall_Text2;
                labelcallname.Text = txt;
            }
            else
            {
                labelcall.Text     = WinFormsStringResource.PersonCall_Text3;
                labelcallname.Text = txt;
            }
        }

        private delegate void CallTimeOUTTimerDelegate(bool t);
        private void CallTimeOUTTimer(bool t) // 启用 或 禁用 发送功能
        {
            OutTime = 0;
            if ( !t )
            {
                this.timerCallTime.Interval = 1000;
                this.timerCallTime.Start();

                if (session.calltype == 1)
                    runSound(GetCallBackRingFile());
                else
                    runSound(GetCallRingFile());
            }

            if ( t )
            {
                this.timerCallTime.Stop();
                stopSound();

                this.pictureBox1.Visible = false;
            }
        }

        private void EndCallTimeOUTTimer(bool t) // 启用 或 禁用 发送功能
        {
            if (InvokeRequired)
            {
                CallTimeOUTTimerDelegate uld = new CallTimeOUTTimerDelegate(CallTimeOUTTimer);
                this.Invoke(uld, new object[] { t });
                return;
            }

            CallTimeOUTTimer(t);
        }

        private void timerCallTime_Tick(object sender, EventArgs e)
        {
            OutTime++;

            String name = "";
            if (LocalSharedData.UserAll.ContainsKey(session.calledid))
            {
                name =  LocalSharedData.UserAll[session.calledid].userName;
            }

            String text = name + WinFormsStringResource.PersonCall_Text4 + OutTime.ToString() + WinFormsStringResource.PersonCall_Text5;
            SetUIStatus(text);

            if( OutTime >= CALLTIME_LENGTH - 1 )
            {
                HttpAPI.ReleaseJoinPersonGroup(session.callid, session.calledid, session.callgroudid, POCClientNetLibrary.MySysMsgType.SYS_MSSAGE_EXIT_PRESON);
                Close();

                EndCallTimeOUTTimer(true);
            }
        }
        #endregion

    }
}
