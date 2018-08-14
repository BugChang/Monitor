using System;

namespace DataProcess
{
	/// <summary>
	/// SendClientNotify 的摘要说明。
	/// </summary>
	public class SendClientNotify
	{
		#region 灯状态改变通知
		/// <summary>
		/// 灯状态改变通知
		/// </summary>
		/// <param name="BoxNO">箱子号码</param>
		/// <param name="LampIndex">1:已取，3:急件</param>
		/// <param name="cType">状态：on/off</param>
		public static void NotifyLamp(string BoxNO, string LampIndex, string cType)
		{
			ClientDataParse.ClientDataParse.SendBroadCast(301, BoxNO + "\r" + LampIndex + "\r" + cType);
		}
		#endregion

		#region 门状态改变通知
		/// <summary>
		/// 门状态改变通知
		/// </summary>
		/// <param name="BoxNO">箱子号码</param>
		/// <param name="cType">状态：on/off</param>
		public static void NotifyDoor(string BoxNO, string cType)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(302, BoxNO + "\r" + cType);
		}
		#endregion

		#region 门禁状态改变通知
		/// <summary>
		/// 门禁状态改变通知
		/// </summary>
		/// <param name="BoxNO">箱子号码</param>
		/// <param name="cType">状态：on/off</param>
		public static void NotifyGate(string BoxNO, string cType)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(303, BoxNO + "\r" + cType);
		}
		#endregion
	
		#region 箱头错误通知
		/// <summary>
		/// 箱头错误通知
		/// </summary>
		/// <param name="BoxNO">箱子号码</param>
		/// <param name="data">错误信息</param>
		public static void NotifyError(string BoxNO, string data)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(304, BoxNO + "\r" + data);
		}
		#endregion
	
		#region 箱头信件数目改变通知
		/// <summary>
		/// 箱头信件数目改变通知
		/// </summary>
		/// <param name="BoxNO">箱子号码</param>
		/// <param name="urgent">急件灯状态：true/false</param>
		/// <param name="letterCount">信件数目</param>
		public static void NotifyLetter(string BoxNO, string urgent, int letterCount)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(305, BoxNO + "\r" + urgent + "\r" + letterCount.ToString());
		}
		#endregion

		#region 箱头单位改变通知
		/// <summary>
		/// 箱头单位改变通知
		/// </summary>
		/// <param name="BoxNO">箱子号码</param>
		/// <param name="urgent">单位名称</param>
		/// <param name="data">单位号码</param>
		public static void NotifyBoxUnit(string BoxNO, string UnitName, string UnitNO)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(306, BoxNO + "\r" + UnitName + "\r" + UnitNO);
		}
		#endregion

		#region 箱头连接状态通知
		/// <summary>
		/// 箱头连接状态通知
		/// </summary>
		/// <param name="BoxNO">箱子号码</param>
		/// <param name="cType">连接状态：on/off</param>
		public static void NotifyConnect(string BoxNO, string cType)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(307, BoxNO + "\r" + cType);
		}
		#endregion



        #region 报刊分发使用，箱头状态通知
        /// <summary>
        /// 报刊分发使用，箱头状态通知
        /// </summary>
        /// <param name="BoxNO">箱子号码</param>
        /// <param name="State">箱子状态：0：没有投入报刊,1：报刊已投入,2：箱头有错误。</param>
        public static void NotifyNewsState(string BoxNO, int State)
        {
            ClientDataParse.ClientDataParse.SendNewsBroadCast(503, BoxNO + "\r" + State.ToString());
        }
        #endregion
	}
}
