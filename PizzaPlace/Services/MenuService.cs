using PizzaPlace.Models.Types;

namespace PizzaPlace.Services;

public class MenuService : IMenuService
{
    public Menu GetMenu(DateTimeOffset menuDate)
    {
        if (menuDate < DateTimeOffset.MinValue)
        {
            throw new NotImplementedException("No menu has been implemented yet.");

        }
        var ingredientsMap = new Dictionary<PizzaOptions, List<string>>
        {
            [PizzaOptions.Margherita] = [ "sauce", "cheese" ],
            [PizzaOptions.Pepperoni] = [ "sauce", "cheese", "pepperoni" ],
            [PizzaOptions.Hawaii] = [ "sauce", "cheese", "ham", "pinapple" ],
            [PizzaOptions.funguy] = [ "sauce", "cheese", "mushrooms" ],
            [PizzaOptions.Swamperoni] = [ "sauce", "cheese", "pepperoni", "mushrooms" ],
            [PizzaOptions.Vegetar] = [ "sauce", "cheese", "paprika", "onion", "olives", "mushrooms", "cherry tomatoes" ],
            [PizzaOptions.Kebab] = [ "sauce", "cheese", "kebab", "salad", "dressing" ],
            [PizzaOptions.Carbonara] = [ "sauce", "cheese", "pepperoni", "bacon", "parmigiano" ],
            [PizzaOptions.Mexicana] = [ "sauce", "cheese", "beef", "paprika", "onion", "jalapeno", "" ],
            [PizzaOptions.Cheezus] = [ "sauce", "cheese", "mozzarella", "parmigiano", "gorgonzola" ],
            [PizzaOptions.MeatParty] = [ "sauce", "cheese", "pepperoni", "ham", "beef", "bacon" ],
            [PizzaOptions.Parma] = [ "sauce", "cheese", "prosciutto", "parmigiano", "rucola"],
        };

        var pizzas = new List<PizzaItem>
        {
            new PizzaItem(PizzaOptions.Margherita, PizzaRecipeType.StandardPizza, ingredientsMap[PizzaOptions.Margherita], 100, 80),
            new PizzaItem(PizzaOptions.Pepperoni, PizzaRecipeType.StandardPizza, ingredientsMap[PizzaOptions.Pepperoni], 110, 90),
            new PizzaItem(PizzaOptions.Hawaii, PizzaRecipeType.StandardPizza, ingredientsMap[PizzaOptions.Hawaii], 110, 90),
            new PizzaItem(PizzaOptions.funguy, PizzaRecipeType.OddPizza, ingredientsMap[PizzaOptions.funguy], 120, 100),
            new PizzaItem(PizzaOptions.Swamperoni, PizzaRecipeType.StandardPizza, ingredientsMap[PizzaOptions.Swamperoni], 110, 90),
            new PizzaItem(PizzaOptions.Vegetar, PizzaRecipeType.StandardPizza, ingredientsMap[PizzaOptions.Vegetar], 110, 90),
            new PizzaItem(PizzaOptions.Kebab, PizzaRecipeType.ExtremelyTastyPizza, ingredientsMap[PizzaOptions.Kebab], 120, 100),
            new PizzaItem(PizzaOptions.Carbonara, PizzaRecipeType.ExtremelyTastyPizza, ingredientsMap[PizzaOptions.Carbonara], 120, 100),
            new PizzaItem(PizzaOptions.Mexicana, PizzaRecipeType.OddPizza, ingredientsMap[PizzaOptions.Mexicana], 120, 100),
            new PizzaItem(PizzaOptions.Cheezus, PizzaRecipeType.ExtremelyTastyPizza, ingredientsMap[PizzaOptions.Cheezus], 120, 100),
            new PizzaItem(PizzaOptions.MeatParty, PizzaRecipeType.ExtremelyTastyPizza, ingredientsMap[PizzaOptions.MeatParty], 130, 110),
            new PizzaItem(PizzaOptions.Parma, PizzaRecipeType.ExtremelyTastyPizza, ingredientsMap[PizzaOptions.Parma], 130, 110)
        };
        var isLunch = menuDate.Hour >= 11 && menuDate.Hour <= 14;
        var pricedItems = pizzas.Select(pizza => new MenuItem(pizza.name.ToString(), pizza.type, isLunch ? pizza.lunchPrice : pizza.price)).ToComparableList();

        var title = isLunch ? "Lunch Menu" : "Regular Menu";

        var menu = new Menu(title, pricedItems);


        return menu;
    }
}
 
public enum PizzaOptions
{
    Margherita = 1,
    Pepperoni,
    Hawaii,
    funguy,
    Swamperoni,
    Vegetar,
    Kebab,
    Carbonara,
    Mexicana,
    Cheezus,
    MeatParty,
    Parma,
}
public readonly struct PizzaItem(PizzaOptions name, PizzaRecipeType type, List<string> ingredients, int price, int lunchPrice)
{
    public readonly PizzaOptions name = name;
    public readonly PizzaRecipeType type = type;
    public readonly List<string> ingredients = ingredients;
    public readonly int price = price;
    public readonly int lunchPrice = lunchPrice;
}