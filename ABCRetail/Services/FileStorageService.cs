using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace ABCRetail.Services
{
    public class FileStorageService
    {
        private readonly ShareDirectoryClient _directoryClient;

        public FileStorageService(IConfiguration config)
        {
            var connStr = config["AzureStorage:ConnectionString"]!;
            var shareName = config["AzureStorage:FileShareName"]!;
            var shareClient = new ShareClient(connStr, shareName);
            shareClient.CreateIfNotExists();
            _directoryClient = shareClient.GetRootDirectoryClient();
        }

        public async Task UploadFileAsync(IFormFile file)
        {
            var fileClient = _directoryClient.GetFileClient(file.FileName);
            using var stream = file.OpenReadStream();
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadAsync(stream);
        }

        public async Task UploadLogTextAsync(string fileName, string content)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            var fileClient = _directoryClient.GetFileClient(fileName);
            using var stream = new MemoryStream(bytes);
            await fileClient.CreateAsync(bytes.Length);
            await fileClient.UploadAsync(stream);
        }

        public async Task<List<ShareFileItem>> ListFilesAsync()
        {
            var files = new List<ShareFileItem>();
            await foreach (var item in _directoryClient.GetFilesAndDirectoriesAsync())
                if (!item.IsDirectory) files.Add(item);
            return files;
        }

        public async Task<string> DownloadFileContentAsync(string fileName)
        {
            var fileClient = _directoryClient.GetFileClient(fileName);
            var download = await fileClient.DownloadAsync();
            using var reader = new StreamReader(download.Value.Content);
            return await reader.ReadToEndAsync();
        }

        public async Task DeleteFileAsync(string fileName)
            => await _directoryClient.GetFileClient(fileName).DeleteIfExistsAsync();
    }
}