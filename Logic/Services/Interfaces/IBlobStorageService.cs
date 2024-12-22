using TeaWork.Data.Models;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task<ProjectFile> AddFile(string fileName, string fileType, int fileSize, int projectId);
        Task<List<ProjectFile>> GetFilesById(int projectId);
        Task<bool> CheckFilling(int projectId);
        Task<bool> UploadFileToBlobAsync(ProjectFile projectFile, Stream fileStream);
        Task<bool> DeleteFileToBlobAsync(string strFileName);
        Task<(byte[] FileContent, string ContentType)> DownloadFileFromBlobAsync(string fileName);

    }
}
