using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter
{
    public partial class DownFilesForm : Form
    {

        //下载类型
        private string PATH_TYPE = "";  //video,audio,session
        private string PATH_DOWNLOAD = "";
        private List<string>  downfilearr = new List<string>();
        /// <summary>
        /// 保存的名字列表
        /// </summary>
        private List<string> downfilearr_savefile = new List<string>();

        private bool DOWN_FINISH_STATE = false;    //为false表示未下载完成
        private bool DOWN_PROCESS_BREAK = false;   //是否打断了下载过程

        //
        private int TOTAL_FILEINDEX = 0;
        private string fileFullName = "";

        public DownFilesForm(string path_type,string path_download)
        {
            InitializeComponent();
            PATH_TYPE = path_type;
            PATH_DOWNLOAD = path_download;

        }

        public void setDownFiles(List<string> filepaths)
        {
            downfilearr.Clear();
            downfilearr.AddRange(filepaths);
        }

        /// <summary>
        /// 用于session
        /// </summary>
        /// <param name="filepaths"></param>
        /// <param name="savePaths"></param>
        public void setDownFiles2(List<string> filepaths, List<string> savePaths)
        {
            downfilearr.Clear();
            downfilearr.AddRange(filepaths);
            //
            downfilearr_savefile.Clear();
            downfilearr_savefile.AddRange(savePaths);

        }

        private void btnState_Click(object sender, EventArgs e)
        {
            //
            if (!DOWN_FINISH_STATE)
            {
                //取消操作
                DialogResult result_q = MessageBox.Show("确定要取消下载吗?",
                   WinFormsStringResource.PromptStr, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result_q == DialogResult.No)
                {
                    return;
                }
                //
                DOWN_PROCESS_BREAK = true;
                Thread.Sleep(1000);
                this.Close();

            } else
            {

                //已经下载完成
                this.Close();
            }

        }

        /// <summary>
        /// 显示进度
        /// </summary>
        /// <param name="val"></param>
        private void ProgressBar_Value(long val, long total_val)
        {
            progressBarFile.Maximum = Convert.ToInt32(total_val/1024);
            progressBarFile.Value = Convert.ToInt32(val / 1024);
            label5.Text = Math.Round(val / 1024.0,1).ToString()+"kb / "+ Math.Round(total_val / 1024.0, 1).ToString()+"kb";
        }


        private void ProgressTotal_Value(int val)
        {
            if (progressBarFile.Maximum == 0)
            {
                progressBarFile.Maximum = 1;
                progressBarFile.Value = progressBarFile.Maximum;
            } else
                progressBarFile.Value = progressBarFile.Maximum;

            TOTAL_FILEINDEX = TOTAL_FILEINDEX + 1;
            progressBarTotal.Value = TOTAL_FILEINDEX;
            Console.WriteLine("progressBarTotal.Value=" + TOTAL_FILEINDEX);    
            if (TOTAL_FILEINDEX== progressBarTotal.Maximum)
            {
                //下载全完成了
                this.DOWN_FINISH_STATE = true;
                btnState.Text = "完成";
                btnView.Visible = true;

            }     
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savefile"></param>
        /// <param name="downloadProgressChanged"></param>
        /// <param name="downloadFileCompleted"></param>
        private void DownloadFile(string url, string savefile, Action<long,long> downloadProgressChanged, Action<int> downloadFileCompleted, int total_index)
        {
            if (this.IsHandleCreated)
            {
                WebClient client = new WebClient();

                if (downloadProgressChanged != null)
                {
                    client.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                    {
                        if (!DOWN_PROCESS_BREAK)
                            this.Invoke(downloadProgressChanged, e.BytesReceived, e.TotalBytesToReceive);
                    };
                }
                if (downloadFileCompleted != null)
                {
                    client.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                    {
                        fileFullName = savefile;
                        if (!DOWN_PROCESS_BREAK)
                            this.Invoke(downloadFileCompleted, total_index);
                    };
                }

                //打断后，不下载
                if (!DOWN_PROCESS_BREAK)
                    client.DownloadFileAsync(new Uri(url), savefile);
            }                      

        }

        delegate void Action(); //.NET Framework 2.0得自定义委托Action

        private void DownFilesForm_Shown(object sender, EventArgs e)
        {
            //
            btnState.Text = "取消";
            btnView.Visible = false;
            this.DOWN_FINISH_STATE = false;

            label2.Text = this.PATH_DOWNLOAD;
            label5.Text = "";
            if (!System.IO.Directory.Exists(PATH_DOWNLOAD))
                System.IO.Directory.CreateDirectory(PATH_DOWNLOAD);
            //
            progressBarTotal.Minimum = 0;
            progressBarTotal.Maximum = downfilearr.Count;
            progressBarFile.Minimum = 0;
            progressBarFile.Maximum = 100; //因为DownloadFile用百分比来回调
            //
            string savefile = "";
            for(int i=0; i<downfilearr.Count;i++)
            {

                if (PATH_TYPE == "session")
                {
                    savefile = downfilearr_savefile[i];
                    savefile = System.IO.Path.Combine(PATH_DOWNLOAD, savefile);
                    DownloadFile(downfilearr[i], savefile, ProgressBar_Value, ProgressTotal_Value, i + 1);
                }
                else
                {
                    //获取存储文件
                    savefile = downfilearr[i].Substring(downfilearr[i].LastIndexOf('/') + 1);
                    savefile = System.IO.Path.Combine(PATH_DOWNLOAD, savefile);
                    DownloadFile(downfilearr[i], savefile, ProgressBar_Value, ProgressTotal_Value, i + 1);
                    //progressBarTotal.Value = i + 1;    
                }                            

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //
            if (DOWN_FINISH_STATE && !fileFullName.Equals(""))
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
                psi.Arguments = "/e,/select," + fileFullName;
                System.Diagnostics.Process.Start(psi);

            }

        }
    }
}
