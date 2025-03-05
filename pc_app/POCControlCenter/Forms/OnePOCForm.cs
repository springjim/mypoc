using POCClientNetLibrary;
using POCControlCenter.DataEntity;
using POCControlCenter.Service;
using POCControlCenter.Service.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POCControlCenter
{
    public partial class OnePOCForm : Form
    {

        public  int groupid=0;
        public  string  groupname = "";
        private ChatClient client;
        public bool m_blUIStalk = false;
        public bool m_blUIPlay = false;
        ControlMainForm  mainForm;
        private delegate void UpdateButtonTalkUIStatusDelegate(Control ctrl);
        private delegate void UpdatePlayUIStatusDelegate(Control ctrl, string s);
        public OnePOCForm()
        {
            InitializeComponent();
        }

        public OnePOCForm(ChatClient client)
        {
            InitializeComponent();
            this.client = client;
            pictureBox1.Image = null;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //如果未停止说话,这里要自动关闭
            if (m_blUIStalk)
                buttonTALK.PerformClick();

            if (groupid > 0)
            {               

                ResponseBase result = PocClient.deleteTmpGroup(groupid, 7, 1);
                
                ControlMainForm.SINGLE_POC_ENABLE = false;  //退出对讲单呼模式

                DialogResult = DialogResult.Cancel;


            } else
            {
                ControlMainForm.SINGLE_POC_ENABLE = false;  //退出对讲单呼模式
                DialogResult = DialogResult.Cancel;
            }
        }

        private void buttonTALK_Click(object sender, EventArgs e)
        {
            if (m_blUIStalk == false)
            {
                //发出抢麦信号
                client.SendMessage((new Data(MyType.TYPE_ROB_MIC)).ToByte());
            }
            else
            {
                mainForm =(ControlMainForm) this.Owner;
                mainForm.EndChat();
                client.SendMessage((new Data(MyType.TYPE_REALASE_MIC)).ToByte());

                m_blUIStalk = false;
                client.AEC_State = 0;  //设为初始态
                SetButtonTalkUIStatus();
            }
        }

        private void UpdateButtonTalkUIStatus(Control ctrl)
        {
            if (m_blUIStalk == false)
            {
                buttonTALK.Image = global::POCControlCenter.Properties.Resources.d_speak40;
                buttonTALK.Text = WinFormsStringResource.Main_buttonTALK_Text1;
                pictureBox1.Image = null;
            }
            else
            {
                buttonTALK.Text = WinFormsStringResource.Main_buttonTALK_Text2;
                buttonTALK.Image = global::POCControlCenter.Properties.Resources.d_nospeak40;
                pictureBox1.Image = global::POCControlCenter.Properties.Resources.shengbo_bar1;
            }
        }

        public  void SetButtonTalkUIStatus()
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

        public  void  SetPlayUIStatus(String Name)
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
            //2017.8.11 加入对讲单呼的识别            
            labelPlayerName.Text = Name;
            if (Name.Trim().Equals(""))
                pictureBox1.Image = null;
            else
                pictureBox1.Image = Properties.Resources.shengbo_bar1;


        }

    }
}
