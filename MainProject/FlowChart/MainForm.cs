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
using System.Resources;
using System.Windows.Forms;
using Northwoods.Go;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;

namespace FlowCharter
{
    /// <summary>
    ///    Summary description for MainForm.
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private PaletteForm mypaletteForm = null;

        #region MyRegion
        private System.Windows.Forms.ToolStripMenuItem fileNewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileCloseMenuItem;
        private System.Windows.Forms.ToolStripSeparator fileMenuSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileSaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileSaveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileSaveAllMenuItem;
        private System.Windows.Forms.ToolStripSeparator fileMenuSeparator2;
        private System.Windows.Forms.ToolStripMenuItem filePrintMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filePrintPreviewMenuItem;
        private System.Windows.Forms.ToolStripSeparator fileMenuSeparator3;
        private System.Windows.Forms.ToolStripMenuItem fileExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editUndoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editRedoMenuItem;
        private System.Windows.Forms.ToolStripSeparator editMenuSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editCutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editCopyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editPasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editDeleteMenuItem;
        private System.Windows.Forms.ToolStripSeparator editMenuSeparator2;
        private System.Windows.Forms.ToolStripMenuItem editSelectAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewZoomInMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewZoomOutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewZoomNormalMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewZoomToFitMenuItem;
        private System.Windows.Forms.ToolStripSeparator viewMenuSeparator1;
        private System.Windows.Forms.ToolStripMenuItem viewOverviewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPropertiesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertCommentMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertNodeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatAlignLeftsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatAlignHorizontalCentersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatAlignRightsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatAlignTopsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatAlignVerticalCentersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatAlignBottomsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowNewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowCascadeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowTileHorizontallyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowTileVerticallyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowCloseAllDocumentsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpAboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem insertMenu;
        private System.Windows.Forms.ToolStripMenuItem formatMenu;
        private System.Windows.Forms.ToolStripMenuItem windowMenu;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.MenuStrip mainMenuBar;
        private System.Windows.Forms.ToolStripButton fileNewToolStripButton;
        private System.Windows.Forms.ToolStripButton fileOpenToolStripButton;
        private System.Windows.Forms.ToolStripButton fileSaveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator separator1ToolStripButton;
        private System.Windows.Forms.ToolStripButton editCutToolStripButton;
        private System.Windows.Forms.ToolStripButton editCopyToolStripButton;
        private System.Windows.Forms.ToolStripButton editPasteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator separator2ToolStripButton;
        private System.Windows.Forms.ToolStripButton editUndoToolStripButton;
        private System.Windows.Forms.ToolStripButton editRedoToolStripButton;
        private System.Windows.Forms.ToolStrip toolBar;
        private System.Windows.Forms.StatusBarPanel statusMessagePanel;
        private System.Windows.Forms.StatusBarPanel statusZoomPanel;
        private System.Windows.Forms.StatusBar statusBar;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.MdiClient mdiClient1;
        private System.Windows.Forms.ToolStripMenuItem menuItem1;
        private System.Windows.Forms.ToolStripMenuItem insertRelationshipMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItem3;
        private System.Windows.Forms.ToolStripMenuItem drawRelationshipMenuItem;
        private DockPanel dockPanel1;
        private BackgroundWorker backgroundWorker1;
        private System.ComponentModel.IContainer components; 
        #endregion

