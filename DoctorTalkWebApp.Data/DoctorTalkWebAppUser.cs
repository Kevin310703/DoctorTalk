using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DoctorTalkWebApp.Data;

// Add profile data for application users by adding properties to the DoctorTalkWebAppUser class
public class DoctorTalkWebAppUser : IdentityUser
{
    public int Rating { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime MemberSince { get; set; }
    public bool IsActive { get; set; }
}

