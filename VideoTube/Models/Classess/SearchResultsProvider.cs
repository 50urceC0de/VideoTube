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
    class SearchResultsProvider
    {

        private IConnectionConfiguration con;
        private User userLoggedInObj;

        public SearchResultsProvider(IConnectionConfiguration con, User userLoggedInObj)
        {
            this.con = con;
            this.userLoggedInObj = userLoggedInObj;
        }

        public async Task<List<Video>> getVideos(string term, string orderBy)
        {
            string query = ("SELECT * FROM videos WHERE title LIKE ('%'+'" + term + "'+'%')  OR uploadedBy LIKE ('%'+'" + term + "'+'%') ORDER BY " + orderBy + " DESC");
            IEnumerable<Videos> videos;
            using (var conn = new SqlConnection(con.Value))
            {
                videos = await conn.QueryAsync<Videos>(query, commandType: CommandType.Text);
            }

            List<Video> _videos = new List<Video>();
            foreach (var row in videos)
            {
                Video video = new Video(this.con, row, this.userLoggedInObj);
                _videos.Add(video);
            }

            return _videos;

        }

    }
}