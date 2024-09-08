using DoctorTalkWebApp.Data;
using DoctorTalkWebApp.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Linq;

namespace DoctorTalkWebApp.Test
{
    [TestFixture]
    public class Post_Service_Should
    {
        [Test]
        public void Return_Filtered_Results_Corresponding_To_Query()
        {
            // Set up the DbContextOptions to use an in-memory database
            var options = new DbContextOptionsBuilder<DoctorTalkWebAppContext>()
                .UseInMemoryDatabase(databaseName: "Search_Database")  // Correct method and parameter name
                .Options;

            // Create an instance of your context using the in-memory database options
            using (var context = new DoctorTalkWebAppContext(options))
            {
                context.Forums.Add(new Data.Models.Forum
                {
                    Id = 19
                });
                context.Posts.Add(new Data.Models.Post
                {
                    Forum = context.Forums.Find(19),
                    Id = 23523,
                    Title = "First Post",
                    Content = "Coffee"
                });
                context.Posts.Add(new Data.Models.Post
                {
                    Forum = context.Forums.Find(19),
                    Id = 21445,
                    Title = "Second Post",
                    Content = "Tea"
                });
                context.Posts.Add(new Data.Models.Post
                {
                    Forum = context.Forums.Find(19),
                    Id = 21345,
                    Title = "Third Post",
                    Content = "Water"
                });
                context.SaveChanges();
            }

            using (var context = new DoctorTalkWebAppContext(options))
            {
                var postService = new PostService(context);
                var result = postService.GetFilteredPosts("Post");
                var postCount = result.Count();

                ClassicAssert.AreEqual(0, postCount);
            }
        }
    }
}
