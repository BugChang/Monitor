using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;

#region 枚举
/// <summary>
/// 状态
/// </summary>
public enum BCState
{
    BCSuccess = 0,	// 成功
    BCErrBCType = 1,
    BCErrBCVersion = 2,
    BCErrBCSecOption = 3,
    BCErrInitParamBuf = 4,
    BCErrSetParam = 5,
    BCErrGetParam = 6,
    BCErrNullParam = 7,
    BCErrBCEncode = 8,
    BCErrSaveBCImg = 9,
    BCErrBCFormat = 10,
    BCErrOpenCom = 11,
    BCErrComNum = 12,
    BCErrScanNotify = 13,
    BCErrImgPath = 14,
    BCErrImgMulti = 15,
    BCErrApplyMemory = 16,
    BCErrSocketInit = 17,
    BCErrConnectSvr = 18,
    BCErrSvrExecute = 19,
    BCErrConfig = 20,
    BCErrCommTimeout = 21,
    BCErrDevBusy = 22,
    BCErrDevEvtInit = 23,
};

public enum BCAttrType
{
    BCTGeneral = 0,	//通用属性
    BCTSpecial = 1,	//专用属性
    BCTCustomize = 2,	//自定义属性
};

public enum BCGeneralAttr
{
    BCGVersion = 0,	//编码版本标识
    BCGCdsId = 1,	//编解码器标识
    BCGSerialId = 2,	//标签序列号
    BCGDate = 3,	//条码制作日期
    BCGEncrypt = 4,	//加密标识
};

