using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vlc.DotNet.Core.Interops.Signatures;
using Vlc.DotNet.Forms;
using CefSharp;
using CefSharp.WinForms;


namespace POCControlCenter
{
    public partial class MediaPlayForm : Form
    {
        //public VlcControl vlcControl1;

        VlcPlayer vlcplay;
        ChromiumWebBrowser webMap = null;

        public string avUrl;
        public string webUrl;

        public Image shengbo_file;
        /// <summary>
        /// 表示是否手动拖动播放进度条
        /// </summary>
        Boolean enterSeek = false;

        /// <summary>
        /// 当前媒体类型: video, audio, sos, other等
        /// 表示当前TabControl进入的类型
        /// </summary>
        public string currMediaType = "video";

        public string lng="";  //经度,用于地图上mark
        public string lat="";  //纬度,用于地图上mark
        public string username_str = "";  //姓名,用于地图上mark

        /// <summary>
        /// 描述当前正在播放的类型
        /// </summary>
        public string currMediaPlayingType = "";

        /// <summary>
        /// 当前播放的位置, 视频
        /// </summary>
        public int currMediaIndex_Video = -1;

        /// <summary>
        /// 当前播放的位置, 语音
        /// </summary>
        public int currMediaIndex_Audio = -1;

        /// <summary>
        /// 记录trackbar的位置
        /// </summary>
        int currTrackBarIndex_Video = 0;
        int currTrackBarIndex_Audio = 0;

        public MediaPlayForm()
        {
            InitializeComponent();            
        }

        public void SetAVUrl(string avurl)
        {
            this.avUrl = avurl;
        }

        public void SetWebUrl(string weburl)
        {
            this.webUrl = weburl;
        }

      
        private void WebBrower_LoadError(object sender, LoadErrorEventArgs e)
        {
            MessageBox.Show("脚本错误，" + e.ErrorText);
        }

        public MediaPlayForm(string avurl,string weburl)
        {
            InitializeComponent();

            this.avUrl = avurl;
            if (weburl != "")
            {
                webMap = new ChromiumWebBrowser(weburl);
                webMap.Dock = DockStyle.Fill;// 填充方式

                CefSharpSettings.LegacyJavascriptBindingEnabled = true;//新cefsharp绑定需要优先申明            
                                                                       //webMap.IsBrowserInitializedChanged += WebBrower_IsBrowserInitializedChanged;
                webMap.LoadError += WebBrower_LoadError;
                this.panel5.Controls.Add(webMap);
            }            

        }


        public MediaPlayForm(string avurl)
        {
            InitializeComponent();
            
            this.avUrl = avurl;
            //vlcControl1 = new Vlc.DotNet.Forms.VlcControl();

            

            //foreach (Control control in panelContainer.Controls)
            //{
            //    if (control is VlcControl)
            //        panelContainer.Controls.Remove(control);
            //}

            //panelContainer.Controls.Add(vlcControl1);
            //vlcControl1.Dock = DockStyle.Fill;
            //vlcControl1.BackColor = Color.Gray;
            //vlcControl1.VlcLibDirectoryNeeded += VlcControl1_VlcLibDirectoryNeeded;


            //vlcControl1.Opening += VlcControl1_Opening;
            //vlcControl1.EndReached += VlcControl1_EndReached;
            //vlcControl1.EncounteredError += VlcControl1_EncounteredError;
            //vlcControl1.Playing += VlcControl1_Playing;
            //vlcControl1.TimeChanged += VlcControl1_TimeChanged;
            //vlcControl1.Paused += VlcControl1_Paused;

            //vlcControl1.EndInit();
            ////
            //vlcControl1.BringToFront();

        }       

