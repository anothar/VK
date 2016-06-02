using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using VkAPI.Media;

namespace VkAPI
{
	public interface IPost
	{
		int Id { get; }
		int Owner_id { get; }
		int From_id { get; }
		int Date { get; }
		int Likes { get; }
		string Text { get; }
		string Post_type { get; }

		List<Photo> Photos { get; }
		List<Posted_photo> Posted_photos { get; }
		List<Video> Videos { get; }
		List<Audio> Audios { get; }
		List<Document> Documents { get; }
		List<Graffity> Graffities { get; }
		List<Link> Links { get; }
		List<Node> Nodes { get; }
		Poll Poll { get; }
		Post Copied_Post { get; }
	}
	public class Post : IPost
	{     
        public Post()
        {

        }
		public int Id					{ get; set; }
		public int Owner_id				{ get; set; }
		public int From_id				{ get; set; }
		public int Date					{ get; set; }
		public int Likes				{ get; set; }
		public string Text				{ get; set; }
		public string Post_type			{ get; set; }
		
		public List<Photo> Photos		{ get; set; }
		public List<Posted_photo> Posted_photos { get; set; }
		public List<Video> Videos		{ get; set; }
		public List<Audio> Audios		{ get; set; }
		public List<Document> Documents	{ get; set; }
		public List<Graffity> Graffities { get; set; }
		public List<Link> Links			{ get; set; }
		public List<Node> Nodes			{ get; set; }
		public Poll Poll				{ get; set; }
		public Post Copied_Post			{ get; set; }

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
		#region interfaces
		public interface IPhoto : IPostedPhoto
		{
			int Id { get; }
			int Date { get; }
			int Album_id { get; }
			int Owner_id { get; }
			int User_id { get; }

			string Text { get; }
			string Photo_75 { get; }
			string Photo_130 { get; }
			string Photo_604 { get; }
			string Photo_807 { get; }
			string Photo_1280 { get; }
			string Photo_2560 { get; }
		}
		public interface IPostedPhoto
		{
			int Id				{ get; }
			int Owner_id		{ get; }

			string Photo_130	{ get; }
			string Photo_604	{ get; }
		}
		public interface IVideo
		{
			int Id				{ get; }
			int Date			{ get; }
			int Owner_id		{ get; }
			int Duration		{ get; }
			int Views			{ get; }

			string Title		{ get; }
			string Description	{ get; }
			string Photo_130	{ get; }
			string Photo_320	{ get; }
			string Photo_640	{ get; }
			string Player		{ get; }
			string Access_key	{ get; }
		}
		public interface IAudio
		{
			int Owner_id	{ get; }
			int Date		{ get; }
			int Duration	{ get; }
			int Id			{ get; }

			string Artist	{ get; }
			string Title	{ get; }
			string Url		{ get; }
		}
		public interface IDocument
		{
			int Id			{ get; }
			int Owner_id	{ get; }
			int Size		{ get; }
			int Date		{ get; }
			int Type		{ get; }

			string Title	{ get; }
			string Ext		{ get; }
			string Url		{ get; }
			string Photo_100 { get; }
			string Photo_130 { get; }
		}
		public interface IGraffity
		{
			int	Id			{ get; }
			int	Owner_id	{ get; }

			string Photo_200 { get; }
			string Photo_586 { get; }
		}
		public interface ILink
		{
			string Url			{ get; }
			string Title		{ get; }
			string Caption		{ get; }
			string Description	{ get; }
			Photo Photo			{ get; }
			bool is_external	{ get; }
		}
		public interface INode
		{
			int Id			{ get; }
			int Owner_id	{ get; }
			int Date		{ get; }
			string Title	{ get; }
			string Text		{ get; }
		}
		public interface IAnswer
		{
			int Id			{ get; }
			string Text		{ get; }
			uint Votes		{ get; }
			double Rate		{ get; }
		}
		public interface IPoll
		{
			int Id			{ get; }
			int Owner_id	{ get; }
			string Question { get; }
			int Votes		{ get; }
			int Answer_id	{ get; }
			List<Answer> Answers { get; }
		}
		#endregion

		public class Photo : IPhoto
		{
			public int Id                  { get; set; }
			public int Date                { get; set; }
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
		public class Posted_photo : IPostedPhoto
		{
			public int Id              { get; set; }
			public int Owner_id        { get; set; }

			public string Photo_130     { get; set; }
			public string Photo_604     { get; set; }
		}
		public class Video : IVideo
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
		public class Audio : IAudio
		{
			public int Owner_id         { get; set; }
			public int Date             { get; set; }
			public int Duration         { get; set; }
			public int Id               { get; set; }
											   
			public string Artist        { get; set; }
			public string Title         { get; set; }
			public string Url           { get; set; }
		}
		public class Document : IDocument
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
		public class Graffity : IGraffity
		{
			public int	Id                  { get; set; }
			public int	Owner_id            { get; set; }
												   
			public string Photo_200         { get; set; }
			public string Photo_586         { get; set; }
		}
		public class Link : ILink
		{
			public string Url           { get; set; }
			public string Title         { get; set; }
			public string Caption       { get; set; }
			public string Description   { get; set; }
			public Photo Photo          { get; set; }
			public bool is_external     { get; set; }
		}
		public class Node : INode
		{
			public int Id              { get; set; }
            public int Owner_id        { get; set; }
            public int Date            { get; set; }
			public string Title         { get; set; }
			public string Text          { get; set; }
		}
		public class Answer : IAnswer
		{
			public int Id               { get; set; }
			public string Text          { get; set; }
			public uint Votes           { get; set; }
			public double Rate          { get; set; }
		}
		public class Poll : IPoll
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