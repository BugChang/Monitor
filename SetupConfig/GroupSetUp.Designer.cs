namespace SetupConfig
{
	partial class GroupSetUp
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
			this.bt_Del = new System.Windows.Forms.Button();
			this.bt_Close = new System.Windows.Forms.Button();
			this.bt_Save = new System.Windows.Forms.Button();
			this.dataSet1 = new System.Data.DataSet();
			this.dataTable1 = new System.Data.DataTable();
			this.dataColumn1 = new System.Data.DataColumn();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.箱组名称DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.前面扫描头bn号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.前面读卡器bn号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.前面多功能屏bn号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.前面语音BN号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.前面指纹仪BN号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.前面串口摄像头BN号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.后面扫描头bn号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.后面读卡器bn号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.后面多功能屏bn号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.后面语音BN号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.后面指纹仪BN号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.后面串口摄像头BN号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.本组逻辑箱号列表DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.代管的组列表 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.清单打印机 = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.dataSetBox1 = new LogInfo.DataSetBox();
			((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataSetBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// bt_Del
			// 
			this.bt_Del.Location = new System.Drawing.Point(166, 6);
			this.bt_Del.Name = "bt_Del";
			this.bt_Del.Size = new System.Drawing.Size(75, 23);
			this.bt_Del.TabIndex = 1;
			this.bt_Del.Text = "删除箱组";
			this.bt_Del.UseVisualStyleBackColor = true;
			this.bt_Del.Click += new System.EventHandler(this.bt_Del_Click);
			// 
			// bt_Close
			// 
			this.bt_Close.Location = new System.Drawing.Point(639, 6);
			this.bt_Close.Name = "bt_Close";
			this.bt_Close.Size = new System.Drawing.Size(75, 23);
			this.bt_Close.TabIndex = 1;
			this.bt_Close.Text = "关    闭";
			this.bt_Close.UseVisualStyleBackColor = true;
			this.bt_Close.Click += new System.EventHandler(this.bt_Close_Click);
			// 
			// bt_Save
			// 
			this.bt_Save.Location = new System.Drawing.Point(558, 6);
			this.bt_Save.Name = "bt_Save";
			this.bt_Save.Size = new System.Drawing.Size(75, 23);
			this.bt_Save.TabIndex = 1;
			this.bt_Save.Text = "保存关闭";
			this.bt_Save.UseVisualStyleBackColor = true;
			this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
			// 
			// dataSet1
			// 
			this.dataSet1.DataSetName = "NewDataSet";
			this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
			// 
			// dataTable1
			// 
			this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1});
			this.dataTable1.TableName = "Table1";
			// 
			// dataColumn1
			// 
			this.dataColumn1.ColumnName = "Column1";
			this.dataColumn1.DefaultValue = "";
			// 
			// dataGridView1
			// 
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.AutoGenerateColumns = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.箱组名称DataGridViewTextBoxColumn,
            this.前面扫描头bn号DataGridViewTextBoxColumn,
            this.前面读卡器bn号DataGridViewTextBoxColumn,
            this.前面多功能屏bn号DataGridViewTextBoxColumn,
            this.前面语音BN号DataGridViewTextBoxColumn,
            this.前面指纹仪BN号DataGridViewTextBoxColumn,
            this.前面串口摄像头BN号DataGridViewTextBoxColumn,
            this.后面扫描头bn号DataGridViewTextBoxColumn,
            this.后面读卡器bn号DataGridViewTextBoxColumn,
            this.后面多功能屏bn号DataGridViewTextBoxColumn,
            this.后面语音BN号DataGridViewTextBoxColumn,
            this.后面指纹仪BN号DataGridViewTextBoxColumn,
            this.后面串口摄像头BN号DataGridViewTextBoxColumn,
            this.本组逻辑箱号列表DataGridViewTextBoxColumn,
            this.代管的组列表,
            this.清单打印机});
			this.dataGridView1.DataMember = "GroupInfo";
			this.dataGridView1.DataSource = this.dataSetBox1;
			this.dataGridView1.Location = new System.Drawing.Point(4, 35);
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 23;
			this.dataGridView1.Size = new System.Drawing.Size(705, 326);
			this.dataGridView1.TabIndex = 0;
			// 
			// 箱组名称DataGridViewTextBoxColumn
			// 
			this.箱组名称DataGridViewTextBoxColumn.DataPropertyName = "箱组名称";
			this.箱组名称DataGridViewTextBoxColumn.HeaderText = "箱组名称";
			this.箱组名称DataGridViewTextBoxColumn.Name = "箱组名称DataGridViewTextBoxColumn";
			this.箱组名称DataGridViewTextBoxColumn.Width = 80;
			// 
			// 前面扫描头bn号DataGridViewTextBoxColumn
			// 
			this.前面扫描头bn号DataGridViewTextBoxColumn.DataPropertyName = "前面扫描头bn号";
			this.前面扫描头bn号DataGridViewTextBoxColumn.HeaderText = "前面扫描头bn号";
			this.前面扫描头bn号DataGridViewTextBoxColumn.Name = "前面扫描头bn号DataGridViewTextBoxColumn";
			this.前面扫描头bn号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 前面读卡器bn号DataGridViewTextBoxColumn
			// 
			this.前面读卡器bn号DataGridViewTextBoxColumn.DataPropertyName = "前面读卡器bn号";
			this.前面读卡器bn号DataGridViewTextBoxColumn.HeaderText = "前面读卡器bn号";
			this.前面读卡器bn号DataGridViewTextBoxColumn.Name = "前面读卡器bn号DataGridViewTextBoxColumn";
			this.前面读卡器bn号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 前面多功能屏bn号DataGridViewTextBoxColumn
			// 
			this.前面多功能屏bn号DataGridViewTextBoxColumn.DataPropertyName = "前面多功能屏bn号";
			this.前面多功能屏bn号DataGridViewTextBoxColumn.HeaderText = "前面多功能屏bn号";
			this.前面多功能屏bn号DataGridViewTextBoxColumn.Name = "前面多功能屏bn号DataGridViewTextBoxColumn";
			this.前面多功能屏bn号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 前面语音BN号DataGridViewTextBoxColumn
			// 
			this.前面语音BN号DataGridViewTextBoxColumn.DataPropertyName = "前面语音BN号";
			this.前面语音BN号DataGridViewTextBoxColumn.HeaderText = "前面语音BN号";
			this.前面语音BN号DataGridViewTextBoxColumn.Name = "前面语音BN号DataGridViewTextBoxColumn";
			this.前面语音BN号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 前面指纹仪BN号DataGridViewTextBoxColumn
			// 
			this.前面指纹仪BN号DataGridViewTextBoxColumn.DataPropertyName = "前面指纹仪BN号";
			this.前面指纹仪BN号DataGridViewTextBoxColumn.HeaderText = "前面指纹仪BN号";
			this.前面指纹仪BN号DataGridViewTextBoxColumn.Name = "前面指纹仪BN号DataGridViewTextBoxColumn";
			this.前面指纹仪BN号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 前面串口摄像头BN号DataGridViewTextBoxColumn
			// 
			this.前面串口摄像头BN号DataGridViewTextBoxColumn.DataPropertyName = "前面串口摄像头BN号";
			this.前面串口摄像头BN号DataGridViewTextBoxColumn.HeaderText = "前面串口摄像头BN号";
			this.前面串口摄像头BN号DataGridViewTextBoxColumn.Name = "前面串口摄像头BN号DataGridViewTextBoxColumn";
			this.前面串口摄像头BN号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 后面扫描头bn号DataGridViewTextBoxColumn
			// 
			this.后面扫描头bn号DataGridViewTextBoxColumn.DataPropertyName = "后面扫描头bn号";
			this.后面扫描头bn号DataGridViewTextBoxColumn.HeaderText = "后面扫描头bn号";
			this.后面扫描头bn号DataGridViewTextBoxColumn.Name = "后面扫描头bn号DataGridViewTextBoxColumn";
			this.后面扫描头bn号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 后面读卡器bn号DataGridViewTextBoxColumn
			// 
			this.后面读卡器bn号DataGridViewTextBoxColumn.DataPropertyName = "后面读卡器bn号";
			this.后面读卡器bn号DataGridViewTextBoxColumn.HeaderText = "后面读卡器bn号";
			this.后面读卡器bn号DataGridViewTextBoxColumn.Name = "后面读卡器bn号DataGridViewTextBoxColumn";
			this.后面读卡器bn号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 后面多功能屏bn号DataGridViewTextBoxColumn
			// 
			this.后面多功能屏bn号DataGridViewTextBoxColumn.DataPropertyName = "后面多功能屏bn号";
			this.后面多功能屏bn号DataGridViewTextBoxColumn.HeaderText = "后面多功能屏bn号";
			this.后面多功能屏bn号DataGridViewTextBoxColumn.Name = "后面多功能屏bn号DataGridViewTextBoxColumn";
			this.后面多功能屏bn号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 后面语音BN号DataGridViewTextBoxColumn
			// 
			this.后面语音BN号DataGridViewTextBoxColumn.DataPropertyName = "后面语音BN号";
			this.后面语音BN号DataGridViewTextBoxColumn.HeaderText = "后面语音BN号";
			this.后面语音BN号DataGridViewTextBoxColumn.Name = "后面语音BN号DataGridViewTextBoxColumn";
			this.后面语音BN号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 后面指纹仪BN号DataGridViewTextBoxColumn
			// 
			this.后面指纹仪BN号DataGridViewTextBoxColumn.DataPropertyName = "后面指纹仪BN号";
			this.后面指纹仪BN号DataGridViewTextBoxColumn.HeaderText = "后面指纹仪BN号";
			this.后面指纹仪BN号DataGridViewTextBoxColumn.Name = "后面指纹仪BN号DataGridViewTextBoxColumn";
			this.后面指纹仪BN号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 后面串口摄像头BN号DataGridViewTextBoxColumn
			// 
			this.后面串口摄像头BN号DataGridViewTextBoxColumn.DataPropertyName = "后面串口摄像头BN号";
			this.后面串口摄像头BN号DataGridViewTextBoxColumn.HeaderText = "后面串口摄像头BN号";
			this.后面串口摄像头BN号DataGridViewTextBoxColumn.Name = "后面串口摄像头BN号DataGridViewTextBoxColumn";
			this.后面串口摄像头BN号DataGridViewTextBoxColumn.Width = 70;
			// 
			// 本组逻辑箱号列表DataGridViewTextBoxColumn
			// 
			this.本组逻辑箱号列表DataGridViewTextBoxColumn.DataPropertyName = "本组逻辑箱号列表";
			this.本组逻辑箱号列表DataGridViewTextBoxColumn.HeaderText = "本组逻辑箱号列表";
			this.本组逻辑箱号列表DataGridViewTextBoxColumn.Name = "本组逻辑箱号列表DataGridViewTextBoxColumn";
			this.本组逻辑箱号列表DataGridViewTextBoxColumn.Width = 160;
			// 
			// 代管的组列表
			// 
			this.代管的组列表.DataPropertyName = "代管的组列表";
			this.代管的组列表.HeaderText = "取件柜代管的组列表（空为全部组）";
			this.代管的组列表.Name = "代管的组列表";
			this.代管的组列表.Width = 200;
			// 
			// 清单打印机
			// 
			this.清单打印机.DataPropertyName = "清单打印机";
			this.清单打印机.DataSource = this.dataSet1;
			this.清单打印机.DisplayMember = "Table1.Column1";
			this.清单打印机.HeaderText = "清单打印机";
			this.清单打印机.Name = "清单打印机";
			this.清单打印机.ValueMember = "Table1.Column1";
			this.清单打印机.Width = 200;
			// 
			// dataSetBox1
			// 
			this.dataSetBox1.DataSetName = "DataSetBox";
			this.dataSetBox1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// GroupSetUp
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(711, 365);
			this.Controls.Add(this.bt_Save);
			this.Controls.Add(this.bt_Close);
			this.Controls.Add(this.bt_Del);
			this.Controls.Add(this.dataGridView1);
			this.Name = "GroupSetUp";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "箱格配置信息";
			this.Load += new System.EventHandler(this.BoxSetUp_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataSetBox1)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
		private LogInfo.DataSetBox dataSetBox1;
		private System.Windows.Forms.Button bt_Del;
        private System.Windows.Forms.Button bt_Close;
		private System.Windows.Forms.Button bt_Save;
		private System.Data.DataSet dataSet1;
		private System.Data.DataTable dataTable1;
		private System.Data.DataColumn dataColumn1;
		private System.Windows.Forms.DataGridViewTextBoxColumn 箱组名称DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 前面扫描头bn号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 前面读卡器bn号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 前面多功能屏bn号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 前面语音BN号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 前面指纹仪BN号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 前面串口摄像头BN号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 后面扫描头bn号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 后面读卡器bn号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 后面多功能屏bn号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 后面语音BN号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 后面指纹仪BN号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 后面串口摄像头BN号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 本组逻辑箱号列表DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 代管的组列表;
		private System.Windows.Forms.DataGridViewComboBoxColumn 清单打印机;

    }
}