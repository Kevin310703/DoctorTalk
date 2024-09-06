using DoctorTalkWebApp.Data;
using DoctorTalkWebApp.Data.Interfaces;
using DoctorTalkWebApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorTalkWebApp.Services
{
    public class ForumService : IForum
    {
        private readonly DoctorTalkWebAppContext _context;

        public ForumService(DoctorTalkWebAppContext context)
        {
            _context = context;
        }

        public async Task Create(Forum forum)
        {
            _context.Add(forum);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int forumId)
        {
            var forum = GetById(forumId);
            _context.Remove(forum);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<DoctorTalkWebAppUser> GetActiveUsers(int id)
        {
            var posts = GetById(id).Posts;
            
            if (posts != null || !posts.Any())
            {
                var postUsers = posts.Select(p => p.User);
                var replyUsers = posts.SelectMany(p => p.Replies).Select(r => r.User);
                return postUsers.Union(replyUsers).Distinct();
            }

            return new List<DoctorTalkWebAppUser>();
        }

        public IEnumerable<Forum> GetAll()
        {
            return _context.Forums
                .Include(forum => forum.Posts);
        }

        public IEnumerable<DoctorTalkWebAppUser> GetAllActiveUsers()
        {
            throw new NotImplementedException();
        }

        public Forum GetById(int id)
        {
            var forum = _context.Forums
                .Where(f => f.Id == id)
                .Include(f => f.Posts).ThenInclude(p => p.User)
                .Include(f => f.Posts).ThenInclude(p => p.Replies).ThenInclude(r => r.User)
                .FirstOrDefault();

            // Ensure Posts is initialized if null
            if (forum != null && forum.Posts == null)
            {
                forum.Posts = new List<Post>();
            }

            return forum;
        }

        public bool HasRecentPost(int id)
        {
            const int hoursAgo = 12;
            var window = DateTime.Now.AddHours(-hoursAgo);
            return GetById(id).Posts.Any(post => post.Created > window);
        }

        public Task UpdateForumDescription(int forumId, string newDescription)
        {
            throw new NotImplementedException();
        }

        public Task UpdateForumTitle(int forumId, string newTitle)
        {
            throw new NotImplementedException();
        }
    }
}
