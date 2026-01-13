using FluentAssertions;
using GameShop.Models;
using Xunit;

namespace GameShop.Tests.Models
{
    public class OrderTests
    {
        [Fact]
        public void Order_ShouldInitialize_WithDefaultValues()
        {
            // Arrange & Act
            var order = new Order();

            // Assert
            order.Id.Should().Be(0);
            order.UserId.Should().BeNull();
            order.OrderDate.Should().Be(default(DateTime));
            order.status.Should().Be(Order.Status.New);
            order.OrderItems.Should().NotBeNull();
            order.OrderItems.Should().BeEmpty();
        }

        [Fact]
        public void Order_ShouldSetProperties_Correctly()
        {
            // Arrange
            var orderDate = new DateTime(2024, 1, 15);
            var order = new Order
            {
                Id = 1,
                UserId = "user123",
                OrderDate = orderDate,
                status = Order.Status.PaymentReceived
            };

            // Assert
            order.Id.Should().Be(1);
            order.UserId.Should().Be("user123");
            order.OrderDate.Should().Be(orderDate);
            order.status.Should().Be(Order.Status.PaymentReceived);
        }

        [Fact]
        public void Order_OrderItems_ShouldBeInitializedAsEmptyList()
        {
            // Arrange & Act
            var order = new Order();

            // Assert
            order.OrderItems.Should().NotBeNull();
            order.OrderItems.Should().BeOfType<List<OrderItem>>();
            order.OrderItems.Should().BeEmpty();
        }

        [Fact]
        public void Order_ShouldAllowAddingOrderItems()
        {
            // Arrange
            var order = new Order { Id = 1 };
            var orderItem1 = new OrderItem { Id = 1, OrderId = 1, GameId = 1, Quantity = 1, Price = 59.99m };
            var orderItem2 = new OrderItem { Id = 2, OrderId = 1, GameId = 2, Quantity = 2, Price = 39.99m };

            // Act
            order.OrderItems.Add(orderItem1);
            order.OrderItems.Add(orderItem2);

            // Assert
            order.OrderItems.Should().HaveCount(2);
            order.OrderItems.Should().Contain(orderItem1);
            order.OrderItems.Should().Contain(orderItem2);
        }

        [Theory]
        [InlineData(Order.Status.New)]
        [InlineData(Order.Status.PaymentReceived)]
        [InlineData(Order.Status.PaymentSuceeded)]
        [InlineData(Order.Status.PaymentRejected)]
        [InlineData(Order.Status.InProgress)]
        [InlineData(Order.Status.Sent)]
        public void Order_Status_ShouldAcceptAllEnumValues(Order.Status status)
        {
            // Arrange
            var order = new Order { status = status };

            // Assert
            order.status.Should().Be(status);
        }

        [Fact]
        public void Order_ShouldHaveUserNavigationProperty()
        {
            // Arrange
            var user = new User
            {
                Id = "user123",
                FirstName = "John",
                LastName = "Doe"
            };
            var order = new Order { User = user };

            // Assert
            order.User.Should().NotBeNull();
            order.User.Should().Be(user);
        }

        [Fact]
        public void Order_Status_DefaultValue_ShouldBeNew()
        {
            // Arrange & Act
            var order = new Order();

            // Assert
            order.status.Should().Be(Order.Status.New);
        }
    }
}
