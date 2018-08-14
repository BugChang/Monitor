using System;

namespace LogInfo
{
	/// <summary>
	/// Constant ��ժҪ˵����
	/// </summary>
	public class Constant
	{
        public const string XMLString_Far = "<?xml version=\"1.0\" encoding=\"gb2312\"?>\r\n<farShine ver=\"1.0\" xmlns=\"http://www.farshine.com\">\r\n<farShine ver=\"1.0\">\r\n</farShine>\r\n</farShine>";
        public const string XMLNameSpace_Far = "http://www.farshine.com";

        public const string MsgConfigFileName = "MsgConfig.xml";
        public const string BoxConfigFileName = "BoxConfig.xml";
        private const string AppConfigFileName = "config.xml";
        public const string LogFileName = "Monitor.log";

		public const ushort ClientListenPort		= 56789;
		public const ushort ClientOtherListenPort	= 56788;
        public const ushort ClientNewsListenPort    = 56787;

        /// <summary>
        /// �����Ƿ�������
        /// </summary>
        public static bool b_AppRunning = false;
        public static bool IsDebug = false;

        //����Service
        private static string m_ServiceURL = "http://localhost/MonitorService/MonitorService.asmx";
        private static string m_ServiceURL_NoLocal = "http://localhost/MonitorService/MonitorService_NoLocal.asmx";

        //AppConfig
        private static int m_DistributeType = 0;// 0:�Զ����ţ�1����������
        private static int m_NewspaperType = 0;// ��ֽ�ַ���0:����ǰ���ȫ��Ͷ�룬1:������ǰ��Ͷ��
        private static bool m_OpenDoorType = true;// ȡ����ʽ��������Ϊ0�Ƿ�����
        public static bool m_IsPiaoJuXiang = false;// �Ƿ���Ʊ����
		private static int m_QuJianType = 1;// ȡ��ƽ̨��ʾ�����ͣ�1Ϊ3��ť��2Ϊ���İ�ť��3Ϊ���İ�ť��4�µ��尴ťȡ������
		private static string m_EmptyBoxName = "����";// û�з��䵥λ�����ӵĵ�λ����

		//Pc104����
        public static int m_TextLeft = 0;	//������ʾλ�����
        public static int m_TextTop = 16;	//������ʾλ�ö���

		//ϵͳ����
        public static int  ThreadCount = 20;	        //ϵͳ�����߳���Ŀ
		private static int m_CanType = 4;	        //can�������ͣ�
		private static int m_CanCount = 2;	        //ϵͳ�����߳���Ŀ
		private static string m_CanNetIP = "192.168.0.234";	        //
		private static int m_CanNetPort = 4001;	        //
		public static bool m_NotifyGroupName = true;	//Ͷ��ʱ����ʾ�ڼ��黹����ʾҪͶ��ĵ�λ�����ƣ�True���ڼ��飬False����λ����
        public static int m_SysRebootTime = 3;	        //ϵͳÿ������������ʱ�䡣��ʽ��Сʱ���磺3����˼��ÿ���賿3������
		//��ʱʱ������
        public static double m_ConnectTimeOut   = 60;    //5��
        public static double m_TimeOut_Sendletter = 15;
        public static double m_TimeOut_CloseDoor = 30;
        public static double m_TimeOut_PutInLetter = 5;
		public static double m_TimeOut_ShowInfoMsg = 3;	//��ͷ��ʾ��ʾ��Ϣ���ص�ԭʼ״̬��ʱ�䣬3��
		public static int m_TimeOut_CloseScreen = 300;	//�ر���ͷ��ʾ��5����

        //SCDCC����
        public static string m_SCDCC_IP = "192.168.2.200";
        public static int    m_SCDCC_Port = 80;

        //Ӳ��¼���������ַ����
        public static string m_DVRServer_IP = "127.0.0.1";
        public static int m_DVRServer_Port = 50000;

		/// <summary>
		/// �Ƿ�ʹ�ñ�������
		/// </summary>
		private static bool m_UserLocalData = true;
		/// <summary>
		/// �Ƿ�ʹ�ñ�����������ļ�
		/// </summary>
		private static bool m_UserLocalBoxConfig = false;

