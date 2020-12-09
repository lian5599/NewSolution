using Newtonsoft.Json;

namespace Helper
{
    public static class JsonConvertX
    {
        public static string Serialize(object obj)
        {
            //var Settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };//多态序列化
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static T Deserialize<T>(string json)
        {
            //var Settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };//多态序列化
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
