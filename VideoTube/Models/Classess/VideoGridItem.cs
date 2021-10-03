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
    class VideoGridItem {

        private Video video;
        private bool largeMode;

    public  VideoGridItem(Video video, bool largeMode) {
        this.video = video;
        this.largeMode = largeMode;
    }

    public async Task<string> create() {
        string thumbnail =await this.createThumbnail();
        string details = this.createDetails();
        string url = "watch?id=" + this.video._video.id;

        return "<a href='"+url+"'><div class='videoGridItem'>"+thumbnail+ details+"</div></a>";
    }

    private async Task<string> createThumbnail() {
        
        string thumbnail =await this.video.getThumbnail();
        string duration = this.video._video.duration;

        return "<div class='thumbnail'><img src='"+thumbnail+"'> <div class='duration'><span>"+duration+"</span></div></div>";

    }

    private string createDetails() {
        string title = this.video._video.title;
        string username = this.video._video.uploadedBy;
        int views = this.video._video.views;
        string description = this.createDescription();
        string timestamp = this.video.getTimeStamp();

        return "<div class='details'> <h3 class='title'>"+title+"</h3> <span class='username'>"+username+"</span>  <div class='stats'> <span class='viewCount'>"+views +" views - </span>  <span class='timeStamp'>"+timestamp+"</span> </div> "+description+" </div>";
    }

    private string createDescription() {
        if(this.largeMode) {
            return "";
        }
        else {
           string description = this.video._video.description;
           description = (description.Length > 350) ? (description.Substring(0, 347) + "..." ): description;
            return "<span class='description'>"+description+"</span>";
        }
    }

}
}