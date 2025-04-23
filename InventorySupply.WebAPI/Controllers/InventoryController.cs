using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly InventorySupplyDbContext _context;

    public InventoryController(InventorySupplyDbContext context)
    {
        _context = context;
    }

     // GET: api/inventory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventoryItems()
    {
        var inventoryItems = await _context.InventoryItems
            .Include(i => i.Product)
            .ThenInclude(p => p.Supplier)
            .ToListAsync();

        return Ok(inventoryItems);
    }
    
    // GET: api/inventory/5
    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
    {
        var item = await _context.InventoryItems
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.InventoryItemId == id);

        if (item == null)
            return NotFound();

        return item;
    }

    // POST: api/inventory
    [HttpPost]
    public async Task<IActionResult> CreateInventory([FromBody] InventoryItem item)
    {
        var product = await _context.Products.FindAsync(item.ProductId);
        if (product == null)
            return NotFound(new { Message = "Product not found" });

        item.DateAdded = DateTime.UtcNow;
        item.LastModified = DateTime.UtcNow;
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        return Ok(item);
    }


    // PUT: api/inventory/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryItem updatedItem)
    {
        if (id != updatedItem.InventoryItemId)
            return BadRequest("Inventory ID mismatch");

        var existingItem = await _context.InventoryItems.FindAsync(id);
        if (existingItem == null)
            return NotFound();

        existingItem.ProductId = updatedItem.ProductId;
        existingItem.QuantityInStock = updatedItem.QuantityInStock;
        existingItem.ReorderLevel = updatedItem.ReorderLevel;
        existingItem.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Inventory updated successfully" });
    }

    // DELETE: api/inventory/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventory(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item == null)
            return NotFound();

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Inventory deleted successfully" });
    }
}
