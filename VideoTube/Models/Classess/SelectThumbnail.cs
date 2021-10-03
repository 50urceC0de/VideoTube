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
    public class SelectThumbnail
    {

        private IConnectionConfiguration con;
        private Video video;

        public SelectThumbnail(IConnectionConfiguration con, Video video)
        {
            this.con = con;
            this.video = video;
        }

        public async Task<string> create()
        {
            var thumbnailData = await this.getThumbnailData();

            string html = "";

            foreach (var data in thumbnailData)
            {
                html += this.createThumbnailItem(data);
            }

            return "<div class='thumbnailItemsContainer'> " + html + " </div> ";
        }

        private string createThumbnailItem(Thumbnail data)
        {

            string selected = data.selected == 1 ? "selected" : "";

            return $"<div class='thumbnailItem {selected}' onclick='setNewThumbnail({data.id}, {data.videoId}, this)'><img src='{data.filePath}' ></div > ";
        }

        private async Task<IEnumerable<Thumbnail>> getThumbnailData()
        {

            string query = ("SELECT * FROM thumbnails WHERE videoId=" + this.video._video.id);
            IEnumerable<Thumbnail> thumbnail;
            using (var conn = new SqlConnection(con.Value))
            {
                thumbnail = await conn.QueryAsync<Thumbnail>(query, commandType: CommandType.Text);
            }

            return thumbnail;
        }
    }
}