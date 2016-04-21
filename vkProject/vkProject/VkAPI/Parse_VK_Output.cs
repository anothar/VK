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
        public void getFuns()
        {
            foreach (var item in Wall)
            {
                List<VkAPI.User> likers = api.getLikes(item);
                foreach (var it in likers)
                {
                    
                }
            }
        }
        publi

		VkAPI.vkAPI api;
		public List<VkAPI.User> Friends { get; private set; }
        public SortedDictionary<VkAPI.User, uint> whoLiked { get; private set; }
		public List<VkAPI.Post> Wall { get; private set; }
	}
}