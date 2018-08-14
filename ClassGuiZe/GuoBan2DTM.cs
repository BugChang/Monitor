using System;
using System.Collections.Generic;
using System.Text;

//using Farshine.GuoBan.Trace;

namespace ClassGuiZe
{
    /// <summary>
    /// 
    /// </summary>
    public class GuoBanTM //: IGuoBan2DTM
    {
        public GuoBanTM()
        {
        }
        public GuoBanTM(string BanBenBS, string TiaoMaBH, string ChengWenRQ, string FaWenDW, string WenZhong, string WenHao, string MiMiDG, string JinJiCD, string WenJianBT, string ZhuSongDW, string FaBuCC, string TiaoMaZZDW, string TiaoMaZZRQ, string BaoMiQX)
        {
            this._BanBenBS = BanBenBS;
            this.TiaoMaBH = TiaoMaBH;
            this.ChengWenRQ = Convert.ToDateTime(ChengWenRQ);
            this.FaWenDW = FaWenDW;
            this.WenZhongKM = WenZhong;
            this.WenHaoQH = WenHao;
            this.MiMiDJ = MiMiDJ;
            this.JinJiCD = JinJiCD;
            this.WenJianBT = WenJianBT;
            this.ZhuSongDW = ZhuSongDW;
            this.ZhiZuoDW = TiaoMaZZDW;
            this.ZhiZuoRQ = Convert.ToDateTime(TiaoMaZZRQ);
            this.BaoMiQX = BaoMiQX;
        }
        /// <summary>
        /// 版本标识
        /// </summary>
        public string BanBenBS
        {
            get
            {
                return _BanBenBS;
            }
            set
            {
                _BanBenBS = value;
            }
        }
        private string _BanBenBS ="";

        /// <summary>
        /// 条码编号
        /// </summary>
        public string TiaoMaBH
        {
            get
            {
                return _TiaoMaBH;
            }
            set
            {
                _TiaoMaBH = value;
            }
        }
        private string _TiaoMaBH ="";

        /// <summary>
        /// 发文单位
        /// </summary>
        public string FaWenDW
        {
            get
            {
                return _FaWenDW;
            }
            set
            {
                _FaWenDW = value;
            }
        }
        private string _FaWenDW = "";

        /// <summary>
        /// 文种或刊名
        /// </summary>
        public string WenZhongKM
        {
            get
            {
                return _WenZhongKM;
            }
            set
            {
                _WenZhongKM = value;
            }
        }
        private string _WenZhongKM = "";

        /// <summary>
        /// 文号或期号
        /// </summary>
        public string WenHaoQH
        {
            get
            {
                return _WenHaoQH;
            }
            set
            {
                _WenHaoQH = value;
            }
        }
        private string _WenHaoQH = "";

        /// <summary>
        /// 秘密等级
        /// </summary>
        public string MiMiDJ
        {
            get
            {
                return _MiMiDJ;
            }
            set
            {
                _MiMiDJ = value;
            }
        }
        private string _MiMiDJ = "";

        /// <summary>
        /// 保密期限
        /// </summary>
        public string BaoMiQX
        {
            get
            {
                return _BaoMiQX;
            }
            set
            {
                _BaoMiQX = value;
            }
        }
        private string _BaoMiQX = "";

        /// <summary>
        /// 紧急程度
        /// </summary>
        public string JinJiCD
        {
            get
            {
                return _JinJiCD;
            }
            set
            {
                _JinJiCD = value;
            }
        }
        private string _JinJiCD = "";

        /// <summary>
        /// 文件标题
        /// </summary>
        public string WenJianBT
        {
            get
            {
                return _WenJianBT;
            }
            set
            {
                _WenJianBT = value;
            }
        }
        private string _WenJianBT = "";

        /// <summary>
        /// 主送单位
        /// </summary>
        public string ZhuSongDW
        {
            get
            {
                return _ZhuSongDW;
            }
            set
            {
                _ZhuSongDW = value;
            }
        }
        private string _ZhuSongDW = "";

        /// <summary>
        /// 发布层次
        /// </summary>
        public string FaBuCC
        {
            get
            {
                return _FaBuCC;
            }
            set
            {
                _FaBuCC = value;
            }
        }
        private string _FaBuCC = "";


        /// <summary>
        /// 成文日期
        /// </summary>
        public DateTime ChengWenRQ
        {
            get
            {
                return _ChengWenRQ;
            }
            set
            {
                _ChengWenRQ = value;
            }
        }
        private DateTime _ChengWenRQ ;//= "";

