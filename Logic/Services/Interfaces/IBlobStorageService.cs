namespace TeaWork.Logic.Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileToBlobAsync(string strFileName, string contecntType, Stream fileStream);
        Task<bool> DeleteFileToBlobAsync(string strFileName);
        Task<(Stream FileStream, string ContentType)> DownloadFileFromBlobAsync(string fileName);
    }
}
