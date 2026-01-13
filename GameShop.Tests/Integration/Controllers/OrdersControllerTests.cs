using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace GameShop.Tests.Integration.Controllers
{
    [Collection("Integration Tests")]
    public class OrdersControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public OrdersControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Index_WithoutAuthentication_ReturnsOK()
        {
            // Orders Index jest dostępne bez autoryzacji, ale pokazuje puste dane dla niezalogowanych
            // Act
            var response = await _client.GetAsync("/Orders");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Details_WithoutAuthentication_ReturnsNotFound()
        {
            // Orders Details nie wymaga autoryzacji, ale zamówienie #1 nie istnieje w testowej bazie
            // Act
            var response = await _client.GetAsync("/Orders/Details/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Orders/Create");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Edit_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Orders/Edit/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Delete_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Orders/Delete/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Create_Post_WithoutAuthentication_ReturnsRedirect()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                { "gameIds[0]", "1" },
                { "quantities[0]", "1" }
            };
            var content = new FormUrlEncodedContent(formData);

            // Act
            var response = await _client.PostAsync("/Orders/Create", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task NonExistentOrder_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/Orders/Details/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
