using NAudio.Wave;
using POCControlCenter.Trtc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter.BroadCast
{
    public partial class RecorderForm : Form
    {

        private WaveIn waveIn;    // waveIn操作类
        private WaveFormat  recordingFormat;    // 录音格式
        private WaveFileWriter writer;    // 录音文件操作类
        public event EventHandler StoppedEvent = delegate { };    // 录音结束事件
        public event EventHandler DataAvailableEvent = delegate { }; // 录音过程中接收到数据事件

        private bool startFlag;
        private string recordFilePath = "";

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

        public double RecordedTime        // 获取到录音的时长
        {
            get
            {
                if (writer == null)
                    return 0;
                return (double)writer.Length / writer.WaveFormat.AverageBytesPerSecond;
            }
        }


        private int Full_RecordDeviceIndex = 0;
        public RecorderForm(int recordDeviceIndex)
        {
            InitializeComponent();
            this.Full_RecordDeviceIndex = recordDeviceIndex;
            this.startFlag = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="filename">保存的文件名</param>
        internal bool StartRecorder(string filename)
        {
            
            // 设置录音格式
            recordingFormat = new WaveFormat(8000, 16, 1);
            // 设置麦克风操作对象
            waveIn = new WaveIn();
            waveIn.DeviceNumber = Full_RecordDeviceIndex;    // 设置使用的录音设备
            waveIn.BufferMilliseconds = 20;

            waveIn.DataAvailable += OnDataAviailable;        // 接收到音频数据时，写入文件
            waveIn.RecordingStopped += OnRecordingStopped;   // 录音结束时执行
            waveIn.WaveFormat = recordingFormat;
            // 设置文件操作类
            writer = new WaveFileWriter(filename, recordingFormat);
            // 开始录音
            waveIn.StartRecording();

            return true;
        }

        /// <summary>
        /// 结束录音
        /// </summary>
        /// <returns></returns>
        internal bool StopRecorder()
        {
            waveIn.StopRecording();
            return true;
        }

        /// <summary>
        /// 录音结束回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (writer != null)
                writer.Dispose();
            writer = null;
            waveIn.Dispose();

            this.picGif.Image = null;
            // 通知结束事件
            StoppedEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// 录音回调函数，写入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataAviailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            int bytesRecorded = e.BytesRecorded;
            if (writer != null)
                writer.Write(e.Buffer, 0, e.BytesRecorded); ;    // 音频数据写入文件
            
            DataAvailableEvent(this, EventArgs.Empty);
        }


        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (!this.startFlag)
            {
                //开始
                SaveFileDialog sfd = new SaveFileDialog();
                //设置保存文件对话框的标题
                sfd.Title = "请选择要保存的文件路径";
                //初始化保存目录，默认exe文件目录
                sfd.InitialDirectory = Application.StartupPath;
                //设置保存文件的类型
                sfd.Filter = "音频文件|*.wav";
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return ;
                }

                this.picGif.Image = Properties.Resources.shengbo_bar;               
                recordFilePath = sfd.FileName;
                StartRecorder(recordFilePath);
                this.startFlag = true;
                btnRecord.Text = "结束录音";
                btnRecord.BackColor = System.Drawing.Color.Red;
               

            } else
            {
                this.StopRecorder();
                this.startFlag = false;
                btnRecord.Text = "开始录音";
                btnRecord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));

                MessageForm msgBox = new MessageForm();
                msgBox.setText("录音完成, 文件位置: \n\r"+ recordFilePath, 1500);
                msgBox.setCancelBtn(false);
                msgBox.ShowDialog();
            }
        }

        private void btnTxtSpeech_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbText.Text.Trim()))
            {
                tbText.Focus();
                MessageBox.Show("请输入文字");
                return;
            }

            //开始
            SaveFileDialog sfd = new SaveFileDialog();
            //设置保存文件对话框的标题
            sfd.Title = "请选择要保存的文件路径";
            //初始化保存目录，默认exe文件目录
            sfd.InitialDirectory = Application.StartupPath;
            //设置保存文件的类型
            sfd.Filter = "音频文件|*.wav";
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            recordFilePath = sfd.FileName;
            using (Stream ret = new MemoryStream())
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {

                synth.GetInstalledVoices().ToList().ForEach(voice =>

                {
                    Debug.WriteLine("Name:{0}, Gender:{1}, Age:{2}",
          voice.VoiceInfo.Description, voice.VoiceInfo.Gender, voice.VoiceInfo.Age);

                });

                if (radioButton1.Checked)
                    synth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);
                    //synth.SelectVoice("Microsoft Kangkang");
                else if (radioButton2.Checked)
                    synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
                //synth.SelectVoice("Microsoft Huihui");

                synth.Volume = 100;

                Debug.WriteLine("选中 Name:{0}, Gender:{1}, Age:{2}", synth.Voice.Name, synth.Voice.Gender,
                     synth.Voice.Age );

                var mi = synth.GetType().GetMethod("SetOutputStream", BindingFlags.Instance | BindingFlags.NonPublic);
                var fmt = new SpeechAudioFormatInfo(8000, AudioBitsPerSample.Eight, AudioChannel.Mono);
                mi.Invoke(synth, new object[] { ret, fmt, true, true });
                synth.Speak(tbText.Text.Trim());
                // Testing code:
                using (var fs = new FileStream(recordFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    ret.Position = 0;
                    byte[] buffer = new byte[4096];
                    for (; ; )
                    {
                        int len = ret.Read(buffer, 0, buffer.Length);
                        if (len == 0) break;
                        fs.Write(buffer, 0, len);
                    }
                }
            }

            // 
            MessageForm msgBox = new MessageForm();
            msgBox.setText("录音完成, 文件位置: \n\r" + recordFilePath, 1500);
            msgBox.setCancelBtn(false);
            msgBox.ShowDialog();


        }
    }
}
