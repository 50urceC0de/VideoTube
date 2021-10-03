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
    public class SignInController : Controller
    {
        IConnectionConfiguration con ;
        // GET: SignIn
        public SignInController(IConnectionConfiguration _con)
        {
            con = _con;
        }

        public ActionResult Index()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(string username,string password)
        {
            if (!username.isNullOrEmpty()&&!password.isNullOrEmpty())
            {
                var account = new Account(con);
                bool wasSuccessful =await account.login(username, password);
                if (wasSuccessful)
                {
                    Session["userLoggedIn"] = username;
                    return RedirectToAction("index", "Home");
                }
                ViewBag.msg = "Your username or password was incorrect ";
            }
            return View();
        }
    }
}