using Communication;
using Communication.Plc;
using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    public class PlcReadTool:ToolBase
    {
        public PlcReadTool():base()
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
            xprop.Name = "长度";
            xprop.Value = 1;
            xprop.ProType = typeof(int);
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "读取类型";
            xprop.Value = "0";
            xprop.Converter =  new MyComboItemConvert("UInt16,Int16,UInt32,Int32,Double,Single,Boolean,String,Byte");
            xprop.ProType = typeof(MyComboItemConvert);
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "输出";
            xprop.Name = "输出";
            xprop.Value = new XList<string>() ;
            xprop.ProType = typeof(XList<string>);
            xprop.Converter = new XListConverter<string>();
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "输出";
            xprop.Name = "显示";
            xprop.ProType = typeof(bool);
            xprop.Value = true;
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "输入";
            xprop.Name = "输入";
            xprop.Value = new XList<string>();
            xprop.ProType = typeof(XList<string>);
            xprop.Converter = new XListConverter<string>();
            Settings.Add(xprop);
        }



        protected override Result Action()
        {
            Result re;
            try
            {
                //int Address = Convert.ToInt32(GetPropValue("地址"));
                int ReadLength = (int)GetPropValue("长度");
                string dataType = GetPropValue("读取类型").ToString();
                string[] outputDefines = (string[])GetPropValue("输出");
                string plcId = GetPropValue("PLC").ToString();
                IPlc PLC = Hardware.Instance.plc(plcId);
                re = (Result)typeof(IPlc).GetMethod("Read" + dataType)
                    .Invoke(PLC, new object[] { GetPropValue("地址"), GetPropValue("长度") });
                if (re.IsSuccess)
                {
                    if (dataType == "Boolean")
                    {
                        bool b = (re.GetContent() as bool[])[0];
                        //Log.Debug(plcId + " Read Bool=>" + graphNode.Text + ":" + b.ToString());
                        if (!b) return new Result(false);
                        return re;
                    }

                    if (outputDefines!=null)
                    {
                        object[] inputs = GetInputs();
                        int length = Math.Min(outputDefines.Length, ReadLength);
                        string logStr = "";
                        for (int i = 0; i < length; i++)
                        {
                            try
                            {
                                Array array = (Array)re.GetContent();
                                if (outputDefines[i].Replace(" ", "") == "") continue;
                                string key = inputs == null ? outputDefines[i] : string.Format(outputDefines[i], inputs);
                                object value = array.GetValue(i);
                                Flow.Instance.AddOutput(key, value);
                                logStr += string.Format("{0}:{1}", key, value);
                                logStr += ";";
                            }
                            catch (Exception)
                            {
                                //content is not array.
                                Flow.Instance.AddOutput(outputDefines[0], re.GetContent());
                                logStr += string.Format("{0}:{1}", outputDefines[0], re.GetContent());
                                logStr += ";";
                                break;
                            }
                        }
                        if (logStr.Length > 0)
                        {
                            if (GetPropValue("显示").Equals(true))
                            {
                                logStr.Remove(logStr.Length - 1);
                                Log.Run(LogLevel.Info, plcId + " Read =>" + logStr);
                            }
                            //else
                            //{
                            //    Log.Debug(plcId + " Read "+ dataType+" =>" + logStr);
                            //}
                        }
                            
                    }   
                    //Log.Run(LogLevel.Info, Newtonsoft.Json.JsonConvert.SerializeObject(re.GetContent()));
                }
                else Log.Run(LogLevel.Error, re.Message);
                return re;
            }
            catch (Exception e)
            {
                Log.Run(LogLevel.Error, e.Message,e);
                UiHelper.ShowError(e.Message);
                return new Result(e.Message, (int)ErrorCode.fatalError);
            }
        }
    }
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    //public class PlcReadInputDefine
    //{
    //    public string address1 { get; set; }
    //    public string address2 { get; set; }
    //    public override string ToString()
    //    {
    //        return address1 + "," + address2;
    //    }
    //}
}
