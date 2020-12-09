namespace PanasonicPlcVirtualServer
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxInt = new System.Windows.Forms.TextBox();
            this.textBoxStr = new System.Windows.Forms.TextBox();
            this.Btn_Create = new System.Windows.Forms.Button();
            this.btn_Serial = new System.Windows.Forms.Button();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.textBox_BaudRate = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxInt
            // 
            this.textBoxInt.Location = new System.Drawing.Point(131, 339);
            this.textBoxInt.Margin = new System.Windows.Forms.Padding(6);
            this.textBoxInt.Name = "textBoxInt";
            this.textBoxInt.Size = new System.Drawing.Size(196, 35);
            this.textBoxInt.TabIndex = 5;
            this.textBoxInt.Text = "8080";
            // 
            // textBoxStr
            // 
            this.textBoxStr.Location = new System.Drawing.Point(131, 267);
            this.textBoxStr.Margin = new System.Windows.Forms.Padding(6);
            this.textBoxStr.Name = "textBoxStr";
            this.textBoxStr.Size = new System.Drawing.Size(196, 35);
            this.textBoxStr.TabIndex = 4;
            this.textBoxStr.Tag = "";
            this.textBoxStr.Text = "127.0.0.1";
            // 
            // Btn_Create
            // 
            this.Btn_Create.Location = new System.Drawing.Point(399, 253);
            this.Btn_Create.Margin = new System.Windows.Forms.Padding(6);
            this.Btn_Create.Name = "Btn_Create";
            this.Btn_Create.Size = new System.Drawing.Size(162, 135);
            this.Btn_Create.TabIndex = 3;
            this.Btn_Create.Text = "网口";
            this.Btn_Create.UseVisualStyleBackColor = true;
            this.Btn_Create.Click += new System.EventHandler(this.Create_Click);
            // 
            // btn_Serial
            // 
            this.btn_Serial.Location = new System.Drawing.Point(399, 19);
            this.btn_Serial.Margin = new System.Windows.Forms.Padding(6);
            this.btn_Serial.Name = "btn_Serial";
            this.btn_Serial.Size = new System.Drawing.Size(162, 135);
            this.btn_Serial.TabIndex = 3;
            this.btn_Serial.Text = "串口";
            this.btn_Serial.UseVisualStyleBackColor = true;
            this.btn_Serial.Click += new System.EventHandler(this.CreateSerial_Click);
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(131, 39);
            this.textBox_Port.Margin = new System.Windows.Forms.Padding(6);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(196, 35);
            this.textBox_Port.TabIndex = 4;
            this.textBox_Port.Text = "COM1";
            // 
            // textBox_BaudRate
            // 
            this.textBox_BaudRate.Location = new System.Drawing.Point(131, 98);
            this.textBox_BaudRate.Margin = new System.Windows.Forms.Padding(6);
            this.textBox_BaudRate.Name = "textBox_BaudRate";
            this.textBox_BaudRate.Size = new System.Drawing.Size(196, 35);
            this.textBox_BaudRate.TabIndex = 5;
            this.textBox_BaudRate.Text = "9600";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox_BaudRate);
            this.Controls.Add(this.textBoxInt);
            this.Controls.Add(this.textBox_Port);
            this.Controls.Add(this.textBoxStr);
            this.Controls.Add(this.btn_Serial);
            this.Controls.Add(this.Btn_Create);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxInt;
        private System.Windows.Forms.TextBox textBoxStr;
        private System.Windows.Forms.Button Btn_Create;
        private System.Windows.Forms.Button btn_Serial;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.TextBox textBox_BaudRate;
    }
}

