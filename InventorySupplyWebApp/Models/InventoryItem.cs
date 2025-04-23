using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySupplyWebApp.Models;

public class InventoryItem
{
    public int ProductId { get; set; }

    public int QuantityInStock { get; set; }

    public int ReorderLevel { get; set; }

    public DateTime DateAdded { get; set; } = DateTime.Now;

    public DateTime LastModified { get; set; } = DateTime.Now;

    public SelectList Products { get; set; } // For dropdown
}