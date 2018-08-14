using System;
using LogInfo;

namespace BoxDataParse
{

    public delegate void d_ReceiveBoxData(string BN_NO, ReceiveDataType iType, string data, bool bFront);

	/// <summary>
	/// DataParse 的摘要说明。
	/// </summary>
	public class DataParse
	{
        const int bufLen = 512;
		public static event d_ReceiveBoxData OnReceiveBoxData;

        static DataParse()
		{
            ListenBox.Listen.OnReceiveBoxData += boxlisten_OnReceiveBoxData;
			OnReceiveBoxData = null;
		}

        public static bool Start()
		{
			if(ListenBox.Listen.Start())
				return true;
			else
				return false;
		}

		public static void Stop()
		{
			ListenBox.Listen.Stop();
			return;
		}

		#region 接收数据
        private static unsafe void boxlisten_OnReceiveBoxData(byte[] data)
		{
            bool bFront = true;
            string BN_NO; int CmdType;
            fixed (byte* pbuf = data)
            {
                CMDHeader* header = (CMDHeader*)pbuf;
                CmdType = header->rpt_code;
                BN_NO = "BN" + header->bn_m2.ToString("0");
                BN_NO += header->bn_m1.ToString("00");
                BN_NO += header->bn_m0.ToString("00");
            }

            int headerlen = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CMDHeader));
            if (data[headerlen + 7] == 1)
                bFront = false;
            bool bArm4Dai = false;
            switch (CmdType)
			{
                case 0x01:					//扫描到条码值
                    bArm4Dai = true;
                    goto Label_TiaoMa;
                case 0x05:					//扫描到条码值
                    Label_TiaoMa:
                    #region 扫描到条码值
                    if (OnReceiveBoxData != null)
                    {
                        DecodeBar decode = new DecodeBar();
                        int barlen = (int)(data[headerlen] | (data[headerlen + 1]<< 8));
                        /*
                       关于解码数据协议简要说明如下：
                          头两个字节固定为0x52 0xe0；
                          第3、第4字节表示数据的长度，第3字节为高字节，第4字节为低字节；这里的数据长度是指从第5个字节到最后一个字节的字节数；
                          第5个字节表示码制类型，
                          1～15表示SCDCC码（由于历史版本的原因，SCDCC码占了多个值）
                          128表示一维码
                          130表示PDF417码
                          其他值预留扩展使用
                       */
                        byte[] SCDCCBuffer = new byte[barlen];
                        Array.Copy(data, headerlen + 8, SCDCCBuffer, 0, SCDCCBuffer.Length);
                        string BarCode = "";
                        //正常情况下，头2个字节都是0x52和0xE0
                        if (SCDCCBuffer[0] == 0x52 && SCDCCBuffer[1] == 0xE0)
                        {
                            //short barCodeLen = (short)(SCDCCBuffer[2] << 8 | SCDCCBuffer[3]);
                            if (SCDCCBuffer[4] >= 0x01 && SCDCCBuffer[4] <= 0x0F)//SCSCC
                            {
                                BarCode = decode.Debar(SCDCCBuffer);
                            }
                            else
                            {
                                BarCode = System.Text.Encoding.GetEncoding(936).GetString(SCDCCBuffer, 5, barlen-5);
                                if (SCDCCBuffer[4] == 0x80)  //一维
                                { 
                                    if(BarCode.StartsWith("*") && BarCode.EndsWith("*"))
                                    {
                                        BarCode = BarCode.Substring(1, BarCode.Length - 2);
                                    }
                                }
                                else if (SCDCCBuffer[4] == 0x82)
                                { }
                                else
                                { }
                            }
                        }
                        else
                        {
                            BarCode = System.Text.Encoding.GetEncoding(936).GetString(SCDCCBuffer, 0, barlen);
                        }
                        BarCode = BarCode.Replace("\0", "");
                        if (bArm4Dai)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.条码信息_4Dai, BarCode, bFront);
                        else
                            OnReceiveBoxData(BN_NO, ReceiveDataType.条码信息, BarCode, bFront);
                    }
					break;
                    #endregion

                case 0x02:					//扫描到条码值
                    bArm4Dai = true;
                    goto Label_ZhengKa;
                case 0x06:					//上报用户刷卡得到的用户卡号
                    Label_ZhengKa:
                    #region 上报用户刷卡得到的用户卡号.可以为管理员卡或者用户卡。
                    if (OnReceiveBoxData != null)
                    {
                        int barlen = (int)(data[headerlen] | (data[headerlen + 1] << 8));
                        int AuthType = data[headerlen + 2]; //认证类型：1=RFID，2=接触式SAM卡，3=A/P帐号密码，
                        //4=USB钥匙，5=普通钥匙，6=指纹数据，7=Cpu卡，8=指静脉
                        string BarCode = "";
                        if (AuthType == 1 || AuthType == 2 || AuthType == 7)
                        {
                            BarCode = System.Text.Encoding.GetEncoding(936).GetString(data, headerlen+8, barlen);
                            if (bArm4Dai)
                                OnReceiveBoxData(BN_NO, ReceiveDataType.证卡信息_4Dai, BarCode, bFront);
                            else
                                OnReceiveBoxData(BN_NO, ReceiveDataType.证卡信息, BarCode, bFront);
                        }
                        else if (AuthType == 8)
                        {
                            //总共是94包数据，总数据长度为752字节，包括四字节效验值。
                            //前47包(0x00600380到0x006003ae)有两字节效验，位于0x006003ae 最后两字节数据。
                            //后47包(0x006003af到0x006003dd)有两字节效验，位于0x006003dd 最后两字节数据。
                            //效验低字节在前，高字节在后。
                            byte[] buf = new byte[748];
                            Array.Copy(data, headerlen + 8, buf, 0, 374);
                            Array.Copy(data, headerlen + 8 + 376, buf, 374, 374);
                            BarCode = Convert.ToBase64String(buf);
                            CmdBuzzer(BN_NO, 1);
                            OnReceiveBoxData(BN_NO, ReceiveDataType.证卡信息_指静脉, BarCode, bFront);
                        }
                    }
                    break;
                    #endregion

