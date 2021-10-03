using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Data;
using VideoTube.Data.Services;
using VideoTube.Models;
namespace VideoTube.Data
{
    public class VideoGrid
    {

        private IConnectionConfiguration con;
        private User userLoggedInObj;
        private bool largeMode = false;
        private string gridClass = "videoGrid";

        public VideoGrid(IConnectionConfiguration con, User userLoggedInObj)
        {
            this.con = con;
            this.userLoggedInObj = userLoggedInObj;
        }

        public async Task<string> create(IEnumerable<Video> videos, string title, bool showFilter)
        {
            string gridItems;
            if (videos == null)
            {
                gridItems = await this.generateItems();
            }
            else
            {
                gridItems =await this.generateItemsFromVideos(videos);
            }

            string header = "";

            if (title != null)
            {
                header = this.createGridHeader(title, showFilter);
            }

            return header + "  <div class='"+this.gridClass+"'>" + gridItems + " </div>";
        }

        public async Task<string> generateItems()
        {
            string query = ("SELECT Top(15) * FROM videos ORDER BY RAND()");

            IEnumerable<Videos> videos;
            using (var conn = new SqlConnection(con.Value))
            {
                videos = await conn.QueryAsync<Videos>(query, commandType: CommandType.Text);
            }

            string elementsHtml = "";
            foreach (var row in videos)
            {
                Video video = new Video(this.con, row, this.userLoggedInObj);
                VideoGridItem item = new VideoGridItem(video, this.largeMode);
                elementsHtml +=await item.create();
               
            }


            return elementsHtml;
        }

        public async Task<string> generateItemsFromVideos(IEnumerable<Video> videos)
        {
            string elementsHtml = "";

            foreach (var video in videos)
            {
                var item = new VideoGridItem(video, this.largeMode);
                elementsHtml +=await item.create();
            }

            return elementsHtml;
        }

        public string createGridHeader(string title, bool showFilter)
        {
            string filter = "";

            if (showFilter)
            {
                string query = System.Web.HttpContext.Current.Request.QueryString["term"].ToString();//"http://_SERVER[HTTP_HOST]_SERVER[REQUEST_URI]";
                string newUrl = "search";
                   
                newUrl +=  $"?term={System.Web.HttpContext.Current.Server.UrlEncode(query).ToString()}";
                 
                filter = "<div class='right'><span>Order by:</span><a href='" + newUrl + "&orderBy=uploadDate'>Upload date</a><a href='" + newUrl + "&orderBy=views'>Most viewed</a></div>";
            }

            return "<div class='videoGridHeader'> <div class='left'> " + title + " </div>" + filter + "</div>";
        }

        public async Task<string> createLarge(IEnumerable<Video> videos, string title, bool showFilter)
        {
            this.gridClass += "large";
            this.largeMode = true;
            return await this.create(videos, title, showFilter);
        }

    }
}