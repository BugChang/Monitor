using System;

namespace DataProcess
{
    public enum DoorGateStatus
    {
        Opened=0,
        Closed=1,
        OpenTimeOut=2,
        CloseTimeOut=4
    }

	/// <summary>
	/// BoxStatus ��ժҪ˵����
	/// </summary>
	public class BoxInfo
	{
        public class BoxStatus
        {
            private string m_BoxBN;
            private bool m_LampJiJian;
            private bool m_LampYiQu;
            private DoorGateStatus m_Door;
            private DoorGateStatus m_Gating;

            private bool m_FrontGDError;
            private bool m_BackGDError;
            private bool m_FullGDError;
            private bool m_ClearGDError;
            private bool m_ChufaGDError;

			public bool m_FullGDZheDang;

			private bool m_ClearGDState;

			private UInt32 dt_LastOperation;
			private bool m_IsCloseScreen;

            /// <summary>
            /// �߼����
            /// </summary>
            public int BoxNO;
            /// <summary>
            /// �Ƿ�����
            /// </summary>
            public bool IsFront;
            /// <summary>
            /// ����BN��
            /// </summary>
            public string BoxBN
            {
                get { return m_BoxBN;}
                set
                {
                    if (value != null) m_BoxBN = value;
                    else m_BoxBN = "";
                }
            }

            #region �����ƺ���ȡ��״̬
            /// <summary>
            /// �Ƿ�����true����
            /// </summary>
            public bool LampJiJian
            {
                get { return m_LampJiJian; }
                set {
                    if (m_LampJiJian == value)
                        return;
                    m_LampJiJian = value;
                    //���͵���̨ȥ
                    SendBoxStatus();
                }
            }

            /// <summary>
            /// �Ƿ�����true����
            /// </summary>
            public bool LampYiQu
            {
                get { return m_LampYiQu; }
                set
                {
                    if (m_LampYiQu == value)
                        return;
                    m_LampYiQu = value;
                    //���͵���̨ȥ
                    SendBoxStatus();
                }
            }
            #endregion

            #region ����
            /// <summary>
            /// ��״̬
            /// </summary>
            public DoorGateStatus Door
            {
                get { return m_Door; }
                set
                {
                    if (m_Door == value)
                        return;
                    m_Door = value;
                    //���͵���̨ȥ
                    SendBoxStatus();
                }
            }

            /// <summary>
            /// ��������
            /// </summary>
            public bool DoorError
            {
                get
                {
                    if (Door == DoorGateStatus.OpenTimeOut || Door == DoorGateStatus.CloseTimeOut)
                        return true;
                    else
                        return false;
                }
            }
            #endregion

            #region �Ž�����
            /// <summary>
            /// �Ž�״̬
            /// </summary>
            public DoorGateStatus Gating
            {
                get { return m_Gating; }
                set
                {
                    if (m_Gating == value)
                        return;
                    m_Gating = value;
                    //���͵���̨ȥ
                    SendBoxStatus();
                }
            }

            /// <summary>
            /// �Ž�����
            /// </summary>
            public bool GateError
            {
                get
                {
                    if (Gating == DoorGateStatus.CloseTimeOut || Gating == DoorGateStatus.OpenTimeOut)
                        return true;
                    else
                        return false;
                }
            }
            #endregion

            #region ���
            /// <summary>
            /// ǰ��紫��������
            /// </summary>
            public bool FrontGDError
            {
                get { return m_FrontGDError; }
                set
                {
                    if (m_FrontGDError == value)
                        return;
                    m_FrontGDError = value;
                    //���͵���̨ȥ
                    SendBoxStatus();
                }
            }
            /// <summary>
            /// �������
            /// </summary>
            public bool BackGDError
            {
                get { return m_BackGDError; }
                set
                {
                    if (m_BackGDError == value)
                        return;
                    m_BackGDError = value;
                    //���͵���̨ȥ
                    SendBoxStatus();
                }
            }
            /// <summary>
            /// ��������������
            /// </summary>
            public bool FullGDError
            {
                get { return m_FullGDError; }
                set
                {
                    if (m_FullGDError == value)
                        return;
                    m_FullGDError = value;
                    //���͵���̨ȥ
                    SendBoxStatus();
                }
            }
			/// <summary>
			/// ��������������
			/// </summary>
			public bool FullGDZheDang
			{
				get { return m_FullGDZheDang; }
				set
				{
					if (m_FullGDZheDang == value)
						return;
					m_FullGDZheDang = value;
				}
			}
            /// <summary>
            /// ��չ�����
            /// </summary>
            public bool ClearGDError
            {
                get { return m_ClearGDError; }
                set
                {
                    if (m_ClearGDError == value)
                        return;
                    m_ClearGDError = value;
                    //���͵���̨ȥ
                    SendBoxStatus();
                }
            }
            /// <summary>
            /// ��չ���Ƿ��ڵ���
            /// </summary>
			public bool ClearGDState
            {
				get { return m_ClearGDState; }
                set
                {
					if (m_ClearGDState == value)
                        return;
					m_ClearGDState = value;
                    //���͵���̨ȥ
                    SendBoxStatus();
                }
            }
            /// <summary>
            /// ����������
            /// </summary>
            public bool ChufaGDError
            {
                get { return m_ChufaGDError; }
                set
                {
                    if (m_ChufaGDError == value)
                        return;
                    m_ChufaGDError = value;
                    //���͵���̨ȥ
                    SendBoxStatus();
                }
            }
            #endregion

