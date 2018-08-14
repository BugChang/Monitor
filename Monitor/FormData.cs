using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Monitor
{
    public partial class FormData : Form
    {
        DataBase.LocalDataSet m_ds;
        public FormData()
        {
            InitializeComponent();
        }

        private void FormData_Load(object sender, EventArgs e)
        {
            m_ds = DataBase.DataSave.LocalData;
            foreach (DataTable dt in m_ds.Tables)
            {
                comboBox1.Items.Add(dt.TableName);
            }
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString().CompareTo("") == 0) return;
            this.dataGridView1.DataSource = m_ds;
            this.dataGridView1.DataMember = comboBox1.SelectedItem.ToString();
        }
    }
}