using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
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
	public partial class ctrPost : UserControl, IPost
	{
		public ctrPost()
		{
			InitializeComponent();
		}
		public ctrPost(IPost post, User user)
		{
			InitializeComponent();
			CopyPost(post);
			LoadUserInformation(user);
			BeginLayoutDesign(null);
		}
		public ctrPost(IPost post, User user, User RepUser)
		{
			InitializeComponent();
			CopyPost(post);
			LoadUserInformation(user);
			BeginLayoutDesign(RepUser);
		}
		public string Text
		{
			get
			{
				return text.Text;
			}
			set
			{
				if(value == null || value == "")
				{
					text.Text = null;
					text.FontSize = 0.1;
				}
				else
				{
					text.Text = value;
					text.FontSize = 13;
				}
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
			private set { user_name.Text = value; }
		}

		public int Id							{ get; private set; }
		public int Owner_id						{ get; private set; }
		public int From_id						{ get; private set; }
		public int Date							{ get; private set; }
		public int Likes
		{
			get
			{
				return Convert.ToInt32(likes.Text);
			}
			private set
			{
				likes.Text = value.ToString();
			}
		}
		public int Reposts
		{
			get
			{
				return Convert.ToInt32(reposts.Text);
			}
			set
			{
				reposts.Text = value.ToString();
			}
		}
		public string Post_type					{ get; private set; }
		public List<Photo> Photos				{ get; private set; }
		public List<Posted_photo> Posted_photos { get; private set; }
		public List<Video> Videos				{ get; private set; }
		public List<Media.Audio> Audios			{ get; private set; }
		public List<Document> Documents			{ get; private set; }
		public List<Graffity> Graffities		{ get; private set; }
		public List<Link> Links					{ get; private set; }
		public List<Node> Nodes					{ get; private set; }
		public Poll Poll						{ get; private set; }
		public Post Copied_Post					{ get; private set; }

		public UIElementCollection VideoPanel { get { return videos.Children; } }
		public UIElementCollection PhotoPanel { get { return photos.Children; } }
		public UIElementCollection AudioPanel { get { return audios.Children; } }
		public UIElementCollection PollPanel  { get { return polls.Children; }  }

		private void CopyPost(IPost post)
		{
			Text				= post.Text;
			Id					= post.Id;
			Owner_id			= post.Owner_id;
			From_id				= post.From_id;
			Date				= post.Date;
			Likes				= post.Likes;
			Reposts				= post.Reposts;
			Post_type			= post.Post_type;
			Photos				= post.Photos;
			Posted_photos		= post.Posted_photos;
			Videos				= post.Videos;
			Audios				= post.Audios;
			Documents			= post.Documents;
			Graffities			= post.Graffities;
			Links				= post.Links;
			Nodes				= post.Nodes;
			Poll				= post.Poll;
			Copied_Post			= post.Copied_Post;
		}
		private void BeginLayoutDesign(User repUser)
		{
			if(repUser != null)
			{
				BitmapImage bilikes = new BitmapImage();
				bilikes.BeginInit();
				var streamlike = File.OpenRead(Environment.CurrentDirectory + @"\like.png");
				bilikes.StreamSource = streamlike;
				bilikes.EndInit();
				likesImage.Source = bilikes;

				BitmapImage bireps = new BitmapImage();
				bireps.BeginInit();
				var streamrep = File.OpenRead(Environment.CurrentDirectory + @"\rep.png");
				bireps.StreamSource = streamrep;
				bireps.EndInit();
				repImage.Source = bireps;
			}
			else
			{
				foot.Height = 0;
			}
			if(Videos != null)
			{
                videos.Margin = new Thickness(5, 0, 5, 5);
				//-------------adding-videos-------------\\
				foreach(Video vid in Videos)
					VideoPanel.Add(new ctrVideo(vid));
			}
			if(Photos != null)
			{
                photos.Margin = new Thickness(5, 0, 5, 5);
				//------------adding-photos--------------\\
                foreach (Photo phot in Photos)
					PhotoPanel.Add(new ctrPhoto(phot));
			}
			if(Poll != null)
			{
                polls.Margin = new Thickness(5, 0, 5, 5);
				//----------------add-poll---------------\\
				PollPanel.Add(new ctrPoll(Poll));
			}
			if (Audios != null)
			{
				foreach(Media.Audio aud in Audios)
					AudioPanel.Add(new Audio(aud));
			}
            if(Links != null)
            {
                links.Margin = new Thickness(5, 0, 5, 5);
                //----------------add-link---------------\\
                links.Children.Add(new ctrLink(Links[0]));
            }
			if(Copied_Post != null)
			{
				repost.Children.Add(new ctrPost(Copied_Post, repUser));
			}
		}
		private void LoadUserInformation(User user)
		{
			User_photo = user.Photo_50;
			User_name = String.Format("{0} {1}", user.First_name, user.Last_name);
		}
		private string user_photo_url;
	}
}