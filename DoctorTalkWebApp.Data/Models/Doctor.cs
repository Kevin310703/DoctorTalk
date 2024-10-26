using DoctorTalkWebApp.Data;
using Microsoft.EntityFrameworkCore;

public class Doctor
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Specialization { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string LicenseNumber { get; set; }
    public string Address { get; set; }
    public string Biography { get; set; }
    public string ProfilePicture { get; set; }
    public string Rank { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime MemberSince { get; set; }
    public int Rating { get; set; }
    public string UserId { get; set; }  // Thêm thuộc tính này
    public virtual DoctorTalkWebAppUser User { get; set; }
}
