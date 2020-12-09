using Communication;
using Communication.Plc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication.Profinet.Melsec;

namespace PanasonicPlcVirtualServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PanasonicPlcServer<SocketServer> panasonicPlcServer;
        PanasonicPlcServer<Serial> panasonicPlcServerBySerial;
        //MelsecMcServer melsecMcServer ;
        private async void Create_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                string Paramstr = textBoxStr.Text;
                int ParamInt = Convert.ToInt32(textBoxInt.Text);
                panasonicPlcServer = new PanasonicPlcServer<SocketServer>(Paramstr, ParamInt);
                // melsecMcServer = new MelsecMcServer(true);
                //melsecMcServer.ServerStart(8080);
            });
            Btn_Create.Enabled = false;
        }
        private async void CreateSerial_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                string Paramstr = textBox_Port.Text;
                int ParamInt = Convert.ToInt32(textBox_BaudRate.Text);
                panasonicPlcServerBySerial = new PanasonicPlcServer<Serial>(Paramstr, ParamInt);
            });
            btn_Serial.Enabled = false;
        }
    }
}
