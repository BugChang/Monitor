using System;
using System.Threading;

namespace ListenBox
{
	/// <summary>
	/// ThreadPool ��ժҪ˵����
	/// </summary>
	public class ThreadPool
	{
		/// <summary>
		/// ������
		/// </summary>
		private class DataQuery
		{
			/// <summary>
			/// �ص�����
			/// </summary>
			public WaitCallback wCall;
			/// <summary>
			/// ����
			/// </summary>
			public object obj;

			public DataQuery(WaitCallback tWCall, object tobj)
			{
				wCall = tWCall;
				obj = tobj;
			}
		}

		private static System.Threading.Thread[] ThreadHandle = new System.Threading.Thread[LogInfo.Constant.ThreadCount];
		private static System.Threading.AutoResetEvent AEvent = null;

		//���ݶ���
        private static System.Collections.Generic.Queue<DataQuery> Query = new System.Collections.Generic.Queue<DataQuery>(32);

        private static bool bRun = false;

		public static bool Start()
		{
			try
			{
				AEvent = new System.Threading.AutoResetEvent(false);
                bRun = true;
				for(int i=0;i<ThreadHandle.Length;i++)
				{
					ThreadHandle[i] = new System.Threading.Thread( new System.Threading.ThreadStart(ThreadCaller) );
					ThreadHandle[i].Name = (i+1).ToString();
					ThreadHandle[i].Start();
				}
			}
			catch(Exception ee)
			{
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, ee.ToString());
				Stop();
				return false;
			}
			return true;
		}

		public static void Stop()
		{
            bRun = false;
            for (int i = 0; i < ThreadHandle.Length; i++)
            {
                AEvent.Set();
                Thread.Sleep(10);
            }
            for (int i = 0; i < ThreadHandle.Length; i++)
			{
				try
				{
                    if (ThreadHandle[i].ThreadState!=ThreadState.Stopped)
    					ThreadHandle[i].Abort();
				}
				catch
				{
				}
			}
			if (AEvent!=null)
				AEvent.Close();
		}

		public static bool QueueUserWorkItem(WaitCallback tWCall, object tobj)
		{
			DataQuery t = new DataQuery(tWCall, tobj);

			//�Ž�����
            lock (Query)
			{
                Query.Enqueue(t);
			}
			return AEvent.Set();
		}

		private static void ThreadCaller()
		{
			DataQuery t = null;

			while (bRun)
			{
				try
				{
					if(Query.Count<=0)
						AEvent.WaitOne();

					//ȡ������
                    lock (Query)
					{
                        t = Query.Dequeue();
					}

					if(t!=null)
					{
						t.wCall(t.obj);
						t = null;
					}
				}
				catch
				{
					//LogInfo.Log.WriteInfo(ee.ToString());
				}
			}
		}

	}
}
