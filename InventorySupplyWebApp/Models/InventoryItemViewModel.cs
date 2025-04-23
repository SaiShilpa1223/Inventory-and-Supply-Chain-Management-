using System.ComponentModel.DataAnnotations;
using InventorySupply.DAL.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySupplyWebApp.Models
{
    public class InventoryItemViewModel
    {
        [Required]
        public int ProductId { get; set; } // Product that the inventory item is linked to

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int QuantityInStock { get; set; } // Quantity of this product in stock

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder level must be a positive number.")]
        public int ReorderLevel { get; set; } // Reorder level: the minimum stock level before reordering is necessary

        [BindNever]
        public IEnumerable<SelectListItem> Products { get; set; } // List of products (only used for dropdowns or lists in forms)

        public int InventoryItemId { get; set; } // Unique identifier for the inventory item

        public int SupplierId { get; set; } // Foreign key for the supplier linked to the inventory item

        public virtual Supplier Supplier { get; set; } // Navigation property to the Supplier, representing the related supplier

        public int Quantity { get; set; } // Quantity of the product (another field for stock, can be same as QuantityInStock)

        public DateTime DateModified { get; set; }
        public Product? Product { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }

}