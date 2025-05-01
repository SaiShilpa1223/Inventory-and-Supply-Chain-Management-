using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySupply.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly InventorySupplyDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(InventorySupplyDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products
            .Include(p => p.Supplier)
            .Include(q => q.Warehouse)
            .ToListAsync();
        // var products = await _context.Products.ToListAsync();
        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
    
    [HttpPost]
    //[Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok(product); // This will return 200 OK
    }
    
    // PUT: api/products/{id}
    [HttpPut("{id}")]
    //[Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> PutProduct(int id, [FromBody] Product product)
    {
        if (id != product.ProductId)
        {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Product updated successfully" });

    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound(new { Message = "Product not found." });
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Deleted successfully" });
    }
}