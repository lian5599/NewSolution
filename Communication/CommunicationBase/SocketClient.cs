using Helper;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Communication;
using System.Threading;

namespace Communication
{
    public class SocketClient : ICommunication
    {
        #region Private members
        private Socket _clientSocket;
        private EndPoint _endPoint;
        private string ip { get; set; } = "127.0.0.1";
        private int port { get; set; } = 8080;
        private int BufferSize { get; set; } = 2048;
        #endregion

        #region Public members
        public bool IsConnected { get; set; }
        public string Id { get; set; }
        #endregion

        #region Events
        public event MessageRecievedEventHandler MessageRecieved;
        public event ConnectionChangedHandler ConnectionChanged;
        public event WarnOccuredHandler WarnOccured;
        public event ErrorOccuredHandler ErrorOccured;

        protected void OnMessageRecieved(byte[] recievedMessage)
        {
            MessageRecieved?.Invoke(this, recievedMessage);
        }
        protected void OnConnectionChanged(bool isConnected)
        {
            ConnectionChanged?.Invoke(this, isConnected);
        }

        private void AutoReconnect(object sender, bool connected)
        {
            if (!connected)
            {
                Task.Run(() =>
                {
                    Initialize(ip, port);
                    Open();
                });
            }
        }
        #endregion

        #region Constructor
        public SocketClient()
        {
            //ConnectionChanged -= AutoReconnect;
            ConnectionChanged += AutoReconnect;
        }

        #endregion

        public void Initialize(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            _clientSocket?.Close();
            IsConnected = false;
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public void Open()
        {
            try
            {
                _clientSocket.Connect(_endPoint);
                IsConnected = true;
                Thread thread = new Thread(() =>
                {
                    ReceiveMessage();
                });
                thread.IsBackground = true;
                thread.Start();
                //Task.Run(() => { ReceiveMessage(); });
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
            _clientSocket?.Close();
            _clientSocket?.Dispose();
        }

        public void Send(string cmd)
        {
            try
            {
                byte[] buffer = Encoding.Default.GetBytes(cmd);
                _clientSocket.Send(buffer, 0, buffer.Length, 0);
            }
            catch (Exception)
            {
                IsConnected = false;
                //OnConnectionChanged(IsConnected);
                throw;
            }
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[BufferSize];
                    int rec = _clientSocket.Receive(buffer, 0, buffer.Length, 0);
                    if (rec <= 0)
                    {
                        throw new SocketException();
                    }
                    Array.Resize(ref buffer, rec);
                    //string message = Encoding.Default.GetString(buffer);
                    OnMessageRecieved(buffer);
                }
                catch (Exception e)
                {
                    IsConnected = false;
                    OnConnectionChanged(IsConnected);
                    break;
                }
            }
        }

        public void Send(byte[] cmd)
        {
            try
            {
                _clientSocket.Send(cmd, 0, cmd.Length, 0);
            }
            catch (Exception)
            {
                IsConnected = false;
                //OnConnectionChanged(IsConnected);
                throw;
            }
        }
    }
}


