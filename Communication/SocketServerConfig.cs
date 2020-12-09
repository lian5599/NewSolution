
namespace DevKit
{
    public class SocketServerConfig
    {
        public string Name { get; set; }
        public string IP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 201;
        public int BufferSize { get; set; } = 2048;
        public string Remarks { get; set; } = "DefaultRemarks";
    }
}
