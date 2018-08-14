using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DataBase;

namespace DataProcess
{
    public delegate void BoxLetterCountChange(GroupStatus g, int boxNo);
    public delegate GroupStatus DGetGroupFromBoxNo(int boxNo);

    /// <summary>
    /// GroupStatus ��ժҪ˵����
    /// </summary>
    public class GroupStatus
    {
        public int ���Ź���Ա֤����� = 0;

        #region ϵͳ״̬����
        public enum EnumGroupStatus
        {
            ���� = 1,
            ��ʾ��Ϣ��ʾ = 101,
            Ͷ�䱨�� = 102,
            ȡ��δ��ձ��� = 103,
            ����ͨͶ = 2,
            Ͷ�俪�Ž� = 3,
            ͨ�뷽ʽѡ�� = 4,
            ͨ���շ� = 5,
            ͨ��ģ���б� = 6,
            ͨ��ģ���б�ȷ�� = 8,
            ͨ��ַ� = 7,

            �û�����Ͷ��Pre = 9,
            �û�����Ͷ�� = 10,

            �û�ȡ������ = 11,
            �û�ȡ�����Ų���ȡ�� = 12,
            �û�ȡ��ȡ��ƽ̨ = 13,
            �û�ȡ��ȡ��ƽ̨��λѡ�� = 14,

            ����ά�� = 21,
            ά������ = 22,
            ������ = 23,
            ������ = 24,

            ���� = 31,
            ����ͨͶ = 32,
            ���˿��Ž� = 33
        }
        #endregion

        #region �������Ϣ��
        public class StBoxInfo
        {
            public string BoxBn;
            public int BoxNo;
            public bool BUsed;
            public int Count;
            public int SendCount;
            public BoxInfo Box;

            private bool _bFront;

            public StBoxInfo(bool bFront)
            {
                BoxBn = "";
                BoxNo = 0;
                BUsed = false;
                Count = SendCount = 0;
                this._bFront = bFront;
            }

            public bool CanGetLetter
            {
                get
                {
                    if (_bFront)
                    {
                        return Box.FrontBox.CanGetLetter();
                    }
                    else
                    {
                        return Box.BackBox.CanGetLetter();
                    }
                }
            }

            public BoxInfo.BoxStatus BoxStatus
            {
                get
                {
                    if (_bFront)
                    {
                        return Box.FrontBox;
                    }
                    else
                    {
                        return Box.BackBox;
                    }
                }
            }
        }
        #endregion

        //�û�Ͷ�ź�֪ͨ������������¼�
        public event BoxLetterCountChange OnLetterIn;
        public event DGetGroupFromBoxNo GetGroupFromBoxNo;

        //����Ϣ
        public bool BFront;
        public string GroupName;//��������
        public string GroupScanBn;//ǰ��ɨ��ͷbn��
        public string GroupCardBn;//ǰ�������bn��
        public string GroupShowBn;//ǰ��๦����bn��
        public string GroupSoundBn;//ǰ������BN��
        public string GroupZhiJingMaiBn;//ǰ��๦����bn��
        public string GroupSheXiangTouBn;//ǰ������BN��

        public string PrinterName;//�嵥��ӡ��

        private string _mDvrip = "";    //����������ͷʹ�õ�DVR¼�����Ip��ַ
        private int _mDvrPort = 0;     //����������ͷʹ�õ�DVR¼�����Ip�˿�
        private int _mDvrChannel = 0;  //����������ͷʹ�õ�DVR¼���������ͷ�˿ڣ���һ·Ϊ0����������

        private bool _mBStartRec;
        private string _mRecFileName;
        private string _mSnapFileName;

        public EnumGroupStatus MCurrentState = EnumGroupStatus.����;
        public Dictionary<int, StBoxInfo> MBoxInfo = new Dictionary<int, StBoxInfo>();
        public string[] MDaiGuanGroup;

        /// <summary>
        /// �Ƿ�������ȡ��������ǣ���û����ͷ
        /// </summary>
        public bool MIsQuJianPingTai;
        Dictionary<int, BoxInfo> _mBoxstatus;
        Dictionary<string, GroupStatus> _mGroupstatus;

        /// <summary>
        /// ������Ϣ
        /// </summary>
        private string _mGongGaoInfo = "";

        private string _mLetterBarCode = "";
        /// <summary>
        /// ���ڱ������ȡ��֤����֤�����
        /// </summary>
        private string _mUserBarCode = "";
        /// <summary>
        /// ���ڱ������ȡ��֤����֤�����
        /// </summary>
        private string _mAdminBarCode = "";
        private bool _mBJiaJi = false;
        private uint _mDtLastOperation;

        /// <summary>
        /// �û����֤���󣬷��ص�֤������
        /// </summary>
        private BarCodeType _mLastCheckCard;
        /// <summary>
        /// �û����֤���󣬷��صĿ��Դ򿪵�����б�
        /// </summary>
        private List<UserGetBoxInfo> _mUserBoxs;
        /// <summary>
        /// �û�ȡ��������12����ʱ��һ��Ļ��ʾ��ȫ��
        /// ���ʱ���ٴ�ˢ�����������12����λ��Ϣ��
        /// </summary>
        private int _mUserBoxsIndex;

        private DataBase.MonitorService.ClassBarcodeMuBanList[] _mCurrentMoBan;
        private int _mCurIndex;

        /// <summary>
        /// �������״̬
        /// </summary>
        private bool _mBChufa;

        /// <summary>
        /// �ǲ���4����Arm��ͷ��Ϊ4s�ļ���ʹ��
        /// </summary>
        private bool _mIs4Dai;

        private string _mJingGaoString;
        private List<int> _mJingGaoBoxNo;


        #region ���캯������ʼ��������Ϣ
        public GroupStatus(DataBase.MonitorService.GroupSetInfoClass group, bool bFront, Dictionary<int, BoxInfo> boxstatus, Dictionary<string, GroupStatus> groupstatus)
        {
            _mRecFileName = "";

            _mJingGaoString = "";
            _mJingGaoBoxNo = new List<int>();

            _mIs4Dai = false;
            _mBoxstatus = boxstatus;
            _mGroupstatus = groupstatus;

            this.BFront = bFront;
            GroupName = group.GroupName;
            if (bFront)
            {
                GroupScanBn = group.GroupFrontScanBN;
                GroupShowBn = group.GroupFrontShowBN;
                GroupSoundBn = group.GroupFrontSoundBN;
                GroupCardBn = group.GroupFrontCardBN;
                if (!string.IsNullOrEmpty(group.GroupFrontZhiJingMaiBN))
                    GroupZhiJingMaiBn = group.GroupFrontZhiJingMaiBN;
                else
                    GroupZhiJingMaiBn = group.GroupFrontCardBN;
                if (!string.IsNullOrEmpty(group.GroupFrontSheXiangTouBN))
                    GroupSheXiangTouBn = group.GroupFrontSheXiangTouBN;
                else
                    GroupSheXiangTouBn = group.GroupFrontShowBN;
            }
            else
            {
                GroupScanBn = group.GroupBackScanBN;
                GroupShowBn = group.GroupBackShowBN;
                GroupSoundBn = group.GroupBackSoundBN;
                GroupCardBn = group.GroupBackCardBN;
                if (!string.IsNullOrEmpty(group.GroupBackZhiJingMaiBN))
                    GroupZhiJingMaiBn = group.GroupBackZhiJingMaiBN;
                else
                    GroupZhiJingMaiBn = group.GroupBackCardBN;
                if (!string.IsNullOrEmpty(group.GroupBackSheXiangTouBN))
                    GroupSheXiangTouBn = group.GroupBackSheXiangTouBN;
                else
                    GroupSheXiangTouBn = group.GroupBackShowBN;
            }

            if (String.IsNullOrEmpty(group.PrinterName))
                PrinterName = "";
            else
                PrinterName = group.PrinterName;
            if (group.GroupNameArray != null)
                MDaiGuanGroup = group.GroupNameArray;
            else
                MDaiGuanGroup = new String[0];

            _mBStartRec = false;

            //��ӵ����б���
            if (!groupstatus.ContainsKey(GroupScanBn))
                groupstatus.Add(GroupScanBn, this);
            if (!groupstatus.ContainsKey(GroupShowBn))
                groupstatus.Add(GroupShowBn, this);
            if (!groupstatus.ContainsKey(GroupCardBn))
                groupstatus.Add(GroupCardBn, this);
            if (!groupstatus.ContainsKey(GroupZhiJingMaiBn))
                groupstatus.Add(GroupZhiJingMaiBn, this);
            if (!groupstatus.ContainsKey(GroupSheXiangTouBn))
                groupstatus.Add(GroupSheXiangTouBn, this);

            MIsQuJianPingTai = false;
            try
            {
                //��ʼ���豸��
                for (int i = 0; i < group.BoxArray.Length; i++)
                {
                    int boxNo = Int32.Parse(group.BoxArray[i]);
                    if (boxstatus.ContainsKey(boxNo))
                    {
                        StBoxInfo box = new StBoxInfo(bFront);
                        box.BoxNo = boxNo;
                        box.BUsed = false;
                        box.Box = boxstatus[boxNo];
                        if (bFront)
                            box.BoxBn = box.Box.FrontBox.BoxBN;
                        else
                            box.BoxBn = box.Box.BackBox.BoxBN;
                        MBoxInfo.Add(boxNo, box);
                    }
                }
            }
            catch
            {
                // ignored
            }
            if (MBoxInfo.Count <= 0)
                MIsQuJianPingTai = true;
            OnLetterIn = null;
        }
        #endregion

        #region  ����״̬
        private bool _mConnected;		//����״̬
        private UInt32 _dtConnect;

        public bool Connected
        {
            get
            {
                if (_mConnected)
                {
                    UInt32 ts = LogInfo.Win32API.GetTickCount() - _dtConnect;
                    ts /= 1000;
                    if (ts >= LogInfo.Constant.ConnectTimeOut)
                        _mConnected = false;
                }
                return _mConnected;
            }
            set
            {
                if (_mConnected)
                {
                    UInt32 ts = LogInfo.Win32API.GetTickCount() - _dtConnect;
                    ts /= 1000;
                    if (ts >= LogInfo.Constant.ConnectTimeOut)
                        _mConnected = false;
                }

                if (!_mConnected)
                {
                    //���з����ʼ��
                    foreach (StBoxInfo box in MBoxInfo.Values)
                    {
                        if (BFront && box.Box.FrontBox.Connected)
                            BoxDataParse.DataParse.CmdBoxToIdle(box.BoxBn);
                        else if (!BFront && box.Box.BackBox.Connected)
                            BoxDataParse.DataParse.CmdBoxToIdle(box.BoxBn);
                    }
                    if (MBoxInfo.Count > 1)
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                        if (GroupScanBn != GroupShowBn)
                            BoxDataParse.DataParse.CmdBoxToIdle(GroupScanBn);

                        string str = LogInfo.NotifyMsg.GetText((int)EnumGroupStatus.����);
                        if (str == "") str = "��ɨ������";
                        str = GroupName + "\r\n" + str;

                        if (!_mIs4Dai)
                            BoxDataParse.DataParse.CmdLCDText_Com(GroupShowBn, 0, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, LogInfo.Constant.TextLeft, LogInfo.Constant.TextTop, str);
                    }
                    CurrentState = EnumGroupStatus.����;
                }
                _dtConnect = LogInfo.Win32API.GetTickCount();
                _mConnected = value;
            }
        }
        #endregion

        #region ����߼�����Ƿ����ڱ���
        public bool CheckBoxInGroup(int boxNo)
        {
            return MBoxInfo.ContainsKey(boxNo);
        }
        #endregion

