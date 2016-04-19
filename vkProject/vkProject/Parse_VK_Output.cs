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
                            case "id":              id      = Convert.ToUInt32(it.FirstChild.Value);    break;
                            case "first_name":      name    = it.FirstChild.Value;                      break;
                            case "second_name":     surname = it.FirstChild.Value;                      break;
                        }
                    }
                    Friends.Add(new VkAPI.User(id, name, surname));
                }
            }
        }
        void getWall()
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
                            Wall.Add(getPost(item));
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
                    case "attachments":         getAttachments(item, ref post);                     break;
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
                switch(type)
                {
                    case "audio":           post.Audios.Add(getAudio(item));                break;
                    case "doc":             post.Documents.Add(getDocument(item));          break;
                    case "photo":           post.Photos.Add(getPhoto(item));                break;
                    case "posted_photo":    post.Posted_photos.Add(getPosted_photo(item));  break;
                    case "video":           post.Videos.Add(getVideo(item));                break;
                    case "graffiti":        post.Graffities.Add(getGraffity(item));         break;
                    case "link":            post.Links.Add(getLink(item));                  break;
                    case "node":            post.Nodes.Add(getNode(item));                  break;
                    case "poll":            post.Poll = getPoll(item);                      break;
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
                    case "id":          audio.Id        = Convert.ToUInt32(item.Value); break;
                    case "owner_id":    audio.Owner_id  = Convert.ToUInt32(item.Value); break;
                    case "duration":    audio.Duration  = Convert.ToUInt32(item.Value); break;
                    case "date":        audio.Date      = Convert.ToUInt32(item.Value); break;
                    case "artist":      audio.Artist    = item.Value; break;
                    case "title":       audio.Title     = item.Value; break;
                    case "url":         audio.Url       = item.Value; break;
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
                    case "id":          document.Id         = Convert.ToUInt32(item.Value); break;
                    case "owner_id":    document.Owner_id   = Convert.ToUInt32(item.Value); break;
                    case "size":        document.Size       = Convert.ToUInt32(item.Value); break;
                    case "date":        document.Date       = Convert.ToUInt32(item.Value); break;
                    case "type":        document.Type       = Convert.ToUInt32(item.Value); break;
                    case "ext":         document.Ext        = item.Value; break;
                    case "title":       document.Title      = item.Value; break;
                    case "url":         document.Url        = item.Value; break;
                    case "photo_100":   document.Photo_100  = item.Value; break;
                    case "photo_130":   document.Photo_130  = item.Value; break;
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
                    case "id":              photo.Id            = Convert.ToUInt32(item.Value); break;
                    case "owner_id":        photo.Owner_id      = Convert.ToUInt32(item.Value); break;
                    case "album_id":        photo.Album_id      = Convert.ToUInt32(item.Value); break;
                    case "date":            photo.Date          = Convert.ToUInt32(item.Value); break;
                    case "width":           photo.Width         = Convert.ToUInt32(item.Value); break;
                    case "height":          photo.Heght         = Convert.ToUInt32(item.Value); break;
                    case "user_id":         photo.User_id       = Convert.ToUInt32(item.Value);
                                                if (photo.User_id == 100)
                                                    photo.User_id = photo.Owner_id;
                                                                                                break;
                    case "text":            photo.Text          = item.Value; break;
                    case "photo_75":        photo.Photo_75      = item.Value; break;
                    case "photo_130":       photo.Photo_130     = item.Value; break;
                    case "photo_604":       photo.Photo_604     = item.Value; break;
                    case "photo_807":       photo.Photo_807     = item.Value; break;
                    case "photo_1280":      photo.Photo_1280    = item.Value; break;
                    case "photo_2560":      photo.Photo_2560    = item.Value; break;
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
                    case "id":          photo.Id            = Convert.ToUInt32(item.Value); break;
                    case "owner_id":    photo.Owner_id      = Convert.ToUInt32(item.Value); break;
                    case "photo_130":   photo.Photo_130     = item.Value; break;
                    case "photo_604":   photo.Photo_604     = item.Value; break;
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
                    case "id":              video.Id            = Convert.ToUInt32(item.Value); break;
                    case "owner_id":        video.Owner_id      = Convert.ToUInt32(item.Value); break;
                    case "duration":        video.Duration      = Convert.ToUInt32(item.Value); break;
                    case "views":           video.Views         = Convert.ToUInt32(item.Value); break;
                    case "date":            video.Date          = Convert.ToUInt32(item.Value); break;
                    case "description":     video.Desription    = item.Value; break;
                    case "photo_130":       video.Photo_130     = item.Value; break;
                    case "photo_320":       video.Photo_320     = item.Value; break;
                    case "photo_640":       video.Photo_640     = item.Value; break;
                    case "title":           video.Title         = item.Value; break;
                    case "player":          video.Player        = item.Value; break;
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
                    case "photo_586 ": graffity.Photo_586     = item.Value; break;
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
                    case "is_external ":        link.is_external        = Convert.ToBoolean(Convert.ToInt32(item.Value)); break;
                    case "caption":             link.Caption            = item.Value;       break;
                    case "description ":        link.Description        = item.Value;       break;
                    case "photo":               link.Photo              = getPhoto(item);   break;
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
                    case "id":              poll.Id = Convert.ToUInt32(item.Value);                     break;
                    case "owner_id":        poll.Owner_id = Convert.ToUInt32(item.Value);               break;
                    case "votes":           poll.Votes = Convert.ToUInt32(item.Value);                  break;
                    case "answer_id":       poll.Answer_id = Convert.ToUInt32(item.Value);              break;
                    case "created":         poll.Created = Convert.ToUInt32(item.Value);                break;
                    case "question":        poll.Question = item.Value;                                 break;
                    case "answers":         
                                    foreach (XmlNode it in item.ChildNodes)
                                    {
                                        Media.Answer answer = new Media.Answer();
                                        foreach (XmlNode i in it.ChildNodes)
                                        {
                                            switch (it.Name)
                                            {
                                                case "id": answer.Id = Convert.ToUInt32(i.Value);       break;
                                                case "rate": answer.Rate = Convert.ToUInt32(i.Value);   break;
                                                case "votes": answer.Votes = Convert.ToUInt32(i.Value); break;
                                                case "text": answer.Text = item.Value;                  break;
                                            }
                                        }
                                        poll.Answers.Add(answer);
                                    }
                                                                                                        break;
                }
            }
            return poll;
        }


        VkAPI.vkAPI api;
        public List<VkAPI.User> Friends     { get; private set; }
        public List<VkAPI.Post> Wall        { get; private set; }
    }
}
