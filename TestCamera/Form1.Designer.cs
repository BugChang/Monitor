namespace TestCamera
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
            this.label1 = new System.Windows.Forms.Label();
            this.tx_IP = new System.Windows.Forms.TextBox();
            this.tx_Port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bt_Snap = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bt_Save = new System.Windows.Forms.Button();
            this.bt_init = new System.Windows.Forms.Button();
            this.lb_ts = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP地址：";
            // 
            // tx_IP
            // 
            this.tx_IP.Location = new System.Drawing.Point(64, 12);
            this.tx_IP.Name = "tx_IP";
            this.tx_IP.Size = new System.Drawing.Size(100, 21);
            this.tx_IP.TabIndex = 1;
            this.tx_IP.Text = "192.9.100.114";
            // 
            // tx_Port
            // 
            this.tx_Port.Location = new System.Drawing.Point(219, 12);
            this.tx_Port.Name = "tx_Port";
            this.tx_Port.Size = new System.Drawing.Size(65, 21);
            this.tx_Port.TabIndex = 3;
            this.tx_Port.Text = "3084";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(180, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "端口：";
            // 
            // bt_Snap
            // 
            this.bt_Snap.Location = new System.Drawing.Point(290, 10);
            this.bt_Snap.Name = "bt_Snap";
            this.bt_Snap.Size = new System.Drawing.Size(47, 23);
            this.bt_Snap.TabIndex = 4;
            this.bt_Snap.Text = "拍照";
            this.bt_Snap.UseVisualStyleBackColor = true;
            this.bt_Snap.Click += new System.EventHandler(this.bt_Snap_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(14, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(781, 362);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // bt_Save
            // 
            this.bt_Save.Enabled = false;
            this.bt_Save.Location = new System.Drawing.Point(343, 10);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(47, 23);
            this.bt_Save.TabIndex = 4;
            this.bt_Save.Text = "保存";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // bt_init
            // 
            this.bt_init.Location = new System.Drawing.Point(406, 10);
            this.bt_init.Name = "bt_init";
            this.bt_init.Size = new System.Drawing.Size(57, 23);
            this.bt_init.TabIndex = 4;
            this.bt_init.Text = "初始化";
            this.bt_init.UseVisualStyleBackColor = true;
            this.bt_init.Click += new System.EventHandler(this.bt_init_Click);
            // 
            // lb_ts
            // 
            this.lb_ts.AutoSize = true;
            this.lb_ts.Location = new System.Drawing.Point(482, 15);
            this.lb_ts.Name = "lb_ts";
            this.lb_ts.Size = new System.Drawing.Size(41, 12);
            this.lb_ts.TabIndex = 6;
            this.lb_ts.Text = "label3";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 413);
            this.Controls.Add(this.lb_ts);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.bt_init);
            this.Controls.Add(this.bt_Snap);
            this.Controls.Add(this.tx_Port);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tx_IP);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "测试串口摄像头";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tx_IP;
        private System.Windows.Forms.TextBox tx_Port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bt_Snap;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button bt_Save;
        private System.Windows.Forms.Button bt_init;
        private System.Windows.Forms.Label lb_ts;
    }
}

