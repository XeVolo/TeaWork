using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Logic.DbContextFactory;
using TeaWork.Logic.Services.Interfaces;

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
        public async Task<string> UploadFileToBlobAsync(string strFileName, string contecntType, Stream fileStream)
        {
            try
            {
                var container = new BlobContainerClient(_blobStorageConnection, _blobContainerName);
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                var blob = container.GetBlobClient(strFileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contecntType });
                var urlString = blob.Uri.ToString();
                return urlString;
            }
            catch (Exception ex)
            {
                //_logger?.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<bool> DeleteFileToBlobAsync(string strFileName)
        {
            try
            {
                var container = new BlobContainerClient(_blobStorageConnection, _blobContainerName);
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                var blob = container.GetBlobClient(strFileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                return true;
            }
            catch (Exception ex)
            {
                //_logger?.LogError(ex.ToString());
                throw;
            }
        }
    }
}
