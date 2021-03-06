﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Configuration;
using System.Net;
using vkProject;

namespace VkAPI
{
    // Класс для низкоуровневой работы с API Вконтакте
    public class vkAPI
    {
        // Конструируем объект vkAPI с полученными заранее access_token и user_id
        public vkAPI(string access_token, int user_id)
        {
            this.user_id = user_id;
            this.access_token = access_token;
        }
        // Общий метод запроса GET
        private string GET(string url, string data)
        {
            WebRequest req = WebRequest.Create(url + '?' + data);
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }
        // Общий метод запроса POST
        private string POST(string url, string data)
        {
            WebRequest req = WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            byte[] arr = Encoding.UTF8.GetBytes(data);
            req.GetRequestStream().Write(arr, 0, arr.Length);

            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string str = sr.ReadToEnd();
            sr.Close();

            return str;
        }
        // Метод запроса GET, специализированный под это приложение
        public string get(string method, string data)
        {
            return GET(Url_Api + method, data + "&" + version_api + "&access_token=" + access_token);
        }
        // Метод запроса POST, специализированный под это приложение
        public string post(string method, string data)
        {
            return POST(Url_Api + method, data + "&" + version_api + "&access_token=" + access_token);
        }
        // Метод возвращает всех друзей пользователя, который вошел в приложение
        public List<User> getFriends()
        {
            Global.WriteLogString("Getting friends...");

			List<User> Friends = new List<User>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.get(Methods.Friends.Get_Xml, ""));
            int count = Convert.ToInt32(doc.DocumentElement.FirstChild.InnerText), offset = 0;
            while (count > 0)
            {
                do
                    doc.LoadXml(this.get(Methods.Friends.Get_Xml, "fields=online&offset=" + offset.ToString()));
                while (doc.DocumentElement.Name != "response");

                XmlNode node = null;
                foreach (XmlNode item in doc.DocumentElement)
                {
                    if (item.Name == "items" && item.Attributes.Count == 1 && item.Attributes[0].Value == "true")
                    {
                        node = item;
                        break;
                    }
                }
                foreach (XmlNode item in node.ChildNodes)
                {
                    if (item.Name == "user")
                    {
                        int id = 0;
                        string name = "", surname = "", photo_50 = null, photo_100 = null;
                        foreach (XmlNode it in item.ChildNodes)
                        {
                            switch (it.Name)
                            {
                                case "id": id = Convert.ToInt32(it.InnerText); break;
                                case "first_name": name = it.InnerText; break;
                                case "last_name": surname = it.InnerText; break;
                                case "photo_50": photo_50 = it.InnerText; break;
                                case "photo_100": photo_100 = it.InnerText; break;
                            }
                        }
                        Friends.Add(new User() { User_id = id, First_name = name, Last_name = surname, Photo_50 = photo_50 });
                    }
                }
                count -= 5000;
                offset += 5000;
            }
			Global.WriteLogString("Friends had been got");

