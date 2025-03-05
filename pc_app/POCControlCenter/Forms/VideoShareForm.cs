using POCControlCenter.DataEntity;
using POCControlCenter.Service;
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
    public partial class VideoShareForm : Form
    {
        public VideoShareForm()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //选择人员
            int sel_count = 0;
            if (cblist_user.CheckedItems.Count == 0)
            {
                MessageBox.Show("请选择分享人员");
                return;
            }

            string useridlist = "";
            for (int i = 0; i < cblist_user.CheckedItems.Count; i++)
            {
                if (useridlist.Equals(""))
                    useridlist = ((User_IDName)cblist_user.CheckedItems[i]).user_id.ToString();
                else
                    useridlist = useridlist + "," 
                        + ((User_IDName)cblist_user.CheckedItems[i]).user_id.ToString();

            }

            if (!useridlist.Equals(""))
            {

                PocClient.shareMoniOrLive( (int)(Utils.getCurrentTimeMillis() / 1000), tbvideo_url.Text,
                    "0", useridlist, LocalSharedData.CURRENTUser.user_id, LocalSharedData.CURRENTUser.user_name);

                DialogResult = DialogResult.OK;

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i=0; i < cblist_user.Items.Count; i++)
            {
                cblist_user.SetItemChecked(i, true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cblist_user.Items.Count; i++)
            {
                cblist_user.SetItemChecked(i, false);
            }
        }
    }
}
