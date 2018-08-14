using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DataBase
{
    public class XGComApi
    {
		//[DllImport("XGComApi.dll", EntryPoint = "XG_CreateVein")]
		//public static extern int XG_CreateVein(ref int pHandle, int iAddr, int pRead, int pWrite, int pPrint);

		////2013.7.18���Ϊbase64�ַ�������
		//[DllImport("XGComApi.dll", EntryPoint = "XG_CharaVerify")]
		//public static extern string XG_CharaVerify(int hHandle, string pEnroll, int EnrollSize, string pChara, int CharaSize);

		//[DllImport("XGComApi.dll", EntryPoint = "XG_DestroyVein")]
		//public static extern int XG_DestroyVein(int hHandle);

		[DllImport("XGComApi.dll", SetLastError = true)]
		public static extern int XG_CreateVein(ref IntPtr VeinHandle, int UserNum);

		[DllImport("XGComApi.dll", SetLastError = true)]
		public static extern int XG_DestroyVein(IntPtr VeinHandle);

		[DllImport("XGComApi.dll", SetLastError = true)]
		public static extern int XG_DelEnrollData(IntPtr VeinHandle, UInt32 User);

		[DllImport("XGComApi.dll")]
		unsafe public static extern int XG_SaveEnrollData(IntPtr VeinHandle, UInt32 User, byte Group, byte* pData, UInt16 Size);

		[DllImport("XGComApi.dll")]
		public static extern int XG_SetSecurity(IntPtr VeinHandle, byte Level);

		[DllImport("XGComApi.dll")]
		unsafe public static extern int XG_Verify(IntPtr VeinHandle, UInt32* pUser, byte* pBuf, UInt32 size, byte Group, UInt16* pQuality);

		//��DLL��ѹ�����ܣ���ȡ�����ݽ�ѹ���ܻ�ԭ��ԭʼ���ݣ�����д������ֻ��д��ԭʼ����
		[DllImport("XGComApi.dll")]
		unsafe public static extern int XG_DecodeEnrollData(IntPtr pSrc, IntPtr pDest);

		/*
        public static bool CompareData(string sEnrollData, string sCharaData)
        {
            bool bRet = false;
            try
            {
                //�Ǽ����ݲ��Ǵ��ļ���ȡ
                int hHandle = 0;
                int ret = XG_CreateVein(ref hHandle, 0, 0, 0, 0);

                string NewEnrollStr = XG_CharaVerify(hHandle, sEnrollData, sEnrollData.Length, sCharaData, sCharaData.Length);
                if (NewEnrollStr != null)
                {
                    //sEnrollData = NewEnrollStr;
                    bRet = true;
                }

                XG_DestroyVein(hHandle);
            }
            catch
            {
            }
            return bRet;
        }
		*/

		public unsafe static string CompareData(List<string> sEnrollData, string sCharaData)
		{
			string szRet = "";
			if (sEnrollData.Count <= 0) return "";

			IntPtr VeinHandle = (IntPtr)0;
			try
			{
				int ret;
				ret = XG_CreateVein(ref VeinHandle, sEnrollData.Count);
				if (ret != 0)
				{
					//�㷨���ʼ��ʧ��
					return "";
				}

				//for (uint i = 0; i < sEnrollData.Count; i++)
				//{
				//    string sdata = sEnrollData[(int)i];
				//    int ll = sdata.Length % 4;
				//    sdata = sdata.Substring(0, sdata.Length - ll);
				//    byte[] buf = Convert.FromBase64String(sdata);
				//    fixed (byte* abuf = buf)
				//    {
				//        ret = XG_DelEnrollData(VeinHandle, i + 1);
				//        ret = XG_SaveEnrollData(VeinHandle, i + 1, 0, abuf, (UInt16)buf.Length);
				//    }
				//}

				for (uint i = 0; i < sEnrollData.Count; i++)
				{
					string sdata = sEnrollData[(int)i];
					byte[] buf = System.Text.Encoding.ASCII.GetBytes(sdata);
					fixed (byte* abuf = buf)
					{
						ret = XG_DelEnrollData(VeinHandle, i + 1);
						ret = XG_SaveEnrollData(VeinHandle, i + 1, 0, abuf, (UInt16)buf.Length);
					}
				}

				UInt16 qp = 0;
				UInt32 USER = 0;
				byte[] buf1 = Convert.FromBase64String(sCharaData);
				fixed (byte* arrayChara = buf1)
				{
					//���ð�ȫ�ȼ�
					ret = XG_SetSecurity(VeinHandle, 2);
					ret = XG_Verify(VeinHandle, &USER, arrayChara, (uint)buf1.Length, 0, &qp);
				}
				if (ret == 0 && USER>0)
				{
					//�ɹ�
					szRet = sEnrollData[(int)USER - 1];
				}
			}
			catch ( Exception ex)
			{
				LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "CompareData: " + ex.ToString());
				szRet = "";
			}
			finally
			{
				if (VeinHandle!=(IntPtr)0)
					XG_DestroyVein(VeinHandle);
			}
			return szRet;
		}
    }

}
