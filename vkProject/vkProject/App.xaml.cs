using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows;

namespace vkProject
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			if(File.Exists(Global.logFile))
				File.Delete(Global.logFile);

			Global.logStream = File.OpenWrite(Global.logFile);
			Global.WriteLogString("Starting program...");
		}
	}
}
