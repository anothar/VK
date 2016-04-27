using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Threading;
using System.Threading.Tasks;
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
            WebGetter brouser = new WebGetter();
            brouser.ShowDialog();
            access_token = brouser.access_token;
            user_id = brouser.user_id;
        }
		string  access_token;
		int     user_id;
        private void getStatistic()
        {
            Parse_Vk_Output vk = new Parse_Vk_Output(new vkAPI(access_token, user_id, new Scope { wall = true, friends = true }));
            Dispatcher.Invoke((Action)(() => textBlock.Text += "Получение списка друзей\n"));
            var Friends = vk.getFriends();
            Dispatcher.Invoke((Action)(() => textBlock.Text += "Получение записей стены\n"));
            var Wall = vk.getWall();
            Dispatcher.Invoke((Action)(() => textBlock.Text += "Получение лайков\n"));
            var whoLiked = vk.getLikes(Wall);
            Dispatcher.Invoke((Action)(() => textBlock.Text += "Готово\n\n"));
 
            foreach (var item in whoLiked)
                Dispatcher.Invoke((Action)(() => textBlock.Text += String.Concat(item.Value.First_name, " ", item.Value.Last_name, " ", item.Key.ToString(), '\n')));
        }

		private void button_Click(object sender, RoutedEventArgs e)
		{
            Task.Factory.StartNew(getStatistic);
            //post1.AddPoll(new VkAPI.Controls.ctrPoll(new VkAPI.Media.Poll() { Answers = new List<VkAPI.Media.Answer>() { new VkAPI.Media.Answer() { Text = "gdfgsdfgs", Id = 0, Rate = 44, Votes = 44 }, new VkAPI.Media.Answer() { Text = "fdfdаыфврпафыоафылафыолваполывпалофыпварфывпадфываолывапвоаывпаволаыфваафывафывафывфаывafsa", Rate = 33, Id = 1, Votes = 3 } }, Answer_id=1, Question="gdfsgdfgsdfhgffdgdfshdfghdfsgfdsgdfgdg" }));
        }

		private void tb_stat_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			tb_posts.Checked = false;
			tb_stat.Checked = true;
		}
		
		private void tb_posts_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
			{
			tb_posts.Checked = true;
			tb_stat.Checked = false;
		}

		private void mainwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Global.temporary != null)
			{
				foreach(var file in Global.temporary)
					File.Delete(file);
			}
		}
	}
}