namespace FlowCharter
{
    partial class PaletteForm
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
            this.myPalette = new FlowCharter.MyPalette();
            this.SuspendLayout();
            // 
            // myPalette
            // 
            this.myPalette.AllowDelete = false;
            this.myPalette.AllowEdit = false;
            this.myPalette.AllowInsert = false;
            this.myPalette.AllowLink = false;
            this.myPalette.AllowMove = false;
            this.myPalette.AllowReshape = false;
            this.myPalette.AllowResize = false;
            this.myPalette.ArrowMoveLarge = 10F;
            this.myPalette.ArrowMoveSmall = 1F;
            this.myPalette.AutoScrollRegion = new System.Drawing.Size(0, 0);
            this.myPalette.BackColor = System.Drawing.Color.White;
            this.myPalette.GridCellSizeHeight = 60F;
            this.myPalette.GridCellSizeWidth = 60F;
            this.myPalette.GridOriginX = 20F;
            this.myPalette.GridOriginY = 5F;
            this.myPalette.Location = new System.Drawing.Point(0, 0);
            this.myPalette.Name = "myPalette";
            this.myPalette.ShowsNegativeCoordinates = false;
            this.myPalette.Size = new System.Drawing.Size(0, 0);
            this.myPalette.TabIndex = 0;
            // 
            // PaletteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.myPalette);
            this.Name = "PaletteForm";
            this.Text = "PaletteForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PaletteForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private MyPalette myPalette;
    }
}