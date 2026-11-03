using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PizzaPlace.Data;
using PizzaPlace.Entities;
using PizzaPlace.Models.Types;

namespace PizzaPlace.Test;

internal static class PostgresTestDatabase
{
    private sealed class UserSecretsMarker
    {
    }

    private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .AddUserSecrets<UserSecretsMarker>()
        .Build();

    public static PizzaDbContext CreateContext()
    {
        var connectionString = Configuration.GetConnectionString("PizzaTestDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "PizzaTestDatabase connection string was not found.");
        }

        var options = new DbContextOptionsBuilder<PizzaDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new PizzaDbContext(options);
    }

    public static async Task ResetAsync(PizzaDbContext dbContext)
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }

    public static async Task SeedRecipeStockAsync(PizzaDbContext dbContext)
    {
        dbContext.Stock.AddRange(
            new StockEntity
            {
                StockType = StockType.Dough,
                Amount = 100
            },
            new StockEntity
            {
                StockType = StockType.Tomatoes,
                Amount = 100
            },
            new StockEntity
            {
                StockType = StockType.Bacon,
                Amount = 100
            });

        await dbContext.SaveChangesAsync();
    }
}
