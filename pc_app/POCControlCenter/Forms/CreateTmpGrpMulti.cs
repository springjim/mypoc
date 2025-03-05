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
    public partial class CreateTmpGrpMulti : Form
    {

        static string tmpgrpname = "";
        static string memberstr = "";

        public CreateTmpGrpMulti()
        {
            InitializeComponent();
        }

        public string getMember( )
        {
            return memberstr;
        }

        public string getGrpname()
        {
            return tmpgrpname;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Boolean sel_b = false;
            memberstr = "";
            for (int i = 0; i < this.checkedListBoxMember.Items.Count; i++)
            {
                if (checkedListBoxMember.GetItemChecked(i))
                {
                    if (memberstr.Equals(""))
                        memberstr = Convert.ToString(((User_IDName)checkedListBoxMember.Items[i]).user_id);
                    else
                        memberstr = memberstr+","+Convert.ToString(((User_IDName)checkedListBoxMember.Items[i]).user_id);

                    sel_b = true;

                }
            }

            if (!sel_b)
            {
                MessageBox.Show(WinFormsStringResource.SelectUser);
            }
            else if (this.textBoxGrpName.Text.Trim().Equals("")) {
                MessageBox.Show(WinFormsStringResource.EnterGrpName);
            } else
            {
                tmpgrpname = this.textBoxGrpName.Text.Trim();
                DialogResult = DialogResult.OK;
            }

        }

        public void clearChecklist()
        {
            checkedListBoxMember.Items.Clear();
        }

        public void addChecklist(string userid, string username)
        {
            checkedListBoxMember.Items.Add(new User_IDName(Convert.ToInt32(userid),username));
        }

        public void addChecklist_checked(string userid, string username)
        {
            checkedListBoxMember.Items.Add(new User_IDName(Convert.ToInt32(userid), username));
            checkedListBoxMember.SetItemChecked(checkedListBoxMember.Items.Count - 1, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("广播组是个特殊的组，只允许调度台讲话, 成员只能收听等");
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
