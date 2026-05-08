using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ABCRetail.Functions
{
    public class BlobStorageFunction
    {
        private readonly ILogger<BlobStorageFunction> _logger;

        public BlobStorageFunction(ILogger<BlobStorageFunction> logger)
        {
            _logger = logger;
        }

        [Function("UploadBlob")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("UploadBlob function triggered.");

            string connectionString = Environment.GetEnvironmentVariable("AzureStorage")!;
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("function-uploads");
            await containerClient.CreateIfNotExistsAsync();

            string blobName = $"order-{Guid.NewGuid()}.txt";
            string content = $"Order created via Azure Function at {DateTime.UtcNow:o}";

            var blobClient = containerClient.GetBlobClient(blobName);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await blobClient.UploadAsync(stream, overwrite: true);

            _logger.LogInformation("Blob uploaded successfully: {BlobName}", blobName);
            return new OkObjectResult($"Blob '{blobName}' uploaded to Blob Storage successfully.");
        }
    }
}