using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoTube.Models
{
    public class Thumbnail
    {
        public int id { get; set; }
        public string filePath { get; set; }
        public string videoId { get; set; }
        public int selected { get; set; }
    }
}