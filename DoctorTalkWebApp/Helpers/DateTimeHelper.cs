namespace DoctorTalkWebApp.Helpers
{
    public class DateTimeHelper
    {
        public static string GetTimeAgo(DateTime createdTime)
        {
            var timeSinceCreation = DateTime.Now - createdTime;
            string timeAgo;

            if (timeSinceCreation.TotalDays >= 1)
            {
                timeAgo = $"{(int)timeSinceCreation.TotalDays} days ago";
            }
            else if (timeSinceCreation.TotalHours >= 1)
            {
                timeAgo = $"{(int)timeSinceCreation.TotalHours} hours ago";
            }
            else if (timeSinceCreation.TotalMinutes >= 1)
            {
                timeAgo = $"{(int)timeSinceCreation.TotalMinutes} minutes ago";
            }
            else
            {
                timeAgo = "Just now";
            }

            return timeAgo;
        }
    }
}
