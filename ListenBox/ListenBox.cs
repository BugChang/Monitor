using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ListenBox
{
	public delegate void ReceiveBoxData(byte[] data);

	/// <summary>
	/// Listen 的摘要说明。
	/// </summary>
	public class Listen
	{
		static bool	    bRun = false;

        public static event ReceiveBoxData OnReceiveBoxData = null;

		#region 开始，结束
        public static bool Start()
		{
			if(bRun)
				return true;

			//初始化线程池
			if(!ThreadPool.Start())
				return false;

			try
			{
                CanClass.CSVP.OnCanGetData += CSVP_OnCanGetData;
                bRun = CanClass.CSVP.Start(LogInfo.Constant.CanCount);
                if (!bRun)
                {
                    LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "开始监听箱子出错.");
                    ThreadPool.Stop();
                    return false;
                }
				LogInfo.Log.WriteInfo( LogInfo.LogType.Info, "开始监听箱子。\r\n" );
			}
			catch(Exception e)
			{
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "开始监听箱子出错：" + e.ToString());
				ThreadPool.Stop();
				return false;
			}

			return true;
		}

        public static void Stop()
		{
			if(bRun)
			{
				bRun = false;

                CanClass.CSVP.Stop();
                ThreadPool.Stop();
			}
		}
		#endregion

		#region 发送数据
        public static unsafe bool SendData(int len, byte[] data)
		{
			bool send = false;

            try
            {
                send = CanClass.CSVP.SendData(len, data);
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
            }

			return send;
		}
		#endregion

        static void CSVP_OnCanGetData(byte[] data)
        {
            WaitCallback wCall = new WaitCallback(CheckData);

            try
            {
                ThreadPool.QueueUserWorkItem(wCall, data);
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
            }
        }

        #region 处理状态数据数据
        // 处理收到的数据
        private static unsafe void CheckData(object b)
        {
            try
            {
                byte[] data = (byte[])b;
                if (OnReceiveBoxData != null)
                {
                    OnReceiveBoxData(data);
                }
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "\r\nListenBox.CheckData: " + ee.ToString());
            }

            return;
        }

        #endregion
	}
}
