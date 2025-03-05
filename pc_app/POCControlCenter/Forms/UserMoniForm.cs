using POCControlCenter.DataEntity;
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

namespace POCControlCenter
{
    public partial class UserMoniForm : Form
    {
        ArrayList lists_resolveType = new ArrayList();
        public UserMoniForm()
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            DialogResult = DialogResult.OK;
        }

        private void UserMoniForm_Load(object sender, EventArgs e)
        {
            comboBoxProtocol.SelectedIndex = 0;          

            lists_resolveType.Add(new MyKeyValue("1", "320 x 240 (QVGA)"));
            lists_resolveType.Add(new MyKeyValue("2", "352 x 288 (CIF)"));
            lists_resolveType.Add(new MyKeyValue("3", "480 x 320 (HQVGA)"));
            lists_resolveType.Add(new MyKeyValue("4", "640 x 480 (VGA)"));
            lists_resolveType.Add(new MyKeyValue("6", "1280 x 720 (720P)"));
            lists_resolveType.Add(new MyKeyValue("7", "1920 x 1080 (1080P)"));

            this.comboBoxResolve.DisplayMember = "pValue";
            this.comboBoxResolve.ValueMember = "pKey";
            this.comboBoxResolve.DataSource = lists_resolveType;

            //默认显示rtsp
            comboBoxProtocol.SelectedIndex = 0;
            //默认显示rtsp的resolve (352 x 288 (CIF))
            comboBoxResolve.SelectedIndex = 1;

        }

        private void comboBoxProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
            /*
            if (comboBoxProtocol.SelectedIndex == 0)
            {
                //rtsp
                BindingSource bs = new BindingSource();
                bs.DataSource = lists_resolveType;               

                lists_resolveType.Clear();
                lists_resolveType.Add(new MyKeyValue("1", "320 x 240 (QVGA)"));
                lists_resolveType.Add(new MyKeyValue("2", "352 x 288 (CIF)"));
                lists_resolveType.Add(new MyKeyValue("3", "480 x 320 (HQVGA)"));
                lists_resolveType.Add(new MyKeyValue("4", "640 x 480 (VGA)"));
                lists_resolveType.Add(new MyKeyValue("6", "1280 x 720 (720P)"));
                lists_resolveType.Add(new MyKeyValue("7", "1920 x 1080 (1080P)"));
                this.comboBoxResolve.DisplayMember = "pValue";
                this.comboBoxResolve.ValueMember = "pKey";
                this.comboBoxResolve.DataSource = bs;

                bs.ResetBindings(false);

            }
            else if (comboBoxProtocol.SelectedIndex == 1)
            {
                BindingSource bs = new BindingSource();
                bs.DataSource = lists_resolveType;
                //rtmp
                lists_resolveType.Clear();
                lists_resolveType.Add(new MyKeyValue("0", "176 x 144 (QCIF)"));
                lists_resolveType.Add(new MyKeyValue("1", "320 x 240 (QVGA)"));
                lists_resolveType.Add(new MyKeyValue("2", "352 x 288 (CIF)"));
                lists_resolveType.Add(new MyKeyValue("3", "480 x 320 (HQVGA)"));
                lists_resolveType.Add(new MyKeyValue("4", "640 x 480 (VGA)"));
                lists_resolveType.Add(new MyKeyValue("5", "720 x 480 (VGA+)"));
                lists_resolveType.Add(new MyKeyValue("6", "1280 x 720 (720P)"));
                lists_resolveType.Add(new MyKeyValue("7", "1920 x 1080 (1080P)"));

                this.comboBoxResolve.DisplayMember = "pValue";
                this.comboBoxResolve.ValueMember = "pKey";
                this.comboBoxResolve.DataSource = bs;

                bs.ResetBindings(false);

            }
            */

        }
    }
}
