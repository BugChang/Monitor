using System;

namespace ListenBox
{
	/// <summary>
	/// ReciveData ��ժҪ˵����
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