			/// <summary>
			/// �Ƿ���ձ�������ȡ��ƽ̨ʹ��
			/// </summary>
			public bool ClearGDWarnning;

            #region ��ȡ������Ϣ
            public bool GetErrorState()
            {
                if (Door == DoorGateStatus.OpenTimeOut)
                    return true;
                else if (Door == DoorGateStatus.CloseTimeOut)
                    return true;

                if (Gating == DoorGateStatus.OpenTimeOut)
                    return true;
                else if (Gating == DoorGateStatus.CloseTimeOut)
                    return true;

                if (FrontGDError)
                    return true;
                if (BackGDError)
                    return true;
                if (FullGDError)
                    return true;
                if (ClearGDError)
                    return true;
                if (ChufaGDError)
                    return true;

                return false;
            }
            public string GetErrorString()
            {
                string Rstr = "";

                if (Door == DoorGateStatus.OpenTimeOut)
                    Rstr += "����������";
                else if (Door == DoorGateStatus.CloseTimeOut)
                    Rstr += "�����رճ�ʱ��";

                if (Gating == DoorGateStatus.OpenTimeOut)
                    Rstr += "���Ž�����";
                else if (Gating == DoorGateStatus.CloseTimeOut)
                    Rstr += "���Ž�����";

                if (FrontGDError)
                    Rstr += "ǰ������";
                if (BackGDError)
                    Rstr += "���������������";
                if (FullGDError)
                    Rstr += "����������";
				if (FullGDZheDang)
					Rstr += "�����������ȡ��";
				if (ClearGDError)
                    Rstr += "��չ�����";
                if (ChufaGDError)
                    Rstr += "����������";

                return Rstr;
            }
            #endregion

            #region �ܷ�����ż�
            public bool CanGetLetter()
            {
				if (GateError || FrontGDError || BackGDError || FullGDError || m_FullGDZheDang || !Connected)
                    return false;
                else
                    return true;
            }
            #endregion

            #region  ����״̬
            private bool m_connected;		//����״̬
            private UInt32 dt_connect;

            public bool Connected
            {
                get
                {
                    if (m_connected)
                    {
                        UInt32 ts = LogInfo.Win32API.GetTickCount() - this.dt_connect;
                        ts /= 1000;
                        if (ts >= LogInfo.Constant.ConnectTimeOut)
                            m_connected = false;
                    }
                    return this.m_connected;
                }
				set
				{
					dt_connect = LogInfo.Win32API.GetTickCount();
					if (this.m_connected != value)
					{
						this.m_connected = value;

						//���ʱ�����m_IsCloseScreen���е�����Ļ
						BoxDataParse.DataParse.CmdScreen(m_BoxBN, !m_IsCloseScreen);
					}
				}
            }
            #endregion

            #region ����״̬
            private bool m_IsNewspaperDistribute;
            /// <summary>
            /// ��ǰ���ı����ַ�״̬
            /// </summary>
            public bool IsNewspaperDistribute
            {
                get { return m_IsNewspaperDistribute; }
                set
                {
                    m_IsNewspaperDistribute = value;
                }
            }
            #endregion

            #region �������ݵ����ݿ�
            /// <summary>
            /// �������ݵ����ݿ�
            /// </summary>
            private void SendBoxStatus()
            {
                DataBase.MonitorService.BoxStatus box = new DataBase.MonitorService.BoxStatus();
                box.BoxNo = BoxNO;
                box.IsFront = IsFront;
                box.LampJiJian = m_LampJiJian;
                box.LampYiQu = m_LampYiQu;
                box.LampCuoWu = GetErrorState();
                box.DoorStatus = (int)Door;
                box.GateStatus = (int)Gating;
                box.FrontGD = FrontGDError;
                box.BackGD = BackGDError;
                box.FullGD = FullGDError;
                box.ClearGD = ClearGDError;
                box.ChufaGD = ChufaGDError;
                DataBase.DataSave.SetBoxStatus(box);
            }
            #endregion

