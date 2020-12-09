using Helper;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communication
{
    public class Serial:ICommunication
    {
        private SerialPort _serialPort;
        private string PortName { get; set; } = "COM1";
        private int BaudRate { get; set; } = 9600;
        private int BufferSize { get; set; } = 2048;

        public bool IsConnected { get; set; }


        #region event
        public event ConnectionChangedHandler ConnectionChanged;
        public event WarnOccuredHandler WarnOccured;
        public event ErrorOccuredHandler ErrorOccured;
        public event MessageRecievedEventHandler MessageRecieved;
        protected void OnMessageRecieved(byte[] recievedMessage)
        {
            MessageRecieved?.Invoke(this, recievedMessage);
        }
        protected void OnConnectionChanged(bool isConnected)
        {
            ConnectionChanged?.Invoke(this,isConnected);          
        }
        private void AutoReconnect(object sender, bool connected)
        {
            if (!connected)
            {
                Task.Run(() =>
                {
                    Initialize(PortName, BaudRate);
                    Open();
                });
            }
        }
        #endregion

        public Serial()
        {
            ConnectionChanged += AutoReconnect;
        }

        public void Initialize(string PortName, int BaudRate)
        {
            this.PortName = PortName;
            this.BaudRate = BaudRate;
            _serialPort?.Close();
            IsConnected = false;
            _serialPort = new SerialPort(PortName, BaudRate,
                System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            _serialPort.ReceivedBytesThreshold = 1;
            _serialPort.ReadTimeout = 100;
        }

        public void Open()
        {
            try
            {
                //ConnectionChanged -= AutoReconnect;
                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);// _serialPort_DataReceived;
                //ConnectionChanged += AutoReconnect;
                _serialPort.Open();
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
                IsConnected = true;

                //Thread thread = new Thread(() =>
                //{
                //    ReceiveMessage();
                //});
                //thread.IsBackground = true;
                //thread.Start();
            }
            catch (Exception e)
            {
                IsConnected = false;
                Thread.Sleep(500);
            }
            finally
            {
                OnConnectionChanged(IsConnected);
            }
        }

        public void Close()
        {
            ConnectionChanged -= AutoReconnect;
            _serialPort?.Close();
            _serialPort?.Dispose();
        }

        public void Send(string cmd)
        {
            try
            {
                byte[] buffer = Encoding.Default.GetBytes(cmd);
                _serialPort.Write(buffer, 0, buffer.Length);
            }
            catch (Exception)
            {
                IsConnected = false;
                OnConnectionChanged(IsConnected);
                throw;
            }
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    int _length = _serialPort.BytesToRead;
                    if (_length > 0)
                    {
                        byte[] buffer = new byte[_length];
                        _serialPort.Read(buffer, 0, buffer.Length);
                        OnMessageRecieved(buffer);
                    }
                    //int temp ;
                    //int i = 0;
                    //do
                    //{
                    //    temp = _serialPort.ReadByte();
                    //    if (temp == -1)
                    //    {

                    //    }
                    //    buffer[i] = (byte)temp;
                    //    i++;
                    //} while (temp != -1);
                    //Array.Resize(ref buffer, i);
                    //OnMessageRecieved(buffer);
                }
                catch (Exception e)
                {
                    IsConnected = false;
                    //OnConnectionChanged(IsConnected);
                    break;
                }
            }
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Serial port communication base on stream buffer mechanism,
            // the whole messsage could be separated into pieces,
            // end user has to combine them if necessary.
            string message = _serialPort.ReadExisting();
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            OnMessageRecieved(buffer);
        }

        public void Send(byte[] cmd)
        {
            try
            {
                _serialPort.Write(cmd, 0, cmd.Length);
            }
            catch (Exception)
            {
                IsConnected = false;
                OnConnectionChanged(IsConnected);
                throw;
            }
        }
    }
}
