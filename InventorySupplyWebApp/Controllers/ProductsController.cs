using System.Text;
using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using InventorySupplyWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InventorySupplyWebApp.Controllers;

[Authorize]
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
        var warehouses = _context.Warehouses.ToList();
        // Fetch the raw JSON string from the API
        var responseString = await _httpClient.GetStringAsync("http://localhost:5146/api/products");

        // Deserialize the response into a List<InventorySupply.DAL.Models.Product>
        var products = JsonConvert.DeserializeObject<List<Product>>(responseString);

        var warehouseIds = products
                .Where(p => p.WarehouseId != 0)
                .Select(p => p.WarehouseId)
                .Distinct()
                .ToList();

        var warehouseLookup = warehouses.ToDictionary(w => w.WarehouseId);

        foreach (var product in products)
        {
            if (warehouseLookup.TryGetValue((int)product.WarehouseId, out var wh))
            {
                product.Warehouse = wh;
            }
            else
            {
                product.Warehouse = null;
            }
        }

        if (products == null)
        {
            products = new List<Product>();            
        }

        return View(products);
    }

    public IActionResult Create()
    {
        var suppliers = _context.Suppliers.ToList();
        var warehouses = _context.Warehouses.ToList();
        if (suppliers == null || !suppliers.Any())
        {
            Console.WriteLine("No suppliers found in database.");
        }

        if (warehouses == null || !warehouses.Any())
        {
            Console.WriteLine("No warehouses found in database.");
        }

        //ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");
        //ViewBag.Warehouses = new SelectList(warehouses, "WarehouseId", "Name");
        var vm = new ProductCreateViewModel
       {
            Suppliers = suppliers.Select(s => new SelectListItem(s.Name, s.SupplierId.ToString())),
            Warehouses = warehouses.Select(w => new SelectListItem(w.Name, w.WarehouseId.ToString()))
       };
        
              return View(vm);
        //return View();
    }

    [HttpPost, Authorize(Roles = "Manager,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
    {
        ModelState.Remove("Suppliers");
        ModelState.Remove("Warehouses");
        if (ModelState.IsValid)
        {
            using var httpClient = new HttpClient();
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(viewModel),
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
        var suppliers = _context.Suppliers.ToList();
        var warehouses = _context.Warehouses.ToList();
        if (product == null) return NotFound();

        var viewModel = new ProductCreateViewModel
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            SupplierId = (int)product.SupplierId,
            WarehouseId = (int)product.WarehouseId
        };

        //ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", product.SupplierId);
        //ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", product.SupplierId);
        //return View(viewModel);

        var vm = new ProductCreateViewModel
        {
            Suppliers = suppliers.Select(s => new SelectListItem(s.Name, s.SupplierId.ToString())),
            Warehouses = warehouses.Select(w => new SelectListItem(w.Name, w.WarehouseId.ToString())),
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            SupplierId = (int)product.SupplierId,
            WarehouseId = (int)product.WarehouseId
        };

        return View(vm);
    }

    [HttpPost, Authorize(Roles = "Manager,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductCreateViewModel viewModel)
    {
        if (id != viewModel.ProductId)
            return NotFound();

        ModelState.Remove("Suppliers");
        ModelState.Remove("Warehouses");
        if (ModelState.IsValid)
        {
            // Map the form data to the API request body
            var productToUpdate = new InventorySupply.DAL.Models.Product
            {
                ProductId = viewModel.ProductId,
                Name = viewModel.Name,
                Description = viewModel.Description,
                Price = viewModel.Price,
                SupplierId = viewModel.SupplierId,
                Quantity = viewModel.Quantity,
                WarehouseId = viewModel.WarehouseId
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
        var warehouses = _context.Warehouses.ToList();
        // Fetch the product details from the external API
        var responseString = await _httpClient.GetStringAsync($"http://localhost:5146/api/products/{id}");

        // Deserialize the response into a Product object
        var product = JsonConvert.DeserializeObject<Product>(responseString);        

        var warehouseLookup = warehouses.ToDictionary(w => w.WarehouseId);

        // Check if product is null
        if (product == null)
        {
            return NotFound();
        }

        if (warehouseLookup.TryGetValue((int)product.WarehouseId, out var wh))
        {
            product.Warehouse = wh;
        }
        else
        {
            product.Warehouse = null;
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

    [HttpPost, ActionName("Delete"), Authorize(Roles = "Manager,Admin")]
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