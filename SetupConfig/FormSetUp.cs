using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LogInfo;

namespace SetupConfig
{
    public partial class FormSetUp : Form
    {
        public FormSetUp()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.tx_ServiceURL.Text = Constant.ServiceURL;
			this.tx_ServiceURL_NoLocal.Text = Constant.ServiceURL_NoLocal;
            this.cb_DistributeType.SelectedIndex = Constant.DistributeType;
            cb_NewspaperType.SelectedIndex = Constant.NewspaperType;
            if (Constant.OpenDoorType)
                cb_OpenDoorType.SelectedIndex = 0;
            else
                cb_OpenDoorType.SelectedIndex = 1;

			this.cb_QuJianType.SelectedIndex = Constant.QuJianType - 1;
			this.tx_EmptyBoxName.Text = Constant.EmptyBoxName;

            tx_Left.Text = Constant.TextLeft.ToString();
            tx_Top.Text = Constant.TextTop.ToString();

            rb_NotifyGroupName1.Checked = Constant.NotifyGroupName;
            rb_NotifyGroupName2.Checked = !Constant.NotifyGroupName;
            tx_SysRebootTime.Text = Constant.SysRebootTime.ToString();

			foreach (string s in Enum.GetNames(typeof(enumCardType)))
			{
				this.cb_CardType.Items.Add(s);
			}
			cb_CardType.SelectedIndex = 0;
			Array values = Enum.GetValues(typeof(enumCardType));
			for (int i = 0; i < values.Length; i++)
			{
				if ((int)values.GetValue(i) == Constant.CanType)
					cb_CardType.SelectedIndex = i;
			}
            tx_CanCount.Text = Constant.CanCount.ToString();
			tx_CanNetIP.Text = Constant.CanNetIP;
			tx_CanNetPort.Text = Constant.CanNetPort.ToString();

            TimeOut_SendLetter.Text = Constant.TimeOut_SendLetter.ToString();
            TimeOut_PutInLetter.Text = Constant.TimeOut_PutInLetter.ToString();
            TimeOut_CloseDoor.Text = Constant.TimeOut_CloseDoor.ToString();
            TimeOut_ShowMessage.Text = Constant.TimeOut_ShowInfoMsg.ToString();
            TimeOut_Connection.Text = Constant.ConnectTimeOut.ToString();
			TimeOut_CloseScreen.Text = Constant.TimeOut_CloseScreen.ToString();

            SCDCC_IP.Text = Constant.SCDCC_IP;
            SCDCC_Port.Text = Constant.SCDCC_Port.ToString();

            DVRServer_IP.Text = Constant.DVRServer_IP;
            DVRServer_Port.Text = Constant.DVRServer_Port.ToString();

			if (Constant.UserLocalData)
				cb_DataType.SelectedIndex = 0;
			else
				cb_DataType.SelectedIndex = 1;
			if (Constant.UserLocalBoxConfig)
				cb_UserLocalBoxConfig.SelectedIndex = 0;
			else
				cb_UserLocalBoxConfig.SelectedIndex = 1;

			cb_UserGetLetterType.SelectedIndex = Constant.UserGetLetterType;

			#region 提示信息
			int index = 0, LineHeight = 27, wDivider = 6;
			foreach (LogInfo.NotifyMsg.msgInfo msg in LogInfo.NotifyMsg.MsgArray.Values)
			{
				Label lb = new Label();
				lb.AutoSize = true;
				lb.Location = new Point(4, 6 + index * LineHeight);
				lb.Text = msg.Comment+"：";
				panel2.Controls.Add(lb);

				TextBox tb = new TextBox();
				tb.Location = new Point(panel2.Width - 227, 3 + index * LineHeight);
				tb.Size = new Size(224, 21);
				tb.Text = msg.Sound;
				tb.Name = "s" + msg.id.ToString();
				tb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				panel2.Controls.Add(tb);

				tb = new TextBox();
				tb.Location = new Point(4 + lb.Width + wDivider, 3 + index * LineHeight);
				tb.Size = new Size(panel2.Width - 227 - 4 - lb.Width - wDivider * 2, 21);
				tb.Text = msg.Text;
				tb.Name = "t" + msg.id.ToString();
				tb.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
				panel2.Controls.Add(tb);

				index++;
			}
			#endregion
        }

