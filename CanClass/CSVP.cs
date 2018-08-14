using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using System.Runtime.InteropServices;

namespace CanClass
{
    public delegate void d_CanGetData(byte[] data);
    public class CSVP
    {
        static bool     m_bOpen;

        public static event d_CanGetData OnCanGetData;
        static uint m_devtype = (uint)LogInfo.Constant.CanType;// AdvCan.VCI_USBCAN2;
        static uint m_devind = 0;
        static int m_CanCount = 2;

        #region 打开CAN
        /// <summary>
        /// 打开CAN
        /// </summary>
        /// <returns>成功否</returns>
        public unsafe static bool Start(int CanCount)
        {
            if (m_bOpen) return true;
            m_CanCount = CanCount;
            try
            {
                m_bOpen = false;
				if (AdvCan.VCI_OpenDevice(m_devtype, m_devind, 0) != AdvCan.STATUS_OK)
                {
                    return false;
                }
				if (LogInfo.Constant.CanType != AdvCan.VCI_CANETTCP)
				{
					//can卡链接
					for (uint m_canind = 0; m_canind < m_CanCount; m_canind++)
					{
						//0x060003--1000Kbps
						//0x060004--800Kbps
						//0x060007--500Kbps
						//0x1C0008--250Kbps
						//0x1C0011--125Kbps
						//0x160023--100Kbps
						//0x1C002C--50Kbps
						//0x1600B3--20Kbps
						//0x1C00E0--10Kbps
						//0x1C01C1--5Kbps	
						//UInt32 canbaud = 0x1C0011;
						//if (AdvCan.VCI_SetReference(m_devtype, m_devind, m_canind, 0, (byte*)&canbaud) != AdvCan.STATUS_OK)
						//{
						//    AdvCan.VCI_CloseDevice(m_devtype, m_devind);
						//    return false;
						//}
						//滤波设置
						VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
						config.AccCode = 0;
						config.AccMask = 0xFFFFFFFF;
						config.Timing0 = 0x03;//125K
						config.Timing1 = 0x1c;
						config.Filter = 0; //双滤波 单滤波
						config.Mode = 0;//正常 只听
						if (AdvCan.VCI_InitCAN(m_devtype, m_devind, m_canind, ref config) != AdvCan.STATUS_OK)
						{
							AdvCan.VCI_CloseDevice(m_devtype, m_devind);
							return false;
						}

						if (AdvCan.VCI_StartCAN(m_devtype, m_devind, m_canind) != AdvCan.STATUS_OK)
						{
							AdvCan.VCI_CloseDevice(m_devtype, m_devind);
							return false;
						}
					}
				}
				else
				{
					//can net 模块链接
					/*VCI_SetReference(m_devtype,index,0,CMD_DESIP,(PVOID)szdesip);
					VCI_SetReference(m_devtype, index, 0, CMD_DESPORT, (PVOID) & desport);

					if (VCI_StartCAN(m_devtype, index, 0) != STATUS_OK)
					{
						MessageBox("打开设备失败!", "警告", MB_OK | MB_ICONQUESTION);
						return;
					}*/
					m_CanCount = 1;
					byte[] buf = System.Text.Encoding.ASCII.GetBytes(LogInfo.Constant.CanNetIP);
					fixed (byte* pbuf = buf)
					{
						if (AdvCan.VCI_SetReference(m_devtype, m_devind, 0, AdvCan.CMD_DESIP, pbuf) != AdvCan.STATUS_OK)
						{
							AdvCan.VCI_CloseDevice(m_devtype, m_devind);
							return false;
						}
					}
					int iPort = LogInfo.Constant.CanNetPort;
					if (AdvCan.VCI_SetReference(m_devtype, m_devind, 0, AdvCan.CMD_DESPORT, (byte*)(&iPort)) != AdvCan.STATUS_OK)
					{
						AdvCan.VCI_CloseDevice(m_devtype, m_devind);
						return false;
					}
					if (AdvCan.VCI_StartCAN(m_devtype, m_devind, 0) != AdvCan.STATUS_OK)
					{
						AdvCan.VCI_CloseDevice(m_devtype, m_devind);
						return false;
					}
				}
                m_bOpen = true;
                Thread t = new Thread(new ThreadStart(GetCanData));
                t.Start();
                return true;
            }
            catch (Exception ee)
            {
                Log.WriteFileLog("打开CAN:" + ee.ToString());
                return false;
            }
        }
        #endregion

