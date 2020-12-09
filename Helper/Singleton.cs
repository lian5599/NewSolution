using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
	public  class Singleton<T> where T : class,new()
	{
		private static volatile T _instance = null;
		private static readonly object _locker = new object();
		public static T GetInstance()
		{
			if (_instance == null)
			{
				lock (_locker)
				{
					if (_instance == null)
					{
						_instance =  Activator.CreateInstance<T>();
					}
				}
			}
			return _instance;
		}
	}

}
