using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class QueueStorageController : Controller
    {
        private readonly QueueStorageService _queueService;

        public QueueStorageController(QueueStorageService queueService)
        {
            _queueService = queueService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Messages = await _queueService.PeekMessagesAsync(32);
            ViewBag.MessageCount = await _queueService.GetMessageCountAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Message cannot be empty.";
                return RedirectToAction(nameof(Index));
            }
            await _queueService.SendMessageAsync(message);
            TempData["Success"] = $"Message enqueued: \"{message}\"";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SendOrderMessage(string orderId, string productName, string action)
        {
            var msg = $"{action} | Order #{orderId} | Product: {productName} | Time: {DateTime.UtcNow:u}";
            await _queueService.SendMessageAsync(msg);
            TempData["Success"] = $"Order message enqueued for Order #{orderId}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ProcessNext()
        {
            var msg = await _queueService.DequeueMessageAsync();
            if (msg != null)
            {
                var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(msg.Body.ToString()));
                await _queueService.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
                TempData["Success"] = $"Processed & removed: \"{decoded}\"";
            }
            else
            {
                TempData["Error"] = "Queue is empty.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}