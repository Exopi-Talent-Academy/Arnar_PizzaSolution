using Microsoft.EntityFrameworkCore;
using PizzaPlace.Entities;

namespace PizzaPlace.Data
{
    public class PizzaDbContext(DbContextOptions<PizzaDbContext> options)
        : DbContext(options) 
    {
        public DbSet<MenuEntity> Menu => Set<MenuEntity>();
        public DbSet<RecipeIngredientsEntity> RecipeIngredients => Set<RecipeIngredientsEntity>();
        public DbSet<StockEntity> Stock => Set<StockEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockEntity>()
                .Property(stock => stock.StockType)
                .HasConversion<string>();

            modelBuilder.Entity<MenuEntity>()
                .Property(menu => menu.Type)
                .HasConversion<string>();
        }
    }
}
