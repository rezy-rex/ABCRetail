using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class BlobStorageController : Controller
    {
        private readonly BlobStorageService _blobService;

        public BlobStorageController(BlobStorageService blobService)
        {
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Blobs = await _blobService.GetAllBlobsAsync();
            ViewBag.GetBlobUrl = (Func<string, string>)_blobService.GetBlobUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction(nameof(Index));
            }
            await _blobService.UploadBlobAsync(file);
            TempData["Success"] = $"'{file.FileName}' uploaded successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string blobName)
        {
            await _blobService.DeleteBlobAsync(blobName);
            TempData["Success"] = "File deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}