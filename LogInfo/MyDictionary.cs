using System;
using System.Collections;

namespace LogInfo
{
	/// <summary>
	/// MyDictionary 的摘要说明。
	/// 字典类
	/// </summary>
	public class MyDictionary : DictionaryBase  
	{
		public MyDictionary()
		{
		}

		public void setUsed(bool b)
		{
			if(b)
			{
				System.Threading.Monitor.Enter(this);
			}
			else
				System.Threading.Monitor.Exit(this);
		}

		public object this[ String key ]  
		{
			get  
			{
				return( (object) Dictionary[key] );
			}
			set  
			{
				Dictionary[key] = value;
			}
		}

		public ICollection Keys  
		{
			get  
			{
				return( Dictionary.Keys );
			}
		}

		public ICollection Values  
		{
			get  
			{
				return( Dictionary.Values );
			}
		}

		public void Add( String key, object value )  
		{
			Dictionary.Add( key, value );
		}

		public bool Contains( String key )  
		{
			return( Dictionary.Contains( key ) );
		}

		public void Remove( String key )  
		{
			Dictionary.Remove( key );
		}

	}

}
