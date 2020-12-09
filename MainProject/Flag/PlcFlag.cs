using Newtonsoft.Json;

namespace TKS
{
    public class FlagModule
    {
        public string PlcId { get; set; }
        public string Id { get; set; } = "Flag1";
        public int Address { get; set; } = 1;
        [JsonIgnore]
        public int CurrentValue { get; set; } = 0;
        public int ValidValue { get; set; } = 1;
        public int OkValue { get; set; } = 2;
        public int NgValue { get; set; } = 3;

    }
}
