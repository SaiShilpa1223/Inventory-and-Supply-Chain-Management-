using InventorySupply.DAL.Models;
using InventorySupplyWebApp.Models;

namespace InventorySupplyWebApp.Interface
{
    public interface ITransferService
    {
        Task<List<TransferProduct>> GetAllTransfersAsync();
        Task CreateTransferAsync(TransferModel model);
        Task<TransferProduct> GetTransferByIdAsync(int id);
        // Optional extras
        Task<bool> ValidateStockAsync(int productId, int quantity, int fromWarehouse);
    }
}
