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

			ShowBefore = new HoverButton();
			ShowBefore.MouseLeftButtonUp += ShowBefore_MouseLeftButtonUp;
			ShowAfter = new HoverButton();
			ShowAfter.MouseLeftButtonUp += ShowAfter_MouseLeftButtonUp;
		}

		private void ShowAfter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			int begin = postEnd;
			int end = Math.Min(begin+defaultcount, Wall.Count);
			Task.Factory.StartNew(() => StartShowPosts(begin, end));
		}
		private void ShowBefore_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			int end = postBegin;
			int begin = Math.Max(end-defaultcount, 0);
			Task.Factory.StartNew(() => StartShowPosts(begin, end));
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            WebGetter brouser = new WebGetter();
            brouser.ShowDialog();

            access_token = brouser.access_token;
            user_id = brouser.user_id;
			Vk = new ParseVkOutput(new vkAPI(access_token, user_id));

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
			ShowAfterPanel.Children.Remove(ShowAfter);
			ShowBeforePanel.Children.Remove(ShowBefore);
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
			postEnd = Math.Min(Wall.Count, defaultcount);
			Task.Factory.StartNew(() => EndPreLoadWall(postBegin, postEnd));
		}
		private void EndPreLoadWall(int begin, int end)
		{
			Dispatcher.Invoke(() =>
			{
				RefreshingHL.LaodWheelRotateStop();
				postButton.Children.Remove(RefreshingHL);
			});
			Task.Factory.StartNew(() => StartShowPosts(begin, end));
		}

		private void StartShowPosts(int begin, int end)
		{
			Dispatcher.Invoke(() => 
			{
				postButton.Children.Remove(RefreshHB);
				ShowAfterPanel.Children.Remove(ShowAfter);
				ShowBeforePanel.Children.Remove(ShowBefore);

				RefreshingLayoutHL.LoadWheelRotateBegin();
				postButton.Children.Add(RefreshingLayoutHL);
				posts.Children.RemoveRange(1, posts.Children.Count - 1);
			});

			ShowPosts(begin, end);
		}
		private void ShowPosts(int begin, int end)
		{
			int outed = 0;
			Dispatcher.Invoke(() => countPost.Visibility = Visibility.Visible);
			Dispatcher.Invoke(() => countPost.Text = String.Format("{0}/{1}", outed, end - begin));
			for(int i = begin; i != end; ++i)
			{
				User curuser;
				curuser = users[Wall[i].Owner_id];
				User curRepUser = null;
				if(Wall[i].Copied_Post != null)
					curRepUser = users[Math.Abs(Wall[i].Copied_Post.Owner_id)];

				++outed;
				Dispatcher.Invoke(() => posts.Children.Add(new ctrPost(Wall[i], curuser, curRepUser)));
				Dispatcher.Invoke(() => countPost.Text = String.Format("{0}/{1}", outed, end - begin));
				Thread.Sleep(50);
			}
			EndShowPosts(begin, end);
		}
		private void EndShowPosts(int begin, int end)
		{
			Dispatcher.Invoke(() =>
			{
				countPost.Visibility = Visibility.Hidden;
				RefreshingLayoutHL.LaodWheelRotateStop();
				postButton.Children.Remove(RefreshingLayoutHL);
				postButton.Children.Add(RefreshHB);
				postBegin = begin;
				postEnd = end;
			});
			

			if(postEnd != Wall.Count)
			{
				Dispatcher.Invoke(() => ShowAfter.Text = "Показать следуюущие ↓");
				Dispatcher.Invoke(() => ShowAfterPanel.Children.Add(ShowAfter));
			}
			if(postBegin != 0)
			{
				Dispatcher.Invoke(() => ShowBefore.Text = "Показать предыдущие ↑");
				Dispatcher.Invoke(() => ShowBeforePanel.Children.Add(ShowBefore));
			}
		}

		private HoverLoading RefreshingHL;
		private HoverLoading RefreshingLayoutHL;
		private HoverLoading StatRefreshingHL;
		private HoverButton RefreshHB;
		private HoverButton StatRefreshHB;
		private HoverButton ShowBefore;
		private HoverButton ShowAfter;
		private Dictionary<int, User> users;
		private List<Post> Wall;
		private string  access_token;
		private int     user_id;
		private ParseVkOutput Vk;
		private int defaultcount = 1;
		private int postBegin = 0;
		private int postEnd = 0;
	}
}