using System.ComponentModel.DataAnnotations;

namespace InventorySupply.DAL.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public int Quantity { get; set; }

    // Foreign key
    [Required]
    public int? SupplierId { get; set; }
    [Required]
    public int? WarehouseId { get; set; }

    // Navigation property
    public Supplier? Supplier { get; set; }
    // Navigation property
    public Warehouse? Warehouse { get; set; }
}