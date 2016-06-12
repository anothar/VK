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
using System.Windows.Shapes;

namespace vkProject
{
	public partial class MessageBoxWindow : Window
	{
		public MessageBoxWindow()
		{
			InitializeComponent();
			
		}
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			(sender as Button).Visibility = Visibility.Hidden;
			_errorText.Text = TextError;
		}
		public void Show(string message, string title, Exception exc)
		{
			_errorMess.Text = message;
			Title = title;
			TextError = exc.Message;
			ShowDialog();
		}
		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Close();
		}
		private string TextError { get; set; }
	}
}