            return Friends;
        }
        // Метод возвращает всех друзей пользователя, которые делали лайки к передаваемому посту
        public List<User> getLikes(Post post)
        {
            List<User> whoLiked = new List<User>();
            XmlDocument doc = new XmlDocument();
            do
                doc.LoadXml(this.get(Methods.Likes.GetList_Xml, "type=post&friends_only=1&owner_id=" + post.Owner_id + "&item_id=" + post.Id));
            while (doc.DocumentElement.Name != "response");

            int count = Convert.ToInt32(doc.DocumentElement.FirstChild.InnerText), offset = 0;
            while (count > 0)
            {
                string get_data = String.Concat("type=post&friends_only=1&extended=1&owner_id=", post.Owner_id, "&item_id=", post.Id, "&count=100&offset=", offset.ToString());
                do
                    doc.LoadXml(this.get(Methods.Likes.GetList_Xml, get_data));
                while (doc.DocumentElement.Name != "response");

				foreach(XmlNode item in doc.DocumentElement.ChildNodes[1])
					if(item.Name == "user")
					{
						int id = 0;
						string name = null, surname = null, photo_50 = null, photo_100 = null;
						foreach(XmlNode it in item.ChildNodes)
						{
							switch(it.Name)
							{
								case "id": id = Convert.ToInt32(it.InnerText); break;
								case "first_name": name = it.InnerText; break;
								case "last_name": surname = it.InnerText; break;
								case "photo_50": photo_50 = it.InnerText; break;
								case "photo_100": photo_100 = it.InnerText; break;
							}
						}
						whoLiked.Add(new User() { User_id = id, First_name = name, Last_name = surname, Photo_50 = photo_50 });
					}
                count -= 100;
                offset += 100;
            }
            return whoLiked;
        }
        // Метод возвращает все записи, которые есть на стене пользователя, и список тех чьими записями пользаватель делился
        public string getUserPhoto(int id)
        {
            string Photo_50 = null;
            XmlDocument doc = new XmlDocument();

            do
                doc.LoadXml(get(Methods.Users.Get_Xml, "fields=photo_50&user_ids=" + id.ToString()));
            while (doc.DocumentElement.Name != "response");

            foreach (XmlNode item in doc.DocumentElement)
                if (item.Name == "user")
                    foreach (XmlNode it in item.ChildNodes)
                        switch (it.Name)
                        {
                            case "photo_50": Photo_50 = it.InnerText; break;
                        }

            return Photo_50;
        }
        public KeyValuePair<Dictionary<int, User>, List<Post>> getWall()
        {
            List<Post> Wall = new List<Post>();
            Dictionary<int, User> Senders = new Dictionary<int, User>();
            XmlDocument doc = new XmlDocument();

            do
                doc.LoadXml(this.get(Methods.Wall.Get_Xml, "count=1"));
            while (doc.DocumentElement.Name != "response");

            int count_of_posts = Convert.ToInt32(doc.DocumentElement.FirstChild.InnerText), offset = 0;

            while (count_of_posts > 0)
            {
                do
                    doc.LoadXml(this.get(Methods.Wall.Get_Xml, "&count=100&extended=1&offset=" + offset.ToString()));
                while (doc.DocumentElement.Name != "response");
                offset += 100;
                count_of_posts -= 100;
                getPosts(doc, ref Wall);
                getSenders(doc, ref Senders);
            }
            return new KeyValuePair<Dictionary<int, User>, List<Post>>(Senders, Wall);
        }

        #region GetWallHelpFunctions
        void getPosts(XmlDocument doc, ref List<Post> Wall)
        {
            foreach (XmlNode it in doc.DocumentElement)
                if (it.Name == "items")
                {
                    if (it.Attributes == null || it.Attributes[0].Value != "true")
                        break;
                    foreach (XmlNode item in it.ChildNodes)
                        if (item.Name == "post")
                            Wall.Add(getPost(item));
                }
        }
        void getSenders(XmlDocument doc, ref Dictionary<int, User> Senders)
        {
            foreach (XmlNode it in doc.DocumentElement)
                if (it.Name == "profiles")
                {
                    if (it.Attributes == null || it.Attributes[0].Value != "true")
                        break;
                    foreach (XmlNode item in it.ChildNodes)
                        if (item.Name == "user")
                        {
                            int id = 0;
                            string name = null, surname = null, photo_50 = null, photo_100 = null;
                            foreach (XmlNode i in item.ChildNodes)
                            {
                                switch (i.Name)
                                {
                                    case "id": id = Convert.ToInt32(i.InnerText); break;
                                    case "first_name": name = i.InnerText; break;
                                    case "last_name": surname = i.InnerText; break;
                                    case "photo_50": photo_50 = i.InnerText; break;
                                    case "photo_100": photo_100 = i.InnerText; break;
                                }
                            }
                            if (!Senders.ContainsKey(id))
                                Senders.Add(id, new User() { User_id = id, First_name = name, Last_name = surname, Photo_50 = photo_50 });
                        }
                }
                else if (it.Name == "groups")
                {
                    if (it.Attributes == null || it.Attributes[0].Value != "true")
                        break;
                    foreach (XmlNode item in it.ChildNodes)
                        if (item.Name == "group")
                        {
                            int id = 0;
                            string name = null, photo_50 = null, photo_100 = null;
                            foreach (XmlNode i in item.ChildNodes)
                            {
                                switch (i.Name)
                                {
                                    case "id": id = Convert.ToInt32(i.InnerText); break;
                                    case "name": name = i.InnerText; break;
                                    case "photo_50": photo_50 = i.InnerText; break;
                                    case "photo_100": photo_100 = i.InnerText; break;
                                }
                            }
                            if (!Senders.ContainsKey(id))
                                Senders.Add(id, new User() { User_id = id, First_name = name, Photo_50 = photo_50 });
                        }
                }
        }

