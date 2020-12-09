
namespace DevKit
{
    public class WebRequestConfig
    {
        public string Name { get; set; }
        public string IP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 201;
        public int BufferSize { get; set; } = 1024;
        public string Remarks { get; set; } = "DefaultRemarks";
    }
}
