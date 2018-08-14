using System;
using System.Runtime.InteropServices;

namespace BoxDataParse
{
	[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack=1)]
	public struct CMDHeader
	{
		[FieldOffset(0)]
        public byte rpt_code;	// �����
		[FieldOffset(1)]
        public byte bn_m0;	    //BN�ŵ�m0λ��0-99,�����
		[FieldOffset(2)]
        public byte bn_m1;		//BN�ŵ�m1λ��0-99,�����
		[FieldOffset(3)]
        public byte bn_m2;      //BN�ŵ�m2λ��0-9,����Ÿ�λ��չ
	}

	[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack=1)]
    public struct tag_cmd_presend
	{
        [FieldOffset(0)]
        public byte type; //Ͷ�����͡�0���ȴ���������ڵ�����Ž���1��ֱ�Ӵ��Ž�
		[FieldOffset(1)]
        public byte style;  //��ʾģʽ��
		[FieldOffset(2)]
        public UInt16 shownum;			//��ʾ����
		[FieldOffset(4)]
        public UInt32 reserved2;
	}

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct tag_cmd_outtext
    {
        /// <summary>
        /// Ԥ�ȶ���õ�λ�õ�����ֵ
        /// 0 ��λ��ƣ�һ��6������  TEXT_SHORTNAME
        /// 1 ������ʾ��3������    TEXT_BOXNUMBER
        /// 2 ��λ��ϸ���ƣ�һ��16������ TEXT_LONGNAME
        /// 3 �澯��ʾ��0�����޺��ָ���  TEXT_WARNINGMSG0
        /// 4 �澯��ʾ��1�����޺��ָ���  TEXT_WARNINGMSG1
        /// 5 �澯��ʾ��2�����޺��ָ���  TEXT_WARNINGMSG2
        /// 6+ ����
        /// </summary>
        [FieldOffset(0)]
        public byte which_block;  //���ֿ�λ�� 
        /// <summary>
        /// ��ʾ���͡�0����ʾ�����ı���1�����ñ����ı���2����ʾ�������ı�
        /// </summary>
        [FieldOffset(1)]
        public byte text_type;    //��ʾ���͡�0����ʾ�����ı���1�����ñ����ı���2����ʾ�������ı�
        /// <summary>
        /// ������������������ʾ�������ı�ʱ����Ч��
        /// </summary>
        [FieldOffset(2)]
        public byte text_index;   //������������������ʾ�������ı�ʱ����Ч��
        /// <summary>
        /// ������ʾʱ�䣬����Ϊ��λ��1-254Ϊ��ʾʱ�䣬0Ϊȡ����ʾ��255Ϊ������ʾ��
        /// </summary>
        [FieldOffset(3)]
        public byte show_time;   //������ʾʱ�䣬����Ϊ��λ��1-254Ϊ��ʾʱ�䣬0Ϊȡ����ʾ��255Ϊ������ʾ��
        /// <summary>
        /// ���ֿ�ĳ��ȣ���λ�ֽ�
        /// </summary>
        [FieldOffset(4)]
        public UInt16 text_len;   //���ֿ�ĳ��ȣ���λ�ֽ�
        /// <summary>
        /// ���ֵ����ϽǺ����꣬0-255
        /// </summary>
        [FieldOffset(6)]
        public byte x;
        /// <summary>
        /// ���ֵ����Ͻ������꣬0-255
        /// </summary>
        [FieldOffset(7)]
        public byte y;

        [FieldOffset(8)]
        public fixed byte content[256]; //���ֵ����ݣ�127��������࣬�ַ������ԣ���text_lenָ���ĳ���       // �����밴GB18030����
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct tag_cmd_outtext_4Dai
    {
        /// <summary>
        /// Ԥ�ȶ���õ�λ�õ�����ֵ
        /// 0 ��λ��ƣ�һ��6������  TEXT_SHORTNAME
        /// 1 ������ʾ��3������    TEXT_BOXNUMBER
        /// 2 ��λ��ϸ���ƣ�һ��16������ TEXT_LONGNAME
        /// 3 �澯��ʾ��0�����޺��ָ���  TEXT_WARNINGMSG0
        /// 4 �澯��ʾ��1�����޺��ָ���  TEXT_WARNINGMSG1
        /// 5 �澯��ʾ��2�����޺��ָ���  TEXT_WARNINGMSG2
        /// 6+ ����
        /// </summary>
        [FieldOffset(0)]
        public byte which_block;  //���ֿ�λ�� 
        [FieldOffset(1)]
        public byte text_font;    //0 Ĭ�����壬һ�������壬  TEXT_FONT_DFFAULT 1 ���壬 TEXT_FONT_SONG 2 ���壬TEXT_FONT_HEI
        [FieldOffset(2)]
        public byte text_size;   //0 Ĭ�ϴ�С��һ����48P��  TEXT_SIZE_DEFAULT 1 ����Ӧ��С�� TEXT_SIZE_AUTO 2 16p����TEXT_SIZE_16P 3 48p���� TEXT_SIZE_48P 4 96p����TEXT_SIZE_96P 5+ �����
         [FieldOffset(3)]
        public byte text_color;   //����ǰ����������ɫ������ֵ 0 Ĭ����ɫ��һ��ָǰ�����Ǹ߶Աȶȣ�ָ�������Ǻ�ɫ��  TEXT_COLOR_DEFAULT
        [FieldOffset(4)]
        public byte show_time;   //������ʾʱ�䣬����Ϊ��λ��1-254Ϊ��ʾʱ�䣬0Ϊȡ����ʾ��255Ϊ������ʾ��
        [FieldOffset(5)]
        public byte text_fx;     //���ֿ���Ч
        [FieldOffset(6)]
        public byte text_len;   //���ֿ�ĳ��ȣ���λ�ֽ�
        [FieldOffset(7)]
        public byte text_Align; //0:����룬1:���У�2:�Ҷ���

        [FieldOffset(8)]
        public fixed byte content[256]; //���ֵ����ݣ�127��������࣬�ַ������ԣ���text_lenָ���ĳ���       // �����밴GB18030����
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct tag_cmd_Sound
    {
        /// <summary>
        /// ���͡�0�����ű���������1�����ñ���������2�����Ÿ������ı�
        /// </summary>
        [FieldOffset(0)]
        public byte tts_set; //���͡�0�����ű���������1�����ñ���������2�����Ÿ������ı�
        /// <summary>
        /// ���ر����������Ϣ���������ڲ��Ÿ����ı�ʱ����Ч��
        /// </summary>
        [FieldOffset(1)]
        public byte tts_index; //���ر����������Ϣ���������ڲ��Ÿ����ı�ʱ����Ч��
        /// <summary>
        /// �����������ݳ��ȣ��ֽڵ�λ�ĳ���
        /// </summary>
        [FieldOffset(2)]
        public UInt16 text_len; //�����������ݳ��ȣ��ֽڵ�λ�ĳ���
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(4)]
        public UInt32 reserved2;

        [FieldOffset(8)]
        public fixed byte content[32]; //���16�����ֵ�����
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct tag_cmd_TempletList
    {
        /// <summary>
        /// ����ĸ�����ÿ������ռ16�ֽ�
        /// </summary>
        [FieldOffset(0)]
        public byte title_count;   //����ĸ�����ÿ������ռ16�ֽڣ�
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(1)]
        public byte reserved0;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(2)]
        public UInt16 reserved1;
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(4)]
        public UInt32 reserved2;

        /// <summary>
        /// ���ֵ����ݣ�ÿ16���ֽڣ�8������Ϊһ�����⡣
        /// 16���������밴GB18030����
        /// </summary>
        [FieldOffset(8)]
        public fixed byte content[256];
        // 
    }
}
