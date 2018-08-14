namespace SetupConfig
{
    partial class BoxSetUp
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
			this.bt_Add = new System.Windows.Forms.Button();
			this.bt_AddGroup = new System.Windows.Forms.Button();
			this.bt_Del = new System.Windows.Forms.Button();
			this.bt_Chg = new System.Windows.Forms.Button();
			this.bt_Close = new System.Windows.Forms.Button();
			this.bt_Save = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.dataSetBox1 = new LogInfo.DataSetBox();
			this.逻辑箱号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.分箱正面bn号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.分箱背面bn号DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.双面锁DataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataSetBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// bt_Add
			// 
			this.bt_Add.Location = new System.Drawing.Point(4, 6);
			this.bt_Add.Name = "bt_Add";
			this.bt_Add.Size = new System.Drawing.Size(75, 23);
			this.bt_Add.TabIndex = 1;
			this.bt_Add.Text = "添加箱格";
			this.bt_Add.UseVisualStyleBackColor = true;
			this.bt_Add.Click += new System.EventHandler(this.bt_Add_Click);
			// 
			// bt_AddGroup
			// 
			this.bt_AddGroup.Location = new System.Drawing.Point(85, 6);
			this.bt_AddGroup.Name = "bt_AddGroup";
			this.bt_AddGroup.Size = new System.Drawing.Size(107, 23);
			this.bt_AddGroup.TabIndex = 1;
			this.bt_AddGroup.Text = "批量添加箱格";
			this.bt_AddGroup.UseVisualStyleBackColor = true;
			this.bt_AddGroup.Click += new System.EventHandler(this.bt_AddGroup_Click);
			// 
			// bt_Del
			// 
			this.bt_Del.Location = new System.Drawing.Point(302, 6);
			this.bt_Del.Name = "bt_Del";
			this.bt_Del.Size = new System.Drawing.Size(75, 23);
			this.bt_Del.TabIndex = 1;
			this.bt_Del.Text = "删除箱格";
			this.bt_Del.UseVisualStyleBackColor = true;
			this.bt_Del.Click += new System.EventHandler(this.bt_Del_Click);
			// 
			// bt_Chg
			// 
			this.bt_Chg.Location = new System.Drawing.Point(210, 6);
			this.bt_Chg.Name = "bt_Chg";
			this.bt_Chg.Size = new System.Drawing.Size(75, 23);
			this.bt_Chg.TabIndex = 1;
			this.bt_Chg.Text = "修改箱格";
			this.bt_Chg.UseVisualStyleBackColor = true;
			this.bt_Chg.Click += new System.EventHandler(this.bt_Chg_Click);
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
			// dataGridView1
			// 
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.AutoGenerateColumns = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.逻辑箱号DataGridViewTextBoxColumn,
            this.分箱正面bn号DataGridViewTextBoxColumn,
            this.分箱背面bn号DataGridViewTextBoxColumn,
            this.双面锁DataGridViewCheckBoxColumn});
			this.dataGridView1.DataMember = "BoxInfo";
			this.dataGridView1.DataSource = this.dataSetBox1;
			this.dataGridView1.Location = new System.Drawing.Point(4, 35);
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 23;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(710, 326);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.DoubleClick += new System.EventHandler(this.dataGridView1_DoubleClick);
			// 
			// dataSetBox1
			// 
			this.dataSetBox1.DataSetName = "DataSetBox";
			this.dataSetBox1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// 逻辑箱号DataGridViewTextBoxColumn
			// 
			this.逻辑箱号DataGridViewTextBoxColumn.DataPropertyName = "逻辑箱号";
			this.逻辑箱号DataGridViewTextBoxColumn.HeaderText = "逻辑箱号";
			this.逻辑箱号DataGridViewTextBoxColumn.Name = "逻辑箱号DataGridViewTextBoxColumn";
			this.逻辑箱号DataGridViewTextBoxColumn.Width = 80;
			// 
			// 分箱正面bn号DataGridViewTextBoxColumn
			// 
			this.分箱正面bn号DataGridViewTextBoxColumn.DataPropertyName = "分箱正面bn号";
			this.分箱正面bn号DataGridViewTextBoxColumn.HeaderText = "分箱正面bn号";
			this.分箱正面bn号DataGridViewTextBoxColumn.Name = "分箱正面bn号DataGridViewTextBoxColumn";
			this.分箱正面bn号DataGridViewTextBoxColumn.Width = 150;
			// 
			// 分箱背面bn号DataGridViewTextBoxColumn
			// 
			this.分箱背面bn号DataGridViewTextBoxColumn.DataPropertyName = "分箱背面bn号";
			this.分箱背面bn号DataGridViewTextBoxColumn.HeaderText = "分箱背面bn号";
			this.分箱背面bn号DataGridViewTextBoxColumn.Name = "分箱背面bn号DataGridViewTextBoxColumn";
			this.分箱背面bn号DataGridViewTextBoxColumn.Width = 150;
			// 
			// 双面锁DataGridViewCheckBoxColumn
			// 
			this.双面锁DataGridViewCheckBoxColumn.DataPropertyName = "双面锁";
			this.双面锁DataGridViewCheckBoxColumn.HeaderText = "双面锁";
			this.双面锁DataGridViewCheckBoxColumn.Name = "双面锁DataGridViewCheckBoxColumn";
			this.双面锁DataGridViewCheckBoxColumn.Width = 60;
			// 
			// BoxSetUp
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(716, 365);
			this.Controls.Add(this.bt_Save);
			this.Controls.Add(this.bt_Close);
			this.Controls.Add(this.bt_Del);
			this.Controls.Add(this.bt_AddGroup);
			this.Controls.Add(this.bt_Chg);
			this.Controls.Add(this.bt_Add);
			this.Controls.Add(this.dataGridView1);
			this.Name = "BoxSetUp";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "箱格配置信息";
			this.Load += new System.EventHandler(this.BoxSetUp_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataSetBox1)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
		private LogInfo.DataSetBox dataSetBox1;
        private System.Windows.Forms.Button bt_Add;
        private System.Windows.Forms.Button bt_AddGroup;
        private System.Windows.Forms.Button bt_Del;
        private System.Windows.Forms.Button bt_Chg;
        private System.Windows.Forms.Button bt_Close;
        private System.Windows.Forms.Button bt_Save;
		private System.Windows.Forms.DataGridViewTextBoxColumn 逻辑箱号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 分箱正面bn号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn 分箱背面bn号DataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewCheckBoxColumn 双面锁DataGridViewCheckBoxColumn;

    }
}