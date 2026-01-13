using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace GameShop.Tests.Integration.Controllers
{
    [Collection("Integration Tests")]
    public class HomeControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public HomeControllerTests(CustomWebApplicationFactory factory)
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
            var response = await _client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Index_ReturnsHtmlContent()
        {
            // Act
            var response = await _client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.Content.Headers.ContentType?.MediaType.Should().Be("text/html");
            content.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Privacy_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/Home/Privacy");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Privacy_ReturnsHtmlContent()
        {
            // Act
            var response = await _client.GetAsync("/Home/Privacy");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.Content.Headers.ContentType?.MediaType.Should().Be("text/html");
            content.Should().Contain("Privacy");
        }

        [Fact]
        public async Task NonExistentRoute_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/Home/NonExistentAction");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
