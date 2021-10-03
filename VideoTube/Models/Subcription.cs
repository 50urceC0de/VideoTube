using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoTube.Models
{
    public class Subcription
    {
        public int id { get; set; }
        public string userTo { get; set; }
        public string userFrom { get; set; }
    }
}