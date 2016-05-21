using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Configuration;
using System.Net;
using System.IO;
using System.Windows.Interop;
using System.Drawing;
using VkAPI.Media;

namespace VkAPI.Controls
{
	public partial class ctrPhoto : UserControl
	{

		public ctrPhoto()
		{
			InitializeComponent();
			text.Visibility = Visibility.Hidden;
		}
		public ctrPhoto(Photo photo)
		{
			InitializeComponent();
			text.Visibility = Visibility.Hidden;
			Text = photo.Text;
			smallImage = photo.Photo_604;
			bigImage = photo.Photo_2560 != null ? photo.Photo_2560 : photo.Photo_1280;

			if (bigImage != null)
			{
				image.Cursor = Cursors.Hand;
				image.MouseLeftButtonUp += Image_MouseLeftButtonUp;
			}
		}

		private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			vkProject.Browser bIm = new vkProject.Browser(bigImage, "Фотография");
			bIm.Show();
		}

		private void text_MouseEnter(object sender, MouseEventArgs e)
		{
			DoubleAnimation da = new DoubleAnimation();
			da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
			da.From = 1.0;
			da.To = 0.0;
			text.BeginAnimation(OpacityProperty, da);
		}
		private void text_MouseLeave(object sender, MouseEventArgs e)
		{
			DoubleAnimation da = new DoubleAnimation();
			da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
			da.From = 0.0;
			da.To = 1.0;
			text.BeginAnimation(OpacityProperty, da);
		}

		public string Text
		{
			get { return text.Text; }
			set
			{
				text.Text = value;
			}
		}
		public string smallImage
		{
			get
			{
				return path_img;
			}
			set
			{
				GetImg(value);
			}
		}
		public string bigImage { get; set; }

		private void GetImg(string url)
		{
			//начать загрузку
			web1 = new WebClient();
			string path_temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + 
														ConfigurationManager.AppSettings["tempDirectory"] + 
														vkProject.Global.temporary_name + Path.GetExtension(url);

			web1.DownloadFileAsync(new Uri(url), path_temp);
			path_img = path_temp;
			web1.DownloadFileCompleted += Web1_DownloadFileCompleted;

			//начать крутить картинку загрузки
			//_source = GetSource();
			//loading.Source = _source;
			//ImageAnimator.Animate(_bitmap, OnFrameChanged);
		}
		private void Web1_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			//stopping loading animating
			//ImageAnimator.StopAnimate(_bitmap, OnFrameChanged);
			loading.Visibility = Visibility.Hidden;
			//writing image;
			var stream = File.OpenRead(path_img);
			BitmapImage im = new BitmapImage();
			im.BeginInit();
			im.CacheOption = BitmapCacheOption.OnLoad;
			im.StreamSource = stream;
			im.EndInit();
			stream.Close();

			vkProject.Global.temporary.Add(path_img);
			loading.Visibility = Visibility.Hidden;
			text.Visibility = Visibility.Visible;
			image.Source = im;
		}
		private string path_img;
		//private BitmapSource GetSource()
		//{
		//	if(_bitmap == null)
		//	{
		//		_bitmap = new Bitmap("712.gif");
		//	}
		//	IntPtr handle = IntPtr.Zero;
		//	handle = _bitmap.GetHbitmap();
		//	return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
		//}
		//private void FrameUpdatedCallback()
		//{
		//	ImageAnimator.UpdateFrames();
		//	if(_source != null)
		//		_source.Freeze();
		//	_source = GetSource();
		//	loading.Source = _source;
		//	InvalidateVisual();
		//}
		//private void OnFrameChanged(object sender, EventArgs e)
		//{
		//	Dispatcher.BeginInvoke(DispatcherPriority.Normal,
		//							new Action(FrameUpdatedCallback));
		//}
		//private BitmapSource _source;
		//private Bitmap _bitmap;
		private WebClient web1;
	}
}