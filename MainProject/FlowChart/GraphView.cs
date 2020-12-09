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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;  // clipboard methods
using System.Drawing.Imaging;          // MetaFile
using System.IO;                       // MemoryStream (for metafile)
using Northwoods.Go;

namespace FlowCharter
{
    public class GraphView : GoView
    {
        public GraphView()
        {
            this.NewLinkClass = typeof(GraphLink);
            this.PortGravity = 30;
            this.GridCellSize = new SizeF(5, 10);
            this.GridSnapDrag = GoViewSnapStyle.Jump;
            this.DragsRealtime = true;
        }

        /// <summary>
        /// A new GraphView will have a GraphDoc as its document.
        /// </summary>
        /// <returns>A <see cref="GraphDoc"/></returns>
        public override GoDocument CreateDocument()
        {
            GoDocument doc = new GraphDoc();
            doc.UndoManager = new GoUndoManager();
            return doc;
        }

        /// <summary>
        /// A convenience property for getting the view's GoDocument as a GraphDoc.
        /// </summary>
        public GraphDoc Doc
        {
            get { return this.Document as GraphDoc; }
        }

        public override IGoLink CreateLink(IGoPort from, IGoPort to)
        {
            IGoLink il = base.CreateLink(from, to);
            if (il != null)
            {
                GoLabeledLink l = il.GoObject as GoLabeledLink;
                if (l != null)
                {
                    GraphNode fromNode = from.Node.GoObject as GraphNode;
                    if (fromNode != null/* && fromNode.Kind == "Decision"*/)
                    {
                        GoText t = new GoText();
                        t.Text = "";
                        t.Selectable = false;
                        t.Editable = true;
                        l.MidLabel = t;
                        //(l as GraphLink).FromOrTo = "To";
                    }
                    //GraphNode toNode = to.Node.GoObject as GraphNode;//bylzydecision
                    //if (toNode != null && toNode.Kind == "Decision")
                    //{
                    //    GoText t = new GoText();
                    //    t.Text = "y";
                    //    t.Selectable = false;
                    //    t.Editable = true;
                    //    l.MidLabel = t;
                    //    //(l as GraphLink).FromOrTo = "From";
                    //}
                    //l.Orthogonal = true;
                    //l.Style = GoStrokeStyle.RoundedLine;
                    //l.ToArrow = true;
                }
            }
            return il;
        }

        /// <summary>
        /// This method is responsible for updating all of the view's visible
        /// state outside of the GoView itself--the title bar, status bar, and properties grid.
        /// </summary>
        public virtual void UpdateFormInfo()
        {
            UpdateTitle();
            //MainForm.App.SetStatusMessage(this.Doc.Location);
            //MainForm.App.SetStatusZoom(this.DocScale);
            //MainForm.App.EnableToolStripEditButtons(this);
            //MainForm.App.EnableToolStripUndoButtons(this);
        }

        /// <summary>
        /// Update the title bar with the view's document's Name, and an indication
        /// of whether the document is read-only and whether it has been modified.
        /// </summary>
        public virtual void UpdateTitle()
        {
            Form win = this.Parent as Form;
            if (win != null)
            {
                String title = this.Document.Name;
                if (this.Doc.IsReadOnly)
                    title += " [Read Only]";
                if (this.Doc.IsModified)
                    title += "*";
                win.Text = title;
            }
        }

        /// <summary>
        /// If the document's name changes, update the title;
        /// if the document's location changes, update the status bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evt"></param>
        protected override void OnDocumentChanged(Object sender, GoChangedEventArgs evt)
        {
            base.OnDocumentChanged(sender, evt);
            if (evt.Hint == GoDocument.ChangedName || evt.Hint == GoDocument.FinishedTransaction || evt.Hint == GoDocument.AbortedTransaction ||
                evt.Hint == GoDocument.RepaintAll || evt.Hint == GoDocument.FinishedUndo || evt.Hint == GoDocument.FinishedRedo)
            {
                UpdateFormInfo();
            }
            else if (evt.Hint == GraphDoc.ChangedLocation)
            {
                //MainForm.App.SetStatusMessage(this.Doc.Location);
            }
            else if (evt.Hint == GoDocument.FinishedTransaction)
            {
                //MainForm.App.EnableToolStripUndoButtons(this);
            }
        }

