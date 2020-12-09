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
    public partial class FormRegister : Form
    {
        public FormRegister()
        {
            InitializeComponent();
        }

        private void Btn_Register_Click(object sender, EventArgs e)
        {
            try
            {
                if (Security.Register(textBox_RegisterCode.Text,out TimeSpan t))
                {
                    MessageBox.Show("注册成功！\r\n有效期剩余："+t.Days.ToString(), "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    base.DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("注册码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }            
        }

        private void FormRegister_Load(object sender, EventArgs e)
        {
            textBox_MachineCode.Text = Security.GetCpu().Substring(0,Security.Length);
            //textBox_MachineCode.Text = Security2.GetDeviceNum().ToString();
        }
    }
}
