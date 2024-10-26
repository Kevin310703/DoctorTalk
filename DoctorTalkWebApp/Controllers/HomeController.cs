using DoctorTalkWebApp.Data.Interfaces;
using DoctorTalkWebApp.Data.Models;
using DoctorTalkWebApp.Models;
using DoctorTalkWebApp.Models.Forum;
using DoctorTalkWebApp.Models.Home;
using DoctorTalkWebApp.Models.Post;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DoctorTalkWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPost _postService;

        public HomeController(ILogger<HomeController> logger, IPost postService)
        {
            _logger = logger;
            _postService = postService;
        }

        public IActionResult Index()
        {
            var model = BuildHomeIndexModel();
            return View(model);
        }

        private HomeIndexModel BuildHomeIndexModel()
        {
            var lastestPosts = _postService.GetLastestPosts(10);
            //var lastestPosts = _postService.GetLastestPosts(10)
            //    .Include(p => p.User).Include(p => p.Doctor);

            var posts = lastestPosts.Select(post => new PostListingModel
            {
                Id = post.Id,
                Title = post.Title,
                AuthorName = post.User?.UserName ?? "Unknown",
                AuthorId = post.User?.Id ?? "Unknown",
                AuthorRating = post.Doctor?.Rating ?? 0,
                DatePosted = post.Created.ToString(),
                Forum = GetForumListingForPost(post)
            });

            return new HomeIndexModel
            {
                LastestPost = posts,
                SearchQuery = ""
            };
        }

        private ForumListingModel GetForumListingForPost(Post post)
        {
            var forum = post.Forum;

            return new ForumListingModel
            {
                Id = forum.Id,
                Name = forum.Title,
                ForumImageUrl = forum.ImageUrl
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
