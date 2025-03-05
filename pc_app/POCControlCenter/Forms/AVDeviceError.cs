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
    public partial class AVDeviceErrorForm : Form
    {

        private bool VideoInputDevice_State;
        private bool AudioOutputDevice_State;
        private bool AudioInputDevice_State;
        public AVDeviceErrorForm()
        {
            InitializeComponent();
        }

        public AVDeviceErrorForm(bool VideoInputDevice_State, bool AudioOutputDevice_State,bool AudioInputDevice_State)
        {
            InitializeComponent();
            this.VideoInputDevice_State = VideoInputDevice_State;
            this.AudioOutputDevice_State = AudioOutputDevice_State;
            this.AudioInputDevice_State = AudioInputDevice_State;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void AVDeviceError_Load(object sender, EventArgs e)
        {
            //
            if (VideoInputDevice_State)
            {
                this.labVideoIN.Text = WinFormsStringResource.DeviceNormal;
                this.labVideoIN.ForeColor = Color.DarkGreen;

            } else
            {
                this.labVideoIN.Text = WinFormsStringResource.DeviceError_VideoInput;
                this.labVideoIN.ForeColor = Color.Red;
            }

            //
            if (AudioOutputDevice_State)
            {
                this.labAudioOUT.Text = WinFormsStringResource.DeviceNormal;
                this.labAudioOUT.ForeColor = Color.DarkGreen;

            }
            else
            {
                this.labAudioOUT.Text = WinFormsStringResource.DeviceError_AudioOutput;
                this.labAudioOUT.ForeColor = Color.Red;
            }

            //
            if (AudioInputDevice_State)
            {
                this.labAudioIN.Text = WinFormsStringResource.DeviceNormal;
                this.labAudioIN.ForeColor = Color.DarkGreen;

            }
            else
            {
                this.labAudioIN.Text = WinFormsStringResource.DeviceError_AudioInput;
                this.labAudioIN.ForeColor = Color.Red;
            }



        }
    }
}
