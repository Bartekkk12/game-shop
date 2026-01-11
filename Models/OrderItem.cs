using System.ComponentModel.DataAnnotations.Schema;

public class OrderItem
{
    public int Id { get; set; }

    // FK
    public int GameId { get; set; }
    public int OrderId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    // Navigation
    public Game Game { get; set; }
    public Order Order { get; set; }
}
