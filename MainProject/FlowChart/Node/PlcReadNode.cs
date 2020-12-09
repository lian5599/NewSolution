using Northwoods.Go;
using System;
using TKS.FlowChart.Tool;

namespace FlowCharter
{
    [Serializable]
    public class PlcReadNode : NodeCreaterBase
    {
        public PlcReadNode(GraphNode graphNode) :base(graphNode)
        {
        }
        public override void CreateNode()
        {
            ToolBase tool = new PlcReadTool();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            InitShape(new GoDrawing(GoFigure.Ellipse), color2, color2, 28, 12, 28, 12);
            UpdatePorts("io", "io", "io", "io");
        }
    }
}
