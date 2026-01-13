using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace GameShop.Tests.Integration.Controllers
{
    [Collection("Integration Tests")]
    public class PublishersControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public PublishersControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Index_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/Publishers");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Index_ReturnsListOfPublishers()
        {
            // Act
            var response = await _client.GetAsync("/Publishers");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("Test Publisher 1");
            content.Should().Contain("Test Publisher 2");
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/Publishers/Details/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsCorrectPublisher()
        {
            // Act
            var response = await _client.GetAsync("/Publishers/Details/1");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("Test Publisher 1");
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/Publishers/Details/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Details_WithNullId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/Publishers/Details/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Publishers/Create");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Edit_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Publishers/Edit/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Delete_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Publishers/Delete/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Index_ContainsExpectedElements()
        {
            // Act
            var response = await _client.GetAsync("/Publishers");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("Wydawcy"); // "Publishers" w nagłówku
            content.Should().Contain("Nazwa"); // "Name"
        }

        [Fact]
        public async Task Edit_WithInvalidId_ReturnsNotFound()
        {
            // Najpierw zaloguj jako admin (w przyszłości)
            // Na razie testujemy tylko przekierowanie
            
            // Act
            var response = await _client.GetAsync("/Publishers/Edit/999");

            // Assert - będzie redirect bo nie jesteśmy zalogowani
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }
    }
}
