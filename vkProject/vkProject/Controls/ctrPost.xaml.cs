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
			if(post.Copied_Post == null)
			{
				this.Text = post.Text;
                if (post.Photos != null)
                    foreach (var photo in post.Photos)
					    AddPhoto(photo);
				//foreach(var video in post.Videos)
				//	AddVideo(video);
				if(post.Poll != null)
					AddPoll(post.Poll);
			}
			else
			{
				this.Text = post.Copied_Post.Text;
                if(post.Copied_Post.Photos != null)
				    foreach(var photo in post.Copied_Post.Photos)
					    AddPhoto(photo);
				//foreach(var video in post.Copied_Post.Videos)
				//	AddVideo(video);
				if(post.Copied_Post.Poll != null)
					AddPoll(post.Poll);
			}
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
		public void AddPoll(Poll poll)
		{
			wall.Children.Add(new ctrPoll(poll));
		}
		List<Photo> photos = new List<Photo>();
		List<Video> videos = new List<Video>();
	}
}
