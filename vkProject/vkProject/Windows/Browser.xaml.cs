﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace vkProject
{
	public partial class Browser : Window
	{
		public Browser(string url, string title)
		{
			InitializeComponent();
			player.Source = new Uri(url);
			Title = title;
		}
	}
}