			#region ������ʾ��
			public bool IsCloseScreen
			{
				get { return m_IsCloseScreen; }
				set
				{
					if (!value)
					{
						//����Ļ�����ʱ��Ҫ����ʱ��
						dt_LastOperation = LogInfo.Win32API.GetTickCount();
					}
					if (m_IsCloseScreen == value)
						return;

					m_IsCloseScreen = value;
					if (Connected)
						BoxDataParse.DataParse.CmdScreen(m_BoxBN, !value);
				}
			}
			#endregion
			#region �ر���ʾ��Ļ
			public void CloseScreen()
			{
				if (string.IsNullOrEmpty(BoxBN))
					return;
				UInt32 dt = LogInfo.Win32API.GetTickCount();
				dt = (dt-dt_LastOperation) / 1000;
				if (dt > LogInfo.Constant.TimeOut_CloseScreen && LogInfo.Constant.TimeOut_CloseScreen > 0)
				{
					if (!IsNewspaperDistribute)
					{
						IsCloseScreen = true;
						dt_LastOperation = dt;
					}
				}
			}
			#endregion

			public BoxStatus()
            {
                BoxBN = "";
                Door = DoorGateStatus.Closed;
                Gating = DoorGateStatus.Closed;
                //��ʼ��Ϊ δ����
                m_connected = false;
                dt_connect = LogInfo.Win32API.GetTickCount();

                m_IsNewspaperDistribute = false;
				m_IsCloseScreen = false;
				dt_LastOperation = LogInfo.Win32API.GetTickCount();

				m_ClearGDState = false;
            }
        }

        /// <summary>
        /// �߼����
        /// </summary>
        public int BoxNO
        {
            get { return FrontBox.BoxNO; }
            set { FrontBox.BoxNO = BackBox.BoxNO = value; }
        }

        /// <summary>
        /// �Ƿ�ͬ������
        /// </summary>
        public bool bSyncError;
		public string GetSyncErrorMsg
		{
			get
			{
				string msg = LogInfo.NotifyMsg.GetText(51);
				if (msg == "")
					msg = "����Ͽ���";
				return msg;
			}
		}

		/// <summary>
        /// �Ƿ�����������
        /// </summary>
        public bool HasTwoLock;

        /// <summary>
        /// ����ǰ��
        /// </summary>
        public BoxStatus FrontBox;
        /// <summary>
        /// ���Ӻ���BN��
        /// </summary>
        public BoxStatus BackBox;

        /// <summary>
        /// �Ƿ�����true����
        /// </summary>
        public bool LampJiJian
        {
            get { return FrontBox.LampJiJian; }
            set { FrontBox.LampJiJian = BackBox.LampJiJian = value; }
        }
        /// <summary>
        /// �Ƿ�����true����
        /// </summary>
        public bool LampYiQu
        {
            get { return FrontBox.LampYiQu; }
            set { FrontBox.LampYiQu = BackBox.LampYiQu = value; }
        }

        /// <summary>
        /// ������λ����
        /// </summary>
        public string BoxUnitName;
        /// <summary>
        /// ������λ����ȫ��
        /// </summary>
        public string BoxUnitFullName;
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool IsQingTuiXiang;

        /// <summary>
        /// ���ʵ��Ͷ������
        /// </summary>
        public int LetterCount_XiangTou;

		public int m_LetterCount;
		/// <summary>
        /// �ż�����
		/// </summary>
		public int LetterCount
		{
			get
			{
				return m_LetterCount > 0 ? m_LetterCount : 0;
			}
			set
			{
				m_LetterCount = value;
			}
		}


        /// <summary>
        /// �쵼������Ϣ
        /// </summary>
        public string LeaderOutMessage;

		/// <summary>
		/// �������ԣ��Ƿ����Ͷ��ȡ�
		/// �߷�AB���ʼ��
		/// </summary>
		public LogInfo.EnumBoxStat BoxProperty;

		#region ��ʼ��
		public BoxInfo()
		{
			bSyncError = false;
            FrontBox = new BoxStatus();
            BackBox = new BoxStatus();
            FrontBox.IsFront = true;
            BackBox.IsFront = false;

            LampJiJian = false;
            LampYiQu = false;

            BoxUnitName = "";
            BoxUnitFullName = "";

            LetterCount_XiangTou = 0;
			LetterCount = -1;

            LeaderOutMessage = "";

			BoxProperty = LogInfo.EnumBoxStat.��;
		}
		#endregion

