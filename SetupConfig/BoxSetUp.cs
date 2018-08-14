using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SetupConfig
{
    public partial class BoxSetUp : Form
    {
        public BoxSetUp()
        {
            InitializeComponent();
        }

        private void bt_Add_Click(object sender, EventArgs e)
        {
            FormBoxAdd frm = new FormBoxAdd();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                LogInfo.DataSetBox.BoxInfoRow row = dataSetBox1.BoxInfo.NewBoxInfoRow();
                row.�߼���� = frm.BoxNO;
                row.��������bn�� = frm.FrontBN;
                row.���䱳��bn�� = frm.BackBN;
                row.˫���� = frm.HasTwoLock;
                dataSetBox1.BoxInfo.AddBoxInfoRow(row);
            }
        }

        private void BoxSetUp_Load(object sender, EventArgs e)
        {
			if (System.IO.File.Exists(LogInfo.Constant.BoxConfigFileName))
			{
				dataSetBox1.ReadXml(LogInfo.Constant.BoxConfigFileName);
				dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
			}
        }

        private void bt_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            dataSetBox1.WriteXml(LogInfo.Constant.BoxConfigFileName);
            this.Close();
		}

		private void dataGridView1_DoubleClick(object sender, EventArgs e)
		{
			bt_Chg_Click(sender, e);
			
		}

		private void bt_Chg_Click(object sender, EventArgs e)
		{
			DataGridView dg = dataGridView1;
			if (dg.CurrentRow ==null) return;

			System.Data.DataRowView rv = (System.Data.DataRowView)dg.CurrentRow.DataBoundItem;
			if (rv == null) return;
			LogInfo.DataSetBox.BoxInfoRow row = (LogInfo.DataSetBox.BoxInfoRow)rv.Row;
			FormBoxAdd frm = new FormBoxAdd( row.�߼����, row.��������bn��, row.���䱳��bn��, row.˫����);
			if (frm.ShowDialog(this) == DialogResult.OK)
			{
				row.�߼���� = frm.BoxNO;
				row.��������bn�� = frm.FrontBN;
				row.���䱳��bn�� = frm.BackBN;
				row.˫���� = frm.HasTwoLock;
			}

		}

		private void bt_Del_Click(object sender, EventArgs e)
		{
			DataGridView dg = dataGridView1;
			if (dg.CurrentRow == null) return;

			System.Data.DataRowView rv = (System.Data.DataRowView)dg.CurrentRow.DataBoundItem;
			dataSetBox1.BoxInfo.Rows.Remove(rv.Row);
		}

		private void bt_AddGroup_Click(object sender, EventArgs e)
		{
			FormBoxAddPi frm = new FormBoxAddPi();
			if (frm.ShowDialog(this) == DialogResult.OK)
			{
				for (int i = frm.StartG; i <= frm.EndG; i++)
				{
					if (frm.IsAgroup)
					{
						LogInfo.DataSetBox.BoxInfoRow row = dataSetBox1.BoxInfo.NewBoxInfoRow();
						row.�߼���� = frm.StartBoxNO + i-frm.StartG;
						row.��������bn�� = "BN" + i.ToString("000") + "00";
						if (frm.StartBG == 0)
							row.���䱳��bn�� = "";
						else
							row.���䱳��bn�� = "BN" + (frm.StartBG+i-frm.StartG).ToString("000") + "00";
						row.˫���� = frm.HasTwoLock;
						dataSetBox1.BoxInfo.AddBoxInfoRow(row);
					}
					else
					{
						for (int j = 1; j <= frm.GroupBoxCount; j++)
						{
							LogInfo.DataSetBox.BoxInfoRow row = dataSetBox1.BoxInfo.NewBoxInfoRow();
							row.�߼���� = frm.StartBoxNO + (i - frm.StartG) * frm.GroupBoxCount + j - 1;
							row.��������bn�� = "BN" + i.ToString("000") + j.ToString("00");
							if (frm.StartBG == 0)
								row.���䱳��bn�� = "";
							else
								row.���䱳��bn�� = "BN" + (frm.StartBG + i - frm.StartG).ToString("000") + j.ToString("00");
							row.˫���� = frm.HasTwoLock;
							dataSetBox1.BoxInfo.AddBoxInfoRow(row);
						}
					}
				}
			}
		}
    }
}