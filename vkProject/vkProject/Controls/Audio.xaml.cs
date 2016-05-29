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
using System.IO;
using System.Configuration;
using System.Net;
using System.Timers;
using VkAPI.Media;

namespace VkAPI.Controls
{
	public partial class Audio : UserControl, IAudio
	{
		public Audio()
		{
			InitializeComponent();
		}
		public Audio(IAudio audio)
		{
			InitializeComponent();
			//-------Инициализация-членов-интерфейса-------\\
			Artist = audio.Artist;
			Date			= audio.Date;
			Duration		= audio.Duration;
			Id				= audio.Id;
			Owner_id		= audio.Owner_id;
			Title			= audio.Title;
			Url				= audio.Url;
			//---------------------------------------------\\
			timetotal.Text = TimeSpan.FromSeconds(Duration).ToString();
			timenow.Text = "00:00:00";
			timeline.Maximum = Duration;
			timer = new Timer(1000);
			timer.Elapsed += Timer_Elapsed;
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			timeline.Value = audioME.Position.Seconds;
			timenow.Text = TimeSpan.FromSeconds(audioME.Position.Seconds).ToString();
		}

		public string Artist		{ get; private set; }
		public int Date				{ get; private set; }
		public int Duration			{ get; private set; }
		public int Id				{ get; private set; }
		public int Owner_id			{ get; private set; }
		public string Title			{ get; private set; }
		public string Url			{ get; private set; }

		private bool isPlaying { get; set; } = false;
		private bool SourceLoaded { get; set; } = false;

		private void playPause_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if(SourceLoaded)
			{
				if(isPlaying)
				{
					playPause.Source = new Uri("playaudio.png", UriKind.Relative);
					audioME.Pause();
					isPlaying = false;
					timer.Stop();
				}
				else
				{
					playPause.Source = new Uri("pauseaudio.png", UriKind.Relative);
					audioME.Play();
					isPlaying = true;
					timer.Start();
				}
			}
			else
			{
				LoadSource();
			}
		}
		private void LoadSource()
		{
			web1 = new WebClient();
			string path_temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
														ConfigurationManager.AppSettings["tempDirectory"] +
														vkProject.Global.temporary_name + ".mp3";

			web1.DownloadFileAsync(new Uri(Url), path_temp);
			PathAudio = path_temp;
			web1.DownloadFileCompleted += Web1_DownloadFileCompleted; ;

		}
		private void Web1_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			audioME.Source = new Uri(PathAudio);
			SourceLoaded = true;
			playPause_MouseUp(audioME, null);
		}

		private string PathAudio { get; set; }
		WebClient web1;
		Timer timer;
	}
}