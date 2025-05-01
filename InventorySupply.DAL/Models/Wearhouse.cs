using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySupply.DAL.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        // Navigation property (optional for EF)
        public List<Product> Products { get; set; } = new();
    }

}
