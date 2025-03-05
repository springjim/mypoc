using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter
{
    class ExeToWinform
    {
        EventHandler appIdleEvent = null;
        Control ParentCon = null;
        string strGUID = "";
        string workdir = "";

        ControlMainForm mainform;

        public ExeToWinform(Control C, string Titlestr, ControlMainForm mainform, string workdir)
        {
            appIdleEvent = new EventHandler(Application_Idle);
            ParentCon = C;
            strGUID = Titlestr;
            this.mainform = mainform;
            this.workdir = workdir;
        }

        /// <summary>
        /// 将属性<code>AppFilename</code>指向的应用程序打开并嵌入此容器
        /// </summary>
        public IntPtr Start(string FileNameStr)
        {
            if (m_AppProcess != null)
            {
                Stop();
            }
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(FileNameStr);
                //info.UseShellExecute = true;
                info.UseShellExecute = false;
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.WorkingDirectory = workdir;
                //info.WindowStyle = ProcessWindowStyle.Normal;

                m_AppProcess = System.Diagnostics.Process.Start(info);
                string MainWindowTitle = m_AppProcess.MainWindowTitle;
                m_AppProcess.WaitForInputIdle();
                Application.Idle += appIdleEvent;
            }
            catch
            {
                if (m_AppProcess != null)
                {
                    if (!m_AppProcess.HasExited)
                        m_AppProcess.Kill();
                    m_AppProcess = null;
                }
            }
            return m_AppProcess.Handle;
        }
        /// <summary>
        /// 确保应用程序嵌入此容器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Idle(object sender, EventArgs e)
        {
            if (this.m_AppProcess == null || this.m_AppProcess.HasExited)
            {
                this.m_AppProcess = null;
                Application.Idle -= appIdleEvent;
                return;
            }

            while (m_AppProcess.MainWindowHandle == IntPtr.Zero)
            {
                Thread.Sleep(100);
                m_AppProcess.Refresh();
            }
            Application.Idle -= appIdleEvent;
            mainform.MAPINFO_HANDLE = m_AppProcess.MainWindowHandle.ToInt32();
            EmbedProcess(m_AppProcess, ParentCon);
        }
        /// <summary>
        /// 应用程序结束运行时要清除这里的标识
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_AppProcess_Exited(object sender, EventArgs e)
        {
            m_AppProcess = null;
        }
        /// <summary>
        /// 将属性<code>AppFilename</code>指向的应用程序关闭
        /// </summary>
        public void Stop()
        {
            if (m_AppProcess != null)// && m_AppProcess.MainWindowHandle != IntPtr.Zero)
            {
                try
                {
                    if (!m_AppProcess.HasExited)
                        m_AppProcess.Kill();
                }
                catch (Exception)
                {
                }
                m_AppProcess = null;
            }
        }


        #region 属性
        /// <summary>
        /// application process
        /// </summary>
        Process m_AppProcess = null;

        /// <summary>
        /// 标识内嵌程序是否已经启动
        /// </summary>
        public bool IsStarted { get { return (this.m_AppProcess != null); } }

        #endregion 属性

        #region Win32 API
        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall)]
        private static extern long GetWindowThreadProcessId(long hWnd, long lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        private static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        public static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            }
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr32(HandleRef hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hwnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetParent(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SWP_NOOWNERZORDER = 0x200;
        private const int SWP_NOREDRAW = 0x8;
        private const int SWP_NOZORDER = 0x4;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int WS_EX_MDICHILD = 0x40;
        private const int SWP_FRAMECHANGED = 0x20;
        private const int SWP_NOACTIVATE = 0x10;
        private const int SWP_ASYNCWINDOWPOS = 0x4000;
        private const int SWP_NOMOVE = 0x2;
        private const int SWP_NOSIZE = 0x1;
        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 0x10000000;
        private const int WM_CLOSE = 0x10;
        private const int WS_CHILD = 0x40000000;

        private const int SW_HIDE = 0; //{隐藏, 并且任务栏也没有最小化图标}
        private const int SW_SHOWNORMAL = 1; //{用最近的大小和位置显示, 激活}
        private const int SW_NORMAL = 1; //{同 SW_SHOWNORMAL}
        private const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}
        private const int SW_SHOWMAXIMIZED = 3; //{最大化, 激活}
        private const int SW_MAXIMIZE = 3; //{同 SW_SHOWMAXIMIZED}
        private const int SW_SHOWNOACTIVATE = 4; //{用最近的大小和位置显示, 不激活}
        private const int SW_SHOW = 5; //{同 SW_SHOWNORMAL}
        private const int SW_MINIMIZE = 6; //{最小化, 不激活}
        private const int SW_SHOWMINNOACTIVE = 7; //{同 SW_MINIMIZE}
        private const int SW_SHOWNA = 8; //{同 SW_SHOWNOACTIVATE}
        private const int SW_RESTORE = 9; //{同 SW_SHOWNORMAL}
        private const int SW_SHOWDEFAULT = 10; //{同 SW_SHOWNORMAL}
        private const int SW_MAX = 10; //{同 SW_SHOWNORMAL}

        #endregion Win32 API

        /// <summary>
        /// 将指定的程序嵌入指定的控件
        /// </summary>
        private void EmbedProcess(Process app, Control control)
        {
            // Get the main handle
            if (app == null || app.MainWindowHandle == IntPtr.Zero || control == null) return;
            try
            {
                // Put it into this form
                SetParent(app.MainWindowHandle, control.Handle);
            }
            catch (Exception)
            { }
            try
            {
                // Remove border and whatnot               
                SetWindowLong(new HandleRef(this, app.MainWindowHandle), GWL_STYLE, WS_VISIBLE);
                SendMessage(app.MainWindowHandle, WM_SETTEXT, IntPtr.Zero, strGUID);
            }
            catch (Exception)
            { }
            try
            {
                // Move the window to overlay it on this window
                MoveWindow(app.MainWindowHandle, 0, 0, control.Width, control.Height, true);
            }
            catch (Exception)
            { }
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        const int WM_SETTEXT = 0x000C;

    }
}
