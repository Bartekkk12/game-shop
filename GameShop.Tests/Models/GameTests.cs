using FluentAssertions;
using GameShop.Models;
using Xunit;

namespace GameShop.Tests.Models
{
    public class GameTests
    {
        [Fact]
        public void Game_ShouldInitialize_WithDefaultValues()
        {
            var game = new Game();

            game.Id.Should().Be(0);
            game.Title.Should().BeNull();
            game.Description.Should().BeNull();
            game.Price.Should().Be(0);
            game.ReleaseDate.Should().Be(default(DateTime));
            game.Stock.Should().Be(0);
            game.CategoryId.Should().Be(0);
            game.PublisherId.Should().Be(0);
            game.GamePlatform.Should().Be(default(Platform));
        }

        [Fact]
        public void Game_ShouldSetProperties_Correctly()
        {
            var releaseDate = new DateTime(2024, 1, 15);
            var game = new Game
            {
                Id = 1,
                Title = "Test Game",
                Description = "Test Description",
                Price = 59.99m,
                ReleaseDate = releaseDate,
                Stock = 100,
                CategoryId = 1,
                PublisherId = 1,
                GamePlatform = Platform.PlayStation
            };

            game.Id.Should().Be(1);
            game.Title.Should().Be("Test Game");
            game.Description.Should().Be("Test Description");
            game.Price.Should().Be(59.99m);
            game.ReleaseDate.Should().Be(releaseDate);
            game.Stock.Should().Be(100);
            game.CategoryId.Should().Be(1);
            game.PublisherId.Should().Be(1);
            game.GamePlatform.Should().Be(Platform.PlayStation);
        }

        [Fact]
        public void Game_ShouldHaveNavigationProperties()
        {
            var category = new Category { Id = 1, Name = "Action" };
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var game = new Game
            {
                Category = category,
                Publisher = publisher
            };

            game.Category.Should().NotBeNull();
            game.Category.Should().Be(category);
            game.Publisher.Should().NotBeNull();
            game.Publisher.Should().Be(publisher);
        }

        [Theory]
        [InlineData(Platform.PlayStation)]
        [InlineData(Platform.Xbox)]
        [InlineData(Platform.NintendoSwitch)]
        public void Game_ShouldAccept_AllPlatformValues(Platform platform)
        {
            var game = new Game { GamePlatform = platform };

            game.GamePlatform.Should().Be(platform);
        }

        [Fact]
        public void Game_Price_ShouldSupportDecimalPrecision()
        {
            var game = new Game { Price = 59.99m };

            game.Price.Should().Be(59.99m);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(1000)]
        public void Game_Stock_ShouldAcceptNonNegativeValues(int stock)
        {
            var game = new Game { Stock = stock };

            game.Stock.Should().Be(stock);
        }
    }
}
