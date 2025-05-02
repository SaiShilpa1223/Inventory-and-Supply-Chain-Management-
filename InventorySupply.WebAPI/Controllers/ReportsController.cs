using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySupply.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : Controller
    {
        private readonly InventorySupplyDbContext _context;

        public ReportsController(InventorySupplyDbContext context)
        {
            _context = context;
        }

        // GET api/reports/products-by-warehouse
        [HttpGet("products-by-warehouse")]
        public async Task<ActionResult<IEnumerable<ProductReportDto>>> GetProductsByWarehouse()
        {
            // Join Products with Warehouses and Suppliers
            var query = from p in _context.Products
                        join w in _context.Warehouses on p.WarehouseId equals w.WarehouseId
                        join s in _context.Suppliers on p.SupplierId equals s.SupplierId
                        select new ProductReportDto
                        {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            Description = p.Description,
                            Quantity = p.Quantity,
                            WarehouseId = w.WarehouseId,
                            WarehouseName = w.Name,
                            SupplierName = s.Name
                        };

            var list = await query
                .OrderBy(r => r.WarehouseName)
                .ThenBy(r => r.Name)
                .ToListAsync();

            return Ok(list);
        }
    }
}

