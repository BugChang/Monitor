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
		const int FV_ERROR_UNKNOWN = -1;						// 未知错误
		const int FV_ERROR_NOT_SUPPORTED = -2;						// 接口函数未实现
		const int FV_ERROR_LIBRARY_NOT_ININIALIZED = -3;						// 库未初始化
		const int FV_ERROR_NOT_ENOUGH_MEMORY = -4;						// 内存不足
		const int FV_ERROR_TIMEOUT = -5;						// 超时
		const int FV_ERROR_INVALID_PARAMETER = -6;						// 输入参数不合法
		const int FV_ERROR_INVALID_DATA = -7;						// 传入的数据内容错误
		const int FV_ERROR_INVALID_OUTPUT_POINTER = -8;						// 输出参数不合法
		const int FV_ERROR_BUFFER_TOO_SMALL = -9;						// 缓冲区太小
		const int FV_ERROR_NOT_FOUND = -10;						// 采集设备/识别模块不存在
		const int FV_ERROR_CLOSED = -11;						// 采集设备/识别算法/识别模块已关闭
		const int FV_ERROR_ALREADY_OPENED = -12;						// 采集设备/识别模块已被使用
		const int FV_ERROR_WORKING = -13;						// 采集设备/识别模块正在工作中
		const int FV_ERROR_INVALID_PARAMETER_COMBINATION = -14;						// 采集设备/识别算法/识别模块的参数组合不正确
		const int FV_ERROR_CANCELED = -15;						// 操作被用户终止
		const int FV_ERROR_NAME_EXISTS = -16;						// 组/模板重名
		const int FV_ERROR_GROUP_NOT_FOUND = -17;						// 组未找到
		const int FV_ERROR_TEMPLATE_NOT_FOUND = -18;						// 模板未找到
		const int FV_ERROR_MAX_NUM_REACHED = -19;						// 组/模版数量已满
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
		/// <param name="nIndex">默认传0</param>
		/// <returns></returns>
		[DllImport("VeinKeyOc_NoDev.dll", SetLastError = true)]
		public extern static UInt32 ynVeinSensorOpen(int nIndex);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hSensor"></param>
		/// <param name="nButtonStatus"></param>
		/// <param name="unWaitTime">默认-1</param>
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
					//算法库初始化失败
					return "";
				}
				handles.Add(hList);

				for (int i = 0; i < sEnrollData.Count; i++)
				{
					UInt32 h = ynCreateTemplateFromMemoryBase64(sEnrollData[i]);
					if (h == 0)
					{
						LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "指静脉添加模板错误，模板序号：" + i.ToString());
						continue;
					}
					handles.Add(h);
					ret = ynTemplateListAddItem(hList, h, i.ToString());
					if (ret != FV_OK)
					{
						LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "指静脉添加模板错误: " + ret.ToString());
						return "";
					}
				}

				byte[] buf = new byte[16];
				UInt32 h1 = ynCreateTemplateFromMemoryBase64(sCharaData);
				if (h1 == 0)
				{
					LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "指静脉比对特征文件加载错误。 ");
					return "";
				}
				handles.Add(h1);
				ret = ynSearch(hList, h1, buf);
				if (ret != FV_OK)
				{
					LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "指静脉比较错误: " + ret.ToString());
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
