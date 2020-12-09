using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    public class ModelMatchTool : ToolBase
    {
        protected override void Init()
        {
            base.Init();
            XProp xprop;
            xprop = new XProp();
            xprop.Category = "参数";
            xprop.Name = "变量名";
            xprop.ProType = typeof(ModelRoi);
            xprop.Value = null;//new ModelRoi(1, "A");
            xprop.EditorType = typeof(Editor1);
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

    public class ModelRoi
    {
        public int I { get; set; }
        public string S { get; set; }
        public ModelRoi(int i, string s)
        {
            I = i;
            S = s;
        }
        public override string ToString()
        {
            return I.ToString() + S;
        }
    }
}
