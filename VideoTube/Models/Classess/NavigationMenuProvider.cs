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
    public class NavigationMenuProvider
    {

        private ConnectionConfiguration con;
        private User userLoggedInObj;

        public NavigationMenuProvider(ConnectionConfiguration con, User userLoggedInObj)
        {
            this.con = con;
            this.userLoggedInObj = userLoggedInObj;
        }

        public  string create()
        {
            string menuHtml = this.createNavItem("Home", "assets/images/icons/home.png", "Home");
            menuHtml += this.createNavItem("Trending", "assets/images/icons/trending.png", "trending");
            menuHtml += this.createNavItem("Subscriptions", "assets/images/icons/subscriptions.png", "subscriptions");
            menuHtml += this.createNavItem("Liked Videos", "assets/images/icons/thumb-up.png", "likedVideos");

            if (User.isLoggedIn())
            {
                menuHtml += this.createNavItem("Settings", "assets/images/icons/settings.png", "settings");
                menuHtml += this.createNavItem("Log Out", "assets/images/icons/logout.png", "logout");

                menuHtml += this.createSubscriptionsSection();
            }

            return "<div class='navigationItems'>" + menuHtml + " </div > ";
        }

        private string createNavItem(string text, string icon, string link)
        {
            return "<div class='navigationItem'><a href = '" + link + "' > <img src = '" + icon + "' ><span>" + text + " </span></a></div> ";
        }

        private string createSubscriptionsSection()
        {
            var subscriptions =  this.userLoggedInObj.getSubscriptions();

            string html = "<span class='heading'>Subscriptions</span>";
            foreach (var sub in subscriptions)
            {
                string subUsername = sub.getUsername();
                html += this.createNavItem(subUsername, sub.user.profilePic, "profile?username="+subUsername+"");
            }
            return html;
        }

    }
}