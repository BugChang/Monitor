using System;

namespace LogInfo
{
	/// <summary>
	/// Win32API 的摘要说明。
	/// </summary>
	public class Win32API
	{
		[System.Runtime.InteropServices.DllImport("KERNEL32.DLL", EntryPoint="GetTickCount")]
		public static extern UInt32 GetTickCount();

		[System.Runtime.InteropServices.DllImport("barDll\\TBarPrinter.dll", EntryPoint = "PrintTempletBarXML")]
		public static extern bool PrintTempletBarXML(string XMLTempleFilePath, string XMLDataFilePath, string PrinterName, ref int pError, byte[] msg);
	}
}