        #region 关闭CAN
        /// <summary>
        /// 关闭CAN
        /// </summary>
        /// <returns>成功否</returns>
        public static bool Stop()
        {
            if (!m_bOpen) return true;

            m_bOpen = false;
            AdvCan.VCI_CloseDevice(m_devtype, m_devind);
            return true; ;
        }
        #endregion

        #region 发送数据
        /// <summary>
        /// 成功返回非0，失败返回0
        /// </summary>
        /// <returns></returns>
        public unsafe static bool SendData(int len, byte[] buf)
        {
            try
            {
				//如果发送的数据的路数在打开的路数之外，返回false
				if (buf[3] >= m_CanCount)
					return false;

                int nMsgCount = (len - 4) / 8;
                if (nMsgCount * 8 + 4 < len) nMsgCount++;

                VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
                sendobj.SendType = (byte)0; //正常发送 单次正常发送 自发自收 单次自发自收
                sendobj.RemoteFlag = (byte)0; //数据帧 远程帧
                sendobj.ExternFlag = (byte)1; //标准帧 扩展帧
                sendobj.DataLen = 8;
                //Initialize msg
                if (buf[0] == (int)LogInfo.SendDataType.模板选择			//CMD_TempletList：命令码cmd_code=0x47定义(模板选择)
					|| buf[0] == (int)LogInfo.SendDataType.取件单位选择)	//CMD_UnitList：命令码cmd_code=0xE4定义(取件单位选择)
                {
                    for (int j = 0; j < nMsgCount; j++)
                    {
                        if (j + 1 < nMsgCount)
                        {
                            sendobj.ID = (uint)(buf[1] | (buf[2] << 8) | (buf[3] << 16) | ((0xC0 + j) << 20));
                            System.Runtime.InteropServices.Marshal.Copy(buf, 12 + j * 8, (IntPtr)sendobj.Data, 8);
                        }
                        else
                        {
                            sendobj.ID = (uint)(buf[1] | (buf[2] << 8) | (buf[3] << 16) | (buf[0] << 20));
                            System.Runtime.InteropServices.Marshal.Copy(buf, 4, (IntPtr)sendobj.Data, 8);
                        }
						//程序更改成循环发送，一直发送直到成功。
						while (AdvCan.VCI_Transmit(m_devtype, m_devind, buf[3], ref sendobj, 1) == 0 && m_bOpen)
                        {
							System.Threading.Thread.Sleep(10);
                        }
                    }
                }
				else if (buf[0] == (int)LogInfo.SendDataType.提示文本 || buf[0] == (int)LogInfo.SendDataType.提示文本串口屏幕)//CMD_OUTTEXT：命令码cmd_code=0x4c定义(在主显示屏上的各个区域显示提示文本)
                {
                    for (int j = 0; j < nMsgCount; j++)
                    {
                        if (j + 1 < nMsgCount)
                        {
                            sendobj.ID = (uint)(buf[1] | (buf[2] << 8) | (buf[3] << 16) | ((0xA0 + j) << 20));
                            System.Runtime.InteropServices.Marshal.Copy(buf, 12 + j * 8, (IntPtr)sendobj.Data, 8);
                        }
                        else
                        {
                            sendobj.ID = (uint)(buf[1] | (buf[2] << 8) | (buf[3] << 16) | (buf[0] << 20));
                            System.Runtime.InteropServices.Marshal.Copy(buf, 4, (IntPtr)(sendobj.Data), 8);
                        }
						//程序更改成循环发送，一直发送直到成功。
						while (AdvCan.VCI_Transmit(m_devtype, m_devind, buf[3], ref sendobj, 1) == 0 && m_bOpen)
						{
							System.Threading.Thread.Sleep(10);
						}
                    }
                }
				else if (buf[0] == (int)LogInfo.SendDataType.语音提示)//CMD_Sound：命令码cmd_code=0x50定义(语音提示)
                {
                    for (int j = 0; j < nMsgCount; j++)
                    {
                        if (j + 1 < nMsgCount)
                        {
                            sendobj.ID = (uint)(buf[1] | (buf[2] << 8) | (buf[3] << 16) | ((0xC0 + j) << 20));
                            System.Runtime.InteropServices.Marshal.Copy(buf, 12 + j * 8, (IntPtr)sendobj.Data, 8);
                        }
                        else
                        {
                            sendobj.ID = (uint)(buf[1] | (buf[2] << 8) | (buf[3] << 16) | (buf[0] << 20));
                            System.Runtime.InteropServices.Marshal.Copy(buf, 4, (IntPtr)sendobj.Data, 8);
                        }
						//程序更改成循环发送，一直发送直到成功。
						while (AdvCan.VCI_Transmit(m_devtype, m_devind, buf[3], ref sendobj, 1) == 0 && m_bOpen)
						{
							System.Threading.Thread.Sleep(10);
						}
                    }
                }
                else
                {
                    for (int j = 0; j < nMsgCount; j++)
                    {
                        sendobj.ID = (uint)(buf[1] | (buf[2] << 8) | ((buf[3] & 0xF) << 16) | (buf[0] << 20));
                        System.Runtime.InteropServices.Marshal.Copy(buf, 4+j*8, (IntPtr)sendobj.Data, 8);
						//程序更改成循环发送，一直发送直到成功。
						while (AdvCan.VCI_Transmit(m_devtype, m_devind, buf[3], ref sendobj, 1) == 0 && m_bOpen)
						{
							System.Threading.Thread.Sleep(10);
						}
						System.Threading.Thread.Sleep(5);
                    }
                }

                return true;
            }
            catch (Exception ee)
            {
                Log.WriteFileLog("发送数据：" + ee.ToString());
                return false;
            }
        }
        #endregion

