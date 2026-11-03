using PizzaPlace.Models;
using PizzaPlace.Repositories;

namespace PizzaPlace.Services;

public class RecipeService(IRecipeRepository recipeRepository) : IRecipeService
{
    public async Task<ComparableList<PizzaRecipeDto>> GetPizzaRecipes(PizzaOrder order)
    {
        var pizzaTypes = order.RequestedOrder
            .Select(x => x.PizzaType)
            .Distinct()
            .ToList();

        ComparableList<PizzaRecipeDto> recipes = [];
        foreach (var pizzaType in pizzaTypes)
        {
            recipes.Add(await recipeRepository.GetRecipe(pizzaType));
        }

        return recipes;
    }

    public async Task<long> AddRecipe(PizzaRecipeDto recipe)
    {
        if (recipe is null)
        {
            throw new NullReferenceException();
        }
        if (recipe.Id != 0)
        {
            throw new PizzaException("id should be zero");
        }
        if (recipe.Ingredients.Count == 0)
        {
            throw new PizzaException("no ingredients given for recipe");
        }


        return await recipeRepository.AddRecipe(recipe);
    }

    public async Task<PizzaRecipeDto> UpdateRecipe(PizzaRecipeDto recipe)
    {
        if (recipe is null)
        {
            throw new NullReferenceException();
        }
        if (recipe.Id == 0)
        {
            throw new PizzaException("no id given");
        }
        if (recipe.Ingredients.Count == 0)
        {
            throw new PizzaException("no ingredients given for recipe");
        }
        return await recipeRepository.UpdateRecipe(recipe);
    }
}
