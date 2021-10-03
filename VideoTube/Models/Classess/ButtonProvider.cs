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
    public class ButtonProvider
    {

        public static string signInstring = "notSignedIn()";

        public static string createLink(string link)
        {
            return User.isLoggedIn() ? link : ButtonProvider.signInstring;
        }

        public static string createButton(string text, string imageSrc, string action, string _class)
        {

            string image = (imageSrc == null) ? "" : "<img src='"+imageSrc+"'>";

            action = ButtonProvider.createLink(action);

            return "<button class='" + _class + "' onclick='"+action+"'>" + image + "<span class='text'>"+text+"</span> </button>";
        }

        public static string createHyperlinkButton(string text, string imageSrc, string href, string _class)
        {

            string image = (imageSrc == null) ? "" : "<img src='"+imageSrc+"'>";

            return "<a href='" + href + "'><button class='" + _class + "'> " + image + "<span class='text'>" + text + "</span></button> </a>";
        }

        public static string createUserProfileButton(IConnectionConfiguration con, string username)
        {
            var userObj = new User(con, username);
            var profilePic = userObj.user.profilePic.isNullOrEmpty()?"assets/images/profilePictures/default.png": userObj.user.profilePic;
            string link = "profile?username="+username+"";

            return "<a href='" + link + "'> <img src='" + profilePic + "' class='profilePicture'></a>";
        }

        public static string createEditVideoButton(int videoId)
        {
            string href = $"editVideo?videoId={videoId}";

            string button = ButtonProvider.createHyperlinkButton("EDIT VIDEO", null, href, "edit button");

            return $"<div class='editVideoButtonContainer'>{button}</div>";
        }

        public static async Task<string> createSubscriberButton(IConnectionConfiguration con, User userToObj, User userLoggedInObj)
        {
            string userTo = userToObj.getUsername();
            var userLoggedIn = userLoggedInObj?.getUsername();

            bool isSubscribedTo = await userLoggedInObj.isSubscribedTo(userTo);
            string buttonText = isSubscribedTo ? "SUBSCRIBED" : "SUBSCRIBE";
            buttonText += " " + await userToObj.getSubscriberCount();

            string buttonClass = isSubscribedTo ? "unsubscribe button" : "subscribe button";
            string action = $"subscribe(\"{userTo}\", \"{userLoggedIn}\", this)";

            string button = ButtonProvider.createButton(buttonText, null, action, buttonClass);

            return "<div class='subscribeButtonContainer'>" + button + " </div>";
        }

        public static string createUserProfileNavigationButton(ConnectionConfiguration con, string username)
        {
            if (User.isLoggedIn())
            {
                return ButtonProvider.createUserProfileButton(con, username);
            }
            else
            {
                return "<a href='signIn'> <span class='signInLink'>SIGN IN</span></a>";
            }
        }

    }
}