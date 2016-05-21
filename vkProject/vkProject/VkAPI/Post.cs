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
        public Post()
        {

        }
        public int Id                   { get; set; }
        public int Owner_id             { get; set; }
        public int From_id              { get; set; }
        public int Date                 { get; set; }
        public int Likes                { get; set; }
        public string Text              { get; set; }
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
        public Post Copied_Post                 { get; set; }
    }
    public class PostException : Exception
	{
		public PostException(string message) : base(message)
		{

		}
		public PostException(uint code)
		{
			this.code = code;
		}
		public uint Code { get { return code; } }
		private uint code;
	}
	public class User
	{
		public User()
		{

		}
		public int User_id				{ get; set; }
		public string First_name        { get; set; }
		public string Last_name         { get; set; }
		public string Photo_50			{ get; set; }

        override public int GetHashCode()
        {
            return User_id.GetHashCode() & First_name.GetHashCode() & Last_name.GetHashCode();
        }
	}

	namespace Media
	{
		public class Photo
		{
			public int Id                  { get; set; }
			public int Date                { get; set; }
			public int Width               { get; set; }
			public int Heght               { get; set; }
			public int Album_id            { get; set; }
			public int Owner_id            { get; set; }
			public int User_id             { get; set; }

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
			public int Id              { get; set; }
			public int Owner_id        { get; set; }

			public string Photo_130     { get; set; }
			public string Photo_604     { get; set; }
		}
		public class Video
		{
			public int Id               { get; set; }
			public int Date             { get; set; }
			public int Owner_id         { get; set; }
			public int Duration         { get; set; }
			public int Views            { get; set; }

			public string Title         { get; set; }
			public string Description   { get; set; }
			public string Photo_130     { get; set; }
			public string Photo_320     { get; set; }
			public string Photo_640     { get; set; }
			public string Player        { get; set; }
            public string Access_key    { get; set; }
        }
		public class Audio
		{
			public int Owner_id         { get; set; }
			public int Date             { get; set; }
			public int Duration         { get; set; }
			public int Id               { get; set; }

			public string Artist        { get; set; }
			public string Title         { get; set; }
			public string Url           { get; set; }
		}
		public class Document
		{
			public int Id              { get; set; }
			public int Owner_id        { get; set; }
			public int Size            { get; set; }
			public int Date            { get; set; }
			public int Type            { get; set; }

			public string Title         { get; set; }
			public string Ext           { get; set; }
			public string Url           { get; set; }
			public string Photo_100     { get; set; }
			public string Photo_130     { get; set; }
		}
		public class Graffity
		{
			public int	Id                  { get; set; }
			public int	Owner_id            { get; set; }
			
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
			public int Id              { get; set; }
            public int Owner_id        { get; set; }
            public int Date            { get; set; }
			public string Title         { get; set; }
			public string Text          { get; set; }
		}
		public class Answer
		{
			public int Id               { get; set; }
			public string Text          { get; set; }
			public uint Votes           { get; set; }
			public double Rate          { get; set; }
		}
		public class Poll
		{
			public int Id               { get; set; }
			public int Owner_id         { get; set; }
            public string Question      { get; set; }
			public int Votes            { get; set; }
			public int Answer_id        { get; set; }
			public List<Answer> Answers { get; set; }
		}
	}
}