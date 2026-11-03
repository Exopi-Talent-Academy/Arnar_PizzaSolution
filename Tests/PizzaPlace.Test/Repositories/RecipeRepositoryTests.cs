using Microsoft.EntityFrameworkCore;
using PizzaPlace.Data;
using PizzaPlace.Models;
using PizzaPlace.Models.Types;
using PizzaPlace.Repositories;
using PizzaPlace.Test;

namespace PizzaPlace.Test.Repositories;

[TestClass]
[DoNotParallelize]
public class RecipeRepositoryTests
{
    private PizzaDbContext _dbContext = null!;

    private static ComparableList<StockDto> GetStandardIngredients() =>
    [
        new StockDto(StockType.Dough, 3),
        new StockDto(StockType.Tomatoes, 10),
        new StockDto(StockType.Bacon, 2),
    ];

    private const int StandardCookingTime = 19;

    [TestInitialize]
    public async Task InitializeDatabase()
    {
        _dbContext = PostgresTestDatabase.CreateContext();
        await PostgresTestDatabase.ResetAsync(_dbContext);
        await PostgresTestDatabase.SeedRecipeStockAsync(_dbContext);
    }

    [TestCleanup]
    public async Task CleanupDatabase()
    {
        await _dbContext.DisposeAsync();
    }

    [TestMethod]
    public async Task AddRecipe_PersistsRecipeAndIngredients()
    {
        var repository = new RecipeRepository(_dbContext);
        var recipe = new PizzaRecipeDto(
            PizzaRecipeType.StandardPizza,
            GetStandardIngredients(),
            StandardCookingTime);

        var recipeId = await repository.AddRecipe(recipe);

        await using var readContext = PostgresTestDatabase.CreateContext();
        var storedRecipe = await readContext.Menu
            .Include(menu => menu.Ingredients)
            .ThenInclude(ingredient => ingredient.Stock)
            .SingleAsync(menu => menu.Id == recipeId);

        Assert.AreEqual(PizzaRecipeType.StandardPizza, storedRecipe.Type);
        Assert.AreEqual(StandardCookingTime, storedRecipe.CookingTimeMinutes);
        Assert.AreEqual(3, storedRecipe.Ingredients.Count);
        CollectionAssert.AreEquivalent(
            GetStandardIngredients().Select(x => x.Amount).ToList(),
            storedRecipe.Ingredients.Select(x => x.Amount).ToList());
        Assert.IsTrue(storedRecipe.Ingredients.All(x => x.Stock is not null));
    }

    [TestMethod]
    public async Task AddRecipe_RejectsDuplicateRecipeType()
    {
        var repository = new RecipeRepository(_dbContext);
        var recipe = new PizzaRecipeDto(
            PizzaRecipeType.StandardPizza,
            GetStandardIngredients(),
            StandardCookingTime);

        await repository.AddRecipe(recipe);

        var exception = await Assert.ThrowsExceptionAsync<PizzaException>(
            () => repository.AddRecipe(recipe));

        StringAssert.Contains(exception.Message, "StandardPizza");
    }

    [TestMethod]
    public async Task GetRecipe_ReturnsPersistedRecipeAndIngredients()
    {
        var writeRepository = new RecipeRepository(_dbContext);
        var recipe = new PizzaRecipeDto(
            PizzaRecipeType.StandardPizza,
            GetStandardIngredients(),
            StandardCookingTime);

        var recipeId = await writeRepository.AddRecipe(recipe);

        await using var readContext = PostgresTestDatabase.CreateContext();
        var readRepository = new RecipeRepository(readContext);

        var actual = await readRepository.GetRecipe(PizzaRecipeType.StandardPizza);

        Assert.AreEqual(recipeId, actual.Id);
        Assert.AreEqual(recipe.RecipeType, actual.RecipeType);
        Assert.AreEqual(recipe.CookingTimeMinutes, actual.CookingTimeMinutes);
        Assert.AreEqual(3, actual.Ingredients.Count);

        foreach (var expectedIngredient in recipe.Ingredients)
        {
            var actualIngredient = actual.Ingredients.Single(
                ingredient => ingredient.StockType == expectedIngredient.StockType);

            Assert.AreEqual(expectedIngredient.Amount, actualIngredient.Amount);
            Assert.IsTrue(actualIngredient.Id > 0);
        }
    }

    [TestMethod]
    public async Task GetRecipe_ThrowsWhenRecipeDoesNotExist()
    {
        await using var readContext = PostgresTestDatabase.CreateContext();
        var repository = new RecipeRepository(readContext);

        var exception = await Assert.ThrowsExceptionAsync<PizzaException>(
            () => repository.GetRecipe(PizzaRecipeType.ExtremelyTastyPizza));

        StringAssert.Contains(exception.Message, "ExtremelyTastyPizza");
    }
}
