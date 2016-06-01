using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace VkAPI
{
    // Класс-обертка vkAPI
	public class ParseVkOutput
	{
        class Comp : IComparer<User>
        {
            public int Compare(User a, User b)
            {
                return a.Last_name.CompareTo(b.Last_name);
            }
        }

        public ParseVkOutput(vkAPI api)
		{
			this.api = api;
		}

		public List<User> getFriends()
		{
            List<User> Friends = api.getFriends();
            return Friends;
		}
        public KeyValuePair<Dictionary<int, User>, List<Post>> getWall()
		{
            var Wall = api.getWall();
            return Wall;
		}
        public List<KeyValuePair<int, User>> getLikes(List<Post> Wall)
        {
            SortedDictionary<User, int> stat = new SortedDictionary<User, int>(new Comp());

            foreach (var item in Wall)
            {
                if (item.Likes == 0)
                    continue;
                List<User> likes = api.getLikes(item);
                foreach (var it in likes)
                {
                    if(stat.ContainsKey(it))
                        stat[it]++;
                    else
                        stat.Add(it, 1);
                }
            }

            List<KeyValuePair<int, User>> whoLiked = new List<KeyValuePair<int, User>>();

            foreach (var item in stat)
                whoLiked.Add(new KeyValuePair<int, User>(item.Value, item.Key));

            whoLiked.Sort(Comparer<KeyValuePair<int, User>>.Create((a, b) => b.Key.CompareTo(a.Key)));

            return whoLiked;
        }
        public string getVideoUrl(Media.Video video)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(api.get("video.get.xml", "owner_id=" + video.Owner_id + "videos=" + video.Owner_id + '_' + video.Id + '_' + video.Access_key));
            foreach (XmlNode item in doc.DocumentElement.ChildNodes[1].ChildNodes)
                if (item.Name == "player")
                    video.Player = item.FirstChild.Value;
            return video.Player;
        }

        vkAPI api;
	}
}