using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GameShop.Models;

namespace GameShop.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<GameShopContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // Stosowanie migracji przed seedowaniem, aby być pewnym, że baza ma odpowiedni schemat
            await context.Database.MigrateAsync();

            // --- Utwórz role w systemie ---
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    Console.WriteLine($"Rola {roleName} została utworzona.");
                }
            }

            // --- Dodaj domyślnego administratora ---
            var adminEmail = "admin@gameshop.com";
            var adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    RegisteredAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    Console.WriteLine($"Domyślny admin został utworzony: {adminEmail} / {adminPassword}");
                }
            }

            // --- Dodaj kategorie gier ---
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Action" },
                    new Category { Name = "RPG" },
                    new Category { Name = "Strategy" }
                );
                Console.WriteLine("Kategorie zostały dodane.");
            }

            // --- Dodaj wydawców ---
            if (!context.Publishers.Any())
            {
                context.Publishers.AddRange(
                    new Publisher { Name = "CD Projekt Red" },
                    new Publisher { Name = "Bandai Namco Entertainment" },
                    new Publisher { Name = "Ubisoft" }
                );
                Console.WriteLine("Wydawcy zostali dodani.");
            }

            // --- Dodaj gry ---
            if (!context.Games.Any())
            {
                context.Games.AddRange(
                    new Game
                    {
                        Title = "Cyberpunk 2077",
                        Category = context.Categories.FirstOrDefault(c => c.Name == "Action"),
                        Publisher = context.Publishers.FirstOrDefault(p => p.Name == "CD Projekt Red"),
                        Price = 199.99m,
                        Stock = 50,
                        GamePlatform = Platform.Xbox
                    },
                    new Game
                    {
                        Title = "Elden Ring",
                        Category = context.Categories.FirstOrDefault(c => c.Name == "RPG"),
                        Publisher = context.Publishers.FirstOrDefault(p => p.Name == "Bandai Namco Entertainment"),
                        Price = 249.99m,
                        Stock = 120,
                        GamePlatform = Platform.PlayStation
                    },
                    new Game
                    {
                        Title = "Assassin's Creed Valhalla",
                        Category = context.Categories.FirstOrDefault(c => c.Name == "Action"),
                        Publisher = context.Publishers.FirstOrDefault(p => p.Name == "Ubisoft"),
                        Price = 149.99m,
                        Stock = 80,
                        GamePlatform = Platform.NintendoSwitch
                    }
                );
                Console.WriteLine("Gry zostały dodane.");
            }

            // Zapisz wszystkie zmiany wprowadzone w bazie danych
            await context.SaveChangesAsync();
        }
    }
}