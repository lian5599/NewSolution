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
    public class SrtingFormatNode : NodeCreaterBase
    {
        public SrtingFormatNode(GraphNode graphNode) : base(graphNode)
        {
        }
        public override void CreateNode()
        {
            ToolBase tool = new StringFormatTool();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            InitShape(new GoDrawing(GoFigure.Output), color8, color8, 15, 12, 15, 12);
            UpdatePorts("io", "io", "io", "io");
        }
    }
}
