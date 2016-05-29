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
	public partial class ctrLink : UserControl, ILink
	{
		public ctrLink()
		{
			InitializeComponent();
		}
		public ctrLink(ILink link)
		{
            //------Инициализация-членов-интерфейса------\\
			InitializeComponent();
            Url = link.Url;
            title.Text = link.Title;
            Title = link.Title;
            Caption = link.Caption;
            Description = link.Description;
            Photo = link.Photo;
            is_external = link.is_external;

            photo.Children.Add(new ctrPhoto(Photo));

		}
        public string Url { get; set; }
        public string Title
        {
            get
            {
                return title.Text;
            }
            set
            {
                title.Text = value;
            }
        }
        public string Caption { get; set; }
        public string Description
        {
            get
            {
                return description.Text;
            }
            set
            {
                description.Text = value;
            }
        }
        public Photo Photo { get; set; }
        public bool is_external { get; set; }

        private void click_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Url);
        }
    }
}
