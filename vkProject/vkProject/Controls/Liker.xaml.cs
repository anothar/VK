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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkAPI.Media;

namespace VkAPI.Controls
{
	public partial class ctrLiker : UserControl
	{
		public ctrLiker(User liker)
		{
            this.liker = liker;
			InitializeComponent();
            if (liker.Photo_50 != null)
                user_ico.Source = new BitmapImage(new Uri(liker.Photo_50));
            name.Text = liker.Last_name + " " + liker.First_name;
		}

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/id" + liker.User_id);
        }

        User liker;
    }
}
