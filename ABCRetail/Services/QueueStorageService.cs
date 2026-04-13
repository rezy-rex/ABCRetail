using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace ABCRetail.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queueClient;

        public QueueStorageService(IConfiguration config)
        {
            var connStr = config["AzureStorage:ConnectionString"]!;
            var queueName = config["AzureStorage:QueueName"]!;
            _queueClient = new QueueClient(connStr, queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessageAsync(string message)
        {
            var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message));
            await _queueClient.SendMessageAsync(encoded);
        }

        public async Task<List<PeekedMessage>> PeekMessagesAsync(int count = 32)
        {
            var response = await _queueClient.PeekMessagesAsync(maxMessages: count);
            return response.Value.ToList();
        }

        public async Task<QueueMessage?> DequeueMessageAsync()
        {
            var response = await _queueClient.ReceiveMessageAsync();
            return response?.Value;
        }

        public async Task DeleteMessageAsync(string messageId, string popReceipt)
            => await _queueClient.DeleteMessageAsync(messageId, popReceipt);

        public async Task<int> GetMessageCountAsync()
        {
            var props = await _queueClient.GetPropertiesAsync();
            return props.Value.ApproximateMessagesCount;
        }
    }
}