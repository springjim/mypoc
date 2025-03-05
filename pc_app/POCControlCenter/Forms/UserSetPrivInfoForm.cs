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
    public partial class UserPrivSetInfoForm : Form
    {

        //FIELD_AUDIORECORD = 1;FIELD_AUDIOCLASS = 2;FIELD_LIFESTATE = 3;
        public int oper_field = 0; //操作字段类型

        public UserPrivSetInfoForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            DialogResult = DialogResult.OK;

        }
    }
}
