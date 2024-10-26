using System.ComponentModel.DataAnnotations;

namespace DoctorTalkWebApp.Models.ApplicationUser
{
    public class ProfileModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string? UserName { get; set; }
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        public string? UserRating { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsAdmin { get; set; }

        public DateTime MemberSince { get; set; }
        public IFormFile? ImageUpLoad { get; set; }

        // Thêm các thuộc tính mới
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Specialization is required")]
        public string? Specialization { get; set; }
        public string? Address { get; set; }
        public string? Biography { get; set; }
        public string? LicenseNumber { get; set; }

        // Thêm thuộc tính để chứa danh sách bài viết
        public IEnumerable<DoctorTalkWebApp.Data.Models.Post> Posts { get; set; }

        public IList<string> Roles { get; set; }
    }
}
