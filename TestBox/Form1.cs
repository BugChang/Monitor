using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestBox
{
    public partial class Form1 : Form
    {
		bool bInBllTest, bInKeyOpenDoor;
        private class GroupStatus
        {
            public bool m_ChuFaGD;
            public bool m_Idle;
            public int m_CurrentDT;

            public GroupStatus()
            {
                m_ChuFaGD = false;
                m_Idle = true;
            }
        }

        public Form1()
        {
			bInBllTest = false;
			bInKeyOpenDoor = false;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ListViewItem it = lv_All.Items.Add("BN00101");
            //it.SubItems.Add("");
            //it = lv_All.Items.Add("BN00102");
            //it.SubItems.Add("");
            //it = lv_All.Items.Add("BN00103");
            //it.SubItems.Add("连接");
            //it = lv_All.Items.Add("BN00198");
            //it.SubItems.Add("连接");
            //it = lv_All.Items.Add("BN00200");
            //it.SubItems.Add("连接");
            //it = lv_All.Items.Add("BN00300");
            //it.SubItems.Add("连接");
            //it = lv_All.Items.Add("BN00401");
            //it.SubItems.Add("连接");
            //it = lv_All.Items.Add("BN00402");
            //it.SubItems.Add("连接");
            //it = lv_All.Items.Add("BN00403");
            //it.SubItems.Add("连接");
            //it = lv_All.Items.Add("BN00498");
            //it.SubItems.Add("连接");
            //it = lv_All.Items.Add("BN00500");
            //it.SubItems.Add("连接");

            //foreach (ListViewItem it1 in lv_All.Items)
            //    it1.Tag = Environment.TickCount;

            BoxDataParse.DataParse.OnReceiveBoxData += new BoxDataParse.d_ReceiveBoxData(DataParse_OnReceiveBoxData);
            bool b = BoxDataParse.DataParse.Start();
            if (!b)
                MessageBox.Show("Can总线连接错误。");
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            BoxDataParse.DataParse.Stop();
        }

        Dictionary<string, int> m_BoxCount = new Dictionary<string, int>();
        Dictionary<string, GroupStatus> m_Group = new Dictionary<string, GroupStatus>();
		void DataParse_OnReceiveBoxData(string BN_NO, LogInfo.ReceiveDataType iType, string data, bool bFront)
        {
            if (this.InvokeRequired)
            {
                BoxDataParse.d_ReceiveBoxData d = new BoxDataParse.d_ReceiveBoxData(DataParse_OnReceiveBoxData);
                this.Invoke(d, new object[] { BN_NO, iType, data, bFront });
                return;
            }

			if (iType != LogInfo.ReceiveDataType.主箱连接 && iType != LogInfo.ReceiveDataType.主箱连接_Version)
            {
                string gBN = BN_NO.Substring(0, 5);
                SetLogText(BN_NO + "\t" + iType.ToString() + "\t外侧：" + bFront.ToString() + "\t" + data);
				if (bInBllTest)
                {
                    #region 业务测试
                    if (iType == LogInfo.ReceiveDataType.光电 && (BN_NO.Substring(5) == "00" || BN_NO.Substring(5) == "98"))
                    {
                        if (!m_Group.ContainsKey(gBN))
                        {
                            GroupStatus g = new GroupStatus();
                            m_Group.Add(gBN, g);
                        }
                        if (data == "触发光电遮挡")
                        {
                            if (!m_Group[gBN].m_ChuFaGD && m_Group[gBN].m_Idle && Environment.TickCount - m_Group[gBN].m_CurrentDT > 5000)
                            {
                                BoxDataParse.DataParse.CmdBoxScan(BN_NO, true, bFront);
                                m_Group[gBN].m_CurrentDT = Environment.TickCount;
                            }
                            m_Group[gBN].m_ChuFaGD = true;
                        }
                        else if (data == "触发光电未遮挡")
                        {
                            m_Group[gBN].m_ChuFaGD = false;
                        }
                    }
                    else if (iType == LogInfo.ReceiveDataType.条码信息 && data != "")
                    {
                        #region 条码信息
                        BoxDataParse.DataParse.CmdBoxScan(BN_NO, false, bFront);
                        if (BN_NO.Substring(5) == "00")
                        {
                            int count = 0;
                            if (m_BoxCount.ContainsKey(BN_NO))
                            {
                                count = m_BoxCount[BN_NO];
                            }
                            else
                            {
                                m_BoxCount.Add(BN_NO, count);
                            }
                            BoxDataParse.DataParse.CmdPreGetLetter(BN_NO, 1, 5, count, bFront);
                            if (!m_Group.ContainsKey(gBN))
                            {
                                GroupStatus gs = new GroupStatus();
                                m_Group.Add(gBN, gs);
                            }
                            m_Group[gBN].m_Idle = false;
                            m_Group[gBN].m_ChuFaGD = false;
                        }
                        else
                        {
                            foreach (ListViewItem it in lv_Select.Items)
                            {
                                if (it.Text.Substring(0, 5) == BN_NO.Substring(0, 5) && it.Text.Substring(5) != "98")
                                {
                                    int count = 0;
                                    if (m_BoxCount.ContainsKey(BN_NO))
                                    {
                                        count = m_BoxCount[BN_NO];
                                    }
                                    else
                                    {
                                        m_BoxCount.Add(BN_NO, count);
                                    }
                                    BoxDataParse.DataParse.CmdPreGetLetter(it.Text, 0, 5, count, bFront);
                                    if (!m_Group.ContainsKey(gBN))
                                    {
                                        GroupStatus gs = new GroupStatus();
                                        m_Group.Add(gBN,gs);
                                    }
                                    m_Group[gBN].m_Idle = false;
                                    m_Group[gBN].m_ChuFaGD = false;
                                }
                            }
                        }
                        #endregion
                    }
					else if (iType == LogInfo.ReceiveDataType.投件投入)
                    {
                        #region 投件投入
                        int count = 1;
                        if (m_BoxCount.ContainsKey(BN_NO))
                        {
                            m_BoxCount[BN_NO]++;
                            count = m_BoxCount[BN_NO];
                        }
                        else
                        {
                            m_BoxCount.Add(BN_NO, count);
                        }
                        if (m_Group.ContainsKey(gBN))
                            m_Group[gBN].m_Idle = true;

                        foreach (ListViewItem it in lv_Select.Items)
                        {
                            if (it.Text.Substring(0, 5) == gBN)
                            {
                                count = 0;
                                if (m_BoxCount.ContainsKey(it.Text))
                                {
                                    count = m_BoxCount[it.Text];
                                }
                                BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, count, LogInfo.enum_LedColor.绿色, bFront);
                                BoxDataParse.DataParse.CmdBoxToIdle(it.Text, bFront);
                            }
                        }
                        #endregion
                    }
					else if (iType == LogInfo.ReceiveDataType.投件抽出)
                    {
                        #region 投件抽出
                        if (m_Group.ContainsKey(gBN))
                            m_Group[gBN].m_Idle = true;

                        foreach (ListViewItem it in lv_Select.Items)
                        {
                            if (it.Text.Substring(0, 5) == gBN)
                            {
                                int count = 0;
                                if (m_BoxCount.ContainsKey(it.Text))
                                {
                                    count = m_BoxCount[it.Text];
                                }
                                BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, count, LogInfo.enum_LedColor.绿色, bFront);
                                BoxDataParse.DataParse.CmdBoxToIdle(it.Text, bFront);
                            }
                        }
                        #endregion
                    }
					else if (iType == LogInfo.ReceiveDataType.证卡信息 && data != "")
                    {
                        #region 证卡信息
                        foreach (ListViewItem it in lv_Select.Items)
                        {
                            if (it.Text.Substring(0, 5) == gBN && it.Text.Substring(5) != "98")
                            {
                                int count = 0;
                                if (m_BoxCount.ContainsKey(it.Text))
                                {
                                    m_BoxCount[it.Text] = 0;
                                }
                                else
                                {
                                    m_BoxCount.Add(it.Text, count);
                                }
                                BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, count, LogInfo.enum_LedColor.绿色, bFront);
                                BoxDataParse.DataParse.CmdOpenDoor(it.Text, bFront);
                                System.Threading.Thread.Sleep(310);
                            }
                        }
                        #endregion
                    }
					else if (iType == LogInfo.ReceiveDataType.门锁)
                    {
                        #region 门锁
                        int count = 0;
                        if (m_BoxCount.ContainsKey(BN_NO))
                        {
                            count = m_BoxCount[BN_NO];
                        }
                        else
                        {
                            m_BoxCount.Add(BN_NO, count);
                        }
                        BoxDataParse.DataParse.CmdBoxLED(BN_NO, 1, count, LogInfo.enum_LedColor.绿色, bFront);
                        #endregion
                    }
					else if (iType == LogInfo.ReceiveDataType.门禁)
                    {
                        #region 门禁
                        if (data == "Closed")
                        {
                            if (m_Group.ContainsKey(gBN))
                            {
                                //m_Group[gBN].m_Idle = true;
                                if (m_Group[gBN].m_ChuFaGD)
                                {
                                    BoxDataParse.DataParse.CmdBoxScan(BN_NO, true, bFront);
                                    m_Group[gBN].m_CurrentDT = Environment.TickCount;
                                }
                                else
                                    m_Group[gBN].m_ChuFaGD = false;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
				if (bInKeyOpenDoor)
				{
					if (iType == LogInfo.ReceiveDataType.按键)
					{
						BoxDataParse.DataParse.CmdOpenDoor(BN_NO, bFront);
					}
				}
            }
			else if (iType == LogInfo.ReceiveDataType.主箱连接)
            {
                if (data == "Start")
                {
                    //初次连接
                    SetLogText(BN_NO + "\t主箱初次连接。");
                }
                #region 主箱连接
                ListViewItem it = null;
                if (!lv_All.Items.ContainsKey(BN_NO))
                {
                    int ibn = Int32.Parse(BN_NO.Substring(2));
                    for (int j = 0; j < lv_All.Items.Count; j++)
                    {
                        string bn = lv_All.Items[j].Text;
                        if (Int32.Parse(bn.Substring(2)) > ibn)
                        {
                            it = lv_All.Items.Insert(j, BN_NO, BN_NO, 0);
                            it.SubItems.Add("断开");
                            break;
                        }
                    }
                    if (it == null)
                    {
                        it = lv_All.Items.Insert(lv_All.Items.Count, BN_NO, BN_NO, 0);
                        it.SubItems.Add("断开");
                    }
                }
                else
                {
                    it = lv_All.Items[BN_NO];
                    if (data == "Start")
                        it.SubItems[1].Text = "断开";
                }
                it.Tag = Environment.TickCount;
                //单位名称
                lock (it)
                {
                    if (it.SubItems[1].Text == "断开")
                    {
                        //重新连接
                        SetLogText(BN_NO + "\t主箱重新连接。");

                        BoxDataParse.DataParse.CmdBoxToIdle(BN_NO);
                        BoxDataParse.DataParse.CmdBoxToIdle(BN_NO, false);
                        BoxDataParse.DataParse.CmdLCDText(it.Text, 0, LogInfo.enum_TextType.显示附带的文本, 0, 255, BN_NO.Substring(2));
                        BoxDataParse.DataParse.CmdLCDText(it.Text, 0, LogInfo.enum_TextType.显示附带的文本, 0, 255, BN_NO.Substring(2), false);
                        if (BN_NO.Substring(5) == "98")
                        {
                            BoxDataParse.DataParse.CmdLCDText_Com(it.Text, 0, LogInfo.enum_TextType.显示附带的文本, 0, 255, 0, 0, BN_NO.Substring(2) + "\r\n请扫描条码...");
                        }
                        BoxDataParse.DataParse.CmdBoxLED(BN_NO, 1, 0, LogInfo.enum_LedColor.绿色);
                        BoxDataParse.DataParse.CmdBoxLED(BN_NO, 1, 0, LogInfo.enum_LedColor.绿色, false);
                        it.SubItems[1].Text = "";
                    }
                }
                #endregion
            }
			else if (iType == LogInfo.ReceiveDataType.主箱连接_Version)
            {
                #region 主箱版本
                ListViewItem it = null;
                if (!lv_All.Items.ContainsKey(BN_NO))
                {
                    int ibn = Int32.Parse(BN_NO.Substring(2));
                    for (int j = 0; j < lv_All.Items.Count; j++)
                    {
                        string bn = lv_All.Items[j].Text;
                        if (Int32.Parse(bn.Substring(2)) > ibn)
                        {
                            it = lv_All.Items.Insert(j, BN_NO, BN_NO, 0);
                            it.SubItems.Add("断开");
                            break;
                        }
                    }
                    if (it == null)
                    {
                        it = lv_All.Items.Insert(lv_All.Items.Count, BN_NO, BN_NO, 0);
                        it.SubItems.Add("断开");
                    }
                }
                else
                {
                    it = lv_All.Items[BN_NO];
                }
                try
                {
                    it.SubItems[2].Text = data;
                }
                catch
                {
                    it.SubItems.Add(data);
                }
                #endregion
            }
        }
        delegate void d_SetLogText(string str);
        void SetLogText(string str)
        {
            if (tx_State.InvokeRequired)
            {
                d_SetLogText d = new d_SetLogText(SetLogText);
                tx_State.Invoke(d, new object[] { str });
            }
            else
            {
                if (tx_State.Text.Length > 2048 && ck_ClearLog.Checked)
                    tx_State.Text = "";
                tx_State.Text = DateTime.Now.ToString("MM-dd HH:mm:ss.fff") + ": " + str + "\r\n" + tx_State.Text;
            }
        }


        #region 添加删除BN号码
        private void bt_Add_Click(object sender, EventArgs e)
        {
            if (lv_All.SelectedItems.Count ==0)
            {
                MessageBox.Show("请选择您要添加的BN号码，再点击添加");
                return;
            }
            for (int j = 0; j < lv_All.SelectedItems.Count; j++)
            {
                string bn = lv_All.SelectedItems[j].Text;
                int ibn = Int32.Parse(bn.Substring(2));
                for (int i = 0; i < lv_Select.Items.Count; i++)
                {
                    if (lv_Select.Items[i].Text == bn)
                        return;
                    if (Int32.Parse(lv_Select.Items[i].Text.Substring(2)) > ibn)
                    {
                        lv_Select.Items.Insert(i, bn);
                        return;
                    }
                }
                lv_Select.Items.Add(bn);
            }
        }

        private void bt_AddAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lv_All.Items.Count; i++)
            {
                lv_All.Items[i].Selected = true;
                bt_Add_Click(sender, e);
                lv_All.Items[i].Selected = false;
            }
        }

        private void bt_Remove_Click(object sender, EventArgs e)
        {
            if (lv_Select.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择您要移除的BN号码，再点击移除");
                return;
            }
            lv_Select.Items.Remove(lv_Select.SelectedItems[0]);
        }

        private void bt_RemoveAll_Click(object sender, EventArgs e)
        {
            lv_Select.Items.Clear();
        }

        private void bt_AddAAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lv_All.Items.Count; i++)
            {
                if (lv_All.Items[i].Text.Substring(5) == "00")
                {
                    lv_All.Items[i].Selected = true;
                    bt_Add_Click(sender, e);
                    lv_All.Items[i].Selected = false;
                }
            }
        }

        private void bt_AddBfAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lv_All.Items.Count; i++)
            {
                if (lv_All.Items[i].Text.Substring(5) != "00" && lv_All.Items[i].Text.Substring(5) != "98")
                {
                    lv_All.Items[i].Selected = true;
                    bt_Add_Click(sender, e);
                    lv_All.Items[i].Selected = false;
                }
            }
        }

        private void bt_AddBzAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lv_All.Items.Count; i++)
            {
                if (lv_All.Items[i].Text.Substring(5) == "98")
                {
                    lv_All.Items[i].Selected = true;
                    bt_Add_Click(sender, e);
                    lv_All.Items[i].Selected = false;
                }
            }
        }

        private void lv_All_DoubleClick(object sender, EventArgs e)
        {
            bt_Add_Click(sender, e);
        }

        private void lv_Select_DoubleClick(object sender, EventArgs e)
        {
            bt_Remove_Click(sender, e);
        }
        #endregion

        #region 发送命令
        private void bt_Send_Click(object sender, EventArgs e)
        {
            if (lv_Command.SelectedItems.Count <= 0)
                return;
            Button bt = (Button)sender;
            bool bFront = true;
            if (bt.Text == "发送内侧命令")
                bFront = false;
            string str = lv_Command.SelectedItems[0].Text;

            if (bt_SendLoop.Equals(sender))
            {
                if (bt.Text != "停止")
                {
                    bSendCommand = true;
                    CommandTypeString = str;
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(SendLoop));
                    bt.Text = "停止";
                }
                else
                {
                    bSendCommand = false;
                    bt.Text = "循环发送命令";
                }
            }
            else
                SendCommand(bFront, str);
        }

        private bool bSendCommand;
        private string CommandTypeString;
        private void SendLoop(object o)
        {
            while (bSendCommand)
            {
                SendCommand(true, CommandTypeString);
                System.Threading.Thread.Sleep(100);
            }
        }

        private delegate void d_SendCommand(bool bFront, string str);
        private void SendCommand(bool bFront, string str)
        {
            if (this.InvokeRequired)
            {
                d_SendCommand d = new d_SendCommand(SendCommand);
                this.Invoke(d, new object[] { bFront, str });
                return;
            }
            switch (str)
            {
                case "准备接受投信信息(不开门禁)":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdPreGetLetter(it.Text, 0, 2, 123, bFront);
                    }
                    break;

                case "准备接受投信信息(打开门禁)":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdPreGetLetter(it.Text, 1, 2, 321, bFront);
                    }
                    break;

                case "指示灯灭":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxLampByBN(it.Text, 15, LogInfo.enum_LampStatus.灭, bFront);
                    }
                    break;

                case "指示灯亮":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
						BoxDataParse.DataParse.CmdBoxLampByBN(it.Text, 15, LogInfo.enum_LampStatus.亮, bFront);
                    }
                    break;

                case "指示灯闪烁":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
						BoxDataParse.DataParse.CmdBoxLampByBN(it.Text, 15, LogInfo.enum_LampStatus.闪烁, bFront);
                    }
                    break;

                case "数码管关闭":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, 888, LogInfo.enum_LedColor.关闭, bFront);
                    }
                    break;

                case "数码管888绿色":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, 888, LogInfo.enum_LedColor.绿色, bFront);
                    }
                    break;

                case "数码管888红色":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, 888, LogInfo.enum_LedColor.红色, bFront);
                    }
                    break;

                case "数码管888橙色":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, 888, LogInfo.enum_LedColor.橙色, bFront);
                    }
                    break;

                case "数码管888绿黑闪烁":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, 888, LogInfo.enum_LedColor.绿黑闪烁, bFront);
                    }
                    break;

                case "数码管888红黑闪烁":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, 888, LogInfo.enum_LedColor.红黑闪烁, bFront);
                    }
                    break;

                case "数码管888橙黑闪烁":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, 888, LogInfo.enum_LedColor.橙黑闪烁, bFront);
                    }
                    break;

                case "数码管888红绿闪烁":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, 888, LogInfo.enum_LedColor.红绿闪烁, bFront);
                    }
                    break;

                case "数码管888橙绿闪烁":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxLED(it.Text, 1, 888, LogInfo.enum_LedColor.橙绿闪烁, bFront);
                    }
                    break;

                case "关门禁":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxGating(it.Text, false, bFront);
                    }
                    break;

                case "开门禁":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxGating(it.Text, true, bFront);
                    }
                    break;

                case "开门锁":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdOpenDoor(it.Text, bFront);
                        System.Threading.Thread.Sleep(310);
                    }
                    break;


                case "查询硬件状态":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdGetBoxState(it.Text, bFront);
                    }
                    break;

                case "交换箱立即软启动":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdResetBox(it.Text, bFront);
                    }
                    break;

                case "强制箱格进入Idle状态":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(it.Text, bFront);
                    }
                    break;

                case "关闭屏幕":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdScreen(it.Text, false, bFront);
                    }
                    break;

                case "打开屏幕":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdScreen(it.Text, true, bFront);
                    }
                    break;

                case "关闭扫描头":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxScan(it.Text, false, bFront);
                    }
                    break;

                case "打开扫描头":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBoxScan(it.Text, true, bFront);
                    }
                    break;

                case "拍照":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
						BoxDataParse.DataParse.CmdTakePhoto(it.Text, 1, bFront);
						System.Threading.Thread.Sleep(200);
						BoxDataParse.DataParse.CmdTakePhoto(it.Text, 2, bFront);
                    }
                    break;

                case "显示屏幕按钮(模板普发)":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdScreenButton(it.Text, true, true, true, false, bFront);
                    }
                    break;

                case "显示屏幕按钮(模板)":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdScreenButton(it.Text, false, true, true, false, bFront);
                    }
                    break;

                case "显示屏幕按钮(普发)":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdScreenButton(it.Text, true, false, true, false, bFront);
                    }
                    break;

                case "显示屏幕按钮(退出)":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdScreenButton(it.Text, false, false, true, false, bFront);
                    }
                    break;

                case "显示屏幕按钮(确认取消)":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdScreenButton(it.Text, false, false, true, true, bFront);
                    }
                    break;

                case "模板列表显示":
                    {
                        FormCount frm = new FormCount();
                        frm.ShowDialog(this);
                        string[] ls = new string[frm.iCount];
                        for (int i = 0; i < ls.Length; i++)
                        {
                            ls[i] = frm.WenBenString + i.ToString();
                        }
                        foreach (ListViewItem it in lv_Select.Items)
                        {
                            BoxDataParse.DataParse.CmdTempletList(it.Text, ls, bFront);
                        }
                    }
                    break;

                case "设置单位名称":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        if (it.Text.Substring(5) == "98")
                            BoxDataParse.DataParse.CmdLCDText_Com(it.Text, 0, LogInfo.enum_TextType.显示附带的文本, 0, 255, 0, 0, tx_WenBen.Text + "\r\n请扫描条码...");
                        else
                        {
                            BoxDataParse.DataParse.CmdLCDText(it.Text, 0, LogInfo.enum_TextType.显示附带的文本, 0, 255, tx_WenBen.Text, bFront);
                            BoxDataParse.DataParse.CmdLCDText(it.Text, 2, LogInfo.enum_TextType.显示附带的文本, 0, 255, "", bFront);
                        }
                    }
                    break;

                case "取件平台-等待扫卡界面":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdQuJian(it.Text, 0, bFront);
                    }
                    break;
                case "取件平台-三按钮取件界面":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdQuJian(it.Text, 1, bFront);
                    }
                    break;
                case "取件平台-四按钮取件界面":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdQuJian(it.Text, 2, bFront);
                    }
                    break;

                case "显示文本1":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdLCDText(it.Text, 3, LogInfo.enum_TextType.显示附带的文本, 0, 255, "远光通联科技有限公司，欢迎您使用公文交换箱格。", bFront);
                        //BoxDataParse.DataParse.CmdLCDText_4Dai(it.Text, 3, 5, "远光通联科技有限公司，\r\n欢迎您使用公文交换箱格。");
                    }
                    break;

                case "显示文本2":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdLCDText(it.Text, 3, LogInfo.enum_TextType.显示附带的文本, 0, 5, tx_WenBen.Text, bFront);
                    }
                    break;

                case "显示文本1(串口屏)":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdLCDText_Com(it.Text, 3, LogInfo.enum_TextType.显示附带的文本, 0, 255, 0, 0, "条码没有预发，  条码没有预发\r\n请做预发登记后投箱。\r\n若需要自动匹配模板或普发，若需要自动匹配模板\r\n请按键选择。   请按键选择。");
                    }
                    break;

                case "显示文本2(串口屏)":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdLCDText_Com(it.Text, 3, LogInfo.enum_TextType.显示附带的文本, 0, 3, 0, 0, tx_WenBen.Text);
                    }
                    break;

                case "语音提示1":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdSound(it.Text, LogInfo.enum_TextType.显示附带的文本, 0, "远光通联", bFront);
                    }
                    break;

                case "语音提示2":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdSound(it.Text, LogInfo.enum_TextType.显示附带的文本, 0, "欢迎使用", bFront);
                    }
                    break;

                case "蜂鸣器1声":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBuzzer(it.Text, 1, bFront);
                    }
                    break;
                case "蜂鸣器3声":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBuzzer(it.Text, 2, bFront);
                    }
                    break;
                case "蜂鸣器长响":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBuzzer(it.Text, 3, bFront);
                    }
                    break;
                case "蜂鸣器关闭":
                    foreach (ListViewItem it in lv_Select.Items)
                    {
                        BoxDataParse.DataParse.CmdBuzzer(it.Text, 0, bFront);
                    }
                    break;

                case "硬件设置":
                    {
                        FormSet frm = new FormSet();
                        if (frm.ShowDialog(this) == DialogResult.OK)
                        {
                            foreach (ListViewItem it in lv_Select.Items)
                            {
                                BoxDataParse.DataParse.CmdBoxSet(it.Text, frm.baud, frm.ph1Type, frm.Com3Type, frm.Com1Type, frm.Com2Type, frm.DoorType, frm.ShengJiangJi, bFront);
                            }
                        }
                    }
                    break;
            }
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem it in lv_All.Items)
                {
                    int t = Environment.TickCount - (int)it.Tag;
                    if (t > 20000 && it.SubItems[1].Text != "断开")
                    {
                        it.SubItems[1].Text = "断开";
                        //主箱断开
                        SetLogText( it.Text + "\t主箱断开。");
                    }
                }
                //foreach (GroupStatus g in m_Group.Values)
                //{
                //    if (Environment.TickCount - g.m_CurrentDT > 4000)
                //        g.m_ChuFaGD = false;
                //}
            }
            catch
            {
            }
        }

        private void bt_Clear_Click(object sender, EventArgs e)
        {
            tx_State.Text = "";
        }

        private void bt_TouXiang_Click(object sender, EventArgs e)
        {
            if (bt_TouXiang.Text == "开始业务测试")
            {
                bt_TouXiang.Text = "结束业务测试";
				bInBllTest = true;
            }
            else
            {
				bInBllTest = false;
                bt_TouXiang.Text = "开始业务测试";
                foreach (ListViewItem it in lv_Select.Items)
                {
                    BoxDataParse.DataParse.CmdBoxToIdle(it.Text);
                }
            }
        }


        private void lv_Command_DoubleClick(object sender, EventArgs e)
        {
            bt_Send_Click(bt_Send, e);
        }

		private void bt_AnJianKaimen_Click(object sender, EventArgs e)
		{
			if (bt_AnJianKaimen.Text == "开始按键开门")
			{
				bInKeyOpenDoor = true;
				bt_AnJianKaimen.Text = "结束按键开门";
			}
			else
			{
				bInKeyOpenDoor = false;
				bt_AnJianKaimen.Text = "开始按键开门";
			}
		}

    }
}