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


public class SuppliersController : Controller
{
    private readonly InventorySupplyDbContext _context;
    private readonly HttpClient _httpClient;

    public SuppliersController(InventorySupplyDbContext context,HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }
    
    public async Task<IActionResult> Index()
    {
        // Fetch the raw JSON string from the API
        var responseString = await _httpClient.GetStringAsync("http://localhost:5146/api/suppliers");

        var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(responseString);

        // If the response is null, use an empty list
        if (suppliers == null)
        {
            suppliers = new List<Supplier>();
        }

        return View(suppliers);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        var suppliers = _context.Suppliers.ToList();
        ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierViewModel viewModel)
    {
        ModelState.Remove("Suppliers"); // Make sure we don't validate Products property
        if (ModelState.IsValid)
        {
            using var httpClient = new HttpClient();
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(viewModel), // full qualification
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync("http://localhost:5146/api/suppliers", jsonContent);

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
    
    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null)
            return NotFound();

        // Map the Supplier entity to the SupplierViewModel
        var viewModel = new SupplierViewModel
        {
            SupplierId = supplier.SupplierId,
            Name = supplier.Name,
            Contact = supplier.Contact,
            Address = supplier.Address
        };

        // Pass the view model to the view
        ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", supplier.SupplierId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SupplierViewModel viewModel)
    {
        if (id != viewModel.SupplierId)
            return NotFound();

        ModelState.Remove("Suppliers"); // Make sure we don't validate Products property

        if (ModelState.IsValid)
        {
            // Map the form data to the Supplier entity
            var supplierToUpdate = new Supplier
            {
                Name = viewModel.Name,
                Address = viewModel.Address,
                Contact = viewModel.Contact,
                SupplierId = viewModel.SupplierId
            };

            // Make the PUT request to the API to update the supplier
            var response = await _httpClient.PutAsJsonAsync($"http://localhost:5146/api/suppliers/{id}", supplierToUpdate);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index)); // Redirect back to the list
            }
            else
            {
                // Handle failure: Log the error or show a meaningful message
                ModelState.AddModelError("", "Failed to update the supplier.");
            }
        }

        // If ModelState is invalid, or the API call failed, repopulate the view with suppliers
        ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", viewModel.SupplierId);
        return View(viewModel);
    }

    
    public async Task<IActionResult> Details(int id)
    {
        // Fetch the supplier details from the external API
        var responseString = await _httpClient.GetStringAsync($"http://localhost:5146/api/suppliers/{id}");

        // Deserialize the response into a supplier object
        var supplier = JsonConvert.DeserializeObject<Supplier>(responseString);

        // Check if supplier is null
        if (supplier == null)
        {
            return NotFound();
        }

        // Return the supplier to the Details view
        return View(supplier);
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await _context.Suppliers
            .Include(p => p.Products)
            .FirstOrDefaultAsync(p => p.SupplierId == id);
        if (supplier == null)
            return NotFound();
        return View(supplier); // Confirmation view
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var supplier = await _context.Suppliers
            .Include(p => p.Products)
            .FirstOrDefaultAsync(p => p.SupplierId == id);

        if (supplier == null)
        {
            return NotFound();
        }

        // Call the API to delete the supplier
        var response = await _httpClient.DeleteAsync($"http://localhost:5146/api/suppliers/{id}");

        if (response.IsSuccessStatusCode)
        {
            // Successfully deleted the supplier, redirect to Index
            return RedirectToAction(nameof(Index));
        }

        // If the delete failed, show an error message and return to the delete view
        ModelState.AddModelError(string.Empty, "Failed to delete supplier. Please try again.");
        return View("Delete", supplier);
    }


}