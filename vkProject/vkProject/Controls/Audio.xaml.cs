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
using System.Configuration;
using System.Net;
using System.Timers;
using VkAPI.Media;

namespace VkAPI.Controls
{
	public partial class Audio : UserControl, IAudio
	{
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

			playImg = new BitmapImage();
			var playstram = File.OpenRead(Environment.CurrentDirectory + @"\play.png");
			playImg.BeginInit();
			playImg.StreamSource = playstram;
			playImg.EndInit();

			pauseImg = new BitmapImage();
			var pausestream = File.OpenRead(Environment.CurrentDirectory + @"\pause.png");
			pauseImg.BeginInit();
			pauseImg.StreamSource = pausestream;
			pauseImg.EndInit();

			timetotal.Text = TimeSpan.FromSeconds(Duration).ToString();
			timenow.Text = "00:00:00";
			timeline.Maximum = Duration;
			timer1 = new Timer(500);
			timer2 = new Timer(500);
			timer2.Elapsed += Timer2_Elapsed;
			timer1.Elapsed += Timer_Elapsed;
			playPause.Source = playImg;
			timeline.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(AudioME_MouseLeftButtonUp), true);
			timeline.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(AudioME_MouseLeftButtonDown), true);
		}
		private void Timer2_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() => timenow.Text = TimeSpan.FromSeconds(Convert.ToInt32(audioME.Position.TotalSeconds)).ToString());
		}
		private void AudioME_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (Duration > 0)
			{
				audioME.Position = TimeSpan.FromSeconds(timeline.Value);
				timer1.Start();
			}
		}
		private void AudioME_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			timer1.Stop();
		}
		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() => timeline.Value = audioME.Position.TotalSeconds);
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
					playPause.Source = playImg;
					audioME.Pause();
					isPlaying = false;
					timer1.Stop();
					timer2.Stop();
				}
				else
				{
					playPause.Source = pauseImg;
					audioME.Play();
					isPlaying = true;
					timer1.Start();
					timer2.Start();
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
		private BitmapImage playImg;
		private BitmapImage pauseImg;
		WebClient web1;
		Timer timer1;
		Timer timer2;
	}
}