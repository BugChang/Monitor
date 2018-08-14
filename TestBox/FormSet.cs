#define TWOINONEPAD

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace TestBox
{
    public partial class FormSet : Form
    {
		public int baud, ph1Type, Com3Type, Com1Type, Com2Type, DoorType, ShengJiangJi;
        public FormSet()
        {
            InitializeComponent();
        }


        private void FormSet_Load(object sender, EventArgs e)
        {
			baud = ph1Type = Com3Type = Com1Type = Com2Type = DoorType = ShengJiangJi = 0;
            cb_baud.SelectedIndex = baud;
            comboBox1.SelectedIndex = ph1Type;
			comboBox2.SelectedIndex = Com3Type;
            comboBox3.SelectedIndex = Com1Type;
            comboBox4.SelectedIndex = Com2Type;
            comboBox5.SelectedIndex = DoorType;
            comboBox6.SelectedIndex = ShengJiangJi;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            baud = cb_baud.SelectedIndex;
            ph1Type = comboBox1.SelectedIndex;
			Com3Type = comboBox2.SelectedIndex;
            Com1Type = comboBox3.SelectedIndex;
            Com2Type = comboBox4.SelectedIndex;
            DoorType = comboBox5.SelectedIndex;
            ShengJiangJi = comboBox6.SelectedIndex;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

		private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
#if (!TWOINONEPAD)
			if (comboBox4.SelectedIndex == 3 && (comboBox3.SelectedIndex == 2 || comboBox2.SelectedIndex == 2 || comboBox2.SelectedIndex == 3))
			{
				MessageBox.Show("指静脉和串口摄像头不能安装到同一个M3板上。\r\n一个板子上不能安装两个指静脉或者两个串口摄像头。");
				comboBox4.SelectedIndex = 0;
			}
#endif
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
#if (!TWOINONEPAD)
			if (comboBox3.SelectedIndex == 2 && (comboBox4.SelectedIndex == 3 || comboBox2.SelectedIndex == 2 || comboBox2.SelectedIndex == 3))
            {
				MessageBox.Show("指静脉和串口摄像头不能安装到同一个M3板上。\r\n一个板子上不能安装两个指静脉或者两个串口摄像头。");
				comboBox3.SelectedIndex = 0;
            }
#endif
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
#if (!TWOINONEPAD)
			if ((comboBox2.SelectedIndex == 2 || comboBox2.SelectedIndex == 3) && (comboBox3.SelectedIndex == 2 || comboBox4.SelectedIndex == 3))
			{
				MessageBox.Show("指静脉和串口摄像头不能安装到同一个M3板上。\r\n一个板子上不能安装两个指静脉或者两个串口摄像头。");
				comboBox2.SelectedIndex = 0;
			}
#endif
		}
    }
}