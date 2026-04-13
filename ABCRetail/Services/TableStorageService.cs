using Azure.Data.Tables;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class TableStorageService
    {
        private readonly TableClient _customersTable;
        private readonly TableClient _productsTable;

        public TableStorageService(IConfiguration config)
        {
            var connStr = config["AzureStorage:ConnectionString"]!;
            var customersTableName = config["AzureStorage:TableNameCustomers"]!;
            var productsTableName = config["AzureStorage:TableNameProducts"]!;

            _customersTable = new TableClient(connStr, customersTableName);
            _customersTable.CreateIfNotExists();

            _productsTable = new TableClient(connStr, productsTableName);
            _productsTable.CreateIfNotExists();
        }

        // --- Customers ---
        public async Task AddCustomerAsync(CustomerEntity customer)
        {
            customer.RowKey = Guid.NewGuid().ToString();
            await _customersTable.AddEntityAsync(customer);
        }

        public async Task<List<CustomerEntity>> GetAllCustomersAsync()
        {
            var results = new List<CustomerEntity>();
            await foreach (var entity in _customersTable.QueryAsync<CustomerEntity>())
                results.Add(entity);
            return results;
        }

        public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
        {
            await _customersTable.DeleteEntityAsync(partitionKey, rowKey);
        }

        // --- Products ---
        public async Task AddProductAsync(ProductEntity product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            await _productsTable.AddEntityAsync(product);
        }

        public async Task<List<ProductEntity>> GetAllProductsAsync()
        {
            var results = new List<ProductEntity>();
            await foreach (var entity in _productsTable.QueryAsync<ProductEntity>())
                results.Add(entity);
            return results;
        }

        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await _productsTable.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}