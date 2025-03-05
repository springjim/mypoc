using agora.rtc;
using POCControlCenter.Service;
using POCControlCenter.Trtc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace POCControlCenter.Agora.Meeting
{
    public partial class AgoraScreenForm : Form
    {
        private AgoraMeetForm  mMainForm;
        private IAgoraRtcEngine rtc_engine_;

        private ScreenCaptureSourceInfo[]  mScreenList;
        private ImageList mImageList;

        private bool mIsMouseDown = false;
        private System.Drawing.Point mFormLocation;     // Form的location
        private System.Drawing.Point mMouseOffset;      // 鼠标的按下位置

        Dictionary<string, ulong> winDict = new Dictionary<string, ulong>();

        public AgoraScreenForm(AgoraMeetForm form, IAgoraRtcEngine rtc_engine_)
        {
            InitializeComponent();

            mMainForm = form;
            this.rtc_engine_ = rtc_engine_;

            mImageList = new ImageList();
            mImageList.ImageSize = new System.Drawing.Size(250, 146);

        }

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
                System.Drawing.Point pt = Control.MousePosition;
                int x = mMouseOffset.X - pt.X;
                int y = mMouseOffset.Y - pt.Y;

                this.Location = new System.Drawing.Point(mFormLocation.X - x, mFormLocation.Y - y);
            }
        }

        private void AgoraScreenForm_Load(object sender, EventArgs e)
        {

            agora.rtc.SIZE thumbSize = new agora.rtc.SIZE() { width = 200, height = 200 };
            agora.rtc.SIZE iconSize = new agora.rtc.SIZE() { width = 30, height = 30 };

            mScreenList = rtc_engine_.GetScreenCaptureSources(new agora.rtc.SIZE(200, 200), new agora.rtc.SIZE(30, 30), true);
            for (uint i = 0; i < mScreenList.Length; i++)
            {
                ScreenCaptureSourceInfo sourse= mScreenList[i];
                string name;

                if (sourse.type.Equals(ScreenCaptureSourceType.ScreenCaptureSourceType_Screen))
                    name = "显示器-["+sourse.sourceId+"]";
                
                else
                    name = sourse.sourceName +"["+ sourse.sourceId+"]";

                //
                // 设置屏幕缩略图
                int width = (int)sourse.thumbImage.width;
                int height = (int)sourse.thumbImage.height;
                if (width == 0)
                    width = 270;

                if (height == 0)
                    height = 146;

                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppRgb);
                if (sourse.thumbImage.length <= 0)
                {
                    // 未找到缩略图，不显示
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);
                    }                   
                    mImageList.Images.Add(name, bmp);
                    winDict.Add(name, sourse.sourceId);
                    continue;
                }
                BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

                int stride = bmpData.Stride;
                IntPtr iptr = bmpData.Scan0;
                int scanBytes = stride * height;
                int posScan = 0, posReal = 0;
                byte[] pixelValues = new byte[scanBytes];

                for (int j = 0; j < sourse.thumbImage.buffer.Length; j++)
                    pixelValues[posScan++] = sourse.thumbImage.buffer[posReal++];

                Marshal.Copy(pixelValues, 0, iptr, scanBytes);
                bmp.UnlockBits(bmpData);
               
                mImageList.Images.Add(name, bmp);
                winDict.Add(name, sourse.sourceId);

            }

            this.screenListView.LargeImageList = mImageList;
            this.screenListView.BeginUpdate();
            for (int i = 0; i < mImageList.Images.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.ImageIndex = i;
                item.Text = mImageList.Images.Keys[i];               

                this.screenListView.Items.Add(item);
            }
            this.screenListView.EndUpdate();
            this.screenListView.HideSelection = true;
            if (this.screenListView.Items.Count > 0)
            {
                this.screenListView.Items[0].Selected = true;
                this.screenListView.Select();
            }

        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            mMainForm.OnSetScreenParamsCallback(null,false);
            this.Close();
        }

        private void confirmBtn_Click(object sender, EventArgs e)
        {
            if (this.screenListView.SelectedItems.Count == 0)
            {
                MessageForm msg = new MessageForm();
                msg.setCancelBtn(false);
                msg.setText("请选择一个需要共享的屏幕！");
                msg.ShowDialog();
                return;
            }

            //判断位置
            string name = this.screenListView.SelectedItems[0].Text;

            if (!this.winDict.TryGetValue(name, out ulong sourceId))
                return;

            Console.WriteLine("sourceId=" + sourceId);

            ScreenCaptureSourceInfo sourceinfo= null;
            for (uint i = 0; i < mScreenList.Length; i++)
            {
                ScreenCaptureSourceInfo info = mScreenList[i];
                if (sourceId.Equals(mScreenList[i].sourceId))
                {
                    sourceinfo = info;
                    break;
                }

            }
            //           

            if (sourceinfo!=null)
                mMainForm.OnSetScreenParamsCallback(sourceinfo,true);

            this.Close();

        }

        private void exitPicBox_Click(object sender, EventArgs e)
        {
            mMainForm.OnSetScreenParamsCallback(null,false);
            this.Close();
        }
    }
}
