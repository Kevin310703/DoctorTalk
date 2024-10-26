using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoctorTalkWebApp.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace DoctorTalkWebApp.Data;

// Add profile data for application users by adding properties to the DoctorTalkWebAppUser class
public class DoctorTalkWebAppUser : IdentityUser
{
    public bool IsActive { get; set; }
    public Doctor Doctor { get; set; }
}

