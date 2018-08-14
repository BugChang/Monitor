namespace TestZhiJingMai
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
			this.bt_Open = new System.Windows.Forms.Button();
			this.bt_Close = new System.Windows.Forms.Button();
			this.bt_Get = new System.Windows.Forms.Button();
			this.bt_MuBan = new System.Windows.Forms.Button();
			this.bt_Search = new System.Windows.Forms.Button();
			this.bt_SearchText = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// bt_Open
			// 
			this.bt_Open.Location = new System.Drawing.Point(12, 12);
			this.bt_Open.Name = "bt_Open";
			this.bt_Open.Size = new System.Drawing.Size(75, 23);
			this.bt_Open.TabIndex = 0;
			this.bt_Open.Text = "打开设备";
			this.bt_Open.UseVisualStyleBackColor = true;
			this.bt_Open.Click += new System.EventHandler(this.bt_Open_Click);
			// 
			// bt_Close
			// 
			this.bt_Close.Location = new System.Drawing.Point(93, 12);
			this.bt_Close.Name = "bt_Close";
			this.bt_Close.Size = new System.Drawing.Size(75, 23);
			this.bt_Close.TabIndex = 1;
			this.bt_Close.Text = "关闭设备";
			this.bt_Close.UseVisualStyleBackColor = true;
			this.bt_Close.Click += new System.EventHandler(this.bt_Close_Click);
			// 
			// bt_Get
			// 
			this.bt_Get.Location = new System.Drawing.Point(12, 41);
			this.bt_Get.Name = "bt_Get";
			this.bt_Get.Size = new System.Drawing.Size(75, 23);
			this.bt_Get.TabIndex = 2;
			this.bt_Get.Text = "采集手指";
			this.bt_Get.UseVisualStyleBackColor = true;
			this.bt_Get.Click += new System.EventHandler(this.bt_Get_Click);
			// 
			// bt_MuBan
			// 
			this.bt_MuBan.Location = new System.Drawing.Point(12, 70);
			this.bt_MuBan.Name = "bt_MuBan";
			this.bt_MuBan.Size = new System.Drawing.Size(75, 23);
			this.bt_MuBan.TabIndex = 3;
			this.bt_MuBan.Text = "加载模板";
			this.bt_MuBan.UseVisualStyleBackColor = true;
			this.bt_MuBan.Click += new System.EventHandler(this.bt_MuBan_Click);
			// 
			// bt_Search
			// 
			this.bt_Search.Location = new System.Drawing.Point(93, 70);
			this.bt_Search.Name = "bt_Search";
			this.bt_Search.Size = new System.Drawing.Size(75, 23);
			this.bt_Search.TabIndex = 4;
			this.bt_Search.Text = "比较信息";
			this.bt_Search.UseVisualStyleBackColor = true;
			this.bt_Search.Click += new System.EventHandler(this.bt_Search_Click);
			// 
			// bt_SearchText
			// 
			this.bt_SearchText.Location = new System.Drawing.Point(12, 99);
			this.bt_SearchText.Name = "bt_SearchText";
			this.bt_SearchText.Size = new System.Drawing.Size(75, 23);
			this.bt_SearchText.TabIndex = 5;
			this.bt_SearchText.Text = "比较信息文本";
			this.bt_SearchText.UseVisualStyleBackColor = true;
			this.bt_SearchText.Click += new System.EventHandler(this.bt_SearchText_Click);
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(12, 128);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(577, 273);
			this.textBox1.TabIndex = 6;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(601, 414);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.bt_SearchText);
			this.Controls.Add(this.bt_Search);
			this.Controls.Add(this.bt_MuBan);
			this.Controls.Add(this.bt_Get);
			this.Controls.Add(this.bt_Close);
			this.Controls.Add(this.bt_Open);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bt_Open;
		private System.Windows.Forms.Button bt_Close;
		private System.Windows.Forms.Button bt_Get;
		private System.Windows.Forms.Button bt_MuBan;
		private System.Windows.Forms.Button bt_Search;
		private System.Windows.Forms.Button bt_SearchText;
		private System.Windows.Forms.TextBox textBox1;
	}
}

