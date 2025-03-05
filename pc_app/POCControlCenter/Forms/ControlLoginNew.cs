using POCClientNetLibrary;
using POCControlCenter.DataEntity;
using POCControlCenter.Service;
using POCControlCenter.Service.Entity;
using POCControlCenter.Service.Model;
using POCControlCenter.Trtc;
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
    public partial class ControlLoginNew : Form
    {

        public String mstrAccount;
        public String mstrPassword;
        public string loginwelcome_caption;
        static int COUNT = 0;
        ChatClient client;

        public ControlLoginNew()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 以下是解决窗口显示的闪烁问题
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public ControlLoginNew(ChatClient client, string welcome)
        {           
            
            this.client = client;
            loginwelcome_caption = welcome;
            InitializeComponent();
        }

        private void ControlLoginNew_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); //  禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); //  双缓冲
            tableLayoutPanel1.GetType().GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tableLayoutPanel1, true, null);

            this.MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            //下面2行引起闪
            //pictureBox1.Image = Image.FromFile("logo_login.png");
            //tableLayoutPanel1.BackgroundImage= Image.FromFile("login_bg.png");
            this.textBoxACCOUNT.Text = mstrAccount;
            this.textBoxPASSWORD.Text = mstrPassword;          

            COUNT = 0;
            buttonOK.Visible = true;
            buttonCancel.Visible = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            COUNT = 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;//等待
                mstrAccount = this.textBoxACCOUNT.Text.Trim();
                mstrPassword = this.textBoxPASSWORD.Text.Trim();

                COUNT += 1;

                if (String.IsNullOrEmpty(mstrAccount))
                {
                    
                    MessageBox.Show("账号不能为空！");
                    if (COUNT >= 25)
                    {
                        this.DialogResult = DialogResult.Cancel;
                    }

                    return;
                }

                if (String.IsNullOrEmpty(mstrPassword))
                {
                    MessageBox.Show("密码不能为空！");                   

                    if (COUNT >= 25)
                    {
                        this.DialogResult = DialogResult.Cancel;
                    }

                    return;
                }

                String strPass = Utils.MD5(mstrPassword);

                PocConstant.GetInstance().userName = mstrAccount;
                PocConstant.GetInstance().password = strPass;

                User_Old u = PocClient.login(mstrAccount, strPass);
               
                if (u != null)
                {
                    client.confServerAddress = HttpAPI.MEDIAIP;
                    client.confServerPort = HttpAPI.MEDIAPort;

                    LocalSharedData.CURRENTUser = u;
                    //当前用户所在的组
                    LocalSharedData.UserAllGROUP = HttpAPI.userAllGroup(u.user_id);
                    foreach (Group grp in LocalSharedData.UserAllGROUP)
                        grp.group_type = GroupTypeEnum.TALK_FIX;
                    //当前用户所在的临时组
                    LocalSharedData.UserAllTempGROUP = HttpAPI.userAllTempGroup(u.user_id);
                    foreach (Group grp in LocalSharedData.UserAllTempGROUP)
                        grp.group_type = GroupTypeEnum.TALK_TMP;

                    //通过参与的群组算出所有用户
                    GroupCustomerVoResponse resp= PocClient.getGroupCustomerVo();
                    if (resp!=null && resp.data!=null && resp.data.Count > 0)
                    {                        
                        //LocalSharedData.UserAllGroupCustomer = new List<User>(resp.data.ToArray());
                        resp.data.ForEach(i => LocalSharedData.UserAllGroupCustomer.Add(i));

                        foreach (User it in LocalSharedData.UserAllGroupCustomer)
                        {
                            LocalSharedData.UserAll.Add(it.userId, it);
                        }
                    }
                   

                    //根据用户选择的语言项
                    if (comboBoxLanguage.SelectedIndex == 0)
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
                    }
                    else if (comboBoxLanguage.SelectedIndex == 1)
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-TW");
                    }
                    else
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    }

                    ////System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-TW");


                    ////////////////////////////

                    PttUserInfoResponse entity= PocClient.userInfo();

                    //DataManager.GetInstance().roomId =Convert.ToUInt32( PocConstant.GetInstance().cmpId);
                    //DataManager.GetInstance().userId = PocConstant.GetInstance().userName;


                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("账号 或 密码 错误！");
                    
                    if (COUNT >= 3)
                    {
                        this.DialogResult = DialogResult.Cancel;
                    }
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;//正常状态
            }
        }
    }
}
