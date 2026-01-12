using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderItem
{
    public int Id { get; set; }

    // FK
    [Required(ErrorMessage = "Gra jest wymagana")]
    public int GameId { get; set; }

    [Required]
    public int OrderId { get; set; }

    [Required]
    [DefaultValue(1)]
    [Range(1, int.MaxValue, ErrorMessage = "Ilość sztuk gier w zamówieniu nie może być mniejsza niż 1")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Cena jest wymagana")]
    [Range(0.00, double.MaxValue, ErrorMessage = "Cena nie może być mniejsza niż zero")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    // Navigation
    public Game? Game { get; set; }
    public Order? Order { get; set; }
}