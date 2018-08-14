using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ListenClient
{
    public delegate void ReceiveClientData(string xmlData, Socket s, EndPoint ep);

    /// <summary>
    /// Listen 接收到的连接。
    /// </summary>
    public class ClientListen
    {
        private static Socket myListener = null;
        private static Socket myListener_other = null;
        private static Socket myListener_news = null;
        private static System.Threading.Thread thread;
        private static System.Threading.Thread thread_other;
        private static System.Threading.Thread thread_news;

        private static ReceiveClientData rcdataHandler = null;

        private static bool bRunning = false;

        //字典类，保存连接进来的连接
        private static LogInfo.MyDictionary dict = new LogInfo.MyDictionary();
        private static LogInfo.MyDictionary dict_other = new LogInfo.MyDictionary();
        private static LogInfo.MyDictionary dict_news = new LogInfo.MyDictionary();

        #region 开始侦听客户端连接
        /// <summary>
        /// 开始侦听客户端连接
        /// </summary>
        /// <returns></returns>
        public static bool Start(ReceiveClientData tmp_rcdataHandler)
        {
            rcdataHandler = tmp_rcdataHandler;
            bRunning = true;
            bool bRet = false;

            if (myListener == null)
            {
                try
                {
                    System.Net.IPAddress localaddr = System.Net.IPAddress.Any; //设定服务器默认IP地址

                    //开始监听
                    myListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    myListener.Bind((EndPoint)(new IPEndPoint(localaddr, LogInfo.Constant.ClientListenPort)));
                    myListener.Listen(100);

                    myListener_other = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    myListener_other.Bind(new IPEndPoint(localaddr, LogInfo.Constant.ClientOtherListenPort));
                    myListener_other.Listen(10);

                    myListener_news = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    myListener_news.Bind(new IPEndPoint(localaddr, LogInfo.Constant.ClientNewsListenPort));
                    myListener_news.Listen(10);

                    thread = new Thread(StartListen);
                    thread.Start();
                    thread_other = new Thread(StartListen_Other);
                    thread_other.Start();
                    thread_news = new Thread(StartListen_News);
                    thread_news.Start();

                    LogInfo.Log.WriteInfo(LogInfo.LogType.Info, "开始监听客户端。\r\n");
                     bRet = true;
                }
                catch (Exception ee)
                {
                    LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "开始监听客户端错误：" + ee.ToString() + "\r\n");
                    MessageBox.Show("请检查网络连接状态！", "错误");
                }
            }

            if (!bRet)
                bRunning = false;
            return bRet;
        }
        #endregion

        #region 监听客户端接收数据
        /// <summary>
        ///监听客户端接收数据
        /// </summary>
        private static void StartListen()
        {
            try
            {
                while (bRunning)
                {
                    if (myListener.Poll(1000, SelectMode.SelectRead))
                    {
                        Socket mySocket = myListener.Accept();

                        if (mySocket.Connected)
                        {
                            string key = mySocket.RemoteEndPoint.ToString();
                            TcpInSocket tTcpInSocket = new TcpInSocket(1, key);
                            tTcpInSocket.OnReceive += rcdataHandler;
                            tTcpInSocket.OnClose += new d_RemoveDict(RemoveDict);
                            tTcpInSocket.mySocket = mySocket;
                            System.Threading.Thread thread1 = new Thread(new ThreadStart(tTcpInSocket.SocketReceiveData));
                            thread1.Start();

                            //加进字典
                            dict.setUsed(true);
                            try
                            {
                                if (dict.Contains(key))
                                {
                                    TcpInSocket t = (TcpInSocket)dict[key];
                                    if (t.mySocket != null)
                                        t.mySocket.Close();
                                    dict.Remove(key);
                                }
                                dict.Add(key, tTcpInSocket);
                            }
                            finally
                            {
                                dict.setUsed(false);
                            }
                        }
                    }
                    else
                        System.Threading.Thread.Sleep(50);
                }
                myListener.Close();
                myListener = null;
            }
            catch (Exception ee)
            {
                if (bRunning)
                {
                    LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "监听客户端错误：" + ee.ToString() + "\r\n");
                    bRunning = false;
                    MessageBox.Show("网络连接出错，请重新启动监控程序。如果还有错误，请检查网络连接。", "错误");
                    System.Windows.Forms.Application.Exit();
                }
            }

        }
        #endregion

        #region 监听业务客户端接收数据
        /// <summary>
        ///监听业务客户端接收数据
        /// </summary>
        private static void StartListen_Other()
        {
            try
            {
                while (bRunning)
                {
                    if (myListener_other.Poll(1000, SelectMode.SelectRead))
                    {
                        Socket mySocket = myListener_other.Accept();

                        if (mySocket.Connected)
                        {
                            string key = mySocket.RemoteEndPoint.ToString();
                            TcpInSocket tTcpInSocket = new TcpInSocket( 2, key);
                            tTcpInSocket.OnReceive += rcdataHandler;
                            tTcpInSocket.OnClose += new d_RemoveDict(RemoveDict);
                            tTcpInSocket.mySocket = mySocket;

                            //加进字典
                            dict_other.setUsed(true);
                            try
                            {
                                if (dict_other.Contains(key))
                                {
                                    TcpInSocket t = (TcpInSocket)dict_other[key];
                                    if (t.mySocket != null)
                                        t.mySocket.Close();
                                    dict_other.Remove(key);
                                }
                                dict_other.Add(key, tTcpInSocket);
                            }
                            finally
                            {
                                dict_other.setUsed(false);
                            }

                            System.Threading.Thread thread1 = new Thread(new ThreadStart(tTcpInSocket.SocketReceiveData));
                            thread1.Start();
                        }
                    }
                    else
                        System.Threading.Thread.Sleep(50);
                }
                myListener_other.Close();
                myListener_other = null;
            }
            catch (Exception ee)
            {
                if (bRunning)
                {
                    LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "监听客户端错误：" + ee.ToString() + "\r\n");
                    MessageBox.Show("网络连接出错，请重新启动监控程序。如果还有错误，请检查网络连接。", "错误");
                }
            }

        }
        #endregion

        #region 报刊分发使用，监听客户端接收数据
        /// <summary>
        ///报刊分发使用，监听客户端接收数据
        /// </summary>
        private static void StartListen_News()
        {
            try
            {
                while (bRunning)
                {
                    if (myListener_news.Poll(1000, SelectMode.SelectRead))
                    {
                        Socket mySocket = myListener_news.Accept();

                        if (mySocket.Connected)
                        {
                            string key = mySocket.RemoteEndPoint.ToString();
                            TcpInSocket tTcpInSocket = new TcpInSocket(3, key);
                            tTcpInSocket.OnReceive += rcdataHandler;
                            tTcpInSocket.OnClose += new d_RemoveDict(RemoveDict);
                            tTcpInSocket.mySocket = mySocket;

                            //加进字典
                            dict_news.setUsed(true);
                            try
                            {
                                if (dict_news.Contains(key))
                                {
                                    TcpInSocket t = (TcpInSocket)dict_news[key];
                                    if (t.mySocket != null)
                                        t.mySocket.Close();
                                    dict_news.Remove(key);
                                }
                                dict_news.Add(key, tTcpInSocket);
                            }
                            finally
                            {
                                dict_news.setUsed(false);
                            }

                            System.Threading.Thread thread1 = new Thread(new ThreadStart(tTcpInSocket.SocketReceiveData));
                            thread1.Start();
                        }
                    }
                    else
                        System.Threading.Thread.Sleep(50);
                }
                myListener_news.Close();
                myListener_news = null;
            }
            catch (Exception ee)
            {
                if (bRunning)
                {
                    LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "监听客户端错误：" + ee.ToString() + "\r\n");
                    MessageBox.Show("网络连接出错，请重新启动监控程序。如果还有错误，请检查网络连接。", "错误");
                }
            }

        }
        #endregion

        #region 关闭线程
        /// <summary>
        /// 关闭线程
        /// </summary>
        public static void Stop()
        {
            try
            {
                bRunning = false;
                thread.Join();
                thread_other.Join();
                thread_news.Join();

                dict.setUsed(true);
                try
                {
                    foreach (string key in dict.Keys)
                    {
                        TcpInSocket t = (TcpInSocket)dict[key];
                        if (t.mySocket != null)
                            t.mySocket.Close();
                    }
                    dict.Clear();
                }
                finally
                {
                    dict.setUsed(false);
                }

                dict_other.setUsed(true);
                try
                {
                    foreach (string key in dict_other.Keys)
                    {
                        TcpInSocket t = (TcpInSocket)dict_other[key];
                        if (t.mySocket != null)
                            t.mySocket.Close();
                    }
                    dict_other.Clear();
                }
                finally
                {
                    dict_other.setUsed(false);
                }

                dict_news.setUsed(true);
                try
                {
                    foreach (string key in dict_news.Keys)
                    {
                        TcpInSocket t = (TcpInSocket)dict_news[key];
                        if (t.mySocket != null)
                            t.mySocket.Close();
                    }
                    dict_news.Clear();
                }
                finally
                {
                    dict_news.setUsed(false);
                }
            }
            catch (Exception ee)
            {
                LogInfo.Log.WriteInfo(LogInfo.LogType.Error, "结束监听客户端错误：" + ee.ToString() + "。\r\n");
            }
        }
        #endregion

        #region 发送数据
        public static void SendNewsData(string msg)
        {
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg);

            byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(msg);
            byte[] buf1 = new byte[buf.Length + 1];
            Array.Copy(buf, 0, buf1, 0, buf.Length);
            buf1[buf1.Length - 1] = 0;

            dict_news.setUsed(true);
            try
            {
                foreach (string key in dict_news.Keys)
                {
                    TcpInSocket t = (TcpInSocket)dict_news[key];
                    if (t.mySocket != null)
                        t.SendData(buf1);
                    else
                        dict_news.Remove(key);
                }
            }
            finally
            {
                dict_news.setUsed(false);
            }
        }

        public static void SendData(string msg)
        {
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg);

            byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(msg);
            byte[] buf1 = new byte[buf.Length + 1];
            Array.Copy(buf, 0, buf1, 0, buf.Length);
            buf1[buf1.Length - 1] = 0;

            dict.setUsed(true);
            try
            {
                foreach (string key in dict.Keys)
                {
                    TcpInSocket t = (TcpInSocket)dict[key];
                    if (t.mySocket != null)
                        t.SendData(buf1);
                    else
                        dict.Remove(key);
                }
            }
            finally
            {
                dict.setUsed(false);
            }
        }

        public static void SendData(string msg, Socket s)
        {
            LogInfo.Log.WriteInfo(LogInfo.LogType.Info, msg);

            try
            {
                if (s != null)
                {
                    byte[] buf = System.Text.Encoding.GetEncoding(936).GetBytes(msg);
                    byte[] buf1 = new byte[buf.Length + 1];
                    Array.Copy(buf, 0, buf1, 0, buf.Length);
                    buf1[buf1.Length - 1] = 0;
                    s.Send(buf1);
                }
            }
            catch
            { }
        }
        #endregion

        #region 从监听列表中移除连接
        public static void RemoveDict(string key, int type)
        {
            if (type == 1)
            {
                dict.setUsed(true);
                try
                {
                    dict.Remove(key);
                }
                finally
                {
                    dict.setUsed(false);
                }
            }
            else if (type == 2)
            {
                dict_other.setUsed(true);
                try
                {
                    dict_other.Remove(key);
                }
                finally
                {
                    dict_other.setUsed(false);
                }
            }
            else if (type == 3)
            {
                dict_news.setUsed(true);
                try
                {
                    dict_news.Remove(key);
                }
                finally
                {
                    dict_news.setUsed(false);
                }
            }
        }
        #endregion

    }
}
