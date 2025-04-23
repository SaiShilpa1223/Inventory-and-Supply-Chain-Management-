using InventorySupply.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySupply.DAL;

public class InventorySupplyDbContext : DbContext
{
    public InventorySupplyDbContext(DbContextOptions<InventorySupplyDbContext> options)
        : base(options)
    { }

    // DbSets for each of your models
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);;
        // Configure the foreign key for Product -> Supplier relationship
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict); // Optional, prevent cascading delete

        // Configure the foreign key for InventoryItem -> Product relationship
        modelBuilder.Entity<InventoryItem>()
            .HasOne(i => i.Product)
            .WithMany() // No need for a navigation property on Product
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // Optional, cascading delete
        
        modelBuilder.Entity<InventoryItem>()
            .Property(i => i.DateAdded)
            .HasDefaultValueSql("GETDATE()");  // Use SQL to get the current date when inserting
    
        modelBuilder.Entity<InventoryItem>()
            .Property(i => i.LastModified)
            .HasDefaultValueSql("GETDATE()");  // Automatically set on modification
        
        modelBuilder.Entity<Supplier>().HasData(
            new Supplier
            {
                SupplierId = 1,
                Name = "Alpha Supplies",
                Contact = "1234567890",
                Address = "123 Alpha Street"
            },
            new Supplier
            {
                SupplierId = 2,
                Name = "Beta Traders",
                Contact = "9876543210",
                Address = "456 Beta Avenue"
            },
            new Supplier
            {
                SupplierId = 3,
                Name = "Gamma Goods",
                Contact = "5555555555",
                Address = "789 Gamma Blvd"
            }
            // ,new InventoryItem
            // {
            //     InventoryItemId = 1,
            //     ProductId = 1,
            //     QuantityInStock = 5,
            //     ReorderLevel = 1,
            // }
        );
    }
}