namespace InventorySupplyWebApp.Models
{
    public class ReportsModel
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
