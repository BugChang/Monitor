using System;
using System.Collections.Generic;
using System.Data;

namespace DataBase
{
	/// <summary>
	/// Class1 的摘要说明。
	/// </summary>
	public class DataSave_NoLocal
    {
		private static LogInfo.DataSetBox m_DataSetBox = new LogInfo.DataSetBox();

		#region 初始化函数
		static DataSave_NoLocal()
		{
			try
			{
				m_DataSetBox.ReadXml(LogInfo.Constant.BoxConfigFileName);
			}
			catch (Exception ex)
			{
				LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "箱格配置信息初始化出错：" + ex.ToString());
			}
		}
		#endregion

        #region 创建对应的类
        private static MonitorService_NoLocal.MonitorService_NoLocal GetConnectionObj()
        {
            MonitorService_NoLocal.MonitorService_NoLocal obj = new DataBase.MonitorService_NoLocal.MonitorService_NoLocal();
            obj.Url = LogInfo.Constant.ServiceURL_NoLocal;
            obj.Timeout = 3 * 60 * 1000;
            return obj;
        }
        #endregion

		#region 获取箱格配置信息
		/// <summary>
		/// 获取箱格配置信息
		/// </summary>
		/// <returns></returns>
		public static LogInfo.DataSetBox DataSetBox
		{
			get { return (LogInfo.DataSetBox)m_DataSetBox.Copy(); }
		}
		#endregion

        #region 返回条码的类型。如果是信件条码，同时返回可以接收的箱子号码
        /// <summary>
        /// 返回条码的类型。如果是信件条码，同时返回可以接收的箱子号码
        /// </summary>
        /// <param name="BarCode">输入，扫描条码</param>
		/// <param name="boxs">输出，箱头号码和收件数量，如：21:1,22:3....</param>
        /// <returns>条码类型</returns>
		public static BarCodeType CheckBarCodeType(string BarCode, List<SendBoxList> boxs)
        {
            DateTime dtStart = DateTime.Now;
            string LogMessage = "检查条码：" + BarCode + "\r\n";

            BarCodeType rType = BarCodeType.无效;
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

				string BoxNOs = "";
				int iRet = obj.CheckBarCodeType(BarCode, ref BoxNOs);
				if (iRet == 0)
					rType = BarCodeType.无效;
				else if (iRet == 1)
				{
					rType = BarCodeType.唯一直投;
					if (BoxNOs!="")
					{
						string[] ar1 = BoxNOs.Split(',');
						foreach (string s in ar1)
						{
							if (s.Trim() == "")
								continue;
							string[] ar2 = s.Split(':');
							if (ar2.Length != 2) continue;
							try
							{
								SendBoxList sl = new SendBoxList();
								sl.BoxNo = Convert.ToInt32(ar2[0]);
								sl.Count = Convert.ToInt32(ar2[1]);
								boxs.Add(sl);
							}
							catch
							{
							}
						}
						if (boxs.Count == 1)
							rType = BarCodeType.唯一指定;
						else if (boxs.Count>1)
							rType = BarCodeType.通码分发;
					}
				}


                LogMessage += "返回检查结果，条码类型：" + rType.ToString() + "\r\n";
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
                LogMessage += "异常，内容：" + ee.ToString() + "\r\n";
            }

            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "耗时 " + ts.ToString() + " 。\r\n" + LogMessage + "\r\n\r\n";
            LogInfo.Log.WriteFileLog(LogMessage);

