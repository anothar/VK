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
using VkAPI;

namespace VkAPI.Controls
{
	public partial class ctrPost : UserControl
	{
		public ctrPost()
		{
			InitializeComponent();
		}
		public ctrPost(Post post)
		{
			InitializeComponent();
			this.Text = post.Text;
		}
		public string Text
		{
			get
			{
				return text.Text;
			}
			set
			{
				text.Text = value;
			}
		}
		public void AddPhoto(Photo photo)
		{
			ctrPhoto ph = new ctrPhoto(photo);
			wall.Children.Add(ph);
			photos.Add(photo);
		}
		public void AddVideo(Video video)
		{
			ctrVideo ph = new ctrVideo(video);
			wall.Children.Add(ph);
			videos.Add(video);
		}
		public void AddPoll(ctrPoll poll)
		{
			wall.Children.Add(poll);
		}
		List<Photo> photos = new List<Photo>();
		List<Video> videos = new List<Video>();
	}
}
