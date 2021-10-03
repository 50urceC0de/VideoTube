using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VideoTube.Models
{
    public class Users
    {
        public int id { get; set; }
         

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"[^A-Za-z0-9]", ErrorMessage = "Your password can only contain letters and numbers")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Your password must be between 5 and 30 characters")]
        public string password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [System.ComponentModel.DataAnnotations.Compare("password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string confirmpassword { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Your first name must be between 2 and 25 characters")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Your last name must be between 2 and 25 characters")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$", ErrorMessage = "Please enter a valid email address")]
        [Remote("EmailExists", "SignUp", HttpMethod = "POST", ErrorMessage = "This email is already in use.")]
        public string email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Your username must be between 5 and 25 characters.")]
        [Remote("UsernameExists", "SignUp", HttpMethod = "POST", ErrorMessage = "User name already taken.")]
        public string username { get; set; }
        public string profilePic { get; set; }
        public DateTime signUpDate { get; set; }
        public string name {
            get { 
                return firstName + " " + lastName; 
            }
        }
    }
}