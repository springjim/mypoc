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
    public partial class AddFixGrpMemberForm : Form
    {
        public AddFixGrpMemberForm()
        {
            InitializeComponent();
        }
        public void clearChecklist()
        {
            checkedListBoxMember.Items.Clear();
        }
        public void addChecklist(string userid, string username)
        {
            checkedListBoxMember.Items.Add(new User_IDName(Convert.ToInt32(userid), username));
        }

        public void addChecklist_checked(string userid, string username)
        {
            checkedListBoxMember.Items.Add(new User_IDName(Convert.ToInt32(userid), username));
            checkedListBoxMember.SetItemChecked(checkedListBoxMember.Items.Count - 1, true);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //判断
            if (checkedListBoxMember.CheckedItems.Count == 0)
            {
                MessageBox.Show("请至少选择一个人员");
                return;
            }
            DialogResult = DialogResult.OK;
            this.Hide();

        }

        private void AddFixGrpMemberForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
        }
    }
}
