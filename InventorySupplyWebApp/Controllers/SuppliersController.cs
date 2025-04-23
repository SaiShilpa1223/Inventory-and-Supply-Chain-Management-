using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using InventorySupplyWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySupplyWebApp.Controllers;

public class SuppliersController : Controller
{
    private readonly InventorySupplyDbContext _context;

    public SuppliersController(InventorySupplyDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var suppliers = await _context.Suppliers.ToListAsync();
        return View(suppliers);
    }

    // public IActionResult Details(int id)
    // {
    //     // var supplier = StaticData.Suppliers.FirstOrDefault(s => s.SupplierId == id);
    //     // if (supplier == null) return NotFound();
    //     // return View(supplier);
    // }
    
    [HttpGet]
    public IActionResult Create()
    {
        var suppliers = _context.Suppliers.ToList();
        ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");
        return View();
    }
    
    // POST: Suppliers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierViewModel viewModel)
    {
        ModelState.Remove("Suppliers"); // Make sure we don't validate Products property

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
            var supplier = new Supplier
            {
                Name = viewModel.Name,
                Contact = viewModel.Contact,
                Address = viewModel.Address
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }
}