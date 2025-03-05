using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POCControlCenter.DataEntity;
using System.Diagnostics;
using POCControlCenter.Service.Entity;
using POCControlCenter.Service;
using POCControlCenter.Service.Model;
using System.Web;

namespace POCControlCenter
{
    public partial class FenceRuleDef : Form
    {

        //MySqlDataAdapter daFenceRule;    //fence的适配器
        //DataSet dsFenceRule;             //fence的dataset
        //
        //MySqlDataAdapter daFenceAlarm;    //fence的适配器
        //DataSet dsFenceAlarm;             //fence的dataset

        FenceRuleItemForm itemForm;
        public int full_fence_id=0;
        //2017.8.16 用户所在时区与服务器所在时区间的差,单位小时
        public int full_ZoneInterval_UserServer = 0;

        public FenceRuleDef()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult=DialogResult.OK;
        }

        private void refreshGridView()
        {
            full_fence_id = Convert.ToInt32(cbFencelist.SelectedValue.ToString());

            List<FenceUserDto> data = new List<FenceUserDto>();
            AllFenceUserInfoResponse resp2 = PocClient.queryAllFenceUserInfo(full_fence_id);
            if (resp2 != null && resp2.data != null)
                data = resp2.data;

            dataGridViewRule.AutoGenerateColumns = false;
            dataGridViewRule.DataSource = data;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            if (itemForm == null || itemForm.IsDisposed)
                itemForm = new FenceRuleItemForm();

            itemForm.btnAdd.Visible = true;
            itemForm.btnModify.Visible = false;

            itemForm.fence_id = full_fence_id;
            itemForm.cbUser.Enabled = true;
            itemForm.rule_id = 0;

            if (itemForm.ShowDialog() == DialogResult.OK)
            {
                //刷新datagridview  full_fence_id
                List<FenceUserDto> data = new List<FenceUserDto>();
                AllFenceUserInfoResponse resp2 = PocClient.queryAllFenceUserInfo(full_fence_id);
                if (resp2 != null && resp2.data != null)
                    data = resp2.data;
               
                dataGridViewRule.AutoGenerateColumns = false;
                dataGridViewRule.DataSource = data;

            }

        }

