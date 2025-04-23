using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySupplyWebApp.Models;

public class SupplierViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(100)]
    public string Contact { get; set; }

    [StringLength(250)]
    public string Address { get; set; }

    // We don't need this to be bound
    [BindNever]
    public IEnumerable<SelectListItem> Suppliers { get; set; }
}