public enum BCAttrName
{
    BCANone = 0,
    BCAFileId = 1,	//秘密载体编号
    BCASendUnitId = 2,	//发件单位编号
    BCASendUnitName = 3,	//发件单位名称
    BCARecvUnitId = 4,	//收件单位编号
    BCARecvUnitName = 5,	//收件单位名称
    BCAAddressee = 6,	//收件人  
    BCASendType = 7,	//收发类型
    BCAFileSec = 8,	//秘密载体密级
    BCAFileType = 9,	//秘密载体类型
    BCAChannel = 10,	//渠道类别
    BCAFileUrgency = 11,	//缓急程度
};
#endregion
#region 中铁信方法
/// <summary>
/// Jy 的摘要说明
/// </summary>
/// 
public class Jy
{
    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    [DllImport("JYBarcodeLib.dll", CharSet = CharSet.Ansi)]
    public static extern BCState SetServer(string ip, Int32 port);
    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="data"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    [DllImport("JYBarcodeLib.dll", CharSet = CharSet.Ansi)]
    public static extern BCState DecodeScdcc(byte[] data, int len);
    /// <summary>
    /// 取解码的属性
    /// </summary>
    /// <param name="eType"></param>
    /// <param name="eAttrId"></param>
    /// <param name="cpValue">返回值是对应条码属性</param>
    /// <param name="size">cpValue的值数组的大小</param>
    /// <returns></returns>
    [DllImport("JYBarcodeLib.dll", CharSet = CharSet.Ansi)]
    public static extern BCState GetAttribute(BCAttrType eType, int eAttrId, System.Text.StringBuilder cpValue, int size);
    public Jy()
    { }
    
}
#endregion
#region 字符、byte64转化类
/// <summary>
/// 字符串Byte64编解码类
/// </summary>
internal class DeBarNode
{
    /// <summary>
    /// 对包含“|、^、&”的字符串进行Byte64编码
    /// </summary>
    /// <param name="barNode"></param>
    /// <returns></returns>
    internal static string EncodeBar(int BarNo, string barNode)
    {
        if (Regex.IsMatch(barNode, "[|,^,&]+"))
        {
            return BarNo.ToString("00") + "^^" + DeCodeStrongToByte64(barNode);
        }
        else
        {
            return BarNo.ToString("00") + "^" + barNode;
        }
    }
    /// <summary>
    /// 字符串转化为Byte64位字符串
    /// </summary>
    /// <param name="inString">要解码的String</param>
    /// <returns>返回Byte64字符串</returns>
    private static string DeCodeStrongToByte64(string inString)
    {
        string returnString = "";
        try
        {
            byte[] b =Encoding.Default.GetBytes(inString);
            returnString = System.Convert.ToBase64String(b, 0, b.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return returnString;
    }
    /// <summary>
    /// Byte64解码为字符串
    /// </summary>
    /// <param name="inByte64String">要解码的Byte64字符串</param>
    /// <returns>解码结果</returns>
    private static string DeCodeByte64ToString(string inByte64String)
    {
        string returnString = "";
        try
        {
            char[] c = inByte64String.ToCharArray();
            byte[] b = System.Convert.FromBase64String(inByte64String);
            returnString = Encoding.Default.GetString(b);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return returnString;
    }
}

#endregion
/// <summary>
/// 解码类
/// </summary>
public class DecodeBar
{
    public string Debar(byte[] bar)
    {
        byte[] b = new byte[10];
        int temp = 256;  //cpValue的值数组的大小

        BCState bc = Jy.SetServer(LogInfo.Constant.SCDCC_IP, LogInfo.Constant.SCDCC_Port);
        bc = Jy.DecodeScdcc(bar, bar.Length);
		if (bc != BCState.BCSuccess)
		{
			return "JY|";
		}
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder(256);
        System.Text.StringBuilder sb1 = new System.Text.StringBuilder();

        //BCAttrType.BCTCustomize
        //通用属性
        sb1.Append("JY");
        //for (int i = 0; i < 5; ++i)
        //{
        //    Jy.GetAttribute(BCAttrType.BCTGeneral, i, sb, 100);
        //    sb1.Append(sb.ToString());
        //}
        try
        {
            for (int i = 0; i < 5; i++)
            {
                sb.Remove(0, sb.Length);
                bc = Jy.GetAttribute(BCAttrType.BCTGeneral, i, sb, temp);
                if (bc!= BCState.BCSuccess)
                    throw new Exception("读取条码错误");
                if (i == 0)
                {
                    if (sb.ToString().Length == 2)//2位
                    {
                        sb1.Append(sb.ToString());
                    }
                    else
                    {
                        throw new Exception("读取条码错误");
                    }
                }
                if (i == 1)
                {
                    if (sb.ToString().Length == 8)//8位
                    {
                        sb1.Append(sb.ToString());
                    }
                    else
                    {
                        throw new Exception("读取条码错误");
                    }
                }
                if (i == 2)
                {
                    if (sb.ToString().Length == 10)//10位
                    {
                        sb1.Append(sb.ToString());
                    }
                    else
                    {
                        throw new Exception("读取条码错误");
                    }
                }
                if (i == 3)
                {
                    if (sb.ToString().Length == 14)//14位
                    {
                        sb1.Append(sb.ToString());
                    }
                    else
                    {
                        throw new Exception("读取条码错误");
                    }
                }
                if (i == 4)
                {
                    if (sb.ToString().Length == 1)//1位
                    {
                        sb1.Append(sb.ToString());
                    }
                    else
                    {
                        throw new Exception("读取条码错误");
                    }
                }
            }

            sb1.Append("&");
            //专用属性
            for (int i = 1; i < 12; ++i)
            {
                sb.Remove(0, sb.Length);
                bc = Jy.GetAttribute(BCAttrType.BCTSpecial, i, sb, temp);
                if (bc == BCState.BCErrGetParam || bc == BCState.BCSuccess)
                {
                    sb1.Append(DeBarNode.EncodeBar(i, sb.ToString()));
                    sb1.Append("|");
                }
                else
                {
                    throw new Exception("读取条码错误");
                }
            }
            sb1.Append("&");
            //自定义属性
            for (int i = 1; i < 5; ++i)
            {
                sb.Remove(0, sb.Length);
                bc = Jy.GetAttribute(BCAttrType.BCTCustomize, i, sb, temp);  
                if (bc == BCState.BCErrGetParam || bc == BCState.BCSuccess)//参数不全时不能抛出异常
                {
                    sb1.Append(DeBarNode.EncodeBar(i, sb.ToString()));
                    sb1.Append("|");
                }
                else
                {
                    throw new Exception("读取条码错误"); 
                }
            }
        }
        catch (Exception ex)
        {
			LogInfo.Log.WriteInfo(LogInfo.LogType.Warnning, "解析SCDCC条码出错：" + ex.ToString());
		}
        sb1.Append("|");
        return sb1.ToString();
    }
}