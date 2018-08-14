using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ListenClient
{
    public delegate void d_RemoveDict(string key, int type);

    /// <summary>
    /// TcpInSocket 的摘要说明。
    /// </summary>
    public class TcpInSocket
    {
        public Socket mySocket = null;
        public event ReceiveClientData OnReceive = null;
        public event d_RemoveDict OnClose = null;
        private string xmlData;
        private System.Text.Encoding ASCII;

        private int type;
        private string key;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="itype">1:界面，2:业务，3:报刊分发</param>
        /// <param name="key"></param>
        public TcpInSocket( int itype, string key)
        {
            this.type = itype;
            this.key = key;
            ASCII = System.Text.Encoding.GetEncoding(936);
        }

        public void SocketReceiveData()
        {
            try
            {
                while (mySocket.Connected)
                {
                    //接收数据
                    byte[] recebyte = new byte[4096];
                    int read = mySocket.Receive(recebyte, recebyte.Length, 0);
                    if (read <= 0) break;

                    int j = 0;
                    for (int i = 0; i < read; i++)
                    {
                        if (recebyte[i] == 0)
                        {
                            xmlData += ASCII.GetString(recebyte, j, i - j);
                            System.Threading.Thread t1 = new Thread(new ThreadStart(this.ProcessData));
                            t1.Start();

                            //等待t1线程处理xmlData
                            while (this.xmlData != "")
                                Thread.Sleep(20);

                            j = i + 1;
                        }
                        else if (i + 1 == read)
                        {
                            xmlData += ASCII.GetString(recebyte, j, i - j + 1);
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "TcpInSocket 出错：" + ee.ToString());
            }
            finally
            {
                if (OnClose != null)
                    OnClose(key, type);
                mySocket.Close();
                if (type == 3)
                {
                    string txmlData = "<?xml version=\"1.0\" encoding=\"gb2312\"?>"
                        + "<farShine ver=\"1.0\" xmlns=\"http://www.farshine.com\">"
                        + "<farShine ver=\"1.0\"><Operation name=\"Newspaper-End-Request\">"
                        + "</Operation></farShine></farShine>";
                    this.OnReceive(txmlData, null, null);
                }
            }
        }

        private void ProcessData()
        {
            try
            {
                string txmlData = this.xmlData;
                this.xmlData = "";

                //记录监控信息
                LogInfo.Log.WriteInfo(LogInfo.LogType.Info, txmlData);

                if (this.OnReceive != null)
                    this.OnReceive(txmlData, mySocket, null);
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "TcpInSocket ProcessData: " + ee.ToString());
            }
        }

        public long SendData(byte[] msg)
        {
            int ret = 0;
            try
            {
                for (int i = 0; i < msg.Length; i += 512)
                {
                    ret += mySocket.Send(msg, i, ((msg.Length - i) > 512 ? 512 : (msg.Length - i)), SocketFlags.None);
                }
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "TcpInSocket SendData: " + ee.ToString());
            }
            return ret;
        }
    }

}
