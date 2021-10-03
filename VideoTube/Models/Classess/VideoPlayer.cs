using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Data;

namespace VideoTube.Data
{
    public class VideoPlayer
    {

        private Video video;

        public VideoPlayer(Video video)
        {
            this.video = video;
        }

        public string create(bool autoPlay)
        {
            string _autoPlay = "";
            if (!autoPlay)
            {
                _autoPlay = "autoplay";
            }
            else
            {
                _autoPlay = "";
            }
            string filePath = this.video._video.filePath;
           // return "<video class='videoPlayer' controls "+ _autoPlay + ">  <source src = '"+filePath+"' type = 'video/mp4' >   Your browser does not support the video tag  </video > ";
            return "<video class='videoPlayer' controls " + _autoPlay + "> Your browser does not support the video tag  </video > ";
        }

    }
}