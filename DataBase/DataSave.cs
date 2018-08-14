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
    /// Class1 ��ժҪ˵����
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
        /// ���һ��Ԥ����ʶ
        /// </summary>
        private static string LastYuFaBiaoZhi = "";
        private static bool m_bInitYuFa = false;
        /// <summary>
        /// ���ع��̱�
        /// </summary>
        private static LocalDataSet m_DataSet = new LocalDataSet();

        private static int iLock�����Ϣ = 0;
        #region ��������
        public static LocalDataSet LocalData
        {
            get { return (LocalDataSet)m_DataSet.Copy(); }
        }
        #endregion

        #region ������Ӧ����
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

        #region ��ʼ��ʼ��
        public static bool Start()
        {
            if (!Constant.UserLocalData)
                return true;
            bool bRet = false;
            try
            {
                //���̿��Ʊ�Ǽƻ���ʶ��ʼ��
                MonitorService.MonitorService obj = GetConnectionObj("GetNewBiaoZhi");
                obj.Dispose();
                //�����������ͬ���ӿ�
                obj = GetConnectionObj("GetGuiZeList");
                GuiZeListClass[] c = obj.GetGuiZeList(MinBoxId, MaxBoxId);
                foreach (GuiZeListClass c1 in c)
                {
                    LocalDataSet.�������Row row = m_DataSet.�������.New�������Row();
                    row.�����ʶ = c1.GZBiaoZhi;
                    row.�������� = c1.Name;
                    row.�������� = c1.BarType;
                    row.����ֱͶ = c1.HasZhiTou;
                    row.������� = c1.CanCheckRcvUnit;
                    row.�����շ� = c1.HasPuFa;
                    row.����ģ�� = c1.HasMoban;
                    m_DataSet.�������.Add�������Row(row);
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
        #region ֹͣ
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

        #region ��ȡ���������Ϣ�б�
        /// <summary>
        /// ��ȡ���������Ϣ�б�
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
                    b.BoxNO = box.�߼����.ToString();
                    b.FrontBoxBN = box.��������bn��;
                    b.BackBoxBN = box.���䱳��bn��;
                    b.HasTwoLock = box.˫����;
                    l.Add(b);
                }
                strRet = l.ToArray();
            }

            //�������
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
        #region ��ȡ����������Ϣͬ���ӿ�
        /// <summary>
        /// ��ȡ����������Ϣͬ���ӿ�
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
                        GroupName = group.��������,
                        GroupFrontScanBN = group.ǰ��ɨ��ͷbn��,
                        GroupFrontShowBN = group.ǰ��๦����bn��,
                        GroupFrontSoundBN = group.ǰ������BN��,
                        GroupFrontCardBN = group.ǰ�������bn��,
                        GroupFrontSheXiangTouBN = group.ǰ�洮������ͷBN��,
                        GroupFrontZhiJingMaiBN = group.ǰ��ָ����BN��,
                        GroupBackCardBN = group.���������bn��,
                        GroupBackScanBN = group.����ɨ��ͷbn��,
                        GroupBackSheXiangTouBN = group.���洮������ͷBN��,
                        GroupBackShowBN = group.����๦����bn��,
                        GroupBackSoundBN = group.��������BN��,
                        GroupBackZhiJingMaiBN = group.����ָ����BN��,
                        PrinterName = !group.IsNull("�嵥��ӡ��") ? group.�嵥��ӡ�� : "",
                        BoxArray = group.�����߼�����б�.Split(',')
                    };

                    if (!group.Is���ܵ����б�Null() && group.���ܵ����б� != "")
                    {
                        g.GroupNameArray = group.���ܵ����б�.Split(',');
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
        #region ��ȡ���������Ϣ�ӿ�
        /// <summary>
        /// ��ȡ���������Ϣ�ӿ�
        /// </summary>
        /// <returns>������ͷ��Ϣ��Ŀ��Ϣ</returns>
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
                    lock (m_DataSet.�����Ϣ)
                    {
                        iLock�����Ϣ = 1;
                        try
                        {
                            foreach (LocalDataSet.�����ϢRow row in m_DataSet.�����Ϣ)
                            {
                                if (row.�߼���� == boxNo)
                                {
                                    box = new BoxInfo();
                                    box.BoxNO = boxNo;
                                    box.BoxShowName = row.���;
                                    box.BoxShowFullName = row.ȫ��;
                                    box.IsQingTuiXiang = row.�Ƿ�������;

                                    box.BoxProperty = row.�������;
                                    //��������
                                    box.SendCount = 0;
                                    box.HasJiJian = false;

                                    //��
                                    var rs = Constant.m_IsPiaoJuXiang ? m_DataSet.Ԥ����.Select("Ͷ��״̬=true and ȡ��״̬=false and �Ƿ���=false") : m_DataSet.Ԥ����.Select("�ռ�������=" + boxNo + " and Ͷ��״̬=true and ȡ��״̬=false and �Ƿ���=false");
                                    foreach (var t in rs)
                                    {
                                        var r = (LocalDataSet.Ԥ����Row)t;
                                        if (r.�Ƿ��Ǽ���)
                                            box.HasJiJian = true;
                                    }
                                    box.SendCount += rs.Length;
                                    //��
                                    rs = m_DataSet.��Χ��.Select("�ռ�������=" + boxNo.ToString() + " and Ͷ��״̬=true and ȡ��״̬=false and �Ƿ���=false");
                                    for (int i = 0; i < rs.Length; i++)
                                    {
                                        LocalDataSet.��Χ��Row r = (LocalDataSet.��Χ��Row)rs[i];
                                        DataRow[] rs2 = m_DataSet.Ԥ����.Select("Ԥ����ʶ='" + r.Ԥ����ʶ + "'");
                                        if (rs2.Length > 0)
                                        {
                                            LocalDataSet.Ԥ����Row r2 = (LocalDataSet.Ԥ����Row)rs2[0];
                                            if (r2.�Ƿ��Ǽ���)
                                                box.HasJiJian = true;
                                        }
                                        box.SendCount += r.Ͷ�����;
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
                        iLock�����Ϣ = 0;
                    }
                }
            }
            return box;
        }
        #endregion


        #region ������������͡�������ż����룬ͬʱ���ؿ��Խ��յ����Ӻ���

        /// <summary>
        /// ������������͡�������ż����룬ͬʱ���ؿ��Խ��յ����Ӻ���
        /// </summary>
        /// <param name="barCode">���룬ɨ������</param>
        /// <param name="boxs">���ؿ�Ͷ�����</param>
        /// <returns>��������</returns>
        public static BarCodeType CheckBarCodeType(string barCode, out List<SendBoxList> boxs)
        {
            DateTime dtStart = DateTime.Now;
            string logMessage = "����ż���" + barCode + "\r\n";
            Log.WriteFileLog(logMessage);
            var rType = BarCodeType.��Ч;
            boxs = new List<SendBoxList>();
            if (Constant.UserLocalData)
            {
                //��ʱɾ�������߼�
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
            logMessage += "���ؼ���ż������" + rType + "\r\n";
            var ts = DateTime.Now - dtStart;
            logMessage = "��ʱ " + ts + " ��\r\n" + logMessage + "\r\n\r\n";
            Log.WriteFileLog(logMessage);

            return rType;
        }
        #endregion
        #region ���֤������

        /// <summary>
        /// ���֤������
        /// </summary>
        /// <param name="bn"></param>
        /// <param name="userCode"></param>
        /// <param name="boxs"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static BarCodeType CheckCardType(string bn, string userCode, out List<UserGetBoxInfo> boxs, out string userName)
        {
            var rType = BarCodeType.��Ч;
            boxs = new List<UserGetBoxInfo>();
            var logMessage = "���֤�����룺" + userCode + "\r\n";
            var dtStart = DateTime.Now;
            userName = "";
            if (Constant.UserLocalData)
            {
                #region ��������
                try
                {
                    lock (m_DataSet.֤���б�)
                    {

                        foreach (LocalDataSet.֤���б�Row row in m_DataSet.֤���б�)
                        {
                            if (row.��֤���� == 1 && row.֤������ == userCode)
                            {
                                UserGetBoxInfo t = new UserGetBoxInfo
                                {
                                    BoxNo = row.������,
                                    ֤����� = row.֤�����.ToString(),
                                    ��λ���� = row.��λ����,
                                    �û����� = row.�û�����,
                                    CardPrintType = (em_CardPrintType)row.�û����Դ�ӡ���嵥������
                                };

                                if (userName == "")
                                    userName = row.�û�����;

                                boxs.Add(t);
                                if (row.֤������ == 1)
                                    rType = BarCodeType.����Ա;
                                else
                                    rType = BarCodeType.����Ա;
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, ee.ToString());
                    logMessage += "�쳣�����ݣ�" + ee.ToString() + "\r\n";
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
                            ��λ���� = unitName,
                            �û����� = userName,
                            ֤����� = userCode
                        };
                        boxs.Add(box);
                    }
                }
            }

            logMessage += "���ؼ��֤����������" + rType + "\r\n";
            foreach (UserGetBoxInfo t in boxs)
            {
                logMessage += "֤����ţ�" + t.֤����� + ", �����룺" + t.BoxNo + "\r\n";
            }
            TimeSpan ts = DateTime.Now - dtStart;
            logMessage = "��ʱ " + ts + " ��\r\n" + logMessage + "\r\n\r\n";
            Log.WriteFileLog(logMessage);

            return rType;
        }
        #endregion
        #region ���ָ����
        /// <summary>
        /// ���ָ����
        /// </summary>
        /// <param name="info">ָ��������Base64�ַ���</param>
        /// <param name="iType">0���ϵ�ָ������1������ָ����</param>
        /// <returns></returns>
        public static BarCodeType CheckZhiJingMaiType(string info, int iType, out List<UserGetBoxInfo> boxs, out string UserName)
        {
            BarCodeType rType = BarCodeType.��Ч;
            boxs = new List<UserGetBoxInfo>();
            string LogMessage = "���ָ������" + info + "�����ͣ�" + iType.ToString() + "\r\n";
            DateTime dtStart = DateTime.Now;
            UserName = "";
            try
            {
                List<string> sEnrollData = new List<string>();
                lock (m_DataSet.֤���б�)
                {
                    foreach (LocalDataSet.֤���б�Row row in m_DataSet.֤���б�)
                    {
                        if (row.��֤���� == 2)
                        {
                            if (!sEnrollData.Contains(row.֤������))
                            {
                                sEnrollData.Add(row.֤������);
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
                    lock (m_DataSet.֤���б�)
                    {
                        foreach (LocalDataSet.֤���б�Row row in m_DataSet.֤���б�)
                        {
                            if (row.��֤���� == 2 && row.֤������.CompareTo(sRet) == 0)
                            {
                                UserGetBoxInfo t = new UserGetBoxInfo();
                                t.BoxNo = row.������;
                                t.֤����� = row.֤�����.ToString();
                                t.��λ���� = row.��λ����;
                                t.�û����� = row.�û�����;
                                t.CardPrintType = (em_CardPrintType)row.�û����Դ�ӡ���嵥������;

                                if (UserName == "")
                                    UserName = row.�û�����;

                                boxs.Add(t);
                                if (row.֤������ == 1)
                                    rType = BarCodeType.����Ա;
                                else
                                    rType = BarCodeType.����Ա;
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
                LogMessage += "�쳣�����ݣ�" + ee.ToString() + "\r\n";
            }

            LogMessage += "���ؼ��ָ���������" + rType.ToString() + "\r\n";
            foreach (UserGetBoxInfo t in boxs)
            {
                LogMessage += "֤����ţ�" + t.֤�����.ToString() + ", �����룺" + t.BoxNo.ToString() + "\r\n";
            }
            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "��ʱ " + ts.ToString() + " ��\r\n" + LogMessage + "\r\n\r\n";
            Log.WriteFileLog(LogMessage);

            return rType;
        }

        public static BarCodeType CheckZhiJingMaiLocationType(string LocationId, out List<UserGetBoxInfo> boxs, out string UserName)
        {
            BarCodeType rType = BarCodeType.��Ч;
            boxs = new List<UserGetBoxInfo>();
            string LogMessage = "���ָ����λ�ã�" + LocationId + "\r\n";
            DateTime dtStart = DateTime.Now;
            UserName = "";
            try
            {
                List<string> sEnrollData = new List<string>();
                lock (m_DataSet.֤���б�)
                {
                    foreach (LocalDataSet.֤���б�Row row in m_DataSet.֤���б�)
                    {
                        if (row.��֤���� == 2 && row.֤������ == LocationId)
                        {
                            UserGetBoxInfo t = new UserGetBoxInfo();
                            t.BoxNo = row.������;
                            t.֤����� = row.֤�����.ToString();
                            t.��λ���� = row.��λ����;
                            t.�û����� = row.�û�����;
                            t.CardPrintType = (em_CardPrintType)row.�û����Դ�ӡ���嵥������;

                            if (UserName == "")
                                UserName = row.�û�����;

                            boxs.Add(t);
                            if (row.֤������ == 1)
                                rType = BarCodeType.����Ա;
                            else
                                rType = BarCodeType.����Ա;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
                LogMessage += "�쳣�����ݣ�" + ee.ToString() + "\r\n";
            }

            LogMessage += "���ؼ��ָ���������" + rType.ToString() + "\r\n";
            foreach (UserGetBoxInfo t in boxs)
            {
                LogMessage += "֤����ţ�" + t.֤�����.ToString() + ", �����룺" + t.BoxNo.ToString() + "\r\n";
            }
            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "��ʱ " + ts.ToString() + " ��\r\n" + LogMessage + "\r\n\r\n";
            Log.WriteFileLog(LogMessage);

            return rType;
        }
        #endregion

        #region �����Ӧģ���б�
        /// <summary>
        /// �����Ӧģ���б�
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
        #region ����ģ����ϸ��Ϣ
        /// <summary>
        /// ����ģ����ϸ��Ϣ
        /// </summary>
        /// <returns></returns>
        public static bool GetMuBanInfo(string BarCode, string ģ���ʶ)
        {
            bool bRet = false;
            try
            {
                MonitorService.MonitorService obj = GetConnectionObj("GetMuBanInfo");

                YuFaClass yf = obj.GetMuBanInfo(ģ���ʶ);
                lock (m_DataSet)
                {
                    LocalDataSet.Ԥ����Row row = null;
                    string bz = yf.YFBizoZhi;
                    if (bz == null || bz == "")
                    {
                        bz = "WW";
                    }
                    if (bz.Substring(0, 1) != "Y" && bz.Substring(0, 1) != "W")
                        bz = "Y" + bz;
                    //���ҵ���ǰ��û��
                    DataRow[] rows = m_DataSet.Ԥ����.Select("Ԥ����ʶ='" + bz + "'");
                    if (rows.Length > 0)
                        row = (LocalDataSet.Ԥ����Row)rows[0];
                    else
                        row = m_DataSet.Ԥ����.NewԤ����Row();

                    row.�Ƿ�ͬ�� = false;
                    row.Ԥ����ʶ = bz;
                    row.������ = GuiZeClass.GetOneBarcode(BarCode);
                    row.�������� = BarCode;
                    row.�������� = yf.Bartype;
                    row.�Ƿ��Ǽ��� = yf.IsJiJian;
                    row.�Ƿ�ɾ�� = false;
                    if (rows.Length <= 0)
                        m_DataSet.Ԥ����.AddԤ����Row(row);
                    if (yf.Bartype == 1)
                    {
                        row.�ռ������� = yf.ReceiveBoxNO;
                        if (yf.Showmsg != null) row.�ݺ� = yf.Showmsg;
                    }
                    else
                    {
                        for (int j = 0; j < yf.FanWei.Length; j++)
                        {
                            LocalDataSet.��Χ��Row r = null;
                            //���ҵ���ǰ��û��
                            DataRow[] rs = m_DataSet.��Χ��.Select("Ԥ����ʶ='" + bz + "' and �ռ�������=" + yf.FanWei[j].ReceiveBoxNO);
                            if (rs.Length > 0)
                                r = (LocalDataSet.��Χ��Row)rs[0];
                            else
                                r = m_DataSet.��Χ��.New��Χ��Row();


                            r.�Ƿ�ͬ�� = false;
                            r.Ԥ����ʶ = bz;
                            r.�ռ������� = yf.FanWei[j].ReceiveBoxNO;
                            if (yf.FanWei[j].Showmsg != null) r.�ݺ� = yf.FanWei[j].Showmsg;
                            r.�ַ����� = yf.FanWei[j].Count;
                            r.�Ƿ�ɾ�� = false;

                            if (rs.Length <= 0)
                                m_DataSet.��Χ��.Add��Χ��Row(r);
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

        #region �����ż�

        /// <summary>
        /// �����ż�
        /// </summary>
        /// <param name="LetterCode"></param>
        /// <param name="BoxNO"></param>
        /// <param name="bUragent"></param>
        /// <returns>Ͷ���ż��ļ�������</returns>
        public static int SaveLetter(string LetterCode, int BoxNO, bool bUragent, int SendCount, string SendCardNO, string RecFileName, bool �Ƿ�����, string ���ŵĹ���Ա��֤�����)
        {
            DateTime dtStart = DateTime.Now; 
            string LogMessage = "�����ż���" + LetterCode + "����ţ�" + BoxNO + "\r\n";

            int iRet = -1; //error

            if (Constant.UserLocalData)
            {
                #region ��������
                try
                {
                    System.Threading.Monitor.Enter(m_DataSet);
                    DataRow[] rs = new DataRow[0];
                    if (SendCardNO == "")
                    {
                        int barID = GuiZeClass.GetBarCodeType(LetterCode);
                        int barType = 1;//�������͡�1��Ψһ�룬2��ͨ��
                        bool bHas = false; bool b1 = false, b2 = false, b3 = false, b4 = false, b5 = false;
                        lock (m_DataSet.�������)
                        {
                            foreach (LocalDataSet.�������Row r1 in m_DataSet.�������)
                            {
                                if (r1.�����ʶ == barID)
                                {
                                    bHas = true; barType = r1.��������;
                                    b1 = r1.����ֱͶ; b2 = r1.�������; b3 = r1.�����շ�; b4 = r1.����ģ��;
                                    b5 = r1.�����ظ�Ͷ��;
                                }
                            }
                        }

                        lock (m_DataSet)
                        {
                            rs = m_DataSet.Ԥ����.Select("������='" + GuiZeClass.GetOneBarcode(LetterCode) + "' or ������56='" + GuiZeClass.GetOneBarcode(LetterCode) + "'");
                            if (rs.Length > 0)
                            {
                                #region Ԥ����
                                LocalDataSet.Ԥ����Row row = (LocalDataSet.Ԥ����Row)rs[0];
                                if (row.�������� == 1)
                                {
                                    for (int j = 0; j < rs.Length; j++)
                                    {
                                        row = (LocalDataSet.Ԥ����Row)rs[j];
                                        if (row.�������� == 1 && row.�ռ������� == BoxNO && row.Ͷ��״̬ == false)
                                        {
                                            row.�Ƿ�ͬ�� = false;
                                            row.Ͷ��״̬ = true;
                                            row.Ͷ��ʱ�� = DateTime.Now;
                                            row.�ֹ��Ӽ� = bUragent;

                                            if (�Ƿ�����)
                                            {
                                                row.�Ƿ����� = true;
                                                row.���Ź���Ա֤����� = ���ŵĹ���Ա��֤�����;
                                            }
                                            else
                                                row.�Ƿ����� = false;

                                            row.¼���ļ����� = RecFileName;
                                            iRet = 0;
                                            if (bUragent || row.�Ƿ��Ǽ���)
                                                iRet = 1;
                                        }
                                    }
                                }
                                else if (row.�������� == 2)
                                {
                                    bHas = false;
                                    for (int j = 0; j < rs.Length; j++)
                                    {
                                        row = (LocalDataSet.Ԥ����Row)rs[j];
                                        if (row.�������� == 2)
                                        {
                                            DataRow[] rs2 = m_DataSet.��Χ��.Select("Ԥ����ʶ='" + row.Ԥ����ʶ + "' and �ռ�������=" + BoxNO.ToString());
                                            for (int i = 0; i < rs2.Length; i++)
                                            {
                                                LocalDataSet.��Χ��Row row2 = (LocalDataSet.��Χ��Row)rs2[i];
                                                if (row2.�ռ������� == BoxNO)
                                                {
                                                    row.�Ƿ�ͬ�� = false;
                                                    row.¼���ļ����� = RecFileName;
                                                    row2.�Ƿ�ͬ�� = false;
                                                    row2.Ͷ��״̬ = true;
                                                    row2.Ͷ��ʱ�� = DateTime.Now;
                                                    row2.Ͷ����� += SendCount;
                                                    bHas = true;
                                                }
                                            }
                                            if (bHas)
                                            {
                                                iRet = 0;
                                                if (row.�Ƿ��Ǽ���)
                                                    iRet = 1;
                                            }
                                        }
                                    }//for
                                    if (!bHas)
                                    {
                                        row = (LocalDataSet.Ԥ����Row)rs[0];
                                        //�շ�
                                        LocalDataSet.��Χ��Row row2 = m_DataSet.��Χ��.New��Χ��Row();
                                        row2.Ԥ����ʶ = row.Ԥ����ʶ;
                                        row2.�ռ������� = BoxNO;
                                        row2.�ַ����� = SendCount;
                                        row2.Ͷ��״̬ = true;
                                        row2.Ͷ��ʱ�� = DateTime.Now;
                                        row2.Ͷ����� = SendCount;
                                        m_DataSet.��Χ��.Add��Χ��Row(row2);

                                        row.�Ƿ�ͬ�� = false;
                                        row.¼���ļ����� = RecFileName;

                                        iRet = 0;
                                        if (row.�Ƿ��Ǽ���)
                                            iRet = 1;
                                    }
                                }
                                #endregion
                            }//if
                        }
                    }//��������
                     //û��Ԥ���ģ�ΪֱͶ�Ļ����շ���
                    if (iRet == -1)
                    {
                        iRet = 0;
                        //��������
                        int barID = GuiZeClass.GetBarCodeType(LetterCode);
                        int barType = 1;//�������͡�1��Ψһ�룬2��ͨ��
                        if (SendCardNO == "")
                        {
                            lock (m_DataSet.�������)
                            {
                                foreach (LocalDataSet.�������Row r1 in m_DataSet.�������)
                                {
                                    if (r1.�����ʶ == barID)
                                    {
                                        barType = r1.��������; break;
                                    }
                                }
                            }
                        }
                        bool bTiaoMaJiJian = GuiZeClass.CheckBarcodeIsJiJian(LetterCode);

                        lock (m_DataSet)
                        {
                            bUragent = bUragent || bTiaoMaJiJian;
                            LocalDataSet.Ԥ����Row row = m_DataSet.Ԥ����.NewԤ����Row();
                            row.Ԥ����ʶ = "WW";
                            m_DataSet.Ԥ����.AddԤ����Row(row);
                            row.������ = GuiZeClass.GetOneBarcode(LetterCode);
                            row.�������� = LetterCode;
                            row.�Ƿ��Ǽ��� = bUragent;
                            row.¼���ļ����� = RecFileName;
                            row.�������� = barType;
                            row.�Ƿ�ͬ�� = false;
                            row.Ԥ��ʱ�� = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            row.DocId = "0";

                            if (row.�������� == 1)
                            {
                                row.�ռ������� = BoxNO;
                                row.Ͷ��״̬ = true;
                                row.Ͷ��ʱ�� = DateTime.Now;
                                row.�ֹ��Ӽ� = bUragent;
                                row.�Ƿ��Ǽ��� = bUragent;
                                row.������ = SendCardNO;
                                if (bUragent)
                                    iRet = 1;
                                if (�Ƿ�����)
                                {
                                    row.�Ƿ����� = true;
                                    row.���Ź���Ա֤����� = ���ŵĹ���Ա��֤�����;
                                }
                                else
                                    row.�Ƿ����� = false;
                            }
                            else if (row.�������� == 2)
                            {
                                //�շ�
                                LocalDataSet.��Χ��Row row2 = m_DataSet.��Χ��.New��Χ��Row();
                                row2.Ԥ����ʶ = row.Ԥ����ʶ;
                                row2.�ռ������� = BoxNO;
                                row2.�ַ����� = SendCount;
                                row2.Ͷ��״̬ = true;
                                row2.Ͷ��ʱ�� = DateTime.Now;
                                row2.Ͷ����� = SendCount;
                                row2.�ݺ� = "";
                                m_DataSet.��Χ��.Add��Χ��Row(row2);
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, ee.ToString());
                    LogMessage += "�쳣�����ݣ�" + ee.ToString() + "\r\n";
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

            LogMessage += "���ر����ż������" + iRet + "\r\n";
            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "��ʱ " + ts + " ��\r\n" + LogMessage + "\r\n\r\n";
            Log.WriteFileLog(LogMessage);

            return iRet;
        }
        #endregion
        #region ���濱����Ϣ
        /// <summary>
        /// ���濱����Ϣ
        /// </summary>
        /// <param name="BoxNO"></param>
        /// <param name="LetterCode"></param>
        /// <param name="AdminBarCode"></param>
        /// <param name="UserBarCode"></param>
        /// <returns>���������sumcopy����</returns>
        public static bool SaveErratumLetter(int BoxNO, string LetterCode, string AdminBarCode, string UserBarCode)
        {
            bool bRet = false;
            if (Constant.UserLocalData)
            {
                #region ��������
                try
                {
                    System.Threading.Monitor.Enter(m_DataSet);
                    DataRow[] rs = m_DataSet.Ԥ����.Select("������='" + GuiZeClass.GetOneBarcode(LetterCode) + "'");
                    for (int j = 0; j < rs.Length; j++)
                    {
                        LocalDataSet.Ԥ����Row row = (LocalDataSet.Ԥ����Row)rs[j];
                        if (row.�������� == 1 && row.�ռ������� == BoxNO)
                        {
                            row.�Ƿ�ͬ�� = false;
                            row.�Ƿ��� = true;
                            row.����ʱ�� = DateTime.Now;
                            row.�������Ա���� = AdminBarCode;
                            row.���󽻻�Ա���� = UserBarCode;
                        }
                        else if (row.�������� == 2)
                        {
                            DataRow[] rs2 = m_DataSet.��Χ��.Select("Ԥ����ʶ='" + row.Ԥ����ʶ + "' and �ռ�������=" + BoxNO.ToString());
                            for (int i = 0; i < rs2.Length; i++)
                            {
                                LocalDataSet.��Χ��Row row2 = (LocalDataSet.��Χ��Row)rs2[i];
                                if (row2.�ռ������� == BoxNO)
                                {
                                    row.�Ƿ�ͬ�� = false;
                                    row2.�Ƿ�ͬ�� = false;
                                    row2.�Ƿ��� = true;
                                    row2.����ʱ�� = DateTime.Now;
                                    row2.�������Ա���� = AdminBarCode;
                                    row2.���󽻻�Ա���� = UserBarCode;
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
        #region ����Ա����ɨ��ȡ��
        /// <summary>
        /// ����Ա����ɨ��ȡ��
        /// </summary>
        /// <param name="BoxNO">�߼����</param>
        /// <param name="UserBarCode"></param>
        /// <returns></returns>
        public static bool Box_UserGetLetter(int BoxNO, string UserBarCode, string PicData, bool bQuJian, bool bSend, bool bRecv, bool ͼ���Ƿ���, string �嵥��ӡ��, string ȡ������������)
        {
            bool bRet = false;
            if (Constant.UserLocalData)
            {
                #region ��������
                try
                {
                    if (bQuJian)
                    {
                        lock (m_DataSet)
                        {
                            foreach (LocalDataSet.Ԥ����Row row in m_DataSet.Ԥ����)
                            {
                                if (row.�������� == 1 && row.�ռ������� == BoxNO && row.Ͷ��״̬ == true)
                                {
                                    row.�Ƿ�ͬ�� = false;
                                    row.ȡ��״̬ = true;
                                    row.ȡ��ʱ�� = DateTime.Now;
                                    row.ȡ����֤������ = UserBarCode;
                                }
                                else if (row.�������� == 2)
                                {
                                    DataRow[] rs2 = m_DataSet.��Χ��.Select("Ͷ��״̬=true and Ԥ����ʶ='" + row.Ԥ����ʶ + "' and �ռ�������=" + BoxNO.ToString());
                                    for (int i = 0; i < rs2.Length; i++)
                                    {
                                        LocalDataSet.��Χ��Row row2 = (LocalDataSet.��Χ��Row)rs2[i];
                                        if (row2.�ռ������� == BoxNO)
                                        {
                                            row.�Ƿ�ͬ�� = false;
                                            row2.�Ƿ�ͬ�� = false;
                                            row2.ȡ��״̬ = true;
                                            row2.ȡ��ʱ�� = DateTime.Now;
                                            row2.ȡ����֤������ = UserBarCode;
                                        }
                                    }
                                }
                            }
                        }//lock

                    }

                    lock (m_DataSet.�û�ȡ����)
                    {
                        LocalDataSet.�û�ȡ����Row rq = m_DataSet.�û�ȡ����.New�û�ȡ����Row();
                        rq.�߼���� = BoxNO;
                        rq.ȡ��ʱ�� = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        rq.�Ƿ�ͬ�� = false;
                        rq.ͬ������ = 0;
                        rq.�û����� = UserBarCode;
                        rq.��Ƭ���� = PicData;
                        rq.�Ƿ��ӡ�����嵥 = bSend;
                        rq.�Ƿ��ӡȡ���嵥 = bRecv;
                        rq.ͼƬ�����Ƿ��Ѿ����� = ͼ���Ƿ���;
                        rq.ȡ����¼ID = 0;
                        rq.�嵥��ӡ�� = �嵥��ӡ��;
                        rq.ȡ������������ = ȡ������������;
                        m_DataSet.�û�ȡ����.Add�û�ȡ����Row(rq);
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
            #region ��������
            try
            {
                lock (m_DataSet.�û�ȡ����)
                {
                    DataRow[] rows = m_DataSet.�û�ȡ����.Select("�߼����=" + BoxNO.ToString() + " and �û�����='" + UserBarCode + "' and ͼƬ�����Ƿ��Ѿ�����=false");
                    if (rows.Length > 0)
                    {
                        rows[0]["��Ƭ����"] = PicData;
                        rows[0]["ͼƬ�����Ƿ��Ѿ�����"] = true;
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
                foreach (LocalDataSet.Ԥ����Row row in m_DataSet.Ԥ����)
                {
                    if (row.�������� != LetterCode) continue;
                    if (row.�������� == 1 && row.�ռ������� == BoxNO)
                    {
                        row.�Ƿ�ͬ�� = false;
                        row.ȡ��״̬ = true;
                        row.ȡ��ʱ�� = DateTime.Now;
                        row.ȡ����֤������ = UserBarCode;
                    }
                    else if (row.�������� == 2)
                    {
                        DataRow[] rs2 = m_DataSet.��Χ��.Select("Ԥ����ʶ='" + row.Ԥ����ʶ + "' and �ռ�������=" + BoxNO.ToString());
                        for (int i = 0; i < rs2.Length; i++)
                        {
                            LocalDataSet.��Χ��Row row2 = (LocalDataSet.��Χ��Row)rs2[i];
                            if (row2.�ռ������� == BoxNO)
                            {
                                row.�Ƿ�ͬ�� = false;
                                row2.�Ƿ�ͬ�� = false;
                                row2.ȡ��״̬ = true;
                                row2.ȡ��ʱ�� = DateTime.Now;
                                row2.ȡ����֤������ = UserBarCode;
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

        #region ��ʾ��Ϣͬ���ӿ�
        /// <summary>
        /// ��ʾ��Ϣͬ���ӿ�
        /// </summary>
        /// <returns></returns>
        public static ClassBoxShowMessage GetBoxShowMessage(int �߼����)
        {
            ClassBoxShowMessage strRet = new ClassBoxShowMessage();
            strRet.�߼���� = �߼����;
            strRet.������Ϣ = "";
            try
            {
                DataRow[] rs = null;
                lock (m_DataSet.��ʾ��Ϣ)
                {
                    rs = m_DataSet.��ʾ��Ϣ.Select("�߼����=" + �߼����.ToString());
                    if (rs != null)
                    {
                        for (int i = 0; i < rs.Length; i++)
                        {
                            if (strRet.������Ϣ.CompareTo("") == 0)
                                strRet.������Ϣ = rs[i][1].ToString().Trim();
                            else
                            {
                                if (�߼���� != 0)
                                    strRet.������Ϣ += rs[i][1].ToString().Trim();
                                else
                                    strRet.������Ϣ += "\r\n" + rs[i][1].ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Log.WriteInfo(LogType.Error, ee.ToString());
                strRet.������Ϣ = "";
            }

            return strRet;
        }
        #endregion

        #region �������״̬
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

        #region ͬ��
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

                #region ͬ��������ԣ��ж�����״̬����������������ͬ����ͬʱ����ʾ��ͷ����Ͽ�
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("GetGuiZeList");

                    GuiZeListClass[] guize = obj.GetGuiZeList(MinBoxId, MaxBoxId);
                    lock (m_DataSet.�������)
                    {
                        m_DataSet.�������.Rows.Clear();
                        for (int i = 0; i < guize.Length; i++)
                        {
                            LocalDataSet.�������Row row = m_DataSet.�������.New�������Row();
                            row.�����ʶ = guize[i].GZBiaoZhi;
                            row.�������� = guize[i].Name;
                            row.�������� = guize[i].BarType;
                            row.����ֱͶ = guize[i].HasZhiTou;
                            row.������� = guize[i].CanCheckRcvUnit;
                            row.�����շ� = guize[i].HasPuFa;
                            row.����ģ�� = guize[i].HasMoban;
                            row.�����ظ�Ͷ�� = guize[i].CanRepeat;
                            m_DataSet.�������.Add�������Row(row);
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteInfo(LogType.Error, "ͬ��������ԣ�" + ee.ToString());
                    Log.WriteFileLog("ͬ��������ԣ�" + ee.ToString());
                    if (bConnected)
                    {
                        bConnected = false;
                        //��ʾ������ͷ����Ͽ�
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

                #region ͬ��Ԥ��
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("GetYuFaInfo");
                    YuFaClass[] Yufa = null;
                    if (!m_bInitYuFa)
                    {
                        //��ʼ��
                        Yufa = obj.GetYuFaInfo("", MinBoxId, MaxBoxId);

                        //ͬ���ɹ�������
                        m_bInitYuFa = true;
                    }
                    else
                    {
                        //��ʼ����
                        Yufa = obj.GetYuFaInfo("1", MinBoxId, MaxBoxId);//LastYuFaBiaoZhi
                    }
                    if (Yufa != null && Yufa.Length > 0)
                    {
                        lock (m_DataSet)
                        {
                            LastYuFaBiaoZhi = Yufa[Yufa.Length - 1].YFBizoZhi;
                            for (int i = 0; i < Yufa.Length; i++)
                            {
                                LocalDataSet.Ԥ����Row row = null;
                                string bz = Yufa[i].YFBizoZhi;
                                if (bz.Substring(0, 1) != "Y" && bz.Substring(0, 1) != "W")
                                    bz = "Y" + bz;
                                //���ҵ���ǰ��û��
                                DataRow[] rows = m_DataSet.Ԥ����.Select("Ԥ����ʶ='" + bz + "'");
                                if (rows.Length > 0)
                                    row = (LocalDataSet.Ԥ����Row)rows[0];
                                else
                                    row = m_DataSet.Ԥ����.NewԤ����Row();

                                row.�Ƿ�ͬ�� = false;
                                row.Ԥ����ʶ = bz;
                                row.������ = Yufa[i].Barcode;
                                if (!String.IsNullOrEmpty(Yufa[i].Barcode56))
                                    row.������56 = Yufa[i].Barcode56;
                                else
                                    row.������56 = "";
                                row.�������� = Yufa[i].FullBarcode;
                                row.�������� = Yufa[i].Bartype;
                                row.�Ƿ��Ǽ��� = Yufa[i].IsJiJian;
                                if (string.IsNullOrEmpty(Yufa[i].YuFaTime))
                                    row.Ԥ��ʱ�� = "";
                                else
                                    row.Ԥ��ʱ�� = Yufa[i].YuFaTime;
                                if (string.IsNullOrEmpty(Yufa[i].DocID))
                                    row.DocId = "";
                                else
                                    row.DocId = Yufa[i].DocID;
                                row.�Ƿ�ɾ�� = Yufa[i].isDelete;

                                row.Ͷ������ = Yufa[i].YuFaType;
                                row.�Ƿ����� = Yufa[i].isTuiXin;
                                if (string.IsNullOrEmpty(Yufa[i].TuiXinUserId))
                                    row.���Ź���Ա֤����� = "";
                                else
                                    row.���Ź���Ա֤����� = Yufa[i].TuiXinUserId;
                                row.����Ͷ�䷽ʽ = Yufa[i].FenZuTouXiangFangShi;

                                if (Yufa[i].Showmsg != null) row.�ݺ� = Yufa[i].Showmsg;

                                if (rows.Length <= 0)
                                    m_DataSet.Ԥ����.AddԤ����Row(row);
                                if (Yufa[i].Bartype == 1)
                                {
                                    row.�ռ������� = Yufa[i].ReceiveBoxNO;
                                    if (Yufa[i].IsTouxiang)
                                    {
                                        row.Ͷ��״̬ = Yufa[i].IsTouxiang;
                                        row.Ͷ��ʱ�� = Convert.ToDateTime(Yufa[i].SendTime);
                                        row.�ֹ��Ӽ� = Yufa[i].IsJiaJi;
                                        if (string.IsNullOrEmpty(Yufa[i].SendUserCardNO))
                                            row.������ = "";
                                        else
                                            row.������ = Yufa[i].SendUserCardNO;
                                        if (string.IsNullOrEmpty(Yufa[i].LuXiangFileName))
                                            row.¼���ļ����� = "";
                                        else
                                            row.¼���ļ����� = Yufa[i].LuXiangFileName;
                                    }
                                    if (Yufa[i].IsKanWu)
                                    {
                                        row.�Ƿ��� = Yufa[i].IsKanWu;
                                        row.����ʱ�� = Convert.ToDateTime(Yufa[i].KanwuTime);
                                        if (string.IsNullOrEmpty(Yufa[i].KwAdminCard))
                                            row.�������Ա���� = "";
                                        else
                                            row.�������Ա���� = Yufa[i].KwAdminCard;
                                        if (string.IsNullOrEmpty(Yufa[i].KwUserCard))
                                            row.���󽻻�Ա���� = "";
                                        else
                                            row.���󽻻�Ա���� = Yufa[i].KwUserCard;
                                    }
                                    if (Yufa[i].IsQuJian)
                                    {
                                        row.ȡ��״̬ = Yufa[i].IsQuJian;
                                        row.ȡ��ʱ�� = Convert.ToDateTime(Yufa[i].ReceiveTime);
                                        if (string.IsNullOrEmpty(Yufa[i].ReceiveUserCard))
                                            row.ȡ����֤������ = "";
                                        else
                                            row.ȡ����֤������ = Yufa[i].ReceiveUserCard;
                                    }
                                }
                                else
                                {
                                    //�з�Χ��ͨ��
                                    DataRow[] rs;
                                    for (int j = 0; j < Yufa[i].FanWei.Length; j++)
                                    {
                                        LocalDataSet.��Χ��Row r = null;
                                        //���ҵ���ǰ��û��
                                        rs = m_DataSet.��Χ��.Select("Ԥ����ʶ='" + bz + "' and �ռ�������=" + Yufa[i].FanWei[j].ReceiveBoxNO);
                                        if (rs.Length > 0)
                                            r = (LocalDataSet.��Χ��Row)rs[0];
                                        else
                                            r = m_DataSet.��Χ��.New��Χ��Row();

                                        r.�Ƿ�ͬ�� = false;
                                        r.Ԥ����ʶ = bz;
                                        r.�ռ������� = Yufa[i].FanWei[j].ReceiveBoxNO;
                                        if (Yufa[i].FanWei[j].Showmsg != null) r.�ݺ� = Yufa[i].FanWei[j].Showmsg;
                                        r.�ַ����� = Yufa[i].FanWei[j].Count;
                                        r.�Ƿ�ɾ�� = Yufa[i].FanWei[j].isDelete;
                                        if (Yufa[i].FanWei[j].IsTouxiang)
                                        {
                                            r.Ͷ��״̬ = Yufa[i].FanWei[j].IsTouxiang;
                                            r.Ͷ��ʱ�� = Convert.ToDateTime(Yufa[i].FanWei[j].SendTime);
                                            r.Ͷ����� = Yufa[i].FanWei[j].SendCount;
                                        }
                                        if (Yufa[i].FanWei[j].IsKanWu)
                                        {
                                            r.�Ƿ��� = Yufa[i].FanWei[j].IsKanWu;
                                            r.����ʱ�� = Convert.ToDateTime(Yufa[i].FanWei[j].KanwuTime);
                                            if (string.IsNullOrEmpty(Yufa[i].FanWei[j].KwAdminCard))
                                                r.�������Ա���� = "";
                                            else
                                                r.�������Ա���� = Yufa[i].FanWei[j].KwAdminCard;
                                            if (string.IsNullOrEmpty(Yufa[i].FanWei[j].KwUserCard))
                                                r.���󽻻�Ա���� = "";
                                            else
                                                r.���󽻻�Ա���� = Yufa[i].FanWei[j].KwUserCard;
                                        }
                                        if (Yufa[i].FanWei[j].IsQuJian)
                                        {
                                            r.ȡ��״̬ = Yufa[i].FanWei[j].IsQuJian;
                                            r.ȡ��ʱ�� = Convert.ToDateTime(Yufa[i].FanWei[j].ReceiveTime);
                                            if (string.IsNullOrEmpty(Yufa[i].FanWei[j].ReceiveUserCard))
                                                r.ȡ����֤������ = "";
                                            else
                                                r.ȡ����֤������ = Yufa[i].FanWei[j].ReceiveUserCard;
                                        }
                                        if (rs.Length <= 0)
                                            m_DataSet.��Χ��.Add��Χ��Row(r);
                                    }//for
                                     //����ǰ��û�з�Χ��¼��
                                    rs = m_DataSet.��Χ��.Select("Ԥ����ʶ='" + bz + "'");
                                    if (rs.Length <= 0)
                                        m_DataSet.Ԥ����.RemoveԤ����Row(row);
                                }
                            }//for
                        }//lock
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteFileLog("ͬ��Ԥ��:" + ee.ToString());
                }
                #endregion

                #region ͬ�������Ϣ
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("GetAllBoxInfo");

                    BoxInfo[] boxs = obj.GetAllBoxInfo(MinBoxId, MaxBoxId);
                    lock (m_DataSet.�����Ϣ)
                    {
                        iLock�����Ϣ = 4;
                        m_DataSet.�����Ϣ.Rows.Clear();
                        for (int i = 0; i < boxs.Length; i++)
                        {
                            LocalDataSet.�����ϢRow row = m_DataSet.�����Ϣ.New�����ϢRow();
                            row.�߼���� = boxs[i].BoxNO;
                            row.��λ��� = boxs[i].UnitCode;
                            row.��� = boxs[i].BoxShowName;
                            row.ȫ�� = boxs[i].BoxShowFullName;
                            row.�Ƿ������� = boxs[i].IsQingTuiXiang;
                            row.������� = boxs[i].SendCount;
                            row.�Ƿ��м��� = boxs[i].HasJiJian;
                            row.�Ǽ����� = boxs[i].DengJiCount;
                            row.�ǼǼ������� = boxs[i].DengJiJJCount;

                            row.������� = boxs[i].BoxProperty;

                            m_DataSet.�����Ϣ.Add�����ϢRow(row);
                        }
                        iLock�����Ϣ = 0;
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteFileLog("ͬ�������Ϣ:" + ee.ToString());
                }
                #endregion

                #region ͬ��֤��
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("GetAllCardInfo");

                    CardInfoClass[] Card = obj.GetAllCardInfo(MinBoxId, MaxBoxId);
                    lock (m_DataSet.֤���б�)
                    {
                        m_DataSet.֤���б�.Rows.Clear();
                        for (int i = 0; i < Card.Length; i++)
                        {
                            LocalDataSet.֤���б�Row row = m_DataSet.֤���б�.New֤���б�Row();
                            row.֤����� = Card[i].ID;
                            row.��֤���� = Card[i].YanZhengType;
                            row.֤������ = Card[i].CardNO;
                            row.֤������ = Card[i].Cardtype;
                            row.������ = Card[i].boxNO;
                            if (string.IsNullOrEmpty(Card[i].UnitName))
                                row.��λ���� = "";
                            else
                                row.��λ���� = Card[i].UnitName;
                            if (string.IsNullOrEmpty(Card[i].UserName))
                                row.�û����� = "";
                            else
                                row.�û����� = Card[i].UserName;

                            if (Card[i].CardPrintType == 0)
                                row.�û����Դ�ӡ���嵥������ = 3;
                            else
                                row.�û����Դ�ӡ���嵥������ = Card[i].CardPrintType;

                            m_DataSet.֤���б�.Add֤���б�Row(row);
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteFileLog("ͬ��֤��:" + ee.ToString());
                }
                #endregion

                #region ͬ������״̬
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
                    Log.WriteFileLog("ͬ������״̬:" + ee.ToString());
                }
                #endregion

                SyncUserGetLetter(0);

                #region ͬ��������Ϣ
                try
                {
                    MonitorService.MonitorService obj = GetConnectionObj("GetBoxShowMessage");
                    ClassBoxShowMessage[] strRet = obj.GetBoxShowMessage(MinBoxId, MaxBoxId);
                    lock (m_DataSet.��ʾ��Ϣ)
                    {
                        m_DataSet.��ʾ��Ϣ.Clear();
                        for (int ii = 0; ii < strRet.Length; ii++)
                        {
                            LocalDataSet.��ʾ��ϢRow row = m_DataSet.��ʾ��Ϣ.New��ʾ��ϢRow();
                            row.�߼���� = strRet[ii].�߼����;
                            row.������Ϣ = strRet[ii].������Ϣ;

                            m_DataSet.��ʾ��Ϣ.Add��ʾ��ϢRow(row);
                        }
                    }
                }
                catch (Exception ee)
                {
                    Log.WriteFileLog("ͬ��������Ϣ:" + ee.ToString());
                }
                #endregion

            }
        }

        private static void SyncSendYuFa(object o)
        {
            if (!bConnected)
                return;
            #region ͬ������
            try
            {
                MonitorService.MonitorService obj = GetConnectionObj("SendYuFaInfo");

                YuFaClass Yufa = null;
                lock (m_DataSet)
                {
                    for (int ii = m_DataSet.Ԥ����.Rows.Count - 1; ii >= 0; ii--)
                    {
                        LocalDataSet.Ԥ����Row row = (LocalDataSet.Ԥ����Row)m_DataSet.Ԥ����.Rows[ii];
                        if (row.�Ƿ�ͬ��)
                            continue;

                        Yufa = new YuFaClass();

                        Yufa.YFBizoZhi = row.Ԥ����ʶ;
                        Yufa.Barcode = row.������;
                        Yufa.Barcode56 = row.������56;
                        Yufa.FullBarcode = row.��������;
                        Yufa.Bartype = row.��������;
                        Yufa.IsJiJian = row.�Ƿ��Ǽ���;
                        Yufa.YuFaTime = row.Ԥ��ʱ��;
                        Yufa.DocID = row.DocId;
                        Yufa.isDelete = row.�Ƿ�ɾ��;

                        if (row.Is�ݺ�Null())
                            Yufa.Showmsg = "";
                        else
                            Yufa.Showmsg = row.�ݺ�;

                        Yufa.YuFaType = row.Ͷ������;
                        Yufa.isTuiXin = row.�Ƿ�����;
                        Yufa.TuiXinUserId = row.���Ź���Ա֤�����;
                        Yufa.FenZuTouXiangFangShi = row.����Ͷ�䷽ʽ;

                        if (Yufa.Bartype == 1)
                        {
                            Yufa.ReceiveBoxNO = row.�ռ�������;
                            Yufa.Showmsg = row.�ݺ�;
                            if (row.Ͷ��״̬)
                            {
                                Yufa.IsTouxiang = row.Ͷ��״̬;
                                Yufa.SendTime = row.Ͷ��ʱ��.ToString("yyyy-MM-dd HH:mm:ss");
                                Yufa.IsJiaJi = row.�ֹ��Ӽ�;
                                Yufa.SendUserCardNO = row.������;
                                Yufa.LuXiangFileName = row.¼���ļ�����;
                            }
                            if (row.�Ƿ���)
                            {
                                Yufa.IsKanWu = row.�Ƿ���;
                                Yufa.KanwuTime = row.����ʱ��.ToString("yyyy-MM-dd HH:mm:ss");
                                Yufa.KwAdminCard = row.�������Ա����;
                                Yufa.KwUserCard = row.���󽻻�Ա����;
                            }
                            if (row.ȡ��״̬)
                            {
                                Yufa.IsQuJian = row.ȡ��״̬;
                                Yufa.ReceiveTime = row.ȡ��ʱ��.ToString("yyyy-MM-dd HH:mm:ss");
                                Yufa.ReceiveUserCard = row.ȡ����֤������;
                            }
                            Yufa.FanWei = new FanWeiClass[0];
                        }
                        else
                        {
                            //���ҵ���ǰ
                            DataRow[] rs = m_DataSet.��Χ��.Select("Ԥ����ʶ='" + Yufa.YFBizoZhi + "' and �Ƿ�ͬ��=false");
                            Yufa.FanWei = new FanWeiClass[rs.Length];
                            for (int i = 0; i < rs.Length; i++)
                            {
                                LocalDataSet.��Χ��Row r = (LocalDataSet.��Χ��Row)rs[i];
                                Yufa.FanWei[i] = new FanWeiClass();
                                Yufa.FanWei[i].YFBizoZhi = r.Ԥ����ʶ;
                                Yufa.FanWei[i].ReceiveBoxNO = r.�ռ�������;
                                Yufa.FanWei[i].Showmsg = r.�ݺ�;
                                Yufa.FanWei[i].Count = r.�ַ�����;
                                Yufa.FanWei[i].isDelete = r.�Ƿ�ɾ��;
                                if (r.Ͷ��״̬)
                                {
                                    Yufa.FanWei[i].IsTouxiang = r.Ͷ��״̬;
                                    Yufa.FanWei[i].SendTime = r.Ͷ��ʱ��.ToString("yyyy-MM-dd HH:mm:ss");
                                    Yufa.FanWei[i].SendCount = r.Ͷ�����;
                                }
                                if (r.�Ƿ���)
                                {
                                    Yufa.FanWei[i].IsKanWu = r.�Ƿ���;
                                    Yufa.FanWei[i].KanwuTime = r.����ʱ��.ToString("yyyy-MM-dd HH:mm:ss");
                                    Yufa.FanWei[i].KwAdminCard = r.�������Ա����;
                                    Yufa.FanWei[i].KwUserCard = r.���󽻻�Ա����;
                                }
                                if (r.ȡ��״̬)
                                {
                                    Yufa.FanWei[i].IsQuJian = r.ȡ��״̬;
                                    Yufa.FanWei[i].ReceiveTime = r.ȡ��ʱ��.ToString("yyyy-MM-dd HH:mm:ss");
                                    Yufa.FanWei[i].ReceiveUserCard = r.ȡ����֤������;
                                }
                            }//for
                        }
                        //ͬ�������������֪ͨ��ͷ��ʾͬ������
                        bool b = obj.SendYuFaInfo(ref Yufa);
                        if (b)
                        {
                            //���±�ʶ��
                            if (row.Ԥ����ʶ == "WW")
                                row.Ԥ����ʶ = Yufa.YFBizoZhi;
                            //����ͬ��״̬
                            row.�Ƿ�ͬ�� = true;
                            if (row.ȡ��״̬ || row.�Ƿ��� || row.�Ƿ�ɾ��)
                                m_DataSet.Ԥ����.RemoveԤ����Row(row);
                            else if (row.�������� != 1)
                            {
                                //���·�Χ
                                DataRow[] rs = m_DataSet.��Χ��.Select("Ԥ����ʶ='" + Yufa.YFBizoZhi + "'");
                                for (int i = 0; i < rs.Length; i++)
                                {
                                    LocalDataSet.��Χ��Row r = (LocalDataSet.��Χ��Row)rs[i];
                                    r.�Ƿ�ͬ�� = true;
                                    if (r.ȡ��״̬ || r.�Ƿ��� || r.�Ƿ�ɾ��)
                                        m_DataSet.��Χ��.Remove��Χ��Row(r);
                                }
                                rs = m_DataSet.��Χ��.Select("Ԥ����ʶ='" + Yufa.YFBizoZhi + "'");
                                if (rs.Length == 0)
                                {
                                    m_DataSet.Ԥ����.RemoveԤ����Row(row);
                                }
                            }
                        }
                    }//foreach
                }//lock
            }
            catch (Exception ee)
            {
                Log.WriteFileLog("ͬ������:" + ee.ToString());
                if (bConnected)
                {
                    bConnected = false;
                    //��ʾ������ͷ����Ͽ�
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

            #region ͬ��ȡ����Ϣ
            try
            {
                MonitorService.MonitorService obj = GetConnectionObj("UserGetLetter");
                lock (m_DataSet.�û�ȡ����)
                {
                    for (int ii = m_DataSet.�û�ȡ����.Rows.Count - 1; ii >= 0; ii--)
                    {
                        LocalDataSet.�û�ȡ����Row row = (LocalDataSet.�û�ȡ����Row)m_DataSet.�û�ȡ����.Rows[ii];
                        if (row.�Ƿ�ͬ��)
                        {
                            m_DataSet.�û�ȡ����.Remove�û�ȡ����Row(row);
                            continue;
                        }

                        if (Constant.UserGetLetterType == 0)
                        {
                            #region �Ϸ�ʽͬ��
                            if (row.ͼƬ�����Ƿ��Ѿ�����)
                            {
                                //�жϴ���
                                row.ͬ������++;
                                bool b = obj.UserGetLetter(row.�߼����, row.�û�����, row.ȡ��ʱ��, row.��Ƭ����, row.�Ƿ��ӡ�����嵥, row.�Ƿ��ӡȡ���嵥);
                                if (b)
                                {
                                    row.�Ƿ�ͬ�� = true;
                                    m_DataSet.�û�ȡ����.Remove�û�ȡ����Row(row);
                                }
                            }
                            #endregion
                        }
                        else if (Constant.UserGetLetterType == 1)
                        {
                            //�µ�3��ͬ����ʽ
                            #region �ȵ�һ��ͬ������ȡ����¼id
                            if (row.ȡ����¼ID == 0)
                            {
                                row.ͬ������++;
                                int id = obj.UserGetLetter_Record(row.�߼����, row.�û�����, row.ȡ��ʱ��);
                                if (id > 0)
                                {
                                    row.ȡ����¼ID = id;
                                    //id��ȡ��ֱ�Ӵ�ӡ�嵥
                                    try
                                    {
                                        UserGetLetterReturnClass ret = obj.UserGetLetter_Report(id, row.ȡ������������, row.�Ƿ��ӡ�����嵥, row.�Ƿ��ӡȡ���嵥);
                                        if (ret.bSuccess)
                                        {
                                            if (ret.bPrintSendReport || ret.bPrintRecvReport)
                                            {
                                                if (!row.Is�嵥��ӡ��Null() && row.�嵥��ӡ��.Trim() != "")
                                                {
                                                    PrintData data = new PrintData(ret, row.�嵥��ӡ��);
                                                    //��ӡ�嵥
                                                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ThreadPrintReport), data);
                                                }
                                                else
                                                {
                                                    Log.WriteInfo(LogType.Error, "�û�ȡ����ӡ�嵥�����嵥��ӡ��û�����á�");
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.WriteInfo(LogType.Error, "�û�ȡ������" + ex.ToString());
                                    }
                                }
                            }
                            #endregion

                            #region ͬ��ȡ����Ƭ
                            if (row.ȡ����¼ID > 0 && row.ͼƬ�����Ƿ��Ѿ�����)
                            {
                                bool b = obj.UserGetLetter_SetPic(row.ȡ����¼ID, row.��Ƭ����);
                                if (b)
                                {
                                    row.�Ƿ�ͬ�� = true;
                                    m_DataSet.�û�ȡ����.Remove�û�ȡ����Row(row);
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Log.WriteFileLog("ͬ��ȡ����Ϣ:" + ee.ToString());
            }
            #endregion
        }
        #endregion

        #region ��ӡ�嵥
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
                    Log.WriteFileLog("׼����ӡ�����嵥��ģ�壺" + data.data.szSendTemplateFileName + "�����ݣ�" + data.data.szSendXmlData + "\r\n\r\n");
                    int tmp = System.Threading.Interlocked.Increment(ref tmpFileIndex);
                    string bartemplate = System.IO.Directory.GetCurrentDirectory() + "\\barDll\\" + data.data.szSendTemplateFileName;
                    string tmpfile = System.IO.Directory.GetCurrentDirectory() + "\\barDll\\" + tmp.ToString() + ".xml";
                    byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(data.data.szSendXmlData);
                    System.IO.File.WriteAllBytes(tmpfile, buf);
                    if (!System.IO.File.Exists(bartemplate))
                    {
                        Log.WriteFileLog("��ӡ�����嵥��û���ҵ������嵥ģ�塣\r\n\r\n");
                    }
                    else if (!System.IO.File.Exists(tmpfile))
                    {
                        Log.WriteFileLog("��ӡ�����嵥����ʱ�����ļ��������\r\n\r\n");
                    }
                    else
                    {
                        bool b = Win32API.PrintTempletBarXML(bartemplate, tmpfile, data.PrinterName, ref pError, msg);
                        if (!b)
                        {
                            Log.WriteInfo(LogType.Error, "��ӡ�嵥�����ش�ӡʧ�ܡ�\r\n\r\n");
                        }
                        Log.WriteFileLog("��ӡ�����嵥�������" + b.ToString() + "\r\n\r\n");
                    }
                    tmp1 = tmpfile;
                }
                if (data.data.bPrintRecvReport)
                {
                    Log.WriteFileLog("׼����ӡ�ռ��嵥��ģ�壺" + data.data.szRecvTemplateFileName + "�����ݣ�" + data.data.szRecvXmlData + "\r\n\r\n");
                    int tmp = System.Threading.Interlocked.Increment(ref tmpFileIndex);
                    string bartemplate = System.IO.Directory.GetCurrentDirectory() + "\\barDll\\" + data.data.szRecvTemplateFileName;
                    string tmpfile = System.IO.Directory.GetCurrentDirectory() + "\\barDll\\" + tmp.ToString() + ".xml";
                    System.IO.File.WriteAllText(tmpfile, data.data.szRecvXmlData, System.Text.Encoding.GetEncoding(936));
                    if (!System.IO.File.Exists(bartemplate))
                    {
                        Log.WriteFileLog("��ӡ�ռ��嵥��û���ҵ��ռ��嵥ģ�塣\r\n\r\n");
                    }
                    else if (!System.IO.File.Exists(tmpfile))
                    {
                        Log.WriteFileLog("��ӡ�ռ��嵥����ʱ�����ļ��������\r\n\r\n");
                    }
                    else
                    {
                        bool b = Win32API.PrintTempletBarXML(bartemplate, tmpfile, data.PrinterName, ref pError, msg);
                        if (!b)
                            Log.WriteInfo(LogType.Error, "��ӡ�嵥�����ش�ӡʧ�ܡ�\r\n\r\n");
                        Log.WriteFileLog("��ӡ�ռ��嵥�������" + b.ToString() + "\r\n\r\n");
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
                Log.WriteFileLog("��ӡ�嵥��" + ex.ToString());
            }
        }
        #endregion

        #region ��ӡ�����嵥
        /// <summary>
        /// ��ӡ�����嵥
        /// </summary>
        /// <param name="UserCardIdID">���룬��ӡ�嵥�Ĺ���Ա��֤�����</param>
        /// <param name="�嵥��ӡ��">���룬�嵥��ӡ��</param>
        /// <returns>�����嵥����</returns>
        public static bool PrintUserReturnLetter(string UserCardId, string �嵥��ӡ��)
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
                        if (�嵥��ӡ��.Trim() != "")
                        {
                            PrintData data = new PrintData(ret, �嵥��ӡ��);
                            //��ӡ�嵥
                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ThreadPrintReport), data);
                        }
                        else
                        {
                            Log.WriteInfo(LogType.Error, "�û�ȡ����ӡ�嵥�����嵥��ӡ��û�����á�");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteFileLog("��ӡ�����嵥��" + ex.ToString());
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
