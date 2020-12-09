using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKS.Manager
{
    public static class Output
    {
        private static Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>();
        public static void OutputDataDefine(string actionId, string dataId)
        {
            if (!data.ContainsKey(actionId))
            {
                data.Add(actionId, new Dictionary<string, object>());

            }
            if (!data[actionId].ContainsKey(dataId))
            {
                data[actionId].Add(dataId, new object());

            }

        }

        public static void Clear()
        {
            data?.Clear();
        }

        public static void OutputData(string actionId, string dataId,object value)
        {
            if (!data.ContainsKey(actionId))
            {
                data.Add(actionId, new Dictionary<string, object>());

            }
            if (!data[actionId].ContainsKey(dataId))
            {
                data[actionId].Add(dataId, new object());
            }
            data[actionId][dataId] = value;
        }
        public static object GetOutputData(string actionId, string dataId)
        {
            try
            {
                return data[actionId][dataId];
            }
            catch (Exception e)
            {

                throw new Exception(e.Message + "参数：" + actionId + ";" + dataId);
            }
        }

        public static Action OutputDefineChange;
    }
}
