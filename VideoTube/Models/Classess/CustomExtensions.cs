using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoTube.Data
{

    public static class StringExtension
    {
        public static bool isNullOrEmpty(this String str)
        {
            return (str == null || str == "") ? true : false;
        }
    }
}