using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class ProcessManager
    {
        public static Dictionary<string, Dictionary<string, object>> OutputDatas { get; set; } = new Dictionary<string, Dictionary<string, object>>();

        public static void OutputData<T>(string toolId,string instanceId,T value)
        {
            if (OutputDatas.ContainsKey(toolId))
            {
                if (OutputDatas[toolId].ContainsKey(instanceId))
                {
                    OutputDatas[toolId][instanceId] = value;
                }
                else OutputDatas[toolId].Add(instanceId, value);
            }
            else
            {
                Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
                keyValuePairs.Add(instanceId, value);
                OutputDatas.Add(toolId, keyValuePairs);
            }
        }

        public static T GetOutputData<T>(string toolId, string instanceId)
        {
            try
            {
                return (T)OutputDatas[toolId][instanceId];
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