        /// <summary>
        /// If the view's document is replaced, update the title;
        /// if the view's scale changes, update the status bar
        /// </summary>
        /// <param name="evt"></param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs evt)
        {
            base.OnPropertyChanged(evt);
            if (evt.PropertyName == "Document")
                UpdateFormInfo();
            else if (evt.PropertyName == "DocScale") return;
            //MainForm.App.SetStatusZoom(this.DocScale);
        }

        private GoObject myPrimarySelection = null;

        protected override void OnObjectGotSelection(GoSelectionEventArgs evt)
        {
            base.OnObjectGotSelection(evt);
            if (myPrimarySelection != this.Selection.Primary)
            {
                myPrimarySelection = this.Selection.Primary;
                //MainForm.App.EnableToolStripEditButtons(this);
            }
        }

        protected override void OnObjectLostSelection(GoSelectionEventArgs evt)
        {
            base.OnObjectLostSelection(evt);
            if (myPrimarySelection != this.Selection.Primary)
            {
                myPrimarySelection = this.Selection.Primary;
                //MainForm.App.EnableToolStripEditButtons(this);
            }
        }

        protected override void OnBackgroundHover(GoInputEventArgs evt)
        {
            foreach (GoObject obj in this.Document)
            {
                IGoNode n = obj as IGoNode;
                if (n != null)
                {
                    foreach (GoPort p in n.Ports)
                    {
                        p.SkipsUndoManager = true;
                        p.Style = GoPortStyle.None;
                        p.SkipsUndoManager = false;
                    }
                }
            }
            base.OnBackgroundHover(evt);
        }


        protected override void OnClipboardCopied(EventArgs evt)
        {
            base.OnClipboardCopied(evt);
            //MainForm.App.EnableToolStripEditButtons(this);
        }

        /// <summary>
        /// Bring up a context menu when the user context clicks in the background.
        /// </summary>
        /// <param name="evt"></param>
        protected override void OnBackgroundContextClicked(GoInputEventArgs evt)
        {
            base.OnBackgroundContextClicked(evt);
            // set up the background context menu
            GoContextMenu cm = new GoContextMenu(this);
            //if (CanInsertObjects())
            //    cm.Items.Add(new ToolStripMenuItem("Paste", null, new EventHandler(this.Paste_Command)));
            if (cm.Items.Count > 0)
                cm.Items.Add(new ToolStripSeparator());
            cm.Items.Add(new ToolStripMenuItem("Properties", null, new EventHandler(this.Properties_Command)));
            cm.Show(this, evt.ViewPoint);
        }

        /// <summary>
        /// Called when the user clicks on the background context menu Paste menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// This calls <see cref="GoView.EditPaste"/> and selects all of the newly pasted objects.
        /// </remarks>
        public void Paste_Command(Object sender, EventArgs e)
        {
            PointF docpt = this.LastInput.DocPoint;
            StartTransaction();
            this.Selection.Clear();
            EditPaste();  // selects all newly pasted objects
            RectangleF copybounds = GoDocument.ComputeBounds(this.Selection, this);
            SizeF offset = new SizeF(docpt.X - copybounds.X, docpt.Y - copybounds.Y);
            MoveSelection(this.Selection, offset, true);
            FinishTransaction("Context Paste");
        }

