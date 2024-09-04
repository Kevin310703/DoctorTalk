using DoctorTalkWebApp.Data.Interfaces;
using DoctorTalkWebApp.Data.Models;
using DoctorTalkWebApp.Models.Forum;
using DoctorTalkWebApp.Models.Post;
using DoctorTalkWebApp.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace DoctorTalkWebApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPost _postService;

        public SearchController(IPost postService)
        {
            _postService = postService;
        }

        public IActionResult Results(string searchQuery)
        {
            var posts = _postService.GetFilteredPosts(searchQuery).ToList();
            var noResults = (!string.IsNullOrEmpty(searchQuery) && !posts.Any());

            var postListings = posts.Select(post => new PostListingModel
            {
                Id = post.Id,
                Forum = BuildForumListing(post),
                AuthorName = post.User.UserName,
                AuthorId = post.User.Id,
                AuthorRating = post.User.Rating,
                Title = post.Title,
                DatePosted = post.Created.ToString(),
                RepliesCount = post.Replies.Count()
            }).OrderByDescending(post => post.DatePosted);

            var model = new SearchResultModel
            {
                EmptySearchResults = noResults,
                Posts = postListings,
                SearchQuery = searchQuery,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Search(string searchQuery)
        {
            return RedirectToAction("Results", new { searchQuery });
        }

        private static ForumListingModel BuildForumListing(Data.Models.Forum forum)
        {
            return new ForumListingModel
            {
                Id = forum.Id,
                ForumImageUrl = forum.ImageUrl,
                Name = forum.Title,
                Description = forum.Description
            };
        }

        private static ForumListingModel BuildForumListing(Post post)
        {
            var forum = post.Forum;
            return BuildForumListing(forum);
        }
    }
}
