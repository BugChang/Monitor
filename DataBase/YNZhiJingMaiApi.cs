using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DataBase
{
    public class YNZhiJingMai
    {
		public const int FV_OK = 1;
		const int YN_NAME_MAX = 255;

		const int YN_LED_OFF = 0;
		const int YN_LED_RED = 1;
		const int YN_LED_GREEN = 2;
		const int YN_LED_ORANGE = 3;

		const int YN_SIGNAL_SUCCEEDED = 1; // Beeeep
		const int YN_SIGNAL_FAILED = 2; // Beep Beep Beep
		const int YN_SIGNAL_NOTICE = 3; // Bep Bep Bep Bep

		// Error Code
		const int FV_ERROR_UNKNOWN = -1;						// δ֪����
		const int FV_ERROR_NOT_SUPPORTED = -2;						// �ӿں���δʵ��
		const int FV_ERROR_LIBRARY_NOT_ININIALIZED = -3;						// ��δ��ʼ��
		const int FV_ERROR_NOT_ENOUGH_MEMORY = -4;						// �ڴ治��
		const int FV_ERROR_TIMEOUT = -5;						// ��ʱ
		const int FV_ERROR_INVALID_PARAMETER = -6;						// ����������Ϸ�
		const int FV_ERROR_INVALID_DATA = -7;						// ������������ݴ���
		const int FV_ERROR_INVALID_OUTPUT_POINTER = -8;						// ����������Ϸ�
		const int FV_ERROR_BUFFER_TOO_SMALL = -9;						// ������̫С
		const int FV_ERROR_NOT_FOUND = -10;						// �ɼ��豸/ʶ��ģ�鲻����
		const int FV_ERROR_CLOSED = -11;						// �ɼ��豸/ʶ���㷨/ʶ��ģ���ѹر�
		const int FV_ERROR_ALREADY_OPENED = -12;						// �ɼ��豸/ʶ��ģ���ѱ�ʹ��
		const int FV_ERROR_WORKING = -13;						// �ɼ��豸/ʶ��ģ�����ڹ�����
		const int FV_ERROR_INVALID_PARAMETER_COMBINATION = -14;						// �ɼ��豸/ʶ���㷨/ʶ��ģ��Ĳ�����ϲ���ȷ
		const int FV_ERROR_CANCELED = -15;						// �������û���ֹ
		const int FV_ERROR_NAME_EXISTS = -16;						// ��/ģ������
		const int FV_ERROR_GROUP_NOT_FOUND = -17;						// ��δ�ҵ�
		const int FV_ERROR_TEMPLATE_NOT_FOUND = -18;						// ģ��δ�ҵ�
		const int FV_ERROR_MAX_NUM_REACHED = -19;						// ��/ģ����������
		const int FV_ERROR_USER = -20000;
		const int FV_ERROR_USER_BEGIN = -20000;
		const int FV_ERROR_USER_END = -10000;

		const int YN_BASE64TEMPLATE_SIZE = 2732;


		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynGetLastError();

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynCloseHandle(UInt32 hObject);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nIndex">Ĭ�ϴ�0</param>
		/// <returns></returns>
		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynVeinSensorOpen(int nIndex);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hSensor"></param>
		/// <param name="nButtonStatus"></param>
		/// <param name="unWaitTime">Ĭ��-1</param>
		/// <returns></returns>
		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynButton(UInt32 hSensor, int nButtonStatus, UInt32 unWaitTime);

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynLED(UInt32 hSensor, int nLEDStatus);

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynBuzzer(UInt32 hSensor, bool bOn);

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static bool ynSignal(UInt32 hSensor, int nSignal);

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static void ynCancel(UInt32 hSensor);

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynRegister(UInt32 hSensor);

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynSaveTemplateFile(UInt32 hTemplate, string filename);

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynLoadTemplateFile(string filename);

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynCreateTemplateList();

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynTemplateListAddItem(UInt32 hList, UInt32 hTemplate, string name);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hSensor"></param>
		/// <param name="hList"></param>
		/// <param name="matchName">YN_NAME_MAX</param>
		/// <returns></returns>
		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynIdentify(UInt32 hSensor, UInt32 hList, byte[] matchName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hList"></param>
		/// <param name="hTemplate"></param>
		/// <param name="matchName">YN_NAME_MAX</param>
		/// <returns></returns>
		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynSearch(UInt32 hList, UInt32 hTemplate, byte[] matchName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hDevice"></param>
		/// <param name="SerialNumber">16</param>
		/// <returns></returns>
		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynDeviceSerial(UInt32 hDevice, byte[] SerialNumber);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hTemplate"></param>
		/// <param name="pTemplateBase64Buffer"></param>
		/// <param name="unTemplateBase64BufferSize"></param>
		/// <returns></returns>
		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static int ynGetTemplateDataBase64(UInt32 hTemplate, byte[] pTemplateBase64Buffer, UInt32 unTemplateBase64BufferSize);

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynCreateTemplateFromMemoryBase64(string pTemplateBase64Data);

		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static void ynGetVersionString(byte[] versionstring);

		public unsafe static string CompareData(List<string> sEnrollData, string sCharaData)
		{
			string szRet = "";
			if (sEnrollData.Count <= 0) return "";

			List<UInt32> handles = new List<uint>();

			try
			{
				UInt32 ret = 0;
				UInt32 hList = ynCreateTemplateList();
				if (hList == 0)
				{
					//�㷨���ʼ��ʧ��
					return "";
				}
				handles.Add(hList);

				for (int i = 0; i < sEnrollData.Count; i++)
				{
					UInt32 h = ynCreateTemplateFromMemoryBase64(sEnrollData[i]);
					if (h == 0)
					{
						LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "ָ�������ģ�����ģ����ţ�" + i.ToString());
						continue;
					}
					handles.Add(h);
					ret = ynTemplateListAddItem(hList, h, i.ToString());
					if (ret != FV_OK)
					{
						LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "ָ�������ģ�����: " + ret.ToString());
						return "";
					}
				}

				byte[] buf = new byte[16];
				UInt32 h1 = ynCreateTemplateFromMemoryBase64(sCharaData);
				if (h1 == 0)
				{
					LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "ָ�����ȶ������ļ����ش��� ");
					return "";
				}
				handles.Add(h1);
				ret = ynSearch(hList, h1, buf);
				if (ret != FV_OK)
				{
					LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "ָ�����Ƚϴ���: " + ret.ToString());
					return "";
				}
				for (int i = 0; i < buf.Length; i++)
				{
					if (buf[i] == 0)
					{
						string str = System.Text.Encoding.ASCII.GetString(buf, 0, i);
						if (str != "")
						{
							int j = Convert.ToInt32(str);
							return sEnrollData[j];
						}
						break;
					}
				}
			}
			catch ( Exception ex)
			{
				LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "CompareData: " + ex.ToString());
				szRet = "";
			}
			finally
			{
				foreach (UInt32 h in handles)
					ynCloseHandle(h);
			}
			return szRet;
		}
    }

}
