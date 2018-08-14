namespace TestBox
{
    partial class FormSet
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
			this.cb_baud = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.comboBox4 = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBox5 = new System.Windows.Forms.ComboBox();
			this.button1 = new System.Windows.Forms.Button();
			this.comboBox6 = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// cb_baud
			// 
			this.cb_baud.FormattingEnabled = true;
			this.cb_baud.Items.AddRange(new object[] {
            "0：19200",
            "1：115200"});
			this.cb_baud.Location = new System.Drawing.Point(119, 6);
			this.cb_baud.Name = "cb_baud";
			this.cb_baud.Size = new System.Drawing.Size(121, 20);
			this.cb_baud.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(101, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "扫描头串口波特率";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "触发光电类型";
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "0：反射",
            "1：对射"});
			this.comboBox1.Location = new System.Drawing.Point(119, 32);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 20);
			this.comboBox1.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 12);
			this.label3.TabIndex = 5;
			this.label3.Text = "串口3用途";
			// 
			// comboBox2
			// 
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Items.AddRange(new object[] {
            "0：扫描头",
            "1：SYN6288语音模块",
            "2：串口摄像头",
            "3：指静脉仪"});
			this.comboBox2.Location = new System.Drawing.Point(77, 58);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(163, 20);
			this.comboBox2.TabIndex = 4;
			this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 87);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(59, 12);
			this.label4.TabIndex = 7;
			this.label4.Text = "串口1用途";
			// 
			// comboBox3
			// 
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Items.AddRange(new object[] {
            "0：串口屏幕",
            "1：SYN6288语音模块",
            "2：串口摄像头"});
			this.comboBox3.Location = new System.Drawing.Point(77, 84);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(163, 20);
			this.comboBox3.TabIndex = 6;
			this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 113);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(59, 12);
			this.label5.TabIndex = 9;
			this.label5.Text = "串口2用途";
			// 
			// comboBox4
			// 
			this.comboBox4.FormattingEnabled = true;
			this.comboBox4.Items.AddRange(new object[] {
            "0：RW163T读卡器(RF卡)",
            "1：M104读卡器(CPU卡)",
            "2：ZLG522S读卡器",
            "3：指静脉仪",
            "4：SYN6288语音模块"});
			this.comboBox4.Location = new System.Drawing.Point(77, 110);
			this.comboBox4.Name = "comboBox4";
			this.comboBox4.Size = new System.Drawing.Size(163, 20);
			this.comboBox4.TabIndex = 8;
			this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 139);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(101, 12);
			this.label6.TabIndex = 11;
			this.label6.Text = "门禁电机工作方式";
			// 
			// comboBox5
			// 
			this.comboBox5.FormattingEnabled = true;
			this.comboBox5.Items.AddRange(new object[] {
            "0：双向旋转",
            "1：单向顺时针选择",
            "2：单向逆时针旋转"});
			this.comboBox5.Location = new System.Drawing.Point(119, 136);
			this.comboBox5.Name = "comboBox5";
			this.comboBox5.Size = new System.Drawing.Size(121, 20);
			this.comboBox5.TabIndex = 10;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(165, 193);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 12;
			this.button1.Text = "确定";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// comboBox6
			// 
			this.comboBox6.FormattingEnabled = true;
			this.comboBox6.Items.AddRange(new object[] {
            "0：无",
            "1：升降机",
            "2：第二电控锁",
            "3：双面显示",
            "4：第二电控锁和双面显示",
            "5：升降机和双面显示"});
			this.comboBox6.Location = new System.Drawing.Point(119, 162);
			this.comboBox6.Name = "comboBox6";
			this.comboBox6.Size = new System.Drawing.Size(121, 20);
			this.comboBox6.TabIndex = 10;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 165);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(89, 12);
			this.label7.TabIndex = 11;
			this.label7.Text = "是否有升降装置";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(38, 193);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 13;
			this.button2.Text = "取消";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// FormSet
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(245, 228);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.comboBox6);
			this.Controls.Add(this.comboBox5);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboBox4);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboBox3);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBox2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cb_baud);
			this.Name = "FormSet";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FormSet";
			this.Load += new System.EventHandler(this.FormSet_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_baud;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button2;
    }
}