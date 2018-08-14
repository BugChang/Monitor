using System;
using System.IO;

namespace LogInfo
{
    public enum LogType
    {
        Error = 0,
        Info = 1,
        Warnning = 2
    }

	/// <summary>
	/// Class1 的摘要说明。
	/// </summary>
	public class Log
	{
		public static bool isDebug = true;

		private static System.Net.Sockets.UdpClient s = null;

		static Log()
		{
			s = new System.Net.Sockets.UdpClient("127.0.0.1", 44444);
		}

        public static void WriteInfo(LogType lt, string msg)
		{
			if(!isDebug)
				return;

			byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(msg);

			s.Send(buf, buf.Length);
		}

        public static void WriteFileLog(string strLog)
        {
            //if (!Constant.IsDebug)
            //   return;

            try
            {
                string pathd = System.IO.Directory.GetCurrentDirectory();  //当前程序路径
                pathd += "\\log";
                if (!Directory.Exists(pathd))
                    Directory.CreateDirectory(pathd);
                string FileName = pathd + "\\" + DateTime.Now.ToString("yy_MM_dd_") + Constant.LogFileName;
                string TimeStr = "记录时间：" + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                byte[] b = System.Text.Encoding.GetEncoding(936).GetBytes(TimeStr + strLog);
                System.IO.FileStream f1 = System.IO.File.Open(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                f1.Seek(0, SeekOrigin.End);
                f1.Write(b, 0, b.Length);
                f1.Close();
            }
            catch
            {
            }
        }

	}
}
