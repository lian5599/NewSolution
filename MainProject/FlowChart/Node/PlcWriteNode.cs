using Northwoods.Go;
using System;
using TKS.FlowChart.Tool;

namespace FlowCharter
{
    [Serializable]
    public class PlcWriteNode : NodeCreaterBase
    {
        public PlcWriteNode(GraphNode graphNode) :base(graphNode)
        {
        }
        public override void CreateNode()
        {
            ToolBase tool = new PlcWriteTool();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            InitShape(new GoDrawing(GoFigure.Ellipse), color9, color9, 28, 12, 28, 12);
            UpdatePorts("io", "io", "io", "io");
        }
    }
}
