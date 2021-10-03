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
    class VideoInfoControls {

        private Video video;
        private User userLoggedInObj;

    public  VideoInfoControls(Video video, User userLoggedInObj) {
        this.video = video;
        this.userLoggedInObj = userLoggedInObj;
    }

    public async Task<string> create() {

        string likeButton = await this.createLikeButton();
        string dislikeButton = await this.createDislikeButton();
        
        return "<div class='controls'>"+likeButton+dislikeButton+" </div>";
    }

    private async Task<string> createLikeButton() {
        int text =await this.video.getLikes();
        int videoId = this.video._video.id;
        string action = $"likeVideo(this, {videoId})";
        string _class = "likeButton";

        string imageSrc = "assets/images/icons/thumb-up.png";

        if(await this.video.wasLikedBy()) {
            imageSrc = "assets/images/icons/thumb-up-active.png";
        }

        return ButtonProvider.createButton(text.ToString(), imageSrc, action, _class);
    }

    private async Task<string> createDislikeButton() {
        int text = await this.video.getDislikes();
        int videoId = this.video._video.id;
        string action = $"dislikeVideo(this, {videoId})";
       string  _class = "dislikeButton";

        string imageSrc = "assets/images/icons/thumb-down.png";

        if(await this.video.wasDislikedBy()) {
            imageSrc = "assets/images/icons/thumb-down-active.png";
        }

        return ButtonProvider.createButton(text.ToString(), imageSrc, action, _class);
    }
}
}