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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Drawing;
using System.Threading.Tasks;
using vkProject.Controls;
using VkAPI;
using VkAPI.Controls;

namespace vkProject
{

	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			RefreshingHL = new HoverLoading();
			RefreshingHL.Text = "Обновление";

			RefreshingLayoutHL = new HoverLoading();
			RefreshingLayoutHL.Text = "Обновление записей";

			RefreshHB = new HoverButton();
			RefreshHB.Text = "Обновить";
			RefreshHB.MouseLeftButtonUp += RefreshBt_MouseLeftButtonUp;

			StatRefreshHB = new HoverButton();
			StatRefreshHB.Text = "Обновить";
			StatRefreshHB.MouseLeftButtonUp += StatRefreshHB_MouseLeftButtonUp;

			StatRefreshingHL = new HoverLoading();
			StatRefreshingHL.Text = "Обновление";
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            WebGetter brouser = new WebGetter();
            brouser.ShowDialog();

            access_token = brouser.access_token;
            user_id = brouser.user_id;
			Vk = new Parse_Vk_Output(new vkAPI(access_token, user_id));

			postButton.Children.Add(RefreshHB);
			statButtons.Children.Add(StatRefreshHB);
		}
		private void StatRefreshHB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			statistic.Children.Clear();
			statButtons.Children.Remove(sender as HoverButton);
			StatRefreshingHL.LoadWheelRotateBegin();
			statButtons.Children.Add(StatRefreshingHL);
			Task.Factory.StartNew(getStatistic);
		}
		private void RefreshBt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			posts.Children.RemoveRange(1, posts.Children.Count - 1);
			postButton.Children.Remove(sender as HoverButton);
			RefreshingHL.LoadWheelRotateBegin();
			postButton.Children.Add(RefreshingHL);
			Task.Factory.StartNew(StartPreLoadWall);
		}
		private void getStatistic()
        {
			if (Wall == null)
			{
				var Posts = Vk.getWall();
				Wall = Posts.Value;
				users = Posts.Key;
			}

            var whoLiked = Vk.getLikes(Wall);
			VkAPI.Media.Poll LikeStat = new VkAPI.Media.Poll();

			int sum = 0;
			foreach(var ans in whoLiked)
				sum += ans.Key;

			foreach(var ans in whoLiked)
				LikeStat.Answers.Add(new VkAPI.Media.Answer() { Rate = 100*ans.Key/sum, Text = String.Format("{0} {1}", ans.Value.First_name, ans.Value.Last_name), Votes = Convert.ToUInt32(ans.Key) });
        }
		private void getStatisticComplite()
		{

		}
		private void tb_stat_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			tb_posts.Checked = false;
			tb_stat.Checked = true;
			postViewer.Visibility = Visibility.Hidden;
			statViewer.Visibility = Visibility.Visible;
		}
		private void tb_posts_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			tb_posts.Checked = true;
			tb_stat.Checked = false;
			postViewer.Visibility = Visibility.Visible;
			statViewer.Visibility = Visibility.Hidden;
		}
		private void mainwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Global.temporary != null)
			{
				foreach(var file in Global.temporary)
					File.Delete(file);
			}
		}
		private void StartPreLoadWall()
		{
			var arr = Vk.getWall();
			users = arr.Key;
			Wall = arr.Value;
			Task.Factory.StartNew(EndPreLoadWall);
		}
		private void EndPreLoadWall()
		{
			Task.Factory.StartNew(StartLayoutDesign);
		}
		private void StartLayoutDesign()
		{
			int outed = 0;

			Dispatcher.Invoke(() => RefreshingHL.LaodWheelRotateStop());
			Dispatcher.Invoke(() => postButton.Children.RemoveAt(0));
			Dispatcher.Invoke(() => RefreshingLayoutHL.LoadWheelRotateBegin());
			Dispatcher.Invoke(() => postButton.Children.Add(RefreshingLayoutHL));
			Dispatcher.Invoke(() => countPost.Visibility = Visibility.Visible);
			Dispatcher.Invoke(() => countPost.Text = String.Format("{0}/{1}", outed, Wall.Count));


			foreach(var item in Wall)
			{
				User curuser;
				curuser = users[item.Owner_id];
				User curRepUser;
				if(item.Copied_Post != null)
					curRepUser = users[Math.Abs(item.Copied_Post.Owner_id)];
				else
					curRepUser = users[item.From_id];

				Dispatcher.Invoke(() => posts.Children.Add(new ctrPost(item, curuser, curRepUser)));

				++outed;
				Dispatcher.Invoke(() => countPost.Text = String.Format("{0}/{1}", outed, Wall.Count));
				Thread.Sleep(50);

			}

			Task.Factory.StartNew(EndLayoutDesing);
		}
		private void EndLayoutDesing()
		{
			Dispatcher.Invoke(() => RefreshingLayoutHL.LaodWheelRotateStop());
			Dispatcher.Invoke(() => postButton.Children.RemoveAt(0));
			Dispatcher.Invoke(() => postButton.Children.Add(RefreshHB));
			Dispatcher.Invoke(() => countPost.Visibility = Visibility.Hidden);
		}

		private HoverLoading RefreshingHL;
		private HoverLoading RefreshingLayoutHL;
		private HoverLoading StatRefreshingHL;
		private HoverButton RefreshHB;
		private HoverButton StatRefreshHB;
		private Dictionary<int, User> users;
		private List<Post> Wall;
		private string  access_token;
		private int     user_id;
		private Parse_Vk_Output Vk;
	}
}