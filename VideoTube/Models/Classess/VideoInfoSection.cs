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
    class VideoInfoSection
    {

        private IConnectionConfiguration con;
        private Video video;
        private User userLoggedInObj;

        public VideoInfoSection(IConnectionConfiguration con, Video video, User userLoggedInObj)
        {
            this.con = con;
            this.video = video;
            this.userLoggedInObj = userLoggedInObj;
        }

        public async Task<string> create()
        {

            return (await this.createPrimaryInfo()) + (await this.createSecondaryInfo());
        }

        private async Task<string> createPrimaryInfo()
        {
            string title = this.video._video.title;
            int views = this.video._video.views;

            VideoInfoControls videoInfoControls = new VideoInfoControls(this.video, this.userLoggedInObj);
            string controls =await videoInfoControls.create();

            return $"<div class='videoInfo'><h1>{title}</h1> <div class='bottomSection'><span class='viewCount'>{views} views</span> { controls } </div> </div>";
        }

        private async Task<string> createSecondaryInfo()
        {

            string description = this.video._video.description;
            DateTime uploadDate = this.video._video.uploadDate;
            string uploadedBy = this.video._video.uploadedBy;
            string profileButton = ButtonProvider.createUserProfileButton(this.con, uploadedBy);
            string actionButton;
            User userToObject =null;
            if (uploadedBy == this.userLoggedInObj?.getUsername())
            {
                actionButton = ButtonProvider.createEditVideoButton(this.video._video.id);
            }
            else
            {
                userToObject = new User(this.con, uploadedBy);
                actionButton =await ButtonProvider.createSubscriberButton(this.con, userToObject, this.userLoggedInObj);
            }

            return $"<div class='secondaryInfo'>  <div class='topRow'>{profileButton}<div class='uploadInfo'><span class='owner'><a href='profile?username={uploadedBy}'>{ uploadedBy }</a> </span> <span class='date'>Published on {uploadDate}</span> </div> {actionButton } </div> <div class='descriptionContainer'> { description}</div>  </div>";
        }

    }
}