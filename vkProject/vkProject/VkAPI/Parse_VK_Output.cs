using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace VkAPI
{
	public class Parse_Vk_Output
	{
        class Comp : IComparer<User>
    {
        public int Compare(User a, User b)
        {
            return a.Last_name.CompareTo(b.Last_name);
        }
    }

        public Parse_Vk_Output(vkAPI api)
		{
			this.api = api;
		}

		public void getFriends()
		{
            Friends = api.getFriends();
		}
        public void getWall()
		{
            Wall = api.getWall();
		}
        public void getLikes()
        {
            whoLiked = new SortedDictionary<VkAPI.User, int>(new Comp());

            foreach (var item in Wall)
            {
                if (item.Likes == 0)
                    continue;
                List<VkAPI.User> likes = api.getLikes(item);
                foreach (var it in likes)
                {
                    if(whoLiked.ContainsKey(it))
                        whoLiked[it]++;
                    else
                        whoLiked.Add(it, 1);
                }
            }
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

        VkAPI.vkAPI api;
		public List<VkAPI.User> Friends { get; private set; }
        public SortedDictionary<VkAPI.User, int> whoLiked { get; private set; }
		public List<VkAPI.Post> Wall { get; private set; }
	}
}