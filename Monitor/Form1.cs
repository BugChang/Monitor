using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Monitor
{
	/// <summary>
	/// Form1 的摘要说明。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
    {
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private System.Windows.Forms.ContextMenu m_contextMenu;
		private System.Windows.Forms.MenuItem menuItem_Exit;
		private System.Windows.Forms.MenuItem menuItem_Split1;
		private System.Windows.Forms.MenuItem menuItem_Show;
		private System.Windows.Forms.MenuItem menuItem_Hide;
		private System.Windows.Forms.Label lb_state;
        private System.Windows.Forms.Label lbRunTime;


        private bool bRun;
        private bool bInit;
        private MenuItem menuItem_Split2;
        private MenuItem mi_ViewLog;
		private Label lb_Version;
		private Button button1;
		DataProcess.DataProcess dp;

        delegate void SetTextCallback(string text, Label lb);
        delegate void CloseWindowCallback();

		public Form1()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();


			//设置进程优先级
			System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.AboveNormal;
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.m_notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.m_contextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItem_Show = new System.Windows.Forms.MenuItem();
			this.menuItem_Hide = new System.Windows.Forms.MenuItem();
			this.menuItem_Split1 = new System.Windows.Forms.MenuItem();
			this.mi_ViewLog = new System.Windows.Forms.MenuItem();
			this.menuItem_Split2 = new System.Windows.Forms.MenuItem();
			this.menuItem_Exit = new System.Windows.Forms.MenuItem();
			this.lb_state = new System.Windows.Forms.Label();
			this.lbRunTime = new System.Windows.Forms.Label();
			this.lb_Version = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_notifyIcon
			// 
			this.m_notifyIcon.ContextMenu = this.m_contextMenu;
			this.m_notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("m_notifyIcon.Icon")));
			this.m_notifyIcon.Text = "监控服务";
			this.m_notifyIcon.Visible = true;
			this.m_notifyIcon.DoubleClick += new System.EventHandler(this.m_notifyIcon_DoubleClick);
			// 
			// m_contextMenu
			// 
			this.m_contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_Show,
            this.menuItem_Hide,
            this.menuItem_Split1,
            this.mi_ViewLog,
            this.menuItem_Split2,
            this.menuItem_Exit});
			// 
			// menuItem_Show
			// 
			this.menuItem_Show.Index = 0;
			this.menuItem_Show.Text = "显示主窗口";
			this.menuItem_Show.Click += new System.EventHandler(this.menuItem_Show_Click);
			// 
			// menuItem_Hide
			// 
			this.menuItem_Hide.Enabled = false;
			this.menuItem_Hide.Index = 1;
			this.menuItem_Hide.Text = "隐藏主窗口";
			this.menuItem_Hide.Click += new System.EventHandler(this.menuItem_Show_Click);
			// 
			// menuItem_Split1
			// 
			this.menuItem_Split1.Index = 2;
			this.menuItem_Split1.Text = "-";
			// 
			// mi_ViewLog
			// 
			this.mi_ViewLog.Index = 3;
			this.mi_ViewLog.Text = "查看本地数据";
			this.mi_ViewLog.Visible = false;
			this.mi_ViewLog.Click += new System.EventHandler(this.mi_ViewLog_Click);
			// 
			// menuItem_Split2
			// 
			this.menuItem_Split2.Index = 4;
			this.menuItem_Split2.Text = "-";
			this.menuItem_Split2.Visible = false;
			// 
			// menuItem_Exit
			// 
			this.menuItem_Exit.Index = 5;
			this.menuItem_Exit.Text = "退出";
			this.menuItem_Exit.Click += new System.EventHandler(this.menuItem_Exit_Click);
			// 
			// lb_state
			// 
			this.lb_state.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lb_state.Location = new System.Drawing.Point(24, 16);
			this.lb_state.Name = "lb_state";
			this.lb_state.Size = new System.Drawing.Size(168, 23);
			this.lb_state.TabIndex = 2;
			this.lb_state.Text = "停止。";
			// 
			// lbRunTime
			// 
			this.lbRunTime.Location = new System.Drawing.Point(25, 48);
			this.lbRunTime.Name = "lbRunTime";
			this.lbRunTime.Size = new System.Drawing.Size(167, 17);
			this.lbRunTime.TabIndex = 3;
			this.lbRunTime.Text = "运行时间：33天4小时55分";
			// 
			// lb_Version
			// 
			this.lb_Version.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lb_Version.Location = new System.Drawing.Point(24, 74);
			this.lb_Version.Name = "lb_Version";
			this.lb_Version.Size = new System.Drawing.Size(168, 23);
			this.lb_Version.TabIndex = 4;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(0, 171);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 5;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(211, 108);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.lb_Version);
			this.Controls.Add(this.lbRunTime);
			this.Controls.Add(this.lb_state);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.ShowInTaskbar = false;
			this.Text = "监控服务系统";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
			this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main() 
		{
			//string ss = @"^(/.+(\.gif|\.png|\.jpg|\.ico|\.css|\.js)(\?.+)?)$";
			//string s1 = "/aaa/bbb.js";
			//string s2 = "aaa/bbb.css?aa=bb";
			//string s3 = "/aaa/bbb/d.jpg?4";
			//System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(ss, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			//System.Text.RegularExpressions.MatchCollection matches = rx.Matches(s1);
			//foreach (System.Text.RegularExpressions.Match match in matches)
			//{
			//    string word = match.Groups["word"].Value;
			//    int index = match.Index;
			//    Console.WriteLine("{0} repeated at position {1}", word, index);
			//}
			//string sss = rx.Replace(s1, "$1");
			//matches = rx.Matches(s2);
			//foreach (System.Text.RegularExpressions.Match match in matches)
			//{
			//    string word = match.Groups["word"].Value;
			//    int index = match.Index;
			//    Console.WriteLine("{0} repeated at position {1}", word, index);
			//}
			//sss = "";
			//sss = rx.Replace(s2, "$1");
			//matches = rx.Matches(s3);
			//foreach (System.Text.RegularExpressions.Match match in matches)
			//{
			//    string word = match.Groups["word"].Value;
			//    int index = match.Index;
			//    Console.WriteLine("{0} repeated at position {1}", word, index);
			//}
			//sss = "";
			//sss = rx.Replace(s3, "a_{R:1}_b");


			//byte[] msg = new byte[1024];
			//int ErrorCode = 0;
			//bool b = LogInfo.Win32API.PrintTempletBarXML("D:\\戴光启交接\\打印源代码\\barTest\\1.btwxml", "D:\\戴光启交接\\打印源代码\\barTest\\1.Xml", "发送至 OneNote", ref ErrorCode, msg);


            //设置工作目录
            System.IO.Directory.SetCurrentDirectory(Application.StartupPath);
            Application.Run(new Form1());

		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
#if DEBUG
            this.mi_ViewLog.Visible = true;
            this.menuItem_Split2.Visible = true;
#else
            this.mi_ViewLog.Visible = false;
            this.menuItem_Split2.Visible = false;
#endif
            dp = new DataProcess.DataProcess();

            //文件修改时间
            try
            {
                DateTime dt = System.IO.File.GetLastWriteTime("Monitor.exe");
                lb_Version.Text = "版本：" + dt.ToString("yyyy-MM-dd");
            }
            catch
            {
            }
            bRun = true;
            bInit = false;
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadCheckTime));
            th.Start();
        }

        private void ThreadCheckTime()
        {
            SetText("正在启动程序.......", lb_state);
            if (dp.Start())
            {
                SetText("运行中...", lb_state);
            }
            else
            {
                SetText("开始失败。", lb_state);
                return;
            }

            uint dt1 = LogInfo.Win32API.GetTickCount();
            while (bRun)
            {
                System.Threading.Thread.Sleep(500);
                try
                {
                    uint dt2 = LogInfo.Win32API.GetTickCount();
                    uint ts = dt2 - dt1;
                    ts = ts / 1000;
                    if (ts>3600)
                    {
                        int curHour = DateTime.Now.Hour;
                        if (curHour == LogInfo.Constant.SysRebootTime)
                        {
                            if (dp != null)
                            {
                                dp.Stop();
                            }
                            System.Threading.Thread.Sleep(10000);
                            System.Diagnostics.Process.Start(Application.StartupPath + "\\Monitor.exe");
                            dp = null;
                            break;
                        }
                    }

                    uint Seconds = ts % 60;
                    ts = (ts - Seconds) / 60;
                    uint Minutes = ts % 60;
                    ts = (ts - Minutes) / 60;
                    uint Hours = ts % 24;
                    uint Days = (ts - Hours) / 24;
                    string msg = "运行时间：" + Days.ToString() + "天"
                        + Hours.ToString() + "小时" + Minutes.ToString() + "分" + Seconds.ToString() + "秒";
                    SetText(msg, lbRunTime);
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString());
                }
            }
            if (bRun)
            {
                CloseWindow();
            }
        }
        private void CloseWindow()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    CloseWindowCallback d = new CloseWindowCallback(CloseWindow);
                    this.Invoke(d);
                }
                else
                {
                    this.Closing -= new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
                    this.Close();
                    Application.Exit();
                }
            }
            catch
            { }
        }
        private void SetText(string text, Label lb)
        {
            try
            {
                if (lb.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    this.Invoke(d, new object[] { text, lb });
                }
                else
                {
                    if (!bInit)
                    {
                        m_notifyIcon_DoubleClick(null, null);
                        bInit = true;
                    }
                    lb.Text = text;
                }
            }
            catch
            { }
        }

		private void Form1_Closing(object sender, CancelEventArgs e)
		{
                bRun = false;
                if (dp != null)
                {
                    dp.Stop();
                    dp = null;
                }
		}

		private void menuItem_Show_Click(object sender, System.EventArgs e)
		{
			MenuItem t = (MenuItem)sender;
			if(t.Text=="显示主窗口")
			{
				this.Visible = true;
				this.menuItem_Show.Enabled = false;
				this.menuItem_Hide.Enabled = true;
			}
			else
			{
				this.Visible = false;
				this.menuItem_Show.Enabled = true;
				this.menuItem_Hide.Enabled = false;
			}
		}

		private void menuItem_Exit_Click(object sender, System.EventArgs e)
		{
                bRun = false;
                if (dp != null)
                {
                    dp.Stop();
                    dp = null;
                }
                Application.Exit();
		}

		private void m_notifyIcon_DoubleClick(object sender, System.EventArgs e)
		{
            if (this.Visible)
            {
                this.Visible = false;
                this.menuItem_Show.Enabled = true;
                this.menuItem_Hide.Enabled = false;
                this.TopMost = false;
            }
            else
            {
                this.Visible = true;
                this.menuItem_Show.Enabled = false;
                this.menuItem_Hide.Enabled = true;
                this.TopMost = true;
                this.Focus();
            }
		}

		private void Form1_Deactivate(object sender, System.EventArgs e)
		{
			this.TopMost = false;
		}

        private void mi_ViewLog_Click(object sender, EventArgs e)
        {
            FormData frm = new FormData();
            frm.Show();
        }

		private void button1_Click(object sender, EventArgs e)
		{
			DataBase.MonitorService.UserGetLetterReturnClass r = new DataBase.MonitorService.UserGetLetterReturnClass();
			r.bPrintSendReport = true;
			r.szSendTemplateFileName = "SendList_20RowA4.btwxml";
			r.szSendXmlData = "<?xml version=\"1.0\" encoding=\"gb2312\"?><XMLROOT><TBARTBARPRINTDATA><DATASTRUCT><COLS>512</COLS><ROWS>3</ROWS></DATASTRUCT><DATACONTENT><ITEM ROW=\"0\" COL=\"2\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>201520490914</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"1\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015年01月22日 12:30</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"3\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>42</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"4\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>崔平</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"6\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>1</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"5\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>最高法院</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"0\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>交换站</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"7\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015年01月22日 13:54</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"21\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000261</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"41\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>立案庭</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"61\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>秘密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"81\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"101\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:30</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"141\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>国家信访局</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"22\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>27960520</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"42\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"62\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>秘密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"82\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"102\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:30</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"142\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>中办机要交通局</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"23\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00980220</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"43\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"63\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"83\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>平急</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"103\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"143\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>国务院办公厅</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"24\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>70001041</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"44\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"64\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"84\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"104\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"144\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>公安部</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"25\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00094856</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"45\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"65\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"85\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"105\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"145\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>商务部</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"26\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000255</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"46\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"66\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"86\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"106\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"146\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>中央政法委</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"27\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00763699</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"47\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"67\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"87\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"107\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"147\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>国家发改委</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"28\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00050014</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"48\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"68\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"88\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"108\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"148\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>国家档案局</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"29\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00990240</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"49\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"69\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>秘密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"89\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"109\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"149\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>国务院办公厅</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"30\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00130003</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"50\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"70\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"90\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"110\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"150\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>国管局</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"31\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000699</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"51\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"71\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>秘密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"91\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>平急</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"111\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:32</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"151\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>中央台办</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"32\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00213035</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"52\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"72\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"92\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"112\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:32</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"152\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>总参二部</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"33\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00002545</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"53\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"73\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"93\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"113\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:32</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"153\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>外交部</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"34\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>27940520</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"54\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"74\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"94\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"114\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:32</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"154\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>中办机要交通局</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"35\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>27950520</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"55\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"75\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"95\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"115\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:32</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"155\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>中办机要交通局</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"36\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>08035014</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"56\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机要室</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"76\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"96\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"116\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:32</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"156\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>新华社</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"37\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00480003</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"57\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>周强</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"77\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"97\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"117\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:32</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"157\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>中央政法委</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"38\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>08065034</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"58\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>周强</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"78\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>秘密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"98\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"118\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:33</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"158\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>新华社</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"39\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>08034009</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"59\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>周强</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"79\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"99\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"119\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:33</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"159\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>新华社</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"40\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000552</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"60\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>周强</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"80\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"100\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"120\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:33</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"160\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>人民日报社</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"2\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>201520490914</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"1\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015年01月22日 12:30</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"3\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>42</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"4\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>崔平</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"6\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"5\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>最高法院</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"0\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>交换站</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"7\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015年01月22日 13:54</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"21\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>08038024</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"41\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>周强</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"61\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"81\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"101\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:33</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"141\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>新华社</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"22\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00870126</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"42\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>周强</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"62\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>秘密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"82\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"102\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:33</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"142\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>求是杂志社</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"23\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>03700614</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"43\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>二区</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"63\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"83\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"103\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:34</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"143\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>最高检察院</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"24\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00030000</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"44\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>二区</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"64\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"84\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"104\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:34</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"144\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>总政</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"25\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>08034043</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"45\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>二区</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"65\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"85\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"105\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:34</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"145\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>新华社</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"26\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000462</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"46\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>二区</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"66\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"86\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"106\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:34</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"146\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>市高法院</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"27\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000468</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"47\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>二区</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"67\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"87\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"107\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:34</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"147\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>市高法院</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"28\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>02100630</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"48\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>二区</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"68\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"88\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"108\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:34</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"148\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>最高检察院</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"29\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000464</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"49\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>二区</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"69\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"89\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"109\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:34</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"149\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>市高法院</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"30\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000325</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"50\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>外事局</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"70\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"90\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"110\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:35</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"150\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>司法部</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"31\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00320004</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"51\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机关党委</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"71\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"91\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"111\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:35</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"151\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>中央国家机关工委</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"32\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000324</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"52\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>外事局</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"72\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"92\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"112\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:35</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"152\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>司法部</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"33\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000245</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"53\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>民三庭</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"73\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"93\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"113\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:35</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"153\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>国家知识产权局</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"34\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000463</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"54\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>民一庭</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"74\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"94\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"114\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:35</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"154\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>市高法院</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"35\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000176</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"55\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>外事局</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"75\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"95\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"115\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:35</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"155\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>外交部</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"36\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00000175</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"56\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>外事局</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"76\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"96\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"116\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:36</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"156\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>外交部</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"37\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>02600236</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"57\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>行装局</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"77\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"97\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"117\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:36</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"157\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>公安部</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"38\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>06000611</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"58\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>研究室</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"78\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"98\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"118\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:36</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"158\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>最高检察院</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"39\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00001977</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"59\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>监察局</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"79\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>机密</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"99\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"119\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:36</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"159\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>中央纪委监察部</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"40\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00090075</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"60\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>行装局</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"80\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"100\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"120\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:37</ITEMDATA></ITEM><ITEM ROW=\"1\" COL=\"160\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>国管局</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"2\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>201520490914</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"1\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015年01月22日 12:31</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"3\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>42</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"4\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>崔平</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"6\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>3</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"5\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>最高法院</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"0\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>交换站</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"7\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015年01月22日 13:54</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"21\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00007644</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"41\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>研究室</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"61\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"81\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"101\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:37</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"141\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>民航局</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"22\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>00870127</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"42\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>沈德咏</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"62\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>秘密</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"82\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"102\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2015-01-22 12:38</ITEMDATA></ITEM><ITEM ROW=\"2\" COL=\"142\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>求是杂志社</ITEMDATA></ITEM></DATACONTENT></TBARTBARPRINTDATA></XMLROOT>";
			r.bPrintRecvReport = true;
			r.szRecvTemplateFileName = "DocRecvList_20RowA4.btwxml";
			r.szRecvXmlData = "<?xml version=\"1.0\" encoding=\"gb2312\"?><XMLROOT><TBARTBARPRINTDATA><DATASTRUCT><COLS>512</COLS><ROWS>1</ROWS></DATASTRUCT><DATACONTENT><ITEM ROW=\"0\" COL=\"2\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>201400030243</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"1\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2014 年 10 月 31 日</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"3\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"4\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>许顺生</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"6\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>1</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"5\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>远光通联</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"0\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>许总</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"21\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2014-10-31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"41\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>软件研发部</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"61\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"81\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2014年10月31日 18:22:45 </ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"101\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"121\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2014-10-31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"22\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2014-10-31</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"42\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>软件研发部</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"62\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA></ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"82\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2014年10月31日 18:22:45 </ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"102\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>普通</ITEMDATA></ITEM><ITEM ROW=\"0\" COL=\"122\" TYPENAME=\"TBarPrintDataItemText\"><ITEMDATA>2014-10-31</ITEMDATA></ITEM></DATACONTENT></TBARTBARPRINTDATA></XMLROOT>";

			DataBase.PrintData p = new DataBase.PrintData(r, "Microsoft XPS Document Writer");
			System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback( DataBase.DataSave.ThreadPrintReport), p);
		}
    }
}
