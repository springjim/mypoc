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
    public partial class DayTimeSel : Form
    {

        public bool excludeself;
        public string startday;
        public string endday;

        public DayTimeSel()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //
            if (dateTimePickerFrm.Value > dateTimePickerTo.Value)
            {
                MessageBox.Show(WinFormsStringResource.FromDateMustBeforeToDate);
                return;
            }
            //如果终点大于起点 7天以上
            TimeSpan ts = dateTimePickerTo.Value - dateTimePickerFrm.Value;
            if ((ts.Days+1) > 7)
            {
                MessageBox.Show(WinFormsStringResource.DateSpanMustLess7days);
                return;
            }            

            excludeself = this.checkBox1.Checked;
            startday = this.dateTimePickerFrm.Text;
            endday = this.dateTimePickerTo.Text;

            DialogResult = DialogResult.OK;

        }

        public void setNowTime()
        {
            this.dateTimePickerFrm.Text = DateTime.Now.ToString();
            this.dateTimePickerTo.Text = DateTime.Now.ToString();
        }

    }
}
