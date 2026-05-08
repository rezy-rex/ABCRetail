using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ABCRetail.Functions
{
    public class FileStorageFunction
    {
        private readonly ILogger<FileStorageFunction> _logger;

        public FileStorageFunction(ILogger<FileStorageFunction> logger)
        {
            _logger = logger;
        }

        [Function("UploadToFileShare")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("UploadToFileShare function triggered.");

            string connectionString = Environment.GetEnvironmentVariable("AzureStorage")!;
            var shareClient = new ShareClient(connectionString, "function-files");
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetRootDirectoryClient();
            string fileName = $"report-{Guid.NewGuid()}.txt";
            var fileClient = directoryClient.GetFileClient(fileName);

            string content = $"File report generated via Azure Function at {DateTime.UtcNow:o}";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            await fileClient.CreateAsync(bytes.Length);
            using var stream = new MemoryStream(bytes);
            await fileClient.UploadRangeAsync(
                new Azure.HttpRange(0, bytes.Length),
                stream
            );

            _logger.LogInformation("File uploaded to Azure File Share: {FileName}", fileName);
            return new OkObjectResult($"File '{fileName}' uploaded to Azure File Share successfully.");
        }
    }
}
