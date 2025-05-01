using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventorySupplyWebApp.Models
{
    public class WarehouseViewModel
    {
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string WarehouseName { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(250)]
        [Required(ErrorMessage = "Address is required.")]

        // We don't need this to be bound
        [BindNever] public IEnumerable<SelectListItem> Warehouses { get; set; }
    }
}
