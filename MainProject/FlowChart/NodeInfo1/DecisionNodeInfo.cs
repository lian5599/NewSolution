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

namespace FlowCharter
{
    [Serializable]
    public class DecisionNodeInfo : GraphNodeInfoBase
    {
        public DecisionNodeInfo(GraphNode n) : base(n) { }

        bool lastResult = false;

        protected override Result Action()
        {
            lastResult = false;
            try
            {
                //lastResult = Output.GetOutputData(actionId, nameId).ToString() == Value;
                foreach (var port in GraphNode.Ports)
                {
                    var links = GraphNode.SourceLinks.Where(link => link.ToPort == port);
                    if (links.Count() > 0)
                    {
                        var nodes = links.Select(link => link.FromNode);
                        bool currentPortNodes = true;
                        foreach (var node in nodes)
                        {
                            var link = links.Where(item => item.FromNode == node);

                            bool opposite = ((link.First() as GoLabeledLink).MidLabel as GoText).Text == "n";//是否取反
                            var nodeInfo = node.UserObject as GraphNodeInfoBase;
                            if (nodeInfo.Result == null)
                            {
                                nodeInfo.Run();
                            }
                            bool subNode = opposite ? !nodeInfo.Result.IsSuccess
                                : (nodeInfo.Result.IsSuccess);
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
            Console.WriteLine(this.ToString() + " runall");
            Result = this.Run();
            //GraphNode.Sources;
            foreach (var item in GraphNode.Destinations)
            {
                GraphNode subNode = item as GraphNode;
                var currentLinks = from link in subNode.Links
                                   where link.FromNode == GraphNode
                                   select link.GoObject;
                if (((currentLinks.First() as GoLabeledLink).MidLabel as GoText).Text == (lastResult ? "y" : "n"))
                {
                    Result = (subNode.UserObject as GraphNodeInfoBase).RunAll();
                }
            }
            return Result;
        }
    }

}
