using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ABCRetail.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IConfiguration config)
        {
            var connStr = config["AzureStorage:ConnectionString"]!;
            var containerName = config["AzureStorage:BlobContainerName"]!;
            _containerClient = new BlobContainerClient(connStr, containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.BlobContainer);
        }

        public async Task<string> UploadBlobAsync(IFormFile file)
        {
            var blobName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobClient = _containerClient.GetBlobClient(blobName);
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            return blobClient.Uri.ToString();
        }

        public async Task<List<BlobItem>> GetAllBlobsAsync()
        {
            var blobs = new List<BlobItem>();
            await foreach (var blob in _containerClient.GetBlobsAsync())
                blobs.Add(blob);
            return blobs;
        }

        public string GetBlobUrl(string blobName)
            => _containerClient.GetBlobClient(blobName).Uri.ToString();

        public async Task DeleteBlobAsync(string blobName)
            => await _containerClient.DeleteBlobIfExistsAsync(blobName);
    }
}