        Post getPost(XmlNode Node)
        {
            Post post = new Post();
            foreach (XmlNode item in Node.ChildNodes)
            {
                switch (item.Name)
                {
                    case "id": post.Id = Convert.ToInt32(item.InnerText); break;
                    case "from_id": post.From_id = Convert.ToInt32(item.InnerText); break;
                    case "owner_id": post.Owner_id = Convert.ToInt32(item.InnerText); break;
                    case "date": post.Date = Convert.ToInt32(item.InnerText); break;
                    case "post_type": post.Post_type = item.InnerText; break;
                    case "text": post.Text = (item.OuterXml == "<text />" ? "" : item.InnerText); break;
                    case "attachments": getAttachments(item, ref post); break;
                    case "copy_history":
						post.Copied_Post = new Post();
						post.Copied_Post = getPost(item.FirstChild); break;
                    case "likes": post.Likes = Convert.ToInt32(item.FirstChild.InnerText); break;
					case "reposts": post.Reposts = Convert.ToInt32(item.FirstChild.InnerText); break;
                }
            }
            return post;
        }
        void getAttachments(XmlNode node, ref Post post)
        {
            if (node.Attributes[0].Value != "true")
                return;
            foreach (XmlNode item in node.ChildNodes)
            {
                string type = item.FirstChild.InnerText;
                switch (type)
                {
                    case "audio":
                        if (post.Audios == null)
                            post.Audios = new List<Media.Audio>();
						post.Audios.Add(getAudio(item.LastChild)); break;
                    case "doc":
                        if (post.Documents == null)
                            post.Documents = new List<Media.Document>();
						post.Documents.Add(getDocument(item.LastChild)); break;
                    case "photo":
                        if(post.Photos == null)
						    post.Photos = new List<Media.Photo>();
						post.Photos.Add(getPhoto(item.LastChild)); break;
                    case "posted_photo":
                        if (post.Posted_photos == null)
                            post.Posted_photos = new List<Media.Posted_photo>();
						post.Posted_photos.Add(getPosted_photo(item.LastChild)); break;
                    case "video":
                        if (post.Videos == null)
                            post.Videos = new List<Media.Video>();
						post.Videos.Add(getVideo(item.LastChild)); break;
                    case "graffiti":
                        if (post.Graffities == null)
                            post.Graffities = new List<Media.Graffity>();
						post.Graffities.Add(getGraffity(item.LastChild)); break;
                    case "link":
                        if (post.Links == null)
                            post.Links = new List<Media.Link>();
						post.Links.Add(getLink(item.LastChild)); break;
                    case "node":
                        if (post.Nodes == null)
                            post.Nodes = new List<Media.Node>();
						post.Nodes.Add(getNode(item.LastChild)); break;
                    case "poll":
                        if (post.Poll == null)
                            post.Poll = new Media.Poll();
						post.Poll = getPoll(item.LastChild); break;
                }
            }
        }

