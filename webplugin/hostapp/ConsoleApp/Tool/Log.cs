using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp.Tool
{
    public class Log
    {
    
        private static StreamWriter tWriter = null;
        private static readonly object lockObject = new object();

        public static void Open()
        {
            try
            {
                //写日志目录
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                path += "log\\";
                Directory.CreateDirectory(path);

                path += DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
                path += ".log";
                // 使用 FileStream 和 StreamWriter 直接写入文件
                tWriter = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    AutoFlush = true // 设置自动刷新
                };

            }
            catch (Exception e)
            {
                Debug.Assert(false);
                //Console.WriteLine(e.ToString());
            }
        }

        public static void Close()
        {
            lock (lockObject)
            {
                if (null != tWriter)
                {
                    tWriter.Flush();
                    tWriter.Close();
                    tWriter = null;
                }
            }
        }

        private static string GetParentMethod()
        {
            StackTrace stackTrace = new StackTrace(true);
            MethodBase methodBase = stackTrace.GetFrame(2).GetMethod(); // 获取调用者的方法信息

            // 取得父方法类全名
            string parentMethod = methodBase.DeclaringType.FullName;

            // 分隔符
            parentMethod += ".";

            // 取得父方法名
            parentMethod += methodBase.Name;

            return parentMethod;
        }

        public static void I(string content)
        {
            string parentMethod = GetParentMethod();
            Write(parentMethod, content);
        }

        private static string getProcessId()
        {
            // 获取当前进程的实例
            Process currentProcess = Process.GetCurrentProcess();
            // 获取并显示进程ID
            int processId = currentProcess.Id;
            return processId + "";

        }

        public static void E(string content)
        {
            string parentMethod = GetParentMethod();
            Write(parentMethod, content);
        }

        private static void Write(string parentMethod, string content)
        {
            string msg = String.Format(" [进程ID={4}], [{0}][{1}], [{2}], [{3}]"
                , System.Threading.Thread.CurrentThread.ManagedThreadId
                , DateTime.Now.ToString("MM-dd HH:mm:ss.fff")
                , parentMethod
                , content,
                getProcessId()
                );           
          
            lock (lockObject)
            {
                if (null != tWriter)
                {
                  tWriter.WriteLine(msg);
                }
            }

        }
    }

}
