using System;
using System.IO;

namespace CanClass
{

	/// <summary>
	/// Class1 ��ժҪ˵����
	/// </summary>
	public class Log
	{
        public static void WriteFileLog(string strLog)
        {
            try
            {
                string pathd = System.IO.Directory.GetCurrentDirectory();  //��ǰ����·��
                pathd += "\\CanLog";
                if (!Directory.Exists(pathd))
                    Directory.CreateDirectory(pathd);
                string FileName = pathd + "\\" + DateTime.Now.ToString("yy_MM_dd") + "_CanLog.log";
                string TimeStr = "��¼ʱ�䣺" + DateTime.Now.ToString("T") + "\r\n";
                byte[] b = System.Text.Encoding.GetEncoding(936).GetBytes(TimeStr + strLog + "\r\n\r\n");
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
