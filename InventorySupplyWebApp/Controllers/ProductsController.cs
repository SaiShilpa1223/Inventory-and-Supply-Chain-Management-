using System.Text;
using InventorySupply.DAL;
using InventorySupplyWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InventorySupplyWebApp.Controllers;

public class ProductsController : Controller
{
    private readonly InventorySupplyDbContext _context;
    private readonly HttpClient _httpClient;

    public ProductsController(InventorySupplyDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index()
    {
        // Fetch the raw JSON string from the API
        var responseString = await _httpClient.GetStringAsync("http://localhost:5146/api/products");

        // Deserialize the response into a List<InventorySupply.DAL.Models.Product>
        var products = JsonConvert.DeserializeObject<List<InventorySupply.DAL.Models.Product>>(responseString);

        // If the response is null, use an empty list
        if (products == null)
        {
            products = new List<InventorySupply.DAL.Models.Product>();
        }

        return View(products);
    }

    public IActionResult Create()
    {
        var suppliers = _context.Suppliers.ToList();
        if (suppliers == null || !suppliers.Any())
        {
            Console.WriteLine("No suppliers found in database.");
        }

        ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            using var httpClient = new HttpClient();
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(viewModel), // full qualification
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync("http://localhost:5146/api/products", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            // Optionally log response error
            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "API Error: " + errorResponse);
        }

        ViewBag.Suppliers = new SelectList(_context.Suppliers.ToList(), "SupplierId", "Name", viewModel.SupplierId);
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var viewModel = new ProductCreateViewModel
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            SupplierId = product.SupplierId
        };

        ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", product.SupplierId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductCreateViewModel viewModel)
    {
        if (id != viewModel.ProductId)
            return NotFound();

        if (ModelState.IsValid)
        {
            // Map the form data to the API request body
            var productToUpdate = new InventorySupply.DAL.Models.Product
            {
                ProductId = viewModel.ProductId,
                Name = viewModel.Name,
                Description = viewModel.Description,
                Price = viewModel.Price,
                SupplierId = viewModel.SupplierId
            };

            // Make the PUT request to the API to update the product
            var response =
                await _httpClient.PutAsJsonAsync($"http://localhost:5146/api/products/{id}", productToUpdate);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index)); // Redirect back to the product list
            }
            else
            {
                // Handle the failure
                ModelState.AddModelError("", "Failed to update the product.");
            }
        }

        ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", viewModel.SupplierId);
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        // Fetch the product details from the external API
        var responseString = await _httpClient.GetStringAsync($"http://localhost:5146/api/products/{id}");

        // Deserialize the response into a Product object
        var product = JsonConvert.DeserializeObject<InventorySupply.DAL.Models.Product>(responseString);

        // Check if product is null
        if (product == null)
        {
            return NotFound();
        }

        // Return the product to the Details view
        return View(product);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null)
            return NotFound();
        return View(product); // Confirmation view
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Delete via API
        var response = await _httpClient.DeleteAsync($"http://localhost:5146/api/products/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index)); // Redirect to Index if deletion is successful
        }

        // If deletion fails, return an error page or show an appropriate message
        return View("Error");
    }
}