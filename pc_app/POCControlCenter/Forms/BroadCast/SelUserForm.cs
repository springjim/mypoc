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

namespace POCControlCenter.BroadCast
{
    public partial class SelUserForm : Form
    {

        static string memberstr = "";

        #region Form Move

        private bool mIsMouseDown = false;
        private Point mFormLocation;     // Form的location
        private Point mMouseOffset;      // 鼠标的按下位置

        private void OnFormMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mIsMouseDown = true;
                mFormLocation = this.Location;
                mMouseOffset = Control.MousePosition;
            }
        }

        private void OnFormMouseUp(object sender, MouseEventArgs e)
        {
            mIsMouseDown = false;
        }

        private void OnFormMouseMove(object sender, MouseEventArgs e)
        {
            if (mIsMouseDown)
            {
                Point pt = Control.MousePosition;
                int x = mMouseOffset.X - pt.X;
                int y = mMouseOffset.Y - pt.Y;

                this.Location = new Point(mFormLocation.X - x, mFormLocation.Y - y);
            }
        }
        #endregion

        public SelUserForm()
        {
            InitializeComponent();
        }

        public void clearChecklist()
        {
            checkedListBoxMember.Items.Clear();
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
                        memberstr = memberstr + "," + Convert.ToString(((User_IDName)checkedListBoxMember.Items[i]).user_id);

                    sel_b = true;

                }
            }

            if (!sel_b)
            {
                MessageBox.Show(WinFormsStringResource.SelectUser);
            }
             
            else
            {                 
                DialogResult = DialogResult.OK;
            }
        }

        public string getMember()
        {
            return memberstr;
        }

        public void addChecklist_checked(string userid, string username)
        {
            checkedListBoxMember.Items.Add(new User_IDName(Convert.ToInt32(userid), username));
            checkedListBoxMember.SetItemChecked(checkedListBoxMember.Items.Count - 1, true);
        }

        public void addChecklist(string userid, string username)
        {
            checkedListBoxMember.Items.Add(new User_IDName(Convert.ToInt32(userid), username));
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
