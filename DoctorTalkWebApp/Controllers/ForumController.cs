using DoctorTalkWebApp.Data.Interfaces;
using DoctorTalkWebApp.Data.Models;
using DoctorTalkWebApp.Models.Forum;
using DoctorTalkWebApp.Models.Post;
using DoctorTalkWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Http.Headers;

namespace DoctorTalkWebApp.Controllers
{
    public class ForumController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IForum _forumService;
        private readonly IPost _postService;
        private readonly IUpload _uploadService;
        private readonly IConfiguration _configuration;

        public ForumController(IForum forumService, IPost postService, 
            IUpload uploadService, IConfiguration configuration
            , IWebHostEnvironment hostingEnvironment)
        {
            _forumService = forumService;
            _postService = postService;
            _uploadService = uploadService;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var forums = _forumService.GetAll()
                .Select(forum => new Models.Forum.ForumListingModel
                {
                    Id = forum.Id,
                    Name = forum.Title,
                    Description = forum.Description,
                    NumberOfPosts = forum.Posts?.Count() ?? 0,
                    NumberOfUsers = _forumService.GetActiveUsers(forum.Id).Count(),
                    ForumImageUrl = forum.ImageUrl,
                    HasRecentPost = _forumService.HasRecentPost(forum.Id)
                });

            var model = new Models.Forum.ForumIndexModel
            {
                ForumListing = forums
            };

            return View(model); // Pass the model to the view here
        }

        public IActionResult Topic(int id, string searchQuery)
        {
            var forum = _forumService.GetById(id);
            var posts = new List<Post>();

            posts = _postService.GetFilteredPosts(forum, searchQuery).ToList();

            var postListings = posts.Select(post => new PostListingModel
            {
                Id = post.Id,
                AuthorId = post.User?.Id ?? Guid.Empty.ToString(),  // Handle null User
                AuthorRating = post.Doctor?.Rating ?? 0,            // Handle null Doctor and default to 0 rating
                AuthorName = post.User?.UserName ?? "Unknown",      // Handle null User and provide a default name
                Title = post.Title,
                DatePosted = post.Created.ToString(),
                RepliesCount = post.Replies?.Count() ?? 0,          // Handle null Replies and default to 0
                Forum = BuildForumListing(post)
            }).ToList();

            var model = new ForumTopicModel
            {
                Posts = postListings,
                Forum = BuildForumListing(forum)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Search(int id, string searchQuery)
        {
            return RedirectToAction("Topic", new {id, searchQuery });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new AddForumModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddForum(AddForumModel model)
        {
            var imageUri = "/images/user/default-female-1.jpg";

            if (model.ImageUpload != null)
            {
                imageUri = await UploadForumImage(model.ImageUpload);
            }

            var forum = new Forum()
            {
                Title = model.Title,
                Description = model.Description,
                Created = DateTime.Now,
                ImageUrl = imageUri
            };

            await _forumService.Create(forum);

            return RedirectToAction("Index", "Forum");
        }

        private async Task<string> UploadForumImage(IFormFile file)
        {
            // Xác định thư mục lưu trữ tệp trong dự án
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/forum");

            // Kiểm tra và tạo thư mục nếu nó chưa tồn tại
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Tạo tên tệp duy nhất để tránh đụng độ với các tệp khác
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            // Xác định đường dẫn tệp đầy đủ
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Lưu tệp vào thư mục của dự án
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn URL của tệp, để có thể truy cập từ trình duyệt
            return "/images/forum/" + uniqueFileName;
        }

        private ForumListingModel BuildForumListing(Post post)
        {
            var forum = post.Forum;

            return BuildForumListing(forum);
        }

        private ForumListingModel BuildForumListing(Forum forum)
        {
            return new ForumListingModel
            {
                Id = forum.Id,
                Name = forum.Title,
                Description = forum.Description,
                ForumImageUrl = forum.ImageUrl
            };
        }
    }
}
