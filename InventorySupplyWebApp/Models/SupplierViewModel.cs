using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySupplyWebApp.Models
{
    public class SupplierViewModel
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        [StringLength(100)]
        public string Contact { get; set; }

        [StringLength(250)]
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        // We don't need this to be bound
        [BindNever] public IEnumerable<SelectListItem> Suppliers { get; set; }
    }
}