        Media.Audio getAudio(XmlNode node)
        {
            Media.Audio audio = new Media.Audio();
            foreach (XmlNode item in node)
            {
                switch (item.Name)
                {
                    case "id": audio.Id = Convert.ToInt32(item.InnerText); break;
                    case "owner_id": audio.Owner_id = Convert.ToInt32(item.InnerText); break;
                    case "duration": audio.Duration = Convert.ToInt32(item.InnerText); break;
                    case "date": audio.Date = Convert.ToInt32(item.InnerText); break;
                    case "artist": audio.Artist = item.InnerText; break;
                    case "title": audio.Title = (item.OuterXml == "<title />" ? "" : item.InnerText); break;
                    case "url": audio.Url = (item.OuterXml == "<url />" ? "" : item.InnerText); break;
                }
            }
            return audio;
        }
        Media.Document getDocument(XmlNode node)
        {
            Media.Document document = new Media.Document();
            foreach (XmlNode item in node)
            {
                switch (item.Name)
                {
                    case "id": document.Id = Convert.ToInt32(item.InnerText); break;
                    case "owner_id": document.Owner_id = Convert.ToInt32(item.InnerText); break;
                    case "size": document.Size = Convert.ToInt32(item.InnerText); break;
                    case "date": document.Date = Convert.ToInt32(item.InnerText); break;
                    case "type": document.Type = Convert.ToInt32(item.InnerText); break;
                    case "ext": document.Ext = item.InnerText; break;
                    case "title": document.Title = (item.OuterXml == "<title />" ? "" : item.InnerText); break;
                    case "url": document.Url = (item.OuterXml == "<url />" ? "" : item.InnerText); break;
                    case "photo_100": document.Photo_100 = item.InnerText; break;
                    case "photo_130": document.Photo_130 = item.InnerText; break;
                }
            }
            return document;
        }
        Media.Photo getPhoto(XmlNode node)
        {
            Media.Photo photo = new Media.Photo();
            foreach (XmlNode item in node)
            {
                switch (item.Name)
                {
                    case "id": photo.Id = Convert.ToInt32(item.InnerText); break;
                    case "owner_id": photo.Owner_id = Convert.ToInt32(item.InnerText); break;
                    case "album_id": photo.Album_id = Convert.ToInt32(item.InnerText); break;
                    case "date": photo.Date = Convert.ToInt32(item.InnerText); break;
                    case "user_id":
                        photo.User_id = Convert.ToInt32(item.InnerText);
                        if (photo.User_id == 100)
                            photo.User_id = photo.Owner_id;
                        break;
                    case "text": photo.Text = (item.OuterXml == "<text />" ? "" : item.InnerText); break;
                    case "photo_75": photo.Photo_75 = item.InnerText; break;
                    case "photo_130": photo.Photo_130 = item.InnerText; break;
                    case "photo_604": photo.Photo_604 = item.InnerText; break;
                    case "photo_807": photo.Photo_807 = item.InnerText; break;
                    case "photo_1280": photo.Photo_1280 = item.InnerText; break;
                    case "photo_2560": photo.Photo_2560 = item.InnerText; break;
                }
            }
            return photo;
        }
        Media.Posted_photo getPosted_photo(XmlNode node)
        {
            Media.Posted_photo photo = new Media.Posted_photo();
            foreach (XmlNode item in node)
            {
                switch (item.Name)
                {
                    case "id": photo.Id = Convert.ToInt32(item.InnerText); break;
                    case "owner_id": photo.Owner_id = Convert.ToInt32(item.InnerText); break;
                    case "photo_130": photo.Photo_130 = item.InnerText; break;
                    case "photo_604": photo.Photo_604 = item.InnerText; break;
                }
            }

            return photo;
        }
        Media.Video getVideo(XmlNode node)
        {
            Media.Video video = new Media.Video();
            foreach (XmlNode item in node)
            {
                switch (item.Name)
                {
                    case "id": video.Id = Convert.ToInt32(item.InnerText); break;
                    case "owner_id": video.Owner_id = Convert.ToInt32(item.InnerText); break;
                    case "duration": video.Duration = Convert.ToInt32(item.InnerText); break;
                    case "views": video.Views = Convert.ToInt32(item.InnerText); break;
                    case "date": video.Date = Convert.ToInt32(item.InnerText); break;
                    case "description": video.Description = (item.OuterXml == "<description />" ? "" : item.InnerText); break;
                    case "photo_130": video.Photo_130 = item.InnerText; break;
                    case "photo_320": video.Photo_320 = item.InnerText; break;
                    case "photo_640": video.Photo_640 = item.InnerText; break;
                    case "title": video.Title = (item.OuterXml == "<title />" ? "" : item.InnerText); break;
                    case "player": video.Player = item.InnerText; break;
                    case "access_key": video.Access_key = item.InnerText; break;
                }
            }
            video.Player = getVideoUrl(video);
            return video;
        }
        string getVideoUrl(Media.Video video)
        {
            XmlDocument doc = new XmlDocument();
            do
                doc.LoadXml(get("video.get.xml", "owner_id=" + video.Owner_id + "&videos=" + video.Owner_id + '_' + video.Id + '_' + video.Access_key));
            while (doc.DocumentElement.Name != "response");
            if(doc.DocumentElement.ChildNodes[1].ChildNodes.Count != 0)
            foreach (XmlNode item in doc.DocumentElement.ChildNodes[1].ChildNodes[0].ChildNodes)
                if (item.Name == "player")
                    video.Player = item.FirstChild.Value;
            return video.Player;
        }
        Media.Graffity getGraffity(XmlNode node)
        {
            Media.Graffity graffity = new Media.Graffity();
            foreach (XmlNode item in node)
            {
                switch (item.Name)
                {
                    case "id": graffity.Id = Convert.ToInt32(item.InnerText); break;
                    case "owner_id": graffity.Owner_id = Convert.ToInt32(item.InnerText); break;
                    case "photo_200 ": graffity.Photo_200 = item.InnerText; break;
                    case "photo_586 ": graffity.Photo_586 = item.InnerText; break;
                }
            }
            return graffity;
        }
        Media.Link getLink(XmlNode node)
        {
            Media.Link link = new Media.Link();
            foreach (XmlNode item in node)
            {
                switch (item.Name)
                {
                    case "is_external": link.is_external = Convert.ToBoolean(Convert.ToInt32(item.InnerText)); break;
                    case "caption": link.Caption = item.InnerText; break;
                    case "description": link.Description = (item.OuterXml == "<description />" ? "" : item.InnerText); break;
                    case "photo": link.Photo = getPhoto(item); break;
                    case "url": link.Url = (item.OuterXml == "<url />" ? "" : item.InnerText); break;
                    case "title": link.Title = (item.OuterXml == "<title />" ? "" : item.InnerText); break;
                }
            }
            return link;
        }
        Media.Node getNode(XmlNode node)
        {
            Media.Node _node = new Media.Node();
            foreach (XmlNode item in node)
            {
                switch (item.Name)
                {
                    case "id": _node.Id = Convert.ToInt32(item.InnerText); break;
                    case "owner_id": _node.Owner_id = Convert.ToInt32(item.InnerText); break;
                    case "date": _node.Date = Convert.ToInt32(item.InnerText); break;
                    case "text": _node.Text = (item.OuterXml == "<text />" ? "" : item.InnerText); break;
                    case "title": _node.Title = (item.OuterXml == "<title />" ? "" : item.InnerText); break;
                }
            }
            return _node;
        }
        Media.Poll getPoll(XmlNode node)
        {
            Media.Poll poll = new Media.Poll();
            poll.Answers = new List<Media.Answer>();
            foreach (XmlNode item in node)
            {
                switch (item.Name)
                {
                    case "id": poll.Id = Convert.ToInt32(item.InnerText); break;
                    case "owner_id": poll.Owner_id = Convert.ToInt32(item.InnerText); break;
                    case "votes": poll.Votes = Convert.ToInt32(item.InnerText); break;
                    case "answer_id": poll.Answer_id = Convert.ToInt32(item.InnerText); break;
                    case "question": poll.Question = item.InnerText; break;
                    case "answers":
                        foreach (XmlNode it in item.ChildNodes)
                        {
                            Media.Answer answer = new Media.Answer();
                            foreach (XmlNode i in it.ChildNodes)
                            {
                                switch (i.Name)
                                {
                                    case "id": answer.Id = Convert.ToInt32(i.InnerText); break;
                                    case "rate": answer.Rate = Convert.ToDouble(i.InnerText.Replace('.', ',')); break;
                                    case "votes": answer.Votes = Convert.ToUInt32(i.InnerText); break;
                                    case "text": answer.Text = (i.OuterXml == "<text />" ? "" : i.InnerText); break;
                                }
                            }
                            poll.Answers.Add(answer);
                        }
                        break;
                }
            }
            return poll;
        }
        #endregion

