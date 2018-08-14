using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using DataBase.MonitorService;
using LogInfo;

namespace DataBase
{
    public delegate void d_SyncError(int BoxNo, bool bError);

    /// <summary>
    /// Class1 的摘要说明。
    /// </summary>
    public class DataSave
    {
        private static int MinBoxId = 9999;
        private static int MaxBoxId = 0;

        public static event d_SyncError OnSyncError;

        private static bool bRun = false;
        private static bool bConnected = true;
        private static System.Threading.Thread m_Thread;

        /// <summary>
        /// 最后一个预发标识
        /// </summary>
        private static string LastYuFaBiaoZhi = "";
        private static bool m_bInitYuFa = false;
        /// <summary>
        /// 本地过程表
        /// </summary>
        private static LocalDataSet m_DataSet = new LocalDataSet();

        private static int iLock箱格信息 = 0;
        #region 本地数据
        public static LocalDataSet LocalData
        {
            get { return (LocalDataSet)m_DataSet.Copy(); }
        }
        #endregion

        #region 创建对应的类
        private static MonitorService.MonitorService GetConnectionObj(string Operation)
        {
            MonitorService.MonitorService obj = new MonitorService.MonitorService();
            string aa = Constant.ServiceURL.Substring(Constant.ServiceURL.Length - 5);
            if (aa.IndexOf(".") >= 0)
                obj.Url = Constant.ServiceURL;
            else
                obj.Url = Constant.ServiceURL + "/" + Operation + ".rdp";
            obj.Timeout = 20 * 1000;
            return obj;
        }
        #endregion

        #region 开始初始化
        public static bool Start()
        {
            if (!Constant.UserLocalData)
                return true;
            bool bRet = false;
            try
            {
                //过程控制表非计划标识起始号
                MonitorService.MonitorService obj = GetConnectionObj("GetNewBiaoZhi");
                obj.Dispose();
                //规则策略数据同步接口
                obj = GetConnectionObj("GetGuiZeList");
                GuiZeListClass[] c = obj.GetGuiZeList(MinBoxId, MaxBoxId);
                foreach (GuiZeListClass c1 in c)
                {
                    LocalDataSet.规则策略Row row = m_DataSet.规则策略.New规则策略Row();
                    row.规则标识 = c1.GZBiaoZhi;
                    row.规则名称 = c1.Name;
                    row.规则类型 = c1.BarType;
                    row.允许直投 = c1.HasZhiTou;
                    row.允许解析 = c1.CanCheckRcvUnit;
                    row.允许普发 = c1.HasPuFa;
                    row.允许模板 = c1.HasMoban;
                    m_DataSet.规则策略.Add规则策略Row(row);
                }

                bRet = true;
                bRun = true;
                m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(TongBuThread));
                m_Thread.Start();
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
                bRun = bRet = false;
            }
            return bRet;
        }
        #endregion
        #region 停止
        public static void Stop()
        {
            if (!Constant.UserLocalData)
                return;

            try
            {
                bRun = false;
                m_Thread.Join();
            }
            catch
            {
            }
        }
        #endregion

        #region 获取箱格配置信息列表
        /// <summary>
        /// 获取箱格配置信息列表
        /// </summary>
        /// <returns></returns>
        public static BoxSetInfoClass[] GetBoxSetInfo()
        {
            BoxSetInfoClass[] strRet = null;
            if (!Constant.UserLocalBoxConfig)
            {
                try
                {
                    var url = Constant.ServiceURL + "GetBoxSetInfo";
                    Log.WriteFileLog(url);
                    var result = HttpHelper.HttpGet(url, "");
                    Log.WriteFileLog(result);
                    var obj = JsonConvert.DeserializeObject<dynamic>(result);
                    Log.WriteFileLog(obj.ToString());
                    var boxSetInfos = new List<BoxSetInfoClass>();

                    foreach (var box in obj)
                    {
                        BoxSetInfoClass boxSetInfo = new BoxSetInfoClass
                        {
                            BoxNO = box.No,
                            BackBoxBN = box.BackBn,
                            FrontBoxBN = box.FrontBn,
                            HasTwoLock = box.IsTwoLock
                        };
                        boxSetInfos.Add(boxSetInfo);
                    }

                    strRet = boxSetInfos.ToArray();

                    //strRet = obj.GetBoxSetInfo();
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, ee.ToString());
                    strRet = new BoxSetInfoClass[0];
                }
            }
            else
            {
                DataSetBox ds = DataSave_NoLocal.DataSetBox;
                List<BoxSetInfoClass> l = new List<BoxSetInfoClass>();
                foreach (DataSetBox.BoxInfoRow box in ds.BoxInfo)
                {
                    BoxSetInfoClass b = new BoxSetInfoClass();
                    b.BoxNO = box.逻辑箱号.ToString();
                    b.FrontBoxBN = box.分箱正面bn号;
                    b.BackBoxBN = box.分箱背面bn号;
                    b.HasTwoLock = box.双面锁;
                    l.Add(b);
                }
                strRet = l.ToArray();
            }

            //设置箱号
            foreach (BoxSetInfoClass b in strRet)
            {
                int boxid = Convert.ToInt32(b.BoxNO);
                if (boxid < MinBoxId || MinBoxId == 9999)
                    MinBoxId = boxid;
                if (boxid > MaxBoxId || MaxBoxId == 0)
                    MaxBoxId = boxid;
            }

