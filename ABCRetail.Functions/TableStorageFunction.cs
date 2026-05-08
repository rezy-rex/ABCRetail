using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ABCRetail.Functions
{
    public class TableStorageFunction
    {
        private readonly ILogger<TableStorageFunction> _logger;

        public TableStorageFunction(ILogger<TableStorageFunction> logger)
        {
            _logger = logger;
        }

        [Function("StoreTableEntity")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("StoreTableEntity function triggered.");

            string connectionString = Environment.GetEnvironmentVariable("AzureStorage")!;
            var serviceClient = new TableServiceClient(connectionString);
            var tableClient = serviceClient.GetTableClient("FunctionOrders");
            await tableClient.CreateIfNotExistsAsync();

            var entity = new TableEntity("FunctionPartition", Guid.NewGuid().ToString())
            {
                { "ProductName", "Azure Function Item" },
                { "Quantity", 3 },
                { "OrderDate", DateTime.UtcNow.ToString("o") }
            };

            await tableClient.AddEntityAsync(entity);

            _logger.LogInformation("Entity stored in Azure Table Storage successfully.");
            return new OkObjectResult("Entity stored in Table Storage successfully.");
        }
    }
}