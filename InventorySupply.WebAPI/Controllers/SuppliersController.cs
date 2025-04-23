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

    // DELETE: api/suppliers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null)
        {
            return NotFound(new { Message = "Supplier not found." });
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Supplier deleted successfully" });
    }

}