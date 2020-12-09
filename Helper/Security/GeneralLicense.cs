using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    public partial class LicenseCreaterForm : Form
    {
        public LicenseCreaterForm()
        {
            InitializeComponent();
            TextBox_MachineCode.Text = Security.GetCpu().Substring(0, Security.Length);
        }

        private void Btn_Genaral_Click(object sender, EventArgs e)
        {
            DateTime dateTime = DateTimePicker1.Value;
            string device = TextBox_MachineCode.Text;
            TextBoxLicense.Text = Security.CreateLicense(device, dateTime);           
        }
    }
}
