using System;
using LogInfo;

namespace BoxDataParse
{

    public delegate void d_ReceiveBoxData(string BN_NO, ReceiveDataType iType, string data, bool bFront);

	/// <summary>
	/// DataParse ��ժҪ˵����
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

		#region ��������
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
                case 0x01:					//ɨ�赽����ֵ
                    bArm4Dai = true;
                    goto Label_TiaoMa;
                case 0x05:					//ɨ�赽����ֵ
                    Label_TiaoMa:
                    #region ɨ�赽����ֵ
                    if (OnReceiveBoxData != null)
                    {
                        DecodeBar decode = new DecodeBar();
                        int barlen = (int)(data[headerlen] | (data[headerlen + 1]<< 8));
                        /*
                       ���ڽ�������Э���Ҫ˵�����£�
                          ͷ�����ֽڹ̶�Ϊ0x52 0xe0��
                          ��3����4�ֽڱ�ʾ���ݵĳ��ȣ���3�ֽ�Ϊ���ֽڣ���4�ֽ�Ϊ���ֽڣ���������ݳ�����ָ�ӵ�5���ֽڵ����һ���ֽڵ��ֽ�����
                          ��5���ֽڱ�ʾ�������ͣ�
                          1��15��ʾSCDCC�루������ʷ�汾��ԭ��SCDCC��ռ�˶��ֵ��
                          128��ʾһά��
                          130��ʾPDF417��
                          ����ֵԤ����չʹ��
                       */
                        byte[] SCDCCBuffer = new byte[barlen];
                        Array.Copy(data, headerlen + 8, SCDCCBuffer, 0, SCDCCBuffer.Length);
                        string BarCode = "";
                        //��������£�ͷ2���ֽڶ���0x52��0xE0
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
                                if (SCDCCBuffer[4] == 0x80)  //һά
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
                            OnReceiveBoxData(BN_NO, ReceiveDataType.������Ϣ_4Dai, BarCode, bFront);
                        else
                            OnReceiveBoxData(BN_NO, ReceiveDataType.������Ϣ, BarCode, bFront);
                    }
					break;
                    #endregion

                case 0x02:					//ɨ�赽����ֵ
                    bArm4Dai = true;
                    goto Label_ZhengKa;
                case 0x06:					//�ϱ��û�ˢ���õ����û�����
                    Label_ZhengKa:
                    #region �ϱ��û�ˢ���õ����û�����.����Ϊ����Ա�������û�����
                    if (OnReceiveBoxData != null)
                    {
                        int barlen = (int)(data[headerlen] | (data[headerlen + 1] << 8));
                        int AuthType = data[headerlen + 2]; //��֤���ͣ�1=RFID��2=�Ӵ�ʽSAM����3=A/P�ʺ����룬
                        //4=USBԿ�ף�5=��ͨԿ�ף�6=ָ�����ݣ�7=Cpu����8=ָ����
                        string BarCode = "";
                        if (AuthType == 1 || AuthType == 2 || AuthType == 7)
                        {
                            BarCode = System.Text.Encoding.GetEncoding(936).GetString(data, headerlen+8, barlen);
                            if (bArm4Dai)
                                OnReceiveBoxData(BN_NO, ReceiveDataType.֤����Ϣ_4Dai, BarCode, bFront);
                            else
                                OnReceiveBoxData(BN_NO, ReceiveDataType.֤����Ϣ, BarCode, bFront);
                        }
                        else if (AuthType == 8)
                        {
                            //�ܹ���94�����ݣ������ݳ���Ϊ752�ֽڣ��������ֽ�Ч��ֵ��
                            //ǰ47��(0x00600380��0x006003ae)�����ֽ�Ч�飬λ��0x006003ae ������ֽ����ݡ�
                            //��47��(0x006003af��0x006003dd)�����ֽ�Ч�飬λ��0x006003dd ������ֽ����ݡ�
                            //Ч����ֽ���ǰ�����ֽ��ں�
                            byte[] buf = new byte[748];
                            Array.Copy(data, headerlen + 8, buf, 0, 374);
                            Array.Copy(data, headerlen + 8 + 376, buf, 374, 374);
                            BarCode = Convert.ToBase64String(buf);
                            CmdBuzzer(BN_NO, 1);
                            OnReceiveBoxData(BN_NO, ReceiveDataType.֤����Ϣ_ָ����, BarCode, bFront);
                        }
                    }
                    break;
                    #endregion