		/// <summary>
		/// ȡ������ͬ����ʽ��0����UserGetLetter��ʽ��1�����µ�3��ͬ����ʽ
		/// </summary>
		private static int m_UserGetLetterType = 0;

        #region ���ݿ�����
        public static string ServiceURL
        {
            get { return m_ServiceURL; }
            set
            {
                if (m_ServiceURL == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("DataBase");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "DataBase", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("ServiceURL");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "ServiceURL", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value;

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_ServiceURL = value;
            }
        }
        public static string ServiceURL_NoLocal
        {
            get { return m_ServiceURL_NoLocal; }
            set
            {
                if (m_ServiceURL_NoLocal == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("DataBase");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "DataBase", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("ServiceURL_NoLocal");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "ServiceURL_NoLocal", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value;

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_ServiceURL_NoLocal = value;
            }
        }
        #endregion

		#region �ַ���ʽ
		/// <summary>
		/// 0:�Զ����ţ�1����������
		/// </summary>
		public static int DistributeType
		{
			get{return m_DistributeType;}
			set{
				if(m_DistributeType==value)
					return;

				//���������ݿ��ļ�
				System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
				xDu.Load(AppConfigFileName);
				System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("AppConfig");
				if(aNode==null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "AppConfig", "");
					xDu.DocumentElement.AppendChild(aNode);
				}
				System.Xml.XmlNode bNode = aNode.SelectSingleNode("DistributeType");
				if(bNode==null)
				{
					bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "DistributeType", "");
					aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
				}

				bNode.Attributes["value"].Value = value.ToString();

				xDu.Save(AppConfigFileName);

				//���õ�ǰ����
				m_DistributeType = value;
			}
		}
		#endregion
        #region ��ֽ�ַ���ʽ
        /// <summary>
        /// ��ֽ�ַ���0:����ǰ���ȫ��Ͷ�룬1:������ǰ��Ͷ��
        /// </summary>
        public static int NewspaperType
        {
            get { return m_NewspaperType; }
            set
            {
                if (m_NewspaperType == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("AppConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "AppConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("NewspaperType");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "NewspaperType", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_NewspaperType = value;
            }
        }
        #endregion
        #region ȡ����ʽ��������Ϊ0�Ƿ�����
        /// <summary>
        /// ȡ����ʽ��������Ϊ0�Ƿ�����
        /// </summary>
        public static bool OpenDoorType
        {
            get { return m_OpenDoorType; }
            set
            {
                if (m_OpenDoorType == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("AppConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "AppConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("OpenDoorType");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "OpenDoorType", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_OpenDoorType = value;
            }
        }
        #endregion

		#region ȡ��ƽ̨��ʾ�����ͣ�1Ϊ3��ť��2Ϊ���İ�ť��3Ϊ���İ�ť��4�µ��尴ťȡ������
		/// <summary>
		/// ȡ��ƽ̨��ʾ�����ͣ�1Ϊ3��ť��2Ϊ���İ�ť��3Ϊ���İ�ť��4�µ��尴ťȡ������
		/// </summary>
		public static int QuJianType
		{
			get { return m_QuJianType; }
			set
			{
				if (m_QuJianType == value)
					return;

				//���������ݿ��ļ�
				System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
				xDu.Load(AppConfigFileName);
				System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("AppConfig");
				if (aNode == null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "AppConfig", "");
					xDu.DocumentElement.AppendChild(aNode);
				}
				System.Xml.XmlNode bNode = aNode.SelectSingleNode("QuJianType");
				if (bNode == null)
				{
					bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "QuJianType", "");
					aNode.AppendChild(bNode);
					System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
					bNode.Attributes.Append(a);
				}

				bNode.Attributes["value"].Value = value.ToString();

				xDu.Save(AppConfigFileName);

				//���õ�ǰ����
				m_QuJianType = value;
			}
		}
		#endregion

		#region û�з��䵥λ�����ӵĵ�λ����
		/// <summary>
		/// û�з��䵥λ�����ӵĵ�λ����
		/// </summary>
		public static string EmptyBoxName
		{
			get { return m_EmptyBoxName; }
			set
			{
				if (m_EmptyBoxName == value)
					return;

				//���������ݿ��ļ�
				System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
				xDu.Load(AppConfigFileName);
				System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("AppConfig");
				if (aNode == null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "AppConfig", "");
					xDu.DocumentElement.AppendChild(aNode);
				}
				System.Xml.XmlNode bNode = aNode.SelectSingleNode("EmptyBoxName");
				if (bNode == null)
				{
					bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "EmptyBoxName", "");
					aNode.AppendChild(bNode);
					System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
					bNode.Attributes.Append(a);
				}

				bNode.Attributes["value"].Value = value.ToString();

				xDu.Save(AppConfigFileName);

				//���õ�ǰ����
				m_EmptyBoxName = value;
			}
		}
		#endregion