        /// <summary>
        /// 条码制作单位
        /// </summary>
        public string ZhiZuoDW
        {
            get
            {
                return _ZhiZuoDW;
            }
            set
            {
                _ZhiZuoDW = value;
            }
        }
        private string _ZhiZuoDW = "";

        /// <summary>
        /// 条码制作日期
        /// </summary>
        public DateTime ZhiZuoRQ
        {
            get
            {
                return _ZhiZuoRQ;
            }
            set
            {
                _ZhiZuoRQ = value;
            }
        }
        private DateTime _ZhiZuoRQ;// = "";

        #region IGuoBan2DTM 成员

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BanBenBS"></param>
        /// <param name="TiaoMaBH"></param>
        /// <param name="ChengWenRQ"></param>
        /// <param name="FaWenDW"></param>
        /// <param name="WenZhong"></param>
        /// <param name="WenHao"></param>
        /// <param name="MiMiDJ"></param>
        /// <param name="JinJiCD"></param>
        /// <param name="WenJianBT"></param>
        /// <param name="ZhuSongDW"></param>
        /// <param name="FaBuCC"></param>
        /// <param name="TiaoMaZZDW"></param>
        /// <param name="TiaoMaZZRQ"></param>
        /// <param name="BaoMiQX"></param>
        /// <returns></returns>
        public string ShengCheng2DTM(string BanBenBS, string TiaoMaBH, string ChengWenRQ, string FaWenDW, string WenZhong, string WenHao, string MiMiDJ, string JinJiCD, string WenJianBT, string ZhuSongDW, string FaBuCC, string TiaoMaZZDW, string TiaoMaZZRQ, string BaoMiQX)
        {
            string ShengChengText = "";

           // GetTiaoMaObject(BanBenBS, TiaoMaBH, ChengWenRQ, FaWenDW, WenZhong, WenHao, MiMiDG, JinJiCD, WenJianBT, ZhuSongDW, FaBuCC, TiaoMaZZDW, TiaoMaZZRQ, BaoMiQX);

            //拼写二维条码(rzy 2008-1-17)
            ShengChengText = BanBenBS + "^" + TiaoMaBH + "^" + FaWenDW + "^" + WenZhong + "^" + WenHao + "^" + ZhuSongDW + "^"
                + WenJianBT + "^" + MiMiDJ + BaoMiQX +"^" + JinJiCD + "^" + ChengWenRQ + "^" + FaBuCC + "^" + TiaoMaZZDW + "^" + TiaoMaZZRQ + "^^|";

            return ShengChengText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GuoBanTMDX"></param>
        /// <returns></returns>
        public string ShengCheng2DTM(GuoBanTM GuoBanTMDX)
        {
            string ShengChengText = "";

            ShengChengText = GuoBanTMDX.BanBenBS + "^" + GuoBanTMDX.TiaoMaBH + "^" + GuoBanTMDX.FaWenDW + "^" + GuoBanTMDX.WenZhongKM + "^"
                + GuoBanTMDX.WenHaoQH + "^" + GuoBanTMDX.ZhuSongDW + "^" +
                GuoBanTMDX.WenJianBT + "^" + GuoBanTMDX.MiMiDJ + GuoBanTMDX.BaoMiQX + "^" + GuoBanTMDX.JinJiCD + "^"
                + GuoBanTMDX.ChengWenRQ.ToString("yyyyMMdd") + "^" + GuoBanTMDX.FaBuCC + "^" + GuoBanTMDX.ZhiZuoDW + "^" + GuoBanTMDX.ZhiZuoRQ.ToString("yyyyMMdd") + "^^|";

            return ShengChengText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TiaoMaZ"></param>
        /// <returns></returns>
        public GuoBanTM JieXi2DTM(string 条码值)
        {
            try
            {
                if (条码值 == null)
                {
                    return null;
                }
                if (条码值.Split('^').Length >= 14)
                {
                    string[] strTMZhi = 条码值.Split('^');
                    this.BanBenBS = strTMZhi[0].ToString().Trim();
                    this.TiaoMaBH = strTMZhi[1].ToString().Trim();
                    this.FaWenDW = strTMZhi[2].ToString().Trim();
                    this.WenZhongKM = strTMZhi[3].ToString().Trim();
                    this.WenHaoQH = strTMZhi[4].ToString().Trim();
                    this.ZhuSongDW = strTMZhi[5].ToString().Trim();
                    this.WenJianBT = strTMZhi[6].ToString().Trim();
                    if (strTMZhi[7].ToString().Trim().Length <= 2)
                    {
                        this.MiMiDJ = strTMZhi[7].ToString().Trim();
                        this.BaoMiQX = "";
                    }
                    else
                    {
                        this.MiMiDJ = strTMZhi[7].ToString().Trim().Substring(0,2);
                        this.BaoMiQX = strTMZhi[7].ToString().Trim().Substring(2);
                    }
                    this.JinJiCD = strTMZhi[8].ToString().Trim();
                    string strChengWenRQ = strTMZhi[9].ToString().Trim();
                    if (strChengWenRQ.Length == 8)
                    {
                        this.ChengWenRQ = DateTime.Parse(strChengWenRQ.Substring(0, 4) 
                            + "-" + strChengWenRQ.Substring(4, 2) 
                            + "-" + strChengWenRQ.Substring(6, 2));
                    }
                    this.FaBuCC = strTMZhi[10].ToString().Trim();
                    this.ZhiZuoDW = strTMZhi[11].ToString().Trim();
                    string strZhiZuoRQ = strTMZhi[12].ToString().Trim();
                    if (strZhiZuoRQ.Length == 8)
                    {
                        this.ZhiZuoRQ = DateTime.Parse(strZhiZuoRQ.Substring(0, 4) 
                            + "-" + strZhiZuoRQ.Substring(4, 2) 
                            + "-" + strZhiZuoRQ.Substring(6, 2));
                    }
                    return this;
                }
                return null;
            }
            //解析失败，则返回null
            catch (Exception ex)
            {
                string ss = ex.ToString();
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 通过条码各段值生成两办条码
        /// </summary>
        /// <param name="BanBenBS"></param>
        /// <param name="TiaoMaBH"></param>
        /// <param name="ChengWenRQ"></param>
        /// <param name="FaWenDW"></param>
        /// <param name="WenZhong"></param>
        /// <param name="WenHao"></param>
        /// <param name="MiMiDJ"></param>
        /// <param name="JinJiCD"></param>
        /// <param name="WenJianBT"></param>
        /// <param name="ZhuSongDW"></param>
        /// <param name="FaBuCC"></param>
        /// <param name="TiaoMaZZDW"></param>
        /// <param name="TiaoMaZZRQ"></param>
        /// <returns></returns>
        protected void GetTiaoMaObject(string BanBenBS, string TiaoMaBH, string ChengWenRQ, string FaWenDW, string WenZhong, string WenHao, string MiMiDJ, string JinJiCD, string WenJianBT, string ZhuSongDW, string FaBuCC, string TiaoMaZZDW, string TiaoMaZZRQ, string BaoMiQX)
        {
            this._BanBenBS = BanBenBS;
            this.TiaoMaBH = TiaoMaBH;
            this.ChengWenRQ = Convert.ToDateTime(ChengWenRQ);
            this.FaWenDW = FaWenDW;
            this.WenZhongKM = WenZhong;
            this.WenHaoQH = WenHao;
            this.MiMiDJ = MiMiDJ;
            this.JinJiCD = JinJiCD;
            this.WenJianBT = WenJianBT;
            this.ZhuSongDW = ZhuSongDW;
            this.ZhiZuoDW = TiaoMaZZDW;
            this.ZhiZuoRQ = Convert.ToDateTime(TiaoMaZZRQ);
            this.BaoMiQX = BaoMiQX;
        }

        #endregion

        public string 条码图片文件名
        {
            get
            {
                return this._WenZhongKM+this._WenHaoQH;
            }
        }

        #region IGuoBan2DTM 成员

        //string IGuoBan2DTM.ShengCheng2DTM(string BanBenBS, string TiaoMaBH, string ChengWenRQ, string FaWenDW, string WenZhong, string WenHao, string MiMiDG, string JinJiCD, string WenJianBT, string ZhuSongDW, string FaBuCC, string TiaoMaZZDW, string TiaoMaZZRQ)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //string IGuoBan2DTM.ShengCheng2DTM(GuoBanTM GuoBanTMDX)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //GuoBanTM IGuoBan2DTM.JieXi2DTM(string TiaoMaZ)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        #endregion
    }
}
