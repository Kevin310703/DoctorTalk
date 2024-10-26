using Microsoft.AspNetCore.Identity;

namespace DoctorTalkWebApp.Models.ApplicationUser
{
    public class ProfileListModel
    {
        public IEnumerable<ProfileModel>? Profiles { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }
    }
}
