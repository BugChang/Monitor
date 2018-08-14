using System;

namespace DataProcess
{
	/// <summary>
	/// SendClientNotify ��ժҪ˵����
	/// </summary>
	public class SendClientNotify
	{
		#region ��״̬�ı�֪ͨ
		/// <summary>
		/// ��״̬�ı�֪ͨ
		/// </summary>
		/// <param name="BoxNO">���Ӻ���</param>
		/// <param name="LampIndex">1:��ȡ��3:����</param>
		/// <param name="cType">״̬��on/off</param>
		public static void NotifyLamp(string BoxNO, string LampIndex, string cType)
		{
			ClientDataParse.ClientDataParse.SendBroadCast(301, BoxNO + "\r" + LampIndex + "\r" + cType);
		}
		#endregion

		#region ��״̬�ı�֪ͨ
		/// <summary>
		/// ��״̬�ı�֪ͨ
		/// </summary>
		/// <param name="BoxNO">���Ӻ���</param>
		/// <param name="cType">״̬��on/off</param>
		public static void NotifyDoor(string BoxNO, string cType)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(302, BoxNO + "\r" + cType);
		}
		#endregion

		#region �Ž�״̬�ı�֪ͨ
		/// <summary>
		/// �Ž�״̬�ı�֪ͨ
		/// </summary>
		/// <param name="BoxNO">���Ӻ���</param>
		/// <param name="cType">״̬��on/off</param>
		public static void NotifyGate(string BoxNO, string cType)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(303, BoxNO + "\r" + cType);
		}
		#endregion
	
		#region ��ͷ����֪ͨ
		/// <summary>
		/// ��ͷ����֪ͨ
		/// </summary>
		/// <param name="BoxNO">���Ӻ���</param>
		/// <param name="data">������Ϣ</param>
		public static void NotifyError(string BoxNO, string data)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(304, BoxNO + "\r" + data);
		}
		#endregion
	
		#region ��ͷ�ż���Ŀ�ı�֪ͨ
		/// <summary>
		/// ��ͷ�ż���Ŀ�ı�֪ͨ
		/// </summary>
		/// <param name="BoxNO">���Ӻ���</param>
		/// <param name="urgent">������״̬��true/false</param>
		/// <param name="letterCount">�ż���Ŀ</param>
		public static void NotifyLetter(string BoxNO, string urgent, int letterCount)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(305, BoxNO + "\r" + urgent + "\r" + letterCount.ToString());
		}
		#endregion

		#region ��ͷ��λ�ı�֪ͨ
		/// <summary>
		/// ��ͷ��λ�ı�֪ͨ
		/// </summary>
		/// <param name="BoxNO">���Ӻ���</param>
		/// <param name="urgent">��λ����</param>
		/// <param name="data">��λ����</param>
		public static void NotifyBoxUnit(string BoxNO, string UnitName, string UnitNO)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(306, BoxNO + "\r" + UnitName + "\r" + UnitNO);
		}
		#endregion

		#region ��ͷ����״̬֪ͨ
		/// <summary>
		/// ��ͷ����״̬֪ͨ
		/// </summary>
		/// <param name="BoxNO">���Ӻ���</param>
		/// <param name="cType">����״̬��on/off</param>
		public static void NotifyConnect(string BoxNO, string cType)
		{
            ClientDataParse.ClientDataParse.SendBroadCast(307, BoxNO + "\r" + cType);
		}
		#endregion



        #region �����ַ�ʹ�ã���ͷ״̬֪ͨ
        /// <summary>
        /// �����ַ�ʹ�ã���ͷ״̬֪ͨ
        /// </summary>
        /// <param name="BoxNO">���Ӻ���</param>
        /// <param name="State">����״̬��0��û��Ͷ�뱨��,1��������Ͷ��,2����ͷ�д���</param>
        public static void NotifyNewsState(string BoxNO, int State)
        {
            ClientDataParse.ClientDataParse.SendNewsBroadCast(503, BoxNO + "\r" + State.ToString());
        }
        #endregion
	}
}
