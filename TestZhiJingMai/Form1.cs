using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestZhiJingMai
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		UInt32 hSensor;

		private void bt_Open_Click(object sender, EventArgs e)
		{
			hSensor = DataBase.YNZhiJingMai.ynVeinSensorOpen(0);
			if (hSensor == 0)
				MessageBox.Show("打开错误：" + DataBase.YNZhiJingMai.ynGetLastError().ToString());
		}

		private void bt_Close_Click(object sender, EventArgs e)
		{
			uint i = DataBase.YNZhiJingMai.ynCloseHandle(hSensor);
			if (i == DataBase.YNZhiJingMai.FV_OK)
				hSensor = 0;
			MessageBox.Show("关闭设备：" + i.ToString());
		}
		int iIndex = 0;
		private void bt_Get_Click(object sender, EventArgs e)
		{
			if (DataBase.YNZhiJingMai.ynButton(hSensor, 0, 5000) == DataBase.YNZhiJingMai.FV_OK)
			{
				uint h = DataBase.YNZhiJingMai.ynRegister(hSensor);
				if (h == 0)
				{
					MessageBox.Show("获取信息失败。");
				}
				else
				{
					iIndex++;
					if (DataBase.YNZhiJingMai.ynSaveTemplateFile(h, iIndex.ToString() + ".dat") == DataBase.YNZhiJingMai.FV_OK)
					{
						MessageBox.Show("保存信息成功。");
					}
					else
					{
						MessageBox.Show("保存信息失败。");
					}
					uint i = DataBase.YNZhiJingMai.ynCloseHandle(h);
					if (i != DataBase.YNZhiJingMai.FV_OK)
						MessageBox.Show("关闭获取模板失败。");
				}
			}
			else
			{
				MessageBox.Show("超时退出。");
			}
		}

		List<string> muBan = new List<string>();
		private void bt_MuBan_Click(object sender, EventArgs e)
		{
			muBan.Clear();
			foreach (string f in System.IO.Directory.GetFiles(Application.StartupPath + "\\V1"))
			{
				byte[] buf = System.IO.File.ReadAllBytes(f);
				string str = System.Convert.ToBase64String(buf);
				muBan.Add(str);
			}
			MessageBox.Show("加载数量：" + muBan.Count.ToString());
		}

		private void bt_Search_Click(object sender, EventArgs e)
		{
			OpenFileDialog d = new OpenFileDialog();
			d.Multiselect = false;
			if (d.ShowDialog() == DialogResult.OK)
			{
				byte[] buf = System.IO.File.ReadAllBytes(d.FileName);
				string str = System.Convert.ToBase64String(buf);
				int dt1 = Environment.TickCount;
				string ret = DataBase.YNZhiJingMai.CompareData(muBan, str);
				dt1 = Environment.TickCount - dt1;
				if (ret != "")
				{
					MessageBox.Show("找到。" + dt1.ToString());
				}
				else
				{
					MessageBox.Show("没有找到。" + dt1.ToString());
				}
			}
		}

		private void bt_SearchText_Click(object sender, EventArgs e)
		{
			byte[] buf = System.Convert.FromBase64String(this.textBox1.Text.Trim());
			//string str = Convert.ToBase64String(buf, 0, 4096);
			//str = System.Text.Encoding.ASCII.GetString(buf, 0, 4096);
			string str = this.textBox1.Text.Trim();
			int dt1 = Environment.TickCount;
			string ret = DataBase.YNZhiJingMai.CompareData(muBan, str);
			dt1 = Environment.TickCount - dt1;
			if (ret != "")
			{
				MessageBox.Show("找到。" + dt1.ToString());
			}
			else
			{
				MessageBox.Show("没有找到。" + dt1.ToString());
			}
		}
	}
}