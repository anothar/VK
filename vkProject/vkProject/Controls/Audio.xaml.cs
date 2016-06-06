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
		#region Конструкторы
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
		#endregion
		#region Поля интерфейса
		public string Artist		{ get; private set; }
		public int Date				{ get; private set; }
		public int Duration			{ get; private set; }
		public int Id				{ get; private set; }
		public int Owner_id			{ get; private set; }
		public string Title			{ get; private set; }
		public string Url			{ get; private set; }
		#endregion
		#region Методы
		/// <summary>
		/// Метод, загружающий медиафайл
		/// </summary>
		private void LoadSource()
		{
			IsLoading = true;

			web1 = new WebClient();
			string path_temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
														ConfigurationManager.AppSettings["tempDirectory"] +
														vkProject.Global.temporary_name + ".mp3";

			web1.DownloadFileAsync(new Uri(Url), path_temp);
			PathAudio = path_temp;
			web1.DownloadFileCompleted += Web1_DownloadFileCompleted; ;

		}
		#endregion
		#region События
		/// <summary>
		/// Происходит при срабатывании timer2
		/// </summary>
		private void Timer2_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() => timenow.Text = TimeSpan.FromSeconds(Convert.ToInt32(audioME.Position.TotalSeconds)).ToString());
		}
		/// <summary>
		/// Вызывается при срабатывании timer1
		/// </summary>
		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() => timeline.Value = audioME.Position.TotalSeconds);
		}
		/// <summary>
		/// Происходит при отжатии кнопки мыши от позиции бегунка
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AudioME_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (Duration > 0)
			{
				audioME.Position = TimeSpan.FromSeconds(timeline.Value);
				timer1.Start();
			}
		}
		/// <summary>
		/// Происходит при нажатии левой кнопки мыши по позиции бегунка
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AudioME_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			timer1.Stop();
		}
		/// <summary>
		/// Вызывается при нажатии на картинку со значком воспроизведения
		/// </summary>
		private void playPause_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if(!IsLoading)
			{
				if(SourceLoaded)
				{
					if(IsPlaying)
					{
						playPause.Source = playImg;
						audioME.Pause();
						IsPlaying = false;
						timer1.Stop();
						timer2.Stop();
					}
					else
					{
						playPause.Source = pauseImg;
						audioME.Play();
						IsPlaying = true;
						timer1.Start();
						timer2.Start();
					}
				}
				else
				{
					LoadSource();
				}
			}
		}
		/// <summary>
		/// Вызывается при окончании загрузки файла
		/// </summary>
		private void Web1_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			audioME.Source = new Uri(PathAudio);
			SourceLoaded = true;
			IsLoading = false;
			playPause_MouseUp(audioME, null);
		}
		#endregion
		#region Поля
		/// <summary>
		/// Загружается ли медиафайл в данный момент
		/// </summary>
		private bool IsLoading { get; set; } = false;
		/// <summary>
		/// Воспроизводится ли медиафайл
		/// </summary>
		private bool IsPlaying { get; set; } = false;
		/// <summary>
		/// Загружен ли медиафайл
		/// </summary>
		private bool SourceLoaded { get; set; } = false;
		/// <summary>
		/// Путь до скачанного медиафайла
		/// </summary>
		private string PathAudio { get; set; }
		/// <summary>
		/// Картинка воспроизведения
		/// </summary>
		private BitmapImage playImg;
		/// <summary>
		/// Картинка паузы
		/// </summary>
		private BitmapImage pauseImg;
		/// <summary>
		/// Вебклиент, загружающий медиафайл
		/// </summary>
		WebClient web1;
		/// <summary>
		/// Таймер, отвечающий за смещение позиции проигрываниея на таймлайне
		/// </summary>
		Timer timer1;
		/// <summary>
		/// Таймер, отвечающий за отображение проигранного времени
		/// </summary>
		Timer timer2;
		#endregion
	}
}