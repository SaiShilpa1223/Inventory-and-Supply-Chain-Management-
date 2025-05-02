using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventorySupplyWebApp.Models
{
    public class TransferModel
    {
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "From Warehouse")]
        public int FromWarehouseId { get; set; }

        [Required]
        [Display(Name = "To Warehouse")]
        public int ToWarehouseId { get; set; }

        [Required]
        [Display(Name = "Transfer Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Transfer quantity must be at least 1.")]
        public int TransferQty { get; set; }

        public IEnumerable<SelectListItem> Products { get; set; }
        public IEnumerable<SelectListItem> Warehouses { get; set; }
    }
}
