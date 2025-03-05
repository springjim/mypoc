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
    public partial class UserBaseForm : Form
    {
        DateTimeSelForm dateForm;
        public UserBaseForm()
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void dateTimePickerhiredate_Enter(object sender, EventArgs e)
        {
             
        }

        private void dateTimePickerExpiredate_Enter(object sender, EventArgs e)
        {
            //dateTimePickerExpiredate.Format = DateTimePickerFormat.Custom;
            //dateTimePickerExpiredate.CustomFormat = "yyyy-MM-dd";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void UserBaseForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
        }

        private void UserBaseForm_Load(object sender, EventArgs e)
        {
             

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dateForm == null)
                dateForm = new DateTimeSelForm();

            if (dateTimePickerhiredate.Text.Trim() != "")
            {
                dateForm.monthCalendar.SetDate(DateTime.ParseExact(dateTimePickerhiredate.Text.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture));
            }

            if (dateForm.ShowDialog() == DialogResult.OK)
            {
                dateTimePickerhiredate.Text = dateForm.monthCalendar.SelectionStart.ToString("yyyy-MM-dd");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dateForm == null)
                dateForm = new DateTimeSelForm();

            if (dateTimePickerExpiredate.Text.Trim() != "")
            {
                dateForm.monthCalendar.SetDate(DateTime.ParseExact(dateTimePickerExpiredate.Text.Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture));
            }

            if (dateForm.ShowDialog() == DialogResult.OK)
            {
                dateTimePickerExpiredate.Text = dateForm.monthCalendar.SelectionStart.ToString("yyyy-MM-dd");
            }
        }
    }
}