                case 0x07:					//投信或者投箱撤销
                    if (OnReceiveBoxData != null)
                    {
                        int ShowCount = (int)(data[headerlen + 1] | (data[headerlen + 2] << 8));
                        //1：投箱信号。投件投入箱格中。  DROP_ONE
                        //3：投箱撤销信号。用户抽出或者等待超时时候。 DROP_CANCLE
                        //2: DROP_NONE
                        if (data[headerlen] == 1)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.投件投入, ShowCount.ToString(), bFront);
                        else
                            OnReceiveBoxData(BN_NO, ReceiveDataType.投件抽出, ShowCount.ToString(), bFront);
                    }
                    break;

				case 0x0B:	//燕南指静脉
					if (OnReceiveBoxData != null)
					{
						int iType = data[headerlen];
						if (iType == 8)
						{
							int datalen = (int)(data[headerlen + 1] | (data[headerlen + 2] << 8));
							int checkSum = (int)(data[headerlen + 3] | (data[headerlen + 4] << 8) | (data[headerlen + 5] << 16) | (data[headerlen + 6] << 24));
							byte[] buf = new byte[datalen];
							Array.Copy(data, headerlen + 8, buf, 0, datalen);

							//数据是Base64格式的数据，我们把后面的0去掉
							int iLen = 0;
							for (; iLen < buf.Length; iLen++)
								if (buf[iLen] == 0)
									break;
							string szData = System.Text.Encoding.ASCII.GetString(buf, 0, iLen);

							OnReceiveBoxData(BN_NO, ReceiveDataType.证卡信息_燕南指静脉, szData, true);
						}
					}
					break;

				case 0x62:
					#region 指静脉数据
					if (OnReceiveBoxData != null)
					{
						int MsgType = data[headerlen];
						int MsgValue = data[headerlen + 1];
						if (MsgType == 1)
						{
							if (MsgValue == 1)
								OnReceiveBoxData(BN_NO, ReceiveDataType.指静脉数据_传输, "当前设备已经准备好接受指静脉模板数据", true);
							else if (MsgValue == 2)
								OnReceiveBoxData(BN_NO, ReceiveDataType.指静脉数据_传输, "本次数据传输验证成功", true);
							else
								OnReceiveBoxData(BN_NO, ReceiveDataType.指静脉数据_传输, "error", true);
						}
						else if (MsgType == 2)
						{
							if (MsgValue == 1)
								OnReceiveBoxData(BN_NO, ReceiveDataType.指静脉数据_写入, "写特征数据成功", true);
							else
								OnReceiveBoxData(BN_NO, ReceiveDataType.指静脉数据_写入, "写特征数据失败", true);
						}
						else if (MsgType == 3)
						{
							int UserId = data[headerlen + 2] + data[headerlen + 3] * 256;
							if (MsgValue == 1)
								OnReceiveBoxData(BN_NO, ReceiveDataType.指静脉数据_验证, UserId.ToString(), true);
							else
								OnReceiveBoxData(BN_NO, ReceiveDataType.指静脉数据_验证, "-1", true);
						}
					}
					#endregion
					break;

				case 0x80:					//RPT_UnitList: 状态rpt_code=0x80定义(取件单位选择回复)
					#region 取件单位选择回复
					{
						string indexStr = "";
						for (int i = 0; i < 8; i++)
						{
							if ((data[headerlen + i] & 0x0F) > 0)
							{
								indexStr += (i * 2).ToString() + ",";
							}
							if ((data[headerlen + i] & 0xF0) > 0)
							{
								indexStr += (i * 2 + 1).ToString() + ",";
							}
						}
						if (indexStr.Length > 0)
						{
							indexStr = indexStr.TrimEnd(',');
						}
						if (OnReceiveBoxData != null)
							OnReceiveBoxData(BN_NO, ReceiveDataType.取件单位选择, indexStr, bFront);
					}
					#endregion
					break;

                case 0x81:					//门禁
                    #region 门禁
                    {
                        string cmdstr = "";
                        if (data[headerlen+1] == 1)//门禁关闭超时
                            cmdstr = "CloseTimeOut";
                        else if (data[headerlen + 1] == 2)//门禁打开超时
                            cmdstr = "OpenTimeOut";
                        else if (data[headerlen + 1] == 0)
                        {
                            if (data[headerlen] == 0) //0：门禁关闭   CURSTAT_DOOR_CLOSE
                            {
                                cmdstr = "Closed";
                            }
                            else if (data[headerlen] == 1) //1：门禁打开   CURSTAT_DOOR_OPEN
                            {
                                cmdstr = "Opened";
                            }
                        }
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.门禁, cmdstr, bFront);
                    }
                    #endregion
                    break;

                case 0x82:					//门锁
                    #region 门锁
                    {
                        string cmdstr = "";
                        if (data[headerlen + 1] == 2)//门锁打开超时
                            cmdstr = "OpenTimeOut";
                        else if (data[headerlen + 1] == 0)
                        {
                            if (data[headerlen] == 0) //0：门锁关闭
                            {
                                cmdstr = "Closed";
                            }
                            else if (data[headerlen] == 1) //1：门锁打开
                            {
                                cmdstr = "Opened";
                            }
                        }
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.门锁, cmdstr, bFront);
                    }
                    #endregion
                    break;

                case 0x83:					//光电
                    {
                        #region 光电
                        int tmp = (int)data[headerlen];
                        int error = (int)data[headerlen+1];
                        string cmdstr = "";

                        if ((error & 0x01) == 0)//0位：0: 前光电正常，1：前光电故障
                        {
                            if ((tmp & 0x01) == 0) //0位：0:前光电未遮挡，1：前光电遮挡
                            {
                                cmdstr = "前光电未遮挡";
                            }
                            else
                            {
                                cmdstr = "前光电遮挡";
                            }
                        }
                        else
                            cmdstr = "前光电故障";
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.光电, cmdstr, bFront);

                        if ((error & 0x02) == 0)//1位：0：后光电正常，1：后光电故障
                        {
                            if ((tmp & 0x02) == 0) //1位：0:后光电未遮挡，1：后光电遮挡 CURSTAT_PH_BACK
                            {
                                cmdstr = "后光电未遮挡";
                            }
                            else
                            {
                                cmdstr = "后光电遮挡";
                            }
                        }
                        else
                            cmdstr = "后光电故障";
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.光电, cmdstr, bFront);

                        if ((error & 0x04) == 0)//2位：0：箱满光电正常，1：箱满光电故障
                        {
                            if ((tmp & 0x04) == 0) //2位：0: 箱满光未遮挡，1：箱满光电遮挡
                            {
                                cmdstr = "箱满光未遮挡";
                            }
                            else
                            {
                                cmdstr = "箱满光电遮挡";
                            }
                        }
                        else
                            cmdstr = "箱满光电故障";
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.光电, cmdstr, bFront);

                        if ((error & 0x08) == 0)//3位：0：箱空光电正常，1：箱空光电故障
                        {
                            if ((tmp & 0x08) == 0) //3位：0: 箱空光未遮挡，1：箱空光电遮挡
                            {
                                cmdstr = "箱空光电未遮挡";
                            }
                            else
                            {
                                cmdstr = "箱空光电遮挡";
                            }
                        }
                        else
                            cmdstr = "箱空光电故障";
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.光电, cmdstr, bFront);

                        if ((error & 0x10) == 0)//4位：0：触发光电正常，1：触发光电故障
                        {
                            if ((tmp & 0x10) == 0) //4位：0: 触发光电未遮挡，1：触发光电遮挡
                            {
                                cmdstr = "触发光电未遮挡";
                            }
                            else
                            {
                                cmdstr = "触发光电遮挡";
                            }
                        }
                        else
                            cmdstr = "触发光电故障";
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.光电, cmdstr, bFront);

                        #endregion
                    }
                    break;

                case 0x84:					//用户按键
					if(OnReceiveBoxData!=null)
                        OnReceiveBoxData(BN_NO, ReceiveDataType.按键, data[headerlen].ToString(), bFront);
					break;

                case 0x85:					//0x85定义(硬件配置
                    if (OnReceiveBoxData != null)
                    {
                        #region
                        //扫描头串口设置。0：扫描头波特率19200，1：扫描头波特率115200
                        //2：串口摄像头波特率19200，3：串口摄像头波特率38400，
                        //4：串口摄像头波特率57600，5：串口摄像头波特率115200，
                        string str = "";
                        if (data[headerlen] == 0)//扫描头串口波特率。0：19200，1：115200
                            str += "扫描头波特率=19200";
                        else if (data[headerlen] == 1)
                            str += "扫描头波特率=115200";
                        else if (data[headerlen] == 2)
                            str += "串口摄像头波特率=19200";
                        else if (data[headerlen] == 3)
                            str += "串口摄像头波特率=38400";
                        else if (data[headerlen] == 4)
                            str += "串口摄像头波特率=57600";
                        else if (data[headerlen] == 5)
                            str += "串口摄像头波特率=115200";

                        str += "，触发光电类型=";
                        if (data[headerlen+1] == 0)//触发光电类型。0：反射，1：对射
                            str += "反射";
                        else
                            str += "对射";

						str += "，串口3用途=";
						if (data[headerlen + 2] == 0)//串口3用途。0：扫描头，1：SYN6288语音模块，2：串口摄像头，//3：指静脉仪
							str += "扫描头";
						else if (data[headerlen + 2] == 1)
							str += "SYN6288语音模块";
						else if (data[headerlen + 2] == 2)
							str += "串口摄像头";
						else if (data[headerlen + 2] == 3)
							str += "指静脉仪";

                        str += "，串口1用途=";
                        if (data[headerlen + 3] == 0)//串口1用途。0：串口屏幕，1：TTS语音模块，2：串口摄像头
                            str += "串口屏幕";
                        else if (data[headerlen + 3] == 1)
                            str += "TTS语音模块";
                        else if (data[headerlen + 3] == 2)
                            str += "串口摄像头";

                        str += "，串口2用途=";
						if (data[headerlen + 4] == 0)//串口2用途。0：RW163T读卡器，1：M104读卡器，2：ZLG522S读卡器, 3：指静脉仪，4. SYN6288语音模块
                            str += "RW163T读卡器";
                        else if (data[headerlen + 4] == 1)
                            str += "M104读卡器";
                        else if (data[headerlen + 4] == 2)
                            str += "ZLG522S读卡器";
                        else if (data[headerlen + 4] == 3)
                            str += "指静脉仪";
						else if (data[headerlen + 4] == 4)
							str += "SYN6288语音模块";

                        str += "，门禁电机工作方式=";
                        if (data[headerlen + 5] == 0)//门禁电机工作方式。0：双向旋转，1：单向顺时针选择，2：单向逆时针旋转
                            str += "双向旋转";
                        else if (data[headerlen + 5] == 1)
                            str += "单向顺时针选择";
                        else
                            str += "单向逆时针旋转";

                        str += "，是否有升降机制=";
                        if (data[headerlen + 6] == 0)//是否有升降机制，0：无，1：升降机，2：第二电控锁，3：双面显示, 4：第二电控锁和双面显示，5：升降机和双面显示
                            str += "没有升降机制";
                        else if (data[headerlen + 6] == 1)
                            str += "有升降机制";
                        else if (data[headerlen + 6] == 2)
                            str += "有第二电控锁";
                        else if (data[headerlen + 6] == 3)
                            str += "有双面显示";
                        else if (data[headerlen + 6] == 4)
                            str += "第二电控锁和双面显示";
                        else if (data[headerlen + 6] == 5)
                            str += "升降机和双面显示";
                        else
                            str += "未知";

                        OnReceiveBoxData(BN_NO, ReceiveDataType.硬件状态, str, bFront);
                        #endregion
                    }
					break;

                case 0x86:					//主箱存在状态
					if(OnReceiveBoxData!=null)
					{
                        int times = data[headerlen] | (data[headerlen+1]<<8) | (data[headerlen+2]<<16) | (data[headerlen+3]<<24);
                        string version = data[headerlen + 7].ToString("X2") + data[headerlen + 6].ToString("X2")
                            + data[headerlen + 5].ToString("X2") + data[headerlen + 4].ToString("X2");
                        if (times==0)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.主箱连接, "Start", true);
                        else
                            OnReceiveBoxData(BN_NO, ReceiveDataType.主箱连接, "Ping", true);
                        OnReceiveBoxData(BN_NO, ReceiveDataType.主箱连接_Version, version.ToString(), true);
					}
					break;

                case 0xE8:					//上报拍摄的照片
                    #region 上报拍摄的照片
                    if (OnReceiveBoxData != null)
                    {
                        int barlen = (int)(data[headerlen] | (data[headerlen + 1] << 8) | (data[headerlen + 2] << 16) | (data[headerlen + 3] << 24));
                        byte[] buf = new byte[barlen];
                        Array.Copy(data, headerlen + 8, buf, 0, barlen);
						try
						{
							System.IO.File.WriteAllBytes("aa.jpg", buf);
						}
						catch { }
                        string pic = Convert.ToBase64String(buf);
                        OnReceiveBoxData(BN_NO, ReceiveDataType.上报拍摄的照片, pic, true);
                    }
                    break;
                    #endregion

			}
		}
		#endregion

		#region 发送数据
        #region 数据包头
        private static unsafe int SetHeader(string BN_NO, SendDataType iType, byte[] buf)
        {
            int headerlen = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CMDHeader));
            fixed (byte* psbuf = buf)
            {
                CMDHeader* header = (CMDHeader*)psbuf;
                header->rpt_code = (byte)iType;		//命令
                header->bn_m2 = (byte)Convert.ToInt16(BN_NO.Substring(2, 1));
                header->bn_m1 = (byte)Convert.ToInt16(BN_NO.Substring(3, 2));
                header->bn_m0 = (byte)Convert.ToInt16(BN_NO.Substring(5, 2));
            }
            return headerlen;
        }
        #endregion

        #region 准备投信信号
        /// <summary>
        /// 准备投信信号
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <returns></returns>
        public static unsafe bool CmdPreGetLetter(string BoxBN, int type, int style, int showcount)
        {
            return CmdPreGetLetter(BoxBN, type, style, showcount, true);
        }
        /// <summary>
        /// 准备投信信号
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <returns></returns>
        public static unsafe bool CmdPreGetLetter(string BoxBN, int type, int style, int showcount, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.准备投信;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType);
            msg.Append(", type:"); msg.Append(type.ToString());
            msg.Append(", style:"); msg.Append(style.ToString());
            msg.Append(", showcount:"); msg.Append(showcount.ToString());
            Log.WriteInfo(LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                tag_cmd_presend* c = (tag_cmd_presend*)(psbuf + headerlen);//数据开始的地方
                c->type = (byte)type;
                c->style = (byte)style;
                c->shownum = (UInt16)showcount;
            }//fixed
            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());
            return bSend;
        }
        #endregion

        #region 扫描头控制
        /// <summary>
        /// 扫描头控制
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="bOpen">控制状态，true：开，false：关</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxScan(string BoxBN, bool bOpen)
        {
            return CmdBoxScan(BoxBN, bOpen, true);
        }
        /// <summary>
        /// 扫描头控制
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="bOpen">控制状态，true：开，false：关</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxScan(string BoxBN, bool bOpen, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.扫描头控制;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", bOpen:"); msg.Append(bOpen.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
                if (bOpen)
                    tmpbuf[0] = (byte)1;			//0：关扫描头，1：开扫描头
                else
                    tmpbuf[0] = (byte)0;
            }//fixed
            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 摄像头控制
        /// <summary>
        /// 摄像头控制
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <returns></returns>
        public static unsafe bool CmdCamera(string BoxBN)
        {
            return CmdCamera(BoxBN, true);
        }
        /// <summary>
        /// 摄像头控制
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <returns></returns>
        public static unsafe bool CmdCamera(string BoxBN, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.摄像头控制;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 控制灯请求

        #region 控制待投灯
        /// <summary>
        /// 控制待投灯
        /// </summary>
        /// <param name="boxBn">BN地址</param>
        /// <param name="cType">控制状态，亮，灭，闪动</param>
        /// <returns></returns>
        public static bool CmdBoxLampDaiTou(string boxBn, enum_LampStatus cType)
        {
            return CmdBoxLampByBN(boxBn, 8, cType);
        }
        #endregion

        #region 控制错误灯
        /// <summary>
        /// 控制错误灯
        /// </summary>
        /// <param name="boxBn">BN地址</param>
        /// <param name="cType">控制状态，亮，灭，闪动</param>
        /// <returns></returns>
        public static bool CmdBoxLampCuoWu(string boxBn, enum_LampStatus cType)
        {
            return CmdBoxLampByBN(boxBn, 4, cType);
        }
        #endregion

        #region 控制急件灯
        /// <summary>
        /// 控制急件灯
        /// </summary>
        /// <param name="boxBn">BN地址</param>
        /// <param name="cType">控制状态，亮，灭，闪动</param>
        /// <returns></returns>
        public static bool CmdBoxLampJiJian(string boxBn, enum_LampStatus cType)
        {
            return CmdBoxLampByBN(boxBn, 2, cType);
        }
        #endregion

        #region 控制已取灯
        /// <summary>
        /// 控制已取灯
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="cType">控制状态，亮，灭，闪动</param>
        /// <returns></returns>
        public static bool CmdBoxLampYiQu(string BoxBN, enum_LampStatus cType)
        {
            return CmdBoxLampByBN(BoxBN, 1, cType);
        }
        #endregion

        #region 控制灯请求
        /// <summary>
        /// 控制灯请求
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="ilamp">灯序号，1：已取，2：急件，4：错误</param>
        /// <param name="cType">控制状态，亮，灭，闪动</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxLampByBN(string BoxBN, int ilamp, enum_LampStatus cType)
        {
            return CmdBoxLampByBN(BoxBN, ilamp, cType, true);
        }
        /// <summary>
        /// 控制灯请求
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="ilamp">灯序号，1：已取，2：急件，4：错误</param>
        /// <param name="cType">控制状态，亮，灭，闪动</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxLampByBN(string BoxBN, int ilamp, enum_LampStatus cType, bool bFront)
        {
            byte[] sendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.控制指示灯;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", ilamp:"); msg.Append(ilamp.ToString());
            msg.Append(", cType:"); msg.Append(cType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, sendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = sendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方

                tmpbuf[0] = (byte)ilamp;		//which_led;  //指示灯的序号，如果配置相同，可以多灯一起设置
                tmpbuf[1] = (byte)cType;		//style; 		 0常灭 1常亮 2闪烁
                tmpbuf[2] = 0x3;			//闪烁灭时间（单位十分之一秒，ds）（只对Style=2H时有效）
                tmpbuf[3] = 0x5;            //闪烁亮时间（单位十分之一秒，ds）（只对Style=2H时有效）
            }//fixed
            if (!bFront)
                sendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), sendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion
        #endregion

        #region 数码管操作
        /// <summary>
        /// 数码管操作
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="whichLed">0保留，永远对应箱子号，LNT_LNT0
        /// 1～9 为基本的LNT888箱存显示定义如下：
        /// 1：屏幕左下角的主数字显示，显示箱存 LNT_LNT1， 对应已投件数量
        /// 2：屏幕右下角第一个显示 ,箱存LNT_LNT2  对应登记数量第1组
        /// 3：屏幕右下角第二个显示  箱存LNT_LNT3  对应登记数量第2组
        /// 4：屏幕右下角第三个显示  箱存LNT_LNT4 对应登记数量第3组
        /// 5～9以此类推
        /// 而20~99双位数，表示已登记下面的明细
        /// 例如，30 为登记第2组的第一个明细条目数量，31为第二个条码数量
        /// 20～29 表示 第1组的各个明细条码数量
        /// 以此类推
        /// </param>
        /// <param name="ledNumber">信件数目</param>
        /// <param name="bFlash">控制状态，false：亮，true：闪</param>
        /// <param name="c">显示颜色</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxLED(string BoxBN, int whichLed, int ledNumber, enum_LedColor c)
        {
            return CmdBoxLED(BoxBN, whichLed, ledNumber, c, true);
        }
        public static unsafe bool CmdBoxLED(string BoxBN, int whichLed, int ledNumber, enum_LedColor c, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.控制数码管;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", whichLed:"); msg.Append(whichLed.ToString());
            msg.Append(", ledNumber:"); msg.Append(ledNumber.ToString());
            msg.Append(", LedColor:"); msg.Append(c.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
                tmpbuf[0] = (byte)whichLed;		//which_lnt; //LNT数码管的序号，如果配置相同，可以多灯一起设置
                tmpbuf[1] = (byte)c;
                tmpbuf[2] = (byte)(ledNumber); 
                tmpbuf[3] = (byte)(ledNumber>>8);
            }//fixed
            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 开关门禁请求
        /// <summary>
        /// 开关门禁请求
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="bOpen">控制状态，true：开，false：关</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxGating(string BoxBN, bool bOpen)
        {
            return CmdBoxGating(BoxBN, bOpen, true);
        }
        public static unsafe bool CmdBoxGating(string BoxBN, bool bOpen, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.门禁控制;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
                if (bOpen)
                    tmpbuf[0] = (byte)1;			//ctl 0：关闭门禁  DOOR_CLOSE  1：打开门禁  DOOR_OPEN
                else
                    tmpbuf[0] = (byte)0;
            }//fixed
            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 开门命令
        /// <summary>
        /// 开门命令
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="bFront">true:外侧，false:内侧</param>
        /// <returns></returns>
        public static bool CmdOpenDoor(string BoxBN, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.电控锁控制;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", bFront:"); msg.Append(bFront.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            if (!bFront)
                SendBuf[headerlen] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 查询箱头状态命令
        /// <summary>
        /// 查询箱头状态命令
        /// </summary>
        /// <param name="BoxBN">箱头BN地址</param>
        /// <returns></returns>
        public static bool CmdGetBoxState(string BoxBN)
        {
            return CmdGetBoxState(BoxBN, true);
        }
        public static bool CmdGetBoxState(string BoxBN, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.查询硬件状态;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 屏幕按钮控制
        /// <summary>
        /// 屏幕按钮控制
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="普发按键">true:显示，false:不显示</param>
        /// <returns></returns>
        public static bool CmdScreenButton(string BoxBN, bool 普发按键, bool 模板分发按键, bool 退出按键, bool 取消按键)
        {
            return CmdScreenButton(BoxBN, 普发按键, 模板分发按键, 退出按键, 取消按键, true);
        }
        public static bool CmdScreenButton(string BoxBN, bool 普发按键, bool 模板分发按键, bool 退出按键, bool 取消按键, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.屏幕按钮控制;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", 普发按键:"); msg.Append(普发按键.ToString());
            msg.Append(", 模板分发按键:"); msg.Append(模板分发按键.ToString());
            msg.Append(", 退出按键:"); msg.Append(退出按键.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            if (普发按键)
                SendBuf[headerlen] = (byte)1;
            if (模板分发按键)
                SendBuf[headerlen+1] = (byte)1;
            if (退出按键)
                SendBuf[headerlen+2] = (byte)1;
            if (取消按键)
                SendBuf[headerlen+3] = (byte)1;

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 取件平台界面控制
        /// <summary>
        /// 取件平台界面控制
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="ScreenType">界面选择。0：等待扫卡界面，1：三按钮取件界面，2：四按钮取件界面</param>
        /// <returns></returns>
        public static bool CmdQuJian(string BoxBN, int ScreenType)
        {
            return CmdQuJian(BoxBN, ScreenType, true);
        }
        public static bool CmdQuJian(string BoxBN, int ScreenType, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.取件平台界面控制;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", ScreenType:"); msg.Append(ScreenType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            SendBuf[headerlen] = (byte)ScreenType;

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 模板选择
        /// <summary>
        /// 模板选择
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="data">模板信息列表显示信息</param>
        /// <returns></returns>
        public static unsafe bool CmdTempletList(string BoxBN, string[] data)
        {
            return CmdTempletList(BoxBN, data, true);
        }
        public static unsafe bool CmdTempletList(string BoxBN, string[] data, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.模板选择;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", data len:"); msg.Append(data.Length.ToString());
			for (int i = 0; i < data.Length; i++)
			{
				msg.Append("\r\n\tData" + i.ToString() + ":" + data[i]);
			}
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
                tag_cmd_TempletList* c = (tag_cmd_TempletList*)tmpbuf;
                c->title_count = (byte)(data.Length);
                //Array.Copy(buf, 1, SendBuf, headerlen+8, buf.Length - 1);
                for (int i = 0; i < 16 && i<data.Length; i++)
                {
                    data[i] = BanQuan(data[i]);
                    byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(data[i]);
                    System.Runtime.InteropServices.Marshal.Copy(buf, 0, (IntPtr)(c->content + i * 16), (buf.Length > 16 ? 16 : buf.Length));
                }
                datalen = 8 + (data.Length > 16 ? 16 : data.Length) * 16;
            }//fixed
            //datalen = System.Runtime.InteropServices.Marshal.SizeOf(typeof(tag_cmd_TempletList));
            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

		#region 取件单位列表选择
		/// <summary>
		/// 取件单位列表选择
		/// </summary>
		/// <param name="BoxBN">BN地址</param>
		/// <param name="data">取件单位列表显示信息</param>
		/// <returns></returns>
		public static unsafe bool CmdUnitList(string BoxBN, string[] data)
		{
			return CmdUnitList(BoxBN, data, true);
		}
		public static unsafe bool CmdUnitList(string BoxBN, string[] data, bool bFront)
		{
			byte[] SendBuf = new byte[512];
			SendDataType iType = SendDataType.取件单位选择;

			System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
			msg.Append(BoxBN);
			msg.Append(", Type:"); msg.Append(iType.ToString());
			msg.Append(", data len:"); msg.Append(data.Length.ToString());
			for (int i = 0; i < data.Length; i++)
			{
				msg.Append("\r\n\tData" + i.ToString() + ":" + data[i]);
			}
			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

			//ip 错误
			if (BoxBN.Length != 7)
				return false;

			int headerlen = SetHeader(BoxBN, iType, SendBuf);
			int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
			fixed (byte* psbuf = SendBuf)
			{
				byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
				tag_cmd_TempletList* c = (tag_cmd_TempletList*)tmpbuf;
				c->title_count = (byte)(data.Length);
				//Array.Copy(buf, 1, SendBuf, headerlen+8, buf.Length - 1);
				for (int i = 0; i < 12 && i < data.Length; i++)
				{
					data[i] = BanQuan(data[i]);
					byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(data[i]);
					System.Runtime.InteropServices.Marshal.Copy(buf, 0, (IntPtr)(c->content + i * 16), (buf.Length > 16 ? 16 : buf.Length));
				}
				datalen = 8 + (data.Length > 12 ? 12 : data.Length) * 16;
			}//fixed
			//datalen = System.Runtime.InteropServices.Marshal.SizeOf(typeof(tag_cmd_TempletList));
			if (!bFront)
				SendBuf[headerlen + 7] = (byte)1;

			bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

			return bSend;
		}
		#endregion

        #region 硬件设置
        /// <summary>
        /// 硬件设置
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="baud">扫描头串口波特率。0：19200，1：115200</param>
        /// <param name="ph1Type">触发光电类型。0：反射，1：对射</param>
		/// <param name="ph2Type">Com3Type。串口3用途。0：扫描头，1：SYN6288语音模块，2：串口摄像头，3：指静脉仪</param>
        /// <param name="Com1Type">串口1用途。0：串口屏幕，1：TTS语音模块</param>
        /// <param name="Com2Type">串口2用途。0：RW163T读卡器，1：M104读卡器，2：ZLG522S读卡器 3：指静脉仪</param>
        /// <param name="DoorType">门禁电机工作方式。0：双向旋转，1：单向顺时针选择，2：单向逆时针旋转</param>
        /// <param name="ShengJiang">是否有升降机制，0：没有，1：有</param>
        /// <returns></returns>
		public static bool CmdBoxSet(string BoxBN, int baud, int ph1Type, int Com3Type, int Com1Type, int Com2Type, int DoorType, int ShengJiang)
        {
			return CmdBoxSet(BoxBN, baud, ph1Type, Com3Type, Com1Type, Com2Type, DoorType, ShengJiang, true);
        }
		public static bool CmdBoxSet(string BoxBN, int baud, int ph1Type, int Com3Type, int Com1Type, int Com2Type, int DoorType, int ShengJiang, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.硬件设置;
            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            SendBuf[headerlen] = (byte)baud;
            SendBuf[headerlen + 1] = (byte)ph1Type;
			SendBuf[headerlen + 2] = (byte)Com3Type;
            SendBuf[headerlen + 3] = (byte)Com1Type;
            SendBuf[headerlen + 4] = (byte)Com2Type;
            SendBuf[headerlen + 5] = (byte)DoorType;
            SendBuf[headerlen + 6] = (byte)ShengJiang;

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 重启箱组命令
        /// <summary>
        /// 重启箱组命令
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <returns></returns>
        public static bool CmdResetBox(string BoxBN)
        {
            return CmdResetBox(BoxBN, true);
        }
        public static bool CmdResetBox(string BoxBN, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.强制启动;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 强制箱格进入Idle状态
        /// <summary>
        /// 强制箱格进入Idle状态
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <returns></returns>
        public static bool CmdBoxToIdle(string BoxBN)
        {
            return CmdBoxToIdle(BoxBN, true);
        }
        public static bool CmdBoxToIdle(string BoxBN, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.强制空闲;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 在主显示屏上的各个区域显示提示文本
        /// <summary>
        /// 在主显示屏上的各个区域显示提示文本
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="which_block">0 单位简称，一般6个汉字  TEXT_SHORTNAME
        /// 1 箱格号显示，3个数字    TEXT_BOXNUMBER
        /// 2 单位详细名称，一般16个汉字 TEXT_LONGNAME
        /// 3 告警显示板0，不限汉字个数  TEXT_WARNINGMSG0
        /// 4 告警显示板1，不限汉字个数  TEXT_WARNINGMSG1
        /// 5 告警显示板2，不限汉字个数  TEXT_WARNINGMSG2
        /// </param>
        /// <param name="showTime">文字显示时间，以秒为单位。1-254为显示时间，0为取消显示，255为永久显示。</param>
        /// <param name="data">显示信息</param>
        /// <returns></returns>
        public static unsafe bool CmdLCDText(string BoxBN, int which_block, enum_TextType ShowType, int text_index, int showTime, string data)
        {
            return CmdLCDText(BoxBN, which_block, ShowType, text_index, showTime, data, true);
        }
        public static unsafe bool CmdLCDText(string BoxBN, int which_block, enum_TextType ShowType, int text_index, int showTime, string data, bool bFront)
        {
            //here
            data = BanQuan(data);
            if (data.Length>128) data = data.Substring(0,128);
            byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(data);
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.提示文本;

            return ShowText(BoxBN, which_block, ShowType, text_index, showTime, 0, 0, data, buf, SendBuf, iType, bFront);
        }
        /// <summary>
        /// 在主显示屏上的各个区域显示提示文本
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="which_block">0 单位简称，一般6个汉字  TEXT_SHORTNAME
        /// 1 箱格号显示，3个数字    TEXT_BOXNUMBER
        /// 2 单位详细名称，一般16个汉字 TEXT_LONGNAME
        /// 3 告警显示板0，不限汉字个数  TEXT_WARNINGMSG0
        /// 4 告警显示板1，不限汉字个数  TEXT_WARNINGMSG1
        /// 5 告警显示板2，不限汉字个数  TEXT_WARNINGMSG2
        /// </param>
        /// <param name="showTime">文字显示时间，以秒为单位。1-254为显示时间，0为取消显示，255为永久显示。</param>
        /// <param name="data">显示信息</param>
        /// <returns></returns>
        public static unsafe bool CmdLCDText_Com(string BoxBN, int which_block, enum_TextType ShowType, int text_index, int showTime, int x, int y, string data)
        {
            data = BanQuan(data);
            if (data.Length > 128) data = data.Substring(0, 128);
            byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(data);
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.提示文本串口屏幕;

            return ShowText(BoxBN, which_block, ShowType, text_index, showTime, x, y, data, buf, SendBuf, iType, true);
        }

        unsafe private static bool ShowText(string BoxBN, int which_block, enum_TextType ShowType, int text_index, int showTime, int x, int y, string data, byte[] buf, byte[] SendBuf, SendDataType iType, bool bFront)
        {
            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", which_block:"); msg.Append(which_block.ToString());
            msg.Append(", ShowType:"); msg.Append(ShowType.ToString());
            msg.Append(", text_index:"); msg.Append(text_index.ToString());
            msg.Append(", showTime:"); msg.Append(showTime.ToString());
            msg.Append(", data:"); msg.Append(data);
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
                tag_cmd_outtext* c = (tag_cmd_outtext*)tmpbuf;
                c->which_block = (byte)which_block;
                c->text_type = (byte)ShowType;
                c->text_index = (byte)text_index;
                c->show_time = (byte)showTime;
                c->text_len = (UInt16)buf.Length;
                c->x = (byte)x;
                c->y = (byte)y;
                //Array.Copy(buf, 1, SendBuf, headerlen+8, buf.Length - 1);
                System.Runtime.InteropServices.Marshal.Copy(buf, 0, (IntPtr)c->content, buf.Length);
                datalen = 8 + buf.Length;
            }//fixed
            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }


        public static unsafe bool CmdLCDText_4Dai(string BoxBN, int which_block, int showTime, string data)
        {
            //data = BanQuan(data);
            if (data.Length > 126) data = data.Substring(0, 126);
            data += "　";
            byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(data);
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.提示文本;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type: 提示文本_4Dai");
            msg.Append(", which_block:"); msg.Append(which_block.ToString());
            msg.Append(", showTime:"); msg.Append(showTime.ToString());
            msg.Append(", data:"); msg.Append(data);
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
                tag_cmd_outtext_4Dai* c = (tag_cmd_outtext_4Dai*)tmpbuf;
                c->which_block = (byte)3;
                c->text_font = 0;
                c->text_size = 0;
                c->text_color = 0;
                c->show_time = (byte)showTime;
                c->text_fx = 0;
                c->text_len = (byte)(buf.Length);
                c->text_Align = 0;
                //Array.Copy(buf, 1, SendBuf, headerlen+8, buf.Length - 1);
                System.Runtime.InteropServices.Marshal.Copy(buf, 0, (IntPtr)c->content, buf.Length);
                datalen = 8 + buf.Length;
            }//fixed

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 屏幕控制
        /// <summary>
        /// 屏幕控制
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="bShow">是否屏幕亮，true：亮，false：灭</param>
        /// <returns></returns>
        public static unsafe bool CmdScreen(string BoxBN, bool bShow)
        {
            return CmdScreen(BoxBN, bShow, true);
        }
        public static unsafe bool CmdScreen(string BoxBN, bool bShow, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.屏幕控制;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
			msg.Append(", bShow:"); msg.Append(bShow.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
                if (bShow)
                    tmpbuf[0] = (byte)0;			//0：屏幕正常显示，1：屏幕熄灭。
                else
                    tmpbuf[0] = (byte)1;
            }//fixed

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 语音提示
        /// <summary>
        /// 语音提示
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="tts_set">0：播放本地语音，1：设置本地语音，2：播放附带的文本</param>
        /// <param name="data">显示信息</param>
        /// <returns></returns>
        public static unsafe bool CmdSound(string BoxBN, enum_TextType tts_set, int tts_index, string data)
        {
            return CmdSound(BoxBN, tts_set, tts_index, data, true);
        }
        public static unsafe bool CmdSound(string BoxBN, enum_TextType tts_set, int tts_index, string data, bool bFront)
        {
            data = BanQuan(data);
            if (data.Length>16) data = data.Substring(0,16);
            byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(data);
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.语音提示;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", tts_set:"); msg.Append(tts_set.ToString());
            msg.Append(", tts_index:"); msg.Append(tts_index.ToString());
            msg.Append(", data:"); msg.Append(data);
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
                tag_cmd_Sound* c = (tag_cmd_Sound*)tmpbuf;
                c->tts_set = (byte)tts_set;
                c->tts_index = (byte)tts_index;
                c->text_len = (UInt16)buf.Length;
                //Array.Copy(buf, 1, SendBuf, headerlen+8, buf.Length - 1);
                System.Runtime.InteropServices.Marshal.Copy(buf, 0, (IntPtr)c->content, buf.Length);
                datalen = 8 + buf.Length;
            }//fixed

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 蜂鸣器控制
        /// <summary>
        /// 蜂鸣器控制
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="type">0：停止，
        /// 1：提示音，只响一声，然后自动停止
        /// 2：提示音，连续响3声，然后自动停止
        /// 3：错误提示音，连响3声，然后连续响，收到停止命令才停止</param>
        /// <returns></returns>
        public static unsafe bool CmdBuzzer(string BoxBN, int type)
        {
            return CmdBuzzer(BoxBN, type, true);
        }
        public static unsafe bool CmdBuzzer(string BoxBN, int type, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.蜂鸣器控制;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
                tmpbuf[0] = (byte)type;
            }//fixed

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 下发箱存控制
        /// <summary>
        /// 下发箱存控制
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
        /// <param name="type">0：停止，
        /// 1：提示音，只响一声，然后自动停止
        /// 2：提示音，连续响3声，然后自动停止
        /// 3：错误提示音，连响3声，然后连续响，收到停止命令才停止</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxLetterSurplus(string BoxBN, int count)
        {
            return CmdBoxLetterSurplus(BoxBN, count, true);
        }
        public static unsafe bool CmdBoxLetterSurplus(string BoxBN, int count, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.下发箱存控制;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", count:"); msg.Append(count.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
                tmpbuf[0] = (byte)(count & 0xff);
                tmpbuf[1] = (byte)((count>>8) & 0xff);
            }//fixed

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region 拍照指令
        /// <summary>
        /// 拍照指令
        /// </summary>
        /// <param name="BoxBN">BN地址</param>
		/// <param name="type">1：照相
        /// 2：获取数据</param>
        /// <returns></returns>
        public static unsafe bool CmdTakePhoto(string BoxBN, int type)
        {
			return CmdTakePhoto(BoxBN, type, true);
        }
		public static unsafe bool CmdTakePhoto(string BoxBN, int type, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.拍照指令;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip 错误
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
				tmpbuf[0] = (byte)(type);
            }//fixed

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

            return bSend;
        }
        #endregion

		#region 指静脉数据同步
		/// <summary>
		/// 准备同步指静脉
		/// </summary>
		/// <param name="BoxBN"></param>
		/// <param name="type">1:准备传输指静脉数据，2：数据发送完毕，3：结束数据传输</param>
		/// <param name="UserId"></param>
		/// <param name="buf"></param>
		/// <returns></returns>
		public static unsafe bool CmdUpdateZhiJingMai(string BoxBN, int type, int UserId, byte[] buf, int buflen)
		{
			byte[] SendBuf = new byte[512];
			SendDataType iType = SendDataType.准备指静脉更新;

			System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
			msg.Append(BoxBN);
			msg.Append(", Type:"); msg.Append(iType.ToString());
			msg.Append(", type:"); msg.Append(type.ToString());
			msg.Append(", UserId:"); msg.Append(UserId.ToString());
			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

			//ip 错误
			if (BoxBN.Length != 7)
				return false;

			long crc = 0;
			for (int i = 0; i < buflen; i++)
			{
				crc += buf[i];
			}

			int headerlen = SetHeader(BoxBN, iType, SendBuf);
			int datalen = 8;    //默认，多数命令都是8个字节数据，除了文字显示命令
			fixed (byte* psbuf = SendBuf)
			{
				byte* tmpbuf = psbuf + headerlen;	//数据开始的地方
				tmpbuf[0] = (byte)(type);
				tmpbuf[1] = (byte)(0);
				tmpbuf[2] = (byte)(buflen);
				tmpbuf[3] = (byte)(buflen >> 8);
				tmpbuf[4] = (byte)(crc);
				tmpbuf[5] = (byte)(crc>>8);
				tmpbuf[6] = (byte)(UserId);
				tmpbuf[7] = (byte)(UserId>>8);
			}//fixed

			bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

			return bSend;
		}
		/// <summary>
		/// 同步指静脉数据
		/// </summary>
		/// <returns></returns>
		public static unsafe bool CmdTransZhiJingMai(string BoxBN, byte[] buf, int buflen)
		{
			byte[] SendBuf = new byte[8192];
			SendDataType iType = SendDataType.指静脉数据传输;

			System.Text.StringBuilder msg = new System.Text.StringBuilder("发送数据, BN:");
			msg.Append(BoxBN);
			msg.Append(", Type:"); msg.Append(iType.ToString());
			msg.Append(", DataLen:"); msg.Append(buf.Length.ToString());
			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

			//ip 错误
			if (BoxBN.Length != 7)
				return false;

			int headerlen = SetHeader(BoxBN, iType, SendBuf);
			int datalen = buflen;    //默认，多数命令都是8个字节数据，除了文字显示命令
			Array.Copy(buf, 0, SendBuf, headerlen, buflen);

			bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "发送数据完成, 是否成功:" + bSend.ToString());

			return bSend;
		}
		#endregion



        #region 半角转全角
        private static string BanQuan(string source)
        {
            string sRet = "";
            for (int i = 0; i < source.Length; i++)
            {
                switch (source[i])
                {
                    #region 数字
                    case '0':
                        sRet += "零";
                        break;

                    case '1':
                        sRet += "一";
                        break;

                    case '2':
                        sRet += "二";
                        break;

                    case '3':
                        sRet += "三";
                        break;

                    case '4':
                        sRet += "四";
                        break;

                    case '5':
                        sRet += "五";
                        break;

                    case '6':
                        sRet += "六";
                        break;

                    case '7':
                        sRet += "七";
                        break;

                    case '8':
                        sRet += "八";
                        break;

                    case '9':
                        sRet += "九";
                        break;
                    #endregion

                    #region 小写字母
                    case 'a':
                        sRet += "ａ";
                        break;

                    case 'b':
                        sRet += "ｂ";
                        break;

                    case 'c':
                        sRet += "ｃ";
                        break;

                    case 'd':
                        sRet += "ｄ";
                        break;

                    case 'e':
                        sRet += "ｅ";
                        break;

                    case 'f':
                        sRet += "ｆ";
                        break;

                    case 'g':
                        sRet += "ｇ";
                        break;

                    case 'h':
                        sRet += "ｈ";
                        break;

                    case 'i':
                        sRet += "ｉ";
                        break;

                    case 'j':
                        sRet += "ｊ";
                        break;

                    case 'k':
                        sRet += "ｋ";
                        break;

                    case 'l':
                        sRet += "ｌ";
                        break;

                    case 'm':
                        sRet += "ｍ";
                        break;

                    case 'n':
                        sRet += "ｎ";
                        break;

                    case 'o':
                        sRet += "ｏ";
                        break;

                    case 'p':
                        sRet += "ｐ";
                        break;

                    case 'q':
                        sRet += "ｑ";
                        break;

                    case 'r':
                        sRet += "ｒ";
                        break;

                    case 's':
                        sRet += "ｓ";
                        break;

                    case 't':
                        sRet += "ｔ";
                        break;

                    case 'u':
                        sRet += "ｕ";
                        break;

                    case 'v':
                        sRet += "ｖ";
                        break;

                    case 'w':
                        sRet += "ｗ";
                        break;

                    case 'x':
                        sRet += "ｘ";
                        break;

                    case 'y':
                        sRet += "ｙ";
                        break;

                    case 'z':
                        sRet += "ｚ";
                        break;
                    #endregion

                    #region 大写字母
                    case 'A':
                        sRet += "Ａ";
                        break;

                    case 'B':
                        sRet += "Ｂ";
                        break;

                    case 'C':
                        sRet += "Ｃ";
                        break;

                    case 'D':
                        sRet += "Ｄ";
                        break;

                    case 'E':
                        sRet += "Ｅ";
                        break;

                    case 'F':
                        sRet += "Ｆ";
                        break;

                    case 'G':
                        sRet += "Ｇ";
                        break;

                    case 'H':
                        sRet += "Ｈ";
                        break;

                    case 'I':
                        sRet += "Ｉ";
                        break;

                    case 'J':
                        sRet += "Ｊ";
                        break;

                    case 'K':
                        sRet += "Ｋ";
                        break;

                    case 'L':
                        sRet += "Ｌ";
                        break;

                    case 'M':
                        sRet += "Ｍ";
                        break;

                    case 'N':
                        sRet += "Ｎ";
                        break;

                    case 'O':
                        sRet += "Ｏ";
                        break;

                    case 'P':
                        sRet += "Ｐ";
                        break;

                    case 'Q':
                        sRet += "Ｑ";
                        break;

                    case 'R':
                        sRet += "Ｒ";
                        break;

                    case 'S':
                        sRet += "Ｓ";
                        break;

                    case 'T':
                        sRet += "Ｔ";
                        break;

                    case 'U':
                        sRet += "Ｕ";
                        break;

                    case 'V':
                        sRet += "Ｖ";
                        break;

                    case 'W':
                        sRet += "Ｗ";
                        break;

                    case 'X':
                        sRet += "Ｘ";
                        break;

                    case 'Y':
                        sRet += "Ｙ";
                        break;

                    case 'Z':
                        sRet += "Ｚ";
                        break;
                    #endregion

                    #region 符号
                    case ' ':
                        sRet += "　";
                        break;

                    case ',':
                        sRet += "，";
                        break;

                    case '.':
                        sRet += "。";
                        break;

                    case ';':
                        sRet += "；";
                        break;

                    case ':':
                        sRet += "：";
                        break;

                    case '(':
                        sRet += "（";
                        break;

                    case ')':
                        sRet += "）";
                        break;

                    case '\'':
                        sRet += "＇";
                        break;

                    case '"':
                        sRet += "〃";
                        break;

                    case '\\':
                        sRet += "＼";
                        break;

                    case '/':
                        sRet += "／";
                        break;

                    case '-':
                        sRet += "－";
                        break;

                    case '_':
                        sRet += "＿";
                        break;

                    //case '\r':
                    //    sRet += "　";
                    //    break;

                    //case '\n':
                    //    sRet += "";
                    //    break;
                    #endregion

                    default:
                        sRet += source[i];
                        break;
                }
            }

            return sRet;
        }
        #endregion

        #region 发送数据
        private static bool SendDataToBox(int len, byte[] buf)
        {
            return ListenBox.Listen.SendData(len, buf); ;
        }
        #endregion

		#endregion

	}
}