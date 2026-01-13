using FluentAssertions;
using Xunit;

namespace GameShop.Tests.Models
{
    public class PublisherTests
    {
        [Fact]
        public void Publisher_ShouldInitialize_WithDefaultValues()
        {
            // Arrange & Act
            var publisher = new Publisher();

            // Assert
            publisher.Id.Should().Be(0);
            publisher.Name.Should().BeNull();
            publisher.Games.Should().NotBeNull();
            publisher.Games.Should().BeEmpty();
        }

        [Fact]
        public void Publisher_ShouldSetProperties_Correctly()
        {
            // Arrange
            var publisher = new Publisher
            {
                Id = 1,
                Name = "Electronic Arts"
            };

            // Assert
            publisher.Id.Should().Be(1);
            publisher.Name.Should().Be("Electronic Arts");
        }

        [Fact]
        public void Publisher_Games_ShouldBeInitializedAsEmptyList()
        {
            // Arrange & Act
            var publisher = new Publisher();

            // Assert
            publisher.Games.Should().NotBeNull();
            publisher.Games.Should().BeOfType<List<Game>>();
            publisher.Games.Should().BeEmpty();
        }

        [Fact]
        public void Publisher_ShouldAllowAddingGames()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var game1 = new Game { Id = 1, Title = "Game 1", PublisherId = 1 };
            var game2 = new Game { Id = 2, Title = "Game 2", PublisherId = 1 };

            // Act
            publisher.Games.Add(game1);
            publisher.Games.Add(game2);

            // Assert
            publisher.Games.Should().HaveCount(2);
            publisher.Games.Should().Contain(game1);
            publisher.Games.Should().Contain(game2);
        }

        [Fact]
        public void Publisher_Name_ShouldAcceptValidStrings()
        {
            // Arrange
            var publisher = new Publisher { Name = "CD Projekt Red" };

            // Assert
            publisher.Name.Should().Be("CD Projekt Red");
        }
    }
}
