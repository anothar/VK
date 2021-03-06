﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Configuration;
using System.Net;
using System.IO;
using System.Windows.Interop;
using System.Drawing;
using VkAPI.Media;

namespace VkAPI.Controls
{
	public partial class ctrPhoto : UserControl, IPhoto
	{

		public ctrPhoto()
		{
			InitializeComponent();
			text.Visibility = Visibility.Hidden;
		}
		public ctrPhoto(IPhoto photo)
		{
			InitializeComponent();
			//---------------------------------Инициализация-членов-интерфейса------------------------------------\\
			Text			=	photo.Text;
			Id				=	photo.Id;
			Date			=	photo.Date;
			Album_id		=	photo.Album_id;
			Owner_id		=	photo.Owner_id;
			User_id			=	photo.User_id;
			Photo_75		=	photo.Photo_75;
			Photo_130		=	photo.Photo_130;
			Photo_604		=	photo.Photo_604;
			Photo_807		=	photo.Photo_807;
			Photo_1280		=	photo.Photo_1280;
			Photo_2560		=	photo.Photo_2560;
			//----------------------------------------------------------------------------------------------------\\

			text.Visibility = Visibility.Hidden;
			smallImage = photo.Photo_604;

			if(photo.Photo_2560 != null)
				bigImage = photo.Photo_2560;
			else if(photo.Photo_1280 != null)
				bigImage = photo.Photo_1280;
			else if(photo.Photo_807 != null)
				bigImage = photo.Photo_807;

			if (bigImage != null)
			{
				image.Cursor = Cursors.Hand;
				image.MouseLeftButtonUp += Image_MouseLeftButtonUp;
			}
		}
		private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			vkProject.Browser bIm = new vkProject.Browser(bigImage, "Фотография");
			bIm.Show();
		}
		private string smallImage
		{
			get { return null; }
			set
			{
				image.Source = new BitmapImage(new Uri(value));
			}
		}
		private string bigImage { get; set; }

		public string Text
		{
			get { return text.Text; }
			set
			{
				text.Text = value;
			}
		}
		public int Id			 { get; private set; }
		public int Date			 { get; private set; }
		public int Album_id		 { get; private set; }
		public int Owner_id		 { get; private set; }
		public int User_id		 { get; private set; }
		public string Photo_75	 { get; private set; }
		public string Photo_130  { get; private set; }
		public string Photo_604  { get; private set; }
		public string Photo_807	 { get; private set; }
		public string Photo_1280 { get; private set; }
		public string Photo_2560 { get; private set; }
	}
}