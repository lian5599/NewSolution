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
    public class OutputNode : NodeCreaterBase
    {
        public OutputNode(GraphNode graphNode) : base(graphNode)
        {
        }
        public override void CreateNode()
        {
            ToolBase tool = new OutputTool();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            InitShape(new GoDrawing(GoFigure.Ellipse), color11, color11, 28, 12, 28, 12);
            UpdatePorts("io", "io", "io", "io");
        }
    }
}
