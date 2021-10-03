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
    public class HomeController : Controller
    {
        private IConnectionConfiguration con;
        private User userLoggedInObj;
        public HomeController(IConnectionConfiguration _con)
        {
            con = _con;
            string usernameLoggedIn = VideoTube.Data.User.isLoggedIn() ? System.Web.HttpContext.Current.Session["userLoggedIn"].ToString() : "";
            userLoggedInObj = new User(con, usernameLoggedIn);
        }
        public async Task<ActionResult> Index()
        {
            var subscriptionsProvider = new SubscriptionsProvider(con, userLoggedInObj);
            List<Video> subscriptionVideos = await subscriptionsProvider.getVideos();
            var videoGrid = new VideoGrid(con, userLoggedInObj);

            if (VideoTube.Data.User.isLoggedIn() && subscriptionVideos.Count > 0)
            {
                ViewBag.data = await videoGrid.create(subscriptionVideos, "Subscriptions", false);
            }

            ViewBag.data = await videoGrid.create(null, "Recommended", false);
            return View();
        }

        public async Task<ActionResult> upload()
        {
            if (!VideoTube.Data.User.isLoggedIn())
            {
                return RedirectToAction("Index", "SignIn");
            }
            var formProvider = new VideoDetailsFormProvider(con);
            ViewBag.data = await formProvider.createUploadForm();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> processing(string titleInput, string descriptionInput, string privacyInput, string categoryInput)
        {
            // 1) create file upload data
            var videoUpoadData = new VideoUploadData(titleInput, descriptionInput, privacyInput, categoryInput,
                userLoggedInObj.getUsername()
            );
            // 2) Process video data (upload)
            var videoProcessor = new VideoProcessor(con);
            bool wasSuccessful = await videoProcessor.upload(videoUpoadData);

            // 3) Check if upload was successful
            if (wasSuccessful)
            {
                TempData["status"] = "Upload successful";
            }
            else
            {
                TempData["status"] = "Some Error Occured!!!";
            }
            return RedirectToAction("upload");
        }
        [HttpGet]
        public async Task<ActionResult> editVideo(int videoId)
        {
            if (!VideoTube.Data.User.isLoggedIn())
            {
                return RedirectToAction("Index", "SignIn");
            }

            if (videoId == 0)
            {
                throw new Exception("No video selected");
            }

            var video = new Video(con, videoId, userLoggedInObj);
            if (video._video.uploadedBy != userLoggedInObj.getUsername())
            {
                ViewBag.msg = "Not your video";
            }

            var videoPlayer = new VideoPlayer(video);
            ViewBag.vidPlayer = videoPlayer.create(false);

            var selectThumbnail = new SelectThumbnail(con, video);
            ViewBag.thumbnail = await selectThumbnail.create();

            var formProvider = new VideoDetailsFormProvider(con);
            ViewBag.form = await formProvider.createEditDetailsForm(video);
            return View();

        }
        [HttpPost]
        public async Task<ActionResult> editVideo(string titleInput, string descriptionInput, string privacyInput, string categoryInput, int videoId)
        {
            var video = new Video(con, videoId, userLoggedInObj);
            if (ModelState.IsValid)
            {
                //var detailsMessage = "";

                var videoData = new VideoUploadData(
                   titleInput,
                   descriptionInput,
                   privacyInput,
                   categoryInput,
                    userLoggedInObj.getUsername()
                );
                if (await videoData.updateDetails(con, video._video.id))
                {
                    ViewBag.msg = "<div class='alert alert-success'>  <strong> SUCCESS! </strong> Details updated successfully! </div > ";
                    video = new Video(con, videoId, userLoggedInObj);
                }
                else
                {
                    ViewBag.msg = "<div class='alert alert-danger'>  <strong> ERROR! </strong> Something went wrong   </div > ";
                }
            }

            var videoPlayer = new VideoPlayer(video);
            ViewBag.vidPlayer = videoPlayer.create(false);

            var selectThumbnail = new SelectThumbnail(con, video);
            ViewBag.thumbnail = await selectThumbnail.create();

            var formProvider = new VideoDetailsFormProvider(con);
            ViewBag.form = await formProvider.createEditDetailsForm(video);
            return View();
        }
        public async Task<ActionResult> likedVideos()
        {
            if (!VideoTube.Data.User.isLoggedIn())
            {
                return RedirectToAction("Index", "SignIn");
            }

            var likedVideosProvider = new LikedVideosProvider(con, userLoggedInObj);
            var videos = await likedVideosProvider.getVideos();

            var videoGrid = new VideoGrid(con, userLoggedInObj);

            if (videos.Count > 0)
            {
                ViewBag.data = await videoGrid.createLarge(videos, "Videos that you have liked", false);
            }
            else
            {
                ViewBag.data = "No videos to show";
            }
            return View();
        }
        public ActionResult logout()
        {
            System.Web.HttpContext.Current.Session["userLoggedIn"] = null;
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> profile(string username)
        {
            if (!username.isNullOrEmpty())
            {
                var profileGenerator = new ProfileGenerator(con, userLoggedInObj, username);
                ViewBag.data = await profileGenerator.create();
            }
            else
            {
                ViewBag.data = "Channel not found";

            }

            return View();
        }
        public async Task<ActionResult> search(string term, string orderBy)
        {
            if (term.isNullOrEmpty())
            {
                ViewBag.data = "You must enter a search term";
                return View();
            }



            if (!orderBy.isNullOrEmpty() || orderBy == "views")
            {
                orderBy = "views";
            }
            else
            {
                orderBy = "uploadDate";
            }

            var searchResultsProvider = new SearchResultsProvider(con, userLoggedInObj);
            var videos = await searchResultsProvider.getVideos(term, orderBy);

            var videoGrid = new VideoGrid(con, userLoggedInObj);

            if (videos.Count > 0)
            {
                ViewBag.data =await videoGrid.createLarge(videos, videos.Count + " results found", true);
            }
            else
            {
                ViewBag.data = "No results found";
            }
            return View();
        }
        public ActionResult settings()
        {
            return View();
        }

        public async Task<ActionResult> subscriptions()
        {
            if (!VideoTube.Data.User.isLoggedIn())
            {
                return RedirectToAction("Index", "SignIn");
            }

            var subscriptionsProvider = new SubscriptionsProvider(con, userLoggedInObj);
            var videos = await subscriptionsProvider.getVideos();

            var videoGrid = new VideoGrid(con, userLoggedInObj);

            if (videos.Count > 0)
            {
                ViewBag.data = await videoGrid.createLarge(videos, "New from your subscriptions", false);
            }
            else
            {
                ViewBag.data = "No videos to show";
            }
            return View();
        }
        public async Task<ActionResult> trending()
        {

            var trendingProvider = new TrendingProvider(con, userLoggedInObj);
            var videos = await trendingProvider.getVideos();

            var videoGrid = new VideoGrid(con, userLoggedInObj);

            if (videos.Count > 0)
            {
                ViewBag.data =await videoGrid.createLarge(videos, "Trending videos uploaded in the last week", false);
            }
            else
            {
                ViewBag.data = "No trending videos to show";
            }
            return View();
        }
        public async Task<ActionResult> watch(string id)
        {
            if (id.isNullOrEmpty())
            {

            }
            var video = new Video(con, id, userLoggedInObj);
            video.incrementViews();


            var videoPlayer = new VideoPlayer(video);
            ViewBag.palyer = videoPlayer.create(true);

            var _videoPlayer = new VideoInfoSection(con, video, userLoggedInObj);
            ViewBag.palyerInfo = await _videoPlayer.create();

            var commentSection = new CommentSection(con, video, userLoggedInObj);
            ViewBag.comment = await commentSection.create();

            var videoGrid = new VideoGrid(con, userLoggedInObj);
            ViewBag.vidGrid = await videoGrid.create(null, null, false);
            return View(video);
        }

    }
}