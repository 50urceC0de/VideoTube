using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Data;
using VideoTube.Models;
using VideoTube.Data.Services;

namespace VideoTube.Data
{
   public class LikedVideosProvider {

        private IConnectionConfiguration con;
        private User userLoggedInObj;

    public  LikedVideosProvider(IConnectionConfiguration con, User userLoggedInObj) {
        this.con = con;
        this.userLoggedInObj = userLoggedInObj;
        }

        public async Task<List<Video>> getVideos() {
        var  videos = new List<Video>();
           string username = this.userLoggedInObj.getUsername();

            string query = ("SELECT id=videoId FROM likes WHERE username='"+username+"' AND commentId=0  ORDER BY id DESC");
            IEnumerable<Videos> _videos;
            using (var conn = new SqlConnection(con.Value))
            {
                _videos = await conn.QueryAsync<Videos>(query, commandType: CommandType.Text);
            }

            foreach (var row in _videos)
            {
              videos.Add(new Video(this.con, row.id, this.userLoggedInObj));
            }

            return videos;
        }
    }
}