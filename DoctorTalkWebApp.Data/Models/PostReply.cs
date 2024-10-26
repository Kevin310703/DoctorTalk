namespace DoctorTalkWebApp.Data.Models
{
    public class PostReply
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public virtual DoctorTalkWebAppUser? User { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Post? Post { get; set; }

        // New properties for nested replies
        public int? ParentReplyId { get; set; } // Foreign key for ParentReply
        public virtual PostReply ParentReply { get; set; } // Navigation property

        public virtual ICollection<PostReply> ChildReplies { get; set; } = new List<PostReply>(); // Child replies
    }
}