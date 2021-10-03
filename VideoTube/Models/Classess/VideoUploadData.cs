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
    public class VideoUploadData
    {

        public Dictionary<string, string> videoDataArray;
        public string title, description, privacy, category, uploadedBy;

        public VideoUploadData(string title, string description, string privacy, string category, string uploadedBy)
        {
            this.videoDataArray =(Dictionary<string, string>) System.Web.HttpContext.Current.Session["UploadData"];
            this.title = title;
            this.description = description;
            this.privacy = privacy;
            this.category = category;
            this.uploadedBy = uploadedBy;
        }

        public async Task<bool> updateDetails(IConnectionConfiguration con,int videoId)
        {
            string query = (@"UPDATE videos SET title=@title, description=@description, privacy=@privacy, category =@category WHERE id =@videoId");
            using (var conn = new SqlConnection(con.Value))
            {
                await conn.ExecuteAsync(query,new { this.title, this.description, this.privacy, this.category, videoId }, commandType: CommandType.Text);
            }
            return true;
        }

    }
}