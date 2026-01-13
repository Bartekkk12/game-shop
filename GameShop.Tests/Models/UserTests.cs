using FluentAssertions;
using GameShop.Models;
using Xunit;

namespace GameShop.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void User_ShouldInitialize_WithDefaultValues()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.FirstName.Should().Be(string.Empty);
            user.LastName.Should().Be(string.Empty);
            user.RegisteredAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            user.Orders.Should().NotBeNull();
            user.Orders.Should().BeEmpty();
        }

        [Fact]
        public void User_ShouldSetProperties_Correctly()
        {
            // Arrange
            var registeredAt = new DateTime(2024, 1, 1);
            var user = new User
            {
                Id = "user123",
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@example.com",
                UserName = "jankowalski",
                RegisteredAt = registeredAt
            };

            // Assert
            user.Id.Should().Be("user123");
            user.FirstName.Should().Be("Jan");
            user.LastName.Should().Be("Kowalski");
            user.Email.Should().Be("jan.kowalski@example.com");
            user.UserName.Should().Be("jankowalski");
            user.RegisteredAt.Should().Be(registeredAt);
        }

        [Fact]
        public void User_Orders_ShouldBeInitializedAsEmptyList()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.Orders.Should().NotBeNull();
            user.Orders.Should().BeOfType<List<Order>>();
            user.Orders.Should().BeEmpty();
        }

        [Fact]
        public void User_ShouldAllowAddingOrders()
        {
            // Arrange
            var user = new User { Id = "user123" };
            var order1 = new Order { Id = 1, UserId = "user123" };
            var order2 = new Order { Id = 2, UserId = "user123" };

            // Act
            user.Orders.Add(order1);
            user.Orders.Add(order2);

            // Assert
            user.Orders.Should().HaveCount(2);
            user.Orders.Should().Contain(order1);
            user.Orders.Should().Contain(order2);
        }

        [Fact]
        public void User_ShouldInheritFromIdentityUser()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.Should().BeAssignableTo<Microsoft.AspNetCore.Identity.IdentityUser>();
        }

        [Fact]
        public void User_FullName_ShouldCombineFirstAndLastName()
        {
            // Arrange
            var user = new User
            {
                FirstName = "Jan",
                LastName = "Kowalski"
            };

            // Act
            var fullName = $"{user.FirstName} {user.LastName}";

            // Assert
            fullName.Should().Be("Jan Kowalski");
        }

        [Theory]
        [InlineData("Anna", "Nowak")]
        [InlineData("Piotr", "Wiśniewski")]
        [InlineData("Maria", "Dąbrowska")]
        public void User_ShouldAcceptVariousNames(string firstName, string lastName)
        {
            // Arrange
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName
            };

            // Assert
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
        }
    }
}
