using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace FlowCharter
{
    public partial class PaletteForm : DockContent
    {
        #region singleton
        private static volatile PaletteForm _instance = null;
        private static readonly object _locker = new object();
        public static PaletteForm GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new PaletteForm();
                    }
                }
            }
            return _instance;
        }
        #endregion
        private PaletteForm()
        {
            myPalette = new MyPalette();
            myPalette.Border3DStyle = System.Windows.Forms.Border3DStyle.Sunken;
            myPalette.Dock = DockStyle.Fill;
            myPalette.GridOrigin = new PointF(10, 10);
            myPalette.GridCellSize = new SizeF(32, 18);
            //myPalette.GridStyle = GoViewGridStyle.Line;
            myPalette.Sorting = SortOrder.Ascending;
            myPalette.Comparer = new PaletteSorter();
            this.Controls.Add(myPalette);
            myPalette.Dock = System.Windows.Forms.DockStyle.Fill;
            InitializeCatalog();
            InitializeComponent();
        }
        private void InitializeCatalog()
        {
            //GoComment c = new GoComment();
            //c.TopLeftMargin = new SizeF(8, 8);
            //c.BottomRightMargin = new SizeF(8, 8);
            //c.Shadowed = false;
            //c.Text = "comments here";
            //myPalette.Document.Add(c);
            //GraphNodeBase n;
            //n = new GraphNodeBase(GraphNodeKind.Start);
            //myPalette.Document.Add(n);
            //n = new GraphNode(GraphNodeKind.End);
            //myPalette.Document.Add(n);
            //n = new GraphNode(GraphNodeKind.Step);
            //myPalette.Document.Add(n);
            //n = new GraphNode(GraphNodeKind.Input);
            //myPalette.Document.Add(n);
            //n = new GraphNode(GraphNodeKind.Output);
            //myPalette.Document.Add(n);
            //n = new GraphNode(GraphNodeKind.Decision);
            //myPalette.Document.Add(n);
            //// myPalette.GridCellSize = new SizeF(40, (int)n.Height + 3);
            //n = new GraphNode(GraphNodeKind.Read);
            //myPalette.Document.Add(n);
            //n = new GraphNode(GraphNodeKind.Write);
            //myPalette.Document.Add(n);
            //n = new GraphNode(GraphNodeKind.ManualOperation);
            //myPalette.Document.Add(n);
            //n = new GraphNode(GraphNodeKind.Database);
            //myPalette.Document.Add(n);
            //n = new GraphNodeBase(GraphNodeKind.PLC);
            //myPalette.Document.Add(n);
            GraphNode n;
            var assembly = Assembly.GetAssembly(typeof(NodeCreaterBase));
            Type[] types = assembly.GetTypes();
            var nodes = types.Where(t => t.BaseType == typeof(NodeCreaterBase));//(t => t.IsSubclassOf(typeof(CommunicationBase)));
            foreach (var item in nodes)
            {
                n = new GraphNode();
                n.Kind = item.Name.Replace("Node", "");
                myPalette.Document.Add(n);
            }
        }

        private void PaletteForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _instance = null;
        }
    }

    public class MyPalette : GoPalette
    {

        public override void LayoutItems()
        {
            if (!this.AutomaticLayout) return;

            bool vert = (this.Orientation == Orientation.Vertical);

            ICollection<GoObject> coll = this.Document;
            if (this.Sorting != SortOrder.None && this.Comparer != null)
            {
                GoObject[] a = this.Document.CopyArray();
                Array.Sort<GoObject>(a, 0, a.Length, this.Comparer);
                if (this.Sorting == SortOrder.Descending)
                    Array.Reverse(a, 0, a.Length);
                coll = a;
            }

            // position all objects so they don't overlap and no
            // opposite scrollbar is needed
            SizeF viewsize = this.DocExtentSize;
            SizeF cellsize = this.GridCellSize;
            PointF gridorigin = this.GridOrigin;
            bool useselobj = this.AlignsSelectionObject;
            bool first = true;
            PointF pnt = gridorigin;
            float maxcol = Math.Min(gridorigin.X, 0);
            float maxrow = Math.Min(gridorigin.Y, 0);

            foreach (GoObject obj in coll)
            {
                // maybe operate on SelectionObject instead of whole object
                GoObject selobj = obj;
                if (useselobj)
                {
                    selobj = obj.SelectionObject;
                    if (selobj == null)
                        selobj = obj;
                }
                selobj.Position = pnt;
                if (vert)
                {
                    pnt = ShiftRight(obj, selobj, maxcol, pnt, cellsize);
                    if (!first && obj.Right >= viewsize.Width)
                    {  // new row?
                        maxcol = Math.Min(gridorigin.X, 0);
                        pnt.X = gridorigin.X;
                        while (pnt.Y < maxrow) pnt.Y = pnt.Y + cellsize.Height;
                        //pnt.Y = Math.Max(pnt.Y + cellsize.Height, maxrow);
                        selobj.Position = pnt;
                        pnt = ShiftRight(obj, selobj, maxcol, pnt, cellsize);
                    }
                    pnt.X += cellsize.Width;
                }
                else
                {  // horizontal orientation
                    pnt = ShiftDown(obj, selobj, maxrow, pnt, cellsize);
                    if (!first && obj.Bottom >= viewsize.Height)
                    {  // new column?
                        maxrow = Math.Min(gridorigin.Y, 0);
                        pnt.Y = gridorigin.Y;
                        pnt.X = Math.Max(pnt.X + cellsize.Width, maxcol);
                        selobj.Position = pnt;
                        pnt = ShiftDown(obj, selobj, maxrow, pnt, cellsize);
                    }
                    pnt.Y += cellsize.Height;
                }
                maxcol = Math.Max(maxcol, obj.Right);
                maxrow = Math.Max(maxrow, obj.Bottom);
                first = false;
            }

            // minimize the size of the document
            this.Document.Bounds = ComputeDocumentBounds();
        }

        private PointF ShiftDown(GoObject obj, GoObject selobj, float maxrow, PointF pnt, SizeF cellsize)
        {
            while (obj.Top < maxrow)
            {
                pnt.Y += cellsize.Height;
                float old = obj.Top;
                selobj.Top = pnt.Y;
                if (obj.Top <= old) break;
            }
            return pnt;
        }

        private PointF ShiftRight(GoObject obj, GoObject selobj, float maxcol, PointF pnt, SizeF cellsize)
        {
            while (obj.Left < maxcol)
            {
                pnt.X += cellsize.Width;
                float old = obj.Left;
                selobj.Left = pnt.X;
                if (obj.Left <= old) break;
            }
            return pnt;
        }
    }

    public class PaletteSorter : IComparer<GoObject>
    {
        public PaletteSorter() { }
        public int Compare(GoObject x, GoObject y)
        {
            GoGroup m = (GoGroup)x;
            GoGroup n = (GoGroup)y;
            if (m == null || n == null) return 0;
            //if (m.Width == n.Width) return 0;
            //if (m.Width > n.Width) return 1;
            if (m.Height == n.Height) return 0;
            if (m.Height > n.Height) return 1;
            return -1;
        }
    }
}
