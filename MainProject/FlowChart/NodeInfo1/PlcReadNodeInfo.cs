using Helper;
using TKS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace FlowCharter
{
    [Serializable]
    public class PlcReadNodeInfo:GraphNodeInfoBase
    {
        public PlcReadNodeInfo(GraphNode n) : base(n) 
        {
        }
        [Category("Param")]
        [Description("ID")]
        public string PlcId { get; set; } = "";

        [Category("Param")]
        [Description("地址")]
        public int Address { get; set; }
        [Category("Param")]
        [Description("长度")]
        public int Length { get; set; }
        [Category("Result")]
        [Description("对比值")]
        public int CompareValue { get; set; } = 1;

        protected override Result Action()
        {
            Result<int []> re;
            try
            {
                re = Hardware.Instance.plc(PlcId).Read(Address,Length);
                if (re.IsSuccess)
                {
                    if (re.Content[0] != CompareValue)
                    {
                        re.IsSuccess = false;
                    }
                    string subStr = "";
                    for (int i = 0; i < Length; i++)
                    {
                        subStr += ((char)re.Content[i]).ToString();
                    }
                    AddOutput("dataStr", subStr);
                    AddOutput(Address.ToString(), re.Content[0]);
                    Log.Run(LogLevel.Info, re.Message);
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
                return new Result(e.Message);
            }
        }
        public override Result RunAll()
        {
            return base.RunAll();
        }
        public override void OutputDeclaration()
        {
            Output.OutputDataDefine(Name, "address" + Address.ToString());
        }
    }

}
