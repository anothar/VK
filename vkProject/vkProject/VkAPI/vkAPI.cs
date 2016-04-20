using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Configuration;
using System.Net;

namespace VkAPI
{
    public class vkAPI
    {
        public vkAPI(string access_token, uint user_id, Scope scope)
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
        public List<VkAPI.User> getFriends()
        {
            List<VkAPI.User> Friends = new List<VkAPI.User>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.get(VkAPI.Methods.Friends.Get_Xml, ""));
            int count = Convert.ToInt32(doc.DocumentElement.FirstChild.FirstChild.Value), offset = 0;
            while (count > 0)
            {
                doc.LoadXml(this.get(VkAPI.Methods.Friends.Get_Xml, "fields=online&offset=" + offset.ToString()));
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
                        uint id = 0;
                        string name = "", surname = "";
                        foreach (XmlNode it in item.ChildNodes)
                        {
                            switch (it.Name)
                            {
                                case "id": id = Convert.ToUInt32(it.FirstChild.Value); break;
                                case "first_name": name = it.FirstChild.Value; break;
                                case "last_name": surname = it.FirstChild.Value; break;
                            }
                        }
                        Friends.Add(new VkAPI.User(id, name, surname));
                    }
                }
                count -= 5000;
                offset += 5000;
            }
            return Friends;
        }
        public List<VkAPI.Post> getWall()
        {
            List<VkAPI.Post> Wall = new List<VkAPI.Post>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.get(VkAPI.Methods.Wall.Get_Xml, "count=1"));
            int count_of_posts = Convert.ToInt32(doc.DocumentElement.FirstChild.Value), offset = 0;
            while (count_of_posts > 0)
            {
                doc.LoadXml(this.get(VkAPI.Methods.Wall.Get_Xml, "count=100&offset=" + offset.ToString()));
                offset += 100;
                count_of_posts -= 100;
                getPosts(doc,ref Wall);
            }
            return Wall;
        }
        public List<VkAPI.User> getLikes(VkAPI.Post post)
        {
            List<VkAPI.User> whoLiked = new List<VkAPI.User>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.get(VkAPI.Methods.Likes.GetList_Xml, "type=post&friends_only=1&owner_id=" + post.Owner_id + "&item_id" + post.Id));
            int count = Convert.ToInt32(doc.DocumentElement.FirstChild.Value), offset = 0;
            while(count > 0)
            {
                doc.LoadXml(this.get(VkAPI.Methods.Likes.GetList_Xml, "type=post&friends_only=1&owner_id=" + post.Owner_id + "&item_id" + post.Id + "&count=100&offset=" + offset.ToString()));
                foreach (XmlNode item in doc.DocumentElement.ChildNodes[1])
                    if (item.Name == "user_id")
                        whoLiked.Add(new VkAPI.User(Convert.ToUInt32(item.Value), null, null));
                count -= 100;
                offset += 100;
            }
            return whoLiked;
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
                    case "id": post.Id = Convert.ToUInt32(item.Value); break;
                    case "from_id": post.From_id = Convert.ToUInt32(item.Value); break;
                    case "owner_id": post.Owner_id = Convert.ToUInt32(item.Value); break;
                    case "date": post.Date = Convert.ToUInt32(item.Value); break;
                    case "post_type": post.Post_type = item.Value; break;
                    case "text": post.Text = item.Value; break;
                    case "attachments": getAttachments(item, ref post); break;
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
                string type = item.FirstChild.Value;
                switch (type)
                {
                    case "audio": post.Audios.Add(getAudio(item)); break;
                    case "doc": post.Documents.Add(getDocument(item)); break;
                    case "photo": post.Photos.Add(getPhoto(item)); break;
                    case "posted_photo": post.Posted_photos.Add(getPosted_photo(item)); break;
                    case "video": post.Videos.Add(getVideo(item)); break;
                    case "graffiti": post.Graffities.Add(getGraffity(item)); break;
                    case "link": post.Links.Add(getLink(item)); break;
                    case "node": post.Nodes.Add(getNode(item)); break;
                    case "poll": post.Poll = getPoll(item); break;
                }
            }
        }

        Media.Audio getAudio(XmlNode node)
        {
            Media.Audio audio = new Media.Audio();
            XmlNode head = node.ChildNodes[1];
            foreach (XmlNode item in head.ChildNodes)
            {
                switch (item.Name)
                {
                    case "id": audio.Id = Convert.ToUInt32(item.Value); break;
                    case "owner_id": audio.Owner_id = Convert.ToUInt32(item.Value); break;
                    case "duration": audio.Duration = Convert.ToUInt32(item.Value); break;
                    case "date": audio.Date = Convert.ToUInt32(item.Value); break;
                    case "artist": audio.Artist = item.Value; break;
                    case "title": audio.Title = item.Value; break;
                    case "url": audio.Url = item.Value; break;
                }
            }
            return audio;
        }
        Media.Document getDocument(XmlNode node)
        {
            Media.Document document = new Media.Document();
            XmlNode head = node.ChildNodes[1];
            foreach (XmlNode item in head.ChildNodes)
            {
                switch (item.Name)
                {
                    case "id": document.Id = Convert.ToUInt32(item.Value); break;
                    case "owner_id": document.Owner_id = Convert.ToUInt32(item.Value); break;
                    case "size": document.Size = Convert.ToUInt32(item.Value); break;
                    case "date": document.Date = Convert.ToUInt32(item.Value); break;
                    case "type": document.Type = Convert.ToUInt32(item.Value); break;
                    case "ext": document.Ext = item.Value; break;
                    case "title": document.Title = item.Value; break;
                    case "url": document.Url = item.Value; break;
                    case "photo_100": document.Photo_100 = item.Value; break;
                    case "photo_130": document.Photo_130 = item.Value; break;
                }
            }
            return document;
        }
        Media.Photo getPhoto(XmlNode node)
        {
            Media.Photo photo = new Media.Photo();
            XmlNode head = node.ChildNodes[1];
            foreach (XmlNode item in head.ChildNodes)
            {
                switch (item.Name)
                {
                    case "id": photo.Id = Convert.ToUInt32(item.Value); break;
                    case "owner_id": photo.Owner_id = Convert.ToUInt32(item.Value); break;
                    case "album_id": photo.Album_id = Convert.ToUInt32(item.Value); break;
                    case "date": photo.Date = Convert.ToUInt32(item.Value); break;
                    case "width": photo.Width = Convert.ToUInt32(item.Value); break;
                    case "height": photo.Heght = Convert.ToUInt32(item.Value); break;
                    case "user_id":
                        photo.User_id = Convert.ToUInt32(item.Value);
                        if (photo.User_id == 100)
                            photo.User_id = photo.Owner_id;
                        break;
                    case "text": photo.Text = item.Value; break;
                    case "photo_75": photo.Photo_75 = item.Value; break;
                    case "photo_130": photo.Photo_130 = item.Value; break;
                    case "photo_604": photo.Photo_604 = item.Value; break;
                    case "photo_807": photo.Photo_807 = item.Value; break;
                    case "photo_1280": photo.Photo_1280 = item.Value; break;
                    case "photo_2560": photo.Photo_2560 = item.Value; break;
                }
            }
            return photo;
        }
        Media.Posted_photo getPosted_photo(XmlNode node)
        {
            Media.Posted_photo photo = new Media.Posted_photo();
            XmlNode head = node.ChildNodes[1];
            foreach (XmlNode item in head.ChildNodes)
            {
                switch (item.Name)
                {
                    case "id": photo.Id = Convert.ToUInt32(item.Value); break;
                    case "owner_id": photo.Owner_id = Convert.ToUInt32(item.Value); break;
                    case "photo_130": photo.Photo_130 = item.Value; break;
                    case "photo_604": photo.Photo_604 = item.Value; break;
                }
            }

            return photo;
        }
        Media.Video getVideo(XmlNode node)
        {
            Media.Video video = new Media.Video();
            XmlNode head = node.ChildNodes[1];
            foreach (XmlNode item in head.ChildNodes)
            {
                switch (item.Name)
                {
                    case "id": video.Id = Convert.ToUInt32(item.Value); break;
                    case "owner_id": video.Owner_id = Convert.ToUInt32(item.Value); break;
                    case "duration": video.Duration = Convert.ToUInt32(item.Value); break;
                    case "views": video.Views = Convert.ToUInt32(item.Value); break;
                    case "date": video.Date = Convert.ToUInt32(item.Value); break;
                    case "description": video.Description = item.Value; break;
                    case "photo_130": video.Photo_130 = item.Value; break;
                    case "photo_320": video.Photo_320 = item.Value; break;
                    case "photo_640": video.Photo_640 = item.Value; break;
                    case "title": video.Title = item.Value; break;
                    case "player": video.Player = item.Value; break;
                }
            }
            return video;
        }
        Media.Graffity getGraffity(XmlNode node)
        {
            Media.Graffity graffity = new Media.Graffity();
            XmlNode head = node.ChildNodes[1];
            foreach (XmlNode item in head.ChildNodes)
            {
                switch (item.Name)
                {
                    case "id": graffity.Id = Convert.ToUInt32(item.Value); break;
                    case "owner_id": graffity.Owner_id = Convert.ToUInt32(item.Value); break;
                    case "photo_200 ": graffity.Photo_200 = item.Value; break;
                    case "photo_586 ": graffity.Photo_586 = item.Value; break;
                }
            }
            return graffity;
        }
        Media.Link getLink(XmlNode node)
        {
            Media.Link link = new Media.Link();
            XmlNode head = node.ChildNodes[1];
            foreach (XmlNode item in head.ChildNodes)
            {
                switch (item.Name)
                {
                    case "is_external ": link.is_external = Convert.ToBoolean(Convert.ToInt32(item.Value)); break;
                    case "caption": link.Caption = item.Value; break;
                    case "description ": link.Description = item.Value; break;
                    case "photo": link.Photo = getPhoto(item); break;
                }
            }
            return link;
        }
        Media.Node getNode(XmlNode node)
        {
            Media.Node _node = new Media.Node();
            XmlNode head = node.ChildNodes[1];
            foreach (XmlNode item in head.ChildNodes)
            {
                switch (item.Name)
                {
                    case "id": _node.Id = Convert.ToUInt32(item.Value); break;
                    case "owner_id": _node.Owner_id = Convert.ToUInt32(item.Value); break;
                    case "date": _node.Date = Convert.ToUInt32(item.Value); break;
                    case "text": _node.Text = item.Value; break;
                    case "title ": _node.Title = item.Value; break;
                }
            }
            return _node;
        }
        Media.Poll getPoll(XmlNode node)
        {
            Media.Poll poll = new Media.Poll();
            XmlNode head = node.ChildNodes[1];
            foreach (XmlNode item in head.ChildNodes)
            {
                switch (item.Name)
                {
                    case "id": poll.Id = Convert.ToUInt32(item.Value); break;
                    case "owner_id": poll.Owner_id = Convert.ToUInt32(item.Value); break;
                    case "votes": poll.Votes = Convert.ToUInt32(item.Value); break;
                    case "answer_id": poll.Answer_id = Convert.ToUInt32(item.Value); break;
                    case "created": poll.Created = Convert.ToUInt32(item.Value); break;
                    case "question": poll.Question = item.Value; break;
                    case "answers":
                        foreach (XmlNode it in item.ChildNodes)
                        {
                            Media.Answer answer = new Media.Answer();
                            foreach (XmlNode i in it.ChildNodes)
                            {
                                switch (it.Name)
                                {
                                    case "id": answer.Id = Convert.ToUInt32(i.Value); break;
                                    case "rate": answer.Rate = Convert.ToUInt32(i.Value); break;
                                    case "votes": answer.Votes = Convert.ToUInt32(i.Value); break;
                                    case "text": answer.Text = item.Value; break;
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
        uint                    client_id = Convert.ToUInt32(ConfigurationManager.AppSettings["client_id"]);
        uint                    user_id;
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
