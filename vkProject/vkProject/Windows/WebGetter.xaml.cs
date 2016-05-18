using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Configuration;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace vkProject
{
	/// <summary>
	/// Логика взаимодействия для WebGetter.xaml
	/// </summary>
	public partial class WebGetter : Window
	{
		public WebGetter()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			brouser.Source = new Uri("https://oauth.vk.com/authorize?client_id=" +
									  ConfigurationManager.AppSettings["client_id"] + '&' +
									  "redirect_uri=https://oauth.vk.com/blank.html&display=page&scope=wall,friends&revoke=1&response_type=token&v=5.50");
		}
		private void brouser_LoadCompleted(object sender, NavigationEventArgs e)
		{
			if(e.Uri.ToString().IndexOf("access_token") != -1)
			{
				string[] data = e.Uri.ToString().Split(new char[] { '=', '&' }); // data[0] = "api.vk.com/....#access_token", data[1] = access_token, data[2] = "expires_in"
				access_token = data[1];                                          // data[3] = expires_in, data[4] = "user_id", data[5] = user_id
				user_id = Convert.ToInt32(data[5]);
				expires_in = Convert.ToInt32(data[3]);
				Close();
			}
		}

		public string access_token { get; private set; }
		public int user_id { get; private set; }
		public int expires_in { get; private set; }
	}
}