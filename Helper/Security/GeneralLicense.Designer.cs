namespace Helper
{
    partial class LicenseCreaterForm
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
            this.DateTimePicker1 = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.TextBox_MachineCode = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Btn_Genaral = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.TextBoxLicense = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DateTimePicker1
            // 
            this.DateTimePicker1.Location = new System.Drawing.Point(161, 62);
            this.DateTimePicker1.MinDate = new System.DateTime(2020, 11, 24, 0, 0, 0, 0);
            this.DateTimePicker1.Name = "DateTimePicker1";
            this.DateTimePicker1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
            this.DateTimePicker1.Size = new System.Drawing.Size(225, 37);
            this.DateTimePicker1.TabIndex = 3;
            this.DateTimePicker1.ValueNullable = new System.DateTime(2020, 11, 24, 0, 0, 0, 0);
            // 
            // TextBox_MachineCode
            // 
            this.TextBox_MachineCode.Location = new System.Drawing.Point(161, 143);
            this.TextBox_MachineCode.Name = "TextBox_MachineCode";
            this.TextBox_MachineCode.Size = new System.Drawing.Size(225, 39);
            this.TextBox_MachineCode.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "有效期：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "机器码：";
            // 
            // Btn_Genaral
            // 
            this.Btn_Genaral.Location = new System.Drawing.Point(477, 62);
            this.Btn_Genaral.Name = "Btn_Genaral";
            this.Btn_Genaral.Size = new System.Drawing.Size(135, 120);
            this.Btn_Genaral.TabIndex = 6;
            this.Btn_Genaral.Values.Text = "生成";
            this.Btn_Genaral.Click += new System.EventHandler(this.Btn_Genaral_Click);
            // 
            // TextBoxLicense
            // 
            this.TextBoxLicense.Location = new System.Drawing.Point(161, 219);
            this.TextBoxLicense.Name = "TextBoxLicense";
            this.TextBoxLicense.Size = new System.Drawing.Size(451, 39);
            this.TextBoxLicense.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 224);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 24);
            this.label4.TabIndex = 5;
            this.label4.Text = "结果：";
            // 
            // LicenseCreaterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 306);
            this.Controls.Add(this.TextBoxLicense);
            this.Controls.Add(this.Btn_Genaral);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBox_MachineCode);
            this.Controls.Add(this.DateTimePicker1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LicenseCreaterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "生成注册码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker DateTimePicker1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox TextBox_MachineCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton Btn_Genaral;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox TextBoxLicense;
        private System.Windows.Forms.Label label4;
    }
}