        #region 接收数据
        /// <summary>
        /// 成功返回非0，失败返回0
        /// </summary>
        /// <returns></returns>
        private unsafe static void GetCanData()
        {
            Dictionary<string, byte[]> barCodeData = new Dictionary<string, byte[]>();
            var CameraData = new Dictionary<uint, System.IO.MemoryStream>();
            Dictionary<uint, System.IO.MemoryStream> YNZhiJingMaiData = new Dictionary<uint, System.IO.MemoryStream>();

            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * 100);
            UInt32 res = 0;
            byte[] buf1 = new byte[8];

            while (m_bOpen)
            {
                for (uint m_canind = 0; m_canind < m_CanCount; m_canind++)
                {
                    try
                    {
                        res = AdvCan.VCI_GetReceiveNum(m_devtype, m_devind, m_canind);
                        if (res == 0)
                            continue;
                        res = AdvCan.VCI_Receive(m_devtype, m_devind, m_canind, pt, 100, 100);
                        for (uint i = 0; i < res; i++)
                        {
                            VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                            //Package receiving ok
                            uint cmd = (obj.ID >> 20) & 0xFF;
                            if ((cmd == 0x01 || cmd == 0x02 || cmd == 0x05 || cmd == 0x06) && (obj.ID & 0xFF) >= 0x80)
                            {
                                //条码的数据,证卡的数据
                                uint BiaoZhi = (obj.ID >> 8) & 0xFFF;
                                byte[] buf;
                                if (barCodeData.ContainsKey(BiaoZhi.ToString()))
                                {
                                    buf = barCodeData[BiaoZhi.ToString()];
                                }
                                else
                                {
                                    buf = new byte[1024];
                                    barCodeData.Add(BiaoZhi.ToString(), buf);
                                }
                                uint j = obj.ID & 0xFF;
                                System.Runtime.InteropServices.Marshal.Copy((IntPtr)obj.Data, buf1, 0, 8);
                                Array.Copy(buf1, 0, buf, (j - 0x80 + 1) * 8 + 4, 8);
                            }
                            else if (cmd == 0x01 || cmd == 0x02 || cmd == 0x05 || cmd == 0x06)
                            {
                                //条码,证卡的头部
                                uint BiaoZhi = (obj.ID >> 8) & 0xFFF;
                                byte[] buf;
                                if (barCodeData.ContainsKey(BiaoZhi.ToString()))
                                {
                                    buf = barCodeData[BiaoZhi.ToString()];
                                    barCodeData.Remove(BiaoZhi.ToString());
                                }
                                else
                                {
                                    buf = new byte[1024];
                                }
                                buf[0] = (byte)((obj.ID >> 20) & 0xFF);  //code
                                buf[1] = (byte)((obj.ID >> 0) & 0xFF);  //m0
                                buf[2] = (byte)((obj.ID >> 8) & 0xFF);  //m1
                                buf[3] = (byte)((obj.ID >> 16) & 0xF);  //m2
                                System.Runtime.InteropServices.Marshal.Copy((IntPtr)obj.Data, buf1, 0, 8);
                                Array.Copy(buf1, 0, buf, 4, 8);
                                OnCanGetData(buf);
                            }
							else if (cmd == 0x0A)
							{
								//燕南指静脉
								uint BN = obj.ID & 0xFFFFF;
								System.IO.MemoryStream ms;
								if (YNZhiJingMaiData.ContainsKey(BN))
								{
									ms = YNZhiJingMaiData[BN];
								}
								else
								{
									ms = new System.IO.MemoryStream();
									YNZhiJingMaiData.Add(BN, ms);
								}
								System.Runtime.InteropServices.Marshal.Copy((IntPtr)obj.Data, buf1, 0, 8);
								ms.Write(buf1, 0, 8);
							}
							else if (cmd == 0x0B)
							{
								//燕南指静脉
								uint BN = obj.ID & 0xFFFFF;
								System.IO.MemoryStream ms;
								if (YNZhiJingMaiData.ContainsKey(BN))
								{
									ms = YNZhiJingMaiData[BN];
									YNZhiJingMaiData.Remove(BN);
								}
								else
								{
									ms = new System.IO.MemoryStream();
								}
								ms.Seek(0, System.IO.SeekOrigin.Begin);
								int getLen = (int)ms.Length;
								byte[] buf = new byte[getLen + 12];
								ms.Read(buf, 12, getLen);
								ms.Close();

								int datalen = (int)(obj.Data[1] | (obj.Data[2] << 8));
								uint checkSum = (uint)(obj.Data[3] | (obj.Data[4] << 8) | (obj.Data[5] << 16) | (obj.Data[6] << 24));
								if ((datalen > getLen) || (getLen-datalen)>8)
								{
									LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "燕南指静脉，接收数据不完整。收到：" + getLen.ToString() + "，应该：" + datalen.ToString());
								}
								else
								{
									uint sum = 0;
									for (int ii = 0; ii < datalen; ii++)
										sum += buf[12 + ii];
									if (sum == checkSum)
									{
										buf[0] = (byte)((obj.ID >> 20) & 0xFF);  //code
										buf[1] = (byte)((obj.ID >> 0) & 0xFF);  //m0
										buf[2] = (byte)((obj.ID >> 8) & 0xFF);  //m1
										buf[3] = (byte)((obj.ID >> 16) & 0xF);  //m2
										for (int ii = 0; ii < 8; ii++)
											buf[4 + ii] = obj.Data[ii];

										OnCanGetData(buf);
									}
									else
										LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "燕南指静脉，接收数据，校验失败。");

								}
							}
                            else if (cmd == 0xE7)
                            {
                                //串口摄像头图片数据
                                uint BN = obj.ID & 0xFFFFF;
                                System.IO.MemoryStream ms;
                                if (CameraData.ContainsKey(BN))
                                {
                                    ms = CameraData[BN];
                                }
                                else
                                {
                                    ms = new System.IO.MemoryStream();
                                    CameraData.Add(BN, ms);
                                }
                                Marshal.Copy((IntPtr)obj.Data, buf1, 0, 8);
                                ms.Write(buf1, 0, 8);
                            }
                            else if (cmd == 0xE8)
                            {
                                //串口摄像头图片传输完成，长度
                                uint bn = obj.ID & 0xFFFFF;
                                System.IO.MemoryStream ms;
                                if (CameraData.ContainsKey(bn))
                                {
                                    ms = CameraData[bn];
                                    CameraData.Remove(bn);
                                }
                                else
                                {
                                    ms = new System.IO.MemoryStream();
                                }
                                ms.Seek(0, System.IO.SeekOrigin.Begin);
                                byte[] buf = new byte[ms.Length+12];
                                ms.Read(buf, 12, buf.Length-12);
                                ms.Close();

                                buf[0] = (byte)((obj.ID >> 20) & 0xFF);  //code
                                buf[1] = (byte)((obj.ID >> 0) & 0xFF);  //m0
                                buf[2] = (byte)((obj.ID >> 8) & 0xFF);  //m1
                                buf[3] = (byte)((obj.ID >> 16) & 0xF);  //m2
								buf[4] = obj.Data[0];
								buf[5] = obj.Data[1];
								buf[6] = obj.Data[2];
								buf[7] = obj.Data[3];
                                OnCanGetData(buf);
                            }
                            else
                            {
                                byte[] buf = new byte[12];
                                buf[0] = (byte)((obj.ID >> 20) & 0xFF);  //code
                                buf[1] = (byte)((obj.ID >> 0) & 0xFF);  //m0
                                buf[2] = (byte)((obj.ID >> 8) & 0xFF);  //m1
                                buf[3] = (byte)((obj.ID >> 16) & 0xF);  //m2
                                Marshal.Copy((IntPtr)obj.Data, buf1, 0, 8);
                                Array.Copy(buf1, 0, buf, 4, 8);
                                OnCanGetData(buf);
                            }
                        }//for
                    }
                    catch (Exception ee)
                    {
                        Log.WriteFileLog("接收数据：" + ee);
                    }
                }//for
                Thread.Sleep(10);
            }//while
        }
        #endregion
    }
}
