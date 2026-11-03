using Microsoft.AspNetCore.Mvc;
using PizzaPlace.Controllers;
using PizzaPlace.Models;
using PizzaPlace.Models.Types;
using PizzaPlace.Repositories;

namespace PizzaPlace.Test.Controllers;

[TestClass]
public class RestockingControllerTests
{
    private static RestockingController GetController(Mock<IStockRepository> stockRepository) =>
        new(stockRepository.Object);

    [TestMethod]
    public async Task Restock_success()
    {
        // Arrange
        StockDto dough = new StockDto(StockType.Dough, 10);
        StockDto tomatoes = new StockDto(StockType.Dough, 10);
        StockDto sulphur = new StockDto(StockType.Dough, 10);

        StockDto doughStock = new StockDto(StockType.Dough, 20+dough.Amount, 1);
        StockDto tomatoesStock = new StockDto(StockType.Dough, 30+tomatoes.Amount, 2);
        StockDto sulphurStock = new StockDto(StockType.Dough, 10+sulphur.Amount, 3);

        ComparableList<StockDto> stock = [dough, tomatoes, sulphur];

        var stockRepository = new Mock<IStockRepository>(MockBehavior.Strict);
        stockRepository.Setup(x => x.AddToStock(dough))
                .ReturnsAsync(doughStock);
        stockRepository.Setup(x => x.AddToStock(tomatoes))
                .ReturnsAsync(tomatoesStock);
        stockRepository.Setup(x => x.AddToStock(sulphur))
                .ReturnsAsync(sulphurStock);

        var controller = GetController(stockRepository);

        // Act
        var actual = await controller.Restock(stock);

        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(actual);
        stockRepository.VerifyAll();
    }

    [TestMethod]
    public async Task Restock_negative_amount_failure()
    {
        // Arrange
        StockDto dough = new StockDto(StockType.Dough, -10);

        ComparableList<StockDto> stock = [dough];

        var stockRepository = new Mock<IStockRepository>(MockBehavior.Strict);


        var controller = GetController(stockRepository);

        // Act
        var actual = await controller.Restock(stock);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(actual);
        stockRepository.VerifyAll();
    }
    [TestMethod]
    public async Task Restock_empty_stock_failure()
    {
        // Arrange

        ComparableList<StockDto> stock = [];

        var stockRepository = new Mock<IStockRepository>(MockBehavior.Strict);


        var controller = GetController(stockRepository);

        // Act
        var actual = await controller.Restock(stock);

        // Assert
        Assert.IsInstanceOfType<BadRequestObjectResult>(actual);
        stockRepository.VerifyAll();
    }
}

