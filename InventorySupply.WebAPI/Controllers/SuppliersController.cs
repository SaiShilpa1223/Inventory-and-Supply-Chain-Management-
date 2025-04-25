using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySupply.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SuppliersController : Controller
{
    private readonly InventorySupplyDbContext _context;
    private readonly ILogger<SuppliersController> _logger;

    public SuppliersController(InventorySupplyDbContext context, ILogger<SuppliersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/suppliers
    [HttpGet]
    public async Task<IActionResult> GetSuppliers()
    {
        var suppliers = await _context.Suppliers
            .Select(s => new Supplier()
            {
                SupplierId = s.SupplierId,
                Name = s.Name,
                Contact = s.Contact,
                Address = s.Address
            })
            .ToListAsync();

        return Ok(suppliers);
    }

    // GET: api/suppliers/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplier(int id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Products)
            .Where(s => s.SupplierId == id)
            .Select(s => new Supplier()
            {
                SupplierId = s.SupplierId,
                Name = s.Name,
                Contact = s.Contact,
                Address = s.Address,
                // Products = s.Products.Select(p => new Product()
                // {
                //     ProductId = p.ProductId,
                //     Name = p.Name,
                //     Description = p.Description,
                //     Price = p.Price
                // }).ToList()
            })
            .FirstOrDefaultAsync();

        if (supplier == null)
            return NotFound(new { Message = "Supplier not found" });

        return Ok(supplier);
    }

    // POST: api/suppliers
    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] Supplier supplier)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        // Return 200 OK instead of 201 Created
        return Ok(supplier);
    }
    
    // PUT: api/suppliers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSupplier(int id, [FromBody] Supplier supplier)
    {
        if (id != supplier.SupplierId)
        {
            return BadRequest(new { Message = "Supplier ID mismatch." });
        }

        _context.Entry(supplier).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Suppliers.Any(e => e.SupplierId == id))
            {
                return NotFound(new { Message = "Supplier not found." });
            }
            else
            {
                throw;
            }
        }

        return Ok(new { Message = "Supplier updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        // Find the supplier with related products
        var supplier = await _context.Suppliers
            .Include(s => s.Products) // Include related products
            .FirstOrDefaultAsync(s => s.SupplierId == id);

        if (supplier == null)
        {
            return NotFound(new { Message = "Supplier not found." });
        }

        // Disassociate products from the supplier by setting SupplierId to null
        foreach (var product in supplier.Products)
        {
            product.SupplierId = null; // Disassociate the product from the supplier
        }

        // Save changes to update product records
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the exception to see if anything went wrong during SaveChanges
            return StatusCode(500, new { Message = "Error saving product disassociations.", Details = ex.Message });
        }

        // Now remove the supplier from the Suppliers table
        _context.Suppliers.Remove(supplier);

        // Save changes to delete the supplier
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the exception for supplier deletion failure
            return StatusCode(500, new { Message = "Error deleting supplier.", Details = ex.Message });
        }

        return Ok(new { Message = "Supplier removed, related products disassociated." });
    }

}