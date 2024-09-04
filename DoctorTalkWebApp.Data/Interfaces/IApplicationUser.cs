using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorTalkWebApp.Data.Interfaces
{
    public interface IApplicationUser
    {
        DoctorTalkWebAppUser GetById(string id);
        IEnumerable<DoctorTalkWebAppUser> GetAll();

        Task IncrementRating(string id);
        Task Add(DoctorTalkWebAppUser user);
        Task Deactivate(DoctorTalkWebAppUser user);
        Task SetProfileImage(string id, Uri uri);
        Task BumpRating(string userId, Type type);
    }
}
