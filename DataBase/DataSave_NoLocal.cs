using System;
using System.Collections.Generic;
using System.Data;

namespace DataBase
{
	/// <summary>
	/// Class1 ��ժҪ˵����
	/// </summary>
	public class DataSave_NoLocal
    {
		private static LogInfo.DataSetBox m_DataSetBox = new LogInfo.DataSetBox();

		#region ��ʼ������
		static DataSave_NoLocal()
		{
			try
			{
				m_DataSetBox.ReadXml(LogInfo.Constant.BoxConfigFileName);
			}
			catch (Exception ex)
			{
				LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "���������Ϣ��ʼ������" + ex.ToString());
			}
		}
		#endregion

        #region ������Ӧ����
        private static MonitorService_NoLocal.MonitorService_NoLocal GetConnectionObj()
        {
            MonitorService_NoLocal.MonitorService_NoLocal obj = new DataBase.MonitorService_NoLocal.MonitorService_NoLocal();
            obj.Url = LogInfo.Constant.ServiceURL_NoLocal;
            obj.Timeout = 3 * 60 * 1000;
            return obj;
        }
        #endregion

		#region ��ȡ���������Ϣ
		/// <summary>
		/// ��ȡ���������Ϣ
		/// </summary>
		/// <returns></returns>
		public static LogInfo.DataSetBox DataSetBox
		{
			get { return (LogInfo.DataSetBox)m_DataSetBox.Copy(); }
		}
		#endregion

        #region ������������͡�������ż����룬ͬʱ���ؿ��Խ��յ����Ӻ���
        /// <summary>
        /// ������������͡�������ż����룬ͬʱ���ؿ��Խ��յ����Ӻ���
        /// </summary>
        /// <param name="BarCode">���룬ɨ������</param>
		/// <param name="boxs">�������ͷ������ռ��������磺21:1,22:3....</param>
        /// <returns>��������</returns>
		public static BarCodeType CheckBarCodeType(string BarCode, List<SendBoxList> boxs)
        {
            DateTime dtStart = DateTime.Now;
            string LogMessage = "������룺" + BarCode + "\r\n";

            BarCodeType rType = BarCodeType.��Ч;
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

				string BoxNOs = "";
				int iRet = obj.CheckBarCodeType(BarCode, ref BoxNOs);
				if (iRet == 0)
					rType = BarCodeType.��Ч;
				else if (iRet == 1)
				{
					rType = BarCodeType.ΨһֱͶ;
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
							rType = BarCodeType.Ψһָ��;
						else if (boxs.Count>1)
							rType = BarCodeType.ͨ��ַ�;
					}
				}


