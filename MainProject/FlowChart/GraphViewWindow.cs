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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Northwoods.Go;
using WeifenLuo.WinFormsUI.Docking;
using System.Collections.Generic;

namespace FlowCharter
{
    /// <summary>
    ///    Summary description for GraphViewWindow.
    /// </summary>
    public class GraphViewWindow : DockContent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public GraphViewWindow()
        {
            InitializeComponent();
            this.HideOnClose = true;//only hide instead close
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.myView = new FlowCharter.GraphView();
            this.SuspendLayout();
            // 
            // myView
            // 
            this.myView.ArrowMoveLarge = 10F;
            this.myView.ArrowMoveSmall = 1F;
            this.myView.BackColor = System.Drawing.Color.White;
            this.myView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myView.DragsRealtime = true;
            this.myView.GridCellSizeHeight = 10F;
            this.myView.GridCellSizeWidth = 5F;
            this.myView.GridSnapDrag = Northwoods.Go.GoViewSnapStyle.Jump;
            this.myView.Location = new System.Drawing.Point(0, 0);
            this.myView.Name = "myView";
            this.myView.PortGravity = 30F;
            this.myView.Size = new System.Drawing.Size(606, 452);
            this.myView.TabIndex = 0;
            this.myView.Text = "Graph View";
            // 
            // GraphViewWindow
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(13, 28);
            this.ClientSize = new System.Drawing.Size(606, 452);
            this.Controls.Add(this.myView);
            this.Name = "GraphViewWindow";
            this.Text = "Graph View Child Window";
            this.ResumeLayout(false);

        }

        public GraphView View
        {
            get { return myView; }
        }

        public GraphDoc Doc
        {
            get { return myView.Doc; }
        }

        protected override void OnLeave(EventArgs evt)
        {
            base.OnLeave(evt);
            this.View.DoEndEdit();
        }


        protected override void OnClosing(CancelEventArgs evt)
        {
            base.OnClosing(evt);
            //if (this.Doc.IsModified)
            //{
            //    IList windows = FindWindows(this.MdiParent, this.Doc);
            //    if (windows.Count <= 1)
            //    {  // only one left, better ask if we need to save
            //        String msg = "Save modified graph?\r\n" + this.Doc.Name;
            //        if (this.Doc.Location != "")
            //            msg += "\r\n(" + this.Doc.Location + ") ";
            //        DialogResult res = MessageBox.Show(this.MdiParent,
            //                                           msg,
            //                                           "Closing Modified Graph",
            //                                           MessageBoxButtons.YesNoCancel);
            //        if (res == DialogResult.Cancel)
            //        {
            //            evt.Cancel = true;
            //        }
            //        else if (res == DialogResult.Yes)
            //        {
            //            if (!Save())
            //                evt.Cancel = true;
            //        }
            //    }
            //}
        }

        protected override void OnClosed(EventArgs evt)
        {
            base.OnClosed(evt);
            IList windows = FindWindows(this.MdiParent, this.Doc);
            if (windows.Count <= 1)
                GraphDoc.RemoveDocument(this.Doc.Location);
        }

        public virtual bool Save()
        {
            String loc = this.Doc.Location;
            int lastslash = loc.LastIndexOf("\\");
            if (loc != "" && !this.Doc.IsReadOnly && lastslash >= 0 && loc.Substring(lastslash + 1) == Doc.Name)
            {
                FileStream file = null;
                try
                {
                    file = File.Open(loc, FileMode.Create);
                    this.Doc.Store(file, loc);
                    this.Doc.IsModified = false;
                    this.View.UpdateFormInfo();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error saving graph as a file");
                    return false;
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
                return true;
            }
            else
            {
                return SaveAs();
            }
        }

        public virtual bool SaveAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (Doc.Name != "")
                dlg.FileName = Doc.Name;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                String loc = dlg.FileName;
                FileStream file = null;
                try
                {
                    file = File.Open(loc, FileMode.Create);
                    this.Doc.Store(file, loc);
                    this.Doc.IsModified = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error saving graph as a file");
                    return false;
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
                return true;
            }
            return false;
        }

