using DoctorTalkWebApp.Data;
using DoctorTalkWebApp.Data.Interfaces;
using DoctorTalkWebApp.Data.Models;
using DoctorTalkWebApp.Models.Post;
using DoctorTalkWebApp.Models.Reply;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorTalkWebApp.Controllers
{
    public class PostController : Controller
    {
        private readonly DoctorTalkWebAppContext _context;
        private readonly IPost _postService;
        private readonly IForum _forumService;
        private readonly IApplicationUser _userService;
        private static UserManager<DoctorTalkWebAppUser>? _userManager;

        public PostController(IPost postService, 
            IForum forumService, 
            UserManager<DoctorTalkWebAppUser> userManager,
            IApplicationUser userService,
            DoctorTalkWebAppContext context) 
        { 
            _postService = postService;
            _forumService = forumService;
            _userManager = userManager;
            _userService = userService;
            _context = context;
        }

        public IActionResult Index(int id)
        {
            var post = _postService.GetById(id);
            if (post == null)
            {
                return NotFound();  // Trả về trang 404 nếu không tìm thấy bài post
            }

            Console.WriteLine("Profile: " + post.Doctor?.Rating);

            var replies = BuildPostReplies(post.Replies);

            var model = new PostIndexModel
            {
                Id = post.Id,
                Title = post.Title,
                AuthorId = post.User?.Id ?? string.Empty,  // Đảm bảo post.User không null
                AuthorName = post.User?.UserName ?? "Unknown", // Đảm bảo không null
                AuthorImageUrl = post.Doctor?.ProfilePicture ?? string.Empty, // Đảm bảo Doctor không null
                AuthorRating = post.Doctor?.Rating ?? 0, // Đảm bảo không null
                Created = post.Created,
                PostContent = post.Content,
                Replies = replies,
                ForumId = post.Forum?.Id ?? 0,  // Đảm bảo không null
                ForumName = post.Forum?.Title ?? "Unknown", // Đảm bảo không null
                IsAuthorAdmin = IsAuthorAdmin(post.User)
            };

            return View(model);
        }

        [Authorize]
        public IActionResult Create(int id)
        {
            var forum = _forumService.GetById(id);

            var model = new NewPostModel
            {
                ForumName = forum.Title,
                ForumId = forum.Id,
                ForumImageUrl = forum.ImageUrl,
                AuthorName = User.Identity.Name
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost(NewPostModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId).Result;

            var post = BuildPost(model, user);

            await _postService.Add(post);
            await _userService.UpdateUserRating(userId, typeof(Post));

            return RedirectToAction("Index", "Post", new { id = post.Id });
        }

        private bool IsAuthorAdmin(DoctorTalkWebAppUser user)
        {
            return _userManager.GetRolesAsync(user).Result.Contains("Admin");
        }

        private Post BuildPost(NewPostModel model, DoctorTalkWebAppUser? user)
        {
            var forum = _forumService.GetById(model.ForumId);

            return new Post
            {
                Title = model.Title,
                Content = model.Content,
                Created = DateTime.Now,
                User = user,
                Forum = forum
            };
        }

        private IEnumerable<PostReplyModel> BuildPostReplies(IEnumerable<PostReply>? replies)
        {
            return replies.Select(reply => new PostReplyModel
            {
                Id = reply.Id,
                AuthorId = reply.User.Id ?? string.Empty,
                AuthorName = reply.User.UserName ?? "Unknown",
                AuthorImageUrl = reply.Doctor.ProfilePicture ?? string.Empty,
                AuthorRating = reply.Doctor.Rating,
                Created = reply.Created,
                ReplyContent = reply.Content,
                IsAuthorAdmin = IsAuthorAdmin(reply.User)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int PostId, string Title, string Content)
        {
            // Lấy bài viết từ cơ sở dữ liệu
            var post = _postService.GetById(PostId);

            if (post == null)
            {
                return NotFound();
            }

            // Kiểm tra xem người dùng hiện tại có phải là tác giả không
            if (post.User.Id != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            // Cập nhật tiêu đề và nội dung
            post.Title = Title;
            post.Content = Content;
            post.Updated = DateTime.Now; // Nếu bạn có trường Updated để lưu thời gian cập nhật

            // Lưu thay đổi
            _context.Update(post);
            await _context.SaveChangesAsync();

            TempData["EditSuccess"] = "Your post has been updated successfully!";

            // Chuyển hướng về trang bài viết
            return RedirectToAction("Index", "Post", new { id = PostId });
        }

    }
}
