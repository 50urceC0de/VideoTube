using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VideoTube.Data;
using VideoTube.Data.Services;
namespace VideoTube.Models
{
    public class SignUpController : Controller
    {
        IConnectionConfiguration con;
        public SignUpController(IConnectionConfiguration _con)
        {
            con = _con;
        }
        // GET: SignUp
        [HttpGet]
        public ActionResult Index()
        {
            Users user = new Users();
            return View(user);
        }
        [HttpPost]
        public async Task<ActionResult> Index(Users user)
        {
            if (ModelState.IsValid)
            {
                var account = new Account(con);
                bool wasSuccessful = await account.register(user);
                if (wasSuccessful)
                {
                    Session["userLoggedIn"] = user.username;
                    return RedirectToAction("index","Home");
                }
            }
            return View(user);
        }
        [HttpPost]
        public ActionResult UsernameExists(string username)
        {
            return Json(Account.usernameExist(username, con), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EmailExists(string username,string email)
        {
            return Json(Account.existEmail(email,username, con), JsonRequestBehavior.AllowGet);
        }
    }
}