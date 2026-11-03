using Microsoft.EntityFrameworkCore;
using PizzaPlace.Data;
using PizzaPlace.Entities;
using PizzaPlace.Models;
using PizzaPlace.Models.Types;
using System.Diagnostics;

namespace PizzaPlace.Repositories;

public class StockRepository(PizzaDbContext dbContext) : IStockRepository
{
    public async Task<StockDto> AddToStock(StockDto stockDto)
    {

        if (stockDto.Amount < 1)
        {
            throw new PizzaException(
                $"Can't add 0 or negative amount of stock item");
        }

        var stockEntity = await dbContext.Stock
            .SingleOrDefaultAsync(stock => stock.StockType == stockDto.StockType);
        
        if (stockEntity is null)
        {
            stockEntity = new StockEntity
            {
                StockType = stockDto.StockType,
                Amount = stockDto.Amount
            };
            dbContext.Stock.Add(stockEntity);
        }
        else
        {
            stockEntity.Amount += stockDto.Amount;
        }

        await dbContext.SaveChangesAsync();

        return new StockDto(
            stockEntity.StockType,
            stockEntity.Amount,
            stockEntity.Id);
    }
    public async Task<StockDto> GetStock(StockType stockType)
    {

        var stockEntity = await dbContext.Stock
            .SingleOrDefaultAsync(stock => stock.StockType == stockType) ??
                throw new PizzaException(
                    $"No stock item of type {stockType} exists");

        await dbContext.SaveChangesAsync();

        return new StockDto(
            stockEntity.StockType,
            stockEntity.Amount,
            stockEntity.Id);

    }
    public async Task<StockDto> TakeStock(StockType stockType, int amount)
    {
        if (amount <= 0)
        {
            throw new PizzaException(
                $"Can't take zero or negative amount of stock");
        }

        var stockEntity = await dbContext.Stock
            .SingleOrDefaultAsync(stock => stock.StockType == stockType) ??
                throw new PizzaException(
                    $"Cannot take from {stockType}, it does not exists");

        if (amount > stockEntity.Amount)
        {
            throw new PizzaException(
                $"not enough stock: {stockEntity.Amount} for amount of: {amount}");
        }
        stockEntity.Amount = int.Max(0, stockEntity.Amount - amount);

        await dbContext.SaveChangesAsync();

        return new StockDto(
            stockEntity.StockType,
            amount,
            stockEntity.Id);
    } 
}
