using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Data;
using VideoTube.Models;
using System.Web.Mvc;
using System.Web;
using VideoTube.Data.Services;

namespace VideoTube.Data
{
    public class User
    {

        private IConnectionConfiguration con;
        public Users user;

        public User(IConnectionConfiguration con, string username)
        {
            this.con = con;

            string query = ("SELECT * FROM users WHERE username = '" + username + "'");
            using (var conn = new SqlConnection(con.Value))
            {
                this.user = conn.QueryFirstOrDefault<Users>(query, commandType: CommandType.Text);
                this.user = this.user == null ? new Users() : this.user;
            }
        }


        public string getUsername()
        {
            return this.user.username;
        }

        public async Task<bool> isSubscribedTo(string userTo)
        {
            string username = this.user.username;
            string query = ("SELECT * FROM subscribers WHERE userTo='" + userTo + "' AND userFrom='" + username + "'");
            IEnumerable<Subcription> subcription;
            using (var conn = new SqlConnection(con.Value))
            {
                subcription = await conn.QueryAsync<Subcription>(query, commandType: CommandType.Text);
            }
            return subcription.Count<Subcription>() > 0;
        }

        public async Task<int> getSubscriberCount()
        {
            string query = ("SELECT * FROM subscribers WHERE userTo='" + this.user.username + "'");
            IEnumerable<Subcription> subcription;
            using (var conn = new SqlConnection(con.Value))
            {
                subcription = await conn.QueryAsync<Subcription>(query, commandType: CommandType.Text);
            }
            return subcription.Count<Subcription>();
        }
        public static bool isLoggedIn()
        {
            return (HttpContext.Current.Session["userLoggedIn"]) != null;
        }
        public List<User> getSubscriptions()
        {
            string query = ("SELECT userTo FROM subscribers WHERE userFrom='" + this.user.username + "'");
            IEnumerable<Subcription> subcription;
            using (var conn = new SqlConnection(con.Value))
            {
                subcription =  conn.Query<Subcription>(query, commandType: CommandType.Text);
            }

            var subs = new List<User>();
            foreach (var row in subcription)
            {
                User user = new User(this.con, row.userTo);
                subs.Add(user);
            }
            return subs;
        }

    }
}