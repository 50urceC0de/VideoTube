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
    public class Comment
    {

        private IConnectionConfiguration con;
        private Comments _comment;
        private User userLoggedInObj;
        private int videoId;

        public Comment(IConnectionConfiguration con, dynamic input, User userLoggedInObj, int videoId)
        {

            if ((input is int)||(input is string))
            {
                string query = ("SELECT * FROM comments where id=" + input);
                using (var conn = new SqlConnection(con.Value))
                {
                    this._comment = conn.QueryFirstOrDefault<Comments>(query, commandType: CommandType.Text);
                }
            }
            else
                this._comment = input;
            this.con = con;
            this.userLoggedInObj = userLoggedInObj;
            this.videoId = videoId;
        }

        public async Task<string> create()
        {
            int id = this._comment.id;
            int videoId = this.getVideoId();
            string body = this._comment.body;
            string postedBy = this._comment.postedBy;
            string profileButton = ButtonProvider.createUserProfileButton(this.con, postedBy);
            string timespan = this.time_elapsed_string(this._comment.datePosted);

            var commentControlsObj = new CommentControls(this.con, this, this.userLoggedInObj);
            string commentControls = await commentControlsObj .create();

            int numResponses =await this.getNumberOfReplies();
            string viewRepliesText;
            if (numResponses > 0)
            {
                viewRepliesText = "<span class='repliesSection viewReplies' onclick='getReplies(" + id + ", this," + videoId + ")'>  View all "+numResponses+" replies</span>";
            }
            else
            {
                viewRepliesText = "<div class='repliesSection'></div>";
            }

            return "<div class='itemContainer'> <div class='comment'>" + profileButton + " <div class='mainContainer'><div class='commentHeader'> <a href='profile?username=" + postedBy + "'> <span class='username'>" + postedBy + "</span>   </a> <span class='timestamp'>" + timespan + "</span> </div> <div class='body'> " + body + "   </div>  </div> </div> " + commentControls + viewRepliesText + "</div>";


        }

        public async Task<int> getNumberOfReplies()
        {
            using (var conn = new SqlConnection(con.Value))
            {
                string query = ("SELECT count(*) as 'count' FROM comments WHERE responseTo=" + this._comment.id);
                var data =(int) await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);
                return data;
            }
        }

        public string time_elapsed_string(string datetime,bool full = false)
        {
            DateTime now = new DateTime();
           // DateTime ago = new DateTime(datetime);
            DateTime diff = DateTime.Now;

            // diff.DayOfWeek = (int)(diff.Day / 7);
            // diff.Day -= diff.DayOfWeek * 7;

            var _string = new List<object>() {
               new{ y = "year" },
                new{m= "month" },
                new{w = "week" },
                new{d = "day"  },
                new{h = "hour"},
                new{i = "minute"},
                new{s = "second"},

            };
            foreach (var k in _string)
            {
                //if (diff.k)
                //{
                    // v = diff.k. ' '.v. (diff.k > 1 ? 's' : '');
                //}
                //else
                //{
                    // unset(_string[k]);
                //}
            }
            return "";
            //if (!full) _string = _string.sl, 0, 1);
           // return _string ? implode(', ', _string). ' ago' : 'just now';
        }

        public int getId()
        {
            return this._comment.id;
        }

        public int getVideoId()
        {
            return this.videoId;
        }

        public async Task<bool> wasLikedBy()
        {
            using (var conn = new SqlConnection(con.Value))
            {
                string query = ("SELECT Count(*) as 'count' FROM likes WHERE username='" + this.userLoggedInObj.getUsername() + "' AND commentId=" + this.getId());
                var data =(int) await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);


                return data > 0;
            }
        }

        public async Task<bool> wasDislikedBy()
        {
            using (var conn = new SqlConnection(con.Value))
            {
                string username = this.userLoggedInObj.getUsername();
                string query = ("SELECT Count(*) as 'count' FROM dislikes WHERE username='" + username + "' AND commentId=" + this.getId());
                var data =(int) await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);


                return data > 0;
            }
        }

        public async Task<int> getLikes()
        {
            using (var conn = new SqlConnection(con.Value))
            {
                string query = ("SELECT count(*) as 'count' FROM likes WHERE commentId=" + this.getId());
                var data =(int) await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);
                int numLikes = data;

                query = ("SELECT count(*) as 'count' FROM dislikes WHERE commentId="+this.getId());
                data = (int)await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);
                int numDislikes = data;

                return numLikes - numDislikes;
            }
        }

        public async Task<int> like()
        {
            int id = this.getId();
            string username = this.userLoggedInObj.getUsername();
            using (var conn = new SqlConnection(con.Value))
            {
                if (await this.wasLikedBy())
                {
                    // User has already liked
                    string query = ("DELETE FROM likes WHERE username='" + username + "' AND commentId=" + id);
                    await conn.ExecuteAsync(query, commandType: CommandType.Text);

                    return -1;
                }
                else
                {
                    string query = ("DELETE FROM dislikes WHERE username='" + username + "' AND commentId=" + id);

                    int count = await conn.ExecuteAsync(query, commandType: CommandType.Text);

                    query = ("INSERT INTO likes(username, commentId) VALUES('" + username + "', " + id + ")");
                    await conn.ExecuteAsync(query, commandType: CommandType.Text);

                    return 1 + count;
                }
            }
        }

        public async Task<int> dislike()
        {
            int id = this.getId();
            string username = this.userLoggedInObj.getUsername();
            using (var conn = new SqlConnection(con.Value))
            {
                if (await this.wasDislikedBy())
                {
                    // User has already liked
                    string query = ("DELETE FROM dislikes WHERE username='" + username + "' AND commentId=" + id);
                    await conn.ExecuteAsync(query, commandType: CommandType.Text);

                    return 1;
                }
                else
                {
                    string query = ("DELETE FROM likes WHERE username='" + username + "' AND commentId=" + id);
                    int count = await conn.ExecuteAsync(query, commandType: CommandType.Text);


                    query = ("INSERT INTO dislikes(username, commentId) VALUES('" + username + "', " + id + ")");
                    await conn.ExecuteAsync(query, commandType: CommandType.Text);

                    return -1 - count;
                }
            }
        }

        public async Task<string> getReplies()
        {
            string query = ("SELECT * FROM comments WHERE responseTo=" + this.getId() + " ORDER BY datePosted ASC");

            IEnumerable<Comments> _comment;
            using (var conn = new SqlConnection(con.Value))
            {
                _comment = await conn.QueryAsync<Comments>(query, commandType: CommandType.Text);
            }

            string comments = "";
            videoId = this.getVideoId();
            foreach (var row in _comment)
            {
                var comment = new Comment(this.con, row, this.userLoggedInObj, videoId);
                comments +=await comment.create();
            }

            return comments;
        }

    }
}