using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySupply.DAL.Models
{
    public class ProductReportDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string WarehouseName { get; set; }
        public int WarehouseId { get; set; }
        public string SupplierName { get; set; }
    }
}
