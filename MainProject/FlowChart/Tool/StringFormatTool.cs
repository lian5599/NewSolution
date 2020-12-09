using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    public class StringFormatTool : ToolBase
    {
        protected override void Init()
        {
            base.Init();
            XProp xprop;
            xprop = new XProp();
            xprop.Category = "输入";
            xprop.Name = "输入";
            xprop.Value = new XList<string>();
            xprop.ProType = typeof(XList<string>);
            xprop.Converter = new XListConverter<string>();
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "输出";
            xprop.Name = "输出格式";
            xprop.Value = "";
            xprop.ProType = typeof(string);
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "输出";
            xprop.Name = "输出";
            xprop.Value = "stringFormat输出名";
            xprop.ProType = typeof(string);
            Settings.Add(xprop);
        }

        protected override Result Action()
        {
            try
            {
                object[] inputs = GetInputs();
                string value = string.Format((string)GetPropValue("输出格式"), inputs);
                string key = GetPropValue("输出").ToString();
                Flow.Instance.AddOutput(key, value);
                Log.Run(LogLevel.Info, string.Format("{0}:{1}",key,value));
                return new Result();
            }
            catch (Exception e)
            {
                Log.Run(LogLevel.Error, e.Message, e);
                return new Result(e.Message,440);
            }
        }
    }
}
