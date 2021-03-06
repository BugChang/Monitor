using System;
using System.Collections.Generic;
using System.Text;

namespace LogInfo
{
	public enum ReceiveDataType
	{
		条码信息,
		证卡信息,
		条码信息_4Dai,
		证卡信息_4Dai,
		证卡信息_指静脉,
		证卡信息_燕南指静脉,
		投件投入,
		投件抽出,

		取件单位选择,
		门禁,
		门锁,
		光电,
		按键,
		主箱连接,
		主箱连接_Version,
		硬件状态,
		上报拍摄的照片,
		指静脉数据_传输,
		指静脉数据_写入,
		指静脉数据_验证,
		m3板卡升级命令
	}
#if false
	//old
	public enum SendDataType
	{
		准备投信 = 0x04,
		扫描头控制 = 0x31,
		摄像头控制 = 0x32,
		控制指示灯 = 0x41,
		控制数码管 = 0x42,
		门禁控制 = 0x43,
		电控锁控制 = 0x44,
		查询硬件状态 = 0x45,
		屏幕按钮控制 = 0x46,
		模板选择 = 0x47,
		取件单位选择 = 0xE4,
		取件平台界面控制 = 0x48,
		硬件设置 = 0x49,
		强制启动 = 0x4a,
		强制空闲 = 0x4b,
		提示文本 = 0x4c,
		提示文本串口屏幕 = 0x4d,
		屏幕控制 = 0x4e,
		语音提示 = 0x50,
		蜂鸣器控制 = 0x51,
		下发箱存控制 = 0x52,
		拍照指令 = 0x53,
		准备指静脉更新 = 0x60,
		指静脉数据传输 = 0x61,
		m3板卡升级 = 0xf0
	}
#else
	//new
	public enum SendDataType
	{
		准备投信 = 0x04,
		扫描头控制 = 0x31,
		摄像头控制 = 0x32,
		控制指示灯 = 0x41,
		控制数码管 = 0x42,
		门禁控制 = 0x43,
		电控锁控制 = 0x44,
		查询硬件状态 = 0x45,
		屏幕按钮控制 = 0x46,
		模板选择 = 0xE3,		//20131230 从0x47更改到0xE3
		取件单位选择 = 0xE4,
		取件平台界面控制 = 0x48,
		硬件设置 = 0x49,
		强制启动 = 0x4a,
		强制空闲 = 0x4b,
		提示文本 = 0xE0,		//20131230 从0x4c更改到0xE0
		提示文本串口屏幕 = 0xE1,	//20131230 从0x4d更改到0xE1
		屏幕控制 = 0x4e,
		语音提示 = 0xE2,		//20131230 从0x50更改到0xE2
		蜂鸣器控制 = 0x51,
		下发箱存控制 = 0x52,
		拍照指令 = 0x53,
		准备指静脉更新 = 0x60,
		指静脉数据传输 = 0x61,
		m3板卡升级 = 0xf0
	}
#endif

	/// <summary>
	/// led显示模式
	/// </summary>
	public enum enum_LedColor
	{
		关闭 = 0,
		绿色 = 1,
		红色 = 2,
		橙色 = 3,
		绿黑闪烁 = 4,
		红黑闪烁 = 5,
		橙黑闪烁 = 6,
		红绿闪烁 = 7,
		橙绿闪烁 = 8
	}
	/// <summary>
	/// 0常灭 1常亮 2闪烁
	/// </summary>
	public enum enum_LampStatus
	{
		灭 = 0,
		亮 = 1,
		闪烁 = 2
	}
	/// <summary>
	/// 文本显示模式，语音播放模式
	/// </summary>
	public enum enum_TextType
	{//显示类型。0：显示本地文本，1：设置本地文本，2：显示附带的文本
		显示本地文本 = 0,
		设置本地文本 = 1,
		显示附带的文本 = 2
	}
}