        private void dataGridViewRule_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.ColumnIndex > -1 && e.RowIndex > -1)  //点击的是鼠标右键，并且不是表头
            {
                //右键选中单元格
                this.dataGridViewRule.Rows[e.RowIndex].Selected = true;
                this.contextMenuStrip1.Show(MousePosition.X, MousePosition.Y); //MousePosition.X, MousePosition.Y 是为了让菜单在所选行的位置显示

            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //
            this.contextMenuStrip1.Hide();

            if (((ContextMenuStrip)sender).Items[0] == e.ClickedItem)
            {
                //删除
                DialogResult dr = MessageBox.Show(WinFormsStringResource.FenceRuleDel,
                    WinFormsStringResource.PromptStr, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    
                    int rule_id= (int)this.dataGridViewRule.SelectedRows[0].Cells["id"].Value;
                    ResponseBase resp=  PocClient.deleteFenceUser(rule_id);
                   
                   if (resp!=null && resp.code==0 )
                    {
                        //再刷新
                        List<FenceUserDto> data = new List<FenceUserDto>();
                        AllFenceUserInfoResponse resp2 = PocClient.queryAllFenceUserInfo(full_fence_id);
                        if (resp2 != null && resp2.data != null)
                            data = resp2.data;

                        dataGridViewRule.AutoGenerateColumns = false;
                        dataGridViewRule.DataSource = data;

                    }                    

                }             


            }
            else if (((ContextMenuStrip)sender).Items[1] == e.ClickedItem)
            {
                //编辑
                if (itemForm == null)
                    itemForm = new FenceRuleItemForm();

                itemForm.btnAdd.Visible = false;
                itemForm.btnModify.Visible = true;

                itemForm.fence_id = full_fence_id;
                itemForm.rule_id= (int)this.dataGridViewRule.SelectedRows[0].Cells["id"].Value;

                //再初始化
                int user_id= (int)this.dataGridViewRule.SelectedRows[0].Cells["userId"].Value;
                string rule_type = "";
                if (this.dataGridViewRule.SelectedRows[0].Cells["ruleType"].Value != DBNull.Value)
                    rule_type = (string)this.dataGridViewRule.SelectedRows[0].Cells["ruleType"].Value;

                string rule_assign = "";
                if (this.dataGridViewRule.SelectedRows[0].Cells["ruleAssign"].Value != DBNull.Value)
                    rule_assign = (string)this.dataGridViewRule.SelectedRows[0].Cells["ruleAssign"].Value;

                string startstr = "";
                if (this.dataGridViewRule.SelectedRows[0].Cells["startTime"].Value != DBNull.Value)
                    startstr = (string)this.dataGridViewRule.SelectedRows[0].Cells["startTime"].Value;


                string endstr = "";
                if (this.dataGridViewRule.SelectedRows[0].Cells["endTime"].Value != DBNull.Value)
                    endstr = (string)this.dataGridViewRule.SelectedRows[0].Cells["endTime"].Value;

                //传入
                itemForm.user_id = user_id;
                itemForm.rule_type = rule_type;
                itemForm.rule_assign = rule_assign;
                itemForm.startstr = startstr;
                itemForm.endstr = endstr;
                //传入end              


                if (itemForm.ShowDialog() == DialogResult.OK)
                {
                    //刷新datagridview
                    List<FenceUserDto> data = new List<FenceUserDto>();
                    AllFenceUserInfoResponse resp2 = PocClient.queryAllFenceUserInfo(full_fence_id);
                    if (resp2 != null && resp2.data != null)
                        data = resp2.data;

                    dataGridViewRule.AutoGenerateColumns = false;
                    dataGridViewRule.DataSource = data;                    

                }

            }

            

        }

        private void contextMenuStrip_Queryalarm_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (((ContextMenuStrip)sender).Items["toolStripMenuItem1"] == e.ClickedItem)
            {
                contextMenuStrip_Queryalarm.Hide();
                ExportToExcel d = new ExportToExcel();
                d.OutputAsExcelFile(this.dgvFenceAlarm);

            } else if (((ContextMenuStrip)sender).Items["toolStripMenuItem2"] == e.ClickedItem)
            {
                contextMenuStrip_Queryalarm.Hide();
                DataGridViewTOPdf.ExportTOPdf(this.dgvFenceAlarm);

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //
            //刷新datagridview
            string alarmType = null;
            string startNotifyTime = null;
            string endNotifyTime = null;           

            if (cbRule.SelectedIndex == 0 || cbRule.SelectedIndex<0)
            {               
                                       

            } else if (cbRule.SelectedIndex == 1 )
            {
                alarmType = "income";
            }
            else if (cbRule.SelectedIndex == 2)
            {
                alarmType = "outcome";  
            }

            startNotifyTime = this.dateTimePickerStart.Text;
            endNotifyTime = this.dateTimePickerEnd.Text;

            List<FenceAlarmNotifyDto> data = new List<FenceAlarmNotifyDto>();
            MultiFenceAlarmNotifyInfoResponse resp2 = PocClient.queryMultiAlarmNotifyInfo(
                LocalSharedData.CURRENTUser.cmpid,alarmType,startNotifyTime,endNotifyTime);
            if (resp2 != null && resp2.data != null)
                data = resp2.data;
            
            dgvFenceAlarm.AutoGenerateColumns = false;
            dgvFenceAlarm.DataSource = data;


        }

        private void FenceRuleDef_Load(object sender, EventArgs e)
        {
            this.dateTimePickerStart.Text = (DateTime.Now.AddDays(-7)).ToString();
            this.dateTimePickerEnd.Text = DateTime.Now.ToString();
        }

        private void FenceRuleDef_FormClosed(object sender, FormClosedEventArgs e)
        {
            DialogResult = DialogResult.OK;          
        }

        private void cbFencelist_SelectedIndexChanged(object sender, EventArgs e)
        {            

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //
            int alarm_type = 0;
            if (cbRule.SelectedIndex > -1)
                alarm_type = cbRule.SelectedIndex;

            string fenceName = this.dgvFenceAlarm.SelectedRows[0].Cells["fenceName2"].Value.ToString();
            string fencePoints= this.dgvFenceAlarm.SelectedRows[0].Cells["fencePoints"].Value.ToString();
            string userName= this.dgvFenceAlarm.SelectedRows[0].Cells["userName2"].Value.ToString();
            string notifyTimeStr= this.dgvFenceAlarm.SelectedRows[0].Cells["notifyTimeStr"].Value.ToString();
            string stayTimeMin= this.dgvFenceAlarm.SelectedRows[0].Cells["stayTimeMin"].Value.ToString();
            string endLatitude= this.dgvFenceAlarm.SelectedRows[0].Cells["endLatitude"].Value.ToString();
            string endLongitude = this.dgvFenceAlarm.SelectedRows[0].Cells["endLongitude"].Value.ToString();
            string alarmTypeName= this.dgvFenceAlarm.SelectedRows[0].Cells["alarmTypeName"].Value.ToString();

            
            System.Diagnostics.Process.Start("iexplore.exe",
                       HttpAPI.FenceAlarmMap_URL + "?fence_name=" + HttpUtility.UrlEncode(fenceName)
                       + "&fence_points="+ fencePoints
                       + "&user_name=" + HttpUtility.UrlEncode(userName)
                       + "&notify_time_str=" + notifyTimeStr
                       + "&stay_time_min=" + stayTimeMin
                       + "&end_latitude=" + endLatitude
                       + "&end_longitude=" + endLongitude
                       + "&alarm_type_name=" + HttpUtility.UrlEncode(alarmTypeName)

                       );

        }

        private void dataGridViewRule_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
