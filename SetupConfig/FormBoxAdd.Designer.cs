namespace SetupConfig
{
    partial class FormBoxAdd
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
			this.tx_BoxNO = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tx_FrontBN = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tx_BackBN = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.bt_OK = new System.Windows.Forms.Button();
			this.bt_Cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "逻辑箱号：";
			// 
			// tx_BoxNO
			// 
			this.tx_BoxNO.Location = new System.Drawing.Point(76, 6);
			this.tx_BoxNO.MaxLength = 3;
			this.tx_BoxNO.Name = "tx_BoxNO";
			this.tx_BoxNO.Size = new System.Drawing.Size(100, 21);
			this.tx_BoxNO.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "正面BN号：";
			// 
			// tx_FrontBN
			// 
			this.tx_FrontBN.Location = new System.Drawing.Point(76, 33);
			this.tx_FrontBN.MaxLength = 7;
			this.tx_FrontBN.Name = "tx_FrontBN";
			this.tx_FrontBN.Size = new System.Drawing.Size(100, 21);
			this.tx_FrontBN.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 63);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "背面BN号：";
			// 
			// tx_BackBN
			// 
			this.tx_BackBN.Location = new System.Drawing.Point(76, 60);
			this.tx_BackBN.MaxLength = 7;
			this.tx_BackBN.Name = "tx_BackBN";
			this.tx_BackBN.Size = new System.Drawing.Size(100, 21);
			this.tx_BackBN.TabIndex = 1;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 90);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 12);
			this.label4.TabIndex = 0;
			this.label4.Text = "是否是双面锁：";
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Location = new System.Drawing.Point(107, 88);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(35, 16);
			this.radioButton1.TabIndex = 2;
			this.radioButton1.Text = "是";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Checked = true;
			this.radioButton2.Location = new System.Drawing.Point(148, 88);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(35, 16);
			this.radioButton2.TabIndex = 3;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "否";
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// bt_OK
			// 
			this.bt_OK.Location = new System.Drawing.Point(12, 112);
			this.bt_OK.Name = "bt_OK";
			this.bt_OK.Size = new System.Drawing.Size(75, 23);
			this.bt_OK.TabIndex = 4;
			this.bt_OK.Text = "确定";
			this.bt_OK.UseVisualStyleBackColor = true;
			this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
			// 
			// bt_Cancel
			// 
			this.bt_Cancel.Location = new System.Drawing.Point(101, 112);
			this.bt_Cancel.Name = "bt_Cancel";
			this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
			this.bt_Cancel.TabIndex = 5;
			this.bt_Cancel.Text = "取消";
			this.bt_Cancel.UseVisualStyleBackColor = true;
			this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
			// 
			// FormBoxAdd
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(192, 147);
			this.Controls.Add(this.bt_Cancel);
			this.Controls.Add(this.bt_OK);
			this.Controls.Add(this.radioButton2);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.tx_BackBN);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tx_FrontBN);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tx_BoxNO);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "FormBoxAdd";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "添加修改箱格";
			this.Load += new System.EventHandler(this.FormBoxAdd_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bt_OK;
        private System.Windows.Forms.Button bt_Cancel;
        private System.Windows.Forms.TextBox tx_BoxNO;
        private System.Windows.Forms.TextBox tx_FrontBN;
        private System.Windows.Forms.TextBox tx_BackBN;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
    }
}