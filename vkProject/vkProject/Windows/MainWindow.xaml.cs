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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace vkProject
{

	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			WebGetter brouser = new WebGetter();
			brouser.ShowDialog();
			access_token = brouser.access_token;
			user_id = brouser.user_id;
		}
		string  access_token;
		uint     user_id;

		private void button_Click(object sender, RoutedEventArgs e)
		{
			VkAPI.vkAPI d = new VkAPI.vkAPI(access_token, user_id, new VkAPI.Scope() { friends = true, wall = true });
            List<VkAPI.User> arr = d.getFriends();
			foreach (var k in arr)
			{
				listBox.Items.Add(k.User_id);
			}
		}
	}
}
