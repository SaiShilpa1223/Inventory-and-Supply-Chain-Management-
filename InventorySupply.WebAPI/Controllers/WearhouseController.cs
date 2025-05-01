using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly InventorySupplyDbContext _context;

    public WarehouseController(InventorySupplyDbContext context)
    {
        _context = context;
    }

    // GET: api/warehouse
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouses()
    {
        var warehouses = await _context.Warehouses
            .Include(w => w.InventoryItems)
            .ThenInclude(i => (i as InventoryItem).Product) // Cast 'i' to 'InventoryItem'
            .ToListAsync();

        return Ok(warehouses);
    }

    // GET: api/warehouse/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Warehouse>> GetWarehouse(int id)
    {
        var warehouse = await _context.Warehouses
            .Include(w => w.InventoryItems)
            .ThenInclude(i => (i as InventoryItem).Product) // Cast 'i' to 'InventoryItem'
            .FirstOrDefaultAsync(w => w.Id == id);

        if (warehouse == null)
            return NotFound();

        return Ok(warehouse);
    }

    // POST: api/warehouse
    [HttpPost]
    public async Task<IActionResult> CreateWarehouse([FromBody] Warehouse warehouse)
    {
        if (string.IsNullOrWhiteSpace(warehouse.Name))
            return BadRequest(new { Message = "Warehouse name is required" });

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        return Ok(warehouse);
    }

    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] Warehouse updatedWarehouse)
    {
        if (id != updatedWarehouse.Id)
            return BadRequest("Warehouse ID mismatch");

        var existingWarehouse = await _context.Warehouses.FindAsync(id);
        if (existingWarehouse == null)
            return NotFound();

        existingWarehouse.Name = updatedWarehouse.Name;
        existingWarehouse.Location = updatedWarehouse.Location;

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Warehouse updated successfully" });
    }

    // DELETE: api/warehouse/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWarehouse(int id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null)
            return NotFound();

        _context.Warehouses.Remove(warehouse);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Warehouse deleted successfully" });
    }
}
