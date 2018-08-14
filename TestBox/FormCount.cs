using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestBox
{
    public partial class FormCount : Form
    {
		public string WenBenString;
		public int iCount;

        public FormCount()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
			WenBenString = textBox2.Text.Trim();
            int i;
            if (Int32.TryParse(textBox1.Text, out i))
                iCount = i;
            this.Close();
        }

        private void FormCount_Load(object sender, EventArgs e)
        {
            iCount = 16;
        }
    }
}