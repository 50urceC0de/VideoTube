using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoTube.Models
{
    public class Videos
    {
        public int id { get; set; }
        public string uploadedBy { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string privacy { get; set; }
        public string filePath { get; set; }
        public string category { get; set; }
        public string thumbnail { get; set; }
        public DateTime uploadDate { get; set; }
        public int views { get; set; }
        public string duration { get; set; }
    }
}