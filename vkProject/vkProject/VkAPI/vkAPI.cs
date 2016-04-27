using System;
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
    public class vkAPI
    {
        public vkAPI(string access_token, int user_id, Scope scope)
        {
            this.user_id = user_id;
            this.access_token = access_token;
            this.scope = scope;
        }
        public string get(string method, string data)
        {
            WebRequest req = WebRequest.Create(Url_Api + method + '?' + data + "&v=5.50&access_token=" + access_token);
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }
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
                        string name = "", surname = "";
                        foreach (XmlNode it in item.ChildNodes)
                        {
                            switch (it.Name)
                            {
                                case "id": id = Convert.ToInt32(it.InnerText); break;
                                case "first_name": name = it.InnerText; break;
                                case "last_name": surname = it.InnerText; break;
                            }
                        }
                        Friends.Add(new User(id, name, surname));
                    }
                }
                count -= 5000;
                offset += 5000;
            }
			Global.WriteLogString("Friends had been got");

			return Friends;
        }
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

                foreach (XmlNode item in doc.DocumentElement.ChildNodes[1])
                    if (item.Name == "user")
                    {
                        int id = 0;
                        string name = null, surname = null;
                        foreach (XmlNode it in item.ChildNodes)
                        {
                            switch(it.Name)
                            {
                                case "id": id = Convert.ToInt32(it.InnerText); break;
                                case "first_name": name = it.InnerText; break;
                                case "last_name": surname = it.InnerText; break;
                            }
                        }
                        whoLiked.Add(new User(id, name, surname));
                    }
                count -= 100;
                offset += 100;
            }
            return whoLiked;
        }
        public List<Post> getWall()
        {
            List<Post> Wall = new List<Post>();
            XmlDocument doc = new XmlDocument();

            do
                doc.LoadXml(this.get(Methods.Wall.Get_Xml, "count=1"));
            while (doc.DocumentElement.Name != "response");

            int count_of_posts = Convert.ToInt32(doc.DocumentElement.FirstChild.InnerText), offset = 0;

            while (count_of_posts > 0)
            {
                do
                    doc.LoadXml(this.get(Methods.Wall.Get_Xml, "count=100&offset=" + offset.ToString()));
                while (doc.DocumentElement.Name != "response");
                offset += 100;
                count_of_posts -= 100;
                getPosts(doc, ref Wall);
            }
            return Wall;
        }

        #region GetWallHelpFunctions
        void getPosts(XmlDocument doc, ref List<VkAPI.Post> Wall)
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
                    case "copy_history": post.Copied_Post = getPost(item.FirstChild); break;
                    case "likes": post.Likes = Convert.ToInt32(item.FirstChild.InnerText); break;
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
                    case "audio": post.Audios.Add(getAudio(item.LastChild)); break;
                    case "doc": post.Documents.Add(getDocument(item.LastChild)); break;
                    case "photo": post.Photos.Add(getPhoto(item.LastChild)); break;
                    case "posted_photo": post.Posted_photos.Add(getPosted_photo(item.LastChild)); break;
                    case "video": post.Videos.Add(getVideo(item.LastChild)); break;
                    case "graffiti": post.Graffities.Add(getGraffity(item.LastChild)); break;
                    case "link": post.Links.Add(getLink(item.LastChild)); break;
                    case "node": post.Nodes.Add(getNode(item.LastChild)); break;
                    case "poll": post.Poll = getPoll(item.LastChild); break;
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
                    case "width": photo.Width = Convert.ToInt32(item.InnerText); break;
                    case "height": photo.Heght = Convert.ToInt32(item.InnerText); break;
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
            return video;
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
                    case "is_external ": link.is_external = Convert.ToBoolean(Convert.ToInt32(item.InnerText)); break;
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
                                switch (it.Name)
                                {
                                    case "id": answer.Id = Convert.ToInt32(i.Value); break;
                                    case "rate": answer.Rate = Convert.ToInt32(i.Value); break;
                                    case "votes": answer.Votes = Convert.ToUInt32(i.Value); break;
                                    case "text": answer.Text = (item.OuterXml == "<text />" ? "" : item.InnerText); break;
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
        int                     client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        int                     user_id;
        string                  access_token;
        Scope                   scope;
    }

    public class Scope
    {
        public bool friends         { get; set; }
        public bool photos          { get; set; }
        public bool audio           { get; set; }
        public bool video           { get; set; }
        public bool docs            { get; set; }
        public bool notes           { get; set; }
        public bool pages           { get; set; }
        public bool status          { get; set; }
        public bool wall            { get; set; }
        public bool groups          { get; set; }
        public bool messages        { get; set; }
        public bool notifications   { get; set; }
        public bool offline         { get; set; }
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
