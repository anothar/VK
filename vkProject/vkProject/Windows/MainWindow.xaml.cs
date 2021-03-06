﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;
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
		#region Конструкторы
		public MainWindow()
		{
			InitializeComponent();

			RefreshingHL = new HoverLoading();
			RefreshingHL.Text = "Обновление";

			RefreshingLayoutHL = new HoverLoading();
			RefreshingLayoutHL.Text = "Обновление записей";

			RefreshHB = new HoverButton();
			RefreshHB.Text = "Обновить";
			PostButton = RefreshHB;
			RefreshHB.MouseLeftButtonUp += RefreshBt_MouseLeftButtonUp;

			StatRefreshHB = new HoverButton();
			StatRefreshHB.Text = "Обновить";
			StatButton = StatRefreshHB;
			StatRefreshHB.MouseLeftButtonUp += StatRefreshHB_MouseLeftButtonUp;

			StatRefreshingHL = new HoverLoading();
			StatRefreshingHL.Text = "Обновление";

			ShowBefore = new HoverButton();
			ShowBefore.Text = "Показать предыдущие ↑";
			ShowBeforePanel.Content = ShowBefore;
			ShowBeforePanel.Visibility = Visibility.Hidden;
			ShowBefore.MouseLeftButtonUp += ShowBefore_MouseLeftButtonUp;

			ShowAfter = new HoverButton();
			ShowAfter.Text = "Показать следуюущие ↓";
			ShowAfterPanel.Content = ShowAfter;
			ShowAfterPanel.Visibility = Visibility.Hidden;
			ShowAfter.MouseLeftButtonUp += ShowAfter_MouseLeftButtonUp;

			defaultcount = Convert.ToInt32(ConfigurationManager.AppSettings["defaultcount"]);

			Directory.CreateDirectory(Environment.CurrentDirectory + Global.CacheDirectory);
		}
		#endregion
		#region События
		/// <summary>
		/// Вызывается при загрузке главного окна
		/// </summary>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            WebGetter brouser = new WebGetter();
            brouser.ShowDialog();

            access_token = brouser.access_token;
            user_id = brouser.user_id;
			Vk = new ParseVkOutput(new vkAPI(access_token, user_id));
		}
		/// <summary>
		/// Вызывается при нажатии на кнопку "Показать следующие"
		/// </summary>
		private void ShowAfter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{			
			int begin = postEnd;
			int end = Math.Min(begin+defaultcount, Wall.Count);
			Task.Factory.StartNew(() => StartShowPosts(begin, end));
		}
		/// <summary>
		/// Вызывается при нажатии на кнопку "Показать предыдущие"
		/// </summary>
		private void ShowBefore_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			int end = postBegin;
			int begin = Math.Max(end-defaultcount, 0);
			Task.Factory.StartNew(() => StartShowPosts(begin, end));
		}
		/// <summary>
		/// Вызывается при нажатии на кнопку "Обновить" для статистики
		/// </summary>
		private void StatRefreshHB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			StatButton = null;
            stat.Clear();
			StatRefreshingHL.LoadWheelRotateBegin();
			StatButton = StatRefreshingHL;
			Task.Factory.StartNew(getStatistic);
		}
		/// <summary>
		/// Вызывается при нажатии на кнопку "Обновить" для записей
		/// </summary>
		private void RefreshBt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			posts.Children.Clear();
			PostButton = sender as HoverButton;
			ShowAfterPanel.Visibility = Visibility.Hidden;
			ShowBeforePanel.Visibility = Visibility.Hidden;
			RefreshingHL.LoadWheelRotateBegin();
			PostButton = RefreshingHL;
			Task.Factory.StartNew(StartPreLoadWall);
		}
		/// <summary>
		/// Вызывается при закрытии окна
		/// </summary>
		private void mainwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Global.temporary != null)
			{
				foreach(var file in Global.temporary)
					File.Delete(file);
			}
        }
        /// <summary>
        /// Вызывается при прокручивании стены
        /// </summary>
        private void postsStroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double x = _postsStroller.ScrollableHeight - e.VerticalOffset;
            if (x < 2000 && _postsStroller.ScrollableHeight > 2000)
                ShowAfter_MouseLeftButtonUp(null, null);
		}
		#endregion
		#region Методы
		/// <summary>
		/// Метод, начинающий загрузку и отображение статистики
		/// </summary>
		private void getStatistic()
        {
			if (Wall == null)
			{
				var Posts = Vk.getWall();
				Wall = Posts.Value;
				users = Posts.Key;
			}

            var whoLiked = Vk.getLikes(Wall);
            
            for(int i = 0; i < whoLiked.Count; ++i)
            {
                whoLiked[i].Value.Photo_50 = Vk.getUserPhoto(whoLiked[i].Value.User_id);
                Dispatcher.Invoke(() => stat.Add(whoLiked[i]));
            }
            getStatisticComplete();
        }
		/// <summary>
		/// Метод, завершающий загрузку и отображение статистики
		/// </summary>
		private void getStatisticComplete()
		{
            Dispatcher.Invoke(() =>
            {
				StatRefreshingHL.LaodWheelRotateStop();
                StatButton = StatRefreshHB;
            });
        }
		/// <summary>
		/// Метод, начинающий предварительную загрузку записей
		/// </summary>
		private void StartPreLoadWall()
		{
			var arr = Vk.getWall();
			users = arr.Key;
			Wall = arr.Value;
			if (postEnd == 0)
				postEnd = Math.Min(Wall.Count, defaultcount);
			Task.Factory.StartNew(() => EndPreLoadWall(postBegin, postEnd));
		}
		/// <summary>
		/// Метод, заверающий предварительную загрузку записей
		/// </summary>
		/// <param name="begin">Итератор на первую запись, которая должна быть отображена</param>
		/// <param name="end">Итератор на последнюю запись, котораю не должна быть отображена</param>
		private void EndPreLoadWall(int begin, int end)
		{
			Dispatcher.Invoke(() =>
			{
				RefreshingHL.LaodWheelRotateStop();
				PostButton = null;
			});
			Task.Factory.StartNew(() => StartShowPosts(begin, end));
		}
		/// <summary>
		/// Метод, начинающий отображение записей в окне
		/// </summary>
		/// <param name="begin">Итератор на первую запись, которая должна быть отображена</param>
		/// <param name="end">Итератор на последнюю запись, котораю не должна быть отображена</param>
		private void StartShowPosts(int begin, int end)
		{
			Dispatcher.Invoke(() => 
			{
				PostButton = null;
				ShowAfterPanel.Visibility = Visibility.Hidden;
				ShowBeforePanel.Visibility = Visibility.Hidden;

				RefreshingLayoutHL.LoadWheelRotateBegin();
				PostButton = RefreshingLayoutHL;
				//posts.Children.Clear();
				//postsStroller.ScrollToTop();
			});

			ShowPosts(begin, end);
		}
		/// <summary>
		/// Метод, отображающий записи в окне
		/// </summary>
		/// <param name="begin">Итератор на первую запись, которая должна быть отображена</param>
		/// <param name="end">Итератор на последнюю запись, котораю не должна быть отображена</param>
		private void ShowPosts(int begin, int end)
		{
			int outed = 0;
			//Dispatcher.Invoke(() => countPost.Visibility = Visibility.Visible);
			//Dispatcher.Invoke(() => countPost.Text = String.Format("{0}/{1}", outed, end - begin));
			for(int i = begin; i != end; ++i)
			{
				User curuser;
				curuser = users[Wall[i].From_id];
				User curRepUser = null;
				if(Wall[i].Copied_Post != null)
					curRepUser = users[Math.Abs(Wall[i].Copied_Post.Owner_id)];

				++outed;
				Dispatcher.Invoke(() => posts.Children.Add(new ctrPost(Wall[i], curuser, curRepUser)));
				//Dispatcher.Invoke(() => countPost.Text = String.Format("{0}/{1}", outed, end - begin));
				Thread.Sleep(50);
			}
			EndShowPosts(begin, end);
		}
		/// <summary>
		/// Метод, закнчивающий отображение записей в окне
		/// </summary>
		/// <param name="begin">Итератор на первую запись, которая должна быть отображена</param>
		/// <param name="end">Итератор на последнюю запись, которая не должна быть отображена</param>
		private void EndShowPosts(int begin, int end)
		{
			Dispatcher.Invoke(() =>
			{
				_CounterOfPost.Visibility = Visibility.Hidden;
				RefreshingLayoutHL.LaodWheelRotateStop();
				PostButton = null;
				PostButton = RefreshHB;
				postBegin = begin;
				postEnd = end;
			});
			

			if(postEnd != Wall.Count)
			{
				//Dispatcher.Invoke(() => ShowAfterPanel.Visibility = Visibility.Visible);
			}
			if(postBegin != 0)
			{
				//Dispatcher.Invoke(() => ShowBeforePanel.Visibility = Visibility.Visible);
			}
		}
		/// <summary>
		/// Выгружает записи
		/// </summary>
		/// <param name="begin">Начало блока, который будет выгружен</param>
		/// <param name="end">Конец блока (не включительно) который будет выгружен</param>
		private void UnloadPosts(int begin, int end)
		{
			int now = begin;
			for (; begin < end; ++ begin)
			{
				posts.Children.RemoveAt(now);
			}
		}
		#endregion
		#region Поля
		/// <summary>
		/// Анимированная панель загрузки для записей
		/// </summary>
		private HoverLoading RefreshingHL;
		/// <summary>
		/// Анимированная панель загрузки для отображения записей
		/// </summary>
		private HoverLoading RefreshingLayoutHL;
		/// <summary>
		/// Анимированная панель загрузки для статистики
		/// </summary>
		private HoverLoading StatRefreshingHL;
		/// <summary>
		/// Кнопка "Обновить" для записей
		/// </summary>
		private HoverButton RefreshHB;
		/// <summary>
		/// Кнопка "Обновить" для статистики
		/// </summary>
		private HoverButton StatRefreshHB;
		/// <summary>
		/// Кнопка "Показать следующие" для записей
		/// </summary>
		private HoverButton ShowBefore;
		/// <summary>
		/// Кнопка "Показать предыдущие" для записей
		/// </summary>
		private HoverButton ShowAfter;
		/// <summary>
		/// Словарь всех пользователей, упомянутых на стене. Доступ осуществляется по Id
		/// </summary>
		private Dictionary<int, User> users;
		/// <summary>
		/// Список всех записей на стене пользователя
		/// </summary>
		private List<Post> Wall;
		private string  access_token;
		private int     user_id;
		/// <summary>
		/// Интерфейс взаимодействия с VkAPI
		/// </summary>
		private ParseVkOutput Vk;
		/// <summary>
		/// Количество подгружаемых записей за раз
		/// </summary>
		private int defaultcount = 50;
		/// <summary>
		/// Итератор на первую запись, которая в данный момент отображена
		/// </summary>
		private int postBegin = 0;
		/// <summary>
		/// Итератор на последнюю запись, которая в данный момент отображена
		/// </summary>
		private int postEnd = 0;
		private UIElement PostButton
		{
			get
			{
				return _PostButtonPanel.Content as UIElement;
			}
			set
			{
				_PostButtonPanel.Content = value;
			}
		}
		private UIElement StatButton
		{
			get
			{
				return _StatButtons.Content as UIElement;
			}
			set
			{
				_StatButtons.Content = value;
			}
		}
		#endregion
	}
}
/*
1. Первая загрузка данных
2. Обновление интерфейса при подходе к нижней границе
3. Кнопки вход, выход.
4. Сохранение авторизации при повторном входе
5. Фоновая загрузка картинки с бегунком
6. Выбор поста в том же окне
*/