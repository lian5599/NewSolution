using TKS.Manager;
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
    public class ScanNode:NodeCreaterBase
    {
        public ScanNode(GraphNode graphNode) :base(graphNode)
        {
        }
        public override void CreateNode()
        {
            ToolBase tool = new ScanTool();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            InitShape(new GoDrawing(GoFigure.Cloud), color1, color1, 28, 12, 28, 12);
            UpdatePorts("io", "io", "io", "io");
        }
    }
}
