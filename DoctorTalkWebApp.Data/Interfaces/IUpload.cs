using Microsoft.WindowsAzure.Storage.Blob;

namespace DoctorTalkWebApp.Data.Interfaces
{
    public interface IUpload
    {
        CloudBlobContainer GetBlobContainer(string connectionString);
    }
}
