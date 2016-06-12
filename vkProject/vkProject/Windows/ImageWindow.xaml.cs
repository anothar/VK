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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net;
using System.Windows.Shapes;

namespace vkProject
{
	/// <summary>
	/// Логика взаимодействия для ImageWindow.xaml
	/// </summary>
	public partial class ImageWindow : Window
	{
		public ImageWindow(VkAPI.Media.IPhoto photo, string urlBigPhoto)
		{
			ErrorMessage = new MessageBoxWindow();
			_description.Text = photo.Text;
		}
		void LoadImage(string urlImage)
		{
			wc = new WebClient();
			wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
			wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
		}
		private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			_loadingRatio.Text = e.ProgressPercentage.ToString() + "%";
		}
		private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			if (!e.Cancelled)
			{
				if (e.Error != null)
				{

				}
			}
		}

		WebClient wc;
		BitmapImage bi;
		MessageBoxWindow ErrorMessage;
	}
}
