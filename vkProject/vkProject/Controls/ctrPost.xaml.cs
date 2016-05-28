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
	public partial class ctrPost : UserControl, IPost
	{
		public ctrPost()
		{
			InitializeComponent();
		}
		public ctrPost(IPost post)
		{
			CopyPost(post);
			BeginLayoutDesign();
			InitializeComponent();
		}
		public ctrPost(IPost post, User user)
		{
			CopyPost(post);
			LoadUserInformation(user);
			BeginLayoutDesign();
			InitializeComponent();
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

		public int Id							{ get; private set; }
		public int Owner_id						{ get; private set; }
		public int From_id						{ get; private set; }
		public int Date							{ get; private set; }
		public int Likes						{ get; private set; }
		public string Post_type					{ get; private set; }
		public List<Photo> Photos				{ get; private set; }
		public List<Posted_photo> Posted_photos { get; private set; }
		public List<Video> Videos				{ get; private set; }
		public List<Audio> Audios				{ get; private set; }
		public List<Document> Documents			{ get; private set; }
		public List<Graffity> Graffities		{ get; private set; }
		public List<Link> Links					{ get; private set; }
		public List<Node> Nodes					{ get; private set; }
		public Poll Poll						{ get; private set; }
		public Post Copied_Post					{ get; private set; }

		public UIElementCollection VideoPanel { get { return videos.Children; } }
		public UIElementCollection PhotoPanel { get { return photos.Children; } }
		public UIElement		   PollPanel  { get { return polls.Children[0]; } private set { polls.Children[0] = value; } }

		private void CopyPost(IPost post)
		{
			Id					= post.Id;
			Owner_id			= post.Owner_id;
			From_id				= post.From_id;
			Date				= post.Date;
			Likes				= post.Likes;
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
		private void BeginLayoutDesign()
		{
			//-------------adding-videos-------------\\
			foreach(Video vid in Videos)
				VideoPanel.Add(new ctrVideo(vid));

			//------------adding-photos--------------\\
			foreach(Photo phot in Photos)
				PhotoPanel.Add(new ctrPhoto(phot));

			//----------------add-poll---------------\\
			PollPanel = new ctrPoll(Poll);
		}
		private void LoadUserInformation(User user)
		{
			User_name = String.Format("{0} {1}", user.First_name, user.Last_name);
			User_photo = user.Photo_50;
		}
		private string user_photo_url;
	}
}