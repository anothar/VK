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
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Navigation;
using System.Windows.Shapes;
using vkProject;
using VkAPI.Media;

namespace VkAPI.Controls
{
	public partial class ctrVideo : UserControl, IVideo
	{
		public ctrVideo()
		{
			description.Opacity = 0.0;
			panel.Opacity = 0.0;

			InitializeComponent();
		}
		public ctrVideo(IVideo video)
		{
			//---------Инициализация-членов-интерфейса----\\
			Title			= video.Title;
			Views			= video.Views;
			Description		= video.Description;
			Id				= video.Id;
			Date			= video.Date;
			Owner_id		= video.Owner_id;
			Duration		= video.Duration;
			Photo_130		= video.Photo_130;
			Photo_320		= video.Photo_320;
			Photo_640		= video.Photo_640;
			Player			= video.Player;
			Access_key		= video.Access_key;
			//--------------------------------------------\\

			if(Photo_640 != null)
				Photo = Photo_640;
			else if(Photo_320 != null)
				Photo = Photo_320;
			else if(Photo_130 != null)
				Photo = Photo_130;

			description.Opacity = 0.0;
			panel.Opacity = 0.0;
			InitializeComponent();
		}
		public string Photo
		{
			get { return photo.Source.ToString(); }
			set { photo.Source = new BitmapImage(new Uri(value)); }
		}
		public string Title
		{
			get { return title.Content.ToString(); }
			private set { title.Content = value; }
		}
		public int Views
		{
			get { return Convert.ToInt32(views.Content.ToString()); }
			private set { views.Content = value.ToString(); }
		}
		public string Description
		{
			get { return description.Text; }
			private set { description.Text = value; }
		}
		public int Id				{ get; private set; }
		public int Date				{ get; private set; }
		public int Owner_id			{ get; private set; }
		public int Duration			{ get; private set; }
		public string Photo_130		{ get; private set; }
		public string Photo_320		{ get; private set; }
		public string Photo_640		{ get; private set; }
		public string Player		{ get; private set; }
		public string Access_key	{ get; private set; }

		private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
		{
			DoubleAnimation da = new DoubleAnimation();
			da.From = 0.0;
			da.To = 1.0;
			da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
			panel.BeginAnimation(OpacityProperty, da);
			description.BeginAnimation(OpacityProperty, da);
		}
		private void panel_MouseLeave(object sender, MouseEventArgs e)
		{
			DoubleAnimation da = new DoubleAnimation();
			da.From = 1.0;
			da.To = 0.0;
			da.BeginTime = TimeSpan.FromMilliseconds(1000);
			da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
			panel.BeginAnimation(OpacityProperty, da);
			description.BeginAnimation(OpacityProperty, da);
		}
		private void play_MouseEnter(object sender, MouseEventArgs e)
		{
			DoubleAnimation da = new DoubleAnimation();
			da.From = 0.65;
			da.To = 1.0;
			da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
			play.BeginAnimation(OpacityProperty, da);
		}
		private void play_MouseLeave(object sender, MouseEventArgs e)
		{
			DoubleAnimation da = new DoubleAnimation();
			da.From = 1.0;
			da.To = 0.65;
			da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
			play.BeginAnimation(OpacityProperty, da);
		}
		private void play_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if(Player != null)
			{
				Browser video = new Browser(Player, "Видеозапись");
				video.Show();
			}
		}
	}
}