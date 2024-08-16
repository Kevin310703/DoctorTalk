using DoctorTalkWebApp.Models;

namespace DoctorTalkWebApp.Interfaces
{
    public interface ILocationService
    {
        Task<List<City>> GetLocationSearch(string location);
        Task<City> GetCityByZipCode(int zipCode);
    }
}
