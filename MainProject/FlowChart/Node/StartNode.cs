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
    public class StartNode : NodeCreaterBase
    {
        public StartNode(GraphNode graphNode) : base(graphNode)
        {
        }
        public override void CreateNode()
        {
            ToolBase tool = new StartTool();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            GoRoundedRectangle rect = new GoRoundedRectangle();
            InitShape(rect, color3, color3, 20, 8, 20, 8);
            UpdatePorts("", "o", "o", "o");
        }
    }
}
