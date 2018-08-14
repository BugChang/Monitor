using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SetupConfig
{
    public partial class GroupSetUp : Form
    {
		public GroupSetUp()
        {
            InitializeComponent();
        }


        private void BoxSetUp_Load(object sender, EventArgs e)
        {
			if (System.IO.File.Exists(LogInfo.Constant.BoxConfigFileName))
			{
				dataSetBox1.ReadXml(LogInfo.Constant.BoxConfigFileName);
			}
			try
			{
				foreach (string pName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
				{
					DataRow row = this.dataTable1.NewRow();
					row[0] = pName;
					this.dataTable1.Rows.Add(row);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("获取打印机列表出错：" + ex.ToString());
			}

        }

        private void bt_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
			//检查箱组名称重复
			foreach (LogInfo.DataSetBox.GroupInfoRow row in dataSetBox1.GroupInfo.Rows)
			{
				int count = 0;
				foreach (LogInfo.DataSetBox.GroupInfoRow row1 in dataSetBox1.GroupInfo.Rows)
				{
					if (row.箱组名称 == row1.箱组名称)
						count++;
				}
				if (count > 1)
				{
					MessageBox.Show("“" + row.箱组名称 + "”有多个数据，请检查数据再保存。");
					return;
				}
			}
			//检查数据的完整性
			foreach (LogInfo.DataSetBox.GroupInfoRow row in dataSetBox1.GroupInfo.Rows)
			{
				if (row.前面扫描头bn号.Length != 7 || row.前面多功能屏bn号.Length != 7 || row.前面读卡器bn号.Length != 7
					|| row.前面扫描头bn号.Substring(0, 2) != "BN" || row.前面多功能屏bn号.Substring(0, 2) != "BN" || row.前面读卡器bn号.Substring(0, 2) != "BN")
				{
					MessageBox.Show("“" + row.箱组名称 + "”的数据有误，请检查数据再保存。");
					return;
				}
			}

            dataSetBox1.WriteXml(LogInfo.Constant.BoxConfigFileName);
            this.Close();
		}

		private void bt_Del_Click(object sender, EventArgs e)
		{
			DataGridView dg = dataGridView1;
			if (dg.CurrentRow == null) return;

			System.Data.DataRowView rv = (System.Data.DataRowView)dg.CurrentRow.DataBoundItem;
			dataSetBox1.BoxInfo.Rows.Remove(rv.Row);
		}

    }
}