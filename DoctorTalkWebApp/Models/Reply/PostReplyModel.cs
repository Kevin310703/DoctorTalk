using DoctorTalkWebApp.Data.Models;

namespace DoctorTalkWebApp.Models.Reply
{
    public class PostReplyModel
    {
        public int Id { get; set; }
        public string? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorImageUrl { get; set; }
        public int AuthorRating { get; set; }
        public bool IsAuthorAdmin { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string? ReplyContent { get; set; }
        public int? ParentReplyId { get; set; }
        public PostReplyModel ParentReply { get; set; }
        public List<PostReplyModel> ChildReplies { get; set; } = new List<PostReplyModel>(); // List of child replies

        public int PostId { get; set; }
        public string? PostTitle { get; set; }
        public string? PostContent { get; set; }

        public string? ForumName { get; set; }
        public string? ForumImageUrl { get; set; }
        public int ForumId { get; set; }
    }
}