                LogMessage += "���ؼ�������������ͣ�" + rType.ToString() + "\r\n";
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
                LogMessage += "�쳣�����ݣ�" + ee.ToString() + "\r\n";
            }

            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "��ʱ " + ts.ToString() + " ��\r\n" + LogMessage + "\r\n\r\n";
            LogInfo.Log.WriteFileLog(LogMessage);

            return rType;
        }
        #endregion

        #region ���֤������
        /// <summary>
        /// ���֤������
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="BoxNOs">�������ͷ���룬�磺1,2....</param>
        /// <returns></returns>
		public static BarCodeType CheckCardType(string UserCode, ref string BoxNOs, out string UnitName, out string UserName)
        {
            DateTime dtStart = DateTime.Now;
            string LogMessage = "���֤�����룺" + UserCode + "\r\n";

            BarCodeType rType = BarCodeType.��Ч;
            BoxNOs = "";
			UnitName = ""; UserName = "";
            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

				int i = obj.CheckCardType(UserCode, ref BoxNOs, out UnitName, out UserName);
				//�������͡�0����Ч�� 1������ά��  2������Ա��3������Ա
				if (i == 2)
					rType = BarCodeType.����Ա;
				else if (i == 3)
					rType = BarCodeType.����Ա;

                LogMessage += "���ؼ�������������ͣ�" + rType.ToString() + "\r\n";
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
                LogMessage += "�쳣�����ݣ�" + ee.ToString() + "\r\n";
            }

            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "��ʱ " + ts.ToString() + " ��\r\n" + LogMessage + "\r\n\r\n";
            LogInfo.Log.WriteFileLog(LogMessage);

            return rType;
        }
        #endregion


        #region �õ��߼����ӵ��ż���Ŀ�ͼ�������
        /// <summary>
        /// �õ��߼����ӵ��ż���Ŀ�ͼ�������
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

        #region ��ѯ�߼���ͷ�������Ϣ
        /// <summary>
        /// ��ѯ�߼���ͷ�������Ϣ
        /// ���ݺͽṹֱ��д���xml�ַ��������ص��������
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
                    //������ֱ��д���xml�ַ��������ص��������
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

        #region ��ѯ��λ�б�
        /// <summary>
        /// ��ѯ��λ�б�ÿ����λ��\n�ָ�����������\r�ָ�
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
                        strRet += Rs["��λ��ʶ"].ToString();
                        strRet += "\r";
                        strRet += Rs["��λ����"].ToString().Trim();
                        strRet += "\r";
                        strRet += Rs["��λ����"].ToString().Trim();
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


		#region ��ѯ���ݿ⣬�ж��û��Ƿ���Ե�¼
        /// <summary>
        /// ��ѯ���ݿ⣬�ж��û��Ƿ���Ե�¼
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

		#region ��ѯ���ݿ⣬��ѯ�û���������Ϣ
		/// <summary>
		/// ��ѯ���ݿ⣬��ѯ�û���������Ϣ
		/// ÿ��Box��\n�ָ����������\r�ָ�
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
                        try { BoxNo = Convert.ToInt32(Rs["�߼����"]).ToString("000"); }
                        catch { continue; }
                        OrgNo = Rs["��λ��ʶ"].ToString();
                        try { OrgShort = Rs["��λ����"].ToString(); }
                        catch { continue; }

                        try { urg = Convert.ToInt32(Rs["����"]); }
                        catch { continue; }
                        try { SumCopy = Convert.ToInt32(Rs["����"]); }
                        catch { continue; }

                        strRet += BoxNo + "\r";
                        strRet += OrgShort + "\r";
                        strRet += OrgNo + "\r";
                        strRet += SumCopy.ToString() + "\r";
                        strRet += Rs["��������"].ToString() + "\n";
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

		#region ������ͷ��λ
		/// <summary>
		/// ������ͷ��λ
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

		#region �����ż�
		/// <summary>
		/// �����ż�
		/// </summary>
		/// <param name="LetterCode"></param>
		/// <param name="BoxNO"></param>
		/// <param name="bUragent"></param>
		/// <returns>Ͷ���ż��ļ�������</returns>
        public static int SaveLetter(string LetterCode, int BoxNO, int SendCount, bool bUragent)
        {
            DateTime dtStart = DateTime.Now;
            string LogMessage = "�����ż���" + LetterCode + "\r\n";

            int bRet = -1;

            try
            {
                MonitorService_NoLocal.MonitorService_NoLocal obj = GetConnectionObj();

				bRet = obj.SaveLetter(LetterCode, BoxNO, SendCount, bUragent);

                LogMessage += "���ر����ż������" + bRet.ToString() + "\r\n";
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
                LogMessage += "�쳣�����ݣ�" + ee.ToString() + "\r\n";
            }

            TimeSpan ts = DateTime.Now - dtStart;
            LogMessage = "��ʱ " + ts.ToString() + " ��\r\n" + LogMessage + "\r\n\r\n";
            LogInfo.Log.WriteFileLog(LogMessage);

            return bRet;
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


        #region ����Ա����ɨ��ȡ��
        /// <summary>
        /// ����Ա����ɨ��ȡ��
        /// </summary>
        /// <param name="BoxID">�߼����</param>
        /// <param name="UserBarCode"></param>
        /// <param name="Alarm">�Ƿ񾯸�</param>
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


        #region �쵼������Ϣ
        /// <summary>
        /// �쵼������Ϣ����źͳ�����Ϣ��2ά����
        /// </summary>
        /// <returns>��źͳ�����Ϣ��2ά����</returns>
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
