using InventorySupply.DAL.Models;
using InventorySupply.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InventorySupplyWebApp.Models;
using Microsoft.EntityFrameworkCore;
using InventorySupplyWebApp.Interface;
using InventorySupplyWebApp.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InventorySupplyWebApp.Controllers
{
    public class TransferController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly InventorySupplyDbContext _context;
        private readonly ITransferService _transferService;

        public TransferController(HttpClient httpClient, InventorySupplyDbContext context, ITransferService transferService)
        {
            _context = context;
            _httpClient = httpClient;
            _transferService = transferService;
        }

        public async Task<IActionResult> Index()
        {
            var transfers = await _context.TransferProducts
                .Include(t => t.Product)
                .Include(t => t.FromWarehouseNav)
                .Include(t => t.ToWarehouseNav)
                .ToListAsync();
            return View(transfers);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new TransferModel
            {
                Products = await _context.Products
                    .Select(p => new SelectListItem { Value = p.ProductId.ToString(), Text = p.Name })
                    .ToListAsync(),
                Warehouses = await _context.Warehouses
                    .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                    .ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransferModel model)
        {
            ModelState.Remove("Products");
            ModelState.Remove("Warehouses");

            if (!ModelState.IsValid)
            {
                await PopulateDropsAsync(model);
                return View(model);
            }

            try
            {
                // This will debit/credit the Products table AND write the TransferProducts log
                await _transferService.CreateTransferAsync(model);
            }
            catch (InvalidOperationException ex)
            {
                // e.g. insufficient stock, missing product
                ModelState.AddModelError("", ex.Message);
                await PopulateDropsAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropsAsync(TransferModel model)
        {
            model.Products = _context.Products
                .Select(p => new SelectListItem { Value = p.ProductId.ToString(), Text = p.Name })
                .ToList();

            model.Warehouses = _context.Warehouses
                .Select(w => new SelectListItem { Value = w.WarehouseId.ToString(), Text = w.Name })
                .ToList();
        }

    }
}
