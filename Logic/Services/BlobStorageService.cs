using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Logic.DbContextFactory;
using TeaWork.Logic.Services.Interfaces;
using TeaWork.Data.Models;
using TeaWork.Data.Enums;
using TeaWork.Data;
using Microsoft.EntityFrameworkCore;
using TeaWork.Components.Notifications;
using Microsoft.CodeAnalysis;

namespace TeaWork.Logic.Services
{
    public class BlobStorageService :IBlobStorageService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        private string _blobStorageConnection;
        private string _blobContainerName = "teawork";

        public BlobStorageService(IConfiguration configuration ,IDbContextFactory dbContextFactory, AuthenticationStateProvider authenticationStateProvider, UserIdentity userIdentity)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = userIdentity;
            _blobStorageConnection = configuration.GetConnectionString("AzureStorageAcount");
        }

        public async Task<ProjectFile> AddFile(string fileName, string fileType,int fileSize,int projectId)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();               
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                ProjectFile projectFile = new ProjectFile
                {
                    FileName = fileName,
                    ContentType = fileType,
                    FileSize = fileSize,
                    ProjectId = projectId,
                    UserId = currentUser.Id,
                    UploadDate = DateTime.Now,
                    IsDeleted = false,
                };

                _context.ProjectFiles.Add(projectFile);
                await _context.SaveChangesAsync();
                return projectFile;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<List<ProjectFile>> GetFilesById(int projectId)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var files = await _context.ProjectFiles
                    .Where(x => x.ProjectId == projectId)
                    .Where(x=>x.IsDeleted==false)
                    .Include(x=>x.User)
                    .ToListAsync();
                return files;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<bool> UploadFileToBlobAsync(ProjectFile projectFile, Stream fileStream)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var container = new BlobContainerClient(_blobStorageConnection, _blobContainerName);
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                var blob = container.GetBlobClient(projectFile.Id.ToString());
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = projectFile.ContentType });
                var urlString = blob.Uri.ToString();
                projectFile.FileStorageUrl = urlString;
                _context.Attach(projectFile).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                //_logger?.LogError(ex.ToString());
                throw new NotImplementedException();
            }
        }
        public async Task<bool> DeleteFileToBlobAsync(string strFileName)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var file = await _context.ProjectFiles.FirstOrDefaultAsync(x => x.Id ==Convert.ToInt32(strFileName));
                var container = new BlobContainerClient(_blobStorageConnection, _blobContainerName);
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                var blob = container.GetBlobClient(strFileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                if (file != null)
                {
                    file.IsDeleted = true;
                    _context.Attach(file).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                //_logger?.LogError(ex.ToString());
                throw new NotImplementedException();
            }
        }
        public async Task<(byte[] FileContent, string ContentType)> DownloadFileFromBlobAsync(string fileName)
        {
            try
            {
                var container = new BlobContainerClient(_blobStorageConnection, _blobContainerName);
                var blob = container.GetBlobClient(fileName);

                if (await blob.ExistsAsync())
                {
                    var memoryStream = new MemoryStream();
                    var properties = await blob.GetPropertiesAsync();
                    var contentType = properties.Value.ContentType;

                    await blob.DownloadToAsync(memoryStream);
                    return (memoryStream.ToArray(), contentType ?? "application/octet-stream");
                }
                else
                {
                    throw new FileNotFoundException($"File '{fileName}' not found in blob storage.");
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

    }
}