                case 0x07:					//Ͷ�Ż���Ͷ�䳷��
                    if (OnReceiveBoxData != null)
                    {
                        int ShowCount = (int)(data[headerlen + 1] | (data[headerlen + 2] << 8));
                        //1��Ͷ���źš�Ͷ��Ͷ������С�  DROP_ONE
                        //3��Ͷ�䳷���źš��û�������ߵȴ���ʱʱ�� DROP_CANCLE
                        //2: DROP_NONE
                        if (data[headerlen] == 1)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.Ͷ��Ͷ��, ShowCount.ToString(), bFront);
                        else
                            OnReceiveBoxData(BN_NO, ReceiveDataType.Ͷ�����, ShowCount.ToString(), bFront);
                    }
                    break;

				case 0x0B:	//����ָ����
					if (OnReceiveBoxData != null)
					{
						int iType = data[headerlen];
						if (iType == 8)
						{
							int datalen = (int)(data[headerlen + 1] | (data[headerlen + 2] << 8));
							int checkSum = (int)(data[headerlen + 3] | (data[headerlen + 4] << 8) | (data[headerlen + 5] << 16) | (data[headerlen + 6] << 24));
							byte[] buf = new byte[datalen];
							Array.Copy(data, headerlen + 8, buf, 0, datalen);

							//������Base64��ʽ�����ݣ����ǰѺ����0ȥ��
							int iLen = 0;
							for (; iLen < buf.Length; iLen++)
								if (buf[iLen] == 0)
									break;
							string szData = System.Text.Encoding.ASCII.GetString(buf, 0, iLen);

							OnReceiveBoxData(BN_NO, ReceiveDataType.֤����Ϣ_����ָ����, szData, true);
						}
					}
					break;

				case 0x62:
					#region ָ��������
					if (OnReceiveBoxData != null)
					{
						int MsgType = data[headerlen];
						int MsgValue = data[headerlen + 1];
						if (MsgType == 1)
						{
							if (MsgValue == 1)
								OnReceiveBoxData(BN_NO, ReceiveDataType.ָ��������_����, "��ǰ�豸�Ѿ�׼���ý���ָ����ģ������", true);
							else if (MsgValue == 2)
								OnReceiveBoxData(BN_NO, ReceiveDataType.ָ��������_����, "�������ݴ�����֤�ɹ�", true);
							else
								OnReceiveBoxData(BN_NO, ReceiveDataType.ָ��������_����, "error", true);
						}
						else if (MsgType == 2)
						{
							if (MsgValue == 1)
								OnReceiveBoxData(BN_NO, ReceiveDataType.ָ��������_д��, "д�������ݳɹ�", true);
							else
								OnReceiveBoxData(BN_NO, ReceiveDataType.ָ��������_д��, "д��������ʧ��", true);
						}
						else if (MsgType == 3)
						{
							int UserId = data[headerlen + 2] + data[headerlen + 3] * 256;
							if (MsgValue == 1)
								OnReceiveBoxData(BN_NO, ReceiveDataType.ָ��������_��֤, UserId.ToString(), true);
							else
								OnReceiveBoxData(BN_NO, ReceiveDataType.ָ��������_��֤, "-1", true);
						}
					}
					#endregion
					break;

				case 0x80:					//RPT_UnitList: ״̬rpt_code=0x80����(ȡ����λѡ��ظ�)
					#region ȡ����λѡ��ظ�
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
							OnReceiveBoxData(BN_NO, ReceiveDataType.ȡ����λѡ��, indexStr, bFront);
					}
					#endregion
					break;

                case 0x81:					//�Ž�
                    #region �Ž�
                    {
                        string cmdstr = "";
                        if (data[headerlen+1] == 1)//�Ž��رճ�ʱ
                            cmdstr = "CloseTimeOut";
                        else if (data[headerlen + 1] == 2)//�Ž��򿪳�ʱ
                            cmdstr = "OpenTimeOut";
                        else if (data[headerlen + 1] == 0)
                        {
                            if (data[headerlen] == 0) //0���Ž��ر�   CURSTAT_DOOR_CLOSE
                            {
                                cmdstr = "Closed";
                            }
                            else if (data[headerlen] == 1) //1���Ž���   CURSTAT_DOOR_OPEN
                            {
                                cmdstr = "Opened";
                            }
                        }
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.�Ž�, cmdstr, bFront);
                    }
                    #endregion
                    break;

                case 0x82:					//����
                    #region ����
                    {
                        string cmdstr = "";
                        if (data[headerlen + 1] == 2)//�����򿪳�ʱ
                            cmdstr = "OpenTimeOut";
                        else if (data[headerlen + 1] == 0)
                        {
                            if (data[headerlen] == 0) //0�������ر�
                            {
                                cmdstr = "Closed";
                            }
                            else if (data[headerlen] == 1) //1��������
                            {
                                cmdstr = "Opened";
                            }
                        }
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.����, cmdstr, bFront);
                    }
                    #endregion
                    break;

                case 0x83:					//���
                    {
                        #region ���
                        int tmp = (int)data[headerlen];
                        int error = (int)data[headerlen+1];
                        string cmdstr = "";

                        if ((error & 0x01) == 0)//0λ��0: ǰ���������1��ǰ������
                        {
                            if ((tmp & 0x01) == 0) //0λ��0:ǰ���δ�ڵ���1��ǰ����ڵ�
                            {
                                cmdstr = "ǰ���δ�ڵ�";
                            }
                            else
                            {
                                cmdstr = "ǰ����ڵ�";
                            }
                        }
                        else
                            cmdstr = "ǰ������";
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.���, cmdstr, bFront);

                        if ((error & 0x02) == 0)//1λ��0������������1���������
                        {
                            if ((tmp & 0x02) == 0) //1λ��0:����δ�ڵ���1�������ڵ� CURSTAT_PH_BACK
                            {
                                cmdstr = "����δ�ڵ�";
                            }
                            else
                            {
                                cmdstr = "�����ڵ�";
                            }
                        }
                        else
                            cmdstr = "�������";
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.���, cmdstr, bFront);

                        if ((error & 0x04) == 0)//2λ��0���������������1������������
                        {
                            if ((tmp & 0x04) == 0) //2λ��0: ������δ�ڵ���1����������ڵ�
                            {
                                cmdstr = "������δ�ڵ�";
                            }
                            else
                            {
                                cmdstr = "��������ڵ�";
                            }
                        }
                        else
                            cmdstr = "����������";
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.���, cmdstr, bFront);

                        if ((error & 0x08) == 0)//3λ��0����չ��������1����չ�����
                        {
                            if ((tmp & 0x08) == 0) //3λ��0: ��չ�δ�ڵ���1����չ���ڵ�
                            {
                                cmdstr = "��չ��δ�ڵ�";
                            }
                            else
                            {
                                cmdstr = "��չ���ڵ�";
                            }
                        }
                        else
                            cmdstr = "��չ�����";
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.���, cmdstr, bFront);

                        if ((error & 0x10) == 0)//4λ��0���������������1������������
                        {
                            if ((tmp & 0x10) == 0) //4λ��0: �������δ�ڵ���1����������ڵ�
                            {
                                cmdstr = "�������δ�ڵ�";
                            }
                            else
                            {
                                cmdstr = "��������ڵ�";
                            }
                        }
                        else
                            cmdstr = "����������";
                        if (OnReceiveBoxData != null)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.���, cmdstr, bFront);

                        #endregion
                    }
                    break;

                case 0x84:					//�û�����
					if(OnReceiveBoxData!=null)
                        OnReceiveBoxData(BN_NO, ReceiveDataType.����, data[headerlen].ToString(), bFront);
					break;

                case 0x85:					//0x85����(Ӳ������
                    if (OnReceiveBoxData != null)
                    {
                        #region
                        //ɨ��ͷ�������á�0��ɨ��ͷ������19200��1��ɨ��ͷ������115200
                        //2����������ͷ������19200��3����������ͷ������38400��
                        //4����������ͷ������57600��5����������ͷ������115200��
                        string str = "";
                        if (data[headerlen] == 0)//ɨ��ͷ���ڲ����ʡ�0��19200��1��115200
                            str += "ɨ��ͷ������=19200";
                        else if (data[headerlen] == 1)
                            str += "ɨ��ͷ������=115200";
                        else if (data[headerlen] == 2)
                            str += "��������ͷ������=19200";
                        else if (data[headerlen] == 3)
                            str += "��������ͷ������=38400";
                        else if (data[headerlen] == 4)
                            str += "��������ͷ������=57600";
                        else if (data[headerlen] == 5)
                            str += "��������ͷ������=115200";

                        str += "�������������=";
                        if (data[headerlen+1] == 0)//����������͡�0�����䣬1������
                            str += "����";
                        else
                            str += "����";

						str += "������3��;=";
						if (data[headerlen + 2] == 0)//����3��;��0��ɨ��ͷ��1��SYN6288����ģ�飬2����������ͷ��//3��ָ������
							str += "ɨ��ͷ";
						else if (data[headerlen + 2] == 1)
							str += "SYN6288����ģ��";
						else if (data[headerlen + 2] == 2)
							str += "��������ͷ";
						else if (data[headerlen + 2] == 3)
							str += "ָ������";

                        str += "������1��;=";
                        if (data[headerlen + 3] == 0)//����1��;��0��������Ļ��1��TTS����ģ�飬2����������ͷ
                            str += "������Ļ";
                        else if (data[headerlen + 3] == 1)
                            str += "TTS����ģ��";
                        else if (data[headerlen + 3] == 2)
                            str += "��������ͷ";

                        str += "������2��;=";
						if (data[headerlen + 4] == 0)//����2��;��0��RW163T��������1��M104��������2��ZLG522S������, 3��ָ�����ǣ�4. SYN6288����ģ��
                            str += "RW163T������";
                        else if (data[headerlen + 4] == 1)
                            str += "M104������";
                        else if (data[headerlen + 4] == 2)
                            str += "ZLG522S������";
                        else if (data[headerlen + 4] == 3)
                            str += "ָ������";
						else if (data[headerlen + 4] == 4)
							str += "SYN6288����ģ��";

                        str += "���Ž����������ʽ=";
                        if (data[headerlen + 5] == 0)//�Ž����������ʽ��0��˫����ת��1������˳ʱ��ѡ��2��������ʱ����ת
                            str += "˫����ת";
                        else if (data[headerlen + 5] == 1)
                            str += "����˳ʱ��ѡ��";
                        else
                            str += "������ʱ����ת";

                        str += "���Ƿ�����������=";
                        if (data[headerlen + 6] == 0)//�Ƿ����������ƣ�0���ޣ�1����������2���ڶ��������3��˫����ʾ, 4���ڶ��������˫����ʾ��5����������˫����ʾ
                            str += "û����������";
                        else if (data[headerlen + 6] == 1)
                            str += "����������";
                        else if (data[headerlen + 6] == 2)
                            str += "�еڶ������";
                        else if (data[headerlen + 6] == 3)
                            str += "��˫����ʾ";
                        else if (data[headerlen + 6] == 4)
                            str += "�ڶ��������˫����ʾ";
                        else if (data[headerlen + 6] == 5)
                            str += "��������˫����ʾ";
                        else
                            str += "δ֪";

                        OnReceiveBoxData(BN_NO, ReceiveDataType.Ӳ��״̬, str, bFront);
                        #endregion
                    }
					break;

                case 0x86:					//�������״̬
					if(OnReceiveBoxData!=null)
					{
                        int times = data[headerlen] | (data[headerlen+1]<<8) | (data[headerlen+2]<<16) | (data[headerlen+3]<<24);
                        string version = data[headerlen + 7].ToString("X2") + data[headerlen + 6].ToString("X2")
                            + data[headerlen + 5].ToString("X2") + data[headerlen + 4].ToString("X2");
                        if (times==0)
                            OnReceiveBoxData(BN_NO, ReceiveDataType.��������, "Start", true);
                        else
                            OnReceiveBoxData(BN_NO, ReceiveDataType.��������, "Ping", true);
                        OnReceiveBoxData(BN_NO, ReceiveDataType.��������_Version, version.ToString(), true);
					}
					break;

                case 0xE8:					//�ϱ��������Ƭ
                    #region �ϱ��������Ƭ
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
                        OnReceiveBoxData(BN_NO, ReceiveDataType.�ϱ��������Ƭ, pic, true);
                    }
                    break;
                    #endregion

			}
		}
		#endregion

		#region ��������
        #region ���ݰ�ͷ
        private static unsafe int SetHeader(string BN_NO, SendDataType iType, byte[] buf)
        {
            int headerlen = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CMDHeader));
            fixed (byte* psbuf = buf)
            {
                CMDHeader* header = (CMDHeader*)psbuf;
                header->rpt_code = (byte)iType;		//����
                header->bn_m2 = (byte)Convert.ToInt16(BN_NO.Substring(2, 1));
                header->bn_m1 = (byte)Convert.ToInt16(BN_NO.Substring(3, 2));
                header->bn_m0 = (byte)Convert.ToInt16(BN_NO.Substring(5, 2));
            }
            return headerlen;
        }
        #endregion

        #region ׼��Ͷ���ź�
        /// <summary>
        /// ׼��Ͷ���ź�
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <returns></returns>
        public static unsafe bool CmdPreGetLetter(string BoxBN, int type, int style, int showcount)
        {
            return CmdPreGetLetter(BoxBN, type, style, showcount, true);
        }
        /// <summary>
        /// ׼��Ͷ���ź�
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <returns></returns>
        public static unsafe bool CmdPreGetLetter(string BoxBN, int type, int style, int showcount, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.׼��Ͷ��;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType);
            msg.Append(", type:"); msg.Append(type.ToString());
            msg.Append(", style:"); msg.Append(style.ToString());
            msg.Append(", showcount:"); msg.Append(showcount.ToString());
            Log.WriteInfo(LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                tag_cmd_presend* c = (tag_cmd_presend*)(psbuf + headerlen);//���ݿ�ʼ�ĵط�
                c->type = (byte)type;
                c->style = (byte)style;
                c->shownum = (UInt16)showcount;
            }//fixed
            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());
            return bSend;
        }
        #endregion

        #region ɨ��ͷ����
        /// <summary>
        /// ɨ��ͷ����
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="bOpen">����״̬��true������false����</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxScan(string BoxBN, bool bOpen)
        {
            return CmdBoxScan(BoxBN, bOpen, true);
        }
        /// <summary>
        /// ɨ��ͷ����
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="bOpen">����״̬��true������false����</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxScan(string BoxBN, bool bOpen, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.ɨ��ͷ����;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", bOpen:"); msg.Append(bOpen.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
                if (bOpen)
                    tmpbuf[0] = (byte)1;			//0����ɨ��ͷ��1����ɨ��ͷ
                else
                    tmpbuf[0] = (byte)0;
            }//fixed
            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ����ͷ����
        /// <summary>
        /// ����ͷ����
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <returns></returns>
        public static unsafe bool CmdCamera(string BoxBN)
        {
            return CmdCamera(BoxBN, true);
        }
        /// <summary>
        /// ����ͷ����
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <returns></returns>
        public static unsafe bool CmdCamera(string BoxBN, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.����ͷ����;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ���Ƶ�����

        #region ���ƴ�Ͷ��
        /// <summary>
        /// ���ƴ�Ͷ��
        /// </summary>
        /// <param name="boxBn">BN��ַ</param>
        /// <param name="cType">����״̬������������</param>
        /// <returns></returns>
        public static bool CmdBoxLampDaiTou(string boxBn, enum_LampStatus cType)
        {
            return CmdBoxLampByBN(boxBn, 8, cType);
        }
        #endregion

        #region ���ƴ����
        /// <summary>
        /// ���ƴ����
        /// </summary>
        /// <param name="boxBn">BN��ַ</param>
        /// <param name="cType">����״̬������������</param>
        /// <returns></returns>
        public static bool CmdBoxLampCuoWu(string boxBn, enum_LampStatus cType)
        {
            return CmdBoxLampByBN(boxBn, 4, cType);
        }
        #endregion

        #region ���Ƽ�����
        /// <summary>
        /// ���Ƽ�����
        /// </summary>
        /// <param name="boxBn">BN��ַ</param>
        /// <param name="cType">����״̬������������</param>
        /// <returns></returns>
        public static bool CmdBoxLampJiJian(string boxBn, enum_LampStatus cType)
        {
            return CmdBoxLampByBN(boxBn, 2, cType);
        }
        #endregion

        #region ������ȡ��
        /// <summary>
        /// ������ȡ��
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="cType">����״̬������������</param>
        /// <returns></returns>
        public static bool CmdBoxLampYiQu(string BoxBN, enum_LampStatus cType)
        {
            return CmdBoxLampByBN(BoxBN, 1, cType);
        }
        #endregion

        #region ���Ƶ�����
        /// <summary>
        /// ���Ƶ�����
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="ilamp">����ţ�1����ȡ��2��������4������</param>
        /// <param name="cType">����״̬������������</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxLampByBN(string BoxBN, int ilamp, enum_LampStatus cType)
        {
            return CmdBoxLampByBN(BoxBN, ilamp, cType, true);
        }
        /// <summary>
        /// ���Ƶ�����
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="ilamp">����ţ�1����ȡ��2��������4������</param>
        /// <param name="cType">����״̬������������</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxLampByBN(string BoxBN, int ilamp, enum_LampStatus cType, bool bFront)
        {
            byte[] sendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.����ָʾ��;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", ilamp:"); msg.Append(ilamp.ToString());
            msg.Append(", cType:"); msg.Append(cType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, sendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = sendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�

                tmpbuf[0] = (byte)ilamp;		//which_led;  //ָʾ�Ƶ���ţ����������ͬ�����Զ��һ������
                tmpbuf[1] = (byte)cType;		//style; 		 0���� 1���� 2��˸
                tmpbuf[2] = 0x3;			//��˸��ʱ�䣨��λʮ��֮һ�룬ds����ֻ��Style=2Hʱ��Ч��
                tmpbuf[3] = 0x5;            //��˸��ʱ�䣨��λʮ��֮һ�룬ds����ֻ��Style=2Hʱ��Ч��
            }//fixed
            if (!bFront)
                sendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), sendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion
        #endregion

        #region ����ܲ���
        /// <summary>
        /// ����ܲ���
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="whichLed">0��������Զ��Ӧ���Ӻţ�LNT_LNT0
        /// 1��9 Ϊ������LNT888�����ʾ�������£�
        /// 1����Ļ���½ǵ���������ʾ����ʾ��� LNT_LNT1�� ��Ӧ��Ͷ������
        /// 2����Ļ���½ǵ�һ����ʾ ,���LNT_LNT2  ��Ӧ�Ǽ�������1��
        /// 3����Ļ���½ǵڶ�����ʾ  ���LNT_LNT3  ��Ӧ�Ǽ�������2��
        /// 4����Ļ���½ǵ�������ʾ  ���LNT_LNT4 ��Ӧ�Ǽ�������3��
        /// 5��9�Դ�����
        /// ��20~99˫λ������ʾ�ѵǼ��������ϸ
        /// ���磬30 Ϊ�Ǽǵ�2��ĵ�һ����ϸ��Ŀ������31Ϊ�ڶ�����������
        /// 20��29 ��ʾ ��1��ĸ�����ϸ��������
        /// �Դ�����
        /// </param>
        /// <param name="ledNumber">�ż���Ŀ</param>
        /// <param name="bFlash">����״̬��false������true����</param>
        /// <param name="c">��ʾ��ɫ</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxLED(string BoxBN, int whichLed, int ledNumber, enum_LedColor c)
        {
            return CmdBoxLED(BoxBN, whichLed, ledNumber, c, true);
        }
        public static unsafe bool CmdBoxLED(string BoxBN, int whichLed, int ledNumber, enum_LedColor c, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.���������;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", whichLed:"); msg.Append(whichLed.ToString());
            msg.Append(", ledNumber:"); msg.Append(ledNumber.ToString());
            msg.Append(", LedColor:"); msg.Append(c.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
                tmpbuf[0] = (byte)whichLed;		//which_lnt; //LNT����ܵ���ţ����������ͬ�����Զ��һ������
                tmpbuf[1] = (byte)c;
                tmpbuf[2] = (byte)(ledNumber); 
                tmpbuf[3] = (byte)(ledNumber>>8);
            }//fixed
            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region �����Ž�����
        /// <summary>
        /// �����Ž�����
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="bOpen">����״̬��true������false����</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxGating(string BoxBN, bool bOpen)
        {
            return CmdBoxGating(BoxBN, bOpen, true);
        }
        public static unsafe bool CmdBoxGating(string BoxBN, bool bOpen, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.�Ž�����;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
                if (bOpen)
                    tmpbuf[0] = (byte)1;			//ctl 0���ر��Ž�  DOOR_CLOSE  1�����Ž�  DOOR_OPEN
                else
                    tmpbuf[0] = (byte)0;
            }//fixed
            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ��������
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="bFront">true:��࣬false:�ڲ�</param>
        /// <returns></returns>
        public static bool CmdOpenDoor(string BoxBN, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.���������;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", bFront:"); msg.Append(bFront.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            if (!bFront)
                SendBuf[headerlen] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ��ѯ��ͷ״̬����
        /// <summary>
        /// ��ѯ��ͷ״̬����
        /// </summary>
        /// <param name="BoxBN">��ͷBN��ַ</param>
        /// <returns></returns>
        public static bool CmdGetBoxState(string BoxBN)
        {
            return CmdGetBoxState(BoxBN, true);
        }
        public static bool CmdGetBoxState(string BoxBN, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.��ѯӲ��״̬;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ��Ļ��ť����
        /// <summary>
        /// ��Ļ��ť����
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="�շ�����">true:��ʾ��false:����ʾ</param>
        /// <returns></returns>
        public static bool CmdScreenButton(string BoxBN, bool �շ�����, bool ģ��ַ�����, bool �˳�����, bool ȡ������)
        {
            return CmdScreenButton(BoxBN, �շ�����, ģ��ַ�����, �˳�����, ȡ������, true);
        }
        public static bool CmdScreenButton(string BoxBN, bool �շ�����, bool ģ��ַ�����, bool �˳�����, bool ȡ������, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.��Ļ��ť����;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", �շ�����:"); msg.Append(�շ�����.ToString());
            msg.Append(", ģ��ַ�����:"); msg.Append(ģ��ַ�����.ToString());
            msg.Append(", �˳�����:"); msg.Append(�˳�����.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            if (�շ�����)
                SendBuf[headerlen] = (byte)1;
            if (ģ��ַ�����)
                SendBuf[headerlen+1] = (byte)1;
            if (�˳�����)
                SendBuf[headerlen+2] = (byte)1;
            if (ȡ������)
                SendBuf[headerlen+3] = (byte)1;

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ȡ��ƽ̨�������
        /// <summary>
        /// ȡ��ƽ̨�������
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="ScreenType">����ѡ��0���ȴ�ɨ�����棬1������ťȡ�����棬2���İ�ťȡ������</param>
        /// <returns></returns>
        public static bool CmdQuJian(string BoxBN, int ScreenType)
        {
            return CmdQuJian(BoxBN, ScreenType, true);
        }
        public static bool CmdQuJian(string BoxBN, int ScreenType, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.ȡ��ƽ̨�������;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", ScreenType:"); msg.Append(ScreenType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            SendBuf[headerlen] = (byte)ScreenType;

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ģ��ѡ��
        /// <summary>
        /// ģ��ѡ��
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="data">ģ����Ϣ�б���ʾ��Ϣ</param>
        /// <returns></returns>
        public static unsafe bool CmdTempletList(string BoxBN, string[] data)
        {
            return CmdTempletList(BoxBN, data, true);
        }
        public static unsafe bool CmdTempletList(string BoxBN, string[] data, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.ģ��ѡ��;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", data len:"); msg.Append(data.Length.ToString());
			for (int i = 0; i < data.Length; i++)
			{
				msg.Append("\r\n\tData" + i.ToString() + ":" + data[i]);
			}
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
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

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

		#region ȡ����λ�б�ѡ��
		/// <summary>
		/// ȡ����λ�б�ѡ��
		/// </summary>
		/// <param name="BoxBN">BN��ַ</param>
		/// <param name="data">ȡ����λ�б���ʾ��Ϣ</param>
		/// <returns></returns>
		public static unsafe bool CmdUnitList(string BoxBN, string[] data)
		{
			return CmdUnitList(BoxBN, data, true);
		}
		public static unsafe bool CmdUnitList(string BoxBN, string[] data, bool bFront)
		{
			byte[] SendBuf = new byte[512];
			SendDataType iType = SendDataType.ȡ����λѡ��;

			System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
			msg.Append(BoxBN);
			msg.Append(", Type:"); msg.Append(iType.ToString());
			msg.Append(", data len:"); msg.Append(data.Length.ToString());
			for (int i = 0; i < data.Length; i++)
			{
				msg.Append("\r\n\tData" + i.ToString() + ":" + data[i]);
			}
			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

			//ip ����
			if (BoxBN.Length != 7)
				return false;

			int headerlen = SetHeader(BoxBN, iType, SendBuf);
			int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
			fixed (byte* psbuf = SendBuf)
			{
				byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
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

			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

			return bSend;
		}
		#endregion

        #region Ӳ������
        /// <summary>
        /// Ӳ������
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="baud">ɨ��ͷ���ڲ����ʡ�0��19200��1��115200</param>
        /// <param name="ph1Type">����������͡�0�����䣬1������</param>
		/// <param name="ph2Type">Com3Type������3��;��0��ɨ��ͷ��1��SYN6288����ģ�飬2����������ͷ��3��ָ������</param>
        /// <param name="Com1Type">����1��;��0��������Ļ��1��TTS����ģ��</param>
        /// <param name="Com2Type">����2��;��0��RW163T��������1��M104��������2��ZLG522S������ 3��ָ������</param>
        /// <param name="DoorType">�Ž����������ʽ��0��˫����ת��1������˳ʱ��ѡ��2��������ʱ����ת</param>
        /// <param name="ShengJiang">�Ƿ����������ƣ�0��û�У�1����</param>
        /// <returns></returns>
		public static bool CmdBoxSet(string BoxBN, int baud, int ph1Type, int Com3Type, int Com1Type, int Com2Type, int DoorType, int ShengJiang)
        {
			return CmdBoxSet(BoxBN, baud, ph1Type, Com3Type, Com1Type, Com2Type, DoorType, ShengJiang, true);
        }
		public static bool CmdBoxSet(string BoxBN, int baud, int ph1Type, int Com3Type, int Com1Type, int Com2Type, int DoorType, int ShengJiang, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.Ӳ������;
            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
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

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ������������
        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <returns></returns>
        public static bool CmdResetBox(string BoxBN)
        {
            return CmdResetBox(BoxBN, true);
        }
        public static bool CmdResetBox(string BoxBN, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.ǿ������;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ǿ��������Idle״̬
        /// <summary>
        /// ǿ��������Idle״̬
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <returns></returns>
        public static bool CmdBoxToIdle(string BoxBN)
        {
            return CmdBoxToIdle(BoxBN, true);
        }
        public static bool CmdBoxToIdle(string BoxBN, bool bFront)
        {
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.ǿ�ƿ���;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ������ʾ���ϵĸ���������ʾ��ʾ�ı�
        /// <summary>
        /// ������ʾ���ϵĸ���������ʾ��ʾ�ı�
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="which_block">0 ��λ��ƣ�һ��6������  TEXT_SHORTNAME
        /// 1 ������ʾ��3������    TEXT_BOXNUMBER
        /// 2 ��λ��ϸ���ƣ�һ��16������ TEXT_LONGNAME
        /// 3 �澯��ʾ��0�����޺��ָ���  TEXT_WARNINGMSG0
        /// 4 �澯��ʾ��1�����޺��ָ���  TEXT_WARNINGMSG1
        /// 5 �澯��ʾ��2�����޺��ָ���  TEXT_WARNINGMSG2
        /// </param>
        /// <param name="showTime">������ʾʱ�䣬����Ϊ��λ��1-254Ϊ��ʾʱ�䣬0Ϊȡ����ʾ��255Ϊ������ʾ��</param>
        /// <param name="data">��ʾ��Ϣ</param>
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
            SendDataType iType = SendDataType.��ʾ�ı�;

            return ShowText(BoxBN, which_block, ShowType, text_index, showTime, 0, 0, data, buf, SendBuf, iType, bFront);
        }
        /// <summary>
        /// ������ʾ���ϵĸ���������ʾ��ʾ�ı�
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="which_block">0 ��λ��ƣ�һ��6������  TEXT_SHORTNAME
        /// 1 ������ʾ��3������    TEXT_BOXNUMBER
        /// 2 ��λ��ϸ���ƣ�һ��16������ TEXT_LONGNAME
        /// 3 �澯��ʾ��0�����޺��ָ���  TEXT_WARNINGMSG0
        /// 4 �澯��ʾ��1�����޺��ָ���  TEXT_WARNINGMSG1
        /// 5 �澯��ʾ��2�����޺��ָ���  TEXT_WARNINGMSG2
        /// </param>
        /// <param name="showTime">������ʾʱ�䣬����Ϊ��λ��1-254Ϊ��ʾʱ�䣬0Ϊȡ����ʾ��255Ϊ������ʾ��</param>
        /// <param name="data">��ʾ��Ϣ</param>
        /// <returns></returns>
        public static unsafe bool CmdLCDText_Com(string BoxBN, int which_block, enum_TextType ShowType, int text_index, int showTime, int x, int y, string data)
        {
            data = BanQuan(data);
            if (data.Length > 128) data = data.Substring(0, 128);
            byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(data);
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.��ʾ�ı�������Ļ;

            return ShowText(BoxBN, which_block, ShowType, text_index, showTime, x, y, data, buf, SendBuf, iType, true);
        }

        unsafe private static bool ShowText(string BoxBN, int which_block, enum_TextType ShowType, int text_index, int showTime, int x, int y, string data, byte[] buf, byte[] SendBuf, SendDataType iType, bool bFront)
        {
            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", which_block:"); msg.Append(which_block.ToString());
            msg.Append(", ShowType:"); msg.Append(ShowType.ToString());
            msg.Append(", text_index:"); msg.Append(text_index.ToString());
            msg.Append(", showTime:"); msg.Append(showTime.ToString());
            msg.Append(", data:"); msg.Append(data);
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
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

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }


        public static unsafe bool CmdLCDText_4Dai(string BoxBN, int which_block, int showTime, string data)
        {
            //data = BanQuan(data);
            if (data.Length > 126) data = data.Substring(0, 126);
            data += "��";
            byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(data);
            byte[] SendBuf = new byte[bufLen];
            SendDataType iType = SendDataType.��ʾ�ı�;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type: ��ʾ�ı�_4Dai");
            msg.Append(", which_block:"); msg.Append(which_block.ToString());
            msg.Append(", showTime:"); msg.Append(showTime.ToString());
            msg.Append(", data:"); msg.Append(data);
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
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

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ��Ļ����
        /// <summary>
        /// ��Ļ����
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="bShow">�Ƿ���Ļ����true������false����</param>
        /// <returns></returns>
        public static unsafe bool CmdScreen(string BoxBN, bool bShow)
        {
            return CmdScreen(BoxBN, bShow, true);
        }
        public static unsafe bool CmdScreen(string BoxBN, bool bShow, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.��Ļ����;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
			msg.Append(", bShow:"); msg.Append(bShow.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
                if (bShow)
                    tmpbuf[0] = (byte)0;			//0����Ļ������ʾ��1����ĻϨ��
                else
                    tmpbuf[0] = (byte)1;
            }//fixed

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ������ʾ
        /// <summary>
        /// ������ʾ
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="tts_set">0�����ű���������1�����ñ���������2�����Ÿ������ı�</param>
        /// <param name="data">��ʾ��Ϣ</param>
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
            SendDataType iType = SendDataType.������ʾ;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", tts_set:"); msg.Append(tts_set.ToString());
            msg.Append(", tts_index:"); msg.Append(tts_index.ToString());
            msg.Append(", data:"); msg.Append(data);
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
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

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ����������
        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="type">0��ֹͣ��
        /// 1����ʾ����ֻ��һ����Ȼ���Զ�ֹͣ
        /// 2����ʾ����������3����Ȼ���Զ�ֹͣ
        /// 3��������ʾ��������3����Ȼ�������죬�յ�ֹͣ�����ֹͣ</param>
        /// <returns></returns>
        public static unsafe bool CmdBuzzer(string BoxBN, int type)
        {
            return CmdBuzzer(BoxBN, type, true);
        }
        public static unsafe bool CmdBuzzer(string BoxBN, int type, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.����������;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
                tmpbuf[0] = (byte)type;
            }//fixed

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region �·�������
        /// <summary>
        /// �·�������
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
        /// <param name="type">0��ֹͣ��
        /// 1����ʾ����ֻ��һ����Ȼ���Զ�ֹͣ
        /// 2����ʾ����������3����Ȼ���Զ�ֹͣ
        /// 3��������ʾ��������3����Ȼ�������죬�յ�ֹͣ�����ֹͣ</param>
        /// <returns></returns>
        public static unsafe bool CmdBoxLetterSurplus(string BoxBN, int count)
        {
            return CmdBoxLetterSurplus(BoxBN, count, true);
        }
        public static unsafe bool CmdBoxLetterSurplus(string BoxBN, int count, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.�·�������;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            msg.Append(", count:"); msg.Append(count.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
                tmpbuf[0] = (byte)(count & 0xff);
                tmpbuf[1] = (byte)((count>>8) & 0xff);
            }//fixed

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

        #region ����ָ��
        /// <summary>
        /// ����ָ��
        /// </summary>
        /// <param name="BoxBN">BN��ַ</param>
		/// <param name="type">1������
        /// 2����ȡ����</param>
        /// <returns></returns>
        public static unsafe bool CmdTakePhoto(string BoxBN, int type)
        {
			return CmdTakePhoto(BoxBN, type, true);
        }
		public static unsafe bool CmdTakePhoto(string BoxBN, int type, bool bFront)
        {
            byte[] SendBuf = new byte[512];
            SendDataType iType = SendDataType.����ָ��;

            System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
            msg.Append(BoxBN);
            msg.Append(", Type:"); msg.Append(iType.ToString());
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

            //ip ����
            if (BoxBN.Length != 7)
                return false;

            int headerlen = SetHeader(BoxBN, iType, SendBuf);
            int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
            fixed (byte* psbuf = SendBuf)
            {
                byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
				tmpbuf[0] = (byte)(type);
            }//fixed

            if (!bFront)
                SendBuf[headerlen + 7] = (byte)1;

            bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

            return bSend;
        }
        #endregion

		#region ָ��������ͬ��
		/// <summary>
		/// ׼��ͬ��ָ����
		/// </summary>
		/// <param name="BoxBN"></param>
		/// <param name="type">1:׼������ָ�������ݣ�2�����ݷ�����ϣ�3���������ݴ���</param>
		/// <param name="UserId"></param>
		/// <param name="buf"></param>
		/// <returns></returns>
		public static unsafe bool CmdUpdateZhiJingMai(string BoxBN, int type, int UserId, byte[] buf, int buflen)
		{
			byte[] SendBuf = new byte[512];
			SendDataType iType = SendDataType.׼��ָ��������;

			System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
			msg.Append(BoxBN);
			msg.Append(", Type:"); msg.Append(iType.ToString());
			msg.Append(", type:"); msg.Append(type.ToString());
			msg.Append(", UserId:"); msg.Append(UserId.ToString());
			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

			//ip ����
			if (BoxBN.Length != 7)
				return false;

			long crc = 0;
			for (int i = 0; i < buflen; i++)
			{
				crc += buf[i];
			}

			int headerlen = SetHeader(BoxBN, iType, SendBuf);
			int datalen = 8;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
			fixed (byte* psbuf = SendBuf)
			{
				byte* tmpbuf = psbuf + headerlen;	//���ݿ�ʼ�ĵط�
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

			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

			return bSend;
		}
		/// <summary>
		/// ͬ��ָ��������
		/// </summary>
		/// <returns></returns>
		public static unsafe bool CmdTransZhiJingMai(string BoxBN, byte[] buf, int buflen)
		{
			byte[] SendBuf = new byte[8192];
			SendDataType iType = SendDataType.ָ�������ݴ���;

			System.Text.StringBuilder msg = new System.Text.StringBuilder("��������, BN:");
			msg.Append(BoxBN);
			msg.Append(", Type:"); msg.Append(iType.ToString());
			msg.Append(", DataLen:"); msg.Append(buf.Length.ToString());
			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg.ToString());

			//ip ����
			if (BoxBN.Length != 7)
				return false;

			int headerlen = SetHeader(BoxBN, iType, SendBuf);
			int datalen = buflen;    //Ĭ�ϣ����������8���ֽ����ݣ�����������ʾ����
			Array.Copy(buf, 0, SendBuf, headerlen, buflen);

			bool bSend = SendDataToBox((int)(datalen + headerlen), SendBuf);

			LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "�����������, �Ƿ�ɹ�:" + bSend.ToString());

			return bSend;
		}
		#endregion



        #region ���תȫ��
        private static string BanQuan(string source)
        {
            string sRet = "";
            for (int i = 0; i < source.Length; i++)
            {
                switch (source[i])
                {
                    #region ����
                    case '0':
                        sRet += "��";
                        break;

                    case '1':
                        sRet += "һ";
                        break;

                    case '2':
                        sRet += "��";
                        break;

                    case '3':
                        sRet += "��";
                        break;

                    case '4':
                        sRet += "��";
                        break;

                    case '5':
                        sRet += "��";
                        break;

                    case '6':
                        sRet += "��";
                        break;

                    case '7':
                        sRet += "��";
                        break;

                    case '8':
                        sRet += "��";
                        break;

                    case '9':
                        sRet += "��";
                        break;
                    #endregion

                    #region Сд��ĸ
                    case 'a':
                        sRet += "��";
                        break;

                    case 'b':
                        sRet += "��";
                        break;

                    case 'c':
                        sRet += "��";
                        break;

                    case 'd':
                        sRet += "��";
                        break;

                    case 'e':
                        sRet += "��";
                        break;

                    case 'f':
                        sRet += "��";
                        break;

                    case 'g':
                        sRet += "��";
                        break;

                    case 'h':
                        sRet += "��";
                        break;

                    case 'i':
                        sRet += "��";
                        break;

                    case 'j':
                        sRet += "��";
                        break;

                    case 'k':
                        sRet += "��";
                        break;

                    case 'l':
                        sRet += "��";
                        break;

                    case 'm':
                        sRet += "��";
                        break;

                    case 'n':
                        sRet += "��";
                        break;

                    case 'o':
                        sRet += "��";
                        break;

                    case 'p':
                        sRet += "��";
                        break;

                    case 'q':
                        sRet += "��";
                        break;

                    case 'r':
                        sRet += "��";
                        break;

                    case 's':
                        sRet += "��";
                        break;

                    case 't':
                        sRet += "��";
                        break;

                    case 'u':
                        sRet += "��";
                        break;

                    case 'v':
                        sRet += "��";
                        break;

                    case 'w':
                        sRet += "��";
                        break;

                    case 'x':
                        sRet += "��";
                        break;

                    case 'y':
                        sRet += "��";
                        break;

                    case 'z':
                        sRet += "��";
                        break;
                    #endregion

                    #region ��д��ĸ
                    case 'A':
                        sRet += "��";
                        break;

                    case 'B':
                        sRet += "��";
                        break;

                    case 'C':
                        sRet += "��";
                        break;

                    case 'D':
                        sRet += "��";
                        break;

                    case 'E':
                        sRet += "��";
                        break;

                    case 'F':
                        sRet += "��";
                        break;

                    case 'G':
                        sRet += "��";
                        break;

                    case 'H':
                        sRet += "��";
                        break;

                    case 'I':
                        sRet += "��";
                        break;

                    case 'J':
                        sRet += "��";
                        break;

                    case 'K':
                        sRet += "��";
                        break;

                    case 'L':
                        sRet += "��";
                        break;

                    case 'M':
                        sRet += "��";
                        break;

                    case 'N':
                        sRet += "��";
                        break;

                    case 'O':
                        sRet += "��";
                        break;

                    case 'P':
                        sRet += "��";
                        break;

                    case 'Q':
                        sRet += "��";
                        break;

                    case 'R':
                        sRet += "��";
                        break;

                    case 'S':
                        sRet += "��";
                        break;

                    case 'T':
                        sRet += "��";
                        break;

                    case 'U':
                        sRet += "��";
                        break;

                    case 'V':
                        sRet += "��";
                        break;

                    case 'W':
                        sRet += "��";
                        break;

                    case 'X':
                        sRet += "��";
                        break;

                    case 'Y':
                        sRet += "��";
                        break;

                    case 'Z':
                        sRet += "��";
                        break;
                    #endregion

                    #region ����
                    case ' ':
                        sRet += "��";
                        break;

                    case ',':
                        sRet += "��";
                        break;

                    case '.':
                        sRet += "��";
                        break;

                    case ';':
                        sRet += "��";
                        break;

                    case ':':
                        sRet += "��";
                        break;

                    case '(':
                        sRet += "��";
                        break;

                    case ')':
                        sRet += "��";
                        break;

                    case '\'':
                        sRet += "��";
                        break;

                    case '"':
                        sRet += "��";
                        break;

                    case '\\':
                        sRet += "��";
                        break;

                    case '/':
                        sRet += "��";
                        break;

                    case '-':
                        sRet += "��";
                        break;

                    case '_':
                        sRet += "��";
                        break;

                    //case '\r':
                    //    sRet += "��";
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

        #region ��������
        private static bool SendDataToBox(int len, byte[] buf)
        {
            return ListenBox.Listen.SendData(len, buf); ;
        }
        #endregion

		#endregion

	}
}