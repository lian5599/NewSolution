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
    public class MesNode : NodeCreaterBase
    {
        public MesNode(GraphNode graphNode) : base(graphNode)
        {
        }
        public override void CreateNode()
        {
            ToolBase tool = new MesTool();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            GoRoundedRectangle rect = new GoRoundedRectangle();
            InitShape(new GoDrawing(GoFigure.Database), color11, color11, 20, 15, 20, 15);
            UpdatePorts("io", "io", "io", "io");
        }
    }
}
