using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    public class InputTool:ToolBase
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
            xprop.Category = "参数";
            xprop.Name = "输出格式";
            xprop.Value = "{0}";
            xprop.ProType = typeof(string);
            Settings.Add(xprop);
        }

        protected override Result Action()
        {
            try
            {
                object[] inputs = GetInputs();
                Log.Run(LogLevel.Info, string.Format((string)GetPropValue("输出格式"), JsonConvertX.Serialize(inputs)));
                return new Result();
            }
            catch (Exception e)
            {
                Log.Run(LogLevel.Error, e.Message, e);
                return new Result(e.Message);
            }
        }
    }
}
