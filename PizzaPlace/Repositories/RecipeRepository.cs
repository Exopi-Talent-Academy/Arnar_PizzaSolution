using Microsoft.EntityFrameworkCore;
using PizzaPlace.Data;
using PizzaPlace.Entities;
using PizzaPlace.Models;
using PizzaPlace.Models.Types;

namespace PizzaPlace.Repositories;

public class RecipeRepository(PizzaDbContext dbContext) : IRecipeRepository
{
    public async Task<long> AddRecipe(PizzaRecipeDto recipe)
    {
        var exists = await dbContext.Menu
            .AnyAsync(menu => menu.Type == recipe.RecipeType);
        if (exists)
        {
            throw new PizzaException(
                $"Recipe already exists for {recipe.RecipeType}");
        }

        var stockTypes = recipe.Ingredients
            .Select(ingredients => ingredients.StockType)
            .Distinct().ToList();

        var stocks = await dbContext.Stock
            .Where(stock => stockTypes.Contains(stock.StockType))
            .ToDictionaryAsync(stock => stock.StockType);

        

        var menuEntity = new MenuEntity
        {
            Type = recipe.RecipeType,
            CookingTimeMinutes = recipe.CookingTimeMinutes,
        };

        menuEntity.Ingredients = recipe.Ingredients
            .Select(ingredient => new RecipeIngredientsEntity
            {
                Amount = ingredient.Amount,
                StockId = stocks[ingredient.StockType].Id
            }).ToList();

        dbContext.Menu.Add(menuEntity);
        await dbContext.SaveChangesAsync();

        return menuEntity.Id;
    }
    public async Task<PizzaRecipeDto> GetRecipe(PizzaRecipeType recipeType)
    {
        var exists = await dbContext.Menu
            .AnyAsync(menu => menu.Type == recipeType);
        if (!exists)
        {
            throw new PizzaException(
                $"Recipe for {recipeType} does not exist");
        }

        var menu = await dbContext.Menu
            .Include(menu => menu.Ingredients)
            .FirstAsync(menu => menu.Type == recipeType);

        var ingredients = new ComparableList<StockDto>();
        foreach (var item in menu.Ingredients)
        {
            var stock = await dbContext.Stock
                .FirstAsync(s => s.Id == item.StockId);
            ingredients.Add(new StockDto(stock.StockType, item.Amount, stock.Id));
        }

        var recipeDto = new PizzaRecipeDto(
            menu.Type, 
            ingredients, 
            menu.CookingTimeMinutes, 
            menu.Id);

        return recipeDto;
    }
    public async Task<PizzaRecipeDto> UpdateRecipe(PizzaRecipeDto recipe)
    {
        var exists = await dbContext.Menu
            .AnyAsync(menu => menu.Type == recipe.RecipeType);
        if (!exists)
        {
            throw new PizzaException(
                $"Recipe for {recipe.RecipeType} does not exist");
        }

        var stockTypes = recipe.Ingredients
            .Select(ingredients => ingredients.StockType)
            .Distinct().ToList();
        var stocks = await dbContext.Stock
            .Where(stock => stockTypes.Contains(stock.StockType))
            .ToDictionaryAsync(stock => stock.StockType);

        var missingStockType = stockTypes
            .Where(stocktype => !stocks.ContainsKey(stocktype))
            .ToList();
        if (missingStockType.Count > 0)
        {
            throw new PizzaException(
                $"Missing stock for the following items: {string.Join(", ", missingStockType)}");
        }

        return recipe;
    }

}
