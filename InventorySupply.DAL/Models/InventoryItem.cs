using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySupply.DAL.Models;

    public class InventoryItem
    {
        [Key]
        public int InventoryItemId { get; set; }  // Primary key for the inventory item

        // Foreign key to Product
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        // Navigation property to the related Product
        public Product? Product { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }  // Quantity currently in stock

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }  // Minimum quantity before reordering is needed
        public DateTime LastModified { get; set; }  // Date when the inventory item was last modified

        // DateAdded - this will be set when the item is first added
        public DateTime DateAdded { get; set; }
    }
