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
            var response = await _client.GetAsync("/Games");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Index_ReturnsListOfGames()
        {
            var response = await _client.GetAsync("/Games");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Test Game 1");
            content.Should().Contain("Test Game 2");
        }

        [Fact]
        public async Task Index_DisplaysGamePrices()
        {
            var response = await _client.GetAsync("/Games");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("59.99");
            content.Should().Contain("49.99");
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/Games/Details/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsCorrectGame()
        {
            var response = await _client.GetAsync("/Games/Details/1");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Test Game 1");
            content.Should().Contain("Description 1");
            content.Should().Contain("59.99");
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/Games/Details/999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Details_WithNullId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/Games/Details/");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Details_DisplaysCategoryAndPublisher()
        {
            var response = await _client.GetAsync("/Games/Details/1");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Action");
            content.Should().Contain("Test Publisher 1");
        }

        [Fact]
        public async Task Create_Get_WithoutAuthentication_ReturnsRedirect()
        {
            var response = await _client.GetAsync("/Games/Create");

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Edit_Get_WithoutAuthentication_ReturnsRedirect()
        {
            var response = await _client.GetAsync("/Games/Edit/1");

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Delete_Get_WithoutAuthentication_ReturnsRedirect()
        {
            var response = await _client.GetAsync("/Games/Delete/1");

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Index_ContainsExpectedElements()
        {
            var response = await _client.GetAsync("/Games");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Gry");
            content.Should().Contain("Tytu");
            content.Should().Contain("Cena");
        }

        [Fact]
        public async Task Index_DisplaysStock()
        {
            var response = await _client.GetAsync("/Games");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("100");
            content.Should().Contain("50");
        }
    }
}
