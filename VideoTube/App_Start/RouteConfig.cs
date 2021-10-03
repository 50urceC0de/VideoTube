using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace VideoTube
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "watch",
                url: "watch/{id}",
                defaults: new { controller = "Home", action = "watch", id = UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "SignIn",
              url: "signIn",
              defaults: new { controller = "SignIn", action = "Index" }
          );
            routes.MapRoute(
                name: "trending",
                url: "trending",
                defaults: new { controller = "Home", action = "trending", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "subscriptions",
               url: "subscriptions",
               defaults: new { controller = "Home", action = "subscriptions", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "likedVideos",
               url: "likedVideos",
               defaults: new { controller = "Home", action = "likedVideos", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "settings",
               url: "settings",
               defaults: new { controller = "Setting", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
              name: "logout",
              url: "logout",
              defaults: new { controller = "Home", action = "logout", id = UrlParameter.Optional }
          );
            routes.MapRoute(
           name: "processing",
           url: "processing",
           defaults: new { controller = "Home", action = "processing" }
       );
            routes.MapRoute(
             name: "signUp",
             url: "signUp",
             defaults: new { controller = "SignUp", action = "Index" }
         );
            routes.MapRoute(
               name: "profile",
               url: "profile",
               defaults: new { controller = "Home", action = "profile", id = UrlParameter.Optional }
           );
            routes.MapRoute(
             name: "editVideo",
             url: "editVideo",
             defaults: new { controller = "Home", action = "editVideo", id = UrlParameter.Optional }
         );
            routes.MapRoute(
            name: "upload",
            url: "upload",
            defaults: new { controller = "Home", action = "upload", id = UrlParameter.Optional }
        );

            // Ajax Route
            routes.MapRoute(
             name: "dislikeComment",
             url: "dislikeComment",
             defaults: new { controller = "Ajax", action = "DislikeComment", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "dislikeVideo",
             url: "dislikeVideo",
             defaults: new { controller = "Ajax", action = "DislikeVideo", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "getCommentReplies",
             url: "getCommentReplies",
             defaults: new { controller = "Ajax", action = "GetCommentReplies", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "likeComment",
             url: "likeComment",
             defaults: new { controller = "Ajax", action = "LikeComment", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "likeVideo",
             url: "likeVideo",
             defaults: new { controller = "Ajax", action = "LikeVideo", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "postComment",
             url: "postComment",
             defaults: new { controller = "Ajax", action = "PostComment", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "subscribe",
             url: "subscribe",
             defaults: new { controller = "Ajax", action = "Subscribe", id = UrlParameter.Optional }
         );
            routes.MapRoute(
             name: "updateThumbnail",
             url: "updateThumbnail",
             defaults: new { controller = "Ajax", action = "UpdateThumbnail", id = UrlParameter.Optional }
         );
            routes.MapRoute(
            name: "search",
            url: "search",
            defaults: new { controller = "Home", action = "search", id = UrlParameter.Optional }
        );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
