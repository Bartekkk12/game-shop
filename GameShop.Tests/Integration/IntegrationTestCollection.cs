using Xunit;

namespace GameShop.Tests.Integration
{
    /// <summary>
    /// Definicja kolekcji testów integracyjnych.
    /// Wszystkie testy w tej kolekcji współdzielą tę samą instancję CustomWebApplicationFactory.
    /// </summary>
    [CollectionDefinition("Integration Tests")]
    public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
        // Ta klasa nie ma implementacji - służy tylko jako marker dla xUnit
    }
}
