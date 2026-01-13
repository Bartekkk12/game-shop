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
            var user = new User();

            user.FirstName.Should().Be(string.Empty);
            user.LastName.Should().Be(string.Empty);
            user.RegisteredAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            user.Orders.Should().NotBeNull();
            user.Orders.Should().BeEmpty();
        }

        [Fact]
        public void User_ShouldSetProperties_Correctly()
        {
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
            var user = new User();

            user.Orders.Should().NotBeNull();
            user.Orders.Should().BeOfType<List<Order>>();
            user.Orders.Should().BeEmpty();
        }

        [Fact]
        public void User_ShouldAllowAddingOrders()
        {
            var user = new User { Id = "user123" };
            var order1 = new Order { Id = 1, UserId = "user123" };
            var order2 = new Order { Id = 2, UserId = "user123" };

            user.Orders.Add(order1);
            user.Orders.Add(order2);

            user.Orders.Should().HaveCount(2);
            user.Orders.Should().Contain(order1);
            user.Orders.Should().Contain(order2);
        }

        [Fact]
        public void User_ShouldInheritFromIdentityUser()
        {
            var user = new User();

            user.Should().BeAssignableTo<Microsoft.AspNetCore.Identity.IdentityUser>();
        }

        [Fact]
        public void User_FullName_ShouldCombineFirstAndLastName()
        {
            var user = new User
            {
                FirstName = "Jan",
                LastName = "Kowalski"
            };

            var fullName = $"{user.FirstName} {user.LastName}";

            fullName.Should().Be("Jan Kowalski");
        }

        [Theory]
        [InlineData("Anna", "Nowak")]
        [InlineData("Piotr", "Wiśniewski")]
        [InlineData("Maria", "Dąbrowska")]
        public void User_ShouldAcceptVariousNames(string firstName, string lastName)
        {
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName
            };

            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
        }
    }
}
