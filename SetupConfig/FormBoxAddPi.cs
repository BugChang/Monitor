using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SetupConfig
{
    public partial class FormBoxAddPi : Form
    {
		public int StartBoxNO;
		public int StartG;
		public int EndG;
		public int StartBG;
		public int GroupBoxCount;
		public bool HasTwoLock;
		public bool IsAgroup;

        public FormBoxAddPi()
        {
            InitializeComponent();
        }

        private void FormBoxAdd_Load(object sender, EventArgs e)
        {
			HasTwoLock = false;
        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void bt_OK_Click(object sender, EventArgs e)
        {
            try
            {
				StartBoxNO = Convert.ToInt32(tx_BoxNo.Text);
				StartG = Convert.ToInt32(tx_StartG.Text);
				EndG = Convert.ToInt32(tx_EndG.Text);
				StartBG = 0;
				if (tx_StartBG.Text.Trim()!="")
					StartBG = Convert.ToInt32(tx_StartBG.Text);
				GroupBoxCount = Convert.ToInt32(tx_BoxCount.Text);
                HasTwoLock = rb_Lock1.Checked;
				IsAgroup = rb_GroupTypeA.Checked;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {
                MessageBox.Show("您输入的数据有误，请检查后输入。");
            }
		}

		private void rb_GroupType_CheckedChanged(object sender, EventArgs e)
		{
			tx_BoxCount.Enabled = rb_GroupTypeB.Checked;
			if (!tx_BoxCount.Enabled)
				tx_BoxCount.Text = "1";
		}

    }
}