using System.ComponentModel.DataAnnotations;

namespace InventorySupply.DAL.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Contact { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        // Navigation property (optional for EF)
        public List<Product> Products { get; set; } = new();
    }
}