using Communication;
using Communication.Plc;
using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TKS.FlowChart.Tool
{
    public class PlcWriteTool:ToolBase
    {
        public PlcWriteTool():base()
        {
            Hardware_PlcChangedEvent();
            Hardware.PlcChangedEvent -= Hardware_PlcChangedEvent;
            Hardware.PlcChangedEvent += Hardware_PlcChangedEvent;
        }

        private void Hardware_PlcChangedEvent()
        {
            string plcDefineStr = "";
            foreach (var item in Hardware.Instance.plcs)
            {
                plcDefineStr += item.Id + ",";
            }
            if (plcDefineStr.Length > 0) plcDefineStr = plcDefineStr.Remove(plcDefineStr.Length - 1,1);
            Settings.Find(item => item.Name == "PLC").Converter = new MyComboItemConvert(plcDefineStr);
        }

        private void DataTypeChanged(object typeObject)
        {
            string typeStr = GetPropValue("写入类型").ToString();
            Type type = Type.GetType("System." + typeStr);
            XProp prop = Settings.Find(item => item.Name == "写入值");

            if (type == typeof(string))
            {
                prop.Value = "";
                prop.ProType = type;
                prop.Converter = null;
            }
            else
            {
                var instanceType = Type.GetType("TKS.FlowChart.Tool.XList`1").MakeGenericType(type);
                prop.Value = Activator.CreateInstance(instanceType);

                prop.ProType = instanceType;

                var ConverterType = Type.GetType("TKS.FlowChart.Tool.XListConverter`1").MakeGenericType(type);
                prop.Converter = (TypeConverter)Activator.CreateInstance(ConverterType);
            }          
            MainForm.UpdatePropertyGridEvent();
        }
        protected override void Init()
        {
            base.Init();
            XProp xprop;
            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "PLC";
            xprop.Value = "0";
            xprop.ProType = typeof(MyComboItemConvert);
            Settings.Add(xprop);
            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "地址";
            xprop.Value = 1;
            xprop.ProType = typeof(int);
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "写入类型";
            xprop.Value = "0";
            xprop.Converter = new MyComboItemConvert("UInt16,Int16,UInt32,Int32,Double,Single,Boolean,String,Byte");
            xprop.ProType = typeof(MyComboItemConvert);
            xprop.ValueChanged = DataTypeChanged;
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "写入值";
            xprop.Value = new XList<ushort>() { 1};
            xprop.ProType = typeof(XList<ushort>);
            xprop.Converter = new XListConverter<ushort>();
            //xprop.ProType = typeof(ushort[]);
            Settings.Add(xprop);
        }

        protected override Result Action()
        {
            Result re;           
            try
            {
                //int Address = Convert.ToInt32(GetPropValue("地址"));
                string plcId = GetPropValue("PLC").ToString();
                IPlc PLC = Hardware.Instance.plc(plcId);
                object value = GetPropValue("写入值");
                re = (Result)typeof(IPlc).GetMethod("Write", new Type[] { typeof(int), value.GetType() })
                    .Invoke(PLC, new object[] { GetPropValue("地址"), value });
                if (re.IsSuccess)
                {
                    re.Message = plcId + " Write "+ GetPropValue("写入类型").ToString() + " Address:" + GetPropValue("地址").ToString() +"; Value:"
                        + Newtonsoft.Json.JsonConvert.SerializeObject(GetPropValue("写入值"));
                    Log.Debug(re.Message);
                }
                else
                {
                    Log.Run(LogLevel.Error, re.Message);
                }
                return re;
            }
            catch (Exception e)
            {
                UiHelper.ShowError(e.Message);
                return new Result(e.Message, (int)ErrorCode.fatalError);
            }
        }
    }
}