        /// <summary>
        /// Bring up the properties dialog for the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Properties_Command(Object sender, EventArgs e)
        {
            GraphDocForm dlg = new GraphDocForm();
            dlg.Doc = this.Doc;
            dlg.ShowDialog();
        }
        //align all object's left sides to the left side of the primary selection
        public virtual void AlignLeftSides()
        {
            GoObject obj = this.Selection.Primary;
            if (obj != null && !(obj is IGoLink))
            {
                StartTransaction();
                float X = obj.SelectionObject.Left;
                foreach (GoObject temp in this.Selection)
                {
                    GoObject t = temp.SelectionObject;
                    if (!(t is IGoLink))
                        t.Left = X;
                }
                FinishTransaction("Align Left Sides");
            }
            else
            {
                MessageBox.Show("Alignment failure: Primary Selection is empty or a link instead of a node.");
            }
        }

        //align all object's horizontal centers to the horizontal center of the primary selection
        public virtual void AlignHorizontalCenters()
        {
            GoObject obj = this.Selection.Primary;
            if (obj != null && !(obj is IGoLink))
            {
                StartTransaction();
                float X = obj.SelectionObject.Center.X;
                foreach (GoObject temp in this.Selection)
                {
                    GoObject t = temp.SelectionObject;
                    if (!(t is IGoLink))
                        t.Center = new PointF(X, t.Center.Y);
                }
                FinishTransaction("Align Horizontal Centers");
            }
            else
            {
                MessageBox.Show("Alignment failure: Primary Selection is empty or a link instead of a node.");
            }
        }

        //align all object's right sides to the right side of the primary selection
        public virtual void AlignRightSides()
        {
            GoObject obj = this.Selection.Primary;
            if (obj != null && !(obj is IGoLink))
            {
                StartTransaction();
                float X = obj.SelectionObject.Right;
                foreach (GoObject temp in this.Selection)
                {
                    GoObject t = temp.SelectionObject;
                    if (!(t is IGoLink))
                        t.Right = X;
                }
                FinishTransaction("Align Right Sides");
            }
            else
            {
                MessageBox.Show("Alignment failure: Primary Selection is empty or a link instead of a node.");
            }
        }

        //align all object's tops to the top of the primary selection
        public virtual void AlignTops()
        {
            GoObject obj = this.Selection.Primary;
            if (obj != null && !(obj is IGoLink))
            {
                StartTransaction();
                float Y = obj.SelectionObject.Top;
                foreach (GoObject temp in this.Selection)
                {
                    GoObject t = temp.SelectionObject;
                    if (!(t is IGoLink))
                        t.Top = Y;
                }
                FinishTransaction("Align Tops");
            }
            else
            {
                MessageBox.Show("Alignment failure: Primary Selection is empty or a link instead of a node.");
            }
        }

        //align all object's vertical centers to the vertical center of the primary selection
        public virtual void AlignVerticalCenters()
        {
            GoObject obj = this.Selection.Primary;
            if (obj != null && !(obj is IGoLink))
            {
                StartTransaction();
                float Y = obj.SelectionObject.Center.Y;
                foreach (GoObject temp in this.Selection)
                {
                    GoObject t = temp.SelectionObject;
                    if (!(t is IGoLink))
                        t.Center = new PointF(t.Center.X, Y);
                }
                FinishTransaction("Align Vertical Centers");
            }
            else
            {
                MessageBox.Show("Alignment failure: Primary Selection is empty or a link instead of a node.");
            }
        }

        //align all object's bottoms to the bottom of the primary selection
        public virtual void AlignBottoms()
        {
            GoObject obj = this.Selection.Primary;
            if (obj != null && !(obj is IGoLink))
            {
                StartTransaction();
                float Y = obj.SelectionObject.Bottom;
                foreach (GoObject temp in this.Selection)
                {
                    GoObject t = temp.SelectionObject;
                    if (!(t is IGoLink))
                        t.Bottom = Y;
                }
                FinishTransaction("Align Bottoms");
            }
            else
            {
                MessageBox.Show("Alignment failure: Primary Selection is empty or a link instead of a node.");
            }
        }

