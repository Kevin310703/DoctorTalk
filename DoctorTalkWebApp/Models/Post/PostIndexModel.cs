﻿using DoctorTalkWebApp.Models.Forum;
using DoctorTalkWebApp.Models.Reply;

namespace DoctorTalkWebApp.Models.Post
{
    public class PostIndexModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorImageUrl { get; set; }
        public int AuthorRating { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string? PostContent { get; set; }
        public bool IsAuthorAdmin { get; set; }

        public int ForumId { get; set; }
        public string? ForumName { get; set; }

        public IEnumerable<PostReplyModel>? Replies { get; set; }
    }
}
