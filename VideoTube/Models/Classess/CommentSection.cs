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
    public class CommentSection {

        private IConnectionConfiguration con;
        private Video video;
        private User userLoggedInObj;

    public  CommentSection(IConnectionConfiguration con, Video video, User userLoggedInObj) {
        this.con = con;
        this.video = video;
        this.userLoggedInObj = userLoggedInObj;
    }

    public async Task<string> create() {
        return await this.createCommentSection();
    }

    private async Task<string> createCommentSection() {
        int numComments =await this.video.getNumberOfComments();
        string postedBy = this.userLoggedInObj.getUsername();
        int videoId = this.video._video.id;

        string profileButton = ButtonProvider.createUserProfileButton(this.con, postedBy);
        string commentAction = $"postComment(this, \"{postedBy}\", {videoId}, null, \"comments\")";
        string commentButton = ButtonProvider.createButton("COMMENT", null, commentAction, "postComment");
        
        var comments =await this.video.getComments();
        string commentItems = "";
        foreach(var comment in comments) {
            commentItems +=await comment.create();
        }

        return "<div class='commentSection'><div class='header'><span class='commentCount'>"+numComments+" Comments</span><div class='commentForm'>"+ profileButton+"<textarea class='commentBodyClass' placeholder='Add a public comment'></textarea>"+commentButton+" </div> </div> <div class='comments'>"+commentItems+" </div></div>";
    }

}
}