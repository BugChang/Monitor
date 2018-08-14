using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 同步指静脉数据
{
	public partial class FormMain : Form
	{
		/// <summary>
		/// -1:无设置，0:错误，1:正确
		/// </summary>
		int iState;

		public FormMain()
		{
			InitializeComponent();
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			groupBox1.Enabled = true;
			groupBox2.Enabled = false;
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			groupBox1.Enabled = false;
			groupBox2.Enabled = true;
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			if (!BoxDataParse.DataParse.Start())
			{
				MessageBox.Show("请连接正确的Usb Can卡。");
				//this.Close();
			}
			BoxDataParse.DataParse.OnReceiveBoxData += new BoxDataParse.d_ReceiveBoxData(DataParse_OnReceiveBoxData);
		}

		void DataParse_OnReceiveBoxData(string BN_NO, LogInfo.ReceiveDataType iType, string data, bool bFront)
		{
			if (iType == LogInfo.ReceiveDataType.指静脉数据_传输)
			{
				SetLog(BN_NO + "，" + iType.ToString() + "，" + data);
				if (data == "当前设备已经准备好接受指静脉模板数据")
				{
					iState = 1;
				}
				else if (data == "本次数据传输验证成功")
				{
					iState = 1;
				}
				else if (data == "error")
				{
					iState = 0;
				}
			}
			if (iType == LogInfo.ReceiveDataType.指静脉数据_写入)
			{
				SetLog(BN_NO + "，" + iType.ToString() + "，" + data);
				if (data == "写特征数据成功")
				{
					iState = 1;
				}
				else if (data == "写特征数据失败")
				{
					iState = 0;
				}
			}
			if (iType == LogInfo.ReceiveDataType.指静脉数据_验证)
			{
				SetLog(BN_NO + "，指静脉数据_验证，UserId:" + data);
			}
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			BoxDataParse.DataParse.Stop();
		}

		private void bt_Close_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void bt_View_Click(object sender, EventArgs e)
		{
			OpenFileDialog d = new OpenFileDialog();
			d.Multiselect = false;
			if (d.ShowDialog() == DialogResult.OK)
			{
				tx_xgd.Text = d.FileName;
			}
		}

		private delegate void d_SetLog(TextBox tb, string msg);
		private void SetLog(string msg)
		{
			SetLog_Tb (tx_Msg, msg);
		}

		private void SetLog_Tb(TextBox tb, string msg)
		{
			if (tb.InvokeRequired)
			{
				d_SetLog d = new d_SetLog(SetLog_Tb);
				tb.Invoke(d, new object[] { tb, msg });
			}
			else
			{
				tb.Text = "接收时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t"
					+ msg + "\r\n\r\n" + tb.Text;
				if (tb.Text.Length > 2048)
				{
					tx_Msg.Text = tx_Msg.Text.Substring(0, 1024);
				}
			}
		}

		string fName;
		int UserId;
		string BN_NO;
		string szConnectString, szSelectSql;
		private void bt_Update_Click(object sender, EventArgs e)
		{
			int bn = 0;
			if (!Int32.TryParse(tx_BN.Text, out bn))
			{
				MessageBox.Show("请输入正确的bn号码。");
				tx_BN.Focus();
				return;
			}
			if (bn <= 0)
			{
				MessageBox.Show("请输入正确的bn号码。");
				tx_BN.Focus();
				return;
			}
			BN_NO = "BN" + bn.ToString("00000");

			if (radioButton1.Checked)
			{
				if (!System.IO.File.Exists(tx_xgd.Text))
				{
					MessageBox.Show("请选择正确的特征文件。");
					return;
				}
				fName = tx_xgd.Text.Trim();
				UserId = 0;
				if (!Int32.TryParse(tx_UserId.Text, out UserId))
				{
					MessageBox.Show("请输入大于0小于500的位置。");
					tx_UserId.Focus();
					return;
				}
				if (UserId <= 0)
				{
					MessageBox.Show("请输入大于0小于500的位置。");
					tx_UserId.Focus();
					return;
				}
				
				System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(SendOneData), 0);
			}
			else
			{
				szConnectString = tx_ConnectString.Text.Trim();
				szSelectSql = tx_SelectSql.Text.Trim();
				System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(SendAllData), 0);
			}
		}

		void SendOneData(object o)
		{
			byte[] buf = System.IO.File.ReadAllBytes(fName);
			string bn_no = BN_NO;

			SendData(bn_no, UserId, buf, buf.Length);

			BoxDataParse.DataParse.CmdUpdateZhiJingMai(BN_NO, 3, UserId, buf, buf.Length);
			SetLog("结束传输指静脉数据。");
		}

		unsafe void SendAllData(object o)
		{
			string bn_no = BN_NO;

			System.Data.SqlClient.SqlConnection Conn = new System.Data.SqlClient.SqlConnection();
			System.Data.SqlClient.SqlDataReader Rs = null;
			try
			{
				Conn.ConnectionString = szConnectString;
				Conn.Open();

				System.Data.SqlClient.SqlCommand Comm = Conn.CreateCommand();
				Comm.CommandText = szSelectSql;
				Rs = Comm.ExecuteReader();
				while (Rs.Read())
				{
					int iUserId = Convert.ToInt32(Rs[0]);
					string xgd = Rs[1].ToString();
					if (xgd.Length % 4 != 0)
					{
						int ilen = xgd.Length - (xgd.Length % 4);
						xgd = xgd.Substring(0, ilen);
					}
					byte[] buf = Convert.FromBase64String(xgd);
					byte[] bufnew = new byte[8192];
					int len =0;
					fixed(byte* pbuf=buf, pbufnew=bufnew)
					{
						IntPtr p = (IntPtr)pbuf;
						IntPtr pnew = (IntPtr)pbufnew;
						len = DataBase.XGComApi.XG_DecodeEnrollData(p, pnew);
					}
					if (len > 0)
					{
						SendData(bn_no, iUserId, bufnew, len);
						System.Threading.Thread.Sleep(10);
					}
				}
			}
			catch (Exception ex)
			{
			}
			finally
			{
				if (Rs != null)
				{
					Rs.Close();
					Rs.Dispose();
				}
				if (Conn.State != ConnectionState.Closed)
				{
					Conn.Close();
				}
				Conn.Dispose();
			}

			BoxDataParse.DataParse.CmdUpdateZhiJingMai(BN_NO, 3, 0, new byte[0], 0);
			SetLog("结束传输指静脉数据。");
		}

		private bool SendData(string bn_no, int iUserId, byte[] buf, int buflen)
		{
			iState = -1;
			int dt = Environment.TickCount + 10000;
			BoxDataParse.DataParse.CmdUpdateZhiJingMai(bn_no, 1, iUserId, buf, buflen);
			while (iState == -1 && Environment.TickCount < dt)
				System.Threading.Thread.Sleep(20);
			if (iState == 1)
				SetLog("准备传输指静脉数据，成功。");
			else
			{
				SetLog("准备传输指静脉数据，失败。");
				return false;
			}

			BoxDataParse.DataParse.CmdTransZhiJingMai(bn_no, buf, buflen);
			SetLog("数据发送完毕。");
			BoxDataParse.DataParse.CmdUpdateZhiJingMai(bn_no, 2, iUserId, buf, buflen);
			iState = -1;
			dt = Environment.TickCount + 10000;
			while (iState == -1 && Environment.TickCount < dt)
				System.Threading.Thread.Sleep(20);
			if (iState == 1)
				SetLog(iUserId.ToString() + ", 本次数据传输验证，成功。");
			else
			{
				SetLog(iUserId.ToString() + ", 本次数据传输验证，失败。");
				return false;
			}

			iState = -1;
			dt = Environment.TickCount + 10000;
			while (iState == -1 && Environment.TickCount < dt)
				System.Threading.Thread.Sleep(20);
			if (iState == 1)
			{
				SetLog(iUserId.ToString() + ", 写特征数据，成功。");
				SetLog_Tb(tx_Msg1, iUserId.ToString() + ", 写特征数据，成功。");
			}
			else
			{
				SetLog(iUserId.ToString() + ", 写特征数据，失败。");
				SetLog_Tb(tx_Msg1, iUserId.ToString() + ", 写特征数据，失败。");
				return false;
			}
			return false;
		}

		private unsafe void bt_TestDecode_Click(object sender, EventArgs e)
		{
			string[] files = System.IO.Directory.GetFiles(Application.StartupPath + "\\指静脉数据\\加密压缩指静脉数据");
			int i=1;
			foreach (string f in files)
			{
				byte[] buf = Convert.FromBase64String(System.IO.File.ReadAllText(f));
				byte[] bufnew = new byte[8192];
				int len = 0;
				fixed (byte* pbuf = buf, pbufnew = bufnew)
				{
					IntPtr p = (IntPtr)pbuf;
					IntPtr pnew = (IntPtr)pbufnew;
					len = DataBase.XGComApi.XG_DecodeEnrollData(p, pnew);
				}
				if (len > 0)
				{
					byte[] buf1 = new byte[len];
					Array.Copy(bufnew, buf1, len);
					System.IO.File.WriteAllBytes(Application.StartupPath + "\\指静脉数据\\" + i.ToString(), buf1);
					i++;
				}
			}
		}


	}
}