using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using POCControlCenter.DataEntity;
using POCControlCenter.Service.Entity;
using POCControlCenter.Service;
using POCControlCenter.Service.Model;

namespace POCControlCenter
{
    public partial class FenceRuleItemForm : Form
    {

        public int fence_id;
        public int rule_id=0;     //用于修改时,传入id
        public int user_id;       //用于修改时,传入
        public string rule_type;   //用于修改时,传入
        public string rule_assign; //用于修改时,传入
        public string startstr;     //用于修改时,传入
        public string endstr;       //用于修改时,传入

        public FenceRuleItemForm()
        {
            InitializeComponent();
        }

        private void FenceRuleItemForm_Load(object sender, EventArgs e)
        {
            ArrayList lists_RuleType = new ArrayList();
            ArrayList lists_RuleWhtBlk = new ArrayList();
            ArrayList lists_User = new ArrayList();

            //注意，以下的键值对与sql的case when语句要一致          
                
           lists_RuleType.Add(new MyKeyValue("income", "进入警告"));
           lists_RuleType.Add(new MyKeyValue("outcome", "出去警告"));
           lists_RuleWhtBlk.Add(new MyKeyValue("include", "黑名单"));              
                       
          

            foreach (KeyValuePair<int, User> kv in LocalSharedData.UserAll)
            {
                lists_User.Add(new MyKeyValue(kv.Key.ToString(),kv.Value.userName));
            }           

            this.cbRuleType.DisplayMember = "pValue";
            this.cbRuleType.ValueMember = "pKey";
            this.cbRuleType.DataSource = lists_RuleType;

            this.cbRuleWhtBlk.DisplayMember = "pValue";
            this.cbRuleWhtBlk.ValueMember = "pKey";
            this.cbRuleWhtBlk.DataSource = lists_RuleWhtBlk;

            this.cbUser.DisplayMember = "pValue";
            this.cbUser.ValueMember = "pKey";
            this.cbUser.DataSource = lists_User;

            if (rule_id>0)
            {
                //表示修改模式
                cbUser.SelectedValue = user_id.ToString();
                cbUser.Enabled = false;

                for (int i = 0; i < cbRuleType.Items.Count; i++)
                {
                    if (cbRuleType.GetItemText(cbRuleType.Items[i]).Equals(rule_type))
                    {
                        cbRuleType.SelectedIndex = i;
                        break;
                    }
                }
                //itemForm.cbRuleType.SelectedIndex =

                for (int i = 0; i < cbRuleWhtBlk.Items.Count; i++)
                {
                    if (cbRuleWhtBlk.GetItemText(cbRuleWhtBlk.Items[i]).Equals(rule_assign))
                    {
                        cbRuleWhtBlk.SelectedIndex = i;
                        break;
                    }
                }


                if (startstr != null && !startstr.Trim().Equals(""))
                {
                    dateTimePickerStart.Checked = true;
                    dateTimePickerStart.Text = startstr;
                }
                else
                {
                    dateTimePickerStart.Checked = false;
                }


                if (endstr != null && !endstr.Trim().Equals(""))
                {
                    dateTimePickerEnd.Checked = true;
                    dateTimePickerEnd.Text = endstr;

                }
                else
                {
                    dateTimePickerEnd.Checked = false;
                }

            } else
            {
                cbUser.Enabled = true;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //
            if (cbUser.Text.ToString().Equals(""))
            {
                MessageBox.Show(WinFormsStringResource.FenceRulePerson);
                return;
            }
            if (cbRuleType.Text.ToString().Equals(""))
            {
                MessageBox.Show(WinFormsStringResource.FenceRuleType);
                return;
            }
            if (cbRuleWhtBlk.Text.ToString().Equals(""))
            {
                MessageBox.Show(WinFormsStringResource.FenceRuleBlkWht);
                return;
            }
            //
            string startstr = "";

            if (this.dateTimePickerStart.Checked)
                startstr=this.dateTimePickerStart.Value.ToString("HH:mm:ss");

            string endstr ="";
            if (this.dateTimePickerEnd.Checked)
                endstr = this.dateTimePickerEnd.Value.ToString("HH:mm:ss");

            FenceUserDto dto = new FenceUserDto();
            dto.fenceId = fence_id;
            dto.userId = Convert.ToInt32(cbUser.SelectedValue.ToString());
            dto.ruleType = cbRuleType.SelectedValue.ToString();
            dto.ruleAssign = cbRuleWhtBlk.SelectedValue.ToString();
            dto.startTime = startstr;
            dto.endTime = endstr;

            ResponseBase resp= PocClient.saveFenceUser(dto);
            if (resp!=null && resp.code == 0)
            {
                MessageBox.Show("保存成功");
                DialogResult = DialogResult.OK;
            } else
            {
                DialogResult = DialogResult.OK;
            }


        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            //
            if (cbUser.Text.ToString().Equals(""))
            {
                MessageBox.Show(WinFormsStringResource.FenceRulePerson);
                return;
            }
            if (cbRuleType.Text.ToString().Equals(""))
            {
                MessageBox.Show(WinFormsStringResource.FenceRuleType);
                return;
            }
            if (cbRuleWhtBlk.Text.ToString().Equals(""))
            {
                MessageBox.Show(WinFormsStringResource.FenceRuleBlkWht);
                return;
            }
            //
            string startstr = "";

            if (this.dateTimePickerStart.Checked)
                startstr = this.dateTimePickerStart.Value.ToString("HH:mm:ss");

            string endstr = "";
            if (this.dateTimePickerEnd.Checked)
                endstr = this.dateTimePickerEnd.Value.ToString("HH:mm:ss");


            FenceUserDto dto = new FenceUserDto();

            dto.id = rule_id;
            //dto.fenceId = fence_id;
            //dto.userId = Convert.ToInt32(cbUser.SelectedValue.ToString());
            dto.ruleType = cbRuleType.SelectedValue.ToString();
            dto.ruleAssign = cbRuleWhtBlk.SelectedValue.ToString();
            dto.startTime = startstr;
            dto.endTime = endstr;

            ResponseBase resp = PocClient.updateFenceUser(dto);
            if (resp != null && resp.code == 0)
            {
                MessageBox.Show("保存成功");
                DialogResult = DialogResult.OK;
            } else
            {
                DialogResult = DialogResult.OK;
            }
            

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void cbRuleType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
