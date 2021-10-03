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
    public class TrendingProvider
    {

        private IConnectionConfiguration con;
        User userLoggedInObj;

        public TrendingProvider(IConnectionConfiguration con, User userLoggedInObj)
        {
            this.con = con;
            this.userLoggedInObj = userLoggedInObj;
        }

        public async Task<List<Video>> getVideos()
        {
            List<Video> _videos = new List<Video>();
            string query = ("SELECT top(15) * FROM videos WHERE uploadDate between getdate() - 7 and getdate() ORDER BY views DESC");
            IEnumerable<Videos> videos;
            using (var conn = new SqlConnection(con.Value))
            {
                videos = await conn.QueryAsync<Videos>(query, commandType: CommandType.Text);
            }
            foreach (var item in videos)
            {
                Video video = new Video(this.con, item, this.userLoggedInObj);
                _videos.Add(video);
            }

            return _videos;
        }
    }
}