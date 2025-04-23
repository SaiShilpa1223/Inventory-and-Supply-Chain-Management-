using InventorySupply.DAL;
using InventorySupplyWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySupplyWebApp.Controllers;
public class ProductsController : Controller
{
    private readonly InventorySupplyDbContext _context;
    
    public ProductsController(InventorySupplyDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        // This includes the Supplier data automatically using a JOIN
        var products = await _context.Products
            .Include(p => p.Supplier)
            .ToListAsync();

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
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierId == viewModel.SupplierId);

            if (supplier == null)
            {
                ModelState.AddModelError("SupplierId", "Invalid supplier.");
            }
            else
            {
                var product = new InventorySupply.DAL.Models.Product
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    SupplierId = viewModel.SupplierId,
                    //Supplier = supplier
                };
                
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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

        return View(viewModel); // Edit.cshtml
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductCreateViewModel viewModel)
    {
        if (id != viewModel.ProductId)
            return NotFound();

        if (ModelState.IsValid)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = viewModel.Name;
            product.Description = viewModel.Description;
            product.Price = viewModel.Price;
            product.SupplierId = viewModel.SupplierId;

            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "Name", viewModel.SupplierId);
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null) return NotFound();
        return View(product);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null) return NotFound();
        return View(product); // confirmation view
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    
    
}