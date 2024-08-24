using DoctorTalkWebApp.Models.Post;

namespace DoctorTalkWebApp.Models.Home
{
    public class HomeIndexModel
    {
        public string SearchQuery { get; set; }
        public IEnumerable<PostListingModel>? LastestPost {  get; set; }
    }
}
