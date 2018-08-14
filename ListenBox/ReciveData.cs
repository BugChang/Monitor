using System;

namespace ListenBox
{
	/// <summary>
	/// ReciveData 的摘要说明。
	/// </summary>
	public class ReciveData
	{
		public byte[]	buf;
		public int		buflen;

		public ReciveData()
		{
			this.buf = new byte[2048];
			buflen = 0;
		}

	}
}