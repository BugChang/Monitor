using System;
using System.Collections.Generic;
using System.Text;

namespace DataBase
{
    public enum BarCodeType
    {
        ��Ч = 0,
        �����Ѿ�Ͷ�� = 10,
        û��Ԥ�� = 1,
        Ψһָ�� = 2,
        Ψһָ������Ͷ�� = 201,
        ΨһֱͶ = 3,
        ͨ��ַ� = 4,
        ͨ��ַ�ȫ��Ͷ�� = 401,
        ͨ���շ� = 5,
        ͨ��ģ�� = 6,
        ͨ���շ�ģ�� = 7,
        ����Ա = 21,
        ����Ա = 22,
        û��ָ���ַ� = 9,
        ����û��Ԥ�� = 11
    }
    public enum CodeGuiZeType
    {
        Ψһ�� = 1,
        ͨ�� = 2
    }
    public class SendBoxList
    {
        public int BoxNo;
        public int Count;
        public string msg;
    }

	public enum em_CardPrintType
	{
		��Ȩ�� = 0,
		�����嵥 = 1,
		�ռ��嵥 = 2,
		�շ��嵥 = 3
	}

	public class UserGetBoxInfo
	{
		public int BoxNo;
		public string ֤�����;
		public string ��λ����;
		public string �û�����;

		/// <summary>
		/// �û����Դ�ӡ���嵥������
		/// <para>0��û��Ȩ�ޣ�ʲô�嵥�����ܴ�ӡ</para>
		/// <para>1�����Դ�ӡ�����嵥</para>
		/// <para>2�����Դ�ӡȡ����ӡ�ռ��嵥</para>
		/// <para>3�������Դ�ӡ</para>
		/// </summary>
		public em_CardPrintType CardPrintType;
	}

}
