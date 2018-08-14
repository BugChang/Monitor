using System;
using System.Collections.Generic;
using System.Xml;
using LogInfo;

namespace DataProcess
{
    /// <summary>
    /// DataProcess 的摘要说明。
    /// </summary>
    public class DataProcess
    {
        class NewsBoxState
        {
            /// <summary>
            /// 逻辑箱号，从1开始
            /// </summary>
            public int BoxNo;
            /// <summary>
            /// 对应的前后设备
            /// </summary>
			public BoxInfo.BoxStatus boxStatus;
            /// <summary>
            /// 报刊数量，显示
            /// </summary>
            public int PaperNum;
            /// <summary>
            /// 是否已经投入报刊
            /// </summary>
            public bool bSendOK;

            public NewsBoxState()
            {
                BoxNo = 0;
                PaperNum = 0;
                bSendOK = false;
            }
        }

        System.Threading.Thread tThread;

        Dictionary<int, BoxInfo> boxstatus = new Dictionary<int, BoxInfo>();
        List<BoxInfo> m_ListBoxstatus = new List<BoxInfo>();
        Dictionary<string, GroupStatus> groupstatus = new Dictionary<string, GroupStatus>();

        /// <summary>
        /// 是否报刊分发状态
        /// </summary>
        bool m_isNewsPaper;
        List<GroupStatus> m_NewsGroup;
        List<NewsBoxState> m_NewsState;


        #region 类初始化函数
        public DataProcess()
        {
            m_isNewsPaper = false;
            ClientDataParse.ClientDataParse.OnReceiveClentData += new ClientDataParse.ReceiveClientData(client_OnReceiveClentData);
            BoxDataParse.DataParse.OnReceiveBoxData += new BoxDataParse.d_ReceiveBoxData(box_listen_OnReceiveBoxData);
            DataBase.DataSave.OnSyncError += new DataBase.d_SyncError(DataSave_OnSyncError);
        }
        #endregion

        #region 启动和关闭监控程序
        public bool Start()
        {
            if (Constant.b_AppRunning)
                return true;

            Constant.b_AppRunning = true;

            //数据连接
            if (!DataBase.DataSave.Start())
                return false;
            Log.WriteFileLog("数据连接OK");

            //初始化箱组状态
            if (!InitBoxStatus())
                goto ErrorHandle;
            Log.WriteFileLog("初始化箱组状态OK");
            if (ClientDataParse.ClientDataParse.Start())
            {
                Log.WriteFileLog("ClientDataParse.ClientDataParse.Start()OK");
                if (BoxDataParse.DataParse.Start())
                {
                    Log.WriteFileLog("BoxDataParse.DataParse.Start()");
                    tThread = new System.Threading.Thread(new System.Threading.ThreadStart(CheckBoxTimeOut));
                    tThread.Start();

                    return true;
                }
                else
                {
                    ClientDataParse.ClientDataParse.Stop();
                }
            }

            ErrorHandle:
            //清除数据
            DataBase.DataSave.Stop();
            groupstatus.Clear();
            boxstatus.Clear();

            Constant.b_AppRunning = false;
            return false;
        }

        public bool Stop()
        {
            if (!Constant.b_AppRunning)
                return true;

            Constant.b_AppRunning = false;
            if (tThread != null)
                tThread.Join();
            ClientDataParse.ClientDataParse.Stop();
            BoxDataParse.DataParse.Stop();

            DataBase.DataSave.Stop();
            //清除数据
            groupstatus.Clear();
            boxstatus.Clear();

            return true;
        }

        #endregion

