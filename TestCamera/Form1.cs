using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestCamera
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void bt_Snap_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 dt = Environment.TickCount;
                string FileData = "";
                bool b = CameraClass.Camera.GetPicture(tx_IP.Text.Trim(), Convert.ToInt32(tx_Port.Text), ref FileData);
                if (!b)
                {
                    MessageBox.Show("抓图时候出错，请检查摄像头连接。");
                }
                else
                {
                    if (this.pictureBox1.Tag != null)
                    {
                        System.IO.MemoryStream ms1 = (System.IO.MemoryStream)this.pictureBox1.Tag;
                        ms1.Close();
                        ms1.Dispose();
                    }
                    byte[] buf = Convert.FromBase64String(FileData);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    ms.Write(buf,0,buf.Length);
                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    Image img = Image.FromStream(ms);
                    //ms.Close();
                    this.pictureBox1.Image = img;
                    this.pictureBox1.Tag = ms;
                    bt_Save.Enabled = true;
                    dt = Environment.TickCount - dt;
                    lb_ts.Text = "成功，耗时：" + dt.ToString();
                    MessageBox.Show("成功，耗时：" + dt.ToString());
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("出错：" + ee.ToString());
                lb_ts.Text = "出错：";
            }
        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog d = new SaveFileDialog();
                d.Filter = "JPG | *.jpg";
                if (d.ShowDialog() == DialogResult.OK)
                {
                    Image img = pictureBox1.Image;
                    img.Save(d.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("出错：" + ee.ToString());
            }
        }

        private void bt_init_Click(object sender, EventArgs e)
        {
            bool b = CameraClass.Camera.SetBaud(tx_IP.Text.Trim(), Convert.ToInt32(tx_Port.Text));
            if (!b)
            {
                MessageBox.Show("初始化时候出错，请检查摄像头连接。");
            }
            else
            {
                MessageBox.Show("初始化成功。");
            }
        }
    }
}