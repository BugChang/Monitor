using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DataProcess
{


	/// <summary>
	/// Class1 的摘要说明。
	/// </summary>
	public class DvrCommand
	{
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private unsafe struct stRecCommand
        {
            /// <summary>
            /// 1:start Record, 2：stop Record，5:Snap
            /// </summary>
            [FieldOffset(0)]
            public int iType;
            [FieldOffset(4)]
            public fixed byte Server[16];
            [FieldOffset(20)]
            public int Port;
            [FieldOffset(24)]
            public int ChannelIndex;
            [FieldOffset(28)]
            public fixed byte FileName[256];
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private unsafe struct stRePlayCommand
        {
            /// <summary>
            /// 11:get Record, 12:get Snap
            /// </summary>
            [FieldOffset(0)]
            public int iType;
            [FieldOffset(4)]
            public fixed byte FileName[256];
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private unsafe struct stRePlayResponse
        {
            /// <summary>
            /// 0:错误没有找到, 其他：返回的文件长度
            /// </summary>
            [FieldOffset(0)]
            public int iFileLen;
            /// <summary>
            /// 当前缓冲区开始的地址
            /// </summary>
            [FieldOffset(4)]
            public int iCurrentPosition;
            [FieldOffset(8)]
            public fixed byte BinaryData[2048];
        }

		private static System.Net.Sockets.UdpClient s = null;

        static DvrCommand()
		{
			s = new System.Net.Sockets.UdpClient(LogInfo.Constant.DVRServer_IP, LogInfo.Constant.DVRServer_Port);
		}

        public static bool StartRec(string Server, int Port, int Channel, ref string FileName)
        {
            return SendCommand(Server, Port, Channel, 1, ref FileName);
        }
        public static bool StopRec(string Server, int Port, int Channel)
        {
            string FileName = "";
            bool b = SendCommand(Server, Port, Channel, 2, ref FileName);
            if (!b) return false;
            return (FileName.CompareTo("ok")==0);
        }
        public static bool GetPicture(string Server, int Port, int Channel, ref string FileName)
        {
            return SendCommand(Server, Port, Channel, 5, ref FileName);
        }

        /// <summary>
        /// 发送录像或者抓图命令
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Port"></param>
        /// <param name="Channel"></param>
        /// <param name="iType">1:start Record, 2：stop Record，5:Snap</param>
        /// <returns></returns>
        public unsafe static bool SendCommand(string Server, int Port, int Channel, int iType, ref string FileName)
		{
            bool bRet = false;
            try
            {
                int len = sizeof(stRecCommand);
                byte[] buf = new byte[len];
                byte[] tmp = System.Text.Encoding.Default.GetBytes(Server);
                fixed (byte* pbuf = buf)
                {
                    stRecCommand* comm = (stRecCommand*)pbuf;
                    comm->iType = iType;
                    comm->Port = Port;
                    System.Runtime.InteropServices.Marshal.Copy(tmp, 0, (IntPtr)comm->Server, tmp.Length);
					comm->ChannelIndex = Channel;
                }
                int i = s.Send(buf, buf.Length);
                if (i > 0)
                {
                    i = 0;
                    while (i<100)
                    {
                        if (s.Available > 0) break;
                        System.Threading.Thread.Sleep(10);
                        i++;
                    }
                    if (s.Available > 0)
                    {
                        System.Net.IPEndPoint ep = new System.Net.IPEndPoint(0,0);
                        buf = s.Receive(ref ep);
                        if (buf.Length == len)
                        {
                            tmp = new byte[256];
                            int count = 0;
                            fixed (byte* pbuf = buf)
                            {
                                stRecCommand* comm = (stRecCommand*)pbuf;
                                while (comm->FileName[count] != 0)
                                    count++;
                                System.Runtime.InteropServices.Marshal.Copy((IntPtr)comm->FileName, tmp, 0, count);
                            }
                            FileName = System.Text.Encoding.Default.GetString(tmp, 0, count);
                            bRet = true;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Warnning, ee.ToString());
            }
            return bRet;
		}

        /// <summary>
        /// 发送录像或者抓图命令
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Port"></param>
        /// <param name="Channel"></param>
        /// <param name="iType">1:start Record, 2：stop Record，5:Snap</param>
        /// <returns></returns>
        public unsafe static bool GetFileCommand(string FileName, out byte[] data)
        {
            bool bRet = false;
            data = null;
            try
            {
                int len = sizeof(stRePlayCommand);
                byte[] buf = new byte[len];
                byte[] tmp = System.Text.Encoding.Default.GetBytes(FileName);
                fixed (byte* pbuf = buf)
                {
                    stRePlayCommand* comm = (stRePlayCommand*)pbuf;
                    comm->iType = 12;
                    System.Runtime.InteropServices.Marshal.Copy(tmp, 0, (IntPtr)comm->FileName, tmp.Length);
                }
                int i = s.Send(buf, buf.Length);
                if (i > 0)
                {
                    i = 0;
                    len = sizeof(stRePlayResponse);
                    while (i < 100)
                    {
                        if (s.Available > 0) break;
                        System.Threading.Thread.Sleep(10);
                        i++;
                    }
                    if (s.Available > 0)
                    {
                        //接收数据，多个反馈
                        System.Net.IPEndPoint ep = new System.Net.IPEndPoint(0, 0);
                        buf = s.Receive(ref ep);
                        if (buf.Length == len)
                        {
                            fixed (byte* pbuf = buf)
                            {
                                stRePlayResponse* comm = (stRePlayResponse*)pbuf;
                                if (data == null)
                                    data = new byte[comm->iFileLen];
                                System.Runtime.InteropServices.Marshal.Copy((IntPtr)comm->BinaryData, data, comm->iCurrentPosition, (comm->iFileLen - comm->iCurrentPosition > 2048 ? 2048 : comm->iFileLen - comm->iCurrentPosition));
                            }
                            //其他包
                            while (true)
                            {
                                i = 0;
                                while (i < 100)
                                {
                                    if (s.Available > 0) break;
                                    System.Threading.Thread.Sleep(10);
                                    i++;
                                }
                                if (s.Available <= 0)
                                {
                                    bRet = false; break;
                                }
                                buf = s.Receive(ref ep);
                                if (buf.Length == len)
                                {
                                    fixed (byte* pbuf = buf)
                                    {
                                        stRePlayResponse* comm = (stRePlayResponse*)pbuf;
                                        System.Runtime.InteropServices.Marshal.Copy((IntPtr)comm->BinaryData, data, comm->iCurrentPosition, (comm->iFileLen - comm->iCurrentPosition > 2048 ? 2048 : comm->iFileLen - comm->iCurrentPosition));
                                        if (comm->iCurrentPosition + 2048 >= comm->iFileLen)
                                        {
                                            bRet = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Warnning, ee.ToString());
            }
            return bRet;
        }
    }
}
