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
        Parse_Vk_Output(vkAPI api)
        {
            this.api = api;
            getFriends();
        }

        void getFriends()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(api.get(VkAPI.Methods.Friends.Get_Xml, ""));
            foreach (XmlNode item in doc.DocumentElement)
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
                            case "second_name": surname = it.FirstChild.Value; break;
                        }
                    }
                    Friends.Add(new VkAPI.User(id, name, surname));
                }
            }
        }
        void getPostsDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(api.get(VkAPI.Methods.Wall.Get_Xml, "count=1"));
            int count_of_posts = Convert.ToInt32(doc.DocumentElement.FirstChild.Value), offset = 0;
            while (count_of_posts > 0)
            {
                doc.LoadXml(api.get(VkAPI.Methods.Wall.Get_Xml, "count=100&offset=" + offset.ToString()));
                offset += 100;
                count_of_posts -= 100;
                getPosts(doc);
            }
        }
        void getPosts(XmlDocument doc)
        {
            foreach (XmlNode it in doc.DocumentElement)
                if (it.Name == "items")
                {
                    if (it.Attributes == null || it.Attributes[0].Value != "true")
                        break;
                    foreach (XmlNode item in it.ChildNodes)
                        if (item.Name == "post")
                            Posts.Add(getPost(item));
                }
        }
        Post getPost(XmlNode Node)
        {
            Post post = new Post();
            foreach (XmlNode item in Node.ChildNodes)
            {
                switch(item.Name)
                {
                    case "id":                  post.Id = Convert.ToUInt32(item.Value);             break;
                    case "from_id":             post.From_id = Convert.ToUInt32(item.Value);        break;
                    case "owner_id":            post.Owner_id = Convert.ToUInt32(item.Value);       break;
                    case "date":                post.Date = Convert.ToUInt32(item.Value);           break;
                    case "post_type":           post.Post_type = item.Value;                        break;
                    case "text":                post.Text = item.Value;                             break;
                    case "attachments":
                        getAttachments(item, out post);
                        break;
                }
            }
            return new Post();
        }
        void getAttachments(XmlNode node, out Post post)
        {

            post = new Post();
        }

        VkAPI.vkAPI api;
        public List<VkAPI.User> Friends { get; private set; }
        public List<VkAPI.Post> Posts { get; private set; }
    }
}
