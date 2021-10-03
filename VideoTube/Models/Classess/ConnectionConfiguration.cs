using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using VideoTube.Data.Services;

namespace VideoTube.Data
{
    public class ConnectionConfiguration : IConnectionConfiguration
    {
        public ConnectionConfiguration() => Value = ConfigurationManager.ConnectionStrings["videodb"].ConnectionString;
        public string Value { get; }
    }
}