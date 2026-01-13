using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace GameShop.Tests.Integration.Controllers
{
    [Collection("Integration Tests")]
    public class GamesControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public GamesControllerTests(CustomWebApplicationFactory factory)
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
            var response = await _client.GetAsync("/Games");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Index_ReturnsListOfGames()
        {
            // Act
            var response = await _client.GetAsync("/Games");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("Test Game 1");
            content.Should().Contain("Test Game 2");
        }

        [Fact]
        public async Task Index_DisplaysGamePrices()
        {
            // Act
            var response = await _client.GetAsync("/Games");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("59.99");
            content.Should().Contain("49.99");
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/Games/Details/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsCorrectGame()
        {
            // Act
            var response = await _client.GetAsync("/Games/Details/1");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("Test Game 1");
            content.Should().Contain("Description 1");
            content.Should().Contain("59.99");
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/Games/Details/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Details_WithNullId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/Games/Details/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Details_DisplaysCategoryAndPublisher()
        {
            // Act
            var response = await _client.GetAsync("/Games/Details/1");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("Action");
            content.Should().Contain("Test Publisher 1");
        }

        [Fact]
        public async Task Create_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Games/Create");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Edit_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Games/Edit/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Delete_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Games/Delete/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Index_ContainsExpectedElements()
        {
            // Act
            var response = await _client.GetAsync("/Games");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("Gry"); // "Games" w nagłówku
            content.Should().Contain("Tytu"); // część słowa "Tytuł" (UTF-8 issues)
            content.Should().Contain("Cena"); // "Price"
        }

        [Fact]
        public async Task Index_DisplaysStock()
        {
            // Act
            var response = await _client.GetAsync("/Games");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("100");
            content.Should().Contain("50");
        }
    }
}
