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
	public partial class ctrLikersList : UserControl
	{
		public ctrLikersList()
		{
			InitializeComponent();
		}
        public ctrLikersList(List<KeyValuePair<int,User>> list)
        {
            InitializeComponent();
            foreach(var user in list)
                Add(user);
        }
        public void Add(KeyValuePair<int, User> user)
        {
            users_panel.Children.Add(new ctrLiker(user.Value));
            count_likes_panel.Children.Add(new TextBlock()
            {
                Text = user.Key.ToString(),
                Height = 50,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5, 5, 5, 5)
            });
        }
        public void Clear()
        {
            users_panel.Children.Clear();
            count_likes_panel.Children.Clear();
        }
    }
}
