using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communication
{
    public class SocketServer : ICommunication
    {
        #region Private members
        private Socket _serverListenSocket;
        private Socket _serverAcceptSocket;

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
                Task.Factory.StartNew(() =>
                {
                    Initialize(ip, port);
                    //Thread.Sleep(500);
                    Open();
                }, TaskCreationOptions.LongRunning);
            }
        }


        #endregion

        public SocketServer()
        {
            ConnectionChanged += AutoReconnect;
        }
        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[BufferSize];
                    int rec = _serverAcceptSocket.Receive(buffer, 0, buffer.Length, 0);
                    if (rec <= 0)
                    {
                        throw new SocketException();
                    }
                    Array.Resize(ref buffer, rec);
                    //string message = Encoding.Default.GetString(buffer);
                    OnMessageRecieved(buffer);
                }
                catch (Exception)
                {
                    IsConnected = false;
                    OnConnectionChanged(IsConnected);
                    break;
                }
            }
        }

        public void Send(string cmd)
        {
            try
            {
                byte[] buffer = Encoding.Default.GetBytes(cmd);
                _serverAcceptSocket.Send(buffer, 0, buffer.Length, 0);
            }
            catch (Exception)
            {
                IsConnected = false;
                throw;
            }
        }

        public void Initialize(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            _serverAcceptSocket?.Close();
            IsConnected = false;
            _serverListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverAcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _serverListenSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            _serverListenSocket.Listen(0);
        }


        public void Open()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    //ConnectionChanged -= AutoReconnect;
                    //ConnectionChanged += AutoReconnect;
                    _serverAcceptSocket = _serverListenSocket.Accept();
                    _serverListenSocket.Close();
                    IsConnected = true;
                    ReceiveMessage();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Close()
        {
            ConnectionChanged -= AutoReconnect;
            _serverListenSocket.Close();
            _serverListenSocket.Dispose();
            _serverAcceptSocket.Close();
            _serverAcceptSocket.Dispose();
        }

        public void Reset()
        {
            _serverListenSocket.Close();
            _serverAcceptSocket.Close();
        }

        public void Send(byte[] cmd)
        {
            try
            {
                _serverAcceptSocket.Send(cmd, 0, cmd.Length, 0);
            }
            catch (Exception)
            {
                IsConnected = false;
                throw;
            }
        }
    }
}
