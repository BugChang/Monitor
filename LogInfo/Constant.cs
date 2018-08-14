using System;

namespace LogInfo
{
	/// <summary>
	/// Constant 的摘要说明。
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
        /// 程序是否运行中
        /// </summary>
        public static bool b_AppRunning = false;
        public static bool IsDebug = false;

        //数据Service
        private static string m_ServiceURL = "http://localhost/MonitorService/MonitorService.asmx";
        private static string m_ServiceURL_NoLocal = "http://localhost/MonitorService/MonitorService_NoLocal.asmx";

        //AppConfig
        private static int m_DistributeType = 0;// 0:自动关门，1：按键关门
        private static int m_NewspaperType = 0;// 报纸分发，0:按键前面的全部投入，1:按键当前的投入
        private static bool m_OpenDoorType = true;// 取件方式，如果箱存为0是否开箱门
        public static bool m_IsPiaoJuXiang = false;// 是否是票据箱
		private static int m_QuJianType = 1;// 取件平台显示的类型，1为3按钮，2为老四按钮，3为新四按钮，4新的五按钮取件界面
		private static string m_EmptyBoxName = "备用";// 没有分配单位的箱子的单位名称

		//Pc104设置
        public static int m_TextLeft = 0;	//文字显示位置左边
        public static int m_TextTop = 16;	//文字显示位置顶端

		//系统设置
        public static int  ThreadCount = 20;	        //系统启动线程数目
		private static int m_CanType = 4;	        //can卡的类型，
		private static int m_CanCount = 2;	        //系统启动线程数目
		private static string m_CanNetIP = "192.168.0.234";	        //
		private static int m_CanNetPort = 4001;	        //
		public static bool m_NotifyGroupName = true;	//投箱时候，提示第几组还是提示要投入的单位的名称，True：第几组，False：单位名称
        public static int m_SysRebootTime = 3;	        //系统每天重新启动的时间。格式：小时。如：3，意思是每天凌晨3点重启
		//超时时间设置
        public static double m_ConnectTimeOut   = 60;    //5秒
        public static double m_TimeOut_Sendletter = 15;
        public static double m_TimeOut_CloseDoor = 30;
        public static double m_TimeOut_PutInLetter = 5;
		public static double m_TimeOut_ShowInfoMsg = 3;	//箱头显示提示信息，回到原始状态的时间，3秒
		public static int m_TimeOut_CloseScreen = 300;	//关闭箱头显示，5分钟

        //SCDCC设置
        public static string m_SCDCC_IP = "192.168.2.200";
        public static int    m_SCDCC_Port = 80;

        //硬盘录像服务程序地址设置
        public static string m_DVRServer_IP = "127.0.0.1";
        public static int m_DVRServer_Port = 50000;

		/// <summary>
		/// 是否使用本地数据
		/// </summary>
		private static bool m_UserLocalData = true;
		/// <summary>
		/// 是否使用本地箱格配置文件
		/// </summary>
		private static bool m_UserLocalBoxConfig = false;

		/// <summary>
		/// 取件数据同步方式，0：用UserGetLetter方式，1：用新的3次同步方式
		/// </summary>
		private static int m_UserGetLetterType = 0;