        #region  ״̬�ı�
        public EnumGroupStatus CurrentState
        {
            get
            {
                return MCurrentState;
            }
            set
            {
                _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                #region ����ͷ��Ӧ��Ļ
                if (value != EnumGroupStatus.����)
                {
                    foreach (StBoxInfo box in MBoxInfo.Values)
                    {
                        box.BoxStatus.IsCloseScreen = false;
                    }
                }
                #endregion
                MCurrentState = value;
                if (value == EnumGroupStatus.���� || value == EnumGroupStatus.��ʾ��Ϣ��ʾ)
                {
                    if (OnLetterIn != null)
                    {
                        OnLetterIn -= new BoxLetterCountChange(g_OnLetterIn);
                        OnLetterIn = null;
                    }
                }
                if (value == EnumGroupStatus.����)
                {
                    #region
                    foreach (StBoxInfo b in MBoxInfo.Values)
                    {
                        if (b.BUsed && b.BoxBn != GroupScanBn)
                        {
                            BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                            //����
                            BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.��ɫ);
                            b.BUsed = false;
                        }
                        //������Ϣ
                        if (b.Box.LeaderOutMessage.CompareTo("") != 0)
                        {
                            BoxDataParse.DataParse.CmdLCDText(b.BoxBn, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, b.Box.LeaderOutMessage);
                        }
                        else if (b.Box.bSyncError)
                        {
                            BoxDataParse.DataParse.CmdLCDText(b.BoxBn, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, b.Box.GetSyncErrorMsg);
                        }
                        else if (b.BoxStatus.FullGDZheDang)
                        {
                            BoxDataParse.DataParse.CmdLCDText(b.BoxBn, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, b.BoxStatus.GetErrorString());
                        }
                    }

                    if (MIsQuJianPingTai)
                    {
                        BoxDataParse.DataParse.CmdQuJian(GroupShowBn, 0);
                    }
                    else
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                        if (GroupShowBn != GroupScanBn)
                            BoxDataParse.DataParse.CmdBoxToIdle(GroupScanBn);
                        if (MBoxInfo.Count > 1)
                        {
                            string str = LogInfo.NotifyMsg.GetText((int)EnumGroupStatus.����);
                            if (str == "") str = "��ɨ������";
                            str = GroupName + "\r\n" + str;
                            if (_mGongGaoInfo != "")
                                str = _mGongGaoInfo;
                            if (!_mIs4Dai)
                            {
                                BoxDataParse.DataParse.CmdLCDText_Com(GroupShowBn, 0, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, LogInfo.Constant.TextLeft, LogInfo.Constant.TextTop, str);
                            }
                        }
                        if (MBoxInfo.Count > 0)
                        {
                            PlaySound((int)EnumGroupStatus.����);
                            if (_mBStartRec)
                            {
                                DvrCommand.StopRec(_mDvrip, _mDvrPort, _mDvrChannel);
                                _mBStartRec = false;
                            }
                            if (_mBChufa)
                            {
                                BoxDataParse.DataParse.CmdBoxScan(GroupScanBn, true);
                            }
                        }
                    }
                    #endregion
                }
                else if (value == EnumGroupStatus.��ʾ��Ϣ��ʾ)
                {
                    if (_mBStartRec)
                    {
                        DvrCommand.StopRec(_mDvrip, _mDvrPort, _mDvrChannel);
                        _mBStartRec = false;
                    }
                    if (_mBChufa)
                    {
                        BoxDataParse.DataParse.CmdBoxScan(GroupScanBn, true);
                    }
                }
                else if (value == EnumGroupStatus.Ͷ�䱨�� || value == EnumGroupStatus.ȡ��δ��ձ���)
                {
                    BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                    foreach (int bno in _mJingGaoBoxNo)
                    {
                        ShowMsgToBox(_mJingGaoString, MBoxInfo[bno].BoxBn, 255);
                        BoxDataParse.DataParse.CmdBoxLampCuoWu(MBoxInfo[bno].BoxBn, LogInfo.enum_LampStatus.��˸);
                        BoxDataParse.DataParse.CmdBuzzer(MBoxInfo[bno].BoxBn, 3);
                    }
                }
                else
                {
                    #region ץͼ ¼��
                    //ץͼ
                    if (MCurrentState == EnumGroupStatus.�û�ȡ������)
                    {
                        if (!string.IsNullOrEmpty(_mDvrip))
                        {
                            bool b = DvrCommand.GetPicture(_mDvrip, _mDvrPort, _mDvrChannel, ref _mSnapFileName);
                            if (!b) _mSnapFileName = "";
                        }
                    }
                    //¼��
                    if (MCurrentState == EnumGroupStatus.����ͨͶ || MCurrentState == EnumGroupStatus.Ͷ�俪�Ž�
                        || MCurrentState == EnumGroupStatus.����ͨͶ || MCurrentState == EnumGroupStatus.���˿��Ž�
                        || MCurrentState == EnumGroupStatus.ͨ��ַ� || MCurrentState == EnumGroupStatus.ͨ�뷽ʽѡ��)
                    {
                        if (!_mBStartRec && !String.IsNullOrEmpty(_mDvrip))
                        {
                            _mRecFileName = "";
                            bool b = DvrCommand.StartRec(_mDvrip, _mDvrPort, _mDvrChannel, ref _mRecFileName);
                            if (!b) _mRecFileName = "";
                            _mBStartRec = true;
                        }
                    }
                    if (MCurrentState == EnumGroupStatus.����)
                    {
                        if (_mBStartRec)
                        {
                            DvrCommand.StopRec(_mDvrip, _mDvrPort, _mDvrChannel);
                            _mBStartRec = false;
                        }
                    }
                    #endregion

                    #region �������ı���ʾ
                    if (MCurrentState == EnumGroupStatus.����ͨͶ && MBoxInfo.Count == 1)
                    {
                    }
                    else if (GroupShowBn.Substring(5, 2) == "98")
                    {
                        ShowMsg((int)value);
                    }
                    PlaySound((int)value);
                    #endregion
                }
            }
        }
        #endregion

        #region �õ�����ź�Ĵ����������ż�������Ա������Ա����
        public void GetBarCode_4Dai(string barcode)
        {
            _mIs4Dai = true;
            GetBarCode(barcode);
        }
        /// <summary>
        /// �õ�����ź�Ĵ����������ż�������Ա������Ա����
        /// </summary>
        /// <param name="data">����ֵ</param>
        public void GetBarCode(string barcode)
        {
            GetBarCode(barcode, false);
        }

        /// <summary>
        /// �õ�����ź�Ĵ����������ż�������Ա������Ա����
        /// </summary>
        /// <param name="barcode">����ֵ</param>
        /// <param name="bToIdle">����ʱ���Ƿ���ת����Idle��Ļ</param>
        public void GetBarCode(string barcode, bool bToIdle)
        {
            BoxDataParse.DataParse.CmdBoxScan(GroupScanBn, false);
            //�ж����������
            BarCodeType barCodeType = BarCodeType.��Ч;
            List<SendBoxList> boxs;
            barCodeType = DataSave.CheckBarCodeType(barcode, out boxs);

            ProcessBarCode(barcode, bToIdle, barCodeType, boxs);
        }

        /// <summary>
        /// �õ�����ź�Ĵ����������ż�������Ա������Ա����
        /// </summary>
        /// <param name="data">����ֵ</param>
        public void GetBarCode(string barcode, List<SendBoxList> boxs)
        {
            ProcessBarCode(barcode, false, BarCodeType.ͨ��ַ�, boxs);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="bToIdle"></param>
        /// <param name="barCodeType"></param>
        /// <param name="boxs"></param>
        private void ProcessBarCode(string barcode, bool bToIdle, BarCodeType barCodeType, List<SendBoxList> boxs)
        {
            if (CurrentState == EnumGroupStatus.���� || CurrentState == EnumGroupStatus.��ʾ��Ϣ��ʾ)
            {
                #region
                switch (barCodeType)
                {
                    case BarCodeType.��Ч:
                        {
                            if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                            ShowMsg(41, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(41);
                        }
                        break;

                    case BarCodeType.û��Ԥ��:
                        {
                            if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                            ShowMsg(41, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(41);
                        }
                        break;
                    case BarCodeType.����û��Ԥ��:
                        {
                            if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                            ShowMsg(61, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(61);
                        }
                        break;

                    case BarCodeType.�����Ѿ�Ͷ��:
                        {
                            if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                            ShowMsg(43, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(43);
                        }
                        break;

                    case BarCodeType.ΨһֱͶ:
                        #region ΨһֱͶ
                        {
                            bool bHasOne = false;
                            bool bIsLock = false;
                            foreach (StBoxInfo tb in MBoxInfo.Values)
                            {
                                if (tb.CanGetLetter && !tb.Box.IsQingTuiXiang
                                    && !string.IsNullOrEmpty(tb.Box.BoxUnitName) && tb.Box.BoxUnitName != LogInfo.Constant.EmptyBoxName)
                                {
                                    if (tb.Box.CheckBoxProperty())
                                    {
                                        tb.BUsed = true;
                                        if (MBoxInfo.Count > 1)
                                            BoxDataParse.DataParse.CmdPreGetLetter(tb.BoxBn, 0, 2, tb.Box.LetterCount);
                                        else
                                            BoxDataParse.DataParse.CmdPreGetLetter(tb.BoxBn, 1, 2, tb.Box.LetterCount);
                                        bHasOne = true;
                                    }
                                    else
                                        bIsLock = true;
                                }
                            }
                            if (bHasOne)
                            {
                                _mLetterBarCode = barcode;
                                _mBJiaJi = false;
                                CurrentState = EnumGroupStatus.����ͨͶ;
                            }
                            else
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                if (bIsLock)
                                {
                                    ShowMsg(402, LogInfo.Constant.TimeOut_ShowInfoMsg);
                                    PlaySound(402);
                                }
                            }
                        }
                        #endregion
                        break;

                    case BarCodeType.Ψһָ��:
                        #region Ψһָ��
                        {
                            List<SendBoxList> sendBox = new List<SendBoxList>();
                            bool bGuZhang = false;
                            bool bIsLock = false;
                            string otherGroupname = "";
                            for (int ii = 0; ii < boxs.Count; ii++)
                            {
                                if (boxs[ii].BoxNo == 0)
                                    continue;
                                if (!MBoxInfo.ContainsKey(boxs[ii].BoxNo))
                                {
                                    if (LogInfo.Constant.NotifyGroupName)
                                    {
                                        otherGroupname = GetGroupFromBoxNo(boxs[ii].BoxNo).GroupName;
                                    }
                                    else
                                    {
                                        if (_mBoxstatus.ContainsKey(boxs[ii].BoxNo))
                                        {
                                            otherGroupname = _mBoxstatus[boxs[ii].BoxNo].BoxUnitName;
                                        }
                                    }
                                    continue;
                                }
                                if (MBoxInfo[boxs[ii].BoxNo].Box.CheckBoxProperty())
                                {
                                    //���û�й������û���Ž�����
                                    if (MBoxInfo[boxs[ii].BoxNo].CanGetLetter)
                                    {
                                        sendBox.Add(boxs[ii]);
                                    }
                                    else
                                        bGuZhang = true;
                                }
                                else
                                    bIsLock = true;
                            }
                            if (sendBox.Count > 0)   //�࿪
                            {
                                //ָ����ͷ�����룬׼��Ͷ��
                                foreach (SendBoxList b in sendBox)
                                {
                                    MBoxInfo[b.BoxNo].BUsed = true;
                                    if (sendBox.Count > 1)
                                        BoxDataParse.DataParse.CmdPreGetLetter(MBoxInfo[b.BoxNo].BoxBn, 0, 2, MBoxInfo[b.BoxNo].Box.LetterCount);
                                    else
                                        BoxDataParse.DataParse.CmdPreGetLetter(MBoxInfo[b.BoxNo].BoxBn, 1, 2, MBoxInfo[b.BoxNo].Box.LetterCount);
                                }
                                _mLetterBarCode = barcode;
                                _mBJiaJi = false;
                                CurrentState = EnumGroupStatus.����ͨͶ;
                            }
                            else if (bGuZhang)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //����������
                                ShowMsgToBox("�����ϲ���Ͷ��", GroupShowBn);
                            }
                            else if (bIsLock)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                ShowMsg(402, LogInfo.Constant.TimeOut_ShowInfoMsg);
                                PlaySound(402);
                            }
                            else
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //����������
                                ShowMsgToBox("�뵽" + otherGroupname + "Ͷ��", GroupShowBn);
                            }
                        }
                        #endregion
                        break;

                    case BarCodeType.Ψһָ������Ͷ��:
                        #region Ψһָ��_����Ͷ��
                        {
                            List<SendBoxList> sendBox = new List<SendBoxList>();
                            bool bGuZhang = false;
                            bool bIsLock = false;
                            string otherGroupname = "";
                            for (int ii = 0; ii < boxs.Count; ii++)
                            {
                                if (boxs[ii].BoxNo == 0)
                                    continue;
                                if (!MBoxInfo.ContainsKey(boxs[ii].BoxNo))
                                {
                                    if (LogInfo.Constant.NotifyGroupName)
                                    {
                                        otherGroupname = GetGroupFromBoxNo(boxs[ii].BoxNo).GroupName;
                                    }
                                    else
                                    {
                                        if (_mBoxstatus.ContainsKey(boxs[ii].BoxNo))
                                        {
                                            otherGroupname = _mBoxstatus[boxs[ii].BoxNo].BoxUnitName;
                                        }
                                    }
                                    continue;
                                }
                                if (MBoxInfo[boxs[ii].BoxNo].Box.CheckBoxProperty())
                                {
                                    //���û�й������û���Ž�����
                                    if (MBoxInfo[boxs[ii].BoxNo].CanGetLetter)
                                    {
                                        sendBox.Add(boxs[ii]);
                                    }
                                    else
                                        bGuZhang = true;
                                }
                                else
                                    bIsLock = true;
                            }
                            if (sendBox.Count == 1)
                            {
                                //ָ����ͷ�����룬׼��Ͷ��
                                foreach (SendBoxList b in sendBox)
                                {
                                    MBoxInfo[b.BoxNo].BUsed = true;
                                    //����
                                    BoxDataParse.DataParse.CmdOpenDoor(MBoxInfo[b.BoxNo].BoxBn, true);
                                }
                                _mLetterBarCode = barcode;
                                _mBJiaJi = false;
                                CurrentState = EnumGroupStatus.�û�����Ͷ��Pre;
                            }
                            else if (sendBox.Count > 1)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //�ҵ�����������
                                ShowMsgToBox("���벻Ψһ��������Ԥ��", GroupShowBn);
                            }
                            else if (bGuZhang)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //����������
                                ShowMsgToBox("�����ϲ���Ͷ��", GroupShowBn);
                            }
                            else if (bIsLock)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                ShowMsg(402, LogInfo.Constant.TimeOut_ShowInfoMsg);
                                PlaySound(402);
                            }
                            else
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //����������
                                ShowMsgToBox("�뵽" + otherGroupname + "Ͷ��", GroupShowBn);
                            }
                        }
                        #endregion
                        break;

                    case BarCodeType.ͨ��ַ�:
                        #region ͨ��ַ�
                        {
                            List<SendBoxList> sendBox = new List<SendBoxList>();
                            bool bGuZhang = false;
                            bool bIsLock = false;
                            string otherGroupname = "";
                            for (int ii = 0; ii < boxs.Count; ii++)
                            {
                                if (boxs[ii].BoxNo == 0)
                                    continue;
                                if (!MBoxInfo.ContainsKey(boxs[ii].BoxNo))
                                {
                                    if (LogInfo.Constant.NotifyGroupName)
                                    {
                                        otherGroupname = GetGroupFromBoxNo(boxs[ii].BoxNo).GroupName;
                                    }
                                    else
                                    {
                                        if (_mBoxstatus.ContainsKey(boxs[ii].BoxNo))
                                        {
                                            otherGroupname = _mBoxstatus[boxs[ii].BoxNo].BoxUnitName;
                                        }
                                    }
                                    continue;
                                }
                                if (MBoxInfo[boxs[ii].BoxNo].Box.CheckBoxProperty())
                                {
                                    //���û�й������û���Ž�����
                                    if (MBoxInfo[boxs[ii].BoxNo].CanGetLetter)
                                    {
                                        sendBox.Add(boxs[ii]);
                                    }
                                    else
                                        bGuZhang = true;
                                }
                                else
                                    bIsLock = true;
                            }
                            if (sendBox.Count > 0)   //�࿪
                            {
                                //ָ����ͷ�����룬׼��Ͷ��
                                foreach (SendBoxList b in sendBox)
                                {
                                    MBoxInfo[b.BoxNo].BUsed = true;
                                    BoxDataParse.DataParse.CmdPreGetLetter(MBoxInfo[b.BoxNo].BoxBn, 1, 2, b.Count);
                                    MBoxInfo[b.BoxNo].Count = b.Count;
                                    MBoxInfo[b.BoxNo].SendCount = 0;
                                }
                                _mLetterBarCode = barcode;
                                BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, false, false, true, false);
                                CurrentState = EnumGroupStatus.ͨ��ַ�;
                            }
                            else if (bGuZhang)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //����������
                                ShowMsgToBox("�����ϲ���Ͷ��", GroupShowBn);
                            }
                            else if (bIsLock)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                ShowMsg(402, LogInfo.Constant.TimeOut_ShowInfoMsg);
                                PlaySound(402);
                            }
                            else
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //����������
                                ShowMsgToBox("�뵽" + otherGroupname + "Ͷ��", GroupShowBn);
                            }
                        }
                        #endregion
                        break;

                    case BarCodeType.ͨ��ַ�ȫ��Ͷ��:
                        #region ͨ��ַ�_ȫ��Ͷ��
                        {
                            List<GroupStatus> allgroup = new List<GroupStatus>();
                            //�ҵ�������
                            for (int ii = 0; ii < boxs.Count; ii++)
                            {
                                if (boxs[ii].BoxNo == 0)
                                    continue;
                                GroupStatus g = GetGroupFromBoxNo(boxs[ii].BoxNo);
                                if (!allgroup.Contains(g))
                                    allgroup.Add(g);
                            }
                            //�ж��������ǲ��Ƕ�����
                            bool bIdle = true;
                            string msg = "";
                            foreach (GroupStatus g in allgroup)
                            {
                                if (g.CurrentState != EnumGroupStatus.���� && g.CurrentState != EnumGroupStatus.��ʾ��Ϣ��ʾ)
                                {
                                    msg = "��" + g.GroupName + "�����ڲ��������Ժ�Ͷ�䡣";
                                    bIdle = false;
                                    break;
                                }
                            }
                            if (!bIdle)
                            {
                                ShowMsgToBox(msg, GroupShowBn);
                                return;
                            }
                            //��������У�����Ͷ��ģʽ
                            _mAllgroup = allgroup;
                            foreach (GroupStatus g in allgroup)
                            {
                                g.OnLetterIn += new BoxLetterCountChange(g_OnLetterIn);
                                g.GetBarCode(barcode, boxs);
                            }
                        }
                        #endregion
                        break;

                    case BarCodeType.ͨ���շ�:
                        {
                            BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, true, false, true, false);
                            _mLetterBarCode = barcode;
                            CurrentState = EnumGroupStatus.ͨ�뷽ʽѡ��;
                        }
                        break;
                    case BarCodeType.ͨ��ģ��:
                        {
                            BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, false, true, true, false);
                            _mLetterBarCode = barcode;
                            CurrentState = EnumGroupStatus.ͨ�뷽ʽѡ��;
                        }
                        break;
                    case BarCodeType.ͨ���շ�ģ��:
                        {

                            BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, true, true, true, false);
                            _mLetterBarCode = barcode;
                            CurrentState = EnumGroupStatus.ͨ�뷽ʽѡ��;
                        }
                        break;

                    case BarCodeType.û��ָ���ַ�:
                        {
                            if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                            ShowMsg(60, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(60);
                        }
                        break;
                }
                #endregion
            }
            else if (CurrentState == EnumGroupStatus.����)
            {
                #region
                //switch (BarCodeType)
                //{
                //    case DataBase.BarCodeType.��Ч:
                //        {
                //            ShowMsg(41, LogInfo.Constant.TimeOut_ShowInfoMsg);
                //            PlaySound(41);
                //        }
                //        break;

                //    case DataBase.BarCodeType.ΨһֱͶ:
                //    case DataBase.BarCodeType.Ψһָ��:
                //    case DataBase.BarCodeType.ͨ��ַ�:
                //    case DataBase.BarCodeType.ͨ���շ�:
                //    case DataBase.BarCodeType.ͨ��ģ��:
                //    case DataBase.BarCodeType.ͨ���շ�ģ��:
                //        {
                //            foreach (stBoxInfo tb in m_BoxInfo.Values)
                //            {
                //                if (tb.CanGetLetter && tb.box.IsQingTuiXiang)
                //                {
                //                    tb.bUsed = true;
                //                    BoxDataParse.DataParse.CmdPreGetLetter(tb.BoxBN, 0, 2, tb.box.LetterCount);
                //                }
                //            }
                //            m_LetterBarCode = Barcode;
                //            m_bJiaJi = false;
                //            CurrentState = enumGroupStatus.����ͨͶ;
                //        }
                //        break;
                //}
                foreach (StBoxInfo tb in MBoxInfo.Values)
                {
                    if (tb.CanGetLetter && tb.Box.IsQingTuiXiang)
                    {
                        tb.BUsed = true;
                        BoxDataParse.DataParse.CmdPreGetLetter(tb.BoxBn, 0, 2, tb.Box.LetterCount);
                    }
                }
                _mLetterBarCode = barcode;
                _mBJiaJi = false;
                CurrentState = EnumGroupStatus.����ͨͶ;
                #endregion
            }
            else if (CurrentState == EnumGroupStatus.������)
            {
                #region
                switch (barCodeType)
                {
                    case BarCodeType.��Ч:
                        {
                            ShowMsg(41, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(41);
                        }
                        break;

                    case BarCodeType.ΨһֱͶ:
                    case BarCodeType.Ψһָ��:
                    case BarCodeType.ͨ��ַ�:
                    case BarCodeType.ͨ���շ�:
                    case BarCodeType.ͨ��ģ��:
                    case BarCodeType.ͨ���շ�ģ��:
                    case BarCodeType.�����Ѿ�Ͷ��:
                        {
                            SaveErataInfo(barcode);
                            _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                        }
                        break;
                }
                #endregion
            }
            else if (CurrentState == EnumGroupStatus.�û�ȡ������ || CurrentState == EnumGroupStatus.�û�ȡ�����Ų���ȡ��)
            {
                #region
                switch (barCodeType)
                {
                    case BarCodeType.��Ч:
                        {
                            ShowMsg(41, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(41);
                        }
                        break;

                    case BarCodeType.ΨһֱͶ:
                    case BarCodeType.Ψһָ��:
                    case BarCodeType.ͨ��ַ�:
                    case BarCodeType.ͨ���շ�:
                    case BarCodeType.ͨ��ģ��:
                    case BarCodeType.ͨ���շ�ģ��:
                    case BarCodeType.�����Ѿ�Ͷ��:
                        {
                            SaveUserGetOneLetter(barcode);
                            if (CurrentState == EnumGroupStatus.�û�ȡ������)
                                CurrentState = EnumGroupStatus.�û�ȡ�����Ų���ȡ��;
                            _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                        }
                        break;
                }
                #endregion
            }
        }
        private List<GroupStatus> _mAllgroup = null;
        void g_OnLetterIn(GroupStatus g, int boxNo)
        {
            //������, �Ե�ǰʱ����д���
            foreach (GroupStatus g1 in _mAllgroup)
            {
                if (g != g1)
                {
                    g1._mDtLastOperation = LogInfo.Win32API.GetTickCount();
                }
            }
        }
        #endregion

        #region �õ�ָ������Ϣ��Ĵ�������Ա������Ա����
        /// <summary>
        /// �õ�ָ������Ϣ��Ĵ���
        /// </summary>
        /// <param name="info">ָ������Base64�ַ���</param>
        /// <param name="iType">ָ���������ͣ�0���ϵ�ָ������1������ָ����</param>
        public void GetZhiJingMaiInfo(string info, int iType)
        {
            BarCodeType barCodeType = BarCodeType.��Ч;
            List<UserGetBoxInfo> t;
            string userName;
            barCodeType = DataSave.CheckZhiJingMaiType(info, iType, out t, out userName);

            ProcessCardInfo(info, barCodeType, t, userName);
        }
        /// <summary>
        /// �õ�ָ������Ϣ��Ĵ���
        /// </summary>
        /// <param name="locationId">ָ����λ����Ϣ</param>
        public void GetZhiJingMaiId(string locationId)
        {
            BarCodeType barCodeType = BarCodeType.��Ч;
            List<UserGetBoxInfo> t;
            string userName;
            barCodeType = DataSave.CheckZhiJingMaiLocationType(locationId, out t, out userName);

            ProcessCardInfo(locationId, barCodeType, t, userName);
        }
        #endregion

        #region �õ�֤���ź�Ĵ�������Ա������Ա����

        /// <summary>
        /// �õ�֤���ź�Ĵ�������Ա������Ա����
        /// </summary>
        /// <param name="cardcode">����ֵ</param>
        /// <param name="bn">����BN��</param>
        public void GetCardCode(string cardcode, string bn)
        {
            var barCodeType = DataSave.CheckCardType(bn, cardcode, out var t, out var userName);
            _mUserBarCode = cardcode;
            ProcessCardInfo(cardcode, barCodeType, t, userName);
        }

        private void ProcessCardInfo(string cardcode, BarCodeType barCodeType, List<UserGetBoxInfo> boxs, string userName)
        {
            //ȡ��ƽ̨
            if (MIsQuJianPingTai)
            {
                if (barCodeType == BarCodeType.����Ա || barCodeType == BarCodeType.����Ա)
                {
                    if (CurrentState == EnumGroupStatus.�û�ȡ��ȡ��ƽ̨��λѡ��)
                    {
                        _mUserBarCode = boxs[0].֤�����.ToString();
                    }
                    if (CurrentState == EnumGroupStatus.����)
                    {
                        _mUserBoxsIndex = 0;
                        #region
                        #region ���Ҫ���������ڲ��ڴ��ܵ�����
                        if (MDaiGuanGroup.Length > 0)
                        {
                            string otherUnitName = "";
                            List<UserGetBoxInfo> hasBox = new List<UserGetBoxInfo>();
                            foreach (UserGetBoxInfo box in boxs)
                            {
                                bool bHas = false;
                                foreach (string gName in MDaiGuanGroup)
                                {
                                    foreach (GroupStatus g in _mGroupstatus.Values)
                                    {
                                        if (g.GroupName == gName && g.CheckBoxInGroup(box.BoxNo))
                                        {
                                            bHas = true;
                                            break;
                                        }
                                    }
                                    if (bHas) break;
                                }
                                if (!bHas && otherUnitName == "")
                                {
                                    otherUnitName = box.��λ����;
                                }
                                if (bHas)
                                    hasBox.Add(box);
                            }
                            if (hasBox.Count <= 0 && otherUnitName != "")
                            {
                                //���鲻������Щ���ӣ���ʾ�û�
                                string msg = "��ȡ������ȡ��" + otherUnitName + "�����ļ����뵽����ȡ����ȡ����";
                                ShowMsgToBox(msg, GroupShowBn);
                                return;
                            }
                            boxs = hasBox;
                        }
                        #endregion

                        if (boxs.Count == 1)
                        {
                            #region ֻ�й���һ����񣬴�����ȡ�����ɡ�
                            //��Ч��
                            _mLastCheckCard = barCodeType;
                            _mUserBarCode = boxs[0].֤�����.ToString();
                            _mUserBoxs = boxs;
                            _mSnapFileName = "";
                            if (_mDvrip != "" && _mDvrPort > 0)
                            {
                                ShowMsgToBox("����ץͼ�����Ժ򡣡���", GroupShowBn);
                                bool b = CameraClass.Camera.GetPicture(_mDvrip, _mDvrPort, ref _mSnapFileName);
                                if (!b)
                                    _mSnapFileName = "";
                                ShowMsgToBox("��", GroupShowBn);
                            }
                            else if (GroupSheXiangTouBn.Length == 7)
                                BoxDataParse.DataParse.CmdTakePhoto(GroupSheXiangTouBn, 1);

                            if (barCodeType == BarCodeType.����Ա)
                            {
                                BoxDataParse.DataParse.CmdQuJian(GroupShowBn, LogInfo.Constant.QuJianType);
                            }
                            else if (barCodeType == BarCodeType.����Ա)
                            {
                                BoxDataParse.DataParse.CmdQuJian(GroupShowBn, 6);
                            }
                            CurrentState = EnumGroupStatus.�û�ȡ��ȡ��ƽ̨;
                            if (���Ź���Ա֤����� > 0)
                                ShowMsgToBox("ϵͳ������ģʽ.....", GroupShowBn);
                            else
                                ShowMsgToBox("\r\n����" + boxs[0].��λ���� + "\r\n\r\nȡ���ˣ�" + boxs[0].�û�����, GroupShowBn);
                            #endregion
                        }
                        else if (boxs.Count >= 1)
                        {
                            #region ���ܶ�������Ҫ�û�ѡ��
                            //��Ч��
                            _mLastCheckCard = barCodeType;
                            _mUserBarCode = boxs[0].֤�����.ToString();
                            _mUserBoxs = boxs;
                            _mSnapFileName = "";
                            if (_mDvrip != "" && _mDvrPort > 0)
                            {
                                ShowMsgToBox("����ץͼ�����Ժ򡣡���", GroupShowBn);
                                bool b = CameraClass.Camera.GetPicture(_mDvrip, _mDvrPort, ref _mSnapFileName);
                                if (!b)
                                    _mSnapFileName = "";
                                ShowMsgToBox("��", GroupShowBn);
                            }
                            else if (GroupSheXiangTouBn.Length == 7)
                                BoxDataParse.DataParse.CmdTakePhoto(GroupSheXiangTouBn, 1);

                            List<string> units = new List<string>();
                            foreach (UserGetBoxInfo box in boxs)
                            {
                                units.Add(box.��λ����);
                                if (units.Count == 12) break;
                            }

                            BoxDataParse.DataParse.CmdUnitList(GroupShowBn, units.ToArray());
                            CurrentState = EnumGroupStatus.�û�ȡ��ȡ��ƽ̨��λѡ��;
                            //ShowMsgToBox("\r\n����" + UnitName + "\r\n\r\nȡ���ˣ�" + UserName, GroupShowBN);
                            #endregion
                        }
                        else
                        {
                            //��Ч��
                            ShowMsg(42, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(42);
                        }
                        #endregion
                    }
                    else if (CurrentState == EnumGroupStatus.�û�ȡ��ȡ��ƽ̨��λѡ�� && boxs[0].֤�����.ToString() == _mUserBarCode)
                    {
                        if (_mUserBoxs.Count > 12)
                        {
                            #region �û�������12����Ҫ������ʾ
                            _mUserBoxsIndex++;
                            if (_mUserBoxsIndex * 12 >= _mUserBoxs.Count) _mUserBoxsIndex = 0;

                            List<string> units = new List<string>();
                            for (int i = _mUserBoxsIndex * 12; i < _mUserBoxs.Count; i++)
                            {
                                units.Add(_mUserBoxs[i].��λ����);
                                if (units.Count == 12) break;
                            }

                            BoxDataParse.DataParse.CmdUnitList(GroupShowBn, units.ToArray());
                            CurrentState = EnumGroupStatus.�û�ȡ��ȡ��ƽ̨��λѡ��;
                            #endregion
                        }
                    }
                }
                else
                {
                    //��Ч��
                    ShowMsg(42, LogInfo.Constant.TimeOut_ShowInfoMsg);
                    PlaySound(42);
                }
                return;
            }
            //��������
            switch (barCodeType)
            {
                case BarCodeType.��Ч:
                    {
                        ShowMsg(42, LogInfo.Constant.TimeOut_ShowInfoMsg);
                        PlaySound(42);
                    }
                    break;

                case BarCodeType.����Ա:
                    if ((CurrentState == EnumGroupStatus.���� || CurrentState == EnumGroupStatus.��ʾ��Ϣ��ʾ) && MBoxInfo.Count > 0)
                    {
                        BoxDataParse.DataParse.CmdOpenDoor(MBoxInfo.FirstOrDefault().Value.BoxBn, true);
                        //m_AdminBarCode = boxs[0].֤�����.ToString();
                        //m_UserBoxs = boxs;
                        //CurrentState = enumGroupStatus.����ά��;
                    }
                    else if ((CurrentState == EnumGroupStatus.���� || CurrentState == EnumGroupStatus.��ʾ��Ϣ��ʾ) || CurrentState == EnumGroupStatus.������)
                    {
                        _mAdminBarCode = "";
                        CurrentState = EnumGroupStatus.����;
                    }
                    break;
                case BarCodeType.����Ա:
                    if (CurrentState == EnumGroupStatus.���� || CurrentState == EnumGroupStatus.��ʾ��Ϣ��ʾ)
                    {
                        //�ж��Ƿ��������
                        bool bQingTui = false;
                        if (MBoxInfo.Count == 1)
                        {
                            foreach (StBoxInfo b in MBoxInfo.Values)
                            {
                                if (b.Box.IsQingTuiXiang)
                                    bQingTui = true;
                            }
                        }
                        if (bQingTui)
                        {
                            if ((CurrentState == EnumGroupStatus.���� || CurrentState == EnumGroupStatus.��ʾ��Ϣ��ʾ))
                            {
                                _mUserBarCode = boxs[0].֤�����;
                                CurrentState = EnumGroupStatus.����;
                            }
                            else if (CurrentState == EnumGroupStatus.����)
                            {
                                _mUserBarCode = "";
                                CurrentState = EnumGroupStatus.����;
                            }
                        }
                        else if ((CurrentState == EnumGroupStatus.���� || CurrentState == EnumGroupStatus.��ʾ��Ϣ��ʾ))
                        {
                            string otherGroupname = "";
                            int iCount = 0;
                            foreach (UserGetBoxInfo b in boxs)
                            {
                                if (!MBoxInfo.ContainsKey(b.BoxNo))
                                {
                                    if (LogInfo.Constant.NotifyGroupName)
                                    {
                                        otherGroupname = GetGroupFromBoxNo(b.BoxNo).GroupName;
                                    }
                                    else
                                    {
                                        if (_mBoxstatus.ContainsKey(b.BoxNo))
                                        {
                                            otherGroupname = _mBoxstatus[b.BoxNo].BoxUnitName;
                                        }
                                    }
                                    continue;
                                }
                                //����
                                iCount++;
                            }
                            if (iCount <= 0)
                            {
                                ShowMsgToBox("�뵽" + otherGroupname + "ȡ��", GroupShowBn);
                            }
                            else
                            {
                                foreach (UserGetBoxInfo b in boxs)
                                {
                                    if (!MBoxInfo.ContainsKey(b.BoxNo))
                                    {
                                        if (LogInfo.Constant.NotifyGroupName)
                                        {
                                            otherGroupname = GetGroupFromBoxNo(b.BoxNo).GroupName;
                                        }
                                        else
                                        {
                                            if (_mBoxstatus.ContainsKey(b.BoxNo))
                                            {
                                                otherGroupname = _mBoxstatus[b.BoxNo].BoxUnitName;
                                            }
                                        }
                                        continue;
                                    }
                                    _mUserBarCode = boxs[0].֤�����.ToString();
                                    _mUserBoxs = boxs;
                                    //����
                                    if (LogInfo.Constant.OpenDoorType || iCount <= 1 || MBoxInfo[b.BoxNo].Box.LetterCount > 0)
                                    {
                                        BoxDataParse.DataParse.CmdOpenDoor(MBoxInfo[b.BoxNo].BoxBn, true);
                                        MBoxInfo[b.BoxNo].BUsed = true;
                                        CurrentState = EnumGroupStatus.�û�ȡ������;
                                    }
                                }
                            }
                        }
                    }
                    else if (CurrentState == EnumGroupStatus.����ά��)
                    {
                        _mAdminBarCode = boxs[0].֤�����.ToString();
                        CurrentState = EnumGroupStatus.������;
                    }
                    else if (CurrentState == EnumGroupStatus.����)
                    {
                        CurrentState = EnumGroupStatus.����;
                    }
                    break;
            }
        }
        #endregion

        #region ȡ����λѡ��
        /// <summary>
        /// ȡ����λѡ��
        /// </summary>
        /// <param name="BoxNo"></param>
        public void ȡ����λѡ��(List<int> idArray)
        {
            if (CurrentState == EnumGroupStatus.�û�ȡ��ȡ��ƽ̨��λѡ�� && idArray.Count > 0)
            {
                List<UserGetBoxInfo> uBox = new List<UserGetBoxInfo>();
                string unitName = "";
                foreach (int id in idArray)
                {
                    uBox.Add(_mUserBoxs[id + _mUserBoxsIndex * 12]);
                    if (unitName == "")
                        unitName = _mUserBoxs[id + _mUserBoxsIndex * 12].��λ����;
                    else
                        unitName += "��" + _mUserBoxs[id + _mUserBoxsIndex * 12].��λ����;
                }
                _mUserBoxs = uBox;
                if (_mLastCheckCard == BarCodeType.����Ա)
                {
                    BoxDataParse.DataParse.CmdQuJian(GroupShowBn, LogInfo.Constant.QuJianType);
                }
                else if (_mLastCheckCard == BarCodeType.����Ա)
                {
                    BoxDataParse.DataParse.CmdQuJian(GroupShowBn, 6);
                }
                CurrentState = EnumGroupStatus.�û�ȡ��ȡ��ƽ̨;
                if (���Ź���Ա֤����� > 0)
                    ShowMsgToBox("ϵͳ������ģʽ.....", GroupShowBn);
                else
                    ShowMsgToBox("\r\n����" + unitName + "\r\n\r\nȡ���ˣ�" + uBox[0].�û�����, GroupShowBn);
            }
        }
        #endregion

        #region ����ɨ��֤��������ʹ��
        /// <summary>
        /// ����ɨ��֤��������ʹ��
        /// </summary>
        /// <param name="cardcode">����ֵ</param>
        public void BoxGetCardCode(int boxNo, string cardcode)
        {
            BarCodeType barCodeType;
            List<UserGetBoxInfo> boxs;
            string userName;
            barCodeType = DataSave.CheckCardType("", cardcode, out boxs, out userName);
            switch (barCodeType)
            {
                case BarCodeType.��Ч:
                    {
                        ShowMsg(42, LogInfo.Constant.TimeOut_ShowInfoMsg);
                        PlaySound(42);
                    }
                    break;

                case BarCodeType.����Ա:
                    if ((CurrentState == EnumGroupStatus.���� || CurrentState == EnumGroupStatus.��ʾ��Ϣ��ʾ))
                    {
                        _mUserBarCode = boxs[0].֤�����.ToString();
                        CurrentState = EnumGroupStatus.����;
                    }
                    else if (CurrentState == EnumGroupStatus.����)
                    {
                        _mUserBarCode = "";
                        CurrentState = EnumGroupStatus.����;
                    }
                    break;
            }

        }
        #endregion

        #region Ͷ���ż��Ĵ�����������ݿ⣬��Ͷ��
        /// <summary>
        /// Ͷ���ż��Ĵ�����������ݿ⣬��Ͷ��
        /// </summary>
        /// <param name="boxNo"></param>
        public void LetterIn(int boxNo, int showCount)
        {
            if (!MBoxInfo.ContainsKey(boxNo)) return;
            if (!MBoxInfo[boxNo].BUsed)
            {
                //������Ͷ��
                if (_mJingGaoBoxNo.Contains(boxNo) && CurrentState == EnumGroupStatus.Ͷ�䱨��)
                {
                    _mJingGaoString = LogInfo.NotifyMsg.GetText(47);
                    ShowMsgToBox(_mJingGaoString, MBoxInfo[boxNo].BoxBn, 255);
                    BoxDataParse.DataParse.CmdBuzzer(MBoxInfo[boxNo].BoxBn, 3);
                }
                else if (CurrentState != EnumGroupStatus.Ͷ�䱨��)
                {
                    if (!_mJingGaoBoxNo.Contains(boxNo))
                        _mJingGaoBoxNo.Add(boxNo);
                    _mJingGaoString = LogInfo.NotifyMsg.GetText(401);
                    CurrentState = EnumGroupStatus.Ͷ�䱨��;
                }
                return;
            }
            int sendCount = 1;
            if (CurrentState == EnumGroupStatus.ͨ��ַ� && LogInfo.Constant.DistributeType == 0)
                sendCount = MBoxInfo[boxNo].Count;

            //ת�����Ž��򿪵�״̬
            if (CurrentState == EnumGroupStatus.����ͨͶ)
            {
                foreach (StBoxInfo b in MBoxInfo.Values)
                {
                    if (b.BoxNo != boxNo && b.BUsed)
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                        //����
                        BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.��ɫ);
                        b.BUsed = false;
                    }
                }
                MCurrentState = EnumGroupStatus.Ͷ�俪�Ž�;
            }
            else if (CurrentState == EnumGroupStatus.����ͨͶ)
            {
                foreach (StBoxInfo b in MBoxInfo.Values)
                {
                    if (b.BoxNo != boxNo && b.BUsed)
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                        //����
                        BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.��ɫ);
                        b.BUsed = false;
                    }
                }
                MCurrentState = EnumGroupStatus.���˿��Ž�;
            }

            if (OnLetterIn != null)
            {
                OnLetterIn(this, boxNo);
            }
            SaveLetter(boxNo, sendCount);
            if (OnLetterIn != null)
            {
                OnLetterIn(this, boxNo);
            }
            _mDtLastOperation = LogInfo.Win32API.GetTickCount();
        }

        private void SaveLetter(int boxNo, int sendCount)
        {
            //���ţ���¼�����ݿ�
            bool �Ƿ����� = false;
            if (���Ź���Ա֤����� > 0)
                �Ƿ����� = true;
            int iRet = 0;
            if (CurrentState == EnumGroupStatus.���˿��Ž� || CurrentState == EnumGroupStatus.����ͨͶ)
                iRet = DataSave.SaveLetter(_mLetterBarCode, boxNo, _mBJiaJi, sendCount, _mUserBarCode, _mRecFileName, �Ƿ�����, ���Ź���Ա֤�����.ToString());
            else
                iRet = DataSave.SaveLetter(_mLetterBarCode, boxNo, _mBJiaJi, sendCount, "", _mRecFileName, �Ƿ�����, ���Ź���Ա֤�����.ToString());
            if (iRet > 0)
                MBoxInfo[boxNo].Box.LampJiJian = true;

            if (iRet >= 0)
            {
                //��ȡ���ݿ⣬�õ��ż���Ŀ�ͼ�����״̬
                DataBase.MonitorService.BoxInfo t = DataSave.GetBoxLetterCount(boxNo);
                if (t != null)
                {
                    MBoxInfo[boxNo].Box.LampJiJian = t.HasJiJian;
                    MBoxInfo[boxNo].Box.LetterCount = t.SendCount;
                    //���ؽṹ��֪ͨ��ͷ
                    MBoxInfo[boxNo].Box.CheckBoxLetter();

                    //֪ͨ�ͻ��ˣ��ı��ż�
                    SendClientNotify.NotifyLetter(boxNo.ToString("000"), (t.HasJiJian ? "true" : "false"), t.SendCount);
                }

                if (CurrentState == EnumGroupStatus.Ͷ�俪�Ž�)
                {
                    MBoxInfo[boxNo].BUsed = false;
                    BoxDataParse.DataParse.CmdBoxGating(MBoxInfo[boxNo].BoxBn, false);
                    //��ʾ��Ϣ
                    ShowMsgToBox(LogInfo.NotifyMsg.GetText(44), MBoxInfo[boxNo].BoxBn);
                    PlaySound(44);
                    CurrentState = EnumGroupStatus.��ʾ��Ϣ��ʾ;
                }
                else if (CurrentState == EnumGroupStatus.���˿��Ž�)
                {
                    MBoxInfo[boxNo].BUsed = false;
                    BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[boxNo].BoxBn);
                    //��ʾ��Ϣ
                    ShowMsgToBox(LogInfo.NotifyMsg.GetText(44), MBoxInfo[boxNo].BoxBn);
                    PlaySound(44);
                    //�ȴ�1��󣬻ظ���ʼ״̬
                    if (MBoxInfo.Count > 1)
                        System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000));
                    CurrentState = EnumGroupStatus.����;
                }
                else if (CurrentState == EnumGroupStatus.ͨ���շ�)
                {
                    MBoxInfo[boxNo].SendCount += sendCount;
                    BoxDataParse.DataParse.CmdBoxLED(MBoxInfo[boxNo].BoxBn, 1, MBoxInfo[boxNo].SendCount, LogInfo.enum_LedColor.��ɫ);
                }
                else if (CurrentState == EnumGroupStatus.ͨ��ַ�)
                {
                    MBoxInfo[boxNo].SendCount += sendCount;
                    if (MBoxInfo[boxNo].SendCount >= MBoxInfo[boxNo].Count)
                    {
                        MBoxInfo[boxNo].BUsed = false;
                        BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[boxNo].BoxBn);
                        bool bold = true;
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            if (b.BUsed)
                            {
                                bold = false;
                                break;
                            }
                        }
                        //��ʾ��Ϣ
                        ShowMsgToBox(LogInfo.NotifyMsg.GetText(44), MBoxInfo[boxNo].BoxBn);
                        PlaySound(44);
                        if (bold)
                        {
                            //�ȴ�1��󣬻ظ���ʼ״̬
                            System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000));
                            CurrentState = EnumGroupStatus.����;
                        }
                    }
                    else
                    {
                        BoxDataParse.DataParse.CmdBoxLED(MBoxInfo[boxNo].BoxBn, 1, MBoxInfo[boxNo].Count - MBoxInfo[boxNo].SendCount, LogInfo.enum_LedColor.��ɫ);
                    }
                }
            }
            else
            {
                MBoxInfo[boxNo].BUsed = false;
                BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[boxNo].BoxBn);
                MBoxInfo[boxNo].Box.CheckBoxLetter();
                bool bold = true;
                foreach (StBoxInfo b in MBoxInfo.Values)
                {
                    if (b.BUsed)
                    {
                        bold = false;
                        break;
                    }
                }
                //��������ʾ
                ShowMsgToBox("���ݱ������,������Ͷ�䡣", MBoxInfo[boxNo].BoxBn);

                //�ȴ�3��󣬻ظ���ʼ״̬
                System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000) - 100);
                if (bold)
                {
                    CurrentState = EnumGroupStatus.����;
                }
            }
        }
        #endregion

        #region Ͷ������Ĵ�����Ͷ��
        /// <summary>
        /// Ͷ������Ĵ�����Ͷ��
        /// </summary>
        /// <param name="boxNo"></param>
        public void LetterOut(int boxNo, int showCount)
        {
            if (!MBoxInfo.ContainsKey(boxNo)) return;
            if (!MBoxInfo[boxNo].BUsed)
            {
                if (_mJingGaoBoxNo.Contains(boxNo) && CurrentState == EnumGroupStatus.Ͷ�䱨��)
                {
                    _mJingGaoString = LogInfo.NotifyMsg.GetText(48);
                    ShowMsgToBox(_mJingGaoString, MBoxInfo[boxNo].BoxBn, 255);
                }
                return;
            }
            if (CurrentState == EnumGroupStatus.Ͷ�俪�Ž� || CurrentState == EnumGroupStatus.���˿��Ž�
                || CurrentState == EnumGroupStatus.����ͨͶ || CurrentState == EnumGroupStatus.����ͨͶ)
            {
                _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                foreach (StBoxInfo bi in MBoxInfo.Values)
                {
                    if (!bi.BUsed) continue;
                    bi.BUsed = false;
                    BoxDataParse.DataParse.CmdBoxGating(bi.BoxBn, false);
                    bi.Box.CheckBoxLetter();
                }

                //��¼��־
                string logMessage = "Ͷ���������ǰ״̬��" + CurrentState.ToString() + "�����룺" + _mLetterBarCode + "����ţ�" + boxNo.ToString() + "\r\n";
                LogInfo.Log.WriteFileLog(logMessage);

                PlaySound(45);
                _mJingGaoString = LogInfo.NotifyMsg.GetText(45);
                _mJingGaoBoxNo.Clear();
                _mJingGaoBoxNo.Add(boxNo);
                CurrentState = EnumGroupStatus.Ͷ�䱨��;
            }
            else if (CurrentState == EnumGroupStatus.ͨ��ַ�)
            {
                //��¼��־
                string logMessage = "Ͷ���������ǰ״̬��" + CurrentState.ToString() + "�����룺" + _mLetterBarCode + "����ţ�" + boxNo.ToString() + "\r\n";
                LogInfo.Log.WriteFileLog(logMessage);
            }
            else if (CurrentState == EnumGroupStatus.ͨ���շ�)
            {
                //��¼��־
                string logMessage = "Ͷ���������ǰ״̬��" + CurrentState.ToString() + "�����룺" + _mLetterBarCode + "����ţ�" + boxNo.ToString() + "\r\n";
                LogInfo.Log.WriteFileLog(logMessage);
            }
        }
        #endregion

        #region ���Ĵ���
        /// <summary>
        /// ���Ĵ���
        /// </summary>
        /// <param name="boxNo">��ͷ��BN��</param>
        public void GdStateChange(int boxNo, string type)
        {
            if (!MBoxInfo.ContainsKey(boxNo)) return;
            if (!MBoxInfo[boxNo].BUsed)
            {
                if (_mJingGaoBoxNo.Contains(boxNo) && CurrentState == EnumGroupStatus.Ͷ�䱨��)
                {
                    BoxDataParse.DataParse.CmdBoxLampCuoWu(MBoxInfo[boxNo].BoxBn, LogInfo.enum_LampStatus.��˸);
                }
                return;
            }
            switch (type)
            {
                case "ǰ������":
                case "�������":
                    MBoxInfo[boxNo].BUsed = false;
                    BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[boxNo].BoxBn);
                    ShowMsgToBox("��糤�ڵ������", MBoxInfo[boxNo].BoxBn);
                    MBoxInfo[boxNo].Box.CheckBoxLetter();
                    if (CurrentState == EnumGroupStatus.ͨ��ַ�)
                    {
                        bool bold = true;
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            if (b.BUsed)
                            {
                                bold = false;
                                break;
                            }
                        }
                        //�ȴ�1��󣬻ظ���ʼ״̬
                        if (bold)
                        {
                            System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000));
                            CurrentState = EnumGroupStatus.����;
                        }
                    }
                    else if (CurrentState == EnumGroupStatus.Ͷ�俪�Ž�)
                    {
                        System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000));
                        CurrentState = EnumGroupStatus.����;
                    }
                    else if (CurrentState == EnumGroupStatus.���˿��Ž�)
                    {
                        System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000));
                        CurrentState = EnumGroupStatus.����;
                    }
                    break;

                case "����������":
                    break;

                case "��չ�����":
                    MBoxInfo[boxNo].BUsed = false;
                    if (CurrentState == EnumGroupStatus.�û�ȡ������)
                    {
                        CurrentState = EnumGroupStatus.����;
                        ShowMsgToBox("��չ����ϣ�����ϵ����Ա", MBoxInfo[boxNo].BoxBn);
                    }
                    else if (CurrentState == EnumGroupStatus.�û�ȡ�����Ų���ȡ��)
                    {
                    }
                    break;
            }
        }
        /// <summary>
        /// �������״̬
        /// </summary>
        /// <param name="bZheDang"></param>
        public void ChufaGdChange(bool bZheDang)
        {
            _mBChufa = bZheDang;
            if (bZheDang && (CurrentState == EnumGroupStatus.���� || CurrentState == EnumGroupStatus.��ʾ��Ϣ��ʾ
                || CurrentState == EnumGroupStatus.���� || CurrentState == EnumGroupStatus.������
                || CurrentState == EnumGroupStatus.�û�ȡ������ || CurrentState == EnumGroupStatus.�û�ȡ�����Ų���ȡ��))
            {
                BoxDataParse.DataParse.CmdBoxScan(GroupScanBn, true);
            }
            else if (!bZheDang)
            {
                //BoxDataParse.DataParse.CmdBoxScan(GroupScanBN, false);
            }
        }
        #endregion

        #region �Ž��Ĵ���
        /// <summary>
        /// �Ž��Ĵ���
        /// </summary>
        /// <param name="boxNo">��ͷ��BN��</param>
        public void GateStateChange(int boxNo, string type)
        {
            if (!MBoxInfo.ContainsKey(boxNo)) return;
            if (!MBoxInfo[boxNo].BUsed)
            {
                return;
            }
            switch (type)
            {
                case "Opened":
                    if (CurrentState == EnumGroupStatus.����ͨͶ)
                    {
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            if (b.BoxNo != boxNo && b.BUsed)
                            {
                                BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                                //����
                                BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.��ɫ);
                                b.BUsed = false;
                            }
                        }
                        if (MBoxInfo.Count > 1)
                            BoxDataParse.DataParse.CmdLCDText(MBoxInfo[boxNo].BoxBn, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, LogInfo.NotifyMsg.GetText((int)EnumGroupStatus.Ͷ�俪�Ž�));
                        CurrentState = EnumGroupStatus.Ͷ�俪�Ž�;
                    }
                    else if (CurrentState == EnumGroupStatus.����ͨͶ)
                    {
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            if (b.BoxNo != boxNo && b.BUsed)
                            {
                                BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                                //����
                                BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.��ɫ);
                                b.BUsed = false;
                            }
                        }
                        if (MBoxInfo.Count > 1)
                            BoxDataParse.DataParse.CmdLCDText(MBoxInfo[boxNo].BoxBn, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, LogInfo.NotifyMsg.GetText((int)EnumGroupStatus.���˿��Ž�));
                        CurrentState = EnumGroupStatus.���˿��Ž�;
                    }
                    break;

                case "Closed":
                    break;

                case "OpenTimeOut":
                    if (CurrentState == EnumGroupStatus.����ͨͶ || CurrentState == EnumGroupStatus.����ͨͶ)
                    {
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                            //����
                            BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.��ɫ);
                            b.BUsed = false;
                        }
                        CurrentState = EnumGroupStatus.����;
                        ShowMsgToBox("�Ž���������ϵ����Ա", MBoxInfo[boxNo].BoxBn);
                    }
                    break;

                case "CloseTimeOut":
                    break;
            }
        }
        #endregion

        #region �����Ĵ���
        /// <summary>
        /// �����Ĵ���
        /// </summary>
        /// <param name="boxNo">��ͷ��BN��</param>
        public void DoorStateChange(int boxNo, string type)
        {
            if (!MBoxInfo.ContainsKey(boxNo)) return;
            if (_mJingGaoBoxNo.Contains(boxNo) && CurrentState == EnumGroupStatus.Ͷ�䱨��)
            {
                BoxDataParse.DataParse.CmdBoxLampCuoWu(MBoxInfo[boxNo].BoxBn, LogInfo.enum_LampStatus.��˸);
            }
            if (!MBoxInfo[boxNo].BUsed)
            {
                //�ж��ǲ��ǹ��ţ�����ʱ�����״̬����ͷ����
                if (type == "Closed")
                {
                    if (MBoxInfo[boxNo].Box.LetterCount == 0 && MBoxInfo[boxNo].Box.LampYiQu)
                    {
                        if (MBoxInfo[boxNo].BoxStatus.ClearGDState)
                        {
                            //û��ȡ��
                            MBoxInfo[boxNo].BoxStatus.ClearGDWarnning = true;
                            PlaySound(49);
                            string msg = LogInfo.NotifyMsg.GetText(49);
                            //��ʾ����ͷ
                            ShowMsgToBox(msg, MBoxInfo[boxNo].BoxBn, 255);
                        }
                    }
                }
                return;
            }
            switch (type)
            {
                case "Opened":
                    if (CurrentState == EnumGroupStatus.�û�����Ͷ��Pre)
                    {
                        //������Ϣ
                        SaveLetter(boxNo, 1);
                        CurrentState = EnumGroupStatus.�û�����Ͷ��;
                    }
                    else if (CurrentState == EnumGroupStatus.�û�ȡ������)
                    {
                    }
                    else if (CurrentState == EnumGroupStatus.ά������)
                    {
                    }
                    else if (CurrentState == EnumGroupStatus.������)
                    {
                    }
                    break;

                case "Closed":
                    MBoxInfo[boxNo].BUsed = false;
                    if (CurrentState == EnumGroupStatus.�û�����Ͷ��)
                    {
                        //��ԭ״̬
                        CurrentState = EnumGroupStatus.����;
                    }
                    else if (CurrentState == EnumGroupStatus.�û�ȡ������)
                    {
                        bool bHas = false;
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            if (b.BUsed)
                            {
                                bHas = true;
                                break;
                            }
                        }
                        if (MBoxInfo[boxNo].BoxStatus.ClearGDState)
                        {
                            PlaySound(49);
                            _mJingGaoString = LogInfo.NotifyMsg.GetText(49);
                            _mJingGaoBoxNo.Clear();
                            _mJingGaoBoxNo.Add(boxNo);
                            CurrentState = EnumGroupStatus.ȡ��δ��ձ���;
                        }
                        else
                        {
                            //����ȡ����Ϣ
                            SaveUserGetLetter(boxNo, false, false, false);
                            if (!bHas)
                                CurrentState = EnumGroupStatus.����;
                        }
                    }
                    else if (CurrentState == EnumGroupStatus.�û�ȡ�����Ų���ȡ��)
                    {
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            if (b.BUsed)
                            {
                                break;
                            }
                        }
                        CurrentState = EnumGroupStatus.����;
                    }
                    else if (CurrentState == EnumGroupStatus.ά������)
                    {
                        CurrentState = EnumGroupStatus.����ά��;
                    }
                    else if (CurrentState == EnumGroupStatus.������)
                    {
                        CurrentState = EnumGroupStatus.������;
                    }
                    break;

                case "OpenTimeOut":
                    MBoxInfo[boxNo].BUsed = false;
                    if (CurrentState == EnumGroupStatus.�û�����Ͷ��
                        || CurrentState == EnumGroupStatus.�û�����Ͷ��Pre)
                    {
                        CurrentState = EnumGroupStatus.����;
                        ShowMsgToBox("������������ϵ����Ա", MBoxInfo[boxNo].BoxBn);
                    }
                    else if (CurrentState == EnumGroupStatus.�û�ȡ������)
                    {
                        CurrentState = EnumGroupStatus.����;
                        ShowMsgToBox("������������ϵ����Ա", MBoxInfo[boxNo].BoxBn);
                    }
                    else if (CurrentState == EnumGroupStatus.ά������)
                    {
                        CurrentState = EnumGroupStatus.����;
                        ShowMsgToBox("������������ϵ����Ա", MBoxInfo[boxNo].BoxBn);
                    }
                    else if (CurrentState == EnumGroupStatus.������)
                    {
                        CurrentState = EnumGroupStatus.����;
                        ShowMsgToBox("������������ϵ����Ա", MBoxInfo[boxNo].BoxBn);
                    }
                    break;

                case "CloseTimeOut":
                    break;
            }
        }
        #endregion

        #region �����Ĵ���
        /// <summary>
        /// �����Ĵ���
        /// </summary>
        /// <param name="BN_NO">��ͷ��BN��</param>
        public void KeyPress(int boxNo, int keyIndex)
        {
            if (keyIndex == 0 || keyIndex == 1 || keyIndex == 60)
            {
                if (!MBoxInfo.ContainsKey(boxNo))
                {
                    if (MBoxInfo[boxNo].BoxStatus.GetErrorString() != "")
                    {
                        BoxDataParse.DataParse.CmdLCDText(MBoxInfo[boxNo].BoxBn, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, (int)LogInfo.Constant.TimeOut_ShowInfoMsg, MBoxInfo[boxNo].BoxStatus.GetErrorString());
                    }
                    return;
                }
                if (MBoxInfo[boxNo].BoxStatus.ClearGDWarnning)
                {
                    MBoxInfo[boxNo].BoxStatus.ClearGDWarnning = false;
                    BoxDataParse.DataParse.CmdBuzzer(MBoxInfo[boxNo].BoxBn, 0);
                    ShowMsgToBox(" ", MBoxInfo[boxNo].BoxBn, 0);
                    BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[boxNo].BoxBn);
                }
                //����д�����Ϣ����ʾ������Ϣ
                #region ��ͷ����
                if (CurrentState == EnumGroupStatus.Ͷ�俪�Ž�)
                {
                    if (!MBoxInfo[boxNo].BUsed) return;
                    _mBJiaJi = !_mBJiaJi;
                    if (_mBJiaJi)
                        BoxDataParse.DataParse.CmdBoxLampJiJian(MBoxInfo[boxNo].BoxBn, LogInfo.enum_LampStatus.��˸);
                    else
                        BoxDataParse.DataParse.CmdBoxLampJiJian(MBoxInfo[boxNo].BoxBn, LogInfo.enum_LampStatus.��);
                    _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                }
                else if (CurrentState == EnumGroupStatus.ͨ��ַ�)
                {
                    if (LogInfo.Constant.DistributeType == 1)
                    {
                        int sendCount = 1;
                        sendCount = MBoxInfo[boxNo].Count - MBoxInfo[boxNo].SendCount;
                        MBoxInfo[boxNo].SendCount += sendCount;
                        SaveLetter(boxNo, sendCount);
                        _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                    }
                }
                else if (CurrentState == EnumGroupStatus.����ά��)
                {
                    MBoxInfo[boxNo].BUsed = true;
                    BoxDataParse.DataParse.CmdOpenDoor(MBoxInfo[boxNo].BoxBn, true);
                    CurrentState = EnumGroupStatus.ά������;
                }
                else if (CurrentState == EnumGroupStatus.������)
                {
                    MBoxInfo[boxNo].BUsed = true;
                    BoxDataParse.DataParse.CmdOpenDoor(MBoxInfo[boxNo].BoxBn, true);
                    CurrentState = EnumGroupStatus.������;
                }
                else if (CurrentState == EnumGroupStatus.Ͷ�䱨��)
                {
                    if (_mJingGaoBoxNo.Contains(boxNo))
                    {
                        //�ظ�ԭʼ״̬
                        foreach (int bno in _mJingGaoBoxNo)
                        {
                            BoxDataParse.DataParse.CmdBuzzer(MBoxInfo[bno].BoxBn, 0);
                            MBoxInfo[bno].Box.CheckBoxLetter();
                            BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[bno].BoxBn);
                        }
                        _mJingGaoString = "";
                        _mJingGaoBoxNo.Clear();
                        CurrentState = EnumGroupStatus.����;
                    }
                }
                else if (CurrentState == EnumGroupStatus.ȡ��δ��ձ���)
                {
                    if (_mJingGaoBoxNo.Contains(boxNo))
                    {
                        //�ظ�ԭʼ״̬
                        foreach (int bno in _mJingGaoBoxNo)
                        {
                            BoxDataParse.DataParse.CmdBuzzer(MBoxInfo[bno].BoxBn, 0);
                            BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[bno].BoxBn);
                        }
                        _mJingGaoString = "";
                        _mJingGaoBoxNo.Clear();

                        //����ȡ����Ϣ
                        SaveUserGetLetter(boxNo, false, false, false);
                        CurrentState = EnumGroupStatus.����;
                    }
                }
                #endregion
            }
            else if (keyIndex == 10)//10���շ�����
            {
                #region
                if (CurrentState == EnumGroupStatus.ͨ�뷽ʽѡ��)
                {
                    CurrentState = EnumGroupStatus.ͨ���շ�;
                    foreach (StBoxInfo st in MBoxInfo.Values)
                    {
                        if (!string.IsNullOrEmpty(st.Box.BoxUnitName) && st.Box.BoxUnitName != LogInfo.Constant.EmptyBoxName)
                        {
                            st.BUsed = true;
                            st.SendCount = 0;
                            BoxDataParse.DataParse.CmdPreGetLetter(st.BoxBn, 1, 2, 0);
                        }
                    }
                    BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, false, false, true, false);
                }
                #endregion
            }
            else if (keyIndex == 11)//11��ģ��ַ�����
            {
                #region
                if (CurrentState == EnumGroupStatus.ͨ�뷽ʽѡ��)
                {
                    _mCurrentMoBan = DataSave.GetBarcodeMuBanList(_mLetterBarCode);
                    if (_mCurrentMoBan.Length > 0)
                    {
                        string[] mb = new string[_mCurrentMoBan.Length];
                        for (int i = 0; i < _mCurrentMoBan.Length; i++)
                            mb[i] = _mCurrentMoBan[i].ģ������;
                        BoxDataParse.DataParse.CmdTempletList(GroupShowBn, mb);
                        CurrentState = EnumGroupStatus.ͨ��ģ���б�;
                    }
                    else
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                        CurrentState = EnumGroupStatus.����;
                        ShowMsgToBox("û���ҵ���Ӧ��ģ�壬��ǼǺ�Ͷ��", GroupShowBn);
                    }
                }
                #endregion
            }
            else if (keyIndex == 12)//12��ȡ���˳�����
            {
                #region
                if (CurrentState == EnumGroupStatus.ͨ�뷽ʽѡ��
                    || CurrentState == EnumGroupStatus.ͨ��ģ���б�
                    || CurrentState == EnumGroupStatus.�û�ȡ��ȡ��ƽ̨��λѡ��)
                {
                    CurrentState = EnumGroupStatus.����;
                }
                else if (CurrentState == EnumGroupStatus.ͨ��ģ���б�ȷ��)
                {
                    if (_mCurrentMoBan.Length > 0)
                    {
                        string[] mb = new string[_mCurrentMoBan.Length];
                        for (int i = 0; i < _mCurrentMoBan.Length; i++)
                            mb[i] = _mCurrentMoBan[i].ģ������;
                        BoxDataParse.DataParse.CmdTempletList(GroupShowBn, mb);
                        CurrentState = EnumGroupStatus.ͨ��ģ���б�;
                    }
                    else
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                        CurrentState = EnumGroupStatus.����;
                        ShowMsgToBox("û���ҵ���Ӧ��ģ�壬��ǼǺ�Ͷ��", GroupShowBn);
                    }
                }
                else if (CurrentState == EnumGroupStatus.ͨ���շ�
                    || CurrentState == EnumGroupStatus.ͨ��ַ�)
                {
                    foreach (StBoxInfo b in MBoxInfo.Values)
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                        b.Box.CheckBoxLetter();
                        b.BUsed = false;
                    }
                    BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                    CurrentState = EnumGroupStatus.����;
                }
                #endregion
            }
            else if (keyIndex == 13)//13��ȷ�ϰ���
            {
                #region
                if (CurrentState == EnumGroupStatus.ͨ��ģ���б�ȷ��)
                {
                    bool b = DataSave.GetMuBanInfo(_mLetterBarCode, _mCurrentMoBan[_mCurIndex].ģ���ʶ);
                    if (b)
                    {
                        MCurrentState = EnumGroupStatus.����;
                        GetBarCode(_mLetterBarCode, true);
                    }
                    else
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                        CurrentState = EnumGroupStatus.����;
                        ShowMsgToBox("û���ҵ���Ӧ��ģ�壬��ǼǺ�Ͷ��", GroupShowBn);
                    }
                }
                #endregion
            }
            else if (keyIndex > 20 && keyIndex <= 36)
            {
                //ģ��ѡ�������+20
                if (CurrentState == EnumGroupStatus.ͨ��ģ���б� && (keyIndex - 20) <= _mCurrentMoBan.Length)
                {
                    _mCurIndex = keyIndex - 21;
                    BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, false, false, true, true);
                    ShowMsgToBox("��ȷ��Ҫʹ�ñ�ģ����\r\nģ����Ϣ��" + _mCurrentMoBan[_mCurIndex].ģ������, GroupShowBn);
                    CurrentState = EnumGroupStatus.ͨ��ģ���б�ȷ��;
                }
            }
            else if (keyIndex >= 41 && keyIndex <= 60)//ȡ�����ƹ���水ťѡ��
            {
                #region
                if (CurrentState == EnumGroupStatus.�û�ȡ��ȡ��ƽ̨)
                {
                    ShowMsgToBox("��", GroupShowBn);
                    if (keyIndex == 41)//ֻ������
                    {
                        SaveUserGetLetter_QuJianPintTai(true, false, false);
                    }
                    else if (keyIndex == 42)//ȡ������
                    {
                        CurrentState = EnumGroupStatus.����;
                    }
                    else if (keyIndex == 43)//��ӡ�嵥��3��ť���棩
                    {
                        SaveUserGetLetter_QuJianPintTai(true, true, true);
                    }
                    else if (keyIndex == 44)//��ӡ�����嵥��4��ť���棩
                    {
                        SaveUserGetLetter_QuJianPintTai(false, true, false);
                    }
                    else if (keyIndex == 45)//��ӡ�ռ��嵥��4��ť���棩
                    {
                        SaveUserGetLetter_QuJianPintTai(true, false, true);
                    }
                    else if (keyIndex == 46)//ֻ��ӡ�嵥��4��ť���棩
                    {
                        SaveUserGetLetter_QuJianPintTai(false, true, true);
                    }
                    else if (keyIndex == 51)//����Ա����������ģʽ
                    {
                        //���Ҫ���������ڲ��ڴ��ܵ�����
                        if (MDaiGuanGroup.Length > 0)
                        {
                            foreach (string gName in MDaiGuanGroup)
                            {
                                foreach (GroupStatus g in _mGroupstatus.Values)
                                {
                                    if (g.GroupName == gName)
                                    {
                                        g.���Ź���Ա֤����� = Convert.ToInt32(_mUserBarCode);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (GroupStatus g in _mGroupstatus.Values)
                            {
                                g.���Ź���Ա֤����� = Convert.ToInt32(_mUserBarCode);
                            }
                        }
                        //��ȡ����
                        ���Ź���Ա֤����� = Convert.ToInt32(_mUserBarCode);
                        ShowMsgToBox("��������ģʽ��.", GroupShowBn);
                        return;
                    }
                    else if (keyIndex == 52)//����Ա���˳�����ģʽ
                    {
                        //���Ҫ���������ڲ��ڴ��ܵ�����
                        if (MDaiGuanGroup.Length > 0)
                        {
                            foreach (string gName in MDaiGuanGroup)
                            {
                                foreach (GroupStatus g in _mGroupstatus.Values)
                                {
                                    if (g.GroupName == gName)
                                    {
                                        g.���Ź���Ա֤����� = 0;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (GroupStatus g in _mGroupstatus.Values)
                            {
                                g.���Ź���Ա֤����� = 0;
                            }
                        }
                        //��ȡ����
                        ���Ź���Ա֤����� = 0;
                        ShowMsgToBox("��������ģʽ��", GroupShowBn);
                        return;
                    }
                    else if (keyIndex == 53)//����Ա����ӡ�����嵥
                    {
                        DataSave.PrintUserReturnLetter(_mUserBarCode, PrinterName);
                    }
                    CurrentState = EnumGroupStatus.����;
                }
                #endregion
            }

        }
        #endregion

        #region �ϱ��������Ƭ
        /// <summary>
        /// �ϱ��������Ƭ
        /// </summary>
        /// <param name="Cardcode">����ֵ</param>
        public void GetM3Photo(string info)
        {
            _mSnapFileName = info;
        }
        #endregion


        #region �����û�ȡ����Ϣ
        public void SaveUserGetLetter_QuJianPintTai(bool bOpenDoor, bool bSend, bool bRecv)
        {
            List<UserGetBoxInfo> boxs = _mUserBoxs;
            List<int> opendBox = new List<int>();
            //����
            if (bOpenDoor)
            {
                foreach (UserGetBoxInfo b in boxs)
                {
                    if (boxs.Count <= 1 || _mBoxstatus[b.BoxNo].LetterCount > 0 || LogInfo.Constant.OpenDoorType)
                    {
                        //����Ļ
                        _mBoxstatus[b.BoxNo].FrontBox.IsCloseScreen = false;
                        //����
                        if (BoxDataParse.DataParse.CmdOpenDoor(_mBoxstatus[b.BoxNo].FrontBox.BoxBN, true))
                        {
                            opendBox.Add(b.BoxNo);
                            //����ȡ����Ϣ
                            DataSave.Box_UserGetLetter(b.BoxNo, _mUserBarCode, "", true, bSend, bRecv, false, PrinterName, GroupName);
                            //���ö�Ӧ��ͷ��״̬
                            _mBoxstatus[b.BoxNo].LetterCount = 0;
                            _mBoxstatus[b.BoxNo].LampYiQu = true;
                            _mBoxstatus[b.BoxNo].LampJiJian = false;
                            //֪ͨ����
                            //֪ͨ�ͻ��ˣ��Ƹı�
                            SendClientNotify.NotifyLamp(b.BoxNo.ToString("000"), "1", "on");
                            //֪ͨ�ͻ��ˣ��ż�Ϊ 0
                            SendClientNotify.NotifyLetter(b.BoxNo.ToString("000"), "false", 0);
                        }
                    }
                }
            }

            //��Ƭ
            ShowMsgToBox("���ڴ������ݣ����Ժ򡣡���", GroupShowBn);
            if (_mDvrip == "" || _mDvrPort == 0)
            {
                if (GroupSheXiangTouBn.Length == 7)
                    BoxDataParse.DataParse.CmdTakePhoto(GroupSheXiangTouBn, 2);
            }
            if ((_mDvrip != "" && _mDvrPort != 0) || GroupSheXiangTouBn.Length == 7)
            {
                uint dtOld = LogInfo.Win32API.GetTickCount();
                while (LogInfo.Win32API.GetTickCount() - dtOld < 6000 && string.IsNullOrEmpty(_mSnapFileName))
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            ShowMsgToBox("��", GroupShowBn);
            if (bOpenDoor)
            {
                foreach (int boxNo in opendBox)
                {
                    DataSave.Box_UserGetLetter_SetPic(boxNo, _mUserBarCode, _mSnapFileName);
                }
            }
            else
            {
                foreach (UserGetBoxInfo b in boxs)
                {
                    //�����ţ�ֻ��ӡ�嵥���з����嵥��ֻ��ӡ�嵥
                    DataSave.Box_UserGetLetter(b.BoxNo, _mUserBarCode, _mSnapFileName, false, bSend, bRecv, true, PrinterName, GroupName);
                }
            }
        }
        public void SaveUserGetLetter(int boxNo, bool bTimeOut, bool bSend, bool bRecv)
        {
            byte[] data;
            string picdata = "";
            if (_mSnapFileName != "" && _mSnapFileName != null)
            {
                if (DvrCommand.GetFileCommand(_mSnapFileName, out data))
                {
                    picdata = Convert.ToBase64String(data);
                }
            }
            DataSave.Box_UserGetLetter(boxNo, _mUserBarCode, picdata, true, bSend, bRecv, true, PrinterName, GroupName);

            //���ö�Ӧ��ͷ��״̬
            MBoxInfo[boxNo].Box.LetterCount = 0;
            MBoxInfo[boxNo].Box.LampYiQu = true;
            MBoxInfo[boxNo].Box.LampJiJian = false;
            MBoxInfo[boxNo].Box.CheckBoxLetter();

            //֪ͨ����
            //֪ͨ�ͻ��ˣ��Ƹı�
            SendClientNotify.NotifyLamp(boxNo.ToString("000"), "1", "on");
            //֪ͨ�ͻ��ˣ��ż�Ϊ 0
            SendClientNotify.NotifyLetter(boxNo.ToString("000"), "false", 0);
        }
        public void SaveUserGetOneLetter(string barCode)
        {
            //�ҵ���ǰ���
            int boxNo = 0;
            foreach (StBoxInfo b in MBoxInfo.Values)
            {
                if (b.BUsed)
                {
                    boxNo = b.BoxNo; break;
                }
            }
            if (boxNo <= 0) return;

            DataSave.Box_UserGetOneLetter(boxNo, _mUserBarCode, barCode);

            //���ö�Ӧ��ͷ��״̬
            MBoxInfo[boxNo].Box.LampYiQu = true;
            //��ȡ���ݿ⣬�õ��ż���Ŀ�ͼ�����״̬
            DataBase.MonitorService.BoxInfo t = DataSave.GetBoxLetterCount(boxNo);
            if (t != null)
            {
                MBoxInfo[boxNo].Box.LampJiJian = t.HasJiJian;
                MBoxInfo[boxNo].Box.LetterCount = t.SendCount;
                //���ؽṹ��֪ͨ��ͷ
                MBoxInfo[boxNo].Box.CheckBoxLetter();

                //֪ͨ�ͻ��ˣ��ı��ż�
                SendClientNotify.NotifyLetter(boxNo.ToString("000"), (t.HasJiJian ? "true" : "false"), t.SendCount);
            }
        }
        #endregion

        #region ���濱����Ϣ
        public void SaveErataInfo(string letterCode)
        {
            //�ҵ���ǰ���
            int boxNo = 0;
            foreach (StBoxInfo sb in MBoxInfo.Values)
            {
                if (sb.BUsed)
                {
                    boxNo = sb.BoxNo; break;
                }
            }
            if (boxNo <= 0) return;
            bool b = DataSave.SaveErratumLetter(boxNo, letterCode, _mAdminBarCode, _mUserBarCode);
            if (b)
            {
                ShowMsgToBox("���뿱��ɹ����������", GroupShowBn, 255);
                BoxDataParse.DataParse.CmdBuzzer(GroupScanBn, 1);
            }
            else
            {
                ShowMsgToBox("���뿱��ʧ�ܣ������¿���", GroupShowBn, 255);
                BoxDataParse.DataParse.CmdBuzzer(GroupScanBn, 2);
            }
            //��ȡ���ݿ⣬�õ��ż���Ŀ�ͼ�����״̬
            DataBase.MonitorService.BoxInfo t = DataSave.GetBoxLetterCount(boxNo);
            if (t != null)
            {
                MBoxInfo[boxNo].Box.LampJiJian = t.HasJiJian;
                MBoxInfo[boxNo].Box.LetterCount = t.SendCount;
                //���ؽṹ��֪ͨ��ͷ
                MBoxInfo[boxNo].Box.CheckBoxLetter();

                //֪ͨ�ͻ��ˣ��ı��ż�
                SendClientNotify.NotifyLetter(boxNo.ToString("000"), (t.HasJiJian ? "true" : "false"), t.SendCount);
            }
        }
        #endregion

        #region ������Ϣ
        public void SetGongGaoInfo(string msg)
        {
            if (_mGongGaoInfo == msg)
                return;
            _mGongGaoInfo = msg;
            if (CurrentState == EnumGroupStatus.����)
            {
                //��ʾ
                if (!_mIs4Dai)
                    BoxDataParse.DataParse.CmdLCDText_Com(GroupShowBn, 0, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, LogInfo.Constant.TextLeft, LogInfo.Constant.TextTop, _mGongGaoInfo);
            }
        }
        #endregion

        #region ��bn��Ӧ����Ļ
        private void OpenBoxScreen(string bn)
        {
            foreach (StBoxInfo box in MBoxInfo.Values)
            {
                if (box.BoxBn == bn)
                {
                    box.BoxStatus.IsCloseScreen = false;
                    break;
                }
            }
        }
        #endregion

        #region ��ʾ��Ϣ����ͷ����ʾ��
        public void ShowMsgToBox(string msg, string bnNo)
        {
            ShowMsgToBox(msg, bnNo, (int)LogInfo.Constant.TimeOut_ShowInfoMsg);
        }
        public void ShowMsgToBox(string msg, string bnNo, int time)
        {
            if (msg == "")
                return;
            if (GroupShowBn != null && GroupShowBn != "")
            {
                if (MBoxInfo.Count > 1 || MIsQuJianPingTai)
                {
                    if (_mIs4Dai)
                        BoxDataParse.DataParse.CmdLCDText_4Dai(GroupShowBn, 3, time, msg);
                    else
                        BoxDataParse.DataParse.CmdLCDText_Com(GroupShowBn, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, time, LogInfo.Constant.TextLeft, LogInfo.Constant.TextTop, msg);
                }
                else
                {
                    //��ʾ����ʾ��Ļʱ��Ӧ�ô���Ļ
                    OpenBoxScreen(GroupShowBn);
                    BoxDataParse.DataParse.CmdLCDText(GroupShowBn, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, time, msg);
                }
            }
            if (GroupShowBn != bnNo)  //b
            {
                //��ʾ����ʾ��Ļʱ��Ӧ�ô���Ļ
                OpenBoxScreen(bnNo);
                BoxDataParse.DataParse.CmdLCDText(bnNo, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, time, msg);
            }
        }
        public void ShowMsg(int id)
        {
            ShowMsg(id, 255);
        }
        public void ShowMsg(int id, double time)
        {
            string str = LogInfo.NotifyMsg.GetText(id);
            if (str != "")
            {
                if (GroupShowBn != null && GroupShowBn != "")
                {
                    if (MBoxInfo.Count > 1 || MIsQuJianPingTai)
                    {
                        if (_mIs4Dai)
                            BoxDataParse.DataParse.CmdLCDText_4Dai(GroupShowBn, 3, (int)time, str);
                        else
                            BoxDataParse.DataParse.CmdLCDText_Com(GroupShowBn, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, (int)time, LogInfo.Constant.TextLeft, LogInfo.Constant.TextTop, str);
                    }
                    else
                    {
                        //��ʾ����ʾ��Ļʱ��Ӧ�ô���Ļ
                        OpenBoxScreen(GroupShowBn);
                        BoxDataParse.DataParse.CmdLCDText(GroupShowBn, 3, LogInfo.enum_TextType.��ʾ�������ı�, 0, (int)time, str);
                    }
                }
            }
        }
        #endregion
        #region ������ʾ
        public void PlaySound(int id)
        {
            if (GroupSoundBn == null || GroupSoundBn == "")
                return;
            string str = LogInfo.NotifyMsg.GetSound(id);
            if (str != "")
            {
                BoxDataParse.DataParse.CmdSound(GroupSoundBn, LogInfo.enum_TextType.��ʾ�������ı�, 0, str);
            }
        }
        #endregion

        #region ��ʱ������
        public void TimeOut()
        {
            //�������״̬
            if (CurrentState == EnumGroupStatus.���� || CurrentState == EnumGroupStatus.Ͷ�䱨��)
                return;

            uint dt = LogInfo.Win32API.GetTickCount() - _mDtLastOperation;
            dt /= 1000;
            bool b = false;
            if (CurrentState == EnumGroupStatus.Ͷ�俪�Ž� || CurrentState == EnumGroupStatus.���˿��Ž�)
            {
                if (dt > LogInfo.Constant.TimeOut_PutInLetter)
                {
                    b = true;
                }
            }
            else if (CurrentState == EnumGroupStatus.����ͨͶ || CurrentState == EnumGroupStatus.ͨ��ַ�
                || CurrentState == EnumGroupStatus.ͨ��ģ���б� || CurrentState == EnumGroupStatus.ͨ���շ�
                || CurrentState == EnumGroupStatus.ͨ�뷽ʽѡ�� || CurrentState == EnumGroupStatus.ͨ��ģ���б�ȷ��
                || CurrentState == EnumGroupStatus.����ͨͶ || CurrentState == EnumGroupStatus.����)
            {
                if (dt > LogInfo.Constant.TimeOut_SendLetter)
                {
                    b = true;
                }
            }
            else if (CurrentState == EnumGroupStatus.�û�ȡ������ || CurrentState == EnumGroupStatus.�û�ȡ�����Ų���ȡ��
                || CurrentState == EnumGroupStatus.�û�ȡ��ȡ��ƽ̨ || CurrentState == EnumGroupStatus.�û�ȡ��ȡ��ƽ̨��λѡ��
                || CurrentState == EnumGroupStatus.����ά�� || CurrentState == EnumGroupStatus.������
                || CurrentState == EnumGroupStatus.ά������ || CurrentState == EnumGroupStatus.������)
            {
                if (dt > LogInfo.Constant.TimeOut_CloseDoor)
                {
                    b = true;
                }
            }
            else if (CurrentState == EnumGroupStatus.��ʾ��Ϣ��ʾ)
            {
                if (dt > LogInfo.Constant.TimeOut_ShowInfoMsg)
                {
                    b = true;
                }
            }
            if (b)
            {
                _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(TimeOutHandler));
            }
        }
        private void TimeOutHandler(object o)
        {
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "��ʱ��" + CurrentState.ToString());
            try
            {
                if (CurrentState == EnumGroupStatus.Ͷ�俪�Ž� || CurrentState == EnumGroupStatus.���˿��Ž�)
                {
                    _mJingGaoBoxNo.Clear();
                    foreach (StBoxInfo b in MBoxInfo.Values)
                    {
                        if (!b.BUsed) continue;
                        b.BUsed = false;
                        b.Box.CheckBoxLetter();
                        if (b.BoxBn != GroupScanBn)
                            BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                        else
                            BoxDataParse.DataParse.CmdBoxGating(b.BoxBn, false);
                        if (b.BoxBn != GroupShowBn)
                            ShowMsgToBox(LogInfo.NotifyMsg.GetText(46), b.BoxBn);
                        _mJingGaoBoxNo.Add(b.BoxNo);
                    }
                    //��¼��־
                    string logMessage = "��ʱ����ǰ״̬��" + CurrentState.ToString() + "�����룺" + _mLetterBarCode + "\r\n";
                    LogInfo.Log.WriteFileLog(logMessage);

                    PlaySound(46);
                    _mJingGaoString = LogInfo.NotifyMsg.GetText(46);
                    CurrentState = EnumGroupStatus.Ͷ�䱨��;
                }
                else if (CurrentState == EnumGroupStatus.����ͨͶ || CurrentState == EnumGroupStatus.ͨ��ַ�
                    || CurrentState == EnumGroupStatus.ͨ��ģ���б� || CurrentState == EnumGroupStatus.ͨ���շ�
                    || CurrentState == EnumGroupStatus.ͨ�뷽ʽѡ�� || CurrentState == EnumGroupStatus.ͨ��ģ���б�ȷ��
                    || CurrentState == EnumGroupStatus.����ͨͶ || CurrentState == EnumGroupStatus.����)
                {
                    _mJingGaoBoxNo.Clear();
                    foreach (StBoxInfo b in MBoxInfo.Values)
                    {
                        if (!b.BUsed) continue;
                        b.BUsed = false;
                        b.Box.CheckBoxLetter();
                        if (b.BoxBn != GroupScanBn)
                            BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                        else
                            BoxDataParse.DataParse.CmdBoxGating(b.BoxBn, false);
                        if (CurrentState == EnumGroupStatus.ͨ��ַ�)
                            _mJingGaoBoxNo.Add(b.BoxNo);
                    }
                    if (CurrentState == EnumGroupStatus.ͨ��ַ�)
                    {
                        //��¼��־
                        string logMessage = "��ʱ����ǰ״̬��" + CurrentState.ToString() + "�����룺" + _mLetterBarCode + "\r\n";
                        LogInfo.Log.WriteFileLog(logMessage);

                        PlaySound(46);
                        _mJingGaoString = LogInfo.NotifyMsg.GetText(46);
                        CurrentState = EnumGroupStatus.Ͷ�䱨��;
                    }
                    else
                        CurrentState = EnumGroupStatus.����;
                }
                else if (CurrentState == EnumGroupStatus.�û�ȡ������ || CurrentState == EnumGroupStatus.�û�ȡ�����Ų���ȡ��
                    || CurrentState == EnumGroupStatus.�û�ȡ��ȡ��ƽ̨ || CurrentState == EnumGroupStatus.�û�ȡ��ȡ��ƽ̨��λѡ��
                    || CurrentState == EnumGroupStatus.����ά�� || CurrentState == EnumGroupStatus.������
                    || CurrentState == EnumGroupStatus.ά������ || CurrentState == EnumGroupStatus.������)
                {
                    foreach (StBoxInfo b in MBoxInfo.Values)
                    {
                        if (!b.BUsed) continue;
                        BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                        b.BUsed = false;
                        if (BFront)
                            b.Box.FrontBox.Door = DoorGateStatus.OpenTimeOut;
                        else
                            b.Box.BackBox.Door = DoorGateStatus.OpenTimeOut;
                        if (CurrentState == EnumGroupStatus.�û�ȡ������)
                        {
                            SaveUserGetLetter(b.BoxNo, true, false, false);
                        }
                        else
                        {
                            b.Box.CheckBoxLetter();
                        }
                    }
                    CurrentState = EnumGroupStatus.����;
                }
                else if (CurrentState == EnumGroupStatus.��ʾ��Ϣ��ʾ)
                {
                    CurrentState = EnumGroupStatus.����;
                }

            }
            catch
            {
            }
        }
        #endregion
    }
}
