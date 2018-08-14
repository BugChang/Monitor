using System;
using System.Collections.Generic;
using System.Xml;
using LogInfo;

namespace DataProcess
{
    /// <summary>
    /// DataProcess ��ժҪ˵����
    /// </summary>
    public class DataProcess
    {
        class NewsBoxState
        {
            /// <summary>
            /// �߼���ţ���1��ʼ
            /// </summary>
            public int BoxNo;
            /// <summary>
            /// ��Ӧ��ǰ���豸
            /// </summary>
			public BoxInfo.BoxStatus boxStatus;
            /// <summary>
            /// ������������ʾ
            /// </summary>
            public int PaperNum;
            /// <summary>
            /// �Ƿ��Ѿ�Ͷ�뱨��
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
        /// �Ƿ񱨿��ַ�״̬
        /// </summary>
        bool m_isNewsPaper;
        List<GroupStatus> m_NewsGroup;
        List<NewsBoxState> m_NewsState;


        #region ���ʼ������
        public DataProcess()
        {
            m_isNewsPaper = false;
            ClientDataParse.ClientDataParse.OnReceiveClentData += new ClientDataParse.ReceiveClientData(client_OnReceiveClentData);
            BoxDataParse.DataParse.OnReceiveBoxData += new BoxDataParse.d_ReceiveBoxData(box_listen_OnReceiveBoxData);
            DataBase.DataSave.OnSyncError += new DataBase.d_SyncError(DataSave_OnSyncError);
        }
        #endregion

        #region �����͹رռ�س���
        public bool Start()
        {
            if (Constant.b_AppRunning)
                return true;

            Constant.b_AppRunning = true;

            //��������
            if (!DataBase.DataSave.Start())
                return false;
            Log.WriteFileLog("��������OK");

            //��ʼ������״̬
            if (!InitBoxStatus())
                goto ErrorHandle;
            Log.WriteFileLog("��ʼ������״̬OK");
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
            //�������
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
            //�������
            groupstatus.Clear();
            boxstatus.Clear();

            return true;
        }

        #endregion

