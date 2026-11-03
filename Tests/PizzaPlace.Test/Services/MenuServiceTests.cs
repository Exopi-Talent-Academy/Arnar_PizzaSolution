using PizzaPlace.Models;
using PizzaPlace.Models.Types;
using PizzaPlace.Repositories;
using PizzaPlace.Services;

namespace PizzaPlace.Test.Services;

[TestClass]
public class MenuServiceTests
{
    private static MenuService GetService(IMenuService menuService) => new();


    [TestMethod]
    public async Task GetBothMenus()
    {
        // Arrange
        var lunchTime = new DateTimeOffset(2026, 7, 7, 12, 0, 0, TimeSpan.Zero);
        var regularHours = new DateTimeOffset(2026, 7, 7, 19, 0, 0, TimeSpan.Zero);
        var mockMenu = new Mock<IMenuService>();
        var menuService = GetService(mockMenu.Object);

        // Act
        var actualLunchMenu = menuService.GetMenu(lunchTime);
        var actualRegularHours = menuService.GetMenu(regularHours);
        // Assert
        Assert.AreEqual("Lunch Menu", actualLunchMenu.Title);
        Assert.AreEqual("Regular Menu", actualRegularHours.Title);

    }
}

