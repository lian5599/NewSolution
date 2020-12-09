using Helper;
using TKS.Manager;
using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TKS;
using System.Reflection;

namespace FlowCharter
{
    [Serializable]
    public class SubFlowNodeInfo : GraphNodeInfoBase
    {
        [Category("Param")]
        [Description("value")]
        public string SubFlowId { get; set; }
        public SubFlowNodeInfo(GraphNode n) : base(n) { }

        protected override Result Action()
        {
            try
            {
                if (Flow.Instance.SubFlows.ContainsKey(SubFlowId))
                {
                    return (Flow.Instance.SubFlows[SubFlowId].UserObject as GraphNodeInfoBase).RunAll();
                }
                else
                {
                    return Flow.Instance.SubFlowsEx[SubFlowId].Run();
                }
                //(item.UserObject as GraphNodeInfoBase).RunAll();
            }
            catch (Exception e)
            {
                Log.Run(LogLevel.Error, e.Message);
                return new Result(e.Message);
            }
        }
    }

}