        // this makes the widths of all objects equal to the width of the main selection.
        public virtual void MakeWidthsSame()
        {
            GoObject obj = this.Selection.Primary;
            if (obj != null && !(obj is IGoLink))
            {
                StartTransaction();
                float W = obj.SelectionObject.Width;
                foreach (GoObject temp in this.Selection)
                {
                    GoObject t = temp.SelectionObject;
                    if (!(t is IGoLink))
                        t.Width = W;
                }
                FinishTransaction("Same Widths");
            }
            else
            {
                MessageBox.Show("Sizing failure: Primary Selection is empty or a link instead of a node.");
            }
        }

        // this makes the heights of all objects equal to the height of the main selection.
        public virtual void MakeHeightsSame()
        {
            GoObject obj = this.Selection.Primary;
            if (obj != null && !(obj is IGoLink))
            {
                StartTransaction();
                float H = obj.SelectionObject.Height;
                foreach (GoObject temp in this.Selection)
                {
                    GoObject t = temp.SelectionObject;
                    if (!(t is IGoLink))
                        t.Height = H;
                }
                FinishTransaction("Same Heights");
            }
            else
            {
                MessageBox.Show("Sizing failure: Primary Selection is empty or a link instead of a node.");
            }
        }

        // this makes the heights and widths of all objects equal to the height and
        //width of the main selection.
        public virtual void MakeSizesSame()
        {
            GoObject obj = this.Selection.Primary;
            if (obj != null && !(obj is IGoLink))
            {
                StartTransaction();
                SizeF S = obj.SelectionObject.Size;
                foreach (GoObject temp in this.Selection)
                {
                    GoObject t = temp.SelectionObject;
                    if (!(t is IGoLink))
                        t.Size = S;
                }
                FinishTransaction("Same Sizes");
            }
            else
            {
                MessageBox.Show("Sizing failure: Primary Selection is empty or a link instead of a node.");
            }
        }


        public virtual void ZoomIn()
        {
            myOriginalScale = true;
            float newscale = (float)(Math.Round(this.DocScale / 0.9f * 100) / 100);
            this.DocScale = newscale;
        }

        public virtual void ZoomOut()
        {
            myOriginalScale = true;
            float newscale = (float)(Math.Round(this.DocScale * 0.9f * 100) / 100);
            this.DocScale = newscale;
        }

        public virtual void ZoomNormal()
        {
            myOriginalScale = true;
            this.DocScale = 1;
        }

        public virtual void ZoomToFit()
        {
            if (myOriginalScale)
            {
                myOriginalDocPosition = this.DocPosition;
                myOriginalDocScale = this.DocScale;
                RescaleToFit();
            }
            else
            {
                this.DocPosition = myOriginalDocPosition;
                this.DocScale = myOriginalDocScale;
            }
            myOriginalScale = !myOriginalScale;
        }

        public static bool IsLinked(GraphNode a, GraphNode b)
        {
            if (a.BottomPort == null)
                return false;
            return a.BottomPort.IsLinked(b.TopPort) ||
                   a.BottomPort.IsLinked(b.LeftPort) ||
                   a.BottomPort.IsLinked(b.RightPort) ||
                   a.BottomPort.IsLinked(b.BottomPort);
        }

        public void CreateRelationshipsAmongSelection()
        {
            GraphNode boss = this.Selection.Primary as GraphNode;
            if (boss != null)
            {
                if (boss.BottomPort == null)
                {
                    MessageBox.Show(this, "Cannot create relationship originating from this node.", "Error", MessageBoxButtons.OK);
                }
                else
                {
                    StartTransaction();
                    foreach (GoObject obj in this.Selection)
                    {
                        GraphNode n = obj as GraphNode;
                        if (n != null && n != boss && !IsLinked(boss, n))
                        {
                            if (n.TopPort == null)
                            {
                                MessageBox.Show(this, "Cannot create relationship concluding with this node.", "Error", MessageBoxButtons.OK);
                            }
                            else
                            {
                                IGoLink l = CreateLink(boss.BottomPort, n.TopPort);
                                if (l != null)
                                {
                                    this.Document.Add(l.GoObject);
                                    RaiseLinkCreated(l.GoObject);
                                }
                            }
                        }
                    }
                    FinishTransaction("Created relationships among selection");
                }
            }
        }

