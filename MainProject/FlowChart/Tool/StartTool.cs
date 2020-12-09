using FlowCharter;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    public class StartTool : ToolBase
    {
        protected override void Init()
        {
            base.Init();
        }

        protected override Result Action()
        {
            graphNode.Shadowed = false;
            Result = new Result();
            foreach (var item in graphNode.Destinations)
            {
                ToolBase tool = (item as GraphNode).UserObject as ToolBase;

                if (Result.ErrorCode != (int)ErrorCode.fatalError)
                {
                    Result = tool.RunAll();
                }
            }
            return Result;
        }

        public override Result RunAll()
        {
            return Action();
        }
    }
}