            return strRet;
        }
        #endregion
        #region 获取箱组配置信息同步接口
        /// <summary>
        /// 获取箱组配置信息同步接口
        /// </summary>
        /// <returns></returns>
        public static GroupSetInfoClass[] GetGroupSetInfo()
        {
            GroupSetInfoClass[] strRet = null;
            if (!Constant.UserLocalBoxConfig)
            {
                try
                {
                    var url = Constant.ServiceURL + "GetGroupSetInfo";
                    var result = HttpHelper.HttpGet(url, "");
                    var obj = JsonConvert.DeserializeObject<dynamic>(result);
                    var groups = new List<GroupSetInfoClass>();

                    foreach (var group in obj)
                    {
                        GroupSetInfoClass groupSetInfo = new GroupSetInfoClass
                        {
                            GroupName = group.Name,
                            GroupFrontScanBN = group.FrontScanBn,
                            GroupFrontShowBN = group.FrontShowBn,
                            GroupFrontSoundBN = group.FrontSoundBn,
                            GroupFrontCardBN = group.FrontReadCardBn,
                            GroupFrontSheXiangTouBN = group.FrontCameraBn,
                            GroupFrontZhiJingMaiBN = group.FrontDigitalVein,

                            GroupBackCardBN = group.BackReadCardBn,
                            GroupBackScanBN = group.BackScanBn,
                            GroupBackSheXiangTouBN = group.BackCameraBn,
                            GroupBackShowBN = group.BackShowBn,
                            GroupBackSoundBN = group.BackSoundBn,
                            GroupBackZhiJingMaiBN = group.BackDigitalVein,
                            PrinterName = "",
                            GroupNameArray = new string[0],
                            BoxArray = group.Boxs.ToString().Split(',')
                        };

                        groups.Add(groupSetInfo);
                    }
                    strRet = groups.ToArray();
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, ee.ToString());
                    strRet = new GroupSetInfoClass[0];
                }
            }
            else
            {
                var ds = DataSave_NoLocal.DataSetBox;
                var l = new List<GroupSetInfoClass>();
                foreach (var group in ds.GroupInfo)
                {
                    var g = new GroupSetInfoClass
                    {
                        GroupName = group.箱组名称,
                        GroupFrontScanBN = group.前面扫描头bn号,
                        GroupFrontShowBN = group.前面多功能屏bn号,
                        GroupFrontSoundBN = group.前面语音BN号,
                        GroupFrontCardBN = group.前面读卡器bn号,
                        GroupFrontSheXiangTouBN = group.前面串口摄像头BN号,
                        GroupFrontZhiJingMaiBN = group.前面指纹仪BN号,
                        GroupBackCardBN = group.后面读卡器bn号,
                        GroupBackScanBN = group.后面扫描头bn号,
                        GroupBackSheXiangTouBN = group.后面串口摄像头BN号,
                        GroupBackShowBN = group.后面多功能屏bn号,
                        GroupBackSoundBN = group.后面语音BN号,
                        GroupBackZhiJingMaiBN = group.后面指纹仪BN号,
                        PrinterName = !group.IsNull("清单打印机") ? group.清单打印机 : "",
                        BoxArray = group.本组逻辑箱号列表.Split(',')
                    };

                    if (!group.Is代管的组列表Null() && group.代管的组列表 != "")
                    {
                        g.GroupNameArray = group.代管的组列表.Split(',');
                    }
                    else
                        g.GroupNameArray = new string[0];

                    l.Add(g);
                }
                strRet = l.ToArray();
            }

            return strRet;
        }
        #endregion
        #region 获取所有箱格信息接口
        /// <summary>
        /// 获取所有箱格信息接口
        /// </summary>
        /// <returns>返回箱头信息数目信息</returns>
        public static BoxInfo GetBoxLetterCount(int boxNo)
        {
            BoxInfo box = null;
            if (!Constant.UserLocalData)
            {
                box = new BoxInfo();
                var url = Constant.ServiceURL + "GetBoxLetterCount";
                var postData = "boxNo=" + boxNo;
                var result = HttpHelper.HttpGet(url, postData);
                var b = JsonConvert.DeserializeObject<dynamic>(result);
                if (b != null)
                {
                    box.BoxNO = b.BoxNo;
                    box.BoxShowName = b.BoxName;
                    box.BoxShowFullName = b.BaoKanString;
                    box.IsQingTuiXiang = false;
                    box.SendCount = b.FileCount;
                    box.HasJiJian = b.HasJiJian;
                    
                    box.BoxProperty = 0;
                }
            }
            else
            {
                lock (m_DataSet)
                {
                    lock (m_DataSet.箱格信息)
                    {
                        iLock箱格信息 = 1;
                        try
                        {
                            foreach (LocalDataSet.箱格信息Row row in m_DataSet.箱格信息)
                            {
                                if (row.逻辑箱号 == boxNo)
                                {
                                    box = new BoxInfo();
                                    box.BoxNO = boxNo;
                                    box.BoxShowName = row.简称;
                                    box.BoxShowFullName = row.全称;
                                    box.IsQingTuiXiang = row.是否清退箱;

                                    box.BoxProperty = row.箱格属性;
                                    //查找数量
                                    box.SendCount = 0;
                                    box.HasJiJian = false;

                                    //信
                                    var rs = Constant.m_IsPiaoJuXiang ? m_DataSet.预发表.Select("投箱状态=true and 取件状态=false and 是否勘误=false") : m_DataSet.预发表.Select("收件箱格号码=" + boxNo + " and 投箱状态=true and 取件状态=false and 是否勘误=false");
                                    foreach (var t in rs)
                                    {
                                        var r = (LocalDataSet.预发表Row)t;
                                        if (r.是否是急件)
                                            box.HasJiJian = true;
                                    }
                                    box.SendCount += rs.Length;
                                    //文
                                    rs = m_DataSet.范围表.Select("收件箱格号码=" + boxNo.ToString() + " and 投箱状态=true and 取件状态=false and 是否勘误=false");
                                    for (int i = 0; i < rs.Length; i++)
                                    {
                                        LocalDataSet.范围表Row r = (LocalDataSet.范围表Row)rs[i];
                                        DataRow[] rs2 = m_DataSet.预发表.Select("预发标识='" + r.预发标识 + "'");
                                        if (rs2.Length > 0)
                                        {
                                            LocalDataSet.预发表Row r2 = (LocalDataSet.预发表Row)rs2[0];
                                            if (r2.是否是急件)
                                                box.HasJiJian = true;
                                        }
                                        box.SendCount += r.投入份数;
                                    }

                                    break;
                                }
                            }
                        }
                        catch (Exception ee)
                        {
                            Log.WriteInfo(LogType.Error, ee.ToString());
                            box = null;
                        }
                        iLock箱格信息 = 0;
                    }
                }
            }
            return box;
        }
        #endregion


        #region 返回条码的类型。如果是信件条码，同时返回可以接收的箱子号码

        /// <summary>
        /// 返回条码的类型。如果是信件条码，同时返回可以接收的箱子号码
        /// </summary>
        /// <param name="barCode">输入，扫描条码</param>
        /// <param name="boxs">返回可投箱箱格</param>
        /// <returns>条码类型</returns>
        public static BarCodeType CheckBarCodeType(string barCode, out List<SendBoxList> boxs)
        {
            DateTime dtStart = DateTime.Now;
            string logMessage = "检查信件：" + barCode + "\r\n";
            Log.WriteFileLog(logMessage);
            var rType = BarCodeType.无效;
            boxs = new List<SendBoxList>();
            if (Constant.UserLocalData)
            {
                //暂时删除本地逻辑
            }
            else
            {
                var url = Constant.ServiceURL + "CheckBarCodeType";
                var postData = "barCode=" + barCode;
                var result = HttpHelper.HttpGet(url, postData);
                if (result != "")
                {
                    var obj = JsonConvert.DeserializeObject<dynamic>(result);
                    if (obj.Record != null)
                    {
                        Log.WriteFileLog("obj.Record != null");
                        foreach (var o in obj.Record)
                        {
                            var sendBoxList = new SendBoxList
                            {
                                BoxNo = o.No,
                                Count = o.FileCount,
                                msg = o.Msg
                            };
                            boxs.Add(sendBoxList);
                        }
                    }

                    rType = (BarCodeType)obj.Type;
                    Log.WriteFileLog("rType= " + rType);
                }
            }
            logMessage += "返回检查信件结果：" + rType + "\r\n";
            var ts = DateTime.Now - dtStart;
            logMessage = "耗时 " + ts + " 。\r\n" + logMessage + "\r\n\r\n";
            Log.WriteFileLog(logMessage);

            return rType;
        }
        #endregion
        #region 检查证卡条码

        /// <summary>
        /// 检查证卡条码
        /// </summary>
        /// <param name="bn"></param>
        /// <param name="userCode"></param>
        /// <param name="boxs"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static BarCodeType CheckCardType(string bn, string userCode, out List<UserGetBoxInfo> boxs, out string userName)
        {
            var rType = BarCodeType.无效;
            boxs = new List<UserGetBoxInfo>();
            var logMessage = "检查证卡条码：" + userCode + "\r\n";
            var dtStart = DateTime.Now;
            userName = "";
            if (Constant.UserLocalData)
            {
                #region 本地数据
                try
                {
                    lock (m_DataSet.证卡列表)
                    {

                        foreach (LocalDataSet.证卡列表Row row in m_DataSet.证卡列表)
                        {
                            if (row.验证类型 == 1 && row.证卡号码 == userCode)
                            {
                                UserGetBoxInfo t = new UserGetBoxInfo
                                {
                                    BoxNo = row.箱格号码,
                                    证卡编号 = row.证卡编号.ToString(),
                                    单位名称 = row.单位名称,
                                    用户名称 = row.用户名称,
                                    CardPrintType = (em_CardPrintType)row.用户可以打印的清单的类型
                                };

                                if (userName == "")
                                    userName = row.用户名称;

                                boxs.Add(t);
                                if (row.证卡类型 == 1)
                                    rType = BarCodeType.管理员;
                                else
                                    rType = BarCodeType.交换员;
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, ee.ToString());
                    logMessage += "异常，内容：" + ee.ToString() + "\r\n";
                }
                #endregion
            }
            else
            {
                var url = Constant.ServiceURL + "CheckCardType";
                var postData = "cardValue=" + userCode + "&bn=" + bn;
                var result = HttpHelper.HttpGet(url, postData);
                if (result != "")
                {
                    var obj = JsonConvert.DeserializeObject<dynamic>(result);
                    rType = (BarCodeType)obj.Type;
                    string unitName = obj.DepartmentName;
                    userName = obj.UserName;
                    foreach (var boxNo in obj.Boxs)
                    {
                        var box = new UserGetBoxInfo
                        {
                            BoxNo = boxNo,
                            单位名称 = unitName,
                            用户名称 = userName,
                            证卡编号 = userCode
                        };
                        boxs.Add(box);
                    }
                }
            }

            logMessage += "返回检查证卡条码结果：" + rType + "\r\n";
            foreach (UserGetBoxInfo t in boxs)
            {
                logMessage += "证卡编号：" + t.证卡编号 + ", 箱格号码：" + t.BoxNo + "\r\n";
            }
            TimeSpan ts = DateTime.Now - dtStart;
            logMessage = "耗时 " + ts + " 。\r\n" + logMessage + "\r\n\r\n";
            Log.WriteFileLog(logMessage);

            return rType;
        }
        #endregion
        #region 检查指静脉
        /// <summary>
        /// 检查指静脉
        /// </summary>
        /// <param name="info">指静脉数据Base64字符串</param>
        /// <param name="iType">0：老的指静脉，1：燕南指静脉</param>
        /// <returns></returns>
        public static BarCodeType CheckZhiJingMaiType(string info, int iType, out List<UserGetBoxInfo> boxs, out string UserName)
        {
            BarCodeType rType = BarCodeType.无效;
            boxs = new List<UserGetBoxInfo>();
            string LogMessage = "检查指静脉：" + info + "，类型：" + iType.ToString() + "\r\n";
            DateTime dtStart = DateTime.Now;
            UserName = "";
            try
            {
                List<string> sEnrollData = new List<string>();
                lock (m_DataSet.证卡列表)
                {
                    foreach (LocalDataSet.证卡列表Row row in m_DataSet.证卡列表)
                    {
                        if (row.验证类型 == 2)
                        {
                            if (!sEnrollData.Contains(row.证卡号码))
                            {
                                sEnrollData.Add(row.证卡号码);
                            }
                        }
                    }
                }
                string sRet = "";
                if (iType == 0)
                    sRet = XGComApi.CompareData(sEnrollData, info);
                else
                    sRet = YNZhiJingMai.CompareData(sEnrollData, info);
                if (sRet != "")
                {
                    lock (m_DataSet.证卡列表)
                    {
                        foreach (LocalDataSet.证卡列表Row row in m_DataSet.证卡列表)
                        {
                            if (row.验证类型 == 2 && row.证卡号码.CompareTo(sRet) == 0)
                            {
                                UserGetBoxInfo t = new UserGetBoxInfo();
                                t.BoxNo = row.箱格号码;
                                t.证卡编号 = row.证卡编号.ToString();
                                t.单位名称 = row.单位名称;
                                t.用户名称 = row.用户名称;
                                t.CardPrintType = (em_CardPrintType)row.用户可以打印的清单的类型;

                                if (UserName == "")
                                    UserName = row.用户名称;

                                boxs.Add(t);
                                if (row.证卡类型 == 1)
                                    rType = BarCodeType.管理员;
                                else
                                    rType = BarCodeType.交换员;
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
                LogMessage += "异常，内容：" + ee.ToString() + "\r\n";
            }

            LogMessage += "返回检查指静脉结果：" + rType.ToString() + "\r\n";
            foreach (UserGetBoxInfo t in boxs)
            {
                LogMessage += "证卡编号：" + t.证卡编号.ToString() + ", 箱格号码：" + t.BoxNo.ToString() + "\r\n";
            }
            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "耗时 " + ts.ToString() + " 。\r\n" + LogMessage + "\r\n\r\n";
            Log.WriteFileLog(LogMessage);

            return rType;
        }

        public static BarCodeType CheckZhiJingMaiLocationType(string LocationId, out List<UserGetBoxInfo> boxs, out string UserName)
        {
            BarCodeType rType = BarCodeType.无效;
            boxs = new List<UserGetBoxInfo>();
            string LogMessage = "检查指静脉位置：" + LocationId + "\r\n";
            DateTime dtStart = DateTime.Now;
            UserName = "";
            try
            {
                List<string> sEnrollData = new List<string>();
                lock (m_DataSet.证卡列表)
                {
                    foreach (LocalDataSet.证卡列表Row row in m_DataSet.证卡列表)
                    {
                        if (row.验证类型 == 2 && row.证卡号码 == LocationId)
                        {
                            UserGetBoxInfo t = new UserGetBoxInfo();
                            t.BoxNo = row.箱格号码;
                            t.证卡编号 = row.证卡编号.ToString();
                            t.单位名称 = row.单位名称;
                            t.用户名称 = row.用户名称;
                            t.CardPrintType = (em_CardPrintType)row.用户可以打印的清单的类型;

                            if (UserName == "")
                                UserName = row.用户名称;

                            boxs.Add(t);
                            if (row.证卡类型 == 1)
                                rType = BarCodeType.管理员;
                            else
                                rType = BarCodeType.交换员;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
                LogMessage += "异常，内容：" + ee.ToString() + "\r\n";
            }

            LogMessage += "返回检查指静脉结果：" + rType.ToString() + "\r\n";
            foreach (UserGetBoxInfo t in boxs)
            {
                LogMessage += "证卡编号：" + t.证卡编号.ToString() + ", 箱格号码：" + t.BoxNo.ToString() + "\r\n";
            }
            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "耗时 " + ts.ToString() + " 。\r\n" + LogMessage + "\r\n\r\n";
            Log.WriteFileLog(LogMessage);

            return rType;
        }
        #endregion

        #region 请求对应模板列表
        /// <summary>
        /// 请求对应模板列表
        /// </summary>
        /// <returns></returns>
        public static ClassBarcodeMuBanList[] GetBarcodeMuBanList(string FullBarcode)
        {
            ClassBarcodeMuBanList[] strRet = null;
            try
            {
                MonitorService.MonitorService obj = GetConnectionObj("GetBarcodeMuBanList");

                strRet = obj.GetBarcodeMuBanList(FullBarcode);
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
                strRet = new ClassBarcodeMuBanList[0];
            }

            return strRet;
        }
        #endregion
        #region 请求模板详细信息
        /// <summary>
        /// 请求模板详细信息
        /// </summary>
        /// <returns></returns>
        public static bool GetMuBanInfo(string BarCode, string 模板标识)
        {
            bool bRet = false;
            try
            {
                MonitorService.MonitorService obj = GetConnectionObj("GetMuBanInfo");

                YuFaClass yf = obj.GetMuBanInfo(模板标识);
                lock (m_DataSet)
                {
                    LocalDataSet.预发表Row row = null;
                    string bz = yf.YFBizoZhi;
                    if (bz == null || bz == "")
                    {
                        bz = "WW";
                    }
                    if (bz.Substring(0, 1) != "Y" && bz.Substring(0, 1) != "W")
                        bz = "Y" + bz;
                    //先找到当前有没有
                    DataRow[] rows = m_DataSet.预发表.Select("预发标识='" + bz + "'");
                    if (rows.Length > 0)
                        row = (LocalDataSet.预发表Row)rows[0];
                    else
                        row = m_DataSet.预发表.New预发表Row();

                    row.是否同步 = false;
                    row.预发标识 = bz;
                    row.条码编号 = GuiZeClass.GetOneBarcode(BarCode);
                    row.完整条码 = BarCode;
                    row.条码类型 = yf.Bartype;
                    row.是否是急件 = yf.IsJiJian;
                    row.是否删除 = false;
                    if (rows.Length <= 0)
                        m_DataSet.预发表.Add预发表Row(row);
                    if (yf.Bartype == 1)
                    {
                        row.收件箱格号码 = yf.ReceiveBoxNO;
                        if (yf.Showmsg != null) row.份号 = yf.Showmsg;
                    }
                    else
                    {
                        for (int j = 0; j < yf.FanWei.Length; j++)
                        {
                            LocalDataSet.范围表Row r = null;
                            //先找到当前有没有
                            DataRow[] rs = m_DataSet.范围表.Select("预发标识='" + bz + "' and 收件箱格号码=" + yf.FanWei[j].ReceiveBoxNO);
                            if (rs.Length > 0)
                                r = (LocalDataSet.范围表Row)rs[0];
                            else
                                r = m_DataSet.范围表.New范围表Row();


                            r.是否同步 = false;
                            r.预发标识 = bz;
                            r.收件箱格号码 = yf.FanWei[j].ReceiveBoxNO;
                            if (yf.FanWei[j].Showmsg != null) r.份号 = yf.FanWei[j].Showmsg;
                            r.分发份数 = yf.FanWei[j].Count;
                            r.是否删除 = false;

                            if (rs.Length <= 0)
                                m_DataSet.范围表.Add范围表Row(r);
                        }//for
                    }
                }//lock
                bRet = true;
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
                bRet = false;
            }

            return bRet;
        }
        #endregion

        #region 保存信件

        /// <summary>
        /// 保存信件
        /// </summary>
        /// <param name="LetterCode"></param>
        /// <param name="BoxNO"></param>
        /// <param name="bUragent"></param>
        /// <returns>投入信件的急件属性</returns>
        public static int SaveLetter(string LetterCode, int BoxNO, bool bUragent, int SendCount, string SendCardNO, string RecFileName, bool 是否退信, string 退信的管理员的证卡编号)
        {
            DateTime dtStart = DateTime.Now; 
            string LogMessage = "保存信件：" + LetterCode + "，箱号：" + BoxNO + "\r\n";

            int iRet = -1; //error

            if (Constant.UserLocalData)
            {
                #region 本地数据
                try
                {
                    System.Threading.Monitor.Enter(m_DataSet);
                    DataRow[] rs = new DataRow[0];
                    if (SendCardNO == "")
                    {
                        int barID = GuiZeClass.GetBarCodeType(LetterCode);
                        int barType = 1;//规则类型。1：唯一码，2：通码
                        bool bHas = false; bool b1 = false, b2 = false, b3 = false, b4 = false, b5 = false;
                        lock (m_DataSet.规则策略)
                        {
                            foreach (LocalDataSet.规则策略Row r1 in m_DataSet.规则策略)
                            {
                                if (r1.规则标识 == barID)
                                {
                                    bHas = true; barType = r1.规则类型;
                                    b1 = r1.允许直投; b2 = r1.允许解析; b3 = r1.允许普发; b4 = r1.允许模板;
                                    b5 = r1.允许重复投箱;
                                }
                            }
                        }

                        lock (m_DataSet)
                        {
                            rs = m_DataSet.预发表.Select("条码编号='" + GuiZeClass.GetOneBarcode(LetterCode) + "' or 条码编号56='" + GuiZeClass.GetOneBarcode(LetterCode) + "'");
                            if (rs.Length > 0)
                            {
                                #region 预发表
                                LocalDataSet.预发表Row row = (LocalDataSet.预发表Row)rs[0];
                                if (row.条码类型 == 1)
                                {
                                    for (int j = 0; j < rs.Length; j++)
                                    {
                                        row = (LocalDataSet.预发表Row)rs[j];
                                        if (row.条码类型 == 1 && row.收件箱格号码 == BoxNO && row.投箱状态 == false)
                                        {
                                            row.是否同步 = false;
                                            row.投箱状态 = true;
                                            row.投箱时间 = DateTime.Now;
                                            row.手工加急 = bUragent;

                                            if (是否退信)
                                            {
                                                row.是否退信 = true;
                                                row.退信管理员证卡编号 = 退信的管理员的证卡编号;
                                            }
                                            else
                                                row.是否退信 = false;

                                            row.录像文件名称 = RecFileName;
                                            iRet = 0;
                                            if (bUragent || row.是否是急件)
                                                iRet = 1;
                                        }
                                    }
                                }
                                else if (row.条码类型 == 2)
                                {
                                    bHas = false;
                                    for (int j = 0; j < rs.Length; j++)
                                    {
                                        row = (LocalDataSet.预发表Row)rs[j];
                                        if (row.条码类型 == 2)
                                        {
                                            DataRow[] rs2 = m_DataSet.范围表.Select("预发标识='" + row.预发标识 + "' and 收件箱格号码=" + BoxNO.ToString());
                                            for (int i = 0; i < rs2.Length; i++)
                                            {
                                                LocalDataSet.范围表Row row2 = (LocalDataSet.范围表Row)rs2[i];
                                                if (row2.收件箱格号码 == BoxNO)
                                                {
                                                    row.是否同步 = false;
                                                    row.录像文件名称 = RecFileName;
                                                    row2.是否同步 = false;
                                                    row2.投箱状态 = true;
                                                    row2.投箱时间 = DateTime.Now;
                                                    row2.投入份数 += SendCount;
                                                    bHas = true;
                                                }
                                            }
                                            if (bHas)
                                            {
                                                iRet = 0;
                                                if (row.是否是急件)
                                                    iRet = 1;
                                            }
                                        }
                                    }//for
                                    if (!bHas)
                                    {
                                        row = (LocalDataSet.预发表Row)rs[0];
                                        //普发
                                        LocalDataSet.范围表Row row2 = m_DataSet.范围表.New范围表Row();
                                        row2.预发标识 = row.预发标识;
                                        row2.收件箱格号码 = BoxNO;
                                        row2.分发份数 = SendCount;
                                        row2.投箱状态 = true;
                                        row2.投箱时间 = DateTime.Now;
                                        row2.投入份数 = SendCount;
                                        m_DataSet.范围表.Add范围表Row(row2);

                                        row.是否同步 = false;
                                        row.录像文件名称 = RecFileName;

                                        iRet = 0;
                                        if (row.是否是急件)
                                            iRet = 1;
                                    }
                                }
                                #endregion
                            }//if
                        }
                    }//不是清退
                     //没有预发的，为直投的或者普发的
                    if (iRet == -1)
                    {
                        iRet = 0;
                        //条码类型
                        int barID = GuiZeClass.GetBarCodeType(LetterCode);
                        int barType = 1;//规则类型。1：唯一码，2：通码
                        if (SendCardNO == "")
                        {
                            lock (m_DataSet.规则策略)
                            {
                                foreach (LocalDataSet.规则策略Row r1 in m_DataSet.规则策略)
                                {
                                    if (r1.规则标识 == barID)
                                    {
                                        barType = r1.规则类型; break;
                                    }
                                }
                            }
                        }
                        bool bTiaoMaJiJian = GuiZeClass.CheckBarcodeIsJiJian(LetterCode);

                        lock (m_DataSet)
                        {
                            bUragent = bUragent || bTiaoMaJiJian;
                            LocalDataSet.预发表Row row = m_DataSet.预发表.New预发表Row();
                            row.预发标识 = "WW";
                            m_DataSet.预发表.Add预发表Row(row);
                            row.条码编号 = GuiZeClass.GetOneBarcode(LetterCode);
                            row.完整条码 = LetterCode;
                            row.是否是急件 = bUragent;
                            row.录像文件名称 = RecFileName;
                            row.条码类型 = barType;
                            row.是否同步 = false;
                            row.预发时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            row.DocId = "0";

                            if (row.条码类型 == 1)
                            {
                                row.收件箱格号码 = BoxNO;
                                row.投箱状态 = true;
                                row.投箱时间 = DateTime.Now;
                                row.手工加急 = bUragent;
                                row.是否是急件 = bUragent;
                                row.清退人 = SendCardNO;
                                if (bUragent)
                                    iRet = 1;
                                if (是否退信)
                                {
                                    row.是否退信 = true;
                                    row.退信管理员证卡编号 = 退信的管理员的证卡编号;
                                }
                                else
                                    row.是否退信 = false;
                            }
                            else if (row.条码类型 == 2)
                            {
                                //普发
                                LocalDataSet.范围表Row row2 = m_DataSet.范围表.New范围表Row();
                                row2.预发标识 = row.预发标识;
                                row2.收件箱格号码 = BoxNO;
                                row2.分发份数 = SendCount;
                                row2.投箱状态 = true;
                                row2.投箱时间 = DateTime.Now;
                                row2.投入份数 = SendCount;
                                row2.份号 = "";
                                m_DataSet.范围表.Add范围表Row(row2);
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, ee.ToString());
                    LogMessage += "异常，内容：" + ee.ToString() + "\r\n";
                }
                finally
                {
                    System.Threading.Monitor.Exit(m_DataSet);
                }
                #endregion

                System.Threading.ThreadPool.QueueUserWorkItem(SyncSendYuFa, 0);
            }
            else
            {
                var url = Constant.ServiceURL + "SaveLetter";

                var postData = "barCode=" + LetterCode + "&no=" + BoxNO + "&fileCount=" + SendCount + "&isJiaJi=" +
                               bUragent;
                var result = HttpHelper.HttpPost(url, postData);

                var obj = JsonConvert.DeserializeObject<dynamic>(result);
                iRet = (int)obj;
            }

            LogMessage += "返回保存信件结果：" + iRet + "\r\n";
            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "耗时 " + ts + " 。\r\n" + LogMessage + "\r\n\r\n";
            Log.WriteFileLog(LogMessage);

            return iRet;
        }
        #endregion
        #region 保存勘误信息
        /// <summary>
        /// 保存勘误信息
        /// </summary>
        /// <param name="BoxNO"></param>
        /// <param name="LetterCode"></param>
        /// <param name="AdminBarCode"></param>
        /// <param name="UserBarCode"></param>
        /// <returns>勘误条码的sumcopy属性</returns>
        public static bool SaveErratumLetter(int BoxNO, string LetterCode, string AdminBarCode, string UserBarCode)
        {
            bool bRet = false;
            if (Constant.UserLocalData)
            {
                #region 本地数据
                try
                {
                    System.Threading.Monitor.Enter(m_DataSet);
                    DataRow[] rs = m_DataSet.预发表.Select("条码编号='" + GuiZeClass.GetOneBarcode(LetterCode) + "'");
                    for (int j = 0; j < rs.Length; j++)
                    {
                        LocalDataSet.预发表Row row = (LocalDataSet.预发表Row)rs[j];
                        if (row.条码类型 == 1 && row.收件箱格号码 == BoxNO)
                        {
                            row.是否同步 = false;
                            row.是否勘误 = true;
                            row.勘误时间 = DateTime.Now;
                            row.勘误管理员卡号 = AdminBarCode;
                            row.勘误交换员卡号 = UserBarCode;
                        }
                        else if (row.条码类型 == 2)
                        {
                            DataRow[] rs2 = m_DataSet.范围表.Select("预发标识='" + row.预发标识 + "' and 收件箱格号码=" + BoxNO.ToString());
                            for (int i = 0; i < rs2.Length; i++)
                            {
                                LocalDataSet.范围表Row row2 = (LocalDataSet.范围表Row)rs2[i];
                                if (row2.收件箱格号码 == BoxNO)
                                {
                                    row.是否同步 = false;
                                    row2.是否同步 = false;
                                    row2.是否勘误 = true;
                                    row2.勘误时间 = DateTime.Now;
                                    row2.勘误管理员卡号 = AdminBarCode;
                                    row2.勘误交换员卡号 = UserBarCode;
                                }
                            }
                        }
                    }
                    bRet = true;
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, ee.ToString());
                }
                finally
                {
                    System.Threading.Monitor.Exit(m_DataSet);
                }
                #endregion
            }
            else
            {
                bRet = DataSave_NoLocal.SaveErratumLetter(BoxNO, LetterCode, AdminBarCode, UserBarCode);
            }
            return bRet;
        }
        #endregion
        #region 交换员箱组扫卡取件
        /// <summary>
        /// 交换员箱组扫卡取件
        /// </summary>
        /// <param name="BoxNO">逻辑箱号</param>
        /// <param name="UserBarCode"></param>
        /// <returns></returns>
        public static bool Box_UserGetLetter(int BoxNO, string UserBarCode, string PicData, bool bQuJian, bool bSend, bool bRecv, bool 图像是否处理, string 清单打印机, string 取件柜箱组名称)
        {
            bool bRet = false;
            if (Constant.UserLocalData)
            {
                #region 本地数据
                try
                {
                    if (bQuJian)
                    {
                        lock (m_DataSet)
                        {
                            foreach (LocalDataSet.预发表Row row in m_DataSet.预发表)
                            {
                                if (row.条码类型 == 1 && row.收件箱格号码 == BoxNO && row.投箱状态 == true)
                                {
                                    row.是否同步 = false;
                                    row.取件状态 = true;
                                    row.取件时间 = DateTime.Now;
                                    row.取件人证卡号码 = UserBarCode;
                                }
                                else if (row.条码类型 == 2)
                                {
                                    DataRow[] rs2 = m_DataSet.范围表.Select("投箱状态=true and 预发标识='" + row.预发标识 + "' and 收件箱格号码=" + BoxNO.ToString());
                                    for (int i = 0; i < rs2.Length; i++)
                                    {
                                        LocalDataSet.范围表Row row2 = (LocalDataSet.范围表Row)rs2[i];
                                        if (row2.收件箱格号码 == BoxNO)
                                        {
                                            row.是否同步 = false;
                                            row2.是否同步 = false;
                                            row2.取件状态 = true;
                                            row2.取件时间 = DateTime.Now;
                                            row2.取件人证卡号码 = UserBarCode;
                                        }
                                    }
                                }
                            }
                        }//lock

                    }

                    lock (m_DataSet.用户取件表)
                    {
                        LocalDataSet.用户取件表Row rq = m_DataSet.用户取件表.New用户取件表Row();
                        rq.逻辑箱号 = BoxNO;
                        rq.取件时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        rq.是否同步 = false;
                        rq.同步次数 = 0;
                        rq.用户卡号 = UserBarCode;
                        rq.照片内容 = PicData;
                        rq.是否打印发件清单 = bSend;
                        rq.是否打印取件清单 = bRecv;
                        rq.图片数据是否已经处理 = 图像是否处理;
                        rq.取件记录ID = 0;
                        rq.清单打印机 = 清单打印机;
                        rq.取件柜箱组名称 = 取件柜箱组名称;
                        m_DataSet.用户取件表.Add用户取件表Row(rq);
                    }

                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(SyncUserGetLetter), 0);

                    bRet = true;
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, ee.ToString());
                }
                #endregion
            }
            else
            {
                var url = Constant.ServiceURL + "Box_UserGetLetter";

                var postData = "cardValue=" + UserBarCode + "&no=" + BoxNO;
                var result = HttpHelper.HttpPost(url, postData);

                var obj = JsonConvert.DeserializeObject<dynamic>(result);
                bRet = (bool)obj;
                //bRet = DataSave_NoLocal.Box_UserGetLetter(BoxNO, UserBarCode, bSend, bRecv);
            }
            return bRet;
        }
        public static bool Box_UserGetLetter_SetPic(int BoxNO, string UserBarCode, string PicData)
        {
            #region 本地数据
            try
            {
                lock (m_DataSet.用户取件表)
                {
                    DataRow[] rows = m_DataSet.用户取件表.Select("逻辑箱号=" + BoxNO.ToString() + " and 用户卡号='" + UserBarCode + "' and 图片数据是否已经处理=false");
                    if (rows.Length > 0)
                    {
                        rows[0]["照片内容"] = PicData;
                        rows[0]["图片数据是否已经处理"] = true;
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(SyncUserGetLetter), 0);
                    }
                }
                return true;
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
            }
            #endregion
            return false;
        }

        public static bool Box_UserGetOneLetter(int BoxNO, string UserBarCode, string LetterCode)
        {
            bool bRet = false;
            try
            {
                System.Threading.Monitor.Enter(m_DataSet);
                foreach (LocalDataSet.预发表Row row in m_DataSet.预发表)
                {
                    if (row.完整条码 != LetterCode) continue;
                    if (row.条码类型 == 1 && row.收件箱格号码 == BoxNO)
                    {
                        row.是否同步 = false;
                        row.取件状态 = true;
                        row.取件时间 = DateTime.Now;
                        row.取件人证卡号码 = UserBarCode;
                    }
                    else if (row.条码类型 == 2)
                    {
                        DataRow[] rs2 = m_DataSet.范围表.Select("预发标识='" + row.预发标识 + "' and 收件箱格号码=" + BoxNO.ToString());
                        for (int i = 0; i < rs2.Length; i++)
                        {
                            LocalDataSet.范围表Row row2 = (LocalDataSet.范围表Row)rs2[i];
                            if (row2.收件箱格号码 == BoxNO)
                            {
                                row.是否同步 = false;
                                row2.是否同步 = false;
                                row2.取件状态 = true;
                                row2.取件时间 = DateTime.Now;
                                row2.取件人证卡号码 = UserBarCode;
                            }
                        }
                    }
                }
                bRet = true;
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
            }
            finally
            {
                System.Threading.Monitor.Exit(m_DataSet);
            }

            return bRet;
        }
        #endregion

        #region 提示信息同步接口
        /// <summary>
        /// 提示信息同步接口
        /// </summary>
        /// <returns></returns>
        public static ClassBoxShowMessage GetBoxShowMessage(int 逻辑箱号)
        {
            ClassBoxShowMessage strRet = new ClassBoxShowMessage();
            strRet.逻辑箱号 = 逻辑箱号;
            strRet.出差信息 = "";
            try
            {
                DataRow[] rs = null;
                lock (m_DataSet.提示信息)
                {
                    rs = m_DataSet.提示信息.Select("逻辑箱号=" + 逻辑箱号.ToString());
                    if (rs != null)
                    {
                        for (int i = 0; i < rs.Length; i++)
                        {
                            if (strRet.出差信息.CompareTo("") == 0)
                                strRet.出差信息 = rs[i][1].ToString().Trim();
                            else
                            {
                                if (逻辑箱号 != 0)
                                    strRet.出差信息 += rs[i][1].ToString().Trim();
                                else
                                    strRet.出差信息 += "\r\n" + rs[i][1].ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
                strRet.出差信息 = "";
            }

            return strRet;
        }
        #endregion

        #region 设置箱格状态
        private static Dictionary<int, BoxStatus> m_BoxStatus = new Dictionary<int, BoxStatus>();
        public static bool SetBoxStatus(BoxStatus box)
        {
            bool bRet = false;
            try
            {
                lock (m_BoxStatus)
                {
                    if (m_BoxStatus.ContainsKey(box.BoxNo))
                    {
                        m_BoxStatus.Remove(box.BoxNo);
                    }
                    m_BoxStatus.Add(box.BoxNo, box);
                }
                bRet = true;
            }
            catch
            {
            }
            return bRet;
        }
        #endregion

        #region 同步
        private static void TongBuThread()
        {
            int iCount = 100;
            while (bRun)
            {
                if (iCount < 50 || (MinBoxId == 9999 && MaxBoxId == 0))
                {
                    System.Threading.Thread.Sleep(100);
                    iCount++;
                    continue;
                }
                iCount = 0;

                #region 同步规则策略，判断网络状态，如果网络出错，则不再同步，同时，提示箱头网络断开
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("GetGuiZeList");

                    GuiZeListClass[] guize = obj.GetGuiZeList(MinBoxId, MaxBoxId);
                    lock (m_DataSet.规则策略)
                    {
                        m_DataSet.规则策略.Rows.Clear();
                        for (int i = 0; i < guize.Length; i++)
                        {
                            LocalDataSet.规则策略Row row = m_DataSet.规则策略.New规则策略Row();
                            row.规则标识 = guize[i].GZBiaoZhi;
                            row.规则名称 = guize[i].Name;
                            row.规则类型 = guize[i].BarType;
                            row.允许直投 = guize[i].HasZhiTou;
                            row.允许解析 = guize[i].CanCheckRcvUnit;
                            row.允许普发 = guize[i].HasPuFa;
                            row.允许模板 = guize[i].HasMoban;
                            row.允许重复投箱 = guize[i].CanRepeat;
                            m_DataSet.规则策略.Add规则策略Row(row);
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, "同步规则策略：" + ee.ToString());
                    Log.WriteFileLog("同步规则策略：" + ee.ToString());
                    if (bConnected)
                    {
                        bConnected = false;
                        //提示所有箱头网络断开
                        OnSyncError(0, true);
                    }
                    continue;
                }
                #endregion
                if (!bConnected)
                {
                    bConnected = true;
                    OnSyncError(0, false);
                }

                SyncSendYuFa(0);

                #region 同步预发
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("GetYuFaInfo");
                    YuFaClass[] Yufa = null;
                    if (!m_bInitYuFa)
                    {
                        //初始化
                        Yufa = obj.GetYuFaInfo("", MinBoxId, MaxBoxId);

                        //同步成功后，设置
                        m_bInitYuFa = true;
                    }
                    else
                    {
                        //初始化后
                        Yufa = obj.GetYuFaInfo("1", MinBoxId, MaxBoxId);//LastYuFaBiaoZhi
                    }
                    if (Yufa != null && Yufa.Length > 0)
                    {
                        lock (m_DataSet)
                        {
                            LastYuFaBiaoZhi = Yufa[Yufa.Length - 1].YFBizoZhi;
                            for (int i = 0; i < Yufa.Length; i++)
                            {
                                LocalDataSet.预发表Row row = null;
                                string bz = Yufa[i].YFBizoZhi;
                                if (bz.Substring(0, 1) != "Y" && bz.Substring(0, 1) != "W")
                                    bz = "Y" + bz;
                                //先找到当前有没有
                                DataRow[] rows = m_DataSet.预发表.Select("预发标识='" + bz + "'");
                                if (rows.Length > 0)
                                    row = (LocalDataSet.预发表Row)rows[0];
                                else
                                    row = m_DataSet.预发表.New预发表Row();

                                row.是否同步 = false;
                                row.预发标识 = bz;
                                row.条码编号 = Yufa[i].Barcode;
                                if (!String.IsNullOrEmpty(Yufa[i].Barcode56))
                                    row.条码编号56 = Yufa[i].Barcode56;
                                else
                                    row.条码编号56 = "";
                                row.完整条码 = Yufa[i].FullBarcode;
                                row.条码类型 = Yufa[i].Bartype;
                                row.是否是急件 = Yufa[i].IsJiJian;
                                if (string.IsNullOrEmpty(Yufa[i].YuFaTime))
                                    row.预发时间 = "";
                                else
                                    row.预发时间 = Yufa[i].YuFaTime;
                                if (string.IsNullOrEmpty(Yufa[i].DocID))
                                    row.DocId = "";
                                else
                                    row.DocId = Yufa[i].DocID;
                                row.是否删除 = Yufa[i].isDelete;

                                row.投箱类型 = Yufa[i].YuFaType;
                                row.是否退信 = Yufa[i].isTuiXin;
                                if (string.IsNullOrEmpty(Yufa[i].TuiXinUserId))
                                    row.退信管理员证卡编号 = "";
                                else
                                    row.退信管理员证卡编号 = Yufa[i].TuiXinUserId;
                                row.分组投箱方式 = Yufa[i].FenZuTouXiangFangShi;

                                if (Yufa[i].Showmsg != null) row.份号 = Yufa[i].Showmsg;

                                if (rows.Length <= 0)
                                    m_DataSet.预发表.Add预发表Row(row);
                                if (Yufa[i].Bartype == 1)
                                {
                                    row.收件箱格号码 = Yufa[i].ReceiveBoxNO;
                                    if (Yufa[i].IsTouxiang)
                                    {
                                        row.投箱状态 = Yufa[i].IsTouxiang;
                                        row.投箱时间 = Convert.ToDateTime(Yufa[i].SendTime);
                                        row.手工加急 = Yufa[i].IsJiaJi;
                                        if (string.IsNullOrEmpty(Yufa[i].SendUserCardNO))
                                            row.清退人 = "";
                                        else
                                            row.清退人 = Yufa[i].SendUserCardNO;
                                        if (string.IsNullOrEmpty(Yufa[i].LuXiangFileName))
                                            row.录像文件名称 = "";
                                        else
                                            row.录像文件名称 = Yufa[i].LuXiangFileName;
                                    }
                                    if (Yufa[i].IsKanWu)
                                    {
                                        row.是否勘误 = Yufa[i].IsKanWu;
                                        row.勘误时间 = Convert.ToDateTime(Yufa[i].KanwuTime);
                                        if (string.IsNullOrEmpty(Yufa[i].KwAdminCard))
                                            row.勘误管理员卡号 = "";
                                        else
                                            row.勘误管理员卡号 = Yufa[i].KwAdminCard;
                                        if (string.IsNullOrEmpty(Yufa[i].KwUserCard))
                                            row.勘误交换员卡号 = "";
                                        else
                                            row.勘误交换员卡号 = Yufa[i].KwUserCard;
                                    }
                                    if (Yufa[i].IsQuJian)
                                    {
                                        row.取件状态 = Yufa[i].IsQuJian;
                                        row.取件时间 = Convert.ToDateTime(Yufa[i].ReceiveTime);
                                        if (string.IsNullOrEmpty(Yufa[i].ReceiveUserCard))
                                            row.取件人证卡号码 = "";
                                        else
                                            row.取件人证卡号码 = Yufa[i].ReceiveUserCard;
                                    }
                                }
                                else
                                {
                                    //有范围的通码
                                    DataRow[] rs;
                                    for (int j = 0; j < Yufa[i].FanWei.Length; j++)
                                    {
                                        LocalDataSet.范围表Row r = null;
                                        //先找到当前有没有
                                        rs = m_DataSet.范围表.Select("预发标识='" + bz + "' and 收件箱格号码=" + Yufa[i].FanWei[j].ReceiveBoxNO);
                                        if (rs.Length > 0)
                                            r = (LocalDataSet.范围表Row)rs[0];
                                        else
                                            r = m_DataSet.范围表.New范围表Row();

                                        r.是否同步 = false;
                                        r.预发标识 = bz;
                                        r.收件箱格号码 = Yufa[i].FanWei[j].ReceiveBoxNO;
                                        if (Yufa[i].FanWei[j].Showmsg != null) r.份号 = Yufa[i].FanWei[j].Showmsg;
                                        r.分发份数 = Yufa[i].FanWei[j].Count;
                                        r.是否删除 = Yufa[i].FanWei[j].isDelete;
                                        if (Yufa[i].FanWei[j].IsTouxiang)
                                        {
                                            r.投箱状态 = Yufa[i].FanWei[j].IsTouxiang;
                                            r.投箱时间 = Convert.ToDateTime(Yufa[i].FanWei[j].SendTime);
                                            r.投入份数 = Yufa[i].FanWei[j].SendCount;
                                        }
                                        if (Yufa[i].FanWei[j].IsKanWu)
                                        {
                                            r.是否勘误 = Yufa[i].FanWei[j].IsKanWu;
                                            r.勘误时间 = Convert.ToDateTime(Yufa[i].FanWei[j].KanwuTime);
                                            if (string.IsNullOrEmpty(Yufa[i].FanWei[j].KwAdminCard))
                                                r.勘误管理员卡号 = "";
                                            else
                                                r.勘误管理员卡号 = Yufa[i].FanWei[j].KwAdminCard;
                                            if (string.IsNullOrEmpty(Yufa[i].FanWei[j].KwUserCard))
                                                r.勘误交换员卡号 = "";
                                            else
                                                r.勘误交换员卡号 = Yufa[i].FanWei[j].KwUserCard;
                                        }
                                        if (Yufa[i].FanWei[j].IsQuJian)
                                        {
                                            r.取件状态 = Yufa[i].FanWei[j].IsQuJian;
                                            r.取件时间 = Convert.ToDateTime(Yufa[i].FanWei[j].ReceiveTime);
                                            if (string.IsNullOrEmpty(Yufa[i].FanWei[j].ReceiveUserCard))
                                                r.取件人证卡号码 = "";
                                            else
                                                r.取件人证卡号码 = Yufa[i].FanWei[j].ReceiveUserCard;
                                        }
                                        if (rs.Length <= 0)
                                            m_DataSet.范围表.Add范围表Row(r);
                                    }//for
                                     //看当前有没有范围记录了
                                    rs = m_DataSet.范围表.Select("预发标识='" + bz + "'");
                                    if (rs.Length <= 0)
                                        m_DataSet.预发表.Remove预发表Row(row);
                                }
                            }//for
                        }//lock
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteFileLog("同步预发:" + ee.ToString());
                }
                #endregion

                #region 同步箱格信息
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("GetAllBoxInfo");

                    BoxInfo[] boxs = obj.GetAllBoxInfo(MinBoxId, MaxBoxId);
                    lock (m_DataSet.箱格信息)
                    {
                        iLock箱格信息 = 4;
                        m_DataSet.箱格信息.Rows.Clear();
                        for (int i = 0; i < boxs.Length; i++)
                        {
                            LocalDataSet.箱格信息Row row = m_DataSet.箱格信息.New箱格信息Row();
                            row.逻辑箱号 = boxs[i].BoxNO;
                            row.单位编号 = boxs[i].UnitCode;
                            row.简称 = boxs[i].BoxShowName;
                            row.全称 = boxs[i].BoxShowFullName;
                            row.是否清退箱 = boxs[i].IsQingTuiXiang;
                            row.箱格数量 = boxs[i].SendCount;
                            row.是否有急件 = boxs[i].HasJiJian;
                            row.登记数量 = boxs[i].DengJiCount;
                            row.登记急件数量 = boxs[i].DengJiJJCount;

                            row.箱格属性 = boxs[i].BoxProperty;

                            m_DataSet.箱格信息.Add箱格信息Row(row);
                        }
                        iLock箱格信息 = 0;
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteFileLog("同步箱格信息:" + ee.ToString());
                }
                #endregion

                #region 同步证卡
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("GetAllCardInfo");

                    CardInfoClass[] Card = obj.GetAllCardInfo(MinBoxId, MaxBoxId);
                    lock (m_DataSet.证卡列表)
                    {
                        m_DataSet.证卡列表.Rows.Clear();
                        for (int i = 0; i < Card.Length; i++)
                        {
                            LocalDataSet.证卡列表Row row = m_DataSet.证卡列表.New证卡列表Row();
                            row.证卡编号 = Card[i].ID;
                            row.验证类型 = Card[i].YanZhengType;
                            row.证卡号码 = Card[i].CardNO;
                            row.证卡类型 = Card[i].Cardtype;
                            row.箱格号码 = Card[i].boxNO;
                            if (string.IsNullOrEmpty(Card[i].UnitName))
                                row.单位名称 = "";
                            else
                                row.单位名称 = Card[i].UnitName;
                            if (string.IsNullOrEmpty(Card[i].UserName))
                                row.用户名称 = "";
                            else
                                row.用户名称 = Card[i].UserName;

                            if (Card[i].CardPrintType == 0)
                                row.用户可以打印的清单的类型 = 3;
                            else
                                row.用户可以打印的清单的类型 = Card[i].CardPrintType;

                            m_DataSet.证卡列表.Add证卡列表Row(row);
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteFileLog("同步证卡:" + ee.ToString());
                }
                #endregion

                #region 同步箱组状态
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("SendBoxStatus");
                    int i = m_BoxStatus.Count;
                    while (i > 0)
                    {
                        BoxStatus box = null;
                        lock (m_BoxStatus)
                        {
                            foreach (BoxStatus b1 in m_BoxStatus.Values)
                            {
                                box = b1;
                                break;
                            }
                            m_BoxStatus.Remove(box.BoxNo);
                        }

                        bool b = obj.SendBoxStatus(box);
                        if (!b)
                            SetBoxStatus(box);
                        i--;
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteFileLog("同步箱组状态:" + ee.ToString());
                }
                #endregion

                SyncUserGetLetter(0);

                #region 同步出差信息
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("GetBoxShowMessage");
                    ClassBoxShowMessage[] strRet = obj.GetBoxShowMessage(MinBoxId, MaxBoxId);
                    lock (m_DataSet.提示信息)
                    {
                        m_DataSet.提示信息.Clear();
                        for (int ii = 0; ii < strRet.Length; ii++)
                        {
                            LocalDataSet.提示信息Row row = m_DataSet.提示信息.New提示信息Row();
                            row.逻辑箱号 = strRet[ii].逻辑箱号;
                            row.出差信息 = strRet[ii].出差信息;

                            m_DataSet.提示信息.Add提示信息Row(row);
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteFileLog("同步出差信息:" + ee.ToString());
                }
                #endregion

            }
        }

        private static void SyncSendYuFa(object o)
        {
            if (!bConnected)
                return;
            #region 同步反馈
            try
            {
                MonitorService.MonitorService obj = GetConnectionObj("SendYuFaInfo");

                YuFaClass Yufa = null;
                lock (m_DataSet)
                {
                    for (int ii = m_DataSet.预发表.Rows.Count - 1; ii >= 0; ii--)
                    {
                        LocalDataSet.预发表Row row = (LocalDataSet.预发表Row)m_DataSet.预发表.Rows[ii];
                        if (row.是否同步)
                            continue;

                        Yufa = new YuFaClass();

                        Yufa.YFBizoZhi = row.预发标识;
                        Yufa.Barcode = row.条码编号;
                        Yufa.Barcode56 = row.条码编号56;
                        Yufa.FullBarcode = row.完整条码;
                        Yufa.Bartype = row.条码类型;
                        Yufa.IsJiJian = row.是否是急件;
                        Yufa.YuFaTime = row.预发时间;
                        Yufa.DocID = row.DocId;
                        Yufa.isDelete = row.是否删除;

                        if (row.Is份号Null())
                            Yufa.Showmsg = "";
                        else
                            Yufa.Showmsg = row.份号;

                        Yufa.YuFaType = row.投箱类型;
                        Yufa.isTuiXin = row.是否退信;
                        Yufa.TuiXinUserId = row.退信管理员证卡编号;
                        Yufa.FenZuTouXiangFangShi = row.分组投箱方式;

                        if (Yufa.Bartype == 1)
                        {
                            Yufa.ReceiveBoxNO = row.收件箱格号码;
                            Yufa.Showmsg = row.份号;
                            if (row.投箱状态)
                            {
                                Yufa.IsTouxiang = row.投箱状态;
                                Yufa.SendTime = row.投箱时间.ToString("yyyy-MM-dd HH:mm:ss");
                                Yufa.IsJiaJi = row.手工加急;
                                Yufa.SendUserCardNO = row.清退人;
                                Yufa.LuXiangFileName = row.录像文件名称;
                            }
                            if (row.是否勘误)
                            {
                                Yufa.IsKanWu = row.是否勘误;
                                Yufa.KanwuTime = row.勘误时间.ToString("yyyy-MM-dd HH:mm:ss");
                                Yufa.KwAdminCard = row.勘误管理员卡号;
                                Yufa.KwUserCard = row.勘误交换员卡号;
                            }
                            if (row.取件状态)
                            {
                                Yufa.IsQuJian = row.取件状态;
                                Yufa.ReceiveTime = row.取件时间.ToString("yyyy-MM-dd HH:mm:ss");
                                Yufa.ReceiveUserCard = row.取件人证卡号码;
                            }
                            Yufa.FanWei = new FanWeiClass[0];
                        }
                        else
                        {
                            //先找到当前
                            DataRow[] rs = m_DataSet.范围表.Select("预发标识='" + Yufa.YFBizoZhi + "' and 是否同步=false");
                            Yufa.FanWei = new FanWeiClass[rs.Length];
                            for (int i = 0; i < rs.Length; i++)
                            {
                                LocalDataSet.范围表Row r = (LocalDataSet.范围表Row)rs[i];
                                Yufa.FanWei[i] = new FanWeiClass();
                                Yufa.FanWei[i].YFBizoZhi = r.预发标识;
                                Yufa.FanWei[i].ReceiveBoxNO = r.收件箱格号码;
                                Yufa.FanWei[i].Showmsg = r.份号;
                                Yufa.FanWei[i].Count = r.分发份数;
                                Yufa.FanWei[i].isDelete = r.是否删除;
                                if (r.投箱状态)
                                {
                                    Yufa.FanWei[i].IsTouxiang = r.投箱状态;
                                    Yufa.FanWei[i].SendTime = r.投箱时间.ToString("yyyy-MM-dd HH:mm:ss");
                                    Yufa.FanWei[i].SendCount = r.投入份数;
                                }
                                if (r.是否勘误)
                                {
                                    Yufa.FanWei[i].IsKanWu = r.是否勘误;
                                    Yufa.FanWei[i].KanwuTime = r.勘误时间.ToString("yyyy-MM-dd HH:mm:ss");
                                    Yufa.FanWei[i].KwAdminCard = r.勘误管理员卡号;
                                    Yufa.FanWei[i].KwUserCard = r.勘误交换员卡号;
                                }
                                if (r.取件状态)
                                {
                                    Yufa.FanWei[i].IsQuJian = r.取件状态;
                                    Yufa.FanWei[i].ReceiveTime = r.取件时间.ToString("yyyy-MM-dd HH:mm:ss");
                                    Yufa.FanWei[i].ReceiveUserCard = r.取件人证卡号码;
                                }
                            }//for
                        }
                        //同步，如果出错，则通知箱头显示同步错误
                        bool b = obj.SendYuFaInfo(ref Yufa);
                        if (b)
                        {
                            //更新标识号
                            if (row.预发标识 == "WW")
                                row.预发标识 = Yufa.YFBizoZhi;
                            //更新同步状态
                            row.是否同步 = true;
                            if (row.取件状态 || row.是否勘误 || row.是否删除)
                                m_DataSet.预发表.Remove预发表Row(row);
                            else if (row.条码类型 != 1)
                            {
                                //更新范围
                                DataRow[] rs = m_DataSet.范围表.Select("预发标识='" + Yufa.YFBizoZhi + "'");
                                for (int i = 0; i < rs.Length; i++)
                                {
                                    LocalDataSet.范围表Row r = (LocalDataSet.范围表Row)rs[i];
                                    r.是否同步 = true;
                                    if (r.取件状态 || r.是否勘误 || r.是否删除)
                                        m_DataSet.范围表.Remove范围表Row(r);
                                }
                                rs = m_DataSet.范围表.Select("预发标识='" + Yufa.YFBizoZhi + "'");
                                if (rs.Length == 0)
                                {
                                    m_DataSet.预发表.Remove预发表Row(row);
                                }
                            }
                        }
                    }//foreach
                }//lock
            }
            catch (Exception ee)
            {
                Log.WriteFileLog("同步反馈:" + ee.ToString());
                if (bConnected)
                {
                    bConnected = false;
                    //提示所有箱头网络断开
                    OnSyncError(0, true);
                }
            }
            #endregion
        }

        private static void SyncUserGetLetter(object o)
        {
            if (!bConnected)
                return;

            SyncSendYuFa(0);

            #region 同步取件信息
            try
            {
                MonitorService.MonitorService obj = GetConnectionObj("UserGetLetter");
                lock (m_DataSet.用户取件表)
                {
                    for (int ii = m_DataSet.用户取件表.Rows.Count - 1; ii >= 0; ii--)
                    {
                        LocalDataSet.用户取件表Row row = (LocalDataSet.用户取件表Row)m_DataSet.用户取件表.Rows[ii];
                        if (row.是否同步)
                        {
                            m_DataSet.用户取件表.Remove用户取件表Row(row);
                            continue;
                        }

                        if (Constant.UserGetLetterType == 0)
                        {
                            #region 老方式同步
                            if (row.图片数据是否已经处理)
                            {
                                //判断次数
                                row.同步次数++;
                                bool b = obj.UserGetLetter(row.逻辑箱号, row.用户卡号, row.取件时间, row.照片内容, row.是否打印发件清单, row.是否打印取件清单);
                                if (b)
                                {
                                    row.是否同步 = true;
                                    m_DataSet.用户取件表.Remove用户取件表Row(row);
                                }
                            }
                            #endregion
                        }
                        else if (Constant.UserGetLetterType == 1)
                        {
                            //新的3次同步方式
                            #region 先第一次同步，获取到记录id
                            if (row.取件记录ID == 0)
                            {
                                row.同步次数++;
                                int id = obj.UserGetLetter_Record(row.逻辑箱号, row.用户卡号, row.取件时间);
                                if (id > 0)
                                {
                                    row.取件记录ID = id;
                                    //id获取后，直接打印清单
                                    try
                                    {
                                        UserGetLetterReturnClass ret = obj.UserGetLetter_Report(id, row.取件柜箱组名称, row.是否打印发件清单, row.是否打印取件清单);
                                        if (ret.bSuccess)
                                        {
                                            if (ret.bPrintSendReport || ret.bPrintRecvReport)
                                            {
                                                if (!row.Is清单打印机Null() && row.清单打印机.Trim() != "")
                                                {
                                                    PrintData data = new PrintData(ret, row.清单打印机);
                                                    //打印清单
                                                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ThreadPrintReport), data);
                                                }
                                                else
                                                {
                                                    Log.WriteInfo(LogType.Error, "用户取件打印清单错误：清单打印机没有设置。");
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.WriteInfo(LogType.Error, "用户取件错误：" + ex.ToString());
                                    }
                                }
                            }
                            #endregion

                            #region 同步取件照片
                            if (row.取件记录ID > 0 && row.图片数据是否已经处理)
                            {
                                bool b = obj.UserGetLetter_SetPic(row.取件记录ID, row.照片内容);
                                if (b)
                                {
                                    row.是否同步 = true;
                                    m_DataSet.用户取件表.Remove用户取件表Row(row);
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Log.WriteFileLog("同步取件信息:" + ee.ToString());
            }
            #endregion
        }
        #endregion

        #region 打印清单
        static int tmpFileIndex = 0;
        public static void ThreadPrintReport(object o)
        {
            int pError = 0;
            byte[] msg = new byte[1024];
            string tmp1 = "", tmp2 = "";
            try
            {
                PrintData data = (PrintData)o;
                if (data.data.bPrintSendReport)
                {
                    Log.WriteFileLog("准备打印发件清单，模板：" + data.data.szSendTemplateFileName + "，数据：" + data.data.szSendXmlData + "\r\n\r\n");
                    int tmp = System.Threading.Interlocked.Increment(ref tmpFileIndex);
                    string bartemplate = System.IO.Directory.GetCurrentDirectory() + "\\barDll\\" + data.data.szSendTemplateFileName;
                    string tmpfile = System.IO.Directory.GetCurrentDirectory() + "\\barDll\\" + tmp.ToString() + ".xml";
                    byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(data.data.szSendXmlData);
                    System.IO.File.WriteAllBytes(tmpfile, buf);
                    if (!System.IO.File.Exists(bartemplate))
                    {
                        Log.WriteFileLog("打印发件清单：没有找到发件清单模板。\r\n\r\n");
                    }
                    else if (!System.IO.File.Exists(tmpfile))
                    {
                        Log.WriteFileLog("打印发件清单：临时数据文件保存错误。\r\n\r\n");
                    }
                    else
                    {
                        bool b = Win32API.PrintTempletBarXML(bartemplate, tmpfile, data.PrinterName, ref pError, msg);
                        if (!b)
                        {
                            Log.WriteInfo(LogType.Error, "打印清单：返回打印失败。\r\n\r\n");
                        }
                        Log.WriteFileLog("打印发件清单，结果：" + b.ToString() + "\r\n\r\n");
                    }
                    tmp1 = tmpfile;
                }
                if (data.data.bPrintRecvReport)
                {
                    Log.WriteFileLog("准备打印收件清单，模板：" + data.data.szRecvTemplateFileName + "，数据：" + data.data.szRecvXmlData + "\r\n\r\n");
                    int tmp = System.Threading.Interlocked.Increment(ref tmpFileIndex);
                    string bartemplate = System.IO.Directory.GetCurrentDirectory() + "\\barDll\\" + data.data.szRecvTemplateFileName;
                    string tmpfile = System.IO.Directory.GetCurrentDirectory() + "\\barDll\\" + tmp.ToString() + ".xml";
                    System.IO.File.WriteAllText(tmpfile, data.data.szRecvXmlData, System.Text.Encoding.GetEncoding(936));
                    if (!System.IO.File.Exists(bartemplate))
                    {
                        Log.WriteFileLog("打印收件清单：没有找到收件清单模板。\r\n\r\n");
                    }
                    else if (!System.IO.File.Exists(tmpfile))
                    {
                        Log.WriteFileLog("打印收件清单：临时数据文件保存错误。\r\n\r\n");
                    }
                    else
                    {
                        bool b = Win32API.PrintTempletBarXML(bartemplate, tmpfile, data.PrinterName, ref pError, msg);
                        if (!b)
                            Log.WriteInfo(LogType.Error, "打印清单：返回打印失败。\r\n\r\n");
                        Log.WriteFileLog("打印收件清单，结果：" + b.ToString() + "\r\n\r\n");
                    }
                    tmp2 = tmpfile;
                }
                if (tmp1 != "")
                {
                    System.Threading.Thread.Sleep(1000);
                    System.IO.File.Delete(tmp1);
                }
                if (tmp2 != "")
                {
                    System.Threading.Thread.Sleep(1000);
                    System.IO.File.Delete(tmp2);
                }
            }
            catch (Exception ex)
            {
                Log.WriteFileLog("打印清单：" + ex.ToString());
            }
        }
        #endregion

        #region 打印退信清单
        /// <summary>
        /// 打印退信清单
        /// </summary>
        /// <param name="UserCardIdID">输入，打印清单的管理员的证卡编号</param>
        /// <param name="清单打印机">输入，清单打印机</param>
        /// <returns>返回清单数据</returns>
        public static bool PrintUserReturnLetter(string UserCardId, string 清单打印机)
        {
            bool bRet = false;
            try
            {
                MonitorService.MonitorService obj = GetConnectionObj("PrintUserReturnLetter");
                UserGetLetterReturnClass ret = obj.PrintUserReturnLetter(UserCardId);
                bRet = ret.bSuccess;
                if (ret.bSuccess)
                {
                    if (ret.bPrintSendReport || ret.bPrintRecvReport)
                    {
                        if (清单打印机.Trim() != "")
                        {
                            PrintData data = new PrintData(ret, 清单打印机);
                            //打印清单
                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ThreadPrintReport), data);
                        }
                        else
                        {
                            Log.WriteInfo(LogType.Error, "用户取件打印清单错误：清单打印机没有设置。");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteFileLog("打印退信清单：" + ex.ToString());
            }

            return bRet;
        }
        #endregion
    }

    public class PrintData
    {
        public UserGetLetterReturnClass data;
        public string PrinterName;

        public PrintData(UserGetLetterReturnClass data, string PrinterName)
        {
            this.data = data;
            this.PrinterName = PrinterName;
        }
    }
}
