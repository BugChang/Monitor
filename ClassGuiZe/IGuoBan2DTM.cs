using System;
using System.Collections.Generic;
using System.Text;

namespace ClassGuiZe
{
    public interface IGuoBan2DTM
    {
        /// <param name="BanBenBS">版本标识</param>
        /// <param name="TiaoMaBH">条码编号</param>
        /// <param name="ChengWenRQ">成文日期</param>
        /// <param name="FaWenDW">发文单位</param>
        /// <param name="WenZhong">文种或刊名</param>
        /// <param name="WenHao">文号或期号</param>
        /// <param name="MiMiDG">秘密等级</param>
        /// <param name="JinJiCD">紧急程度</param>
        /// <param name="WenJianBT">文件标题</param>
        /// <param name="ZhuSongDW">主送单位</param>
        /// <param name="FaBuCC">发布层次</param>
        /// <param name="TiaoMaZZDW">条码制作单位</param>
        /// <param name="TiaoMaZZRQ">条码制作日期</param>
        /// <param name="s">文号或期号</param>
        /// <param name="d">文种或刊名</param>
        string ShengCheng2DTM(string BanBenBS, string TiaoMaBH, string ChengWenRQ, string FaWenDW, string WenZhong, string WenHao, string MiMiDG, string JinJiCD, string WenJianBT, string ZhuSongDW, string FaBuCC, string TiaoMaZZDW, string TiaoMaZZRQ);

        /// <param name="GuoBanTMDX">国办条码对象</param>
        string ShengCheng2DTM(GuoBanTM GuoBanTMDX);

        /// <param name="TiaoMaZ">条码值</param>
        GuoBanTM JieXi2DTM(string TiaoMaZ);
    }
}
