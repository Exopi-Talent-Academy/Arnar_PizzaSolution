using Microsoft.AspNetCore.Mvc;
using PizzaPlace.Models;
using PizzaPlace.Repositories;

namespace PizzaPlace.Controllers;

[Route("api/restocking")]
public class RestockingController(IStockRepository stockRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Restock([FromBody] ComparableList<StockDto> stock)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (stock is null || stock.Count == 0)
        {
            return BadRequest("need at least one item to add to restock.");
        }

        foreach (var ingredient in stock)
        {
            if (ingredient.Amount < 0)
            {
                return BadRequest("Can't add negative amount to stock");
            }
            await stockRepository.AddToStock(ingredient);
        }

        return Ok("stock added");
    }
}
