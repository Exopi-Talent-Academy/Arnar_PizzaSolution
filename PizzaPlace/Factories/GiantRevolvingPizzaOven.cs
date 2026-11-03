using PizzaPlace.Models;
using PizzaPlace.Models.Types;
using PizzaPlace.Pizzas;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace PizzaPlace.Factories;

/// <summary>
/// Produces pizza on one giant revolving surface. Downside is, that all pizzas must have the same cooking time, when prepared at the same time.
/// </summary>
public class GiantRevolvingPizzaOven(TimeProvider timeProvider) : PizzaOven(timeProvider)
{
    private const int GiantRevolvingPizzaOvenCapacity = 120;

    protected override int Capacity => GiantRevolvingPizzaOvenCapacity;

    protected override IEnumerable<IEnumerable<(Func<Task<Pizza?>>, Guid)>> PlanPizzaMaking(IEnumerable<(PizzaRecipeDto Recipe, Guid Guid)> recipeOrders)
    {
        var cookTimesGrouped = recipeOrders.GroupBy(order => order.Recipe.CookingTimeMinutes);
        var batches = new List<IEnumerable<(Func<Task<Pizza?>>, Guid)>>();
        foreach (var group in cookTimesGrouped)
        {
            foreach (var batch in group.Chunk(Capacity))
            {
                var newBatch = new List<(Func<Task<Pizza?>>, Guid)>();
                foreach (var pizza in batch)
                {
                    var recipe = pizza.Item1;
                    var orderGuid = pizza.Item2;
                    newBatch.Add((MakePizza(recipe, recipe.CookingTimeMinutes), orderGuid));
                }

                batches.Add(newBatch);
            }
        }
        return batches;

    }

    private Func<Task<Pizza?>> MakePizza(PizzaRecipeDto recipe, int cookTimeMinutes) => async () =>
    {
        await CookPizza(cookTimeMinutes);
        return GetPizza(recipe.RecipeType);
    };
}
