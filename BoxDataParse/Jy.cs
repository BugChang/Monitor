using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;

#region ö��
/// <summary>
/// ״̬
/// </summary>
public enum BCState
{
    BCSuccess = 0,	// �ɹ�
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
    BCTGeneral = 0,	//ͨ������
    BCTSpecial = 1,	//ר������
    BCTCustomize = 2,	//�Զ�������
};

public enum BCGeneralAttr
{
    BCGVersion = 0,	//����汾��ʶ
    BCGCdsId = 1,	//���������ʶ
    BCGSerialId = 2,	//��ǩ���к�
    BCGDate = 3,	//������������
    BCGEncrypt = 4,	//���ܱ�ʶ
};

public enum BCAttrName
{
    BCANone = 0,
    BCAFileId = 1,	//����������
    BCASendUnitId = 2,	//������λ���
    BCASendUnitName = 3,	//������λ����
    BCARecvUnitId = 4,	//�ռ���λ���
    BCARecvUnitName = 5,	//�ռ���λ����
    BCAAddressee = 6,	//�ռ���  
    BCASendType = 7,	//�շ�����
    BCAFileSec = 8,	//���������ܼ�
    BCAFileType = 9,	//������������
    BCAChannel = 10,	//�������
    BCAFileUrgency = 11,	//�����̶�
};
#endregion
#region �����ŷ���
/// <summary>
/// Jy ��ժҪ˵��
/// </summary>
/// 
public class Jy
{
    /// <summary>
    /// ����
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    [DllImport("JYBarcodeLib.dll", CharSet = CharSet.Ansi)]
    public static extern BCState SetServer(string ip, Int32 port);
    /// <summary>
    /// ����
    /// </summary>
    /// <param name="data"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    [DllImport("JYBarcodeLib.dll", CharSet = CharSet.Ansi)]
    public static extern BCState DecodeScdcc(byte[] data, int len);
    /// <summary>
    /// ȡ���������
    /// </summary>
    /// <param name="eType"></param>
    /// <param name="eAttrId"></param>
    /// <param name="cpValue">����ֵ�Ƕ�Ӧ��������</param>
    /// <param name="size">cpValue��ֵ����Ĵ�С</param>
    /// <returns></returns>
    [DllImport("JYBarcodeLib.dll", CharSet = CharSet.Ansi)]
    public static extern BCState GetAttribute(BCAttrType eType, int eAttrId, System.Text.StringBuilder cpValue, int size);
    public Jy()
    { }
    
}
#endregion
#region �ַ���byte64ת����
/// <summary>
/// �ַ���Byte64�������
/// </summary>
internal class DeBarNode
{
    /// <summary>
    /// �԰�����|��^��&�����ַ�������Byte64����
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
    /// �ַ���ת��ΪByte64λ�ַ���
    /// </summary>
    /// <param name="inString">Ҫ�����String</param>
    /// <returns>����Byte64�ַ���</returns>
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
    /// Byte64����Ϊ�ַ���
    /// </summary>
    /// <param name="inByte64String">Ҫ�����Byte64�ַ���</param>
    /// <returns>������</returns>
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
/// ������
/// </summary>
public class DecodeBar
{
    public string Debar(byte[] bar)
    {
        byte[] b = new byte[10];
        int temp = 256;  //cpValue��ֵ����Ĵ�С

        BCState bc = Jy.SetServer(LogInfo.Constant.SCDCC_IP, LogInfo.Constant.SCDCC_Port);
        bc = Jy.DecodeScdcc(bar, bar.Length);
		if (bc != BCState.BCSuccess)
		{
			return "JY|";
		}
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder(256);
        System.Text.StringBuilder sb1 = new System.Text.StringBuilder();

        //BCAttrType.BCTCustomize
        //ͨ������
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
                    throw new Exception("��ȡ�������");
                if (i == 0)
                {
                    if (sb.ToString().Length == 2)//2λ
                    {
                        sb1.Append(sb.ToString());
                    }
                    else
                    {
                        throw new Exception("��ȡ�������");
                    }
                }
                if (i == 1)
                {
                    if (sb.ToString().Length == 8)//8λ
                    {
                        sb1.Append(sb.ToString());
                    }
                    else
                    {
                        throw new Exception("��ȡ�������");
                    }
                }
                if (i == 2)
                {
                    if (sb.ToString().Length == 10)//10λ
                    {
                        sb1.Append(sb.ToString());
                    }
                    else
                    {
                        throw new Exception("��ȡ�������");
                    }
                }
                if (i == 3)
                {
                    if (sb.ToString().Length == 14)//14λ
                    {
                        sb1.Append(sb.ToString());
                    }
                    else
                    {
                        throw new Exception("��ȡ�������");
                    }
                }
                if (i == 4)
                {
                    if (sb.ToString().Length == 1)//1λ
                    {
                        sb1.Append(sb.ToString());
                    }
                    else
                    {
                        throw new Exception("��ȡ�������");
                    }
                }
            }

            sb1.Append("&");
            //ר������
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
                    throw new Exception("��ȡ�������");
                }
            }
            sb1.Append("&");
            //�Զ�������
            for (int i = 1; i < 5; ++i)
            {
                sb.Remove(0, sb.Length);
                bc = Jy.GetAttribute(BCAttrType.BCTCustomize, i, sb, temp);  
                if (bc == BCState.BCErrGetParam || bc == BCState.BCSuccess)//������ȫʱ�����׳��쳣
                {
                    sb1.Append(DeBarNode.EncodeBar(i, sb.ToString()));
                    sb1.Append("|");
                }
                else
                {
                    throw new Exception("��ȡ�������"); 
                }
            }
        }
        catch (Exception ex)
        {
			LogInfo.Log.WriteInfo(LogInfo.LogType.Warnning, "����SCDCC�������" + ex.ToString());
		}
        sb1.Append("|");
        return sb1.ToString();
    }
}