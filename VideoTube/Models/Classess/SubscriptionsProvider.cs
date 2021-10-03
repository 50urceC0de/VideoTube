using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Data;
using VideoTube.Data.Services;
using VideoTube.Models;

namespace VideoTube.Data
{
    class SubscriptionsProvider {

        private IConnectionConfiguration con;
        private User userLoggedInObj;

    public  SubscriptionsProvider(IConnectionConfiguration con, User userLoggedInObj) {
        this.con = con;
        this.userLoggedInObj = userLoggedInObj;
        }

        public async Task<List<Video>> getVideos() {
        List<Video> videos =new  List<Video>();
            List<User> subscriptions = this.userLoggedInObj.getSubscriptions();
            if (subscriptions.Count > 0) {
            
            // user1, user2, user3
            // SELECT * FROM videos WHERE uploadedBy = ? OR uploadedBy = ? OR uploadedBy = ? 
            // query.bindParam(1, "user1");
            // query.bindParam(2, "user2");
            // query.bindParam(3, "user3");

            string condition = "";
            int i = 0;

                while (i < subscriptions.Count) {

                    if (i == 0) {
                    condition+= "WHERE uploadedBy=?";
                    }
                else {
                    condition+= " OR uploadedBy=?";
                    }
                i++;
                }

            string videoSql = "SELECT * FROM videos condition ORDER BY uploadDate DESC";
              
            i = 1;

                foreach (var sub in subscriptions  ) {

                string subUsername = sub.getUsername();
                //string videoQuery.bindValue(i, subUsername);
                i++;
                }

                IEnumerable<Videos> _videos;
                using (var conn = new SqlConnection(con.Value))
                {
                    _videos = await conn.QueryAsync<Videos>(videoSql, commandType: CommandType.Text);
                }
                foreach (var row in _videos)
                {
                    Video video = new Video(this.con, row, this.userLoggedInObj);
                    videos.Add(video);
                }

            }

            return videos;

        }

    }
}