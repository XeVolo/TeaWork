using TeaWork.Data.Models;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task<ProjectFile> AddFile(string fileName, string fileType, int fileSize, int projectId);
        Task<List<ProjectFile>> GetFilesById(int projectId);
        Task<bool> CheckFilling(int projectId);
        Task<bool> UploadFileToBlob(ProjectFile projectFile, Stream fileStream);
        Task<bool> DeleteFileToBlob(string strFileName);
        Task<(byte[] FileContent, string ContentType)> DownloadFileFromBlob(string fileName);

    }
}
