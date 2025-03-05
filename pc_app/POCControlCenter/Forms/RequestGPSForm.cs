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
    public partial class RequestGPSForm : Form
    {
        /// <summary>
        /// 动作类型,1表示仅查看，2表示发送一次指令并查看，3 表示循环请求定们, 0表示取消
        /// </summary>
        public Int32 action_cmd = 1;
        public RequestGPSForm()
        {
            InitializeComponent();
            cbAccuracy.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cbAccuracy.SelectedIndex == -1)
            {
                MessageBox.Show("请选择精度要求");
                return;
            }
            action_cmd = 2;
            DialogResult = DialogResult.OK;
        }

        private void RequestGPSForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            action_cmd = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            action_cmd = 1;
            DialogResult = DialogResult.OK;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (cbReqInterval.SelectedIndex == -1  ||  cbReqInterval.Text.Trim()=="" )
            {
                MessageBox.Show("请选择请求定位间隔");
                return;
            }
            action_cmd = 3;
            DialogResult = DialogResult.OK;
        }
    }
}
