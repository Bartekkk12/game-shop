using FluentAssertions;
using Xunit;

namespace GameShop.Tests.Models
{
    public class OrderItemTests
    {
        [Fact]
        public void OrderItem_ShouldInitialize_WithDefaultValues()
        {
            var orderItem = new OrderItem();

            orderItem.Id.Should().Be(0);
            orderItem.GameId.Should().Be(0);
            orderItem.OrderId.Should().Be(0);
            orderItem.Quantity.Should().Be(0);
            orderItem.Price.Should().Be(0);
        }

        [Fact]
        public void OrderItem_ShouldSetProperties_Correctly()
        {
            var orderItem = new OrderItem
            {
                Id = 1,
                GameId = 10,
                OrderId = 5,
                Quantity = 3,
                Price = 49.99m
            };

            orderItem.Id.Should().Be(1);
            orderItem.GameId.Should().Be(10);
            orderItem.OrderId.Should().Be(5);
            orderItem.Quantity.Should().Be(3);
            orderItem.Price.Should().Be(49.99m);
        }

        [Fact]
        public void OrderItem_ShouldHaveNavigationProperties()
        {
            var game = new Game { Id = 1, Title = "Test Game" };
            var order = new Order { Id = 1 };
            var orderItem = new OrderItem
            {
                Game = game,
                Order = order
            };

            orderItem.Game.Should().NotBeNull();
            orderItem.Game.Should().Be(game);
            orderItem.Order.Should().NotBeNull();
            orderItem.Order.Should().Be(order);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(100)]
        public void OrderItem_Quantity_ShouldAcceptPositiveValues(int quantity)
        {
            var orderItem = new OrderItem { Quantity = quantity };

            orderItem.Quantity.Should().Be(quantity);
        }

        [Fact]
        public void OrderItem_Price_ShouldSupportDecimalPrecision()
        {
            var orderItem = new OrderItem { Price = 59.99m };

            orderItem.Price.Should().Be(59.99m);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(19.99)]
        [InlineData(59.99)]
        [InlineData(99.99)]
        public void OrderItem_Price_ShouldAcceptVariousPrices(decimal price)
        {
            var orderItem = new OrderItem { Price = price };

            orderItem.Price.Should().Be(price);
        }

        [Fact]
        public void OrderItem_ShouldCalculateTotalPrice()
        {
            var orderItem = new OrderItem
            {
                Quantity = 3,
                Price = 49.99m
            };

            var totalPrice = orderItem.Quantity * orderItem.Price;

            totalPrice.Should().Be(149.97m);
        }
    }
}
