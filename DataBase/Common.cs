using System;
using System.Collections.Generic;
using System.Text;

namespace DataBase
{
    public enum BarCodeType
    {
        无效 = 0,
        条码已经投箱 = 10,
        没有预发 = 1,
        唯一指定 = 2,
        唯一指定开门投件 = 201,
        唯一直投 = 3,
        通码分发 = 4,
        通码分发全组投箱 = 401,
        通码普发 = 5,
        通码模板 = 6,
        通码普发模板 = 7,
        管理员 = 21,
        交换员 = 22,
        没有指定分发 = 9,
        条码没有预发 = 11
    }
    public enum CodeGuiZeType
    {
        唯一码 = 1,
        通码 = 2
    }
    public class SendBoxList
    {
        public int BoxNo;
        public int Count;
        public string msg;
    }

	public enum em_CardPrintType
	{
		无权限 = 0,
		发件清单 = 1,
		收件清单 = 2,
		收发清单 = 3
	}

	public class UserGetBoxInfo
	{
		public int BoxNo;
		public string 证卡编号;
		public string 单位名称;
		public string 用户名称;

		/// <summary>
		/// 用户可以打印的清单的类型
		/// <para>0：没有权限，什么清单都不能打印</para>
		/// <para>1：可以打印发件清单</para>
		/// <para>2：可以打印取件打印收件清单</para>
		/// <para>3：都可以打印</para>
		/// </summary>
		public em_CardPrintType CardPrintType;
	}

}
