
namespace Communication
{
    public interface ICommunication
    {
        bool IsConnected { get; set; }
        void Initialize(string paramStr,int paramInt);
        void Open();
        void Send(string cmd);
        void Send(byte[] cmd);
        void Close();

        event ConnectionChangedHandler ConnectionChanged;
        event WarnOccuredHandler WarnOccured;
        event ErrorOccuredHandler ErrorOccured;
        event MessageRecievedEventHandler MessageRecieved;
    }
    public delegate void MessageRecievedEventHandler(object sender, byte[] recievedMessage);
    public delegate void ConnectionChangedHandler(object sender,bool connected);
    public delegate void WarnOccuredHandler(string description);
    public delegate void ErrorOccuredHandler(string description);
}
