using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.FlowChart.Tool;

namespace FlowCharter
{
    [Serializable]
    public class EndNode : NodeCreaterBase
    {
        public EndNode(GraphNode graphNode) : base(graphNode)
        {
        }
        public override void CreateNode()
        {
            ToolBase tool = new ToolBase();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            GoRoundedRectangle rect = new GoRoundedRectangle();
            InitShape(rect, color6, color6, 20, 8, 20, 8);
            UpdatePorts("i", "i", "", "i");
        }
    }
}
