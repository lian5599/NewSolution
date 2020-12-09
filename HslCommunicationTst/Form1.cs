using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communication;
using HslCommunication;
using HslCommunication.Core;
using HslCommunication.LogNet;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Panasonic;

namespace HslCommunicationTst
{
    public partial class Form1 : Form
    {
        IReadWriteNet panasonicSerial;
        PanasonicMewtocol plc;
        PanasonicMewtocolOverTcp plcTcp;
        MelsecMcServer melsecMcServer;
        MelsecMcNet melsecMc;

        public Form1()
        {
            InitializeComponent();
            //plc = new PanasonicMewtocol(0x01);
            //panasonicSerial = plc;
            //plc.SerialPortInni("COM5", 115200);
            //plc.LogNet = new LogNetSingle("D://123.txt");
            //if (!plc.IsOpen())
            //{
            //    var isconnect = plc.Open();
            //    plc.LogNet.WriteDebug("123123");
            //    MessageBox.Show(isconnect.ToMessageShowString());
            //}
            //textBoxAddress.Text = "D1000";
            //textBoxLength.Text = "1";
            //textBox_Valuee.Text = "1";
            //melsecMcServer = new MelsecMcServer(true);
            //melsecMcServer.ServerStart(8080);
            //melsecMc = new MelsecMcNet("127.0.0.1", 8080);
            //melsecMc.ReceiveTimeOut = 1000;
            //melsecMc.ConnectServer();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string address = textBoxAddress.Text;
                ushort length = Convert.ToUInt16(textBoxLength.Text);

                var response = melsecMc.ReadFloatAsync(address, length);
                await response;
                MessageBox.Show(response.ToJsonString());
            }
            catch (Exception ex)
            {

            }
        }

        private async void button_Write_Click(object sender, EventArgs e)
        {
            try
            {
                string address = textBoxAddress.Text;
                ushort length = Convert.ToUInt16(textBoxLength.Text);
                ushort value = Convert.ToUInt16(textBox_Valuee.Text);
                ushort[] array = new ushort[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = value;
                }
                var response = melsecMc.WriteAsync(address, array);
                await response;
                MessageBox.Show(response.ToJsonString());
            }
            catch (Exception ex)
            {

            }
        }
    }
}
