using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace InventorySupply.DAL;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<InventorySupplyDbContext>
{
    public InventorySupplyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventorySupplyDbContext>();

        // Build the connection string for your database
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new InventorySupplyDbContext(optionsBuilder.Options);
    }
}