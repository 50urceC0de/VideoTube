using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoTube.Models
{
    public class Comments
    {
        public int id { get; set; }
        public int videoId { get; set; }
        public string body { get; set; }
        public string postedBy { get; set; }
        public string responseTo { get; set; }
        public string datePosted { get; set; }
    }
}