        public MainForm()
        {
            myMainForm = this;

            //// let the user adjust the width of the panel containing the palette
            //Splitter splitv = new Splitter();
            //splitv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            //splitv.Dock = DockStyle.Left;
            //this.Controls.Add(splitv);

            //myPanel = new Panel();
            //myPanel.Controls.Add(myPalette);
            //myPanel.Dock = DockStyle.Left;
            //myPanel.Width = 200;
            //this.Controls.Add(myPanel);

            InitializeComponent();

            this.fileNewToolStripButton.Click += new System.EventHandler(this.fileNewMenuItem_Click);
            this.fileOpenToolStripButton.Click += new System.EventHandler(this.fileOpenMenuItem_Click);
            this.fileSaveToolStripButton.Click += new System.EventHandler(this.fileSaveMenuItem_Click);
            this.editCutToolStripButton.Click += new System.EventHandler(this.editCutMenuItem_Click);
            this.editCopyToolStripButton.Click += new System.EventHandler(this.editCopyMenuItem_Click);
            this.editPasteToolStripButton.Click += new System.EventHandler(this.editPasteMenuItem_Click);
            this.editUndoToolStripButton.Click += new System.EventHandler(this.editUndoMenuItem_Click);
            this.editRedoToolStripButton.Click += new System.EventHandler(this.editRedoMenuItem_Click);

            mypaletteForm = PaletteForm.GetInstance();
            mypaletteForm.Show(dockPanel1, DockState.DockLeft);
            // start up with an empty graph
            fileNewMenuItem_Click(this, null);
            ShowProperties();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.formatAlignVerticalCentersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatAlignTopsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewZoomInMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatAlignHorizontalCentersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileCloseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSelectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowCascadeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatAlignBottomsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileSaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSaveAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenuSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.filePrintMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filePrintPreviewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenuSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editRedoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.statusMessagePanel = new System.Windows.Forms.StatusBarPanel();
            this.helpAboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenuBar = new System.Windows.Forms.MenuStrip();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editUndoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editRedoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editCopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editPasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewZoomOutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewZoomNormalMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewZoomToFitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.viewOverviewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPropertiesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.insertCommentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertNodeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.insertRelationshipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.drawRelationshipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.formatAlignLeftsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatAlignRightsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.windowNewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowTileHorizontallyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowTileVerticallyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowCloseAllDocumentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusZoomPanel = new System.Windows.Forms.StatusBarPanel();
            this.editUndoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.editPasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.fileOpenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.fileNewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.fileSaveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.separator2ToolStripButton = new System.Windows.Forms.ToolStripSeparator();
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.imageList1 = new System.Windows.Forms.ImageList();
            this.separator1ToolStripButton = new System.Windows.Forms.ToolStripSeparator();
            this.editCutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.mdiClient1 = new System.Windows.Forms.MdiClient();
            this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.statusMessagePanel)).BeginInit();
            this.mainMenuBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusZoomPanel)).BeginInit();
            this.toolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // formatAlignVerticalCentersMenuItem
            // 
            this.formatAlignVerticalCentersMenuItem.Name = "formatAlignVerticalCentersMenuItem";
            this.formatAlignVerticalCentersMenuItem.Size = new System.Drawing.Size(409, 44);
            this.formatAlignVerticalCentersMenuItem.Text = "Align Vertical Centers";
            this.formatAlignVerticalCentersMenuItem.Click += new System.EventHandler(this.formatAlignVerticalCentersMenuItem_Click);
            // 
            // formatAlignTopsMenuItem
            // 
            this.formatAlignTopsMenuItem.Name = "formatAlignTopsMenuItem";
            this.formatAlignTopsMenuItem.Size = new System.Drawing.Size(409, 44);
            this.formatAlignTopsMenuItem.Text = "Align Tops";
            this.formatAlignTopsMenuItem.Click += new System.EventHandler(this.formatAlignTopsMenuItem_Click);
            // 
            // viewZoomInMenuItem
            // 
            this.viewZoomInMenuItem.Name = "viewZoomInMenuItem";
            this.viewZoomInMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F6)));
            this.viewZoomInMenuItem.Size = new System.Drawing.Size(456, 44);
            this.viewZoomInMenuItem.Text = "Zoom &In";
            this.viewZoomInMenuItem.Click += new System.EventHandler(this.viewZoomInMenuItem_Click);
            // 
            // formatAlignHorizontalCentersMenuItem
            // 
            this.formatAlignHorizontalCentersMenuItem.Name = "formatAlignHorizontalCentersMenuItem";
            this.formatAlignHorizontalCentersMenuItem.Size = new System.Drawing.Size(409, 44);
            this.formatAlignHorizontalCentersMenuItem.Text = "Align Horizontal Centers";
            this.formatAlignHorizontalCentersMenuItem.Click += new System.EventHandler(this.formatAlignHorizontalCentersMenuItem_Click);
            // 
            // fileCloseMenuItem
            // 
            this.fileCloseMenuItem.Name = "fileCloseMenuItem";
            this.fileCloseMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.fileCloseMenuItem.Size = new System.Drawing.Size(379, 44);
            this.fileCloseMenuItem.Text = "&Close";
            this.fileCloseMenuItem.Click += new System.EventHandler(this.fileCloseMenuItem_Click);
            // 
            // editSelectAllMenuItem
            // 
            this.editSelectAllMenuItem.Name = "editSelectAllMenuItem";
            this.editSelectAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.editSelectAllMenuItem.Size = new System.Drawing.Size(330, 44);
            this.editSelectAllMenuItem.Text = "Select &All";
            this.editSelectAllMenuItem.Click += new System.EventHandler(this.editSelectAllMenuItem_Click);
            // 
            // windowCascadeMenuItem
            // 
            this.windowCascadeMenuItem.Name = "windowCascadeMenuItem";
            this.windowCascadeMenuItem.Size = new System.Drawing.Size(370, 44);
            this.windowCascadeMenuItem.Text = "&Cascade";
            this.windowCascadeMenuItem.Click += new System.EventHandler(this.windowCascadeMenuItem_Click);
            // 
            // formatAlignBottomsMenuItem
            // 
            this.formatAlignBottomsMenuItem.Name = "formatAlignBottomsMenuItem";
            this.formatAlignBottomsMenuItem.Size = new System.Drawing.Size(409, 44);
            this.formatAlignBottomsMenuItem.Text = "Align Bottoms";
            this.formatAlignBottomsMenuItem.Click += new System.EventHandler(this.formatAlignBottomsMenuItem_Click);
            // 
            // fileNewMenuItem
            // 
            this.fileNewMenuItem.Name = "fileNewMenuItem";
            this.fileNewMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.fileNewMenuItem.Size = new System.Drawing.Size(379, 44);
            this.fileNewMenuItem.Text = "&New";
            this.fileNewMenuItem.Click += new System.EventHandler(this.fileNewMenuItem_Click);
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileNewMenuItem,
            this.fileOpenMenuItem,
            this.fileCloseMenuItem,
            this.fileMenuSeparator1,
            this.fileSaveMenuItem,
            this.fileSaveAsMenuItem,
            this.fileSaveAllMenuItem,
            this.fileMenuSeparator2,
            this.filePrintMenuItem,
            this.filePrintPreviewMenuItem,
            this.fileMenuSeparator3,
            this.fileExitMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(72, 36);
            this.fileMenu.Text = "&File";
            this.fileMenu.DropDownOpening += new System.EventHandler(this.fileMenu_DropDownOpening);
            // 
            // fileOpenMenuItem
            // 
            this.fileOpenMenuItem.Name = "fileOpenMenuItem";
            this.fileOpenMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.fileOpenMenuItem.Size = new System.Drawing.Size(379, 44);
            this.fileOpenMenuItem.Text = "&Open";
            this.fileOpenMenuItem.Click += new System.EventHandler(this.fileOpenMenuItem_Click);
            // 
            // fileMenuSeparator1
            // 
            this.fileMenuSeparator1.Name = "fileMenuSeparator1";
            this.fileMenuSeparator1.Size = new System.Drawing.Size(376, 6);
            // 
            // fileSaveMenuItem
            // 
            this.fileSaveMenuItem.Name = "fileSaveMenuItem";
            this.fileSaveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fileSaveMenuItem.Size = new System.Drawing.Size(379, 44);
            this.fileSaveMenuItem.Text = "&Save";
            this.fileSaveMenuItem.Click += new System.EventHandler(this.fileSaveMenuItem_Click);
            // 
            // fileSaveAsMenuItem
            // 
            this.fileSaveAsMenuItem.Name = "fileSaveAsMenuItem";
            this.fileSaveAsMenuItem.Size = new System.Drawing.Size(379, 44);
            this.fileSaveAsMenuItem.Text = "Save &As";
            this.fileSaveAsMenuItem.Click += new System.EventHandler(this.fileSaveAsMenuItem_Click);
            // 
            // fileSaveAllMenuItem
            // 
            this.fileSaveAllMenuItem.Name = "fileSaveAllMenuItem";
            this.fileSaveAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.fileSaveAllMenuItem.Size = new System.Drawing.Size(379, 44);
            this.fileSaveAllMenuItem.Text = "Save A&ll";
            this.fileSaveAllMenuItem.Click += new System.EventHandler(this.fileSaveAllMenuItem_Click);
            // 
            // fileMenuSeparator2
            // 
            this.fileMenuSeparator2.Name = "fileMenuSeparator2";
            this.fileMenuSeparator2.Size = new System.Drawing.Size(376, 6);
            // 
            // filePrintMenuItem
            // 
            this.filePrintMenuItem.Name = "filePrintMenuItem";
            this.filePrintMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.filePrintMenuItem.Size = new System.Drawing.Size(379, 44);
            this.filePrintMenuItem.Text = "&Print";
            this.filePrintMenuItem.Click += new System.EventHandler(this.filePrintMenuItem_Click);
            // 
            // filePrintPreviewMenuItem
            // 
            this.filePrintPreviewMenuItem.Name = "filePrintPreviewMenuItem";
            this.filePrintPreviewMenuItem.Size = new System.Drawing.Size(379, 44);
            this.filePrintPreviewMenuItem.Text = "Print Preview";
            this.filePrintPreviewMenuItem.Click += new System.EventHandler(this.filePrintPreviewMenuItem_Click);
            // 
            // fileMenuSeparator3
            // 
            this.fileMenuSeparator3.Name = "fileMenuSeparator3";
            this.fileMenuSeparator3.Size = new System.Drawing.Size(376, 6);
            // 
            // fileExitMenuItem
            // 
            this.fileExitMenuItem.Name = "fileExitMenuItem";
            this.fileExitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.fileExitMenuItem.Size = new System.Drawing.Size(379, 44);
            this.fileExitMenuItem.Text = "E&xit";
            this.fileExitMenuItem.Click += new System.EventHandler(this.fileExitMenuItem_Click);
            // 
            // editRedoToolStripButton
            // 
            this.editRedoToolStripButton.ImageIndex = 7;
            this.editRedoToolStripButton.Name = "editRedoToolStripButton";
            this.editRedoToolStripButton.Size = new System.Drawing.Size(46, 36);
            this.editRedoToolStripButton.ToolTipText = "Redo last edit";
            // 
            // statusMessagePanel
            // 
            this.statusMessagePanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusMessagePanel.Name = "statusMessagePanel";
            this.statusMessagePanel.Width = 999;
            // 
            // helpAboutMenuItem
            // 
            this.helpAboutMenuItem.Name = "helpAboutMenuItem";
            this.helpAboutMenuItem.Size = new System.Drawing.Size(214, 44);
            this.helpAboutMenuItem.Text = "&About";
            this.helpAboutMenuItem.Click += new System.EventHandler(this.helpAboutMenuItem_Click);
            // 
            // editCutMenuItem
            // 
            this.editCutMenuItem.Name = "editCutMenuItem";
            this.editCutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.editCutMenuItem.Size = new System.Drawing.Size(330, 44);
            this.editCutMenuItem.Text = "Cu&t";
            this.editCutMenuItem.Click += new System.EventHandler(this.editCutMenuItem_Click);
            // 
            // mainMenuBar
            // 
            this.mainMenuBar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.mainMenuBar.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.mainMenuBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mainMenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.editMenu,
            this.viewMenu,
            this.insertMenu,
            this.formatMenu,
            this.windowMenu,
            this.helpMenu});
            this.mainMenuBar.Location = new System.Drawing.Point(0, 42);
            this.mainMenuBar.Name = "mainMenuBar";
            this.mainMenuBar.Size = new System.Drawing.Size(1100, 40);
            this.mainMenuBar.TabIndex = 0;
            // 
            // editMenu
            // 
            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editUndoMenuItem,
            this.editRedoMenuItem,
            this.editMenuSeparator1,
            this.editCutMenuItem,
            this.editCopyMenuItem,
            this.editPasteMenuItem,
            this.editDeleteMenuItem,
            this.editMenuSeparator2,
            this.editSelectAllMenuItem});
            this.editMenu.Name = "editMenu";
            this.editMenu.Size = new System.Drawing.Size(75, 36);
            this.editMenu.Text = "&Edit";
            this.editMenu.DropDownOpening += new System.EventHandler(this.editMenu_DropDownOpening);
            // 
            // editUndoMenuItem
            // 
            this.editUndoMenuItem.Name = "editUndoMenuItem";
            this.editUndoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.editUndoMenuItem.Size = new System.Drawing.Size(330, 44);
            this.editUndoMenuItem.Text = "&Undo";
            this.editUndoMenuItem.Click += new System.EventHandler(this.editUndoMenuItem_Click);
            // 
            // editRedoMenuItem
            // 
            this.editRedoMenuItem.Name = "editRedoMenuItem";
            this.editRedoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.editRedoMenuItem.Size = new System.Drawing.Size(330, 44);
            this.editRedoMenuItem.Text = "&Redo";
            this.editRedoMenuItem.Click += new System.EventHandler(this.editRedoMenuItem_Click);
            // 
            // editMenuSeparator1
            // 
            this.editMenuSeparator1.Name = "editMenuSeparator1";
            this.editMenuSeparator1.Size = new System.Drawing.Size(327, 6);
            // 
            // editCopyMenuItem
            // 
            this.editCopyMenuItem.Name = "editCopyMenuItem";
            this.editCopyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.editCopyMenuItem.Size = new System.Drawing.Size(330, 44);
            this.editCopyMenuItem.Text = "&Copy";
            this.editCopyMenuItem.Click += new System.EventHandler(this.editCopyMenuItem_Click);
            // 
            // editPasteMenuItem
            // 
            this.editPasteMenuItem.Name = "editPasteMenuItem";
            this.editPasteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.editPasteMenuItem.Size = new System.Drawing.Size(330, 44);
            this.editPasteMenuItem.Text = "&Paste";
            this.editPasteMenuItem.Click += new System.EventHandler(this.editPasteMenuItem_Click);
            // 
            // editDeleteMenuItem
            // 
            this.editDeleteMenuItem.Name = "editDeleteMenuItem";
            this.editDeleteMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.editDeleteMenuItem.Size = new System.Drawing.Size(330, 44);
            this.editDeleteMenuItem.Text = "&Delete";
            this.editDeleteMenuItem.Click += new System.EventHandler(this.editDeleteMenuItem_Click);
            // 
            // editMenuSeparator2
            // 
            this.editMenuSeparator2.Name = "editMenuSeparator2";
            this.editMenuSeparator2.Size = new System.Drawing.Size(327, 6);
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewZoomInMenuItem,
            this.viewZoomOutMenuItem,
            this.viewZoomNormalMenuItem,
            this.viewZoomToFitMenuItem,
            this.viewMenuSeparator1,
            this.viewOverviewMenuItem,
            this.viewPropertiesMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(86, 36);
            this.viewMenu.Text = "&View";
            // 
            // viewZoomOutMenuItem
            // 
            this.viewZoomOutMenuItem.Name = "viewZoomOutMenuItem";
            this.viewZoomOutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F6)));
            this.viewZoomOutMenuItem.Size = new System.Drawing.Size(456, 44);
            this.viewZoomOutMenuItem.Text = "Zoom &Out";
            this.viewZoomOutMenuItem.Click += new System.EventHandler(this.viewZoomOutMenuItem_Click);
            // 
            // viewZoomNormalMenuItem
            // 
            this.viewZoomNormalMenuItem.Name = "viewZoomNormalMenuItem";
            this.viewZoomNormalMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F6)));
            this.viewZoomNormalMenuItem.Size = new System.Drawing.Size(456, 44);
            this.viewZoomNormalMenuItem.Text = "Zoom &Normal";
            this.viewZoomNormalMenuItem.Click += new System.EventHandler(this.viewZoomNormalMenuItem_Click);
            // 
            // viewZoomToFitMenuItem
            // 
            this.viewZoomToFitMenuItem.Name = "viewZoomToFitMenuItem";
            this.viewZoomToFitMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.viewZoomToFitMenuItem.Size = new System.Drawing.Size(456, 44);
            this.viewZoomToFitMenuItem.Text = "Zoom To &Fit";
            this.viewZoomToFitMenuItem.Click += new System.EventHandler(this.viewZoomToFitMenuItem_Click);
            // 
            // viewMenuSeparator1
            // 
            this.viewMenuSeparator1.Name = "viewMenuSeparator1";
            this.viewMenuSeparator1.Size = new System.Drawing.Size(453, 6);
            // 
            // viewOverviewMenuItem
            // 
            this.viewOverviewMenuItem.Name = "viewOverviewMenuItem";
            this.viewOverviewMenuItem.Size = new System.Drawing.Size(456, 44);
            this.viewOverviewMenuItem.Text = "Over&view";
            this.viewOverviewMenuItem.Click += new System.EventHandler(this.viewOverviewMenuItem_Click);
            // 
            // viewPropertiesMenuItem
            // 
            this.viewPropertiesMenuItem.Name = "viewPropertiesMenuItem";
            this.viewPropertiesMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.viewPropertiesMenuItem.Size = new System.Drawing.Size(456, 44);
            this.viewPropertiesMenuItem.Text = "&Properties";
            this.viewPropertiesMenuItem.Click += new System.EventHandler(this.viewPropertiesMenuItem_Click);
            // 
            // insertMenu
            // 
            this.insertMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertCommentMenuItem,
            this.insertNodeMenuItem,
            this.menuItem1,
            this.insertRelationshipMenuItem,
            this.menuItem3,
            this.drawRelationshipMenuItem});
            this.insertMenu.Name = "insertMenu";
            this.insertMenu.Size = new System.Drawing.Size(94, 36);
            this.insertMenu.Text = "&Insert";
            this.insertMenu.DropDownOpening += new System.EventHandler(this.insertMenu_DropDownOpening);
            // 
            // insertCommentMenuItem
            // 
            this.insertCommentMenuItem.Name = "insertCommentMenuItem";
            this.insertCommentMenuItem.Size = new System.Drawing.Size(469, 44);
            this.insertCommentMenuItem.Text = "Comment";
            this.insertCommentMenuItem.Click += new System.EventHandler(this.insertCommentMenuItem_Click);
            // 
            // insertNodeMenuItem
            // 
            this.insertNodeMenuItem.Name = "insertNodeMenuItem";
            this.insertNodeMenuItem.Size = new System.Drawing.Size(469, 44);
            this.insertNodeMenuItem.Text = "Node";
            this.insertNodeMenuItem.Click += new System.EventHandler(this.insertNodeMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Name = "menuItem1";
            this.menuItem1.Size = new System.Drawing.Size(469, 44);
            this.menuItem1.Text = "-";
            // 
            // insertRelationshipMenuItem
            // 
            this.insertRelationshipMenuItem.Name = "insertRelationshipMenuItem";
            this.insertRelationshipMenuItem.Size = new System.Drawing.Size(469, 44);
            this.insertRelationshipMenuItem.Text = "Relationship Among Selection";
            this.insertRelationshipMenuItem.Click += new System.EventHandler(this.insertRelationshipMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Name = "menuItem3";
            this.menuItem3.Size = new System.Drawing.Size(469, 44);
            this.menuItem3.Text = "-";
            // 
            // drawRelationshipMenuItem
            // 
            this.drawRelationshipMenuItem.Name = "drawRelationshipMenuItem";
            this.drawRelationshipMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.drawRelationshipMenuItem.Size = new System.Drawing.Size(469, 44);
            this.drawRelationshipMenuItem.Text = "Draw Relationship";
            this.drawRelationshipMenuItem.Click += new System.EventHandler(this.drawRelationshipMenuItem_Click);
            // 
            // formatMenu
            // 
            this.formatMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.formatAlignLeftsMenuItem,
            this.formatAlignHorizontalCentersMenuItem,
            this.formatAlignRightsMenuItem,
            this.formatAlignTopsMenuItem,
            this.formatAlignVerticalCentersMenuItem,
            this.formatAlignBottomsMenuItem});
            this.formatMenu.Name = "formatMenu";
            this.formatMenu.Size = new System.Drawing.Size(110, 36);
            this.formatMenu.Text = "F&ormat";
            this.formatMenu.DropDownOpening += new System.EventHandler(this.formatMenu_DropDownOpening);
            // 
            // formatAlignLeftsMenuItem
            // 
            this.formatAlignLeftsMenuItem.Name = "formatAlignLeftsMenuItem";
            this.formatAlignLeftsMenuItem.Size = new System.Drawing.Size(409, 44);
            this.formatAlignLeftsMenuItem.Text = "Align Left Sides";
            this.formatAlignLeftsMenuItem.Click += new System.EventHandler(this.formatAlignLeftsMenuItem_Click);
            // 
            // formatAlignRightsMenuItem
            // 
            this.formatAlignRightsMenuItem.Name = "formatAlignRightsMenuItem";
            this.formatAlignRightsMenuItem.Size = new System.Drawing.Size(409, 44);
            this.formatAlignRightsMenuItem.Text = "Align Right Sides";
            this.formatAlignRightsMenuItem.Click += new System.EventHandler(this.formatAlignRightsMenuItem_Click);
            // 
            // windowMenu
            // 
            this.windowMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowNewMenuItem,
            this.windowCascadeMenuItem,
            this.windowTileHorizontallyMenuItem,
            this.windowTileVerticallyMenuItem,
            this.windowCloseAllDocumentsMenuItem});
            this.windowMenu.Name = "windowMenu";
            this.windowMenu.Size = new System.Drawing.Size(122, 36);
            this.windowMenu.Text = "&Window";
            // 
            // windowNewMenuItem
            // 
            this.windowNewMenuItem.Name = "windowNewMenuItem";
            this.windowNewMenuItem.Size = new System.Drawing.Size(370, 44);
            this.windowNewMenuItem.Text = "&New Window";
            this.windowNewMenuItem.Click += new System.EventHandler(this.windowNewMenuItem_Click);
            // 
            // windowTileHorizontallyMenuItem
            // 
            this.windowTileHorizontallyMenuItem.Name = "windowTileHorizontallyMenuItem";
            this.windowTileHorizontallyMenuItem.Size = new System.Drawing.Size(370, 44);
            this.windowTileHorizontallyMenuItem.Text = "Tile Hori&zontally";
            this.windowTileHorizontallyMenuItem.Click += new System.EventHandler(this.windowTileHorizontallyMenuItem_Click);
            // 
            // windowTileVerticallyMenuItem
            // 
            this.windowTileVerticallyMenuItem.Name = "windowTileVerticallyMenuItem";
            this.windowTileVerticallyMenuItem.Size = new System.Drawing.Size(370, 44);
            this.windowTileVerticallyMenuItem.Text = "Tile &Vertically";
            this.windowTileVerticallyMenuItem.Click += new System.EventHandler(this.windowTileVerticallyMenuItem_Click);
            // 
            // windowCloseAllDocumentsMenuItem
            // 
            this.windowCloseAllDocumentsMenuItem.Name = "windowCloseAllDocumentsMenuItem";
            this.windowCloseAllDocumentsMenuItem.Size = new System.Drawing.Size(370, 44);
            this.windowCloseAllDocumentsMenuItem.Text = "C&lose All Documents";
            this.windowCloseAllDocumentsMenuItem.Click += new System.EventHandler(this.windowCloseAllDocumentsMenuItem_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpAboutMenuItem});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(85, 36);
            this.helpMenu.Text = "&Help";
            // 
            // editCopyToolStripButton
            // 
            this.editCopyToolStripButton.ImageIndex = 4;
            this.editCopyToolStripButton.Name = "editCopyToolStripButton";
            this.editCopyToolStripButton.Size = new System.Drawing.Size(46, 36);
            this.editCopyToolStripButton.ToolTipText = "Copy to clipboard";
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 763);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusMessagePanel,
            this.statusZoomPanel});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(1100, 42);
            this.statusBar.TabIndex = 2;
            this.statusBar.Text = "statusBar";
            // 
            // statusZoomPanel
            // 
            this.statusZoomPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusZoomPanel.Name = "statusZoomPanel";
            this.statusZoomPanel.Text = "100%";
            this.statusZoomPanel.Width = 68;
            // 
            // editUndoToolStripButton
            // 
            this.editUndoToolStripButton.ImageIndex = 6;
            this.editUndoToolStripButton.Name = "editUndoToolStripButton";
            this.editUndoToolStripButton.Size = new System.Drawing.Size(46, 36);
            this.editUndoToolStripButton.ToolTipText = "Undo last edit";
            // 
            // editPasteToolStripButton
            // 
            this.editPasteToolStripButton.ImageIndex = 5;
            this.editPasteToolStripButton.Name = "editPasteToolStripButton";
            this.editPasteToolStripButton.Size = new System.Drawing.Size(46, 36);
            this.editPasteToolStripButton.ToolTipText = "Paste from clipboard";
            // 
            // fileOpenToolStripButton
            // 
            this.fileOpenToolStripButton.ImageIndex = 1;
            this.fileOpenToolStripButton.Name = "fileOpenToolStripButton";
            this.fileOpenToolStripButton.Size = new System.Drawing.Size(46, 36);
            this.fileOpenToolStripButton.ToolTipText = "Open document";
            // 
            // fileNewToolStripButton
            // 
            this.fileNewToolStripButton.ImageIndex = 0;
            this.fileNewToolStripButton.Name = "fileNewToolStripButton";
            this.fileNewToolStripButton.Size = new System.Drawing.Size(46, 36);
            this.fileNewToolStripButton.ToolTipText = "New document";
            // 
            // fileSaveToolStripButton
            // 
            this.fileSaveToolStripButton.ImageIndex = 2;
            this.fileSaveToolStripButton.Name = "fileSaveToolStripButton";
            this.fileSaveToolStripButton.Size = new System.Drawing.Size(46, 36);
            this.fileSaveToolStripButton.ToolTipText = "Save current document";
            // 
            // separator2ToolStripButton
            // 
            this.separator2ToolStripButton.Name = "separator2ToolStripButton";
            this.separator2ToolStripButton.Size = new System.Drawing.Size(6, 6);
            // 
            // toolBar
            // 
            this.toolBar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolBar.ImageList = this.imageList1;
            this.toolBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileNewToolStripButton,
            this.fileOpenToolStripButton,
            this.fileSaveToolStripButton,
            this.separator1ToolStripButton,
            this.editCutToolStripButton,
            this.editCopyToolStripButton,
            this.editPasteToolStripButton,
            this.editUndoToolStripButton,
            this.editRedoToolStripButton});
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(1100, 42);
            this.toolBar.TabIndex = 1;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Silver;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            // 
            // separator1ToolStripButton
            // 
            this.separator1ToolStripButton.Name = "separator1ToolStripButton";
            this.separator1ToolStripButton.Size = new System.Drawing.Size(6, 42);
            // 
            // editCutToolStripButton
            // 
            this.editCutToolStripButton.ImageIndex = 3;
            this.editCutToolStripButton.Name = "editCutToolStripButton";
            this.editCutToolStripButton.Size = new System.Drawing.Size(46, 36);
            this.editCutToolStripButton.ToolTipText = "Cut selection to clipboard";
            // 
            // mdiClient1
            // 
            this.mdiClient1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mdiClient1.Location = new System.Drawing.Point(0, 82);
            this.mdiClient1.Name = "mdiClient1";
            this.mdiClient1.Size = new System.Drawing.Size(1100, 681);
            this.mdiClient1.TabIndex = 3;
            // 
            // dockPanel1
            // 
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.Location = new System.Drawing.Point(0, 82);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size(1100, 681);
            this.dockPanel1.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(13, 28);
            this.ClientSize = new System.Drawing.Size(1100, 805);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.mainMenuBar);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.mdiClient1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mainMenuBar;
            this.Name = "MainForm";
            this.Text = "FlowCharter - Northwoods Software GoDiagram";
            ((System.ComponentModel.ISupportInitialize)(this.statusMessagePanel)).EndInit();
            this.mainMenuBar.ResumeLayout(false);
            this.mainMenuBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusZoomPanel)).EndInit();
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public GraphNodeForm ShowProperties(GoView view)
        {
            GraphNodeForm myGraphNodeForm = GraphNodeForm.GetInstance();
            myGraphNodeForm.View = view;
            myGraphNodeForm.Show(dockPanel1, DockState.DockRight);
            return myGraphNodeForm;
        }
        public GraphNodeForm ShowProperties()
        {
            GraphNodeForm myGraphNodeForm = GraphNodeForm.GetInstance();
            myGraphNodeForm.Show(dockPanel1, DockState.DockRight);
            return myGraphNodeForm;
        }

        // Menu commands (ToolStripMenuItem Click event handlers)

        protected void fileMenu_DropDownOpening(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                GoDocument doc = view.Document;
                fileCloseMenuItem.Enabled = true;
                fileSaveMenuItem.Enabled = doc.IsModified;
                fileSaveAsMenuItem.Enabled = true;
                fileSaveAllMenuItem.Enabled = true;
                filePrintMenuItem.Enabled = true;
                filePrintPreviewMenuItem.Enabled = true;
            }
            else
            {
                fileCloseMenuItem.Enabled = false;
                fileSaveMenuItem.Enabled = false;
                fileSaveAsMenuItem.Enabled = false;
                fileSaveAllMenuItem.Enabled = false;
                filePrintMenuItem.Enabled = false;
                filePrintPreviewMenuItem.Enabled = false;
            }
        }
        protected void fileNewMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphViewWindow canvas = new GraphViewWindow();
            canvas.Show(dockPanel1, DockState.Document);
            canvas.View.UpdateFormInfo();
            GraphDoc doc = canvas.View.Doc;
            //doc.AddTitleAndAnnotation();
            doc.UndoManager.Clear();
            doc.IsModified = false;
        }

        protected void fileOpenMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphViewWindow.Open(dockPanel1);
        }

        protected void fileCloseMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphViewWindow canvas = this.ActiveMdiChild as GraphViewWindow;
            if (canvas != null)
                canvas.Close();
        }

        protected void fileSaveMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphViewWindow canvas = this.ActiveMdiChild as GraphViewWindow;
            if (canvas != null)
                canvas.Save();
        }

        protected void fileSaveAsMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphViewWindow canvas = this.ActiveMdiChild as GraphViewWindow;
            if (canvas != null)
                canvas.SaveAs();
        }

        protected void fileSaveAllMenuItem_Click(Object sender, EventArgs evt)
        {
            foreach (Form f in this.MdiChildren)
            {
                GraphViewWindow w = f as GraphViewWindow;
                if (w != null)
                    w.Save();
            }
        }

        protected void filePrintMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                view.Print();
            }
        }

        protected void filePrintPreviewMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                view.PrintPreview();
            }
        }

        protected void fileExitMenuItem_Click(Object sender, EventArgs evt)
        {
            Close();
        }


        protected void editMenu_DropDownOpening(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                editUndoMenuItem.Enabled = view.CanUndo();
                if (editUndoMenuItem.Enabled)
                {
                    editUndoMenuItem.Text = "Undo " + view.Document.UndoManager.UndoPresentationName;
                }
                else
                {
                    editUndoMenuItem.Text = "Undo";
                }
                editRedoMenuItem.Enabled = view.CanRedo();
                if (editRedoMenuItem.Enabled)
                {
                    editRedoMenuItem.Text = "Redo " + view.Document.UndoManager.RedoPresentationName;
                }
                else
                {
                    editRedoMenuItem.Text = "Redo";
                }
                editCutMenuItem.Enabled = view.CanEditCut();
                editCopyMenuItem.Enabled = view.CanEditCopy();
                editPasteMenuItem.Enabled = view.CanEditPaste();
                editDeleteMenuItem.Enabled = view.CanEditDelete();
            }
        }

        protected void editUndoMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                view.Undo();
            }
        }

        protected void editRedoMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                view.Redo();
            }
        }

        protected void editCutMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                view.EditCut();
            }
        }

        protected void editCopyMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                view.EditCopy();
            }
        }

        protected void editPasteMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                view.EditPaste();
            }
        }

        protected void editDeleteMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                view.EditDelete();
            }
        }

        protected void editSelectAllMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                view.SelectAll();
            }
        }


        protected void viewZoomInMenuItem_Click(object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.ZoomIn();
        }

        protected void viewZoomOutMenuItem_Click(object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.ZoomOut();
        }

        protected void viewZoomNormalMenuItem_Click(object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.ZoomNormal();
        }

        protected void viewZoomToFitMenuItem_Click(object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.ZoomToFit();
        }


        protected void viewOverviewMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null)
            {
                ShowOverview(view);
            }
        }

        public GoOverview ShowOverview(GoView view)
        {
            if (myOverviewForm == null)
            {
                GoOverview ov = new GoOverview();
                ov.BackColor = view.BackColor;
                Form form = new Form();
                form.Text = "Overview";
                ov.Dock = DockStyle.Fill;
                form.Controls.Add(ov);
                form.Size = new Size(200, 200);
                form.ShowInTaskbar = false;
                form.Owner = this;
                myOverviewForm = form;
                myOverview = ov;
                myOverviewForm.Closed += new EventHandler(this.overview_Closed);
            }
            myOverview.Observed = view;
            myOverviewForm.Show();
            return myOverview;
        }

        protected void overview_Closed(object sender, EventArgs evt)
        {
            if (sender == myOverviewForm)
            {
                myOverviewForm = null;
                myOverview = null;
            }
        }


        protected void viewPropertiesMenuItem_Click(Object sender, EventArgs evt)
        {
            GoView view = GetCurrentGoView();
            if (view != null && view.Document is GraphDoc)
            {
                GraphDocForm dlg = new GraphDocForm();
                dlg.Doc = (GraphDoc)view.Document;
                dlg.ShowDialog();
            }
        }

        protected void insertMenu_DropDownOpening(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            bool ok = (view != null && !view.Doc.IsReadOnly);
            insertCommentMenuItem.Enabled = ok;
            insertNodeMenuItem.Enabled = ok;
            insertRelationshipMenuItem.Enabled = ok && view.Selection.Count >= 2;
            drawRelationshipMenuItem.Enabled = ok && view.Document.Count >= 2;
        }

        protected void insertCommentMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.Doc.InsertComment();
        }

        protected void insertNodeMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.Doc.InsertNode();//lzy
        }

        private void insertRelationshipMenuItem_Click(object sender, System.EventArgs e)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.CreateRelationshipsAmongSelection();
        }

        private void drawRelationshipMenuItem_Click(object sender, System.EventArgs e)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.StartDrawingRelationship();
        }


        protected void formatMenu_DropDownOpening(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            bool ok = (view != null &&
                       !view.Doc.IsReadOnly &&
                       (view.Selection.Count >= 2));
            formatAlignLeftsMenuItem.Enabled = ok;
            formatAlignHorizontalCentersMenuItem.Enabled = ok;
            formatAlignRightsMenuItem.Enabled = ok;
            formatAlignTopsMenuItem.Enabled = ok;
            formatAlignVerticalCentersMenuItem.Enabled = ok;
            formatAlignBottomsMenuItem.Enabled = ok;
        }

        protected void formatAlignLeftsMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.AlignLeftSides();
        }

        protected void formatAlignHorizontalCentersMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.AlignHorizontalCenters();
        }

        protected void formatAlignRightsMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.AlignRightSides();
        }

        protected void formatAlignTopsMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.AlignTops();
        }

        protected void formatAlignVerticalCentersMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.AlignVerticalCenters();
        }

        protected void formatAlignBottomsMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.AlignBottoms();
        }

        protected void formatSameSizeWidthsMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.MakeWidthsSame();
        }

        protected void formatSameSizeHeightsMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.MakeHeightsSame();
        }

        protected void formatSameSizeBothMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
                view.MakeSizesSame();
        }

        protected void windowNewMenuItem_Click(Object sender, EventArgs evt)
        {
            GraphView view = GetCurrentGraphView();
            if (view != null)
            {
                GraphViewWindow w = new GraphViewWindow();
                w.View.Document = view.Document;
                w.MdiParent = this;
                w.View.UpdateTitle();
                w.Show();
            }
        }

        protected void windowCascadeMenuItem_Click(Object sender, EventArgs evt)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        protected void windowTileHorizontallyMenuItem_Click(Object sender, EventArgs evt)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        protected void windowTileVerticallyMenuItem_Click(Object sender, EventArgs evt)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        protected void windowCloseAllDocumentsMenuItem_Click(Object sender, EventArgs evt)
        {
            foreach (Form f in this.MdiChildren)
            {
                f.Close();
            }
        }

        protected void helpAboutMenuItem_Click(Object sender, EventArgs evt)
        {
            String msg = "Built using GoDiagram(tm) from Northwoods Software\r\nGoDiagram version " + GoView.Version.ToString();
            msg += "\r\nCopyright ?Northwoods Software Corporation, 1998-2020. All Rights Reserved.";
            MessageBox.Show(msg, "About GoDiagram", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void EnableToolStripEditButtons(GoView view)
        {
            if (view != null && view.Document != null && view.Selection != null)
            {
                GoDocument doc = view.Document;
                fileSaveToolStripButton.Enabled = doc.IsModified;
                editCutToolStripButton.Enabled = view.CanEditCut();
                editCopyToolStripButton.Enabled = view.CanEditCopy();
                editPasteToolStripButton.Enabled = view.CanEditPaste();
            }
        }

        public void EnableToolStripUndoButtons(GoView view)
        {
            if (view != null && view.Document != null && view.Selection != null)
            {
                GoDocument doc = view.Document;
                editUndoToolStripButton.Enabled = view.CanUndo();
                if (editUndoToolStripButton.Enabled)
                {
                    editUndoToolStripButton.ToolTipText = "Undo " + doc.UndoManager.UndoPresentationName;
                }
                else
                {
                    editUndoToolStripButton.ToolTipText = "(Undo)";
                }
                editRedoToolStripButton.Enabled = view.CanRedo();
                if (editRedoToolStripButton.Enabled)
                {
                    editRedoToolStripButton.ToolTipText = "Redo " + doc.UndoManager.RedoPresentationName;
                }
                else
                {
                    editRedoToolStripButton.ToolTipText = "(Redo)";
                }
            }
        }


        // palette event handlers

        protected void myPalette_KeyDown(Object sender, KeyEventArgs evt)
        {
            if (evt.KeyCode == Keys.Insert)
            {
                GraphView view = GetCurrentGraphView();
                GoObject obj = myPalette.Selection.Primary;
                if (view != null && obj != null)
                {
                    view.StartTransaction();
                    GoObject newobj = view.Doc.AddCopy(obj, view.Doc.NextNodePosition());
                    view.FinishTransaction("Insert Node From Palette");
                }
            }
        }


        protected override void OnMdiChildActivate(EventArgs evt)
        {
            base.OnMdiChildActivate(evt);
            GraphViewWindow w = this.ActiveMdiChild as GraphViewWindow;
            if (w != null)
            {
                GraphNodeForm.GetInstance().View = w.ActiveControl as GoView;
                w.View.UpdateFormInfo();
            }
        }


        // globally useful methods
        public void SetStatusMessage(String s)
        {
            statusMessagePanel.Text = s;
        }

        public void SetStatusZoom(float scale)
        {
            String m = Math.Round((double)scale * 100, 3).ToString();
            statusZoomPanel.Text = m + "%";
        }


        public GoView GetCurrentGoView()
        {
            if (this.ActiveMdiChild != null)
                return this.ActiveMdiChild.ActiveControl as GoView;
            else
                return null;
        }

        public GraphView GetCurrentGraphView()
        {
            if (this.ActiveMdiChild != null)
                return this.ActiveMdiChild.ActiveControl as GraphView;
            else
                return null;
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        //public static void Main(string[] args)
        //{
        //    Application.EnableVisualStyles();
        //    Application.Run(new MainForm());
        //}

        public static MainForm App
        {
            get { return myMainForm; }
        }

        private static MainForm myMainForm = null;

        private Form myOverviewForm = null;
        private GoOverview myOverview = null;
        private Panel myPanel = null;
        private GoPalette myPalette = null;
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
}
