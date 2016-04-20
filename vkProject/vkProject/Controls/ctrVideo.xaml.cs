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
	public partial class ctrVideo : UserControl
	{
		public ctrVideo()
		{
			InitializeComponent();
			description.Opacity = 0.0;
			panel.Opacity = 0.0;
		}
		public ctrVideo(Video video)
		{
			InitializeComponent();
			description.Opacity = 0.0;
			panel.Opacity = 0.0;
			Title = video.Title;
			Views = video.Views;
			Description = video.Description;
			Url = video.Player;
			Photo = video.Photo_640;
		}
		public string Title
		{
			get { return title.Content.ToString(); }
			set { title.Content = value; }
		}
		public uint Views
		{
			get { return Convert.ToUInt32(views.Content.ToString()); }
			set { views.Content = Convert.ToString(value); }
		}
		public string Description
		{
			get { return description.Text; }
			set { description.Text = value; }
		}
		public string Url
		{
			get { return url; }
			set { url = value; }
		}
		public string Photo
		{
			get { return photo.Source.ToString(); }
			set { photo.Source = new BitmapImage(new Uri(value)); }
		}

		private string url;
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
			play.Visibility = Visibility.Hidden;
			Browser video = new Browser(Url);
			video.Show();
		}
	}
}