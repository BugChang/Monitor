using System;
using System.Collections.Generic;
using System.Text;

namespace LogInfo.BoxSet
{
	public partial class BoxSetInfoClass
	{

		private string boxNOField;

		private string frontBoxBNField;

		private bool hasTwoLockField;

		private string backBoxBNField;

		/// <summary>
		/// 逻辑箱号
		/// </summary>
		public string BoxNO
		{
			get
			{
				return this.boxNOField;
			}
			set
			{
				this.boxNOField = value;
			}
		}

		/// <summary>
		/// 前BN号码
		/// </summary>
		public string FrontBoxBN
		{
			get
			{
				return this.frontBoxBNField;
			}
			set
			{
				this.frontBoxBNField = value;
			}
		}

		/// <summary>
		/// 是否有双面锁
		/// </summary>
		public bool HasTwoLock
		{
			get
			{
				return this.hasTwoLockField;
			}
			set
			{
				this.hasTwoLockField = value;
			}
		}

		/// <summary>
		/// 后面BN号码
		/// </summary>
		public string BackBoxBN
		{
			get
			{
				return this.backBoxBNField;
			}
			set
			{
				this.backBoxBNField = value;
			}
		}
	}
}
