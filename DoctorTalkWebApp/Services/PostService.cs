using DoctorTalkWebApp.Data;
using DoctorTalkWebApp.Data.Interfaces;
using DoctorTalkWebApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorTalkWebApp.Services
{
    public class PostService : IPost
    {
        private readonly DoctorTalkWebAppContext _context;

        public PostService(DoctorTalkWebAppContext context) 
        {
            _context = context;
        }

        public async Task Add(Post post)
        {
            _context.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task AddReply(PostReply reply)
        {
            _context.PostReplies.Add(reply);
            await _context.SaveChangesAsync();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task EditPostContent(int id, string newContent)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetAll()
        {
            return _context.Posts
                .Include(post => post.User)
                .Include(post => post.Replies).ThenInclude(reply => reply.User)
                .Include(post => post.Forum);
        }

        public IEnumerable<Post> GetAllPostsByUserId(string userId)
        {
            return _context.Posts
                .Include(p => p.Forum)
                .Include(p => p.User)
                .Where(p => p.User.Id == userId)
                .OrderByDescending(p => p.Created)
                .ToList();
        }

        public Post GetById(int id)
        {
            return _context.Posts.Where(p => p.Id == id)
                .Include(post => post.User)
                .Include(p => p.Doctor)
                .Include(post => post.Replies).ThenInclude(reply => reply.User)
                .Include(post => post.Forum)
                .FirstOrDefault(); // Changed from .First() to .FirstOrDefault()
        }

        public IEnumerable<Post> GetFilteredPosts(Forum forum, string searchQuery)
        {
            return String.IsNullOrEmpty(searchQuery)
                ? forum.Posts
                : forum.Posts.Where(post
                    => post.Title.Contains(searchQuery)
                    || post.Content.Contains(searchQuery));
        }

        public IEnumerable<Post> GetFilteredPosts(string searchQuery)
        {
            var normalized = searchQuery.ToLower();
            return GetAll().Where(post
                => post.Title.ToLower().Contains(normalized)
                || post.Content.ToLower().Contains(normalized));
        }

        public IEnumerable<Post> GetLastestPosts(int n)
        {
            return GetAll().OrderByDescending(post => post.Created).Take(n);    
        }

        public IEnumerable<Post> GetPostsByForums(int id)
        {
            return _context.Forums
                .Where(forum => id == forum.Id).First()
                .Posts;
        }

        public PostReply GetReplyById(int replyId)
        {
            return _context.PostReplies
                       .Include(r => r.User)     // Bao gồm thông tin của User (nếu cần)
                       .Include(r => r.Doctor)   // Bao gồm thông tin của Doctor (nếu cần)
                       .Include(r => r.Post)     // Bao gồm thông tin của Post liên quan (nếu cần)
                       .FirstOrDefault(r => r.Id == replyId);
        }

        public async Task UpdateReply(PostReply reply)
        {
            _context.PostReplies.Update(reply);
            await _context.SaveChangesAsync();
        }
    }
}
