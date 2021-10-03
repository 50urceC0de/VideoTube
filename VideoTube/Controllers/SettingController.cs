using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VideoTube.Data;
using VideoTube.Data.Services;

namespace VideoTube.Controllers
{
    public class SettingController : Controller
    {
        // GET: Setting

        private IConnectionConfiguration con;
        private User userLoggedInObj;
        public SettingController(IConnectionConfiguration _con)
        {
            con = _con;
            string usernameLoggedIn = VideoTube.Data.User.isLoggedIn() ? System.Web.HttpContext.Current.Session["userLoggedIn"].ToString() : "";
            userLoggedInObj = new User(con, usernameLoggedIn);
        }
        public ActionResult Index()
        {
            if (!VideoTube.Data.User.isLoggedIn())
            {
                return RedirectToAction("Index", "SignIn");
            }
            var detailsMessage = "";
            var passwordMessage = "";
            var formProvider = new SettingsFormProvider();

            ViewBag.data = formProvider.createUserDetailsForm(userLoggedInObj.user.firstName, userLoggedInObj.user.lastName, userLoggedInObj.user.email);

            ViewBag.passwordForm = formProvider.createPasswordForm();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(string firstName, string lastName, string email)
        {
            //            if (isset($_POST["saveDetailsButton"]))
            //            {
            var account = new Account(con);


            if (await account.updateDetails(firstName, lastName, email, userLoggedInObj.getUsername()))
            {
                string detailsMessage = "<div class='alert alert-success'> < strong > SUCCESS! </ strong > Details updated successfully!  </ div > ";
            }
            else
            {
                // var errorMessage = account.getFirstError();

                // if ($errorMessage == "")
                var errorMessage = "Something went wrong";

                var detailsMessage = "<div class='alert alert-danger'>  <strong > ERROR! </strong >" + errorMessage + "</div> ";
            }
            //            }

            //            if (isset($_POST["savePasswordButton"]))
            //            {

            //            }
            return View();
        }

        public async Task<ActionResult> SavePassword(string oldPassword, string newPassword, string newPassword2)
        {
            var account = new Account(con);


            if (await account.updatePassword(oldPassword, newPassword, newPassword2, userLoggedInObj.getUsername()))
            {
                var passwordMessage = "<div class='alert alert-success'> < strong > SUCCESS! </ strong > Password updated successfully!  </ div > ";
            }
            else
            {
                // $errorMessage = $account->getFirstError();

                //if ($errorMessage == "") $
                var errorMessage = "Something went wrong";

                var passwordMessage = "<div class='alert alert-danger'>  < strong > ERROR! </ strong > " + errorMessage + "</ div > ";
            }
            return View();
        }
    }
}