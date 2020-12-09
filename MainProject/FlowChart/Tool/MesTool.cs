using Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    public class MesTool : ToolBase
    {
        Assembly assembly;
        protected override void Init()
        {
            base.Init();
            XProp xprop;            
            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "方法";
            xprop.Value = "0";
            assembly = Assembly.LoadFrom("MesAdapter.dll");
            var methods = assembly.GetType(ConfigurationManager.AppSettings["MesAdapter"])
                .GetMethods().Where(item => item.IsPublic && item.ReturnType == typeof(Result));
            string methodsStr = "";
            foreach (var item in methods)
            {
                methodsStr += item.Name;
                if (!item.Equals(methods.Last()))
                {
                    methodsStr += ",";
                }
            }
            xprop.Converter = new MyComboItemConvert(methodsStr);
            xprop.ProType = typeof(MyComboItemConvert);
            Settings.Add(xprop);

            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "参数";
            xprop.Value = new XList<string>();
            xprop.ProType = typeof(XList<string>);
            xprop.Converter = new XListConverter<string>();
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
            string methodStr = GetPropValue("方法").ToString();
            try
            {  
                object[] inputs = GetInputs();

                string[] ParamDefines = (string[])GetPropValue("参数");
                object[] parameters = null;
                Type[] parametersType = null; 
                if (ParamDefines != null)
                {
                    int inputLength = ParamDefines.Length;
                    parameters = new object[inputLength];
                    parametersType = new Type[inputLength];
                    for (int i = 0; i < inputLength; i++)
                    {
                        string paramId = string.Format(ParamDefines[i], inputs);
                        parameters[i] = Flow.Instance.GetOutput(paramId);
                        parametersType[i] = parameters[i].GetType();
                    }
                }

                var method = assembly.GetType(ConfigurationManager.AppSettings["MesAdapter"]).GetMethod(methodStr, parametersType);
                if (method == null)
                {
                    throw new Exception("参数不匹配");
                }
                Result result = (Result)method.Invoke(null, parameters);

                LogLevel logLevel;
                if (!result.IsSuccess)
                {
                    logLevel = LogLevel.Error;
                    //result.ErrorCode = (int)ErrorCode.fatalError;
                }
                else
                {
                    logLevel = LogLevel.Info;
                    result.ErrorCode = 0;
                }
                if (result.Message!="")
                {
                    Log.Run(logLevel, result.Message);
                }
                return result;
            }
            catch (Exception e)
            {
                string msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                msg = methodStr + "失败：" + msg;
                Log.Run(LogLevel.Error, msg, e);
                return new Result(msg/*, (int)ErrorCode.fatalError*/);
            }
        }
    }
}
