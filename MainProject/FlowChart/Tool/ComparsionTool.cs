using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    public class ComparsionTool : ToolBase
    {
        protected override void Init()
        {
            base.Init();
            XProp xprop;
            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "变量名";
            xprop.ProType = typeof(string);
            xprop.Value = "";
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "对比值";
            xprop.ProType = typeof(string);
            xprop.Value = "";
            Settings.Add(xprop);
        }

        protected override Result Action()
        {
            try
            {
                string parameter = Flow.Instance.GetOutput(GetPropValue("变量名").ToString()).ToString();
                string compareValue = GetPropValue("对比值").ToString();
                if (parameter == compareValue) return new Result();
                else return new Result(false);
            }
            catch (Exception e)
            {
                //Log.Run(LogLevel.Warming, e.Message,e);
                return new Result(false,e.Message);
            }
        }
    }
}
