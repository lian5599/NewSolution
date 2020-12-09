namespace TKS.SubForm
{
    partial class HardWareForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.communicationBaseBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.contextMenuStripOutput = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgv = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hardwareIdDataGridViewComboBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.kindDataGridViewComboBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.modeDataGridViewComboBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.paramStrDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.paramIntDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeoutDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.communicationBaseBindingSource)).BeginInit();
            this.contextMenuStripOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // communicationBaseBindingSource
            // 
            this.communicationBaseBindingSource.DataSource = typeof(Communication.CommunicationBase);
            // 
            // contextMenuStripOutput
            // 
            this.contextMenuStripOutput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.contextMenuStripOutput.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripOutput.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.contextMenuStripOutput.Name = "contextMenuStripOutput";
            this.contextMenuStripOutput.Size = new System.Drawing.Size(301, 162);
            this.contextMenuStripOutput.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripOutput_Opening);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(300, 38);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AutoGenerateColumns = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.hardwareIdDataGridViewComboBoxColumn,
            this.kindDataGridViewComboBoxColumn,
            this.modeDataGridViewComboBoxColumn,
            this.paramStrDataGridViewTextBoxColumn,
            this.paramIntDataGridViewTextBoxColumn,
            this.timeoutDataGridViewTextBoxColumn});
            this.dgv.ContextMenuStrip = this.contextMenuStripOutput;
            this.dgv.DataSource = this.communicationBaseBindingSource;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            this.dgv.RowHeadersWidth = 30;
            this.dgv.RowTemplate.Height = 37;
            this.dgv.Size = new System.Drawing.Size(800, 450);
            this.dgv.TabIndex = 3;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.MinimumWidth = 10;
            this.idDataGridViewTextBoxColumn.Name = "dataGridViewTextBoxColumn1";
            this.idDataGridViewTextBoxColumn.Width = 84;
            // 
            // hardwareIdDataGridViewTextBoxColumn
            // 
            this.hardwareIdDataGridViewComboBoxColumn.DataPropertyName = "HardwareId";
            this.hardwareIdDataGridViewComboBoxColumn.HeaderText = "HardwareId";
            this.hardwareIdDataGridViewComboBoxColumn.MinimumWidth = 10;
            this.hardwareIdDataGridViewComboBoxColumn.Name = "hardwareIdDataGridViewTextBoxColumn";
            this.hardwareIdDataGridViewComboBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.hardwareIdDataGridViewComboBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.hardwareIdDataGridViewComboBoxColumn.Width = 185;
            // 
            // KindDataGridViewTextBoxColumn2
            // 
            this.kindDataGridViewComboBoxColumn.DataPropertyName = "Kind";
            this.kindDataGridViewComboBoxColumn.HeaderText = "Kind";
            this.kindDataGridViewComboBoxColumn.MinimumWidth = 10;
            this.kindDataGridViewComboBoxColumn.Name = "KindDataGridViewTextBoxColumn2";
            this.kindDataGridViewComboBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.kindDataGridViewComboBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.kindDataGridViewComboBoxColumn.Width = 112;
            // 
            // modeDataGridViewTextBoxColumn3
            // 
            this.modeDataGridViewComboBoxColumn.DataPropertyName = "Mode";
            this.modeDataGridViewComboBoxColumn.HeaderText = "Mode";
            this.modeDataGridViewComboBoxColumn.MinimumWidth = 10;
            this.modeDataGridViewComboBoxColumn.Name = "modeDataGridViewTextBoxColumn3";
            this.modeDataGridViewComboBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.modeDataGridViewComboBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.modeDataGridViewComboBoxColumn.Width = 127;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.paramStrDataGridViewTextBoxColumn.DataPropertyName = "ParamStr";
            this.paramStrDataGridViewTextBoxColumn.HeaderText = "ParamStr";
            this.paramStrDataGridViewTextBoxColumn.MinimumWidth = 10;
            this.paramStrDataGridViewTextBoxColumn.Name = "dataGridViewTextBoxColumn4";
            this.paramStrDataGridViewTextBoxColumn.Width = 157;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.paramIntDataGridViewTextBoxColumn.DataPropertyName = "ParamInt";
            this.paramIntDataGridViewTextBoxColumn.HeaderText = "ParamInt";
            this.paramIntDataGridViewTextBoxColumn.MinimumWidth = 10;
            this.paramIntDataGridViewTextBoxColumn.Name = "dataGridViewTextBoxColumn5";
            this.paramIntDataGridViewTextBoxColumn.Width = 157;
            // 
            // timeoutDataGridViewTextBoxColumn
            // 
            this.timeoutDataGridViewTextBoxColumn.DataPropertyName = "Timeout";
            this.timeoutDataGridViewTextBoxColumn.HeaderText = "Timeout";
            this.timeoutDataGridViewTextBoxColumn.MinimumWidth = 10;
            this.timeoutDataGridViewTextBoxColumn.Name = "timeoutDataGridViewTextBoxColumn";
            this.timeoutDataGridViewTextBoxColumn.Width = 153;
            // 
            // HardWareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgv);
            this.Name = "HardWareForm";
            this.TabText = "HardWareFrom";
            this.Text = "HardWareFrom";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this._FormClosing);
            this.Load += new System.EventHandler(this.HardWareFrom_Load);
            ((System.ComponentModel.ISupportInitialize)(this.communicationBaseBindingSource)).EndInit();
            this.contextMenuStripOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource communicationBaseBindingSource;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOutput;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView dgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeoutDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewComboBoxColumn hardwareIdDataGridViewComboBoxColumn;
        //private System.Windows.Forms.DataGridViewComboBoxColumn kindDataGridViewComboBoxColumn;
        //private System.Windows.Forms.DataGridViewComboBoxColumn modeDataGridViewComboBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn paramStrDataGridViewTextBoxColumn;
        //private System.Windows.Forms.DataGridViewTextBoxColumn paramIntDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn hardwareIdDataGridViewComboBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn kindDataGridViewComboBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn modeDataGridViewComboBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramStrDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramIntDataGridViewTextBoxColumn;
    }
}