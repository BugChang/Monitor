using System;
using System.Xml;
using System.Net;
using System.Net.Sockets;

namespace ClientDataParse
{
	public delegate string ReceiveClientData(int itype, string data, string data2);

	/// <summary>
	/// Class1 ��ժҪ˵����
	/// </summary>
	public class ClientDataParse
	{
		private static XmlNamespaceManager nsmgr;
        private static string XMLString_Far;
        private static string XMLNameSpace_Far;

        public static event ReceiveClientData OnReceiveClentData = null;

        static ClientDataParse()
        {
            XMLString_Far = LogInfo.Constant.XMLString_Far;
            XMLNameSpace_Far = LogInfo.Constant.XMLNameSpace_Far;
            XmlDocument xDu = new XmlDocument();
            xDu.LoadXml(XMLString_Far);
            nsmgr = new XmlNamespaceManager(xDu.NameTable);
            nsmgr.AddNamespace("aa", XMLNameSpace_Far);
        }

        public static bool Start()
        {
            return ListenClient.ClientListen.Start(ProccessData);

        }

        public static void Stop()
        {
            ListenClient.ClientListen.Stop();
        }
		//���������յ�������
        private static void ProccessData(string xmlData, Socket s, EndPoint ep)
		{
			try
			{
				System.Xml.XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlData);

				System.Xml.XmlNodeList tList_far = doc.DocumentElement.SelectNodes("aa:farShine/aa:Operation", nsmgr);

				//�������յ������ݵ�����
				foreach(System.Xml.XmlNode tNode in tList_far)
				{
					switch(tNode.Attributes["name"].Value)
					{
							#region ������ʼ��
						case "User-Login-Request":
							User_Login(tNode, s, ep);
							break;

						case "User-Interface-Request":
                            User_Interface(tNode, s, ep);
							break;

                        case "UserList-Request":
                            UserList_Request(tNode, s, ep);
                            break;
							#endregion
					
							#region ҵ��ƽ̨���ط�����ͨѶ
						case "Exchanger-GetLetter-Box-Request":
                            Exchanger_GetLetter_Box(tNode, s, ep);
							break;

						case "Check-Letter-Request":
                            Check_Letter(tNode, s, ep);
							break;

                            #endregion

							#region ��ؿͻ������ط�����ͨѶ
						case "Open-Door-Request":
                            Open_Door(tNode, s, ep);
							break;

						case "Box-Lamp-Request":
                            Box_Lamp(tNode, s, ep);
							break;

						case "Box-Gating-Request":
                            Box_Gating(tNode, s, ep);
							break;

						case "Box-Letters-Request":
                            Box_Letters(tNode, s, ep);
							break;

                        case "All-LampYiQu-Request":
                            All_LampYiQu(tNode, s, ep);
							break;

						case "OpenAll-Door-Request":
                            OpenAll_Door(tNode, s, ep);
							break;

						case "CheckAll-Letter-Request":
                            CheckAll_Letter(tNode, s, ep);
							break;

						case "Reset-Request":
                            Reset_Request(tNode, s, ep);
							break;

                        case "Front-GetLetter-Request":
                            Front_GetLetter_Request(tNode, s, ep);
                            break;
							#endregion

							#region �����������
						case "UnitList-Request":
                            UnitList_Request(tNode, s, ep);
							break;

						case "Box-UnitChange-Request":
                            Box_UnitChange(tNode, s, ep);
							break;
							#endregion

                        #region �����ַ�ͨѶ
                        case "Newspaper-Start-Request":
                            Newspaper_Start_Request(tNode, s, ep);
                            break;

                        case "Newspaper-End-Request":
                            Newspaper_End_Request(tNode, s, ep);
                            break;
                        #endregion
                    }

				}

			}
			catch(Exception ee)
			{
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "�ͻ��˽��յ������ݴ������" + ee.ToString() + "\r\n");
			}
		}

		#region �û���¼��Ϣ��ѯ��101
		/// <summary>
		/// �û���¼��Ϣ��ѯ��101
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void User_Login(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string Username = SourceNode.SelectSingleNode("aa:User", nsmgr).Attributes["name"].Value;
			string UserPwd = SourceNode.SelectSingleNode("aa:User", nsmgr).Attributes["pwd"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
				RStr = OnReceiveClentData( 101, Username + ":" +  UserPwd, "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��User-Login-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "User-Login-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� User name
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "User", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = Username;
			tNode.Attributes.Append( tAttribute );
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s, ep);
		}
		#endregion

		#region ��ؿͻ��˽�����Ϣ��ѯ��102
		/// <summary>
		/// ��ؿͻ��˽�����Ϣ��ѯ��102
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void User_Interface(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string Username = SourceNode.SelectSingleNode("aa:User", nsmgr).Attributes["name"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
                RStr = OnReceiveClentData(102, Username, "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��User-Interface-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "User-Interface-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� User name
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "User", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = Username;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			//���� Table
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Table", XMLNameSpace_Far);
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Box NO
			string[] Boxs = RStr.Split('\n');
			for(int i=0;i<Boxs.Length;i++)
			{
				string[] oneBox = Boxs[i].Split( '\r' );
				if(oneBox.Length<8)
					continue;
				tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
				tAttribute = xDu.CreateAttribute("NO");
				tAttribute.Value = oneBox[0];
				tNode.Attributes.Append( tAttribute );
				tAttribute = xDu.CreateAttribute("Unit");
				tAttribute.Value = oneBox[1];
				tNode.Attributes.Append( tAttribute );
				tAttribute = xDu.CreateAttribute("UnitNO");
				tAttribute.Value = oneBox[2];
				tNode.Attributes.Append( tAttribute );
				tAttribute = xDu.CreateAttribute("letters");
				tAttribute.Value = oneBox[3];
				tNode.Attributes.Append( tAttribute );
				tAttribute = xDu.CreateAttribute("userright");
				tAttribute.Value = oneBox[4];
				tNode.Attributes.Append( tAttribute );
				tAttribute = xDu.CreateAttribute("state");
				tAttribute.Value = oneBox[5];
				tNode.Attributes.Append( tAttribute );
				tAttribute = xDu.CreateAttribute("ConnectState");
				tAttribute.Value = oneBox[6];
				tNode.Attributes.Append( tAttribute );
				tAttribute = xDu.CreateAttribute("Errstring");
				tAttribute.Value = oneBox[7];
				tNode.Attributes.Append( tAttribute );
				aNode.AppendChild( tNode );
			}

			SendXmlDataToClient(xDu.InnerXml, s, ep);
		}
		#endregion

        #region �û��б��ѯ��103
        /// <summary>
        /// �û��б��ѯ��103
        /// </summary>
        /// <param name="SourceNode"></param>
        /// <param name="s"></param>
        private static void UserList_Request(XmlNode SourceNode, Socket s, EndPoint ep)
        {
            string RStr = "error";

            if (OnReceiveClentData != null)
                RStr = OnReceiveClentData(103, "", "");

            System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
            xDu.LoadXml(XMLString_Far);
            System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

            //���� Operation name=��UserList-Response��
            System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
            System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
            tAttribute.Value = "UserList-Response";
            tNode.Attributes.Append(tAttribute);
            aNode.AppendChild(tNode);
            aNode = tNode;

            string[] Users = RStr.Split('\n');
            for (int i = 0; i < Users.Length; i++)
            {
                string[] oneUser = Users[i].Split('\r');
                if (oneUser.Length < 3)
                    continue;
                //���� User LoginName=���û���¼���ơ� UserName=���û���ʾ���ơ� Right=��Admin/User��
                tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "User", XMLNameSpace_Far);

                tAttribute = xDu.CreateAttribute("LoginName");
                tAttribute.Value = oneUser[0];
                tNode.Attributes.Append(tAttribute);

                tAttribute = xDu.CreateAttribute("UserName");
                tAttribute.Value = oneUser[1];
                tNode.Attributes.Append(tAttribute);

                tAttribute = xDu.CreateAttribute("Right");
                tAttribute.Value = oneUser[2];
                tNode.Attributes.Append(tAttribute);

                aNode.AppendChild(tNode);
            }

            SendXmlDataToClient(xDu.InnerXml, s, ep);
        }
        #endregion


		#region �û�ȡ�����󣨿�ָ�����ţ���202
		/// <summary>
		/// �û�ȡ�����󣨿�ָ�����ţ���202
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void Exchanger_GetLetter_Box(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string UserBarCode = SourceNode.SelectSingleNode("aa:User", nsmgr).Attributes["BoxNO"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
                RStr = OnReceiveClentData(202, UserBarCode, "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��Exchanger-GetLetter-Box-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "Exchanger-GetLetter-Box-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Box NO
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "User", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion

		#region �˶���������203
		/// <summary>
		/// �˶���������203
		/// </summary>
		/// <param name="tNode"></param>
		/// <param name="s"></param>
        private static void Check_Letter(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string BoxNO = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["NO"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
                RStr = OnReceiveClentData(203, BoxNO, "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��Check-Letter-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "Check-Letter-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Box NO
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("NO");
			tAttribute.Value = BoxNO;
			tNode.Attributes.Append( tAttribute );
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion


        #region ��������401
		/// <summary>
		/// ��������401
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void Open_Door(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string BoxNO = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["NO"].Value;
			string bFront = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["bFront"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
				RStr = OnReceiveClentData(401, BoxNO, bFront);

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��Open-Door-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "Open-Door-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Box NO
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("NO");
			tAttribute.Value = BoxNO;
			tNode.Attributes.Append( tAttribute );
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
        #endregion

	    #region ��ʾ��Ϣ��פ

	    public void SendMsg(string no,string msg)
	    {
        }

	    #endregion

        #region ���Ƶ�����402
        /// <summary>
        /// ���Ƶ�����402
        /// </summary>
        /// <param name="SourceNode"></param>
        /// <param name="s"></param>
        private static void Box_Lamp(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string BoxNO = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["NO"].Value;
			string BoxLamp = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["lamp"].Value;
			string BoxValue = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["value"].Value;
			string bFront = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["bFront"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
				RStr = OnReceiveClentData(402, BoxNO + ":" + BoxLamp + ":" + BoxValue, bFront);

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��Box-Lamp-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "Box-Lamp-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Box NO
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("NO");
			tAttribute.Value = BoxNO;
			tNode.Attributes.Append( tAttribute );
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion

		#region �����Ž�����403
		/// <summary>
		/// �����Ž�����403
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void Box_Gating(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string BoxNO = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["NO"].Value;
			string BoxValue = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["value"].Value;
			string bFront = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["bFront"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
				RStr = OnReceiveClentData(403, BoxNO + ":" + BoxValue, bFront);

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��Box-Gating-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "Box-Gating-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Box NO
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("NO");
			tAttribute.Value = BoxNO;
			tNode.Attributes.Append( tAttribute );
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion

		#region ����������404
		/// <summary>
		/// ����������404
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void Box_Letters(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string BoxNO = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["NO"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
                RStr = OnReceiveClentData(404, BoxNO, "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��Box-Letters-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "Box-Letters-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Box NO
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("NO");
			tAttribute.Value = BoxNO;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			//���� Table
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Table", XMLNameSpace_Far);
			aNode.AppendChild( tNode );
            tNode.InnerText = RStr;

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion

		#region ����������󣭵�λ�б�405
		/// <summary>
		/// ����������󣭵�λ�б�405
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void UnitList_Request(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string RStr = "error";

			if(OnReceiveClentData!=null)
                RStr = OnReceiveClentData(405, "", "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��UnitList-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "UnitList-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Unit
			string[] Units = RStr.Split('\n');
			for(int i=0;i<Units.Length;i++)
			{
				string[] oneUnit = Units[i].Split( '\r' );
				if(oneUnit.Length<3)
					continue;
				tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Unit", XMLNameSpace_Far);
                tAttribute = xDu.CreateAttribute("UnitBH");
				tAttribute.Value = oneUnit[0];
				tNode.Attributes.Append( tAttribute );
				tAttribute = xDu.CreateAttribute("NO");
				tAttribute.Value = oneUnit[1];
				tNode.Attributes.Append( tAttribute );
				tAttribute = xDu.CreateAttribute("name");
				tAttribute.Value = oneUnit[2];
				tNode.Attributes.Append( tAttribute );
				aNode.AppendChild( tNode );
			}

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion

		#region ����������󣭸��ĵ�λ��Ŷ�Ӧ��406
		/// <summary>
		/// ����������󣭸��ĵ�λ��Ŷ�Ӧ��406
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void Box_UnitChange(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string BoxNO = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["NO"].Value;
			string UnitNO = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["UnitNO"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
                RStr = OnReceiveClentData(406, BoxNO + ":" + UnitNO, "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��Box-UnitChange-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "Box-UnitChange-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Box NO
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("NO");
			tAttribute.Value = BoxNO;
			tNode.Attributes.Append( tAttribute );
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion

		#region ����������ȡ������407
		/// <summary>
		/// �������е�����407
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void All_LampYiQu(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			XmlNode t = SourceNode.SelectSingleNode("aa:Lamp", nsmgr);
			string TypeValue = t.Attributes["value"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
                RStr = OnReceiveClentData(407, TypeValue, "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��All-Lamp-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
            tAttribute.Value = "All-LampYiQu-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Box NO
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Lamp", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion

		#region ���������ţ�408
		/// <summary>
		/// ���������ţ�408
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void OpenAll_Door(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string RStr = "error";

			string BoxNO = "";
			XmlNodeList tList = SourceNode.SelectNodes("aa:Box", nsmgr);
			for(int i=0;i<tList.Count;i++)
			{
				if(BoxNO.CompareTo("")!=0)
					BoxNO += ":";
				BoxNO += tList[i].Attributes["NO"].Value;
			}

			if(OnReceiveClentData!=null)
                RStr = OnReceiveClentData(408, BoxNO, "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��OpenAll-Door-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "OpenAll-Door-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Door value
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Door", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion

		#region �˶�����������409
		/// <summary>
		/// �˶�����������409
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void CheckAll_Letter(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string RStr = "error";

			if(OnReceiveClentData!=null)
                RStr = OnReceiveClentData(409, "", "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��CheckAll-Letter-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "CheckAll-Letter-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Letter value
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Letter", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion

		#region ������������410
		/// <summary>
		/// ������������410
		/// </summary>
		/// <param name="SourceNode"></param>
		/// <param name="s"></param>
        private static void Reset_Request(XmlNode SourceNode, Socket s, EndPoint ep)
		{
			string BoxNO = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["NO"].Value;
			string RStr = "error";

			if(OnReceiveClentData!=null)
                RStr = OnReceiveClentData(410, BoxNO, "");

			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			//���� Operation name=��Reset-Response��
			System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
			System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
			tAttribute.Value = "Reset-Response";
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );
			aNode = tNode;

			//���� Box
			tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
			tAttribute = xDu.CreateAttribute("value");
			tAttribute.Value = RStr;
			tNode.Attributes.Append( tAttribute );
			aNode.AppendChild( tNode );

			SendXmlDataToClient(xDu.InnerXml, s,ep);
		}
		#endregion

        #region �û����ȡ������416
        /// <summary>
        /// �û����ȡ������416
        /// </summary>
        /// <param name="SourceNode"></param>
        /// <param name="s"></param>
        private static void Front_GetLetter_Request(XmlNode SourceNode, Socket s, EndPoint ep)
        {
            string BoxNO = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["NO"].Value;
            string LetterID = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["LetterID"].Value;
            string IsPrint = SourceNode.SelectSingleNode("aa:Box", nsmgr).Attributes["IsPrint"].Value;
            string qmPic = SourceNode.SelectSingleNode("aa:Box", nsmgr).InnerText;
            string RStr = "error\r\r";

            if (OnReceiveClentData != null)
                RStr = OnReceiveClentData(416, BoxNO + "\r" + IsPrint + "\r" + LetterID, qmPic);

            System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
            xDu.LoadXml(XMLString_Far);
            System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

            //���� Operation name=��Front-GetLetter-Response��
            System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
            System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
            tAttribute.Value = "Front-GetLetter-Response";
            tNode.Attributes.Append(tAttribute);
            aNode.AppendChild(tNode);
            aNode = tNode;

            string[] Infos = RStr.Split('\r');
            if (Infos.Length != 3)
            {
                //���� Box value
                tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
                tAttribute = xDu.CreateAttribute("value");
                tAttribute.Value = "error";
                tNode.Attributes.Append(tAttribute);
                aNode.AppendChild(tNode);

                //���� PrintStr value
                tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "PrintStr", XMLNameSpace_Far);
                tAttribute = xDu.CreateAttribute("value");
                tAttribute.Value = "";
                tNode.Attributes.Append(tAttribute);

                tNode.InnerText = "";
                aNode.AppendChild(tNode);
            }
            else
            {
                //���� Box value
                tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
                tAttribute = xDu.CreateAttribute("value");
                tAttribute.Value = Infos[0];
                tNode.Attributes.Append(tAttribute);
                aNode.AppendChild(tNode);

                //���� PrintStr value
                tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "PrintStr", XMLNameSpace_Far);
                tAttribute = xDu.CreateAttribute("value");
                tAttribute.Value = Infos[1];
                tNode.Attributes.Append(tAttribute);

                tNode.InnerText = Infos[2];
                aNode.AppendChild(tNode);
            }

            SendXmlDataToClient(xDu.InnerXml, s, ep);
        }
        #endregion

        #region �ַ���ʼ����501
        /// <summary>
        /// �ַ���ʼ����501
        /// </summary>
        /// <param name="SourceNode"></param>
        /// <param name="s"></param>
        private static void Newspaper_Start_Request(XmlNode SourceNode, Socket s, EndPoint ep)
        {
            string data = "";
            string RStr = "error";

            try
            {
                foreach (XmlNode t in SourceNode.ChildNodes)
                {
                    string boxno = t.Attributes["NO"].Value;
                    string count = t.Attributes["value"].Value;
                    if (data == "")
                    {
                        data = boxno + "\r" + count;
                    }
                    else
                    {
                        data += "\n" + boxno + "\r" + count;
                    }
                }

                if (data != "")
                {
                    if (OnReceiveClentData != null)
                        RStr = OnReceiveClentData(501, data, "");
                }
            }
            catch
            {
            }

            System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
            xDu.LoadXml(XMLString_Far);
            System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

            //���� Operation name=��Newspaper-Start-Response��
            System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
            System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
            tAttribute.Value = "Newspaper-Start-Response";
            tNode.Attributes.Append(tAttribute);
            aNode.AppendChild(tNode);
            aNode = tNode;

            //���� State value
            tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "State", XMLNameSpace_Far);
            tAttribute = xDu.CreateAttribute("value");
            tAttribute.Value = RStr;
            tNode.Attributes.Append(tAttribute);
            aNode.AppendChild(tNode);

            SendXmlDataToClient(xDu.InnerXml, s, ep);
        }
        #endregion

        #region �ַ���������502
        /// <summary>
        /// �ַ���������502
        /// </summary>
        /// <param name="SourceNode"></param>
        /// <param name="s"></param>
        private static void Newspaper_End_Request(XmlNode SourceNode, Socket s, EndPoint ep)
        {
            string RStr = "error";

            if (OnReceiveClentData != null)
                RStr = OnReceiveClentData(502, "", "");

            System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
            xDu.LoadXml(XMLString_Far);
            System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

            //���� Operation name=��Newspaper-End-Response��
            System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
            System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
            tAttribute.Value = "Newspaper-End-Response";
            tNode.Attributes.Append(tAttribute);
            aNode.AppendChild(tNode);
            aNode = tNode;

            //���� State value
            tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "State", XMLNameSpace_Far);
            tAttribute = xDu.CreateAttribute("value");
            tAttribute.Value = RStr;
            tNode.Attributes.Append(tAttribute);
            aNode.AppendChild(tNode);

            SendXmlDataToClient(xDu.InnerXml, s, ep);
        }
        #endregion


		#region �������ݵ��ͻ���
        /// <summary>
        /// �������ݵ��ͻ���
        /// </summary>
        /// <param name="xmlData"></param>
        public static void SendXmlDataToClient(string xmlData, Socket s, EndPoint ep)
        {
            ListenClient.ClientListen.SendData(xmlData, s);
        }

		/// <summary>
		/// ���͹㲥���ݵ��ͻ���
		/// </summary>
		/// <param name="xmlData"></param>
        public static void SendBroadCast(int iType, string strData)
		{
			System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
			xDu.LoadXml(XMLString_Far);
			System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

			switch(iType)
			{
				case 301:			//Box-Lamp-Notify
				{
					//���� Operation name=��Box-Lamp-Notify��
					System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
					System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
					tAttribute.Value = "Box-Lamp-Notify";
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );
					aNode = tNode;

					//���� Box NO
					string[] oneBox = strData.Split( '\r' );
					if(oneBox.Length<3)
						return;
					tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
					tAttribute = xDu.CreateAttribute("NO");
					tAttribute.Value = oneBox[0];
					tNode.Attributes.Append( tAttribute );
					tAttribute = xDu.CreateAttribute("lamp");
					tAttribute.Value = oneBox[1];
					tNode.Attributes.Append( tAttribute );
					tAttribute = xDu.CreateAttribute("value");
					tAttribute.Value = oneBox[2];
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );

				}
					break;

				case 302:
				{
					//���� Operation name=��Box-Door-Notify��
					System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
					System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
					tAttribute.Value = "Box-Door-Notify";
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );
					aNode = tNode;

					//���� Box NO
					string[] oneBox = strData.Split( '\r' );
					if(oneBox.Length<2)
						return;
					tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
					tAttribute = xDu.CreateAttribute("NO");
					tAttribute.Value = oneBox[0];
					tNode.Attributes.Append( tAttribute );
					tAttribute = xDu.CreateAttribute("value");
					tAttribute.Value = oneBox[1];
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );

				}
					break;

				case 303:
				{
					//���� Operation name=��Box-Gating-Notify��
					System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
					System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
					tAttribute.Value = "Box-Gating-Notify";
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );
					aNode = tNode;

					//���� Box NO
					string[] oneBox = strData.Split( '\r' );
					if(oneBox.Length<2)
						return;
					tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
					tAttribute = xDu.CreateAttribute("NO");
					tAttribute.Value = oneBox[0];
					tNode.Attributes.Append( tAttribute );
					tAttribute = xDu.CreateAttribute("value");
					tAttribute.Value = oneBox[1];
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );

				}
					break;

				case 304:
				{
					//���� Operation name=��Box-Error-Notify��
					System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
					System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
					tAttribute.Value = "Box-Error-Notify";
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );
					aNode = tNode;

					//���� Box NO
					string[] oneBox = strData.Split( '\r' );
					if(oneBox.Length<2)
						return;
					tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
					tAttribute = xDu.CreateAttribute("NO");
					tAttribute.Value = oneBox[0];
					tNode.Attributes.Append( tAttribute );
					tAttribute = xDu.CreateAttribute("value");
					tAttribute.Value = oneBox[1];
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );

				}
					break;

				case 305:
				{
					//���� Operation name=��Box-LetterCount-Notify��
					System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
					System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
					tAttribute.Value = "Box-LetterCount-Notify";
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );
					aNode = tNode;

					//���� Box NO
					string[] oneBox = strData.Split( '\r' );
					if(oneBox.Length<3)
						return;
					tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
					tAttribute = xDu.CreateAttribute("NO");
					tAttribute.Value = oneBox[0];
					tNode.Attributes.Append( tAttribute );
					tAttribute = xDu.CreateAttribute("urgent");
					tAttribute.Value = oneBox[1];
					tNode.Attributes.Append( tAttribute );
					tAttribute = xDu.CreateAttribute("value");
					tAttribute.Value = oneBox[2];
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );

				}
					break;

				case 306:
				{
					//���� Operation name=��Box-UnitChange-Notify��
					System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
					System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
					tAttribute.Value = "Box-UnitChange-Notify";
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );
					aNode = tNode;

					//���� Box NO
					string[] oneBox = strData.Split( '\r' );
					if(oneBox.Length<3)
						return;
					tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
					tAttribute = xDu.CreateAttribute("NO");
					tAttribute.Value = oneBox[0];
					tNode.Attributes.Append( tAttribute );
					tAttribute = xDu.CreateAttribute("Unit");
					tAttribute.Value = oneBox[1];
					tNode.Attributes.Append( tAttribute );
					tAttribute = xDu.CreateAttribute("UnitNO");
					tAttribute.Value = oneBox[1];
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );

				}
					break;

				case 307:
				{
					//���� Operation name=��Box-Connected-Notify��
					System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
					System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
					tAttribute.Value = "Box-Connected-Notify";
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );
					aNode = tNode;

					//���� Box NO
					string[] oneBox = strData.Split( '\r' );
					if(oneBox.Length<2)
						return;
					tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
					tAttribute = xDu.CreateAttribute("NO");
					tAttribute.Value = oneBox[0];
					tNode.Attributes.Append( tAttribute );
					tAttribute = xDu.CreateAttribute("value");
					tAttribute.Value = oneBox[1];
					tNode.Attributes.Append( tAttribute );
					aNode.AppendChild( tNode );

				}
					break;
			}

            ListenClient.ClientListen.SendData(xDu.InnerXml);
		}
        /// <summary>
        /// ���ͱ����ַ��㲥���ݵ��ͻ���
        /// </summary>
        /// <param name="xmlData"></param>
        public static void SendNewsBroadCast(int iType, string strData)
        {
            System.Xml.XmlDocument xDu = new System.Xml.XmlDocument();
            xDu.LoadXml(XMLString_Far);
            System.Xml.XmlNode aNode = xDu.DocumentElement.ChildNodes[0];

            switch (iType)
            {
                case 503:			//Newspaper-Box-NotiFy
                    {
                        //���� Operation name=��Newspaper-Box-NotiFy��
                        System.Xml.XmlNode tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Operation", XMLNameSpace_Far);
                        System.Xml.XmlAttribute tAttribute = xDu.CreateAttribute("name");
                        tAttribute.Value = "Newspaper-Box-NotiFy";
                        tNode.Attributes.Append(tAttribute);
                        aNode.AppendChild(tNode);
                        aNode = tNode;

                        //���� <Box NO=����ͷ�߼���š� state=����ͷ��״̬�� />
                        string[] oneBox = strData.Split('\r');
                        if (oneBox.Length < 2)
                            return;
                        tNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Box", XMLNameSpace_Far);
                        tAttribute = xDu.CreateAttribute("NO");
                        tAttribute.Value = oneBox[0];
                        tNode.Attributes.Append(tAttribute);
                        tAttribute = xDu.CreateAttribute("state");
                        tAttribute.Value = oneBox[1];
                        tNode.Attributes.Append(tAttribute);
                        aNode.AppendChild(tNode);

                    }
                    break;

            }

            ListenClient.ClientListen.SendNewsData(xDu.InnerXml);
        }
		#endregion
	}
}
