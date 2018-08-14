using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DataBase
{
    class GuiZeClass
    {
        /// <summary>
        /// 1、	获取条码规则类型接口
        /// </summary>
        /// <param name="FullBarCode">需要分析的完整条码</param>
        /// <returns>返回条码所属的规则的标识。</returns>
        public static int GetBarCodeType(string FullBarCode)
        {
            try
            {
                System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFile(System.IO.Directory.GetCurrentDirectory() + "\\ClassGuiZe.dll");

                // Get the type of 'MyClass'.
                Type myType = ass.GetType("ClassGuiZe.GuiZe");

                System.Reflection.MethodInfo md = myType.GetMethod("GetBarCodeType");
                object o1 = md.Invoke(null, new object[] { FullBarCode });
                return (int)o1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e.Message);
            }

            return 0;
        }

        /// <summary>
        /// 2、	获取二维条码中一维条码编号接口
        /// </summary>
        /// <param name="FullBarCode">需要分析的完整条码</param>
        /// <returns>返回二维条码中的一维条码编号。如果输入的是一维条码，则返回原条码即可。</returns>
        public static string GetOneBarcode(string FullBarCode)
        {
            try
            {
                System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFile(System.IO.Directory.GetCurrentDirectory() + "\\ClassGuiZe.dll");

                // Get the type of 'MyClass'.
                Type myType = ass.GetType("ClassGuiZe.GuiZe");

                System.Reflection.MethodInfo md = myType.GetMethod("GetOneBarcode");
                object o1 = md.Invoke(null, new object[] { FullBarCode });
                return o1.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e.Message);
            }

            return "";

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
            try
            {
                System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFile(System.IO.Directory.GetCurrentDirectory() + "\\ClassGuiZe.dll");

                // Get the type of 'MyClass'.
                Type myType = ass.GetType("ClassGuiZe.GuiZe");

                System.Reflection.MethodInfo md = myType.GetMethod("GetBarcodeUnitCode");
                object o1 = md.Invoke(null, new object[] { FullBarCode });
                return o1.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e.Message);
            }

            return "";
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
            try
            {
                System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFile(System.IO.Directory.GetCurrentDirectory() + "\\ClassGuiZe.dll");

                // Get the type of 'MyClass'.
                Type myType = ass.GetType("ClassGuiZe.GuiZe");

                System.Reflection.MethodInfo md = myType.GetMethod("GetBarcodeSendUnitCode");
                object o1 = md.Invoke(null, new object[] { FullBarCode });
                return o1.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e.Message);
            }

            return "";
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
			try
			{
				System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFile(System.IO.Directory.GetCurrentDirectory() + "\\ClassGuiZe.dll");

				// Get the type of 'MyClass'.
				Type myType = ass.GetType("ClassGuiZe.GuiZe");

				System.Reflection.MethodInfo md = myType.GetMethod("CheckBarcodeIsJiJian");
				object o1 = md.Invoke(null, new object[] { FullBarCode });
				return Convert.ToBoolean(o1);
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception : " + e.Message);
			}

			return false;
		}
    }
}