        #region ������ʾλ�����
        /// <summary>
        /// ������ʾλ����� 0~255
        /// </summary>
        public static int TextLeft
        {
            get { return m_TextLeft; }
            set
            {
                if (m_TextLeft == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("Pc104Config");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Pc104Config", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("TextLeft");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "TextLeft", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_TextLeft = value;
            }
        }
        #endregion
        #region ������ʾλ�ö���
        /// <summary>
        /// ������ʾλ����� 0~255
        /// </summary>
        public static int TextTop
        {
            get { return m_TextTop; }
            set
            {
                if (m_TextTop == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("Pc104Config");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Pc104Config", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("TextTop");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "TextTop", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_TextTop = value;
            }
        }
        #endregion

        #region Ͷ��ʱ����ʾ�ڼ��黹����ʾҪͶ��ĵ�λ�����ƣ�True���ڼ��飬False����λ����
        /// <summary>
        /// Ͷ��ʱ����ʾ�ڼ��黹����ʾҪͶ��ĵ�λ�����ƣ�True���ڼ��飬False����λ����
        /// </summary>
        public static bool NotifyGroupName
        {
            get { return m_NotifyGroupName; }
            set
            {
                if (m_NotifyGroupName == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("NotifyGroupName");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "NotifyGroupName", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString(); ;

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_NotifyGroupName = value;
            }
        }
        #endregion
        #region ϵͳÿ������������ʱ�䡣��ʽ��Сʱ���磺3����˼��ÿ���賿3������
        /// <summary>
        /// ϵͳÿ������������ʱ�䡣��ʽ��Сʱ���磺3����˼��ÿ���賿3������
        /// </summary>
        public static int SysRebootTime
        {
            get { return m_SysRebootTime; }
            set
            {
                if (m_SysRebootTime == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("SysRebootTime");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SysRebootTime", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_SysRebootTime = value;
            }
        }
        #endregion

		#region Can������
		/// <summary>
		/// Can������
		/// </summary>
		public static int CanType
		{
			get { return m_CanType; }
			set
			{
				if (m_CanType == value)
					return;

				//���������ݿ��ļ�
				System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
				xDu.Load(AppConfigFileName);
				System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
				if (aNode == null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
					xDu.DocumentElement.AppendChild(aNode);
				}
				System.Xml.XmlNode bNode = aNode.SelectSingleNode("CanType");
				if (bNode == null)
				{
					bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "CanType", "");
					aNode.AppendChild(bNode);
					System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
					bNode.Attributes.Append(a);
				}

				bNode.Attributes["value"].Value = value.ToString();

				xDu.Save(AppConfigFileName);

				//���õ�ǰ����
				m_CanType = value;
			}
		}
		#endregion
		#region Can��·��
        /// <summary>
        /// Can��·��
        /// </summary>
        public static int CanCount
        {
            get { return m_CanCount; }
            set
            {
                if (m_CanCount == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("CanCount");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "CanCount", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_CanCount = value;
            }
        }
        #endregion
		#region CanNet IP
		/// <summary>
		/// CanNet IP
		/// </summary>
		public static string CanNetIP
		{
			get { return m_CanNetIP; }
			set
			{
				if (m_CanNetIP == value)
					return;

				//���������ݿ��ļ�
				System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
				xDu.Load(AppConfigFileName);
				System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
				if (aNode == null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
					xDu.DocumentElement.AppendChild(aNode);
				}
				System.Xml.XmlNode bNode = aNode.SelectSingleNode("CanNetIP");
				if (bNode == null)
				{
					bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "CanNetIP", "");
					aNode.AppendChild(bNode);
					System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
					bNode.Attributes.Append(a);
				}

				bNode.Attributes["value"].Value = value.ToString();

				xDu.Save(AppConfigFileName);

				//���õ�ǰ����
				m_CanNetIP = value;
			}
		}
		#endregion
		#region CanNet Port
		/// <summary>
		///  CanNet Port
		/// </summary>
		public static int CanNetPort
		{
			get { return m_CanNetPort; }
			set
			{
				if (m_CanNetPort == value)
					return;

				//���������ݿ��ļ�
				System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
				xDu.Load(AppConfigFileName);
				System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
				if (aNode == null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
					xDu.DocumentElement.AppendChild(aNode);
				}
				System.Xml.XmlNode bNode = aNode.SelectSingleNode("CanNetPort");
				if (bNode == null)
				{
					bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "CanNetPort", "");
					aNode.AppendChild(bNode);
					System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
					bNode.Attributes.Append(a);
				}

				bNode.Attributes["value"].Value = value.ToString();

				xDu.Save(AppConfigFileName);

				//���õ�ǰ����
				m_CanNetPort = value;
			}
		}
		#endregion

        #region ͨ�������ʱ����ͷ��˸ʱ��
        /// <summary>
        /// ͨ�������ʱ����ͷ��˸ʱ��
        /// </summary>
        public static double TimeOut_SendLetter
        {
            get { return m_TimeOut_Sendletter; }
            set
            {
                if (m_TimeOut_Sendletter == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("TimeOut_SendLetter");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "TimeOut_SendLetter", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_TimeOut_Sendletter = value;
            }
        }
        #endregion
        #region Ͷ�ݿڴ򿪺󣬵ȴ�Ͷ���ʱ�䡣��ʱ�Զ��ر�Ͷ�ݿڡ�
        /// <summary>
        /// Ͷ�ݿڴ򿪺󣬵ȴ�Ͷ���ʱ�䡣��ʱ�Զ��ر�Ͷ�ݿڡ�
        /// </summary>
        public static double TimeOut_PutInLetter
        {
            get { return m_TimeOut_PutInLetter; }
            set
            {
                if (m_TimeOut_PutInLetter == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("TimeOut_PutInLetter");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "TimeOut_PutInLetter", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_TimeOut_PutInLetter = value;
            }
        }
        #endregion
        #region ���Ŵ򿪺󣬵ȴ����ŵ�ʱ�䡣��ʱ��ʾ���˹���
        /// <summary>
        /// ���Ŵ򿪺󣬵ȴ����ŵ�ʱ�䡣��ʱ��ʾ���˹���
        /// </summary>
        public static double TimeOut_CloseDoor
        {
            get { return m_TimeOut_CloseDoor; }
            set
            {
                if (m_TimeOut_CloseDoor == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("TimeOut_CloseDoor");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "TimeOut_CloseDoor", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_TimeOut_CloseDoor = value;
            }
        }
        #endregion
        #region ��ʾ��Ļ��ʾ��Ϣ��ʱ�䡣��ʱ���س�ʼ״̬
        /// <summary>
        /// ��ʾ��Ļ��ʾ��Ϣ��ʱ�䡣��ʱ���س�ʼ״̬
        /// </summary>
        public static double TimeOut_ShowInfoMsg
        {
            get { return m_TimeOut_ShowInfoMsg; }
            set
            {
                if (m_TimeOut_ShowInfoMsg == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("TimeOut_ShowInfoMsg");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "TimeOut_ShowInfoMsg", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_TimeOut_ShowInfoMsg = value;
            }
        }
        #endregion
        #region ��ͷping���ʱ�䣬��������ʱ�䣬��ͷ�Ͽ�
        /// <summary>
        /// ��ͷping���ʱ�䣬��������ʱ�䣬��ͷ�Ͽ�
        /// </summary>
        public static double ConnectTimeOut
        {
            get { return m_ConnectTimeOut; }
            set
            {
                if (m_ConnectTimeOut == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("TimeOut_Connection");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "TimeOut_Connection", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_ConnectTimeOut = value;
            }
        }
        #endregion
		#region �ر���ͷ��ʾ��5����
		/// <summary>
		/// �ر���ͷ��ʾ��5����
        /// </summary>
		public static int TimeOut_CloseScreen
        {
			get { return m_TimeOut_CloseScreen; }
            set
            {
				if (m_TimeOut_CloseScreen == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
				System.Xml.XmlNode bNode = aNode.SelectSingleNode("TimeOut_CloseScreen");
                if (bNode == null)
                {
					bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "TimeOut_CloseScreen", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
				m_TimeOut_CloseScreen = value;
            }
        }
        #endregion

        #region SCDCC����
        /// <summary>
        /// SCDCC IP
        /// </summary>
        public static string SCDCC_IP
        {
            get { return m_SCDCC_IP; }
            set
            {
                if (m_SCDCC_IP == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SCDCC");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SCDCC", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("Ip");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Ip", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value;

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_SCDCC_IP = value;
            }
        }
        /// <summary>
        /// SCDCC Port
        /// </summary>
        public static int SCDCC_Port
        {
            get { return m_SCDCC_Port; }
            set
            {
                if (m_SCDCC_Port == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SCDCC");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SCDCC", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("Port");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Port", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_SCDCC_Port = value;
            }
        }

        #endregion

        #region Ӳ��¼���������ַ����
        /// <summary>
        /// DVRServer_IP
        /// </summary>
        public static string DVRServer_IP
        {
            get { return m_DVRServer_IP; }
            set
            {
                if (m_DVRServer_IP == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("DVRServer");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "DVRServer", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("Ip");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Ip", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value;

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_DVRServer_IP = value;
            }
        }
        /// <summary>
        /// DVRServer_Port
        /// </summary>
        public static int DVRServer_Port
        {
            get { return m_DVRServer_Port; }
            set
            {
                if (m_DVRServer_Port == value)
                    return;

                //���������ݿ��ļ�
                System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
                xDu.Load(AppConfigFileName);
                System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("DVRServer");
                if (aNode == null)
                {
                    aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "DVRServer", "");
                    xDu.DocumentElement.AppendChild(aNode);
                }
                System.Xml.XmlNode bNode = aNode.SelectSingleNode("Port");
                if (bNode == null)
                {
                    bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Port", "");
                    aNode.AppendChild(bNode);
                    System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
                    bNode.Attributes.Append(a);
                }

                bNode.Attributes["value"].Value = value.ToString();

                xDu.Save(AppConfigFileName);

                //���õ�ǰ����
                m_DVRServer_Port = value;
            }
        }

        #endregion

		#region �Ƿ�ʹ�ñ�������
		/// <summary>
		/// �Ƿ�ʹ�ñ�������
		/// </summary>
		public static bool UserLocalData
		{
			get { return m_UserLocalData; }
			set
			{
				if (m_UserLocalData == value)
					return;

				//���������ݿ��ļ�
				System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
				xDu.Load(AppConfigFileName);
				System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
				if (aNode == null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
					xDu.DocumentElement.AppendChild(aNode);
				}
				System.Xml.XmlNode bNode = aNode.SelectSingleNode("UserLocalData");
				if (bNode == null)
				{
					bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "UserLocalData", "");
					aNode.AppendChild(bNode);
					System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
					bNode.Attributes.Append(a);
				}

				bNode.Attributes["value"].Value = value.ToString();

				xDu.Save(AppConfigFileName);

				//���õ�ǰ����
				m_UserLocalData = value;
			}
		}
		#endregion

		#region �Ƿ�ʹ�ñ�����������ļ�
		/// <summary>
		/// �Ƿ�ʹ�ñ�����������ļ�
		/// </summary>
		public static bool UserLocalBoxConfig
		{
			get { return m_UserLocalBoxConfig; }
			set
			{
				if (m_UserLocalBoxConfig == value)
					return;

				//���������ݿ��ļ�
				System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
				xDu.Load(AppConfigFileName);
				System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
				if (aNode == null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
					xDu.DocumentElement.AppendChild(aNode);
				}
				System.Xml.XmlNode bNode = aNode.SelectSingleNode("UserLocalBoxConfig");
				if (bNode == null)
				{
					bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "UserLocalBoxConfig", "");
					aNode.AppendChild(bNode);
					System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
					bNode.Attributes.Append(a);
				}

				bNode.Attributes["value"].Value = value.ToString();

				xDu.Save(AppConfigFileName);

				//���õ�ǰ����
				m_UserLocalBoxConfig = value;
			}
		}
		#endregion

		#region ȡ������ͬ����ʽ��0����UserGetLetter��ʽ��1�����µ�3��ͬ����ʽ
		/// <summary>
		/// ȡ������ͬ����ʽ��0����UserGetLetter��ʽ��1�����µ�3��ͬ����ʽ
		/// </summary>
		public static int UserGetLetterType
		{
			get { return m_UserGetLetterType; }
			set
			{
				if (m_UserGetLetterType == value)
					return;

				//���������ݿ��ļ�
				System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
				xDu.Load(AppConfigFileName);
				System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
				if (aNode == null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "SystemConfig", "");
					xDu.DocumentElement.AppendChild(aNode);
				}
				System.Xml.XmlNode bNode = aNode.SelectSingleNode("UserGetLetterType");
				if (bNode == null)
				{
					bNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "UserGetLetterType", "");
					aNode.AppendChild(bNode);
					System.Xml.XmlAttribute a = xDu.CreateAttribute("value");
					bNode.Attributes.Append(a);
				}

				bNode.Attributes["value"].Value = value.ToString();

				xDu.Save(AppConfigFileName);

				//���õ�ǰ����
				m_UserGetLetterType = value;
			}
		}
		#endregion

        #region ��ʼ������
        static Constant()
		{
			try
			{
				//���������ݿ��ļ�
				System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
				xDu.Load(AppConfigFileName);
				System.Xml.XmlNode aNode = xDu.DocumentElement.SelectSingleNode("DataBase");
				if(aNode!=null && aNode.ChildNodes!=null)
				{
					foreach(System.Xml.XmlNode tNode in aNode.ChildNodes)
					{
						switch( tNode.Name )
						{
                            case "ServiceURL":
                                m_ServiceURL = tNode.Attributes["value"].Value;
                                break;
                            case "ServiceURL_NoLocal":
                                m_ServiceURL_NoLocal = tNode.Attributes["value"].Value;
                                break;
						}
					}
				}

				//AppConfig��Ϣ
				aNode = xDu.DocumentElement.SelectSingleNode("AppConfig");
				if(aNode!=null && aNode.ChildNodes!=null)
				{
					foreach(System.Xml.XmlNode tNode in aNode.ChildNodes)
					{
						switch( tNode.Name )
						{
							case "DistributeType":
                                try
                                {
                                    m_DistributeType = Convert.ToInt32(tNode.Attributes["value"].Value);
                                }
                                catch
                                { }
                                break;

                            case "NewspaperType":
                                try
                                {
                                    m_NewspaperType = Convert.ToInt32(tNode.Attributes["value"].Value);
                                }
                                catch
                                { }
                                break;

                            case "OpenDoorType":
								if(tNode.Attributes["value"].Value.ToLower().CompareTo("true")==0)
                                    m_OpenDoorType = true;
								else
                                    m_OpenDoorType = false;
								break;

                            case "IsPiaoJuXiang":
								if(tNode.Attributes["value"].Value.ToLower().CompareTo("true")==0)
                                    m_IsPiaoJuXiang = true;
								else
                                    m_IsPiaoJuXiang = false;
								break;

							case "QuJianType":
								if(!Int32.TryParse(tNode.Attributes["value"].Value, out m_QuJianType))
									m_QuJianType = 1;
								break;

							case "EmptyBoxName":
								m_EmptyBoxName = tNode.Attributes["value"].Value;
								break;
                        }
					}
				}

				// Pc104Config
				aNode = xDu.DocumentElement.SelectSingleNode("Pc104Config");
				if(aNode!=null && aNode.ChildNodes!=null)
				{
					foreach(System.Xml.XmlNode tNode in aNode.ChildNodes)
					{
						switch( tNode.Name )
						{
                            case "TextLeft":
                                try
                                {
                                    m_TextLeft = Convert.ToInt32(tNode.Attributes["value"].Value);
                                }
                                catch
                                { }
                                break;

                            case "TextTop":
                                try
                                {
                                    m_TextTop = Convert.ToInt32(tNode.Attributes["value"].Value);
                                }
                                catch
                                { }
                                break;
                        }
					}
				}

				//SystemConfig
				aNode = xDu.DocumentElement.SelectSingleNode("SystemConfig");
				if(aNode!=null && aNode.ChildNodes!=null)
				{
					foreach(System.Xml.XmlNode tNode in aNode.ChildNodes)
					{
						switch( tNode.Name )
						{
                            case "ThreadCount":
								try
								{
                                    ThreadCount = Convert.ToInt32(tNode.Attributes["value"].Value);
								}
								catch
								{}
								break;

                            case "CanCount":
								try
								{
                                    m_CanCount = Convert.ToInt32(tNode.Attributes["value"].Value);
								}
								catch
								{ m_CanCount = 1; }
								break;

							case "CanType":
								try
								{
                                    m_CanType = Convert.ToInt32(tNode.Attributes["value"].Value);
								}
								catch
								{ m_CanType = 4; }
								break;

							case "CanNetIP":
								try
								{
									m_CanNetIP = tNode.Attributes["value"].Value;
								}
								catch
								{ m_CanType = 4; }
								break;

							case "CanNetPort":
								try
								{
									m_CanNetPort = Convert.ToInt32(tNode.Attributes["value"].Value);
								}
								catch
								{ m_CanType = 4; }
								break;

							case "NotifyGroupName":
								try
								{
                                    m_NotifyGroupName = Convert.ToBoolean(tNode.Attributes["value"].Value);
								}
								catch
								{}
								break;

                            case "SysRebootTime":
                                try
                                {
                                    m_SysRebootTime = Convert.ToInt32(tNode.Attributes["value"].Value);
                                }
                                catch
                                { }
                                break;

                            case "TimeOut_SendLetter":
                                m_TimeOut_Sendletter = Convert.ToDouble(tNode.Attributes["value"].Value);
                                break;

                            case "TimeOut_CloseDoor":
                                m_TimeOut_CloseDoor = Convert.ToDouble(tNode.Attributes["value"].Value);
                                break;

                            case "TimeOut_PutInLetter":
                                m_TimeOut_PutInLetter = Convert.ToDouble(tNode.Attributes["value"].Value);
                                break;

                            case "TimeOut_ShowInfoMsg":
                                m_TimeOut_ShowInfoMsg = Convert.ToDouble(tNode.Attributes["value"].Value);
                                break;

                            case "TimeOut_Connection":
                                m_ConnectTimeOut = Convert.ToDouble(tNode.Attributes["value"].Value);
                                break;

							case "TimeOut_CloseScreen":
								m_TimeOut_CloseScreen = Convert.ToInt32(tNode.Attributes["value"].Value);
                                break;

							case "UserLocalData":
								try
								{
									m_UserLocalData = Convert.ToBoolean(tNode.Attributes["value"].Value);
								}
								catch
								{}
								break;

							case "UserLocalBoxConfig":
								try
								{
									m_UserLocalBoxConfig = Convert.ToBoolean(tNode.Attributes["value"].Value);
								}
								catch
								{}
								break;

							case "UserGetLetterType":
								m_UserGetLetterType = Convert.ToInt32(tNode.Attributes["value"].Value);
                                break;
                        }
                    }
                }

                //SCDCC
                aNode = xDu.DocumentElement.SelectSingleNode("SCDCC");
                if (aNode != null && aNode.ChildNodes != null)
                {
                    foreach (System.Xml.XmlNode tNode in aNode.ChildNodes)
                    {
                        switch (tNode.Name)
                        {
                            case "Ip":
                                m_SCDCC_IP = tNode.Attributes["value"].Value;
                                break;

                            case "Port":
                                m_SCDCC_Port = Convert.ToInt32(tNode.Attributes["value"].Value);
                                break;
                        }
                    }
                }

                //DVRServer
                aNode = xDu.DocumentElement.SelectSingleNode("DVRServer");
                if (aNode != null && aNode.ChildNodes != null)
                {
                    foreach (System.Xml.XmlNode tNode in aNode.ChildNodes)
                    {
                        switch (tNode.Name)
                        {
                            case "Ip":
                                m_DVRServer_IP = tNode.Attributes["value"].Value;
                                break;

                            case "Port":
                                m_DVRServer_Port = Convert.ToInt32(tNode.Attributes["value"].Value);
                                break;
                        }
                    }
                }

			}
			catch (Exception ee)
            { string aa = ee.ToString(); }
        }
        #endregion

	}
}
