using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace CameraClass
{
    public class Camera
    {
        public static bool SetBaud(string Server, int Port)
        {
            bool bRet = false;
            //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                byte[] buf = new byte[128];
                s.Connect(Server, Port);
                //修改串口初始波特率指令： 56 00 31 06 04 02 00 08 XX YY
                //返回：76 00 31 00
                //XX YY 速率
                //AE C8 9600
                //56 E4 19200
                //2A F2 38400
                //1C 4C 57600
                //0D A6 115200
                byte[] command = new byte[] { 0x56, 0x00, 0x31, 0x06, 0x04, 0x02, 0x00, 0x08, 0x0D, 0xA6 };
                byte[] ret = new byte[] { 0x76, 0x00, 0x31, 0x00 };
                int i = s.Send(command);
                if (i != command.Length)
                    return false;
                UInt32 dt = LogInfo.Win32API.GetTickCount();
                i = 0;
                while (i < ret.Length && (LogInfo.Win32API.GetTickCount() - dt) < 5000)
                {
                    i += s.Receive(buf, i, buf.Length - i, SocketFlags.None);
                }
                if (i < ret.Length)
                    return false;
                for (i = 0; i < ret.Length; i++)
                {
                    if (buf[i] != ret[i])
                        return false;
                }
                //设置拍照图片大小指令： （默认大小为：320 * 240）
                //56 00 31 05 04 01 00 19 11 （320*240） 返回：76 00 31 00 00
                //56 00 31 05 04 01 00 19 00 （640*480）
                //56 00 31 05 04 01 00 19 22 （160*120）
                //注意：设置图片大小指令后，需要复位一次，新的设置值才会生效！
                command = new byte[] { 0x56, 0x00, 0x31, 0x05, 0x04, 0x01, 0x00, 0x19, 0x11 };
                ret = new byte[] { 0x76, 0x00, 0x31, 0x00, 0x00 };
                i = s.Send(command);
                if (i != command.Length)
                    return false;
                dt = LogInfo.Win32API.GetTickCount();
                i = 0;
                while (i < ret.Length && (LogInfo.Win32API.GetTickCount() - dt) < 5000)
                {
                    i += s.Receive(buf, i, buf.Length - i, SocketFlags.None);
                }
                if (i < ret.Length)
                    return false;
                for (i = 0; i < ret.Length; i++)
                {
                    if (buf[i] != ret[i])
                        return false;
                }
                //设置拍照图片压缩率指令：56 00 31 05 01 01 12 04 XX 返回：76 00 31 00 00
                //XX 一般选36 （范围：00 ----FF）
                command = new byte[] { 0x56, 0x00, 0x31, 0x05, 0x01, 0x01, 0x12, 0x04, 0x36 };
                ret = new byte[] { 0x76, 0x00, 0x31, 0x00, 0x00 };
                i = s.Send(command);
                if (i != command.Length)
                    return false;
                dt = LogInfo.Win32API.GetTickCount();
                i = 0;
                while (i < ret.Length && (LogInfo.Win32API.GetTickCount() - dt) < 5000)
                {
                    i += s.Receive(buf, i, buf.Length - i, SocketFlags.None);
                }
                if (i < ret.Length)
                    return false;
                for (i = 0; i < ret.Length; i++)
                {
                    if (buf[i] != ret[i])
                        return false;
                }

                //复位指令：56 00 26 00 返回：76 00 26 00 +DSP版本信息
                command = new byte[] { 0x56, 0x00, 0x26, 0x00 };
                ret = new byte[] { 0x76, 0x00, 0x26, 0x00 };
                i = s.Send(command);
                if (i != command.Length)
                    return false;
                dt = LogInfo.Win32API.GetTickCount();
                i = 0;
                while (i < ret.Length && (LogInfo.Win32API.GetTickCount() - dt) < 5000)
                {
                    i += s.Receive(buf, i, buf.Length - i, SocketFlags.None);
                }
                if (i < ret.Length)
                    return false;
                for (i = 0; i < ret.Length; i++)
                {
                    if (buf[i] != ret[i])
                        return false;
                }

                bRet = true;
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Warnning, "设置速率出错：" + ee.ToString());
            }
            finally
            {
                if (s != null)
                {
                    if (s.Connected)
                        s.Close();
                }
            }
            return bRet;
        }

        public static bool GetPicture(string Server, int Port, ref string FileData)
        {
            int TimeWait = 3000000;
            bool bRet = false;
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            FileData = "";
            try
            {
                byte[] buf = new byte[128];
                s.Connect(Server, Port);
                //清空缓存
                //清空图片缓存指令：56 00 36 01 03 返回：76 00 36 00 00
                byte[] command = new byte[] { 0x56, 0x00, 0x36, 0x01, 0x03 };
                byte[] ret = new byte[] { 0x76, 0x00, 0x36, 0x00, 0x00 };
                int i = s.Send(command);
                if (i != command.Length)
                    return false;
                i = 0;
                while (i < ret.Length && s.Poll(TimeWait, SelectMode.SelectRead))
                {
                    i += s.Receive(buf, i, buf.Length - i, SocketFlags.None);
                }

                //照相
                //拍照指令：56 00 36 01 00 返回：76 00 36 00 00
                command = new byte[] { 0x56, 0x00, 0x36, 0x01, 0x00 };
                ret = new byte[] { 0x76, 0x00, 0x36, 0x00, 0x00, 0x00, 0x00 };
                i = s.Send(command);
                if (i != command.Length)
                    return false;
                i = 0;
                while (i < ret.Length - 2 && s.Poll(TimeWait, SelectMode.SelectRead))
                {
                    i += s.Receive(buf, i, buf.Length - i, SocketFlags.None);
                }
                if (i != ret.Length-2)
                    return false;
                for (i = 0; i < ret.Length-2; i++)
                {
                    if (buf[i] != ret[i])
                        return false;
                }
                //取得长度
                //56 00 34 01 00 返回：76 00 34 00 04 00 00 XX YY,  XX YY -------图片数据长度，XX 为高位字节，YY 为低位字节
                command[2] = (byte)0x34;
                ret[2] = (byte)0x34; ret[4] = (byte)0x04;
                i = s.Send(command);
                if (i != command.Length)
                    return false;
                i = 0;
                while (i < ret.Length + 2 && s.Poll(TimeWait, SelectMode.SelectRead))
                {
                    i += s.Receive(buf, i, buf.Length - i, SocketFlags.None);
                }
                if (i != ret.Length+2)
                    return false;
                for (i = 0; i < ret.Length; i++)
                {
                    if (buf[i] != ret[i])
                        return false;
                }
                //长度
                int len = (buf[7] << 8) + buf[8];
                //获取照片
                //56 00 32 0C 00 0A 00 00 XX XX 00 00 YY YY 00 FF
                //返回：76 00 32 00 00 FF D8 。。。。。。FF D9 76 00 32 00 00
                //00 00 XX XX --- 起始地址（先高位字节，后低位字节。必须是8 的倍数）
                //00 00 YY YY --- 本次读的数据长度（先高位字节，后低位字节）
                //注意：完整的JPEG图片文件一定是以FF D8 开始，FF D9 结束。
                buf = new byte[len];
                command = new byte[] { 0x56, 0x00, 0x32, 0x0C, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x00, 0xFF };
                ret = new byte[] { 0x76, 0x00, 0x32, 0x00, 0x00 };
                //分批接收数据，每次少量接收，可以保证数据不丢失
                int packLen = 256;
                int index = 0, rlen = len;
                byte[] tmpbuf = new byte[packLen+10];
                int redCount = 10;
                while (index < len && redCount>0)
                {
                    rlen = ((index + packLen > len) ? len - index : packLen);

                    command[8] = (byte)((index >> 8) & 0xFF);
                    command[9] = (byte)(index & 0xFF);
                    command[12] = (byte)((rlen >> 8) & 0xFF);
                    command[13] = (byte)(rlen & 0xFF);
                    i = s.Send(command);
                    if (i != command.Length)
                        return false;
                    redCount = 0;
                    while (redCount < rlen && s.Poll(TimeWait, SelectMode.SelectRead))
                    {
                        i = s.Receive(tmpbuf, redCount, tmpbuf.Length - redCount, SocketFlags.None);
                        if (i <= 0)
                            return false;
                        redCount += i;
                    }
                    if (i > 0)
                    {
                        //System.Threading.Thread.Sleep(50);
                    }

                    if (false)
                    {
                        //不判断数据的准确性了，全部处理
                        if (redCount != buf.Length)
                            return false;
                        for (i = 0; i < ret.Length; i++)
                        {
                            if (buf[i] != ret[i])
                                return false;
                            if (buf[buf.Length - ret.Length + i] != ret[i])
                                return false;
                        }
                    }
                    //拷贝数据到缓冲区
                    if (redCount > 10)
                    {
                        Array.Copy(tmpbuf, 5, buf, index, (redCount - 10 > rlen ? rlen : redCount - 10));
                        index += redCount - 10;
                        if (tmpbuf[redCount - 7] == 0xFF && tmpbuf[redCount - 6] == 0xD9)
                            break;
                    }
                }
                //转换base64
                FileData = Convert.ToBase64String(buf, 0, index);
                //System.IO.FileStream f = System.IO.File.Open("c:\\a.jpg", System.IO.FileMode.OpenOrCreate);
                //f.Write(buf, ret.Length, redCount - ret.Length * 2);
                //f.Close();
                //清空缓存
                //清空图片缓存指令：56 00 36 01 03 返回：76 00 36 00 00
                command = new byte[] { 0x56, 0x00, 0x36, 0x01, 0x03 };
                ret = new byte[] { 0x76, 0x00, 0x36, 0x00, 0x00 };
                i = s.Send(command);
                if (i != command.Length)
                    return false;
                i = 0;
                while (i < ret.Length && s.Poll(TimeWait, SelectMode.SelectRead))
                {
                    i += s.Receive(buf, i, buf.Length - i, SocketFlags.None);
                }

                bRet = true;
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Warnning, "抓取图片出错：" + ee.ToString());
            }
            finally
            {
                if (s != null)
                {
                    if (s.Connected)
                        s.Close();
                }
            }
            return bRet;
        }
    }
}
