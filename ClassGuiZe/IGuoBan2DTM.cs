using System;
using System.Collections.Generic;
using System.Text;

namespace ClassGuiZe
{
    public interface IGuoBan2DTM
    {
        /// <param name="BanBenBS">�汾��ʶ</param>
        /// <param name="TiaoMaBH">������</param>
        /// <param name="ChengWenRQ">��������</param>
        /// <param name="FaWenDW">���ĵ�λ</param>
        /// <param name="WenZhong">���ֻ���</param>
        /// <param name="WenHao">�ĺŻ��ں�</param>
        /// <param name="MiMiDG">���ܵȼ�</param>
        /// <param name="JinJiCD">�����̶�</param>
        /// <param name="WenJianBT">�ļ�����</param>
        /// <param name="ZhuSongDW">���͵�λ</param>
        /// <param name="FaBuCC">�������</param>
        /// <param name="TiaoMaZZDW">����������λ</param>
        /// <param name="TiaoMaZZRQ">������������</param>
        /// <param name="s">�ĺŻ��ں�</param>
        /// <param name="d">���ֻ���</param>
        string ShengCheng2DTM(string BanBenBS, string TiaoMaBH, string ChengWenRQ, string FaWenDW, string WenZhong, string WenHao, string MiMiDG, string JinJiCD, string WenJianBT, string ZhuSongDW, string FaBuCC, string TiaoMaZZDW, string TiaoMaZZRQ);

        /// <param name="GuoBanTMDX">�����������</param>
        string ShengCheng2DTM(GuoBanTM GuoBanTMDX);

        /// <param name="TiaoMaZ">����ֵ</param>
        GuoBanTM JieXi2DTM(string TiaoMaZ);
    }
}
