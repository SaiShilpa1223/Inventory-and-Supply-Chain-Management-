using InventorySupply.DAL;
using InventorySupply.DAL.Models;
using InventorySupplyWebApp.Interface;
using InventorySupplyWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySupplyWebApp.Services
{
    public class TransferService : ITransferService
    {
        private readonly InventorySupplyDbContext _context;

        public TransferService(InventorySupplyDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransferProduct>> GetAllTransfersAsync()
        {
            return await _context.TransferProducts
                                 .Include(t => t.Product)
                                 .ToListAsync();
        }

        public async Task<TransferProduct> GetTransferByIdAsync(int id)
        {
            return await _context.TransferProducts
                                 .Include(t => t.Product)
                                 .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> ValidateStockAsync(int productId, int quantity, int fromWarehouse)
        {       

            var prod = await _context.Products
                           .FirstOrDefaultAsync(p =>
                               p.ProductId == productId
                            && p.WarehouseId == fromWarehouse);

            return prod != null && prod.Quantity >= quantity;
        }

        public async Task CreateTransferAsync(TransferModel model)
        {
            // start a transaction so both the Products table and the TransferProducts table
            // update together
            using var tx = await _context.Database.BeginTransactionAsync();

            // 1) load the “from” product
            var src = await _context.Products
                             .FirstOrDefaultAsync(p => p.ProductId == model.ProductId
                                                    && p.WarehouseId == model.FromWarehouseId);

            if (src == null)
                throw new InvalidOperationException("Source warehouse does not have that product.");

            if (src.Quantity < model.TransferQty)
                throw new InvalidOperationException("Insufficient stock to transfer.");

            // debit the source
            src.Quantity -= model.TransferQty;
            _context.Products.Update(src);

            // 2) load or create the “to” product
            var dest = await _context.Products
                              .FirstOrDefaultAsync(p => p.ProductId == model.ProductId
                                                     && p.WarehouseId == model.ToWarehouseId);

            if (dest != null)
            {
                dest.Quantity += model.TransferQty;
                _context.Products.Update(dest);
            }
            else
            {
                // clone the product metadata into the new warehouse
                dest = new Product
                {
                    Name = src.Name,
                    Description = src.Description,
                    SupplierId = src.SupplierId,
                    WarehouseId = model.ToWarehouseId,
                    Quantity = model.TransferQty,
                    Price= src.Price,
                };
                _context.Products.Add(dest);
            }

            // 3) persist product changes
            await _context.SaveChangesAsync();

            // 4) log the transfer
            var log = new TransferProduct
            {
                ProductId = model.ProductId,
                TranferQty = model.TransferQty,
                FromWarehouse = model.FromWarehouseId,
                ToWarehouse = model.ToWarehouseId,
                TransferDate = DateTime.UtcNow
            };
            _context.TransferProducts.Add(log);
            await _context.SaveChangesAsync();

            // 5) commit
            await tx.CommitAsync();
        }
    }
}