        private void MediaPlayForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (vlcplay != null)
                {                   
                    vlcplay.Stop(); 
                }
                this.Hide();

            }
            catch (MissingMethodException me)
            {
                MessageBox.Show(me.Message);
            } catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
            }

        }

        private void MediaPlayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (vlcControl1 != null && (vlcControl1.State == MediaStates.Opening
               || vlcControl1.State == MediaStates.Playing
               ))
            {
                //vlcControl1.Pause();
                MessageBox.Show("有媒体在播放，请先关闭它");
                e.Cancel = true;
            }
            else e.Cancel = false;
            */
        }

        private void MediaPlayForm_Load(object sender, EventArgs e)
        {
            shengbo_file = Image.FromFile(Path.Combine(System.Environment.CurrentDirectory, "shengbo_bar.gif"));
            if (avUrl != "")
            {

                if (currMediaType.Equals("audio"))
                {
                    //this.vlcControl1.BackgroundImage = null;
                    pictureBox1.Image = shengbo_file;
                    picVideo.Visible = false;
                    pictureBox1.Visible = true;
                    tableLayoutPanelMap.Visible = false;

                }
                else if (currMediaType.Equals("video"))
                {
                    //this.vlcControl1.BackgroundImage = null;
                    pictureBox1.Image = null;
                    picVideo.Visible = true;
                    pictureBox1.Visible = false;
                    tableLayoutPanelMap.Visible = false;
                }
                else if (currMediaType.Equals("sos"))
                {
                    //this.vlcControl1.BackgroundImage = null;                    

                    pictureBox1.Image = null;
                    picVideo.Visible = false;
                    pictureBox1.Visible = false;
                    tableLayoutPanelMap.Visible = true;
                }

                //vlcControl1.Play(avUrl);
                if (currMediaType.Equals("audio") || (currMediaType.Equals("video")) )
                {

                    vlcplay = new VlcPlayer();
                    vlcplay.SetRenderWindow(picVideo.Handle.ToInt32());
                    vlcplay.PlayURL(avUrl);
                    //
                    timerVlc.Enabled = true; //开始定时分析

                }                   

            }
        }

        private delegate void OperByVLCEventDelegate();
        private delegate void UpdateCurrTimeByVLCEventDelegate(long newtime);

        private void VlcControl1_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            e.VlcLibDirectory = new DirectoryInfo(System.Environment.CurrentDirectory);
            //
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;
            if (AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture == ProcessorArchitecture.X86)
                //e.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\lib\x86\"));
                e.VlcLibDirectory = new DirectoryInfo(System.Environment.CurrentDirectory);
            else
            {
                MessageBox.Show("未包vlc的x64的库支持");
                //e.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\lib\x64\"));
            }

            //if (!e.VlcLibDirectory.Exists)
            //{
            //    var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            //    folderBrowserDialog.Description = "Select Vlc libraries folder.";
            //    folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            //    folderBrowserDialog.ShowNewFolderButton = true;
            //    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        e.VlcLibDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath);
            //    }
            //}

        }

        private void VlcControl1_Opening(object sender, Vlc.DotNet.Core.VlcMediaPlayerOpeningEventArgs e)
        {
            /*
            if (this.vlcControl1.InvokeRequired == false)
            {
                execMediaState_opened();
            }
            else
            {
                OperByVLCEventDelegate DMSGD = new OperByVLCEventDelegate(execMediaState_opened);
                if (this.vlcControl1.IsHandleCreated)
                    this.vlcControl1.Invoke(DMSGD);
            }
            */

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

        private void execMediaState_timechange(long newtime)
        {
            //media_state.Text = newtime.ToString();
            if (!enterSeek)
            {
                if ((Int32)newtime / 1000 > trackBar1.Maximum)
                    trackBar1.Value = trackBar1.Maximum;
                else
                    trackBar1.Value = (Int32)newtime / 1000;
            }

            if (this.currMediaType.Equals("video"))
                this.currTrackBarIndex_Video = trackBar1.Value;
            else
                this.currTrackBarIndex_Audio = trackBar1.Value;

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
                this.media_state.Invoke(DMSGD, new object[] { e.NewTime });
            }
        }

        private void MediaState_paused()
        {
            /*
            this.media_state.Text = "已暂停";
            if (this.currMediaType.Equals("video") && this.currTrackBarIndex_Video > 0)
            {
                this.trackBar1.Value = currTrackBarIndex_Video;

            }
            else if (this.currMediaType.Equals("audio") && this.currTrackBarIndex_Audio > 0)
            {
                this.trackBar1.Value = currTrackBarIndex_Audio;
                this.vlcControl1.BackgroundImage = null;
            }
            */

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

        private void execMediaState_opened()
        {
            trackBar1.Enabled = false;
            media_state.Text =
                            WinFormsStringResource.MediaState_opened;
            /*
            if (currMediaType.Equals("audio"))
            {
                this.vlcControl1.BackgroundImage = null; 
                pictureBox1.Image = shengbo_file;
                vlcControl1.Visible = false;
                pictureBox1.Visible = true;
                tableLayoutPanelMap.Visible = false;

            } else if (currMediaType.Equals("video"))
            {
                this.vlcControl1.BackgroundImage = null;
                pictureBox1.Image = null;
                vlcControl1.Visible = true;
                pictureBox1.Visible = false;
                tableLayoutPanelMap.Visible = false;
            } else if (currMediaType.Equals("sos"))
            {
                this.vlcControl1.BackgroundImage = null;
                pictureBox1.Image = null;
                vlcControl1.Visible = false;
                pictureBox1.Visible = false;
                tableLayoutPanelMap.Visible = true;
            }
            */

        }
        private void MediaState_errored()
        {
            /*
            trackBar1.Enabled = false;
            media_state.Text =
               WinFormsStringResource.MediaState_errored;
            //
            vlcControl1.BackgroundImage = null;
            button3.Enabled = false;
            button1.Enabled = false;
            */

        }

        private void MediaState_playing()
        {
            /*
            trackBar1.Enabled = true;
            media_state.Text =
                WinFormsStringResource.MediaState_playing;
            if (currMediaType.Equals("audio"))
            {
                this.vlcControl1.BackgroundImage = null;
                pictureBox1.Image = shengbo_file;
                vlcControl1.Visible = false;
                pictureBox1.Visible = true;
                tableLayoutPanelMap.Visible = false;
            }
            else if (currMediaType.Equals("video"))
            {
                this.vlcControl1.BackgroundImage = null;
                pictureBox1.Image = null;
                vlcControl1.Visible = true;
                pictureBox1.Visible = false;
                tableLayoutPanelMap.Visible = false;

            } else if (currMediaType.Equals("sos"))
            {

                this.vlcControl1.BackgroundImage = null;
                pictureBox1.Image = null;
                vlcControl1.Visible = false;
                pictureBox1.Visible = false;
                tableLayoutPanelMap.Visible = true;

            }
            */
        }

        private void MediaState_ended()
        {
            /*
            media_state.Text =
                WinFormsStringResource.MediaState_ended;
            if (currMediaType.Equals("audio"))
            {
                this.vlcControl1.BackgroundImage = null;
                this.pictureBox1.Visible = true;
                this.vlcControl1.Visible = false;
                this.pictureBox1.Image = null;
                tableLayoutPanelMap.Visible = false;

            }
            else if (currMediaType.Equals("video"))
            {
                this.vlcControl1.BackgroundImage = null;
                this.pictureBox1.Visible = false;
                this.vlcControl1.Visible = true;
                tableLayoutPanelMap.Visible = false;

            }
            else if (currMediaType.Equals("sos"))
            {
                this.vlcControl1.BackgroundImage = null;
                this.pictureBox1.Visible = false;
                this.vlcControl1.Visible = false;
                tableLayoutPanelMap.Visible = true;

            }
            */
        }

        private void trackBar1_Enter(object sender, EventArgs e)
        {
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

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            
            if (vlcplay!=null &&  vlcplay.IsPlaying())
            {
                vlcplay.SetPlayPos((float)(trackBar1.Value /(1.0*trackBar1.Maximum)));
            }           


        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            if (vlcplay != null && vlcplay.IsPlaying())
            {
                vlcplay.Pause();
                button3.Text = "播放";
            } else if (vlcplay != null && !vlcplay.IsPlaying())
            {
                vlcplay.Play();
                button3.Text = "暂停";
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            //
            if (vlcControl1 != null && vlcControl1.IsPlaying)
            {               
                vlcControl1.Pause();
                vlcControl1.Time = 0l;
                vlcControl1.Play();
            }
            else if (vlcControl1 != null && !vlcControl1.IsPlaying)
            {
                vlcControl1.Time = 0l;
                vlcControl1.Play();
                
            }
            button3.Text = "暂停";
            */

        }

        private void MediaPlayForm_Resize(object sender, EventArgs e)
        {
            //
            if (currMediaType == "sos")
            {
                //if (lng!="" && lat!="" )
                //webMap.Navigate(HttpAPI.SOS_MAP_LOACTION_URL + "?lng=" + lng
                //            + "&lat=" + lat + "&place=" + HttpAPI.ToUrlEncode(username_str));

            }

        }

        private void vlcControl1_Stopped(object sender, Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs e)
        {
            //

        }

        private void button2_Click(object sender, EventArgs e)
        {
           

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1 )
            {
                if (vlcplay != null && vlcplay.IsPlaying())
                {
                    //MessageBox.Show(comboBox1.SelectedItem.ToString());
                    if (comboBox1.SelectedIndex != 2)
                        vlcplay.SetVideoAspectRatio(comboBox1.SelectedItem.ToString());
                    else
                        vlcplay.SetVideoAspectRatio_Origin();
                }
            }
        }

        private void timerVlc_Tick(object sender, EventArgs e)
        {
            //
            if (vlcplay != null)
            {
                if (vlcplay.IsPlaying())
                {
                    button3.Text = "暂停";
                    media_state.Text = "播放中";
                    trackBar1.Value =(int) vlcplay.GetPlayTime();

                } else
                {
                    media_state.Text = "已停止";
                }
                //
                

            }

        }
    }
}
