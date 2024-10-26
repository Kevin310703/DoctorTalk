using DoctorTalkWebApp.Data.Interfaces;
using DoctorTalkWebApp.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DoctorTalkWebApp.Data;

public class DoctorTalkWebAppContext : IdentityDbContext<DoctorTalkWebAppUser>
{
    public DoctorTalkWebAppContext(DbContextOptions<DoctorTalkWebAppContext> options)
        : base(options)
    {
    }

    public DbSet<DoctorTalkWebAppUser> ApplicationUsers { get; set; }
    public DbSet<Forum> Forums { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostReply> PostReplies { get; set; }
    public DbSet<Doctor> Doctors { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<Doctor>()
        .HasOne(d => d.User)
        .WithOne(u => u.Doctor)
        .HasForeignKey<Doctor>(d => d.UserId)  // Giả sử rằng Doctor sử dụng Id của User làm khóa ngoại
        .OnDelete(DeleteBehavior.Restrict);  // Sử dụng Restrict để tránh các vấn đề về CASCADE
    }
}
