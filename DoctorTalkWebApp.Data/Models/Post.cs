namespace DoctorTalkWebApp.Data.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public virtual DoctorTalkWebAppUser? User { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Forum? Forum { get; set; }

        public virtual IEnumerable<PostReply>? Replies { get; set; }
    }
}