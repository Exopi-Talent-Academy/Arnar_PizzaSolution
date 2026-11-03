using PizzaPlace.Models;
using PizzaPlace.Models.Types;
using PizzaPlace.Repositories;
using PizzaPlace.Services;

namespace PizzaPlace.Test.Services;

[TestClass]
public class StockServiceTests
{
    private static StockService GetService(Mock<IStockRepository> stockRepository) =>
        new StockService(stockRepository.Object);

    private static RecipeService GetService(Mock<IRecipeRepository> recipeRepository) =>
        new(recipeRepository.Object);


    [TestMethod]
    public async Task HasSufficientStock()
    {
        // Arrange

        var reqOrder = new ComparableList<PizzaAmount>
        {
            new PizzaAmount(PizzaRecipeType.StandardPizza, 1),
            new PizzaAmount(PizzaRecipeType.OddPizza, 4),
            new PizzaAmount(PizzaRecipeType.ExtremelyTastyPizza, 2)
        };
        var order = new PizzaOrder(reqOrder);

        var standardRecipe = new PizzaRecipeDto(PizzaRecipeType.StandardPizza,
            [
                new StockDto(StockType.Dough, 2),
                new StockDto(StockType.Tomatoes, 1)
            ], 10);
        var tastyRecipe = new PizzaRecipeDto(PizzaRecipeType.ExtremelyTastyPizza,
            [
                new StockDto(StockType.UnicornDust, 1),
                new StockDto(StockType.BellPeppers, 1)
            ], 15);
        var OddRecipe = new PizzaRecipeDto(PizzaRecipeType.OddPizza,
            [
                new StockDto(StockType.Dough, 2),
                new StockDto(StockType.Sulphur, 5)
            ], 20);
        ComparableList<PizzaRecipeDto> recipeDtos = [standardRecipe, tastyRecipe, OddRecipe];


        var stockRepository = new Mock<IStockRepository>(MockBehavior.Strict);
        stockRepository.Setup(x => x.GetStock(StockType.Dough))
                .ReturnsAsync(new StockDto(StockType.Dough, 30));
        stockRepository.Setup(x => x.GetStock(StockType.Tomatoes))
                .ReturnsAsync(new StockDto(StockType.Tomatoes, 10));
        stockRepository.Setup(x => x.GetStock(StockType.UnicornDust))
                .ReturnsAsync(new StockDto(StockType.UnicornDust, 3));
        stockRepository.Setup(x => x.GetStock(StockType.BellPeppers))
                .ReturnsAsync(new StockDto(StockType.BellPeppers, 5));
        stockRepository.Setup(x => x.GetStock(StockType.Sulphur))
                .ReturnsAsync(new StockDto(StockType.Sulphur, 30));

        var service = GetService(stockRepository);

        // Act
        var actual = await service.HasInsufficientStock(order, recipeDtos);

        // Assert
        Assert.IsTrue(!actual);
    }

    [TestMethod]
    public async Task HasInSufficientStock()
    {
        // Arrange

        var reqOrder = new ComparableList<PizzaAmount>
        {
            new PizzaAmount(PizzaRecipeType.StandardPizza, 1),
            new PizzaAmount(PizzaRecipeType.OddPizza, 4),
            new PizzaAmount(PizzaRecipeType.ExtremelyTastyPizza, 2)
        };
        var order = new PizzaOrder(reqOrder);

        var standardRecipe = new PizzaRecipeDto(PizzaRecipeType.StandardPizza,
            [
                new StockDto(StockType.Dough, 2),
                new StockDto(StockType.Tomatoes, 1)
            ], 10);
        var tastyRecipe = new PizzaRecipeDto(PizzaRecipeType.ExtremelyTastyPizza,
            [
                new StockDto(StockType.UnicornDust, 1),
                new StockDto(StockType.BellPeppers, 1)
            ], 15);
        var OddRecipe = new PizzaRecipeDto(PizzaRecipeType.OddPizza,
            [
                new StockDto(StockType.Dough, 2),
                new StockDto(StockType.Sulphur, 5)
            ], 20);
        ComparableList<PizzaRecipeDto> recipeDtos = [standardRecipe, tastyRecipe, OddRecipe];


        var stockRepository = new Mock<IStockRepository>(MockBehavior.Strict);
        stockRepository.Setup(x => x.GetStock(StockType.Dough))
                .ReturnsAsync(new StockDto(StockType.Dough, 30));
        stockRepository.Setup(x => x.GetStock(StockType.Tomatoes))
                .ReturnsAsync(new StockDto(StockType.Tomatoes, 10));
        stockRepository.Setup(x => x.GetStock(StockType.UnicornDust))
                .ReturnsAsync(new StockDto(StockType.UnicornDust, 3));
        stockRepository.Setup(x => x.GetStock(StockType.BellPeppers))
                .ReturnsAsync(new StockDto(StockType.BellPeppers, 5));
        stockRepository.Setup(x => x.GetStock(StockType.Sulphur))
                .ReturnsAsync(new StockDto(StockType.Sulphur, 5));

        var service = GetService(stockRepository);

        // Act
        var actual = await service.HasInsufficientStock(order, recipeDtos);

        // Assert
        Assert.IsTrue(actual);
    }
}