        private void bt_Cancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bt_SaveAndClose_Click(object sender, EventArgs e)
        {
            bt_Save_Click(sender, e);
            this.Close();
        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            Constant.ServiceURL = this.tx_ServiceURL.Text;
			Constant.ServiceURL_NoLocal = this.tx_ServiceURL_NoLocal.Text;
            Constant.DistributeType = this.cb_DistributeType.SelectedIndex;
            Constant.NewspaperType = cb_NewspaperType.SelectedIndex;
            Constant.OpenDoorType = cb_OpenDoorType.SelectedIndex == 0;
			Constant.QuJianType = this.cb_QuJianType.SelectedIndex + 1;
			Constant.EmptyBoxName = this.tx_EmptyBoxName.Text.Trim();


            Constant.TextLeft = Convert.ToInt32(tx_Left.Text);
            Constant.TextTop = Convert.ToInt32(tx_Top.Text);

            Constant.NotifyGroupName = rb_NotifyGroupName1.Checked;
            Constant.SysRebootTime = Convert.ToInt32(tx_SysRebootTime.Text);

			Array values = Enum.GetValues(typeof(enumCardType));
			Constant.CanType = (int)values.GetValue(cb_CardType.SelectedIndex);
            Constant.CanCount = Convert.ToInt32(tx_CanCount.Text);
			Constant.CanNetIP = tx_CanNetIP.Text.Trim();
			Constant.CanNetPort = Convert.ToInt32(tx_CanNetPort.Text);

            Constant.TimeOut_SendLetter = Convert.ToDouble(TimeOut_SendLetter.Text);
            Constant.TimeOut_PutInLetter = Convert.ToDouble(TimeOut_PutInLetter.Text);
            Constant.TimeOut_CloseDoor = Convert.ToDouble(TimeOut_CloseDoor.Text);
            Constant.TimeOut_ShowInfoMsg = Convert.ToDouble(TimeOut_ShowMessage.Text);
            Constant.ConnectTimeOut = Convert.ToDouble(TimeOut_Connection.Text);
			Constant.TimeOut_CloseScreen = Convert.ToInt32(TimeOut_CloseScreen.Text);

            Constant.SCDCC_IP = SCDCC_IP.Text;
            Constant.SCDCC_Port = Convert.ToInt32(SCDCC_Port.Text);

            Constant.DVRServer_IP = DVRServer_IP.Text;
            Constant.DVRServer_Port = Convert.ToInt32(DVRServer_Port.Text);

			Constant.UserLocalData = cb_DataType.SelectedIndex == 0;
			Constant.UserLocalBoxConfig = this.cb_UserLocalBoxConfig.SelectedIndex == 0;

			Constant.UserGetLetterType = cb_UserGetLetterType.SelectedIndex;

			//提示信息
			foreach (LogInfo.NotifyMsg.msgInfo msg in LogInfo.NotifyMsg.MsgArray.Values)
			{
				string str = msg.id.ToString();
				if (panel2.Controls.ContainsKey("t" + str))
				{
					TextBox tb = (TextBox)panel2.Controls["t" + str];
					NotifyMsg.SetText(msg.id, tb.Text);
				}
				if (panel2.Controls.ContainsKey("s" + str))
				{
					TextBox tb = (TextBox)panel2.Controls["s" + str];
					NotifyMsg.SetSound(msg.id, tb.Text);
				}
			}

            MessageBox.Show("保存成功。\r\n您需要重新启动监控程序，所做的配置才能生效。");
        }

        private void text_TextChanged(object sender, EventArgs e)
        {
            TextBox tx = (TextBox)sender;
            try
            {
                short i = Convert.ToInt16(tx.Text);
            }
            catch
            {
                tx.Text = "0";
                MessageBox.Show("您输入的值有错误，请输入正确数字。");
            }
        }

        private void text_Double_TextChanged(object sender, EventArgs e)
        {
            TextBox tx = (TextBox)sender;
            try
            {
                double i = Convert.ToDouble(tx.Text);
            }
            catch
            {
                tx.Text = "0";
                MessageBox.Show("您输入的值有错误，请输入正确数字。");
            }
        }

        private void SCDCC_IP_TextChanged(object sender, EventArgs e)
        {
            try
            {
                System.Net.IPAddress ip = System.Net.IPAddress.Parse(SCDCC_IP.Text);
            }
            catch
            {
                SCDCC_IP.Text = "127.0.0.1";
                MessageBox.Show("您输入的IP地址有错误，请输入正确的地址。");
            }
        }

        private void bt_BoxInfo_Click(object sender, EventArgs e)
        {
            BoxSetUp frm = new BoxSetUp();
            frm.ShowDialog(this);
        }

		private void bt_GroupInfo_Click(object sender, EventArgs e)
		{
			GroupSetUp frm = new GroupSetUp();
			frm.ShowDialog(this);
		}

		private void cb_DataType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cb_DataType.SelectedIndex == 1)
			{
				cb_UserLocalBoxConfig.SelectedIndex = 0;
				cb_UserLocalBoxConfig.Enabled = false;
			}
			else
				cb_UserLocalBoxConfig.Enabled = true;
		}

    }
}