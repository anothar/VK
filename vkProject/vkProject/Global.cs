using System.Collections.Generic;
using System;

namespace vkProject
{
	public static class Global
	{
		public static string temporary_name
		{
			get
			{
				++_temp_name;
				return _temp_name.ToString();
			}
		}
		public static List<string> temporary = new List<string>();
		private static  uint _temp_name = 0;
	}
}