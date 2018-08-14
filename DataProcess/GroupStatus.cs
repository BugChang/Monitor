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
    /// GroupStatus 的摘要说明。
    /// </summary>
    public class GroupStatus
    {
        public int 退信管理员证卡编号 = 0;

        #region 系统状态定义
        public enum EnumGroupStatus
        {
            空闲 = 1,
            提示信息显示 = 101,
            投箱报警 = 102,
            取件未清空报警 = 103,
            条码通投 = 2,
            投箱开门禁 = 3,
            通码方式选择 = 4,
            通码普发 = 5,
            通码模板列表 = 6,
            通码模板列表确认 = 8,
            通码分发 = 7,

            用户开门投件Pre = 9,
            用户开门投件 = 10,

            用户取件开门 = 11,
            用户取件开门部分取件 = 12,
            用户取件取件平台 = 13,
            用户取件取件平台单位选择 = 14,

            管理维护 = 21,
            维护开门 = 22,
            管理勘误 = 23,
            勘误开门 = 24,

            清退 = 31,
            清退通投 = 32,
            清退开门禁 = 33
        }
        #endregion

        #region 组箱格信息类
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

        //用户投信后通知界面更改信数事件
        public event BoxLetterCountChange OnLetterIn;
        public event DGetGroupFromBoxNo GetGroupFromBoxNo;

        //组信息
        public bool BFront;
        public string GroupName;//箱组名称
        public string GroupScanBn;//前面扫描头bn号
        public string GroupCardBn;//前面读卡器bn号
        public string GroupShowBn;//前面多功能屏bn号
        public string GroupSoundBn;//前面语音BN号
        public string GroupZhiJingMaiBn;//前面多功能屏bn号
        public string GroupSheXiangTouBn;//前面语音BN号

        public string PrinterName;//清单打印机

        private string _mDvrip = "";    //箱组上摄像头使用的DVR录像机的Ip地址
        private int _mDvrPort = 0;     //箱组上摄像头使用的DVR录像机的Ip端口
        private int _mDvrChannel = 0;  //箱组上摄像头使用的DVR录像机的摄像头端口，第一路为0，依次类推

        private bool _mBStartRec;
        private string _mRecFileName;
        private string _mSnapFileName;

        public EnumGroupStatus MCurrentState = EnumGroupStatus.空闲;
        public Dictionary<int, StBoxInfo> MBoxInfo = new Dictionary<int, StBoxInfo>();
        public string[] MDaiGuanGroup;

        /// <summary>
        /// 是否是自助取件，如果是，则没有箱头
        /// </summary>
        public bool MIsQuJianPingTai;
        Dictionary<int, BoxInfo> _mBoxstatus;
        Dictionary<string, GroupStatus> _mGroupstatus;

        /// <summary>
        /// 公告信息
        /// </summary>
        private string _mGongGaoInfo = "";

        private string _mLetterBarCode = "";
        /// <summary>
        /// 现在保存的是取件证卡的证卡编号
        /// </summary>
        private string _mUserBarCode = "";
        /// <summary>
        /// 现在保存的是取件证卡的证卡编号
        /// </summary>
        private string _mAdminBarCode = "";
        private bool _mBJiaJi = false;
        private uint _mDtLastOperation;

        /// <summary>
        /// 用户检查证卡后，返回的证卡类型
        /// </summary>
        private BarCodeType _mLastCheckCard;
        /// <summary>
        /// 用户检查证卡后，返回的可以打开的箱格列表
        /// </summary>
        private List<UserGetBoxInfo> _mUserBoxs;
        /// <summary>
        /// 用户取件箱格大于12个的时候，一屏幕显示不全，
        /// 这个时候，再次刷卡，会进入下12个单位信息。
        /// </summary>
        private int _mUserBoxsIndex;

        private DataBase.MonitorService.ClassBarcodeMuBanList[] _mCurrentMoBan;
        private int _mCurIndex;

        /// <summary>
        /// 触发光电状态
        /// </summary>
        private bool _mBChufa;

        /// <summary>
        /// 是不是4代的Arm箱头，为4s的兼容使用
        /// </summary>
        private bool _mIs4Dai;

        private string _mJingGaoString;
        private List<int> _mJingGaoBoxNo;


        #region 构造函数，初始化变量信息
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

            //添加到组列表中
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
                //初始化设备号
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

        #region  连接状态
        private bool _mConnected;		//连接状态
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
                    //所有分箱初始化
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

                        string str = LogInfo.NotifyMsg.GetText((int)EnumGroupStatus.空闲);
                        if (str == "") str = "请扫描条码";
                        str = GroupName + "\r\n" + str;

                        if (!_mIs4Dai)
                            BoxDataParse.DataParse.CmdLCDText_Com(GroupShowBn, 0, LogInfo.enum_TextType.显示附带的文本, 0, 255, LogInfo.Constant.TextLeft, LogInfo.Constant.TextTop, str);
                    }
                    CurrentState = EnumGroupStatus.空闲;
                }
                _dtConnect = LogInfo.Win32API.GetTickCount();
                _mConnected = value;
            }
        }
        #endregion

        #region 检查逻辑箱号是否属于本组
        public bool CheckBoxInGroup(int boxNo)
        {
            return MBoxInfo.ContainsKey(boxNo);
        }
        #endregion

        #region  状态改变
        public EnumGroupStatus CurrentState
        {
            get
            {
                return MCurrentState;
            }
            set
            {
                _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                #region 打开箱头对应屏幕
                if (value != EnumGroupStatus.空闲)
                {
                    foreach (StBoxInfo box in MBoxInfo.Values)
                    {
                        box.BoxStatus.IsCloseScreen = false;
                    }
                }
                #endregion
                MCurrentState = value;
                if (value == EnumGroupStatus.空闲 || value == EnumGroupStatus.提示信息显示)
                {
                    if (OnLetterIn != null)
                    {
                        OnLetterIn -= new BoxLetterCountChange(g_OnLetterIn);
                        OnLetterIn = null;
                    }
                }
                if (value == EnumGroupStatus.空闲)
                {
                    #region
                    foreach (StBoxInfo b in MBoxInfo.Values)
                    {
                        if (b.BUsed && b.BoxBn != GroupScanBn)
                        {
                            BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                            //数字
                            BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.绿色);
                            b.BUsed = false;
                        }
                        //出差信息
                        if (b.Box.LeaderOutMessage.CompareTo("") != 0)
                        {
                            BoxDataParse.DataParse.CmdLCDText(b.BoxBn, 3, LogInfo.enum_TextType.显示附带的文本, 0, 255, b.Box.LeaderOutMessage);
                        }
                        else if (b.Box.bSyncError)
                        {
                            BoxDataParse.DataParse.CmdLCDText(b.BoxBn, 3, LogInfo.enum_TextType.显示附带的文本, 0, 255, b.Box.GetSyncErrorMsg);
                        }
                        else if (b.BoxStatus.FullGDZheDang)
                        {
                            BoxDataParse.DataParse.CmdLCDText(b.BoxBn, 3, LogInfo.enum_TextType.显示附带的文本, 0, 255, b.BoxStatus.GetErrorString());
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
                            string str = LogInfo.NotifyMsg.GetText((int)EnumGroupStatus.空闲);
                            if (str == "") str = "请扫描条码";
                            str = GroupName + "\r\n" + str;
                            if (_mGongGaoInfo != "")
                                str = _mGongGaoInfo;
                            if (!_mIs4Dai)
                            {
                                BoxDataParse.DataParse.CmdLCDText_Com(GroupShowBn, 0, LogInfo.enum_TextType.显示附带的文本, 0, 255, LogInfo.Constant.TextLeft, LogInfo.Constant.TextTop, str);
                            }
                        }
                        if (MBoxInfo.Count > 0)
                        {
                            PlaySound((int)EnumGroupStatus.空闲);
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
                else if (value == EnumGroupStatus.提示信息显示)
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
                else if (value == EnumGroupStatus.投箱报警 || value == EnumGroupStatus.取件未清空报警)
                {
                    BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                    foreach (int bno in _mJingGaoBoxNo)
                    {
                        ShowMsgToBox(_mJingGaoString, MBoxInfo[bno].BoxBn, 255);
                        BoxDataParse.DataParse.CmdBoxLampCuoWu(MBoxInfo[bno].BoxBn, LogInfo.enum_LampStatus.闪烁);
                        BoxDataParse.DataParse.CmdBuzzer(MBoxInfo[bno].BoxBn, 3);
                    }
                }
                else
                {
                    #region 抓图 录像
                    //抓图
                    if (MCurrentState == EnumGroupStatus.用户取件开门)
                    {
                        if (!string.IsNullOrEmpty(_mDvrip))
                        {
                            bool b = DvrCommand.GetPicture(_mDvrip, _mDvrPort, _mDvrChannel, ref _mSnapFileName);
                            if (!b) _mSnapFileName = "";
                        }
                    }
                    //录像
                    if (MCurrentState == EnumGroupStatus.条码通投 || MCurrentState == EnumGroupStatus.投箱开门禁
                        || MCurrentState == EnumGroupStatus.清退通投 || MCurrentState == EnumGroupStatus.清退开门禁
                        || MCurrentState == EnumGroupStatus.通码分发 || MCurrentState == EnumGroupStatus.通码方式选择)
                    {
                        if (!_mBStartRec && !String.IsNullOrEmpty(_mDvrip))
                        {
                            _mRecFileName = "";
                            bool b = DvrCommand.StartRec(_mDvrip, _mDvrPort, _mDvrChannel, ref _mRecFileName);
                            if (!b) _mRecFileName = "";
                            _mBStartRec = true;
                        }
                    }
                    if (MCurrentState == EnumGroupStatus.清退)
                    {
                        if (_mBStartRec)
                        {
                            DvrCommand.StopRec(_mDvrip, _mDvrPort, _mDvrChannel);
                            _mBStartRec = false;
                        }
                    }
                    #endregion

                    #region 声音和文本提示
                    if (MCurrentState == EnumGroupStatus.条码通投 && MBoxInfo.Count == 1)
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

        #region 得到条码号后的处理，可以是信件，交换员，管理员条码
        public void GetBarCode_4Dai(string barcode)
        {
            _mIs4Dai = true;
            GetBarCode(barcode);
        }
        /// <summary>
        /// 得到条码号后的处理，可以是信件，交换员，管理员条码
        /// </summary>
        /// <param name="data">条码值</param>
        public void GetBarCode(string barcode)
        {
            GetBarCode(barcode, false);
        }

        /// <summary>
        /// 得到条码号后的处理，可以是信件，交换员，管理员条码
        /// </summary>
        /// <param name="barcode">条码值</param>
        /// <param name="bToIdle">出错时候，是否先转换到Idle屏幕</param>
        public void GetBarCode(string barcode, bool bToIdle)
        {
            BoxDataParse.DataParse.CmdBoxScan(GroupScanBn, false);
            //判断条码的类型
            BarCodeType barCodeType = BarCodeType.无效;
            List<SendBoxList> boxs;
            barCodeType = DataSave.CheckBarCodeType(barcode, out boxs);

            ProcessBarCode(barcode, bToIdle, barCodeType, boxs);
        }

        /// <summary>
        /// 得到条码号后的处理，可以是信件，交换员，管理员条码
        /// </summary>
        /// <param name="data">条码值</param>
        public void GetBarCode(string barcode, List<SendBoxList> boxs)
        {
            ProcessBarCode(barcode, false, BarCodeType.通码分发, boxs);
        }

        /// <summary>
        /// 处理条码
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="bToIdle"></param>
        /// <param name="barCodeType"></param>
        /// <param name="boxs"></param>
        private void ProcessBarCode(string barcode, bool bToIdle, BarCodeType barCodeType, List<SendBoxList> boxs)
        {
            if (CurrentState == EnumGroupStatus.空闲 || CurrentState == EnumGroupStatus.提示信息显示)
            {
                #region
                switch (barCodeType)
                {
                    case BarCodeType.无效:
                        {
                            if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                            ShowMsg(41, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(41);
                        }
                        break;

                    case BarCodeType.没有预发:
                        {
                            if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                            ShowMsg(41, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(41);
                        }
                        break;
                    case BarCodeType.条码没有预发:
                        {
                            if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                            ShowMsg(61, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(61);
                        }
                        break;

                    case BarCodeType.条码已经投箱:
                        {
                            if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                            ShowMsg(43, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(43);
                        }
                        break;

                    case BarCodeType.唯一直投:
                        #region 唯一直投
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
                                CurrentState = EnumGroupStatus.条码通投;
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

                    case BarCodeType.唯一指定:
                        #region 唯一指定
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
                                    //如果没有光电错误和没有门禁错误
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
                            if (sendBox.Count > 0)   //多开
                            {
                                //指定箱头的条码，准备投信
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
                                CurrentState = EnumGroupStatus.条码通投;
                            }
                            else if (bGuZhang)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //本箱格光电故障
                                ShowMsgToBox("箱格故障不能投箱", GroupShowBn);
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
                                //是在其他组
                                ShowMsgToBox("请到" + otherGroupname + "投箱", GroupShowBn);
                            }
                        }
                        #endregion
                        break;

                    case BarCodeType.唯一指定开门投件:
                        #region 唯一指定_开门投件
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
                                    //如果没有光电错误和没有门禁错误
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
                                //指定箱头的条码，准备投信
                                foreach (SendBoxList b in sendBox)
                                {
                                    MBoxInfo[b.BoxNo].BUsed = true;
                                    //开门
                                    BoxDataParse.DataParse.CmdOpenDoor(MBoxInfo[b.BoxNo].BoxBn, true);
                                }
                                _mLetterBarCode = barcode;
                                _mBJiaJi = false;
                                CurrentState = EnumGroupStatus.用户开门投件Pre;
                            }
                            else if (sendBox.Count > 1)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //找到多条，错误
                                ShowMsgToBox("条码不唯一，请重新预发", GroupShowBn);
                            }
                            else if (bGuZhang)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //本箱格光电故障
                                ShowMsgToBox("箱格故障不能投箱", GroupShowBn);
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
                                //是在其他组
                                ShowMsgToBox("请到" + otherGroupname + "投箱", GroupShowBn);
                            }
                        }
                        #endregion
                        break;

                    case BarCodeType.通码分发:
                        #region 通码分发
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
                                    //如果没有光电错误和没有门禁错误
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
                            if (sendBox.Count > 0)   //多开
                            {
                                //指定箱头的条码，准备投信
                                foreach (SendBoxList b in sendBox)
                                {
                                    MBoxInfo[b.BoxNo].BUsed = true;
                                    BoxDataParse.DataParse.CmdPreGetLetter(MBoxInfo[b.BoxNo].BoxBn, 1, 2, b.Count);
                                    MBoxInfo[b.BoxNo].Count = b.Count;
                                    MBoxInfo[b.BoxNo].SendCount = 0;
                                }
                                _mLetterBarCode = barcode;
                                BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, false, false, true, false);
                                CurrentState = EnumGroupStatus.通码分发;
                            }
                            else if (bGuZhang)
                            {
                                if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                                //本箱格光电故障
                                ShowMsgToBox("箱格故障不能投箱", GroupShowBn);
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
                                //是在其他组
                                ShowMsgToBox("请到" + otherGroupname + "投箱", GroupShowBn);
                            }
                        }
                        #endregion
                        break;

                    case BarCodeType.通码分发全组投箱:
                        #region 通码分发_全组投箱
                        {
                            List<GroupStatus> allgroup = new List<GroupStatus>();
                            //找到所有组
                            for (int ii = 0; ii < boxs.Count; ii++)
                            {
                                if (boxs[ii].BoxNo == 0)
                                    continue;
                                GroupStatus g = GetGroupFromBoxNo(boxs[ii].BoxNo);
                                if (!allgroup.Contains(g))
                                    allgroup.Add(g);
                            }
                            //判断所有组是不是都空闲
                            bool bIdle = true;
                            string msg = "";
                            foreach (GroupStatus g in allgroup)
                            {
                                if (g.CurrentState != EnumGroupStatus.空闲 && g.CurrentState != EnumGroupStatus.提示信息显示)
                                {
                                    msg = "“" + g.GroupName + "”正在操作，请稍候投箱。";
                                    bIdle = false;
                                    break;
                                }
                            }
                            if (!bIdle)
                            {
                                ShowMsgToBox(msg, GroupShowBn);
                                return;
                            }
                            //所有组空闲，进入投箱模式
                            _mAllgroup = allgroup;
                            foreach (GroupStatus g in allgroup)
                            {
                                g.OnLetterIn += new BoxLetterCountChange(g_OnLetterIn);
                                g.GetBarCode(barcode, boxs);
                            }
                        }
                        #endregion
                        break;

                    case BarCodeType.通码普发:
                        {
                            BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, true, false, true, false);
                            _mLetterBarCode = barcode;
                            CurrentState = EnumGroupStatus.通码方式选择;
                        }
                        break;
                    case BarCodeType.通码模板:
                        {
                            BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, false, true, true, false);
                            _mLetterBarCode = barcode;
                            CurrentState = EnumGroupStatus.通码方式选择;
                        }
                        break;
                    case BarCodeType.通码普发模板:
                        {

                            BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, true, true, true, false);
                            _mLetterBarCode = barcode;
                            CurrentState = EnumGroupStatus.通码方式选择;
                        }
                        break;

                    case BarCodeType.没有指定分发:
                        {
                            if (bToIdle) BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                            ShowMsg(60, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(60);
                        }
                        break;
                }
                #endregion
            }
            else if (CurrentState == EnumGroupStatus.清退)
            {
                #region
                //switch (BarCodeType)
                //{
                //    case DataBase.BarCodeType.无效:
                //        {
                //            ShowMsg(41, LogInfo.Constant.TimeOut_ShowInfoMsg);
                //            PlaySound(41);
                //        }
                //        break;

                //    case DataBase.BarCodeType.唯一直投:
                //    case DataBase.BarCodeType.唯一指定:
                //    case DataBase.BarCodeType.通码分发:
                //    case DataBase.BarCodeType.通码普发:
                //    case DataBase.BarCodeType.通码模板:
                //    case DataBase.BarCodeType.通码普发模板:
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
                //            CurrentState = enumGroupStatus.清退通投;
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
                CurrentState = EnumGroupStatus.清退通投;
                #endregion
            }
            else if (CurrentState == EnumGroupStatus.勘误开门)
            {
                #region
                switch (barCodeType)
                {
                    case BarCodeType.无效:
                        {
                            ShowMsg(41, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(41);
                        }
                        break;

                    case BarCodeType.唯一直投:
                    case BarCodeType.唯一指定:
                    case BarCodeType.通码分发:
                    case BarCodeType.通码普发:
                    case BarCodeType.通码模板:
                    case BarCodeType.通码普发模板:
                    case BarCodeType.条码已经投箱:
                        {
                            SaveErataInfo(barcode);
                            _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                        }
                        break;
                }
                #endregion
            }
            else if (CurrentState == EnumGroupStatus.用户取件开门 || CurrentState == EnumGroupStatus.用户取件开门部分取件)
            {
                #region
                switch (barCodeType)
                {
                    case BarCodeType.无效:
                        {
                            ShowMsg(41, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(41);
                        }
                        break;

                    case BarCodeType.唯一直投:
                    case BarCodeType.唯一指定:
                    case BarCodeType.通码分发:
                    case BarCodeType.通码普发:
                    case BarCodeType.通码模板:
                    case BarCodeType.通码普发模板:
                    case BarCodeType.条码已经投箱:
                        {
                            SaveUserGetOneLetter(barcode);
                            if (CurrentState == EnumGroupStatus.用户取件开门)
                                CurrentState = EnumGroupStatus.用户取件开门部分取件;
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
            //所有组, 对当前时间进行处理
            foreach (GroupStatus g1 in _mAllgroup)
            {
                if (g != g1)
                {
                    g1._mDtLastOperation = LogInfo.Win32API.GetTickCount();
                }
            }
        }
        #endregion

        #region 得到指静脉信息后的处理，交换员，管理员条码
        /// <summary>
        /// 得到指静脉信息后的处理
        /// </summary>
        /// <param name="info">指静脉的Base64字符串</param>
        /// <param name="iType">指静脉的类型，0：老的指静脉，1：燕南指静脉</param>
        public void GetZhiJingMaiInfo(string info, int iType)
        {
            BarCodeType barCodeType = BarCodeType.无效;
            List<UserGetBoxInfo> t;
            string userName;
            barCodeType = DataSave.CheckZhiJingMaiType(info, iType, out t, out userName);

            ProcessCardInfo(info, barCodeType, t, userName);
        }
        /// <summary>
        /// 得到指静脉信息后的处理
        /// </summary>
        /// <param name="locationId">指静脉位置信息</param>
        public void GetZhiJingMaiId(string locationId)
        {
            BarCodeType barCodeType = BarCodeType.无效;
            List<UserGetBoxInfo> t;
            string userName;
            barCodeType = DataSave.CheckZhiJingMaiLocationType(locationId, out t, out userName);

            ProcessCardInfo(locationId, barCodeType, t, userName);
        }
        #endregion

        #region 得到证卡号后的处理，交换员，管理员条码

        /// <summary>
        /// 得到证卡号后的处理，交换员，管理员条码
        /// </summary>
        /// <param name="cardcode">条码值</param>
        /// <param name="bn">箱子BN号</param>
        public void GetCardCode(string cardcode, string bn)
        {
            var barCodeType = DataSave.CheckCardType(bn, cardcode, out var t, out var userName);
            _mUserBarCode = cardcode;
            ProcessCardInfo(cardcode, barCodeType, t, userName);
        }

        private void ProcessCardInfo(string cardcode, BarCodeType barCodeType, List<UserGetBoxInfo> boxs, string userName)
        {
            //取件平台
            if (MIsQuJianPingTai)
            {
                if (barCodeType == BarCodeType.管理员 || barCodeType == BarCodeType.交换员)
                {
                    if (CurrentState == EnumGroupStatus.用户取件取件平台单位选择)
                    {
                        _mUserBarCode = boxs[0].证卡编号.ToString();
                    }
                    if (CurrentState == EnumGroupStatus.空闲)
                    {
                        _mUserBoxsIndex = 0;
                        #region
                        #region 检查要开的箱门在不在代管的组中
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
                                    otherUnitName = box.单位名称;
                                }
                                if (bHas)
                                    hasBox.Add(box);
                            }
                            if (hasBox.Count <= 0 && otherUnitName != "")
                            {
                                //本组不管理这些箱子，提示用户
                                string msg = "本取件柜不能取“" + otherUnitName + "”的文件，请到其他取件柜处取件。";
                                ShowMsgToBox(msg, GroupShowBn);
                                return;
                            }
                            boxs = hasBox;
                        }
                        #endregion

                        if (boxs.Count == 1)
                        {
                            #region 只有管理一个箱格，打开箱门取件即可。
                            //有效卡
                            _mLastCheckCard = barCodeType;
                            _mUserBarCode = boxs[0].证卡编号.ToString();
                            _mUserBoxs = boxs;
                            _mSnapFileName = "";
                            if (_mDvrip != "" && _mDvrPort > 0)
                            {
                                ShowMsgToBox("正在抓图，请稍候。。。", GroupShowBn);
                                bool b = CameraClass.Camera.GetPicture(_mDvrip, _mDvrPort, ref _mSnapFileName);
                                if (!b)
                                    _mSnapFileName = "";
                                ShowMsgToBox("　", GroupShowBn);
                            }
                            else if (GroupSheXiangTouBn.Length == 7)
                                BoxDataParse.DataParse.CmdTakePhoto(GroupSheXiangTouBn, 1);

                            if (barCodeType == BarCodeType.交换员)
                            {
                                BoxDataParse.DataParse.CmdQuJian(GroupShowBn, LogInfo.Constant.QuJianType);
                            }
                            else if (barCodeType == BarCodeType.管理员)
                            {
                                BoxDataParse.DataParse.CmdQuJian(GroupShowBn, 6);
                            }
                            CurrentState = EnumGroupStatus.用户取件取件平台;
                            if (退信管理员证卡编号 > 0)
                                ShowMsgToBox("系统在退信模式.....", GroupShowBn);
                            else
                                ShowMsgToBox("\r\n　　" + boxs[0].单位名称 + "\r\n\r\n取件人：" + boxs[0].用户名称, GroupShowBn);
                            #endregion
                        }
                        else if (boxs.Count >= 1)
                        {
                            #region 代管多个箱格，需要用户选择
                            //有效卡
                            _mLastCheckCard = barCodeType;
                            _mUserBarCode = boxs[0].证卡编号.ToString();
                            _mUserBoxs = boxs;
                            _mSnapFileName = "";
                            if (_mDvrip != "" && _mDvrPort > 0)
                            {
                                ShowMsgToBox("正在抓图，请稍候。。。", GroupShowBn);
                                bool b = CameraClass.Camera.GetPicture(_mDvrip, _mDvrPort, ref _mSnapFileName);
                                if (!b)
                                    _mSnapFileName = "";
                                ShowMsgToBox("　", GroupShowBn);
                            }
                            else if (GroupSheXiangTouBn.Length == 7)
                                BoxDataParse.DataParse.CmdTakePhoto(GroupSheXiangTouBn, 1);

                            List<string> units = new List<string>();
                            foreach (UserGetBoxInfo box in boxs)
                            {
                                units.Add(box.单位名称);
                                if (units.Count == 12) break;
                            }

                            BoxDataParse.DataParse.CmdUnitList(GroupShowBn, units.ToArray());
                            CurrentState = EnumGroupStatus.用户取件取件平台单位选择;
                            //ShowMsgToBox("\r\n　　" + UnitName + "\r\n\r\n取件人：" + UserName, GroupShowBN);
                            #endregion
                        }
                        else
                        {
                            //无效卡
                            ShowMsg(42, LogInfo.Constant.TimeOut_ShowInfoMsg);
                            PlaySound(42);
                        }
                        #endregion
                    }
                    else if (CurrentState == EnumGroupStatus.用户取件取件平台单位选择 && boxs[0].证卡编号.ToString() == _mUserBarCode)
                    {
                        if (_mUserBoxs.Count > 12)
                        {
                            #region 用户箱格多余12个，要分屏显示
                            _mUserBoxsIndex++;
                            if (_mUserBoxsIndex * 12 >= _mUserBoxs.Count) _mUserBoxsIndex = 0;

                            List<string> units = new List<string>();
                            for (int i = _mUserBoxsIndex * 12; i < _mUserBoxs.Count; i++)
                            {
                                units.Add(_mUserBoxs[i].单位名称);
                                if (units.Count == 12) break;
                            }

                            BoxDataParse.DataParse.CmdUnitList(GroupShowBn, units.ToArray());
                            CurrentState = EnumGroupStatus.用户取件取件平台单位选择;
                            #endregion
                        }
                    }
                }
                else
                {
                    //无效卡
                    ShowMsg(42, LogInfo.Constant.TimeOut_ShowInfoMsg);
                    PlaySound(42);
                }
                return;
            }
            //正常箱组
            switch (barCodeType)
            {
                case BarCodeType.无效:
                    {
                        ShowMsg(42, LogInfo.Constant.TimeOut_ShowInfoMsg);
                        PlaySound(42);
                    }
                    break;

                case BarCodeType.管理员:
                    if ((CurrentState == EnumGroupStatus.空闲 || CurrentState == EnumGroupStatus.提示信息显示) && MBoxInfo.Count > 0)
                    {
                        BoxDataParse.DataParse.CmdOpenDoor(MBoxInfo.FirstOrDefault().Value.BoxBn, true);
                        //m_AdminBarCode = boxs[0].证卡编号.ToString();
                        //m_UserBoxs = boxs;
                        //CurrentState = enumGroupStatus.管理维护;
                    }
                    else if ((CurrentState == EnumGroupStatus.空闲 || CurrentState == EnumGroupStatus.提示信息显示) || CurrentState == EnumGroupStatus.管理勘误)
                    {
                        _mAdminBarCode = "";
                        CurrentState = EnumGroupStatus.空闲;
                    }
                    break;
                case BarCodeType.交换员:
                    if (CurrentState == EnumGroupStatus.空闲 || CurrentState == EnumGroupStatus.提示信息显示)
                    {
                        //判断是否清退箱格
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
                            if ((CurrentState == EnumGroupStatus.空闲 || CurrentState == EnumGroupStatus.提示信息显示))
                            {
                                _mUserBarCode = boxs[0].证卡编号;
                                CurrentState = EnumGroupStatus.清退;
                            }
                            else if (CurrentState == EnumGroupStatus.清退)
                            {
                                _mUserBarCode = "";
                                CurrentState = EnumGroupStatus.空闲;
                            }
                        }
                        else if ((CurrentState == EnumGroupStatus.空闲 || CurrentState == EnumGroupStatus.提示信息显示))
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
                                //开门
                                iCount++;
                            }
                            if (iCount <= 0)
                            {
                                ShowMsgToBox("请到" + otherGroupname + "取件", GroupShowBn);
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
                                    _mUserBarCode = boxs[0].证卡编号.ToString();
                                    _mUserBoxs = boxs;
                                    //开门
                                    if (LogInfo.Constant.OpenDoorType || iCount <= 1 || MBoxInfo[b.BoxNo].Box.LetterCount > 0)
                                    {
                                        BoxDataParse.DataParse.CmdOpenDoor(MBoxInfo[b.BoxNo].BoxBn, true);
                                        MBoxInfo[b.BoxNo].BUsed = true;
                                        CurrentState = EnumGroupStatus.用户取件开门;
                                    }
                                }
                            }
                        }
                    }
                    else if (CurrentState == EnumGroupStatus.管理维护)
                    {
                        _mAdminBarCode = boxs[0].证卡编号.ToString();
                        CurrentState = EnumGroupStatus.管理勘误;
                    }
                    else if (CurrentState == EnumGroupStatus.清退)
                    {
                        CurrentState = EnumGroupStatus.空闲;
                    }
                    break;
            }
        }
        #endregion

        #region 取件单位选择
        /// <summary>
        /// 取件单位选择
        /// </summary>
        /// <param name="BoxNo"></param>
        public void 取件单位选择(List<int> idArray)
        {
            if (CurrentState == EnumGroupStatus.用户取件取件平台单位选择 && idArray.Count > 0)
            {
                List<UserGetBoxInfo> uBox = new List<UserGetBoxInfo>();
                string unitName = "";
                foreach (int id in idArray)
                {
                    uBox.Add(_mUserBoxs[id + _mUserBoxsIndex * 12]);
                    if (unitName == "")
                        unitName = _mUserBoxs[id + _mUserBoxsIndex * 12].单位名称;
                    else
                        unitName += "，" + _mUserBoxs[id + _mUserBoxsIndex * 12].单位名称;
                }
                _mUserBoxs = uBox;
                if (_mLastCheckCard == BarCodeType.交换员)
                {
                    BoxDataParse.DataParse.CmdQuJian(GroupShowBn, LogInfo.Constant.QuJianType);
                }
                else if (_mLastCheckCard == BarCodeType.管理员)
                {
                    BoxDataParse.DataParse.CmdQuJian(GroupShowBn, 6);
                }
                CurrentState = EnumGroupStatus.用户取件取件平台;
                if (退信管理员证卡编号 > 0)
                    ShowMsgToBox("系统在退信模式.....", GroupShowBn);
                else
                    ShowMsgToBox("\r\n　　" + unitName + "\r\n\r\n取件人：" + uBox[0].用户名称, GroupShowBn);
            }
        }
        #endregion

        #region 分箱扫描证卡，清退使用
        /// <summary>
        /// 分箱扫描证卡，清退使用
        /// </summary>
        /// <param name="cardcode">条码值</param>
        public void BoxGetCardCode(int boxNo, string cardcode)
        {
            BarCodeType barCodeType;
            List<UserGetBoxInfo> boxs;
            string userName;
            barCodeType = DataSave.CheckCardType("", cardcode, out boxs, out userName);
            switch (barCodeType)
            {
                case BarCodeType.无效:
                    {
                        ShowMsg(42, LogInfo.Constant.TimeOut_ShowInfoMsg);
                        PlaySound(42);
                    }
                    break;

                case BarCodeType.交换员:
                    if ((CurrentState == EnumGroupStatus.空闲 || CurrentState == EnumGroupStatus.提示信息显示))
                    {
                        _mUserBarCode = boxs[0].证卡编号.ToString();
                        CurrentState = EnumGroupStatus.清退;
                    }
                    else if (CurrentState == EnumGroupStatus.清退)
                    {
                        _mUserBarCode = "";
                        CurrentState = EnumGroupStatus.空闲;
                    }
                    break;
            }

        }
        #endregion

        #region 投入信件的处理，保存的数据库，关投口
        /// <summary>
        /// 投入信件的处理，保存的数据库，关投口
        /// </summary>
        /// <param name="boxNo"></param>
        public void LetterIn(int boxNo, int showCount)
        {
            if (!MBoxInfo.ContainsKey(boxNo)) return;
            if (!MBoxInfo[boxNo].BUsed)
            {
                //非正常投箱
                if (_mJingGaoBoxNo.Contains(boxNo) && CurrentState == EnumGroupStatus.投箱报警)
                {
                    _mJingGaoString = LogInfo.NotifyMsg.GetText(47);
                    ShowMsgToBox(_mJingGaoString, MBoxInfo[boxNo].BoxBn, 255);
                    BoxDataParse.DataParse.CmdBuzzer(MBoxInfo[boxNo].BoxBn, 3);
                }
                else if (CurrentState != EnumGroupStatus.投箱报警)
                {
                    if (!_mJingGaoBoxNo.Contains(boxNo))
                        _mJingGaoBoxNo.Add(boxNo);
                    _mJingGaoString = LogInfo.NotifyMsg.GetText(401);
                    CurrentState = EnumGroupStatus.投箱报警;
                }
                return;
            }
            int sendCount = 1;
            if (CurrentState == EnumGroupStatus.通码分发 && LogInfo.Constant.DistributeType == 0)
                sendCount = MBoxInfo[boxNo].Count;

            //转换到门禁打开的状态
            if (CurrentState == EnumGroupStatus.条码通投)
            {
                foreach (StBoxInfo b in MBoxInfo.Values)
                {
                    if (b.BoxNo != boxNo && b.BUsed)
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                        //数字
                        BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.绿色);
                        b.BUsed = false;
                    }
                }
                MCurrentState = EnumGroupStatus.投箱开门禁;
            }
            else if (CurrentState == EnumGroupStatus.清退通投)
            {
                foreach (StBoxInfo b in MBoxInfo.Values)
                {
                    if (b.BoxNo != boxNo && b.BUsed)
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                        //数字
                        BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.绿色);
                        b.BUsed = false;
                    }
                }
                MCurrentState = EnumGroupStatus.清退开门禁;
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
            //发信，记录到数据库
            bool 是否退信 = false;
            if (退信管理员证卡编号 > 0)
                是否退信 = true;
            int iRet = 0;
            if (CurrentState == EnumGroupStatus.清退开门禁 || CurrentState == EnumGroupStatus.清退通投)
                iRet = DataSave.SaveLetter(_mLetterBarCode, boxNo, _mBJiaJi, sendCount, _mUserBarCode, _mRecFileName, 是否退信, 退信管理员证卡编号.ToString());
            else
                iRet = DataSave.SaveLetter(_mLetterBarCode, boxNo, _mBJiaJi, sendCount, "", _mRecFileName, 是否退信, 退信管理员证卡编号.ToString());
            if (iRet > 0)
                MBoxInfo[boxNo].Box.LampJiJian = true;

            if (iRet >= 0)
            {
                //读取数据库，得到信件数目和急件灯状态
                DataBase.MonitorService.BoxInfo t = DataSave.GetBoxLetterCount(boxNo);
                if (t != null)
                {
                    MBoxInfo[boxNo].Box.LampJiJian = t.HasJiJian;
                    MBoxInfo[boxNo].Box.LetterCount = t.SendCount;
                    //本地结构，通知箱头
                    MBoxInfo[boxNo].Box.CheckBoxLetter();

                    //通知客户端，改变信件
                    SendClientNotify.NotifyLetter(boxNo.ToString("000"), (t.HasJiJian ? "true" : "false"), t.SendCount);
                }

                if (CurrentState == EnumGroupStatus.投箱开门禁)
                {
                    MBoxInfo[boxNo].BUsed = false;
                    BoxDataParse.DataParse.CmdBoxGating(MBoxInfo[boxNo].BoxBn, false);
                    //提示信息
                    ShowMsgToBox(LogInfo.NotifyMsg.GetText(44), MBoxInfo[boxNo].BoxBn);
                    PlaySound(44);
                    CurrentState = EnumGroupStatus.提示信息显示;
                }
                else if (CurrentState == EnumGroupStatus.清退开门禁)
                {
                    MBoxInfo[boxNo].BUsed = false;
                    BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[boxNo].BoxBn);
                    //提示信息
                    ShowMsgToBox(LogInfo.NotifyMsg.GetText(44), MBoxInfo[boxNo].BoxBn);
                    PlaySound(44);
                    //等待1秒后，回复初始状态
                    if (MBoxInfo.Count > 1)
                        System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000));
                    CurrentState = EnumGroupStatus.清退;
                }
                else if (CurrentState == EnumGroupStatus.通码普发)
                {
                    MBoxInfo[boxNo].SendCount += sendCount;
                    BoxDataParse.DataParse.CmdBoxLED(MBoxInfo[boxNo].BoxBn, 1, MBoxInfo[boxNo].SendCount, LogInfo.enum_LedColor.红色);
                }
                else if (CurrentState == EnumGroupStatus.通码分发)
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
                        //提示信息
                        ShowMsgToBox(LogInfo.NotifyMsg.GetText(44), MBoxInfo[boxNo].BoxBn);
                        PlaySound(44);
                        if (bold)
                        {
                            //等待1秒后，回复初始状态
                            System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000));
                            CurrentState = EnumGroupStatus.空闲;
                        }
                    }
                    else
                    {
                        BoxDataParse.DataParse.CmdBoxLED(MBoxInfo[boxNo].BoxBn, 1, MBoxInfo[boxNo].Count - MBoxInfo[boxNo].SendCount, LogInfo.enum_LedColor.红色);
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
                //警告音提示
                ShowMsgToBox("数据保存出错,请重新投箱。", MBoxInfo[boxNo].BoxBn);

                //等待3秒后，回复初始状态
                System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000) - 100);
                if (bold)
                {
                    CurrentState = EnumGroupStatus.空闲;
                }
            }
        }
        #endregion

        #region 投件抽出的处理，关投口
        /// <summary>
        /// 投件抽出的处理，关投口
        /// </summary>
        /// <param name="boxNo"></param>
        public void LetterOut(int boxNo, int showCount)
        {
            if (!MBoxInfo.ContainsKey(boxNo)) return;
            if (!MBoxInfo[boxNo].BUsed)
            {
                if (_mJingGaoBoxNo.Contains(boxNo) && CurrentState == EnumGroupStatus.投箱报警)
                {
                    _mJingGaoString = LogInfo.NotifyMsg.GetText(48);
                    ShowMsgToBox(_mJingGaoString, MBoxInfo[boxNo].BoxBn, 255);
                }
                return;
            }
            if (CurrentState == EnumGroupStatus.投箱开门禁 || CurrentState == EnumGroupStatus.清退开门禁
                || CurrentState == EnumGroupStatus.条码通投 || CurrentState == EnumGroupStatus.清退通投)
            {
                _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                foreach (StBoxInfo bi in MBoxInfo.Values)
                {
                    if (!bi.BUsed) continue;
                    bi.BUsed = false;
                    BoxDataParse.DataParse.CmdBoxGating(bi.BoxBn, false);
                    bi.Box.CheckBoxLetter();
                }

                //记录日志
                string logMessage = "投件抽出，当前状态：" + CurrentState.ToString() + "，条码：" + _mLetterBarCode + "，箱号：" + boxNo.ToString() + "\r\n";
                LogInfo.Log.WriteFileLog(logMessage);

                PlaySound(45);
                _mJingGaoString = LogInfo.NotifyMsg.GetText(45);
                _mJingGaoBoxNo.Clear();
                _mJingGaoBoxNo.Add(boxNo);
                CurrentState = EnumGroupStatus.投箱报警;
            }
            else if (CurrentState == EnumGroupStatus.通码分发)
            {
                //记录日志
                string logMessage = "投件抽出，当前状态：" + CurrentState.ToString() + "，条码：" + _mLetterBarCode + "，箱号：" + boxNo.ToString() + "\r\n";
                LogInfo.Log.WriteFileLog(logMessage);
            }
            else if (CurrentState == EnumGroupStatus.通码普发)
            {
                //记录日志
                string logMessage = "投件抽出，当前状态：" + CurrentState.ToString() + "，条码：" + _mLetterBarCode + "，箱号：" + boxNo.ToString() + "\r\n";
                LogInfo.Log.WriteFileLog(logMessage);
            }
        }
        #endregion

        #region 光电的处理
        /// <summary>
        /// 光电的处理
        /// </summary>
        /// <param name="boxNo">箱头的BN号</param>
        public void GdStateChange(int boxNo, string type)
        {
            if (!MBoxInfo.ContainsKey(boxNo)) return;
            if (!MBoxInfo[boxNo].BUsed)
            {
                if (_mJingGaoBoxNo.Contains(boxNo) && CurrentState == EnumGroupStatus.投箱报警)
                {
                    BoxDataParse.DataParse.CmdBoxLampCuoWu(MBoxInfo[boxNo].BoxBn, LogInfo.enum_LampStatus.闪烁);
                }
                return;
            }
            switch (type)
            {
                case "前光电故障":
                case "后光电故障":
                    MBoxInfo[boxNo].BUsed = false;
                    BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[boxNo].BoxBn);
                    ShowMsgToBox("光电长遮挡或故障", MBoxInfo[boxNo].BoxBn);
                    MBoxInfo[boxNo].Box.CheckBoxLetter();
                    if (CurrentState == EnumGroupStatus.通码分发)
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
                        //等待1秒后，回复初始状态
                        if (bold)
                        {
                            System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000));
                            CurrentState = EnumGroupStatus.空闲;
                        }
                    }
                    else if (CurrentState == EnumGroupStatus.投箱开门禁)
                    {
                        System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000));
                        CurrentState = EnumGroupStatus.空闲;
                    }
                    else if (CurrentState == EnumGroupStatus.清退开门禁)
                    {
                        System.Threading.Thread.Sleep((int)(LogInfo.Constant.TimeOut_ShowInfoMsg * 1000));
                        CurrentState = EnumGroupStatus.空闲;
                    }
                    break;

                case "箱满光电故障":
                    break;

                case "箱空光电故障":
                    MBoxInfo[boxNo].BUsed = false;
                    if (CurrentState == EnumGroupStatus.用户取件开门)
                    {
                        CurrentState = EnumGroupStatus.空闲;
                        ShowMsgToBox("箱空光电故障，请联系管理员", MBoxInfo[boxNo].BoxBn);
                    }
                    else if (CurrentState == EnumGroupStatus.用户取件开门部分取件)
                    {
                    }
                    break;
            }
        }
        /// <summary>
        /// 触发光电状态
        /// </summary>
        /// <param name="bZheDang"></param>
        public void ChufaGdChange(bool bZheDang)
        {
            _mBChufa = bZheDang;
            if (bZheDang && (CurrentState == EnumGroupStatus.空闲 || CurrentState == EnumGroupStatus.提示信息显示
                || CurrentState == EnumGroupStatus.清退 || CurrentState == EnumGroupStatus.勘误开门
                || CurrentState == EnumGroupStatus.用户取件开门 || CurrentState == EnumGroupStatus.用户取件开门部分取件))
            {
                BoxDataParse.DataParse.CmdBoxScan(GroupScanBn, true);
            }
            else if (!bZheDang)
            {
                //BoxDataParse.DataParse.CmdBoxScan(GroupScanBN, false);
            }
        }
        #endregion

        #region 门禁的处理
        /// <summary>
        /// 门禁的处理
        /// </summary>
        /// <param name="boxNo">箱头的BN号</param>
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
                    if (CurrentState == EnumGroupStatus.条码通投)
                    {
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            if (b.BoxNo != boxNo && b.BUsed)
                            {
                                BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                                //数字
                                BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.绿色);
                                b.BUsed = false;
                            }
                        }
                        if (MBoxInfo.Count > 1)
                            BoxDataParse.DataParse.CmdLCDText(MBoxInfo[boxNo].BoxBn, 3, LogInfo.enum_TextType.显示附带的文本, 0, 255, LogInfo.NotifyMsg.GetText((int)EnumGroupStatus.投箱开门禁));
                        CurrentState = EnumGroupStatus.投箱开门禁;
                    }
                    else if (CurrentState == EnumGroupStatus.清退通投)
                    {
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            if (b.BoxNo != boxNo && b.BUsed)
                            {
                                BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                                //数字
                                BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.绿色);
                                b.BUsed = false;
                            }
                        }
                        if (MBoxInfo.Count > 1)
                            BoxDataParse.DataParse.CmdLCDText(MBoxInfo[boxNo].BoxBn, 3, LogInfo.enum_TextType.显示附带的文本, 0, 255, LogInfo.NotifyMsg.GetText((int)EnumGroupStatus.清退开门禁));
                        CurrentState = EnumGroupStatus.清退开门禁;
                    }
                    break;

                case "Closed":
                    break;

                case "OpenTimeOut":
                    if (CurrentState == EnumGroupStatus.条码通投 || CurrentState == EnumGroupStatus.清退通投)
                    {
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                            //数字
                            BoxDataParse.DataParse.CmdBoxLED(b.BoxBn, 1, b.Box.LetterCount, LogInfo.enum_LedColor.绿色);
                            b.BUsed = false;
                        }
                        CurrentState = EnumGroupStatus.空闲;
                        ShowMsgToBox("门禁出错，请联系管理员", MBoxInfo[boxNo].BoxBn);
                    }
                    break;

                case "CloseTimeOut":
                    break;
            }
        }
        #endregion

        #region 门锁的处理
        /// <summary>
        /// 门锁的处理
        /// </summary>
        /// <param name="boxNo">箱头的BN号</param>
        public void DoorStateChange(int boxNo, string type)
        {
            if (!MBoxInfo.ContainsKey(boxNo)) return;
            if (_mJingGaoBoxNo.Contains(boxNo) && CurrentState == EnumGroupStatus.投箱报警)
            {
                BoxDataParse.DataParse.CmdBoxLampCuoWu(MBoxInfo[boxNo].BoxBn, LogInfo.enum_LampStatus.闪烁);
            }
            if (!MBoxInfo[boxNo].BUsed)
            {
                //判断是不是关门，关门时候箱空状态和箱头计数
                if (type == "Closed")
                {
                    if (MBoxInfo[boxNo].Box.LetterCount == 0 && MBoxInfo[boxNo].Box.LampYiQu)
                    {
                        if (MBoxInfo[boxNo].BoxStatus.ClearGDState)
                        {
                            //没有取空
                            MBoxInfo[boxNo].BoxStatus.ClearGDWarnning = true;
                            PlaySound(49);
                            string msg = LogInfo.NotifyMsg.GetText(49);
                            //提示到箱头
                            ShowMsgToBox(msg, MBoxInfo[boxNo].BoxBn, 255);
                        }
                    }
                }
                return;
            }
            switch (type)
            {
                case "Opened":
                    if (CurrentState == EnumGroupStatus.用户开门投件Pre)
                    {
                        //保存信息
                        SaveLetter(boxNo, 1);
                        CurrentState = EnumGroupStatus.用户开门投件;
                    }
                    else if (CurrentState == EnumGroupStatus.用户取件开门)
                    {
                    }
                    else if (CurrentState == EnumGroupStatus.维护开门)
                    {
                    }
                    else if (CurrentState == EnumGroupStatus.勘误开门)
                    {
                    }
                    break;

                case "Closed":
                    MBoxInfo[boxNo].BUsed = false;
                    if (CurrentState == EnumGroupStatus.用户开门投件)
                    {
                        //还原状态
                        CurrentState = EnumGroupStatus.空闲;
                    }
                    else if (CurrentState == EnumGroupStatus.用户取件开门)
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
                            CurrentState = EnumGroupStatus.取件未清空报警;
                        }
                        else
                        {
                            //保存取件信息
                            SaveUserGetLetter(boxNo, false, false, false);
                            if (!bHas)
                                CurrentState = EnumGroupStatus.空闲;
                        }
                    }
                    else if (CurrentState == EnumGroupStatus.用户取件开门部分取件)
                    {
                        foreach (StBoxInfo b in MBoxInfo.Values)
                        {
                            if (b.BUsed)
                            {
                                break;
                            }
                        }
                        CurrentState = EnumGroupStatus.空闲;
                    }
                    else if (CurrentState == EnumGroupStatus.维护开门)
                    {
                        CurrentState = EnumGroupStatus.管理维护;
                    }
                    else if (CurrentState == EnumGroupStatus.勘误开门)
                    {
                        CurrentState = EnumGroupStatus.管理勘误;
                    }
                    break;

                case "OpenTimeOut":
                    MBoxInfo[boxNo].BUsed = false;
                    if (CurrentState == EnumGroupStatus.用户开门投件
                        || CurrentState == EnumGroupStatus.用户开门投件Pre)
                    {
                        CurrentState = EnumGroupStatus.空闲;
                        ShowMsgToBox("门锁出错，请联系管理员", MBoxInfo[boxNo].BoxBn);
                    }
                    else if (CurrentState == EnumGroupStatus.用户取件开门)
                    {
                        CurrentState = EnumGroupStatus.空闲;
                        ShowMsgToBox("门锁出错，请联系管理员", MBoxInfo[boxNo].BoxBn);
                    }
                    else if (CurrentState == EnumGroupStatus.维护开门)
                    {
                        CurrentState = EnumGroupStatus.空闲;
                        ShowMsgToBox("门锁出错，请联系管理员", MBoxInfo[boxNo].BoxBn);
                    }
                    else if (CurrentState == EnumGroupStatus.勘误开门)
                    {
                        CurrentState = EnumGroupStatus.空闲;
                        ShowMsgToBox("门锁出错，请联系管理员", MBoxInfo[boxNo].BoxBn);
                    }
                    break;

                case "CloseTimeOut":
                    break;
            }
        }
        #endregion

        #region 按键的处理
        /// <summary>
        /// 按键的处理
        /// </summary>
        /// <param name="BN_NO">箱头的BN号</param>
        public void KeyPress(int boxNo, int keyIndex)
        {
            if (keyIndex == 0 || keyIndex == 1 || keyIndex == 60)
            {
                if (!MBoxInfo.ContainsKey(boxNo))
                {
                    if (MBoxInfo[boxNo].BoxStatus.GetErrorString() != "")
                    {
                        BoxDataParse.DataParse.CmdLCDText(MBoxInfo[boxNo].BoxBn, 3, LogInfo.enum_TextType.显示附带的文本, 0, (int)LogInfo.Constant.TimeOut_ShowInfoMsg, MBoxInfo[boxNo].BoxStatus.GetErrorString());
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
                //如果有错误信息，提示错误信息
                #region 箱头按键
                if (CurrentState == EnumGroupStatus.投箱开门禁)
                {
                    if (!MBoxInfo[boxNo].BUsed) return;
                    _mBJiaJi = !_mBJiaJi;
                    if (_mBJiaJi)
                        BoxDataParse.DataParse.CmdBoxLampJiJian(MBoxInfo[boxNo].BoxBn, LogInfo.enum_LampStatus.闪烁);
                    else
                        BoxDataParse.DataParse.CmdBoxLampJiJian(MBoxInfo[boxNo].BoxBn, LogInfo.enum_LampStatus.灭);
                    _mDtLastOperation = LogInfo.Win32API.GetTickCount();
                }
                else if (CurrentState == EnumGroupStatus.通码分发)
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
                else if (CurrentState == EnumGroupStatus.管理维护)
                {
                    MBoxInfo[boxNo].BUsed = true;
                    BoxDataParse.DataParse.CmdOpenDoor(MBoxInfo[boxNo].BoxBn, true);
                    CurrentState = EnumGroupStatus.维护开门;
                }
                else if (CurrentState == EnumGroupStatus.管理勘误)
                {
                    MBoxInfo[boxNo].BUsed = true;
                    BoxDataParse.DataParse.CmdOpenDoor(MBoxInfo[boxNo].BoxBn, true);
                    CurrentState = EnumGroupStatus.勘误开门;
                }
                else if (CurrentState == EnumGroupStatus.投箱报警)
                {
                    if (_mJingGaoBoxNo.Contains(boxNo))
                    {
                        //回复原始状态
                        foreach (int bno in _mJingGaoBoxNo)
                        {
                            BoxDataParse.DataParse.CmdBuzzer(MBoxInfo[bno].BoxBn, 0);
                            MBoxInfo[bno].Box.CheckBoxLetter();
                            BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[bno].BoxBn);
                        }
                        _mJingGaoString = "";
                        _mJingGaoBoxNo.Clear();
                        CurrentState = EnumGroupStatus.空闲;
                    }
                }
                else if (CurrentState == EnumGroupStatus.取件未清空报警)
                {
                    if (_mJingGaoBoxNo.Contains(boxNo))
                    {
                        //回复原始状态
                        foreach (int bno in _mJingGaoBoxNo)
                        {
                            BoxDataParse.DataParse.CmdBuzzer(MBoxInfo[bno].BoxBn, 0);
                            BoxDataParse.DataParse.CmdBoxToIdle(MBoxInfo[bno].BoxBn);
                        }
                        _mJingGaoString = "";
                        _mJingGaoBoxNo.Clear();

                        //保存取件信息
                        SaveUserGetLetter(boxNo, false, false, false);
                        CurrentState = EnumGroupStatus.空闲;
                    }
                }
                #endregion
            }
            else if (keyIndex == 10)//10：普发按键
            {
                #region
                if (CurrentState == EnumGroupStatus.通码方式选择)
                {
                    CurrentState = EnumGroupStatus.通码普发;
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
            else if (keyIndex == 11)//11：模板分发按键
            {
                #region
                if (CurrentState == EnumGroupStatus.通码方式选择)
                {
                    _mCurrentMoBan = DataSave.GetBarcodeMuBanList(_mLetterBarCode);
                    if (_mCurrentMoBan.Length > 0)
                    {
                        string[] mb = new string[_mCurrentMoBan.Length];
                        for (int i = 0; i < _mCurrentMoBan.Length; i++)
                            mb[i] = _mCurrentMoBan[i].模板名称;
                        BoxDataParse.DataParse.CmdTempletList(GroupShowBn, mb);
                        CurrentState = EnumGroupStatus.通码模板列表;
                    }
                    else
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                        CurrentState = EnumGroupStatus.空闲;
                        ShowMsgToBox("没有找到对应的模板，请登记后投箱", GroupShowBn);
                    }
                }
                #endregion
            }
            else if (keyIndex == 12)//12：取消退出按键
            {
                #region
                if (CurrentState == EnumGroupStatus.通码方式选择
                    || CurrentState == EnumGroupStatus.通码模板列表
                    || CurrentState == EnumGroupStatus.用户取件取件平台单位选择)
                {
                    CurrentState = EnumGroupStatus.空闲;
                }
                else if (CurrentState == EnumGroupStatus.通码模板列表确认)
                {
                    if (_mCurrentMoBan.Length > 0)
                    {
                        string[] mb = new string[_mCurrentMoBan.Length];
                        for (int i = 0; i < _mCurrentMoBan.Length; i++)
                            mb[i] = _mCurrentMoBan[i].模板名称;
                        BoxDataParse.DataParse.CmdTempletList(GroupShowBn, mb);
                        CurrentState = EnumGroupStatus.通码模板列表;
                    }
                    else
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                        CurrentState = EnumGroupStatus.空闲;
                        ShowMsgToBox("没有找到对应的模板，请登记后投箱", GroupShowBn);
                    }
                }
                else if (CurrentState == EnumGroupStatus.通码普发
                    || CurrentState == EnumGroupStatus.通码分发)
                {
                    foreach (StBoxInfo b in MBoxInfo.Values)
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(b.BoxBn);
                        b.Box.CheckBoxLetter();
                        b.BUsed = false;
                    }
                    BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                    CurrentState = EnumGroupStatus.空闲;
                }
                #endregion
            }
            else if (keyIndex == 13)//13：确认按键
            {
                #region
                if (CurrentState == EnumGroupStatus.通码模板列表确认)
                {
                    bool b = DataSave.GetMuBanInfo(_mLetterBarCode, _mCurrentMoBan[_mCurIndex].模板标识);
                    if (b)
                    {
                        MCurrentState = EnumGroupStatus.空闲;
                        GetBarCode(_mLetterBarCode, true);
                    }
                    else
                    {
                        BoxDataParse.DataParse.CmdBoxToIdle(GroupShowBn);
                        CurrentState = EnumGroupStatus.空闲;
                        ShowMsgToBox("没有找到对应的模板，请登记后投箱", GroupShowBn);
                    }
                }
                #endregion
            }
            else if (keyIndex > 20 && keyIndex <= 36)
            {
                //模板选择的索引+20
                if (CurrentState == EnumGroupStatus.通码模板列表 && (keyIndex - 20) <= _mCurrentMoBan.Length)
                {
                    _mCurIndex = keyIndex - 21;
                    BoxDataParse.DataParse.CmdScreenButton(GroupShowBn, false, false, true, true);
                    ShowMsgToBox("您确定要使用本模板吗？\r\n模板信息：" + _mCurrentMoBan[_mCurIndex].模板详情, GroupShowBn);
                    CurrentState = EnumGroupStatus.通码模板列表确认;
                }
            }
            else if (keyIndex >= 41 && keyIndex <= 60)//取件控制柜界面按钮选择
            {
                #region
                if (CurrentState == EnumGroupStatus.用户取件取件平台)
                {
                    ShowMsgToBox("　", GroupShowBn);
                    if (keyIndex == 41)//只开箱门
                    {
                        SaveUserGetLetter_QuJianPintTai(true, false, false);
                    }
                    else if (keyIndex == 42)//取消返回
                    {
                        CurrentState = EnumGroupStatus.空闲;
                    }
                    else if (keyIndex == 43)//打印清单（3按钮界面）
                    {
                        SaveUserGetLetter_QuJianPintTai(true, true, true);
                    }
                    else if (keyIndex == 44)//打印发件清单（4按钮界面）
                    {
                        SaveUserGetLetter_QuJianPintTai(false, true, false);
                    }
                    else if (keyIndex == 45)//打印收件清单（4按钮界面）
                    {
                        SaveUserGetLetter_QuJianPintTai(true, false, true);
                    }
                    else if (keyIndex == 46)//只打印清单（4按钮界面）
                    {
                        SaveUserGetLetter_QuJianPintTai(false, true, true);
                    }
                    else if (keyIndex == 51)//管理员，进入退信模式
                    {
                        //检查要开的箱门在不在代管的组中
                        if (MDaiGuanGroup.Length > 0)
                        {
                            foreach (string gName in MDaiGuanGroup)
                            {
                                foreach (GroupStatus g in _mGroupstatus.Values)
                                {
                                    if (g.GroupName == gName)
                                    {
                                        g.退信管理员证卡编号 = Convert.ToInt32(_mUserBarCode);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (GroupStatus g in _mGroupstatus.Values)
                            {
                                g.退信管理员证卡编号 = Convert.ToInt32(_mUserBarCode);
                            }
                        }
                        //本取件柜
                        退信管理员证卡编号 = Convert.ToInt32(_mUserBarCode);
                        ShowMsgToBox("进入退信模式。.", GroupShowBn);
                        return;
                    }
                    else if (keyIndex == 52)//管理员，退出退信模式
                    {
                        //检查要开的箱门在不在代管的组中
                        if (MDaiGuanGroup.Length > 0)
                        {
                            foreach (string gName in MDaiGuanGroup)
                            {
                                foreach (GroupStatus g in _mGroupstatus.Values)
                                {
                                    if (g.GroupName == gName)
                                    {
                                        g.退信管理员证卡编号 = 0;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (GroupStatus g in _mGroupstatus.Values)
                            {
                                g.退信管理员证卡编号 = 0;
                            }
                        }
                        //本取件柜
                        退信管理员证卡编号 = 0;
                        ShowMsgToBox("进入正常模式。", GroupShowBn);
                        return;
                    }
                    else if (keyIndex == 53)//管理员，打印退信清单
                    {
                        DataSave.PrintUserReturnLetter(_mUserBarCode, PrinterName);
                    }
                    CurrentState = EnumGroupStatus.空闲;
                }
                #endregion
            }

        }
        #endregion

        #region 上报拍摄的照片
        /// <summary>
        /// 上报拍摄的照片
        /// </summary>
        /// <param name="Cardcode">条码值</param>
        public void GetM3Photo(string info)
        {
            _mSnapFileName = info;
        }
        #endregion


        #region 保存用户取件信息
        public void SaveUserGetLetter_QuJianPintTai(bool bOpenDoor, bool bSend, bool bRecv)
        {
            List<UserGetBoxInfo> boxs = _mUserBoxs;
            List<int> opendBox = new List<int>();
            //开门
            if (bOpenDoor)
            {
                foreach (UserGetBoxInfo b in boxs)
                {
                    if (boxs.Count <= 1 || _mBoxstatus[b.BoxNo].LetterCount > 0 || LogInfo.Constant.OpenDoorType)
                    {
                        //打开屏幕
                        _mBoxstatus[b.BoxNo].FrontBox.IsCloseScreen = false;
                        //开门
                        if (BoxDataParse.DataParse.CmdOpenDoor(_mBoxstatus[b.BoxNo].FrontBox.BoxBN, true))
                        {
                            opendBox.Add(b.BoxNo);
                            //保存取件信息
                            DataSave.Box_UserGetLetter(b.BoxNo, _mUserBarCode, "", true, bSend, bRecv, false, PrinterName, GroupName);
                            //设置对应箱头的状态
                            _mBoxstatus[b.BoxNo].LetterCount = 0;
                            _mBoxstatus[b.BoxNo].LampYiQu = true;
                            _mBoxstatus[b.BoxNo].LampJiJian = false;
                            //通知界面
                            //通知客户端，灯改变
                            SendClientNotify.NotifyLamp(b.BoxNo.ToString("000"), "1", "on");
                            //通知客户端，信件为 0
                            SendClientNotify.NotifyLetter(b.BoxNo.ToString("000"), "false", 0);
                        }
                    }
                }
            }

            //照片
            ShowMsgToBox("正在传输数据，请稍候。。。", GroupShowBn);
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
            ShowMsgToBox("　", GroupShowBn);
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
                    //不开门，只打印清单，有发件清单和只打印清单
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

            //设置对应箱头的状态
            MBoxInfo[boxNo].Box.LetterCount = 0;
            MBoxInfo[boxNo].Box.LampYiQu = true;
            MBoxInfo[boxNo].Box.LampJiJian = false;
            MBoxInfo[boxNo].Box.CheckBoxLetter();

            //通知界面
            //通知客户端，灯改变
            SendClientNotify.NotifyLamp(boxNo.ToString("000"), "1", "on");
            //通知客户端，信件为 0
            SendClientNotify.NotifyLetter(boxNo.ToString("000"), "false", 0);
        }
        public void SaveUserGetOneLetter(string barCode)
        {
            //找到当前箱格
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

            //设置对应箱头的状态
            MBoxInfo[boxNo].Box.LampYiQu = true;
            //读取数据库，得到信件数目和急件灯状态
            DataBase.MonitorService.BoxInfo t = DataSave.GetBoxLetterCount(boxNo);
            if (t != null)
            {
                MBoxInfo[boxNo].Box.LampJiJian = t.HasJiJian;
                MBoxInfo[boxNo].Box.LetterCount = t.SendCount;
                //本地结构，通知箱头
                MBoxInfo[boxNo].Box.CheckBoxLetter();

                //通知客户端，改变信件
                SendClientNotify.NotifyLetter(boxNo.ToString("000"), (t.HasJiJian ? "true" : "false"), t.SendCount);
            }
        }
        #endregion

        #region 保存勘误信息
        public void SaveErataInfo(string letterCode)
        {
            //找到当前箱格
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
                ShowMsgToBox("条码勘误成功，请继续。", GroupShowBn, 255);
                BoxDataParse.DataParse.CmdBuzzer(GroupScanBn, 1);
            }
            else
            {
                ShowMsgToBox("条码勘误失败，请重新勘误。", GroupShowBn, 255);
                BoxDataParse.DataParse.CmdBuzzer(GroupScanBn, 2);
            }
            //读取数据库，得到信件数目和急件灯状态
            DataBase.MonitorService.BoxInfo t = DataSave.GetBoxLetterCount(boxNo);
            if (t != null)
            {
                MBoxInfo[boxNo].Box.LampJiJian = t.HasJiJian;
                MBoxInfo[boxNo].Box.LetterCount = t.SendCount;
                //本地结构，通知箱头
                MBoxInfo[boxNo].Box.CheckBoxLetter();

                //通知客户端，改变信件
                SendClientNotify.NotifyLetter(boxNo.ToString("000"), (t.HasJiJian ? "true" : "false"), t.SendCount);
            }
        }
        #endregion

        #region 公告信息
        public void SetGongGaoInfo(string msg)
        {
            if (_mGongGaoInfo == msg)
                return;
            _mGongGaoInfo = msg;
            if (CurrentState == EnumGroupStatus.空闲)
            {
                //显示
                if (!_mIs4Dai)
                    BoxDataParse.DataParse.CmdLCDText_Com(GroupShowBn, 0, LogInfo.enum_TextType.显示附带的文本, 0, 255, LogInfo.Constant.TextLeft, LogInfo.Constant.TextTop, _mGongGaoInfo);
            }
        }
        #endregion

        #region 打开bn对应的屏幕
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

        #region 显示信息到箱头的显示屏
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
                        BoxDataParse.DataParse.CmdLCDText_Com(GroupShowBn, 3, LogInfo.enum_TextType.显示附带的文本, 0, time, LogInfo.Constant.TextLeft, LogInfo.Constant.TextTop, msg);
                }
                else
                {
                    //显示到提示屏幕时候，应该打开屏幕
                    OpenBoxScreen(GroupShowBn);
                    BoxDataParse.DataParse.CmdLCDText(GroupShowBn, 3, LogInfo.enum_TextType.显示附带的文本, 0, time, msg);
                }
            }
            if (GroupShowBn != bnNo)  //b
            {
                //显示到提示屏幕时候，应该打开屏幕
                OpenBoxScreen(bnNo);
                BoxDataParse.DataParse.CmdLCDText(bnNo, 3, LogInfo.enum_TextType.显示附带的文本, 0, time, msg);
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
                            BoxDataParse.DataParse.CmdLCDText_Com(GroupShowBn, 3, LogInfo.enum_TextType.显示附带的文本, 0, (int)time, LogInfo.Constant.TextLeft, LogInfo.Constant.TextTop, str);
                    }
                    else
                    {
                        //显示到提示屏幕时候，应该打开屏幕
                        OpenBoxScreen(GroupShowBn);
                        BoxDataParse.DataParse.CmdLCDText(GroupShowBn, 3, LogInfo.enum_TextType.显示附带的文本, 0, (int)time, str);
                    }
                }
            }
        }
        #endregion
        #region 语音提示
        public void PlaySound(int id)
        {
            if (GroupSoundBn == null || GroupSoundBn == "")
                return;
            string str = LogInfo.NotifyMsg.GetSound(id);
            if (str != "")
            {
                BoxDataParse.DataParse.CmdSound(GroupSoundBn, LogInfo.enum_TextType.显示附带的文本, 0, str);
            }
        }
        #endregion

        #region 超时处理方法
        public void TimeOut()
        {
            //如果空闲状态
            if (CurrentState == EnumGroupStatus.空闲 || CurrentState == EnumGroupStatus.投箱报警)
                return;

            uint dt = LogInfo.Win32API.GetTickCount() - _mDtLastOperation;
            dt /= 1000;
            bool b = false;
            if (CurrentState == EnumGroupStatus.投箱开门禁 || CurrentState == EnumGroupStatus.清退开门禁)
            {
                if (dt > LogInfo.Constant.TimeOut_PutInLetter)
                {
                    b = true;
                }
            }
            else if (CurrentState == EnumGroupStatus.条码通投 || CurrentState == EnumGroupStatus.通码分发
                || CurrentState == EnumGroupStatus.通码模板列表 || CurrentState == EnumGroupStatus.通码普发
                || CurrentState == EnumGroupStatus.通码方式选择 || CurrentState == EnumGroupStatus.通码模板列表确认
                || CurrentState == EnumGroupStatus.清退通投 || CurrentState == EnumGroupStatus.清退)
            {
                if (dt > LogInfo.Constant.TimeOut_SendLetter)
                {
                    b = true;
                }
            }
            else if (CurrentState == EnumGroupStatus.用户取件开门 || CurrentState == EnumGroupStatus.用户取件开门部分取件
                || CurrentState == EnumGroupStatus.用户取件取件平台 || CurrentState == EnumGroupStatus.用户取件取件平台单位选择
                || CurrentState == EnumGroupStatus.管理维护 || CurrentState == EnumGroupStatus.管理勘误
                || CurrentState == EnumGroupStatus.维护开门 || CurrentState == EnumGroupStatus.勘误开门)
            {
                if (dt > LogInfo.Constant.TimeOut_CloseDoor)
                {
                    b = true;
                }
            }
            else if (CurrentState == EnumGroupStatus.提示信息显示)
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
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "超时：" + CurrentState.ToString());
            try
            {
                if (CurrentState == EnumGroupStatus.投箱开门禁 || CurrentState == EnumGroupStatus.清退开门禁)
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
                    //记录日志
                    string logMessage = "超时，当前状态：" + CurrentState.ToString() + "，条码：" + _mLetterBarCode + "\r\n";
                    LogInfo.Log.WriteFileLog(logMessage);

                    PlaySound(46);
                    _mJingGaoString = LogInfo.NotifyMsg.GetText(46);
                    CurrentState = EnumGroupStatus.投箱报警;
                }
                else if (CurrentState == EnumGroupStatus.条码通投 || CurrentState == EnumGroupStatus.通码分发
                    || CurrentState == EnumGroupStatus.通码模板列表 || CurrentState == EnumGroupStatus.通码普发
                    || CurrentState == EnumGroupStatus.通码方式选择 || CurrentState == EnumGroupStatus.通码模板列表确认
                    || CurrentState == EnumGroupStatus.清退通投 || CurrentState == EnumGroupStatus.清退)
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
                        if (CurrentState == EnumGroupStatus.通码分发)
                            _mJingGaoBoxNo.Add(b.BoxNo);
                    }
                    if (CurrentState == EnumGroupStatus.通码分发)
                    {
                        //记录日志
                        string logMessage = "超时，当前状态：" + CurrentState.ToString() + "，条码：" + _mLetterBarCode + "\r\n";
                        LogInfo.Log.WriteFileLog(logMessage);

                        PlaySound(46);
                        _mJingGaoString = LogInfo.NotifyMsg.GetText(46);
                        CurrentState = EnumGroupStatus.投箱报警;
                    }
                    else
                        CurrentState = EnumGroupStatus.空闲;
                }
                else if (CurrentState == EnumGroupStatus.用户取件开门 || CurrentState == EnumGroupStatus.用户取件开门部分取件
                    || CurrentState == EnumGroupStatus.用户取件取件平台 || CurrentState == EnumGroupStatus.用户取件取件平台单位选择
                    || CurrentState == EnumGroupStatus.管理维护 || CurrentState == EnumGroupStatus.管理勘误
                    || CurrentState == EnumGroupStatus.维护开门 || CurrentState == EnumGroupStatus.勘误开门)
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
                        if (CurrentState == EnumGroupStatus.用户取件开门)
                        {
                            SaveUserGetLetter(b.BoxNo, true, false, false);
                        }
                        else
                        {
                            b.Box.CheckBoxLetter();
                        }
                    }
                    CurrentState = EnumGroupStatus.空闲;
                }
                else if (CurrentState == EnumGroupStatus.提示信息显示)
                {
                    CurrentState = EnumGroupStatus.空闲;
                }

            }
            catch
            {
            }
        }
        #endregion
    }
}
