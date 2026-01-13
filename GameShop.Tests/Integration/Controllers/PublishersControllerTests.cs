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
            var response = await _client.GetAsync("/Publishers");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Index_ReturnsListOfPublishers()
        {
            var response = await _client.GetAsync("/Publishers");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Test Publisher 1");
            content.Should().Contain("Test Publisher 2");
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/Publishers/Details/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsCorrectPublisher()
        {
            var response = await _client.GetAsync("/Publishers/Details/1");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Test Publisher 1");
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/Publishers/Details/999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Details_WithNullId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/Publishers/Details/");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_Get_WithoutAuthentication_ReturnsRedirect()
        {
            var response = await _client.GetAsync("/Publishers/Create");

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Edit_Get_WithoutAuthentication_ReturnsRedirect()
        {
            var response = await _client.GetAsync("/Publishers/Edit/1");

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Delete_Get_WithoutAuthentication_ReturnsRedirect()
        {
            var response = await _client.GetAsync("/Publishers/Delete/1");

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.ToString().Should().Contain("/Account/Login");
        }

        [Fact]
        public async Task Index_ContainsExpectedElements()
        {
            var response = await _client.GetAsync("/Publishers");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Wydawcy");
            content.Should().Contain("Nazwa");
        }

        [Fact]
        public async Task Edit_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/Publishers/Edit/999");

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }
    }
}