        #region 同步出错，提示信息
        void DataSave_OnSyncError(int BoxNo, bool bError)
        {
            if (BoxNo == 0)
            {
                if (bError)
                {
                    //网络断开
                    foreach (BoxInfo box in boxstatus.Values)
                    {
                        if (!box.bSyncError)
                        {
                            box.bSyncError = true;
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 255, box.GetSyncErrorMsg);
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 255, box.GetSyncErrorMsg);
                        }
                    }
                }
                else
                {
                    foreach (BoxInfo box in boxstatus.Values)
                    {
                        if (box.bSyncError)
                        {
                            box.bSyncError = false;
                            //清空现有显示
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 0, " ");
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 0, " ");
                        }
                    }
                }
            }
            else
            {
                //单个同步出错，可能接口错误
                if (bError)
                {
                    //网络断开
                    if (boxstatus.ContainsKey(BoxNo))
                    {
                        BoxInfo box = boxstatus[BoxNo];
                        if (!box.bSyncError)
                        {
                            box.bSyncError = true;
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 255, box.GetSyncErrorMsg);
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 255, box.GetSyncErrorMsg);
                        }
                    }
                }
                else
                {
                    if (boxstatus.ContainsKey(BoxNo))
                    {
                        BoxInfo box = boxstatus[BoxNo];
                        if (box.bSyncError)
                        {
                            box.bSyncError = false;
                            //清空现有显示
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 0, " ");
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 0, " ");
                        }
                    }
                }
            }
        }
        #endregion

        #region 监听界面端口
        private string client_OnReceiveClentData(int itype, string data, string data2)
        {
            string Rstr = "error";

            try
            {
                switch (itype)
                {
                    #region 101用户登录信息查询，
                    case 101:
                        {
                            string[] datas = data.Split(':');
                            if (datas.Length == 2)
                            {
                                //查询数据库，判断用户是否可以登录
                                if (DataBase.DataSave_NoLocal.CheckLogin(datas[0], datas[1]))
                                {
                                    Rstr = "ok";
                                }
                            }
                        }
                        break;
                    #endregion

                    #region 102监控客户端界面信息查询，
                    case 102:
                        {
                            Rstr = "";
                            //查询数据库，查询用户的箱子信息
                            string tmpStr = DataBase.DataSave_NoLocal.UserBoxInfo(data);
                            string[] Boxs = tmpStr.Split('\n');
                            for (int i = 0; i < Boxs.Length; i++)
                            {
                                string[] oneBox = Boxs[i].Split('\r');
                                if (oneBox.Length < 5)
                                    continue;
                                int boxNo = Convert.ToInt32(oneBox[0]);
                                if (!boxstatus.ContainsKey(boxNo))
                                    continue;
                                int letterCount = Convert.ToInt32(oneBox[3]);
                                Rstr += oneBox[0] + "\r" + oneBox[1] + "\r" + oneBox[2] + "\r" + letterCount.ToString("000") + "\r" + oneBox[4];
                                //state
                                string state = "";
                                if (boxstatus[boxNo].FrontBox.Door == DoorGateStatus.Closed)
                                    state += "0";
                                else
                                    state += "1";
                                if (boxstatus[boxNo].FrontBox.Gating == DoorGateStatus.Closed)
                                    state += "0";
                                else
                                    state += "1";
                                if (!boxstatus[boxNo].LampYiQu)
                                    state += "0";
                                else
                                    state += "1";
                                if (!boxstatus[boxNo].LampJiJian)
                                    state += "0";
                                else
                                    state += "1";
                                Rstr += "\r" + state;
                                //ConnectState
                                if (boxstatus[boxNo].FrontBox.Connected)
                                    Rstr += "\rtrue";
                                else
                                    Rstr += "\rfalse";
                                //Errstring
                                Rstr += "\r" + boxstatus[boxNo].FrontBox.GetErrorString();
                                Rstr += "\n";
                            }
                        }
                        break;
                    #endregion


                    #region 202用户取件请求（开指定箱门），
                    case 202:
                        {
                            string[] boxs = data.Split(',');
                            for (int iii = 0; iii < boxs.Length; iii++)
                            {
                                try
                                {
                                    int boxid = Convert.ToInt32(boxs[iii]);
                                    if (!boxstatus.ContainsKey(boxid))
                                        break;
                                    string BoxNO = (boxid).ToString("000");

                                    DataBase.DataSave.Box_UserGetLetter(boxid, "", "", true, true, true, true, "", "");

                                    //设置对应箱头的状态
                                    boxstatus[boxid].LampYiQu = true;
                                    boxstatus[boxid].LampJiJian = false;
                                    boxstatus[boxid].LetterCount = 0;

                                    //通知客户端，灯改变
                                    SendClientNotify.NotifyLamp(BoxNO, "1", "on");
                                    //通知客户端，信件为 0
                                    SendClientNotify.NotifyLetter(BoxNO, "false", 0);

                                    //开门 
                                    if (BoxDataParse.DataParse.CmdOpenDoor(boxstatus[boxid].FrontBox.BoxBN, true))
                                    {
                                        Rstr = "ok";
                                    }
                                }
                                catch
                                { }
                            }
                        }
                        break;
                    #endregion

                    #region 203核对信数请求，
                    case 203:
                        {
                            int BoxNo = Convert.ToInt32(data);
                            if (!boxstatus.ContainsKey(BoxNo))
                                break;

                            //读取数据库，得到信件数目和急件灯状态
                            DataBase.MonitorService.BoxInfo t = DataBase.DataSave.GetBoxLetterCount(BoxNo);
                            if (t != null)
                            {
                                boxstatus[BoxNo].LampJiJian = t.HasJiJian;
                                boxstatus[BoxNo].LetterCount = t.SendCount;
                                //本地结构，通知箱头
                                boxstatus[BoxNo].CheckBoxLetter();

                                //通知客户端，改变信件
                                SendClientNotify.NotifyLetter(data, (boxstatus[BoxNo].LampJiJian ? "true" : "false"), boxstatus[BoxNo].LetterCount);
                            }

                            Rstr = "ok";
                        }
                        break;
                    #endregion

                    #region 401开门请求，
                    case 401:
                        {
                            int boxNo = Convert.ToInt32(data);
                            if (boxstatus.ContainsKey(boxNo))
                            {
                                if (Convert.ToBoolean(data2))
                                {
                                    if (BoxDataParse.DataParse.CmdOpenDoor(boxstatus[boxNo].FrontBox.BoxBN, true))
                                    {
                                        Rstr = "ok";
                                    }
                                }
                                else
                                {
                                    if (BoxDataParse.DataParse.CmdOpenDoor(boxstatus[boxNo].BackBox.BoxBN, true))
                                    {
                                        Rstr = "ok";
                                    }
                                }
                            }
                        }
                        break;
                    #endregion

                    #region 402控制灯请求，
                    case 402:
                        {
                            string[] datas = data.Split(':');
                            if (datas.Length == 3)
                            {
                                int boxNo = Convert.ToInt32(datas[0]);
                                if (boxstatus.ContainsKey(boxNo))
                                {
                                    //1：已取灯，2：错误灯，3：急件灯，4：待机灯
                                    if (datas[1] == "1")
                                    {
                                        if (Convert.ToBoolean(data2))
                                        {
                                            if (BoxDataParse.DataParse.CmdBoxLampYiQu(boxstatus[boxNo].FrontBox.BoxBN,
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.亮 : enum_LampStatus.灭))
                                            {
                                                Rstr = "ok";
                                            }
                                        }
                                        else
                                        {
                                            if (BoxDataParse.DataParse.CmdBoxLampYiQu(boxstatus[boxNo].BackBox.BoxBN,
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.亮 : enum_LampStatus.灭))
                                            {
                                                Rstr = "ok";
                                            }
                                        }
                                    }
                                    else if (datas[1] == "2")
                                    {
                                        if (Convert.ToBoolean(data2))
                                        {
                                            if (BoxDataParse.DataParse.CmdBoxLampCuoWu(boxstatus[boxNo].FrontBox.BoxBN,
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.亮 : enum_LampStatus.灭))
                                            {
                                                Rstr = "ok";
                                            }
                                        }
                                        else
                                        {
                                            if (BoxDataParse.DataParse.CmdBoxLampCuoWu(boxstatus[boxNo].BackBox.BoxBN,
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.亮 : enum_LampStatus.灭))
                                            {
                                                Rstr = "ok";
                                            }
                                        }
                                    }
                                    else if (datas[1] == "3")
                                    {
                                        if (Convert.ToBoolean(data2))
                                        {
                                            if (BoxDataParse.DataParse.CmdBoxLampJiJian(boxstatus[boxNo].FrontBox.BoxBN,
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.亮 : enum_LampStatus.灭))
                                            {
                                                Rstr = "ok";
                                            }
                                        }
                                        else
                                        {
                                            if (BoxDataParse.DataParse.CmdBoxLampJiJian(boxstatus[boxNo].BackBox.BoxBN,
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.亮 : enum_LampStatus.灭))
                                            {
                                                Rstr = "ok";
                                            }
                                        }
                                    }
                                    if (Rstr == "ok")
                                    {
                                        //本地结构，改变灯
                                        if (datas[1] == "1")
                                            boxstatus[boxNo].LampYiQu = (datas[2].ToLower().CompareTo("on") == 0);
                                        else if (datas[1] == "3")
                                            boxstatus[boxNo].LampJiJian = (datas[2].ToLower().CompareTo("on") == 0);

                                        //通知客户端，灯改变
                                        if (datas[2].CompareTo("on") != 0)
                                            SendClientNotify.NotifyLamp(datas[0], datas[1], "off");
                                        else
                                            SendClientNotify.NotifyLamp(datas[0], datas[1], "on");
                                    }
                                }
                            }
                        }
                        break;
                    #endregion

                    #region 403开关门禁请求，
                    case 403:
                        {
                            string[] datas = data.Split(':');
                            if (datas.Length == 2)
                            {
                                int boxNo = Convert.ToInt32(datas[0]);
                                if (boxstatus.ContainsKey(boxNo))
                                {
                                    if (Convert.ToBoolean(data2))
                                    {
                                        if (BoxDataParse.DataParse.CmdBoxGating(boxstatus[boxNo].FrontBox.BoxBN, datas[1].ToLower().CompareTo("on") == 0))
                                        {
                                            Rstr = "ok";
                                        }
                                    }
                                    else
                                    {
                                        if (BoxDataParse.DataParse.CmdBoxGating(boxstatus[boxNo].BackBox.BoxBN, datas[1].ToLower().CompareTo("on") == 0))
                                        {
                                            Rstr = "ok";
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    #endregion

                    #region 404箱存浏览请求，
                    case 404:
                        {
                            //查询逻辑箱头的箱存信息
                            Rstr = DataBase.DataSave_NoLocal.BoxLetterView(data);
                        }
                        break;
                    #endregion

                    #region 405箱体分配请求－单位列表，
                    case 405:
                        {
                            //查询单位列表
                            Rstr = DataBase.DataSave_NoLocal.UnitList();
                        }
                        break;
                    #endregion

                    #region 406箱体分配请求－更改单位箱号对应，
                    case 406:
                        {
                            string[] datas = data.Split(':');
                            if (datas.Length == 2)
                            {
                                if (DataBase.DataSave_NoLocal.Box_UnitChange(datas[0], datas[1]))
                                {
                                    int boxNo = Convert.ToInt32(datas[0]);
                                    if (boxstatus.ContainsKey(boxNo))
                                    {
                                        boxstatus[boxNo].InitBoxStatus();
                                    }
                                    Rstr = "ok";
                                }
                            }
                        }
                        break;
                    #endregion

                    #region 407灭所有已取灯请求，
                    case 407:
                        {
                            if (data.CompareTo("on") == 0)
                            {
                                //先重置箱头数组，灯置为亮
                                foreach (BoxInfo bi in boxstatus.Values)
                                {
                                    if (!bi.LampYiQu)
                                    {
                                        bi.LampYiQu = true;
                                        //通知客户端
                                        SendClientNotify.NotifyLamp(bi.BoxNO.ToString("000"), "1", "on");
                                    }
                                }
                            }
                            else
                            {
                                //先重置箱头数组，已取灯置为灭
                                foreach (BoxInfo bi in boxstatus.Values)
                                {
                                    if (bi.LampYiQu)
                                    {
                                        bi.LampYiQu = false;
                                        //通知客户端
                                        SendClientNotify.NotifyLamp(bi.BoxNO.ToString("000"), "1", "off");
                                    }
                                }
                            }
                            //找到所有组，发送控制已取灯命令
                            foreach (GroupStatus gs in groupstatus.Values)
                            {
                                //控制本组的已取灯 on,off
                                foreach (GroupStatus.StBoxInfo stbox in gs.MBoxInfo.Values)
                                {
                                    if (BoxDataParse.DataParse.CmdBoxLampYiQu(stbox.BoxBn,
                                            data.ToLower().CompareTo("on") == 0 ? enum_LampStatus.亮 : enum_LampStatus.灭))
                                    {
                                        Rstr = "ok";
                                    }
                                }
                            }
                            Rstr = "ok";
                        }
                        break;
                    #endregion

                    #region 408开所有箱门
                    case 408:
                        {
                            string[] BoxNOs = data.Split(':');
                            foreach (string BoxNO in BoxNOs)
                            {
                                int boxNo = Convert.ToInt32(BoxNO);
                                if (boxstatus.ContainsKey(boxNo))
                                {
                                    BoxDataParse.DataParse.CmdOpenDoor(boxstatus[boxNo].FrontBox.BoxBN, true);
                                    System.Threading.Thread.Sleep(310);
                                }
                            }
                            Rstr = "ok";
                        }
                        break;
                    #endregion

                    #region 409核对所有信数
                    case 409:
                        {
                            //找到所有箱头，核对信数
                            foreach (BoxInfo bi in boxstatus.Values)
                            {
                                //读取数据库，得到信件数目和急件灯状态
                                DataBase.MonitorService_NoLocal.BoxInfo t = DataBase.DataSave_NoLocal.GetBoxLetterCount(bi.BoxNO);
                                if (t != null)
                                {
                                    bi.LampJiJian = t.HasJiJian;
                                    bi.LetterCount = t.SendCount;
                                    bi.LetterCount_XiangTou = t.SendCount;
                                    bi.BoxUnitName = t.BoxShowName;
                                    bi.BoxUnitFullName = t.BoxShowFullName;
                                    //本地结构，通知箱头
                                    bi.CheckBoxLetter();


                                    //通知客户端，改变信件
                                    SendClientNotify.NotifyLetter(bi.BoxNO.ToString("000"), (bi.LampJiJian ? "true" : "false"), bi.LetterCount);
                                    //发送错误到界面
                                    SendClientNotify.NotifyError(bi.BoxNO.ToString("000"), bi.FrontBox.GetErrorString());
                                    //亮错误灯
                                    BoxDataParse.DataParse.CmdBoxLampCuoWu(bi.FrontBox.BoxBN, (bi.FrontBox.GetErrorString() == "" ? enum_LampStatus.灭 : enum_LampStatus.亮));
                                }
                            }
                            Rstr = "ok";
                        }
                        break;
                    #endregion

                    #region 410重启箱组请求，
                    case 410:
                        {
                            int boxNo = Convert.ToInt32(data);
                            if (boxstatus.ContainsKey(boxNo))
                            {
                                if (BoxDataParse.DataParse.CmdResetBox(boxstatus[boxNo].FrontBox.BoxBN))
                                    Rstr = "ok";
                            }
                        }
                        break;
                    #endregion


                    #region 501 报刊分发通讯－分发开始请求，
                    case 501:
                        {
                            bool bRight = true;
                            string[] datas = data.Split('\n');
                            List<NewsBoxState> tNewsState = new List<NewsBoxState>();
                            List<GroupStatus> tNewsGroup = new List<GroupStatus>();

                            //回复以前的状态
                            //设置这些组的状态退出报刊分发
                            if (m_NewsGroup != null)
                            {
                                foreach (GroupStatus gs in m_NewsGroup)
                                {
                                    //显示亮
                                    BoxDataParse.DataParse.CmdScreen(gs.GroupShowBn, true);
                                    foreach (GroupStatus.StBoxInfo stbox in gs.MBoxInfo.Values)
                                        stbox.BoxStatus.IsCloseScreen = false;
                                }
                            }
                            if (m_NewsState != null)
                            {
                                //回复箱头的状态
                                foreach (NewsBoxState b in m_NewsState)
                                {
                                    b.boxStatus.IsNewspaperDistribute = false;
                                    b.boxStatus.IsCloseScreen = false;
                                    boxstatus[b.BoxNo].CheckBoxLetter();
                                }
                            }

                            //解析接收到的信息
                            for (int ii = 0; ii < datas.Length; ii++)
                            {
                                string[] boxnos = datas[ii].Split('\r');
                                if (boxnos.Length == 2)
                                {
                                    int boxno = Int32.Parse(boxnos[0]);
                                    //boxid出错
                                    if (!boxstatus.ContainsKey(boxno))
                                    {
                                        bRight = false;
                                        break;
                                    }
                                    NewsBoxState b = new NewsBoxState();
                                    b.BoxNo = boxno;
                                    b.PaperNum = Int32.Parse(boxnos[1]);
                                    tNewsState.Add(b);

                                    //设置箱头的报刊分发状态
                                    if (boxstatus[boxno].BackBox.BoxBN == "")
                                    {
                                        b.boxStatus = boxstatus[boxno].FrontBox;
                                        boxstatus[boxno].FrontBox.IsNewspaperDistribute = true;
                                        List<GroupStatus> g = GetGroupFromBoxBN(b.boxStatus.BoxBN);
                                        //先查找是否有本组，有退出，没有添加
                                        foreach (GroupStatus gs in g)
                                        {
                                            if (!tNewsGroup.Contains(gs))
                                            {
                                                tNewsGroup.Add(gs);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        b.boxStatus = boxstatus[boxno].BackBox;
                                        boxstatus[boxno].BackBox.IsNewspaperDistribute = true;
                                        List<GroupStatus> g = GetGroupFromBoxBN(b.boxStatus.BoxBN);
                                        //先查找是否有本组，有退出，没有添加
                                        foreach (GroupStatus gs in g)
                                        {
                                            if (!tNewsGroup.Contains(gs))
                                            {
                                                tNewsGroup.Add(gs);
                                            }
                                        }
                                    }
                                }
                            }
                            if (!bRight)
                                break;

                            //设置这些箱子的组号
                            m_NewsGroup = tNewsGroup;
                            //设置箱子状态
                            m_NewsState = tNewsState;

                            //设置这些组的状态到报刊分发
                            foreach (GroupStatus gs in m_NewsGroup)
                            {
                                //显示灭
                                BoxDataParse.DataParse.CmdScreen(gs.GroupShowBn, false);
                            }

                            //下传箱头信数
                            foreach (NewsBoxState b in m_NewsState)
                            {
                                BoxInfo.BoxStatus bt = GetBoxNOFromBoxBN(b.boxStatus.BoxBN);
                                if (bt.Connected)
                                {
                                    if (b.PaperNum <= 0)
                                    {
                                        b.boxStatus.IsCloseScreen = true;
                                    }
                                    else
                                    {
                                        //显示亮
                                        b.boxStatus.IsCloseScreen = false;
                                        BoxDataParse.DataParse.CmdBoxLED(b.boxStatus.BoxBN, 1, b.PaperNum, enum_LedColor.红色);
                                    }
                                }
                                else
                                {
                                    //发送通知到业务平台，箱头有错
                                    SendClientNotify.NotifyNewsState(b.BoxNo.ToString("000"), 2);
                                }
                            }
                            m_isNewsPaper = true;
                            Rstr = "ok";
                        }
                        break;
                    #endregion

                    #region 502 报刊分发通讯－分发结束请求，
                    case 502:
                        if (m_NewsState != null && m_NewsGroup != null)
                        {
                            m_isNewsPaper = false;
                            //设置这些组的状态退出报刊分发
                            if (m_NewsGroup != null)
                            {
                                foreach (GroupStatus gs in m_NewsGroup)
                                {
                                    //显示亮
                                    BoxDataParse.DataParse.CmdScreen(gs.GroupShowBn, true);
                                    foreach (GroupStatus.StBoxInfo stbox in gs.MBoxInfo.Values)
                                        stbox.BoxStatus.IsCloseScreen = false;
                                }
                            }
                            if (m_NewsState != null)
                            {
                                //回复箱头的状态
                                foreach (NewsBoxState b in m_NewsState)
                                {
                                    b.boxStatus.IsCloseScreen = false;
                                    b.boxStatus.IsNewspaperDistribute = false;
                                    boxstatus[b.BoxNo].CheckBoxLetter();
                                }
                            }
                            //清除数组
                            m_NewsGroup = null;
                            m_NewsState = null;
                            Rstr = "ok";
                        }
                        break;
                        #endregion
                }
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, "解析客户端的箱号出错：" + ee.ToString() + "\r\n");
            }

            return Rstr;
        }

        #endregion

        #region 从BN转换到箱号
        private BoxInfo.BoxStatus GetBoxNOFromBoxBN(string bn)
        {
            foreach (BoxInfo box in boxstatus.Values)
            {
                if (box.FrontBox.BoxBN == bn)
                    return box.FrontBox;
                if (box.BackBox.BoxBN == bn)
                    return box.BackBox;
            }
            return null;
        }
        #endregion

        #region 从BoxBN找到对应组
        private List<GroupStatus> GetGroupFromBoxBN(string bn)
        {
            List<GroupStatus> ret = new List<GroupStatus>();
            foreach (GroupStatus g in groupstatus.Values)
            {
                foreach (GroupStatus.StBoxInfo b in g.MBoxInfo.Values)
                {
                    if (b.BoxBn == bn)
                    {
                        if (!ret.Contains(g))
                            ret.Add(g);
                        break;
                    }
                }
            }
            return ret;
        }
        #endregion

        #region 监听箱头端口
        private void box_listen_OnReceiveBoxData(string BN_NO, ReceiveDataType iType, string data, bool bFront)
        {
            try
            {
                BoxInfo.BoxStatus boxStatu = GetBoxNOFromBoxBN(BN_NO);

                if (iType != ReceiveDataType.主箱连接 && iType != ReceiveDataType.主箱连接_Version)
                {
                    string msg = "BN:" + BN_NO + ", Type:" + iType.ToString() + ", bFront:" + bFront.ToString() + ", Data:" + data;
                    Log.WriteInfo(LogType.Info, msg);
                }
                //如果是分箱，设置分箱的连接时间
                if (boxStatu != null)
                {
                    lock (boxStatu)
                    {
                        //箱子
                        if (!boxStatu.Connected || (iType == ReceiveDataType.主箱连接 && data == "Start"))
                        {
                            //设置初始信息
                            boxstatus[boxStatu.BoxNO].InitBoxStatus();
                            System.Threading.Thread.Sleep(100);
                            BoxDataParse.DataParse.CmdBoxToIdle(BN_NO);
                            BoxDataParse.DataParse.CmdGetBoxState(BN_NO);
                        }
                        boxStatu.Connected = true;
                    }
                }

                switch (iType)
                {
                    #region Ping
                    case ReceiveDataType.主箱连接:
                        {
                            switch (data)
                            {
                                case "Start":
                                case "Ping":    //PING
                                    {
                                        if (groupstatus.ContainsKey(BN_NO))
                                        {
                                            GroupStatus gs = (GroupStatus)groupstatus[BN_NO];
                                            gs.Connected = true;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    #endregion

                    #region 门禁
                    case ReceiveDataType.门禁:
                        {
                            if (boxStatu == null)
                                break;

                            #region 门禁处理
                            switch (data)
                            {
                                case "Opened":
                                    if (boxStatu.Gating != DoorGateStatus.Opened)
                                    {
                                        if (boxStatu.GateError)
                                        {
                                            boxStatu.Gating = DoorGateStatus.Opened;
                                            string errstr = boxStatu.GetErrorString();
                                            //发送错误到界面
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //灭错误灯
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.灭);
                                            }
                                        }
                                        else
                                            boxStatu.Gating = DoorGateStatus.Opened;

                                        //发送通知到客户端
                                        SendClientNotify.NotifyGate(boxStatu.BoxNO.ToString("000"), "on");
                                    }
                                    break;

                                case "Closed":
                                    if (boxStatu.Gating != DoorGateStatus.Closed)
                                    {
                                        if (boxStatu.GateError)
                                        {
                                            boxStatu.Gating = DoorGateStatus.Closed;
                                            string errstr = boxStatu.GetErrorString();
                                            //发送错误到界面
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //灭错误灯
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.灭);
                                            }
                                        }
                                        else
                                            boxStatu.Gating = DoorGateStatus.Closed;

                                        //发送通知到客户端
                                        SendClientNotify.NotifyGate(boxStatu.BoxNO.ToString("000"), "off");
                                    }

                                    break;

                                case "OpenTimeOut":
                                    if (boxStatu.Gating != DoorGateStatus.OpenTimeOut)
                                    {
                                        boxStatu.Gating = DoorGateStatus.OpenTimeOut;
                                        string errstr = boxStatu.GetErrorString();
                                        //发送错误到界面
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                        //亮错误灯
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.亮);
                                    }
                                    break;

                                case "CloseTimeOut":
                                    if (boxStatu.Gating != DoorGateStatus.CloseTimeOut)
                                    {
                                        boxStatu.Gating = DoorGateStatus.CloseTimeOut;
                                        string errstr = boxStatu.GetErrorString();
                                        //发送错误到界面
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                        //亮错误灯
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.亮);
                                    }
                                    break;
                            }
                            #endregion
                            List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                            foreach (GroupStatus g in groups)
                            {
                                //组信息处理
                                g.GateStateChange(boxStatu.BoxNO, data);
                            }
                        }
                        break;
                    #endregion

                    #region 门锁
                    case ReceiveDataType.门锁:
                        {
                            if (boxStatu == null)
                                break;

                            #region 门锁处理
                            switch (data)
                            {
                                case "Opened":
                                    if (boxStatu.Door != DoorGateStatus.Opened)
                                    {
                                        if (boxStatu.DoorError)
                                        {
                                            boxStatu.Door = DoorGateStatus.Opened;
                                            string errstr = boxStatu.GetErrorString();
                                            //发送错误到界面
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //灭错误灯
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.灭);
                                            }
                                        }
                                        else
                                            boxStatu.Door = DoorGateStatus.Opened;

                                        //发送通知到客户端
                                        SendClientNotify.NotifyDoor(boxStatu.BoxNO.ToString("000"), "on");
                                    }
                                    break;

                                case "Closed":
                                    if (boxStatu.Door != DoorGateStatus.Closed)
                                    {
                                        if (boxStatu.DoorError)
                                        {
                                            boxStatu.Door = DoorGateStatus.Closed;
                                            string errstr = boxStatu.GetErrorString();
                                            //发送错误到界面
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //灭错误灯
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.灭);
                                            }
                                        }
                                        else
                                            boxStatu.Door = DoorGateStatus.Closed;

                                        //发送通知到客户端
                                        SendClientNotify.NotifyDoor(boxStatu.BoxNO.ToString("000"), "off");

                                        if (boxstatus[boxStatu.BoxNO].LetterCount == 0)
                                        {
                                            //核对一下数量
                                            boxstatus[boxStatu.BoxNO].CheckBoxLetter();
                                        }
                                    }
                                    break;

                                case "OpenTimeOut":
                                    if (boxStatu.Door != DoorGateStatus.OpenTimeOut)
                                    {
                                        boxStatu.Door = DoorGateStatus.OpenTimeOut;
                                        string errstr = boxStatu.GetErrorString();
                                        //发送错误到界面
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                        //亮错误灯
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.亮);
                                    }
                                    break;

                            }
                            #endregion
                            List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                            foreach (GroupStatus g in groups)
                            {
                                //组信息处理
                                g.DoorStateChange(boxStatu.BoxNO, data);
                            }
                        }
                        break;
                    #endregion

                    #region 光电
                    case ReceiveDataType.光电:
                        {
                            if (groupstatus.ContainsKey(BN_NO))
                            {
                                if (data == "触发光电遮挡" || data == "触发光电未遮挡")
                                {
                                    GroupStatus g = groupstatus[BN_NO];
                                    g.ChufaGdChange(data == "触发光电遮挡");
                                }
                            }
                            if (boxStatu == null)
                            {
                                break;
                            }

                            #region 光电处理
                            switch (data)
                            {
                                case "前光电遮挡":   //前光电遮挡
                                case "前光电未遮挡":
                                    {
                                        if (boxStatu.FrontGDError)
                                        {
                                            boxStatu.FrontGDError = false;
                                            string errstr = boxStatu.GetErrorString();
                                            //发送错误到界面
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //灭错误灯
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.灭);
                                            }
                                        }
                                    }
                                    break;
                                case "前光电故障":       //光电错误代码
                                    if (!boxStatu.FrontGDError)
                                    {
                                        boxStatu.FrontGDError = true;
                                        //发送错误到界面
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), boxStatu.GetErrorString());
                                        //亮错误灯
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.亮);
                                    }
                                    break;

                                case "后光电遮挡":
                                case "后光电未遮挡":
                                    {
                                        if (boxStatu.BackGDError)
                                        {
                                            boxStatu.BackGDError = false;
                                            string errstr = boxStatu.GetErrorString();
                                            //发送错误到界面
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //灭错误灯
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.灭);
                                            }
                                        }
                                    }
                                    break;
                                case "后光电故障":       //光电错误代码
                                    if (!boxStatu.BackGDError)
                                    {
                                        boxStatu.BackGDError = true;
                                        //发送错误到界面
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), boxStatu.GetErrorString());
                                        //亮错误灯
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.亮);
                                    }
                                    break;

                                case "箱满光电遮挡":
                                case "箱满光未遮挡":
                                    {
                                        string err1 = boxStatu.GetErrorString();
                                        if (boxStatu.FullGDError)
                                        {
                                            boxStatu.FullGDError = false;
                                        }
                                        if (data == "箱满光电遮挡")
                                            boxStatu.FullGDZheDang = true;
                                        else
                                            boxStatu.FullGDZheDang = false;
                                        string errstr = boxStatu.GetErrorString();
                                        //发送错误到界面
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                        if (errstr.CompareTo("") == 0 && err1.CompareTo("") != 0)
                                        {
                                            //灭错误灯
                                            BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.灭);
                                            BoxDataParse.DataParse.CmdLCDText(BN_NO, 3, enum_TextType.显示附带的文本, 0, 1, " ");
                                        }
                                        else if (errstr.CompareTo("") != 0)
                                        {
                                            //亮错误灯
                                            BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.亮);
                                            if (data == "箱满光电遮挡")
                                                BoxDataParse.DataParse.CmdLCDText(BN_NO, 3, enum_TextType.显示附带的文本, 0, 255, errstr);
                                        }
                                    }
                                    break;
                                case "箱满光电故障":      //光电错误代码
                                    if (!boxStatu.FullGDError)
                                    {
                                        boxStatu.FullGDError = true;
                                        //发送错误到界面
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), boxStatu.GetErrorString());
                                        //亮错误灯
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.亮);
                                    }
                                    break;

                                case "箱空光电遮挡":
                                case "箱空光电未遮挡":
                                    {
                                        if (data == "箱空光电遮挡")
                                            boxStatu.ClearGDState = true;
                                        else
                                            boxStatu.ClearGDState = false;

                                        if (boxStatu.ClearGDError)
                                        {
                                            boxStatu.ClearGDError = false;
                                            string errstr = boxStatu.GetErrorString();
                                            //发送错误到界面
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //灭错误灯
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.灭);
                                            }
                                        }
                                    }
                                    break;
                                case "箱空光电故障":      //光电错误代码
                                    if (!boxStatu.ClearGDError)
                                    {
                                        boxStatu.ClearGDError = true;
                                        //发送错误到界面
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), boxStatu.GetErrorString());
                                        //亮错误灯
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.亮);
                                    }
                                    break;

                                case "触发光电未遮挡":
                                case "触发光电遮挡":
                                    {
                                        if (boxStatu.ChufaGDError)
                                        {
                                            boxStatu.ChufaGDError = false;
                                            string errstr = boxStatu.GetErrorString();
                                            //发送错误到界面
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //灭错误灯
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.灭);
                                            }
                                        }
                                    }
                                    break;
                                case "触发光电故障":      //光电错误代码
                                    if (!boxStatu.ChufaGDError)
                                    {
                                        boxStatu.ChufaGDError = true;
                                        //发送错误到界面
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), boxStatu.GetErrorString());
                                        //亮错误灯
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.亮);
                                    }
                                    break;
                            }
                            #endregion
                            List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                            foreach (GroupStatus g in groups)
                            {
                                //组信息处理
                                g.GdStateChange(boxStatu.BoxNO, data);
                            }
                        }
                        break;
                    #endregion

                    #region 按键处理
                    case ReceiveDataType.按键:
                        {
                            int boxNo = 0;
                            if (boxStatu != null)
                            {
                                boxNo = boxStatu.BoxNO;
                                //唤醒屏幕
                                boxStatu.IsCloseScreen = false;
                            }

                            if (m_isNewsPaper)
                                News_PushButton(boxNo);
                            else if (boxNo > 0)
                            {
                                List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                                foreach (GroupStatus g in groups)
                                {
                                    //组信息处理
                                    g.KeyPress(boxNo, Int32.Parse(data));
                                }
                            }
                            else if (boxNo == 0)
                            {
                                groupstatus[BN_NO].KeyPress(0, Int32.Parse(data));
                            }
                        }
                        break;
                    #endregion

                    #region 条码
                    case ReceiveDataType.条码信息:
                        //组信息处理
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];
                            string BarCode = CusTrim(data);
                            if (BarCode.Length > 1)
                                gs.GetBarCode(BarCode);
                        }
                        break;
                    case ReceiveDataType.条码信息_4Dai:
                        //组信息处理
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];
                            string BarCode = CusTrim(data);
                            if (BarCode.Length > 1)
                                gs.GetBarCode_4Dai(BarCode);
                        }
                        break;
                    #endregion

                    #region 投件投入
                    case ReceiveDataType.投件投入:
                        {
                            int ShowCount = Convert.ToInt32(data);
                            List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                            foreach (GroupStatus g in groups)
                            {
                                //保存信息
                                g.LetterIn(boxStatu.BoxNO, ShowCount);
                            }
                        }
                        break;
                    #endregion
                    #region 投件抽出
                    case ReceiveDataType.投件抽出:
                        {
                            int ShowCount = Convert.ToInt32(data);
                            List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                            foreach (GroupStatus g in groups)
                            {
                                //保存信息
                                g.LetterOut(boxStatu.BoxNO, ShowCount);
                            }
                        }
                        break;
                    #endregion

                    #region 证卡条码
                    case ReceiveDataType.证卡信息_4Dai:
                    case ReceiveDataType.证卡信息:
                        //组信息处理
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];

                            string barCode = CusTrim(data);
                            if (barCode.Length > 1)
                                gs.GetCardCode(barCode, BN_NO);

                        }
                        else if (boxStatu != null)
                        {
                            List<GroupStatus> gs = GetGroupFromBoxBN(BN_NO);
                            string barCode = CusTrim(data);
                            foreach (GroupStatus g in gs)
                            {
                                if (barCode.Length > 1)
                                    g.BoxGetCardCode(boxStatu.BoxNO, barCode);
                            }
                        }
                        break;
                    #endregion

                    #region 证卡信息_指静脉
                    case ReceiveDataType.证卡信息_指静脉:
                        //组信息处理
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];

                            gs.GetZhiJingMaiInfo(data, 0);
                        }
                        break;

                    case ReceiveDataType.证卡信息_燕南指静脉:
                        //组信息处理
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];

                            gs.GetZhiJingMaiInfo(data, 1);
                        }
                        break;

                    case ReceiveDataType.指静脉数据_验证:
                        //组信息处理
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];

                            gs.GetZhiJingMaiId(data);
                        }
                        break;
                    #endregion

                    #region 取件单位选择
                    case ReceiveDataType.取件单位选择:
                        //组信息处理
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];
                            string[] ids = data.Split(',');
                            List<int> idArray = new List<int>();
                            foreach (string id in ids)
                            {
                                idArray.Add(Convert.ToInt32(id));
                            }
                            gs.取件单位选择(idArray);
                        }
                        break;
                    #endregion


                    #region 上报拍摄的照片
                    case ReceiveDataType.上报拍摄的照片:
                        //组信息处理
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];

                            gs.GetM3Photo(data);
                        }
                        break;
                        #endregion

                }
            }
            catch (Exception e)
            {
                Log.WriteInfo(LogType.Error, "监听箱头端口：" + e.ToString() + "\r\n");
            }

        }
        #endregion

        #region 初始化箱子状态
        private bool InitBoxStatus()
        {
            try
            {
                DataBase.MonitorService.BoxSetInfoClass[] boxs = DataBase.DataSave.GetBoxSetInfo();
                if (boxs.Length <= 0) return false;
                int i = 0;
                for (i = 0; i < boxs.Length; i++)
                {
                    BoxInfo box = new BoxInfo();
                    box.FrontBox.BoxNO = Int32.Parse(boxs[i].BoxNO);
                    box.FrontBox.BoxBN = boxs[i].FrontBoxBN;
                    box.HasTwoLock = boxs[i].HasTwoLock;
                    if (!box.HasTwoLock)
                    {
                        box.BackBox.BoxNO = Int32.Parse(boxs[i].BoxNO);
                        box.BackBox.BoxBN = boxs[i].BackBoxBN;
                    }
                    boxstatus.Add(box.BoxNO, box);
                    m_ListBoxstatus.Add(box);
                }

                //初始化组信息
                DataBase.MonitorService.GroupSetInfoClass[] groups = DataBase.DataSave.GetGroupSetInfo();
                if (groups.Length <= 0)
                {
                    boxstatus.Clear();
                    return false;
                }
                for (i = 0; i < groups.Length; i++)
                {
                    GroupStatus gs = new GroupStatus(groups[i], true, boxstatus, groupstatus);
                    //用户投信事件
                    gs.OnLetterIn += new BoxLetterCountChange(Box_OnLetterIn);
                    gs.GetGroupFromBoxNo += new DGetGroupFromBoxNo(gs_GetGroupNameFromBoxNo);
                    if (groups[i].GroupBackScanBN != null && groups[i].GroupBackScanBN != "")
                    {
                        gs = new GroupStatus(groups[i], false, boxstatus, groupstatus);
                        //用户投信事件
                        gs.OnLetterIn += new BoxLetterCountChange(Box_OnLetterIn);
                        gs.GetGroupFromBoxNo += new DGetGroupFromBoxNo(gs_GetGroupNameFromBoxNo);
                    }
                }

                return true;
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
                boxstatus.Clear();
                groupstatus.Clear();
                return false;
            }
        }

        GroupStatus gs_GetGroupNameFromBoxNo(int BoxNO)
        {
            foreach (GroupStatus g in groupstatus.Values)
            {
                if (g.MBoxInfo.ContainsKey(BoxNO))
                {
                    return g;
                }
            }

            return null;
        }
        #endregion

        #region 判断箱头连接状态
        private void CheckBoxTimeOut()
        {
            int i = 0, j = 0, k = 0;

            while (Constant.b_AppRunning)
            {
                try
                {
                    k++;
                    if (k > 12)
                    {
                        k = 0;
                        foreach (GroupStatus g in groupstatus.Values)
                        {
                            g.TimeOut();
                        }
                    }

                    if (i >= m_ListBoxstatus.Count)
                        i = 0;
                    BoxInfo bi = m_ListBoxstatus[i++];
                    //休眠屏幕
                    if (Constant.TimeOut_CloseScreen > 0)
                    {
                        bi.FrontBox.CloseScreen();
                        bi.BackBox.CloseScreen();
                    }
                    //读取数据库，更新箱头信件数目，以及急件等状态
                    DataBase.MonitorService.BoxInfo box = DataBase.DataSave.GetBoxLetterCount(bi.BoxNO);

                    if (box != null)
                    {
                        bi.BoxProperty = (EnumBoxStat)box.BoxProperty;
                        bi.IsQingTuiXiang = box.IsQingTuiXiang;
                        if (box.BoxShowName != bi.BoxUnitName)
                        {
                            bi.BoxUnitName = box.BoxShowName;
                            BoxDataParse.DataParse.CmdLCDText(bi.FrontBox.BoxBN, 0, enum_TextType.显示附带的文本, 0, 255, box.BoxShowName);
                        }
                        //if (box.BoxShowFullName != bi.BoxUnitFullName)
                        //{
                        //    bi.BoxUnitFullName = box.BoxShowFullName;
                        //    BoxDataParse.DataParse.CmdLCDText(bi.FrontBox.BoxBN, 2, enum_TextType.显示附带的文本, 0, 255, box.BoxShowFullName);
                        //    if (bi.BackBox.BoxBN != "")
                        //        BoxDataParse.DataParse.CmdLCDText(bi.BackBox.BoxBN, 2, enum_TextType.显示附带的文本, 0, 255, box.BoxShowFullName);
                        //}
                        if (box.SendCount != bi.m_LetterCount)// && (box.SendCount != 0 || LogInfo.Constant.m_IsPiaoJuXiang)
                        {
                            bi.LetterCount = box.SendCount;
                            BoxDataParse.DataParse.CmdBoxLED(bi.FrontBox.BoxBN, 1, box.SendCount, enum_LedColor.绿色);
                            if (bi.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdBoxLED(bi.BackBox.BoxBN, 1, box.SendCount, enum_LedColor.绿色);
                        }
                        if (box.HasJiJian != bi.LampJiJian && box.SendCount != 0)
                        {
                            bi.LampJiJian = box.HasJiJian;
                            BoxDataParse.DataParse.CmdBoxLampJiJian(bi.FrontBox.BoxBN, enum_LampStatus.亮);
                            if (bi.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdBoxLampJiJian(bi.BackBox.BoxBN, enum_LampStatus.亮);
                        }
                        if (bi.BackBox.BoxBN != "")
                            BoxDataParse.DataParse.CmdLCDText(bi.BackBox.BoxBN, 0, enum_TextType.显示附带的文本, 0, 255, box.BoxShowName);
                        if (!string.IsNullOrEmpty(box.BoxShowFullName))
                        {
                            BoxDataParse.DataParse.CmdLCDText(bi.FrontBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 255, box.BoxShowFullName);
                        }
                    }
                    else
                    {
                        //备用箱头
                        if (bi.BoxUnitName != Constant.EmptyBoxName)
                        {
                            bi.BoxUnitName = Constant.EmptyBoxName;
                            BoxDataParse.DataParse.CmdLCDText(bi.FrontBox.BoxBN, 0, enum_TextType.显示附带的文本, 0, 255, bi.BoxUnitName);
                            if (bi.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(bi.BackBox.BoxBN, 0, enum_TextType.显示附带的文本, 0, 255, bi.BoxUnitName);

                            bi.BoxUnitFullName = Constant.EmptyBoxName;
                            BoxDataParse.DataParse.CmdLCDText(bi.FrontBox.BoxBN, 2, enum_TextType.显示附带的文本, 0, 255, bi.BoxUnitName);
                            if (bi.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(bi.BackBox.BoxBN, 2, enum_TextType.显示附带的文本, 0, 255, bi.BoxUnitName);
                        }
                    }

                    //领导出差信息
                    j++;
                    if (j > 200)
                    {
                        GetLeaderOutMessage();
                        j = 0;
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, ee.ToString());
                }
                System.Threading.Thread.Sleep(500);
            }
        }
        #endregion


        #region 得到领导出差信息
        private void GetLeaderOutMessage()
        {
            try
            {
                DataBase.MonitorService.ClassBoxShowMessage outMessage;
                //轮询所有箱格的领导出差信息
                foreach (BoxInfo box in boxstatus.Values)
                {
                    outMessage = DataBase.DataSave.GetBoxShowMessage(box.BoxNO);
                    //出差信息变化的处理，其他的不处理。变化包括新增和取消
                    if (outMessage.出差信息.CompareTo(box.LeaderOutMessage) != 0)
                    {
                        box.LeaderOutMessage = outMessage.出差信息;
                        if (outMessage.出差信息.CompareTo("") == 0)
                        {
                            //清空现有显示
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 0, "");
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 0, "");
                        }
                        else
                        {
                            //新增或者修改出差信息
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 255, box.LeaderOutMessage);
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.显示附带的文本, 0, 255, box.LeaderOutMessage);
                        }
                    }
                }
                //中控的公告信息
                outMessage = DataBase.DataSave.GetBoxShowMessage(0);
                if (outMessage.出差信息.CompareTo("") != 0)
                {
                    foreach (GroupStatus gs in groupstatus.Values)
                    {
                        gs.SetGongGaoInfo(outMessage.出差信息);
                    }
                }
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
            }
        }
        #endregion

        #region 用户投信后，通知界面函数
        private void Box_OnLetterIn(GroupStatus g, int BoxNO)
        {
            try
            {
                if (!boxstatus.ContainsKey(BoxNO)) return;
                //通知到监控界面
                SendClientNotify.NotifyLetter(BoxNO.ToString("000"), (boxstatus[BoxNO].LampJiJian ? "true" : "false"), boxstatus[BoxNO].LetterCount);
            }
            catch
            { }
        }
        #endregion

        private string CusTrim(string data)
        {
            string BarCode = data.Trim();
            while (BarCode.Length > 0 && (BarCode[0] == '\r' || BarCode[0] == '\n' || BarCode[0] == ' ' || BarCode[0] == '\t'))
                BarCode = BarCode.Substring(1);
            while (BarCode.Length > 0 && (BarCode[BarCode.Length - 1] == '\r' || BarCode[BarCode.Length - 1] == '\n' || BarCode[BarCode.Length - 1] == ' ' || BarCode[BarCode.Length - 1] == '\t'))
                BarCode = BarCode.Substring(0, BarCode.Length - 1);
            return BarCode;
        }

        #region 报刊分发按键，投入箱中的处理
        private void News_PushButton(int boxno)
        {
            try
            {
                //判断分发箱中有没有它
                NewsBoxState bCurrent = null;
                foreach (NewsBoxState b in m_NewsState)
                {
                    if (b.BoxNo == boxno)
                    {
                        bCurrent = b;
                        break;
                    }
                }
                if (bCurrent == null)
                    return;

                //设置本箱为投入，通知业务
                if (Constant.NewspaperType == 1)
                {
                    if (!bCurrent.bSendOK)
                    {
                        bCurrent.bSendOK = true;
                        SendClientNotify.NotifyNewsState(boxno.ToString("000"), 1);
                        //亮已取，灭急件
                        //如果按键的箱头的数目为0，则不作任何处理
                        if (bCurrent.PaperNum > 0)
                        {
                            if (boxstatus[boxno].BackBox.BoxBN == "")
                                BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].FrontBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.绿色);
                            else
                                BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].BackBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.绿色);
                        }
                    }
                    else
                    {
                        //翻转
                        bCurrent.bSendOK = false;
                        SendClientNotify.NotifyNewsState(boxno.ToString("000"), 0);
                        //亮急件，灭已取
                        //如果按键的箱头的数目为0，则不作任何处理
                        if (bCurrent.PaperNum > 0)
                        {
                            if (boxstatus[boxno].BackBox.BoxBN == "")
                                BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].FrontBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.红色);
                            else
                                BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].BackBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.红色);
                        }
                    }
                }
                else if (Constant.NewspaperType == 0)
                {
                    bool oldstate = bCurrent.bSendOK;
                    foreach (NewsBoxState b in m_NewsState)
                    {
                        if (!oldstate)
                        {
                            if (!b.bSendOK)
                            {
                                b.bSendOK = true;
                                SendClientNotify.NotifyNewsState(b.BoxNo.ToString("000"), 1);
                                //亮已取，灭急件
                                //如果按键的箱头的数目为0，则不作任何处理
                                if (bCurrent.PaperNum > 0)
                                {
                                    if (boxstatus[boxno].BackBox.BoxBN == "")
                                        BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].FrontBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.绿色);
                                    else
                                        BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].BackBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.绿色);
                                }
                            }
                        }
                        else
                        {
                            //翻转
                            if (b.bSendOK)
                            {
                                b.bSendOK = false;
                                SendClientNotify.NotifyNewsState(b.BoxNo.ToString("000"), 0);
                                //亮已取，灭急件
                                //如果按键的箱头的数目为0，则不作任何处理
                                if (bCurrent.PaperNum > 0)
                                {
                                    if (boxstatus[boxno].BackBox.BoxBN == "")
                                        BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].FrontBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.红色);
                                    else
                                        BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].BackBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.红色);
                                }
                            }
                        }
                        if (b == bCurrent) break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteInfo(LogType.Error, "报刊分发按键错误：" + e.ToString() + "\r\n");
            }
        }
        #endregion

    }
}
