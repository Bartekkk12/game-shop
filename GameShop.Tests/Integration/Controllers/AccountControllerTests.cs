using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace GameShop.Tests.Integration.Controllers
{
    [Collection("Integration Tests")]
    public class AccountControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public AccountControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Register_Get_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/Account/Register");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Register_Get_ReturnsRegisterForm()
        {
            var response = await _client.GetAsync("/Account/Register");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Register");
            content.Should().Contain("Email");
            content.Should().Contain("Password");
            content.Should().Contain("FirstName");
            content.Should().Contain("LastName");
        }

        [Fact]
        public async Task Login_Get_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/Account/Login");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_Get_ReturnsLoginForm()
        {
            var response = await _client.GetAsync("/Account/Login");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Login");
            content.Should().Contain("Email");
            content.Should().Contain("Password");
        }

        [Fact]
        public async Task Login_Get_WithReturnUrl_IncludesReturnUrl()
        {
            var response = await _client.GetAsync("/Account/Login?returnUrl=%2FGames");
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("/Games");
        }

        [Fact]
        public async Task Login_Post_WithInvalidCredentials_ReturnsError()
        {
            var loginPage = await _client.GetAsync("/Account/Login");
            var loginContent = await loginPage.Content.ReadAsStringAsync();
            var token = ExtractAntiForgeryToken(loginContent);

            var formData = new Dictionary<string, string>
            {
                { "Email", "invalid@test.com" },
                { "Password", "WrongPassword" },
                { "RememberMe", "false" },
                { "__RequestVerificationToken", token }
            };
            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/Account/Login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().Contain("Login");
        }

        [Fact]
        public async Task Register_Post_WithoutAntiForgeryToken_ReturnsBadRequest()
        {
            var formData = new Dictionary<string, string>
            {
                { "Email", "newuser@test.com" },
                { "Password", "NewUser123!" },
                { "ConfirmPassword", "NewUser123!" },
                { "FirstName", "New" },
                { "LastName", "User" }
            };
            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/Account/Register", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AccessDenied_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/Account/AccessDenied");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AccessDenied_DisplaysAccessDeniedMessage()
        {
            var response = await _client.GetAsync("/Account/AccessDenied");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("zabroniony");
        }

        private string ExtractAntiForgeryToken(string htmlContent)
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                htmlContent,
                @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
