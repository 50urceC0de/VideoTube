using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Data;
using VideoTube.Data.Services;

namespace VideoTube.Data
{
    public class CommentControls
    {

        private IConnectionConfiguration con;
        private Comment comment;
        private User userLoggedInObj;

        public CommentControls(IConnectionConfiguration con, Comment comment, User userLoggedInObj)
        {
            this.con = con;
            this.comment = comment;
            this.userLoggedInObj = userLoggedInObj;
        }

        public async Task<string> create()
        {

            string replyButton = this.createReplyButton();
            string likesCount =await this.createLikesCount();
            string likeButton =await this.createLikeButton();
            string dislikeButton =await this.createDislikeButton();
            string replySection = this.createReplySection();

            return "<div class='controls'>" + replyButton + likesCount + likeButton + dislikeButton + "</div>" + replySection;
        }

        private string createReplyButton()
        {
            string text = "REPLY";
            string action = "toggleReply(this)";

            return ButtonProvider.createButton(text, null, action, null);
        }

        private async Task<string> createLikesCount()
        {
            int text =await this.comment.getLikes();
            if (text == 0) return "<span class='likesCount'>text</span>"; ;

            return "<span class='likesCount'>"+text+"</span>";
        }

        private string createReplySection()
        {
            string postedBy = this.userLoggedInObj.getUsername();
            int videoId = this.comment.getVideoId();
            int commentId = this.comment.getId();

            string profileButton = ButtonProvider.createUserProfileButton(this.con, postedBy);

            string cancelButtonAction = "toggleReply(this)";
            string cancelButton = ButtonProvider.createButton("Cancel", null, cancelButtonAction, "cancelComment");

            string postButtonAction = $"postComment(this, \"{postedBy}\", {videoId}, {commentId}, \"repliesSection\")";
            string postButton = ButtonProvider.createButton("Reply", null, postButtonAction, "postComment");

            return "<div class='commentForm hidden'>" + profileButton + "<textarea class='commentBodyClass' placeholder='Add a public comment'></textarea>" + cancelButton + postButton + "</div>";
        }

        private async Task<string> createLikeButton()
        {
            int commentId = this.comment.getId();
            int videoId = this.comment.getVideoId();
            string action = $"likeComment({commentId}, this, {videoId})";
            string _class = "likeButton";

            string imageSrc = "assets/images/icons/thumb-up.png";

            if (await this.comment.wasLikedBy())
            {
                imageSrc = "assets/images/icons/thumb-up-active.png";
            }

            return ButtonProvider.createButton("", imageSrc, action, _class);
        }

        private async Task<string> createDislikeButton()
        {
            int commentId = this.comment.getId();
            int videoId = this.comment.getVideoId();
            string action = $"dislikeComment({commentId}, this, {videoId})";
            string _class = "dislikeButton";

            string imageSrc = "assets/images/icons/thumb-down.png";

            if (await this.comment.wasDislikedBy())
            {
                imageSrc = "assets/images/icons/thumb-down-active.png";
            }

            return ButtonProvider.createButton("", imageSrc, action, _class);
        }
    }
}