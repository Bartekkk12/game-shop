using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using GameShop.Models;
using System.Linq;

namespace GameShop.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Usuń istniejącą konfigurację GameShopContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GameShopContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Dodaj GameShopContext używając InMemory database ze wspólną nazwą dla wszystkich testów
                services.AddDbContext<GameShopContext>(options =>
                {
                    options.UseInMemoryDatabase("SharedTestDb");
                });

                // Zbuduj ServiceProvider
                var sp = services.BuildServiceProvider();

                // Utwórz scope dla inicjalizacji bazy danych
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<GameShopContext>();
                    var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                    var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

                    try
                    {
                        // Inicjalizuj dane testowe tylko jeśli jeszcze nie istnieją
                        InitializeDbForTests(db, userManager, roleManager).Wait();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd podczas inicjalizacji bazy danych: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                }
            });

            // Ustaw środowisko na testowe
            builder.UseEnvironment("Testing");
        }

        private async Task InitializeDbForTests(GameShopContext db, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // NIE czyść bazy - pozwól danym testowym przetrwać między testami
            // Sprawdź czy dane już istnieją przed dodaniem

            // Dodaj role
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Dodaj użytkowników testowych
            var adminUser = await userManager.FindByEmailAsync("admin@test.com");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                    FirstName = "Admin",
                    LastName = "Test",
                    RegisteredAt = DateTime.Now,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var normalUser = await userManager.FindByEmailAsync("user@test.com");
            if (normalUser == null)
            {
                normalUser = new User
                {
                    UserName = "user@test.com",
                    Email = "user@test.com",
                    FirstName = "Normal",
                    LastName = "User",
                    RegisteredAt = DateTime.Now,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(normalUser, "User123!");
                await userManager.AddToRoleAsync(normalUser, "User");
            }

            // Dodaj kategorie testowe
            if (!db.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Action" },
                    new Category { Name = "RPG" },
                    new Category { Name = "Strategy" }
                };
                db.Categories.AddRange(categories);
                await db.SaveChangesAsync();
            }

            // Dodaj wydawców testowych
            if (!db.Publishers.Any())
            {
                var publishers = new[]
                {
                    new Publisher { Name = "Test Publisher 1" },
                    new Publisher { Name = "Test Publisher 2" }
                };
                db.Publishers.AddRange(publishers);
                await db.SaveChangesAsync();
            }

            // Dodaj gry testowe
            if (!db.Games.Any())
            {
                var games = new[]
                {
                    new Game
                    {
                        Title = "Test Game 1",
                        Description = "Description 1",
                        Price = 59.99m,
                        ReleaseDate = DateTime.Now,
                        Stock = 100,
                        CategoryId = db.Categories.First(c => c.Name == "Action").Id,
                        PublisherId = db.Publishers.First(p => p.Name == "Test Publisher 1").Id,
                        GamePlatform = GameShop.Models.Platform.PlayStation
                    },
                    new Game
                    {
                        Title = "Test Game 2",
                        Description = "Description 2",
                        Price = 49.99m,
                        ReleaseDate = DateTime.Now,
                        Stock = 50,
                        CategoryId = db.Categories.First(c => c.Name == "RPG").Id,
                        PublisherId = db.Publishers.First(p => p.Name == "Test Publisher 2").Id,
                        GamePlatform = GameShop.Models.Platform.Xbox
                    }
                };
                db.Games.AddRange(games);
                await db.SaveChangesAsync();
            }
        }
    }
}
