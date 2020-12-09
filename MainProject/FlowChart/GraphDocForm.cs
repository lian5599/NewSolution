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

namespace FlowCharter {
	/// <summary>
	/// Summary description for GraphDocForm.
	/// </summary>
	public class GraphDocForm : System.Windows.Forms.Form {
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button okButton;

    /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GraphDocForm() {
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.cancelButton = new System.Windows.Forms.Button();
      this.okButton = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.nameTextBox.Location = new System.Drawing.Point(24, 32);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(376, 20);
      this.nameTextBox.TabIndex = 1;
      this.nameTextBox.Text = "";
      // 
      // cancelButton
      // 
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(247, 112);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.TabIndex = 4;
      this.cancelButton.Text = "Cancel";
      // 
      // okButton
      // 
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(103, 112);
      this.okButton.Name = "okButton";
      this.okButton.TabIndex = 3;
      this.okButton.Text = "OK";
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(24, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(376, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Document Name:";
      // 
      // GraphDocForm
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(428, 153);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.cancelButton,
                                                                  this.okButton,
                                                                  this.nameTextBox,
                                                                  this.label1});
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "GraphDocForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Document Properties";
      this.ResumeLayout(false);

    }
		#endregion

    protected void EnableControls(bool enable) {
      this.nameTextBox.Enabled = enable;
      this.okButton.Enabled = enable;
    }

    protected void UpdateDialog() {
      if (this.Doc == null) return;
      this.nameTextBox.Text = this.Doc.Name;
      EnableControls(!this.Doc.IsReadOnly);
    }

    protected void UpdateObject() {
      if (this.Doc == null) return;
      this.Doc.StartTransaction();
      this.Doc.Name = this.nameTextBox.Text;
      this.Doc.FinishTransaction("Change Document Properties");
    }

    public GraphDoc Doc {
      get { return myDoc; }
      set {
        if (value == null) return;
        myDoc = value;
        UpdateDialog();
      }
    }

    private GraphDoc myDoc = null;

    private void okButton_Click(object sender, System.EventArgs e) {
      UpdateObject();
    }
	}
}
