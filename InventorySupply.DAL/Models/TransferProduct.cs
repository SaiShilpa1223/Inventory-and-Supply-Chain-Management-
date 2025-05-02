using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySupply.DAL.Models
{
    public class TransferProduct
    {
        public int Id { get; set; }

        // Foreign key to Product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Transfer details
        public int TranferQty { get; set; }
        public int FromWarehouse { get; set; }
        public int ToWarehouse { get; set; }

        public Warehouse FromWarehouseNav { get; set; }
        public Warehouse ToWarehouseNav { get; set; }

        public DateTime TransferDate { get; set; }
    }
}
