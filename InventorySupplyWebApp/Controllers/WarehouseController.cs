using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using InventorySupplyWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

//[Authorize]
public class WarehouseController : Controller
{
    private readonly InventorySupplyDbContext _context;
    private readonly HttpClient _httpClient;

    public WarehouseController(InventorySupplyDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index()
    {
        // Fetch the raw JSON string from the API
        var responseString = await _httpClient.GetStringAsync("http://localhost:5146/api/warehouse");

        var Warehouse = JsonConvert.DeserializeObject<List<Warehouse>>(responseString);

        // If the response is null, use an empty list
        if (Warehouse == null)
        {
            Warehouse = new List<Warehouse>();
        }
        return View(Warehouse);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var Warehouse = _context.Warehouses.ToList();
        ViewBag.Warehouse = new SelectList(Warehouse, "WarehousesId", "Name");
        return View();
    }

    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Create(WarehouseViewModel viewModel)
    // {
    //     ModelState.Remove("Warehouses"); // Make sure we don't validate Products property
    //     if (ModelState.IsValid)
    //     {
    //         using var httpClient = new HttpClient();
    //         var jsonContent = new StringContent(
    //             System.Text.Json.JsonSerializer.Serialize(viewModel), // full qualification
    //             Encoding.UTF8,
    //             "application/json"
    //         );
    //
    //         var response = await httpClient.PostAsync("http://localhost:5146/api/warehouse", jsonContent);
    //
    //         if (response.IsSuccessStatusCode)
    //         {
    //             var responseData = await response.Content.ReadAsStringAsync();
    //             var createdWarehouse = JsonConvert.DeserializeObject<Warehouse>(responseData);
    //
    //             // Now you can access createdWarehouse.WarehouseId
    //             return RedirectToAction(nameof(Index));
    //         }
    //
    //         // Optionally log response error
    //         var errorResponse = await response.Content.ReadAsStringAsync();
    //         ModelState.AddModelError(string.Empty, "API Error: " + errorResponse);
    //     }
    //
    //     return View(viewModel);
    // }

    public async Task<IActionResult> Create(WarehouseViewModel viewModel)
    {
        ModelState.Remove("Suppliers");
        ModelState.Remove("Warehouses");

        if (ModelState.IsValid)
        {
            // Map WarehouseViewModel to Warehouse model
            var model = new Warehouse
            {
                Name = viewModel.WarehouseName, // Use WarehouseName from the ViewModel
                Location = viewModel.Location
            };

            using var httpClient = new HttpClient();
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(model), // Send the Warehouse model
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync("http://localhost:5146/api/warehouse", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "API Error: " + errorResponse);
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null)
            return NotFound();

        // Map the Supplier entity to the SupplierViewModel
        var viewModel = new WarehouseViewModel
        {
            WarehouseId = warehouse.WarehouseId,
            WarehouseName = warehouse.Name,
            Location = warehouse.Location,
        };

        // Pass the view model to the view
        ViewBag.Warehouses = new SelectList(_context.Suppliers, "WarehouseId", "Name", warehouse.WarehouseId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WarehouseViewModel viewModel)
    {
        if (id != viewModel.WarehouseId)
            return NotFound();

        ModelState.Remove("Warehouses"); // Make sure we don't validate Products property

        if (ModelState.IsValid)
        {
            // Map the form data to the Warehouse entity
            var warehouseToUpdate = new Warehouse
            {
                Name = viewModel.WarehouseName,
                Location = viewModel.Location,
                WarehouseId = viewModel.WarehouseId
            };

            // Make the PUT request to the API to update the warehouse
            var response = await _httpClient.PutAsJsonAsync($"http://localhost:5146/api/warehouse/{id}", warehouseToUpdate);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index)); // Redirect back to the list
            }
            else
            {
                // Handle failure: Log the error or show a meaningful message
                ModelState.AddModelError("", "Failed to update the warehouse.");
            }
        }

        // If ModelState is invalid, or the API call failed, repopulate the view with suppliers
        ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", viewModel.WarehouseId);
        return View(viewModel);
    }


    public async Task<IActionResult> Details(int id)
    {
        // Fetch the supplier details from the external API
        var responseString = await _httpClient.GetStringAsync($"http://localhost:5146/api/warehouse/{id}");

        // Deserialize the response into a supplier object
        var warehouse = JsonConvert.DeserializeObject<Warehouse>(responseString);

        // Check if supplier is null
        if (warehouse == null)
        {
            return NotFound();
        }

        // Return the warehouse to the Details view
        return View(warehouse);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var warehouse = await _context.Warehouses
            .Include(p => p.Products)
            .FirstOrDefaultAsync(p => p.WarehouseId == id);
        if (warehouse == null) // Fix: Changed 'Warehouse' to 'warehouse' to refer to the variable
            return NotFound();
        return View(warehouse); // Fix: Changed 'Warehouse' to 'warehouse' to refer to the variable
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var warehouse = await _context.Warehouses
            .Include(p => p.Products)
            .FirstOrDefaultAsync(p => p.WarehouseId == id);

        if (warehouse == null)
        {
            return NotFound();
        }

        // Call the API to delete the supplier
        var response = await _httpClient.DeleteAsync($"http://localhost:5146/api/warehouses/{id}");

        if (response.IsSuccessStatusCode)
        {
            // Successfully deleted the supplier, redirect to Index
            return RedirectToAction(nameof(Index));
        }

        // If the delete failed, show an error message and return to the delete view
        ModelState.AddModelError(string.Empty, "Failed to delete supplier. Please try again.");
        return View("Delete", warehouse);
    }
}
