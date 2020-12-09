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
    public class PlcWriteNodeInfo : GraphNodeInfoBase
    {
        public PlcWriteNodeInfo(GraphNode n) : base(n) { }

        [Category("Param")]
        [Description("ID")]
        public string PlcId { get; set; } = "";

        [Category("Param")]
        [Description("地址")]
        public int Address { get; set; }
        [Category("Param")]
        [Description("值")]
        public int[] Values { get; set; }

        protected override Result Action()
        {
            Result re;
            try
            {
                re = Hardware.Instance.plc(PlcId).Write(Address, Values);
                if (re.IsSuccess)
                {
                    Log.Run(LogLevel.Info, re.Message);
                }
                else Log.Run(LogLevel.Error, re.Message);
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
