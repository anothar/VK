using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
