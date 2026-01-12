using Microsoft.EntityFrameworkCore;
using GameShop.Models;

public class GameShopContext : DbContext
{
    public GameShopContext(DbContextOptions<GameShopContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
}