        public void StartDrawingRelationship()
        {
            //this.Tool = new RelationshipTool(this);
        }

        #region ClipboardExtension

        // what follows here is a clipboard extension that will result in an Enhanced Metafile being 
        // added to the clipboard such that PowerPoint can paste scalable vector-based graphics.
        // the low level nature of this seems to be necessary because of issues in the .NET clipboard.

        // note that shawdows and gradient fills cause some visual problems in the pasted flowcahrt.

        [DllImport("user32.dll")]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
        [DllImport("user32.dll")]
        static extern bool CloseClipboard();
        [DllImport("gdi32.dll")]
        static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
        [DllImport("gdi32.dll")]
        static extern bool DeleteEnhMetaFile(IntPtr hemf);

        // Metafile mf is set to a state that is not valid inside this function.
        static public bool PutEnhMetafileOnClipboard(IntPtr hWnd, Metafile mf)
        {
            bool bResult = false;
            IntPtr hEMF, hEMF2;
            hEMF = mf.GetHenhmetafile(); // invalidates mf
            if (!hEMF.Equals(new IntPtr(0)))
            {
                hEMF2 = CopyEnhMetaFile(hEMF, new IntPtr(0));
                if (!hEMF2.Equals(new IntPtr(0)))
                {
                    if (OpenClipboard(hWnd))
                    {
                        IntPtr hRes = SetClipboardData(14 /*CF_ENHMETAFILE*/, hEMF2);
                        bResult = hRes.Equals(hEMF2);
                        CloseClipboard();
                    }
                }
                DeleteEnhMetaFile(hEMF);
            }
            return bResult;
        }

        public override void CopyToClipboard(IGoCollection coll)
        {
            base.CopyToClipboard(coll);

            RectangleF bounds = GoDocument.ComputeBounds(coll, this);
            Metafile mf = GetMetafileFromCollection(coll, bounds, this.DocScale, false);
            PutEnhMetafileOnClipboard(this.Handle, mf);

        }

        public virtual Metafile GetMetafileFromCollection(IGoCollection coll, RectangleF bounds, float scale, bool paper)
        {
            lock (typeof(GoView))
            {
                Graphics gbm = CreateGraphics();
                IntPtr bufdc = gbm.GetHdc();
                MemoryStream str = new MemoryStream();
                Metafile mf = new Metafile(str, bufdc, bounds, MetafileFrameUnit.Pixel, EmfType.EmfPlusDual);

                Graphics gmf = Graphics.FromImage(mf);
                gmf.PageUnit = GraphicsUnit.Pixel;
                gmf.SmoothingMode = this.SmoothingMode;
                gmf.TextRenderingHint = this.TextRenderingHint;
                gmf.InterpolationMode = this.InterpolationMode;
                gmf.CompositingQuality = this.CompositingQuality;
                gmf.PixelOffsetMode = this.PixelOffsetMode;

                if (paper)
                {
                    RectangleF b = bounds;
                    b.Inflate(1, 1);
                    PaintPaperColor(gmf, b);
                }

                foreach (GoObject obj in coll)
                {
                    if (!obj.CanView())
                        continue;
                    obj.Paint(gmf, this);
                }

                gmf.Dispose();

                gbm.ReleaseHdc(bufdc);
                gbm.Dispose();
                mf.Dispose();

                byte[] data = str.GetBuffer();
                return new Metafile(new MemoryStream(data, false));
            }
        }

        #endregion // ClipboardExtension

        private bool myOriginalScale = true;
        private PointF myOriginalDocPosition = new PointF();
        private float myOriginalDocScale = 1.0f;
    }

}