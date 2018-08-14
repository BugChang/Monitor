using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DataBase
{
    class GuiZeClass
    {
        /// <summary>
        /// 1��	��ȡ����������ͽӿ�
        /// </summary>
        /// <param name="FullBarCode">��Ҫ��������������</param>
        /// <returns>�������������Ĺ���ı�ʶ��</returns>
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
        /// 2��	��ȡ��ά������һά�����Žӿ�
        /// </summary>
        /// <param name="FullBarCode">��Ҫ��������������</param>
        /// <returns>���ض�ά�����е�һά�����š�����������һά���룬�򷵻�ԭ���뼴�ɡ�</returns>
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
        /// 3��	��ȡ�����ռ���λ��Žӿ�
        /// </summary>
        /// <param name="FullBarCode">��Ҫ��������������</param>
        /// <returns>���ش������н��������ռ���λ�ı�š�
        /// ��������в����ռ���λ��Ϣ���򷵻ؿգ�""�����ɡ�
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
        /// 4��	��ȡ���뷢����λ��Žӿ�
        /// </summary>
        /// <param name="FullBarCode">��Ҫ��������������</param>
        /// <returns>���ش������н������ķ�����λ�ı�š�
        /// ��������н���������������λ��Ϣ���򷵻ؿգ�""�����ɡ�
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
		/// 5��	��ȡ����������Խӿ�
		/// </summary>
		/// <param name="FullBarCode">��Ҫ��������������</param>
		/// <returns>���ش������н������ķ�����λ�ı�š�
		/// ��������н���������������λ��Ϣ���򷵻ؿգ�""�����ɡ�
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
