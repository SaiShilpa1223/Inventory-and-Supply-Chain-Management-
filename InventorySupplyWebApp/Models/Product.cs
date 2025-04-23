using System.ComponentModel.DataAnnotations;
using InventorySupply.DAL.Models;

public class Product
{
    public int ProductId { get; set; }
    
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
    
    // Navigation property
    public Supplier Supplier { get; set; }
}
