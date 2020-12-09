using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;
using TKS.SubForm;

namespace TKS.FlowChart.Tool
{
    public class OutputTool : ToolBase
    {
        private void DataTypeChanged(object typeObject)
        {
            string typeStr = GetPropValue("类型").ToString();
            Type type = Type.GetType("System." + typeStr);
            XProp prop = Settings.Find(item => item.Name == "值");

            prop.ProType = type;
            prop.Value = null;
            MainForm.UpdatePropertyGridEvent();
        }
        protected override void Init()
        {
            base.Init();
            XProp xprop;
            xprop = new XProp();
            xprop.Category = "输出";
            xprop.Name = "键";
            xprop.Value = "Key";
            xprop.ProType = typeof(string);
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "输出";
            xprop.Name = "类型";
            xprop.Value = "0";
            xprop.Converter = new MyComboItemConvert("UInt16,Int16,UInt32,Int32,Double,Single,Boolean,String,UInt16[],Int16[],UInt32[],Int32[],Double[],Single[],Boolean[],String[]");
            xprop.ProType = typeof(MyComboItemConvert);
            xprop.ValueChanged = DataTypeChanged;
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "值";
            xprop.ProType = typeof(ushort);
            Settings.Add(xprop);
        }

        protected override Result Action()
        {
            try
            {
                string key = GetPropValue("键").ToString();
                object value = GetPropValue("值");
                Flow.Instance.AddOutput(key, value);
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
