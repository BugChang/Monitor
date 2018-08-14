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
				MessageBox.Show("��ȡ��ӡ���б����" + ex.ToString());
			}

        }

        private void bt_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
			//������������ظ�
			foreach (LogInfo.DataSetBox.GroupInfoRow row in dataSetBox1.GroupInfo.Rows)
			{
				int count = 0;
				foreach (LogInfo.DataSetBox.GroupInfoRow row1 in dataSetBox1.GroupInfo.Rows)
				{
					if (row.�������� == row1.��������)
						count++;
				}
				if (count > 1)
				{
					MessageBox.Show("��" + row.�������� + "���ж�����ݣ����������ٱ��档");
					return;
				}
			}
			//������ݵ�������
			foreach (LogInfo.DataSetBox.GroupInfoRow row in dataSetBox1.GroupInfo.Rows)
			{
				if (row.ǰ��ɨ��ͷbn��.Length != 7 || row.ǰ��๦����bn��.Length != 7 || row.ǰ�������bn��.Length != 7
					|| row.ǰ��ɨ��ͷbn��.Substring(0, 2) != "BN" || row.ǰ��๦����bn��.Substring(0, 2) != "BN" || row.ǰ�������bn��.Substring(0, 2) != "BN")
				{
					MessageBox.Show("��" + row.�������� + "���������������������ٱ��档");
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