using PizzaPlace.Models;
using PizzaPlace.Models.Types;
using PizzaPlace.Pizzas;

namespace PizzaPlace.Factories;

/// <summary>
/// Producing one line of pizza. 
/// Taking 7 minutes to setup - and then 5 minutes less for every subsequent pizza of the same recipe type to a minimum of 4 minutes.
/// </summary>
public class AssemblyLinePizzaOven(TimeProvider timeProvider) : PizzaOven(timeProvider)
{
    private const int AssemblyLineCapacity = 1;
    public const int SetupTimeMinutes = 7;
    public const int SubsequentPizzaTimeSavingsInMinutes = 5;
    public const int MinimumCookingTimeMinutes = 4;

    protected override int Capacity => AssemblyLineCapacity;

    protected override IEnumerable<IEnumerable<(Func<Task<Pizza?>>, Guid)>> PlanPizzaMaking(IEnumerable<(PizzaRecipeDto Recipe, Guid Guid)> recipeOrders)
    {
        PizzaRecipeType? previousType = null;
        var previousTime = 0;
        var batch = new List<(Func<Task<Pizza?>>, Guid)>();
        foreach (var (recipe, guid) in recipeOrders)
        {
            var isNewTypePizza = recipe.RecipeType != previousType;

            var cookTime = isNewTypePizza
                ? recipe.CookingTimeMinutes + SetupTimeMinutes
                : Math.Max(previousTime - SubsequentPizzaTimeSavingsInMinutes, MinimumCookingTimeMinutes);

            batch.Add((MakePizza(recipe, cookTime), guid));

            previousType = recipe.RecipeType;
            previousTime = cookTime;
        }
        return [batch];
    }

    private Func<Task<Pizza?>> MakePizza(PizzaRecipeDto recipe, int cookTime) => async () =>
    {
        await CookPizza(cookTime);
        return GetPizza(recipe.RecipeType);
    };
}
