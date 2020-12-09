using FlowCharter;
using Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TKS.FlowChart.Tool;

namespace TKS.Manager
{
    public class Flow
    {
        #region Singleton
        private static volatile Flow _instance = null;
        private static readonly object _locker = new object();
        public static Flow Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new Flow();
                        }
                    }
                }
                return _instance;
            }
        } 
        #endregion
        private Flow()
        {
            flowUserObjs = new Dictionary<string, List<XProps>>();
            SubFlows = new Dictionary<string, GraphNode>();            
            SubFlowsEx = new Dictionary<string, ProcessBase>();
            MainFlows = new List<GraphNode>();
            LoadConfig();

            Outputs = new Dictionary<string, object>();
        }

        #region userObject
        private const string folder = @".\Flow\";
        private Dictionary<string, List<XProps>> flowUserObjs { get; set; }

        public List<XProps> FlowUserObj(string id)
        {
            if (!flowUserObjs.ContainsKey(id)) flowUserObjs.Add(id, new List<XProps>());
            return flowUserObjs[id];
        }
        public void LoadConfig()
        {
            if (!System.IO.Directory.Exists(@".\Config\" + folder))
            {
                System.IO.Directory.CreateDirectory(@".\Config\" + folder);
            }
            var filePaths = Directory.GetFiles(@".\Config\" + folder, "*.cfg");
            flowUserObjs.Clear();
            foreach (var item in filePaths)
            {
                int index = item.LastIndexOf("\\") + 1;
                string name = item.Substring(index).Replace(".cfg", "");
                try
                {
                    flowUserObjs.Add(name, Configuration.GetConfig<List<XProps>>(folder + name));
                }
                catch (Exception)
                {
                }
            }

            //Assembly assembly = Assembly.LoadFrom(@".\ProcessInstance.dll");
            //var getProcesses = assembly.GetTypes().Where(item => item.IsSubclassOf(typeof(ProcessBase)));
            //foreach (var item in getProcesses)
            //{
            //    ProcessBase process = (ProcessBase)Activator.CreateInstance(item);
            //    SubFlowsEx.Add(process.Id, process);
            //}
        }
        public void Save(string FlowId)
        {
            try
            {
                Configuration.SaveConfig(folder + FlowId, flowUserObjs[FlowId]);
            }
            catch (Exception)
            {
            }
        }
        public void SaveAll()
        {
            foreach (var item in flowUserObjs)
            {
                Configuration.SaveConfig(folder + item.Key, item);
            }
        }
        #endregion

        #region Flow Chart
        public Dictionary<string, GraphNode> SubFlows { get; set; }
        public List<GraphNode> MainFlows { get; set; }
        public Dictionary<string, ProcessBase> SubFlowsEx { get; set; }

        public void SetModifiable(bool b)
        {
            foreach (var item in MainFlows)
            {
                item.Document.SetModifiable(b);
            }
            foreach (var item in SubFlows.Values)
            {
                item.Document.SetModifiable(b);
            }
        } 
        #endregion

        #region Output
        public Dictionary<string, object> Outputs;
        public void AddOutput(string key, object value)
        {
            if (!Outputs.ContainsKey(key))
            {
                Outputs.Add(key, new object());
            }
            Outputs[key] = value;
        }
        public void RemoveOutput(string key)
        {
            if (!Outputs.ContainsKey(key))
            {
                Outputs.Remove(key);
            }
        }
        public object GetOutput(string key)
        {
            if (Outputs.ContainsKey(key))
            {
                return Outputs[key];
            }
            throw new Exception("参数\"" + key + "\"不存在");
        } 
        #endregion
    }
}
