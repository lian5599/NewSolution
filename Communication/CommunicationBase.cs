using Communication.Plc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Communication
{
    public class CommunicationBase
    {
        protected readonly object WriteLocker = new object();
        public string Id { get; set; } = "Id1";
        public string HardwareId { get; set; } = "PLC";
        public string Kind { get; set; } = "Panasonic";
        public string Mode { get; set; } = "Serial";
        public string ParamStr { get; set; } = "COM1";
        public int ParamInt { get; set; } = 9600;
        public int Timeout { get; set; } = 2000;  
        protected ICommunication communication { get; set; }
        protected AutoResetEvent ResponseEvent { get; set; } = new AutoResetEvent(false);


        #region event
        public event ConnectionChangedHandler ConnectionChangeEvent;
        private void ConnectionChanged(object sender, bool connected)
        {
            ConnectionChangeEvent?.Invoke(this, communication.IsConnected);
        }
        protected virtual void MessageRecieved(object sender, byte[] recievedMessage) { }

        #endregion

        public void Connect()
        {
            try
            {
                communication.MessageRecieved -= MessageRecieved;
                communication.ConnectionChanged -= ConnectionChanged;
                communication.Initialize(ParamStr, ParamInt);                
                communication.MessageRecieved += MessageRecieved;
                communication.ConnectionChanged += ConnectionChanged;
                communication.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public bool IsConnected()
        {
            if (communication == null)
            {
                return false;
            }
            return communication.IsConnected;
        }

        protected void SendCommand(string command, bool isWait = true)
        {
            lock (WriteLocker)
            {
                if (!communication.IsConnected)
                {
                    Thread.Sleep(500);
                    throw new Exception(Id +"未连接");
                }
                try
                {
                    ResponseEvent.Reset();
                    communication.Send(command);
                    if (isWait && !ResponseEvent.WaitOne(Timeout))
                    {
                        throw new Exception(Id + "回复超时:" + Timeout.ToString() + "ms");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        protected void SendCommand(byte[] command, bool isWait = true)
        {
            lock (WriteLocker)
            {
                if (!communication.IsConnected)
                {
                    Thread.Sleep(500);
                    throw new Exception(Id + "未连接");
                }
                try
                {
                    ResponseEvent.Reset();
                    communication.Send(command);
                    if (isWait && !ResponseEvent.WaitOne(Timeout))
                    {
                        throw new Exception(Id + "回复超时:" + Timeout.ToString() + "ms");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void Close()
        {
            communication?.Close();
        }
    }
}
