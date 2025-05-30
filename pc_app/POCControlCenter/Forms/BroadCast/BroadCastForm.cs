﻿using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using POCClientNetLibrary;
using POCControlCenter.DataEntity;
using POCControlCenter.Service;
using POCControlCenter.Service.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.BroadCast
{
    public partial class BroadCastForm : Form
    {
        private ChatClient client;

        public bool m_blUIStalk = false;   //对讲时用
        public bool m_blUIPlay = false;    //对讲时用
        ControlMainForm mainForm;

        /// <summary>
        /// 1: 表示是临式组形式,  2: 表示是即兴创建的，则退出窗口要删除的
        /// </summary>
        private int m_createMode;

        private int groupId;      //广播组ID
        private string groupName;  //广播组名称

        private List<User> members = new List<User>();  //包含的成员列表 (包含调度台本身)
        private delegate void UpdateButtonTalkUIStatusDelegate(Control ctrl);
        private delegate void UpdatePlayUIStatusDelegate(Control ctrl, string s);
        private delegate void UpdateNetUserUIStatusDelegate(int groupID, int userID ,int status);

        private delegate bool PlayWaveFileFrameDelegate(WaveFileReader _waveReader); //执行播放wav的委托
        private PlayWaveFileFrameDelegate  _waveInvoke = null;

        private AMRNBCodec codec;
        private BufferedWaveProvider wavePlayProvider;

        private WaveFileReader waveReader;
        /// <summary>
        /// 是否中断文件广播
        /// </summary>
        private  Boolean isBroadBreaking = false;
        //模仿窗口标题栏拖动
        #region Form Move

        private bool mIsMouseDown = false;
        private Point mFormLocation;     // Form的location
        private Point mMouseOffset;      // 鼠标的按下位置

        private void OnFormMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mIsMouseDown = true;
                mFormLocation = this.Location;
                mMouseOffset = Control.MousePosition;
            }
        }

        private void OnFormMouseUp(object sender, MouseEventArgs e)
        {
            mIsMouseDown = false;
        }

        private void OnFormMouseMove(object sender, MouseEventArgs e)
        {
            if (mIsMouseDown)
            {
                Point pt = Control.MousePosition;
                int x = mMouseOffset.X - pt.X;
                int y = mMouseOffset.Y - pt.Y;

                this.Location = new Point(mFormLocation.X - x, mFormLocation.Y - y);
            }
        }
        #endregion
        public BroadCastForm(ChatClient client, int groupId,string groupName, List<User> members, int m_createMode)
        {
            InitializeComponent();
            this.client = client;
            this.groupId = groupId;
            this.groupName = groupName;

            this.members.AddRange(members);
            this.m_createMode = m_createMode;
            this.codec = new AMRNBCodec();

            WaveFormat wFormat = new WaveFormat(8000, 16, 1);          

            this.wavePlayProvider = new BufferedWaveProvider(wFormat);
             
            this.waveReader = null;
        }



        private void btnClose_Click(object sender, EventArgs e)
        {
            //如果未停止说话,这里要自动关闭
            if (m_blUIStalk)
                buttonTALK.PerformClick();
            if (this.m_createMode == 1)
            {
                DialogResult = DialogResult.Cancel;

            } else
            {
                //动态创建的
                if (groupId > 0)
                {
                    ResponseBase result = PocClient.deleteTmpGroup(groupId, 7, 1);
                    ControlMainForm.BROADCAST_POC_ENABLE = false;  //退出临时广播模式
                    DialogResult = DialogResult.Cancel;

                }
                else
                {
                    ControlMainForm.BROADCAST_POC_ENABLE = false;  //退出临时广播模式
                    DialogResult = DialogResult.Cancel;
                }
            }
        }

        private void BroadCastForm_Load(object sender, EventArgs e)
        {
            labelPlayerName.Text = "";
            this.labGroupName.Text = groupName + "(" + members.Count + ")";

            //
            flpUserPanel.SuspendLayout(); //先挂起布局逻辑
            foreach (User user in members)
            {

                UserControlBroad userControl = new UserControlBroad();//一般控件也可以
                userControl.labUserName.Text = user.userName;
                if (user.userId == LocalSharedData.CURRENTUser.user_id)
                {
                    userControl.picMic.Visible = true;
                    userControl.picDel.Visible = false;
                }
                else
                {
                    userControl.picMic.Visible = false;
                    if (m_createMode == 1)
                    {
                        userControl.picDel.Visible = false;
                        userControl.picInvite.Visible = true;
                    }                        
                    else
                    {
                        userControl.picDel.Visible = true;
                        userControl.picInvite.Visible = true;
                    }
                       
                }

                if (user.logon == 1)
                    userControl.picUserImg.Image = Properties.Resources.user_icon_online;
                else
                    userControl.picUserImg.Image = Properties.Resources.user_icon_offline;


                userControl.picDel.Tag = userControl;
                userControl.picSwitch.Tag = userControl;
                userControl.picInvite.Tag = userControl;

                userControl.Tag = user; //关联

                userControl.picDel.Click += PicDel_Click;
                userControl.picSwitch.Click += PicSwitch_Click;
                userControl.picInvite.Click += PicInvite_Click;

                flpUserPanel.Controls.Add(userControl);//添加布局
            }

            if (m_createMode == 1)
            {
                btnAddUser.Visible = false;

            }
            else
            {
                btnAddUser.Visible = true;
            }

            flpUserPanel.ResumeLayout(false);//恢复布局逻辑
            flpUserPanel.AutoScroll = true;//这步很重要，在子控件比较多的时候必须调用这个方法才会出现滚动条
            //broadCastForm.flpUserPanel.AutoScrollPosition = new Point(0, this.flpMsg.DisplayRectangle.Height);//这里是自动将滚动条滚动到底部，看需求而定

            //立即强制同步所有参与终端
            if (client != null)
            {
                PocClient.syncGroup(Convert.ToString(groupId)
                     , labGroupName.Text, Convert.ToString(LocalSharedData.CURRENTUser.user_id),
                     "2"
                     );


                //告诉
                client.SendMessage(
                  (new Data()).ReportMessageEncode(groupId, LocalSharedData.CURRENTUser.user_id)
              );

            }

        }

        private void PicInvite_Click(object sender, EventArgs e)
        {
             
            Object obj = ((PictureBox)sender).Tag;
            UserControlBroad userControlBroad = (UserControlBroad)obj;
            User user = userControlBroad.Tag as User;           
            if (client != null)
            {
                //TMD 被邀请的要写在第3个参数上
                client.SendMessage((new Data()).GroupSyncCommandMessageEncode(groupId, groupName,
                    user.userId, LocalSharedData.CURRENTUser.user_id));
            }     
            
        }

        /// <summary>
        /// 授与指定终端麦权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PicSwitch_Click(object sender, EventArgs e)
        {
            //

        }

        private void PicDel_Click(object sender, EventArgs e)
        {
            //静态广播组不允许删除
            if (this.m_createMode == 2)
            {
                UserControlBroad tagControl = (UserControlBroad)(((PictureBox)sender).Tag);
                User user = tagControl.Tag as User;
                //1发送踢除数据包  
                ResponseBase res = null;

                res = PocClient.groupUserChange(groupId, 1, 0, Convert.ToString(user.userId));

                if (res != null && res.error == 0)
                {
                    //强拆成功
                    members.Remove(members.Find(delegate (User o)
                    {
                        return o.userId == user.userId;
                    }));

                    //2.并删除自身
                    flpUserPanel.Controls.Remove(tagControl);
                    labGroupName.Text = groupName + "(" + members.Count + ")";
                }
                else
                {
                    MessageBox.Show(res.errorMsg);
                }
            }            

        }

        private void buttonTALK_Click(object sender, EventArgs e)
        {
            if (m_blUIStalk == false)
            {
                //发出抢麦信号
                client.SendMessage((new Data(MyType.TYPE_ROB_BROAD_MIC)).ToByte());
            }
            else
            {
                btnSelFile.Enabled = true;

                mainForm = (ControlMainForm)this.Owner;
                mainForm.EndChat();
                client.SendMessage((new Data(MyType.TYPE_REALASE_MIC)).ToByte());

                m_blUIStalk = false;
                isBroadBreaking = true;

                client.AEC_State = 0;
                SetButtonTalkUIStatus();
            }
        }

        //用户状态登录、登出的回调
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        /// <param name="status"> 1: 进入， 2: 退出 </param>
        public void UpdateUserStatus(int groupID, int userID, int status)
        {
            if (!this.IsHandleCreated) return;
            if (flpUserPanel.InvokeRequired == true)
            {
                UpdateNetUserUIStatusDelegate uld = new UpdateNetUserUIStatusDelegate(UpdateUserStatus);
                this.Invoke(uld, new object[] { groupID, userID });
                return;
            }
            //
            if (groupID == this.groupId)
            {
                //同一组的
                foreach (Control control in flpUserPanel.Controls)
                {
                    UserControlBroad broadControl = (UserControlBroad)control;
                    User user = broadControl.Tag as User;
                    if (groupID == groupId && user.userId == userID)
                    {
                        //再看状态
                        if (status == 1)
                        {
                            //上线在该组
                            broadControl.picUserImg.Image = Properties.Resources.user_icon_online;
                        }
                        else
                        {
                            broadControl.picUserImg.Image = Properties.Resources.user_icon_offline;
                        }

                        break;

                    }

                }

            } else
            {
                //不同组的，则要找出userId就可以了
                foreach (Control control in flpUserPanel.Controls)
                {
                    UserControlBroad broadControl = (UserControlBroad)control;
                    User user = broadControl.Tag as User;
                    if (user.userId == userID)
                    {                        
                        broadControl.picUserImg.Image = Properties.Resources.user_icon_offline;
                        break;
                    }

                }

            }
            

        }

        public void SetButtonTalkUIStatus()
        {
            if (buttonTALK.InvokeRequired == true)
            {
                //如果调用方上创建buttonTALK之外的线程
                UpdateButtonTalkUIStatusDelegate uld = new UpdateButtonTalkUIStatusDelegate(UpdateButtonTalkUIStatus);
                buttonTALK.Invoke(uld, new object[] { buttonTALK });
            }
            else
            {
                UpdateButtonTalkUIStatus(buttonTALK);
            }
        }

        /// <summary>
        /// 抢到麦后的回调
        /// </summary>
        /// <param name="ctrl"></param>
        private void UpdateButtonTalkUIStatus(Control ctrl)
        {
            if (m_blUIStalk == false)
            {
                buttonTALK.Image = global::POCControlCenter.Properties.Resources.d_speak40;
                buttonTALK.Text =  "开始广播";
                labelPlayerName.Text = "";

                //这时要把 "选择文件广播"的按钮 enable
                btnSelFile.Enabled = true;
            }
            else
            {
                buttonTALK.Text = "结束广播";
                buttonTALK.Image = global::POCControlCenter.Properties.Resources.d_nospeak40;
                labelPlayerName.Text = "自己";

                //这时要把 "选择文件广播"的按钮 disable
                btnSelFile.Enabled = false;

                //这时，如果waveReader不为空,则是文件广播
                if (this.waveReader != null)
                {
                    _waveInvoke = new PlayWaveFileFrameDelegate(PlayWaveFileFrame);
                    _waveInvoke.BeginInvoke(this.waveReader, WavePlayEnd, null);                   

                }
                
            }
        }

        private void WavePlayEnd(IAsyncResult result)
        {
            try
            {
                bool ret = _waveInvoke.EndInvoke(result);
                if (ret)
                {
                    Task.Delay(1500).Wait();

                    Debug.WriteLine("buttonTALK.PerformClick(): 3");
                    waveReader.Dispose();
                    waveReader = null;

                    Task.Delay(500).Wait();

                    ControlMainForm.BROADCAST_POC_FILE_MODE_ENABLE = false;   //恢得到MIC广播模式                    

                    client.SendMessage((new Data(MyType.TYPE_REALASE_MIC)).ToByte());
                    client.recordingbuffer.Reset();

                    m_blUIStalk = false;
                    client.AEC_State = 0;

                    //以下是跨线程调用了，不能直接写
                    //buttonTALK.Image = global::POCControlCenter.Properties.Resources.d_speak40;
                    //buttonTALK.Text = "开始广播";
                    //labelPlayerName.Text = "";
                    SetButtonTalkUIStatus();
                }

            } catch (Exception ee)
            {
                Debug.WriteLine(ee.Message);
            }

         }

        private bool PlayWaveFileFrame(WaveFileReader _waveReader)
        {
            isBroadBreaking = false;
            
            ISampleProvider provider = waveReader.ToSampleProvider();
                
            IWaveProvider provider16 = provider.ToWaveProvider16();

            byte[] buffer = new byte[320];  //每20ms 的大小就是320个字节
            int bytesRead;
            int len;
            byte[] outbuf = null;
            client.AEC_State = 1;

            do
            {
                bytesRead = provider16.Read(buffer, 0, buffer.Length);
                //codec.Encode(ref outbuf, buffer, buffer.Length);   //直接原样发送，不编码, app端接收不用解码, 直接播放
                len = buffer.Length;
                if (len > 0)
                {

                    //client.SendMessage((new Data()).MediaEncode_FileFrame(outbuf));
                    client.SendMessage((new Data()).MediaEncode_FileFrame(buffer));
                    Task.Delay(1).Wait(); //不加这句,会立即执行完
                }

            } while (bytesRead != 0 && !isBroadBreaking);

            

            //2024.3.24 加入
            // 计算缓冲区大小（以字节为单位）  

            //int bytesPerSecond = waveReader.WaveFormat.AverageBytesPerSecond;
            //int bufferSize = (int)(bytesPerSecond * (10 / 1000.0));
            //BufferedWaveProvider bufferedWaveProvider = new BufferedWaveProvider(waveReader.WaveFormat);
          
            ////bufferedWaveProvider.DiscardOnBufferOverflow = true;

            //byte[] buffer = new byte[320];  //每20ms 的大小就是320个字节
            //int bytesRead;
            //int len;
            //byte[] outbuf = null;
            //client.AEC_State = 1;

            //do
            //{
            //    bytesRead = bufferedWaveProvider.Read(buffer, 0, buffer.Length);
            //    codec.Encode(ref outbuf, buffer, buffer.Length);
            //    len = outbuf.Length;
            //    if (len > 0)
            //    {

            //        //client.RecordMediaData(outbuf, outbuf.Length);
            //        client.SendMessage((new Data()).MediaEncode_FileFrame(outbuf));
            //        Task.Delay(1).Wait(); //不加这句,会立即执行完
            //    }

            //} while (bytesRead != 0 && !isBroadBreaking);


            return true;

        }

        public void SetPlayUIStatus(String Name)
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

        private void UpdatePlayUIStatus(Control ctrl, string Name)
        {
            //识别广播中的讲话人           
            labelPlayerName.Text = Name;

        }
        

        private void btnSelFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "选择已录音文件进行广播";
                openFileDialog.Multiselect = false;
                //初始化保存目录，默认exe文件目录
                openFileDialog.InitialDirectory = Application.StartupPath;
                //设置保存文件的类型
                openFileDialog.Filter = "音频文件|*.wav";
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                string filePath = openFileDialog.FileName;
                waveReader = new WaveFileReader(filePath);
                

                //发出抢麦信号
                mainForm = (ControlMainForm)this.Owner;
                 

                ControlMainForm.BROADCAST_POC_FILE_MODE_ENABLE = true;   //开始文件广播模式
                
                client.SendMessage((new Data(MyType.TYPE_ROB_BROAD_MIC)).ToByte());

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            

        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            //弹出人员选择框
            SelUserForm crtform = new SelUserForm();
            crtform.clearChecklist();

            foreach (User user in LocalSharedData.UserAllGroupCustomer)
            {
                if (user.userId != LocalSharedData.CURRENTUser.user_id)
                {
                    User myuser = members.Find(delegate (User o)
                    {

                        return o.userId == user.userId;
                    });
                    if (myuser == null)
                    {
                        //不在才加入
                        if (user.logon == 1)
                            crtform.addChecklist(Convert.ToString(user.userId), user.userName + " <" + WinFormsStringResource.StatusOnline + ">");
                        else
                            crtform.addChecklist(Convert.ToString(user.userId), user.userName);

                    }

                }
            }
            //
            try
            {
                if (crtform.ShowDialog() == DialogResult.OK)
                {
                    string members = crtform.getMember();
                    if (members.Trim().Equals(""))
                        return;

                    ResponseBase res = PocClient.groupUserChange(this.groupId, 1, 1, crtform.getMember());

                    if (res != null && res.error == 0)
                    {
                        string[] memArr = members.Split(',');
                        foreach (string str in memArr)
                        {
                            User myuser = LocalSharedData.UserAllGroupCustomer.Find(delegate (User o)
                            {
                                return o.userId == Convert.ToInt32(str);
                            });
                            if (myuser == null) continue;

                            //1.加入members
                            this.members.Add(myuser);

                            //2.加入control
                            UserControlBroad userControl = new UserControlBroad();//一般控件也可以
                            userControl.labUserName.Text = myuser.userName;

                            userControl.picMic.Visible = false;     //临时对讲显示麦               
                            userControl.picDel.Visible = true;
                            userControl.picInvite.Visible = true;
                            userControl.picSwitch.Visible = false;  //临时对讲不用切换麦                       

                            //加上后，初设为未进入，让心跳来同步
                            userControl.picUserImg.Image = Properties.Resources.user_icon_offline;

                            userControl.picDel.Tag = userControl;
                            userControl.picSwitch.Tag = userControl;
                            userControl.picInvite.Tag = userControl;

                            userControl.Tag = myuser; //关联

                            userControl.picDel.Click += PicDel_Click;
                            userControl.picSwitch.Click += PicSwitch_Click;
                            userControl.picInvite.Click += PicInvite_Click;

                            flpUserPanel.Controls.Add(userControl);//添加布局 
                                                                   //再邀请同步
                            if (client != null)
                            {
                                //TMD 被邀请的要写在第3个参数上
                                client.SendMessage((new Data()).GroupInviteCommandMessageEncode(groupId, groupName,
                                    myuser.userId, LocalSharedData.CURRENTUser.user_id));
                            }

                        }
                    }


                    //统计人数
                    this.labGroupName.Text = this.groupName + "(" + this.members.Count + ")";


                }
            }
            finally
            {
                crtform = null;
            }

        }
    }
}
