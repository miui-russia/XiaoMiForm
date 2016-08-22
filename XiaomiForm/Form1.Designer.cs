namespace XiaomiWinForm
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.GetXiaoMiData = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.FromDate = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // GetXiaoMiData
            // 
            this.GetXiaoMiData.Location = new System.Drawing.Point(24, 36);
            this.GetXiaoMiData.Name = "GetXiaoMiData";
            this.GetXiaoMiData.Size = new System.Drawing.Size(75, 23);
            this.GetXiaoMiData.TabIndex = 0;
            this.GetXiaoMiData.Text = "GetData";
            this.GetXiaoMiData.UseVisualStyleBackColor = true;
            this.GetXiaoMiData.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(24, 89);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(438, 258);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // FromDate
            // 
            this.FromDate.AutoSize = true;
            this.FromDate.Location = new System.Drawing.Point(156, 41);
            this.FromDate.Name = "FromDate";
            this.FromDate.Size = new System.Drawing.Size(59, 12);
            this.FromDate.TabIndex = 2;
            this.FromDate.Text = "FromDate:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy/MM/dd";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(215, 36);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 21);
            this.dateTimePicker1.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 390);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.FromDate);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.GetXiaoMiData);
            this.Name = "Form1";
            this.Text = "Happy Movement";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GetXiaoMiData;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label FromDate;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
    }
}