        public virtual bool Reload()
        {
            String loc = this.Doc.Location;
            if (loc != "")
            {
                FileStream file = File.Open(loc, FileMode.Open);
                GraphDoc olddoc = this.View.Doc;
                GraphDoc newdoc = null;
                try
                {
                    newdoc = GraphDoc.Load(file, loc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error reading graph from a file");
                    return false;
                }
                finally
                {
                    file.Close();
                }
                if (newdoc != null)
                {
                    IList windows = GraphViewWindow.FindWindows(this.MdiParent, olddoc);
                    foreach (Object obj in windows)
                    {
                        GraphViewWindow w = obj as GraphViewWindow;
                        if (w != null)
                        {
                            w.View.Document = newdoc;
                        }
                    }
                }
            }
            return true;
        }


        public static GraphViewWindow Open(DockPanel dockPanel1)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                String loc = dlg.FileName;
                GraphDoc olddoc = GraphDoc.FindDocument(loc);
                if (olddoc != null)
                {
                    return null;
                    //IList windows = GraphViewWindow.FindWindows(mdiparent, olddoc);
                    //if (windows.Count > 0)
                    //{
                    //    GraphViewWindow w = windows[0] as GraphViewWindow;
                    //    if (w.Reload())
                    //    {
                    //        w.Show();
                    //       w.Activate();
                    //    }
                    //    return w;
                    //}
                }
                else
                {
                    Stream file = dlg.OpenFile();
                    if (file != null)
                    {
                        GraphDoc doc = null;
                        try
                        {
                            doc = GraphDoc.Load(file, loc);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show( ex.Message, "Error reading graph from a file");
                        }
                        finally
                        {
                            file.Close();
                        }
                        if (doc != null)
                        {
                            GraphViewWindow w = new GraphViewWindow();
                            w.View.Document = doc;
                            w.Show(dockPanel1, DockState.Document);
                            w.Activate();
                            return w;
                        }
                    }
                    return null;
                }
            }
            return null;
        }

        public static List<GraphViewWindow> OpenAll()
        {
            List<GraphViewWindow> windows = new List<GraphViewWindow>();
            if (!System.IO.Directory.Exists(@".\Config\FlowChart\"))
            {
                System.IO.Directory.CreateDirectory(@".\Config\FlowChart\");
            }
            var filePaths = Directory.GetFiles(@".\Config\FlowChart\");
            foreach (var item in filePaths)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = item;
                String loc = item;
                GraphDoc olddoc = GraphDoc.FindDocument(loc);
                if (olddoc != null)
                {
                    //IList windows = GraphViewWindow.FindWindows(mdiparent, olddoc);
                    //if (windows.Count > 0)
                    //{
                    //    GraphViewWindow w = windows[0] as GraphViewWindow;
                    //    if (w.Reload())
                    //    {
                    //        w.Show();
                    //       w.Activate();
                    //    }
                    //    return w;
                    //}
                }
                else
                {
                    Stream file = dlg.OpenFile();
                    if (file != null)
                    {
                        GraphDoc doc = null;
                        try
                        {
                            doc = GraphDoc.Load(file, loc);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error reading graph from a file");
                        }
                        finally
                        {
                            file.Close();
                        }
                        if (doc != null)
                        {
                            GraphViewWindow w = new GraphViewWindow();
                            w.View.Document = doc;
                            w.Activate();
                            windows.Add(w);
                        }
                    }
                }

            }
            return windows;
        }

        public static IList FindWindows(Form mdiparent, GraphDoc doc)
        {
            ArrayList windows = new ArrayList();
            Form[] children = mdiparent.MdiChildren;
            foreach (Form f in children)
            {
                GraphViewWindow w = f as GraphViewWindow;
                if (w != null && w.Doc == doc)
                {
                    windows.Add(w);
                }
            }
            return windows;
        }


        private GraphView myView;
    }
}
