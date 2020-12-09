using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class Settings
    {
        public static List<SettingCfg> Config = new List<SettingCfg>();
        public static string GetSetting(string key)
        {
            var setting = Config.Find(item => item.Key == key);
            return setting == null ? "" : setting.Value.ToString();
        }
        public static void AddSetting(string key)
        {
            var setting = Config.Find(item => item.Key == key);
            if (setting == null)
            {
                Config.Add(new SettingCfg(key, ""));
            }
        }
    }
    public class SettingCfg
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public SettingCfg(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
