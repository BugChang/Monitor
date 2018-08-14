namespace SetupConfig
{
	partial class FormBoxAddPi
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
			this.label1 = new System.Windows.Forms.Label();
			this.tx_StartG = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.rb_Lock1 = new System.Windows.Forms.RadioButton();
			this.rb_Lock2 = new System.Windows.Forms.RadioButton();
			this.bt_OK = new System.Windows.Forms.Button();
			this.bt_Cancel = new System.Windows.Forms.Button();
			this.tx_EndG = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tx_StartBG = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rb_GroupTypeB = new System.Windows.Forms.RadioButton();
			this.rb_GroupTypeA = new System.Windows.Forms.RadioButton();
			this.tx_BoxCount = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tx_BoxNo = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "正面起始组号：";
			// 
			// tx_StartG
			// 
			this.tx_StartG.Location = new System.Drawing.Point(97, 33);
			this.tx_StartG.MaxLength = 3;
			this.tx_StartG.Name = "tx_StartG";
			this.tx_StartG.Size = new System.Drawing.Size(100, 21);
			this.tx_StartG.TabIndex = 1;
			this.tx_StartG.Text = "001";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(220, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 12);
			this.label4.TabIndex = 0;
			this.label4.Text = "是否是双面锁：";
			// 
			// rb_Lock1
			// 
			this.rb_Lock1.AutoSize = true;
			this.rb_Lock1.Location = new System.Drawing.Point(315, 61);
			this.rb_Lock1.Name = "rb_Lock1";
			this.rb_Lock1.Size = new System.Drawing.Size(35, 16);
			this.rb_Lock1.TabIndex = 2;
			this.rb_Lock1.Text = "是";
			this.rb_Lock1.UseVisualStyleBackColor = true;
			// 
			// rb_Lock2
			// 
			this.rb_Lock2.AutoSize = true;
			this.rb_Lock2.Checked = true;
			this.rb_Lock2.Location = new System.Drawing.Point(356, 61);
			this.rb_Lock2.Name = "rb_Lock2";
			this.rb_Lock2.Size = new System.Drawing.Size(35, 16);
			this.rb_Lock2.TabIndex = 3;
			this.rb_Lock2.TabStop = true;
			this.rb_Lock2.Text = "否";
			this.rb_Lock2.UseVisualStyleBackColor = true;
			// 
			// bt_OK
			// 
			this.bt_OK.Location = new System.Drawing.Point(239, 141);
			this.bt_OK.Name = "bt_OK";
			this.bt_OK.Size = new System.Drawing.Size(75, 23);
			this.bt_OK.TabIndex = 4;
			this.bt_OK.Text = "确定";
			this.bt_OK.UseVisualStyleBackColor = true;
			this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
			// 
			// bt_Cancel
			// 
			this.bt_Cancel.Location = new System.Drawing.Point(328, 141);
			this.bt_Cancel.Name = "bt_Cancel";
			this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
			this.bt_Cancel.TabIndex = 5;
			this.bt_Cancel.Text = "取消";
			this.bt_Cancel.UseVisualStyleBackColor = true;
			this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
			// 
			// tx_EndG
			// 
			this.tx_EndG.Location = new System.Drawing.Point(303, 33);
			this.tx_EndG.MaxLength = 3;
			this.tx_EndG.Name = "tx_EndG";
			this.tx_EndG.Size = new System.Drawing.Size(100, 21);
			this.tx_EndG.TabIndex = 7;
			this.tx_EndG.Text = "002";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(220, 36);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(89, 12);
			this.label5.TabIndex = 6;
			this.label5.Text = "正面结束组号：";
			// 
			// tx_StartBG
			// 
			this.tx_StartBG.Location = new System.Drawing.Point(97, 60);
			this.tx_StartBG.MaxLength = 3;
			this.tx_StartBG.Name = "tx_StartBG";
			this.tx_StartBG.Size = new System.Drawing.Size(100, 21);
			this.tx_StartBG.TabIndex = 9;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(14, 63);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(89, 12);
			this.label6.TabIndex = 8;
			this.label6.Text = "背面起始组号：";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rb_GroupTypeB);
			this.groupBox1.Controls.Add(this.rb_GroupTypeA);
			this.groupBox1.Controls.Add(this.tx_BoxCount);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(16, 87);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(387, 48);
			this.groupBox1.TabIndex = 12;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "箱组类型";
			// 
			// rb_GroupTypeB
			// 
			this.rb_GroupTypeB.AutoSize = true;
			this.rb_GroupTypeB.Checked = true;
			this.rb_GroupTypeB.Location = new System.Drawing.Point(75, 20);
			this.rb_GroupTypeB.Name = "rb_GroupTypeB";
			this.rb_GroupTypeB.Size = new System.Drawing.Size(53, 16);
			this.rb_GroupTypeB.TabIndex = 15;
			this.rb_GroupTypeB.TabStop = true;
			this.rb_GroupTypeB.Text = "B型箱";
			this.rb_GroupTypeB.UseVisualStyleBackColor = true;
			this.rb_GroupTypeB.CheckedChanged += new System.EventHandler(this.rb_GroupType_CheckedChanged);
			// 
			// rb_GroupTypeA
			// 
			this.rb_GroupTypeA.AutoSize = true;
			this.rb_GroupTypeA.Location = new System.Drawing.Point(6, 20);
			this.rb_GroupTypeA.Name = "rb_GroupTypeA";
			this.rb_GroupTypeA.Size = new System.Drawing.Size(53, 16);
			this.rb_GroupTypeA.TabIndex = 14;
			this.rb_GroupTypeA.Text = "A型箱";
			this.rb_GroupTypeA.UseVisualStyleBackColor = true;
			this.rb_GroupTypeA.CheckedChanged += new System.EventHandler(this.rb_GroupType_CheckedChanged);
			// 
			// tx_BoxCount
			// 
			this.tx_BoxCount.Location = new System.Drawing.Point(226, 19);
			this.tx_BoxCount.MaxLength = 3;
			this.tx_BoxCount.Name = "tx_BoxCount";
			this.tx_BoxCount.Size = new System.Drawing.Size(32, 21);
			this.tx_BoxCount.TabIndex = 13;
			this.tx_BoxCount.Text = "1";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(143, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 12);
			this.label2.TabIndex = 12;
			this.label2.Text = "每组分箱个数：";
			// 
			// tx_BoxNo
			// 
			this.tx_BoxNo.Location = new System.Drawing.Point(97, 6);
			this.tx_BoxNo.MaxLength = 3;
			this.tx_BoxNo.Name = "tx_BoxNo";
			this.tx_BoxNo.Size = new System.Drawing.Size(100, 21);
			this.tx_BoxNo.TabIndex = 14;
			this.tx_BoxNo.Text = "001";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(89, 12);
			this.label3.TabIndex = 13;
			this.label3.Text = "起始逻辑箱号：";
			// 
			// FormBoxAddPi
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(411, 191);
			this.Controls.Add(this.tx_BoxNo);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.tx_StartBG);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.tx_EndG);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.bt_Cancel);
			this.Controls.Add(this.bt_OK);
			this.Controls.Add(this.rb_Lock2);
			this.Controls.Add(this.rb_Lock1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tx_StartG);
			this.Controls.Add(this.label1);
			this.Name = "FormBoxAddPi";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "批量添加箱格";
			this.Load += new System.EventHandler(this.FormBoxAdd_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bt_OK;
        private System.Windows.Forms.Button bt_Cancel;
		private System.Windows.Forms.TextBox tx_StartG;
        private System.Windows.Forms.RadioButton rb_Lock1;
        private System.Windows.Forms.RadioButton rb_Lock2;
		private System.Windows.Forms.TextBox tx_EndG;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tx_StartBG;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rb_GroupTypeB;
		private System.Windows.Forms.RadioButton rb_GroupTypeA;
		private System.Windows.Forms.TextBox tx_BoxCount;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tx_BoxNo;
		private System.Windows.Forms.Label label3;
    }
}