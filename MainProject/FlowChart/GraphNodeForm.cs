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
using System.Windows.Forms;
using Northwoods.Go;
using WeifenLuo.WinFormsUI.Docking;
using TKS.FlowChart.Tool;

namespace FlowCharter
{
    /// <summary>
    /// Properties grid for editing GraphNode information.
    /// </summary>
    public class GraphNodeForm : DockContent, IButtonControl
    {
        #region singleton
        private static volatile GraphNodeForm _instance = null;
        private static readonly object _locker = new object();
        public static GraphNodeForm GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new GraphNodeForm();
                    }
                }
            }
            return _instance;
        }
        #endregion

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private GraphNodeForm()
        {
            InitializeComponent();
            this.CancelButton = this;
            myDocChangedHandler = new GoChangedEventHandler(this.DocumentChanged);
            myGotSelectionHandler = new GoSelectionEventHandler(this.NodeGotSelection);
            myLostSelectionHandler = new GoSelectionEventHandler(this.NodeLostSelection);
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

        private void InitializeComponent()
        {
            this.myNodeCombo = new System.Windows.Forms.ComboBox();
            this.myGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // myNodeCombo
            // 
            this.myNodeCombo.Dock = System.Windows.Forms.DockStyle.Top;
            this.myNodeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.myNodeCombo.DropDownWidth = 296;
            this.myNodeCombo.Location = new System.Drawing.Point(0, 0);
            this.myNodeCombo.Name = "myNodeCombo";
            this.myNodeCombo.Size = new System.Drawing.Size(1069, 32);
            this.myNodeCombo.TabIndex = 1;
            this.myNodeCombo.Visible = false;
            this.myNodeCombo.SelectedIndexChanged += new System.EventHandler(this.myNodeCombo_SelectedIndexChanged);
            // 
            // myGrid
            // 
            this.myGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myGrid.Location = new System.Drawing.Point(0, 32);
            this.myGrid.Name = "myGrid";
            this.myGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.myGrid.Size = new System.Drawing.Size(1069, 912);
            this.myGrid.TabIndex = 2;
            // 
            // GraphNodeForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(13, 28);
            this.ClientSize = new System.Drawing.Size(1069, 944);
            this.Controls.Add(this.myGrid);
            this.Controls.Add(this.myNodeCombo);
            this.Name = "GraphNodeForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Node Properties";
            this.ResumeLayout(false);

        }

        public PropertyGrid Grid
        {
            get { return myGrid; }
        }

        public ComboBox NodeCombo
        {
            get { return myNodeCombo; }
        }

        public GoView View
        {
            get { return myView; }
            set
            {
                GoView old = myView;
                if (true)//(old != value)
                {
                    if (old != null)
                    {
                        old.Document.Changed -= myDocChangedHandler;
                        old.ObjectGotSelection -= myGotSelectionHandler;
                        old.ObjectLostSelection -= myLostSelectionHandler;
                        this.NodeCombo.SelectedIndex = -1;
                        this.NodeCombo.Items.Clear();
                    }
                    myView = value;
                    if (myView == null)
                    {
                        this.NodeInfo = null;
                        return;
                    }
                    this.NodeCombo.BeginUpdate();
                    ComboBox.ObjectCollection items = this.NodeCombo.Items;
                    foreach (GoObject obj in myView.Document)
                    {
                        IGoNode n = obj as IGoNode;
                        if (n != null && n.UserObject != null)
                        {
                            items.Add(n.UserObject);
                        }
                    }
                    this.NodeCombo.EndUpdate();
                    myView.Document.Changed += myDocChangedHandler;
                    myView.ObjectGotSelection += myGotSelectionHandler;
                    myView.ObjectLostSelection += myLostSelectionHandler;
                    IGoNode nn = (myView.Selection != null ? myView.Selection.Primary as IGoNode : null);
                    if (nn != null && nn.UserObject != null)
                        this.NodeInfo = (nn.UserObject as ToolBase).Settings;
                    else
                        this.NodeInfo = null;
                }
            }
        }

        protected void DocumentChanged(Object sender, GoChangedEventArgs evt)
        {
            switch (evt.Hint)
            {
                case GoLayer.InsertedObject:
                    {
                        // added a node to the document--gotta add it to the combobox's list of nodes
                        IGoNode n = evt.Object as IGoNode;
                        if (n != null && n.UserObject != null)
                        {
                            this.NodeCombo.Items.Add(n.UserObject);//((n.UserObject as ToolBase).ID);
                        }
                        break;
                    }
                case GoLayer.RemovedObject:
                    {
                        // removed a node from the document--gotta remove from the combobox's list of nodes
                        IGoNode n = evt.GoObject as IGoNode;
                        if (n != null && this.Grid.SelectedObject == n.UserObject)
                        {
                            this.Grid.SelectedObject = null;
                            int i = this.NodeCombo.Items.IndexOf(n.UserObject);
                            if (i >= 0)
                            {
                                this.NodeCombo.Items.RemoveAt(i);
                            }
                        }
                        break;
                    }
                case GoLayer.ChangedObject:
                    {
                        switch (evt.SubHint)
                        {
                            case GoObject.ChangedBounds:
                                {
                                    // because we're displaying the GraphNode's size and position,
                                    // we need to update the grid when the bounds change
                                    IGoNode n = evt.GoObject as IGoNode;
                                    if (n != null && n.UserObject != null &&
                                      this.Grid.SelectedObject == n.UserObject)
                                    {
                                        RedisplayInfo();
                                    }
                                    break;
                                }
                            case GoText.ChangedText:
                                {
                                    // need to update combobox's list of node names
                                    IGoNode n = evt.GoObject.ParentNode as IGoNode;
                                    if (n != null && n.UserObject != null)
                                    {
                                        int i = this.NodeCombo.Items.IndexOf(n.UserObject);
                                        if (i >= 0)
                                        {
                                            this.NodeCombo.Items[i] = n.UserObject;  // reset to update displayed string
                                        }
                                        RedisplayInfo();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case GoDocument.ChangedName:
                    {
                        // because we're displaying the document name in the grid,
                        // we need to keep it up-to-date too
                        RedisplayInfo();
                        break;
                    }
            }
        }

        public void RedisplayInfo()
        {
            this.Grid.SelectedObject = this.Grid.SelectedObject;  // reset to update whole node info
            this.Grid.Invalidate();
        }


        protected void NodeGotSelection(Object sender, GoSelectionEventArgs evt)
        {
            if (evt.GoObject == this.View.Selection.Primary)
            {
                IGoNode n = evt.GoObject as IGoNode;
                if (n != null && n.UserObject != null)
                    this.NodeInfo = (n.UserObject as ToolBase).Settings;
                else
                    this.NodeInfo = null;
            }
        }

        protected void NodeLostSelection(Object sender, GoSelectionEventArgs evt)
        {
            if (this.View.Selection.Primary == null)
            {
                this.NodeInfo = null;
            }
        }

        public XProps NodeInfo
        {
            get { return this.Grid.SelectedObject as XProps; }
            set
            {
                if (mySettingNodeInfo) return;
                mySettingNodeInfo = true;
                // always reset, even when NodeInfo identity hasn't changed
                if (value != null)
                {
                    this.Grid.SelectedObject = value;
                    int i = this.NodeCombo.Items.IndexOf(value);
                    this.NodeCombo.SelectedIndex = i;
                    this.Grid.Enabled = myView.Document.AllowEdit;
                }
                else
                {
                    this.Grid.SelectedObject = null;
                    this.NodeCombo.SelectedIndex = -1;
                }
                mySettingNodeInfo = false;
            }
        }

        protected override void OnActivated(EventArgs evt)
        {
            if (this.View != null)
                this.View.StartTransaction();
            base.OnActivated(evt);
        }

        protected override void OnDeactivate(EventArgs evt)
        {
            base.OnDeactivate(evt);
            if (this.View != null)
                this.View.FinishTransaction("Node Property Grid Changes");
        }

        protected override void OnClosed(EventArgs evt)
        {
            base.OnClosed(evt);
            if (this.View != null)
            {
                this.View.FinishTransaction("Node Property Grid Changes");
            }
            this.View = null;
        }

        protected void myNodeCombo_SelectedIndexChanged(Object sender, EventArgs evt)
        {
            if (mySettingNodeInfo) return;
            if (this.NodeCombo.SelectedIndex >= 0 &&
                this.NodeCombo.SelectedIndex < this.NodeCombo.Items.Count)
            {
                ToolBase n = this.NodeCombo.Items[this.NodeCombo.SelectedIndex] as ToolBase;
                this.Grid.SelectedObject = n;//this.NodeCombo.Items[this.NodeCombo.SelectedIndex];//
                if (n != null && n.graphNode != this.View.Selection.Primary)
                {
                    this.View.Selection.Select(n.graphNode);
                    this.View.ScrollRectangleToVisible(n.graphNode.Bounds);
                }
            }
            else if (this.NodeCombo.SelectedIndex < 0)
            {
                this.Grid.SelectedObject = null;
                this.NodeCombo.Text = "";
            }
        }

        // IButtonControl, to handle Escape key by switching focus to the node's view
        public void NotifyDefault(bool v) { }

        public void PerformClick()
        {
            if (this.View != null)
            {
                this.View.RequestFocus();
            }
        }

        public void RefreshUI()
        {
            if (myView == null)
            {
                this.NodeInfo = null;
                return;
            }
            IGoNode nn = (myView.Selection != null ? myView.Selection.Primary as IGoNode : null);
            if (nn != null && nn.UserObject != null)
            {
                this.NodeInfo = (nn.UserObject as ToolBase).Settings;
            }
            else
                this.NodeInfo = null;
        }

        private GoView myView = null;
        private GoChangedEventHandler myDocChangedHandler = null;
        private GoSelectionEventHandler myGotSelectionHandler = null;
        private GoSelectionEventHandler myLostSelectionHandler = null;
        private bool mySettingNodeInfo = false;
        private PropertyGrid myGrid;
        private System.Windows.Forms.ComboBox myNodeCombo;
    }
}
