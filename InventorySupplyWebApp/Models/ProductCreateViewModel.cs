using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySupplyWebApp.Models;

public class ProductCreateViewModel
{
    public int ProductId { get; set; } // Needed for Edit
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }

    [Required]
    public int? SupplierId { get; set; }
    [Required]
    public int? WarehouseId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [BindNever] public IEnumerable<SelectListItem> Suppliers { get; set; }
    [BindNever] public IEnumerable<SelectListItem> Warehouses { get; set; }
}