using Azure.Storage.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ABCRetail.Functions
{
    public class QueueStorageFunction
    {
        private readonly ILogger<QueueStorageFunction> _logger;

        public QueueStorageFunction(ILogger<QueueStorageFunction> logger)
        {
            _logger = logger;
        }

        [Function("WriteToQueue")]
        public async Task<IActionResult> WriteMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("WriteToQueue function triggered.");

            string connectionString = Environment.GetEnvironmentVariable("AzureStorage")!;
            var queueClient = new QueueClient(connectionString, "function-orders");
            await queueClient.CreateIfNotExistsAsync();

            string message = $"Order queued via Azure Function at {DateTime.UtcNow:o}";
            await queueClient.SendMessageAsync(
                Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message))
            );

            _logger.LogInformation("Message written to queue successfully.");
            return new OkObjectResult("Message written to Queue Storage successfully.");
        }

        [Function("ReadFromQueue")]
        public async Task<IActionResult> ReadMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("ReadFromQueue function triggered.");

            string connectionString = Environment.GetEnvironmentVariable("AzureStorage")!;
            var queueClient = new QueueClient(connectionString, "function-orders");
            await queueClient.CreateIfNotExistsAsync();

            var response = await queueClient.ReceiveMessageAsync();
            if (response.Value == null)
                return new OkObjectResult("Queue is empty.");

            string decoded = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(response.Value.MessageText)
            );
            await queueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);

            _logger.LogInformation("Message read from queue: {Message}", decoded);
            return new OkObjectResult($"Message received: {decoded}");
        }
    }
}