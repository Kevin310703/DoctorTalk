using DoctorTalkWebApp.Data;
using DoctorTalkWebApp.Data.Interfaces;
using DoctorTalkWebApp.Data.Models;
using DoctorTalkWebApp.Models.Reply;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DoctorTalkWebApp.Controllers
{
    [Authorize]
    public class ReplyController : Controller
    {
        private readonly IPost _postService;
        private readonly IApplicationUser _userService;
        private readonly UserManager<DoctorTalkWebAppUser> _userManager;

        public ReplyController(IPost postService, 
            UserManager<DoctorTalkWebAppUser> userManager,
            IApplicationUser userService)
        {
            _postService = postService;
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<IActionResult> Create(int id)
        {
            var post = _postService.GetById(id);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var doctor = _userService.GetDoctorByUserId(user.Id);

            var model = new PostReplyModel
            {
                PostContent = post.Content,
                PostTitle = post.Title,
                PostId = post.Id,
                AuthorName = User.Identity.Name,
                AuthorImageUrl = doctor.ProfilePicture,
                AuthorRating = doctor.Rating,
                IsAuthorAdmin = User.IsInRole("Admin"),
                Created = DateTime.Now,
                ForumId = post.Forum.Id,
                ForumName = post.Forum.Title,
                ForumImageUrl = post.Forum.ImageUrl
            };

            return View(model);
        }

        public async Task<IActionResult> AddReply(PostReplyModel model, int? parentReplyId)
        {
            Console.WriteLine("PostId: " + model.PostId);
            Console.WriteLine("Id: " + model.Id);
            // Retrieve the post by PostId
            var post = _postService.GetById(model.PostId);
            if (post == null)
            {
                // Handle the case where the post does not exist
                return NotFound("Post not found.");
            }

            // Retrieve the parent reply if it exists
            PostReply parentReply = null;
            if (parentReplyId.HasValue)
            {
                parentReply = _postService.GetReplyById(parentReplyId.Value);
                if (parentReply == null)
                {
                    // Handle the case where the parent reply does not exist
                    return NotFound("Parent reply not found.");
                }
            }

            // Retrieve the current user and their associated doctor
            var user = await _userManager.GetUserAsync(User);
            var doctor = _userService.GetDoctorByUserId(user.Id);

            if (doctor == null)
            {
                // Handle the case where the user does not have an associated doctor
                // You can create a new doctor record or return an error
                return BadRequest("Associated doctor not found for the user.");
            }

            // Proceed with creating the reply
            var reply = new PostReply
            {
                Post = post,
                Content = model.ReplyContent,
                Created = DateTime.Now,
                ParentReply = parentReply,
                User = user,
                Doctor = doctor
            };

            await _postService.AddReply(reply);

            return RedirectToAction("Index", "Post", new { id = model.PostId });
        }

        private PostReply BuildReply(PostReplyModel model, DoctorTalkWebAppUser? user, Doctor doctor, int? parentReplyId)
        {
            var post = _postService.GetById(model.PostId);
            var parentReply = parentReplyId.HasValue 
                ? _postService.GetReplyById(parentReplyId.Value) 
                : null;

            return new PostReply
            {
                Post = post,
                Content = model.ReplyContent,
                Created = DateTime.Now,
                User = user,
                Doctor = doctor,
                ParentReply = parentReply
            };
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReply(PostReplyModel model)
        {
            var reply = _postService.GetReplyById(model.Id);

            // Kiểm tra quyền chỉnh sửa
            if (reply.User.Id != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            // Cập nhật nội dung reply
            reply.Content = model.ReplyContent;
            reply.Updated = DateTime.Now; // Lưu thời gian chỉnh sửa

            await _postService.UpdateReply(reply);

            return RedirectToAction("Index", "Post", new { id = reply.Post.Id });
        }

        private PostReplyModel BuildReplyModel(PostReply reply)
        {
            return new PostReplyModel
            {
                Id = reply.Id,
                ReplyContent = reply.Content,
                Created = reply.Created,
                Updated = reply.Updated,
                PostId = reply.Post.Id,
                ParentReplyId = reply.ParentReply?.Id,
                ParentReply = reply.ParentReply != null ? BuildMinimalReplyModel(reply.ParentReply) : null,
                ChildReplies = reply.ChildReplies?.Select(BuildMinimalReplyModel).ToList(),
                AuthorId = reply.User.Id,
                AuthorName = reply.User.UserName,
                AuthorImageUrl = reply.Doctor?.ProfilePicture ?? "default_profile.png",
                AuthorRating = reply.Doctor?.Rating ?? 0,
                IsAuthorAdmin = _userManager.IsInRoleAsync(reply.User, "Admin").Result
            };
        }

        private PostReplyModel BuildMinimalReplyModel(PostReply reply)
        {
            return new PostReplyModel
            {
                Id = reply.Id,
                ReplyContent = reply.Content,
                Created = reply.Created,
                AuthorId = reply.User.Id,
                AuthorName = reply.User.UserName,
                AuthorImageUrl = reply.Doctor?.ProfilePicture ?? "default_profile.png",
                AuthorRating = reply.Doctor?.Rating ?? 0,
                IsAuthorAdmin = _userManager.IsInRoleAsync(reply.User, "Admin").Result
            };
        }

    }
}
