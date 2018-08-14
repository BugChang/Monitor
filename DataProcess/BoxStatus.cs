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
	/// BoxStatus 的摘要说明。
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
            /// 逻辑箱号
            /// </summary>
            public int BoxNO;
            /// <summary>
            /// 是否正面
            /// </summary>
            public bool IsFront;
            /// <summary>
            /// 箱子BN号
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

            #region 急件灯和已取灯状态
            /// <summary>
            /// 是否亮，true：亮
            /// </summary>
            public bool LampJiJian
            {
                get { return m_LampJiJian; }
                set {
                    if (m_LampJiJian == value)
                        return;
                    m_LampJiJian = value;
                    //发送到后台去
                    SendBoxStatus();
                }
            }

            /// <summary>
            /// 是否亮，true：亮
            /// </summary>
            public bool LampYiQu
            {
                get { return m_LampYiQu; }
                set
                {
                    if (m_LampYiQu == value)
                        return;
                    m_LampYiQu = value;
                    //发送到后台去
                    SendBoxStatus();
                }
            }
            #endregion

            #region 门锁
            /// <summary>
            /// 门状态
            /// </summary>
            public DoorGateStatus Door
            {
                get { return m_Door; }
                set
                {
                    if (m_Door == value)
                        return;
                    m_Door = value;
                    //发送到后台去
                    SendBoxStatus();
                }
            }

            /// <summary>
            /// 门锁错误
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

            #region 门禁错误
            /// <summary>
            /// 门禁状态
            /// </summary>
            public DoorGateStatus Gating
            {
                get { return m_Gating; }
                set
                {
                    if (m_Gating == value)
                        return;
                    m_Gating = value;
                    //发送到后台去
                    SendBoxStatus();
                }
            }

            /// <summary>
            /// 门禁错误
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

            #region 光电
            /// <summary>
            /// 前光电传感器错误
            /// </summary>
            public bool FrontGDError
            {
                get { return m_FrontGDError; }
                set
                {
                    if (m_FrontGDError == value)
                        return;
                    m_FrontGDError = value;
                    //发送到后台去
                    SendBoxStatus();
                }
            }
            /// <summary>
            /// 后光电错误。
            /// </summary>
            public bool BackGDError
            {
                get { return m_BackGDError; }
                set
                {
                    if (m_BackGDError == value)
                        return;
                    m_BackGDError = value;
                    //发送到后台去
                    SendBoxStatus();
                }
            }
            /// <summary>
            /// 箱满传感器错误
            /// </summary>
            public bool FullGDError
            {
                get { return m_FullGDError; }
                set
                {
                    if (m_FullGDError == value)
                        return;
                    m_FullGDError = value;
                    //发送到后台去
                    SendBoxStatus();
                }
            }
			/// <summary>
			/// 箱满传感器错误
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
            /// 箱空光电错误。
            /// </summary>
            public bool ClearGDError
            {
                get { return m_ClearGDError; }
                set
                {
                    if (m_ClearGDError == value)
                        return;
                    m_ClearGDError = value;
                    //发送到后台去
                    SendBoxStatus();
                }
            }
            /// <summary>
            /// 箱空光电是否遮挡。
            /// </summary>
			public bool ClearGDState
            {
				get { return m_ClearGDState; }
                set
                {
					if (m_ClearGDState == value)
                        return;
					m_ClearGDState = value;
                    //发送到后台去
                    SendBoxStatus();
                }
            }
            /// <summary>
            /// 触发光电错误。
            /// </summary>
            public bool ChufaGDError
            {
                get { return m_ChufaGDError; }
                set
                {
                    if (m_ChufaGDError == value)
                        return;
                    m_ChufaGDError = value;
                    //发送到后台去
                    SendBoxStatus();
                }
            }
            #endregion

			/// <summary>
			/// 是否箱空报警，在取件平台使用
			/// </summary>
			public bool ClearGDWarnning;

            #region 获取错误信息
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
                    Rstr += "开门锁错误。";
                else if (Door == DoorGateStatus.CloseTimeOut)
                    Rstr += "门锁关闭超时。";

                if (Gating == DoorGateStatus.OpenTimeOut)
                    Rstr += "开门禁错误。";
                else if (Gating == DoorGateStatus.CloseTimeOut)
                    Rstr += "关门禁错误。";

                if (FrontGDError)
                    Rstr += "前光电错误。";
                if (BackGDError)
                    Rstr += "箱格已满或后光电错误。";
                if (FullGDError)
                    Rstr += "箱满光电错误。";
				if (FullGDZheDang)
					Rstr += "箱格已满，请取件";
				if (ClearGDError)
                    Rstr += "箱空光电错误。";
                if (ChufaGDError)
                    Rstr += "触发光电错误。";

                return Rstr;
            }
            #endregion

            #region 能否接收信件
            public bool CanGetLetter()
            {
				if (GateError || FrontGDError || BackGDError || FullGDError || m_FullGDZheDang || !Connected)
                    return false;
                else
                    return true;
            }
            #endregion

            #region  连接状态
            private bool m_connected;		//连接状态
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

						//这个时候根据m_IsCloseScreen进行调整屏幕
						BoxDataParse.DataParse.CmdScreen(m_BoxBN, !m_IsCloseScreen);
					}
				}
            }
            #endregion

            #region 报刊状态
            private bool m_IsNewspaperDistribute;
            /// <summary>
            /// 当前箱格的报刊分发状态
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

            #region 发送数据到数据库
            /// <summary>
            /// 发送数据到数据库
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

			#region 休眠显示屏
			public bool IsCloseScreen
			{
				get { return m_IsCloseScreen; }
				set
				{
					if (!value)
					{
						//打开屏幕，这个时候要设置时间
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
			#region 关闭显示屏幕
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
                //初始化为 未连接
                m_connected = false;
                dt_connect = LogInfo.Win32API.GetTickCount();

                m_IsNewspaperDistribute = false;
				m_IsCloseScreen = false;
				dt_LastOperation = LogInfo.Win32API.GetTickCount();

				m_ClearGDState = false;
            }
        }

        /// <summary>
        /// 逻辑箱号
        /// </summary>
        public int BoxNO
        {
            get { return FrontBox.BoxNO; }
            set { FrontBox.BoxNO = BackBox.BoxNO = value; }
        }

        /// <summary>
        /// 是否同步出错
        /// </summary>
        public bool bSyncError;
		public string GetSyncErrorMsg
		{
			get
			{
				string msg = LogInfo.NotifyMsg.GetText(51);
				if (msg == "")
					msg = "网络断开。";
				return msg;
			}
		}

		/// <summary>
        /// 是否有两个门锁
        /// </summary>
        public bool HasTwoLock;

        /// <summary>
        /// 箱子前面
        /// </summary>
        public BoxStatus FrontBox;
        /// <summary>
        /// 箱子后面BN号
        /// </summary>
        public BoxStatus BackBox;

        /// <summary>
        /// 是否亮，true：亮
        /// </summary>
        public bool LampJiJian
        {
            get { return FrontBox.LampJiJian; }
            set { FrontBox.LampJiJian = BackBox.LampJiJian = value; }
        }
        /// <summary>
        /// 是否亮，true：亮
        /// </summary>
        public bool LampYiQu
        {
            get { return FrontBox.LampYiQu; }
            set { FrontBox.LampYiQu = BackBox.LampYiQu = value; }
        }

        /// <summary>
        /// 所属单位名称
        /// </summary>
        public string BoxUnitName;
        /// <summary>
        /// 所属单位名称全称
        /// </summary>
        public string BoxUnitFullName;
        /// <summary>
        /// 是否清退箱格
        /// </summary>
        public bool IsQingTuiXiang;

        /// <summary>
        /// 箱格实际投件数量
        /// </summary>
        public int LetterCount_XiangTou;

		public int m_LetterCount;
		/// <summary>
        /// 信件数量
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
        /// 领导出差信息
        /// </summary>
        public string LeaderOutMessage;

		/// <summary>
		/// 箱格的属性，是否可以投箱等。
		/// 高法AB箱格开始。
		/// </summary>
		public LogInfo.EnumBoxStat BoxProperty;

		#region 初始化
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

			BoxProperty = LogInfo.EnumBoxStat.无;
		}
		#endregion

        #region 初始屏幕信息
        public bool InitBoxStatus()
        {
			if (!string.IsNullOrEmpty(FrontBox.BoxBN))
			{
				BoxDataParse.DataParse.CmdBoxLED(FrontBox.BoxBN, 0, this.BoxNO, LogInfo.enum_LedColor.绿色);
				if (BoxUnitName != "")
				{
					BoxDataParse.DataParse.CmdLCDText(FrontBox.BoxBN, 0, LogInfo.enum_TextType.显示附带的文本, 0, 255, BoxUnitName);
					BoxDataParse.DataParse.CmdLCDText(FrontBox.BoxBN, 2, LogInfo.enum_TextType.显示附带的文本, 0, 255, BoxUnitFullName);
					BoxDataParse.DataParse.CmdBoxLampDaiTou(FrontBox.BoxBN, LogInfo.enum_LampStatus.亮);
				}
			}
			if (!string.IsNullOrEmpty(BackBox.BoxBN))
			{
				BoxDataParse.DataParse.CmdBoxLED(BackBox.BoxBN, 0, this.BoxNO, LogInfo.enum_LedColor.绿色);
				if (BoxUnitName != "")
				{
					BoxDataParse.DataParse.CmdLCDText(BackBox.BoxBN, 0, LogInfo.enum_TextType.显示附带的文本, 0, 255, BoxUnitName);
					BoxDataParse.DataParse.CmdLCDText(BackBox.BoxBN, 2, LogInfo.enum_TextType.显示附带的文本, 0, 255, BoxUnitFullName);
					BoxDataParse.DataParse.CmdBoxLampDaiTou(BackBox.BoxBN, LogInfo.enum_LampStatus.亮);
				}
			}
			CheckBoxLetter();

            return true;
        }
        #endregion

        #region 核对本箱格的数量
		public bool CheckBoxLetter()
		{
			if (!FrontBox.IsNewspaperDistribute)
			{
				BoxDataParse.DataParse.CmdBoxLampJiJian(FrontBox.BoxBN, LampJiJian ? LogInfo.enum_LampStatus.亮 : LogInfo.enum_LampStatus.灭);
				BoxDataParse.DataParse.CmdBoxLampYiQu(FrontBox.BoxBN, LampYiQu ? LogInfo.enum_LampStatus.亮 : LogInfo.enum_LampStatus.灭);
				BoxDataParse.DataParse.CmdBoxLetterSurplus(FrontBox.BoxBN, LetterCount < 0 ? 0 : LetterCount);
				BoxDataParse.DataParse.CmdBoxLED(FrontBox.BoxBN, 1, LetterCount < 0 ? 0 : LetterCount, LogInfo.enum_LedColor.绿色);

				if (FrontBox.GetErrorState())
					BoxDataParse.DataParse.CmdBoxLampCuoWu(FrontBox.BoxBN, LogInfo.enum_LampStatus.亮);
				else
					BoxDataParse.DataParse.CmdBoxLampCuoWu(FrontBox.BoxBN, LogInfo.enum_LampStatus.灭);
			}
			if (!BackBox.IsNewspaperDistribute && !string.IsNullOrEmpty(BackBox.BoxBN))
			{
				BoxDataParse.DataParse.CmdBoxLampJiJian(BackBox.BoxBN, LampJiJian ? LogInfo.enum_LampStatus.亮 : LogInfo.enum_LampStatus.灭);
				BoxDataParse.DataParse.CmdBoxLampYiQu(BackBox.BoxBN, LampYiQu ? LogInfo.enum_LampStatus.亮 : LogInfo.enum_LampStatus.灭);
				BoxDataParse.DataParse.CmdBoxLetterSurplus(BackBox.BoxBN, LetterCount < 0 ? 0 : LetterCount);
				BoxDataParse.DataParse.CmdBoxLED(BackBox.BoxBN, 1, LetterCount < 0 ? 0 : LetterCount, LogInfo.enum_LedColor.绿色);

				if (BackBox.GetErrorState())
					BoxDataParse.DataParse.CmdBoxLampCuoWu(BackBox.BoxBN, LogInfo.enum_LampStatus.亮);
				else
					BoxDataParse.DataParse.CmdBoxLampCuoWu(BackBox.BoxBN, LogInfo.enum_LampStatus.灭);
			}
			return true;
		}
        #endregion

		#region 根据箱格属性，判断本箱格能否投箱
		/// <summary>
		/// 根据箱格属性，判断本箱格能否投箱
		/// </summary>
		/// <returns></returns>
		public bool CheckBoxProperty()
		{
			return true;
			bool b = false;
			switch (BoxProperty)
			{
				case LogInfo.EnumBoxStat.无:
					b = true;
					break;

				case LogInfo.EnumBoxStat.单日投:
					if (DateTime.Now.Day % 2 == 1)
					{
						b = true;
					}
					break;

				case LogInfo.EnumBoxStat.双日投:
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
