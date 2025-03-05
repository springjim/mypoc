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
    public partial class SessionQueryForm : Form
    {
        ArrayList lists_GrpType = new ArrayList();
        public SessionQueryForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void SessionQueryForm_Load(object sender, EventArgs e)
        {
            //
            lists_GrpType.Clear();
            //加载所有群组名称

            lists_GrpType.Add(new MyKeyValue("-1", "不限"));
            foreach (Group grp in LocalSharedData.UserAllGROUP)
            {
                lists_GrpType.Add(new MyKeyValue(grp.group_id.ToString(), grp.group_name));
            }

            foreach (Group grp in LocalSharedData.UserAllTempGROUP)
            {
                lists_GrpType.Add(new MyKeyValue(grp.group_id.ToString(), grp.group_name));
            }
            //
            this.cbGroup.DisplayMember = "pValue";
            this.cbGroup.ValueMember = "pKey";
            this.cbGroup.DataSource = lists_GrpType;
            //记住上次有选择的群组名称,to do

            if (cbFileType.Items.Count == 0)
            {
                ArrayList lists_FileType = new ArrayList();
                lists_FileType.Add(new MyKeyValue("", "不限"));
                lists_FileType.Add(new MyKeyValue("picture", "图片文件"));
                lists_FileType.Add(new MyKeyValue("audio", "语音文件"));
                lists_FileType.Add(new MyKeyValue("file", "其它文件"));
                this.cbFileType.DisplayMember = "pValue";
                this.cbFileType.ValueMember = "pKey";
                this.cbFileType.DataSource = lists_FileType;
            }

            //
            if (cbPageSize.Text.Trim() == "")
                if (cbPageSize.Items.Count > 0)
                    cbPageSize.SelectedIndex = 0;


        }
    }
}
