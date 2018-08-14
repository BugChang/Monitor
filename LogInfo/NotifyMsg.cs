using System;
using System.Xml;

namespace LogInfo
{
	/// <summary>
	/// BoxToIP 的摘要说明。
	/// 对逻辑箱号和IP地址进行翻译
	/// </summary>
	public class NotifyMsg
	{
		public class msgInfo
		{
			public int id = 0;
			public string Comment = "";
			public string Text = "";
			public string Sound = "";
		}

		private static System.Collections.Generic.Dictionary<int, msgInfo> m_MsgArray = new System.Collections.Generic.Dictionary<int, msgInfo>();

		#region 初始化
		static NotifyMsg()
		{
			try
			{
				System.Xml.XmlDocument doc = new XmlDocument();
				doc.Load(Constant.MsgConfigFileName);

				if (doc.DocumentElement.ChildNodes.Count <= 0)
					return;

				#region 初始化信息
				foreach (XmlNode tNode in doc.DocumentElement.ChildNodes)
				{
					if (tNode.Name != "Msg") continue;
					int id = int.Parse(tNode.Attributes["id"].Value);
					string Comment = tNode.Attributes["Comment"].Value;
					string Text = tNode.Attributes["Text"].Value;
					string Sound = tNode.Attributes["Sound"].Value;

					//重名出现，错误
					if (m_MsgArray.ContainsKey(id))
						continue;

					msgInfo m = new msgInfo();
					m.id = id; m.Text = Text; m.Sound = Sound; m.Comment = Comment;
					m_MsgArray.Add(id, m);
				}
				#endregion
			}
			catch (Exception ee)
			{
				LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "加载提示信息出错：" + ee.ToString() + "\r\n");
			}
		}
		#endregion

		#region 得到所有信息列表
		public static System.Collections.Generic.Dictionary<int, msgInfo> MsgArray
		{
			get
			{
				return m_MsgArray;
			}
		}
		#endregion

		#region 得到文本提示
		public static string GetText(int id)
		{
			string sRet = "";
			try
			{
				if (m_MsgArray.ContainsKey(id))
					sRet = m_MsgArray[id].Text;
			}
			catch (Exception ee)
			{
				LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "得到提示信息出错：" + ee.ToString());
			}
			return sRet;
		}
		#endregion
		#region 设置文本提示
		public static void SetText(int id, string str)
		{
			try
			{
				if (m_MsgArray.ContainsKey(id))
				{
					if (m_MsgArray[id].Text == str)
						return;
				}

				System.Xml.XmlDocument xDu = new XmlDocument();
				xDu.Load(Constant.MsgConfigFileName);

				System.Xml.XmlNode aNode = null;
				for (int i = 0; i < xDu.DocumentElement.ChildNodes.Count; i++)
				{
					if (xDu.DocumentElement.ChildNodes[i].Attributes["id"].Value == id.ToString())
					{
						aNode = xDu.DocumentElement.ChildNodes[i];
						break;
					}
				}
				if (aNode == null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Msg", "");
					xDu.DocumentElement.AppendChild(aNode);
					System.Xml.XmlAttribute a = xDu.CreateAttribute("id");
					a.Value = id.ToString();
					aNode.Attributes.Append(a);

					a = xDu.CreateAttribute("Text");
					aNode.Attributes.Append(a);
					a = xDu.CreateAttribute("Sound");
					aNode.Attributes.Append(a);
				}

				aNode.Attributes["Text"].Value = str;

				xDu.Save(Constant.MsgConfigFileName);

				//设置当前属性
				if (m_MsgArray.ContainsKey(id))
				{
					m_MsgArray[id].Text = str;
				}
				else
				{
					msgInfo m = new msgInfo();
					m.id = id; m.Text = str; m.Sound = "";
					m_MsgArray.Add(id, m);
				}
			}
			catch (Exception ee)
			{
				LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "加载提示信息出错：" + ee.ToString() + "\r\n");
			}

		}
		#endregion

		#region 得到语音提示
		public static string GetSound(int id)
		{
			string sRet = "";
			try
			{
				if (m_MsgArray.ContainsKey(id))
					sRet = m_MsgArray[id].Sound;
			}
			catch (Exception ee)
			{
				LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "得到提示信息出错：" + ee.ToString());
			}
			return sRet;
		}
		#endregion
		#region 设置语音提示
		public static void SetSound(int id, string str)
		{
			try
			{
				if (m_MsgArray.ContainsKey(id))
				{
					if (m_MsgArray[id].Sound == str)
						return;
				}

				System.Xml.XmlDocument xDu = new XmlDocument();
				xDu.Load(Constant.MsgConfigFileName);

				System.Xml.XmlNode aNode = null;
				for (int i = 0; i < xDu.DocumentElement.ChildNodes.Count; i++)
				{
					if (xDu.DocumentElement.ChildNodes[i].Attributes["id"].Value == id.ToString())
					{
						aNode = xDu.DocumentElement.ChildNodes[i];
						break;
					}
				}
				if (aNode == null)
				{
					aNode = xDu.CreateNode(System.Xml.XmlNodeType.Element, "Msg", "");
					xDu.DocumentElement.AppendChild(aNode);

					System.Xml.XmlAttribute a = xDu.CreateAttribute("id");
					a.Value = id.ToString();
					aNode.Attributes.Append(a);

					a = xDu.CreateAttribute("Text");
					aNode.Attributes.Append(a);
					a = xDu.CreateAttribute("Sound");
					aNode.Attributes.Append(a);
				}

				aNode.Attributes["Sound"].Value = str;

				xDu.Save(Constant.MsgConfigFileName);

				//设置当前属性
				if (m_MsgArray.ContainsKey(id))
				{
					m_MsgArray[id].Sound = str;
				}
				else
				{
					msgInfo m = new msgInfo();
					m.id = id; m.Sound = str; m.Text = "";
					m_MsgArray.Add(id, m);
				}
			}
			catch (Exception ee)
			{
				LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "加载提示信息出错：" + ee.ToString() + "\r\n");
			}

		}
		#endregion

	}
}
