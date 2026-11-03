using PizzaPlace.Models;
using PizzaPlace.Models.Types;
using PizzaPlace.Pizzas;
using PizzaPlace.Repositories;

namespace PizzaPlace.Services;

public class StockService(IStockRepository stockRepository) : IStockService
{
    public async Task<bool> HasInsufficientStock(PizzaOrder order, ComparableList<PizzaRecipeDto> recipeDtos)
    {
        // Validate input, the order and recipeDtos
        //if (recipeDtos.Contains(pizzas.PizzaType))
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }
        if (recipeDtos == null)
        {
            throw new ArgumentNullException(nameof(recipeDtos));
        }

        foreach (var pizza in order.RequestedOrder)
        {
            if (pizza.Amount == 0)
            {
                throw new ArgumentException("pizza amount must be greater than zero", nameof(order));
            }

            var countValid = recipeDtos.Count(recipe => recipe.RecipeType == pizza.PizzaType);
            if (countValid != 1)
            {
                throw new ArgumentException("no valid recipe found for a pizza in order", nameof(order));
            }

        }


        // count total ingredients then check stock
        var neededIngredients = new Dictionary<StockType, int>();
        foreach (var pizzas in order.RequestedOrder)
        {

            foreach (var recipe in recipeDtos)
            {
                if (pizzas.PizzaType == recipe.RecipeType)
                {
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        neededIngredients.TryGetValue(ingredient.StockType, out var currentAmount);
                        neededIngredients[ingredient.StockType] = currentAmount + (ingredient.Amount * pizzas.Amount);
                    }
                }
            }
        }
        foreach (var (key, value) in neededIngredients)
        {
            var availableStock = await stockRepository.GetStock(key);
            if (availableStock.Amount < value)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<ComparableList<StockDto>> GetStock(PizzaOrder order, ComparableList<PizzaRecipeDto> recipeDtos)
    {
        if (await HasInsufficientStock(order, recipeDtos))
        {
            throw new PizzaException("Not enough stock for this order");
        }

        ComparableList<StockDto> stock = [];
        foreach (var pizza in order.RequestedOrder)
        {
            foreach (var recipe in recipeDtos)
            {
                if (pizza.PizzaType == recipe.RecipeType)
                {
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        var amount = pizza.Amount * ingredient.Amount;
                        stock.Add(await stockRepository.TakeStock(ingredient.StockType, amount));
                    }
                }
            }
        }
        return stock;

    }


}