        #region ��ʼ��Ļ��Ϣ
        public bool InitBoxStatus()
        {
			if (!string.IsNullOrEmpty(FrontBox.BoxBN))
			{
				BoxDataParse.DataParse.CmdBoxLED(FrontBox.BoxBN, 0, this.BoxNO, LogInfo.enum_LedColor.��ɫ);
				if (BoxUnitName != "")
				{
					BoxDataParse.DataParse.CmdLCDText(FrontBox.BoxBN, 0, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, BoxUnitName);
					BoxDataParse.DataParse.CmdLCDText(FrontBox.BoxBN, 2, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, BoxUnitFullName);
					BoxDataParse.DataParse.CmdBoxLampDaiTou(FrontBox.BoxBN, LogInfo.enum_LampStatus.��);
				}
			}
			if (!string.IsNullOrEmpty(BackBox.BoxBN))
			{
				BoxDataParse.DataParse.CmdBoxLED(BackBox.BoxBN, 0, this.BoxNO, LogInfo.enum_LedColor.��ɫ);
				if (BoxUnitName != "")
				{
					BoxDataParse.DataParse.CmdLCDText(BackBox.BoxBN, 0, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, BoxUnitName);
					BoxDataParse.DataParse.CmdLCDText(BackBox.BoxBN, 2, LogInfo.enum_TextType.��ʾ�������ı�, 0, 255, BoxUnitFullName);
					BoxDataParse.DataParse.CmdBoxLampDaiTou(BackBox.BoxBN, LogInfo.enum_LampStatus.��);
				}
			}
			CheckBoxLetter();

            return true;
        }
        #endregion

        #region �˶Ա���������
		public bool CheckBoxLetter()
		{
			if (!FrontBox.IsNewspaperDistribute)
			{
				BoxDataParse.DataParse.CmdBoxLampJiJian(FrontBox.BoxBN, LampJiJian ? LogInfo.enum_LampStatus.�� : LogInfo.enum_LampStatus.��);
				BoxDataParse.DataParse.CmdBoxLampYiQu(FrontBox.BoxBN, LampYiQu ? LogInfo.enum_LampStatus.�� : LogInfo.enum_LampStatus.��);
				BoxDataParse.DataParse.CmdBoxLetterSurplus(FrontBox.BoxBN, LetterCount < 0 ? 0 : LetterCount);
				BoxDataParse.DataParse.CmdBoxLED(FrontBox.BoxBN, 1, LetterCount < 0 ? 0 : LetterCount, LogInfo.enum_LedColor.��ɫ);

				if (FrontBox.GetErrorState())
					BoxDataParse.DataParse.CmdBoxLampCuoWu(FrontBox.BoxBN, LogInfo.enum_LampStatus.��);
				else
					BoxDataParse.DataParse.CmdBoxLampCuoWu(FrontBox.BoxBN, LogInfo.enum_LampStatus.��);
			}
			if (!BackBox.IsNewspaperDistribute && !string.IsNullOrEmpty(BackBox.BoxBN))
			{
				BoxDataParse.DataParse.CmdBoxLampJiJian(BackBox.BoxBN, LampJiJian ? LogInfo.enum_LampStatus.�� : LogInfo.enum_LampStatus.��);
				BoxDataParse.DataParse.CmdBoxLampYiQu(BackBox.BoxBN, LampYiQu ? LogInfo.enum_LampStatus.�� : LogInfo.enum_LampStatus.��);
				BoxDataParse.DataParse.CmdBoxLetterSurplus(BackBox.BoxBN, LetterCount < 0 ? 0 : LetterCount);
				BoxDataParse.DataParse.CmdBoxLED(BackBox.BoxBN, 1, LetterCount < 0 ? 0 : LetterCount, LogInfo.enum_LedColor.��ɫ);

				if (BackBox.GetErrorState())
					BoxDataParse.DataParse.CmdBoxLampCuoWu(BackBox.BoxBN, LogInfo.enum_LampStatus.��);
				else
					BoxDataParse.DataParse.CmdBoxLampCuoWu(BackBox.BoxBN, LogInfo.enum_LampStatus.��);
			}
			return true;
		}
        #endregion

		#region ����������ԣ��жϱ�����ܷ�Ͷ��
		/// <summary>
		/// ����������ԣ��жϱ�����ܷ�Ͷ��
		/// </summary>
		/// <returns></returns>
		public bool CheckBoxProperty()
		{
			return true;
			bool b = false;
			switch (BoxProperty)
			{
				case LogInfo.EnumBoxStat.��:
					b = true;
					break;

				case LogInfo.EnumBoxStat.����Ͷ:
					if (DateTime.Now.Day % 2 == 1)
					{
						b = true;
					}
					break;

				case LogInfo.EnumBoxStat.˫��Ͷ:
					if (DateTime.Now.Day % 2 == 0)
					{
						b = true;
					}
					break;
			}

			return b;
		}
		#endregion
	}
}
