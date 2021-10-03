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
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace VideoTube.Data
{
    public class Video
    {

        private IConnectionConfiguration con;
        public Videos _video;
        private User userLoggedInObj;
        JavaScriptSerializer js = new JavaScriptSerializer();

        public Video(IConnectionConfiguration con, dynamic input, User userLoggedInObj)
        {
            this.con = con;
            this.userLoggedInObj = userLoggedInObj;

            if (!(input is string)&& !(input is int))
            {
                this._video = input;
            }
            else
            {
                string query = ("SELECT * FROM videos WHERE id = " + input);
                using (var conn = new SqlConnection(con.Value))
                {
                    this._video = conn.QueryFirstOrDefault<Videos>(query, commandType: CommandType.Text);
                }
            }
        }



        public string getTimeStamp()
        {
            var value = this._video.uploadDate;
            return value.ToString("yyyyMMddHHmmssfff");
        }



        public async void incrementViews()
        {
            string query = ("UPDATE videos SET views=views+1 WHERE id=" + this._video.id);
            using (var conn = new SqlConnection(con.Value))
            {
                await conn.ExecuteAsync(query, commandType: CommandType.Text);
                this._video.views = this._video.views + 1;
            }
        }

        public async Task<int> getLikes()
        {
            string query = ("SELECT count(*) as 'count' FROM likes WHERE videoId = " + this._video.id);
            using (var conn = new SqlConnection(con.Value))
            {
                var data =(int) await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);
                return data;
            }

        }

        public async Task<int> getDislikes()
        {
            string query = ("SELECT count(*) as 'count' FROM dislikes WHERE videoId = " + this._video.id);
            using (var conn = new SqlConnection(con.Value))
            {
                var data =(int) await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);
                return data;
            }
        }

        public async Task<string> like()
        {
            string username = this.userLoggedInObj.getUsername();
            using (var conn = new SqlConnection(con.Value))
            {
                if (await this.wasLikedBy())
                {
                    // User has already liked
                    string query = ("DELETE FROM likes WHERE username='" + username + "' AND videoId=" + this._video.id);
                    await conn.ExecuteAsync(query, commandType: CommandType.Text);
                    var result = new Dictionary<string, int>() { { "likes", -1 }, { "dislikes", 0 } };
                    return js.Serialize(result);
                }
                else
                {
                    string query = ("DELETE FROM dislikes WHERE username='" + username + "' AND videoId=" + this._video.id);
                    var count = await conn.ExecuteAsync(query, commandType: CommandType.Text);

                    query = ("INSERT INTO likes(username, videoId) VALUES('" + username + "', " + this._video.id + ")");
                    await conn.ExecuteAsync(query, commandType: CommandType.Text);

                    var result = new Dictionary<string, int>() { { "likes", 1 }, { "dislikes", (0 - count) } };

                    return js.Serialize(result);
                }
            }
        }

        public async Task<string> dislike()
        {
            string username = this.userLoggedInObj.getUsername();
            using (var conn = new SqlConnection(con.Value))
            {
                if (await this.wasDislikedBy())
                {
                    // User has already liked
                    string query = ("DELETE FROM dislikes WHERE username='" + username + "' AND videoId=" + this._video.id);
                    await conn.ExecuteAsync(query, commandType: CommandType.Text);
                    var result = new Dictionary<string, int>() { { "likes", 0 }, { "dislikes", -1 } };
                    return js.Serialize(result);


                }
                else
                {
                    string query = ("DELETE FROM likes WHERE username='" + username + "' AND videoId=" + this._video.id);

                    int count = await conn.ExecuteAsync(query, commandType: CommandType.Text);

                    query = ("INSERT INTO dislikes(username, videoId) VALUES('" + username + "', " + this._video.id + ")");
                    await conn.ExecuteAsync(query, commandType: CommandType.Text);
                    var result = new Dictionary<string, int>() { { "likes", (0 - count) }, { "dislikes", 1 } };

                    return js.Serialize(result);
                }
            }
        }

        public async Task<bool> wasLikedBy()
        {
            using (var conn = new SqlConnection(con.Value))
            {
                string username = this.userLoggedInObj?.getUsername();
                string query = ("SELECT Count(*) as 'count' FROM likes WHERE username='" + username + "' AND videoId=" + this._video.id);
                var data = (int)await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);

                return data > 0;
            }
        }

        public async Task<bool> wasDislikedBy()
        {
            using (var conn = new SqlConnection(con.Value))
            {
                string username = this.userLoggedInObj?.getUsername();
                string query = ("SELECT Count(*) FROM dislikes WHERE username='" + username + "' AND videoId=" + this._video.id);
                var data = (int)await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);

                return data > 0;
            }
        }

        public async Task<int> getNumberOfComments()
        {
            using (var conn = new SqlConnection(con.Value))
            {
                string query = ("SELECT Count(*) as 'count' FROM comments WHERE videoId=" + this._video.id);
                var data =(int) await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);
                return data;
            }
        }

        public async Task<List<Comment>> getComments()
        {
            string query = ("SELECT * FROM comments WHERE videoId=" + this._video.id + " AND responseTo=0 ORDER BY datePosted DESC");
            IEnumerable<Comments> _comment;
            using (var conn = new SqlConnection(con.Value))
            {
                _comment = await conn.QueryAsync<Comments>(query, commandType: CommandType.Text);
            }

            var comments = new List<Comment>();

            foreach (var row in _comment)
            {
                var comment = new Comment(this.con, row, this.userLoggedInObj, this._video.id);
                comments.Add(comment);
            }

            return comments;
        }

        public async Task<string> getThumbnail()
        {
            string query = ("SELECT filePath FROM thumbnails WHERE videoId=" + this._video.id + " AND selected=1");

            using (var conn = new SqlConnection(con.Value))
            {
                var thumbnail = await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);
                return thumbnail.ToString();
            }
        }

    }
}