using Microsoft.EntityFrameworkCore;
using PizzaPlace.Data;
using PizzaPlace.Entities;
using PizzaPlace.Models;
using PizzaPlace.Models.Types;
using PizzaPlace.Repositories;
using PizzaPlace.Test;
using System.ComponentModel.DataAnnotations;

namespace PizzaPlace.Test.Repositories;

[TestClass]
[DoNotParallelize]
public class StockRepositoryTests
{
    private PizzaDbContext _dbContext = null!;

    [TestInitialize]
    public async Task InitializeDatabase()
    {
        _dbContext = PostgresTestDatabase.CreateContext();
        await PostgresTestDatabase.ResetAsync(_dbContext);
    }

    [TestCleanup]
    public async Task CleanupDatabase()
    {
        await _dbContext.DisposeAsync();
    }

    [TestMethod]
    public async Task AddToStock_CreatesAndPersistsStock()
    {
        var repository = new StockRepository(_dbContext);

        var result = await repository.AddToStock(
            new StockDto(StockType.TrippleBacon, 10));

        await using var readContext = PostgresTestDatabase.CreateContext();
        var storedStock = await readContext.Stock
            .SingleAsync(stock => stock.StockType == StockType.TrippleBacon);

        Assert.IsTrue(result.Id > 0);
        Assert.AreEqual(10, result.Amount);
        Assert.AreEqual(10, storedStock.Amount);
    }

    [TestMethod]
    public async Task AddToStock_IncreasesExistingAmount()
    {
        var repository = new StockRepository(_dbContext);

        await repository.AddToStock(
            new StockDto(StockType.UnicornDust, 10));

        var result = await repository.AddToStock(
            new StockDto(StockType.UnicornDust, 13));

        Assert.AreEqual(23, result.Amount);

        await using var readContext = PostgresTestDatabase.CreateContext();
        var storedStock = await readContext.Stock
            .SingleAsync(stock => stock.StockType == StockType.UnicornDust);

        Assert.AreEqual(23, storedStock.Amount);
    }

    [TestMethod]
    public async Task AddToStock_RejectsNegativeAmount()
    {
        var repository = new StockRepository(_dbContext);

        var exception = await Assert.ThrowsExceptionAsync<PizzaException>(
            () => repository.AddToStock(
                new StockDto(StockType.TrippleBacon, -10)));

        Assert.AreEqual("Can't add 0 or negative amount of stock item", exception.Message);
    }

    [TestMethod]
    public async Task GetStock_RejectsWhenStockDoesNotExist()
    {
        var repository = new StockRepository(_dbContext);

        var exception = await Assert.ThrowsExceptionAsync<PizzaException>(
            () => repository.GetStock(StockType.UngenericSpices));

        Assert.AreEqual($"No stock item of type {StockType.UngenericSpices} exists", exception.Message);
    }

    [TestMethod]
    public async Task TakeStock_DecreasesStoredAmount()
    {
        var repository = new StockRepository(_dbContext);

        await repository.AddToStock(
            new StockDto(StockType.FermentedDough, 12));

        var result = await repository.TakeStock(
            StockType.FermentedDough,
            7);

        Assert.AreEqual(StockType.FermentedDough, result.StockType);
        Assert.AreEqual(7, result.Amount);

        var remaining = await repository.GetStock(StockType.FermentedDough);
        Assert.AreEqual(5, remaining.Amount);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-3)]
    public async Task TakeStock_RejectsZeroOrNegativeAmount(int amount)
    {
        var repository = new StockRepository(_dbContext);

        await repository.AddToStock(
            new StockDto(StockType.FermentedDough, 5));

        var exception = await Assert.ThrowsExceptionAsync<PizzaException>(
            () => repository.TakeStock(StockType.FermentedDough, amount));

        Assert.AreEqual(
            "Can't take zero or negative amount of stock",
            exception.Message);
    }

    [TestMethod]
    public async Task TakeStock_RejectsInsufficientStock()
    {
        var repository = new StockRepository(_dbContext);
        var stockAmount = 5;
        var takeAmount = 6;

        await repository.AddToStock(
            new StockDto(StockType.FermentedDough, stockAmount));

        var exception = await Assert.ThrowsExceptionAsync<PizzaException>(
            () => repository.TakeStock(StockType.FermentedDough, takeAmount));

        Assert.AreEqual(
            $"not enough stock: {stockAmount} for amount of: {takeAmount}",
            exception.Message);
    }
}
