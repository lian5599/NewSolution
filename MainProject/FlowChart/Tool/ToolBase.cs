using FlowCharter;
using Helper;
using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    public class ToolBase
    {
        public XProps Settings { get; set; }
        public GraphNode graphNode { get; set; }
        public Result Result { get; set; }
        public ToolBase(GraphNode n)
        {
            this.graphNode = n;
            Settings = new XProps();
            Init();
        }
        public ToolBase()
        {
            Settings = new XProps();
            Init();
        }
        protected virtual void Init()
        {
            XProp xprop = new XProp();
            xprop.Category = "身份";
            xprop.Description = "保存时生成，具有唯一性";
            xprop.Name = "ID";
            xprop.ReadOnly = true;
            xprop.Value = "-1";
            xprop.ProType = typeof(int);
            Settings.Add(xprop);
        }
        public object GetPropValue(string name)
        {
            var prop = Settings.Find(item => item.Name == name);
            if (prop.Converter != null && prop.Converter.GetType() == typeof(MyComboItemConvert))
            {
                return prop.Converter.ConvertTo(prop.Value, typeof(string));
            }
            else if (prop.Converter != null && prop.Converter.GetType().Name == "XListConverter`1")
            {
                return prop.Value.GetType().GetMethod("ToArray").Invoke(prop.Value, null);
            }
            if (prop.Value != null && prop.ProType != null && prop.Value.GetType() != prop.ProType)
            {
                object value = typeof(Convert).GetMethod("To" + prop.ProType.Name, new Type[] { typeof(object) }).Invoke(null, new object[] { prop.Value });
                return value;
            }
            return prop.Value;
        }
        public void SetPropValue(string name, object value)
        {
            var prop = Settings.Find(item => item.Name == name);
            prop.Value = value;
        }
        public object PropValue(string name)
        {
            var setting = Settings.Find(item => item.Name == name);
            if (setting == null) return null;
            if (setting.Value.GetType() != typeof(string) && setting.Converter != null)
            {
                return setting.Converter.ConvertTo(setting.Value, typeof(string));
            }
            return setting.Value;
        }

        public Result Run()
        {
            Result = Action();
            if (Result.IsSuccess)
            {
                //Thread.Sleep(500);
                graphNode.Shadowed = false;
            }
            else graphNode.Shadowed = true;
            return Result;
        }
        protected virtual Result Action()
        {
            return new Result();
        }

        public virtual Result RunAll()
        {
            this.Run();
            bool ParentIsSuccuss = Result.IsSuccess;
            foreach (var item in graphNode.Destinations)
            {
                if (Result.ErrorCode == (int)ErrorCode.fatalError) return Result;

                GraphNode subNode = item as GraphNode;
                var currentLinks = from link in subNode.Links
                                   where link.FromNode == graphNode
                                   select link.GoObject;
                foreach (var link in currentLinks)
                {
                    bool IsNegative = ((link as GoLabeledLink).MidLabel as GoText).Text == "N";
                    if (IsNegative ^ ParentIsSuccuss)
                    {
                        ToolBase tool = (item as GraphNode).UserObject as ToolBase;
                        Result ResultTemp = tool.RunAll();
                        if (!ResultTemp.IsSuccess)
                        {
                            Result = ResultTemp;
                        }
                    }
                }              
            }
            return Result;
        }
        protected virtual void Dispose()
        {
            this.Result = null;
            foreach (var item in graphNode.Sources)
            {
                GraphNode subNode = item as GraphNode;
                (subNode.UserObject as ToolBase).Dispose();
            }
        }

        protected object[] GetInputs(string id = "输入")
        {
            string[] inputDefines = (string[])GetPropValue(id);
            object[] inputs = null;
            if (inputDefines != null)
            {
                int inputLength = inputDefines.Length;
                inputs = new object[inputLength];
                for (int i = 0; i < inputLength; i++)
                {
                    string inputId = string.Format(inputDefines[i], inputs);
                    inputs[i] = Flow.Instance.GetOutput(inputId);
                }
            }
            return inputs;
        }
    }
}
