using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class FileStorageController : Controller
    {
        private readonly FileStorageService _fileService;

        public FileStorageController(FileStorageService fileService)
        {
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Files = await _fileService.ListFilesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file.";
                return RedirectToAction(nameof(Index));
            }
            await _fileService.UploadFileAsync(file);
            TempData["Success"] = $"'{file.FileName}' uploaded.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CreateLog(string fileName, string content)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                TempData["Error"] = "File name is required.";
                return RedirectToAction(nameof(Index));
            }
            if (!fileName.EndsWith(".txt")) fileName += ".txt";
            var logContent = $"[{DateTime.UtcNow:u}] ABC Retail Log\n\n{content}";
            await _fileService.UploadLogTextAsync(fileName, logContent);
            TempData["Success"] = $"Log '{fileName}' created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string fileName)
        {
            await _fileService.DeleteFileAsync(fileName);
            TempData["Success"] = $"'{fileName}' deleted.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ViewFile(string fileName)
        {
            ViewBag.FileName = fileName;
            ViewBag.Content = await _fileService.DownloadFileContentAsync(fileName);
            return View();
        }
    }
}