            return rType;
        }
        #endregion

        #region 检查证卡条码
        /// <summary>
        /// 检查证卡条码
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="BoxNOs">输出，箱头号码，如：1,2....</param>
        /// <returns></returns>
		public static BarCodeType CheckCardType(string UserCode, ref string BoxNOs, out string UnitName, out string UserName)
        {
            DateTime dtStart = DateTime.Now;
            string LogMessage = "检查证卡条码：" + UserCode + "\r\n";

            BarCodeType rType = BarCodeType.无效;
            BoxNOs = "";
			UnitName = ""; UserName = "";
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

				int i = obj.CheckCardType(UserCode, ref BoxNOs, out UnitName, out UserName);
				//条码类型。0：无效， 1：厂方维修  2：管理员，3：交换员
				if (i == 2)
					rType = BarCodeType.管理员;
				else if (i == 3)
					rType = BarCodeType.交换员;

                LogMessage += "返回检查结果，条码类型：" + rType.ToString() + "\r\n";
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
                LogMessage += "异常，内容：" + ee.ToString() + "\r\n";
            }

            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "耗时 " + ts.ToString() + " 。\r\n" + LogMessage + "\r\n\r\n";
            LogInfo.Log.WriteFileLog(LogMessage);

            return rType;
        }
        #endregion


        #region 得到逻辑箱子的信件数目和急件属性
        /// <summary>
        /// 得到逻辑箱子的信件数目和急件属性
        /// </summary>
        /// <param name="BoxId"></param>
        /// <param name="LetterCount"></param>
        /// <param name="pri"></param>
        public static MonitorService_NoLocal.BoxInfo GetBoxLetterCount(int BoxId)
        {
            MonitorService_NoLocal.BoxInfo t;

            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

                t = obj.GetBoxLetterCount(BoxId);


            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
                t = null;
            }

            return t;
        }
        #endregion

        #region 查询逻辑箱头的箱存信息
        /// <summary>
        /// 查询逻辑箱头的箱存信息
        /// 数据和结构直接写入成xml字符串，返回到界面后处理
        /// </summary>
        /// <param name="BoxNO"></param>
        /// <returns></returns>
        public static string BoxLetterView(string BoxNO)
        {
            string strRet = "";
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

                DataSet ds = obj.BoxLetterView(BoxNO);
                if (ds.Tables.Count == 2)
                {
                    //把数据直接写入成xml字符串，返回到界面后处理
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    ds.WriteXml(ms);
                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    byte[] buf = new byte[ms.Length];
                    ms.Read(buf, 0, buf.Length);
                    ms.Close();
                    strRet = System.Text.Encoding.UTF8.GetString(buf);
                }
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
                strRet = "";
            }

            return strRet;
        }
        #endregion

        #region 查询单位列表
        /// <summary>
        /// 查询单位列表，每个单位用\n分割，号码和名称用\r分割
        /// </summary>
        /// <returns></returns>
        public static string UnitList()
        {
            string strRet = "";
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

                DataSet ds = obj.UnitList();
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow Rs = ds.Tables[0].Rows[i];
                        strRet += Rs["单位标识"].ToString();
                        strRet += "\r";
                        strRet += Rs["单位代码"].ToString().Trim();
                        strRet += "\r";
                        strRet += Rs["单位名称"].ToString().Trim();
                        strRet += "\n";
                    }
                }
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
            }

            return strRet;
        }
        #endregion


		#region 查询数据库，判断用户是否可以登录
        /// <summary>
        /// 查询数据库，判断用户是否可以登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="pwd"></param>
        public static bool CheckLogin(string UserName, string pwd)
        {
            bool bRet = false;
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

                bRet = obj.CheckLogin(UserName, pwd);
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
            }

            return bRet;
        }
        #endregion

		#region 查询数据库，查询用户的箱子信息
		/// <summary>
		/// 查询数据库，查询用户的箱子信息
		/// 每个Box用\n分割，各个属性用\r分割
		/// </summary>
		/// <param name="UserName"></param>
        public static string UserBoxInfo(string UserName)
        {
            string strRet = "";
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

                DataSet ds = obj.UserBoxInfo(UserName);
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow Rs = ds.Tables[0].Rows[i];

                        string BoxNo, OrgNo, OrgShort;
                        int SumCopy = 0, urg = 0;
                        try { BoxNo = Convert.ToInt32(Rs["逻辑箱号"]).ToString("000"); }
                        catch { continue; }
                        OrgNo = Rs["单位标识"].ToString();
                        try { OrgShort = Rs["单位名称"].ToString(); }
                        catch { continue; }

                        try { urg = Convert.ToInt32(Rs["缓急"]); }
                        catch { continue; }
                        try { SumCopy = Convert.ToInt32(Rs["数量"]); }
                        catch { continue; }

                        strRet += BoxNo + "\r";
                        strRet += OrgShort + "\r";
                        strRet += OrgNo + "\r";
                        strRet += SumCopy.ToString() + "\r";
                        strRet += Rs["控制类型"].ToString() + "\n";
                    }
                }
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
            }

            return strRet;
        }
        #endregion

		#region 设置箱头单位
		/// <summary>
		/// 设置箱头单位
		/// </summary>
		/// <param name="BoxNO"></param>
		/// <param name="UnitNO"></param>
		/// <returns></returns>
        public static bool Box_UnitChange(string BoxNO, string UnitBH)
        {
            bool bRet = false;
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

                int BoxId = Convert.ToInt32(BoxNO);
                bRet = obj.Box_UnitChange(BoxId, UnitBH);
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
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
        public static int SaveLetter(string LetterCode, int BoxNO, int SendCount, bool bUragent)
        {
            DateTime dtStart = DateTime.Now;
            string LogMessage = "保存信件：" + LetterCode + "\r\n";

            int bRet = -1;

            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

				bRet = obj.SaveLetter(LetterCode, BoxNO, SendCount, bUragent);

                LogMessage += "返回保存信件结果：" + bRet.ToString() + "\r\n";
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
                LogMessage += "异常，内容：" + ee.ToString() + "\r\n";
            }

            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "耗时 " + ts.ToString() + " 。\r\n" + LogMessage + "\r\n\r\n";
            LogInfo.Log.WriteFileLog(LogMessage);

            return bRet;
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
            int iRet = 0;
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

				iRet = obj.SaveErratumLetter(BoxNO, LetterCode, AdminBarCode, UserBarCode);
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
            }

            return iRet!=-1;
        }
        #endregion


        #region 交换员箱组扫卡取件
        /// <summary>
        /// 交换员箱组扫卡取件
        /// </summary>
        /// <param name="BoxID">逻辑箱号</param>
        /// <param name="UserBarCode"></param>
        /// <param name="Alarm">是否警告</param>
        /// <returns></returns>
        public static bool Box_UserGetLetter(int BoxID, string UserBarCode, bool bSendReport, bool bRecvReport)
        {
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

                return obj.Box_UserGetLetter(BoxID, UserBarCode, bSendReport, bRecvReport);
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
            }

            return false;
        }
		#endregion


        #region 领导出差信息
        /// <summary>
        /// 领导出差信息，箱号和出差信息用2维数组
        /// </summary>
        /// <returns>箱号和出差信息用2维数组</returns>
        public static MonitorService_NoLocal.ClassBoxShowMessage[] LeaderOutMessage()
        {
			MonitorService_NoLocal.ClassBoxShowMessage[] strRet = null;
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

                return obj.GetLeaderOutMessage();
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
				strRet = new DataBase.MonitorService_NoLocal.ClassBoxShowMessage[0];
            }

            return strRet;
        }
        #endregion

	}

}
