using System.Collections.Generic;
using System.IO;
using System.Text;
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
		public static bool debug = true;

		public static void WriteLogString(string mes)
		{
			byte[] bytes = new UTF8Encoding(true).GetBytes(mes + "\r\n");
			logStream.Write(bytes, 0, bytes.Length);
		}
		public static void WriteLog(string mes)
		{
			byte[] bytes = new UTF8Encoding(true).GetBytes(mes);
			logStream.Write(bytes, 0, bytes.Length);
		}

		private static  uint _temp_name = 0;
		public static string logFile = Environment.CurrentDirectory + @"\log.txt";
		public static FileStream logStream;
	}
}