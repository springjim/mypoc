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
    public partial class CreateTmpGrp : Form
    {
        static string tmpgrpname = "";
        static string memberstr = "";

        public CreateTmpGrp()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxGrpName.Text.Trim() == "")
            {
                MessageBox.Show(WinFormsStringResource.CreateTmpGrpNameISNull);
                return;
            }
            this.DialogResult=DialogResult.OK;
        }

        public void setGrpname(string grpname)
        {
            this.textBoxGrpName.Text = grpname;
        }

        public string getGrpname()
        {
            return textBoxGrpName.Text.Trim();
        }

        public void setMember(string members)
        {
            this.labelMember.Text = members;
        }
        

    }
}
