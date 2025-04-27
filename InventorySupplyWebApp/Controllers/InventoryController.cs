using System.Text;
using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using InventorySupplyWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InventorySupplyWebApp.Controllers;

public class InventoryController : Controller
{
    private readonly InventorySupplyDbContext _context;
    private readonly HttpClient _httpClient;

    public InventoryController(InventorySupplyDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index()
    {
        // Fetch the raw JSON string from the API
        var responseString = await _httpClient.GetStringAsync("http://localhost:5146/api/inventory");

        var inventories = JsonConvert.DeserializeObject<List<InventoryItem>>(responseString);

        // If the response is null, use an empty list
        if (inventories == null)
        {
            inventories = new List<InventoryItem>();
        }

        return View(inventories);
    }

    public IActionResult Create()
    {
        var productList = _context.Products
            .Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            })
            .ToList();

        var viewModel = new InventoryItemViewModel
        {
            Products = productList
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InventoryItemViewModel viewModel)
    {
        ModelState.Remove("Products"); // Make sure we don't validate Products property
        ModelState.Remove("Supplier"); // Make sure we don't validate Products property

        // Debug: check ModelState errors
        foreach (var entry in ModelState)
        {
            if (entry.Value.Errors.Any())
            {
                Console.WriteLine(
                    $"Error in {entry.Key}: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
            }
        }

        if (ModelState.IsValid)
        {
            using var httpClient = new HttpClient();
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(viewModel), // full qualification
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync("http://localhost:5146/api/inventory", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            // Optionally log response error
            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "API Error: " + errorResponse);
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var inventoryItem = await _context.InventoryItems
            .Include(i => i.Product) // Include related product info
            .FirstOrDefaultAsync(i => i.InventoryItemId == id);
        if (inventoryItem == null) return NotFound();
        return View(inventoryItem); // confirmation view
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Delete via API
        var response = await _httpClient.DeleteAsync($"http://localhost:5146/api/inventory/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index)); // Redirect to Index if deletion is successful
        }

        // If deletion fails, return an error page or show an appropriate message
        return View("Error");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var inventoryItem = await _context.InventoryItems
            .Include(i => i.Product) // Include related product info
            .FirstOrDefaultAsync(i => i.InventoryItemId == id);

        if (inventoryItem == null)
            return NotFound();

        var viewModel = new InventoryItemViewModel
        {
            InventoryItemId = inventoryItem.InventoryItemId,
            ProductId = inventoryItem.ProductId,
            QuantityInStock = inventoryItem.QuantityInStock,
            ReorderLevel = inventoryItem.ReorderLevel,
            DateAdded = inventoryItem.DateAdded,
            Products = _context.Products.Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InventoryItemViewModel viewModel)
    {
        if (id != viewModel.InventoryItemId)
            return NotFound();

        ModelState.Remove("Products"); // Make sure we don't validate Products property
        ModelState.Remove("Supplier"); // Make sure we don't validate Products property

        // Debug: check ModelState errors
        foreach (var entry in ModelState)
        {
            if (entry.Value.Errors.Any())
            {
                Console.WriteLine(
                    $"Error in {entry.Key}: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
            }
        }

        if (ModelState.IsValid)
        {
            // Map the form data to the API request body
            var inventoryToUpdate = new InventoryItem()
            {
                InventoryItemId = viewModel.InventoryItemId,
                ProductId = viewModel.ProductId,
                ReorderLevel = viewModel.ReorderLevel,
                QuantityInStock = viewModel.QuantityInStock,
                LastModified = DateTime.UtcNow
            };

            // Make the PUT request to the API to update the inventory
            var response =
                await _httpClient.PutAsJsonAsync($"http://localhost:5146/api/inventory/{id}", inventoryToUpdate);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index)); // Redirect back to the inventory list
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
        // Fetch the inventory details from the external API
        var responseString = await _httpClient.GetStringAsync($"http://localhost:5146/api/inventory/{id}");

        // Deserialize the response into a inventory object
        var inventory = JsonConvert.DeserializeObject<InventoryItem>(responseString);

        // Check if inventory is null
        if (inventory == null)
        {
            return NotFound();
        }

        // Return the inventory to the Details view
        return View(inventory);
    }
}