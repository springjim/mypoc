﻿using System;
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
    public partial class VideoQueryForm : Form
    {
        public VideoQueryForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void VideoQueryForm_Load(object sender, EventArgs e)
        {
            if (cbPageSize.Text.Trim()=="")
                if (cbPageSize.Items.Count > 0)
                cbPageSize.SelectedIndex = 0;
        }
    }
}
