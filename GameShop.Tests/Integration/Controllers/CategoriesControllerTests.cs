using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit;

namespace GameShop.Tests.Integration.Controllers
{
    [Collection("Integration Tests")]
    public class CategoriesControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public CategoriesControllerTests(CustomWebApplicationFactory factory)
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
            var response = await _client.GetAsync("/Categories");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Index_ReturnsListOfCategories()
        {
            // Act
            var response = await _client.GetAsync("/Categories");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("Action");
            content.Should().Contain("RPG");
            content.Should().Contain("Strategy");
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/Categories/Details/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsCorrectCategory()
        {
            // Act
            var response = await _client.GetAsync("/Categories/Details/1");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("Action");
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/Categories/Details/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Details_WithNullId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/Categories/Details/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Categories/Create");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Edit_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Categories/Edit/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Delete_Get_WithoutAuthentication_ReturnsRedirect()
        {
            // Act
            var response = await _client.GetAsync("/Categories/Delete/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Index_ContainsExpectedElements()
        {
            // Act
            var response = await _client.GetAsync("/Categories");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("Kategorie"); // "Categories" w nagłówku
            content.Should().Contain("Nazwa"); // "Name"
        }
    }
}
