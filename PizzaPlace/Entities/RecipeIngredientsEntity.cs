using PizzaPlace.Models.Types;

namespace PizzaPlace.Entities
{
    public class RecipeIngredientsEntity
    {
        public long Id { get; set; }
        public int Amount { get; set; }
        public long MenuId { get; set; }
        public MenuEntity Menu { get; set; } = null!;
        public long StockId { get; set; }
        public StockEntity Stock { get; set; } = null!;
    }
}
