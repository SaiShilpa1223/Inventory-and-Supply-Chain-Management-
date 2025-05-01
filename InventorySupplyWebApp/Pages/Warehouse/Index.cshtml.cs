using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventorySupplyWebApp.Pages.Warehouse
{
    public class IndexModel : PageModel
    {
        private readonly InventorySupplyDbContext _context;

        public IndexModel(InventorySupplyDbContext context)
        {
            _context = context;
        }

        public List<InventoryItem> InventoryList { get; set; } = new();

        public async Task OnGetAsync()
        {
            InventoryList = await _context.InventoryItems
                .Include(i => i.Product)
                .Include(i => i.InventoryItemId)
                .ToListAsync();
        }
    }
}