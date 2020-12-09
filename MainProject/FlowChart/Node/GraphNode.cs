/*
 *  Copyright ?Northwoods Software Corporation, 1998-2020. All Rights
 *  Reserved.
 *
 *  Restricted Rights: Use, duplication, or disclosure by the U.S.
 *  Government is subject to restrictions as set forth in subparagraph
 *  (c) (1) (ii) of DFARS 252.227-7013, or in FAR 52.227-19, or in FAR
 *  52.227-14 Alt. III, as applicable.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Northwoods.Go;
using TKS.FlowChart.Tool;

namespace FlowCharter
{
    /// <summary>
    /// A node representing a step or action in a flowchart.
    /// </summary>
    [Serializable]
    public class GraphNode : GoTextNode
    {
        private NodeCreaterBase NodeCreater;
        public GraphNode()
        {
            //InitCommon();
        }
        protected void InitCommon()
        {
            // assume GraphNodeKind.Step                   
            this.Label.Wrapping = true;
            this.Label.Editable = true;
            this.Label.Alignment = Middle;
            this.Label.TextColor = Color.White;
            this.Label.Font = new Font("Calibri", 12, FontStyle.Bold);
            this.Editable = true;
        }

        /// <summary>
        /// Make sure the GraphNodeInfo, held in the UserObject property,
        /// is specific for the newly copied GraphNode.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public override GoObject CopyObject(GoCopyDictionary env)
        {
            GraphNode newobj = (GraphNode)base.CopyObject(env);
            if (newobj != null)
            {
                if (newobj.UserObject != null)
                {
                    try
                    {
                        var oldTool = newobj.UserObject as ToolBase;
                        newobj.UserObject = Activator.CreateInstance(newobj.UserObject.GetType());
                        var newTool = newobj.UserObject as ToolBase;
                        (newobj.UserObject as ToolBase).graphNode = newobj;
                        for (int i = 0; i < newTool.Settings.Count(); i++)
                        {
                            newTool.Settings[i].Value = oldTool.Settings[i].DeepCopyValue();
                        }
                        //(newobj.UserObject as ToolBase).Settings = oldTool.Settings.DeepCopy();
                    }
                    catch (Exception e)
                    {

                        //throw;
                    }
                }
                else newobj.UserObject = new ToolBase(newobj);
            }
            //newobj.Text += " copy";
            return newobj;
        }

        /// <summary>
        /// The location for each node is the Center.
        /// </summary>
        public override PointF Location
        {
            get { return this.Center; }
            set { this.Center = value; }
        }

        /// <summary>
        /// Adjust port positions for certain background shapes.
        /// </summary>
        /// <param name="childchanged"></param>
        public override void LayoutChildren(GoObject childchanged)
        {
            base.LayoutChildren(childchanged);
            GoDrawing draw = this.Background as GoDrawing;
            if (draw != null)
            {
                PointF tempPoint;
                if (draw.Figure == GoFigure.ManualOperation || draw.Figure == GoFigure.Input || draw.Figure == GoFigure.Output)
                {
                    if (this.RightPort != null)
                    {
                        draw.GetNearestIntersectionPoint(new PointF(this.RightPort.Center.X + .01f, this.RightPort.Center.Y),
                          this.RightPort.Center, out tempPoint);
                        this.RightPort.Right = tempPoint.X;
                    }
                    if (this.LeftPort != null)
                    {
                        draw.GetNearestIntersectionPoint(new PointF(this.LeftPort.Center.X + .01f, this.LeftPort.Center.Y),
                          this.LeftPort.Center, out tempPoint);
                        this.LeftPort.Left = tempPoint.X;
                    }
                }
            }
        }

        /// <summary>
        /// When the mouse passes over a node, display all of its ports.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        /// <remarks>
        /// All ports on all nodes are hidden when the mouse hovers over the background.
        /// </remarks>
        public override bool OnMouseOver(GoInputEventArgs evt, GoView view)
        {
            GraphView v = view as GraphView;
            if (v != null)
            {
                foreach (GoPort p in this.Ports)
                {
                    p.SkipsUndoManager = true;
                    p.Style = GoPortStyle.Ellipse;
                    p.SkipsUndoManager = false;
                }
            }
            return false;
        }

        /// <summary>
        /// Bring up a GraphNode specific context menu.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public override GoContextMenu GetContextMenu(GoView view)
        {
            if (view is GoOverview) return null;
            if (!(view.Document is GraphDoc)) return null;
            GoContextMenu cm = new GoContextMenu(view);
            cm.Items.Add(new ToolStripMenuItem("Exe", null, new EventHandler(this.Exe_Command)));
            return cm;
        }

        public void DrawRelationship_Command(Object sender, EventArgs e)
        {
            GoView v = GoContextMenu.FindView(sender as ToolStripMenuItem);
            if (v != null)
            {
                //RelationshipTool t = new RelationshipTool(v);
                //t.Predecessor = this;
                //v.Tool = t;
            }
        }

        public void Cut_Command(Object sender, EventArgs e)
        {
            GoView v = GoContextMenu.FindView(sender as ToolStripMenuItem);
            if (v != null)
                v.EditCut();
        }

        public void Copy_Command(Object sender, EventArgs e)
        {
            GoView v = GoContextMenu.FindView(sender as ToolStripMenuItem);
            if (v != null)
                v.EditCopy();
        }

        public async void Exe_Command(Object sender, EventArgs e)
        {
            ToolBase tool = this.UserObject as ToolBase;
            if (tool != null)
            {
                await Task.Run(() => 
                {
                    tool.Run();
                });
            }
        }

        private string kind;
        public string Kind 
        {
            get
            {
                return kind;
            }
            set
            {
                if (kind!=value)
                {
                    kind = value;

                    var assembly = Assembly.GetAssembly(typeof(NodeCreaterBase));
                    Type[] types = assembly.GetTypes();
                    var nodes = types.Where(t => t.BaseType == typeof(NodeCreaterBase));
                    foreach (var item in nodes)
                    {
                        if (kind == item.Name.Replace("Node",""))
                        {
                            NodeCreater = (NodeCreaterBase)Activator.CreateInstance(item, this);
                            NodeCreater.CreateNode();                          
                            return;
                        }
                    }
                    throw new Exception("没有对应的类型\"" + kind + "Node\"");
                }
            }
            }

        public new GoPort CreatePort(int spot)
        {
            GoPort p = base.CreatePort(spot);
            p.Size = new SizeF(p.Size.Width * 2, p.Size.Height * 2);
            p.EndSegmentLength = 15 + 5;  // arrowheadlength + some offset 
            p.Brush = null;
            return p;
        }

        //public virtual void InitShape(GoShape shape, Color bColor, Color pColor, float tmx, float tmy, float bmx, float bmy)
        //{
        //    kind = this.GetType().Name.Replace("Node", "");
        //    bool solidColor = false;
        //    if (solidColor)
        //    {
        //        shape.BrushColor = bColor;
        //        shape.PenColor = pColor;
        //    }
        //    else
        //    {
        //        shape.FillSimpleGradient(bColor, Lighter(bColor), MiddleTop);
        //        shape.PenColor = Darker(bColor);
        //    }
        //    this.TopLeftMargin = new SizeF(tmx, tmy);
        //    this.BottomRightMargin = new SizeF(bmx, bmy);
        //    this.Background = shape;
        //    this.Text = kind;
        //}


        ////protected virtual void OnKindChanged(GraphNodeKind oldkind, GraphNodeKind newkind)
        ////{
        ////    // update the parts, based on the Kind of node this now is
        ////    switch (newkind)
        ////    {
        ////        case GraphNodeKind.Start:
        ////            {
        ////                GoRoundedRectangle rect = new GoRoundedRectangle();
        ////                InitShape("Start", rect, color3, color3, 20, 5, 20, 3);
        ////                UpdatePorts("", "o", "o", "o");
        ////                rect.Corner = new SizeF(this.Height / 2, this.Height / 2); // perfect rounded ends
        ////                this.UserObject = new GraphNodeInfo(this);//bylzy
        ////                break;
        ////            }
        ////        case GraphNodeKind.End:
        ////            {
        ////                GoRoundedRectangle rect = new GoRoundedRectangle();
        ////                InitShape("End", rect, color6, color6, 20, 5, 20, 3);
        ////                UpdatePorts("i", "i", "", "i");
        ////                rect.Corner = new SizeF(this.Height / 2, this.Height / 2);
        ////                break;
        ////            }
        ////        case GraphNodeKind.Step:
        ////            {
        ////                InitShape("Step", new GoDrawing(GoFigure.Rectangle), color4, color4, 22, 15, 22, 15);
        ////                UpdatePorts("io", "io", "io", "io");
        ////                break;
        ////            }
        ////        case GraphNodeKind.Input:
        ////            {
        ////                InitShape("Input", new GoDrawing(GoFigure.Input), color7, color7, 28, 12, 28, 12);
        ////                UpdatePorts("io", "io", "io", "io");
        ////                break;
        ////            }
        ////        case GraphNodeKind.Output:
        ////            {
        ////                InitShape("Output", new GoDrawing(GoFigure.Input), color8, color8, 28, 12, 28, 12);
        ////                UpdatePorts("io", "io", "io", "io");
        ////                break;
        ////            }
        ////        case GraphNodeKind.Decision:
        ////            {
        ////                InitShape("Decision", new GoDrawing(GoFigure.Decision), color1, color1, 35, 20, 35, 20);
        ////                UpdatePorts("i", "io", "o", "io");
        ////                break;
        ////            }
        ////        case GraphNodeKind.Read:
        ////            {
        ////                InitShape("Read", new GoDrawing(GoFigure.Ellipse), color2, color2, 28, 12, 28, 12);
        ////                UpdatePorts("io", "io", "io", "io");
        ////                break;
        ////            }
        ////        case GraphNodeKind.Write:
        ////            {
        ////                InitShape("Write", new GoDrawing(GoFigure.Ellipse), color9, color9, 28, 12, 28, 12);
        ////                UpdatePorts("io", "io", "io", "io");
        ////                break;
        ////            }
        ////        case GraphNodeKind.ManualOperation:
        ////            {
        ////                InitShape("Manual \nOperation", new GoDrawing(GoFigure.ManualOperation), color10, color10, 20, 10, 20, 10);
        ////                UpdatePorts("io", "io", "io", "io");
        ////                break;
        ////            }
        ////        case GraphNodeKind.Database:
        ////            {
        ////                InitShape("Database", new GoDrawing(GoFigure.Database), color11, Darker(color11), 10, 30, 10, 15);
        ////                UpdatePorts("io", "io", "io", "io");
        ////                break;
        ////            }
        ////        case GraphNodeKind.PLC:
        ////            {
        ////                InitShape(newkind.ToString(), new GoDrawing(GoFigure.Ellipse), color2, color2, 28, 12, 28, 12);
        ////                UpdatePorts("io", "io", "io", "io");
        ////                this.UserObject = new PlcNodeInfo(this);//bylzy
        ////                break;
        ////            }
        ////        default: throw new InvalidEnumArgumentException("newkind", (int)newkind, typeof(GraphNodeKind));
        ////    }
        ////}

        //private Color Darker(Color c)
        //{
        //    return Color.FromArgb(Darken(c.R), Darken(c.G), Darken(c.B));
        //}

        //private int Darken(int a)
        //{
        //    a = a - 30;
        //    if (a < 0) a = 0;
        //    return a;
        //}
        //private Color Lighter(Color c)
        //{
        //    return Color.FromArgb(Lighten(c.R), Lighten(c.G), Lighten(c.B));
        //}
        //private int Lighten(int a)
        //{
        //    a = a - 30;
        //    if (a < 0) a = 0;
        //    return a;
        //}


        //protected void UpdatePorts(String t, String r, String b, String l)
        //{  // TopPort, RightPort, BottomPort, LeftPort
        //    if (t == "")
        //    {
        //        this.TopPort = null;
        //    }
        //    else
        //    {
        //        if (this.TopPort == null) this.TopPort = CreatePort(MiddleTop);
        //        if (this.TopPort != null)
        //        {
        //            this.TopPort.IsValidFrom = t.IndexOf('o') > -1;
        //            this.TopPort.IsValidTo = t.IndexOf('i') > -1;
        //        }
        //    }
        //    if (r == "")
        //    {
        //        this.RightPort = null;
        //    }
        //    else
        //    {
        //        if (this.RightPort == null) this.RightPort = CreatePort(MiddleRight);
        //        if (this.RightPort != null)
        //        {
        //            this.RightPort.IsValidFrom = r.IndexOf('o') > -1;
        //            this.RightPort.IsValidTo = r.IndexOf('i') > -1;
        //        }
        //    }
        //    if (b == "")
        //    {
        //        this.BottomPort = null;
        //    }
        //    else
        //    {
        //        if (this.BottomPort == null) this.BottomPort = CreatePort(MiddleBottom);
        //        if (this.BottomPort != null)
        //        {
        //            this.BottomPort.IsValidFrom = b.IndexOf('o') > -1;
        //            this.BottomPort.IsValidTo = b.IndexOf('i') > -1;
        //        }
        //    }
        //    if (l == "")
        //    {
        //        this.LeftPort = null;
        //    }
        //    else
        //    {
        //        if (this.LeftPort == null) this.LeftPort = CreatePort(MiddleLeft);
        //        if (this.LeftPort != null)
        //        {
        //            this.LeftPort.IsValidFrom = l.IndexOf('o') > -1;
        //            this.LeftPort.IsValidTo = l.IndexOf('i') > -1;
        //        }
        //    }
        //}

        public bool IsPredecessor
        {
            get { return this.BottomPort != null; }
        }

        public override void ChangeValue(GoChangedEventArgs e, bool undo)
        {
            if (e.SubHint == ChangedKind) ;
            //myKind = (GraphNodeKind)e.GetInt(undo);
            else
                base.ChangeValue(e, undo);
        }

        //protected Color color1 = Color.FromArgb(247, 182, 31);  // yellowish
        //protected Color color2 = Color.FromArgb(231, 121, 108); // orange-pinkish
        //protected Color color3 = Color.FromArgb(44, 167, 160);  //aqua
        //protected Color color4 = Color.FromArgb(81, 160, 239);  //sky blue
        //protected Color color5 = Color.FromArgb(97, 143, 175);  // pale blue
        //protected Color color6 = Color.FromArgb(212, 74, 80);  //  reddish pink
        //protected Color color7 = Color.FromArgb(141, 109, 120); //light purple
        //protected Color color8 = Color.FromArgb(114, 112, 132); // blueish grey
        //protected Color color9 = Color.FromArgb(162, 119, 80);  // dull gold
        //protected Color color10 = Color.FromArgb(202, 81, 20);  // dark orange
        //protected Color color11 = Color.FromArgb(77, 96, 130); // steel blue


        public const int ChangedKind = GoObject.LastChangedHint + 7;

    }
}