        #region 数据库属性
        public static string ServiceURL
        {
            get { return m_ServiceURL; }
            set
            {
                if (m_ServiceURL == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
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

                //读出该数据库文件
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

                //设置当前属性
                m_ServiceURL_NoLocal = value;
            }
        }
        #endregion

		#region 分发方式
		/// <summary>
		/// 0:自动关门，1：按键关门
		/// </summary>
		public static int DistributeType
		{
			get{return m_DistributeType;}
			set{
				if(m_DistributeType==value)
					return;

				//读出该数据库文件
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

				//设置当前属性
				m_DistributeType = value;
			}
		}
		#endregion
        #region 报纸分发方式
        /// <summary>
        /// 报纸分发，0:按键前面的全部投入，1:按键当前的投入
        /// </summary>
        public static int NewspaperType
        {
            get { return m_NewspaperType; }
            set
            {
                if (m_NewspaperType == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_NewspaperType = value;
            }
        }
        #endregion
        #region 取件方式，如果箱存为0是否开箱门
        /// <summary>
        /// 取件方式，如果箱存为0是否开箱门
        /// </summary>
        public static bool OpenDoorType
        {
            get { return m_OpenDoorType; }
            set
            {
                if (m_OpenDoorType == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_OpenDoorType = value;
            }
        }
        #endregion

		#region 取件平台显示的类型，1为3按钮，2为老四按钮，3为新四按钮，4新的五按钮取件界面
		/// <summary>
		/// 取件平台显示的类型，1为3按钮，2为老四按钮，3为新四按钮，4新的五按钮取件界面
		/// </summary>
		public static int QuJianType
		{
			get { return m_QuJianType; }
			set
			{
				if (m_QuJianType == value)
					return;

				//读出该数据库文件
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

				//设置当前属性
				m_QuJianType = value;
			}
		}
		#endregion

		#region 没有分配单位的箱子的单位名称
		/// <summary>
		/// 没有分配单位的箱子的单位名称
		/// </summary>
		public static string EmptyBoxName
		{
			get { return m_EmptyBoxName; }
			set
			{
				if (m_EmptyBoxName == value)
					return;

				//读出该数据库文件
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

				//设置当前属性
				m_EmptyBoxName = value;
			}
		}
		#endregion


        #region 文字显示位置左边
        /// <summary>
        /// 文字显示位置左边 0~255
        /// </summary>
        public static int TextLeft
        {
            get { return m_TextLeft; }
            set
            {
                if (m_TextLeft == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_TextLeft = value;
            }
        }
        #endregion
        #region 文字显示位置顶端
        /// <summary>
        /// 文字显示位置左边 0~255
        /// </summary>
        public static int TextTop
        {
            get { return m_TextTop; }
            set
            {
                if (m_TextTop == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_TextTop = value;
            }
        }
        #endregion

        #region 投箱时候，提示第几组还是提示要投入的单位的名称，True：第几组，False：单位名称
        /// <summary>
        /// 投箱时候，提示第几组还是提示要投入的单位的名称，True：第几组，False：单位名称
        /// </summary>
        public static bool NotifyGroupName
        {
            get { return m_NotifyGroupName; }
            set
            {
                if (m_NotifyGroupName == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_NotifyGroupName = value;
            }
        }
        #endregion
        #region 系统每天重新启动的时间。格式：小时。如：3，意思是每天凌晨3点重启
        /// <summary>
        /// 系统每天重新启动的时间。格式：小时。如：3，意思是每天凌晨3点重启
        /// </summary>
        public static int SysRebootTime
        {
            get { return m_SysRebootTime; }
            set
            {
                if (m_SysRebootTime == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_SysRebootTime = value;
            }
        }
        #endregion

		#region Can卡类型
		/// <summary>
		/// Can卡类型
		/// </summary>
		public static int CanType
		{
			get { return m_CanType; }
			set
			{
				if (m_CanType == value)
					return;

				//读出该数据库文件
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

				//设置当前属性
				m_CanType = value;
			}
		}
		#endregion
		#region Can卡路数
        /// <summary>
        /// Can卡路数
        /// </summary>
        public static int CanCount
        {
            get { return m_CanCount; }
            set
            {
                if (m_CanCount == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
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

				//读出该数据库文件
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

				//设置当前属性
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

				//读出该数据库文件
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

				//设置当前属性
				m_CanNetPort = value;
			}
		}
		#endregion

        #region 通开条码的时候，箱头闪烁时间
        /// <summary>
        /// 通开条码的时候，箱头闪烁时间
        /// </summary>
        public static double TimeOut_SendLetter
        {
            get { return m_TimeOut_Sendletter; }
            set
            {
                if (m_TimeOut_Sendletter == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_TimeOut_Sendletter = value;
            }
        }
        #endregion
        #region 投递口打开后，等待投入的时间。超时自动关闭投递口。
        /// <summary>
        /// 投递口打开后，等待投入的时间。超时自动关闭投递口。
        /// </summary>
        public static double TimeOut_PutInLetter
        {
            get { return m_TimeOut_PutInLetter; }
            set
            {
                if (m_TimeOut_PutInLetter == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_TimeOut_PutInLetter = value;
            }
        }
        #endregion
        #region 箱门打开后，等待关门的时间。超时提示忘了关门
        /// <summary>
        /// 箱门打开后，等待关门的时间。超时提示忘了关门
        /// </summary>
        public static double TimeOut_CloseDoor
        {
            get { return m_TimeOut_CloseDoor; }
            set
            {
                if (m_TimeOut_CloseDoor == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_TimeOut_CloseDoor = value;
            }
        }
        #endregion
        #region 提示屏幕显示信息的时间。超时返回初始状态
        /// <summary>
        /// 提示屏幕显示信息的时间。超时返回初始状态
        /// </summary>
        public static double TimeOut_ShowInfoMsg
        {
            get { return m_TimeOut_ShowInfoMsg; }
            set
            {
                if (m_TimeOut_ShowInfoMsg == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_TimeOut_ShowInfoMsg = value;
            }
        }
        #endregion
        #region 箱头ping间隔时间，超过设置时间，箱头断开
        /// <summary>
        /// 箱头ping间隔时间，超过设置时间，箱头断开
        /// </summary>
        public static double ConnectTimeOut
        {
            get { return m_ConnectTimeOut; }
            set
            {
                if (m_ConnectTimeOut == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
                m_ConnectTimeOut = value;
            }
        }
        #endregion
		#region 关闭箱头显示，5分钟
		/// <summary>
		/// 关闭箱头显示，5分钟
        /// </summary>
		public static int TimeOut_CloseScreen
        {
			get { return m_TimeOut_CloseScreen; }
            set
            {
				if (m_TimeOut_CloseScreen == value)
                    return;

                //读出该数据库文件
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

                //设置当前属性
				m_TimeOut_CloseScreen = value;
            }
        }
        #endregion

        #region SCDCC设置
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

                //读出该数据库文件
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

                //设置当前属性
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

                //读出该数据库文件
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

                //设置当前属性
                m_SCDCC_Port = value;
            }
        }

        #endregion

        #region 硬盘录像服务程序地址设置
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

                //读出该数据库文件
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

                //设置当前属性
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

                //读出该数据库文件
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

                //设置当前属性
                m_DVRServer_Port = value;
            }
        }

        #endregion

		#region 是否使用本地数据
		/// <summary>
		/// 是否使用本地数据
		/// </summary>
		public static bool UserLocalData
		{
			get { return m_UserLocalData; }
			set
			{
				if (m_UserLocalData == value)
					return;

				//读出该数据库文件
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

				//设置当前属性
				m_UserLocalData = value;
			}
		}
		#endregion

		#region 是否使用本地箱格配置文件
		/// <summary>
		/// 是否使用本地箱格配置文件
		/// </summary>
		public static bool UserLocalBoxConfig
		{
			get { return m_UserLocalBoxConfig; }
			set
			{
				if (m_UserLocalBoxConfig == value)
					return;

				//读出该数据库文件
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

				//设置当前属性
				m_UserLocalBoxConfig = value;
			}
		}
		#endregion

		#region 取件数据同步方式，0：用UserGetLetter方式，1：用新的3次同步方式
		/// <summary>
		/// 取件数据同步方式，0：用UserGetLetter方式，1：用新的3次同步方式
		/// </summary>
		public static int UserGetLetterType
		{
			get { return m_UserGetLetterType; }
			set
			{
				if (m_UserGetLetterType == value)
					return;

				//读出该数据库文件
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

				//设置当前属性
				m_UserGetLetterType = value;
			}
		}
		#endregion

        #region 初始化函数
        static Constant()
		{
			try
			{
				//读出该数据库文件
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

				//AppConfig信息
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
