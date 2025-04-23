using InventorySupply.DAL;
using InventorySupplyWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySupplyWebApp.Controllers;

public class InventoryController : Controller
{
    private readonly InventorySupplyDbContext _context;

    public InventoryController(InventorySupplyDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var inventoryItems = await _context.InventoryItems
            .Include(i => i.Product) // Includes Product navigation property
            .ToListAsync();

        return View(inventoryItems);
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
        var inventory = await _context.InventoryItems.FindAsync(id);
        if (inventory != null)
        {
            _context.InventoryItems.Remove(inventory);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
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

    if (ModelState.IsValid)
    {
        var inventoryItem = await _context.InventoryItems.FindAsync(id);
        if (inventoryItem == null)
            return NotFound();

        var product = await _context.Products.FindAsync(viewModel.ProductId);
        if (product == null)
        {
            ModelState.AddModelError("ProductId", "Invalid Product.");
            return View(viewModel);
        }

        inventoryItem.ProductId = viewModel.ProductId;
        inventoryItem.QuantityInStock = viewModel.QuantityInStock;
        inventoryItem.ReorderLevel = viewModel.ReorderLevel;
        inventoryItem.LastModified = DateTime.UtcNow; // Update the last modified timestamp

        _context.Update(inventoryItem);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index)); // Redirect to the inventory index page
    }

    viewModel.Products = _context.Products.Select(p => new SelectListItem
    {
        Value = p.ProductId.ToString(),
        Text = p.Name
    }).ToList();

    return View(viewModel);
}

    public async Task<IActionResult> Details(int id)
    {
        var inventory = await _context.InventoryItems
            .Include(p => p.Product)
            .FirstOrDefaultAsync(p => p.InventoryItemId == id);
        if (inventory == null) return NotFound();
        return View(inventory);
    }
    
    public IActionResult Create()
    {
        var viewModel = new InventoryItemViewModel
        {
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
    public async Task<IActionResult> Create(InventoryItemViewModel viewModel)
    {
        ModelState.Remove("Products"); // Make sure we don't validate Products property
        ModelState.Remove("Supplier"); // Make sure we don't validate Products property

        // Debug: check ModelState errors
        foreach (var entry in ModelState)
        {
            if (entry.Value.Errors.Any())
            {
                Console.WriteLine($"Error in {entry.Key}: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
            }
        }


        if (ModelState.IsValid)
        {
            var inventoryItem = new  InventorySupply.DAL.Models.InventoryItem
            {
                ProductId = viewModel.ProductId,
                QuantityInStock = viewModel.QuantityInStock,
                ReorderLevel = viewModel.ReorderLevel,
                DateAdded = DateTime.Now,  // Or let SQL handle it with default
                LastModified = DateTime.Now  // Or set manually
            };

            _context.InventoryItems.Add(inventoryItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Repopulate Products list for redisplay if invalid
        viewModel.Products = _context.Products
            .Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            })
            .ToList();

        return View(viewModel);
    }
}