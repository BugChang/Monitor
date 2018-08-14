using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClassGuiZe
{
    public class GuiZe
    {
        /// <summary>
        /// 1、	获取条码规则类型接口
        /// </summary>
        /// <param name="FullBarCode">需要分析的完整条码</param>
        /// <returns>返回条码所属的规则的标识。</returns>
        public static int GetBarCodeType(string FullBarCode)
        {
            int ret = 0;
            //计算追溯码长度
            int tracingCodeLength = FullBarCode.Length;

            Regex reg_GB = new Regex(@"^GB0626-2005(\^[^\^]*){14}\^\|$", RegexOptions.Singleline);
            Regex reg_QGWS = new Regex(@"^\([0-9]{1,3}\).+\^\|$", RegexOptions.Singleline);
            Regex reg_GDJHZ = new Regex(@"^BQZNJH200802(\^[^\^]*){13}\^\|$", RegexOptions.Singleline);

            if (reg_GB.Match(FullBarCode).Success)
            {
                //文件_通码_机关公文二维条码
                ret = 6;
            }
            else if (reg_GDJHZ.Match(FullBarCode).Success)
            {
                //文件_通码_机关公文二维条码
                ret = 6;
            }
            else if (tracingCodeLength == 33)
            {
                //文件_QGWS1
                ret = 7;
            }
            else if (tracingCodeLength == 12)
            {
                //信件_唯一码_市机12位
                ret = 5;
            }
            else if (tracingCodeLength == 13)
            {
                //信件_唯一码_EMS13位信件
                ret = 4;
            }
            else if (tracingCodeLength == 17)
            {
                //信件_唯一码_安全部33位信件
                ret = 3;
            }
            else if (tracingCodeLength == 26)
            {
                //信件_唯一码_国办交换站26位信件
                ret = 2;
            }
            else if (tracingCodeLength == 37)
            {
                //TracingCode, 为信件条码，信件_QGWS1
                ret = 1;
            }
            else if (FullBarCode.StartsWith("JY"))
            {
                ret = 8;   //SCDCC条码        
            }
            return ret;
        }

        /// <summary>
        /// 2、	获取二维条码中一维条码编号接口
        /// </summary>
        /// <param name="FullBarCode">需要分析的完整条码</param>
        /// <returns>返回二维条码中的一维条码编号。如果输入的是一维条码，则返回原条码即可。</returns>
        public static string GetOneBarcode(string FullBarCode)
        {
            string strResult = "";
            switch (GetBarCodeType(FullBarCode))
            {

                //case 1:
                //    GuoBanTM objGuoBanTM = new GuoBanTM().JieXi2DTM(FullBarCode.Trim());
                //    if (!(objGuoBanTM == null))
                //    {
                //        strResult = objGuoBanTM.TiaoMaBH.Trim();
                //    }
                //    else
                //        strResult = FullBarCode;
                //    break;

                default:
                    strResult = FullBarCode;
                    break;
            }
            return strResult;
        }

        /// <summary>
        /// 3、	获取条码收件单位编号接口
        /// </summary>
        /// <param name="FullBarCode">需要分析的完整条码</param>
        /// <returns>返回从条码中解析出的收件单位的编号。
        /// 如果条码中不含收件单位信息，则返回空（""）即可。
        /// </returns>
        public static string GetBarcodeUnitCode(string FullBarCode)
        {
            string strResult = "";
            switch (GuiZe.GetBarCodeType(FullBarCode))
            {
                //case 2:
                //    strResult = FullBarCode.Substring(22, 3);
                //    break;

                default:
                    strResult = "";
                    break;
            }
            return strResult;
        }

        /// <summary>
        /// 4、	获取条码发件单位编号接口
        /// </summary>
        /// <param name="FullBarCode">需要分析的完整条码</param>
        /// <returns>返回从条码中解析出的发件单位的编号。
        /// 如果条码中解析不出来发件单位信息，则返回空（""）即可。
        /// </returns>
        public static string GetBarcodeSendUnitCode(string FullBarCode)
        {
            string strResult = "";
            switch (GuiZe.GetBarCodeType(FullBarCode))
            {
                //case 1:
                //    GuoBanTM objGuoBanTM = new GuoBanTM().JieXi2DTM(FullBarCode.Trim());
                //    if (!(objGuoBanTM == null))
                //    {
                //        strResult = objGuoBanTM.TiaoMaBH.Trim();
                //    }
                //    break;
                //case 2:
                //    strResult = FullBarCode.Substring(0, 3);
                //    break;

                default:
                    strResult = "";
                    break;
            }
            return strResult;
        }

		/// <summary>
		/// 5、	获取条码紧急属性接口
		/// </summary>
		/// <param name="FullBarCode">需要分析的完整条码</param>
		/// <returns>返回从条码中解析出的发件单位的编号。
		/// 如果条码中解析不出来发件单位信息，则返回空（""）即可。
		/// </returns>
		public static bool CheckBarcodeIsJiJian(string FullBarCode)
		{
			bool bRet = false;
			switch (GuiZe.GetBarCodeType(FullBarCode))
			{
				//case 1:
				//    GuoBanTM objGuoBanTM = new GuoBanTM().JieXi2DTM(FullBarCode.Trim());
				//    if (!(objGuoBanTM == null))
				//    {
				//        strResult = objGuoBanTM.TiaoMaBH.Trim();
				//    }
				//    break;
				//case 2:
				//    strResult = FullBarCode.Substring(0, 3);
				//    break;

				default:
					bRet = false;
					break;
			}
			return bRet;
		}
    }
}
