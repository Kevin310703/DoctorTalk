using DoctorTalkWebApp.Data;
using DoctorTalkWebApp.Data.Interfaces;
using DoctorTalkWebApp.Models.ApplicationUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Security.Claims;

namespace DoctorTalkWebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<DoctorTalkWebAppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly DoctorTalkWebAppContext _context;
        private readonly IApplicationUser _userService;
        private readonly IPost _postService;
        private readonly IUpload _uploadService;
        private readonly IConfiguration _configuration;

        public ProfileController(UserManager<DoctorTalkWebAppUser> userManager,
            IApplicationUser userService, IUpload uploadService, IConfiguration configuration,
            DoctorTalkWebAppContext context, IWebHostEnvironment hostingEnvironment, IPost postService,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userService = userService;
            _uploadService = uploadService;
            _configuration = configuration;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _postService = postService;
            _roleManager = roleManager;
        }

        public IActionResult Detail(string id)
        {
            var user = _userService.GetById(id);
            var doctor = _userService.GetDoctorByUserId(user.Id);
            var userRoles = _userManager.GetRolesAsync(user).Result;

            // Thêm tham số thời gian hiện tại vào URL ảnh để đảm bảo trình duyệt không dùng cache
            var profileImageUrl = doctor.ProfilePicture;
            if (!string.IsNullOrEmpty(profileImageUrl))
            {
                profileImageUrl += $"?t={DateTime.Now.Ticks}";
            }

            var model = new ProfileModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = doctor.FullName,
                UserRating = doctor.Rating.ToString(),
                Email = user.Email,
                ProfileImageUrl = profileImageUrl,
                MemberSince = doctor.MemberSince,
                IsAdmin = userRoles.Contains("Admin"),
                // Cập nhật thêm các trường mới
                PhoneNumber = doctor.PhoneNumber,
                Specialization = doctor.Specialization,
                LicenseNumber = doctor.LicenseNumber,
                Biography = doctor.Biography,
                Address = doctor.Address,
                Posts = _postService.GetAllPostsByUserId(user.Id)
            };

            return View(model);
        }

        // GET: Profile/Edit/id
        public IActionResult Edit(string id)
        {
            var user = _context.Users.Include(u => u.Doctor).FirstOrDefault(u => u.Id == id); // Include Doctor details

            if (user == null)
            {
                return NotFound();
            }

            var profileModel = new ProfileModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.Doctor.FullName,
                Email = user.Email,
                ProfileImageUrl = user.Doctor?.ProfilePicture, // Load existing profile picture
                PhoneNumber = user.Doctor.PhoneNumber,
                Specialization = user.Doctor.Specialization,
                LicenseNumber = user.Doctor.LicenseNumber,
                Biography = user.Doctor.Biography,
                Address = user.Doctor.Address
            };

            return View(profileModel); // Return view Edit with user's info
        }

        // POST: Profile/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileModel model, IFormFile ImageUpLoad)
        {
            Console.WriteLine("FullName: " + model.FullName);
            Console.WriteLine("UserId: " + model.UserId);
            Console.WriteLine("UserName: " + model.UserName);
            Console.WriteLine("Email: " + model.Email);
            Console.WriteLine("ProfileImageUrl: " + model.ProfileImageUrl);
            Console.WriteLine("PhoneNumber: " + model.PhoneNumber);
            Console.WriteLine("Specialization: " + model.Specialization);
            Console.WriteLine("Address: " + model.Address);
            Console.WriteLine("Biography: " + model.Biography);
            Console.WriteLine("LicenseNumber: " + model.LicenseNumber);

            // Nếu ImageUpLoad là null, xóa bất kỳ lỗi xác thực nào liên quan đến trường này
            ModelState.Remove("ImageUpLoad");

            if (ModelState.IsValid)
            {
                var user = await _context.Users.Include(u => u.Doctor).FirstOrDefaultAsync(u => u.Id == model.UserId);

                if (user != null)
                {
                    bool hasChanges = false;

                    // Cập nhật các thông tin của User nếu có thay đổi
                    if (user.Doctor.FullName != model.FullName || user.UserName != model.UserName || user.Email != model.Email)
                    {
                        user.Doctor.FullName = model.FullName;
                        user.UserName = model.UserName;
                        user.Email = model.Email;
                        hasChanges = true;
                    }

                    // Cập nhật thông tin Doctor nếu có thay đổi
                    if (user.Doctor != null)
                    {
                        if (user.Doctor.PhoneNumber != model.PhoneNumber || user.Doctor.Specialization != model.Specialization ||
                            user.Doctor.Address != model.Address || user.Doctor.Biography != model.Biography ||
                            user.Doctor.LicenseNumber != model.LicenseNumber)
                        {
                            user.Doctor.PhoneNumber = model.PhoneNumber;
                            user.Doctor.Specialization = model.Specialization;
                            user.Doctor.Address = model.Address;
                            user.Doctor.Biography = model.Biography;
                            user.Doctor.LicenseNumber = model.LicenseNumber;
                            user.Doctor.UpdatedAt = DateTime.Now;  // Cập nhật UpdatedAt
                            hasChanges = true;
                        }

                        // Kiểm tra và cập nhật ảnh đại diện nếu có
                        if (ImageUpLoad != null && ImageUpLoad.Length > 0)
                        {
                            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images/user/profiles");
                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }

                            var fileName = user.Id + Path.GetExtension(ImageUpLoad.FileName);
                            var fullPath = Path.Combine(filePath, fileName);

                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                await ImageUpLoad.CopyToAsync(stream);
                            }

                            user.Doctor.ProfilePicture = $"/images/user/profiles/{fileName}";
                            user.Doctor.UpdatedAt = DateTime.Now;  // Cập nhật UpdatedAt nếu ảnh thay đổi
                            hasChanges = true;
                        }
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu nếu có
                    if (hasChanges)
                    {
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Changes saved successfully.");

                        return RedirectToAction(nameof(Detail), new { id = user.Id });
                    }
                    else
                    {
                        Console.WriteLine("No changes detected.");
                    }
                }
            }
            else
            {
                Console.WriteLine("ModelState is not valid");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var doctors = _userService.GetAllDoctors()
                .OrderByDescending(doctor => doctor.Rating)
                .ToList();

            var profiles = new List<ProfileModel>();

            foreach (var doctor in doctors)
            {
                var user = doctor.User; // Assuming doctor.User is of type DoctorTalkWebAppUser

                var roles = await _userManager.GetRolesAsync(user);

                profiles.Add(new ProfileModel
                {
                    UserId = doctor.UserId,
                    UserName = user.UserName ?? "Unknown",
                    FullName = doctor.FullName,
                    Email = doctor.Email,
                    ProfileImageUrl = doctor.ProfilePicture,
                    UserRating = doctor.Rating.ToString(),
                    MemberSince = doctor.MemberSince,
                    Roles = roles
                });
            }

            var allRoles = _roleManager.Roles.ToList();

            var model = new ProfileListModel
            {
                Profiles = profiles,
                Roles = allRoles
            };

            return View(model);
        }

        public IActionResult Deactivate(string userId)
        {
            var user = _userService.GetById(userId);
            _userService.Deactivate(user);
            return RedirectToAction("Index", "Profile");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUserRole(string UserId, string SelectedRole)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                // Handle error if needed
            }

            var addResult = await _userManager.AddToRoleAsync(user, SelectedRole);
            if (!addResult.Succeeded)
            {
                // Handle error if needed
            }

            // Use TempData to pass success message
            TempData["SuccessMessage"] = "User role updated successfully.";

            return RedirectToAction("Index");
        }
    }
}
