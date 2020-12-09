using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    public class SubFlowTool:ToolBase
    {
        protected override void Init()
        {
            base.Init();
        }

        protected override Result Action()
        {
            try
            {
                string SubFlowId = graphNode.Text;
                if (Flow.Instance.SubFlows.ContainsKey(SubFlowId))
                {
                    return (Flow.Instance.SubFlows[SubFlowId].UserObject as ToolBase).RunAll();
                }
                else if (Flow.Instance.SubFlowsEx.ContainsKey(SubFlowId))
                {
                    return Flow.Instance.SubFlowsEx[SubFlowId].Run();
                }
                else
                {
                    string msg = "不存在名为\"" + SubFlowId + "\"的子流程";
                    Log.Run(LogLevel.Error, msg);
                    return new Result(msg, (int)ErrorCode.fatalError);
                }
            }
            catch (Exception e)
            {
                Log.Run(LogLevel.Error, e.Message,e);
                return new Result(e.Message, (int)ErrorCode.fatalError);
            }
        }
    }
}
