using PizzaPlace.Models.Types;

namespace PizzaPlace.Entities
{
    public class MenuEntity
    {
        public long Id { get; set; }
        public PizzaRecipeType Type { get; set; }
        public int CookingTimeMinutes { get; set; }
        public List<RecipeIngredientsEntity> Ingredients { get; set; } = [];
    }
}
