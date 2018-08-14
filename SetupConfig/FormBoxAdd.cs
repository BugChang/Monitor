using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SetupConfig
{
    public partial class FormBoxAdd : Form
    {
        public int BoxNO;
        public string FrontBN;
        public string BackBN;
        public bool HasTwoLock;

        public FormBoxAdd()
        {
            this.Text = "添加箱格";
            this.BoxNO = 0;
            InitializeComponent();
        }

        public FormBoxAdd(int BoxNo, string FrontBN, string BackBN, bool HasTwoLock) : this()
        {
            this.Text = "修改箱格";
            this.BoxNO = BoxNo;
            this.FrontBN = FrontBN;
            this.BackBN = BackBN;
            this.HasTwoLock = HasTwoLock;
        }

        private void FormBoxAdd_Load(object sender, EventArgs e)
        {
            if (BoxNO > 0)
            {
                tx_BoxNO.Text = BoxNO.ToString();
                tx_BoxNO.Enabled = false;
                tx_FrontBN.Text = FrontBN;
                tx_BackBN.Text = BackBN;
                if (HasTwoLock)
                    radioButton1.Checked = true;
                else
                    radioButton2.Checked = true;
            }
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
                BoxNO = Convert.ToInt32(tx_BoxNO.Text);
                FrontBN = tx_FrontBN.Text.Trim().Substring(0, 7);
                if (tx_BackBN.Text.Trim().Length >= 7)
                    BackBN = tx_BackBN.Text.Trim().Substring(0, 7);
                else
                    BackBN = "";
                HasTwoLock = radioButton1.Checked;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {
                MessageBox.Show("您输入的数据有误，请检查后输入。");
            }
        }

    }
}