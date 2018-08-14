using System;
using System.Runtime.InteropServices;

namespace BoxDataParse
{
	[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack=1)]
	public struct CMDHeader
	{
		[FieldOffset(0)]
        public byte rpt_code;	// 命令号
		[FieldOffset(1)]
        public byte bn_m0;	    //BN号的m0位，0-99,分箱号
		[FieldOffset(2)]
        public byte bn_m1;		//BN号的m1位，0-99,主箱号
		[FieldOffset(3)]
        public byte bn_m2;      //BN号的m2位，0-9,主箱号高位扩展
	}

	[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack=1)]
    public struct tag_cmd_presend
	{
        [FieldOffset(0)]
        public byte type; //投箱类型。0：等待触发光电遮挡后打开门禁，1：直接打开门禁
		[FieldOffset(1)]
        public byte style;  //显示模式。
		[FieldOffset(2)]
        public UInt16 shownum;			//显示数字
		[FieldOffset(4)]
        public UInt32 reserved2;
	}

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct tag_cmd_outtext
    {
        /// <summary>
        /// 预先定义好的位置的索引值
        /// 0 单位简称，一般6个汉字  TEXT_SHORTNAME
        /// 1 箱格号显示，3个数字    TEXT_BOXNUMBER
        /// 2 单位详细名称，一般16个汉字 TEXT_LONGNAME
        /// 3 告警显示板0，不限汉字个数  TEXT_WARNINGMSG0
        /// 4 告警显示板1，不限汉字个数  TEXT_WARNINGMSG1
        /// 5 告警显示板2，不限汉字个数  TEXT_WARNINGMSG2
        /// 6+ 待定
        /// </summary>
        [FieldOffset(0)]
        public byte which_block;  //文字块位置 
        /// <summary>
        /// 显示类型。0：显示本地文本，1：设置本地文本，2：显示附带的文本
        /// </summary>
        [FieldOffset(1)]
        public byte text_type;    //显示类型。0：显示本地文本，1：设置本地文本，2：显示附带的文本
        /// <summary>
        /// 本地文字索引。在显示附带的文本时候无效。
        /// </summary>
        [FieldOffset(2)]
        public byte text_index;   //本地文字索引。在显示附带的文本时候无效。
        /// <summary>
        /// 文字显示时间，以秒为单位。1-254为显示时间，0为取消显示，255为永久显示。
        /// </summary>
        [FieldOffset(3)]
        public byte show_time;   //文字显示时间，以秒为单位。1-254为显示时间，0为取消显示，255为永久显示。
        /// <summary>
        /// 文字块的长度，单位字节
        /// </summary>
        [FieldOffset(4)]
        public UInt16 text_len;   //文字块的长度，单位字节
        /// <summary>
        /// 文字的左上角横坐标，0-255
        /// </summary>
        [FieldOffset(6)]
        public byte x;
        /// <summary>
        /// 文字的左上角纵坐标，0-255
        /// </summary>
        [FieldOffset(7)]
        public byte y;

        [FieldOffset(8)]
        public fixed byte content[256]; //文字的内容，127个汉字最多，字符串属性：以text_len指定的长度       // 汉字请按GB18030编码
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct tag_cmd_outtext_4Dai
    {
        /// <summary>
        /// 预先定义好的位置的索引值
        /// 0 单位简称，一般6个汉字  TEXT_SHORTNAME
        /// 1 箱格号显示，3个数字    TEXT_BOXNUMBER
        /// 2 单位详细名称，一般16个汉字 TEXT_LONGNAME
        /// 3 告警显示板0，不限汉字个数  TEXT_WARNINGMSG0
        /// 4 告警显示板1，不限汉字个数  TEXT_WARNINGMSG1
        /// 5 告警显示板2，不限汉字个数  TEXT_WARNINGMSG2
        /// 6+ 待定
        /// </summary>
        [FieldOffset(0)]
        public byte which_block;  //文字块位置 
        [FieldOffset(1)]
        public byte text_font;    //0 默认字体，一般是宋体，  TEXT_FONT_DFFAULT 1 宋体， TEXT_FONT_SONG 2 黑体，TEXT_FONT_HEI
        [FieldOffset(2)]
        public byte text_size;   //0 默认大小，一般是48P，  TEXT_SIZE_DEFAULT 1 自适应大小， TEXT_SIZE_AUTO 2 16p点阵，TEXT_SIZE_16P 3 48p点阵， TEXT_SIZE_48P 4 96p点阵，TEXT_SIZE_96P 5+ 带添加
         [FieldOffset(3)]
        public byte text_color;   //字体前景，背景颜色的索引值 0 默认颜色，一般指前景就是高对比度，指背景就是黑色，  TEXT_COLOR_DEFAULT
        [FieldOffset(4)]
        public byte show_time;   //文字显示时间，以秒为单位。1-254为显示时间，0为取消显示，255为永久显示。
        [FieldOffset(5)]
        public byte text_fx;     //文字块特效
        [FieldOffset(6)]
        public byte text_len;   //文字块的长度，单位字节
        [FieldOffset(7)]
        public byte text_Align; //0:左对齐，1:居中，2:右对齐

        [FieldOffset(8)]
        public fixed byte content[256]; //文字的内容，127个汉字最多，字符串属性：以text_len指定的长度       // 汉字请按GB18030编码
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct tag_cmd_Sound
    {
        /// <summary>
        /// 类型。0：播放本地语音，1：设置本地语音，2：播放附带的文本
        /// </summary>
        [FieldOffset(0)]
        public byte tts_set; //类型。0：播放本地语音，1：设置本地语音，2：播放附带的文本
        /// <summary>
        /// 本地保存的语音信息的索引。在播放附带文本时候无效。
        /// </summary>
        [FieldOffset(1)]
        public byte tts_index; //本地保存的语音信息的索引。在播放附带文本时候无效。
        /// <summary>
        /// 附带文字内容长度，字节单位的长度
        /// </summary>
        [FieldOffset(2)]
        public UInt16 text_len; //附带文字内容长度，字节单位的长度
        /// <summary>
        /// 
        /// </summary>
        [FieldOffset(4)]
        public UInt32 reserved2;

        [FieldOffset(8)]
        public fixed byte content[32]; //最多16个汉字的语音
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit, Pack = 1)]
    public unsafe struct tag_cmd_TempletList
    {
        /// <summary>
        /// 标题的个数，每个标题占16字节
        /// </summary>
        [FieldOffset(0)]
        public byte title_count;   //标题的个数，每个标题占16字节，
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
        /// 文字的内容，每16个字节，8个汉字为一个标题。
        /// 16个。汉字请按GB18030编码
        /// </summary>
        [FieldOffset(8)]
        public fixed byte content[256];
        // 
    }
}
