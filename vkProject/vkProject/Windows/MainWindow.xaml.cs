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
using VkAPI.Controls;

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
			//token_starts = new DateTime(Environment.TickCount*10);
			//expires_in = new TimeSpan();
			//expires_in = TimeSpan.FromSeconds(brouser.expires_in);

			Vk = new Parse_Vk_Output(new vkAPI(access_token, user_id));
        }

		DateTime token_starts;
		TimeSpan expires_in;
		string  access_token;
		int     user_id;

        private void getStatistic()
        {
			Global.WriteLogString("Statistic have been called...");
			//проверить активность токена
			if(!check_valid_data())
			{
				Global.WriteLogString("Outdated data. Refreshing the data");
				Window_Loaded(this, null); //получить новый, если устарел
			}

			Global.WriteLogString("Data is fresh");
            var Friends = Vk.getFriends();
            var Wall = Vk.getWall();
            var whoLiked = Vk.getLikes(Wall.Value);
         }
        private void getWall()
        {
            Global.WriteLogString("getWall have been called");
            foreach (var item in Wall)
            {
				User curuser;
				if(item.Copied_Post != null)
					curuser = users[Math.Abs(item.Copied_Post.Owner_id)];
				else
					curuser = users[item.From_id];
                Dispatcher.Invoke((Action)(() => posts.Children.Add(new ctrPost(item) { User_name = String.Format("{0} {1}", curuser.First_name, curuser.Last_name), User_photo = curuser.Photo_50 })));
            }
        }
		private Parse_Vk_Output Vk;
		bool wall_loaded = false;
		bool stat_loaded = false;
		private void tb_stat_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			tb_posts.Checked = false;
			tb_stat.Checked = true;

			if(!stat_loaded) ;
			//сбор информации
		}
		private void tb_posts_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if(!wall_loaded)
            {
                Task.Factory.StartNew(getWall);
				wall_loaded = true;
            }
			Task load = new Task(start_load_wall);
			start_load_wall();
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
		private void start_load_wall()
		{
			KeyValuePair<Dictionary<int, User>, List<Post>> arr = Vk.getWall();
			users = arr.Key;
			Wall = arr.Value;
			getWall();
		}

		bool check_valid_data()
		{
			Global.WriteLogString("Checling data valid");
			return (new DateTime(Environment.TickCount * 10) - token_starts < expires_in - TimeSpan.FromSeconds(900));
		}

		//инфа
		User cur_user;
		Dictionary<int, User> users;
		List<Post> Wall;

		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			
		}
	}
}