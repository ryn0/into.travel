using IntoTravel.Data.Models.AzureStorage.Blob;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntoTravel.Data.Repositories.Interfaces
{
    public interface ISiteFilesRepository
    {
        SiteFileDirectory ListFiles(string prefix = null);

        void DeleteFile(string blobPath);

        Task UploadAsync(IFormFile file, string directory = null);

        void CreateFolder(string folderPath, string directory = null);

        void DeleteFolder(string folderPath);
    }
}
