
using System;
using System.Collections.Generic;

namespace Helper
{
    public static class Configuration
    {
        public static void SaveConfig(string name, object configuration)
        {
            if (!System.IO.Directory.Exists("Config"))
            {
                System.IO.Directory.CreateDirectory("Config");
            }
            if (!System.IO.Directory.Exists(@"Config\"))
            {
                System.IO.Directory.CreateDirectory(@"Config\");
            }
            string json = JsonConvertX.Serialize(configuration);
            File.Write(@"Config\" + name + ".cfg", json);
        }

        public static T GetProcess<T>(string name, object process)
        {
            if (!System.IO.Directory.Exists(@"Config\Process"))
            {
                System.IO.Directory.CreateDirectory(@"Config\Process");
            }

            var json = File.Read(@"Config\Process\" + name + ".proc");
            return JsonConvertX.Deserialize<T>(json);
        }

        public static T GetConfig<T>(string name)
        {
            try
            {
                if (!System.IO.Directory.Exists(@"Config\"))
                {
                    System.IO.Directory.CreateDirectory(@"Config\");
                }
                var json = File.Read(@"Config\" + name + ".cfg");               
                return JsonConvertX.Deserialize<T>(json);
            }
            catch (Exception)
            {
                var instance = Activator.CreateInstance(typeof(T));
                SaveConfig(name, instance);
                return (T)instance;
            }
        }

        public static void CreateHalconPath()
        {
            //Root path
            if (!System.IO.Directory.Exists("Halcon"))
            {
                System.IO.Directory.CreateDirectory("Halcon");
            }

            string[] subPath = new string[3] { "Images", "Regions", "Procedures" };
            foreach (var path in subPath)
            {
                if (!System.IO.Directory.Exists(@"Halcon\" + path))
                {
                    System.IO.Directory.CreateDirectory(@"Halcon\" + path);
                }
            }

            if (!System.IO.Directory.Exists(@"Halcon\Images\Fails"))
            {
                System.IO.Directory.CreateDirectory(@"Halcon\Images\Fails");
            }
        }

        /// <summary>
        /// 将object对象转换为实体对象
        /// </summary>
        /// <typeparam name="T">实体对象类名</typeparam>
        /// <param name="asObject">object对象</param>
        /// <returns></returns>
        public static T ConvertObject<T>(object asObject) where T : new()
        {
            string str = JsonConvertX.Serialize(asObject);
            T t = JsonConvertX.Deserialize<T>(str);
            return t;
        }

    }
}
