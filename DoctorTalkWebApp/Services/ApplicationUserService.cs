using DoctorTalkWebApp.Data;
using DoctorTalkWebApp.Data.Interfaces;

namespace DoctorTalkWebApp.Services
{
    public class ApplicationUserService : IApplicationUser
    {
        private readonly DoctorTalkWebAppContext _context;

        public ApplicationUserService(DoctorTalkWebAppContext context)
        {
            _context = context;
        }

        public Task Add(DoctorTalkWebAppUser user)
        {
            throw new NotImplementedException();
        }

        public Task BumpRating(string userId, Type type)
        {
            throw new NotImplementedException();
        }

        public Task Deactivate(DoctorTalkWebAppUser user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DoctorTalkWebAppUser> GetAll()
        {
            return _context.ApplicationUsers;
        }

        public DoctorTalkWebAppUser GetById(string id)
        {
            return GetAll().FirstOrDefault(user => user.Id == id);
        }

        public Task IncrementRating(string id)
        {
            throw new NotImplementedException();
        }

        public async Task SetProfileImage(string id, Uri uri)
        {
            var user = GetById(id);
            user.ProfileImageUrl = uri.AbsoluteUri;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
