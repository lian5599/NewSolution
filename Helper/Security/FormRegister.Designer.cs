namespace Helper
{
    partial class FormRegister
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
            this.textBox_MachineCode = new System.Windows.Forms.TextBox();
            this.textBox_RegisterCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Btn_Register = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox_MachineCode
            // 
            this.textBox_MachineCode.Location = new System.Drawing.Point(146, 36);
            this.textBox_MachineCode.Multiline = true;
            this.textBox_MachineCode.Name = "textBox_MachineCode";
            this.textBox_MachineCode.Size = new System.Drawing.Size(294, 44);
            this.textBox_MachineCode.TabIndex = 0;
            // 
            // textBox_RegisterCode
            // 
            this.textBox_RegisterCode.Location = new System.Drawing.Point(146, 133);
            this.textBox_RegisterCode.Multiline = true;
            this.textBox_RegisterCode.Name = "textBox_RegisterCode";
            this.textBox_RegisterCode.Size = new System.Drawing.Size(294, 35);
            this.textBox_RegisterCode.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "机器码：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "注册码：";
            // 
            // Btn_Register
            // 
            this.Btn_Register.Location = new System.Drawing.Point(298, 260);
            this.Btn_Register.Name = "Btn_Register";
            this.Btn_Register.Size = new System.Drawing.Size(134, 70);
            this.Btn_Register.TabIndex = 2;
            this.Btn_Register.Text = "注册";
            this.Btn_Register.UseVisualStyleBackColor = true;
            this.Btn_Register.Click += new System.EventHandler(this.Btn_Register_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(394, 24);
            this.label3.TabIndex = 3;
            this.label3.Text = "请将机器码发给厂商，以获取注册码";
            // 
            // FormRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 351);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Btn_Register);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_RegisterCode);
            this.Controls.Add(this.textBox_MachineCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormRegister";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "注册窗口";
            this.Load += new System.EventHandler(this.FormRegister_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_MachineCode;
        private System.Windows.Forms.TextBox textBox_RegisterCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Btn_Register;
        private System.Windows.Forms.Label label3;
    }
}