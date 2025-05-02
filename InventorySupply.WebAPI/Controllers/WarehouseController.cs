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
    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouses()
    // {
    //     var warehouses = await _context.Warehouses
    //         .Include(w => w.InventoryItems)
    //         .ThenInclude(i => (i as InventoryItem).Product) // Cast 'i' to 'InventoryItem'
    //         .ToListAsync();
    //
    //     return Ok(warehouses);
    // }

    [HttpGet]
    public async Task<IActionResult> GetWarehouses()
    {
        var warehouses = await _context.Warehouses
            .Include(p => p.InventoryItems).ThenInclude(i => (i as InventoryItem).Product)
            .ToListAsync();
        // var products = await _context.Products.ToListAsync();
        return Ok(warehouses);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWarehouse(int id)
    {
        var warehouse = await _context.Warehouses
            .Include(w => w.InventoryItems)
        .ThenInclude(i => (i as InventoryItem).Product) // Cast 'i' to 'InventoryItem'
        .FirstOrDefaultAsync(w => w.WarehouseId == id);

        if (warehouse == null)
        {
            return NotFound();
        }

        return Ok(warehouse);
    }

    // POST: api/warehouse
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Warehouse model)
    {
        var warehouse = new Warehouse
        {
            Name = model.Name,
            Location = model.Location
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        return Ok(warehouse);
    }

    // [HttpPost]
    // public async Task<IActionResult> CreateWarehouse([FromBody] Warehouse warehouse)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }
    //
    //     _context.Warehouses.Add(warehouse);
    //     await _context.SaveChangesAsync();
    //
    //     return Ok(warehouse); // This will return 200 OK
    // }

    // [HttpPost]
    // public async Task<IActionResult> CreateWarehouse([FromBody] Warehouse warehouse)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }
    //     
    //     if (string.IsNullOrWhiteSpace(warehouse.Name))
    //         return BadRequest(new { Message = "Warehouse name is required" });
    //
    //     _context.Warehouses.Add(warehouse);
    //     await _context.SaveChangesAsync();
    //
    //     return Ok(warehouse);
    // }


    // PUT: api/warehouse/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] Warehouse updatedWarehouse)
    {
        if (id != updatedWarehouse.WarehouseId)
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
