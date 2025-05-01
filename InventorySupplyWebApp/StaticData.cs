using InventorySupply.DAL.Models;
using InventorySupplyWebApp.Models;

namespace InventorySupplyWebApp;

public static class StaticData
{
    // public static List<Supplier> Suppliers = new List<Supplier>
    // {
    //     new Supplier { SupplierId = 1, Name = "Supplier A", Contact = "123-456-789", Address = "123 Main St" },
    //     new Supplier { SupplierId = 2, Name = "Supplier B", Contact = "987-654-321", Address = "456 Elm St" }
    // };

    public static List<Product> Products = new List<Product>
    {
        new Product { ProductId = 1, Name = "Product 1", Description = "Description of Product 1", Price = 10.00m, SupplierId = 1 },
        new Product { ProductId = 2, Name = "Product 2", Description = "Description of Product 2", Price = 20.00m, SupplierId = 2 }
    };

    // public static List<InventoryItem> InventoryItems = new List<InventoryItem>
    // {
    //     new InventoryItem { InventoryItemId = 1, ProductId = 1, Quantity = 100, UnitPrice = 10.00m },
    //     new InventoryItem { InventoryItemId = 2, ProductId = 2, Quantity = 50, UnitPrice = 20.00m }
    // };
}