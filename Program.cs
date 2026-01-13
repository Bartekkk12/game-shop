using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GameShop.Data; // Importujemy przestrzeń nazw dla SeedData
using GameShop.Models;

var builder = WebApplication.CreateBuilder(args);

// **Add services to the container**
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<GameShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));

// **Add Identity**
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<GameShopContext>()
.AddDefaultTokenProviders();

// **Configure cookie settings**
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// **Add Data Protection**
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/app/DataProtection-Keys"))
    .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

var app = builder.Build();

// **Configure the HTTP request pipeline**
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// **Inicjalizuj bazę danych i dokonaj seedowania tylko gdy nie jesteśmy w środowisku Test**
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        try
        {
            // Pobieramy kontekst bazy danych i stosujemy migracje
            var db = services.GetRequiredService<GameShopContext>();
            db.Database.Migrate();

            Console.WriteLine("Zastosowano wszystkie migracje dla bazy danych.");

            // Wywołanie metody SeedData.Initialize do wypełnienia bazy danych
            await SeedData.Initialize(services);

            Console.WriteLine("Dane zostały zainicjalizowane (role, admin, gry, wydawcy itp.)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas inicjalizacji bazy danych: {ex.Message}");
        }
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// **Make the implicit Program class public so test projects can access it**
public partial class Program { }