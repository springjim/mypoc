using POCControlCenter.DataEntity;
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
    public partial class DelTmpGrpMulti : Form
    {
        static string tmpgrpname = "";
        static string memberstr = "";

        public DelTmpGrpMulti()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Boolean sel_b = false;
            memberstr = "";
            for (int i = 0; i < this.checkedListBoxGrp.Items.Count; i++)
            {
                if (checkedListBoxGrp.GetItemChecked(i))
                {
                    if (memberstr.Equals(""))
                        memberstr = Convert.ToString(((Group_IDName)checkedListBoxGrp.Items[i]).group_id);
                    else
                        memberstr = memberstr + "," + Convert.ToString(((Group_IDName)checkedListBoxGrp.Items[i]).group_id);

                    sel_b = true;

                }
            }

            if (!sel_b)
            {
                MessageBox.Show(WinFormsStringResource.SelectTmpGrp);
            }            
            else
            {               
                DialogResult = DialogResult.OK;
            }
        }

        public void clearChecklist()
        {
            checkedListBoxGrp.Items.Clear();
        }

        public void addChecklist(string groupid, string groupname)
        {
            checkedListBoxGrp.Items.Add(new Group_IDName(Convert.ToInt32(groupid), groupname));
        }

        public string getMember()
        {
            return memberstr;
        }


    }
}
