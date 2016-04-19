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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VkAPI.Controls
{
	/// <summary>
	/// Логика взаимодействия для Photo.xaml
	/// </summary>
	public partial class Photo : UserControl
	{
		public Photo()
		{
			InitializeComponent();
		}

		private void text_MouseEnter(object sender, MouseEventArgs e)
		{
			DoubleAnimation da = new DoubleAnimation();
			da.Duration = new Duration(TimeSpan.FromMilliseconds(100));
			da.From = 1.0;
			da.To = 0.0;
			text.BeginAnimation(OpacityProperty, da);
		}
		private void text_MouseLeave(object sender, MouseEventArgs e)
		{
			DoubleAnimation da = new DoubleAnimation();
			da.Duration = new Duration(TimeSpan.FromMilliseconds(100));
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
		public string Image
		{
			get
			{
				return image.Source.ToString();
			}
			set
			{
				image.Source = new BitmapImage(new Uri(value));
			}
		}
	}
}
