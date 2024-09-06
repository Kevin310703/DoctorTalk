namespace DoctorTalkWebApp.Models.Forum
{
    public class ForumListingModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ForumImageUrl { get; set; }

        public int NumberOfPosts { get; set; }
        public int NumberOfUsers { get; set; }
        public bool HasRecentPost { get; set; }
    }
}
