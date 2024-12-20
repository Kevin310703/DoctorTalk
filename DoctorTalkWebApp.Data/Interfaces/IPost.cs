﻿using DoctorTalkWebApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorTalkWebApp.Data.Interfaces
{
    public interface IPost
    {
        Post GetById(int id);
        IEnumerable<Post> GetAllPostsByUserId(string userId);
        IEnumerable<Post> GetAll();
        IEnumerable<Post> GetFilteredPosts(Forum forum, string searchQuery);
        IEnumerable<Post> GetFilteredPosts(string searchQuery);
        IEnumerable<Post> GetPostsByForums(int id);
        IEnumerable<Post> GetLastestPosts(int n);

        Task Add(Post post);
        Task Delete(int id);
        Task EditPostContent(int id, string newContent);

        PostReply GetReplyById(int replyId);
        Task AddReply(PostReply reply);
        Task UpdateReply(PostReply reply);
    }
}
