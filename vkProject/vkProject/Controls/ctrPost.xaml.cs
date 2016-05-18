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
		public string User_photo
		{
			get { return user_photo_url; }
			set
			{
				user_ico.Source = new BitmapImage(new Uri(value));
				user_photo_url = value;
			}
		}
		public string User_name
		{
			get { return user_name.Text; }
			set { user_name.Text = value; }
		}
		public void AddPhoto(Photo photo)
		{
			ctrPhoto ph = new ctrPhoto(photo);
			photos.Children.Add(ph);
		}
		public void AddVideo(Video video)
		{
			ctrVideo ph = new ctrVideo(video);
			videos.Children.Add(ph);
		}
		public void AddPoll(Poll poll)
		{
			pools.Children.Add(new ctrPoll(poll));
		}

		private string user_photo_url;
	}
}