        const string            Url_Api = "https://api.vk.com/method/";
        const string            version_api = "v=5.52";
        int                     client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        int                     user_id;
        string                  access_token;
    }

    public class Methods
    {
        public class Users
        {
            /// <summary>
            /// Возвращает расширенную информацию о пользователях JSON
            /// </summary>
            public static string Get { get { return "users.get"; } }
            /// <summary>
            ///  Возвращает расширенную информацию о пользователях XML
            /// </summary>
            public static string Get_Xml { get { return "users.get.xml"; } }
            /// <summary>
            /// Возвращает список пользователей в соответствии с заданным критерием поиска JSON
            /// </summary>
            public static string Search { get { return "users.search"; } }
            /// <summary>
            /// Возвращает список пользователей в соответствии с заданным критерием поиска XML
            /// </summary>
            public static string Search_Xml { get { return "users.search.xml"; } }
            /// <summary>
            /// Возвращает информацию о том, установил ли пользователь приложение JSON
            /// </summary>
            public static string isAppUser { get { return "users.isAppUser"; } }
            /// <summary>
            /// Возвращает информацию о том, установил ли пользователь приложение XML
            /// </summary>
            public static string isAppUser_Xml { get { return "users.isAppUser.xml"; } }
            /// <summary>
            /// Возвращает список идентификаторов пользователей и сообществ, которые входят в список подписок пользователя JSON
            /// </summary>
            public static string getSubscriptions { get { return "users.getSubscriptions"; } }
            /// <summary>
            /// Возвращает список идентификаторов пользователей и сообществ, которые входят в список подписок пользователя XML
            /// </summary>
            public static string getSubscriptions_Xml { get { return "users.getSubscriptions.xml"; } }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя JSON
            /// </summary>
            public static string getFollowers { get { return "users.getFollowers"; } }
            /// <summary>
            /// Возвращает список идентификаторов пользователей, которые являются подписчиками пользователя XML
            /// </summary>
            public static string getFollowers_Xml { get { return "users.getFollowers.xml"; } }
            /// <summary>
            /// Позволяет пожаловаться на пользователя JSON
            /// </summary>
            public static string report { get { return "users.report"; } }
            /// <summary>
            /// Позволяет пожаловаться на пользователя XML
            /// </summary>
            public static string report_Xml { get { return "users.report.xml"; } }
            /// <summary>
            /// Индексирует текущее местоположение пользователя и возвращает список пользователей, которые находятся вблизи JSON
            /// </summary>
            public static string getNearby { get { return "users.getNearby"; } }
            /// <summary>
            /// Индексирует текущее местоположение пользователя и возвращает список пользователей, которые находятся вблизи XML
            /// </summary>
            public static string getNearby_Xml { get { return "users.getNearby.xml"; } }
        }
        public class Wall
        {
            /// <summary>
            /// Возвращает список записей со стены пользователя или сообщества JSON
            /// </summary>
            public static string Get { get { return "wall.get"; } }
            /// <summary>
            /// Возвращает список записей со стены пользователя или сообщества XML
            /// </summary>
            public static string Get_Xml { get { return "wall.get.xml"; } }
            /// <summary>
            /// Метод, позволяющий осуществлять поиск по стенам пользователей JSON
            /// </summary>
            public static string Search { get { return "wall.search"; } }
            /// <summary>
            /// Метод, позволяющий осуществлять поиск по стенам пользователей XML
            /// </summary>
            public static string Search_Xml { get { return "wall.search.xml"; } }
            /// <summary>
            /// Возвращает список записей со стен пользователей или сообществ по их идентификаторам JSON
            /// </summary>
            public static string GetById { get { return "wall.getById"; } }
            /// <summary>
            /// Возвращает список записей со стен пользователей или сообществ по их идентификаторам XML
            /// </summary>
            public static string GetById_Xml { get { return "wall.getById.xml"; } }

        }
        public class Friends
        {
            /// <summary>
            /// Возвращает список идентификаторов друзей пользователя или расширенную информацию о друзьях пользователя JSON
            /// </summary>
            public static string Get { get { return "friends.get"; } }
            /// <summary>
			/// Возвращает список идентификаторов друзей пользователя или расширенную информацию о друзьях пользователя XML
			/// </summary>
			public static string Get_Xml { get { return "friends.get.xml"; } }


        }
        public class Likes
        {
            /// <summary>
			/// Получает список идентификаторов пользователей, которые добавили заданный объект в свой список Мне нравится JSON
			/// </summary>
			public static string GetList { get { return "likes.getList"; } }
            /// <summary>
			/// Получает список идентификаторов пользователей, которые добавили заданный объект в свой список Мне нравится XML
			/// </summary>
			public static string GetList_Xml { get { return "likes.getList.xml"; } }
        }
    }
}
