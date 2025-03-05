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
    public partial class LocalVideoViewForm : Form
    {
        public LocalVideoViewForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void LocalVideoViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
        }

        

    }
}
