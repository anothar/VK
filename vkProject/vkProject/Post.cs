using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkAPI.Media;

namespace VkAPI
{
	public class Post
	{     
        public uint Id                  { get; set; }
        public uint Owner_id            { get; set; }
        public uint From_id             { get; set; }
        public uint Date                { get; set; }
        public string Text              { get; set; } // текст записи (null, если нет)
        public string Post_type         { get; set; }

		public List<Photo> Photos               { get; set; }
		public List<Posted_photo> Posted_photos { get; set; }
		public List<Video> Videos               { get; set; }
		public List<Audio> Audios               { get; set; }
		public List<Document> Documents         { get; set; }
		public List<Graffity> Graffities        { get; set; }
		public List<Link> Links                 { get; set; }
		public List<Node> Nodes                 { get; set; }
		public Poll Poll                        { get; set; }
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
			User_id = id;
			First_name = name;
			Last_name = surname;
		}
		public uint User_id             { get; private set; }
		public string First_name        { get; private set; }
		public string Last_name         { get; private set; }
	}

	namespace Media
	{
		public class Photo
		{
			public uint Id                  { get; set; }
			public uint Date                { get; set; }
			public uint Width               { get; set; }
			public uint Heght               { get; set; }
			public uint Album_id            { get; set; }
			public uint Owner_id            { get; set; }
			public uint User_id             { get; set; }

			public string Text              { get; set; }
			public string Photo_75          { get; set; }
			public string Photo_130         { get; set; }
			public string Photo_604         { get; set; }
			public string Photo_807         { get; set; }
			public string Photo_1280        { get; set; }
			public string Photo_2560        { get; set; }

		}
		public class Posted_photo
		{
			public uint Id              { get; set; }
			public uint Owner_id        { get; set; }

			public string Photo_130     { get; set; }
			public string Photo_604     { get; set; }
		}
		public class Video
		{
			public uint Id              { get; set; }
			public uint Date            { get; set; }
			public uint Owner_id        { get; set; }
			public uint Duration        { get; set; }
			public uint Views           { get; set; }

			public string Title         { get; set; }
			public string Desription    { get; set; }
			public string Photo_130     { get; set; }
			public string Photo_320     { get; set; }
			public string Photo_640     { get; set; }
			public string Player        { get; set; }
		}
		public class Audio
		{
			public uint Owner_id        { get; set; }
			public uint Date            { get; set; }
			public uint Duration        { get; set; }
			public uint Id              { get; set; }

			public string Artist        { get; set; }
			public string Title         { get; set; }
			public string Url           { get; set; }
		}
		public class Document
		{
			public uint Id              { get; set; }
			public uint Owner_id        { get; set; }
			public uint Size            { get; set; }
			public uint Date            { get; set; }
			public uint Type            { get; set; }

			public string Title         { get; set; }
			public string Ext           { get; set; }
			public string Url           { get; set; }
			public string Photo_100     { get; set; }
			public string Photo_130     { get; set; }
		}
		public class Graffity
		{
			public uint	Id                  { get; set; }
			public uint	Owner_id            { get; set; }
			
			public string Photo_200         { get; set; }
			public string Photo_586         { get; set; }
		}
		public class Link
		{
			public string Url           { get; set; }
			public string Title         { get; set; }
			public string Caption       { get; set; }
			public string Description   { get; set; }
			public Photo Photo          { get; set; }
			public bool is_external     { get; set; }
		}
		public class Node
		{
			public uint Id              { get; set; }
            public uint Owner_id        { get; set; }
            public uint Date            { get; set; }
			public string Title         { get; set; }
			public string Text          { get; set; }
		}
		public class Answer
		{
			public uint Id              { get; set; }
			public string Text          { get; set; }
			public uint Votes           { get; set; }
			public uint Rate            { get; set; }
		}
		public class Poll
		{
			public uint Id              { get; set; }
			public uint Owner_id        { get; set; }
            public uint Created         { get; set; }
            public string Question      { get; set; }
			public uint Votes           { get; set; }
			public uint Answer_id       { get; set; }
			public List<Answer> Answers     { get; set; }
		}
	}
}