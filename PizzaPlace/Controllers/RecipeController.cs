using Microsoft.AspNetCore.Mvc;
using PizzaPlace.Models;
using PizzaPlace.Models.Types;
using PizzaPlace.Repositories;

namespace PizzaPlace.Controllers;

[Route("api/recipe")]
public class RecipeController(IRecipeRepository recipeRepository) : Controller
{
    [HttpPost("addrecipe")]
    public async Task<IActionResult> AddRecipe([FromBody] PizzaRecipeDto recipe)
    {
        return Ok(await recipeRepository.AddRecipe(recipe));
    }

    [HttpPost("updaterecipe")]
    public async Task<IActionResult> UpdateRecipe([FromBody] PizzaRecipeDto recipe)
    {

        return Ok(await recipeRepository.UpdateRecipe(recipe));
    }

    [HttpPost("getrecipe")]
    public async Task<IActionResult> GetRecipe([FromBody] PizzaRecipeType recipe)
    {

        return Ok(await recipeRepository.GetRecipe(recipe));
    }

}
