namespace 同步指静脉数据
{
	partial class FormMain
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
			this.bt_Update = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.tx_Msg = new System.Windows.Forms.TextBox();
			this.bt_Close = new System.Windows.Forms.Button();
			this.tx_BN = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tx_UserId = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.bt_View = new System.Windows.Forms.Button();
			this.tx_xgd = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tx_SelectSql = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.tx_ConnectString = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.bt_TestDecode = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.tx_Msg1 = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// bt_Update
			// 
			this.bt_Update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bt_Update.Location = new System.Drawing.Point(617, 460);
			this.bt_Update.Name = "bt_Update";
			this.bt_Update.Size = new System.Drawing.Size(61, 23);
			this.bt_Update.TabIndex = 5;
			this.bt_Update.Text = "更新";
			this.bt_Update.UseVisualStyleBackColor = true;
			this.bt_Update.Click += new System.EventHandler(this.bt_Update_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(4, 288);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 12);
			this.label3.TabIndex = 6;
			this.label3.Text = "日志信息：";
			// 
			// tx_Msg
			// 
			this.tx_Msg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tx_Msg.Location = new System.Drawing.Point(68, 285);
			this.tx_Msg.Multiline = true;
			this.tx_Msg.Name = "tx_Msg";
			this.tx_Msg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tx_Msg.Size = new System.Drawing.Size(340, 169);
			this.tx_Msg.TabIndex = 7;
			// 
			// bt_Close
			// 
			this.bt_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bt_Close.Location = new System.Drawing.Point(684, 460);
			this.bt_Close.Name = "bt_Close";
			this.bt_Close.Size = new System.Drawing.Size(61, 23);
			this.bt_Close.TabIndex = 8;
			this.bt_Close.Text = "关闭";
			this.bt_Close.UseVisualStyleBackColor = true;
			this.bt_Close.Click += new System.EventHandler(this.bt_Close_Click);
			// 
			// tx_BN
			// 
			this.tx_BN.Location = new System.Drawing.Point(87, 258);
			this.tx_BN.MaxLength = 5;
			this.tx_BN.Name = "tx_BN";
			this.tx_BN.Size = new System.Drawing.Size(102, 21);
			this.tx_BN.TabIndex = 10;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(4, 261);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(83, 12);
			this.label5.TabIndex = 9;
			this.label5.Text = "设备BN码： BN";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.tx_UserId);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.bt_View);
			this.groupBox1.Controls.Add(this.tx_xgd);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(733, 78);
			this.groupBox1.TabIndex = 11;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "单个更新";
			// 
			// tx_UserId
			// 
			this.tx_UserId.Location = new System.Drawing.Point(75, 47);
			this.tx_UserId.Name = "tx_UserId";
			this.tx_UserId.Size = new System.Drawing.Size(126, 21);
			this.tx_UserId.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(207, 50);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 12);
			this.label4.TabIndex = 6;
			this.label4.Text = "（即存储位置）";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "用户ID：";
			// 
			// bt_View
			// 
			this.bt_View.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bt_View.Location = new System.Drawing.Point(672, 18);
			this.bt_View.Name = "bt_View";
			this.bt_View.Size = new System.Drawing.Size(55, 23);
			this.bt_View.TabIndex = 4;
			this.bt_View.Text = "浏览";
			this.bt_View.UseVisualStyleBackColor = true;
			this.bt_View.Click += new System.EventHandler(this.bt_View_Click);
			// 
			// tx_xgd
			// 
			this.tx_xgd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tx_xgd.Location = new System.Drawing.Point(75, 20);
			this.tx_xgd.Name = "tx_xgd";
			this.tx_xgd.ReadOnly = true;
			this.tx_xgd.Size = new System.Drawing.Size(598, 21);
			this.tx_xgd.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 12);
			this.label1.TabIndex = 2;
			this.label1.Text = "特征文件：";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.tx_SelectSql);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.tx_ConnectString);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Enabled = false;
			this.groupBox2.Location = new System.Drawing.Point(12, 96);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(733, 156);
			this.groupBox2.TabIndex = 12;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "批量更新";
			// 
			// tx_SelectSql
			// 
			this.tx_SelectSql.Location = new System.Drawing.Point(92, 95);
			this.tx_SelectSql.Multiline = true;
			this.tx_SelectSql.Name = "tx_SelectSql";
			this.tx_SelectSql.Size = new System.Drawing.Size(635, 55);
			this.tx_SelectSql.TabIndex = 16;
			this.tx_SelectSql.Text = "SELECT IA_VALUE,IA_REMARK\r\n  FROM IDENTITY_AUTH\r\n  Where IA_TYPE=2\r\n  Order By IA" +
				"_VALUE";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 98);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(89, 12);
			this.label7.TabIndex = 9;
			this.label7.Text = "数据查询语句：";
			// 
			// tx_ConnectString
			// 
			this.tx_ConnectString.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tx_ConnectString.Location = new System.Drawing.Point(8, 32);
			this.tx_ConnectString.Multiline = true;
			this.tx_ConnectString.Name = "tx_ConnectString";
			this.tx_ConnectString.Size = new System.Drawing.Size(719, 57);
			this.tx_ConnectString.TabIndex = 8;
			this.tx_ConnectString.Text = "Data Source=192.9.100.141;Initial Catalog=file_transfer_kjb;User ID=sa;Password=s" +
				"a2014";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 17);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(113, 12);
			this.label6.TabIndex = 7;
			this.label6.Text = "数据库连接字符串：";
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Checked = true;
			this.radioButton1.Location = new System.Drawing.Point(258, 259);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(71, 16);
			this.radioButton1.TabIndex = 13;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "单个更新";
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Location = new System.Drawing.Point(355, 259);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(71, 16);
			this.radioButton2.TabIndex = 14;
			this.radioButton2.Text = "批量更新";
			this.radioButton2.UseVisualStyleBackColor = true;
			this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
			// 
			// bt_TestDecode
			// 
			this.bt_TestDecode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bt_TestDecode.Location = new System.Drawing.Point(6, 460);
			this.bt_TestDecode.Name = "bt_TestDecode";
			this.bt_TestDecode.Size = new System.Drawing.Size(61, 23);
			this.bt_TestDecode.TabIndex = 15;
			this.bt_TestDecode.Text = "测试解压";
			this.bt_TestDecode.UseVisualStyleBackColor = true;
			this.bt_TestDecode.Click += new System.EventHandler(this.bt_TestDecode_Click);
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(414, 288);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(65, 12);
			this.label8.TabIndex = 16;
			this.label8.Text = "更新信息：";
			// 
			// tx_Msg1
			// 
			this.tx_Msg1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tx_Msg1.Location = new System.Drawing.Point(474, 285);
			this.tx_Msg1.Multiline = true;
			this.tx_Msg1.Name = "tx_Msg1";
			this.tx_Msg1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tx_Msg1.Size = new System.Drawing.Size(271, 169);
			this.tx_Msg1.TabIndex = 17;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(757, 495);
			this.Controls.Add(this.tx_Msg1);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.bt_TestDecode);
			this.Controls.Add(this.radioButton2);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.tx_BN);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.bt_Close);
			this.Controls.Add(this.tx_Msg);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.bt_Update);
			this.Name = "FormMain";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bt_Update;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tx_Msg;
		private System.Windows.Forms.Button bt_Close;
		private System.Windows.Forms.TextBox tx_BN;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox tx_UserId;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button bt_View;
		private System.Windows.Forms.TextBox tx_xgd;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox tx_ConnectString;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.TextBox tx_SelectSql;
		private System.Windows.Forms.Button bt_TestDecode;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox tx_Msg1;
	}
}

