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
			Vk = new Parse_Vk_Output(new vkAPI(access_token, user_id, new Scope() { wall = true, friends = true }));
        }
		string  access_token;
		int     user_id;

        private void getStatistic()
        {
			Global.WriteLogString("Statistic had been called...");
            var Friends = Vk.getFriends();
            var Wall = Vk.getWall();
            var whoLiked = Vk.getLikes(Wall);
         }
        private void getWall()
        {
            Global.WriteLogString("getWall had been called");
            var Wall = Vk.getWall();
            foreach (var item in Wall)
            {
                Dispatcher.Invoke((Action)(() => posts.Children.Add(new ctrPost(item))));
            }
        }
		private Parse_Vk_Output Vk;
		private void tb_stat_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			//Task.Factory.StartNew(getStatistic);
			tb_posts.Checked = false;
			tb_stat.Checked = true;
		}
		private void tb_posts_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
            if (!state)
            {
                Task.Factory.StartNew(getWall);
                state = true;
            }
			tb_posts.Checked = true;
			tb_stat.Checked = false;

		}
        bool state = false; //зачем?
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