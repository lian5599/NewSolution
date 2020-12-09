using Northwoods.Go;
using System;
using System.Drawing;
using TKS.FlowChart.Tool;

namespace FlowCharter
{
    public class NodeCreaterBase
    {
        protected GraphNode GraphNode;
        public NodeCreaterBase(GraphNode graphNode)
        {
            this.GraphNode = graphNode;
            graphNode.Label.Wrapping = true;
            graphNode.Label.Editable = true;
            graphNode.Label.Alignment = GoObject.Middle;
            graphNode.Label.TextColor = Color.White;
            graphNode.Label.Font = new Font("Calibri", 12, FontStyle.Bold);
            graphNode.Editable = true;
        }
        public virtual void CreateNode()
        {
            ToolBase tool = new ToolBase();
            tool.graphNode = GraphNode;
            GraphNode.UserObject = tool;
            InitShape(new GoDrawing(GoFigure.Ellipse), color2, color2, 28, 12, 28, 12);
            UpdatePorts("io", "io", "io", "io");
        }

        protected void UpdatePorts(String t, String r, String b, String l)
        {  // TopPort, RightPort, BottomPort, LeftPort
            if (t == "")
            {
                GraphNode.TopPort = null;
            }
            else
            {
                if (GraphNode.TopPort == null) GraphNode.TopPort = CreatePort(GoObject.MiddleTop);
                if (GraphNode.TopPort != null)
                {
                    GraphNode.TopPort.IsValidFrom = t.IndexOf('o') > -1;
                    GraphNode.TopPort.IsValidTo = t.IndexOf('i') > -1;
                }
            }
            if (r == "")
            {
                GraphNode.RightPort = null;
            }
            else
            {
                if (GraphNode.RightPort == null) GraphNode.RightPort = CreatePort(GoObject.MiddleRight);
                if (GraphNode.RightPort != null)
                {
                    GraphNode.RightPort.IsValidFrom = r.IndexOf('o') > -1;
                    GraphNode.RightPort.IsValidTo = r.IndexOf('i') > -1;
                }
            }
            if (b == "")
            {
                GraphNode.BottomPort = null;
            }
            else
            {
                if (GraphNode.BottomPort == null) GraphNode.BottomPort = CreatePort(GoObject.MiddleBottom);
                if (GraphNode.BottomPort != null)
                {
                    GraphNode.BottomPort.IsValidFrom = b.IndexOf('o') > -1;
                    GraphNode.BottomPort.IsValidTo = b.IndexOf('i') > -1;
                }
            }
            if (l == "")
            {
                GraphNode.LeftPort = null;
            }
            else
            {
                if (GraphNode.LeftPort == null) GraphNode.LeftPort = CreatePort(GoObject.MiddleLeft);
                if (GraphNode.LeftPort != null)
                {
                    GraphNode.LeftPort.IsValidFrom = l.IndexOf('o') > -1;
                    GraphNode.LeftPort.IsValidTo = l.IndexOf('i') > -1;
                }
            }
        }

        protected  GoPort CreatePort(int spot)
        {
            GoPort p = GraphNode.CreatePort(spot);
            p.Size = new SizeF(p.Size.Width * 2, p.Size.Height * 2);
            p.EndSegmentLength = 15 + 5;  // arrowheadlength + some offset 
            p.Brush = null;
            return p;
        }

        public virtual void InitShape(GoShape shape, Color bColor, Color pColor, float tmx, float tmy, float bmx, float bmy)
        {
            bool solidColor = false;
            if (solidColor)
            {
                shape.BrushColor = bColor;
                shape.PenColor = pColor;
            }
            else
            {
                shape.FillSimpleGradient(bColor, Lighter(bColor), GoObject.MiddleTop);//MiddleTop = 32
                shape.PenColor = Darker(bColor);
            }
            GraphNode.TopLeftMargin = new SizeF(tmx, tmy);
            GraphNode.BottomRightMargin = new SizeF(bmx, bmy);
            GraphNode.Background = shape;
            GraphNode.Text = GraphNode.Kind;
        }

        private Color Darker(Color c)
        {
            return Color.FromArgb(Darken(c.R), Darken(c.G), Darken(c.B));
        }

        private int Darken(int a)
        {
            a = a - 30;
            if (a < 0) a = 0;
            return a;
        }
        private Color Lighter(Color c)
        {
            return Color.FromArgb(Lighten(c.R), Lighten(c.G), Lighten(c.B));
        }
        private int Lighten(int a)
        {
            a = a - 30;
            if (a < 0) a = 0;
            return a;
        }

        protected Color color1 = Color.FromArgb(247, 182, 31);  // yellowish
        protected Color color2 = Color.FromArgb(231, 121, 108); // orange-pinkish
        protected Color color3 = Color.FromArgb(44, 167, 160);  //aqua
        protected Color color4 = Color.FromArgb(81, 160, 239);  //sky blue
        protected Color color5 = Color.FromArgb(97, 143, 175);  // pale blue
        protected Color color6 = Color.FromArgb(212, 74, 80);  //  reddish pink
        protected Color color7 = Color.FromArgb(141, 109, 120); //light purple
        protected Color color8 = Color.FromArgb(114, 112, 132); // blueish grey
        protected Color color9 = Color.FromArgb(162, 119, 80);  // dull gold
        protected Color color10 = Color.FromArgb(202, 81, 20);  // dark orange
        protected Color color11 = Color.FromArgb(77, 96, 130); // steel blue
    }
}
