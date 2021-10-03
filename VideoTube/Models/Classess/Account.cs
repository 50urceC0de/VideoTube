using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using VideoTube.Models;
using VideoTube.Data.Services;
using System.Security.Cryptography;
using System.Text;

namespace VideoTube.Data
{


    public class Account
    {

        private IConnectionConfiguration con;

        public Account(IConnectionConfiguration con)
        {
            this.con = con;
        }

        public async Task<bool> login(string un, string pw)
        {
            using (var sha = SHA512.Create())
            {
                byte[] sourceByte = Encoding.UTF8.GetBytes(pw);
                byte[] hashByte = sha.ComputeHash(sourceByte);
                pw = BitConverter.ToString(hashByte).Replace("-", String.Empty);
            }

            using (var conn = new SqlConnection(con.Value))
            {
                string query = ("SELECT * FROM users WHERE username='" + un + "' AND password='" + pw + "'");
                var data = await conn.QueryAsync<Users>(query, commandType: CommandType.Text);

                if (data.Count() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> register(Users user)
        {
            string pw = "";
            using (var sha = SHA512.Create())
            {
                byte[] sourceByte = Encoding.UTF8.GetBytes(user.password);
                byte[] hashByte = sha.ComputeHash(sourceByte);
                pw = BitConverter.ToString(hashByte).Replace("_", String.Empty);
            }
            string pic = "assets/images/profilePictures/default.png";

            using (var conn = new SqlConnection(con.Value))
            {
                string query = (@"INSERT INTO users (firstName, lastName, username, email, password, profilePic)   VALUES(@fn, @ln, @un, @em, @pw, @pic)");
                await conn.ExecuteAsync(query, new { user.firstName, user.lastName, user.username, user.email, pw, pic }, commandType: CommandType.Text);

                return true;
            }
        }

        public async Task<bool> updateDetails(string fn, string ln, string em, string un)
        {

            
                using (var conn = new SqlConnection(con.Value))
                {
                    string query = (@"UPDATE users SET firstName=@fn, lastName=@ln, email=@em WHERE username=@un");
                    await conn.ExecuteAsync(query, new { fn, ln, em, un }, commandType: CommandType.Text);
                    return true;
                }
             
        }

        public async Task<bool> updatePassword(string oldPw, string pw, string pw2, string un)
        {
            this.validateOldPassword(oldPw, un);
            using (var conn = new SqlConnection(con.Value))
            {
                string query = (@"UPDATE users SET password=@pw WHERE username=@un");
                using (var sha = SHA512.Create())
                {
                    byte[] sourceByte = Encoding.UTF8.GetBytes(pw);
                    byte[] hashByte = sha.ComputeHash(sourceByte);
                    pw = BitConverter.ToString(hashByte).Replace("_", String.Empty);
                }
                await conn.ExecuteAsync(query, new { pw, un }, commandType: CommandType.Text);
            }
            return true;
        }

        private bool validateOldPassword(string oldPw, string un)
        {
            string pw = "";
            using (var sha = SHA512.Create())
            {
                byte[] sourceByte = Encoding.UTF8.GetBytes(oldPw);
                byte[] hashByte = sha.ComputeHash(sourceByte);
                pw = BitConverter.ToString(hashByte).Replace("_", String.Empty);
            }

            using (var conn = new SqlConnection(con.Value))
            {
                string query = (@"SELECT Count(*) as 'count' FROM users WHERE username='"+un+"' AND password='"+pw+"'");
                var data =(int) conn.ExecuteScalar(query, commandType: CommandType.Text);

                if (data > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

         


        public static bool usernameExist(string un, IConnectionConfiguration con)
        {
            string query = (@"SELECT username FROM users WHERE username=@un");
            using (var conn = new SqlConnection(con.Value))
            {
                var data = conn.ExecuteScalar(query, new { un }, commandType: CommandType.Text);
                if (data != null)
                    return true;
                else
                    return false;
            }
        }

        public static bool existEmail(string em, string un, IConnectionConfiguration con)
        {
            string query = (@"SELECT count(email) as 'count' FROM users WHERE email=@em AND username == @un");
            using (var conn = new SqlConnection(con.Value))
            {
                var data =(int) conn.ExecuteScalar(query, new { em, un }, commandType: CommandType.Text);
                if (data != null)
                    return true;
                else
                    return false;
            }
        }
    }
}