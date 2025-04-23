using System.ComponentModel.DataAnnotations;

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
    public int SupplierId { get; set; }
}