﻿using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKS.FlowChart.Tool;

namespace FlowCharter
{
    [Serializable]
    public class ComparsionNode : NodeCreaterBase
    {
        public ComparsionNode(GraphNode graphNode) : base(graphNode)
        {
        }
        public override void CreateNode()
        {
            ToolBase tool = new ComparsionTool();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            InitShape(new GoDrawing(GoFigure.Decision), color1, color1, 35, 20, 35, 20);
            UpdatePorts("io", "io", "io", "io");
        }
    }
}
