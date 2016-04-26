using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkAPI;

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
			//WebGetter brouser = new WebGetter();
			//brouser.ShowDialog();
			//access_token = brouser.access_token;
			//user_id = brouser.user_id;
		}
		string  access_token;
		int     user_id;

		private void button_Click(object sender, RoutedEventArgs e)
		{
			//Parse_Vk_Output vk = new Parse_Vk_Output(new vkAPI(access_token, user_id, new Scope { wall = true, friends = true }));
			//vk.getFriends();
			//vk.getWall();
			//vk.getLikes();
			post1.AddPoll(new VkAPI.Controls.ctrPoll(new VkAPI.Media.Poll() { Answers = new List<VkAPI.Media.Answer>() { new VkAPI.Media.Answer() { Text = "вариант1", Id = 0, Rate = 44, Votes = 44 }, new VkAPI.Media.Answer() { Text = "вариант2", Rate = 93, Id = 1, Votes = 3 } }, Answer_id = 0, Question="вопрос" }));
        }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			
			if (Global.temporary != null)
			{
				foreach(var file in Global.temporary)
					File.Delete(file);
			}
		}
	}
}
