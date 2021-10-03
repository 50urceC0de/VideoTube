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
    public class ProfileGenerator
    {

        private IConnectionConfiguration con;
        private User userLoggedInObj;
        private ProfileData profileData;

        public ProfileGenerator(IConnectionConfiguration con, User userLoggedInObj, string profileUsername)
        {
            this.con = con;
            this.userLoggedInObj = userLoggedInObj;
            this.profileData = new ProfileData(con, profileUsername);
        }

        public async Task<string> create()
        {
            var profileUsername = this.profileData.getProfileUsername();

            if (!await this.profileData.userExists())
            {
                return "User does not exist";
            }

            string coverPhotoSection = this.createCoverPhotoSection();
            string headerSection = await this.createHeaderSection();
            string tabsSection = this.createTabsSection();
            string contentSection = await this.createContentSection();
            return "<div class='profileContainer'>" + coverPhotoSection + headerSection + tabsSection + contentSection + "</div>";
        }

        public string createCoverPhotoSection()
        {
            string coverPhotoSrc = this.profileData.getCoverPhoto();
            string name = this.profileData.getProfileUserFullName();
            return "<div class='coverPhotoContainer'><img src='" + coverPhotoSrc + "' class='coverPhoto'><span class='channelName'>" + name + "</span> </div>";
        }

        public async Task<string> createHeaderSection()
        {
            string profileImage = this.profileData.getProfilePic();
            string name = this.profileData.getProfileUserFullName();
            int subCount = await this.profileData.getSubscriberCount();

            string button =await this.createHeaderButton();

            return "<div class='profileHeader'> <div class='userInfoContainer'><img class='profileImage' src='" + profileImage + "'><div class='userInfo'><span class='title'>" + name + "</span><span class='subscriberCount'>" + subCount + " subscribers</span>  </div> </div><div class='buttonContainer'> <div class='buttonItem'>     " + button + " </div>  </div> </div>";
        }

        public string createTabsSection()
        {
            return "<ul class='nav nav-tabs' role='tablist'>  <li class='nav-item'>  <a class='nav-link active' id='videos-tab' data-toggle='tab'    href='#videos' role='tab' aria-controls='videos' aria-selected='true'>VIDEOS</a>  </li>  <li class='nav-item'>  <a class='nav-link' id='about-tab' data-toggle='tab' href='#about' role='tab'   aria-controls='about' aria-selected='false'>ABOUT</a>  </li>  </ul>";
        }

        public async Task<string> createContentSection()
        {

            var videos = await this.profileData.getUsersVideos();
            string videoGridHtml;
            if (videos.Count > 0)
            {
                var videoGrid = new VideoGrid(this.con, this.userLoggedInObj);
                videoGridHtml =await videoGrid.create(videos, null, false);
            }
            else
            {
                videoGridHtml = "<span>This user has no videos</span>";
            }

            string aboutSection = this.createAboutSection();

            return "<div class='tab-content channelContent'> <div class='tab-pane fade show active' id='videos' role='tabpanel' aria-labelledby='videos-tab'>" + videoGridHtml + "</div>  <div class='tab-pane fade' id='about' role='tabpanel' aria-labelledby='about-tab'>" + aboutSection + "  </div> </div>";
        }

        private async Task<string> createHeaderButton()
        {
            if (this.userLoggedInObj.getUsername() == this.profileData.getProfileUsername())
            {
                return "";
            }
            else
            {
                return await ButtonProvider.createSubscriberButton(this.con, this.profileData.getProfileUserObj(), this.userLoggedInObj);
            }
        }

        private string createAboutSection()
        {
            string html = "<div class='section'><div class='title'> <span>Details</span></div><div class='values'>";

            Dictionary<string, dynamic> details = this.profileData.getAllUserDetails();
            foreach (var k in details)
            {
                html += "<span>"+k.Key+": "+k.Value+"</span>";
            }

            html += "</div></div>";

            return html;
        }
    }
}