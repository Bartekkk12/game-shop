using FluentAssertions;
using Xunit;

namespace GameShop.Tests.Models
{
    public class CategoryTests
    {
        [Fact]
        public void Category_ShouldInitialize_WithDefaultValues()
        {
            var category = new Category();

            category.Id.Should().Be(0);
            category.Name.Should().BeNull();
            category.Games.Should().NotBeNull();
            category.Games.Should().BeEmpty();
        }

        [Fact]
        public void Category_ShouldSetProperties_Correctly()
        {
            var category = new Category
            {
                Id = 1,
                Name = "Action"
            };

            category.Id.Should().Be(1);
            category.Name.Should().Be("Action");
        }

        [Fact]
        public void Category_Games_ShouldBeInitializedAsEmptyList()
        {
            var category = new Category();

            category.Games.Should().NotBeNull();
            category.Games.Should().BeOfType<List<Game>>();
            category.Games.Should().BeEmpty();
        }

        [Fact]
        public void Category_ShouldAllowAddingGames()
        {
            var category = new Category { Id = 1, Name = "Action" };
            var game1 = new Game { Id = 1, Title = "Game 1", CategoryId = 1 };
            var game2 = new Game { Id = 2, Title = "Game 2", CategoryId = 1 };

            category.Games.Add(game1);
            category.Games.Add(game2);

            category.Games.Should().HaveCount(2);
            category.Games.Should().Contain(game1);
            category.Games.Should().Contain(game2);
        }

        [Fact]
        public void Category_Name_ShouldAcceptValidStrings()
        {
            var category = new Category { Name = "Adventure & RPG" };

            category.Name.Should().Be("Adventure & RPG");
        }
    }
}
