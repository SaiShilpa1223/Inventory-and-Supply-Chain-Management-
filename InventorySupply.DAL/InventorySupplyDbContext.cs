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
    public DbSet<Warehouse> Warehouses { get; set; }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);;
        // Configure the foreign key for Product -> Supplier relationship
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .HasForeignKey(p => p.Id)
            .OnDelete(DeleteBehavior.Restrict); // Optional, prevent cascading delete

        // Configure the foreign key for InventoryItem -> Product relationship
        modelBuilder.Entity<InventoryItem>()
            .HasOne(i => i.Product)
            .WithMany() // No need for a navigation property on Product
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // Optional, cascading delete

        // Configure the foreign key for InventoryItem -> Warehouse relationship
        modelBuilder.Entity<InventoryItem>()
            .HasOne(i => i.Warehouse)
            .WithMany(w => w.InventoryItems)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior as needed

        // Configure default values for InventoryItem
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
        modelBuilder.Entity<User>()
           .HasMany(u => u.RefreshTokens)
           .WithOne(rt => rt.User)
           .HasForeignKey(rt => rt.UserId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}