using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DevKit
{
    public class SocketServer: IDevice
    {
        #region Private members
        private SocketServerConfig _configData;
        private Socket _serverListenSocket;
        private Socket _serverAcceptSocket;
        #endregion

        #region Public members
        public bool Connected { get; set; }
        public string Name { get; set; }
        #endregion

        #region Events
        public delegate void MessageRecievedEventHandler(object sender, string recievedMessage);

        public event MessageRecievedEventHandler MessageRecieved;
        public event ConnectionChangedHandler ConnectionChanged;
        public event WarnOccuredHandler WarnOccured;
        public event ErrorOccuredHandler ErrorOccured;

        protected void OnMessageRecieved(string recievedMessage)
        {
            MessageRecieved?.Invoke(this, recievedMessage);
        }
        #endregion

        #region Constructor
        public SocketServer(string name)
        {
            Name = name;
        }
        #endregion

        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[_configData.BufferSize];
                    int rec = _serverAcceptSocket.Receive(buffer, 0, buffer.Length, 0);
                    if (rec <= 0)
                    {
                        throw new SocketException();
                    }
                    Array.Resize(ref buffer, rec);
                    string message = Encoding.Default.GetString(buffer);
                    OnMessageRecieved(message);
                }
                catch (Exception)
                {
                    Connected = false;
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
                Connected = false;
                throw;
            }
        }

        public void Initialize()
        {
            _configData = Configuration.GetConfig<SocketServerConfig>(Name);
            if (_configData.Name == null)
            {
                _configData.Name = this.Name;
                Configuration.SaveConfig(Name, _configData);
            }

            _serverListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverAcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _serverListenSocket.Bind(new IPEndPoint(IPAddress.Parse(_configData.IP), _configData.Port));
            _serverListenSocket.Listen(0);
        }

        public void Open()
        {
            Task.Run(() =>
            {
                try
                {
                    _serverAcceptSocket = _serverListenSocket.Accept();
                    Connected = true;
                    ReceiveMessage();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                }
            });
        }

        public void Close()
        {
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
    }
}


