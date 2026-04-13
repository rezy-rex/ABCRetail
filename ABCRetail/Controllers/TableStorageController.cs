using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class TableStorageController : Controller
    {
        private readonly TableStorageService _tableService;

        public TableStorageController(TableStorageService tableService)
        {
            _tableService = tableService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Customers = await _tableService.GetAllCustomersAsync();
            ViewBag.Products = await _tableService.GetAllProductsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerEntity customer)
        {
            await _tableService.AddCustomerAsync(customer);
            TempData["Success"] = $"Customer '{customer.FirstName} {customer.LastName}' added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(string partitionKey, string rowKey)
        {
            await _tableService.DeleteCustomerAsync(partitionKey, rowKey);
            TempData["Success"] = "Customer deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductEntity product)
        {
            await _tableService.AddProductAsync(product);
            TempData["Success"] = $"Product '{product.ProductName}' added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string partitionKey, string rowKey)
        {
            await _tableService.DeleteProductAsync(partitionKey, rowKey);
            TempData["Success"] = "Product deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}