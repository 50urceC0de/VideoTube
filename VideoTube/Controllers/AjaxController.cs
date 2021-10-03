using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VideoTube.Data;
using VideoTube.Data.Services;
using VideoTube.Models;

namespace VideoTube.Controllers
{
    public class AjaxController : Controller
    {
        private IConnectionConfiguration con;

        public AjaxController(IConnectionConfiguration _con)
        {
            con = _con;
        }
        // GET: Ajax
        public async Task<int> DislikeComment(int videoId, string commentId)
        {
            var username = Session["userLoggedIn"].ToString();


            var userLoggedInObj = new User(con, username);
            var comment = new Comment(con, commentId, userLoggedInObj, videoId);

            return await comment.dislike();
        }
        public async Task<string> DislikeVideo(int videoId)
        {
            var username = Session["userLoggedIn"].ToString();

            var userLoggedInObj = new User(con, username);
            var video = new Video(con, videoId, userLoggedInObj);

            return await video.dislike();
        }
        public async Task<string> GetCommentReplies(int videoId, int commentId)
        {
            var username = Session["userLoggedIn"].ToString();


            var userLoggedInObj = new User(con, username);
            var comment = new Comment(con, commentId, userLoggedInObj, videoId);

            return await comment.getReplies();
        }
        public async Task<int> LikeComment(int videoId, int commentId)
        {

            var username = Session["userLoggedIn"].ToString();

            var userLoggedInObj = new User(con, username);
            var comment = new Comment(con, commentId, userLoggedInObj, videoId);

            return await comment.like();
        }
        public async Task<string> LikeVideo(int videoId)
        {
            var username = Session["userLoggedIn"].ToString();

            var userLoggedInObj = new User(con, username);
            var video = new Video(con, videoId, userLoggedInObj);

            return await video.like();
        }
        public string getVData(int videoId)
        {
            var video = new Video(con, videoId, null);
            return video._video.filePath.ToString();
        }

        public async Task<string> PostComment(string commentText, string postedBy, int videoId, string responseTo)
        {
            if (!commentText.isNullOrEmpty() && !postedBy.isNullOrEmpty() && videoId != 0)
            {

                var userLoggedInObj = new User(con, Session["userLoggedIn"].ToString());

                var query = ("INSERT INTO comments(postedBy, videoId, responseTo, body) VALUES('" + postedBy + "', '" + videoId + "', '" + responseTo + "', '" + commentText + "');select SCOPE_IDENTITY()");

                using (var conn = new SqlConnection(con.Value))
                {
                    var id = await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);
                    var newComment = new Comment(con,Convert.ToInt32(id), userLoggedInObj, videoId);
                    return await newComment.create();
                }

            }
            else
            {
                return "One or more parameters are not passed into subscribe the file";
            }
        }
        public async Task<string> Subscribe(string userTo, string userFrom)
        {
            if (!userTo.isNullOrEmpty() && !userFrom.isNullOrEmpty())
            {
                // check if the user is subbed
                
                using (var conn = new SqlConnection(con.Value))
                {
                    string query = ("SELECT * FROM subscribers WHERE userTo='" + userTo + "' AND userFrom='" + userFrom + "'");
                    IEnumerable<Subcription> subcription;
                    subcription = await conn.QueryAsync<Subcription>(query, commandType: CommandType.Text);


                    if (subcription.Count<Subcription>() == 0)
                    {
                        // Insert
                        query = ("INSERT INTO subscribers(userTo, userFrom) VALUES('"+ userTo + "', '" + userFrom + "')");
                        await conn.ExecuteAsync(query, commandType: CommandType.Text);
                    }
                    else
                    {
                        // Delete
                        query = ("DELETE FROM subscribers WHERE userTo='" + userTo + "' AND userFrom='" + userFrom + "'");
                        await conn.ExecuteAsync(query, commandType: CommandType.Text);
                    }

                    query = ("SELECT * FROM subscribers WHERE userTo='" + userTo + "'");
                    subcription = await conn.QueryAsync<Subcription>(query, commandType: CommandType.Text);

                    return subcription.Count<Subcription>().ToString();
                }
            }
            else
            {
                return "One or more parameters are not passed into subscribe the file";
            }
        }
        public async Task<string> UpdateThumbnail(int videoId, int thumbnailId)
        {
            if (videoId != 0 && thumbnailId != 0)
            {

                string query = ("UPDATE thumbnails SET selected=0 WHERE videoId=" + videoId);
                using (var conn = new SqlConnection(con.Value))
                {
                    await conn.ExecuteAsync(query, commandType: CommandType.Text);
                    query = ("UPDATE thumbnails SET selected=1 WHERE id=" + thumbnailId);
                    await conn.ExecuteAsync(query, commandType: CommandType.Text);
                }

                return "Succesfull update";
            }
            else
            {
                return "One or more parameters are not passed into updateThumbnail the file";
            }
        }
    }
}