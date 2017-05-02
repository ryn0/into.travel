using IntoTravel.Data.Models.AzureStorage.Blob;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace IntoTravel.Data.Repositories.Interfaces
{
    public interface ISiteFilesRepository
    {
        SiteFileDirectory ListFiles(string prefix = null);

        Task DeleteFileAsync(string blobPath);

        Task UploadAsync(IFormFile file, string directory = null);

        Task CreateFolderAsync(string folderPath, string directory = null);

        Task DeleteFolderAsync(string folderPath);
    }
}
