using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POCControlCenter.DataEntity;

namespace POCControlCenter
{
    public partial class UserGPSConfigForm : Form
    {
        public UserGPSConfigForm()
        {
            InitializeComponent();

            ArrayList lists_flag_autoLocation_str = new ArrayList();
            ArrayList lists_priv_hideLocSwitch_str = new ArrayList();
            ArrayList lists_locationMode_str = new ArrayList();
            ArrayList lists_locationInterval_str = new ArrayList();

            lists_flag_autoLocation_str.Add(new MyKeyValue("Y", "是"));
            lists_flag_autoLocation_str.Add(new MyKeyValue("N", "否"));
            flag_autoLocation_str.DisplayMember = "pValue";
            flag_autoLocation_str.ValueMember = "pKey";
            flag_autoLocation_str.DataSource = lists_flag_autoLocation_str;
            //
            lists_priv_hideLocSwitch_str.Add(new MyKeyValue("Y", "是"));
            lists_priv_hideLocSwitch_str.Add(new MyKeyValue("N", "否"));
            priv_hideLocSwitch_str.DisplayMember = "pValue";
            priv_hideLocSwitch_str.ValueMember = "pKey";
            priv_hideLocSwitch_str.DataSource = lists_priv_hideLocSwitch_str;

            //一般                高精                用户设置
            lists_locationMode_str.Add(new MyKeyValue("0", "一般"));
            lists_locationMode_str.Add(new MyKeyValue("1", "高精"));
            lists_locationMode_str.Add(new MyKeyValue("2", "用户设置"));
            locationMode_str.DisplayMember = "pValue";
            locationMode_str.ValueMember = "pKey";
            locationMode_str.DataSource = lists_locationMode_str;

            //循环定位时间间隔(单位：秒):30,60,180，0则由用户设置
            lists_locationInterval_str.Add(new MyKeyValue("0", "用户设置"));
            lists_locationInterval_str.Add(new MyKeyValue("10", "10"));
            lists_locationInterval_str.Add(new MyKeyValue("15", "15"));
            lists_locationInterval_str.Add(new MyKeyValue("30", "30"));
            lists_locationInterval_str.Add(new MyKeyValue("60", "60"));
            lists_locationInterval_str.Add(new MyKeyValue("180", "180"));
            locationInterval_str.DisplayMember = "pValue";
            locationInterval_str.ValueMember = "pKey";
            locationInterval_str.DataSource = lists_locationInterval_str;

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void UserGPSConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
        }
    }
}
