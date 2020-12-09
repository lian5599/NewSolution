using FlowCharter;
using Helper;
using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.Manager;

namespace TKS.FlowChart.Tool
{
    //todo complete
    public class DecisionTool : ToolBase
    {
        bool lastResult = false;
        protected override Result Action()
        {
            lastResult = false;
            try
            {
                foreach (var port in graphNode.Ports)
                {
                    var links = graphNode.SourceLinks.Where(link => link.ToPort == port);
                    if (links.Count() > 0)
                    {
                        var nodes = links.Select(link => link.FromNode);
                        bool currentPortNodes = true;
                        foreach (var node in nodes)
                        {
                            var link = links.Where(item => item.FromNode == node);

                            bool opposite = ((link.First() as GoLabeledLink).MidLabel as GoText).Text == "N";//是否取反
                            var tool = node.UserObject as ToolBase;
                            if (tool.Result == null)
                            {
                                tool.Run();
                            }
                            bool subNode = opposite ? !tool.Result.IsSuccess
                                : (tool.Result.IsSuccess);
                            currentPortNodes = currentPortNodes && subNode;
                            if (!currentPortNodes) break;
                        }
                        lastResult = lastResult || currentPortNodes;
                        if (lastResult) break;
                    }
                }
                return new Result(lastResult);
            }
            catch (Exception e)
            {
                Log.Run(LogLevel.Error, e.Message);
                return new Result(e.Message);
            }
        }

        public override Result RunAll()
        {
            return Action();
        }
    }
}
