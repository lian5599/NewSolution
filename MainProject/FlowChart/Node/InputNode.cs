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
    public class InputNode : NodeCreaterBase
    {
        public InputNode(GraphNode graphNode) : base(graphNode)
        {
        }
        public override void CreateNode()
        {
            ToolBase tool = new InputTool();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            InitShape(new GoDrawing(GoFigure.Ellipse), color10, color10, 28, 12, 28, 12);
            UpdatePorts("io", "io", "io", "io");
        }
    }
}
