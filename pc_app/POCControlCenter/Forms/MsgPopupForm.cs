using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace POCControlCenter
{

    public partial class MsgPopupForm : Form
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll ")]
        private static extern bool ShowWindowAsync(System.IntPtr hWnd, int cmdShow);                

        [DllImport("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        //下面是可用的常量,按照不合的动画结果声明本身须要的
        private const int AW_HOR_POSITIVE = 0x0001;//自左向右显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记
        private const int AW_HOR_NEGATIVE = 0x0002;//自右向左显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记
        private const int AW_VER_POSITIVE = 0x0004;//自顶向下显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记
        private const int AW_VER_NEGATIVE = 0x0008;//自下向上显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记该标记
        private const int AW_CENTER = 0x0010;//若应用了AW_HIDE标记,则使窗口向内重叠;不然向外扩大
        private const int AW_HIDE = 0x10000;//隐蔽窗口
        private const int AW_ACTIVE = 0x20000;//激活窗口,在应用了AW_HIDE标记后不要应用这个标记
        private const int AW_SLIDE = 0x40000;//应用滑动类型动画结果,默认为迁移转变动画类型,当应用AW_CENTER标记时,这个标记就被忽视
        private const int AW_BLEND = 0x80000;//应用淡入淡出结果

        //private ControlMainForm mainfrm;
        private Form mainfrm;

        //2017.10.12
        //用于区分消息类型: 短文消息，文件消息,视频消息, 围栏警告消息
        //MESSAGE_TEXT,MESSAGE_FILE,MESSAGE_VIDEO,MESSAGE_FENCEALARM
        public string MESSAGE_TYPE = "";   
        

        public MsgPopupForm(Form mainfrm)
        {
            InitializeComponent();
            this.mainfrm = mainfrm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            if (mainfrm != null )
            {
                mainfrm.Activate();
                mainfrm.BringToFront();
                //这里要分析出程序名，不能写死了
                Process[] temp = Process.GetProcessesByName("POCControlCenter");//在所有已启动的进程中查找需要的进程；
                if (temp.Length > 0)//如果查找到
                {
                    IntPtr handle = temp[0].MainWindowHandle;
                    SwitchToThisWindow(handle, true);    // 激活，显示在最前
                }               
               
            }
            if (MESSAGE_TYPE.Equals("MESSAGE_VIDEO"))
            {
                //2018.09.01 以下注释,视频消息会通知到视频窗口
                //mainfrm.setVideoDisplay();
            } else if (MESSAGE_TYPE.Equals("MESSAGE_TEXT") || MESSAGE_TYPE.Equals("MESSAGE_FILE") )
            {
                //2018.09.01 以下注释,视频消息会通知到消息窗口
                //mainfrm.setVoiceDisplay();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private static void SetForegroud(Process instance)

        {

            IntPtr mainFormHandle = instance.MainWindowHandle;

            if (mainFormHandle != IntPtr.Zero)

            {

                ShowWindowAsync(mainFormHandle, 1);

                SetForegroundWindow(mainFormHandle);

            }

        }

        private static Process GetExistProcess()

        {
            Process currentProcess = Process.GetCurrentProcess();
            foreach (Process process in Process.GetProcessesByName(currentProcess.ProcessName))

            {

                if ((process.Id != currentProcess.Id) &&

                    (Assembly.GetExecutingAssembly().Location == currentProcess.MainModule.FileName))

                {

                    return process;

                }

            }

            return null;

        }

        void ActivateApp(string processName)
        {
            Process[] p = Process.GetProcessesByName(processName);

            // Activate the first application we find with this name
            if (p.Count() > 0)
            {
                IntPtr mainFormHandle = p[0].MainWindowHandle;

                if (mainFormHandle != IntPtr.Zero)

                {

                    ShowWindowAsync(mainFormHandle, 1);

                    SetForegroundWindow(mainFormHandle);

                }
                
            }
        }

        private void MsgPopupForm_Load(object sender, EventArgs e)
        {
            
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int top = height - 35 - this.Height;
            int left = width - this.Width - 5;
            this.Top = top;
            this.Left = left;
            this.TopMost = true;

            AnimateWindow(this.Handle, 500, AW_SLIDE | AW_VER_NEGATIVE);
            this.ShowInTaskbar = false;
            Timer_Close.Interval = 4000;
            Timer_Close.Tick += new EventHandler(Timer_Close_Tick);
            Timer_Close.Start();


        }

        /// <summary>  
        /// 关闭窗口的定时器响应事件  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void Timer_Close_Tick(object sender, EventArgs e)
        {
            Timer_Close.Stop();

            this.Close();
        }

        private void MsgPopupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AnimateWindow(this.Handle, 1000, AW_BLEND | AW_HIDE);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>  
        /// 窗口已经关闭  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void FormMessageBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnimateWindow(this.Handle, 1000, AW_SLIDE + AW_VER_POSITIVE + AW_HIDE);


            Timer_Close.Stop();
            Timer_Close.Dispose();
        }


        /// <summary>  
        /// 鼠标移动在消息框上  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void FormMessageBox_MouseMove(object sender, MouseEventArgs e)
        {
            this.Timer_Close.Stop();
        }


        /// <summary>  
        /// 鼠标移动离开消息框上  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void FormMessageBox_MouseLeave(object sender, EventArgs e)
        {
            this.Timer_Close.Start();
        }
    }
}