        #region ͬ��������ʾ��Ϣ
        void DataSave_OnSyncError(int BoxNo, bool bError)
        {
            if (BoxNo == 0)
            {
                if (bError)
                {
                    //����Ͽ�
                    foreach (BoxInfo box in boxstatus.Values)
                    {
                        if (!box.bSyncError)
                        {
                            box.bSyncError = true;
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 255, box.GetSyncErrorMsg);
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 255, box.GetSyncErrorMsg);
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
                            //���������ʾ
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 0, " ");
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 0, " ");
                        }
                    }
                }
            }
            else
            {
                //����ͬ���������ܽӿڴ���
                if (bError)
                {
                    //����Ͽ�
                    if (boxstatus.ContainsKey(BoxNo))
                    {
                        BoxInfo box = boxstatus[BoxNo];
                        if (!box.bSyncError)
                        {
                            box.bSyncError = true;
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 255, box.GetSyncErrorMsg);
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 255, box.GetSyncErrorMsg);
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
                            //���������ʾ
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 0, " ");
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 0, " ");
                        }
                    }
                }
            }
        }
        #endregion

        #region ��������˿�
        private string client_OnReceiveClentData(int itype, string data, string data2)
        {
            string Rstr = "error";

            try
            {
                switch (itype)
                {
                    #region 101�û���¼��Ϣ��ѯ��
                    case 101:
                        {
                            string[] datas = data.Split(':');
                            if (datas.Length == 2)
                            {
                                //��ѯ���ݿ⣬�ж��û��Ƿ���Ե�¼
                                if (DataBase.DataSave_NoLocal.CheckLogin(datas[0], datas[1]))
                                {
                                    Rstr = "ok";
                                }
                            }
                        }
                        break;
                    #endregion

                    #region 102��ؿͻ��˽�����Ϣ��ѯ��
                    case 102:
                        {
                            Rstr = "";
                            //��ѯ���ݿ⣬��ѯ�û���������Ϣ
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


                    #region 202�û�ȡ�����󣨿�ָ�����ţ���
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

                                    //���ö�Ӧ��ͷ��״̬
                                    boxstatus[boxid].LampYiQu = true;
                                    boxstatus[boxid].LampJiJian = false;
                                    boxstatus[boxid].LetterCount = 0;

                                    //֪ͨ�ͻ��ˣ��Ƹı�
                                    SendClientNotify.NotifyLamp(BoxNO, "1", "on");
                                    //֪ͨ�ͻ��ˣ��ż�Ϊ 0
                                    SendClientNotify.NotifyLetter(BoxNO, "false", 0);

                                    //���� 
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

                    #region 203�˶���������
                    case 203:
                        {
                            int BoxNo = Convert.ToInt32(data);
                            if (!boxstatus.ContainsKey(BoxNo))
                                break;

                            //��ȡ���ݿ⣬�õ��ż���Ŀ�ͼ�����״̬
                            DataBase.MonitorService.BoxInfo t = DataBase.DataSave.GetBoxLetterCount(BoxNo);
                            if (t != null)
                            {
                                boxstatus[BoxNo].LampJiJian = t.HasJiJian;
                                boxstatus[BoxNo].LetterCount = t.SendCount;
                                //���ؽṹ��֪ͨ��ͷ
                                boxstatus[BoxNo].CheckBoxLetter();

                                //֪ͨ�ͻ��ˣ��ı��ż�
                                SendClientNotify.NotifyLetter(data, (boxstatus[BoxNo].LampJiJian ? "true" : "false"), boxstatus[BoxNo].LetterCount);
                            }

                            Rstr = "ok";
                        }
                        break;
                    #endregion

                    #region 401��������
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

                    #region 402���Ƶ�����
                    case 402:
                        {
                            string[] datas = data.Split(':');
                            if (datas.Length == 3)
                            {
                                int boxNo = Convert.ToInt32(datas[0]);
                                if (boxstatus.ContainsKey(boxNo))
                                {
                                    //1����ȡ�ƣ�2������ƣ�3�������ƣ�4��������
                                    if (datas[1] == "1")
                                    {
                                        if (Convert.ToBoolean(data2))
                                        {
                                            if (BoxDataParse.DataParse.CmdBoxLampYiQu(boxstatus[boxNo].FrontBox.BoxBN,
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.�� : enum_LampStatus.��))
                                            {
                                                Rstr = "ok";
                                            }
                                        }
                                        else
                                        {
                                            if (BoxDataParse.DataParse.CmdBoxLampYiQu(boxstatus[boxNo].BackBox.BoxBN,
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.�� : enum_LampStatus.��))
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
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.�� : enum_LampStatus.��))
                                            {
                                                Rstr = "ok";
                                            }
                                        }
                                        else
                                        {
                                            if (BoxDataParse.DataParse.CmdBoxLampCuoWu(boxstatus[boxNo].BackBox.BoxBN,
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.�� : enum_LampStatus.��))
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
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.�� : enum_LampStatus.��))
                                            {
                                                Rstr = "ok";
                                            }
                                        }
                                        else
                                        {
                                            if (BoxDataParse.DataParse.CmdBoxLampJiJian(boxstatus[boxNo].BackBox.BoxBN,
                                                datas[2].ToLower().CompareTo("on") == 0 ? enum_LampStatus.�� : enum_LampStatus.��))
                                            {
                                                Rstr = "ok";
                                            }
                                        }
                                    }
                                    if (Rstr == "ok")
                                    {
                                        //���ؽṹ���ı��
                                        if (datas[1] == "1")
                                            boxstatus[boxNo].LampYiQu = (datas[2].ToLower().CompareTo("on") == 0);
                                        else if (datas[1] == "3")
                                            boxstatus[boxNo].LampJiJian = (datas[2].ToLower().CompareTo("on") == 0);

                                        //֪ͨ�ͻ��ˣ��Ƹı�
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

                    #region 403�����Ž�����
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

                    #region 404����������
                    case 404:
                        {
                            //��ѯ�߼���ͷ�������Ϣ
                            Rstr = DataBase.DataSave_NoLocal.BoxLetterView(data);
                        }
                        break;
                    #endregion

                    #region 405����������󣭵�λ�б�
                    case 405:
                        {
                            //��ѯ��λ�б�
                            Rstr = DataBase.DataSave_NoLocal.UnitList();
                        }
                        break;
                    #endregion

                    #region 406����������󣭸��ĵ�λ��Ŷ�Ӧ��
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

                    #region 407��������ȡ������
                    case 407:
                        {
                            if (data.CompareTo("on") == 0)
                            {
                                //��������ͷ���飬����Ϊ��
                                foreach (BoxInfo bi in boxstatus.Values)
                                {
                                    if (!bi.LampYiQu)
                                    {
                                        bi.LampYiQu = true;
                                        //֪ͨ�ͻ���
                                        SendClientNotify.NotifyLamp(bi.BoxNO.ToString("000"), "1", "on");
                                    }
                                }
                            }
                            else
                            {
                                //��������ͷ���飬��ȡ����Ϊ��
                                foreach (BoxInfo bi in boxstatus.Values)
                                {
                                    if (bi.LampYiQu)
                                    {
                                        bi.LampYiQu = false;
                                        //֪ͨ�ͻ���
                                        SendClientNotify.NotifyLamp(bi.BoxNO.ToString("000"), "1", "off");
                                    }
                                }
                            }
                            //�ҵ������飬���Ϳ�����ȡ������
                            foreach (GroupStatus gs in groupstatus.Values)
                            {
                                //���Ʊ������ȡ�� on,off
                                foreach (GroupStatus.StBoxInfo stbox in gs.MBoxInfo.Values)
                                {
                                    if (BoxDataParse.DataParse.CmdBoxLampYiQu(stbox.BoxBn,
                                            data.ToLower().CompareTo("on") == 0 ? enum_LampStatus.�� : enum_LampStatus.��))
                                    {
                                        Rstr = "ok";
                                    }
                                }
                            }
                            Rstr = "ok";
                        }
                        break;
                    #endregion

                    #region 408����������
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

                    #region 409�˶���������
                    case 409:
                        {
                            //�ҵ�������ͷ���˶�����
                            foreach (BoxInfo bi in boxstatus.Values)
                            {
                                //��ȡ���ݿ⣬�õ��ż���Ŀ�ͼ�����״̬
                                DataBase.MonitorService_NoLocal.BoxInfo t = DataBase.DataSave_NoLocal.GetBoxLetterCount(bi.BoxNO);
                                if (t != null)
                                {
                                    bi.LampJiJian = t.HasJiJian;
                                    bi.LetterCount = t.SendCount;
                                    bi.LetterCount_XiangTou = t.SendCount;
                                    bi.BoxUnitName = t.BoxShowName;
                                    bi.BoxUnitFullName = t.BoxShowFullName;
                                    //���ؽṹ��֪ͨ��ͷ
                                    bi.CheckBoxLetter();


                                    //֪ͨ�ͻ��ˣ��ı��ż�
                                    SendClientNotify.NotifyLetter(bi.BoxNO.ToString("000"), (bi.LampJiJian ? "true" : "false"), bi.LetterCount);
                                    //���ʹ��󵽽���
                                    SendClientNotify.NotifyError(bi.BoxNO.ToString("000"), bi.FrontBox.GetErrorString());
                                    //�������
                                    BoxDataParse.DataParse.CmdBoxLampCuoWu(bi.FrontBox.BoxBN, (bi.FrontBox.GetErrorString() == "" ? enum_LampStatus.�� : enum_LampStatus.��));
                                }
                            }
                            Rstr = "ok";
                        }
                        break;
                    #endregion

                    #region 410������������
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


                    #region 501 �����ַ�ͨѶ���ַ���ʼ����
                    case 501:
                        {
                            bool bRight = true;
                            string[] datas = data.Split('\n');
                            List<NewsBoxState> tNewsState = new List<NewsBoxState>();
                            List<GroupStatus> tNewsGroup = new List<GroupStatus>();

                            //�ظ���ǰ��״̬
                            //������Щ���״̬�˳������ַ�
                            if (m_NewsGroup != null)
                            {
                                foreach (GroupStatus gs in m_NewsGroup)
                                {
                                    //��ʾ��
                                    BoxDataParse.DataParse.CmdScreen(gs.GroupShowBn, true);
                                    foreach (GroupStatus.StBoxInfo stbox in gs.MBoxInfo.Values)
                                        stbox.BoxStatus.IsCloseScreen = false;
                                }
                            }
                            if (m_NewsState != null)
                            {
                                //�ظ���ͷ��״̬
                                foreach (NewsBoxState b in m_NewsState)
                                {
                                    b.boxStatus.IsNewspaperDistribute = false;
                                    b.boxStatus.IsCloseScreen = false;
                                    boxstatus[b.BoxNo].CheckBoxLetter();
                                }
                            }

                            //�������յ�����Ϣ
                            for (int ii = 0; ii < datas.Length; ii++)
                            {
                                string[] boxnos = datas[ii].Split('\r');
                                if (boxnos.Length == 2)
                                {
                                    int boxno = Int32.Parse(boxnos[0]);
                                    //boxid����
                                    if (!boxstatus.ContainsKey(boxno))
                                    {
                                        bRight = false;
                                        break;
                                    }
                                    NewsBoxState b = new NewsBoxState();
                                    b.BoxNo = boxno;
                                    b.PaperNum = Int32.Parse(boxnos[1]);
                                    tNewsState.Add(b);

                                    //������ͷ�ı����ַ�״̬
                                    if (boxstatus[boxno].BackBox.BoxBN == "")
                                    {
                                        b.boxStatus = boxstatus[boxno].FrontBox;
                                        boxstatus[boxno].FrontBox.IsNewspaperDistribute = true;
                                        List<GroupStatus> g = GetGroupFromBoxBN(b.boxStatus.BoxBN);
                                        //�Ȳ����Ƿ��б��飬���˳���û�����
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
                                        //�Ȳ����Ƿ��б��飬���˳���û�����
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

                            //������Щ���ӵ����
                            m_NewsGroup = tNewsGroup;
                            //��������״̬
                            m_NewsState = tNewsState;

                            //������Щ���״̬�������ַ�
                            foreach (GroupStatus gs in m_NewsGroup)
                            {
                                //��ʾ��
                                BoxDataParse.DataParse.CmdScreen(gs.GroupShowBn, false);
                            }

                            //�´���ͷ����
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
                                        //��ʾ��
                                        b.boxStatus.IsCloseScreen = false;
                                        BoxDataParse.DataParse.CmdBoxLED(b.boxStatus.BoxBN, 1, b.PaperNum, enum_LedColor.��ɫ);
                                    }
                                }
                                else
                                {
                                    //����֪ͨ��ҵ��ƽ̨����ͷ�д�
                                    SendClientNotify.NotifyNewsState(b.BoxNo.ToString("000"), 2);
                                }
                            }
                            m_isNewsPaper = true;
                            Rstr = "ok";
                        }
                        break;
                    #endregion

                    #region 502 �����ַ�ͨѶ���ַ���������
                    case 502:
                        if (m_NewsState != null && m_NewsGroup != null)
                        {
                            m_isNewsPaper = false;
                            //������Щ���״̬�˳������ַ�
                            if (m_NewsGroup != null)
                            {
                                foreach (GroupStatus gs in m_NewsGroup)
                                {
                                    //��ʾ��
                                    BoxDataParse.DataParse.CmdScreen(gs.GroupShowBn, true);
                                    foreach (GroupStatus.StBoxInfo stbox in gs.MBoxInfo.Values)
                                        stbox.BoxStatus.IsCloseScreen = false;
                                }
                            }
                            if (m_NewsState != null)
                            {
                                //�ظ���ͷ��״̬
                                foreach (NewsBoxState b in m_NewsState)
                                {
                                    b.boxStatus.IsCloseScreen = false;
                                    b.boxStatus.IsNewspaperDistribute = false;
                                    boxstatus[b.BoxNo].CheckBoxLetter();
                                }
                            }
                            //�������
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
                Log.WriteInfo(LogType.Error, "�����ͻ��˵���ų���" + ee.ToString() + "\r\n");
            }

            return Rstr;
        }

        #endregion

        #region ��BNת�������
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

        #region ��BoxBN�ҵ���Ӧ��
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

        #region ������ͷ�˿�
        private void box_listen_OnReceiveBoxData(string BN_NO, ReceiveDataType iType, string data, bool bFront)
        {
            try
            {
                BoxInfo.BoxStatus boxStatu = GetBoxNOFromBoxBN(BN_NO);

                if (iType != ReceiveDataType.�������� && iType != ReceiveDataType.��������_Version)
                {
                    string msg = "BN:" + BN_NO + ", Type:" + iType.ToString() + ", bFront:" + bFront.ToString() + ", Data:" + data;
                    Log.WriteInfo(LogType.Info, msg);
                }
                //����Ƿ��䣬���÷��������ʱ��
                if (boxStatu != null)
                {
                    lock (boxStatu)
                    {
                        //����
                        if (!boxStatu.Connected || (iType == ReceiveDataType.�������� && data == "Start"))
                        {
                            //���ó�ʼ��Ϣ
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
                    case ReceiveDataType.��������:
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

                    #region �Ž�
                    case ReceiveDataType.�Ž�:
                        {
                            if (boxStatu == null)
                                break;

                            #region �Ž�����
                            switch (data)
                            {
                                case "Opened":
                                    if (boxStatu.Gating != DoorGateStatus.Opened)
                                    {
                                        if (boxStatu.GateError)
                                        {
                                            boxStatu.Gating = DoorGateStatus.Opened;
                                            string errstr = boxStatu.GetErrorString();
                                            //���ʹ��󵽽���
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //������
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                            }
                                        }
                                        else
                                            boxStatu.Gating = DoorGateStatus.Opened;

                                        //����֪ͨ���ͻ���
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
                                            //���ʹ��󵽽���
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //������
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                            }
                                        }
                                        else
                                            boxStatu.Gating = DoorGateStatus.Closed;

                                        //����֪ͨ���ͻ���
                                        SendClientNotify.NotifyGate(boxStatu.BoxNO.ToString("000"), "off");
                                    }

                                    break;

                                case "OpenTimeOut":
                                    if (boxStatu.Gating != DoorGateStatus.OpenTimeOut)
                                    {
                                        boxStatu.Gating = DoorGateStatus.OpenTimeOut;
                                        string errstr = boxStatu.GetErrorString();
                                        //���ʹ��󵽽���
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                        //�������
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                    }
                                    break;

                                case "CloseTimeOut":
                                    if (boxStatu.Gating != DoorGateStatus.CloseTimeOut)
                                    {
                                        boxStatu.Gating = DoorGateStatus.CloseTimeOut;
                                        string errstr = boxStatu.GetErrorString();
                                        //���ʹ��󵽽���
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                        //�������
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                    }
                                    break;
                            }
                            #endregion
                            List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                            foreach (GroupStatus g in groups)
                            {
                                //����Ϣ����
                                g.GateStateChange(boxStatu.BoxNO, data);
                            }
                        }
                        break;
                    #endregion

                    #region ����
                    case ReceiveDataType.����:
                        {
                            if (boxStatu == null)
                                break;

                            #region ��������
                            switch (data)
                            {
                                case "Opened":
                                    if (boxStatu.Door != DoorGateStatus.Opened)
                                    {
                                        if (boxStatu.DoorError)
                                        {
                                            boxStatu.Door = DoorGateStatus.Opened;
                                            string errstr = boxStatu.GetErrorString();
                                            //���ʹ��󵽽���
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //������
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                            }
                                        }
                                        else
                                            boxStatu.Door = DoorGateStatus.Opened;

                                        //����֪ͨ���ͻ���
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
                                            //���ʹ��󵽽���
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //������
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                            }
                                        }
                                        else
                                            boxStatu.Door = DoorGateStatus.Closed;

                                        //����֪ͨ���ͻ���
                                        SendClientNotify.NotifyDoor(boxStatu.BoxNO.ToString("000"), "off");

                                        if (boxstatus[boxStatu.BoxNO].LetterCount == 0)
                                        {
                                            //�˶�һ������
                                            boxstatus[boxStatu.BoxNO].CheckBoxLetter();
                                        }
                                    }
                                    break;

                                case "OpenTimeOut":
                                    if (boxStatu.Door != DoorGateStatus.OpenTimeOut)
                                    {
                                        boxStatu.Door = DoorGateStatus.OpenTimeOut;
                                        string errstr = boxStatu.GetErrorString();
                                        //���ʹ��󵽽���
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                        //�������
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                    }
                                    break;

                            }
                            #endregion
                            List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                            foreach (GroupStatus g in groups)
                            {
                                //����Ϣ����
                                g.DoorStateChange(boxStatu.BoxNO, data);
                            }
                        }
                        break;
                    #endregion

                    #region ���
                    case ReceiveDataType.���:
                        {
                            if (groupstatus.ContainsKey(BN_NO))
                            {
                                if (data == "��������ڵ�" || data == "�������δ�ڵ�")
                                {
                                    GroupStatus g = groupstatus[BN_NO];
                                    g.ChufaGdChange(data == "��������ڵ�");
                                }
                            }
                            if (boxStatu == null)
                            {
                                break;
                            }

                            #region ��紦��
                            switch (data)
                            {
                                case "ǰ����ڵ�":   //ǰ����ڵ�
                                case "ǰ���δ�ڵ�":
                                    {
                                        if (boxStatu.FrontGDError)
                                        {
                                            boxStatu.FrontGDError = false;
                                            string errstr = boxStatu.GetErrorString();
                                            //���ʹ��󵽽���
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //������
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                            }
                                        }
                                    }
                                    break;
                                case "ǰ������":       //���������
                                    if (!boxStatu.FrontGDError)
                                    {
                                        boxStatu.FrontGDError = true;
                                        //���ʹ��󵽽���
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), boxStatu.GetErrorString());
                                        //�������
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                    }
                                    break;

                                case "�����ڵ�":
                                case "����δ�ڵ�":
                                    {
                                        if (boxStatu.BackGDError)
                                        {
                                            boxStatu.BackGDError = false;
                                            string errstr = boxStatu.GetErrorString();
                                            //���ʹ��󵽽���
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //������
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                            }
                                        }
                                    }
                                    break;
                                case "�������":       //���������
                                    if (!boxStatu.BackGDError)
                                    {
                                        boxStatu.BackGDError = true;
                                        //���ʹ��󵽽���
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), boxStatu.GetErrorString());
                                        //�������
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                    }
                                    break;

                                case "��������ڵ�":
                                case "������δ�ڵ�":
                                    {
                                        string err1 = boxStatu.GetErrorString();
                                        if (boxStatu.FullGDError)
                                        {
                                            boxStatu.FullGDError = false;
                                        }
                                        if (data == "��������ڵ�")
                                            boxStatu.FullGDZheDang = true;
                                        else
                                            boxStatu.FullGDZheDang = false;
                                        string errstr = boxStatu.GetErrorString();
                                        //���ʹ��󵽽���
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                        if (errstr.CompareTo("") == 0 && err1.CompareTo("") != 0)
                                        {
                                            //������
                                            BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                            BoxDataParse.DataParse.CmdLCDText(BN_NO, 3, enum_TextType.��ʾ�������ı�, 0, 1, " ");
                                        }
                                        else if (errstr.CompareTo("") != 0)
                                        {
                                            //�������
                                            BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                            if (data == "��������ڵ�")
                                                BoxDataParse.DataParse.CmdLCDText(BN_NO, 3, enum_TextType.��ʾ�������ı�, 0, 255, errstr);
                                        }
                                    }
                                    break;
                                case "����������":      //���������
                                    if (!boxStatu.FullGDError)
                                    {
                                        boxStatu.FullGDError = true;
                                        //���ʹ��󵽽���
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), boxStatu.GetErrorString());
                                        //�������
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                    }
                                    break;

                                case "��չ���ڵ�":
                                case "��չ��δ�ڵ�":
                                    {
                                        if (data == "��չ���ڵ�")
                                            boxStatu.ClearGDState = true;
                                        else
                                            boxStatu.ClearGDState = false;

                                        if (boxStatu.ClearGDError)
                                        {
                                            boxStatu.ClearGDError = false;
                                            string errstr = boxStatu.GetErrorString();
                                            //���ʹ��󵽽���
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //������
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                            }
                                        }
                                    }
                                    break;
                                case "��չ�����":      //���������
                                    if (!boxStatu.ClearGDError)
                                    {
                                        boxStatu.ClearGDError = true;
                                        //���ʹ��󵽽���
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), boxStatu.GetErrorString());
                                        //�������
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                    }
                                    break;

                                case "�������δ�ڵ�":
                                case "��������ڵ�":
                                    {
                                        if (boxStatu.ChufaGDError)
                                        {
                                            boxStatu.ChufaGDError = false;
                                            string errstr = boxStatu.GetErrorString();
                                            //���ʹ��󵽽���
                                            SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), errstr);
                                            if (errstr.CompareTo("") == 0)
                                            {
                                                //������
                                                BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                            }
                                        }
                                    }
                                    break;
                                case "����������":      //���������
                                    if (!boxStatu.ChufaGDError)
                                    {
                                        boxStatu.ChufaGDError = true;
                                        //���ʹ��󵽽���
                                        SendClientNotify.NotifyError(boxStatu.BoxNO.ToString("000"), boxStatu.GetErrorString());
                                        //�������
                                        BoxDataParse.DataParse.CmdBoxLampCuoWu(BN_NO, enum_LampStatus.��);
                                    }
                                    break;
                            }
                            #endregion
                            List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                            foreach (GroupStatus g in groups)
                            {
                                //����Ϣ����
                                g.GdStateChange(boxStatu.BoxNO, data);
                            }
                        }
                        break;
                    #endregion

                    #region ��������
                    case ReceiveDataType.����:
                        {
                            int boxNo = 0;
                            if (boxStatu != null)
                            {
                                boxNo = boxStatu.BoxNO;
                                //������Ļ
                                boxStatu.IsCloseScreen = false;
                            }

                            if (m_isNewsPaper)
                                News_PushButton(boxNo);
                            else if (boxNo > 0)
                            {
                                List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                                foreach (GroupStatus g in groups)
                                {
                                    //����Ϣ����
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

                    #region ����
                    case ReceiveDataType.������Ϣ:
                        //����Ϣ����
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];
                            string BarCode = CusTrim(data);
                            if (BarCode.Length > 1)
                                gs.GetBarCode(BarCode);
                        }
                        break;
                    case ReceiveDataType.������Ϣ_4Dai:
                        //����Ϣ����
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];
                            string BarCode = CusTrim(data);
                            if (BarCode.Length > 1)
                                gs.GetBarCode_4Dai(BarCode);
                        }
                        break;
                    #endregion

                    #region Ͷ��Ͷ��
                    case ReceiveDataType.Ͷ��Ͷ��:
                        {
                            int ShowCount = Convert.ToInt32(data);
                            List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                            foreach (GroupStatus g in groups)
                            {
                                //������Ϣ
                                g.LetterIn(boxStatu.BoxNO, ShowCount);
                            }
                        }
                        break;
                    #endregion
                    #region Ͷ�����
                    case ReceiveDataType.Ͷ�����:
                        {
                            int ShowCount = Convert.ToInt32(data);
                            List<GroupStatus> groups = GetGroupFromBoxBN(BN_NO);
                            foreach (GroupStatus g in groups)
                            {
                                //������Ϣ
                                g.LetterOut(boxStatu.BoxNO, ShowCount);
                            }
                        }
                        break;
                    #endregion

                    #region ֤������
                    case ReceiveDataType.֤����Ϣ_4Dai:
                    case ReceiveDataType.֤����Ϣ:
                        //����Ϣ����
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

                    #region ֤����Ϣ_ָ����
                    case ReceiveDataType.֤����Ϣ_ָ����:
                        //����Ϣ����
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];

                            gs.GetZhiJingMaiInfo(data, 0);
                        }
                        break;

                    case ReceiveDataType.֤����Ϣ_����ָ����:
                        //����Ϣ����
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];

                            gs.GetZhiJingMaiInfo(data, 1);
                        }
                        break;

                    case ReceiveDataType.ָ��������_��֤:
                        //����Ϣ����
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];

                            gs.GetZhiJingMaiId(data);
                        }
                        break;
                    #endregion

                    #region ȡ����λѡ��
                    case ReceiveDataType.ȡ����λѡ��:
                        //����Ϣ����
                        if (groupstatus.ContainsKey(BN_NO))
                        {
                            GroupStatus gs = groupstatus[BN_NO];
                            string[] ids = data.Split(',');
                            List<int> idArray = new List<int>();
                            foreach (string id in ids)
                            {
                                idArray.Add(Convert.ToInt32(id));
                            }
                            gs.ȡ����λѡ��(idArray);
                        }
                        break;
                    #endregion


                    #region �ϱ��������Ƭ
                    case ReceiveDataType.�ϱ��������Ƭ:
                        //����Ϣ����
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
                Log.WriteInfo(LogType.Error, "������ͷ�˿ڣ�" + e.ToString() + "\r\n");
            }

        }
        #endregion

        #region ��ʼ������״̬
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

                //��ʼ������Ϣ
                DataBase.MonitorService.GroupSetInfoClass[] groups = DataBase.DataSave.GetGroupSetInfo();
                if (groups.Length <= 0)
                {
                    boxstatus.Clear();
                    return false;
                }
                for (i = 0; i < groups.Length; i++)
                {
                    GroupStatus gs = new GroupStatus(groups[i], true, boxstatus, groupstatus);
                    //�û�Ͷ���¼�
                    gs.OnLetterIn += new BoxLetterCountChange(Box_OnLetterIn);
                    gs.GetGroupFromBoxNo += new DGetGroupFromBoxNo(gs_GetGroupNameFromBoxNo);
                    if (groups[i].GroupBackScanBN != null && groups[i].GroupBackScanBN != "")
                    {
                        gs = new GroupStatus(groups[i], false, boxstatus, groupstatus);
                        //�û�Ͷ���¼�
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

        #region �ж���ͷ����״̬
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
                    //������Ļ
                    if (Constant.TimeOut_CloseScreen > 0)
                    {
                        bi.FrontBox.CloseScreen();
                        bi.BackBox.CloseScreen();
                    }
                    //��ȡ���ݿ⣬������ͷ�ż���Ŀ���Լ�������״̬
                    DataBase.MonitorService.BoxInfo box = DataBase.DataSave.GetBoxLetterCount(bi.BoxNO);

                    if (box != null)
                    {
                        bi.BoxProperty = (EnumBoxStat)box.BoxProperty;
                        bi.IsQingTuiXiang = box.IsQingTuiXiang;
                        if (box.BoxShowName != bi.BoxUnitName)
                        {
                            bi.BoxUnitName = box.BoxShowName;
                            BoxDataParse.DataParse.CmdLCDText(bi.FrontBox.BoxBN, 0, enum_TextType.��ʾ�������ı�, 0, 255, box.BoxShowName);
                        }
                        //if (box.BoxShowFullName != bi.BoxUnitFullName)
                        //{
                        //    bi.BoxUnitFullName = box.BoxShowFullName;
                        //    BoxDataParse.DataParse.CmdLCDText(bi.FrontBox.BoxBN, 2, enum_TextType.��ʾ�������ı�, 0, 255, box.BoxShowFullName);
                        //    if (bi.BackBox.BoxBN != "")
                        //        BoxDataParse.DataParse.CmdLCDText(bi.BackBox.BoxBN, 2, enum_TextType.��ʾ�������ı�, 0, 255, box.BoxShowFullName);
                        //}
                        if (box.SendCount != bi.m_LetterCount)// && (box.SendCount != 0 || LogInfo.Constant.m_IsPiaoJuXiang)
                        {
                            bi.LetterCount = box.SendCount;
                            BoxDataParse.DataParse.CmdBoxLED(bi.FrontBox.BoxBN, 1, box.SendCount, enum_LedColor.��ɫ);
                            if (bi.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdBoxLED(bi.BackBox.BoxBN, 1, box.SendCount, enum_LedColor.��ɫ);
                        }
                        if (box.HasJiJian != bi.LampJiJian && box.SendCount != 0)
                        {
                            bi.LampJiJian = box.HasJiJian;
                            BoxDataParse.DataParse.CmdBoxLampJiJian(bi.FrontBox.BoxBN, enum_LampStatus.��);
                            if (bi.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdBoxLampJiJian(bi.BackBox.BoxBN, enum_LampStatus.��);
                        }
                        if (bi.BackBox.BoxBN != "")
                            BoxDataParse.DataParse.CmdLCDText(bi.BackBox.BoxBN, 0, enum_TextType.��ʾ�������ı�, 0, 255, box.BoxShowName);
                        if (!string.IsNullOrEmpty(box.BoxShowFullName))
                        {
                            BoxDataParse.DataParse.CmdLCDText(bi.FrontBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 255, box.BoxShowFullName);
                        }
                    }
                    else
                    {
                        //������ͷ
                        if (bi.BoxUnitName != Constant.EmptyBoxName)
                        {
                            bi.BoxUnitName = Constant.EmptyBoxName;
                            BoxDataParse.DataParse.CmdLCDText(bi.FrontBox.BoxBN, 0, enum_TextType.��ʾ�������ı�, 0, 255, bi.BoxUnitName);
                            if (bi.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(bi.BackBox.BoxBN, 0, enum_TextType.��ʾ�������ı�, 0, 255, bi.BoxUnitName);

                            bi.BoxUnitFullName = Constant.EmptyBoxName;
                            BoxDataParse.DataParse.CmdLCDText(bi.FrontBox.BoxBN, 2, enum_TextType.��ʾ�������ı�, 0, 255, bi.BoxUnitName);
                            if (bi.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(bi.BackBox.BoxBN, 2, enum_TextType.��ʾ�������ı�, 0, 255, bi.BoxUnitName);
                        }
                    }

                    //�쵼������Ϣ
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


        #region �õ��쵼������Ϣ
        private void GetLeaderOutMessage()
        {
            try
            {
                DataBase.MonitorService.ClassBoxShowMessage outMessage;
                //��ѯ���������쵼������Ϣ
                foreach (BoxInfo box in boxstatus.Values)
                {
                    outMessage = DataBase.DataSave.GetBoxShowMessage(box.BoxNO);
                    //������Ϣ�仯�Ĵ��������Ĳ������仯����������ȡ��
                    if (outMessage.������Ϣ.CompareTo(box.LeaderOutMessage) != 0)
                    {
                        box.LeaderOutMessage = outMessage.������Ϣ;
                        if (outMessage.������Ϣ.CompareTo("") == 0)
                        {
                            //���������ʾ
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 0, "");
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 0, "");
                        }
                        else
                        {
                            //���������޸ĳ�����Ϣ
                            BoxDataParse.DataParse.CmdLCDText(box.FrontBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 255, box.LeaderOutMessage);
                            if (box.BackBox.BoxBN != "")
                                BoxDataParse.DataParse.CmdLCDText(box.BackBox.BoxBN, 3, enum_TextType.��ʾ�������ı�, 0, 255, box.LeaderOutMessage);
                        }
                    }
                }
                //�пصĹ�����Ϣ
                outMessage = DataBase.DataSave.GetBoxShowMessage(0);
                if (outMessage.������Ϣ.CompareTo("") != 0)
                {
                    foreach (GroupStatus gs in groupstatus.Values)
                    {
                        gs.SetGongGaoInfo(outMessage.������Ϣ);
                    }
                }
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
            }
        }
        #endregion

        #region �û�Ͷ�ź�֪ͨ���溯��
        private void Box_OnLetterIn(GroupStatus g, int BoxNO)
        {
            try
            {
                if (!boxstatus.ContainsKey(BoxNO)) return;
                //֪ͨ����ؽ���
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

        #region �����ַ�������Ͷ�����еĴ���
        private void News_PushButton(int boxno)
        {
            try
            {
                //�жϷַ�������û����
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

                //���ñ���ΪͶ�룬֪ͨҵ��
                if (Constant.NewspaperType == 1)
                {
                    if (!bCurrent.bSendOK)
                    {
                        bCurrent.bSendOK = true;
                        SendClientNotify.NotifyNewsState(boxno.ToString("000"), 1);
                        //����ȡ���𼱼�
                        //�����������ͷ����ĿΪ0�������κδ���
                        if (bCurrent.PaperNum > 0)
                        {
                            if (boxstatus[boxno].BackBox.BoxBN == "")
                                BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].FrontBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.��ɫ);
                            else
                                BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].BackBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.��ɫ);
                        }
                    }
                    else
                    {
                        //��ת
                        bCurrent.bSendOK = false;
                        SendClientNotify.NotifyNewsState(boxno.ToString("000"), 0);
                        //������������ȡ
                        //�����������ͷ����ĿΪ0�������κδ���
                        if (bCurrent.PaperNum > 0)
                        {
                            if (boxstatus[boxno].BackBox.BoxBN == "")
                                BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].FrontBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.��ɫ);
                            else
                                BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].BackBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.��ɫ);
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
                                //����ȡ���𼱼�
                                //�����������ͷ����ĿΪ0�������κδ���
                                if (bCurrent.PaperNum > 0)
                                {
                                    if (boxstatus[boxno].BackBox.BoxBN == "")
                                        BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].FrontBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.��ɫ);
                                    else
                                        BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].BackBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.��ɫ);
                                }
                            }
                        }
                        else
                        {
                            //��ת
                            if (b.bSendOK)
                            {
                                b.bSendOK = false;
                                SendClientNotify.NotifyNewsState(b.BoxNo.ToString("000"), 0);
                                //����ȡ���𼱼�
                                //�����������ͷ����ĿΪ0�������κδ���
                                if (bCurrent.PaperNum > 0)
                                {
                                    if (boxstatus[boxno].BackBox.BoxBN == "")
                                        BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].FrontBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.��ɫ);
                                    else
                                        BoxDataParse.DataParse.CmdBoxLED(boxstatus[boxno].BackBox.BoxBN, 1, bCurrent.PaperNum, enum_LedColor.��ɫ);
                                }
                            }
                        }
                        if (b == bCurrent) break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteInfo(LogType.Error, "�����ַ���������" + e.ToString() + "\r\n");
            }
        }
        #endregion

    }
}
