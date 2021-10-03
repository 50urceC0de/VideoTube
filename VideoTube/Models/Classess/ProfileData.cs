using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Data;
using VideoTube.Models;
using VideoTube.Data.Services;

namespace VideoTube.Data
{
    public class ProfileData
    {

        private IConnectionConfiguration con;
        public User profileUserObj;

        public ProfileData(IConnectionConfiguration con, string profileUsername)
        {
            this.con = con;
            this.profileUserObj = new User(con, profileUsername);
        }

        public User getProfileUserObj()
        {
            return this.profileUserObj;
        }

        public string getProfileUsername()
        {
            return this.profileUserObj.getUsername();
        }

        public async Task<bool> userExists()
        {
            using (var conn = new SqlConnection(con.Value))
            {
                string profileUsername = this.getProfileUsername();

                string query = ("SELECT Count(*) FROM users WHERE username = '" + profileUsername+"'");
                var data = await conn.QueryFirstOrDefaultAsync(query, commandType: CommandType.Text);

                return data.Length != 0;
            }
        }

        public string getCoverPhoto()
        {
            return "assets/images/coverPhotos/default-cover-photo.jpg";
        }

        public string getProfileUserFullName()
        {
            return this.profileUserObj.user.name;
        }

        public string getProfilePic()
        {
            return this.profileUserObj.user.profilePic;
        }

        public async Task<int> getSubscriberCount()
        {
            return await this.profileUserObj.getSubscriberCount();
        }

        public async Task<List<Video>> getUsersVideos()
        {
            string query = ("SELECT * FROM videos WHERE uploadedBy='" + this.getProfileUsername() + "' ORDER BY uploadDate DESC");
            IEnumerable<Videos> _comment;
            using (var conn = new SqlConnection(con.Value))
            {
                _comment = await conn.QueryAsync<Videos>(query, commandType: CommandType.Text);
            }

            var videos = new List<Video>();
            foreach (var row in _comment)
            {
                videos.Add(new Video(this.con, row, this.profileUserObj));
            }
            return videos;
        }

        public Dictionary<string, dynamic> getAllUserDetails()
        {
            return new Dictionary<string, dynamic>() {
            { "Name" ,this.getProfileUserFullName()},
            {    "Username" , this.getProfileUsername() },
            {    "Subscribers" ,this.getSubscriberCount() },
            {    "Total views" ,this.getTotalViews() },
            {    "Sign up date", this.getSignUpDate() }
            };
        }

        private async Task<int> getTotalViews()
        {
            using (var conn = new SqlConnection(con.Value))
            {
                string query = ("SELECT sum(views) FROM videos WHERE uploadedBy='" + this.getProfileUsername() + "'");
                var data =(int) await conn.ExecuteScalarAsync(query, commandType: CommandType.Text);

                return data;
            }
        }

        private DateTime getSignUpDate()
        {
            DateTime date = this.profileUserObj.user.signUpDate;
            return date;
        }
    }
}