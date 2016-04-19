using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkAPI
{
	public class Post
	{
		/// <summary>
		/// текст записи (null, если нет)
		/// </summary>
		public string description { get; set; }
		/// <summary>
		/// массив ссылок на фотографии (null, если нет)
		/// </summary>
		public string[] pictures
		{
			get
			{
				return pictures;
			}
			set
			{
				if(value.Length > 10)
					throw new PostException("Слишком много прикрепленных изображений!");
				int now = 0;
				pictures = new string[10];
				for(; now < value.Length; ++now)
					pictures[now] = value[now];
				for(; now < 10; ++now)
					pictures[now] = null;
			}
		}
		/// <summary>
		/// массив ссылок на видеозаписи (null, если нет)
		/// </summary>
		public string[] videos
		{
			get
			{
				return videos;
			}
			set
			{
				if(value.Length > 10)
					throw new PostException("Слишком много прикрепленных видеозаписей!");
				int now = 0;
				videos = new string[10];
				for(; now < value.Length; ++now)
					videos[now] = value[now];
				for(; now < 10; ++now)
					videos[now] = null;
			}
		}
		/// <summary>
		/// массив ссылок на аудиозаписи (null, если нет)
		/// </summary>
		public string[] audios
		{
			get
			{
				return audios;
			}
			set
			{
				if(value.Length > 10)
					throw new PostException("Слишком много прикрепленных видеозаписей!");
				int now = 0;
				audios = new string[10];
				for(; now < value.Length; ++now)
					audios[now] = value[now];
				for(; now < 10; ++now)
					audios[now] = null;
			}
		}
		/// <summary>
		/// опрос (null, если нет)
		/// </summary>
		public KeyValuePair<string, int>[] interview { get; set; }
	}
	public class PostException : Exception
	{
		public PostException(string message) : base(message)
		{

		}
	}
	public class User
	{
		public User(uint id, string name, string surname)
		{
			user_id = id;
			first_name = name;
			last_name = surname;
		}
		public uint user_id { get; private set; }
		public string first_name { get; private set; }
		public string last_name { get; private set; }
	}

	namespace Media
	{
		public class Photo
		{
			public uint Id { get; set; }
			public uint Date { get; set; }
			public uint Width { get; set; }
			public uint Heght { get; set; }
			public uint Album_id { get; set; }
			public uint Owner_id { get; set; }
			public uint User_id { get; set; }

			public string Text { get; set; }
			public string Photo_75 { get; set; }
			public string Photo_130 { get; set; }
			public string Photo_604 { get; set; }
			public string Photo_807 { get; set; }
			public string Photo_1280 { get; set; }
			public string Photo_2560 { get; set; }

		}
		public class Posted_photo
		{
			public uint Id { get; set; }
			public uint Owner_id { get; set; }

			public string Photo_130 { get; set; }
			public string Photo_604 { get; set; }
		}
		public class Video
		{
			public uint Id { get; set; }
			public uint Date { get; set; }
			public uint Owner_id { get; set; }
			public uint Duration { get; set; }
			public uint Views { get; set; }

			public string Title { get; set; }
			public string Desription { get; set; }
			public string Photo_130 { get; set; }
			public string Photo_320 { get; set; }
			public string Photo_604 { get; set; }
			public string Player { get; set; }
		}
		public class Audio
		{
			public uint Owner_id { get; set; }
			public uint Date { get; set; }
			public uint Duration { get; set; }
			public uint Id { get; set; }

			public string Artist { get; set; }
			public string Title { get; set; }
			public string Url { get; set; }
		}
		public class Document
		{
			public uint Id { get; set; }
			public uint Owner_id { get; set; }
			public uint Size { get; set; }
			public uint Date { get; set; }
			public uint Type { get; set; }

			public string Title { get; set; }
			public string Ext { get; set; }
			public string Url { get; set; }
			public string Photo_100 { get; set; }
			public string Photo_130 { get; set; }
		}
	}
}