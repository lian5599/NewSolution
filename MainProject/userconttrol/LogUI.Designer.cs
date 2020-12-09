namespace TKS
{
    partial class LogUI
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listView_Log = new TKS.ListViewNF();
            this.contextMenuStripOutput = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView_Log
            // 
            this.listView_Log.ContextMenuStrip = this.contextMenuStripOutput;
            this.listView_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Log.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listView_Log.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_Log.HideSelection = false;
            this.listView_Log.Location = new System.Drawing.Point(0, 0);
            this.listView_Log.Name = "listView_Log";
            this.listView_Log.Size = new System.Drawing.Size(649, 598);
            this.listView_Log.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listView_Log.TabIndex = 0;
            this.listView_Log.UseCompatibleStateImageBehavior = false;
            this.listView_Log.View = System.Windows.Forms.View.Details;
            this.listView_Log.Resize += new System.EventHandler(this.listView_Log_SizeChanged);
            // 
            // contextMenuStripOutput
            // 
            this.contextMenuStripOutput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.contextMenuStripOutput.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripOutput.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.contextMenuStripOutput.Name = "contextMenuStripOutput";
            this.contextMenuStripOutput.Size = new System.Drawing.Size(145, 42);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(144, 38);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // LogUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView_Log);
            this.Name = "LogUI";
            this.Size = new System.Drawing.Size(649, 598);
            this.Load += new System.EventHandler(this.LogUI_Load);
            this.contextMenuStripOutput.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ListViewNF listView_Log;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOutput;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
    }
}
